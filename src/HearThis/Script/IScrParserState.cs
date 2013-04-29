using System.Collections.Generic;
using Paratext;

namespace HearThis.Script
{
	/// <summary>
	///
	/// </summary>
	public interface IScrParserState
	{
		void UpdateState(List<UsfmToken> tokenList, int tokenIndex);
		ScrTag NoteTag { get; }
		ScrTag CharTag { get; }
		ScrTag ParaTag { get; }
		bool ParaStart { get; }
	}
}