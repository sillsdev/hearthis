using Palaso.Code;

namespace HearThis.Script
{
	public class ChapterInfo
	{
		public readonly int ChapterNumber1Based;
		public readonly int VersesPossible;
		public readonly int VersesTranslated;
		public readonly int VersesRecorded;

		public ChapterInfo(int chapterNumber1Based, int versesPossible, int versesTranslated, int versesRecorded)
		{
			Guard.Against(chapterNumber1Based <1, "Chapter number is 1-based");
			ChapterNumber1Based = chapterNumber1Based;
			VersesPossible = versesPossible;
			VersesTranslated = versesTranslated;
			VersesRecorded = versesRecorded;
		}

		public bool HasVerses
		{
			get { return VersesTranslated > 0;  }
		}
	}
}