using System;
using System.Collections.Generic;
using System.Globalization;
using Palaso.Code;
using Paratext;

namespace HearThis.Script
{
	public class ParatextScriptProvider : IScriptProvider
	{
		private readonly IScripture _paratextProject;
		private Dictionary<int, Dictionary<int, List<ScriptLine>>> _script; // book <chapter, lines>
		private Dictionary<int, int[]>  _chapterVerseCount;//book <chapter, verseCount>
		private const char Space = ' ';

		public ParatextScriptProvider(IScripture paratextProject)
		{
			Guard.AgainstNull(paratextProject,"paratextProject");
			_paratextProject = paratextProject;
			_chapterVerseCount = new Dictionary<int, int[]>();
			_script = new Dictionary<int, Dictionary<int, List<ScriptLine>>>();
		}

		/// <summary>
		/// The "line" is a bit of script (Book name, chapter #, section headings, etc.)
		/// </summary>
		public ScriptLine GetLine(int bookNumber0Based, int chapterNumber, int lineNumber0Based)
		{
			try
			{
				if (!_script.ContainsKey(bookNumber0Based)
					|| !_script[bookNumber0Based].ContainsKey(chapterNumber)
					|| _script[bookNumber0Based][chapterNumber].Count - 1 < lineNumber0Based)
					return null;
				return _script[bookNumber0Based][chapterNumber][lineNumber0Based];
			}
			catch(Exception)
			{
				return null;
			}
		}

		public int GetScriptLineCount(int bookNumber0Based, int chapter1Based)
		{
			try
			{
				if (!_script.ContainsKey(bookNumber0Based)
					 || !_script[bookNumber0Based].ContainsKey(chapter1Based))
					return 0;
				return _script[bookNumber0Based][chapter1Based].Count;
			}
			catch(Exception)
			{
				return 0;
			}
		}


		public int GetScriptLineCount(int bookNumber0Based)
		{
			int r = 0;
			try
			{
				if (!_script.ContainsKey(bookNumber0Based))
					return 0;
				foreach (var chapter in _script[bookNumber0Based])
				{
					r += GetScriptLineCount(bookNumber0Based, chapter.Key);
				}
			}
			catch (Exception)
			{
				return 0;
			}
			return r;
		}


		public int GetTranslatedVerseCount(int bookNumber, int chapterNumber1Based)
		{
			try
			{
				if (_chapterVerseCount == null || !_chapterVerseCount.ContainsKey(bookNumber) || _chapterVerseCount[bookNumber].Length < chapterNumber1Based - 1)
					return 0;

				return _chapterVerseCount[bookNumber][chapterNumber1Based];
			}
			catch(Exception)
			{
				return 0;
			}
		}

