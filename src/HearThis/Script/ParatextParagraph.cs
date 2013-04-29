using System.Collections.Generic;
using Paratext;

namespace HearThis.Script
{
	/// <summary>
	/// This class is used in ParatextScriptProvider.LoadBook. It manages the accumulation of text from multiple
	/// input tokens (markers or the following text tokens) and splitting the accumulated text of a paragraph
	/// into ScriptLines (that is, segments of an appropriate length and content to attempt to record as a unit).
	/// </summary>
	public class ParatextParagraph
	{
		//this was unreliable as the inner format stuff is apparently a reference, so it would change unintentionally
		//public ScrParserState State { get; private set; }
		public ScrTag State { get; private set; }
		public string text { get; private set; }

		public bool HasData
		{
			get { return !string.IsNullOrEmpty(text); }
		}

		public void Add(string s)
		{
			text += s;
			//Debug.WriteLine("Add " + s + " : " + State.Marker + " bold=" + State.Bold + " center=" + State.JustificationType);
		}

		public void StartNewParagraph(IScrParserState scrParserState)
		{
			text = "";
			State = scrParserState.ParaTag;
			//              Debug.WriteLine("Start " + State.Marker + " bold=" + State.Bold + " center=" + State.JustificationType);
		}

		/// <summary>
		/// Break the input into (trimmed) lines at sentence-final punctuation.
		/// Also responsible to convert double angle brackets to proper double-quote characters,
		/// and keep them attached to the right sentences.
		/// Possible enhancements:
		///		- Handle converting single angle brackets to appropriate single quotes
		///			- Handle special case of >>> (single followed by double)
		///		- Handle non-Roman sentence-final punctuation
		///		- Keep other post-end-of-sentence characters attached (closing parenthesis? >>>?
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ScriptLine> BreakIntoLines()
		{
			var separators = new char[] { '.', '?', '!' };
			var input = text.Replace("<<", "“").Replace(">>", "”").Trim();
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
					while (limOfLine < input.Length && char.IsWhiteSpace(input[limOfLine]))
						limOfLine++;
					if (limOfLine < input.Length && input[limOfLine] == '”')
						limOfLine++;
					var sentence = input.Substring(start, limOfLine - start);
					start = limOfLine;
					var trimSentence = sentence.Trim();
					if (!string.IsNullOrEmpty(trimSentence))
					{
						var x = GetScriptLine(trimSentence);

						yield return x;
					}
				}
			}
			else
			{
				yield return GetScriptLine(input);
			}
		}

		private ScriptLine GetScriptLine(string s)
		{
			//Debug.WriteLine("Emitting "+s+" bold="+State.Bold+" center="+State.JustificationType);
			return new ScriptLine()
			{
				Text = s,
				Bold = State.Bold,
				Centered = State.JustificationType == ScrJustificationType.scCenter,
				FontSize = State.FontSize,
				FontName = State.Fontname
			};
		}
	}
}
