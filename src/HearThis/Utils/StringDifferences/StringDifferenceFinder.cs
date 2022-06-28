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
using System.Linq;
using System.Text;
using SIL.Extensions;
using SIL.Reporting;
using static Icu.Character;
using static System.Char;

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
			try
			{
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
			catch (Exception e)
			{
				Logger.WriteError(e);
				ErrorReport.NotifyUserOfProblem(e, $"A problem occurred trying to compute differences between {origStr} and {newStr} (HT-441). Please report this.");
				OriginalStringDifferences.Clear();
				OriginalStringDifferences.Add(new StringDifferenceSegment(DifferenceType.Deletion, origStr));
				NewStringDifferences.Clear();
				OriginalStringDifferences.Add(new StringDifferenceSegment(DifferenceType.Addition, newStr));
			}
		}

		private enum CharactersToKeepTogether
		{
			None = 0,
			PrecedingBaseCharacter = 1,
			PrecedingAndFollowingBaseCharacter = 2,
			AllCharactersInWord = Int32.MaxValue,
		}

		private class CharacterComparisonState
		{
			private Func<CharactersToKeepTogether, bool> NeedToRemovePreviouslyAddedCharacter { get; set; }

			public bool KeepWholeWordsTogether { get; private set; }

			private int _charactersToRemoveIfMismatchFound;

			private bool _matched = true;

			private int _charactersInCurrentWord;

			public int CharactersToRemove => _matched ? 0 : _charactersToRemoveIfMismatchFound;

			public CharacterComparisonState(Func<CharactersToKeepTogether, bool> needToRemovePreviouslyAddedCharacter, bool keepWholeWordsTogether = false)
			{
				NeedToRemovePreviouslyAddedCharacter = needToRemovePreviouslyAddedCharacter;
				KeepWholeWordsTogether = keepWholeWordsTogether;
			}

			public void Reset(Func<CharactersToKeepTogether, bool> needToRemovePreviouslyAddedCharacter)
			{
				_matched = true;
				_charactersToRemoveIfMismatchFound = 0;
				_charactersInCurrentWord = 0;
				NeedToRemovePreviouslyAddedCharacter = needToRemovePreviouslyAddedCharacter;
			}

			public bool DoCharactersMatch(string origStr, int o, string newStr, int n, out bool surrogate)
			{
				Debug.Assert(_matched, "Once a mismatch is found, no more characters should be tested until Reset is called.");

				int chOrig = origStr[o];
				if (IsHighSurrogate((char)chOrig))
				{
					surrogate = true;
					chOrig = ConvertToUtf32(origStr, o);
				}
				else if (IsLowSurrogate((char)chOrig))
				{
					surrogate = true;
					chOrig = ConvertToUtf32(origStr, o - 1);
				}
				else
				{
					surrogate = false;
				}

				int chNew = newStr[n];
				if (IsHighSurrogate((char)chNew))
					chNew = ConvertToUtf32(newStr, n);
				else if (IsLowSurrogate((char)chNew))
					chNew = ConvertToUtf32(newStr, n - 1);

				var b = (CharactersToKeepTogether)Math.Max((int)GetCharactersToKeepWith(chOrig),
					(int)GetCharactersToKeepWith(chNew));
				KeepWholeWordsTogether |= b == CharactersToKeepTogether.AllCharactersInWord;

				_matched = chOrig == chNew;

				if (_matched)
				{
					if (origStr.IsLikelyWordForming(o))
						_charactersInCurrentWord++;
					else
						_charactersInCurrentWord = 0;

					if (!KeepWholeWordsTogether)
					{
						if (b == CharactersToKeepTogether.None)
							_charactersToRemoveIfMismatchFound = 0;
						else
							_charactersToRemoveIfMismatchFound += (int)b;
					}
				}
				else if (KeepWholeWordsTogether)
				{
					if (origStr.IsLikelyWordForming(o) || newStr.IsLikelyWordForming(n))
						_charactersToRemoveIfMismatchFound = _charactersInCurrentWord;
					else
						_charactersToRemoveIfMismatchFound = 0;
				}
				else
				{
					if (NeedToRemovePreviouslyAddedCharacter(b))
						_charactersToRemoveIfMismatchFound++;
				}

				return _matched;
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
		}

		// The "magic number" here is a percentage of characters in a string that must be
		// whitespace for a string to be considered broken up into "reasonable" sized words.
		// Current unit tests pass for percentages between 6%-13% whitespace. If this threshold
		// is set too high, strings from languages that use white-space delimited words but have
		// fairly long words will have minor diacritic or spelling changes identified character-by-
		// character instead of word-by-word (which might look weird and not be as helpful). If the
		// threshold is set too low, agglutinative and scriptio continua languages will end up
		// having very long words or entire strings marked as different, rather than helping the
		// user see more precisely where the differences are. I think 8% is a good guess at the
		// correct threshold, but time will tell. It shouldn't be the end of the world if this
		// occasionally guesses the wrong way.
		private bool HaveReasonableNumberOfWordBreaks(params string[] strings) =>
			strings.Any(s => s.Count(IsWhiteSpace) > 0.08 * s.Length);

		/// <summary>
		/// Works through the two strings to find segments that are the same and different.
		/// </summary>
		private IEnumerable<StringDifferenceSegment> ComputeDifferences(string origStr, string newStr)
		{
			// Step 1: Look for and return an initial common substring
			var bldr = new StringBuilder();
			int o = 0;
			int n = 0;

			// In the case of either PrecedingBaseCharacter or
			// PrecedingAndFollowingBaseCharacter, we will have already added
			// the preceding base character, so we need to remove it .
			bool MustRemovePrevBase(CharactersToKeepTogether c) => c != CharactersToKeepTogether.None;
			var ccState = new CharacterComparisonState(MustRemovePrevBase,
				HaveReasonableNumberOfWordBreaks(origStr, newStr));

			for (; o < origStr.Length && n < newStr.Length; o++, n++)
			{
				if (ccState.DoCharactersMatch(origStr, o, newStr, n, out var surrogate))
				{
					bldr.Append(origStr[o]);
					if (surrogate)
					{
						bldr.Append(origStr[++o]);
						n++;
					}
				}
				else
				{
					if (ccState.CharactersToRemove > 0)
					{
						bldr.Remove(bldr.Length - ccState.CharactersToRemove, ccState.CharactersToRemove);
						n -= ccState.CharactersToRemove;
						o -= ccState.CharactersToRemove;
					}
					break;
				}
			}

			if (bldr.Length > 0)
				yield return new StringDifferenceSegment(DifferenceType.Same, bldr.ToString());

			// Step 2: If we've run out of characters in one of the two strings (i.e., they started
			// off identically, but text was chopped off the original version or added to the end
			// of the new version), return an addition or deletion representing the trailing
			// difference.
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

			// Step 3: Look for a trailing common substring (don't return it yet)
			bldr.Clear();
			// Since we are working backward from the end of the string, we only need to
			// remove a character if this is a diacritic that would have joined the
			// previous character in the string to the following one (which we've already
			// processed).
			bool MustRemoveFollowingBase(CharactersToKeepTogether c) =>
				c == CharactersToKeepTogether.PrecedingAndFollowingBaseCharacter;
			ccState.Reset(MustRemoveFollowingBase);

			for (o = origStr.Length - 1, n = newStr.Length - 1;
			     o >= 0 && n >= 0; o--, n--)
			{
				if (ccState.DoCharactersMatch(origStr, o, newStr, n, out var surrogate))
				{
					bldr.Insert(0, origStr[o]);
					if (surrogate)
					{
						bldr.Insert(0, origStr[--o]);
						n--;
					}
				}
				else
				{
					if (ccState.CharactersToRemove > 0)
					{
						bldr.Remove(0, ccState.CharactersToRemove);
						n -= ccState.CharactersToRemove;
						o -= ccState.CharactersToRemove;
					}
					break;
				}
			}

			// Step 4: Within the middle chunk that is different, find the longest common
			// substring. Note: If we have already found viramas or letters that combine to form
			// ligatures (or totally different glyphs) we pass in 1.0 as the last param, which
			// prevents it from even trying to find a common substring that is not a whole word
			// (because in this kind of script it is virtually impossible to know which characters
			// will combine how in order to be able to visually indicate to the user what part of a
			// word changed). Otherwise, we tweak this method's default "magic number" of 15%
			// similarity because for our purposes we generally want to avoid returning partial
			// words as matches. But for scriptio continua or highly agglutinative languages, we
			// do want to allow for them. It's possible that this will need to be parameterized
			// so that the user can control the sensitivity of this (probably using a slider
			// control), but it would be hard to describe and they would really have to play with
			// it to figure out what works best. In any case, the visual color-coding we provide
			// is designed to be a help, so if it's a little too helpful at times and not quite
			// helpful enough at others, it's probably not the end of the world.
			var common = origStr.Substring(0, origStr.Length - bldr.Length).GetLongestUsefulCommonSubstring(
				newStr.Substring(0, newStr.Length - bldr.Length), out _, ccState.KeepWholeWordsTogether ? 1.0 :.20);

			if (common.Length > 1)
			{
				var iOrig = origStr.IndexOf(common, StringComparison.Ordinal);
				var iNew = newStr.IndexOf(common, StringComparison.Ordinal);
				if (iNew < iOrig)
					ExpandCommonSubstringIfNeeded(ref iNew, ref iOrig, ref common, newStr, origStr);
				else
					ExpandCommonSubstringIfNeeded(ref iOrig, ref iNew, ref common, origStr, newStr);

				// Step 4.A.i: Return the leading addition and/or deletion
				if (iOrig > 0)
					yield return new StringDifferenceSegment(DifferenceType.Deletion, origStr.Substring(0, iOrig));
				
				if (iNew > 0)
					yield return new StringDifferenceSegment(DifferenceType.Addition, newStr.Substring(0, iNew));

				// Step 4.A.ii: Return the common substring
				yield return new StringDifferenceSegment(DifferenceType.Same, common);

				// Step 4.A.iii: Return the trailing addition and/or deletion
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
				// Step 4.B: Nothing found in common between the beginning of the first difference
				// and the end of the last difference. Just return the addition and/or deletion
				if (o >= 0)
					yield return new StringDifferenceSegment(DifferenceType.Deletion, origStr.Substring(0, o + 1));
				if (n >= 0)
					yield return new StringDifferenceSegment(DifferenceType.Addition, newStr.Substring(0, n + 1));
			}

			// Step 5: Now return the trailing common substring found in step 3. 
			if (bldr.Length > 0)
				yield return new StringDifferenceSegment(DifferenceType.Same, bldr.ToString());
		}

		/// <summary>
		/// This helper method adjusts the results from GetLongestUsefulCommonSubstring (which gets
		/// whole words if possible) to include the leading and or trailing characters from the
		/// shorter of the two strings if they fully match the corresponding leading/trailing
		/// characters in the longer string. This prevents the possibility of (an) extra
		/// addition(s) in the shorter string that could have been more logically represented as an
		/// addition only in the longer string.
		/// </summary>
		/// <param name="iShort">Index to the start of the common substring in shortStr</param>
		/// <param name="iLong">Index to the start of the common substring in longStr</param>
		/// <param name="common">The longest "useful" common substring between shortStr and
		/// longStr, which this method will expand if possible</param>
		/// <param name="shortStr">The string whose leading portion (before the common substring)
		/// is shorter</param>
		/// <param name="longStr">The string whose leading portion (before the common substring)
		/// is longer</param>
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
