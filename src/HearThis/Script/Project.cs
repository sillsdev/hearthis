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
using System.Collections.Generic;
using System.IO;
using HearThis.Properties;
using HearThis.Publishing;

namespace HearThis.Script
{
	public class Project : ISkippedStyleInfoProvider, IPublishingInfoProvider
	{
		private BookInfo _selectedBook;
		private ChapterInfo _selectedChapterInfo;
		public List<BookInfo> Books { get; set; }
		private readonly ScriptProviderBase _scriptProvider;
		private int _selectedScriptLine;
		public event EventHandler OnSelectedBookChanged;

		public event ScriptBlockChangedHandler OnScriptBlockRecordingRestored;

		public delegate void ScriptBlockChangedHandler(Project sender, int book, int chapter, ScriptLine scriptBlock);

		public Project(ScriptProviderBase scriptProvider)
		{
			_scriptProvider = scriptProvider;
			_scriptProvider.OnScriptBlockUnskipped += OnScriptBlockUnskipped;
			Name = _scriptProvider.ProjectFolderName;
			Books = new List<BookInfo>(_scriptProvider.VersificationInfo.BookCount);

			if (Settings.Default.Book < 0 || Settings.Default.Book >= BibleStatsBase.kCanonicalBookCount)
				Settings.Default.Book = 0;
			for (int bookNumber = 0; bookNumber < _scriptProvider.VersificationInfo.BookCount; ++bookNumber)
			{
				var bookInfo = new BookInfo(Name, bookNumber, _scriptProvider);
				Books.Add(bookInfo);
				if (bookNumber == Settings.Default.Book)
					SelectedBook = bookInfo;
			}
		}

		public BookInfo SelectedBook
		{
			get { return _selectedBook; }
			set
			{
				if (_selectedBook != value)
				{
					_selectedBook = value;
					_scriptProvider.LoadBook(_selectedBook.BookNumber);
					GotoInitialChapter();

					Settings.Default.Book = value.BookNumber;

					if (OnSelectedBookChanged != null)
						OnSelectedBookChanged(this, new EventArgs());
				}
			}
		}

		public string EthnologueCode
		{
			get { return _scriptProvider.EthnologueCode; }
		}

		public string CurrentBookName
		{
			get { return _selectedBook.Name; }
		}

		public ScriptLine GetBlock(string bookName, int chapterNumber, int lineNumber0Based)
		{
			return _scriptProvider.GetBlock(_scriptProvider.VersificationInfo.GetBookNumber(bookName), chapterNumber, lineNumber0Based);
		}

		public IBibleStats VersificationInfo { get; private set; }

		public void GotoInitialChapter()
		{
			if (_selectedChapterInfo == null &&
				Settings.Default.Chapter >= SelectedBook.FirstChapterNumber && Settings.Default.Chapter <= SelectedBook.ChapterCount)
			{
				// This is the very first time for this project. In this case rather than going to the start of the book,
				// we want to go back to the chapter the user was in when they left off last time.
				SelectedChapterInfo = SelectedBook.GetChapter(Settings.Default.Chapter);
			}
			else
			{
				SelectedChapterInfo = _selectedBook.GetFirstChapter();
			}
		}

		public ChapterInfo SelectedChapterInfo
		{
			get { return _selectedChapterInfo; }
			set
			{
				if (_selectedChapterInfo != value)
				{
					_selectedChapterInfo = value;
					Settings.Default.Chapter = value.ChapterNumber1Based;
					SelectedScriptBlock = 0;
				}
			}
		}

		/// <summary>
		/// This is a portion of the Scripture text that is to be recorded as a single clip. Blocks are broken up by paragraph breaks and
		/// sentence-final punctuation, not verses. This is a 0-based index.
		/// </summary>
		public int SelectedScriptBlock
		{
			get { return _selectedScriptLine; }
			set
			{
				_selectedScriptLine = value;
				SendFocus();
			}
		}

		private void SendFocus()
		{
			if (SelectedBook == null || SelectedBook.BookNumber >= _scriptProvider.VersificationInfo.BookCount
				|| SelectedChapterInfo == null || SelectedScriptBlock >= SelectedChapterInfo.GetScriptBlockCount())
				return;
			var abbr = _scriptProvider.VersificationInfo.GetBookCode(SelectedBook.BookNumber);
			var block = SelectedBook.GetBlock(SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock);
			var targetRef = string.Format("{0} {1}:{2}", abbr, SelectedChapterInfo.ChapterNumber1Based, block.Verse);
			ParatextFocusHandler.SendFocusMessage(targetRef);
		}

		public string Name { get; set; }

		public bool HaveSelectedScript
		{
			get { return SelectedScriptBlock >= 0; }
		}

		public IEnumerable<string> AllEncounteredParagraphStyleNames
		{
			get { return _scriptProvider.AllEncounteredParagraphStyleNames; }
		}

		public int GetLineCountForChapter(bool includeSkipped)
		{
			if (includeSkipped)
				return _scriptProvider.GetScriptBlockCount(_selectedBook.BookNumber, _selectedChapterInfo.ChapterNumber1Based);
			return _scriptProvider.GetUnskippedScriptBlockCount(_selectedBook.BookNumber,
				_selectedChapterInfo.ChapterNumber1Based);
		}

		public void LoadBook(int bookNumber0Based)
		{
			_scriptProvider.LoadBook(bookNumber0Based);
		}

		internal ChapterInfo GetNextChapterInfo()
		{
			var currentChapNum = SelectedChapterInfo.ChapterNumber1Based;
			if (currentChapNum == SelectedBook.ChapterCount)
				throw new ArgumentOutOfRangeException("Tried to get too high a chapter number.");

			return SelectedBook.GetChapter(currentChapNum + 1);
		}

		internal int GetNextChapterNum()
		{
			return GetNextChapterInfo().ChapterNumber1Based;
		}

		internal string GetPathToRecordingForSelectedLine()
		{
			return ClipRepository.GetPathToLineRecording(Name, SelectedBook.Name,
				SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock);
		}

		public void SetSkippedStyle(string style, bool skipped)
		{
			_scriptProvider.SetSkippedStyle(style, skipped);
		}

		public bool IsSkippedStyle(string style)
		{
			return _scriptProvider.IsSkippedStyle(style);
		}

		public void ClearAllSkippedBlocks()
		{
			_scriptProvider.ClearAllSkippedBlocks(Books);
		}

		public void BackUpRecordingForSkippedLine()
		{
			var recordingPath = GetPathToRecordingForSelectedLine();
			File.Move(recordingPath, Path.ChangeExtension(recordingPath, "skip"));
		}

		private void OnScriptBlockUnskipped(IScriptProvider sender, int bookNumber, int chapterNumber, ScriptLine scriptBlock)
		{
			var recordingPath = ClipRepository.GetPathToLineRecording(
				Name, Books[bookNumber].Name, chapterNumber, scriptBlock.Number - 1);
			var skipPath = Path.ChangeExtension(recordingPath, "skip");
			if (File.Exists(skipPath))
			{
				File.Move(skipPath, recordingPath);

				if (OnScriptBlockRecordingRestored != null)
					OnScriptBlockRecordingRestored(this, bookNumber, chapterNumber, scriptBlock);
			}
		}
	}
}
