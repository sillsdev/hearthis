// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
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
		private readonly string _projectName;
		private readonly string _name;
		public readonly int ChapterCount;

		/// <summary>
		/// [0] == intro, [1] == chapter 1, etc.
		/// </summary>
		private readonly IScriptProvider _scriptProvider;

		public BookInfo(string projectName, int number, IScriptProvider scriptProvider)
		{
			BookNumber = number;
			_projectName = projectName;
			_name = scriptProvider.VersificationInfo.GetBookName(number);
			ChapterCount = scriptProvider.VersificationInfo.GetChaptersInBook(number);
			_scriptProvider = scriptProvider;
		}

		public string LocalizedName
		{
			// TODO: Implement this - probably as part of making this a Paratext plugin
			get { return _name; }
		}

		/// <summary>
		/// 0-based book number
		/// </summary>
		public int BookNumber { get; private set; }

		public bool HasVerses
		{
			get
			{
				for (int i = 0; i < _scriptProvider.VersificationInfo.GetChaptersInBook(BookNumber); i++)
				{
					if (_scriptProvider.GetTranslatedVerseCount(BookNumber, i + 1) > 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		public ScriptLine GetBlock(int chapter, int block)
		{
			return _scriptProvider.GetBlock(BookNumber, chapter, block);
		}

//        /// <summary>
//        /// bool HasVersesMethod(chapter)
//        /// </summary>
//        public Func<int, int> VerseCountMethod { get; set; }

		public string Name
		{
			get { return _name; }
		}

		public bool HasIntroduction
		{
			get { return _scriptProvider.GetScriptBlockCount(BookNumber, 0) > 0; }
		}

		internal string ProjectName
		{
			get { return _projectName; }
		}

		internal IScriptProvider ScriptProvider
		{
			get { return _scriptProvider; }
		}

		/// <summary>
		/// Gets 0 if there is an intro before chapter 1; otherwise returns 1
		/// </summary>
		internal int FirstChapterNumber
		{
			get { return HasIntroduction ? 0 : 1; }
		}

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
			int scriptBlockCount = _scriptProvider.GetScriptBlockCount(BookNumber);
			if (scriptBlockCount == 0)
				return 0; //should it be 0 or 100 or -1 or what?
			int countOfRecordingsForBook = ClipRepository.GetCountOfRecordingsForBook(ProjectName, Name);
			if (countOfRecordingsForBook == 0)
				return 0;
			return Math.Max(1, (int)(100.0 * countOfRecordingsForBook / scriptBlockCount));
		}

		public int CalculatePercentageTranslated()
		{
			// TODO: Use statistics to get a real percentage

			if (_scriptProvider.GetTranslatedVerseCount(BookNumber, 1) > 0)
				return 100;
			return 0;
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
			return ClipRepository.GetChapterFolder(_projectName, _name, chapterNumber);
		}

		public virtual int GetCountOfRecordingsForChapter(int chapterNumber)
		{
			return ClipRepository.GetCountOfRecordingsInFolder(GetChapterFolder(chapterNumber));
		}
	}
}
