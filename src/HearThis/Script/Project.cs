// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International. All Rights Reserved.
// <copyright from='2011' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Publishing;
using SIL.IO;
using static HearThis.Script.BibleStatsBase;

namespace HearThis.Script
{
	public class Project : ISkippedStyleInfoProvider, IPublishingInfoProvider
	{
		public const string InfoTxtFileName = "info.txt";
		private BookInfo _selectedBook;
		private ChapterInfo _selectedChapterInfo;
		public List<BookInfo> Books { get; }
		private readonly ScriptProviderBase _scriptProvider;
		private int _selectedScriptLine;
		public event EventHandler SelectedBookChanged;
		public event EventHandler ExtraClipsCollectionChanged;

		public event ScriptBlockChangedHandler ScriptBlockRecordingRestored;
		public delegate void ScriptBlockChangedHandler(Project sender, int book, int chapter, ScriptLine scriptBlock);

		public event SkippedStylesChangedHandler SkippedStylesChanged;
		public delegate void SkippedStylesChangedHandler(Project sender, string styleName, bool newSkipValue);

		public Project(ScriptProviderBase scriptProvider)
		{
			_scriptProvider = scriptProvider;
			ProjectSettings = _scriptProvider.ProjectSettings;
			VersificationInfo = _scriptProvider.VersificationInfo;
			_scriptProvider.ScriptBlockUnskipped += OnScriptBlockUnskipped;
			_scriptProvider.SkippedStylesChanged += OnSkippedStylesChanged;
			Name = _scriptProvider.ProjectFolderName;
			Books = new List<BookInfo>(_scriptProvider.VersificationInfo.BookCount);

			if (Settings.Default.Book < 0 || Settings.Default.Book >= BibleStatsBase.kCanonicalBookCount)
				Settings.Default.Book = 0;
			for (int bookNumber = 0; bookNumber < _scriptProvider.VersificationInfo.BookCount; ++bookNumber)
			{
				var bookInfo = new BookInfo(Name, bookNumber, _scriptProvider);
				Books.Add(bookInfo);
				if (bookNumber == Settings.Default.Book)
					SelectedBook = bookInfo;
			}

			// This "works" to hide the OT or NT line of books if nothing is translated for any
			// book in that testament. However, it forces all books to be loaded, which thwarts the
			// lazy loading that allows the UI to come up more quickly, so it probably isn't worth
			// it. What we could perhaps do is use events to keep track of books being loaded and
			// then, once all books are loaded, call
			// RecordingToolControl.ShowBookButtonsOnlyForTestamentsWithContent.
			//if (Books.Count > kCountOfOTBooks)
			//{
			//	if (Books.Take(kCountOfOTBooks).All(b => !b.HasTranslatedContent))
			//	{
			//		// Don't include/show OT books if none of them have content
			//		Books.RemoveRange(0, kCountOfOTBooks);
			//		if (Settings.Default.Book < kCountOfOTBooks)
			//		{
			//			SelectedBook = Books[0];
			//			Settings.Default.Book = SelectedBook.BookNumber;
			//		}
			//	}
			//	else if (Books.Skip(kCountOfOTBooks).All(b => !b.HasTranslatedContent))
			//	{
			//		// Don't include/show NT books if none of them have content
			//		Books.RemoveRange(kCountOfOTBooks, Books.Count - kCountOfOTBooks);
			//		if (Settings.Default.Book >= kCountOfOTBooks)
			//		{
			//			SelectedBook = Books[0];
			//			Settings.Default.Book = SelectedBook.BookNumber;
			//		}
			//	}
			//}
			// See https://community.scripture.software.sil.org/t/not-all-of-the-text-is-visible/4116/3
			// For multivoice (glyssenscript-based) projects, we can really quickly determine
			// whether books exist in the script or not, so to minimize the visually jarring
			// effect of potentially later hiding a testament's worth of books, we can simply
			// remove all the books for either testament if none of the books for that testament
			// are present in the script.
			if (scriptProvider is MultiVoiceScriptProvider mvsp && Books.Count > kCountOfOTBooks)
			{
				if (Books.Take(kCountOfOTBooks).All(b => !mvsp.BookExistsInScript(b.BookNumber)))
				{
					// Don't include/show OT books if none of them have content
					Books.RemoveRange(0, kCountOfOTBooks);
					if (Settings.Default.Book < kCountOfOTBooks)
					{
						SelectedBook = Books[0];
						Settings.Default.Book = SelectedBook.BookNumber;
					}
				}
				else if (Books.Skip(kCountOfOTBooks).All(b => !mvsp.BookExistsInScript(b.BookNumber)))
				{
					// Don't include/show NT books if none of them have content
					Books.RemoveRange(kCountOfOTBooks, Books.Count - kCountOfOTBooks);
					if (Settings.Default.Book >= kCountOfOTBooks)
					{
						SelectedBook = Books[0];
						Settings.Default.Book = SelectedBook.BookNumber;
					}
				}
			}

		}

