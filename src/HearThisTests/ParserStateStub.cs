using System.Collections.Generic;
using HearThis.Script;
using Paratext;

namespace HearThisTests
{
	internal class ParserStateStub : IScrParserState
	{
		private string _endMarker = null;
		public HashSet<string> NoteMarkers = new HashSet<string>(new[] { "nt", "nt1", "f" });
		public HashSet<string> ParaMarkers = new HashSet<string>(new[] { "mt", "mt1", "mt2", "ip", "im", "ms", "imt", "s", "s1", "c", "p" });

		public void UpdateState(List<UsfmToken> tokenList, int tokenIndex)
		{
			CharTag = null;
			ParaStart = false;

			if (NoteMarkers.Contains(tokenList[tokenIndex].Marker))
			{
				NoteTag = new ScrTag();
				_endMarker = tokenList[tokenIndex].EndMarker;
				//ParaTag = null;
			}

			if (NoteTag != null && tokenList[tokenIndex].Marker == _endMarker)
			{
				NoteTag = null;
				_endMarker = null;
			}

			if (ParaMarkers.Contains(tokenList[tokenIndex].Marker))
			{
				ParaTag = new ScrTag { Marker = tokenList[tokenIndex].Marker };
				ParaStart = true;
				NoteTag = null;
			}

		}

		public ScrTag NoteTag { get; private set; }
		public ScrTag CharTag { get; private set; }
		public ScrTag ParaTag { get; internal set; }
		public bool ParaStart { get; private set; }
	}
}
