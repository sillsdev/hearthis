using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using HearThis.Publishing;

namespace HearThis.Script
{
	public class Project : ISkippedStyleInfoProvider
	{
		private BookInfo _selectedBook;
		private ChapterInfo _selectedChapterInfo;
		public static readonly BibleStats Statistics = new BibleStats();
		public List<BookInfo> Books { get; set; }
		private readonly ScriptProviderBase _scriptProvider;
		private int _selectedScriptLine;

		public event ScriptBlockChangedHandler OnScriptBlockRecordingRestored;
		public delegate void ScriptBlockChangedHandler(Project sender, int book, int chapter, ScriptLine scriptBlock);

		public Project(ScriptProviderBase scriptProvider)
		{
			_scriptProvider = scriptProvider;
			_scriptProvider.OnScriptBlockUnskipped += OnScriptBlockUnskipped;
			Name = _scriptProvider.ProjectFolderName;
			Books = new List<BookInfo>();

			for (int bookNumber = 0; bookNumber < Statistics.BookNames.Count(); ++bookNumber)
			{
				Books.Add(new BookInfo(Name, bookNumber, _scriptProvider));
			}

			SelectedBook = Books.First();
		}

		public BookInfo SelectedBook
		{
			get { return _selectedBook; }
			set
			{
				if (_selectedBook != value)
				{
					_selectedBook = value;
					_scriptProvider.LoadBook(_selectedBook.BookNumber);
				   GotoInitialChapter();
				}
			}
		}

		public string EthnologueCode { get { return _scriptProvider.EthnologueCode; } }

		public void GotoInitialChapter()
		{
			SelectedChapterInfo = _selectedBook.GetFirstChapter();
		}

		public ChapterInfo SelectedChapterInfo
		{
			get { return _selectedChapterInfo; }
			set
			{
				if (_selectedChapterInfo != value)
				{
					_selectedChapterInfo = value;
					SelectedScriptBlock = 0;
				}
			}
		}

		/// <summary>
		/// This is a portion of the Scripture text that is to be recorded as a single clip. Blocks are broken up by paragraph breaks and
		/// sentence-final punctuation, not verses. This is a 0-based index.
		/// </summary>
		public int SelectedScriptBlock
		{
			get { return _selectedScriptLine; }
			set
			{
				_selectedScriptLine = value;
				SendFocus();
			}
		}

		private void SendFocus()
		{
			var threeLetterAbreviations = new BibleStats().ThreeLetterAbreviations;
			if (SelectedBook == null || SelectedBook.BookNumber >= threeLetterAbreviations.Count
				|| SelectedChapterInfo == null || SelectedScriptBlock >= SelectedChapterInfo.GetScriptBlockCount())
				return;
			var abbr = threeLetterAbreviations[SelectedBook.BookNumber];
			var block = SelectedBook.GetBlock(SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock);
			var targetRef = string.Format("{0} {1}:{2}", abbr, SelectedChapterInfo.ChapterNumber1Based, block.Verse);
			ParatextFocusHandler.SendFocusMessage(targetRef);
		}

		public string Name { get; set; }

		public bool HaveSelectedScript
		{
			get { return SelectedScriptBlock >= 0; }
		}

		public IEnumerable<string> AllEncounteredParagraphStyleNames
		{
			get
			{
				return _scriptProvider.AllEncounteredParagraphStyleNames;
			}
		}

		public int GetLineCountForChapter(bool includeSkipped)
		{
			if (includeSkipped)
				return _scriptProvider.GetScriptBlockCount(_selectedBook.BookNumber, _selectedChapterInfo.ChapterNumber1Based);
			return _scriptProvider.GetUnskippedScriptBlockCount(_selectedBook.BookNumber, _selectedChapterInfo.ChapterNumber1Based);
		}

		public void LoadBookAsync(int bookNumber0Based, Action action)
		{
			var worker = new BackgroundWorker();
			worker.DoWork += delegate
			{
				_scriptProvider.LoadBook(bookNumber0Based);
			};
			worker.RunWorkerCompleted += delegate { action(); };
			worker.RunWorkerAsync();
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
			return ClipRecordingRepository.GetPathToLineRecording(Name, SelectedBook.Name,
				SelectedChapterInfo.ChapterNumber1Based, SelectedScriptBlock);
		}

		public void SetSkippedStyle(string style, bool skipped)
		{
			_scriptProvider.SetSkippedStyle(style, skipped);
		}

		public bool IsSkippedStyle(string style)
		{
			return _scriptProvider.IsSkippedStyle(style);
		}

		public void ClearAllSkippedBlocks()
		{
			_scriptProvider.ClearAllSkippedBlocks(Books);
		}

		public void BackUpRecordingForSkippedLine()
		{
			var recordingPath = GetPathToRecordingForSelectedLine();
			File.Move(recordingPath, Path.ChangeExtension(recordingPath, "skip"));
		}

		void OnScriptBlockUnskipped(IScriptProvider sender, int bookNumber, int chapterNumber, ScriptLine scriptBlock)
		{
			var recordingPath = ClipRecordingRepository.GetPathToLineRecording(
				Name, Books[bookNumber].Name, chapterNumber, scriptBlock.Number - 1);
			var skipPath = Path.ChangeExtension(recordingPath, "skip");
			if (File.Exists(skipPath))
			{
				File.Move(skipPath, recordingPath);

				if (OnScriptBlockRecordingRestored != null)
					OnScriptBlockRecordingRestored(this, bookNumber, chapterNumber, scriptBlock);
			}
		}
	}
}