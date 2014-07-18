using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
			if (UsfmTokens != null && UsfmTokens.Count > 0 && UsfmTokens[0].HasData && UsfmTokens[0].Data[0] == verseRef.Book)
				return UsfmTokens;
			return new List<UsfmToken>();
		}

		public IScrParserState CreateScrParserState(VerseRef verseRef)
		{
			return new ParserState(new ScrParserState(
				new ScrStylesheet(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "usfm.sty")),
				verseRef));
		}

		public string DefaultFont { get; private set; }
		public string EthnologueCode { get { return "KAL"; } }
		public string Name { get { return "Stub"; } }
		#endregion
	}
}
