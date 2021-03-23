using System.Collections.Generic;
using HearThis.Script;
using Paratext.Data;

namespace HearThisTests
{
	internal class ParserStateStub : IScrParserState
	{
		private string _endNoteMarker = null;
		private string _endCharMarker = null;
		public readonly HashSet<string> NoteMarkers = new HashSet<string>(new[] { "nt", "nt1", "ft" });
		public readonly HashSet<string> ParaMarkers = new HashSet<string>(new[] { "mt", "mt1", "mt2", "ip", "im", "ms", "imt", "q1", "s", "s1", "c", "cl", "cp", "p", "h", "r", "toc1", "toc2", "toc3", "toca1", "toca2", "toca3" });
		public readonly HashSet<string> ParaMarkersNonReadable = new HashSet<string>(new[] { "rem" });

		public readonly HashSet<string> CharMarkers = new HashSet<string>(new[] { "bk", "fig", "rq", "tl" });

		public void UpdateState(List<UsfmToken> tokenList, int tokenIndex)
		{
			ParaStart = false;
			IsPublishable = false;

			var marker = tokenList[tokenIndex].Marker;

			if (CharMarkers.Contains(marker))
			{
				CharTag = new ScrTag(marker);
				_endCharMarker = marker + "*";
			}

			if (CharTag != null && marker == _endCharMarker)
			{
				CharTag = null;
				_endCharMarker = null;
			}

			if (NoteMarkers.Contains(marker))
			{
				NoteTag = new ScrTag(marker);
				_endNoteMarker = tokenList[tokenIndex].EndMarker;
			}

			if (NoteTag != null && marker == _endNoteMarker)
			{
				NoteTag = null;
				_endNoteMarker = null;
			}

			if (ParaMarkers.Contains(marker))
			{
				ParaTag = new ScrTag(marker);
				switch (ParaTag.Marker)
				{
					case "c":
						AddTextProperty(ParaTag, TextProperties.scChapter);
						break;
					case "cl": // fall through
					case "cp":
						AddTextProperty(ParaTag, TextProperties.scParagraph);
						AddTextProperty(ParaTag, TextProperties.scPublishable);
						IsPublishable = true;
						break;
					default:
						AddTextProperty(ParaTag, TextProperties.scParagraph);
						AddTextProperty(ParaTag, TextProperties.scPublishable);
						AddTextProperty(ParaTag, TextProperties.scVernacular); // This is not really required, but it corresponds to what would be in usfm.sty
						IsPublishable = true;
						ParaStart = true;
						break;
				}
				NoteTag = null;
			}

			if (ParaMarkersNonReadable.Contains(marker))
			{
				ParaTag = new ScrTag(marker);
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
		public bool IsPublishable { get; private set; }

		private void AddTextProperty(ScrTag srcTag, TextProperties property)
		{
			srcTag.TextProperties = srcTag.TextProperties | property;
		}
	}
}
