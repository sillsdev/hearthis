using System.Collections.Generic;
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
				book.VerseCountMethod = ((chapterNumber1Based) => _scriptProvider.TranslatedVerses(bookNumberDelegateSafe, chapterNumber1Based));
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
					SelectedChapterInfo = value.GetChapter(1);
				}
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
					SelectedScriptLine = 1;
				}
			}
		}

		/// <summary>
		/// This  would be the verse, except there are more things than verses to read (chapter #, section headings, etc.)
		/// </summary>
		public int SelectedScriptLine { get; set; }

		public string Name { get; set; }

		public int GetLineCountForChapter()
		{
			return _scriptProvider.GetLineCountForChapter(_selectedBook.BookNumber,_selectedChapterInfo.ChapterNumber1Based);
		}
	}
}