		public void LoadBook(int bookNumber0Based)
		{
			if(_script.ContainsKey(bookNumber0Based))
			{
				return;//already loaded
			}
			// review: this slows loading down;
			// it was added because I occasionaly got an error accessing the _script, on the following line:
			// _script.Add(bookNumber0Based, bookScript);
			lock (_script)
			{
				var bookScript = new Dictionary<int, List<ScriptLine>>(); //chapter, lines
				_script.Add(bookNumber0Based, bookScript);

				var verseRef = new VerseRef(bookNumber0Based + 1, 1, 0 /*verse*/, _paratextProject.Versification);

				var tokens = _paratextProject.GetUsfmTokens(verseRef, false, true);
				var state = _paratextProject.CreateScrParserState(verseRef);
				var paragraph = new ParatextParagraph() {DefaultFont = _paratextProject.DefaultFont};
				var versesPerChapter = GetArrayForVersesPerChapter(bookNumber0Based);

				//Introductory lines, before the start of the chapter, will be in chapter 0
				int currentChapter1Based = 0;
				var chapterLines = GetNewChapterLines(bookNumber0Based, currentChapter1Based);
				paragraph.Verse = "0"; // until we encounter /v

				var passedFirstChapterMarker = false;
				var chapterLabelScopeIsBook = false;
				var chapterLabelIsSupplied = false;
				var chapterCharacterIsSupplied = false;
				const string defaultChapterLabel = "Chapter ";
				var chapterLabel = string.Empty;
				var chapterCharacter = string.Empty;
				var lookingForVerseText = false;
				var lookingForChapterLabel = false;
				var lookingForChapterCharacter = false;
				var collectingChapterInfo = false;

				for (var i = 0; i < tokens.Count; i++)
				{
					UsfmToken t = tokens[i];
					state.UpdateState(tokens, i);

					if (state.NoteTag != null)
						continue; // skip note text tokens
					if (state.CharTag != null && state.CharTag.Marker == "fig")
						continue; // skip figure tokens
					if (state.ParaTag != null && !MarkerIsReadable(state.ParaTag))
						continue; // skip any undesired paragraph types

					if (state.ParaStart)
					{
						// If we've been collecting chapter info and we're starting a paragraph that we'll need to write out
						// then we need to emit our chapter string.
						// (\cl and \cp have TextProperty paragraph, but are not "publishable vernacular")
						if (collectingChapterInfo && state.IsPublishableVernacular)
						{
							EmitChapterString(paragraph, chapterLabelScopeIsBook, chapterLabelIsSupplied, chapterCharacterIsSupplied,
								chapterLabel, chapterCharacter);
							collectingChapterInfo = false;
						}
						if (paragraph.HasData)
						{
							chapterLines.AddRange(paragraph.BreakIntoLines());
						}
						paragraph.StartNewParagraph(state);
						if (currentChapter1Based == 0)
							versesPerChapter[0]++; // this helps to show that there is some content in the intro
					}

					switch (t.Marker)
					{
						case null: // This is most likely a text node
							var tokenText = tokens[i].Text;

							if (!string.IsNullOrEmpty(tokenText))
							{
								// was paragraph.Add(tokens[i].Text.Trim());
								// removing the Trim() fixed InlineTag spacing problem
								// It looks like BreakIntoLines() already trims script lines anyway.
								tokenText = tokenText.TrimStart();
								// if tokenText was just a space...
								if (tokenText.Length > 0)
								{
									if (tokenText[tokenText.Length - 1] != Space)
									{
										// If this will be the end of a line, it will get trimmed anyway
										// if not, it keeps things like footnote markers from producing
										// words that are jammed together.
										// We may eventually need exceptions for certain situations with quotes?
										tokenText += Space;
									}
									if (lookingForChapterCharacter)
									{
										chapterCharacter = tokenText;
										lookingForChapterCharacter = false;
										chapterCharacterIsSupplied = true;
										continue; // Wait until we hit another unrelated ParaStart to write out
									}
									if (lookingForChapterLabel)
									{
										chapterLabel = tokenText;
										lookingForChapterLabel = false;
										chapterLabelIsSupplied = true;
										continue; // Wait until we hit another unrelated ParaStart to write out
									}
									if (lookingForVerseText)
									{
										lookingForVerseText = false;
										versesPerChapter[currentChapter1Based]++;
									}
									paragraph.Add(tokenText);
								}
							}
							break;
						case "v":
							paragraph.Verse = t.Data[0].Trim();
							// don't be fooled by empty \v markers
							if (lookingForVerseText)
							{
								paragraph.Add(Space.ToString(CultureInfo.CurrentUICulture));
								versesPerChapter[currentChapter1Based]++;
							}
							lookingForVerseText = true;
							break;
						case "c":
							paragraph.Verse = "0"; // until next /v
							lookingForVerseText = false;
							lookingForChapterLabel = false;
							lookingForChapterCharacter = false;
							chapterLabelIsSupplied = false;
							chapterCharacterIsSupplied = false;
							collectingChapterInfo = true;
							if (!chapterLabelScopeIsBook)
								chapterLabel = defaultChapterLabel; //TODO: Localize
							if (t.HasData)
							{
								var chapterString = t.Data[0].Trim();
								currentChapter1Based = int.Parse(chapterString);
								chapterLines = GetNewChapterLines(bookNumber0Based, currentChapter1Based);
								passedFirstChapterMarker = true;
								chapterCharacter = chapterString; // Wait until we hit another unrelated ParaStart to write out
							}
							break;
						case "cl":
							lookingForChapterLabel = true;
							if (!passedFirstChapterMarker)
								chapterLabelScopeIsBook = true;
							break;
						case "cp":
							lookingForChapterCharacter = true;
							break;
						default: // Some other Marker
							break;
					}
				}
				// emit the last line
				if (paragraph.HasData)
				{
					chapterLines.AddRange(paragraph.BreakIntoLines());
				}
			}
		}

		public string EthnologueCode { get { return _paratextProject.EthnologueCode; } }

		private void EmitChapterString(ParatextParagraph paragraph, bool labelScopeIsBook, bool labelIsSupplied, bool characterIsSupplied,
			string chapLabel, string chapCharacter)
		{
			var result = chapLabel + chapCharacter;
			if (!labelScopeIsBook && labelIsSupplied)
			{
				if (characterIsSupplied)
					result = chapLabel + chapCharacter;
				else
					result = chapLabel;
			}
			paragraph.Add(result);
		}

		private bool MarkerIsReadable(ScrTag tag)
		{
			// Enhance: GJM Eventually, hopefully, we can base this on a new 'for-audio'
			// flag in TextProperties.
			var isPublishable = tag.TextProperties.HasFlag(TextProperties.scPublishable);
			var isVernacular = tag.TextProperties.HasFlag(TextProperties.scVernacular);
			var isParagraph = tag.TextProperties.HasFlag(TextProperties.scParagraph);
			if (isParagraph && isPublishable && isVernacular)
				return true;
			if (isParagraph && isPublishable && (tag.Marker == "cl" || tag.Marker == "cp"))
				return true;
			return tag.TextProperties.HasFlag(TextProperties.scChapter);
		}

		private List<ScriptLine> GetNewChapterLines(int bookNumber1Based, int currentChapter1Based)
		{
			var chapterLines = new List<ScriptLine>();
			_script[bookNumber1Based][currentChapter1Based] = chapterLines;
			return chapterLines;
		}

		private int[] GetArrayForVersesPerChapter(int bookNumber1Based)
		{
			int[] versesPerChapter;
			if (!_chapterVerseCount.TryGetValue(bookNumber1Based, out versesPerChapter))
			{
				versesPerChapter = new int[200];
				_chapterVerseCount[bookNumber1Based] = versesPerChapter;
			}
			return versesPerChapter;
		}
	}
}