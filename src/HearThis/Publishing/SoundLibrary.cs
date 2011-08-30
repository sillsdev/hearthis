using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Palaso.CommandLineProcessing;
using Palaso.IO;
using Palaso.Progress.LogBox;
using Palaso.Reporting;
using Palaso.Extensions;

namespace HearThis.Publishing
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


		public void SaveAllBooks(IAudioEncoder encoder, string projectName, string publishRoot, IProgress progress)
		{
			foreach (string dir in Directory.GetDirectories(GetProjectFolder(projectName)))
			{
				string bookName = Path.GetFileName(dir);
				//var filePath = Path.Combine(publishPath, bookName);
				SaveAllChapters(encoder, projectName, bookName, publishRoot, progress);
			}
		}

		public void SaveAllChapters(IAudioEncoder encoder, string projectName, string bookName, string publishRoot, IProgress progress)
		{
			var bookFolder = GetBookFolder(projectName, bookName);
			foreach (var dirPath in Directory.GetDirectories(bookFolder))
			{
				var chapterNumber = int.Parse(Path.GetFileName(dirPath));
				SaveSingleChapter(encoder, projectName, bookName, chapterNumber, publishRoot, progress);
			}
		}

		private void SaveSingleChapter(IAudioEncoder encoder, string projectName, string bookName, int chapterNumber, string rootPath, IProgress progress)
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
					var destPathWithoutExtension = Path.Combine(rootPath,
							string.Format("{0}-{1}", bookName, chapterNumber.ToString()));
					progress.WriteMessage("   Converting to {0}", encoder.FormatName);
					encoder.Encode(pathToJoinedWavFile, destPathWithoutExtension, progress);
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
	}
}
