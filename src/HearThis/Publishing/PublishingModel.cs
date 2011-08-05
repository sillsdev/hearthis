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
				PublishPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
										   "HearThis-" + projectName);
//            }
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="ProgressCallback">will send 0..100</param>
		/// <param name="progress"></param>
		/// <returns>true if successful</returns>
		public bool Publish(IProgress progress)
		{
			try
			{
				if (Directory.Exists(PublishPath))
				{
					foreach (var file in Directory.GetFiles(PublishPath))
					{
						File.Delete(file);
					}
				}
				else
				{
					Directory.CreateDirectory(PublishPath);
				}

				_library.SaveAllBooks(_projectName, PublishPath, progress);

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
