using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HearThis
{
	public class SoundLibrary
	{
		public string GetPath(string name, BookInfo selectedBook, ChapterInfo selectedChapter, int selectedVerse)
		{
			return Path.GetTempFileName();
		}
	}
}
