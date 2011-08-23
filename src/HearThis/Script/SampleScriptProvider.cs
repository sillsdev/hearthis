namespace HearThis.Script
{
	public class SampleScriptProvider : IScriptProvider
	{
		public string GetLine(int bookNumber, int chapterNumber, int lineNumber)
		{
			return string.Format("Pretend this is text from Book {0}, Chapter {1}, Verse {2}", bookNumber, chapterNumber,
								 lineNumber);
		}

		public int GetLineCountForChapter(int book, int chapter)
		{
			return 1;
		}

		public string[] GetLines(int bookNumber, int chapterNumber)
		{
			var x = new string[0];
			x[0] = GetLine(bookNumber, chapterNumber, 1);
			return x;
		}
	}
}