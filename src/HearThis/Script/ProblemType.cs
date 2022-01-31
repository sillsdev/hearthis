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
		Resolved = None, // Same as NOT setting the Unresolved flag, but makes code more readable.

		// The following problem types should be in ascending order of severity
		RecordingWithNoInfo = 2,
		Major = 4,
		// No need to specify other bits to distinguish between TextChange and ExtraRecordings
		// because we treat them as equally severe.
		TextChange = Major,
		/// <summary>
		/// Currently, extra clips can be fixed, but not ignored, so we can define them as Unresolved.
		/// Once they are fixed, they simply go away.
		/// </summary>
		ExtraRecordings = Major | Unresolved,

		// Unresolved problems are always 
		Unresolved = 128,
	}

	public static class ProblemTypeExtensions
	{
		public static bool NeedsAttention(this ProblemType problem)
		{
			return (problem & (ProblemType.Major | ProblemType.Unresolved)) == (ProblemType.Major | ProblemType.Unresolved);
		}
	}
}
