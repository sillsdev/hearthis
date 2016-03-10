// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using SIL.Unicode;

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
		private readonly IScrProjectSettings _scrProjSettings;
		private readonly HashSet<char> _additionalSeparators;
		private readonly bool _breakAtFirstLevelQuotes;
		private readonly string _firstLevelStartQuotationMark;
		private readonly string _firstLevelEndQuotationMark;

		public bool NestedQuotesEncountered { get; private set; }

		public SentenceClauseSplitter(char[] additionalSeparators)
		{
			if (additionalSeparators != null && additionalSeparators.Any())
				_additionalSeparators = new HashSet<char>(additionalSeparators);
			_breakAtFirstLevelQuotes = false;
		}

		public SentenceClauseSplitter(char[] additionalSeparators, bool breakAtFirstLevelQuotes,
			IScrProjectSettings scrProjSettings) : this(additionalSeparators)
		{
			_scrProjSettings = scrProjSettings;
			_firstLevelStartQuotationMark = scrProjSettings.FirstLevelStartQuotationMark;
			_firstLevelEndQuotationMark = scrProjSettings.FirstLevelEndQuotationMark;

			_breakAtFirstLevelQuotes = breakAtFirstLevelQuotes && _firstLevelStartQuotationMark != null;
			if (_breakAtFirstLevelQuotes)
				Debug.Assert(_firstLevelEndQuotationMark != null);
		}

		public IScrProjectSettings ScrProjSettings
		{
			get { return _scrProjSettings; }
		}

		private bool IsSeparator(char c)
		{
			return CharacterUtils.IsSentenceFinalPunctuation(c) ||
				(_additionalSeparators != null && _additionalSeparators.Contains(c));
		}

		public IEnumerable<Chunk> BreakIntoChunks(string input)
		{
			int quoteDepth = 0;
			int start = 0;
			while (start < input.Length)
			{
				int startSearch = start;
				while (startSearch < input.Length &&
					(Char.IsWhiteSpace(input[startSearch]) || IsSeparator(input[startSearch])))
				startSearch++;
				int limOfLine = -1;
				for (int i = startSearch; i < input.Length; i++)
				{
					if (IsSeparator(input[i]))
					{
						if (!AtEndOfQuoteFollowedByLowerCaseLetter(input, i + 1))
						{
							limOfLine = i;
							break;
						}
					}
					if (_breakAtFirstLevelQuotes)
					{
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
					var category = char.GetUnicodeCategory(c);
					if (_firstLevelEndQuotationMark != null && AtQuote(input, limOfLine, _firstLevelEndQuotationMark))
					{
						limOfLine += _firstLevelEndQuotationMark.Length;
						if (_breakAtFirstLevelQuotes)
							quoteDepth--;
					}
					else if ((char.IsWhiteSpace(c) || category == UnicodeCategory.FinalQuotePunctuation ||
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
				if (!string.IsNullOrEmpty(trimSentence))
					yield return new Chunk() {Text = trimSentence, Start = startCurrent};
			}
		}

		private bool AtEndOfQuoteFollowedByLowerCaseLetter(string input, int start)
		{
			int i = start;
			bool atEndOfQuote = false;
			while (i < input.Length && !char.IsLetter(input[i]))
				atEndOfQuote |= (char.GetUnicodeCategory(input[i++]) == UnicodeCategory.FinalQuotePunctuation);
			if (i < input.Length)
				return (atEndOfQuote && char.IsLower(input[i]));
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
