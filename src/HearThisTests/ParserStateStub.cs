using System.Collections.Generic;
using HearThis.Script;
using Paratext;

namespace HearThisTests
{
	internal class ParserStateStub : IScrParserState
	{
		private string _endMarker = null;
		public HashSet<string> NoteMarkers = new HashSet<string>(new[] { "nt", "nt1", "ft" });
		public HashSet<string> ParaMarkers = new HashSet<string>(new[] { "mt", "mt1", "mt2", "ip", "im", "ms", "imt", "q1", "s", "s1", "c", "p" });

		public void UpdateState(List<UsfmToken> tokenList, int tokenIndex)
		{
			CharTag = null;
			ParaStart = false;

			var marker = tokenList[tokenIndex].Marker;

			if (NoteMarkers.Contains(marker))
			{
				NoteTag = new ScrTag();
				_endMarker = tokenList[tokenIndex].EndMarker;
			}

			if (NoteTag != null && marker == _endMarker)
			{
				NoteTag = null;
				_endMarker = null;
			}

			if (ParaMarkers.Contains(marker))
			{
				ParaTag = new ScrTag { Marker = marker };
				if (ParaTag.Marker == "c")
				{
					ParaTag.AddTextProperty(TextProperties.scChapter);
				}
				else
				{
					ParaTag.AddTextProperty(TextProperties.scParagraph);
					ParaTag.AddTextProperty(TextProperties.scPublishable);
					ParaTag.AddTextProperty(TextProperties.scVernacular);
				}
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
