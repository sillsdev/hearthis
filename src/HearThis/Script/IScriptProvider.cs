using System.Collections.Generic;

namespace HearThis.Script
{
	public interface IScriptProvider
	{
		/// <summary>
		/// The "block" is a bit of script that can be recorded as a single clip; text is generally broken into blocks based on
		/// paragraph breaks and sentence-final punctuation, not verse breaks.
		/// </summary>
		ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based);

		int GetScriptBlockCount(int bookNumber, int chapter1Based);
		int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based);
		int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based);
		int GetTranslatedVerseCount(int bookNumber0Based, int chapterNumber1Based);
		int GetScriptBlockCount(int bookNumber);
		void LoadBook(int bookNumber0Based);
		string EthnologueCode { get; }
		string ProjectFolderName { get; }
		IEnumerable<string> AllEncounteredParagraphStyleNames { get; }
		IBibleStats VersificationInfo { get; }
	}
}