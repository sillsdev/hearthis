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
		public HashSet<string> ParaMarkers = new HashSet<string>(new[] { "mt", "mt1", "mt2", "ip", "im", "ms", "imt", "q1", "s", "s1", "c", "cl", "cp", "p", "h", "r", "toc1" });
		public HashSet<string> ParaMarkersNonReadable = new HashSet<string>(new[] { "rem" });

		public HashSet<string> CharMarkers = new HashSet<string>(new[] { "bk", "fig", "rq" });

		public void UpdateState(List<UsfmToken> tokenList, int tokenIndex)
		{
			ParaStart = false;
			IsPublishableVernacular = false;

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
				switch (ParaTag.Marker)
				{
					case "c":
						AddTextProperty(ParaTag, TextProperties.scChapter);
						break;
					case "cl": // fall through
					case "cp":
						AddTextProperty(ParaTag, TextProperties.scParagraph);
						AddTextProperty(ParaTag, TextProperties.scPublishable);
						IsPublishableVernacular = true;
						break;
					default:
						AddTextProperty(ParaTag, TextProperties.scParagraph);
						AddTextProperty(ParaTag, TextProperties.scPublishable);
						AddTextProperty(ParaTag, TextProperties.scVernacular);
						IsPublishableVernacular = true;
						ParaStart = true;
						break;
				}
				NoteTag = null;
			}

			if (ParaMarkersNonReadable.Contains(marker))
			{
				ParaTag = new ScrTag { Marker = marker };
				AddTextProperty(ParaTag, TextProperties.scParagraph);
				AddTextProperty(ParaTag, TextProperties.scNonpublishable);
				ParaStart = true;
				NoteTag = null;
			}
		}

		public ScrTag NoteTag { get; private set; }
		public ScrTag CharTag { get; private set; }
		public ScrTag ParaTag { get; internal set; }
		public bool ParaStart { get; private set; }
		public bool IsPublishableVernacular { get; private set; }

		private void AddTextProperty(ScrTag srcTag, TextProperties property)
		{
			srcTag.TextProperties = srcTag.TextProperties | property;
		}
	}
}
