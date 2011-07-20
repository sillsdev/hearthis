using System.Collections.Generic;
using System.IO;
using Palaso.IO;
using System.Linq;
using Paratext;

namespace HearThis
{
	public class Project
	{
		private readonly ScrText _paratextProject;
		private BookInfo _selectedBook;
		private ChapterInfo _selectedChapter;
		public List<string> BookNames ;
		public List<int> ChaptersPerBook;
		private Dictionary<int, int[]> VersesPerChapterPerBook;
		public List<BookInfo> Books { get; set; }

		public Project(ScrText paratextProject)
		{
			_paratextProject = paratextProject;
			Books = new List<BookInfo>();
			LoadStatistics();
			var chapterCounts = ChaptersPerBook.ToArray();


			for (int bookNumber = 0; bookNumber < BookNames.Count(); ++bookNumber )
			{
				int bookNumberDelegateSafe = bookNumber;
				var book = new BookInfo(bookNumber, BookNames.ElementAt(bookNumber), chapterCounts[bookNumber], VersesPerChapterPerBook[bookNumber]);
				bookNumberDelegateSafe = bookNumber;
				book.GetVerse = ((chapter, verse) => GetVerse(paratextProject, bookNumberDelegateSafe, chapter, verse));
				Books.Add(book);
			}
			SelectedBook = Books.First();
		}

		public string GetVerse(ScrText paratextText, int bookNumber, int chapterNumber, int verseNumber)
		{
			//return "verse "+chapterNumber +":"+verseNumber;//todo, count verses
			return paratextText.GetVerseText(new VerseRef(bookNumber+1, chapterNumber, verseNumber, paratextText.Versification), true);
		}



		public Project()
		{
			Books = new List<BookInfo>();
			LoadStatistics();
			int i = 0;
			foreach (var name in BookNames)
			{
				Books.Add(new BookInfo(i, name, ChaptersPerBook[i], VersesPerChapterPerBook[i]));
				++i;
			}
			SelectedBook = Books.First();
		}

		private void LoadStatistics()
		{
			BookNames = new List<string>();
			ChaptersPerBook = new List<int>();
			VersesPerChapterPerBook = new Dictionary<int, int[]>();
			int index = 0;
			foreach (
				string line in File.ReadAllLines(FileLocator.GetFileDistributedWithApplication("chapterCounts.txt")))
			{
				var parts = line.Trim().Split(new char[] {'\t'});
				if (parts.Length > 2)
				{
					BookNames.Add(parts[0]);
					ChaptersPerBook.Add(int.Parse(parts[1]));
					var verseArray = new List<int>();
					for (int i = 2; i < parts.Length; i++)
					{
					   verseArray.Add(int.Parse(parts[i]));
					}
					VersesPerChapterPerBook.Add(index, verseArray.ToArray());
				}
				++index;
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
					SelectedChapter = value.GetChapter(0);
				}
			}
		}

		public ChapterInfo SelectedChapter
		{
			get { return _selectedChapter; }
			set
			{
				if (_selectedChapter != value)
				{
					_selectedChapter = value;
					SelectedVerse = 1;
				}
			}
		}

		public int SelectedVerse { get; set; }

		public string Name
		{
			get { return _paratextProject.Name; }
		}
	}
}