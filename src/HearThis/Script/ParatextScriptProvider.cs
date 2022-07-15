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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using SIL.Code;
using Paratext.Data;
using SIL.Reporting;
using SIL.Scripture;

namespace HearThis.Script
{
	public class ParatextScriptProvider : ScriptProviderBase, IScrProjectSettingsProvider
	{
		private readonly IScripture _paratextProject;
		private readonly Dictionary<int, Dictionary<int, List<ScriptLine>>> _script; // book <chapter, lines>
		private readonly Dictionary<int, int[]> _chapterVerseCount; //book <chapter, verseCount>
		private readonly HashSet<string> _allEncounteredParagraphStyleNames; // This will not include the ones that are always ignored.
		private readonly IBibleStats _versificationInfo;

		/// <summary>
		/// These are markers that ARE paragraph and IsPublishable, but we don't want to read them.
		/// They should be followed by a single text node that will be skipped too. This list used
		/// to include some other markers that are not usually included in a recording but could be.
		/// See <see cref="ScriptProviderBase.StylesToSkipByDefault"/> for details.
		/// </summary>
		/// <remarks>The h1-3 markers are now deprecated in USFM</remarks>
		private readonly HashSet<string> _furtherParagraphIgnorees = new HashSet<string> {"h", "h1", "h2", "h3", "toc1", "toc2", "toc3", "toca1", "toca2", "toca3"}; // id marker is already non-publishable in USFM, so I removed it from this list

		// These are inline markers that we don't want to read.
		// They should be followed by a single text node that will be skipped too.
		private readonly ReadOnlyCollection<string> _furtherInlineIgnorees = new List<string> { "rq" }.AsReadOnly();
		private SentenceClauseSplitter _sentenceSplitter;

		public ParatextScriptProvider(IScripture paratextProject)
		{
			Guard.AgainstNull(paratextProject, "paratextProject");
			_paratextProject = paratextProject;
			_chapterVerseCount = new Dictionary<int, int[]>();
			_script = new Dictionary<int, Dictionary<int, List<ScriptLine>>>();
			_allEncounteredParagraphStyleNames = new HashSet<string>();
			_versificationInfo = new ParatextVersificationInfo(paratextProject.Versification);

			Initialize(() =>
			{
				char[] separators = null;
				string additionalBreakCharacters = ProjectSettings.AdditionalBlockBreakCharacters?.Replace(" ", string.Empty);
				if (!String.IsNullOrEmpty(additionalBreakCharacters))
					separators = additionalBreakCharacters.ToArray();
				_sentenceSplitter = new SentenceClauseSplitter(separators, ProjectSettings.BreakQuotesIntoBlocks, paratextProject);
			});
		}

		public override string FontName => _paratextProject.DefaultFont;

		public override bool RightToLeft => _paratextProject.RightToLeft;

		public override string EthnologueCode => _paratextProject.EthnologueCode;

		protected override IStyleInfoProvider StyleInfo => _paratextProject.StyleInfo;

		/// <summary>
		/// The "block" is a bit of script (Book name, chapter #, section headings, etc.)
		/// </summary>
		/// <exception cref="KeyNotFoundException">Book or chapter not loaded or invalid number</exception>
		/// <exception cref="IndexOutOfRangeException">Block number out of range</exception>
		public override ScriptLine GetBlock(int bookNumber0Based, int chapterNumber, int blockNumber0Based)
		{
			Dictionary<int, List<ScriptLine>> chapterLines;
			lock (_script)
			{
				LoadBook(bookNumber0Based);
				chapterLines = _script[bookNumber0Based];
				Monitor.Enter(chapterLines);
			}

			try
			{
				return chapterLines[chapterNumber][blockNumber0Based];
			}
			finally
			{
				Monitor.Exit(chapterLines);
			}
		}

		public override void UpdateSkipInfo()
		{
			LoadSkipInfo();
			lock (_script)
			{
				foreach (var bookKvp in _script)
				{
					lock (bookKvp.Value)
					{
						foreach (var chapKvp in bookKvp.Value)
						{
							PopulateSkippedFlag(bookKvp.Key, chapKvp.Key, chapKvp.Value);
						}
					}
				}
			}
		}

