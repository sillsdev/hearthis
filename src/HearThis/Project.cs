using System.Collections.Generic;
using System.IO;
using Palaso.IO;
using System.Linq;

namespace HearThis
{
	public class Project
	{
		private BookInfo _selectedBook;
		private ChapterInfo _selectedChapter;

		public Project()
		{
			Books = new List<BookInfo>();
			int count = 0;
			foreach (string line in File.ReadAllLines(FileLocator.GetFileDistributedWithApplication("chapterCounts.txt")))
			{
				var parts = line.Trim().Split(new char[] { '\t' });
				if (parts.Length == 2)
				{
					++count;
					Books.Add(new BookInfo(count,parts[0], int.Parse(parts[1])));
				}
			}
			SelectedBook = Books.First();
		}

		public List<BookInfo> Books { get; set; }

		public BookInfo SelectedBook
		{
			get { return _selectedBook; }
			set
			{
				if (_selectedBook != value)
				{
					_selectedBook = value;
					SelectedChapter = value.GetChapter(1);
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

		protected int SelectedVerse { get; set; }
	}
}