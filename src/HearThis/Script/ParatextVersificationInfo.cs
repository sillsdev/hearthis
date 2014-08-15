using Paratext;

namespace HearThis.Script
{
	class ParatextVersificationInfo : BibleStatsBase, IBibleStats
	{
		private readonly ScrVers _scrVers;

		public ParatextVersificationInfo(ScrVers scrVers)
		{
			_scrVers = scrVers;
		}

		public int GetChaptersInBook(int bookNumber0Based)
		{
			int bookNumber1Based = bookNumber0Based + 1;
			if (_scrVers == ScrVers.Septuagint)
			{
				if (bookNumber1Based == Canon.BookIdToNumber("EST", true))
					return _scrVers.LastChapter(Canon.BookIdToNumber("ESG", true));
				if (bookNumber1Based == Canon.BookIdToNumber("DAN", true))
					return _scrVers.LastChapter(Canon.BookIdToNumber("DAG", true));
			}
			return _scrVers.LastChapter(bookNumber1Based);
		}
	}
}
