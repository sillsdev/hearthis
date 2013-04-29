using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paratext;

namespace HearThis.Script
{
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
			IScrParserState state = new ScrParserState(_scrText, verseRef);

		}

		#endregion
	}
}
