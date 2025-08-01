// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021-2025, SIL Global.
// <copyright from='2021' to='2025' company='SIL Global'>
//		Copyright (c) 2021-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using PtxUtils;
using SIL.Reporting;
using SIL.Xml;
using static HearThis.FileContentionHelper;

namespace HearThis.Script
{
	/// <summary>
	/// This is a companion to ScriptProviderBase.DoDataMigration. When a migration step requires
	/// chapter-by-chapter migration and the need to resume safely is interrupted, this class can
	/// track how far the migration got. If the last book/chapter started are different from the
	/// last book/chapter completed, then the last one started is in an inconsistent state and will
	/// (presumably) need manual intervention to be migrated correctly. Note that this class does
	/// not store the data version number since that is stored in the (permanently persisted)
	/// project info file. The file that this class gets serialized to should only exist if a
	/// migration from the version stored in that file was interrupted.
	/// </summary>
	[Serializable]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class MigrationProgressTracker
	{
		private const string kFileName = "MigrationProgress.xml";
		private string _filename;

		/// <summary>
		/// Required for deserialization
		/// </summary>
		public MigrationProgressTracker()
		{
		}

		/// <summary>
		/// Creates a tracker, either by deserializing from a file for the given project or by
		/// constructing and initializing a new one. Note that if the file exists and the last
		/// started chapter was not noted as complete, an entry will be added automatically to
		/// the list of chapters potentially needing manual migration.
		/// </summary>
		/// <param name="getBookName">Function to get the book name from the last book started</param>
		public static MigrationProgressTracker Create(string projectFolder, Func<int, string> getBookName)
		{
			if (!Directory.Exists(projectFolder))
				throw new ArgumentException("Project must exist to be migrated!", nameof(projectFolder));
			var filename = Path.Combine(projectFolder, kFileName);
			MigrationProgressTracker tracker;
			if (File.Exists(filename))
			{
				tracker = DeserializeFromFile<MigrationProgressTracker>(filename, out var error);
				if (error != null)
				{
					Logger.WriteError(error);
					ErrorReport.ReportNonFatalException(error);
					throw new ProjectOpenCancelledException(projectFolder, error);
				}
				if (tracker.ChapterWasInterrupted)
					tracker.AddCurrentChapterAsPotentiallyNeedingMigration(getBookName);
			}
			else
				tracker = new MigrationProgressTracker {ChaptersPotentiallyNeedingManualMigration = new SerializableDictionary<string, List<int>>()};
			tracker._filename = filename;
			return tracker;
		}

		[XmlAttribute("lastBookStarted")]
		public int LastBookStarted { get; set; }

		[XmlAttribute("lastBookCompleted")]
		public int LastBookCompleted { get; set; }

		[XmlAttribute("lastChapterStarted")]
		public int LastChapterStarted { get; set; }

		[XmlAttribute("lastChapterCompleted")]
		public int LastChapterCompleted { get; set; }

		public SerializableDictionary<string, List<int>> ChaptersPotentiallyNeedingManualMigration { get; set; }

		public void Start(int book, int chapter)
		{
			LastBookStarted = book;
			LastChapterStarted = chapter;
			Save();
		}

		public void NoteCompletedCurrentBookAndChapter()
		{
			LastBookCompleted = LastBookStarted;
			LastChapterCompleted = LastChapterStarted;
			Save();
		}

		public void NoteMigrationComplete()
		{
			File.Delete(_filename);
		}

		public bool PreviousMigrationWasInterrupted =>
			default != LastBookStarted || default != LastBookCompleted ||
			default != LastChapterStarted || default != LastChapterCompleted;

		public bool ChapterWasInterrupted =>
			LastBookStarted != LastBookCompleted ||
			LastChapterStarted != LastChapterCompleted;

		/// <summary>
		/// Adds the current chapter (i.e., the last book/chapter started) to the collection
		/// of chapters potentially needing manual migration.
		/// </summary>
		/// <param name="getBookName">Function to get the book name from the last book started</param>
		private void AddCurrentChapterAsPotentiallyNeedingMigration(Func<int, string> getBookName)
		{
			AddCurrentChapterAsPotentiallyNeedingMigration(getBookName(LastBookStarted));
		}

		private void Save()
		{
			XmlSerializationHelper.SerializeToFileWithWriteThrough(_filename, this, out var error);
			if (error != null)
			{
				Logger.WriteError(error);
				Logger.WriteEvent("MigrationProgressTracker state at time of failure:" + Environment.NewLine +
					XmlSerializationHelper.SerializeToString(this, true));
				throw new Exception("Unable to save migration progress file: " + _filename, error);
			}
		}

		/// <summary>
		/// Adds the current chapter (i.e., the last book/chapter started) to the collection
		/// of chapters potentially needing manual migration.
		/// </summary>
		/// <param name="currentBookName">Caller is responsible for ensuring that this is the book
		/// name corresponding to LastBookStarted.
		/// </param>
		public void AddCurrentChapterAsPotentiallyNeedingMigration(string currentBookName)
		{
			if (ChaptersPotentiallyNeedingManualMigration.TryGetValue(currentBookName, out var chapters))
				chapters.Add(LastChapterStarted);
			else
				ChaptersPotentiallyNeedingManualMigration[currentBookName] = new List<int>(new[] {LastChapterStarted});
		}
	}
}
