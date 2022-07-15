// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DesktopAnalytics;
using HearThis.Properties;
using L10NSharp;
using SIL.Progress;
using SIL.Reporting;

namespace HearThis.Publishing
{
	public class PublishingModel
	{

		public enum VerseIndexFormatType
		{
			None,
			CueSheet,
			AudacityLabelFileVerseLevel,
			AudacityLabelFilePhraseLevel,
		}

		private readonly IPublishingInfoProvider _infoProvider;
		private readonly string _projectName;
		private string _audioFormat;
		private bool _publishOnlyCurrentBook;
		public IPublishingMethod PublishingMethod { get; private set; }
		public VerseIndexFormatType VerseIndexFormat { get; set; }
		internal int FilesInput { get; set; }
		internal int FilesOutput { get; set; }
		public string EthnologueCode { get; }

		public PublishingModel(string projectName, string ethnologueCode)
		{
			_projectName = projectName;
			EthnologueCode = ethnologueCode;
			_audioFormat = Settings.Default.PublishAudioFormat;
			_publishOnlyCurrentBook = Settings.Default.PublishCurrentBookOnly;
		}

		public PublishingModel(IPublishingInfoProvider infoProvider) : this(infoProvider.Name, infoProvider.EthnologueCode)
		{
			_infoProvider = infoProvider;
		}

		internal bool PublishOnlyCurrentBook
		{
			get => _publishOnlyCurrentBook;
			set => _publishOnlyCurrentBook = Settings.Default.PublishCurrentBookOnly = value;
		}

		public string AudioFormat
		{
			get { return _audioFormat; }
			set
			{
				if (PublishingMethod != null)
					throw new InvalidOperationException("The audio format cannot be changed after Publish method has been called.");
				Settings.Default.PublishAudioFormat = _audioFormat = value;
			}
		}
		/// <summary>
		/// Root shared by all projects (all languages). This is all we let the user specify. Just wraps the Settings "PublishRootPath"
		/// If specified path doesn't exist, silently falls back to default location in My Documents.
		/// </summary>
		public string PublishRootPath
		{
			get
			{
				if (string.IsNullOrEmpty(Settings.Default.PublishRootPath) || !Directory.Exists(Settings.Default.PublishRootPath))
				{
					PublishRootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				}
				return Settings.Default.PublishRootPath;
			}
			set
			{
				Settings.Default.PublishRootPath = value;
				Settings.Default.Save();
			}
		}

		/// <summary>
		/// We use a directory directly underneath the PublishRootPath, named for this project.
		/// The directory may or may not exist.
		/// </summary>
		public string PublishThisProjectPath => Path.Combine(PublishRootPath, "HearThis-" + _projectName);

		public IPublishingInfoProvider PublishingInfoProvider => _infoProvider;

		public bool IncludeBook(string bookName)
		{
			return _infoProvider.IncludeBook(bookName);
		}

		public bool Publish(IProgress progress)
		{
			SetPublishingMethod();

			try
			{
				if (!Directory.Exists(PublishThisProjectPath))
				{
					Directory.CreateDirectory(PublishThisProjectPath);
				}
				var p = Path.Combine(PublishThisProjectPath, PublishingMethod.RootDirectoryName);
				FilesInput = FilesOutput = 0;
				if (PublishOnlyCurrentBook)
				{
					PublishingMethod.DeleteExistingPublishedFiles(p, _infoProvider.CurrentBookName);
					ClipRepository.PublishAllChapters(this, _projectName, _infoProvider.CurrentBookName, p, progress);
				}
				else
					ClipRepository.PublishAllBooks(this, _projectName, p, progress);
				progress.WriteMessage(LocalizationManager.GetString("PublishDialog.Done", "Done"));

				if (AudioFormat == "scrAppBuilder" && VerseIndexFormat == VerseIndexFormatType.AudacityLabelFilePhraseLevel)
				{
					progress.WriteMessage(""); // blank line
					progress.WriteMessage(Format(LocalizationManager.GetString(
							"PublishDialog.ScriptureAppBuilderInstructionsAboutBlockBreakChars",
							"When building the app using Scripture App Builder, in order for " +
							"the text highlighting to work correctly make sure that the " +
							"recording block or phrase-ending characters specified on the 'Features - Audio' " +
							"page in SAB include the characters that {1} uses to break the text into " +
							"recording blocks in your project: {0}",
							"Param 0: list of characters; " +
							"Param 1: \"HearThis\" (product name)"),
						_infoProvider.BlockBreakCharacters, Program.kProduct));
				}
			}
			catch (Exception error)
			{
				progress.WriteError(error.Message);
				ErrorReport.NotifyUserOfProblem(error,
					LocalizationManager.GetString("PublishDialog.Error", "Sorry, the program made some mistake... ") + error.Message);
				return false;
			}
			var properties = new Dictionary<string, string>
				{
					{"FilesInput", FilesInput.ToString()},
					{"FilesOutput", FilesOutput.ToString()},
					{"Type", PublishingMethod.GetType().Name}
				};
			Analytics.Track("Published", properties);
			return true;
		}


		/// <summary>
		/// In production code, this should only be called by Publish method, but it's exposed here to
		/// make it accessible for tests.
		/// </summary>
		protected void SetPublishingMethod()
		{
			Debug.Assert(PublishingMethod == null);
			switch (AudioFormat)
			{
				case "audiBible":
					PublishingMethod = new AudiBiblePublishingMethod(new AudiBibleEncoder(), EthnologueCode);
					break;
				case "saber":
					PublishingMethod = new SaberPublishingMethod();
					break;
				case "megaVoice":
					PublishingMethod = new MegaVoicePublishingMethod();
					break;
				case "scrAppBuilder":
					PublishingMethod = new ScriptureAppBuilderPublishingMethod(EthnologueCode);
					break;
				case "mp3":
					PublishingMethod = new BunchOfFilesPublishingMethod(new LameEncoder());
					break;
				case "ogg":
					PublishingMethod = new BunchOfFilesPublishingMethod(new OggEncoder());
					break;
				default:
					PublishingMethod = new BunchOfFilesPublishingMethod(new FlacEncoder());
					break;
			}
		}

		public bool BooksToExportHaveProblemsNeedingAttention()
		{
			return _infoProvider != null &&
				_infoProvider.HasProblemNeedingAttention(PublishOnlyCurrentBook ? _infoProvider.CurrentBookName : null);
		}
	}
}
