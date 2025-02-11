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

namespace HearThis.Script
{
	public interface ISkippedStyleInfoProvider
	{
		void SetSkippedStyle(string style, bool skipped);
		bool IsSkippedStyle(string style);
		IReadOnlyList<string> StylesToSkipByDefault { get; }
	}
}
