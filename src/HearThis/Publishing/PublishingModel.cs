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
				_defaultPublishRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
										   "HearThis-" + projectName);
			PublishPath = _defaultPublishRoot;
//            }
		}

		public IAudioEncoder Encoder;
		private string _defaultPublishRoot;

		/// <summary>
		///
		/// </summary>
		/// <param name="ProgressCallback">will send 0..100</param>
		/// <param name="progress"></param>
		/// <param name="encoder"></param>
		/// <returns>true if successful</returns>
		public bool Publish(IProgress progress, IAudioEncoder encoder)
		{
			try
			{
				var p = PublishPath;
				if (p == _defaultPublishRoot)
					p = Path.Combine(PublishPath, encoder.FormatName);

				if(!Directory.Exists(PublishPath))
				{
					Directory.CreateDirectory(PublishPath);
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

				  _library.SaveAllBooks(encoder,  _projectName, p, progress);
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

		public string PublishPath { get; set; }
	}
}
