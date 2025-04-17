using System;
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

		/// <summary>
		/// This method is called when a script block is recorded, restored from a backup, or moved
		/// into a new position.
		/// </summary>
		/// <param name="scriptBlock">The script block whose recorded clip file was just recorded,
		/// restored, etc.</param>
		/// <param name="exceptionHandlerOverride">If there is a need to handle exceptions in a
		/// manner other than the default behavior (which should be described by specific
		/// implementers), the caller should set this parameter, providing a function that handles
		/// the exception appropriately and then returns <c>true</c> to indicate that no further
		/// handling is needed or <c>false</c> to request that the default handling also be
		/// performed.</param>
		public abstract void OnScriptBlockRecorded(ScriptLine scriptBlock,
			Func<Exception, bool> exceptionHandlerOverride = null);

		public abstract void Save(bool preserveModifiedTime = false);
	}
}
