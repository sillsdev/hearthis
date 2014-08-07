using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
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
		private string _text;
		private int _initialLineNumber0Based;
		private int _finalLineNumber0Based;
		private readonly HashSet<string> _introHeadingStyles = new HashSet<string> { "is", "imt", "imt1", "imt2", "imt3", "imt4", "imte", "imte1", "imte2", "is1", "is2", "iot" };
		public SentenceClauseSplitter SentenceSplitter { get; private set; }

		private string _verse = "0";

		public ParatextParagraph(SentenceClauseSplitter splitter)
		{
			SentenceSplitter = splitter;
			_text = string.Empty;
		}

		// Used to keep track of where new verses start
		class VerseStart
		{
			public string Verse;
			public int Offset;
		}

		List<VerseStart> _starts = new List<VerseStart>();

		public void NoteChapterStart()
		{
			NoteVerseStart("0");
		}

		public void NoteVerseStart(string verse)
		{
			_verse = verse;
			NoteVerseStart();
		}

		private void NoteVerseStart()
		{
			_starts.Add(new VerseStart() {Verse = _verse, Offset = _text.Length});
		}

		public bool HasData
		{
			get { return _text.Any(t => !Char.IsWhiteSpace(t)); }
		}

		public void AddHardLineBreak()
		{
			Add(ScriptLine.kLineBreak.ToString(CultureInfo.InvariantCulture));
			ContainsHardLineBreaks = true;
		}

		public void Add(string s)
		{
			if (State == null || _finalLineNumber0Based > _initialLineNumber0Based)
				throw new InvalidOperationException("Must call StartNewParagraph before adding Text to ParatextParagraph.");
			_text += ConvertChevronsToCurlyQuotes(s);
			//Debug.WriteLine("Add " + s + " : " + State.Marker + " bold=" + State.Bold + " center=" + State.JustificationType);
		}

		public void StartNewParagraph(IScrParserState scrParserState, bool resetLineNumber)
		{
			if (HasData && _finalLineNumber0Based == _initialLineNumber0Based)
			{
				var bldr = new StringBuilder();
				bool fFirstTime = true;
				foreach (ScriptLine item in BreakIntoBlocks())
				{
					if (item != null)
					{
						string sItem = item.Text;
						if (!fFirstTime && (!string.IsNullOrEmpty(sItem)))
							bldr.Append(" ");
						bldr.Append(sItem);
					}
					fFirstTime = bldr.Length == 0;
				}
				Debug.Fail("Looks like BreakIntoBlocks never got called for paragraph: " + bldr);
			}
			_text = string.Empty;
			ContainsHardLineBreaks = false;
			_starts.Clear();
			NoteVerseStart();
			State = scrParserState.CharTag != null && scrParserState.CharTag.Marker == "qs" ? scrParserState.CharTag : scrParserState.ParaTag;
			_initialLineNumber0Based = resetLineNumber ? 0 : _finalLineNumber0Based;
			_finalLineNumber0Based = _initialLineNumber0Based;

			//              Debug.WriteLine("Start " + State.Marker + " bold=" + State.Bold + " center=" + State.JustificationType);
		}

		/// <summary>
		/// Break the input into (trimmed) blocks at sentence-final punctuation.
		/// Sentence-final punctuation which occurs before any non-white text is attached to the following sentence.
		/// Also responsible to convert double angle brackets to proper double-quote characters,
		/// and keep them attached to the right sentences.
		/// Possible enhancements:
		///		- Handle converting single angle brackets to appropriate single quotes
		///			- Handle special case of >>> (single followed by double)
		///		- Handle non-Roman sentence-final punctuation
		///		- Keep other post-end-of-sentence characters attached (closing parenthesis? >>>?
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ScriptLine> BreakIntoBlocks()
		{
			_finalLineNumber0Based = _initialLineNumber0Based;
			foreach (var chunk in SentenceSplitter.BreakIntoChunks(_text.Trim()))
			{
				var x = GetScriptLine(chunk.Text, _finalLineNumber0Based++);
				SetScriptVerse(x, chunk.Start, chunk.Start + chunk.Text.Length);
				yield return x;
			}
		}

		public static string ConvertChevronsToCurlyQuotes(string s)
		{
			// REVIEW: This will probably not be needed if we get Paratext data via plug-in interface.
			// Common way of representing quotes in Paratext. The >>> combination is special to avoid getting the double first;
			// <<< is not special as the first two are correctly changed to double quote, then the third to single.
			// It is, of course, important to do all the double replacements before the single, otherwise, the single will just match doubles twice.
			// ENHANCE: Make more efficient.
			return s.Replace(">>>", "’”").Replace("<<", "“").Replace(">>", "”").Replace("<", "‘").Replace(">", "’");
		}

		private void SetScriptVerse(ScriptLine block, int start, int lim)
		{
			var startVerse = _starts[0].Verse;
			var endVerse = startVerse;
			foreach (VerseStart verseStart in _starts)
			{
				var offset = verseStart.Offset;
				if (offset <= start)
					startVerse = verseStart.Verse;
				if (offset >= lim)
					break;
				endVerse = verseStart.Verse;
			}
			if (endVerse == startVerse)
				block.Verse = startVerse;
			else
				block.Verse = startVerse + "-" + endVerse;
		}

		private ScriptLine GetScriptLine(string s, int lineNumber0Based)
		{
			//Debug.WriteLine("Emitting "+s+" bold="+State.Bold+" center="+State.JustificationType);
			var fontName = (string.IsNullOrWhiteSpace(State.Fontname)) ? DefaultFont : State.Fontname;
			return new ScriptLine()
			{
				Number = lineNumber0Based + 1,
				Text = s,
				Bold = State.Bold,
				// For now we want everything aligned left. Otherwise it gets separated from the hints that show which bit to read.
				Centered = false, //State.JustificationType == ScrJustificationType.scCenter,
				FontSize = State.FontSize,
				FontName = fontName,
				Heading = IsHeading,
				ParagraphStyle = State.Name,
				ForceHardLineBreakSplitting = ContainsHardLineBreaks
			};
		}

		public bool ContainsHardLineBreaks { get; private set; }

		private bool IsHeading
		{
			get
			{
				if (State.TextType == ScrTextType.scTitle || State.TextType == ScrTextType.scSection || State.HasTextProperty(TextProperties.scChapter))
					return true;
				return (_introHeadingStyles.Contains(State.Marker));
			}
		}

		private string _defaultFont;
		public string DefaultFont
		{
			get { return _defaultFont ?? ""; }
			set { _defaultFont = value; }
		}

		internal void ImproveState(IScrParserState state)
		{
			if (State.TextType == ScrTextType.scTitle && state.ParaTag != null && state.ParaTag.TextType == ScrTextType.scTitle)
			{
				State = state.ParaTag;
			}
			else
			{
				throw new InvalidOperationException(string.Format("Invalid state change attempted from {0} to {1}",
					State.Marker, state.ParaTag != null ? state.ParaTag.Marker : "null"));
			}
		}
	}
}
