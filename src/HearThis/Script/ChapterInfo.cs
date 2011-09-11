using HearThis.Publishing;
using Palaso.Code;

namespace HearThis.Script
{
	public class ChapterInfo
	{
		private readonly string _projectName;
		private readonly string _bookName;
		private readonly int _bookNumber;
		public readonly int ChapterNumber1Based;
		public readonly int VersesPossible;
		public readonly int VersesTranslated;
		private readonly IScriptProvider _scriptProvider;


		public ChapterInfo(string projectName, string bookName, int bookNumber, int chapterNumber1Based, int versesPossible, int versesTranslated, IScriptProvider scriptProvider)
		{
			Guard.Against(chapterNumber1Based <1, "Chapter number is 1-based");
			_projectName = projectName;
			_bookName = bookName;
			_bookNumber = bookNumber;
			ChapterNumber1Based = chapterNumber1Based;
			VersesPossible = versesPossible;
			VersesTranslated = versesTranslated;
			_scriptProvider = scriptProvider;
		}

		public bool HasVerses
		{
			get { return VersesTranslated > 0;  }
		}

		public int CalculatePercentageRecorded()
		{
				var repo = new LineRecordingRepository();
				return 100* repo.GetCountOfRecordingsForChapter(_projectName, _bookName, ChapterNumber1Based)/_scriptProvider.GetLineCountForChapter(_bookNumber,ChapterNumber1Based);
		}
	}
}