using System;
using System.Collections.Generic;
using HearThis.Script;
using Paratext;

namespace HearThisTests
{
	internal class ScriptureStub : IScripture
	{
		public List<UsfmToken> UsfmTokens;

		#region IScripture Members

		public ScrVers Versification
		{
			get { return ScrVers.English; }
		}

		public List<UsfmToken> GetUsfmTokens(VerseRef verseRef, bool singleChapter, bool doMapIn)
		{
			return UsfmTokens;
		}

		public IScrParserState CreateScrParserState(VerseRef verseRef)
		{
			return new ParserStateStub();
		}

		#endregion
	}
}
