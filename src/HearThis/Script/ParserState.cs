// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global. All Rights Reserved.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using Paratext.Data;

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
			_chapterScrTag = _parserState.ScrStylesheet.Tags.First(t => t.Marker == "c");
		}

		#region IScrParserState Members

		public void UpdateState(List<UsfmToken> tokenList, int tokenIndex)
		{
			_parserState.UpdateState(tokenList, tokenIndex);
			_useChapterTag = tokenList[tokenIndex].Marker == _chapterScrTag.Marker;
		}

		public ScrTag NoteTag => _parserState.NoteTag;

		public ScrTag CharTag => _parserState.CharTag;

		public ScrTag ParaTag => _useChapterTag ? _chapterScrTag : _parserState.ParaTag;

		public bool ParaStart => _parserState.ParaStart;

		public bool IsPublishable => _parserState.IsPublishable && !_parserState.IsFigure;

		#endregion
	}
}
