// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International. All Rights Reserved.
// <copyright from='2023' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International. All Rights Reserved.
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
