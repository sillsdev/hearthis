﻿// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2017-2025, SIL Global.
// <copyright from='2017' to='2025' company='SIL Global'>
//		Copyright (c) 2017-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
namespace HearThis.Publishing
{
	/// <summary>
	/// A thin wrapper around the ClipRepository functions, to allow testing.
	/// </summary>
	public interface IRecordingAvailability
	{
		bool HasClipUnfiltered(string projectName, string bookName, int chapterNumber1Based, int lineNumberZeroBased);
	}
}
