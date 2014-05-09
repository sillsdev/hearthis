using System.Collections.Generic;
using HearThis.Script;
using Paratext;

namespace HearThisTests
{
	internal class ScriptureStub : IScripture
	{
		public List<UsfmToken> UsfmTokens;

		public void SetDefaultFont(string fontName)
		{
			DefaultFont = fontName;
		}

		#region IScripture Members

		public ScrVers Versification
		{
			get { return ScrVers.English; }
		}

		public List<UsfmToken> GetUsfmTokens(VerseRef verseRef, bool singleChapter)
		{
			return UsfmTokens;
		}

		public IScrParserState CreateScrParserState(VerseRef verseRef)
		{
			return new ParserStateStub();
		}

		public string DefaultFont { get; private set; }
		public string EthnologueCode { get { return "KAL"; } }

		#endregion
	}
}
