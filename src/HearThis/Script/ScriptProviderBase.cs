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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Publishing;
using SIL.Reporting;
using SIL.Xml;

namespace HearThis.Script
{
	public abstract class ScriptProviderBase : IScriptProvider, ISkippedStyleInfoProvider
	{
		internal const string kProjectInfoFilename = "projectInfo.xml";
		public const string kSkippedLineInfoFilename = "SkippedLineInfo.xml";
		private Tuple<int, int> _chapterHavingSkipFlagPopulated;
		private readonly Dictionary<int, Dictionary<int, Dictionary<int, ScriptLineIdentifier>>> _skippedLines = new Dictionary<int, Dictionary<int, Dictionary<int, ScriptLineIdentifier>>>();
		private string _skipFilePath;
		private string _projectSettingsFilePath;
		private ProjectSettings _projectSettings;
		private List<string> _skippedParagraphStyles = new List<string>();
		private DateTime _dateOfMigrationToHt203;

		public event ScriptBlockChangedHandler OnScriptBlockUnskipped;
		public delegate void ScriptBlockChangedHandler(IScriptProvider sender, int book, int chapter, ScriptLine scriptBlock);

		public abstract ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based);
		public abstract void UpdateSkipInfo();

		// by default this is the same as GetBlock, except it simply returns null if the line number is out of range.
		// Filtering script providers override.
		public virtual ScriptLine GetUnfilteredBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			if (lineNumber0Based < 0 || lineNumber0Based >= GetUnfilteredScriptBlockCount(bookNumber, chapterNumber))
				return null;
			return GetBlock(bookNumber, chapterNumber, lineNumber0Based);
		}

		public abstract int GetScriptBlockCount(int bookNumber, int chapter1Based);
		public virtual int GetUnfilteredScriptBlockCount(int bookNumber, int chapter1Based)
		{
			return GetScriptBlockCount(bookNumber, chapter1Based);
		}

		public abstract int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based);
		public abstract int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based);
		public abstract int GetTranslatedVerseCount(int bookNumberDelegateSafe, int chapterNumber1Based);

		public virtual int GetUnfilteredTranslatedVerseCount(int bookNumberDelegateSafe, int chapterNumber1Based)
		{
			return GetTranslatedVerseCount(bookNumberDelegateSafe, chapterNumber1Based);
		}
		public abstract int GetScriptBlockCount(int bookNumber);
		public abstract void LoadBook(int bookNumber0Based);
		public abstract string EthnologueCode { get; }
		public abstract bool RightToLeft { get; }
		public abstract string FontName { get; }
		public abstract string ProjectFolderName { get; }
		public abstract IEnumerable<string> AllEncounteredParagraphStyleNames { get; }
		public abstract IBibleStats VersificationInfo { get; }
		protected virtual IStyleInfoProvider StyleInfo { get; } 

		public virtual bool NestedQuotesEncountered => false;

		public ProjectSettings ProjectSettings => _projectSettings;

		#region ISkippedStyleInfoProvider implementation and related methods
		protected string ProjectFolderPath => Program.GetApplicationDataFolder(ProjectFolderName);

		/// <summary>
		/// Currently restricted to current character blocks
		/// </summary>
		/// <param name="books"></param>
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

		protected void Initialize(Action preDataMigrationInitializer = null)
		{
			if (_skipFilePath != null)
				throw new InvalidOperationException("Initialize should only be called once!");

			bool existingHearThisProject = Directory.Exists(ProjectFolderPath) &&
				Directory.EnumerateFiles(ProjectFolderPath, "*", SearchOption.AllDirectories).Any();
			
			LoadSkipInfo();
			LoadProjectSettings(existingHearThisProject);
			preDataMigrationInitializer?.Invoke();
			if (existingHearThisProject)
				DoDataMigration();
			else
			{
				_projectSettings.Version = Settings.Default.CurrentDataVersion;
				SaveProjectSettings();
			}
		}

		private void LoadProjectSettings(bool existingHearThisProject)
		{
			Debug.Assert(_projectSettings == null);

			_projectSettingsFilePath = Path.Combine(ProjectFolderPath, kProjectInfoFilename);
			if (File.Exists(_projectSettingsFilePath))
				_projectSettings = XmlSerializationHelper.DeserializeFromFile<ProjectSettings>(_projectSettingsFilePath);
			if (_projectSettings == null) // If deserialization fails, re-create settings file with default settings.
			{
				_projectSettings = new ProjectSettings();
				_projectSettings.NewlyCreatedSettingsForExistingProject = existingHearThisProject;
			}
		}

		public void SaveProjectSettings()
		{
			if (_projectSettings == null)
				throw new InvalidOperationException("Initialize must be called first.");
			XmlSerializationHelper.SerializeToFile(_projectSettingsFilePath, _projectSettings);
		}

		private void DoDataMigration()
		{
			// Note: If the NewlyCreatedSettingsForExistingProject flag is set in the project
			// settings we are migrating a project from an early version of HearThis that did
			// not previously have settings or whose settings file got corrupted. In this case,
			// we skip any steps whose only function is to unconditionally migrate settings to
			// values that might not be the defaults.
			while (_projectSettings.Version < Settings.Default.CurrentDataVersion)
			{
				switch (_projectSettings.Version)
				{
					case 0:
						// This corrects data in a bogus state by having recorded clips for blocks
						// marked with a skipped style.
						BackupAnyClipsForSkippedStyles();
						break;
					case 1:
						// Original projects always broke at paragraphs,
						// but now the default is to keep them together.
						// This ensures we don't mess up existing recordings.
						if (ClipRepository.HasRecordingsForProject(ProjectFolderName))
							_projectSettings.BreakAtParagraphBreaks = true;
						break;
					case 2:
						if (!_projectSettings.NewlyCreatedSettingsForExistingProject)
						{
							// Settings that used to be per-user really should be per-project.
							_projectSettings.BreakQuotesIntoBlocks = Settings.Default.BreakQuotesIntoBlocks;
							_projectSettings.ClauseBreakCharacters = Settings.Default.ClauseBreakCharacters;
							_projectSettings.AdditionalBlockBreakCharacters = Settings.Default.AdditionalBlockBreakCharacters;
						}
						break;
					case 3:
						// HT-376: Unfortunately, HT v. 2.0.3 introduced a change whereby the numbering of
						// existing clips could be out of sync with the data, so any chapter with one of the
						// new StylesToSkipByDefault that has not had anything recorded since the
						// migration to that version needs to have clips shifted forward to account for the
						// new blocks (even though they are most likely skipped). (Any chapter where the user
						// has recorded something since the migration to that version could also be affected,
						// but the user will have to migrate it manually because we can't know which clips
						// might need to be moved.) If _dateOfMigrationToHt203 is "default", then we can
						// safely migrate any affected chapters.
						ChapterInfo.PrepareForClipShiftDataMigration();
						try
						{
							var chaptersPotentiallyNeedingManualMigration = new Dictionary<string, List<int>>();
							ProcessBlocksWhere(s => StylesToSkipByDefault.Contains(s.ParagraphStyle),
								delegate(string projectName, string bookName, int chapterIndex, int blockIndex, IScriptProvider scriptProvider)
								{
									bool chapterMigratedSuccessfully = false;
									try
									{
										chapterMigratedSuccessfully = ClipRepository.ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate(
											projectName, bookName, chapterIndex, blockIndex, _dateOfMigrationToHt203, scriptProvider);
									}
									catch (Exception e)
									{
										// REVIEW: Do we need to report these errors more specifically in the migration report?
										// This is (I think) highly unlikely, but it could happen if a clip file were locked, open
										// in another app, etc.
										Logger.WriteError(e);
									}
									if (!chapterMigratedSuccessfully)
									{
										if (!chaptersPotentiallyNeedingManualMigration.TryGetValue(bookName, out var chapters))
											chaptersPotentiallyNeedingManualMigration[bookName] = new List<int>(new [] {chapterIndex});
										else
											chapters.Add(chapterIndex);
									}

								});
							if (chaptersPotentiallyNeedingManualMigration.Any())
							{
								var reportToken = _projectSettings.LastDataMigrationReportNag = _projectSettings.Version.ToString();
								var filename = _projectSettings.LastDataMigrationReportNag =
									GetDataMigrationReportFilename(reportToken);
								new XElement("ChaptersNeedingManualMigration", chaptersPotentiallyNeedingManualMigration.Select(kv => new XElement(kv.Key, kv.Value)))
									.Save(filename, SaveOptions.OmitDuplicateNamespaces);
							}
						}
						finally
						{
							ChapterInfo.ClipShiftDataMigrationIsComplete();
						}
						break;
				}
				_projectSettings.Version++;
			}

			_projectSettings.Version = Settings.Default.CurrentDataVersion;
			SaveProjectSettings();
		}

		public string GetDataMigrationReportFilename(string token) =>
			Path.Combine(ProjectFolderPath, $"DataMigrationReport_{token}.xml");

		public static string GetUrlForHelpWithDataMigrationProblem(string dataMigrationReportToken)
		{
			switch(dataMigrationReportToken)
			{
				case "3":
					return "https://community.scripture.software.sil.org/t/hearthis-2-2-3-removed-because-of-bug/2001";
				default:
					throw new ArgumentException("Unexpected data migration token", nameof(dataMigrationReportToken));
			}
		}

		public IEnumerable<string> StylesToSkipByDefault
		{
			get
			{
				// For now all implementations that deal with styles are based on the USFM standard
				// or something similar. Other implementations leave StyleInfo undefined.
				if (StyleInfo == null)
					return new string[0];
				// HT-374: The following* used to be unconditionally ignored by virtue of being
				// included in ParatextScriptProvider._furtherParagraphIgnorees, but it turns out
				// that there was a case where someone did want to record the intro outline
				// material. "r" was also moved here because it seemed reasonable that some users
				// might also want to include parallel passage info.
				// * "iot" was not previous in the list, but this was an inadvertent omission.
				// These markers are defined in USFM and it therefore seems reasonable to hard-
				// code them here. If a custom stylesheet is used that defines the markers
				// differently and they fail the test of publishable vernacular paragraph styles,
				// then we'll not assume they are necessarily to be included in this list (though
				// they'll probably get excluded anyway if they aren't publishable vernacular
				// fields).
				var markersToIgnoreByDefault = new[] { "r", "iot", "io1", "io2", "io3" };

				return markersToIgnoreByDefault.Where(m =>
					StyleInfo.IsParagraph(m) && StyleInfo.IsPublishableVernacular(m))
					.Select(m => StyleInfo.GetStyleName(m));
			}
		}

		protected void LoadSkipInfo()
		{
			lock (_skippedLines)
			{
				_skipFilePath = Path.Combine(ProjectFolderPath, kSkippedLineInfoFilename);
				var skippedLines = SkippedScriptLines.Create(_skipFilePath, this);
				foreach (var skippedLine in skippedLines.SkippedLinesList)
					AddSkippedLine(skippedLine);
				_skippedParagraphStyles = skippedLines.SkippedParagraphStyles;
				_dateOfMigrationToHt203 = skippedLines.DateOfMigrationToVersion1;
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
			if (_skipFilePath == null) // This will be null when doing initial deserialization.
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

			XmlSerializationHelper.SerializeToFile(_skipFilePath, objectToSerialize);
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
						BackUpAnyClipsForSkippedStyle(style);
						Save();
					}
				}
				else
				{
					if (_skippedParagraphStyles.Remove(style))
					{
						RestoreAnyClipsForUnskippedStyle(style);
						Save();
					}
				}
			}
		}

		private void BackupAnyClipsForSkippedStyles()
		{
			foreach (var style in _skippedParagraphStyles)
				BackUpAnyClipsForSkippedStyle(style);
		}

		// Currently only for current character
		private void BackUpAnyClipsForSkippedStyle(string style)
		{
			// This method will check to see whether the clip exists - does nothing if not.
			ProcessBlocksHavingStyle(style, ClipRepository.BackUpRecordingForSkippedLine);
		}

		// currently only for current character
		private void RestoreAnyClipsForUnskippedStyle(string style)
		{
			ProcessBlocksHavingStyle(style, (projectName, bookName, chapterIndex, blockIndex, scriptProvider) =>
				ClipRepository.RestoreBackedUpClip(projectName, bookName, chapterIndex, blockIndex, scriptProvider));
		}

		private void ProcessBlocksHavingStyle(string style, Action<string, string, int, int, IScriptProvider> action)
		{
			ProcessBlocksWhere(s => s.ParagraphStyle == style, action);
		}

		private void ProcessBlocksWhere(Predicate<ScriptLine> predicate, Action<string, string, int, int, IScriptProvider> action)
		{
			for (int b = 0; b < VersificationInfo.BookCount; b++)
			{
				LoadBook(b);
				var bookName = VersificationInfo.GetBookName(b);
				for (int c = 0; c <= VersificationInfo.GetChaptersInBook(b); c++)
				{
					for (int i = 0; i < GetScriptBlockCount(b, c); i++)
					{
						if (predicate(GetBlock(b, c, i)))
						{
							action(ProjectFolderName, bookName, c, i, this);
						}
					}
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
