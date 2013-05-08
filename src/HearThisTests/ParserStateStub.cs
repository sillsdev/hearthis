using System.Collections.Generic;
using HearThis.Script;
using Paratext;

namespace HearThisTests
{
	internal class ParserStateStub : IScrParserState
	{
		private string _endNoteMarker = null;
		private string _endCharMarker = null;
		public HashSet<string> NoteMarkers = new HashSet<string>(new[] { "nt", "nt1", "ft" });
		public HashSet<string> ParaMarkers = new HashSet<string>(new[] { "mt", "mt1", "mt2", "ip", "im", "ms", "imt", "q1", "s", "s1", "c", "p", "h" });
		public HashSet<string> ParaMarkersNonReadable = new HashSet<string>(new[] { "rem" });

		public HashSet<string> CharMarkers = new HashSet<string>(new[] { "bk", "fig" });

		public void UpdateState(List<UsfmToken> tokenList, int tokenIndex)
		{
			ParaStart = false;

			var marker = tokenList[tokenIndex].Marker;

			if (CharMarkers.Contains(marker))
			{
				CharTag = new ScrTag { Marker = marker };
				_endCharMarker = marker + "*";
			}

			if (CharTag != null && marker == _endCharMarker)
			{
				CharTag = null;
				_endCharMarker = null;
			}

			if (NoteMarkers.Contains(marker))
			{
				NoteTag = new ScrTag { Marker = marker };
				_endNoteMarker = tokenList[tokenIndex].EndMarker;
			}

			if (NoteTag != null && marker == _endNoteMarker)
			{
				NoteTag = null;
				_endNoteMarker = null;
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

			if (ParaMarkersNonReadable.Contains(marker))
			{
				ParaTag = new ScrTag { Marker = marker };
				ParaTag.AddTextProperty(TextProperties.scParagraph);
				ParaTag.AddTextProperty(TextProperties.scNonpublishable);
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
