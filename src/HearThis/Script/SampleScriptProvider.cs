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

		public int GetLineCountForChapter(int book, int chapter)
		{
			return _stats.GetPossibleVersesInChapter(book,chapter);
		}

		public bool HasVerses(int bookNumberDelegateSafe, int chapterNumber)
		{
			return true;
		}

//        public string[] GetLines(int bookNumber, int chapterNumber)
//        {
//            var x = new string[0];
//            x[0] = GetLine(bookNumber, chapterNumber, 1);
//            return x;
//        }
	}
}