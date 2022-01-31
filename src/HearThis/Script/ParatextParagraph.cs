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
using System.Text;
using Paratext.Data;

namespace HearThis.Script
{
	/// <summary>
	/// This class is used in ParatextScriptProvider.LoadBook. It manages the accumulation of text from multiple
	/// input tokens (markers or the following text tokens) and splitting the accumulated text of a paragraph
	/// into ScriptLines (that is, segments of an appropriate length and content to attempt to record as a unit).
	/// </summary>
	public class ParatextParagraph
	{
		private class QuoteMarkPair
		{
			private readonly string _start;
			private readonly string _end;

			public QuoteMarkPair(string start, string end)
			{
				_start = start;
				_end = end;
			}

			public string Start { get { return _start; } }
			public string End { get { return _end; } }
		}

		//this was unreliable as the inner format stuff is apparently a reference, so it would change unintentionally
		//public ScrParserState State { get; private set; }
		public ScrTag State { get; private set; }
		private ScrTag _lastScriptureTextParaState;
		private StringBuilder _text;
		private int _initialLineNumber0Based;
		private int _finalLineNumber0Based;
		private readonly HashSet<string> _introHeadingStyles = new HashSet<string> { "is", "imt", "imt1", "imt2", "imt3", "imt4", "imte", "imte1", "imte2", "is1", "is2", "iot" };
		public SentenceClauseSplitter SentenceSplitter { get; private set; }
		private int quoteDepth;
		private readonly List<QuoteMarkPair> _quoteMarks;

		private string _verse = "0";

		public ParatextParagraph(SentenceClauseSplitter splitter)
		{
			SentenceSplitter = splitter;
			var settings = splitter.ScrProjSettings;
			if (!string.IsNullOrEmpty(settings.FirstLevelStartQuotationMark) && "<<" != settings.FirstLevelStartQuotationMark &&
				!string.IsNullOrEmpty(settings.FirstLevelEndQuotationMark) && ">>" != settings.FirstLevelEndQuotationMark)
			{
				 _quoteMarks = new List<QuoteMarkPair>(3);
				 _quoteMarks.Add(new QuoteMarkPair(settings.FirstLevelStartQuotationMark, settings.FirstLevelEndQuotationMark));
				if (!string.IsNullOrEmpty(settings.SecondLevelStartQuotationMark) && !string.IsNullOrEmpty(settings.SecondLevelEndQuotationMark))
				{
					_quoteMarks.Add(new QuoteMarkPair(settings.SecondLevelStartQuotationMark, settings.SecondLevelEndQuotationMark));

					if (!string.IsNullOrEmpty(settings.ThirdLevelStartQuotationMark) && !string.IsNullOrEmpty(settings.ThirdLevelEndQuotationMark))
					{
						_quoteMarks.Add(new QuoteMarkPair(settings.ThirdLevelStartQuotationMark, settings.ThirdLevelEndQuotationMark));
					}
				}
			}
			_text = new StringBuilder();
			quoteDepth = 0;
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
			get
			{
				for (int i = 0; i < _text.Length; i++)
					if (!Char.IsWhiteSpace(_text[i]))
						return true;
				return false;
			}
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
			int startAt = _text.Length;
			_text.Append(s);
			ConvertChevronsToCurlyQuotes(startAt);
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
			_text.Clear();
			quoteDepth = 0;
			ContainsHardLineBreaks = false;
			_starts.Clear();
			NoteVerseStart();
			State = scrParserState.CharTag != null && scrParserState.CharTag.Marker == "qs" ? scrParserState.CharTag : scrParserState.ParaTag;
			// Though it is not common, it is possible to have a chapter break mid-paragraph.
			// In that case, we want to use the previous "verse text" paragraph as our containing
			// paragraph for any subsequent verse text.
			if (State == null)
				State = _lastScriptureTextParaState;
			else if ((State.TextType & ScrTextType.scVerseText) != 0)
				_lastScriptureTextParaState = State;
			_initialLineNumber0Based = resetLineNumber ? 0 : _finalLineNumber0Based;
			_finalLineNumber0Based = _initialLineNumber0Based;

			//              Debug.WriteLine("Start " + State.Marker + " bold=" + State.Bold + " center=" + State.JustificationType);
		}

		/// <summary>
		/// Represents a "dummy" parser state in order to be able to force a simulated paragraph
		/// into the data stream where it is missing in the actual data coming from Paratext.
		/// HearThis doesn't really care that much about paragraphs, but the paragraph that
		/// contains potentially recordable text does tell it a) whether it is a Scripture Heading
		/// (often omitted from recordings) and b) what the base font face, style and size are for
		/// the text. As such, HearThis can't accommodate text not contained by a paragraph.
		/// </summary>
		/// <remarks>
		/// Normally, IScrParserState is implemented by an object that wraps a Paratext
		/// ScrParserState object, with its properties determined by the preceding markers in the
		/// stream.
		/// </remarks>
		private class HearThisDummyParaState : IScrParserState
		{
			public HearThisDummyParaState(ScrTag paraTag) => ParaTag = paraTag;

