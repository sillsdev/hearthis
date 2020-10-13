// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
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