		public override int GetScriptBlockCount(int bookNumber0Based, int chapter1Based)
		{
			return GetScriptBlocks(bookNumber0Based, chapter1Based).Count;
		}

		public override int GetSkippedScriptBlockCount(int bookNumber0Based, int chapter1Based)
		{
			return GetScriptBlocks(bookNumber0Based, chapter1Based).Count(s => s.Skipped);
		}

		public override int GetUnskippedScriptBlockCount(int bookNumber0Based, int chapter1Based)
		{
			return GetScriptBlocks(bookNumber0Based, chapter1Based).Count(s => !s.Skipped);
		}

		private List<ScriptLine> GetScriptBlocks(int bookNumber0Based, int chapter1Based)
		{
			Dictionary<int, List<ScriptLine>> chapterLines;
			lock (_script)
			{
				LoadBook(bookNumber0Based);
				chapterLines = _script[bookNumber0Based];
				Monitor.Enter(chapterLines);
			}

			try
			{
				if (chapterLines.TryGetValue(chapter1Based, out var scriptLines))
					return scriptLines;
			}
			finally
			{
				Monitor.Exit(chapterLines);
			}
			return new List<ScriptLine>();
		}

		public override int GetScriptBlockCount(int bookNumber0Based)
		{
			Dictionary<int, List<ScriptLine>> chapterLines;
			lock (_script)
			{
				LoadBook(bookNumber0Based);
				chapterLines = _script[bookNumber0Based];
				// Note: in this case (unlike some others), we cannot release our lock on _script yet
				// because the call to GetScriptBlockCount below re-locks it and this can cause dead-lock
				// if another thread locks _script and then wants to lock the dictionary of chapter lines
				// for this same book.
				lock (chapterLines)
				{
					return chapterLines.Sum(chapter => GetScriptBlockCount(bookNumber0Based, chapter.Key));
				}
			}
		}

		public override int GetTranslatedVerseCount(int bookNumber0Based, int chapterNumber1Based)
		{
			lock (_chapterVerseCount)
			{
				if (!_chapterVerseCount.ContainsKey(bookNumber0Based) ||
					_chapterVerseCount[bookNumber0Based].Length < chapterNumber1Based - 1)
				{
					return 0;
				}

				return _chapterVerseCount[bookNumber0Based][chapterNumber1Based];
			}
		}

		public override void LoadBook(int bookNumber0Based)
		{
			List<UsfmToken> tokens;
			IScrParserState state;
			Dictionary<int, List<ScriptLine>> chapterLines;
			lock (_script)
			{
				if (_script.ContainsKey(bookNumber0Based))
				{
					return; //already loaded
				}

				chapterLines = new Dictionary<int, List<ScriptLine>>();
				Monitor.Enter(chapterLines);
				_script.Add(bookNumber0Based, chapterLines); // dictionary of chapter to lines

				var verseRef = new VerseRef(bookNumber0Based + 1, 1, 0 /*verse*/, _paratextProject.Versification);

				tokens = _paratextProject.GetUsfmTokens(verseRef);
				state = _paratextProject.CreateScrParserState(verseRef);
			}

			Logger.WriteEvent("Loading book: " + BCVRef.NumberToBookCode(bookNumber0Based + 1));

			try
			{
				LoadBookData(bookNumber0Based, chapterLines, tokens, state);
			}
			finally
			{
				Monitor.Exit(chapterLines);
			}
		}

