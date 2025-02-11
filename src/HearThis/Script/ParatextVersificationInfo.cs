// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014-2025, SIL Global.
// <copyright from='2014' to='2025' company='SIL Global'>
//		Copyright (c) 2014-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using SIL.Scripture;

namespace HearThis.Script
{
	class ParatextVersificationInfo : BibleStatsBase, IBibleStats
	{
		private readonly ScrVers _scrVers;

		public ParatextVersificationInfo(ScrVers scrVers)
		{
			_scrVers = scrVers;
		}

		public int GetChaptersInBook(int bookNumber0Based)
		{
			return _scrVers.GetLastChapter(bookNumber0Based + 1);
		}
	}
}
