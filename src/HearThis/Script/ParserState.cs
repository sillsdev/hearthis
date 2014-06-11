using System.Collections.Generic;
using System.Linq;
using Paratext;

namespace HearThis.Script
{
	/// <summary>
	/// This is the real implementation of IScrParserState which implements the interface by
	/// wrapping a real ScrParserState.
	/// </summary>
	class ParserState : IScrParserState
	{
		private readonly ScrParserState _parserState;
		private readonly ScrTag _chapterScrTag;
		private bool _useChapterTag;

		public ParserState(ScrParserState scrParserState)
		{
			_parserState = scrParserState;
			var chapter = _parserState.ScrStylesheet.Tags.First(t => t.Marker == "c");
			_chapterScrTag = new ScrTag {
				Marker = chapter.Marker,
				Name = chapter.Name,
				Bold = chapter.Bold,
				Color = chapter.Color,
				ColorValue = chapter.ColorValue,
				Fontname = chapter.Fontname,
				FontSize = chapter.FontSize,
				Italic = chapter.Italic,
				TextProperties = chapter.TextProperties,
			};
		}

		#region IScrParserState Members

		public void UpdateState(List<UsfmToken> tokenList, int tokenIndex)
		{
			_parserState.UpdateState(tokenList, tokenIndex);
			_useChapterTag = tokenList[tokenIndex].Marker == _chapterScrTag.Marker;
		}

		public ScrTag NoteTag
		{
			get { return _parserState.NoteTag; }
		}

		public ScrTag CharTag
		{
			get { return _parserState.CharTag; }
		}

		public ScrTag ParaTag
		{
			get { return _useChapterTag ? _chapterScrTag : _parserState.ParaTag; }
		}

		public bool ParaStart
		{
			get { return _parserState.ParaStart; }
		}

		public bool IsPublishableVernacular
		{
			get { return _parserState.IsPublishableVernacular; }
		}
		#endregion
	}
}
