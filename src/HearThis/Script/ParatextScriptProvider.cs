// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
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
using SIL.Code;
using Paratext.Data;
using SIL.Scripture;

namespace HearThis.Script
{
	public class ParatextScriptProvider : ScriptProviderBase, IScrProjectSettingsProvider
	{
		private readonly IScripture _paratextProject;
		private readonly Dictionary<int, Dictionary<int, List<ScriptLine>>> _script; // book <chapter, lines>
		private readonly Dictionary<int, int[]> _chapterVerseCount; //book <chapter, verseCount>
		private const char kSpace = ' ';
		private HashSet<string> _allEncounteredParagraphStyleNames; // This will not include the ones that are always ignored.
		private IBibleStats _versificationInfo;

		/// <summary>
		/// These are markers that ARE paragraph and IsPublishable, but we don't want to read them.
		/// They should be followed by a single text node that will be skipped too. This list used
		/// to include some other markers that are not usually included in a recording but could be.
		/// See <see cref="ScriptProviderBase.StylesToSkipByDefault"/> for details.
		/// </summary>
		private readonly HashSet<string> _furtherParagraphIgnorees = new HashSet<string> {"id", "h", "h1", "h2", "h3", "toc1", "toc2", "toc3"};

		// These are inline markers that we don't want to read.
		// They should be followed by a single text node that will be skipped too.
		private readonly ReadOnlyCollection<string> _furtherInlineIgnorees = new List<string> { "rq" }.AsReadOnly();
		private readonly SentenceClauseSplitter _sentenceSplitter;

		public ParatextScriptProvider(IScripture paratextProject)
		{
			Guard.AgainstNull(paratextProject, "paratextProject");
			_paratextProject = paratextProject;
			_chapterVerseCount = new Dictionary<int, int[]>();
			_script = new Dictionary<int, Dictionary<int, List<ScriptLine>>>();
			_allEncounteredParagraphStyleNames = new HashSet<string>();
			_versificationInfo = new ParatextVersificationInfo(paratextProject.Versification);

			Initialize();

			char[] separators = null;
			string additionalBreakCharacters = ProjectSettings.AdditionalBlockBreakCharacters?.Replace(" ", string.Empty);
			if (!String.IsNullOrEmpty(additionalBreakCharacters))
				separators = additionalBreakCharacters.ToArray();
			_sentenceSplitter = new SentenceClauseSplitter(separators, ProjectSettings.BreakQuotesIntoBlocks, paratextProject);
		}

		public override string FontName
		{
			get
			{
				return _paratextProject.DefaultFont;
			}
		}

		public override bool RightToLeft
		{
			get { return _paratextProject.RightToLeft; }
		}

		public override string EthnologueCode => _paratextProject.EthnologueCode;

		protected override IStyleInfoProvider StyleInfo => _paratextProject.StyleInfo;

		/// <summary>
		/// The "block" is a bit of script (Book name, chapter #, section headings, etc.)
		/// </summary>
		/// <exception cref="KeyNotFoundException">Book or chapter not loaded or invalid number</exception>
		/// <exception cref="IndexOutOfRangeException">Block number out of range</exception>
		public override ScriptLine GetBlock(int bookNumber0Based, int chapterNumber, int blockNumber0Based)
		{
			lock (_script)
			{
				return _script[bookNumber0Based][chapterNumber][blockNumber0Based];
			}
		}

		public override void UpdateSkipInfo()
		{
			LoadSkipInfo();
			foreach (var bookKvp in _script)
			{
				foreach (var chapKvp in bookKvp.Value)
				{
					PopulateSkippedFlag(bookKvp.Key, chapKvp.Key, chapKvp.Value);
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
			lock (_script)
			{
				Dictionary<int, List<ScriptLine>> chapterLines;
				if (_script.TryGetValue(bookNumber0Based, out chapterLines))
				{
					List<ScriptLine> scriptLines;
					if (chapterLines.TryGetValue(chapter1Based, out scriptLines))
						return scriptLines;
				}
			}
			return new List<ScriptLine>();
		}

		public override int GetScriptBlockCount(int bookNumber0Based)
		{
			lock (_script)
			{
				Dictionary<int, List<ScriptLine>> chapterLines;
				return _script.TryGetValue(bookNumber0Based, out chapterLines) ?
					chapterLines.Sum(chapter => GetScriptBlockCount(bookNumber0Based, chapter.Key)) : 0;
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
			lock (_script)
			{
				if (_script.ContainsKey(bookNumber0Based))
				{
					return; //already loaded
				}

				_script.Add(bookNumber0Based, new Dictionary<int, List<ScriptLine>>()); // dictionary of chapter to lines

				var verseRef = new VerseRef(bookNumber0Based + 1, 1, 0 /*verse*/, _paratextProject.Versification);

				tokens = _paratextProject.GetUsfmTokens(verseRef);
				state = _paratextProject.CreateScrParserState(verseRef);
			}

			var paragraph = new ParatextParagraph(_sentenceSplitter) { DefaultFont = _paratextProject.DefaultFont, RightToLeft = _paratextProject.RightToLeft };
			var versesPerChapter = GetArrayForVersesPerChapter(bookNumber0Based);

			//Introductory lines, before the start of the chapter, will be in chapter 0
			int currentChapter1Based = 0;
			var chapterLines = GetNewChapterLines(bookNumber0Based, currentChapter1Based);
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
					|| t.Marker == "c" || t.Marker == "qs" || previousMarker == "qs" || previousMarker == "d")
					// Even though \qs (Selah) is really a character style, we want to treat it like a separate paragraph.
					// \d is "Heading - Descriptive Title - Hebrew Subtitle" (TextType is VerseText)
				{
					var isTitle = state.ParaTag != null && state.ParaTag.TextType == ScrTextType.scTitle;
					if (!isTitle || !inTitle)
					{
						// If we've been collecting chapter info and we're starting a new paragraph that we'll need to write out
						// then we need to emit our chapter string first.
						// [\cl and \cp have TextProperty paragraph, and IsPublishable is true,
						// but they DON'T have TextProperty Vernacular!]
						if (collectingChapterInfo && state.ParaTag.TextProperties.HasFlag(TextProperties.scVernacular))
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
						lock (_allEncounteredParagraphStyleNames)
							_allEncounteredParagraphStyleNames.Add(t.Marker == "qs" ? state.CharTag.Name : state.ParaTag.Name);
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
							}
							if (inTitle && paragraph.HasData)
								paragraph.AddHardLineBreak();
							paragraph.Add(tokenText);
						}
						break;
					case "v":
						paragraph.NoteVerseStart(t.Data[0].Trim());
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

		public override string ProjectFolderName
		{
			get { return _paratextProject.Name; }
		}

		public IScrProjectSettings ScrProjectSettings
		{
			get { return _paratextProject; }
		}

		public override IEnumerable<string> AllEncounteredParagraphStyleNames
		{
			get
			{
				lock (_allEncounteredParagraphStyleNames)
					return _allEncounteredParagraphStyleNames;
			}
		}

		public override bool NestedQuotesEncountered
		{
			get { return _sentenceSplitter.NestedQuotesEncountered; }
		}

		public override IBibleStats VersificationInfo
		{
			get { return _versificationInfo; }
		}

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

		private List<ScriptLine> GetNewChapterLines(int bookNumber1Based, int currentChapter1Based)
		{
			var chapterLines = new List<ScriptLine>();
			lock (_script)
				_script[bookNumber1Based][currentChapter1Based] = chapterLines;
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
			return sb.ToString();
		}
#endif
	}
}
