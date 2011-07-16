using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HearThis
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
				if (GetVerse == null)
				{
					var r = new Random();
					return r.Next(4) == 1;
				}

				//at the moment, we just look for verse 1
				return !string.IsNullOrEmpty(GetVerse(1,1));
			}
		}

		public bool HasSomeRecordings
		{
			get
			{
				if (GetVerse == null)
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
				if (GetVerse == null)
				{
					var r = new Random();
					return r.Next(10) == 1;
				}
				return false;
			}
		}

		public Func<int, int, string> GetVerse { get; set; }

		public virtual ChapterInfo GetChapter(int i)
		{
			return new ChapterInfo(i, _versesPerChapter[i] /*note, this is still the possible verses, not the actual*/, 0 /*we don't know yet*/);
		}
	}

	public class DummyBookInfo : BookInfo
	{
		public DummyBookInfo():base(19, "Psalms", 10, new int[]{3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3})
		{

		}
		public override ChapterInfo GetChapter(int i)
		{
			return new ChapterInfo(i, 30, 25);
		}
	}

	public class ChapterInfo
	{
		public readonly int ChapterNumber;
		public readonly int VersePotentialCount;
		public readonly int VersesRecorded;

		public ChapterInfo(int chapterNumber, int versePotentialCount, int versesRecorded)
		{
			ChapterNumber = chapterNumber;
			VersePotentialCount = versePotentialCount;
			VersesRecorded = versesRecorded;
		}

		public double PercentDone
		{
			get { return (double) VersesRecorded/ (double)VersePotentialCount; }

		}
	}
}
