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

		public BookInfo(int number, string name, int chapterCount)
		{
			BookNumber = number;
			_name = name;
			ChapterCount = chapterCount;
		}

		public string LocalizedName
		{
			//TODO
			get { return _name; }
		}

		public int BookNumber { get; private set; }

		public virtual ChapterInfo GetChapter(int i)
		{
			return new ChapterInfo(i, i*2/*we don't know yet*/, 0 /*we don't know yet*/);
		}
	}

	public class DummyBookInfo : BookInfo
	{
		public DummyBookInfo():base(19, "Psalms", 10)
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
		public readonly int Verses;
		public readonly int VersesRecorded;

		public ChapterInfo(int chapterNumber, int verses, int versesRecorded)
		{
			ChapterNumber = chapterNumber;
			Verses = verses;
			VersesRecorded = versesRecorded;
		}

		public double PercentDone
		{
			get { return (double) VersesRecorded/ (double)Verses; }

		}
	}
}
