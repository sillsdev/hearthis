using System.Collections.Generic;
using Paratext;

namespace HearThis.Script
{
	/// <summary>
	///
	/// </summary>
	public interface IScripture
	{
		ScrVers Versification { get; }
		List<UsfmToken> GetUsfmTokens(VerseRef verseRef, bool singleChapter, bool doMapIn);
		IScrParserState CreateScrParserState(VerseRef verseRef);
	}
}