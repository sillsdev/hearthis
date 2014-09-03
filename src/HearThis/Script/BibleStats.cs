// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using Palaso.IO;

namespace HearThis.Script
{
	public class BibleStatsBase
	{
		public const int kCanonicalBookCount = 66;

		private static readonly List<string> s_bookNames;
		private static readonly List<string> s_threeLetterAbreviations;

		static BibleStatsBase()
		{
			s_threeLetterAbreviations = new List<string>(new[]
			{
				"Gen",
				"Exo",
				"Lev",
				"Num",
				"Deu",
				"Jos",
				"Jdg",
				"Rut",
				"1sa",
				"2sa",
				"1ki",
				"2ki",
				"1ch",
				"2ch",
				"Ezr",
				"Neh",
				"Est",
				"Job",
				"Psa",
				"Pro",
				"Ecc",
				"Sng",
				"Isa",
				"Jer",
				"Lam",
				"Ezk",
				"Dan",
				"Hos",
				"Jol",
				"Amo",
				"Oba",
				"Jon",
				"Mic",
				"Nam",
				"Hab",
				"Zep",
				"Hag",
				"Zec",
				"Mal",
				"Mat",
				"Mrk",
				"Luk",
				"Jhn",
				"Act",
				"Rom",
				"1co",
				"2co",
				"Gal",
				"Eph",
				"Php",
				"Col",
				"1th",
				"2th",
				"1ti",
				"2ti",
				"Tit",
				"Phm",
				"Heb",
				"Jas",
				"1pe",
				"2pe",
				"1jn",
				"2jn",
				"3jn",
				"Jud",
				"Rev"
			});

			s_bookNames = new List<string>(new[]
			{
				"Genesis",
				"Exodus",
				"Leviticus",
				"Numbers",
				"Deuteronomy",
				"Joshua",
				"Judges",
				"Ruth",
				"1 Samuel",
				"2 Samuel",
				"1 Kings",
				"2 Kings",
				"1 Chronicles",
				"2 Chronicles",
				"Ezra",
				"Nehemiah",
				"Esther",
				"Job",
				"Psalms",
				"Proverbs",
				"Ecclesiastes",
				"Song of Songs",
				"Isaiah",
				"Jeremiah",
				"Lamentations",
				"Ezekiel",
				"Daniel",
				"Hosea",
				"Joel",
				"Amos",
				"Obadiah",
				"Jonah",
				"Micah",
				"Nahum",
				"Habakkuk",
				"Zephaniah",
				"Haggai",
				"Zechariah",
				"Malachi",
				"Matthew",
				"Mark",
				"Luke",
				"John",
				"Acts",
				"Romans",
				"1 Corinthians",
				"2 Corinthians",
				"Galatians",
				"Ephesians",
				"Philippians",
				"Colossians",
				"1 Thessalonians",
				"2 Thessalonians",
				"1 Timothy",
				"2 Timothy",
				"Titus",
				"Philemon",
				"Hebrews",
				"James",
				"1 Peter",
				"2 Peter",
				"1 John",
				"2 John",
				"3 John",
				"Jude",
				"Revelation"
			});
		}

		public int BookCount
		{
			get { return s_bookNames.Count; }
		}

		/// <summary>Gets the 0-based book index</summary>
		public int GetBookNumber(string bookName)
		{
			return s_bookNames.FindIndex(0, b => b == bookName);
		}

		public string GetBookCode(int bookNumber0Based)
		{
			return s_threeLetterAbreviations[bookNumber0Based];
		}

		public string GetBookName(int bookNumber0Based)
		{
			return s_bookNames[bookNumber0Based];
		}
	}

	public class BibleStats : BibleStatsBase, IBibleStats
	{
		private readonly List<int> _chaptersPerBook;
		private readonly Dictionary<int, int[]> _versesPerChapterPerBook;

		public BibleStats()
		{
			_chaptersPerBook = new List<int>(kCanonicalBookCount);
			_versesPerChapterPerBook = new Dictionary<int, int[]>();
			int index = 0;
			foreach (string line in File.ReadAllLines(FileLocator.GetFileDistributedWithApplication("chapterCounts.txt")))
			{
				var parts = line.Trim().Split(new[] {'\t'});
				if (parts.Length > 3)
				{
					_chaptersPerBook.Add(int.Parse(parts[1]));
					var verseArray = new List<int>();
					for (int i = 3; i < parts.Length; i++)
						verseArray.Add(int.Parse(parts[i]));
					_versesPerChapterPerBook.Add(index, verseArray.ToArray());
				}
				++index;
			}
		}

		public int GetVersesInChapter(int bookNumber0Based, int chapterOneBased)
		{
			return _versesPerChapterPerBook[bookNumber0Based][chapterOneBased - 1];
		}

		public int GetChaptersInBook(int bookNumber0Based)
		{
			return _chaptersPerBook[bookNumber0Based];
		}
	}
}