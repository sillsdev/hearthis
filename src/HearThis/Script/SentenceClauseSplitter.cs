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
	/// Results are trimmed.
	/// </summary>
	internal class SentenceClauseSplitter
	{
		public static IEnumerable<Chunk> BreakIntoChunks(string input, char[] separators)
		{
			if (input.IndexOfAny(separators) > 0)
			{
				int start = 0;
				while (start < input.Length)
				{
					int limOfLine = input.IndexOfAny(separators, start);
					if (limOfLine < 0)
						limOfLine = input.Length;
					else
						limOfLine++;

					// Advance over any combination of white space and closing punctuation. This will include trailing white space that we
					// don't actually want, but later we trim the result.
					while (limOfLine < input.Length &&
						(char.IsWhiteSpace(input[limOfLine]) || char.GetUnicodeCategory(input[limOfLine]) == UnicodeCategory.FinalQuotePunctuation))
						limOfLine++;

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
