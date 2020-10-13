// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using HearThis.Publishing;

namespace HearThis.Script
{
	public class BookInfo
	{
		public readonly int ChapterCount;

		public BookInfo(string projectName, int number, IScriptProvider scriptProvider)
		{
			BookNumber = number;
			ProjectName = projectName;
			Name = scriptProvider.VersificationInfo.GetBookName(number);
			ChapterCount = scriptProvider.VersificationInfo.GetChaptersInBook(number);
			ScriptProvider = scriptProvider;
		}

		// TODO: Implement this - probably as part of making this a Paratext plugin
		public string LocalizedName => Name;

		/// <summary>
		/// 0-based book number
		/// </summary>
		public int BookNumber { get; private set; }

		// That is, has some translated material (for the current character, if any)
		public bool HasVerses
		{
			get
			{
				for (int i = 0; i < ScriptProvider.VersificationInfo.GetChaptersInBook(BookNumber); i++)
				{
					if (ScriptProvider.GetTranslatedVerseCount(BookNumber, i + 1) > 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		public ScriptLine GetBlock(int chapter, int block)
		{
			return ScriptProvider.GetBlock(BookNumber, chapter, block);
		}

		public ScriptLine GetUnfilteredBlock(int chapter, int block)
		{
			return ScriptProvider.GetUnfilteredBlock(BookNumber, chapter, block);
		}

//        /// <summary>
//        /// bool HasVersesMethod(chapter)
//        /// </summary>
//        public Func<int, int> VerseCountMethod { get; set; }

		public string Name { get; }

		public bool HasIntroduction => ScriptProvider.GetUnfilteredScriptBlockCount(BookNumber, 0) > 0;

		internal string ProjectName { get; }

		/// <summary>
		/// [0] == intro, [1] == chapter 1, etc.
		/// </summary>
		internal IScriptProvider ScriptProvider { get; }

		/// <summary>
		/// Gets 0 if there is an intro before chapter 1; otherwise returns 1
		/// </summary>
		internal int FirstChapterNumber => HasIntroduction ? 0 : 1;

		/// <summary>
		/// Gets intro or chapter 1, whichever comes first
		/// </summary>
		public ChapterInfo GetFirstChapter()
		{
			return GetChapter(FirstChapterNumber);
		}

		public virtual ChapterInfo GetChapter(int chapterOneBased)
		{
			return ChapterInfo.Create(this, chapterOneBased);
		}

		public int CalculatePercentageRecorded()
		{
			int scriptBlockCount = ScriptProvider.GetScriptBlockCount(BookNumber);
			if (scriptBlockCount == 0)
				return 0; //should it be 0 or 100 or -1 or what?
			int countOfRecordingsForBook = ClipRepository.GetCountOfRecordingsForBook(ProjectName, Name, ScriptProvider);
			if (countOfRecordingsForBook == 0)
				return 0;
			return Math.Max(1, (int)(100.0 * countOfRecordingsForBook / scriptBlockCount));
		}

		/// <summary>
		/// Indicates whether there is any content for this book. (Note: In the case of a multi-voice.
		/// script, this returns false if there is nothing in the book for the current character).
		/// </summary>
		public bool HasTranslatedContent
		{
			get
			{
				for (int chapter = 0; chapter < ScriptProvider.VersificationInfo.GetChaptersInBook(BookNumber); chapter++)
				{
					if (ScriptProvider.GetTranslatedVerseCount(BookNumber, chapter) > 0)
						return true;
				}
				return false;
			}
		}


		public void MakeDummyRecordings()
		{
			Cursor.Current = Cursors.WaitCursor;
			for (int c = 0 /*0 is introduction*/; c <= ChapterCount; c++)
			{
				GetChapter(c).MakeDummyRecordings();
			}
			Cursor.Current = Cursors.Default;
		}

		/// <summary>
		/// Virtual for testing
		/// </summary>
		public virtual string GetChapterFolder(int chapterNumber)
		{
			return ClipRepository.GetChapterFolder(ProjectName, Name, chapterNumber);
		}

		public virtual int GetCountOfRecordingsForChapter(int chapterNumber)
		{
			return ClipRepository.GetCountOfRecordingsInFolder(GetChapterFolder(chapterNumber), ScriptProvider);
		}
	}
}
