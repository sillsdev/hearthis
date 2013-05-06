using System.Collections.Generic;
using System.IO;
using Palaso.IO;

namespace HearThis.Script
{
	public class BibleStats
	{
		public List<string> BookNames;
		public List<int> ChaptersPerBook;
		public List<string> ThreeLetterAbreviations;
		public Dictionary<int, int[]> VersesPerChapterPerBook;

		public BibleStats()
		{
			LoadStatistics();
		}
		private void LoadStatistics()
		{
			BookNames = new List<string>();
			ChaptersPerBook = new List<int>();
			VersesPerChapterPerBook = new Dictionary<int, int[]>();
			ThreeLetterAbreviations  = new List<string>();
			int index = 0;
			foreach (
				string line in File.ReadAllLines(FileLocator.GetFileDistributedWithApplication("chapterCounts.txt")))
			{
				var parts = line.Trim().Split(new char[] {'\t'});
				if (parts.Length > 3)
				{
					BookNames.Add(parts[0].Trim());
					ChaptersPerBook.Add(int.Parse(parts[1]));
					ThreeLetterAbreviations.Add(parts[2]);
					var verseArray = new List<int>();
					for (int i = 3; i < parts.Length; i++)
					{
						verseArray.Add(int.Parse(parts[i]));
					}
					VersesPerChapterPerBook.Add(index, verseArray.ToArray());
				}
				++index;
			}
		}

		public int GetBookNumber(string bookName)
		{
			return BookNames.FindIndex(0, b => b == bookName);
		}

		public string GetBookName(int bookNumber)
		{
			return BookNames[bookNumber];
		}

		public int GetPossibleVersesInChapter(int book, int chapterOneBased)
		{
			return VersesPerChapterPerBook[book][chapterOneBased-1];
		}

		public int GetChaptersInBook(int bookNumber1Based)
		{
			return ChaptersPerBook[bookNumber1Based-1];
		}
	}
}