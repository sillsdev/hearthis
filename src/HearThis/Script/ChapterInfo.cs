using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Publishing;
using Palaso.IO;
using Palaso.Xml;
using L10NSharp;

namespace HearThis.Script
{
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Class to maintain information about chapters and support XML serialization
	/// </summary>
	/// ------------------------------------------------------------------------------------
	[Serializable]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class ChapterInfo
	{
		public enum DataMigrationStatus
		{
			NotStarted,
			Incomplete,
			Completed,
			ManualCheckingRequired,
		}

		public const string kChapterInfoFilename = "info.xml";

		private string _projectName;
		private string _bookName;
		private int _bookNumber;
		private IScriptProvider _scriptProvider;

		[XmlAttribute("Number")]
		public int ChapterNumber1Based;
		public DataMigrationStatus VersesInChapterStatus;

		/// <summary>
		/// This is for informational purposes only (at least for now). Any recording made
		/// in HearThis before this was implemented will not be reflected in this list, so
		/// it should NOT be used to determine the actual number of recordings. If/when we
		/// start to use this to determine if recordings are possibly out-of-date, we'll
		/// need to handle the case where a recording exists but is not reflected here.
		/// To enable XML serialization, this is not a SortedList, but it is expected to be
		/// ordered by LineNumber.
		/// </summary>
		public List<ScriptLine> Recordings { get; set; }

		/// <summary>
		/// Use this instead of the default constructor to instantiate an instance of this class
		/// </summary>
		/// <param name="book">Info about the book containing this chapter</param>
		/// <param name="chapterNumber1Based">[0] == intro, [1] == chapter 1, etc.</param>
		public static ChapterInfo Create(BookInfo book, int chapterNumber1Based)
		{
			ChapterInfo chapterInfo = null;
			string filePath = Path.Combine(book.GetChapterFolder(chapterNumber1Based), kChapterInfoFilename);
			if (File.Exists(filePath))
			{
				try
				{
					chapterInfo = XmlSerializationHelper.DeserializeFromFile<ChapterInfo>(filePath);
					int prevLineNumber = 0;
					HashSet<int> existingLineNumbers = new HashSet<int>();
					int firstIncorrectLineNumber = 0;
					foreach (ScriptLine line in chapterInfo.Recordings)
					{
						if (existingLineNumbers.Contains(line.LineNumber))
						{
							throw new ConstraintException(String.Format(
								"Unexpected duplicate ScriptLine found in Recordings collection. Book: {0}, Chapter: {1}, Line: {2}",
								chapterInfo._bookNumber, chapterInfo.ChapterNumber1Based, line.LineNumber));
						}
						if (line.LineNumber < prevLineNumber && firstIncorrectLineNumber == 0)
							firstIncorrectLineNumber = line.LineNumber;
					}
					if (firstIncorrectLineNumber > 0)
					{
						Debug.Fail(String.Format("Recordings collection not correctly sorted, loaded from file: {0}" +
							Environment.NewLine + "First error at line number {1}", filePath, firstIncorrectLineNumber));
						chapterInfo.Recordings = new List<ScriptLine>(chapterInfo.Recordings.OrderBy(s => s.LineNumber));
					}
				}
				catch (Exception e)
				{
					Analytics.ReportException(e);
					Debug.Fail(e.Message);
				}
			}
			if (chapterInfo == null)
			{
				chapterInfo = new ChapterInfo();
				chapterInfo.ChapterNumber1Based = chapterNumber1Based;
				chapterInfo.Recordings = new List<ScriptLine>();
			}

			chapterInfo._projectName = book.ProjectName;
			chapterInfo._bookName = book.Name;
			chapterInfo._bookNumber = book.BookNumber;
			chapterInfo._scriptProvider = book.ScriptProvider;

			chapterInfo.MigrateIfNeeded(book);

			return chapterInfo;
		}

		private void MigrateIfNeeded(BookInfo bookInfo)
		{
			if (VersesInChapterStatus == DataMigrationStatus.Completed ||
				_scriptProvider is SampleScriptProvider)
			{
				return;
			}

			if (VersesInChapterStatus == DataMigrationStatus.Incomplete)
			{
				var properties = new Dictionary<string, string>();
				properties["_projectName"] = _projectName;
				properties["_bookNumber"] = _bookNumber.ToString();
				properties["ChapterNumber1Based"] = ChapterNumber1Based.ToString();

				Analytics.Track("Detected previously failed chapter info migration", properties);
				return;
			}

			if (bookInfo.GetCountOfRecordingsForChapter(ChapterNumber1Based) == 0)
			{
				VersesInChapterStatus = DataMigrationStatus.Completed;
				// No need to save this. It will get saved if/when a recording is made.
				return;
			}

			var chapterFolder = bookInfo.GetChapterFolder(ChapterNumber1Based);
			var path = Path.Combine(chapterFolder, kChapterInfoFilename);

			int nextChapterWithLines = 0;
			if (ChapterNumber1Based > 0)
			{
				for (int c = ChapterNumber1Based + 1; c <= Project.Statistics.ChaptersPerBook[_bookNumber]; c++)
				{
					if (_scriptProvider.GetScriptLineCount(_bookNumber, c) > 0)
					{
						nextChapterWithLines = c;
						break;
					}
				}
			}

			if (ChapterNumber1Based == 0 || nextChapterWithLines == 0)
			{
				VersesInChapterStatus = DataMigrationStatus.Completed;
				Save(path);
				return;
			}

			VersesInChapterStatus = DataMigrationStatus.Incomplete;
			Save(path);

			int scriptLinesToMove = _scriptProvider.GetScriptLineCountFromLastParagraph(_bookNumber, ChapterNumber1Based);
			var nextChapter = bookInfo.GetChapterFolder(nextChapterWithLines);
			int destFileNumber = GetScriptLineCount() - scriptLinesToMove;
			bool madeBackups = false;
			for (int i = 0; i < scriptLinesToMove; i++)
			{
				var existingFile = Path.Combine(nextChapter, i + ".wav");
				if (File.Exists(existingFile))
				{
					var destFile = Path.Combine(chapterFolder, (destFileNumber++) + ".wav");
					if (File.Exists(destFile))
					{
						var backupFile = destFile + ".bak";
						if (File.Exists(backupFile))
							File.Delete(backupFile);
						File.Move(destFile, backupFile);
						madeBackups = true;
					}
					File.Move(existingFile, destFile);
				}
			}

			int max = 0;
			foreach (var recording in Directory.EnumerateFiles(nextChapter, "*.wav"))
			{
				int number;
				if (Int32.TryParse(Path.GetFileNameWithoutExtension(recording), out number))
					max = Math.Max(max, number);
			}

			for (int i = 0; i < max; i++)
			{
				var existingFile = Path.Combine(nextChapter, (scriptLinesToMove + i) + ".wav");
				if (File.Exists(existingFile))
					File.Move(existingFile, Path.Combine(nextChapter, i + ".wav"));
			}

			VersesInChapterStatus = madeBackups ? DataMigrationStatus.ManualCheckingRequired : DataMigrationStatus.Completed;
			Save(path);
		}

		public string MigrationMessage
		{
			get
			{
				switch (VersesInChapterStatus)
				{
					case DataMigrationStatus.Incomplete:
						return LocalizationManager.GetString("MiscErrors.MigrationFailedPreviously",
							"HearThis has detected that an attempt to do an important data migration failed to complete for this chapter." +
							" Existing recordings for this chapter or the following chapter fail to match the text." +
							" Please check the recordings starting from the last paragraph of this chapter.");
					case DataMigrationStatus.ManualCheckingRequired:
						return string.Format(LocalizationManager.GetString("MiscErrors.TextChangeDetectedDuringMigration",
							"HearThis has detected that the text has changed since recordings were last made. To avoid losing data, some" +
							" existing recordings were saved with a .bak extension in {0}. Existing recordings for this chapter or the" +
							" following chapter or fail to match the text. Please check these recordings."),
							LineRecordingRepository.GetChapterFolder(_projectName, _bookName, ChapterNumber1Based));

					default:
						return null;
				}
			}
		}

		public bool IsEmpty
		{
			get { return GetScriptLineCount() == 0; }
		}

		public int GetScriptLineCount()
		{
			return _scriptProvider.GetScriptLineCount(_bookNumber, ChapterNumber1Based);
		}

		public int CalculatePercentageRecorded()
		{
			int scriptLineCount = GetScriptLineCount();
			// First check Recordings collection in memory - it's faster (though not guaranteed reliable since someone could delete a file).
			if (Recordings.Count == scriptLineCount)
				return 100;

			if (scriptLineCount == 0)
				return 0;//should it be 0 or 100 or -1 or what?
			return 100* LineRecordingRepository.GetCountOfRecordingsForChapter(_projectName, _bookName, ChapterNumber1Based)/scriptLineCount;
		}

		public bool RecordingsFinished
		{
			get
			{
				int scriptLineCount = GetScriptLineCount();
				// First check Recordings collection in memory - it's faster (though not guaranteed reliable since someone could delete a file).
				if (Recordings.Count == scriptLineCount)
					return true;
				// Older versions of HT didn't maintain in-memory recording info, so see if we have the right number of recordings.
				// ENHANCE: for maximum reliability, we should check for the existence of the exact filenames we expect.
				return CalculatePercentageRecorded() == 100;
			}
		}

		public int CalculatePercentageTranslated()
		{
			 return (_scriptProvider.GetTranslatedVerseCount(_bookNumber, ChapterNumber1Based));
		}

		public void MakeDummyRecordings()
		{
			using (TempFile sound = new TempFile())
			{
				byte[] buffer = new byte[Resources.think.Length];
				Resources.think.Read(buffer, 0, buffer.Length);
				File.WriteAllBytes(sound.Path, buffer);
				for (int line = 0; line < GetScriptLineCount(); line++)
				{
					var path = LineRecordingRepository.GetPathToLineRecording(_projectName, _bookName, ChapterNumber1Based, line);

					if (!File.Exists(path))
					{
						File.Copy(sound.Path, path, false);
					}
				}
			}
		}

		private String Folder
		{
			get { return LineRecordingRepository.GetChapterFolder(_projectName, _bookName, ChapterNumber1Based); }
		}

		private String FilePath
		{
			get { return Path.Combine(Folder, kChapterInfoFilename); }
		}

		public void RemoveRecordings()
		{
			Directory.Delete(Folder, true);
			Save();
		}

		private void Save()
		{
			Save(FilePath);
		}

		private void Save(string filePath)
		{
			XmlSerializationHelper.SerializeToFile(filePath, this);
		}

		public string ToXmlString()
		{
			return XmlSerializationHelper.SerializeToString(this);
		}

		public void OnRecordingSaved(int lineNumber, ScriptLine selectedScriptLine)
		{
			Debug.Assert(selectedScriptLine.LineNumber > 0);
			int iInsert = 0;
			for (int i = 0; i < Recordings.Count; i++)
			{
				var recording = Recordings[i];
				if (recording.LineNumber == lineNumber)
				{
					Recordings[i] = selectedScriptLine;
					iInsert = -1;
					break;
				}
				if (recording.LineNumber > lineNumber)
				{
					break;
				}
				iInsert++;
			}
			if (iInsert >= 0)
				Recordings.Insert(iInsert, selectedScriptLine);
			Save();
		}
	}
}