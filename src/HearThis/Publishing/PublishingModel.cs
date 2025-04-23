// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
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
using static System.Environment;
using static System.String;

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
			get => _audioFormat;
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
				if (IsNullOrEmpty(Settings.Default.PublishRootPath) ||
				    !Directory.Exists(Settings.Default.PublishRootPath))
				{
					PublishRootPath = GetFolderPath(SpecialFolder.MyDocuments);
				}
				return Settings.Default.PublishRootPath;
			}
			set
			{
				Settings.Default.PublishRootPath = value;
				FileContentionHelper.SaveSettings();
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
				var path = Path.Combine(PublishThisProjectPath, PublishingMethod.RootDirectoryName);
				FilesInput = FilesOutput = 0;
				if (PublishOnlyCurrentBook)
				{
					PublishingMethod.DeleteExistingPublishedFiles(path, _infoProvider.CurrentBookName);
					ClipRepository.PublishAllChapters(this, _projectName, _infoProvider.CurrentBookName, path, progress);
				}
				else
					ClipRepository.PublishAllBooks(this, _projectName, path, progress);

				foreach (var message in PublishingMethod.GetFinalInformationalMessages(this))
					progress.WriteMessage(message);
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
			// Note that the case labels are derived from the Name property of the RadioButton controls, so the
			// case of these strings must match the case used in the control names.
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
				// This is OGG Vorbus, we are keeping it as "ogg" because originally, it was the only OGG option
				// and if we change the name now, previous user settings may break
				case "ogg":
					PublishingMethod = new BunchOfFilesPublishingMethod(new OggEncoder());
					break;
				case "opus":
					PublishingMethod = new BunchOfFilesPublishingMethod(new OpusEncoder());
					break;
				case "kulumi":
					PublishingMethod = new KulumiPublishingMethod();
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
