// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
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

		public bool IsPublishable
		{
			get { return _parserState.IsPublishable && !_parserState.IsFigure; }
		}
		#endregion
	}
}
