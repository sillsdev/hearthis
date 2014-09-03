// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
namespace HearThis.Script
{
	public interface ISkippedStyleInfoProvider
	{
		void SetSkippedStyle(string style, bool skipped);
		bool IsSkippedStyle(string style);
	}
}