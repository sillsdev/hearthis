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
		private readonly IScriptProvider _scriptProvider;

		/// <summary>
		///
		/// </summary>
		/// <param name="projectName"></param>
		/// <param name="bookName"></param>
		/// <param name="bookNumber"></param>
		/// <param name="chapterNumber1Based">[0] == intro, [1] == chapter 1, etc.</param>
		/// <param name="versesPossible"></param>
		/// <param name="scriptProvider"></param>
		public ChapterInfo(string projectName, string bookName, int bookNumber, int chapterNumber1Based, int versesPossible, IScriptProvider scriptProvider)
		{
			_projectName = projectName;
			_bookName = bookName;
			_bookNumber = bookNumber;
			ChapterNumber1Based = chapterNumber1Based;
			VersesPossible = versesPossible;
			_scriptProvider = scriptProvider;
		}

		public bool IsEmpty
		{
			get { return _scriptProvider.GetScriptLineCount(_bookNumber, ChapterNumber1Based) == 0; }
		}

		public int CalculatePercentageRecorded()
		{
				var repo = new LineRecordingRepository();
			int scriptLineCount = _scriptProvider.GetScriptLineCount(_bookNumber,ChapterNumber1Based);
			if (scriptLineCount == 0)
				return 0;//should it be 0 or 100 or -1 or what?
			return 100* repo.GetCountOfRecordingsForChapter(_projectName, _bookName, ChapterNumber1Based)/scriptLineCount;
		}

		public int CalculatePercentageTranslated()
		{
			 return (_scriptProvider.GetTranslatedVerseCount(_bookNumber, ChapterNumber1Based));
		}
	}
}