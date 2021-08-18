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

namespace HearThis.Script
{
	public interface ISkippedStyleInfoProvider
	{
		void SetSkippedStyle(string style, bool skipped);
		bool IsSkippedStyle(string style);
		IReadOnlyList<string> StylesToSkipByDefault { get; }
	}
}
