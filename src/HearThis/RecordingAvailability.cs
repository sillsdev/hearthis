// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022-2025, SIL Global.
// <copyright from='2017' to='2025' company='SIL Global'>
//		Copyright (c) 2022-2025, SIL Global.
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
