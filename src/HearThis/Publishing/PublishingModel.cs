using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DesktopAnalytics;
using HearThis.Properties;
using L10NSharp;
using Palaso.Progress;
using Palaso.Reporting;

namespace HearThis.Publishing
{
	public class PublishingModel
	{

		public enum VerseIndexFormatType
		{
			None,
			CueSheet,
			AudacityLabelFile
		}

		private readonly IPublishingInfoProvider _infoProvider;
		private readonly string _projectName;
		private string _audioFormat;
		private bool _publishOnlyCurrentBook;
		public IPublishingMethod PublishingMethod { get; private set; }
		public VerseIndexFormatType VerseIndexFormat { get; set; }
		internal int FilesInput { get; set; }
		internal int FilesOutput { get; set; }
		public string EthnologueCode { get; private set; }

		public PublishingModel(string projectName, string ethnologueCode)
		{
			_projectName = projectName;
			EthnologueCode = ethnologueCode;
			_audioFormat = Settings.Default.PublishAudioFormat;
			_publishOnlyCurrentBook = Settings.Default.PublishCurrentBookOnly;
		}

		public PublishingModel(IPublishingInfoProvider infoProvider) :
			this(infoProvider.Name, infoProvider.EthnologueCode)
		{
			_infoProvider = infoProvider;
		}

		internal bool PublishOnlyCurrentBook
		{
			get { return _publishOnlyCurrentBook; }
			set { _publishOnlyCurrentBook = Settings.Default.PublishCurrentBookOnly = value; }
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
		public string PublishThisProjectPath
		{
			get { return Path.Combine(PublishRootPath, "HearThis-" + _projectName); }
		}


		public IPublishingInfoProvider PublishingInfoProvider
		{
			get { return _infoProvider; }
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
				var p = Path.Combine(PublishThisProjectPath, PublishingMethod.GetRootDirectoryName());
				if (Directory.Exists(p))
				{
					foreach (var file in Directory.GetFiles(p))
						File.Delete(file);
					}
				else
				{
					Directory.CreateDirectory(p);
				}
				FilesInput = FilesOutput = 0;
				if (PublishOnlyCurrentBook)
					ClipRepository.PublishAllChapters(this, _projectName, _infoProvider.CurrentBookName, p, progress);
				else
					ClipRepository.PublishAllBooks(this, _projectName, p, progress);
				UsageReporter.SendNavigationNotice("Publish");
				progress.WriteMessage("Done");
			}
			catch (Exception error)
			{
				progress.WriteError(error.Message);
				ErrorReport.NotifyUserOfProblem(error,
					LocalizationManager.GetString("PublishDialog.Error", "Sorry, the program made some mistake... " + error.Message));
				return false;
			}
			var properties = new Dictionary<string, string>()
				{
					{"FilesInput", FilesInput.ToString()},
					{"FilesOutput", FilesOutput.ToString()},
					{"Type", PublishingMethod.GetType().Name}
				};
			Analytics.Track("Published", properties);
			return true;
		}

		private void SetPublishingMethod()
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
	}
}
