using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HearThis.Publishing;

namespace HearThis.Script
{
	public class Project
	{
		private BookInfo _selectedBook;
		private ChapterInfo _selectedChapterInfo;
		public static readonly BibleStats Statistics = new BibleStats();
		public List<BookInfo> Books { get; set; }
		private readonly IScriptProvider _scriptProvider;
		private int _selectedScriptLine;

		public Project(string name, IScriptProvider scriptProvider)
		{
			_scriptProvider = scriptProvider;

			Name = name;
			Books = new List<BookInfo>();

			for (int bookNumber = 0; bookNumber < Statistics.BookNames.Count(); ++bookNumber)
			{
				Books.Add(new BookInfo(Name, bookNumber, _scriptProvider));
			}

			SelectedBook = Books.First();
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
				}
			}
		}

		public string EthnologueCode { get { return _scriptProvider.EthnologueCode; } }

		public void GotoInitialChapter()
		{
			SelectedChapterInfo = _selectedBook.GetChapter(_selectedBook.HasIntroduction ? 0 : 1);
		}

		public ChapterInfo SelectedChapterInfo
		{
			get { return _selectedChapterInfo; }
			set
			{
				if (_selectedChapterInfo != value)
				{
					_selectedChapterInfo = value;
					SelectedScriptLine = 1;
				}
			}
		}

		/// <summary>
		/// This would be the verse, except there are more things than verses to read (chapter #, section headings, etc.)
		/// </summary>
		public int SelectedScriptLine
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
			var threeLetterAbreviations = new BibleStats().ThreeLetterAbreviations;
			if (SelectedBook == null || SelectedBook.BookNumber >= threeLetterAbreviations.Count
				|| SelectedChapterInfo == null || SelectedScriptLine >= SelectedChapterInfo.GetScriptLineCount())
				return;
			var abbr = threeLetterAbreviations[SelectedBook.BookNumber];
			var line = SelectedBook.GetLineMethod(SelectedChapterInfo.ChapterNumber1Based, SelectedScriptLine);
			var targetRef = string.Format("{0} {1}:{2}", abbr, SelectedChapterInfo.ChapterNumber1Based, line.Verse);
			ParatextFocusHandler.SendFocusMessage(targetRef);
		}

		public string Name { get; set; }

		public bool HaveSelectedScript
		{
			get { return SelectedScriptLine >= 0; }
		}

		public int GetLineCountForChapter()
		{
			return _scriptProvider.GetScriptLineCount(_selectedBook.BookNumber,_selectedChapterInfo.ChapterNumber1Based);
		}

		public void LoadBookAsync(int bookNumber0Based, Action action)
		{
			var worker = new BackgroundWorker();
			worker.DoWork += delegate { _scriptProvider.LoadBook(bookNumber0Based);};
			worker.RunWorkerCompleted += delegate { action(); };
			worker.RunWorkerAsync();
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
			return LineRecordingRepository.GetPathToLineRecording(Name, SelectedBook.Name,
				SelectedChapterInfo.ChapterNumber1Based, SelectedScriptLine);
		}
	}
}