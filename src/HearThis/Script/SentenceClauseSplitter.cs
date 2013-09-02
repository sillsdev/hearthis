using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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
	internal class SentenceClauseSplitter
	{
		public static IEnumerable<Chunk> BreakIntoChunks(string input, char[] separators)
		{
			if (input.IndexOfAny(separators) >= 0)
			{
				int start = 0;
				while (start < input.Length)
				{
					int startSearch = start;
					while (startSearch < input.Length && (Char.IsWhiteSpace(input[startSearch]) || separators.Contains(input[startSearch])))
						startSearch++;
					int limOfLine = input.IndexOfAny(separators, startSearch);
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
						if ((char.IsWhiteSpace(c) || category == UnicodeCategory.FinalQuotePunctuation ||
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
						yield return new Chunk() { Text = trimSentence, Start = startCurrent };
				}
			}
			else
				yield return new Chunk() {Text = input, Start = 0};
		}
	}

	internal class Chunk
	{
		public string Text;
		public int Start; // index of first char of chunk in original string
	}
}
