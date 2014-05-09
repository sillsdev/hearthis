using System.Collections.Generic;
using Paratext;

namespace HearThis.Script
{
	/// <summary>
	/// This exposes the things we care about out of ScrText, providing an
	/// anti-corruption layer between Paratext and HearThis and allowing us to test the code
	/// that calls Paratext.
	/// </summary>
	public interface IScripture
	{
		ScrVers Versification { get; }
		List<UsfmToken> GetUsfmTokens(VerseRef verseRef, bool singleChapter);
		IScrParserState CreateScrParserState(VerseRef verseRef);
		string DefaultFont { get; }
		string EthnologueCode { get; }
	}
}