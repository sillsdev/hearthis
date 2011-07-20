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
		public string GetPath(string name, string bookName, int chapterNumber, int verseNumber, string extension)
		{
			var sil = CreateDictionaryIfNeeded(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"SIL");

			var hearThis = CreateDictionaryIfNeeded(sil,"HearThis");

			var project = CreateDictionaryIfNeeded(hearThis, name);
			var book = CreateDictionaryIfNeeded(project, bookName.Trim());
			var chapter = CreateDictionaryIfNeeded(book, chapterNumber.ToString());
			return Path.Combine(chapter, verseNumber.ToString() + extension);
		}

		private string CreateDictionaryIfNeeded(string parent, string child)
		{
			var path = Path.Combine(parent, child);
				if(!Directory.Exists(path))
					Directory.CreateDirectory(path);
			return path;
		}
	}
}
