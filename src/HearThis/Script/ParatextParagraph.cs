using System.Collections.Generic;
using Paratext;

namespace HearThis.Script
{
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

		public IEnumerable<ScriptLine> BreakIntoLines()
		{
			//TODO: this doesn't really parse well enough... e.g. "hello." will leave the quote to the next line.
			var separators = new char[] { '.', '?', '!' };
			if (text.IndexOfAny(separators) > 0)
			{

				foreach (var sentence in text.Split(separators))
				{
					//todo: this will replace the actual punctuation marks with periods.
					var trimSentence = sentence.Trim();
					if (!string.IsNullOrEmpty(trimSentence))
					{
						trimSentence = trimSentence.Replace("<<", "“");
						trimSentence = trimSentence.Replace(">>", "”");
						trimSentence = trimSentence.TrimStart('”');
						var x = GetScriptLine(trimSentence + ".");

						yield return x;
					}
				}
			}
			else
			{
				yield return GetScriptLine(text);
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