		private void LoadBookData(int bookNumber0Based, Dictionary<int, List<ScriptLine>> bookData, List<UsfmToken> tokens, IScrParserState state)
		{
			var paragraph = new ParatextParagraph(_sentenceSplitter) {DefaultFont = _paratextProject.DefaultFont, RightToLeft = _paratextProject.RightToLeft};
			var versesPerChapter = GetArrayForVersesPerChapter(bookNumber0Based);

			//Introductory lines, before the start of the chapter, will be in chapter 0
			int currentChapter1Based = 0;
			var chapterLines = GetNewChapterLines(bookData, currentChapter1Based);
			paragraph.NoteChapterStart();

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
			var processingGlossaryWord = false;
			var inTitle = false;
			var collectingChapterInfo = false;

			UsfmToken t = null;
			string previousMarker = null;
			for (var i = 0; i < tokens.Count; i++)
			{
				var previousTextType = state.ParaTag?.TextType ?? ScrTextType.scOther;
				previousMarker = t?.Marker ?? previousMarker;

				t = tokens[i];

				state.UpdateState(tokens, i);

				if (!state.IsPublishable || state.NoteTag != null)
					continue; // skip note text tokens and anything non-publishable (including figures)
				if (state.CharTag != null && _furtherInlineIgnorees.Contains(state.CharTag.Marker))
					continue; // skip character tokens that Paratext says are publishable, but that we would never want to record.
				if (state.ParaTag != null && !MarkerIsReadable(state.ParaTag))
					continue; // skip any undesired paragraph types

				if ((state.ParaStart && (ProjectSettings.BreakAtParagraphBreaks ||
					previousTextType != ScrTextType.scVerseText || state.ParaTag?.TextType != ScrTextType.scVerseText))
					|| t.Marker == "c" || t.Marker == "qs" || previousMarker == "qs" || previousMarker == "d" || previousMarker == "c")
					// Even though \qs (Selah) is really a character style, we want to treat it like a separate paragraph.
					// \d is "Heading - Descriptive Title - Hebrew Subtitle" (TextType is VerseText)
					// A chapter can (rarely) occur mid-paragraph, but since a "c" gets treated as a paragraph,
					// We need to re-open a new verse text paragraph to contain any subsequent verse text.
				{
					var isTitle = state.ParaTag != null && state.ParaTag.TextType == ScrTextType.scTitle;
					if (!isTitle || !inTitle)
					{
						// If we've been collecting chapter info and we're starting a new paragraph that we'll need to write out
						// then we need to emit our chapter string first.
						// [\cl and \cp have TextProperty paragraph, and IsPublishable is true,
						// but they DON'T have TextProperty Vernacular!]
						if (collectingChapterInfo && ((state.ParaTag == null && state.IsPublishable)
							||state.ParaTag.TextProperties.HasFlag(TextProperties.scVernacular)))
						{
							EmitChapterString(paragraph, chapterLabelScopeIsBook, chapterLabelIsSupplied, chapterCharacterIsSupplied,
								chapterLabel, chapterCharacter);
							collectingChapterInfo = false;
							lookingForChapterCharacter = false;
							lookingForChapterLabel = false;
						}

						if (paragraph.HasData)
						{
							chapterLines.AddRange(paragraph.BreakIntoBlocks(StylesToSkipByDefault.Contains(paragraph.State.Name)));
						}

						paragraph.StartNewParagraph(state, t.Marker == "c");
						var styleName = t.Marker == "qs" ? state.CharTag.Name : state.ParaTag?.Name;
						if (styleName != null)
						{
							lock (_allEncounteredParagraphStyleNames)
								_allEncounteredParagraphStyleNames.Add(styleName);
						}

						if (currentChapter1Based == 0)
							versesPerChapter[0]++; // this helps to show that there is some content in the intro
					}
					else if (isTitle && t.Marker == "mt")
					{
						// We may have gotten a secondary or tertiary title before. Now we have the main title,
						// so we want to note that in the paragraph.
						paragraph.ImproveState(state);
					}

					inTitle = isTitle;
				}

				switch (t.Marker)
				{
					case null: // This is most likely a text node
						var tokenText = tokens[i].Text;

						if (!string.IsNullOrEmpty(tokenText))
						{
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

							if (processingGlossaryWord)
							{
								int barPos = tokenText.IndexOf('|');
								if (barPos >= 0)
									tokenText = tokenText.Substring(0, barPos).TrimEnd();
								processingGlossaryWord = false;
							}

							if (lookingForVerseText)
							{
								lookingForVerseText = false;
								versesPerChapter[currentChapter1Based]++;

								if (paragraph.State == null && _paratextProject is ParatextScripture ptProject)
								{
									// HT-415: this is (as the name implies) a hack to allow HearThis
									// to load a Paratext book that is missing paragraph markers after the \c.
									paragraph.ForceNewParagraph(ptProject.DefaultScriptureParaTag);
								}
							}

							if (inTitle && paragraph.HasData)
								paragraph.AddHardLineBreak();

							// HT-420: Although it is illegal USFM, Paratext allows text to occur
							// after a chapter number and it is not currently flagged as an error
							// by the Basic checks. However, HT can't handle it and we would never
							// want to record it.
							if (previousMarker != "c")
								paragraph.Add(tokenText);
						}

						break;
					case "v":
						paragraph.NoteVerseStart(t.Data.Trim());
						// Empty \v markers don't count. Set a flag and wait for actual contents
						lookingForVerseText = true;
						break;
					case "c":
						paragraph.NoteChapterStart();
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
							PopulateSkippedFlag(bookNumber0Based, currentChapter1Based, chapterLines);
							var chapterString = t.Data.Trim();
							currentChapter1Based = int.Parse(chapterString);
							chapterLines = GetNewChapterLines(bookData, currentChapter1Based);
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
					case "w":
					case "+w":
						processingGlossaryWord = true;
						break;
					default: // Some other Marker
						break;
				}
			}

			// emit the last paragraph's lines
			if (paragraph.HasData)
			{
				chapterLines.AddRange(paragraph.BreakIntoBlocks(StylesToSkipByDefault.Contains(paragraph.State.Name)));
			}

			PopulateSkippedFlag(bookNumber0Based, currentChapter1Based, chapterLines);
		}

