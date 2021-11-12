// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------

using System;

namespace HearThis.Script
{
	public class ExtraRecordingInfo
	{
		public ScriptLine RecordingInfo { get; }
		public string ClipFile { get; }

		public ExtraRecordingInfo(string file, ScriptLine recordingInfo)
		{
			ClipFile = file ?? throw new ArgumentNullException(nameof(file));
			RecordingInfo = recordingInfo;
		}
	}
}
