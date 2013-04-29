namespace HearThis.Script
{
	public class SampleScriptProvider : IScriptProvider
	{
		private BibleStats _stats;

		public SampleScriptProvider()
		{
			_stats = new BibleStats();
		}
		public ScriptLine GetLine(int bookNumber, int chapterNumber, int lineNumber)
		{
			string line;
			if (lineNumber == 0)
				line = _stats.GetBookName(bookNumber) + " Chapter " + chapterNumber;
			else
			{
				line = "Here if we were using a real project, there would be a sentence for you to read.";

				if (chapterNumber == 1)
				{
					if (lineNumber == 1)
						line = "Some introductory material about " + _stats.GetBookName(bookNumber);
				}
			}

			return new ScriptLine()
					{
						Text =line,
						FontName = "Arial",
						FontSize = 12
					};
		}

		public int GetScriptLineCount(int book, int chapter1Based)
		{
			if(chapter1Based ==0)//introduction
				return 0;

			return _stats.GetPossibleVersesInChapter(book, chapter1Based);
		}

		public int GetTranslatedVerseCount(int bookNumberDelegateSafe, int chapterNumber1Based)
		{
			return 1;
		}

		public int GetScriptLineCount(int bookNumber)
		{
			return _stats.GetChaptersInBook(bookNumber+1)*10;
		}

		public void LoadBook(int bookNumber0Based)
		{

		}
	}
}