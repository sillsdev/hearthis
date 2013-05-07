using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Palaso.IO;
using System.Linq;

namespace HearThis.Script
{
	public class Project
	{
		private BookInfo _selectedBook;
		private ChapterInfo _selectedChapterInfo;
		public BibleStats Statistics;
		public List<BookInfo> Books { get; set; }
		private IScriptProvider _scriptProvider;

		public Project(string name, IScriptProvider scriptProvider)
		{
			_scriptProvider = scriptProvider;

			Name = name;
			Books = new List<BookInfo>();
			Statistics = new BibleStats();

			var chapterCounts = Statistics.ChaptersPerBook.ToArray();

			for (int bookNumber = 0; bookNumber < Statistics.BookNames.Count(); ++bookNumber)
			{
				int bookNumberDelegateSafe = bookNumber;
				var book = new BookInfo(Name, bookNumber, Statistics.BookNames.ElementAt(bookNumber), chapterCounts[bookNumber], Statistics.VersesPerChapterPerBook[bookNumber],
					_scriptProvider);
				bookNumberDelegateSafe = bookNumber;
				book.GetLineMethod = ((chapter, line) => _scriptProvider.GetLine(bookNumberDelegateSafe, chapter, line));
				Books.Add(book);
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
				   GotoInitialChapter();
				}
			}
		}

		public string EthnologueCode { get { return _scriptProvider.EthnologueCode; } }

		public void GotoInitialChapter()
		{
				if (_selectedBook.HasIntroduction)
					SelectedChapterInfo = _selectedBook.GetChapter(0);
				else
					SelectedChapterInfo = _selectedBook.GetChapter(1);
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
		/// This  would be the verse, except there are more things than verses to read (chapter #, section headings, etc.)
		/// </summary>
		public int SelectedScriptLine { get; set; }

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
			worker.DoWork += new DoWorkEventHandler(delegate { _scriptProvider.LoadBook(bookNumber0Based);});
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate { action(); });
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
	}
}