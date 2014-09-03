// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using Paratext;

namespace HearThis.Script
{
	/// <summary>
	/// This exposes the things we care about out of ScrParserState, providing an
	/// anti-corruption layer between Paratext and HearThis and allowing us to test the code
	/// that calls Paratext.
	/// </summary>
	public interface IScrParserState
	{
		void UpdateState(List<UsfmToken> tokenList, int tokenIndex);
		ScrTag NoteTag { get; }
		ScrTag CharTag { get; }
		ScrTag ParaTag { get; }
		bool ParaStart { get; }
		bool IsPublishableVernacular { get; }
	}
}