		public override string ProjectFolderName => _paratextProject.Name;

		public IScrProjectSettings ScrProjectSettings => _paratextProject;

		public override IEnumerable<string> AllEncounteredParagraphStyleNames
		{
			get
			{
				lock (_allEncounteredParagraphStyleNames)
					return _allEncounteredParagraphStyleNames;
			}
		}

		public override bool NestedQuotesEncountered => _sentenceSplitter.NestedQuotesEncountered;

		public override IBibleStats VersificationInfo => _versificationInfo;

		private void EmitChapterString(ParatextParagraph paragraph, bool labelScopeIsBook, bool labelIsSupplied,
			bool characterIsSupplied, string chapLabel, string chapCharacter)
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
			var isParagraph = tag.TextProperties.HasFlag(TextProperties.scParagraph);
			if (isParagraph && isPublishable && !_furtherParagraphIgnorees.Contains(tag.Marker))
				return true;
			return tag.TextProperties.HasFlag(TextProperties.scChapter);
		}

		private List<ScriptLine> GetNewChapterLines(Dictionary<int, List<ScriptLine>> bookData, int currentChapter1Based)
		{
			var chapterLines = new List<ScriptLine>();
			bookData[currentChapter1Based] = chapterLines;
			return chapterLines;
		}

		private int[] GetArrayForVersesPerChapter(int bookNumber0Based)
		{
			int[] versesPerChapter;
			lock (_chapterVerseCount)
			{
				if (!_chapterVerseCount.TryGetValue(bookNumber0Based, out versesPerChapter))
				{
					versesPerChapter = new int[200];
					_chapterVerseCount[bookNumber0Based] = versesPerChapter;
				}
			}
			return versesPerChapter;
		}

#if DEBUG
		// Useful for debugging tests
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			lock (_script)
			{
				foreach (var chapterLines in _script.Values)
				{
					lock (chapterLines)
					{
						foreach (var chapterLine in chapterLines.Values)
						{
							foreach (var scriptLine in chapterLine)
							{
								if (scriptLine.Skipped)
									sb.Append("{Skipped} ");
								sb.Append(scriptLine.Text).Append(Environment.NewLine);
							}
						}
					}
				}
			}
			return sb.ToString();
		}
#endif
	}
}
