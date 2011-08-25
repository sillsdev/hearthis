namespace HearThis.Script
{
	public class SampleScriptProvider : IScriptProvider
	{
		public ScriptLine GetLine(int bookNumber, int chapterNumber, int lineNumber)
		{
			return new ScriptLine()
					   {
						   Text =
							   string.Format("Pretend this is text from Book {0}, Chapter {1}, Verse {2}", bookNumber,
											 chapterNumber,
											 lineNumber)
					   };
		}

		public int GetLineCountForChapter(int book, int chapter)
		{
			return 1;
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