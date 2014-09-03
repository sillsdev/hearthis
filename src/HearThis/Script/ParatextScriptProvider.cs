// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using HearThis.Properties;
using Microsoft.Win32;
using Palaso.Code;
using Paratext;

namespace HearThis.Script
{
	public class ParatextScriptProvider : ScriptProviderBase
	{
		private const string ParaTExtRegistryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\ScrChecks\1.0\Settings_Directory";

		private readonly IScripture _paratextProject;
		private readonly Dictionary<int, Dictionary<int, List<ScriptLine>>> _script; // book <chapter, lines>
		private readonly Dictionary<int, int[]>  _chapterVerseCount; //book <chapter, verseCount>
		private const char kSpace = ' ';
		private HashSet<string> _allEncounteredParagraphStyleNames; // This will not included the ones that are always ignored.
		private IBibleStats _versificationInfo;

		// These are markers that ARE paragraph and IsPublishableVernacular, but we don't want to read them.
		// They should be followed by a single text node that will be skipped too.
		private readonly HashSet<string> _furtherParagraphIgnorees = new HashSet<string> { "id", "h", "h1", "h2", "h3", "r", "toc1", "toc2", "toc3", "io1","io2","io3" };

		// These are inline markers that we don't want to read.
		// They should be followed by a single text node that will be skipped too.
		private readonly ReadOnlyCollection<string> _furtherInlineIgnorees = new List<string> { "rq" }.AsReadOnly();
		private readonly SentenceClauseSplitter _sentenceSplitter;

		public static bool ParatextIsInstalled
		{
			get
			{
				var path = Registry.GetValue(ParaTExtRegistryKey, "", null);
				return path != null && Directory.Exists(path.ToString());
			}
		}

		public ParatextScriptProvider(IScripture paratextProject)
		{
			Guard.AgainstNull(paratextProject,"paratextProject");
			_paratextProject = paratextProject;
			_chapterVerseCount = new Dictionary<int, int[]>();
			_script = new Dictionary<int, Dictionary<int, List<ScriptLine>>>();
			_allEncounteredParagraphStyleNames = new HashSet<string>();
			_versificationInfo = new ParatextVersificationInfo(paratextProject.Versification);

			LoadSkipInfo();

			// Note... while one might think that char.GetUnicodeCategory could tell you if a character was a sentence separator, this is not the case.
			// This is because, for example, '.' can be used for various things (abbreviation, decimal point, as well as sentence terminator).
			// This should be a complete list of code points with the \p{Sentence_Break=STerm} or \p{Sentence_Break=ATerm} properties that also
			// have the \p{Terminal_Punctuation} property. This list is up-to-date as of Unicode v6.1.
			// ENHANCE: Ideally this should be dynamic.
			var separators = new[] { '.', '?', '!',
				'\u0589', // ARMENIAN FULL STOP
				'\u061F', // ARABIC QUESTION MARK
				'\u06D4', // ARABIC FULL STOP
				'\u0700', // SYRIAC END OF PARAGRAPH
				'\u0701', // SYRIAC SUPRALINEAR FULL STOP
				'\u0702', // SYRIAC SUBLINEAR FULL STOP
				'\u07F9', // NKO EXCLAMATION MARK
				'\u0964', // DEVANAGARI DANDA
				'\u0965', // DEVANAGARI DOUBLE DANDA
				'\u104A', // MYANMAR SIGN LITTLE SECTION
				'\u104B', // MYANMAR SIGN SECTION
				'\u1362', // ETHIOPIC FULL STOP
				'\u1367', // ETHIOPIC QUESTION MARK
				'\u1368', // ETHIOPIC PARAGRAPH SEPARATOR
				'\u166E', // CANADIAN SYLLABICS FULL STOP
				'\u1803', // MONGOLIAN FULL STOP
				'\u1809', // MONGOLIAN MANCHU FULL STOP
				'\u1944', // LIMBU EXCLAMATION MARK
				'\u1945', // LIMBU QUESTION MARK
				'\u1AA8', // TAI THAM SIGN KAAN
				'\u1AA9', // TAI THAM SIGN KAANKUU
				'\u1AAA', // TAI THAM SIGN SATKAAN
				'\u1AAB', // TAI THAM SIGN SATKAANKUU
				'\u1B5A', // BALINESE PANTI
				'\u1B5B', // BALINESE PAMADA
				'\u1B5E', // BALINESE CARIK SIKI
				'\u1B5F', // BALINESE CARIK PAREREN
				'\u1C3B', // LEPCHA PUNCTUATION TA-ROL
				'\u1C3C', // LEPCHA PUNCTUATION NYET THYOOM TA-ROL
				'\u1C7E', // OL CHIKI PUNCTUATION MUCAAD
				'\u1C7F', // OL CHIKI PUNCTUATION DOUBLE MUCAAD
				'\u203C', // DOUBLE EXCLAMATION MARK
				'\u203D', // INTERROBANG
				'\u2047', // DOUBLE QUESTION MARK
				'\u2048', // QUESTION EXCLAMATION MARK
				'\u2049', // EXCLAMATION QUESTION MARK
				'\u2E2E', // REVERSED QUESTION MARK
				'\u3002', // IDEOGRAPHIC FULL STOP
				'\uA4FF', // LISU PUNCTUATION FULL STOP
				'\uA60E', // VAI FULL STOP
				'\uA60F', // VAI QUESTION MARK
				'\uA6F3', // BAMUM FULL STOP
				'\uA6F7', // BAMUM QUESTION MARK
				'\uA876', // PHAGS-PA MARK SHAD
				'\uA877', // PHAGS-PA MARK DOUBLE SHAD
				'\uA8CE', // SAURASHTRA DANDA
				'\uA8CF', // SAURASHTRA DOUBLE DANDA
				'\uA92F', // KAYAH LI SIGN SHYA
				'\uA9C8', // JAVANESE PADA LINGSA
				'\uA9C9', // JAVANESE PADA LUNGSI
				'\uAA5D', // CHAM PUNCTUATION DANDA
				'\uAA5E', // CHAM PUNCTUATION DOUBLE DANDA
				'\uAA5F', // CHAM PUNCTUATION TRIPLE DANDA
				'\uAAF0', // MEETEI MAYEK CHEIKHAN
				'\uAAF1', // MEETEI MAYEK AHANG KHUDAM
				'\uABEB', // MEETEI MAYEK CHEIKHEI
				'\uFE52', // SMALL FULL STOP
				'\uFE56', // SMALL QUESTION MARK
				'\uFE57', // SMALL EXCLAMATION MARK
				'\uFF01', // FULLWIDTH EXCLAMATION MARK
				'\uFF0E', // FULLWIDTH FULL STOP
				'\uFF1F', // FULLWIDTH QUESTION MARK
				'\uFF61', // HALFWIDTH IDEOGRAPHIC FULL STOP
				// These would require surrogate pairs
				//'\u11047', // BRAHMI DANDA
				//'\u11048', // BRAHMI DOUBLE DANDA
				//'\u110BE', // KAITHI SECTION MARK
				//'\u110BF', // KAITHI DOUBLE SECTION MARK
				//'\u110C0', // KAITHI DANDA
				//'\u110C1', // KAITHI DOUBLE DANDA
				//'\u11141', // CHAKMA DANDA
				//'\u11142', // CHAKMA DOUBLE DANDA
				//'\u11143', // CHAKMA QUESTION MARK
				//'\u111C5', // SHARADA DANDA
				//'\u111C6', // SHARADA DOUBLE DANDA
			};
			_sentenceSplitter = new SentenceClauseSplitter(separators, Settings.Default.BreakQuotesIntoBlocks, paratextProject);
		}

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