			public ScrTag NoteTag => null;

			public ScrTag CharTag => null;

			public ScrTag ParaTag { get; }

			public bool ParaStart => true;

			public bool IsPublishable => true;

			public void UpdateState(List<UsfmToken> tokenList, int tokenIndex)
			{
				throw new NotImplementedException();
			}
		}

		// HT-415: this is (as the name implies) a hack to allow HearThis
		// to load a Paratext book that is missing paragraph markers after the \c.
		internal void ForceNewParagraph(ScrTag paraTag)
		{
			StartNewParagraph(new HearThisDummyParaState(paraTag), false);
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
		/// <param name="keepTogether">Flag indicating the special case of a paragraph that should
		/// not be broken into multiple blocks. If true, this method will return a single
		/// ScriptLine. (Basically, this is a convenience to still get the trimming logic and the
		/// creation of a ScriptLine that comes back as an IEnumerable. But it means that the
		/// method name is kind of misleading in this case.)</param>
		/// <returns></returns>
		public IEnumerable<ScriptLine> BreakIntoBlocks(bool keepTogether = false)
		{
			_finalLineNumber0Based = _initialLineNumber0Based;

			// Trim
			while (_text.Length > 0 && Char.IsWhiteSpace(_text[0]))
				_text.Remove(0, 1);
			while (_text.Length > 1 && Char.IsWhiteSpace(_text[_text.Length - 1]))
				_text.Remove(_text.Length - 1, 1);

			foreach (var chunk in GetChunks(keepTogether))
			{
				var x = GetScriptLine(chunk.Text, _finalLineNumber0Based++);
				x.RightToLeft = RightToLeft;
				SetScriptVerse(x, chunk.Start, chunk.Start + chunk.Text.Length);
				yield return x;
			}
		}

		private IEnumerable<Chunk> GetChunks(bool keepTogether)
		{
			var text = _text.ToString();
			if (keepTogether)
			{
				yield return new Chunk {Text = text};
			}
			else
			{
				foreach (var chunk in SentenceSplitter.BreakIntoChunks(text))
					yield return chunk;
			}
		}

		private void ConvertChevronsToCurlyQuotes(int startAt)
		{
			if (_quoteMarks == null)
				return;
			// Greater-than (>) and less-than (<) symbols are sometimes used as "chevrons" to indicate
			// the start and end of quotes in Paratext. When nested, this can yield text that is not very
			// human-readble, e.g. <<< or >>>. This code used to be done using efficient regular expression
			// replacements, but it did not correctly handle the possibility of deeply nested quotes.
			for (int i = startAt; i < _text.Length; i++)
			{
				char ch = _text[i];
				if (ch == '<')
				{
					if (quoteDepth % 2 == 0)
					{
						// Looking for an opening double chevron
						if (i + 1 < _text.Length && _text[i + 1] == '<')
						{
							_text.Remove(i, 2);
							_text.Insert(i, _quoteMarks[quoteDepth++ % _quoteMarks.Count].Start);
							continue;
						}
					}
					// Found an opening single chevron
					if (_quoteMarks.Count > 1)
					{
						_text.Remove(i, 1);
						if (quoteDepth % 2 == 0)
						{
							// We were looking for an even level quote (i.e., double chevron), but found an odd level
							// (single chevron) instead, so we do an extra increment of the level to jump ahead.
							quoteDepth++;
						}
						_text.Insert(i, _quoteMarks[quoteDepth++ % _quoteMarks.Count].Start);
					}
					else
						quoteDepth++;
				}
				else if (ch == '>' && quoteDepth > 0)
				{
					if (quoteDepth % 2 == 1)
					{
						// Looking for a closing double chevron
						if (i + 1 < _text.Length && _text[i + 1] == '>')
						{
							_text.Remove(i, 2);
							_text.Insert(i, _quoteMarks[--quoteDepth % _quoteMarks.Count].End);
						}
					}
					else if (_quoteMarks.Count > 1)
					{
						// Found a closing single chevron
						_text.Remove(i, 1);
						_text.Insert(i, _quoteMarks[--quoteDepth % _quoteMarks.Count].End);
					}
					else
						quoteDepth--;
				}
			}
		}

		private void SetScriptVerse(ScriptLine block, int start, int lim)
		{
			var startVerse = _starts[0].Verse;
			var otherVerses = string.Empty;
			foreach (VerseStart verseStart in _starts)
			{
				var offset = verseStart.Offset;
				if (offset <= start)
					startVerse = verseStart.Verse;
				if (offset >= lim)
					break;
				if (verseStart.Verse != startVerse)
				{
					otherVerses += "~" + verseStart.Verse; // Use tilde to distinguish from explicit verse bridges in the text
					block.AddVerseOffset(offset - start);
				}
			}
			block.Verse = startVerse;
			if (otherVerses.Length > 0)
				block.Verse += otherVerses;
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
				HeadingType = State.Marker.TrimStart('\\'),
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

		private bool _rightToLeft;
		public bool RightToLeft
		{
			get { return _rightToLeft; }
			set { _rightToLeft = value; }
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
