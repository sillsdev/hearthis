namespace HearThis.Script
{
	public interface IScriptProvider
	{
		/// <summary>
		/// The "line" is a bit of script; it would be the verse, except there are more things than verses to read (chapter #, section headings, etc.)
		/// </summary>
		string GetLine(int bookNumber, int chapterNumber, int lineNumber);

	   // string[] GetLines(int bookNumber, int chapterNumber);
		int GetLineCountForChapter(int bookNumber, int chapterNumber);
	}
}