using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearThis.Publishing;

namespace HearThis
{
	public class RecordingAvailability : IRecordingAvailability
	{
		public bool GetHaveClipUnfiltered(string projectName, string bookName, int chapterNumber1Based, int lineNumberZeroBased)
		{
			return ClipRepository.GetHaveClipUnfiltered(projectName, bookName, chapterNumber1Based, lineNumberZeroBased);
		}
	}
}
