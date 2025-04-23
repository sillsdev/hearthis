// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014-2025, SIL Global.
// <copyright from='2014' to='2025' company='SIL Global'>
//		Copyright (c) 2014-2025, SIL Global.
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
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Publishing;
using L10NSharp;
using SIL.Reporting;
using SIL.Xml;
using static HearThis.FileContentionHelper;

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
		private readonly HashSet<char> _allEncounteredSentenceEndingCharacters = new HashSet<char>();
		
		public event ScriptBlockChangedHandler ScriptBlockUnskipped;
		public delegate void ScriptBlockChangedHandler(IScriptProvider sender, int book, int chapter, ScriptLine scriptBlock);

		public event SkippedStylesChangedHandler SkippedStylesChanged;
		public delegate void SkippedStylesChangedHandler(IScriptProvider sender, string styleName, bool newSkipValue);

		public abstract ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based);
		public abstract void UpdateSkipInfo();
		protected virtual ChapterRecordingInfoBase GetChapterInfo(int book, int chapter)
		{
			return ChapterInfo.Create(new BookInfo(ProjectFolderName, book, this), chapter);
		}

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
		public virtual IEnumerable<char> AllEncounteredSentenceEndingCharacters
		{
			get
			{
				lock (_allEncounteredSentenceEndingCharacters)
					return _allEncounteredSentenceEndingCharacters;
			}
		}
		public abstract IBibleStats VersificationInfo { get; }
		protected virtual IStyleInfoProvider StyleInfo { get; } 

		public virtual bool NestedQuotesEncountered => false;

		public ProjectSettings ProjectSettings => _projectSettings;

		#region ISkippedStyleInfoProvider implementation and related methods
		protected string ProjectFolderPath => Program.GetApplicationDataFolder(ProjectFolderName);

		/// <summary>
		/// Currently restricted to current character blocks
		/// </summary>
		/// <param name="books">Collection of objects containing information about a project's
		/// books</param>
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

		protected virtual void Initialize(Action preDataMigrationInitializer = null)
		{
			Logger.WriteEvent("Initializing script provider for " + ProjectFolderName);
			if (_skipFilePath != null)
				throw new InvalidOperationException("Initialize should only be called once!");

			bool existingHearThisProject = Directory.Exists(ProjectFolderPath) &&
				Directory.EnumerateFiles(ProjectFolderPath, "*", SearchOption.AllDirectories).Any();
			
			LoadSkipInfo();
			LoadProjectSettings(existingHearThisProject);
			preDataMigrationInitializer?.Invoke();
			if (existingHearThisProject)
			{
				if (_projectSettings.Version > Settings.Default.CurrentDataVersion)
				{
					throw new IncompatibleProjectDataVersionException(ProjectFolderName, _projectSettings.Version);
				}
				DoDataMigration();
			}
			else
			{
				_projectSettings.Version = Settings.Default.CurrentDataVersion;
				SaveProjectSettings();
			}
		}

		private void LoadProjectSettings(bool existingHearThisProject)
		{
			Debug.Assert(_projectSettings == null);

			Logger.WriteEvent("Loading project settings for " + (existingHearThisProject ? "existing" : "new") + " project.");

			_projectSettingsFilePath = Path.Combine(ProjectFolderPath, kProjectInfoFilename);
			bool retry;
			string prevErrorMessage = null;
			string prevContents = null;
			do
			{
				if (File.Exists(_projectSettingsFilePath))
				{
					_projectSettings = DeserializeFromFile<ProjectSettings>(_projectSettingsFilePath, out var error);
					if (_projectSettings != null)
					{
						Logger.WriteEvent("Project settings loaded. Version = " + _projectSettings.Version);
						return;
					}

					if (prevErrorMessage != error.Message)
					{
						Logger.WriteError(error);
						prevErrorMessage = error.Message;
					}

					try
					{
						var contents = File.ReadAllText(_projectSettingsFilePath);
						if (contents != prevContents)
						{
							Logger.WriteEvent("File contents:" + Environment.NewLine + contents);
							prevContents = contents;
						}
					}
					catch
					{
					}

					var msg = string.Format(LocalizationManager.GetString("Project.SettingsFileError",
						"An error occurred reading the project settings file:{0}If you ignore this, some things might" +
						" not work correctly, including the possible misalignment of clips and blocks.",
						"Param: Error details"),
						Environment.NewLine + error.Message + Environment.NewLine);

					var result = MessageBox.Show(msg, Program.kProduct, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
					switch (result)
					{
						case DialogResult.Abort:
							throw new ProjectOpenCancelledException(ProjectFolderName, error);
						case DialogResult.Retry:
							retry = true;
							break;
						case DialogResult.Ignore:
							Logger.WriteEvent("User chose to ignore error loading project settings.");
							retry = false;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else
				{
					if (existingHearThisProject)
						Logger.WriteEvent("Project settings file did not exist!");
					break;
				}
			} while (retry);

			// Create settings file with default settings.
			_projectSettings = new ProjectSettings
			{
				NewlyCreatedSettingsForExistingProject = existingHearThisProject
			};
			Logger.WriteEvent("Newly created project settings. Version = " + _projectSettings.Version);
		}

		public void SaveProjectSettings()
		{
			if (_projectSettings == null)
				throw new InvalidOperationException("Initialize must be called first.");
			XmlSerializationHelper.SerializeToFileWithWriteThrough(_projectSettingsFilePath,
				_projectSettings, out var error);
			if (error != null)
			{
				Logger.WriteError(error);
				Logger.WriteEvent("Settings:" + Environment.NewLine +
					XmlSerializationHelper.SerializeToString(_projectSettings, true));
				throw new Exception("Unable to save file: " + _projectSettingsFilePath, error);
			}
		}

		private void DoDataMigration()
		{
			if (_projectSettings.Version == Settings.Default.CurrentDataVersion)
				return;

			// As a sanity check, let's ensure that the settings file is writable. If not,
			// there's no point doing the migration and then not being able to know we did
			// it.
			if (!RobustFileAddOn.IsWritable(_projectSettingsFilePath, out var error))
			{
				Logger.WriteError(error);
				ErrorReport.NotifyUserOfProblem(error, LocalizationManager.GetString("Project.SettingsFileNotWritable",
					"Data migration is required for project {0}, but the settings file cannot be written.{1}",
					"Param 0: project name; " +
					"Param 1: error details"),
					ProjectFolderName, Environment.NewLine + error.Message);
				throw new ProjectOpenCancelledException(ProjectFolderName, error);
			}

			void LogMigrationStep()
			{
				Logger.WriteEvent($"Migrating {ProjectFolderName} to version {_projectSettings.Version + 1}.");
			}

			// Note: If the NewlyCreatedSettingsForExistingProject flag is set in the project
			// settings we are migrating a project from an early version of HearThis that did
			// not previously have settings or whose settings file got corrupted. In this case,
			// we skip any steps whose only function is to unconditionally migrate settings to
			// values that might not be the defaults.
			do
			{
				switch (_projectSettings.Version)
				{
					case 0:
						LogMigrationStep();
						// This corrects data in a bogus state by having recorded clips for blocks
						// marked with a skipped style.
						BackupAnyClipsForSkippedStyles();
						break;
					case 1:
						LogMigrationStep();
						// Original projects always broke at paragraphs,
						// but now the default is to keep them together.
						// This ensures we don't mess up existing recordings.
						if (ClipRepository.HasRecordingsForProject(ProjectFolderName))
							_projectSettings.BreakAtParagraphBreaks = true;
						break;
					case 2:
						if (!_projectSettings.NewlyCreatedSettingsForExistingProject)
						{
							LogMigrationStep();
							// Settings that used to be per-user really should be per-project.
							_projectSettings.BreakQuotesIntoBlocks = Settings.Default.BreakQuotesIntoBlocks;
							_projectSettings.ClauseBreakCharacters = Settings.Default.ClauseBreakCharacters;
							_projectSettings.AdditionalBlockBreakCharacters = Settings.Default.AdditionalBlockBreakCharacters;
						}
						break;
					case 3:
						LogMigrationStep();
						// HT-376: Unfortunately, HT v. 2.0.3 introduced a change whereby the numbering of
						// existing clips could be out of sync with the data, so any chapter with one of the
						// new StylesToSkipByDefault that has not had anything recorded since the
						// migration to that version needs to have clips shifted forward to account for the
						// new blocks (even though they are most likely skipped). (Any chapter where the user
						// has recorded something since the migration to that version could also be affected,
						// but the user will have to migrate it manually because we can't know which clips
						// might need to be moved.) If _dateOfMigrationToHt203 is "default", then we can
						// safely migrate any affected chapters.
						var stopwatch = Stopwatch.StartNew();
						var chaptersPotentiallyNeedingManualMigration = MigrateDataToVersion4ByShiftingClipsAsNeeded(stopwatch);
						if (chaptersPotentiallyNeedingManualMigration.Any())
						{
							var reportToken = _projectSettings.LastDataMigrationReportNag = _projectSettings.Version.ToString();
							var filename = GetDataMigrationReportFilename(reportToken);

							using (var writer = new StreamWriter(filename))
							{
								writer.WriteLine("Chapters needing manual migration:");
								foreach (var kvp in chaptersPotentiallyNeedingManualMigration)
								{
									writer.WriteLine(kvp.Key + "\t" + string.Join(", ", kvp.Value));
								}
							}
						}
						break;
				}

				_projectSettings.Version++;
				SaveProjectSettings();
			} while (_projectSettings.Version < Settings.Default.CurrentDataVersion);
		}

		internal Dictionary<string, List<int>> MigrateDataToVersion4ByShiftingClipsAsNeeded(Stopwatch stopwatch)
		{
			var tracker = MigrationProgressTracker.Create(ProjectFolderPath, bookNum => VersificationInfo.GetBookName(bookNum));
			
			ProcessBlocksWhere(s => StylesToSkipByDefault.Contains(s.ParagraphStyle),
				delegate(string projectName, string bookName, int chapterIndex, int blockIndex, IScriptProvider scriptProvider)
				{
					bool chapterMigratedSuccessfully = false;
					try
					{
						var bookNum = scriptProvider.VersificationInfo.GetBookNumber(bookName);
						tracker.Start(bookNum, chapterIndex);
						chapterMigratedSuccessfully = ClipRepository.ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate(
							projectName, bookName, chapterIndex, blockIndex, _dateOfMigrationToHt203,
							() => GetChapterInfo(bookNum, chapterIndex));
						tracker.NoteCompletedCurrentBookAndChapter();
					}
					catch (Exception e)
					{
						// REVIEW: Do we need to report these errors more specifically in the migration report?
						// This is (I think) highly unlikely, but it could happen if a clip file were locked, open
						// in another app, etc.
						Logger.WriteError(e);
					}

					if (!chapterMigratedSuccessfully)
						tracker.AddCurrentChapterAsPotentiallyNeedingMigration(bookName);

					if (stopwatch != null && stopwatch.ElapsedMilliseconds > 2500)
					{
						stopwatch = null;
						var msg = LocalizationManager.GetString("DataMigration.PleaseBePatient",
							"Please wait while {0} migrates the data for {1}. Thank you for your patience!");
						MessageBox.Show(string.Format(msg, Program.kProduct, projectName),
							Program.kProduct, MessageBoxButtons.OK);
					}
				}, tracker.LastBookStarted, tracker.PreviousMigrationWasInterrupted ? tracker.LastChapterStarted + 1 : 0);

			tracker.NoteMigrationComplete();
			return tracker.ChaptersPotentiallyNeedingManualMigration;
		}

		public string GetDataMigrationReportFilename(string token) =>
			Path.Combine(ProjectFolderPath, $"DataMigrationReport_{token}.txt");

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

		public IReadOnlyList<string> StylesToSkipByDefault
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
					.Select(m => StyleInfo.GetStyleName(m)).ToList();
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

		protected void AddEncounteredSentenceEndingCharacter(char ch)
		{
			lock(_allEncounteredSentenceEndingCharacters)
				_allEncounteredSentenceEndingCharacters.Add(ch);
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
				scriptBlock.SkippedChanged += (line) => HandleSkippedFlagChanged(bookNumber, chapterNumber, line);

			lock (_skippedLines)
			{
				_chapterHavingSkipFlagPopulated = new Tuple<int, int>(bookNumber, chapterNumber);
				foreach (var scriptBlock in scriptLines)
					scriptBlock.Skipped = false;

				if (_skippedLines.TryGetValue(bookNumber, out var chapters))
				{
					if (chapters.TryGetValue(chapterNumber, out var lines))
					{
						foreach (var scriptBlock in scriptLines)
						{
							if (lines.TryGetValue(scriptBlock.Number, out var id))
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
			if (!_skippedLines.TryGetValue(skippedLine.BookNumber, out var chapters))
			{
				chapters = new Dictionary<int, Dictionary<int, ScriptLineIdentifier>>();
				_skippedLines[skippedLine.BookNumber] = chapters;
			}

			if (!chapters.TryGetValue(skippedLine.ChapterNumber, out var lines))
			{
				lines = new Dictionary<int, ScriptLineIdentifier>();
				chapters[skippedLine.ChapterNumber] = lines;
			}

			lines[skippedLine.LineNumber] = skippedLine;
		}

		private void RemoveSkippedLine(int book, int chapter, ScriptLine scriptBlock)
		{
			Debug.Assert(!scriptBlock.Skipped);
			if (!_skippedLines.TryGetValue(book, out var chapters))
				throw new KeyNotFoundException("Attempting to remove skipped line for non-existent book: " + book);
			if (!chapters.TryGetValue(chapter, out var lines))
				throw new KeyNotFoundException("Attempting to remove skipped line for non-existent chapter: " + chapter + " in book " + book);
			if (lines.Remove(scriptBlock.Number))
				ScriptBlockUnskipped?.Invoke(this, book, chapter, scriptBlock);
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

			objectToSerialize.Save(_skipFilePath);
		}

		public void SetSkippedStyle(string style, bool skipped)
		{
			bool changeMade = false; 
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
						changeMade = true;
					}
				}
				else
				{
					if (_skippedParagraphStyles.Remove(style))
					{
						RestoreAnyClipsForUnskippedStyle(style);
						Save();
						changeMade = true;
					}
				}
			}

			if (changeMade)
				SkippedStylesChanged?.Invoke(this, style, skipped);
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
				ClipRepository.RestoreBackedUpClip(projectName, bookName, chapterIndex, blockIndex, scriptProvider),
				skipExplicitlySkippedBlocks: true);
		}

		private void ProcessBlocksHavingStyle(string style,
			Action<string, string, int, int, IScriptProvider> action,
			bool skipExplicitlySkippedBlocks = false)
		{
			ProcessBlocksWhere(s => s.ParagraphStyle == style, action, skipExplicitlySkippedBlocks: skipExplicitlySkippedBlocks);
		}

		private void ProcessBlocksWhere(Predicate<ScriptLine> predicate,
			Action<string, string, int, int, IScriptProvider> action,
			int startBook = 0, int startChapter = 0, bool skipExplicitlySkippedBlocks = false)
		{
			for (int b = startBook; b < VersificationInfo.BookCount; b++)
			{
				LoadBook(b);
				var bookName = VersificationInfo.GetBookName(b);
				for (int c = startChapter; c <= VersificationInfo.GetChaptersInBook(b); c++)
				{
					for (int i = 0; i < GetScriptBlockCount(b, c); i++)
					{
						bool skip = false;
						if (skipExplicitlySkippedBlocks)
						{
							if (_skippedLines.TryGetValue(b, out var bookSkips))
							{
								if (bookSkips.TryGetValue(c, out var chapterSkips))
								{
									// our index is 0-based, but the LineNumber property in
									// ScriptLineIdentifier is 1-based
									skip = chapterSkips.ContainsKey(i + 1);
								}
							}
						}

						if (!skip && predicate(GetBlock(b, c, i)))
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
