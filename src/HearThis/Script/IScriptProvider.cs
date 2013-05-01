namespace HearThis.Script
{
	public interface IScriptProvider
	{
		/// <summary>
		/// The "line" is a bit of script; it would be the verse, except there are more things than verses to read (chapter #, section headings, etc.)
		/// </summary>
		ScriptLine GetLine(int bookNumber, int chapterNumber, int lineNumber0Based);

	   // string[] GetLines(int bookNumber, int chapter1Based);
		int GetScriptLineCount(int bookNumber, int chapter1Based);
		int GetTranslatedVerseCount(int bookNumberDelegateSafe, int chapterNumber1Based);
		int GetScriptLineCount(int bookNumber);
		void LoadBook(int bookNumber0Based);
	}
}