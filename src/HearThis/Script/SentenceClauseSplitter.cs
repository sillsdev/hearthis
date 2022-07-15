// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2011' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using SIL.ObjectModel;
using SIL.Unicode;
using static System.Char;
using static System.String;

namespace HearThis.Script
{
	/// <summary>
	/// This class is responsible to split a string into segments terminated by one of a specified set of
	/// splitter characters. The terminating character is left as part of the previous string.
	/// Closing punctuation characters like quotes and brackets (and optional space between them)
	/// are kept with the previous chunk.
	/// Separators which occur before any non-white text are attached to the following sentence.
	/// Results are trimmed.
	/// </summary>
	public class SentenceClauseSplitter
	{
		private readonly IReadOnlySet<char> _additionalSeparators;
		private readonly bool _breakAtFirstLevelQuotes;
		private readonly string _firstLevelStartQuotationMark;
		private readonly string _firstLevelEndQuotationMark;

		public delegate void CharacterEncounteredHandler(SentenceClauseSplitter sender,
			char character);
		public event CharacterEncounteredHandler SentenceFinalPunctuationEncountered;

		public bool NestedQuotesEncountered { get; private set; }

		public SentenceClauseSplitter(IReadOnlySet<char> additionalSeparators)
		{
			if (additionalSeparators != null && additionalSeparators.Any())
				_additionalSeparators = additionalSeparators;
			_breakAtFirstLevelQuotes = false;
		}

		public SentenceClauseSplitter(IReadOnlySet<char> additionalSeparators, bool breakAtFirstLevelQuotes,
			IScrProjectSettings scrProjSettings = null) : this(additionalSeparators)
		{
			ScrProjSettings = scrProjSettings;
			if (!breakAtFirstLevelQuotes)
				return;

			if (scrProjSettings == null)
				throw new ArgumentNullException(nameof(scrProjSettings),
					"Project settings must be provided if caller wishes to have script broken out at first-level quotes.");

			_firstLevelStartQuotationMark = scrProjSettings.FirstLevelStartQuotationMark;
			_firstLevelEndQuotationMark = scrProjSettings.FirstLevelEndQuotationMark;

			if (IsNullOrWhiteSpace(_firstLevelStartQuotationMark) || IsNullOrWhiteSpace(_firstLevelEndQuotationMark))
			{
				// We can get into infinite loops if these are empty strings. I'm not sure what might go wrong if we try to treat
				// white space as quote markers, but Paratext at least does not allow it; I don't think we should try to either.
				// So as defensive programming, if either of these isn't some real, non-white text, we won't try to support
				// breaking at first level quotes.
				_firstLevelEndQuotationMark = _firstLevelStartQuotationMark = null;
				_breakAtFirstLevelQuotes = false;
			}
			else
				_breakAtFirstLevelQuotes = true;
		}

		public IScrProjectSettings ScrProjSettings { get; }

		private enum SeparatorType
		{
			NotASeparator,
			SentenceFinalPunctuation,
			Custom,
		}

		private bool IsSeparator(char c) => GetSeparatorType(c) != SeparatorType.NotASeparator;

		private SeparatorType GetSeparatorType(char c)
		{
			if (CharacterUtils.IsSentenceFinalPunctuation(c))
				return SeparatorType.SentenceFinalPunctuation;
			if (_additionalSeparators != null && _additionalSeparators.Contains(c))
				return SeparatorType.Custom;
			return SeparatorType.NotASeparator;
		}

		public IEnumerable<Chunk> BreakIntoChunks(string input)
		{
			int quoteDepth = 0;
			int start = 0;
			while (start < input.Length)
			{
				int startSearch = start;
				while (startSearch < input.Length &&
					(IsWhiteSpace(input[startSearch]) || IsSeparator(input[startSearch])))
					startSearch++;
				int limOfLine = -1;
				for (int i = startSearch; i < input.Length; i++)
				{
					var separator = GetSeparatorType(input[i]);
					if (separator != SeparatorType.NotASeparator)
					{
						if (!AtEndOfQuoteFollowedByLowerCaseLetter(input, i + 1))
						{
							if (separator == SeparatorType.SentenceFinalPunctuation)
								SentenceFinalPunctuationEncountered?.Invoke(this, input[i]);
							limOfLine = i;
							break;
						}
					}
					if (_breakAtFirstLevelQuotes)
					{
						Debug.Assert(_firstLevelEndQuotationMark != null);
						if (AtQuote(input, i, _firstLevelStartQuotationMark))
						{
							if (quoteDepth == 0 && i - 1 > startSearch)
							{
								limOfLine = i - 1;
								break;
							}
							if (++quoteDepth > 1)
								NestedQuotesEncountered = true;
						}

						if (AtQuote(input, i, _firstLevelEndQuotationMark) && --quoteDepth == 0)
						{
							limOfLine = i + _firstLevelEndQuotationMark.Length - 1;
							break;
						}
					}
				}
				if (limOfLine < 0)
					limOfLine = input.Length;
				else
					limOfLine++;

				// Advance over any combination of white space and closing punctuation. This will include trailing white space that we
				// don't actually want, but later we trim the result.
				while (limOfLine < input.Length)
				{
					var c = input[limOfLine];
					var category = GetUnicodeCategory(c);
					if (_firstLevelEndQuotationMark != null && AtQuote(input, limOfLine, _firstLevelEndQuotationMark))
					{
						limOfLine += _firstLevelEndQuotationMark.Length;
						if (_breakAtFirstLevelQuotes)
							quoteDepth--;
					}
					else if ((IsWhiteSpace(c) || category == UnicodeCategory.FinalQuotePunctuation ||
						category == UnicodeCategory.ClosePunctuation))
					{
						limOfLine++;
					}
					else
						break;
				}

				var sentence = input.Substring(start, limOfLine - start);
				int startCurrent = start;
				start = limOfLine;
				var trimSentence = sentence.Trim();
				if (!IsNullOrEmpty(trimSentence))
					yield return new Chunk() {Text = trimSentence, Start = startCurrent};
			}
		}

		private bool AtEndOfQuoteFollowedByLowerCaseLetter(string input, int start)
		{
			int i = start;
			bool atEndOfQuote = false;
			while (i < input.Length && !IsLetter(input[i]))
				atEndOfQuote |= (GetUnicodeCategory(input[i++]) == UnicodeCategory.FinalQuotePunctuation);
			if (i < input.Length)
				return (atEndOfQuote && IsLower(input[i]));
			return false;
		}

		private static bool AtQuote(string input, int i, string quotationMark)
		{
			int lenQuoteMarks = quotationMark.Length;
			bool atQuote = lenQuoteMarks <= input.Length - i;
			for (int iqs = 0; iqs < lenQuoteMarks && atQuote; iqs++)
				atQuote &= input[i + iqs] == quotationMark[iqs];
			return atQuote;
		}
	}

	public class Chunk
	{
		public string Text;
		public int Start; // index of first char of chunk in original string
	}
}
