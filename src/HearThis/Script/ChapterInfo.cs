namespace HearThis.Script
{
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