using System;

namespace HearThis.Script
{
	public class BookInfo
	{
		private readonly string _projectName;
		private readonly string _name;
		public readonly int ChapterCount;
		private readonly int[] _versesPerChapter;
		private readonly IScriptProvider _scriptProvider;

		public BookInfo(string projectName, int number, string name, int chapterCount, int[] versesPerChapter, IScriptProvider scriptProvider)
		{
			BookNumber = number;
			_projectName = projectName;
			_name = name;
			ChapterCount = chapterCount;
			_versesPerChapter = versesPerChapter;
			_scriptProvider = scriptProvider;
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

		public virtual ChapterInfo GetChapter(int chapterOneBased)
		{
			return new ChapterInfo(_projectName, Name, BookNumber, chapterOneBased,
				_versesPerChapter[chapterOneBased-1], //note, this is still the possible verses, not the actual
				VerseCountMethod(chapterOneBased),
				_scriptProvider);
		}
	}

//    public class DummyBookInfo : BookInfo
//    {
//        public DummyBookInfo():base("Sample", 19, "Psalms", 10, new int[]{3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3}, TODO)
//        {
//
//        }
//        public override ChapterInfo GetChapter(int chapterOneBased)
//        {
//            return new ChapterInfo("Sample", Name, BookNumber, chapterOneBased+1, 30, 25, );
//        }
//    }
}
