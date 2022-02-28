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
using System.Text;
using static Icu.Character;

namespace HearThis.StringDifferences
{
	/// <summary>
	/// Class to compare two strings, one representing the "original" version of a text and the
	/// other representing the "new" version, and provide the results as a sequence of segments
	/// where the text of each segment is identified as an "Addition" to a "Deletion" from or the
	/// "Same" as the text of the other string. The number of segments computed is not guaranteed
	/// to be the same in both sets of results, but the number of "Same" segments will always be
	/// identical and their relative orders will correspond to each other. For example, comparing
	/// strings ABCXYZ and XYZABC would result in only one Same segment -- along with a Deletion
	/// from the old and an Addition to the new -- not two Same segments in inverted order.
	/// </summary>
	/// <remarks>This is intended for relatively short text strings. Currently the algorithm limits
	/// the total number of segments to five, so for very large strings with lots of small
	/// differences, that might not be enough to pinpoint useful differences. If two strings
	/// are almost completely different, the current algorithm identifies even a single character
	/// as a matching (Same) segment. With the possible exception of very short strings in an
	/// ideographic script, this is probably not useful, so there is room for some enhancement. In
	/// practice, it is much less likely to be helpful to need the aid of a computer to find
	/// differences in very short strings.
	/// Note also that since this is a two-way diff, we have no knowledge of a common base version
	/// and cannot therefore say in any real historical sense whether a change was an addition or a
	/// deletion. 
	/// </remarks>
	public class StringDifferenceFinder
	{
		public List<StringDifferenceSegment> OriginalStringDifferences { get; } = new List<StringDifferenceSegment>();
		public List<StringDifferenceSegment> NewStringDifferences { get; } = new List<StringDifferenceSegment>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="origStr">The string representing the "before" state of the text</param>
		/// <param name="newStr">The string representing the "after" state of the text</param>
		/// <remarks>
		/// The algorithm for computing differences works with strings that are normalized
		/// either as composed or decomposed. For efficiency it expects them to both be
		/// composed or both be decomposed and does not check this or convert them to ensure
		/// this condition is met.
		/// This constructor could wait to compute the differences lazily
		/// when first asked for the results, but in practice, the caller always
		/// wants the results, so it is simpler to just go ahead and compute it
		/// now rather than keeping track of whether is has already been computed.</remarks>
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

		private enum CharactersToKeepTogether
		{
			None = 0,
			PrecedingBaseCharacter = 1,
			PrecedingAndFollowingBaseCharacter = 2,
			AllCharactersInWord = Int32.MaxValue,
		}

		private static CharactersToKeepTogether GetCharactersToKeepWith(int ch)
		{
			// ENHANCE: Implement correct logic for Hangul syllables

			if (ch == '\u200d') // Zero-width Joiner
				return CharactersToKeepTogether.PrecedingAndFollowingBaseCharacter;

			switch (GetCharType(ch))
			{
				// REVIEW: It's possible we'd want to treat modifier letters in a similar fashion, but
				// they do not form combined graphemes, and can sometimes modify the following letter
				// rather than the preceding one.
				//case UCharCategory.MODIFIER_LETTER:
				//	break;
				case UCharCategory.NON_SPACING_MARK:
					switch (GetIntPropertyValue(ch, UProperty.CANONICAL_COMBINING_CLASS))
					{
						// TODO: Write unit tests and handle additional combining classes.
						// TODO: case 1: (Note that a few of these are most likely to be used in Latin script and could return
						// PrecedingBaseCharacter, but most are for complex Asian scripts, so some other character is likely to
						// return AllCharactersInWord. I'm leaning toward PrecedingBaseCharacter, but it might be good to find
						// some real examples.
						case 6: // Han Reading Combining Class (Vietnamese alternate reading marks)
						// TODO:	case 7:
						// TODO:	case 8:
						// TODO: 10...199 (fixed position class)
						case 9: // Virama
							return CharactersToKeepTogether.AllCharactersInWord;
						case 233: // Double_Below
						case 234: // Double_Above
							return CharactersToKeepTogether.PrecedingAndFollowingBaseCharacter;
					}
					return CharactersToKeepTogether.PrecedingBaseCharacter;
				// REVIEW: These are seldom (if ever) used in normal (e.g., Scripture) text. Given
				// the complexity in figuring out the number of base characters, possibly best to
				// not support them at all here. But if they are used in a complex script where
				// they are likely to apply to more than one "base" character, it will probably
				// be the case that one of those characters will return AllCharactersInWord, and
				// then it won't matter.
				case UCharCategory.ENCLOSING_MARK: // See http://unicode.org/L2/L2003/03026-enclosing-marks.htm
					return CharactersToKeepTogether.PrecedingBaseCharacter;
				case UCharCategory.COMBINING_SPACING_MARK:
					return CharactersToKeepTogether.AllCharactersInWord;
			}

			return 0;
		}

