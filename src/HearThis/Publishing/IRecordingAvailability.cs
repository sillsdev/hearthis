// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2017' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
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
