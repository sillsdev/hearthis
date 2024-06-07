// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2017' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using HearThis.Publishing;

namespace HearThis
{
	public class RecordingAvailability : IRecordingAvailability
	{
		public bool HasClipUnfiltered(string projectName, string bookName, int chapterNumber1Based, int lineNumberZeroBased)
		{
			return ClipRepository.HasClipUnfiltered(projectName, bookName, chapterNumber1Based, lineNumberZeroBased);
		}
	}
}
