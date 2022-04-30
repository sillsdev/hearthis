// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2022' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
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
