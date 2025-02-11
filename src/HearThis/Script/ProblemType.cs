// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020-2025, SIL Global.
// <copyright from='2020' to='2025' company='SIL Global'>
//		Copyright (c) 2020-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;

namespace HearThis.Script
{
	/// <summary>
	/// Types of problems that can be detected and dealt with in the "Check for Problems" tool.
	/// </summary>
	[Flags]
	public enum ProblemType
	{
		None = 0,
		Resolved = None, // Same as NOT setting the Unresolved flag, but makes code more readable.

		// The following problem types should be in ascending order of severity
		/// <summary>
		/// A clip that was recorded before HearThis started saving info about the script at the
		/// time of recording. This is just a placeholder and not currently used because we have
		/// decided not to show this as a "problem" on the book and chapter buttons or on the
		/// slider. We just detect it on the fly if the user selects the block.
		/// </summary>
		RecordingWithNoInfo = 2,
		Major = 4,
		// No need to specify other bits to distinguish between TextChange and ExtraRecordings
		// because we treat them as equally severe.
		TextChange = Major,
		/// <summary>
		/// Currently, extra clips and clips for skipped blocks can be fixed, but not ignored,
		/// so we can define them as Unresolved. Once they are fixed, they simply go away.
		/// </summary>
		ExtraRecordings = Major | Unresolved,
		ClipForSkippedBlock = Major | Unresolved,

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