		public ProjectSettings ProjectSettings { get; }

		public BookInfo SelectedBook
		{
			get => _selectedBook;
			set
			{
				if (_selectedBook != value)
				{
					_selectedBook = value;
					_scriptProvider.LoadBook(_selectedBook.BookNumber);
					GoToInitialChapter();

					Settings.Default.Book = value.BookNumber;

					SelectedBookChanged?.Invoke(this, new EventArgs());
				}
			}
		}

		internal void SaveProjectSettings()
		{
			_scriptProvider.SaveProjectSettings();
		}

		/// <summary>
		/// Return the content of the info.txt file we create to help HearThisAndroid.
		/// It contains a line for each book.
		/// Each line contains BookName;blockcount:recordedCount,... for each chapter
		/// (Not filtered by character.)
		/// </summary>
		internal string GetProjectRecordingStatusInfoFileContent()
		{
			var sb = new StringBuilder();
			for (int iBook = 0; iBook < Books.Count; iBook++)
			{
				var book = Books[iBook];
				var bookName = book.Name;
				sb.Append(bookName);
				sb.Append(";");
				//sb.Append(book.ChapterCount);
				//sb.Append(";");
				//sb.Append(book.HasVerses ? "y" : "n");
				//sb.Append(";");
				if (!book.HasTranslatedContent)
				{
					sb.AppendLine("");
					continue;
				}
				for (int iChap = 0; iChap <= _scriptProvider.VersificationInfo.GetChaptersInBook(iBook); iChap++)
				{
					var chap = book.GetChapter(iChap);
					var lines = chap.UnfilteredScriptBlockCount;
					if (iChap != 0)
						sb.Append(',');
					sb.Append(lines);
					sb.Append(":");
					sb.Append(chap.CalculateUnfilteredPercentageTranslated());
					//for (int iline = 0; iline < lines; iline++)
					//	_lineRecordingRepository.WriteLineText(projectName, bookName, ichap, iline,
					//		chap.GetScriptLine(iline).Text);
				}
				sb.AppendLine("");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Gets the full path of the info.txt file where we will store the information
		/// we collect to help HearThisAndroid
		/// </summary>
		public string GetProjectRecordingStatusInfoFilePath()
		{
			return Path.Combine(Program.GetApplicationDataFolder(Name), InfoTxtFileName);
		}

		public bool IsRealProject => !(_scriptProvider is SampleScriptProvider);

		public string EthnologueCode => _scriptProvider.EthnologueCode;

		public bool RightToLeft => _scriptProvider.RightToLeft;

		public string FontName => _scriptProvider.FontName;

		public string CurrentBookName => _selectedBook.Name;

		public bool IncludeBook(string bookName)
		{
			int bookIndex = _scriptProvider.VersificationInfo.GetBookNumber(bookName);
			if (bookIndex < 0)
				return false;
			for (int iChapter = 0; iChapter < _scriptProvider.VersificationInfo.GetChaptersInBook(bookIndex); iChapter++)
			{
				if (_scriptProvider.GetUnfilteredTranslatedVerseCount(bookIndex, iChapter) > 0)
					return true;
			}
			return false;
		}

		public ScriptLine GetUnfilteredBlock(string bookName, int chapterNumber, int lineNumber0Based)
		{
			return _scriptProvider.GetUnfilteredBlock(_scriptProvider.VersificationInfo.GetBookNumber(bookName), chapterNumber, lineNumber0Based);
		}

		public ScriptLine GetUnfilteredBlock(int index)
		{
			if (index < 0 || index >= LineCountForChapter)
				return null;
			return SelectedBook.GetUnfilteredBlock(SelectedChapterInfo.ChapterNumber1Based, index);
		}

		public bool IsExtraBlockSelected =>
			IndexIntoExtraRecordings >= 0 && IndexIntoExtraRecordings < ExtraRecordings.Count;

		public ScriptLine GetRecordingInfoOfSelectedExtraBlock => IsExtraBlockSelected ?
			ExtraRecordings[IndexIntoExtraRecordings].RecordingInfo : null;

		public IBibleStats VersificationInfo { get; }

		public int BookNameComparer(string x, string y)
		{
			return Comparer.Default.Compare(_scriptProvider.VersificationInfo.GetBookNumber(x),
				_scriptProvider.VersificationInfo.GetBookNumber(y));
		}

		public bool BreakQuotesIntoBlocks => ProjectSettings.BreakQuotesIntoBlocks;

		/// <summary>
		/// Note that this is NOT the same as ProjectSettings.AdditionalBlockBreakCharacters.
		/// This property is implemented especially to support publishing and may include
		/// additional characters not stored in the project setting by the same name.
		/// </summary>
		string IPublishingInfoProvider.BlockBreakCharacters
		{
			get
			{
				var bldr = new StringBuilder();
				foreach (var c in _scriptProvider.AllEncounteredSentenceEndingCharacters)
				{
					bldr.Append(c);
					bldr.Append(" ");
				}
				bldr.Append(ProjectSettings.AdditionalBlockBreakCharacters);
				var firstLevelStartQuotationMark = ScrProjectSettings?.FirstLevelStartQuotationMark;
				if (BreakQuotesIntoBlocks && !String.IsNullOrEmpty(firstLevelStartQuotationMark))
				{
					if (bldr.Length > 0)
						bldr.Append(" ");
					bldr.Append(firstLevelStartQuotationMark);
					var firstLevelEndQuotationMark = ScrProjectSettings.FirstLevelEndQuotationMark;
					if (firstLevelStartQuotationMark != firstLevelEndQuotationMark)
						bldr.Append(" ").Append(firstLevelEndQuotationMark);
				}

				if (bldr.Length > 1 && bldr[bldr.Length - 1] == ' ')
					bldr.Remove(bldr.Length - 1, 1);
				return bldr.ToString();
			}
		}

		public void GoToInitialChapter()
		{
			if (_selectedChapterInfo == null &&
				Settings.Default.Chapter >= SelectedBook.FirstChapterNumber && Settings.Default.Chapter <= SelectedBook.ChapterCount)
			{
				// This is the very first time for this project. In this case rather than going to the start of the book,
				// we want to go back to the chapter the user was in when they left off last time.
				SelectedChapterInfo = SelectedBook.GetChapter(Settings.Default.Chapter);
			}
			else
			{
				SelectedChapterInfo = _selectedBook.GetFirstChapter();
			}
		}

		public ChapterInfo SelectedChapterInfo
		{
			get => _selectedChapterInfo;
			set
			{
				if (_selectedChapterInfo != value)
				{
					_selectedChapterInfo = value;
					Settings.Default.Chapter = value.ChapterNumber1Based;
					SelectedScriptBlock = 0;
				}
			}
		}

		/// <summary>
		/// This is a portion of the Scripture text that is to be recorded as a single clip. Blocks are broken up by paragraph breaks and
		/// sentence-final punctuation, not verses. This is a 0-based index.
		/// Project.SelectedScriptBlock is unfiltered (that is, an index into all the blocks in the chapter, not into
		/// the current-character list).
		/// </summary>
		public int SelectedScriptBlock
		{
			get => _selectedScriptLine;
			set
			{
				_selectedScriptLine = value;
				SendFocus();
			}
		}

		private int IndexIntoExtraRecordings => ExtraRecordings.Count == 0 ? -1 : SelectedScriptBlock - LineCountForChapter;

		public bool IsExtraClipSelected => IndexIntoExtraRecordings >= 0;

		public string ClipFilePathForSelectedLine => IndexIntoExtraRecordings >= 0 ?
			ExtraRecordings[IndexIntoExtraRecordings].ClipFile : GetPathToRecordingForSelectedLine();

		public bool SelectedLineHasClip => File.Exists(ClipFilePathForSelectedLine);

		public Dictionary<string, string> GetAudioContextInfoForAnalytics(ScriptLine currentScriptLine)
		{
			return new Dictionary<string, string>
			{
				{"book", SelectedBook.Name},
				{"chapter", SelectedChapterInfo.ChapterNumber1Based.ToString()},
				{"scriptBlock", SelectedScriptBlock.ToString()},
				{"wordsInLine", (currentScriptLine?.ApproximateWordCount ?? 0).ToString()},
				{"IndexIntoExtraRecordings", IndexIntoExtraRecordings.ToString()}
			};
		}

		public string FontNameForSelectedBlock => ScriptOfSelectedBlock?.FontName ?? FontName;

		/// <summary>
		/// Normally this is simply ScriptOfSelectedBlock.FontSize. However, when the
		/// selected block is an "extra" block for which no script line exists,
		/// we look backwards to find a preceding non-heading block to use as a basis
		/// for returning a "standard" size. Failing that, we just return a hard-coded
		/// default.
		/// </summary>
		public int FontSizeForSelectedBlock
		{
			get
			{
				var script = ScriptOfSelectedBlock;
				if (script != null)
					return script.FontSize;

				for (var i = _selectedScriptLine - 1; i >= 0; i--)
				{
					script = SelectedBook.GetUnfilteredBlock(SelectedChapterInfo.ChapterNumber1Based, i);
					if (script != null && !script.Heading)
						return script.FontSize;
				}
				return 12;
			}
		}

		private void SendFocus()
		{
			if (SelectedBook == null || SelectedBook.BookNumber >= _scriptProvider.VersificationInfo.BookCount
				|| SelectedChapterInfo == null || SelectedScriptBlock >= SelectedChapterInfo.UnfilteredScriptBlockCount)
				return;
			var abbr = _scriptProvider.VersificationInfo.GetBookCode(SelectedBook.BookNumber);
			var block = ScriptOfSelectedBlock;
			var verse = block.Verse ?? "";
			int i = verse.IndexOfAny(new[] {'-', '~'});
			if (i > 0)
				verse = verse.Substring(0, i);
			var targetRef = $"{abbr} {SelectedChapterInfo.ChapterNumber1Based}:{verse}";
			ParatextFocusHandler.SendFocusMessage(targetRef);
		}

		public string Name { get; }

		public IScrProjectSettings ScrProjectSettings
		{
			get
			{
				var paratextScriptProvider = _scriptProvider as IScrProjectSettingsProvider;
				return paratextScriptProvider?.ScrProjectSettings;
			}
		}

		public bool HasNestedQuotes => _scriptProvider.NestedQuotesEncountered;

		public IEnumerable<string> AllEncounteredParagraphStyleNames =>
			_scriptProvider.AllEncounteredParagraphStyleNames;

		public IActorCharacterProvider ActorCharacterProvider => _scriptProvider as IActorCharacterProvider;

		public string CurrentCharacter => ActorCharacterProvider?.Character;

		// Unfiltered by character
		public int LineCountForChapter => _selectedChapterInfo.UnfilteredScriptBlockCount;

		private IReadOnlyList<ExtraRecordingInfo> ExtraRecordings => SelectedChapterInfo.GetExtraClips();

		public int TotalCountOfBlocksAndExtraClipsForChapter =>
			LineCountForChapter + ExtraRecordings.Count;

		public string PathToLastClipRecorded { get; private set; }

		public bool HasRecordedClip(int line)
		{
			if (line >= LineCountForChapter)
			{
				var indexExtra = line - LineCountForChapter;
				return ExtraRecordings.Count > indexExtra && File.Exists(ExtraRecordings[indexExtra].ClipFile);
			}

			return HasClipForUnfilteredScriptLine(line);
		}

		private bool HasClipForUnfilteredScriptLine(int block)
		{
			Debug.Assert(block < LineCountForChapter);
			return ClipRepository.HasClipUnfiltered(Name, SelectedBook.Name,
				SelectedChapterInfo.ChapterNumber1Based, block);
		}

		public bool IsHole(int block) =>
			block >= 0 && block < LineCountForChapter &&
			!HasClipForUnfilteredScriptLine(block) && !GetBlock(block).Skipped;

		private ScriptLine GetBlock(int block) =>
			ScriptProvider.GetBlock(SelectedBook.BookNumber,
				SelectedChapterInfo.ChapterNumber1Based, block);

		public bool HasRecordedClipForSelectedScriptLine() =>
			SelectedScriptBlock != LineCountForChapter && HasClipForUnfilteredScriptLine(SelectedScriptBlock);

		public void LoadBook(int bookNumber0Based)
		{
			_scriptProvider.LoadBook(bookNumber0Based);
		}

		public bool HasProblemNeedingAttention(string bookName = null)
		{
			if (bookName == null)
				return Books.Any(bookInfo => (bookInfo.GetWorstProblemInBook() & ProblemType.Major) > 0);


			return (Books.Single(b => b.Name == bookName).GetWorstProblemInBook() & ProblemType.Major) > 0;
		}

		private ScriptLine GetRecordingInfo(int i) => SelectedChapterInfo.Recordings.FirstOrDefault(r => r.Number == i + 1);
		private string GetCurrentScriptText(int i) => SelectedBook.GetBlock(SelectedChapterInfo.ChapterNumber1Based, i).Text;

		public ScriptLine GetCurrentOrDeletedRecordingInfo(int oneBasedBlockNumber) =>
			SelectedChapterInfo?.Recordings.FirstOrDefault(r => r.Number == oneBasedBlockNumber) ??
			SelectedChapterInfo?.DeletedRecordings?.FirstOrDefault(r => r.Number == oneBasedBlockNumber);

		public bool DoesCurrentSegmentHaveProblem()
		{
			return (SelectedScriptBlock != LineCountForChapter || ExtraRecordings.Any()) &&
				DoesSegmentHaveProblems(SelectedScriptBlock, true);
		}

		public bool DoesSegmentHaveProblems(int i, bool treatLackOfInfoAsProblem = false)
		{
			var scriptLine = GetUnfilteredBlock(i);
			if (scriptLine == null)
				return true;
			var recordingInfo = GetRecordingInfo(i);
			if (scriptLine.Skipped && HasRecordedClip(i))
				return true;
			if (recordingInfo == null)
				return treatLackOfInfoAsProblem && HasRecordedClipForSelectedScriptLine();

			var currentText = GetCurrentScriptText(i);
			// In rare instances, the text may be subsequently reverted back to the way it
			// was when the clip was originally recorded; this should not be treated as a problem.
			return recordingInfo.Text != currentText && recordingInfo.OriginalText != currentText;
		}

		public bool DoesSegmentHaveIgnoredProblem(int i)
		{
			var recordingInfo = GetRecordingInfo(i);
			return recordingInfo?.OriginalText != null && recordingInfo.OriginalText != GetCurrentScriptText(i);
		}

		public void RefreshExtraClips()
		{
			if (SelectedChapterInfo != null)
			{
				var prevCount = SelectedChapterInfo.GetExtraClips(false).Count;
				
				if (SelectedChapterInfo.GetExtraClips(true).Count < prevCount)
				{
					SelectedScriptBlock = Math.Min(SelectedScriptBlock, TotalCountOfBlocksAndExtraClipsForChapter - 1);
					ExtraClipsCollectionChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		internal ChapterInfo GetNextChapterInfo()
		{
			var currentChapNum = SelectedChapterInfo.ChapterNumber1Based;
			if (currentChapNum == SelectedBook.ChapterCount)
				throw new ArgumentOutOfRangeException("Tried to get too high a chapter number.");

			return SelectedBook.GetChapter(currentChapNum + 1);
		}

		internal int GetNextChapterNum()
		{
			return GetNextChapterInfo().ChapterNumber1Based;
		}

		internal string GetPathToRecordingForSelectedLine()
		{
			return ClipRepository.GetPathToLineRecordingUnfiltered(Name, SelectedBook.Name,
				SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock);
		}

		internal string ProjectFolder => ClipRepository.GetProjectFolder(Name);

		public void SetSkippedStyle(string style, bool skipped)
		{
			_scriptProvider.SetSkippedStyle(style, skipped);
		}

		public bool IsSkippedStyle(string style) => _scriptProvider.IsSkippedStyle(style);

		public IReadOnlyList<string> StylesToSkipByDefault => _scriptProvider.StylesToSkipByDefault;

		public void ClearAllSkippedBlocks()
		{
			_scriptProvider.ClearAllSkippedBlocks(Books);
		}

		public IScriptProvider ScriptProvider => _scriptProvider;

		private void OnScriptBlockUnskipped(IScriptProvider sender, int bookNumber, int chapterNumber, ScriptLine scriptBlock)
		{
			// passing an unfiltered scriptBlockNumber, so do NOT pass a script provider so it won't be adjusted
			if (ClipRepository.RestoreBackedUpClip(Name, Books[bookNumber].Name, chapterNumber, scriptBlock.Number - 1))
				ScriptBlockRecordingRestored?.Invoke(this, bookNumber, chapterNumber, scriptBlock);
		}

		private void OnSkippedStylesChanged(IScriptProvider sender, string styleName, bool newSkipValue)
		{
			SkippedStylesChanged?.Invoke(this, styleName, newSkipValue);
		}

		/// <summary>
		/// Strictly speaking, skipped lines are not recordable, but we leave the button semi-enabled (grayed out
		/// and doesn't actually allow recording, but it does give the user a message if clicked),
		/// so we want to treat them as "recordable" if skipping is the only thing standing in the way.
		/// Similarly, if we're in overview mode, nothing can currently be recorded, but we want to show things that could be
		/// if we were in the right character as recordable.
		/// </summary>
		/// <param name="book">Scripture book number, 0-based</param>
		/// <param name="chapterNumber1Based">1-based (0 represents the introduction)</param>
		/// <param name="lineNo0Based">0-based (does not necessarily/typically correspond to verse
		/// numbers). When called for a project that uses a filtered set of blocks, this should
		/// be the unfiltered block number.
		/// </param>
		/// <returns>A value indicating whether the specified book, chapter and line refer to a
		/// location that a) exists in the script and b) is not filtered out for the currently
		/// selected Actor/Character (if any).</returns>
		public bool IsLineCurrentlyRecordable(int book, int chapterNumber1Based, int lineNo0Based)
		{
			var line = _scriptProvider.GetUnfilteredBlock(book, chapterNumber1Based, lineNo0Based);
			if (string.IsNullOrEmpty(line?.Text))
				return false;
			if (ActorCharacterProvider == null || ActorCharacterProvider.Character == null)
				return true; // no filtering (or overview mode).
			return line.Character == ActorCharacterProvider.Character && line.Actor == ActorCharacterProvider.Actor;
		}

		public ScriptLine ScriptOfSelectedBlock =>
			SelectedBook.GetUnfilteredBlock(SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock);

		/// <summary>
		/// Deletes the clip for the selected block.
 		/// </summary>
		public bool DeleteClipForSelectedBlock()
		{
			if (SelectedScriptBlock >= LineCountForChapter)
			{
				var index = ClipRepository.GetAdjustedIndexForExtraRecordingBasedOnDeletedClips(
					ExtraRecordings, SelectedScriptBlock - LineCountForChapter);
				var fileNumber = ClipRepository.DeleteExtraRecording(LineCountForChapter, Name,
					CurrentBookName, SelectedChapterInfo.ChapterNumber1Based, index);
				if (fileNumber >= 0)
				{
					SelectedChapterInfo.OnExtraClipDeleted(fileNumber + 1);
					return true;
				}

				return false;
			}

			if (ClipRepository.DeleteLineRecording(Name, SelectedBook.Name,
				SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock))
			{
				SelectedChapterInfo.OnClipDeleted(ScriptOfSelectedBlock);
				return true;
			}

			return false;
		}

		public bool UndeleteClipForSelectedBlock()
		{
			if (ClipRepository.UndeleteLineRecording(Name, SelectedBook, SelectedChapterInfo,
				SelectedScriptBlock))
			{
				ScriptBlockRecordingRestored?.Invoke(this, SelectedBook.BookNumber, SelectedChapterInfo.ChapterNumber1Based, ScriptOfSelectedBlock);
				return true;
			}

			return false;
		}

		public List<ScriptLine> GetRecordableBlocksUpThroughNextHoleToTheRight()
		{
			var indices = new List<int>();
			for (var i = SelectedScriptBlock; i < LineCountForChapter; i++)
				indices.Add(i);
			return GetRecordableBlocksUpThroughHole(indices);
		}

		public List<ScriptLine> GetRecordableBlocksAfterPreviousHoleToTheLeft()
		{
			var indices = new List<int>();
			for (var i = SelectedScriptBlock; i >= 0; i--)
				indices.Add(i);
			return GetRecordableBlocksUpThroughHole(indices, true);
		}

		private List<ScriptLine> GetRecordableBlocksUpThroughHole(IEnumerable<int> indices, bool reverseList = false)
		{
			var bookInfo = SelectedBook;
			var chapter = SelectedChapterInfo.ChapterNumber1Based;
			var lines = new List<ScriptLine>();
			foreach (var i in indices)
			{
				if (!IsLineCurrentlyRecordable(bookInfo.BookNumber, chapter, i))
					break;
				var block = bookInfo.ScriptProvider.GetBlock(bookInfo.BookNumber, chapter, i);
				if (block.Skipped)
					break;

				if (reverseList)
					lines.Insert(0, block);
				else
					lines.Add(block);
				if (!ClipRepository.HasClip(Name, bookInfo.Name, chapter, i, ScriptProvider))
					return lines;
			}
			return new List<ScriptLine>();
		}

		public void HandleSoundFileCreated()
		{
			var clipPath = GetPathToRecordingForSelectedLine();
			PathToLastClipRecorded = clipPath;
			// Getting this into a local variable is not only more efficient, it also
			// prevents an annoying problem when working with the sample project, whereby
			// re-getting the current script line loses information that has not yet been saved.
			var currentScriptLine = GetUnfilteredBlock(SelectedScriptBlock);
			if (currentScriptLine.Skipped)
			{
				var skipPath = Path.ChangeExtension(clipPath, ClipRepository.kSkipFileExtension);
				clipPath = null;
				if (File.Exists(skipPath))
				{
					try
					{
						RobustFile.Delete(skipPath);
					}
					catch (Exception e)
					{
						// Bummer. But we can probably ignore this.
						Analytics.ReportException(e);
					}
				}
			}

			currentScriptLine.OriginalText = null;

			bool isSelectedScriptBlockLastUnskippedInChapter =
				LineCountForChapter == SelectedScriptBlock + 1;
			if (isSelectedScriptBlockLastUnskippedInChapter)
				DeleteClipsBeyondLastClip();
			if (ActorCharacterProvider != null)
			{
				// We presume the recording just made was made by the current actor for the current character.
				// (Or if none has been set, they will correctly be null.)
				currentScriptLine.Actor = ActorCharacterProvider.Actor;
				currentScriptLine.Character = ActorCharacterProvider.Character;
			}
			else
			{
				// Probably redundant, but it MIGHT have been previously recorded with a known actor.
				currentScriptLine.Actor = currentScriptLine.Character = null;
			}
			currentScriptLine.RecordingTime = DateTime.UtcNow;
			SelectedChapterInfo.OnScriptBlockRecorded(currentScriptLine);

			if (clipPath != null && !File.Exists(clipPath))
				throw new FileNotFoundException("Corrupted file deleted", clipPath);
		}

		private void DeleteClipsBeyondLastClip()
		{
			ClipRepository.DeleteAllClipsAfterLine(Name, SelectedBook.Name,
				SelectedChapterInfo, SelectedScriptBlock);
			RefreshExtraClips();
		}

		public void HandleExtraBlocksShifted()
		{
			var lineNumberOfLastRealBlock = SelectedChapterInfo.GetScriptBlockCount() - 1;
			var scriptLine = GetBlock(lineNumberOfLastRealBlock);
			scriptLine.RecordingTime = GetActualClipRecordingTime(lineNumberOfLastRealBlock);
			SelectedChapterInfo.OnScriptBlockRecorded(scriptLine);
			RefreshExtraClips();
		}

		public DateTime GetActualClipRecordingTime(int lineNumber0Based) =>
			ClipRepository.GetActualCreationTimeOfLineRecording(Name, SelectedBook.Name,
				SelectedChapterInfo.ChapterNumber1Based, lineNumber0Based, ScriptProvider);

		public DateTime GetActualClipBackupRecordingTimeForSelectedBlock() =>
			ClipRepository.GetActualClipBackupRecordingTime(Name, SelectedBook.Name,
				SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock, ScriptProvider);

		public bool HasBackupFileForSelectedBlock()
		{
			return ClipRepository.HasBackupFile(Name, SelectedBook.Name,
				SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock);
		}

		public bool UndeleteLineRecordingForSelectedBlock()
		{
			return ClipRepository.UndeleteLineRecording(Name,
				SelectedBook, SelectedChapterInfo, SelectedScriptBlock);
		}
	}
}
