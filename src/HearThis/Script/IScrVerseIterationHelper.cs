// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023-2025, SIL Global.
// <copyright from='2023' to='2025' company='SIL Global'>
//		Copyright (c) 2023-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------

using SIL.Scripture;

namespace HearThis.Script
{
	public interface IScrVerseIterationHelper
	{
		bool TryGetPreviousVerse(BCVRef verse, out BCVRef previousVerse);
		bool TryGetNextVerse(BCVRef verse, out BCVRef nextVerse);
	}
}
