using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Palaso.Extensions;

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
			var chapter = CreateDictionaryIfNeeded(book, chapterNumber.ToString());
			return chapter;
		}

		private string GetBookFolder(string projectName, string bookName)
		{
			var sil = CreateDictionaryIfNeeded(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SIL");

			var hearThis = CreateDictionaryIfNeeded(sil, "HearThis");

			var project = CreateDictionaryIfNeeded(hearThis, projectName);
			var book = CreateDictionaryIfNeeded(project, bookName.Trim());
			return book;
		}

		private string CreateDictionaryIfNeeded(string parent, string child)
		{
			var path = Path.Combine(parent, child);
				if(!Directory.Exists(path))
					Directory.CreateDirectory(path);
			return path;
		}

		public void SaveFlac(string projectName, string bookName, string path)
		{
			var bookFOlder = GetBookFolder(projectName, bookName);
			foreach(var dirPath in Directory.GetDirectories(bookName))
			{
				foreach(var file in Directory.GetFiles(dirPath))
				{

				}
			}
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

	}
}
