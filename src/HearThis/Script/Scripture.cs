using System.Collections.Generic;
using Paratext;

namespace HearThis.Script
{
	/// <summary>
	/// This is the real implementation of IScripture which implements the interface by
	/// wrapping a real ScrText.
	/// </summary>
	class Scripture : IScripture
	{
		private ScrText _scrText;

		public Scripture(ScrText text)
		{
			_scrText = text;
		}

		#region IScripture Members

		public ScrVers Versification
		{
			get { return _scrText.Versification; }
		}

		public List<UsfmToken> GetUsfmTokens(VerseRef verseRef, bool singleChapter, bool doMapIn)
		{
			var parser = new ScrParser(_scrText, true);
			return parser.GetUsfmTokens(verseRef, false, true);

		}

		public IScrParserState CreateScrParserState(VerseRef verseRef)
		{
			return new ParserState(new ScrParserState(_scrText, verseRef));
		}

		#endregion
	}
}
