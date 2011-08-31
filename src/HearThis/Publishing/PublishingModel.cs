using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HearThis.Properties;
using Palaso.Progress.LogBox;
using Palaso.Reporting;

namespace HearThis.Publishing
{
	public class PublishingModel
	{
		private readonly SoundLibrary _library;
		private readonly string _projectName;

		public PublishingModel(SoundLibrary library, string projectName)
		{
			_library = library;
			_projectName = projectName;
			//PublishPath = Settings.Default.PublishPath;
//            if (string.IsNullOrEmpty(PublishPath) || !Directory.Exists(PublishPath))
//            {
				_defaultRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
										   "HearThis-" + projectName);
			RootPath = _defaultRootPath;
//            }

			PublishingMethod = new BunchOfFilesPublishingMethod(new FlacEncoder());
		}

		public IAudioEncoder Encoder;
		private string _defaultRootPath;

		public IPublishingMethod PublishingMethod { get; set; }

		/// <summary>
		///
		/// </summary>
		/// <param name="ProgressCallback">will send 0..100</param>
		/// <param name="progress"></param>
		/// <param name="encoder"></param>
		/// <returns>true if successful</returns>
		public bool Publish(IProgress progress)
		{
			try
			{
				var p = RootPath;
				if (p == _defaultRootPath)
					p = Path.Combine(RootPath,  PublishingMethod.GetRootDirectoryName());

				if(!Directory.Exists(RootPath))
				{
					Directory.CreateDirectory(RootPath);
				}


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

				_library.SaveAllBooks(PublishingMethod, _projectName, p, progress);
				UsageReporter.SendNavigationNotice("Publish");
				progress.WriteMessage("Done");
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem(error, "Sorry, the program made some mistake... " + error.Message);
				return false;
			}
			return true;
		}

		public string RootPath { get; set; }
	}
}
