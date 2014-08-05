using System;
using System.Collections.Generic;
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

		public enum VerseIndexFormat
		{
			None,
			CueSheet,
			AudacityLabelFile
		}

		public VerseIndexFormat verseIndexFormat { get; set; }

		private string _projectName;
		public IAudioEncoder Encoder;
		internal int FilesInput { get; set; }
		internal int FilesOutput { get; set; }

		public PublishingModel(string projectName)
			: this(projectName, "")
		{ }

		public PublishingModel(string projectName, string ethnologueCode)
		{
			_projectName = projectName;
			EthnologueCode = ethnologueCode;

			PublishingMethod = new BunchOfFilesPublishingMethod(new FlacEncoder());
		}

		public string EthnologueCode { get; private set; }
		/// <summary>
		/// Root shared by all projects (all languages). This is all we let the user specify. Just wraps the Settings "PublishRootPath"
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
			get
			{
				return Path.Combine(PublishRootPath, "HearThis-" + _projectName);
			}
		}

		public IPublishingMethod PublishingMethod { get; set; }

		public bool Publish(IProgress progress)
		{
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
					{
						File.Delete(file);
					}
				}
				else
				{
					Directory.CreateDirectory(p);
				}
				FilesInput = FilesOutput = 0;
				ClipRecordingRepository.PublishAllBooks(this, _projectName, p, progress);
				UsageReporter.SendNavigationNotice("Publish");
				progress.WriteMessage("Done");
			}
			catch (Exception error)
			{
				progress.WriteError(error.Message);
				ErrorReport.NotifyUserOfProblem(error, LocalizationManager.GetString("PublishDialog.Error", "Sorry, the program made some mistake... " + error.Message));
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

	}
}
