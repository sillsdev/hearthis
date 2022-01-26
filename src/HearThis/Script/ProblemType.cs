// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2020' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;

namespace HearThis.Script
{
	[Flags]
	public enum ProblemType
	{
		None = 0,
		Ignored = None, // Same as NOT setting the Unresolved flag, but makes it easier to write readable code.
		Unresolved = 1,
		Major = 2,
		RecordingWithNoInfo = 4,
		/// <summary>
		/// Currently, extra clips can be fixed, but not ignored, so we can define them as Unresolved.
		/// Once they are fixed, they simply go away.
		/// </summary>
		ExtraRecordings = 8 | Major | Unresolved,
		TextChange = 16 | Major,
	}

	public static class ProblemTypeExtensions
	{
		public static bool NeedsAttention(this ProblemType problem)
		{
			return (problem & (ProblemType.Major | ProblemType.Unresolved)) == (ProblemType.Major | ProblemType.Unresolved);
		}
	}
}
