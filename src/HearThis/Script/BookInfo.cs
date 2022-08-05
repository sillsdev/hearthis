// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2011' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
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

		/// <summary>
		/// Constructs an object that holds information about a Scripture book in a particular project
		/// </summary>
		/// <param name="projectName">Name of project containing the book</param>
		/// <param name="number">0-based book number</param>
		/// <param name="scriptProvider">The object that supplies information about the translated text
		/// and the text of individual blocks</param>
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
		public int BookNumber { get; }

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

		public ChapterInfo GetChapter(int chapterOneBased)
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

		public ProblemType GetWorstProblemInBook()
		{
			var worst = ProblemType.None;
			for (var i = 0; i <= ChapterCount; i++)
			{
				var worstInChapter = GetChapter(i).WorstProblemInChapter;
				if (worstInChapter > worst)
				{
					// For our purposes, we treat all unresolved major problems as equally bad
					// (i.e., in need of attention). If a problem is minor or has been resolved,
					// it does not need attention, so we keep looking to see if we come across
					// something that does.
					if (worstInChapter.NeedsAttention())
						return worstInChapter;
					worst = worstInChapter;
				}
			}
			return worst;
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
