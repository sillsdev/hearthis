using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Palaso.Extensions;
using Palaso.IO;
using Palaso.Progress.LogBox;
using Palaso.Reporting;

namespace HearThis
{
	public class SoundLibrary
	{
		public string GetPath(string projectName, string bookName, int chapterNumber, int verseNumber, string extension)
		{
			var chapter = GetChapterFolder(projectName, bookName, chapterNumber);
			return Path.Combine(chapter, verseNumber.ToString() + extension);
		}

		private string GetChapterFolder(string projectName, string bookName, int chapterNumber)
		{
			var book = GetBookFolder(projectName, bookName);
			var chapter = CreateDirectoryIfNeeded(book, chapterNumber.ToString());
			return chapter;
		}

		private string GetBookFolder(string projectName, string bookName)
		{
			var project = GetProjectFolder(projectName);
			var book = CreateDirectoryIfNeeded(project, bookName.Trim());
			return book;
		}

		private string GetProjectFolder(string projectName)
		{
			var sil = CreateDirectoryIfNeeded(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SIL");

			var hearThis = CreateDirectoryIfNeeded(sil, "HearThis");

			var project = CreateDirectoryIfNeeded(hearThis, projectName);
			return project;
		}

		private string CreateDirectoryIfNeeded(string parent, string child)
		{
			var path = Path.Combine(parent, child);
				if(!Directory.Exists(path))
					Directory.CreateDirectory(path);
			return path;
		}


//        public void mciConvertWavMP3(string fileName, bool waitFlag)
//        {
//            string outfile = "-b 32 --resample 22.05 -m m \"" + pworkingDir + fileName + "\" \"" + pworkingDir + fileName.Replace(".wav", ".mp3") + "\"";
//            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
//            psi.FileName = "\"" + pworkingDir + "lame.exe" + "\"";
//            psi.Arguments = outfile;
//            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
//            System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
//            if (waitFlag)
//            {
//                p.WaitForExit();
//            }
//        }

		public void SaveAllBooks(string projectName, string publishRoot, IProgress progress)
		{
			foreach (string dir in Directory.GetDirectories(GetProjectFolder(projectName)))
			{
				string bookName = Path.GetFileName(dir);
				//var filePath = Path.Combine(publishPath, bookName);
				SaveAllChaptersFlac(projectName, bookName, publishRoot, progress);
			}
		}

		public void SaveAllChaptersFlac(string projectName, string bookName, string publishRoot, IProgress progress)
		{
			var bookFolder = GetBookFolder(projectName, bookName);
			foreach (var dirPath in Directory.GetDirectories(bookFolder))
			{
				var chapterNumber = int.Parse(Path.GetFileName(dirPath));
				SaveSingleChapterFlac(projectName, bookName, chapterNumber, publishRoot, progress);
			}
		}

		private void SaveSingleChapterFlac(string projectName, string bookName, int chapterNumber, string rootPath, IProgress progress)
		{
			try
			{

				var verseFiles = Directory.GetFiles(GetChapterFolder(projectName, bookName, chapterNumber));
				if (verseFiles.Length == 0)
					return;


				var paths = new List<string>();
				foreach (var file in verseFiles)
				{
					paths.Add(file);
				}
				var fileList = Path.GetTempFileName();
				File.WriteAllLines(fileList, paths.ToArray());

				var dest = Path.Combine(rootPath, string.Format("{0}-{1}.flac", bookName, chapterNumber.ToString()));
				if (File.Exists(dest))
				{
					File.Delete(dest);
				}

				if (verseFiles.Length == 1)
				{
					progress.WriteMessage("Converting {0} {1}", bookName, chapterNumber.ToString());
					Process.Start(FileLocator.GetFileDistributedWithApplication(false, "shntool.exe"),
								  string.Format("conv  -O always -o flac -F '{0}'", fileList));
					string singleConvertedFile = verseFiles[0].Replace(".wav", ".flac");
					File.Move(singleConvertedFile, dest);
				}
				else
				{
					//NB: shntool will choke if you surround the output directory (-d) with single quotes. TODO: make 8.3?
					if (rootPath.Contains(" "))
					{
						ErrorReport.NotifyUserOfProblem(
							"Oops. The audio directory path has a space, and that's going to be a problem ({0})",
							rootPath);
					}
					string arguments = string.Format("join -d {0} -F {1} -o flac -O always", rootPath, fileList);
					progress.WriteMessage("Converting {0} {1}", bookName, chapterNumber.ToString());
					Process.Start(FileLocator.GetFileDistributedWithApplication(false, "shntool.exe"), arguments);
					File.Move(Path.Combine(rootPath, "joined.flac"), dest);
				}

			}
			catch (Exception error)
			{
				progress.WriteException(error);
			}
		}
	}
}
