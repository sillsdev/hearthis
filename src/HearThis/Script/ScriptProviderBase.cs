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
using System.Diagnostics;
using System.IO;
using DesktopAnalytics;
using SIL.Xml;

namespace HearThis.Script
{
	public abstract class ScriptProviderBase : IScriptProvider, ISkippedStyleInfoProvider
	{
		private Tuple<int, int> _chapterHavingSkipFlagPopulated;
		private readonly Dictionary<int, Dictionary<int, Dictionary<int, ScriptLineIdentifier>>> _skippedLines = new Dictionary<int, Dictionary<int, Dictionary<int, ScriptLineIdentifier>>>();
		private string _skipfilePath;
		private List<string> _skippedParagraphStyles = new List<string>();

		public event ScriptBlockChangedHandler OnScriptBlockUnskipped;
		public delegate void ScriptBlockChangedHandler(IScriptProvider sender, int book, int chapter, ScriptLine scriptBlock);

		public abstract ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based);
		public abstract int GetScriptBlockCount(int bookNumber, int chapter1Based);
		public abstract int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based);
		public abstract int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based);
		public abstract int GetTranslatedVerseCount(int bookNumberDelegateSafe, int chapterNumber1Based);
		public abstract int GetScriptBlockCount(int bookNumber);
		public abstract void LoadBook(int bookNumber0Based);
		public abstract string EthnologueCode { get; }
		public abstract string FontName { get; }
		public abstract string ProjectFolderName { get; }
		public abstract IEnumerable<string> AllEncounteredParagraphStyleNames { get; }
		public abstract IBibleStats VersificationInfo { get; }
		public virtual bool NestedQuotesEncountered
		{
			get { return false; }
		}

		#region ISkippedStyleInfoProvider implementation and related methods
		protected string ProjectFolderPath
		{
			get { return Program.GetApplicationDataFolder(ProjectFolderName); }
		}

		public void ClearAllSkippedBlocks(IEnumerable<BookInfo> books)
		{
			lock (_skippedLines)
			{
				foreach (var bookInfo in books)
				{
					for (int iChapter = 0; iChapter < bookInfo.ChapterCount; iChapter++)
					{
						_chapterHavingSkipFlagPopulated = new Tuple<int, int>(bookInfo.BookNumber, iChapter);
						var chapterInfo = bookInfo.GetChapter(iChapter);
						for (int iBlock = 0; iBlock < chapterInfo.GetScriptBlockCount(); iBlock++)
						{
							bookInfo.GetBlock(iChapter, iBlock).Skipped = false;
						}
					}
				}
				_chapterHavingSkipFlagPopulated = null;
				_skippedLines.Clear();

				Save();
			}
		}

		protected void LoadSkipInfo()
		{
			lock (_skippedLines)
			{
				if (_skipfilePath != null)
					throw new InvalidOperationException("LoadSkipInfo should only be called once!");
				_skipfilePath = Path.Combine(ProjectFolderPath, "SkippedLineInfo.xml");
				var skippedLines = SkippedScriptLines.Create(_skipfilePath);
				foreach (var skippedLine in skippedLines.SkippedLinesList)
					AddSkippedLine(skippedLine);
				_skippedParagraphStyles = skippedLines.SkippedParagraphStyles;
				ScriptLine.SkippedStyleInfoProvider = this;
			}
		}

		/// <summary>
		/// Given a list of script lines in a particular book and chapter, this method will
		/// set the Skipped flag set if at some point in the past, AddSkippedLine was
		/// called for it and the verse and text still match.
		/// </summary>
		/// <param name="bookNumber">1-based book number</param>
		/// <param name="chapterNumber">1-based chapter number (where 0 represents the introduction)</param>
		/// <param name="scriptLines">Object representing the script line as provided by an
		/// IScriptProvider. This object's Skipped flag will (potentially) be modified by this
		/// method.</param>
		protected void PopulateSkippedFlag(int bookNumber, int chapterNumber, List<ScriptLine> scriptLines)
		{
			foreach (var scriptBlock in scriptLines)
				scriptBlock.OnSkippedChanged += (line) => HandleSkippedFlagChanged(bookNumber, chapterNumber, line);

			lock (_skippedLines)
			{
				_chapterHavingSkipFlagPopulated = new Tuple<int, int>(bookNumber, chapterNumber);
				foreach (var scriptBlock in scriptLines)
					scriptBlock.Skipped = false;

				Dictionary<int, Dictionary<int, ScriptLineIdentifier>> chapters;
				if (_skippedLines.TryGetValue(bookNumber, out chapters))
				{
					Dictionary<int, ScriptLineIdentifier> lines;
					if (chapters.TryGetValue(chapterNumber, out lines))
					{
						foreach (var scriptBlock in scriptLines)
						{
							ScriptLineIdentifier id;
							if (lines.TryGetValue(scriptBlock.Number, out id))
							{
								scriptBlock.Skipped = (id.Verse == scriptBlock.Verse && id.Text == scriptBlock.Text);
							}
						}
					}
				}
				_chapterHavingSkipFlagPopulated = null;
			}
		}

		private void HandleSkippedFlagChanged(int bookNumber, int chapterNumber, ScriptLine line)
		{
			lock (_skippedLines)
			{
				if (_chapterHavingSkipFlagPopulated != null && _chapterHavingSkipFlagPopulated.Item1 == bookNumber && _chapterHavingSkipFlagPopulated.Item2 == chapterNumber)
					return;

				if (line.Skipped)
					AddSkippedLine(bookNumber, chapterNumber, line);
				else
					RemoveSkippedLine(bookNumber, chapterNumber, line);

				Save();
			}
		}

		private void AddSkippedLine(int book, int chapter, ScriptLine scriptBlock)
		{
			Debug.Assert(scriptBlock.Skipped);
			AddSkippedLine(new ScriptLineIdentifier(book, chapter, scriptBlock));
		}

		private void AddSkippedLine(ScriptLineIdentifier skippedLine)
		{
			Dictionary<int, Dictionary<int, ScriptLineIdentifier>> chapters;
			if (!_skippedLines.TryGetValue(skippedLine.BookNumber, out chapters))
			{
				chapters = new Dictionary<int, Dictionary<int, ScriptLineIdentifier>>();
				_skippedLines[skippedLine.BookNumber] = chapters;
			}
			Dictionary<int, ScriptLineIdentifier> lines;
			if (!chapters.TryGetValue(skippedLine.ChapterNumber, out lines))
			{
				lines = new Dictionary<int, ScriptLineIdentifier>();
				chapters[skippedLine.ChapterNumber] = lines;
			}

			lines[skippedLine.LineNumber] = skippedLine;
		}

		private void RemoveSkippedLine(int book, int chapter, ScriptLine scriptBlock)
		{
			Debug.Assert(!scriptBlock.Skipped);
			Dictionary<int, Dictionary<int, ScriptLineIdentifier>> chapters;
			if (!_skippedLines.TryGetValue(book, out chapters))
				throw new KeyNotFoundException("Attempting to remove skipped line for non-existent book: " + book);
			Dictionary<int, ScriptLineIdentifier> lines;
			if (!chapters.TryGetValue(chapter, out lines))
				throw new KeyNotFoundException("Attempting to remove skipped line for non-existent book: " + book);
			if (lines.Remove(scriptBlock.Number) && OnScriptBlockUnskipped != null)
				OnScriptBlockUnskipped(this, book, chapter, scriptBlock);
		}

		private void Save()
		{
			if (_skipfilePath == null) // This will be null when doing initial deserialization.
				return;

			var skippedLineList = new List<ScriptLineIdentifier>();
			foreach (var book in _skippedLines.Keys)
			{
				foreach (var chapter in _skippedLines[book].Keys)
				{
					foreach (var line in _skippedLines[book][chapter].Keys)
					{
						skippedLineList.Add(_skippedLines[book][chapter][line]);
					}
				}
			}

			var objectToSerialize = new SkippedScriptLines
			{
				SkippedParagraphStyles = _skippedParagraphStyles,
				SkippedLinesList = skippedLineList,
			};

			XmlSerializationHelper.SerializeToFile(_skipfilePath, objectToSerialize);
		}

		public void SetSkippedStyle(string style, bool skipped)
		{
			lock (_skippedLines)
			{
				if (skipped)
				{
					if (!_skippedParagraphStyles.Contains(style))
					{
						var details = new Dictionary<string, string>(1);
						details["style"] = style;
						Analytics.Track("Added skipped style", details);
						_skippedParagraphStyles.Add(style);
						Save();
					}
				}
				else
				{
					if (_skippedParagraphStyles.Remove(style))
						Save();
				}
			}
		}

		public bool IsSkippedStyle(string style)
		{
			lock (_skippedLines)
				return _skippedParagraphStyles.Contains(style);
		}
		#endregion
	}
}