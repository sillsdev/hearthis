using System;

namespace HearThis.Script
{
	public class BookInfo
	{
		private readonly string _name;
		public readonly int ChapterCount;
		private readonly int[] _versesPerChapter;

		public BookInfo(int number, string name, int chapterCount, int[] versesPerChapter)
		{
			BookNumber = number;
			_name = name;
			ChapterCount = chapterCount;
			_versesPerChapter = versesPerChapter;
		}

		public string LocalizedName
		{
			//TODO
			get { return _name; }
		}

		public int BookNumber { get; private set; }

		public bool HasVerses
		{
			get
			{
				if (VerseCountMethod == null)
				{
					var r = new Random();
					return r.Next(4) == 1;
				}

				//at the moment, we just look for verses in the first chapter
				return VerseCountMethod(1) > 0;
			}
		}

		public bool HasSomeRecordings
		{
			get
			{
				if (GetLineMethod == null)
				{
					var r = new Random();
					return r.Next(8) == 1;
				}
				return false;
			}
		}


		public bool HasAllRecordings
		{
			get
			{
				if (GetLineMethod == null)
				{
					var r = new Random();
					return r.Next(10) == 1;
				}
				return false;
			}
		}


		public Func<int, int, ScriptLine> GetLineMethod { get; set; }

		/// <summary>
		/// bool HasVersesMethod(chapter)
		/// </summary>
		public Func<int, int> VerseCountMethod { get; set; }

		public string Name
		{
			get { return _name; }

		}

		public virtual ChapterInfo GetChapter(int oneBased)
		{
			return new ChapterInfo(oneBased,
				_versesPerChapter[oneBased-1], //note, this is still the possible verses, not the actual
				VerseCountMethod(oneBased),
				0 /*we don't know yet*/);
		}
	}

	public class DummyBookInfo : BookInfo
	{
		public DummyBookInfo():base(19, "Psalms", 10, new int[]{3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3})
		{

		}
		public override ChapterInfo GetChapter(int oneBased)
		{
			return new ChapterInfo(oneBased+1, 30, 25, 0);
		}
	}
}
