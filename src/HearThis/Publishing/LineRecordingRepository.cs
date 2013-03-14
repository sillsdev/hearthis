using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Palaso.CommandLineProcessing;
using Palaso.IO;
using Palaso.Extensions;
using Palaso.Progress;

namespace HearThis.Publishing
{
	/// <summary>
	/// Each script line is recorded and stored as its own file.  This class manages that collection of files.
	/// </summary>
	public class LineRecordingRepository
	{
		private static string _sHearThisFolder;

		public string GetPathToLineRecording(string projectName, string bookName, int chapterNumber, int lineNumber)
		{
			var chapter = GetChapterFolder(projectName, bookName, chapterNumber);
			return Path.Combine(chapter, lineNumber.ToString() + ".wav");
		}

		public bool GetHaveScriptLineFile(string projectName, string bookName, int chapterNumber, int lineNumber)
		{
			var path = GetPathToLineRecording(projectName, bookName, chapterNumber,lineNumber);
			return File.Exists(path);
		}




		public string GetChapterFolder(string projectName, string bookName, int chapterNumber)
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
			if (_sHearThisFolder == null)
			{
				var sil =
					CreateDirectoryIfNeeded(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
											"SIL");
				_sHearThisFolder = CreateDirectoryIfNeeded(sil, "HearThis");
			}

			var project = CreateDirectoryIfNeeded(_sHearThisFolder, projectName);
			return project;
		}

		private string CreateDirectoryIfNeeded(string parent, string child)
		{
			var path = Path.Combine(parent, child);
				if(!Directory.Exists(path))
					Directory.CreateDirectory(path);
			return path;
		}


		public void PublishAllBooks(IPublishingMethod publishingMethod, string projectName, string publishRoot, IProgress progress)
		{
			Directory.Delete(publishRoot, true);
			foreach (string dir in Directory.GetDirectories(GetProjectFolder(projectName)))
			{
				if (progress.CancelRequested)
					return;
				string bookName = Path.GetFileName(dir);
				//var filePath = Path.Combine(publishPath, bookName);
				PublishAllChapters(publishingMethod, projectName, bookName, publishRoot, progress);
			}
		}

		public void PublishAllChapters(IPublishingMethod publishingMethod,string projectName, string bookName, string publishRoot, IProgress progress)
		{
			var bookFolder = GetBookFolder(projectName, bookName);
			foreach (var dirPath in Directory.GetDirectories(bookFolder))
			{
				if (progress.CancelRequested)
					return;
				var chapterNumber = int.Parse(Path.GetFileName(dirPath));
				PublishSingleChapter(publishingMethod, projectName, bookName, chapterNumber, publishRoot, progress);
			}
		}

		private void PublishSingleChapter(IPublishingMethod publishingMethod, string projectName, string bookName, int chapterNumber, string rootPath, IProgress progress)
		{
			try
			{
				var verseFiles = Directory.GetFiles(GetChapterFolder(projectName, bookName, chapterNumber));
				if (verseFiles.Length == 0)
					return;

				progress.WriteMessage("{0} {1}", bookName, chapterNumber.ToString());
				var paths = new List<string>();
				foreach (var file in verseFiles)
				{
					paths.Add(file);
				}
				var fileList = Path.GetTempFileName();
				File.WriteAllLines(fileList, paths.ToArray());

				string pathToJoinedWavFile = Path.GetTempPath().CombineForPath("joined.wav");
				using(TempFile.TrackExisting(pathToJoinedWavFile))
				{
					if (verseFiles.Length == 1)
					{
						File.Copy(verseFiles[0], pathToJoinedWavFile);
					}
					else
					{
						progress.WriteMessage("   Joining script lines");
												string arguments = string.Format("join -d \"{0}\" -F \"{1}\" -O always", Path.GetDirectoryName(pathToJoinedWavFile), fileList);
						  RunCommandLine(progress, FileLocator.GetFileDistributedWithApplication(false, "shntool.exe"), arguments);

					 }

					publishingMethod.PublishChapter(rootPath, bookName, chapterNumber, pathToJoinedWavFile, progress);
				}
			}
			catch (Exception error)
			{
				progress.WriteException(error);
			}
		}

		public static void RunCommandLine(IProgress progress, string exePath, string arguments)
		{
			progress.WriteVerbose(exePath + " " + arguments);
			ExecutionResult result = CommandLineRunner.Run(exePath, arguments, null, 60, progress);
		}

		public int GetCountOfRecordingsForChapter(string projectName, string bookName, int chapterNumber)
		{
			Debug.WriteLine("GetCOuntOfRecordings(" + chapterNumber + ")");
			var path = GetChapterFolder(projectName, bookName, chapterNumber);
			if (!Directory.Exists(path))
				return 0;
			return Directory.GetFileSystemEntries(path).Length;
		}


		public int GetCountOfRecordingsForBook(string projectName, string name)
		{
			var path = GetBookFolder(projectName, name);
			if (!Directory.Exists(path))
				return 0;
			int count = 0;
			foreach (var directory in Directory.GetDirectories(path))
			{
				count += Directory.GetFileSystemEntries(directory).Length;
			}
			return count;
		}
	}
}
