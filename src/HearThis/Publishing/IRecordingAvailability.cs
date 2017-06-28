using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearThis.Publishing
{
	/// <summary>
	/// A thin wrapper around the ClipRepository functions, to allow testing.
	/// </summary>
	public interface IRecordingAvailability
	{
		bool GetHaveClipUnfiltered(string projectName, string bookName, int chapterNumber1Based, int lineNumberZeroBased);
	}
}
