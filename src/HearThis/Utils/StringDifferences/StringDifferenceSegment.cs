// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022-2025, SIL Global.
// <copyright from='2022' to='2025' company='SIL Global'>
//		Copyright (c) 2022-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;

namespace HearThis.StringDifferences
{
	public enum DifferenceType
	{
		Same,
		Addition,
		Deletion,
	}

	public class StringDifferenceSegment
	{
		public DifferenceType Type { get; }
		public String Text { get; }

		public StringDifferenceSegment(DifferenceType type, string text)
		{
			Type = type;
			Text = text;
		}
	}
}