				tokens = _paratextProject.GetUsfmTokens(verseRef, false);
				state = _paratextProject.CreateScrParserState(verseRef);
			}

			var paragraph = new ParatextParagraph(_sentenceSplitter, Settings.Default.ReplaceChevronsWithQuotes) { DefaultFont = _paratextProject.DefaultFont };
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

			for (var i = 0; i < tokens.Count; i++)
			{
				UsfmToken t = tokens[i];
				state.UpdateState(tokens, i);

				if (!state.IsPublishableVernacular || state.NoteTag != null)
					continue; // skip note text tokens and anything non-publishable
				if (state.CharTag != null && _furtherInlineIgnorees.Contains(state.CharTag.Marker))
					continue; // skip figure tokens
				if (state.ParaTag != null && !MarkerIsReadable(state.ParaTag))
					continue; // skip any undesired paragraph types

				if (state.ParaStart || t.Marker == "c" || t.Marker == "qs")
					// Even though \qs (Selah) is really a character style, we want to treat it like a separate paragraph.
				{
					var isTitle = state.ParaTag != null && state.ParaTag.TextType == ScrTextType.scTitle;
					if (!isTitle || !inTitle)
					{
						// If we've been collecting chapter info and we're starting a new paragraph that we'll need to write out
						// then we need to emit our chapter string first.
						// [\cl and \cp have TextProperty paragraph, and IsPublishableVernacular is true,
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
							chapterLines.AddRange(paragraph.BreakIntoBlocks());
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
						processingGlossaryWord = true;
						break;
					default: // Some other Marker
						break;
				}
			}
			// emit the last paragraph's lines
			if (paragraph.HasData)
			{
				chapterLines.AddRange(paragraph.BreakIntoBlocks());
			}
			PopulateSkippedFlag(bookNumber0Based, currentChapter1Based, chapterLines);
		}

		public override string EthnologueCode { get { return _paratextProject.EthnologueCode; } }

		public override string ProjectFolderName
		{
			get { return _paratextProject.Name; }
		}

		public override IEnumerable<string> AllEncounteredParagraphStyleNames
		{
			get
			{
				lock (_allEncounteredParagraphStyleNames)
					return _allEncounteredParagraphStyleNames;
			}
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
			var isVernacular = tag.TextProperties.HasFlag(TextProperties.scVernacular);
			var isParagraph = tag.TextProperties.HasFlag(TextProperties.scParagraph);
			if (isParagraph && isPublishable && isVernacular && !_furtherParagraphIgnorees.Contains(tag.Marker))
				return true;
			if (isParagraph && isPublishable && (tag.Marker == "cl" || tag.Marker == "cp"))
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
	}
}