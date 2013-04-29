using System.Collections.Generic;
using Paratext;

namespace HearThis.Script
{
	/// <summary>
	/// This is the real implementation of IScrParserState which implements the interface by
	/// wrapping a real ScrParserState.
	/// </summary>
	class ParserState : IScrParserState
	{
		private ScrParserState _parserState;

		public ParserState(ScrParserState scrParserState)
		{
			_parserState = scrParserState;
		}

		#region IScrParserState Members

		public void UpdateState(List<UsfmToken> tokenList, int tokenIndex)
		{
			_parserState.UpdateState(tokenList, tokenIndex);
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
			get { return _parserState.ParaTag; }
		}

		public bool ParaStart
		{
			get { return _parserState.ParaStart; }
		}

		#endregion
	}
}
