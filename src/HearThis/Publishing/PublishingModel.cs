using System;
using System.IO;
using HearThis.Properties;
using L10NSharp;
using Palaso.Progress;
using Palaso.Reporting;
using Segmentio;

namespace HearThis.Publishing
{
	public class PublishingModel
	{
		private LineRecordingRepository _library;
		private string _projectName;
		public IAudioEncoder Encoder;

		public PublishingModel(LineRecordingRepository library, string projectName)
			: this(library, projectName, "")
		{}

		public PublishingModel(LineRecordingRepository library, string projectName, string ethnologueCode)
		{
			_library = library;
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
				return 	 Path.Combine(PublishRootPath, "HearThis-" + _projectName);
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
				_library.FilesInput = _library.FilesOutput = 0;
				_library.PublishAllBooks(PublishingMethod, _projectName, p, progress);
				UsageReporter.SendNavigationNotice("Publish");
				progress.WriteMessage("Done");
			}
			catch (Exception error)
			{
				progress.WriteError(error.Message);
				ErrorReport.NotifyUserOfProblem(error, LocalizationManager.GetString("PublishDialog.Error", "Sorry, the program made some mistake... " + error.Message));
				return false;
			}
			var properties = new Segmentio.Model.Properties()
				{
					{"FilesInput", _library.FilesInput},
					{"FilesOutput", _library.FilesOutput},
					{"Type", PublishingMethod.GetType().Name}
				};
			Analytics.Client.Track(Settings.Default.IdForAnalytics, "Published", properties);
			return true;
		}
	}
}
