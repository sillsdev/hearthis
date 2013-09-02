using System.Collections.Generic;
using System.Globalization;
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

		private string _verse = "0";

		// Used to keep track of where new verses start
		class VerseStart
		{
			public string Verse;
			public int Offset;
		}

		List<VerseStart> _starts = new List<VerseStart>();

		public string Verse
		{
			get { return _verse; }
			set
			{
				_verse = value;
				NoteVerseStart();
			}
		}

		private void NoteVerseStart()
		{
			_starts.Add(new VerseStart() {Verse = _verse, Offset = (text ?? "").Length});
		}

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
			_starts.Clear();
			NoteVerseStart();
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
			//Note... while one might thing that char.GetUnicodeCategory could tell you if a character was a sentence separator, this is not the case.
			//I gather this is becuase, for example, '.' can be used for various things (abbreviation, decimal point, as well as sentence terminator).
			var separators = new char[] { '.', '?', '!',
				'।', '॥' //devenagri
			};
			// Common way of representing quotes in Paratext. The >>> combination is special to avoid getting the double first;
			// <<< is not special as the first two are correctly changed to double quote, then the third to single.
			// It is, of course, important to do all the double replacements before the single, otherwise, the single will just match doubles twice.
			var input = text.Replace(">>>","’”").Replace("<<", "“").Replace(">>", "”").Replace("<","‘").Replace(">","’").Trim();
			foreach (var chunk in SentenceClauseSplitter.BreakIntoChunks(input, separators))
			{
				var x = GetScriptLine(chunk.Text);
				SetScriptVerse(x, chunk.Start);
				yield return x;
			}
		}

		private void SetScriptVerse(ScriptLine line, int start)
		{
			if (_starts.Count == 0)
			{
				line.Verse = Verse;
				return; // not sure this can happen, playing safe.
			}
			var verse = _starts[0].Verse;
			for (int i = 0; i < _starts.Count; i++)
			{
				if (_starts[i].Offset > start)
					break;
				verse = _starts[i].Verse;
			}
			line.Verse = verse;
		}

		private ScriptLine GetScriptLine(string s)
		{
			//Debug.WriteLine("Emitting "+s+" bold="+State.Bold+" center="+State.JustificationType);
			var fontName = (string.IsNullOrWhiteSpace(State.Fontname)) ? DefaultFont : State.Fontname;
			return new ScriptLine()
			{
				Text = s,
				Bold = State.Bold,
				// For now we want everything aligned left. Otherwise it gets separated from the hints that show which bit to read.
				Centered = false, //State.JustificationType == ScrJustificationType.scCenter,
				FontSize = State.FontSize,
				FontName = fontName
			};
		}

		private string _defaultFont;
		public string DefaultFont
		{
			get { return _defaultFont ?? ""; }
			set { _defaultFont = value; }
		}
	}
}