		private static bool HasReasonableWordBreaks(string s, List<char> whitespaceChars)
		{
			var wsCharCount = 0;
			foreach (var c in s)
			{
				if (char.IsWhiteSpace(c))
				{
					wsCharCount++;
					if (!whitespaceChars.Contains(c))
						whitespaceChars.Add(c);
				}
			}

			return wsCharCount > .08 * s.Length;
		}

		/// <summary>
		/// Works through the two strings to find segments that are the same and different.
		/// </summary>
		private IEnumerable<StringDifferenceSegment> ComputeDifferences(string origStr, string newStr)
		{
			int lastDiffOrigEnd = 0;
			var whitespaceChars = new List<char>();
			if (HasReasonableWordBreaks(origStr, whitespaceChars) || HasReasonableWordBreaks(newStr, whitespaceChars))
			{
				int lastDiffNewEnd = 0;
				var result = DiffPlex.Differ.Instance.CreateWordDiffs(origStr, newStr, false, false, whitespaceChars.ToArray());
				foreach (var diffBlock in result.DiffBlocks)
				{
					if (diffBlock.DeleteStartA > lastDiffOrigEnd)
						yield return new StringDifferenceSegment(DifferenceType.Same, origStr.Substring(lastDiffOrigEnd, diffBlock.DeleteStartA));

					var sb = new StringBuilder();

					if (diffBlock.DeleteCountA > 0)
					{
						for (int w = 0; w < diffBlock.DeleteCountA; w++)
						{
							while (!whitespaceChars.Contains(origStr[++lastDiffOrigEnd]))
								sb.Append(origStr[lastDiffOrigEnd]);
							while (whitespaceChars.Contains(origStr[lastDiffOrigEnd]))
								sb.Append(origStr[lastDiffOrigEnd++]);
						}
						yield return new StringDifferenceSegment(DifferenceType.Deletion, sb.ToString());
					}

					if (diffBlock.InsertCountB > 0)
					{
						sb = new StringBuilder();
						for (int w = 0; w < diffBlock.InsertCountB; w++)
						{
							while (!whitespaceChars.Contains(newStr[++lastDiffNewEnd]))
								sb.Append(newStr[lastDiffNewEnd]);
							while (whitespaceChars.Contains(newStr[lastDiffNewEnd]))
								sb.Append(newStr[lastDiffNewEnd++]);
						}
						yield return new StringDifferenceSegment(DifferenceType.Addition, sb.ToString());
					}
				}
				if (lastDiffOrigEnd < origStr.Length)
					yield return new StringDifferenceSegment(DifferenceType.Same, origStr.Substring(lastDiffOrigEnd));
			}
			else
			{
				var result = DiffPlex.Differ.Instance.CreateCharacterDiffs(origStr, newStr, false, false);
				foreach (var diffBlock in result.DiffBlocks)
				{
					int o = 0;
					//int n = 0;

					int ch = origStr[diffBlock.DeleteStartA];
					//if (IsHighSurrogate((char)ch))
					//	ch = ConvertToUtf32(origStr, diffBlock.DeleteStartA);
					//else if (IsLowSurrogate((char)ch))
					//	ch = ConvertToUtf32(origStr, diffBlock.DeleteStartA - ++o);

					if (diffBlock.DeleteCountA > 0 && GetCharactersToKeepWith(ch) != CharactersToKeepTogether.None)
						o++;
					else if (diffBlock.InsertCountB > 0)
					{
						ch = newStr[diffBlock.InsertStartB];
						if (GetCharactersToKeepWith(ch) != CharactersToKeepTogether.None)
							o++;
					}

					if (diffBlock.DeleteStartA > lastDiffOrigEnd)
						yield return new StringDifferenceSegment(DifferenceType.Same, origStr.Substring(lastDiffOrigEnd, diffBlock.DeleteStartA - o));

					if (diffBlock.DeleteCountA > 0)
						yield return new StringDifferenceSegment(DifferenceType.Deletion, origStr.Substring(diffBlock.DeleteStartA - o, diffBlock.DeleteCountA));

					if (diffBlock.InsertCountB > 0)
						yield return new StringDifferenceSegment(DifferenceType.Addition, newStr.Substring(diffBlock.InsertStartB - o, diffBlock.InsertCountB));

					lastDiffOrigEnd = diffBlock.DeleteStartA + diffBlock.DeleteCountA;
				}

				if (lastDiffOrigEnd < origStr.Length)
					yield return new StringDifferenceSegment(DifferenceType.Same, origStr.Substring(lastDiffOrigEnd));
			}
		}
	}
}
