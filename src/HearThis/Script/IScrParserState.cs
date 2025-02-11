// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using Paratext.Data;

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
		/// <summary>
		/// The implementation of this property should ensure that figures are not considered "publishable" because in an audio recording they aren't.
		/// </summary>
		bool IsPublishable { get; }
	}
}
