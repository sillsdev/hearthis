using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

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
		private readonly HashSet<char> _separators;
		private readonly string _firstLevelStartQuotationMark;
		private readonly string _firstLevelEndQuotationMark;

		public SentenceClauseSplitter(char[] separators)
		{
			_separators = new HashSet<char>(separators);
		}

		public SentenceClauseSplitter(char[] separators, string firstLevelStartQuotationMark, string firstLevelEndQuotationMark) : this(separators)
		{
			_firstLevelStartQuotationMark = firstLevelStartQuotationMark;
			_firstLevelEndQuotationMark = firstLevelEndQuotationMark;
			if (_firstLevelStartQuotationMark != null)
				Debug.Assert(_firstLevelEndQuotationMark != null);
		}

		public IEnumerable<Chunk> BreakIntoChunks(string input)
		{
			int quoteDepth = 0;
			int start = 0;
			while (start < input.Length)
			{
				int startSearch = start;
				while (startSearch < input.Length &&
						(Char.IsWhiteSpace(input[startSearch]) || _separators.Contains(input[startSearch])))
					startSearch++;
				int limOfLine = -1;
				for (int i = startSearch; i < input.Length; i++)
				{
					if (_separators.Contains(input[i]))
					{
						if (!AtEndOfQuoteFollowedByLowerCaseLetter(input, i + 1))
						{
							limOfLine = i;
							break;
						}
					}
					if (_firstLevelStartQuotationMark != null)
					{
						if (AtQuote(input, i, _firstLevelStartQuotationMark) && quoteDepth++ == 0 && i - 1 > startSearch)
						{
							limOfLine = i - 1;
							break;
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
