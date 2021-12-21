using System.Collections.Generic;
using System.Linq;
using static System.Int32;

namespace HearThis.Script
{
	public abstract class ChapterRecordingInfoBase
	{
		public abstract IReadOnlyList<ScriptLine> RecordingInfo { get; }

		public void AdjustLineNumbers(int blockNumberOfStartingShiftedClip0Based, int shiftedBy, int blockCount = MaxValue,
			bool preserveModifiedTime = false)
		{
			// Note: ScriptLine.Number is 1-based, not 0-based
			foreach (var recordingInfo in RecordingInfo
				.SkipWhile(i => i.Number <= blockNumberOfStartingShiftedClip0Based)
				.Take(blockCount))
			{
				recordingInfo.Number += shiftedBy;
			}

			Save(preserveModifiedTime);
		}

		public abstract void OnScriptBlockRecorded(ScriptLine selectedScriptBlock);

		public abstract void Save(bool preserveModifiedTime = false);
	}
}
