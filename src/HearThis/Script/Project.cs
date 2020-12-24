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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HearThis.Properties;
using HearThis.Publishing;

namespace HearThis.Script
{
	public class Project : ISkippedStyleInfoProvider, IPublishingInfoProvider
	{
		public const string InfoTxtFileName = "info.txt";
		private BookInfo _selectedBook;
		private ChapterInfo _selectedChapterInfo;
		public List<BookInfo> Books { get; }
		private readonly ScriptProviderBase _scriptProvider;
		private int _selectedScriptLine;
		public event EventHandler SelectedBookChanged;

		public event ScriptBlockChangedHandler ScriptBlockRecordingRestored;

		public delegate void ScriptBlockChangedHandler(Project sender, int book, int chapter, ScriptLine scriptBlock);

		public Project(ScriptProviderBase scriptProvider)
		{
			_scriptProvider = scriptProvider;
			ProjectSettings = _scriptProvider.ProjectSettings;
			VersificationInfo = _scriptProvider.VersificationInfo;
			_scriptProvider.ScriptBlockUnskipped += OnScriptBlockUnskipped;
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

		public ProjectSettings ProjectSettings { get; }

		public BookInfo SelectedBook
		{
			get { return _selectedBook; }
			set
			{
				if (_selectedBook != value)
				{
					_selectedBook = value;
					_scriptProvider.LoadBook(_selectedBook.BookNumber);
					GoToInitialChapter();

					Settings.Default.Book = value.BookNumber;

					SelectedBookChanged?.Invoke(this, new EventArgs());
				}
			}
		}

		internal void SaveProjectSettings()
		{
			_scriptProvider.SaveProjectSettings();
		}

		/// <summary>
		/// Return the content of the info.txt file we create to help HearThisAndroid.
		/// It contains a line for each book.
		/// Each line contains BookName;blockcount:recordedCount,... for each chapter
		/// (Not filtered by character.)
		/// </summary>
		/// <returns></returns>
		internal string GetProjectRecordingStatusInfoFileContent()
		{
			var sb = new StringBuilder();
			for (int ibook = 0; ibook < Books.Count; ibook++)
			{
				var book = Books[ibook];
				var bookName = book.Name;
				sb.Append(bookName);
				sb.Append(";");
				//sb.Append(book.ChapterCount);
				//sb.Append(";");
				//sb.Append(book.HasVerses ? "y" : "n");
				//sb.Append(";");
				if (!book.HasVerses)
				{
					sb.AppendLine("");
					continue;
				}
				for (int ichap = 0; ichap <= _scriptProvider.VersificationInfo.GetChaptersInBook(ibook); ichap++)
				{
					var chap = book.GetChapter(ichap);
					var lines = chap.GetUnfilteredScriptBlockCount();
					if (ichap != 0)
						sb.Append(',');
					sb.Append(lines);
					sb.Append(":");
					sb.Append(chap.CalculateUnfilteredPercentageTranslated());
					//for (int iline = 0; iline < lines; iline++)
					//	_lineRecordingRepository.WriteLineText(projectName, bookName, ichap, iline,
					//		chap.GetScriptLine(iline).Text);
				}
				sb.AppendLine("");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Where we will store the info.txt file we create to help HearThisAndroid
		/// </summary>
		/// <returns></returns>
		public string GetProjectRecordingStatusInfoFilePath()
		{
			return Path.Combine(Program.GetApplicationDataFolder(Name), InfoTxtFileName);
		}

		public bool IsRealProject
		{
			get { return !(_scriptProvider is SampleScriptProvider); }
		}

		public string EthnologueCode
		{
			get { return _scriptProvider.EthnologueCode; }
		}

		public bool RightToLeft
		{
			get { return _scriptProvider.RightToLeft; }
		}

		public string FontName
		{
			get { return _scriptProvider.FontName; }
		}

		public string CurrentBookName => _selectedBook.Name;

		public bool IncludeBook(string bookName)
		{
			int bookIndex = _scriptProvider.VersificationInfo.GetBookNumber(bookName);
			if (bookIndex < 0)
				return false;
			for (int iChapter = 0; iChapter < _scriptProvider.VersificationInfo.GetChaptersInBook(bookIndex); iChapter++)
			{
				if (_scriptProvider.GetUnfilteredTranslatedVerseCount(bookIndex, iChapter) > 0)
					return true;
			}
			return false;
		}

		public ScriptLine GetUnfilteredBlock(string bookName, int chapterNumber, int lineNumber0Based)
		{
			return _scriptProvider.GetUnfilteredBlock(_scriptProvider.VersificationInfo.GetBookNumber(bookName), chapterNumber, lineNumber0Based);
		}

		public IBibleStats VersificationInfo { get; private set; }

		public int BookNameComparer(string x, string y)
		{
			return Comparer.Default.Compare(_scriptProvider.VersificationInfo.GetBookNumber(x),
				_scriptProvider.VersificationInfo.GetBookNumber(y));
		}

		public bool BreakQuotesIntoBlocks => ProjectSettings.BreakQuotesIntoBlocks;

		/// <summary>
		/// Note that this is NOT the same as ProjectSettings.AdditionalBlockBreakCharacters.
		/// This property is implemented especially to support publishing and may include
		/// additional characters not stored in the project setting by the same name.
		/// </summary>
		string IPublishingInfoProvider.AdditionalBlockBreakCharacters
		{
			get
			{
				var bldr = new StringBuilder(ProjectSettings.AdditionalBlockBreakCharacters);
				var firstLevelStartQuotationMark = ScrProjectSettings?.FirstLevelStartQuotationMark;
				if (BreakQuotesIntoBlocks && !String.IsNullOrEmpty(firstLevelStartQuotationMark))
				{
					if (bldr.Length > 0)
						bldr.Append(" ");
					bldr.Append(firstLevelStartQuotationMark);
					var firstLevelEndQuotationMark = ScrProjectSettings.FirstLevelEndQuotationMark;
					if (firstLevelStartQuotationMark != firstLevelEndQuotationMark)
						bldr.Append(" ").Append(firstLevelEndQuotationMark);
				}
				return bldr.ToString();
			}
		}

		public void GoToInitialChapter()
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
		/// Project.SelectedScriptBlock is unfiltered (that is, an index into all the blocks in the chapter, not into
		/// the current-character list).
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
				|| SelectedChapterInfo == null || SelectedScriptBlock >= SelectedChapterInfo.GetUnfilteredScriptBlockCount())
				return;
			var abbr = _scriptProvider.VersificationInfo.GetBookCode(SelectedBook.BookNumber);
			var block = SelectedBook.GetUnfilteredBlock(SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock);
			var verse = block.Verse ?? "";
			int i = verse.IndexOfAny(new[] {'-', '~'});
			if (i > 0)
				verse = verse.Substring(0, i);
			var targetRef = string.Format("{0} {1}:{2}", abbr, SelectedChapterInfo.ChapterNumber1Based, verse);
			ParatextFocusHandler.SendFocusMessage(targetRef);
		}

		public string Name { get; set; }

		public IScrProjectSettings ScrProjectSettings
		{
			get
			{
				var paratextScriptProvider = _scriptProvider as IScrProjectSettingsProvider;
				return paratextScriptProvider?.ScrProjectSettings;
			}
		}

		public bool HasNestedQuotes
		{
			get { return _scriptProvider.NestedQuotesEncountered; }
		}

		public bool HaveSelectedScript
		{
			get { return SelectedScriptBlock >= 0; }
		}

		public IEnumerable<string> AllEncounteredParagraphStyleNames
		{
			get { return _scriptProvider.AllEncounteredParagraphStyleNames; }
		}

		public IActorCharacterProvider ActorCharacterProvider => _scriptProvider as IActorCharacterProvider;

		public string CurrentCharacter => ActorCharacterProvider?.Character;

		// Unfiltered by character
		public int GetLineCountForChapter(bool includeSkipped)
		{
			if (includeSkipped)
				return _scriptProvider.GetUnfilteredScriptBlockCount(_selectedBook.BookNumber, _selectedChapterInfo.ChapterNumber1Based);
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
			return ClipRepository.GetPathToLineRecordingUnfiltered(Name, SelectedBook.Name,
				SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock);
		}

		internal string ProjectFolder => ClipRepository.GetProjectFolder(Name);

		public void SetSkippedStyle(string style, bool skipped)
		{
			_scriptProvider.SetSkippedStyle(style, skipped);
		}

		public bool IsSkippedStyle(string style) => _scriptProvider.IsSkippedStyle(style);

		public IEnumerable<string> StylesToSkipByDefault => _scriptProvider.StylesToSkipByDefault;

		public void ClearAllSkippedBlocks()
		{
			_scriptProvider.ClearAllSkippedBlocks(Books);
		}

		public IScriptProvider ScriptProvider => _scriptProvider;

		private void OnScriptBlockUnskipped(IScriptProvider sender, int bookNumber, int chapterNumber, ScriptLine scriptBlock)
		{
			// passing an unfiltered scriptBlockNumber, so do NOT pass a script provider so it won't be adjusted
			if (ClipRepository.RestoreBackedUpClip(Name, Books[bookNumber].Name, chapterNumber, scriptBlock.Number - 1))
				ScriptBlockRecordingRestored?.Invoke(this, bookNumber, chapterNumber, scriptBlock);
		}

		/// <summary>
		/// Strictly speaking, skipped lines are not recordable, but we leave the button enabled and give the user a message if clicked,
		/// so we want to treat them as recordable if skipping is the only thing standing in the way.
		/// Similarly, if we're in overview mode, nothing can currently be recorded, but we want to show things that could be
		/// if we were in the right character as recordable.
		/// </summary>
		/// <param name="book"></param>
		/// <param name="chapterNumber1Based"></param>
		/// <param name="lineNo0Based"></param>
		/// <returns></returns>
		public bool IsLineCurrentlyRecordable(int book, int chapterNumber1Based, int lineNo0Based)
		{
			var line = _scriptProvider.GetUnfilteredBlock(book, chapterNumber1Based, lineNo0Based);
			if (string.IsNullOrEmpty(line.Text))
				return false;
			if (ActorCharacterProvider == null || ActorCharacterProvider.Character == null)
				return true; // no filtering (or overview mode).
			return line.Character == ActorCharacterProvider.Character && line.Actor == ActorCharacterProvider.Actor;
		}
	}
}
