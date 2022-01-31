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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using SIL.Extensions;

namespace HearThis.StringDifferences
{
	public class StringDifferenceFinder
	{
		public List<StringDifferenceSegment> OriginalStringDifferences { get; } = new List<StringDifferenceSegment>();
		public List<StringDifferenceSegment> NewStringDifferences { get; } = new List<StringDifferenceSegment>();

		public StringDifferenceFinder(string origStr, string newStr)
		{
			if (string.IsNullOrEmpty(origStr))
				throw new ArgumentException("String must not be null or empty", nameof(origStr));
			if (string.IsNullOrEmpty(newStr))
				throw new ArgumentException("String must not be null or empty", nameof(newStr));
			foreach (var segment in ComputeDifferences(origStr, newStr))
			{
				switch (segment.Type)
				{
					case DifferenceType.Same:
						OriginalStringDifferences.Add(segment);
						NewStringDifferences.Add(segment);
						break;
					case DifferenceType.Addition:
						NewStringDifferences.Add(segment);
						break;
					case DifferenceType.Deletion:
						OriginalStringDifferences.Add(segment);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private IEnumerable<StringDifferenceSegment> ComputeDifferences(string origStr, string newStr)
		{
			StringBuilder bldr = new StringBuilder();
			int o = 0;
			int n = 0;
			while (o < origStr.Length && n < newStr.Length)
			{
				if (origStr[o] == newStr[n])
				{
					bldr.Append(origStr[o++]);
					n++;
				}
				else
					break;
			}

			if (bldr.Length > 0)
				yield return new StringDifferenceSegment(DifferenceType.Same, bldr.ToString());

			origStr = origStr.Substring(o);
			newStr = newStr.Substring(n);

			if (origStr.Length == 0)
			{
				if (newStr.Length > 0)
					yield return new StringDifferenceSegment(DifferenceType.Addition, newStr);
				yield break;
			}
			if (newStr.Length == 0)
			{
				yield return new StringDifferenceSegment(DifferenceType.Deletion, origStr);
				yield break;
			}

			bldr.Clear();

			o = origStr.Length - 1;
			n = newStr.Length - 1;
			while (o >= 0 && n >= 0)
			{
				if (origStr[o] == newStr[n])
				{
					bldr.Insert(0, origStr[o--]);
					n--;
				}
				else
					break;
			}

			var common = origStr.Substring(0, origStr.Length - bldr.Length).GetLongestUsefulCommonSubstring(
				newStr.Substring(0, newStr.Length - bldr.Length), out _, .20);

			if (common.Length > 1)
			{
				var iOrig = origStr.IndexOf(common);
				var iNew = newStr.IndexOf(common);
				if (iNew < iOrig)
					ExpandCommonSubstringIfNeeded(ref iNew, ref iOrig, ref common, newStr, origStr);
				else
					ExpandCommonSubstringIfNeeded(ref iOrig, ref iNew, ref common, origStr, newStr);

				if (iOrig > 0)
					yield return new StringDifferenceSegment(DifferenceType.Deletion, origStr.Substring(0, iOrig));
				
				if (iNew > 0)
					yield return new StringDifferenceSegment(DifferenceType.Addition, newStr.Substring(0, iNew));

				yield return new StringDifferenceSegment(DifferenceType.Same, common);

				var iDelStart = iOrig + common.Length;
				var delLength = origStr.Length - bldr.Length - iDelStart;
				if (delLength > 0)
					yield return new StringDifferenceSegment(DifferenceType.Deletion, origStr.Substring(iDelStart, delLength));

				var iAddStart = iNew + common.Length;
				var addLength = newStr.Length - bldr.Length - iAddStart;
				if (addLength > 0)
					yield return new StringDifferenceSegment(DifferenceType.Addition, newStr.Substring(iAddStart, addLength));
			}
			else
			{
				if (o >= 0)
					yield return new StringDifferenceSegment(DifferenceType.Deletion, origStr.Substring(0, o + 1));
				if (n >= 0)
					yield return new StringDifferenceSegment(DifferenceType.Addition, newStr.Substring(0, n + 1));
			}

			if (bldr.Length > 0)
				yield return new StringDifferenceSegment(DifferenceType.Same, bldr.ToString());
		}

		private void ExpandCommonSubstringIfNeeded(ref int iShort, ref int iLong, ref string common,
			string shortStr, string longStr)
		{
			Debug.Assert(iShort <= iLong);
			if (shortStr.Substring(0, iShort) == longStr.Substring(iLong - iShort, iShort))
			{
				common = common.Insert(0, shortStr.Substring(0, iShort));
				iLong -= iShort;
				iShort = 0;
			}

			var shortRemaining = shortStr.Substring(iShort + common.Length);
			if (longStr.Length >= iLong + common.Length + shortRemaining.Length &&
				shortRemaining == longStr.Substring(iLong + common.Length, shortRemaining.Length))
			{
				common += shortRemaining;
			}
		}
	}
}
