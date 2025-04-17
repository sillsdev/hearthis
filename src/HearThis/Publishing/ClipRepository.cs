// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DesktopAnalytics;
using L10NSharp;
using SIL.CommandLineProcessing;
using SIL.IO;
using SIL.Extensions;
using SIL.Progress;
using SIL.Reporting;
using HearThis.Script;
using NAudio.Wave;
using static System.Int32;
using static System.IO.Path;
using static System.String;
using static HearThis.Program;
using static HearThis.Script.ParatextScriptProvider;

namespace HearThis.Publishing
{
	/// <summary>
	/// Each script block is recorded and each clip stored as its own file.  This class manages that collection of files.
	/// </summary>
	public static class ClipRepository
	{
		internal const string kSkipFileExtension = "skip";
		internal const string kBackupFileExtension = "bak";

		#region Retrieval and Deletion methods

		/// <summary>
		/// Gets the path to the indicated line. If a script provider is provided that implements
		/// IActorCharacterProvider and there is a current character, lineNumber is relative to the
		/// lines for that character.
		/// </summary>
		/// <param name="projectName">Paratext short name, text Release Bundle project name
		/// (language code + underscore + internal name), or multi-voice recording project
		/// name (with GUID)</param>
		/// <param name="bookName">English Scripture book name (spelled out)</param>
		/// <param name="chapterNumber">1-based (0 represents the introduction)</param>
		/// <param name="lineNumber">0-based (does not necessarily/typically correspond to verse
		/// numbers). When called for a project that uses a filtered set of blocks, this should
		/// be the filtered/apparent block number. In this case, the scriptProvider MUST be
		/// supplied in order for this to be translated into a real (persisted) block number.
		/// </param>
		/// <param name="scriptProvider">Used to translate a filtered/apparent block number
		/// into a real (persisted) block number. Optional if project does not use a script that
		/// involves this kind of filtering.</param>
		/// <returns>The actual file name of the .wav file, with fully-qualified path.</returns>
		public static string GetPathToLineRecording(string projectName, string bookName,
			int chapterNumber, int lineNumber, IScriptProvider scriptProvider = null)
		{
			return GetClipFileInfo(projectName, bookName, chapterNumber, lineNumber, scriptProvider, out _);
		}

		public static DateTime GetActualCreationTimeOfLineRecording(string projectName, string bookName,
			int chapterNumber, int lineNumber, IScriptProvider scriptProvider = null)
		{
			return new FileInfo(GetPathToLineRecording(projectName, bookName,
				chapterNumber, lineNumber, scriptProvider)).CreationTime;
		}

		public static DateTime GetActualClipBackupRecordingTime(string projectName, string bookName,
			int chapterNumber, int lineNumber, IScriptProvider scriptProvider = null)
		{
			return new FileInfo(GetPathToBackup(projectName, bookName,
				chapterNumber, lineNumber, scriptProvider)).CreationTime;
		}

		public static bool SkipFileExists(string projectName, string bookName,
			int chapterNumber, int fileNumber)
		{
			var chapter = GetChapterFolder(projectName, bookName, chapterNumber);
			return File.Exists(Combine(chapter, fileNumber + $".{kSkipFileExtension}"));
		}

		/// <summary>
		/// Gets the path to the indicated line. If a script provider is provided that implements
		/// IActorCharacterProvider and there is a current character, lineNumber is relative to the
		/// lines for that character.
		/// </summary>
		/// <param name="projectName">Paratext short name, text Release Bundle project name
		/// (language code + underscore + internal name), or multi-voice recording project
		/// name (with GUID)</param>
		/// <param name="bookName">English Scripture book name (spelled out)</param>
		/// <param name="chapterNumber">1-based (0 represents the introduction)</param>
		/// <param name="lineNumber">0-based (does not necessarily/typically correspond to verse
		/// numbers). When called for a project that uses a filtered set of blocks, this should
		/// be the filtered/apparent block number. In this case, the scriptProvider MUST be
		/// supplied in order for this to be translated into a real (persisted) block number.
		/// </param>
		/// <param name="scriptProvider">Used to translate a filtered/apparent block number
		/// into a real (persisted) block number. Optional if project does not use a script that
		/// involves this kind of filtering.</param>
		/// <returns>An object representing the clip file</returns>
		public static IClipFile GetClipFile(string projectName, string bookName, int chapterNumber,
			int lineNumber, IScriptProvider scriptProvider = null)
		{
			var filePath = GetClipFileInfo(projectName, bookName, chapterNumber, lineNumber, scriptProvider, out var fileNumber);
			return new BlockClipOrSkipFile(filePath, fileNumber);
		}
		
		private static string GetClipFileInfo(string projectName, string bookName,
			int chapterNumber, int lineNumber, IScriptProvider scriptProvider, out int fileNumber)
		{
			var chapter = GetChapterFolder(projectName, bookName, chapterNumber);
			fileNumber = GetRealLineNumber(bookName, chapterNumber, lineNumber, scriptProvider);
			return Combine(chapter, fileNumber + ".wav");
		}

		public static string GetPathToLineRecordingUnfiltered(string projectName, string bookName, int chapterNumber, int lineNumber)
		{
			// Not passing a script provider means that line number won't get adjusted.
			return GetPathToLineRecording(projectName, bookName, chapterNumber, lineNumber);
		}

		// When HearThis is filtering by character, generally it pretends the only blocks a chapter has are the ones that
		// character is supposed to record. However, the clip file has to use the real block number (actually one less
		// than the number recorded in the block) so that clips from different characters don't overwrite each other.
		// This routine converts from a possibly-filtered block number address to a real one.
		private static int GetRealLineNumber(string bookName, int chapterNumber, int lineNumber, IScriptProvider scriptProvider)
		{
			var adjustedLineNumber = lineNumber;
			if (scriptProvider != null)
			{
				var bookNumber = scriptProvider.VersificationInfo.GetBookNumber(bookName);
				// We do sometimes find ourselves in an unrecorded chapter asking for the path to block 0.
				// That will crash GetBlock, so check.
				if (scriptProvider.GetScriptBlockCount(bookNumber, chapterNumber) > lineNumber)
				{
					var block = scriptProvider.GetBlock(bookNumber, chapterNumber, lineNumber);
					adjustedLineNumber = block.Number - 1;
				}
			}
			return adjustedLineNumber;
		}

		/// <summary>
		/// See whether we have the specified clip. If a scriptProvider is passed which implements IActorCharacterProvider
		/// and it has a current character, lineNumber is relative to the lines for that character.
		/// </summary>
		public static bool HasClip(string projectName, string bookName, int chapterNumber, int lineNumber, IScriptProvider scriptProvider = null)
		{
			var path = GetPathToLineRecording(projectName, bookName, chapterNumber, lineNumber, scriptProvider);
			return File.Exists(path);
		}

		public static bool HasClipUnfiltered(string projectName, string bookName, int chapterNumber, int lineNumber)
		{
			// Not passing a script provider ensures that the line number won't get adjusted.
			return HasClip(projectName, bookName, chapterNumber, lineNumber);
		}

		/// <summary>
		/// Gets the path to the backup file for the indicated line.
		/// </summary>
		/// <param name="projectName">Paratext short name, text Release Bundle project name
		/// (language code + underscore + internal name), or multi-voice recording project
		/// name (with GUID)</param>
		/// <param name="bookName">English Scripture book name (spelled out)</param>
		/// <param name="chapterNumber">1-based (0 represents the introduction)</param>
		/// <param name="lineNumber">0-based (does not necessarily/typically correspond to verse
		/// numbers).</param>
		/// <param name="scriptProvider">Used to translate a filtered/apparent block number
		/// into a real (persisted) block number. Optional if project does not use a script that
		/// involves this kind of filtering.</param>
		/// <returns>The actual file name of the backup file, with fully-qualified path.</returns>
		public static string GetPathToBackup(string projectName, string bookName,
			int chapterNumber, int lineNumber, IScriptProvider scriptProvider = null)
		{
			return ChangeExtension(GetPathToLineRecording(projectName, bookName,
				chapterNumber, lineNumber, scriptProvider), kBackupFileExtension);
		}

		public static bool HasBackupFile(string projectName, string book, int chapter, int line) =>
			File.Exists(GetPathToBackup(projectName, book, chapter, line));

		public static string GetChapterFolder(string projectName, string bookName, int chapterNumber)
		{
			var book = GetBookFolder(projectName, bookName);
			var chapter = Utils.CreateDirectory(book, chapterNumber.ToString());
			return chapter;
		}

		private static string GetBookFolder(string projectName, string bookName)
		{
			var project = GetProjectFolder(projectName);
			var book = Utils.CreateDirectory(project, bookName.Trim());
			return book;
		}

		public static string GetProjectFolder(string projectName)
		{
			return GetApplicationDataFolder(projectName);
		}

		public static int GetCountOfRecordingsInFolder(string path, IScriptProvider scriptProvider)
		{
			if (!Directory.Exists(path))
				return 0;
			var provider = scriptProvider as IActorCharacterProvider;
			var soundFilesInFolder = GetSoundFilesInFolder(path);
			if (provider == null)
				return soundFilesInFolder.Length;
			if (!TryParse(GetFileName(path), out var chapter))
				return 0; // Probably a copy of a folder made for "backup" purposes - don't count it. (Note: Current production code can't hit this, but since this is a public method, we'll play it safe.)
			var bookName = GetFileName(GetDirectoryName(path));
			int book = scriptProvider.VersificationInfo.GetBookNumber(bookName);
			return soundFilesInFolder.Count(f =>
			{
				if (!TryParse(GetFileNameWithoutExtension(f), out var lineNo0Based))
					return false; // don't count files whose names don't parse as numbers
				return provider.IsBlockInCharacter(book, chapter, lineNo0Based);
			});
		}

		/// <summary>
		/// Checks to see whether the given file is not a valid WAV file. If it isn't,
		/// it deletes it.
		/// </summary>
		/// <returns><c>true</c> if the file was invalid (or -- if progress is set -- if it is
		/// missing); <c>false</c>, otherwise.</returns>
		public static bool IsInvalidClipFile(string filename, IProgress progress = null)
		{
			if (IsNullOrEmpty(filename))
				throw new ArgumentNullException(nameof(filename));

			if (!File.Exists(filename))
			{
				if (progress == null)
				{
					throw new FileNotFoundException(
						$"Cannot check validity of nonexistent file: {filename}", filename);
				}

				// This happened during an operation (e.g., publishing) that is likely processing
				// lots of files and therefore could be using a collection of files that no longer
				// matches what is on disk. Though unexpected, perhaps the user or some other
				// clean-up process already got rid of this file. We were just going to delete it
				// if it was invalid anyway, so let's just log the oddity and report it as though
				// we deleted it.
				progress.WriteWarning(LocalizationManager.GetString(
					"ClipRepository.ClipNoLongerExistsWarning",
					"Attempted to check validity of nonexistent file: {0}"), filename);
				Analytics.Track("Clip not found while publishing", new Dictionary<string, string>
				{
					{"filename", filename}
				});
				return true;
			}

			try
			{
				using (var _ = new WaveFileReader(filename))
					return false;
			}
			catch (Exception e)
			{
				var msg = Format(LocalizationManager.GetString("ClipRepository.InvalidClip",
					"Invalid WAV file {0}\r\n{1}\r\n{2} will attempt to delete it.",
					"Param 0: WAV file name; " +
					"Param 1: Error message; " +
					"Param 2: \"HearThis\" (product name)"),
					filename, e.Message, kProduct);
				Logger.WriteEvent(msg);
				progress?.WriteError(msg);
			}

			try
			{
				RobustFile.Delete(filename);
			}
			catch (Exception e)
			{
				var msg = Format(LocalizationManager.GetString("ClipRepository.DeleteInvalidClipProblem",
						"Failed to delete invalid WAV file: {0}", "Param 0: WAV file name."), filename);

				Console.WriteLine(msg);

				if (progress == null)
				{
					ErrorReport.ReportNonFatalExceptionWithMessage(e, msg);
				}
				else
				{
					progress.WriteException(e);
					progress.WriteError(msg);
					throw;
				}
			}

			return true;
		}

		private static IEnumerable<string> GetNumericDirectories(string path)
		{
			if (Directory.Exists(path))
				return Directory.GetDirectories(path).Where(dir => TryParse(GetFileName(dir), out int _));
			throw new DirectoryNotFoundException($"GetNumericDirectories called with invalid path: {path}");
		}

		public static int GetCountOfRecordingsForBook(string projectName, string name, IScriptProvider scriptProvider)
		{
			var path = GetBookFolder(projectName, name);
			if (!Directory.Exists(path))
				return 0;
			return GetNumericDirectories(path).Sum(directory => GetCountOfRecordingsInFolder(directory, scriptProvider));
		}

		public static bool HasRecordingsForProject(string projectName)
		{
			return Directory.GetDirectories(GetApplicationDataFolder(projectName))
				.Any(bookDirectory => GetNumericDirectories(bookDirectory).Any(chDirectory => GetSoundFilesInFolder(chDirectory).Length > 0));
		}

		// line number is not character-filtered.
		public static bool DeleteLineRecording(string projectName, string bookName, int chapterNumber, int lineNumber)
		{
			// just being careful...
			if (HasClipUnfiltered(projectName, bookName, chapterNumber, lineNumber))
			{
				return DeleteClipWithBackup(
					GetPathToLineRecordingUnfiltered(projectName, bookName, chapterNumber, lineNumber));
			}
			return false;
		}

		/// <summary>
		/// Deletes a WAV file that is beyond the extent of the known blocks
		/// for the script.
		/// </summary>
		/// <param name="countOfKnownBlocks">The known number of real blocks in this chapter
		/// (from the scriptProvider)</param>
		/// <param name="projectName">Name of the project</param>
		/// <param name="bookName">English name of the Scripture book</param>
		/// <param name="chapterNumber">Chapter number (0 indicates intro)</param>
		/// <param name="iExtraClip">0-based index into the excess books. Typically, this will
		/// be one less than the number of the file to be deleted, but in cases where there are
		/// "holes" in the excess files (i.e., they are not contiguous), it will not be.</param>
		/// <returns>If successful, returns the number of the wav file that was deleted;
		/// otherwise -1.</returns>
		public static int DeleteExtraRecording(int countOfKnownBlocks, string projectName,
			string bookName, int chapterNumber, int iExtraClip)
		{
			var list = GetAllExcessClipFiles(countOfKnownBlocks, projectName,
				bookName,chapterNumber).ToList();
			if (iExtraClip >= list.Count || iExtraClip < 0)
				throw new ArgumentOutOfRangeException(nameof(iExtraClip), iExtraClip,
					$"{bookName} {chapterNumber} has {list.Count} extra clips.");

			var path = list[iExtraClip];
			return DeleteClipWithBackup(path) ? Parse(GetFileNameWithoutExtension(path)): -1;
		}

		public static int GetAdjustedIndexForExtraRecordingBasedOnDeletedClips(
			IReadOnlyList<ExtraRecordingInfo> extraRecordings, int index)
		{
			return index - extraRecordings.Select(er => er.ClipFile).Count(f => !File.Exists(f));
		}

		private static bool DeleteClipWithBackup(string path)
		{
			var backupPath = ChangeExtension(path, kBackupFileExtension);
			try
			{
				RobustFile.Move(path, backupPath, true);
				return true;
			}
			catch (IOException err)
			{
				ErrorReport.NotifyUserOfProblem(err,
					Format(LocalizationManager.GetString("ClipRepository.DeleteClipProblem",
						"HearThis was unable to delete this clip. File may be locked. Restarting HearThis might solve this problem. File: {0}"), path));
				return false;
			}
		}

		// line number is not character-filtered.
		public static bool UndeleteLineRecording(string projectName, BookInfo book,
			ChapterInfo chapterInfo, int lineNumber)
		{
			var bookName = book.Name;
			int chapterNumber = chapterInfo.ChapterNumber1Based;
			if (HasClipUnfiltered(projectName, bookName, chapterNumber, lineNumber))
				return false; // At least for now, do not allow overwriting current clip.

			var path = GetPathToLineRecordingUnfiltered(projectName, bookName, chapterNumber, lineNumber);
			var backupPath = ChangeExtension(path, kBackupFileExtension);

			var deletedScriptLine = book.GetUnfilteredBlock(chapterNumber, lineNumber);
			try
			{
				RobustFile.Move(backupPath, path);
				chapterInfo.OnClipUndeleted(deletedScriptLine);
				return true;
			}
			catch (Exception e)
			{
				if (deletedScriptLine != null &&
				    (e is FileNotFoundException || e is InvalidFileException))
				{
					chapterInfo.DeletedRecordings.Remove(deletedScriptLine);
					chapterInfo.Save();
				}

				throw;
			}
		}

		/// <summary>
		/// Class representing a (WAV) file that stores either a recorded audio clip or a "skip"
		/// file corresponding to a block in the script. Note: a "skip" file indicates the user
		/// decided not to record the corresponding block.
		/// </summary>
		private class BlockClipOrSkipFile : IClipFile
		{
			public string FilePath { get; private set; }
			public int Number { get; private set; }
			private FileInfo _fileInfo;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="filePath">The actual file name of the .wav or .skip file, with
			/// fully-qualified path.</param>
			/// <param name="fileNumber">The numeric value corresponding to the file name.
			/// This is a 0-based block number. (Note: the Block/Line numbers displayed to
			/// the user and stored in the the chapter info files are 1-based.)</param>
			public BlockClipOrSkipFile(string filePath, int fileNumber)
			{
				FilePath = filePath;
				Number = fileNumber;
			}
			
			public void Delete()
			{
				RobustFile.Delete(FilePath);
				FilePath = null;
				Number = MinValue;
				_fileInfo = null;
			}

			/// <summary>
			/// Shift file the specified number of block positions. Caller is responsible for
			/// ensuring that the destination position is free of a conflicting clip or skip
			/// file.
			/// </summary>
			/// <param name="positions">The number of positions forward (positive) or backward
			/// (negative) to move the file</param>
			public void ShiftPosition(int positions)
			{
				var destPath = GetIntendedDestinationPath(positions);
				// This intentionally does NOT overwrite. It will fail if caller attempts to
				// move a clip or skip file to a destination file that exists.
				RobustFile.Move(FilePath, destPath);
				try
				{
					// If there is a backup file in the dest position, attempt to clean it up.
					RobustFile.Delete(ChangeExtension(destPath, kBackupFileExtension));
				}
				catch (Exception e)
				{
					// Oh, well. We tried.
					Console.WriteLine(e);
				}
				FilePath = destPath;
				Number += positions;
				_fileInfo = null;
			}

			public string GetIntendedDestinationPath(int positions) =>
				Combine(Directory, ChangeExtension((Number + positions).ToString(), Extension));

			private FileInfo FileInfo => _fileInfo ?? (_fileInfo = new FileInfo(FilePath));

			public DateTime LastWriteTimeUtc  => FileInfo.LastWriteTimeUtc;

			private string Directory
			{
				get
				{
					var directory = GetDirectoryName(FilePath);
					if (directory == null)
						throw new ArgumentException($"ClipOrSkipFile created using a filename that is not valid: {FilePath}");
					return GetDirectoryName(FilePath);
				}
			}

			private string Extension => GetExtension(FilePath);
		}

		private class BlockClipOrSkipFileComparer : IComparer<BlockClipOrSkipFile>
		{
			private readonly int _direction;

			public BlockClipOrSkipFileComparer(bool ascending)
			{
				_direction = ascending ? 1 : -1;
			}

			public int Compare(BlockClipOrSkipFile x, BlockClipOrSkipFile y)
			{
				if (ReferenceEquals(x, y))
					return 0;
				if (ReferenceEquals(null, y))
					return 1 * _direction;
				if (ReferenceEquals(null, x))
					return -1 * _direction;
				return x.Number.CompareTo(y.Number) * _direction;
			}
		}

		private static IEnumerable<BlockClipOrSkipFile> AllClipAndSkipFiles(string projectName, string bookName, int chapterNumber1Based)
		{
			foreach (var file in FilesInChapterFolder(projectName, bookName, chapterNumber1Based))
			{
				var extension = GetExtension(file);
				if ((extension == ".wav" || extension == ".skip") &&
					TryParse(GetFileNameWithoutExtension(file), out var lineNumberForFile))
					yield return new BlockClipOrSkipFile(file, lineNumberForFile);
			}
		}

		private static IEnumerable<string> FilesInChapterFolder(string projectName, string bookName, int chapterNumber1Based)
			=> Directory.GetFiles(GetChapterFolder(projectName, bookName, chapterNumber1Based));

		/// <summary>
		/// lineNumber is unfiltered
		/// </summary>
		public static void DeleteAllClipsAfterLine(string projectName, string bookName, ChapterInfo chapter, int lineNumber)
		{
			foreach (var file in AllClipAndSkipFiles(projectName, bookName, chapter.ChapterNumber1Based))
			{
				if (file.Number > lineNumber)
					file.Delete();
			}

			// Saving has a side effect of removing any orphaned (not corresponding to a WAV file)
			// recording info entries beyond the last known block.
			chapter.Save();
		}

		public static void BackUpRecordingForSkippedLine(string projectName, string bookName, int chapterNumber1Based, int block, IScriptProvider scriptProvider = null)
		{
			var recordingPath = GetPathToLineRecording(projectName, bookName, chapterNumber1Based, block, scriptProvider);
			if (File.Exists(recordingPath))
				RobustFile.Move(recordingPath, ChangeExtension(recordingPath, kSkipFileExtension), true);
		}

		/// <summary>
		/// This restores a clip from a skip file. Use UndeleteLineRecording to
		/// restore from a backup file.
		/// </summary>
		/// <param name="projectName">Paratext short name, text Release Bundle project name
		/// (language code + underscore + internal name), or multi-voice recording project
		/// name (with GUID)</param>
		/// <param name="bookName">English Scripture book name (spelled out)</param>
		/// <param name="chapterNumber1Based">1-based (0 represents the introduction)</param>
		/// <param name="lineNumber">0-based (does not necessarily/typically correspond to verse
		/// numbers). When called for a project that uses a filtered set of blocks, this should
		/// be the filtered/apparent block number. In this case, the scriptProvider MUST be
		/// supplied in order for this to be translated into a real (persisted) block number.
		/// </param>
		/// <param name="scriptProvider">Used to translate a filtered/apparent block number
		/// into a real (persisted) block number. Optional if project does not use a script that
		/// involves this kind of filtering.</param>
		/// <returns>Flag indicating whether the backed up clip was restored.</returns>
		public static bool RestoreBackedUpClip(string projectName, string bookName,
			int chapterNumber1Based, int lineNumber, IScriptProvider scriptProvider = null)
		{
			var recordingPath = GetPathToLineRecording(projectName, bookName, chapterNumber1Based,
				lineNumber, scriptProvider);
			var skipPath = ChangeExtension(recordingPath, kSkipFileExtension);
			if (File.Exists(skipPath))
			{
				if (File.Exists(recordingPath))
				{
					// HT-465: This should never happen, but apparently can. I have not found a way
					// to reproduce it and I'm not 100% sure what to do when it happens, but I
					// think it makes sense to keep the newer file, back up the older one, tell the
					// user something is wrong and ask them to report it, so I can try to gather
					// information that might enable me to fix it.
					var skipWriteTime = new FileInfo(skipPath).LastWriteTimeUtc;
					var clipWriteTime = new FileInfo(recordingPath).LastWriteTimeUtc;
					var details = new Dictionary<string, string>
					{
						["recordingPath"] = recordingPath,
						["skipWriteTime"] = skipWriteTime.ToISO8601TimeFormatWithUTCString(),
						["clipWriteTime"] = clipWriteTime.ToISO8601TimeFormatWithUTCString()
					};
					Analytics.Track("BothSkipAndClipExist", details);

					bool filesAreIdentical;
					try
					{
						filesAreIdentical = RobustFile.ReadAllBytes(skipPath).AreByteArraysEqual(RobustFile.ReadAllBytes(recordingPath));
					}
					catch // Arg! Now what? Probably unlikely that the files are identical.
					{
						filesAreIdentical = false;
					}

					if (filesAreIdentical)
					{
						RobustFile.Delete(skipPath);
						return false;
					}

					var msg = Format(LocalizationManager.GetString("ClipRepository.BothSkipAndClipExist",
						"{0} found an existing clip for a block that was marked as being skipped.",
							"Low-priority for localization. Param 0: \"HearThis\" (product name)"), kProduct);

					if (skipWriteTime.CompareTo(clipWriteTime) > 0)
					{
						// Skip file is newer. Back up existing clip and replace it with the restored skip file.
						var backedUpClipFile = ChangeExtension(recordingPath, "oldclip.wav");
						RobustFile.Delete(backedUpClipFile); // Unlikely to exist, but just in case.
						RobustFile.Move(recordingPath, backedUpClipFile);
						ErrorReport.ReportNonFatalMessageWithStackTrace(msg + " " +
							Format(LocalizationManager.GetString("ClipRepository.SkipFileIsNewer",
							"Because the skip file is newer, that file is replacing the existing" +
							" clip, but the other version is being kept as {0}",
							"Low-priority for localization. Param 0: file name"), backedUpClipFile));
					}
					else
					{
						// Recording is newer (or exactly the same, though that's probably impossible). Back up
						// skip file.
						var backedUpSkipFile = skipPath + "old.wav";
						RobustFile.Delete(backedUpSkipFile); // Unlikely to exist, but just in case.
						RobustFile.Move(skipPath, backedUpSkipFile);
						ErrorReport.ReportNonFatalMessageWithStackTrace(msg + " " +
							Format(LocalizationManager.GetString("ClipRepository.SkipFileIsNewer",
								"Because the existing clip is newer, that file is being kept, " +
								"but the other version is being kept as {0}",
								"Low-priority for localization. Param 0: file name"), backedUpSkipFile));
						return false;
					}
				}
				RobustFile.Move(skipPath, recordingPath);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Shifts the requested clip files and adjusts the Number of their corresponding
		/// ChapterInfo records (if they exist)
		/// </summary>
		/// <param name="projectName">The name of the HearThis project</param>
		/// <param name="bookName">The English (short) name of the Scripture book</param>
		/// <param name="chapterNumber1Based">The chapter (0 => intro)</param>
		/// <param name="iBlock">The 0-based index of the first clip to shift (corresponds
		/// to the actual number/name of the file)</param>
		/// <param name="blockCount">The number of clips to shift. (Assuming caller wants to
		/// shift a contiguous run of clips, caller is responsible for ensuring that there are
		/// no gaps in the existing sequence of clips. (Gaps are not counted as clips.) If
		/// this number is greater than the number of existing clips starting from
		/// <see cref="iBlock"/>, all the remaining clips will be shifted. This might include
		/// clips that are "extras" (beyond the clips accounted for by the source text).</param>
		/// <param name="offset">The number of positions to shift clips forward (positive) or
		/// backward (negative). A value of 0 is not an error, but it results in no change.</param>
		/// <param name="getRecordingInfo">A function to get the recording information for the
		/// chapter specified by <see cref="chapterNumber1Based"/>.</param>
		/// <returns>A result indicating the actual number of clips that were attempted to be
		/// shifted, the number successfully shifted and any error that occurred. Note that the
		/// number attempted will typically be <see cref="blockCount"/> but can be less if the
		/// caller requested to shift more clips than were present.</returns>
		public static ClipShiftingResult ShiftClips(string projectName,
			string bookName, int chapterNumber1Based, int iBlock, int blockCount, int offset,
			Func<ChapterRecordingInfoBase> getRecordingInfo)
		{
			if (offset == 0) // meaningless
				return new ClipShiftingResult(0);
			return ShiftClips(projectName, bookName, chapterNumber1Based, iBlock, offset,
				getRecordingInfo, blockCount);
		}

		// HT-376: Unfortunately, HT v. 2.0.3 introduced a change whereby the numbering of
		// existing clips could be out of sync with the data, so any chapter with one of the
		// new default SkippedParagraphStyles that has not had anything recorded since the
		// migration to that version needs to have clips shifted forward to account for the
		// new blocks (even though they are most likely skipped). (Any chapter where the user
		// has recorded something since the migration to that version could also be affected,
		// but the user will have to migrate it manually -- unless ALL the clips in that
		// chapter were recorded after the migration -- because we can't know which clips
		// might need to be moved.) If a "default" cutoff date is specified, then we can
		// safely migrate any affected chapters. If this returns false, it indicates that this
		// chapter might require manual cleanup.
		public static bool ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate(string projectName,
			string bookName, int chapterNumber1Based, int iBlock, DateTime cutoff,
			Func<ChapterRecordingInfoBase> getRecordingInfo)
		{
			var result = ShiftClips(projectName, bookName, chapterNumber1Based, iBlock, 1,
				getRecordingInfo, cutoff:cutoff, preserveModifiedTime:true);
			if (result.Error != null)
				throw result.Error;
			return result.Attempted == result.SuccessfulMoves;
		}

		public static IEnumerable<string> GetAllExcessClipFiles(int countOfKnownSegments,
			string projectName, string bookName, int chapterNumber)
		{
			var dictionary = new SortedDictionary<int, string>();
			foreach (var file in FilesInChapterFolder(projectName, bookName, chapterNumber))
			{
				var extension = GetExtension(file);
				if (extension == ".wav" &&
					TryParse(GetFileNameWithoutExtension(file), out var lineNumberForFile) &&
					lineNumberForFile >= countOfKnownSegments)
				{
					dictionary[lineNumberForFile] = file;
				}
			}

			return dictionary.Values;
		}

		private static ClipShiftingResult ShiftClips(string projectName, string bookName, int chapterNumber,
			int iStartBlock, int offset, Func<ChapterRecordingInfoBase> getRecordingInfo,
			int blockCount = MaxValue, DateTime cutoff = default, bool preserveModifiedTime = false)
		{
			Debug.Assert(offset != 0);
			ClipShiftingResult result = null;
			try
			{
				var allFilesAfterBlock = AllClipAndSkipFiles(projectName, bookName, chapterNumber)
					.Where(f => f.Number >= iStartBlock).ToArray();
				if (allFilesAfterBlock.Length == 0)
					return new ClipShiftingResult(0);
				if (cutoff != default && allFilesAfterBlock.Any(f => f.LastWriteTimeUtc >= cutoff))
					return new ClipShiftingResult(allFilesAfterBlock.All(f => f.LastWriteTimeUtc >= cutoff) ? 0 : allFilesAfterBlock.Length);

				BlockClipOrSkipFile[] filesToShift;
				if (blockCount >= allFilesAfterBlock.Length)
				{
					// We have to move them in the correct order to avoid clobbering the next one.
					allFilesAfterBlock.Sort(new BlockClipOrSkipFileComparer(offset < 0));
					filesToShift = allFilesAfterBlock;
				}
				else
				{
					// We first have to sort them in ascending order to get the correct ones
					allFilesAfterBlock.Sort(new BlockClipOrSkipFileComparer(true));
					var theFilesWeWant = allFilesAfterBlock.Take(blockCount);
					// Now get them in the correct order to avoid clobbering the next one.
					filesToShift = offset > 0 ? theFilesWeWant.Reverse().ToArray() :
						theFilesWeWant.ToArray();
				}
				result = new ClipShiftingResult(filesToShift.Length);
				foreach (var file in filesToShift)
				{
					result.LastAttemptedMove = file;
					file.ShiftPosition(offset);
					result.SuccessfulMoves++;
				}

				result.LastAttemptedMove = null;

				getRecordingInfo().AdjustLineNumbers(iStartBlock, offset, blockCount, preserveModifiedTime);
			}
			catch (Exception e)
			{
				if (result == null)
					result = new ClipShiftingResult(-1);
				result.Error = e;
			}
			return result;
		}

		public class ClipShiftingResult
		{
			public int Attempted { get; }
			public int SuccessfulMoves { get; internal set; }
			public IClipFile LastAttemptedMove { get; internal set; }
			public Exception Error;

			internal ClipShiftingResult(int plannedMoves)
			{
				Attempted = plannedMoves;
			}
		}
		#endregion

		#region Publishing methods

		public static void PublishAllBooks(PublishingModel publishingModel, string projectName,
			string publishRoot, IProgress progress)
		{
			if (!RobustIO.DeleteDirectoryAndContents(publishRoot))
			{
				progress.WriteError(Format(LocalizationManager.GetString("ClipRepository.DeleteFolder",
					"Existing folder could not be deleted: {0}"), publishRoot));
				return;
			}

			var bookNames = new List<string>(Directory.GetDirectories(GetApplicationDataFolder(projectName)).Select(GetFileName));
			bookNames.Sort(publishingModel.PublishingInfoProvider.BookNameComparer);

			foreach (var bookName in bookNames)
			{
				if (progress.CancelRequested)
					return;
				PublishAllChapters(publishingModel, projectName, bookName, publishRoot, progress);
				if (progress.ErrorEncountered)
					return;
			}
		}

		public static void PublishAllChapters(PublishingModel publishingModel, string projectName,
			string bookName, string publishRoot, IProgress progress)
		{
			if (!publishingModel.IncludeBook(bookName)) // Maybe book has been deleted in Paratext.
				return;

			var bookFolder = GetBookFolder(projectName, bookName);
			var chapters = new List<int>(GetNumericDirectories(bookFolder).Select(dir => Parse(GetFileName(dir))));
			chapters.Sort();
			foreach (var chapterNumber in chapters)
			{
				if (progress.CancelRequested)
					return;
				PublishSingleChapter(publishingModel, projectName, bookName, chapterNumber, publishRoot, progress);
				if (progress.ErrorEncountered)
					return;
			}
		}

		private static string[] GetSoundFilesInFolder(string path) =>
			Directory.GetFiles(path, "*.wav");

		public static bool GetDoAnyClipsExistForProject(string projectName)
		{
			return Directory.GetFiles(GetApplicationDataFolder(projectName), "*.wav", SearchOption.AllDirectories).Any();
		}

		public static bool GetDoAnyClipsExistInChapter(string projectName, string bookName, int chapter)
		{
			return GetSoundFilesInFolder(GetChapterFolder(projectName, bookName, chapter)).Any();
		}

		private static void PublishSingleChapter(PublishingModel publishingModel, string projectName,
			string bookName, int chapterNumber, string rootPath, IProgress progress)
		{
			try
			{
				var clipFiles = GetSoundFilesInFolder(GetChapterFolder(projectName, bookName, chapterNumber));
				if (clipFiles.Length == 0)
					return;

				// If a clip file is invalid, it will cause the export to abort. Although rare, it
				// is annoying and confusing to users. Better to just delete the bogus file and let
				// the user know.
				if (RemoveInvalidWavFiles(progress, ref clipFiles) && clipFiles.Length == 0)
					return;

				clipFiles = clipFiles.OrderBy(name =>
				{
					if (TryParse(GetFileNameWithoutExtension(name), out var result))
						return result;
					throw new Exception(Format(LocalizationManager.GetString("ClipRepository.UnexpectedWavFile", "Unexpected WAV file: {0}"), name));
				}).ToArray();

				publishingModel.FilesInput += clipFiles.Length;
				publishingModel.FilesOutput++;

				progress.WriteMessage("{0} {1}", bookName, chapterNumber.ToString());

				string pathToJoinedWavFile = GetTempPath().CombineForPath("joined.wav");
				using (TempFile.TrackExisting(pathToJoinedWavFile))
				{
					MergeAudioFiles(clipFiles, pathToJoinedWavFile, progress);

					PublishVerseIndexFiles(rootPath, bookName, chapterNumber, clipFiles, publishingModel, progress);

					var lastClipFile = clipFiles.LastOrDefault();
					if (lastClipFile != null)
					{
						int lineNumber = Parse(GetFileNameWithoutExtension(lastClipFile));
						try
						{
							publishingModel.PublishingInfoProvider.GetUnfilteredBlock(bookName, chapterNumber, lineNumber);
						}
						catch (ArgumentOutOfRangeException)
						{
							progress.WriteWarning(Format(LocalizationManager.GetString("ClipRepository.ExtraneousClips",
								"Unexpected clips were encountered in the folder for {0} {1}.",
								"Param 0: Book name; Param 1: Chapter number"), bookName, chapterNumber));
						}
					}
					publishingModel.PublishingMethod.PublishChapter(rootPath, bookName, chapterNumber, pathToJoinedWavFile,
						progress);
				}
			}
			catch (Exception error)
			{
				progress.WriteError(error.Message);
			}
		}

		private static bool RemoveInvalidWavFiles(IProgress progress, ref string[] clipFiles)
		{
			// Although it is rare, we occasionally encounter a clip file that is corrupt
			// (usually empty). We don't know what causes this. It could be something in
			// HearThis, but I'm guessing that it is some kind of hardware glitch or system
			// software failure. This causes the export to fail, and sadly the
			// only recourse is to re-record it (or restore it from a backup if a valid
			// backup exists).
			var removedAny = false;
			for (var i = 0; i < clipFiles.Length; i++)
			{
				if (IsInvalidClipFile(clipFiles[i], progress))
				{
					clipFiles[i] = null;
					removedAny = true;
				}
			}

			if (removedAny)
				clipFiles = clipFiles.Where(f => f != null).ToArray();

			return removedAny;
		}

		internal static void MergeAudioFiles(IReadOnlyCollection<string> files, string pathToJoinedWavFile, IProgress progress)
		{
			var outputDirectoryName = GetDirectoryName(pathToJoinedWavFile);
			if (files.Count == 1)
			{
				RobustFile.Copy(files.First(), pathToJoinedWavFile, true);
			}
			else
			{
				var fileList = GetTempFileName();
				File.WriteAllLines(fileList, files.ToArray());
				progress.WriteMessage("   " + LocalizationManager.GetString("ClipRepository.MergeAudioProgress", "Joining clips", "Appears in progress indicator"));
				string arguments = Format("join -d \"{0}\" -F \"{1}\" -O always -r none", outputDirectoryName,
					fileList);
				RunCommandLine(progress, FileLocationUtilities.GetFileDistributedWithApplication(false, "shntool.exe"), arguments);

				// Passing just the directory name for output file means the output file is ALWAYS joined.wav.
				// It's possible to pass more of a file name, but that just makes things more complex, because
				// shntool will always prepend 'joined' to the name we really want.
				// Some callers actually want the name to be 'joined.wav'. If not, we just rename it afterwards.
				var outputFilePath = pathToJoinedWavFile;
				if (GetFileName(pathToJoinedWavFile) != "joined.wav")
				{
					outputFilePath = Combine(outputDirectoryName, "joined.wav");
				}
				if (!File.Exists(outputFilePath))
				{
					throw new ApplicationException(
						"Um... shntool.exe failed to produce the file of the joined clips. Reroute the power to the secondary transfer conduit.");
				}
				if (GetFileName(pathToJoinedWavFile) != "joined.wav")
				{
					RobustFile.Delete(pathToJoinedWavFile);
					RobustFile.Move(outputFilePath, pathToJoinedWavFile);
				}
			}
		}

		public static void RunCommandLine(IProgress progress, string exePath, string arguments, int timeoutInSeconds = 600)
		{
			progress.WriteVerbose(exePath + " " + arguments);
			ExecutionResult result = CommandLineRunner.Run(exePath, arguments, null, timeoutInSeconds, progress);
			result.RaiseExceptionIfFailed("");
		}

		/// <summary>
		/// Publish Audacity Label Files or cue sheet to text files
		/// </summary>
		public static void PublishVerseIndexFiles(string rootPath, string bookName, int chapterNumber, string[] verseFiles,
			PublishingModel publishingModel, IProgress progress)
		{
			// get the output path
			var outputPath = ChangeExtension(
				publishingModel.PublishingMethod.GetFilePathWithoutExtension(rootPath, bookName, chapterNumber), "txt");

			try
			{
				// clear the text file if it already exists
				RobustFile.Delete(outputPath);
			}
			catch (Exception error)
			{
				progress.WriteError(error.Message);
			}

			if (publishingModel.VerseIndexFormat != PublishingModel.VerseIndexFormatType.None)
			{
				string contents = GetVerseIndexFileContents(bookName, chapterNumber, verseFiles,
					publishingModel, outputPath);

				if (contents == null)
					return;

				try
				{
					using (StreamWriter writer = new StreamWriter(outputPath, false))
						writer.Write(contents);
				}
				catch (Exception error)
				{
					progress.WriteError(error.Message);
				}
			}
		}

		internal static string GetVerseIndexFileContents(string bookName, int chapterNumber, string[] verseFiles,
			PublishingModel publishingModel, string outputPath)
		{
			switch (publishingModel.VerseIndexFormat)
			{
				case PublishingModel.VerseIndexFormatType.AudacityLabelFileVerseLevel:
					return chapterNumber == 0 ? null :
						GetAudacityLabelFileContents(verseFiles, publishingModel.PublishingInfoProvider, bookName, chapterNumber, false);
				case PublishingModel.VerseIndexFormatType.AudacityLabelFilePhraseLevel:
					return GetAudacityLabelFileContents(verseFiles, publishingModel.PublishingInfoProvider, bookName, chapterNumber, true);
				case PublishingModel.VerseIndexFormatType.CueSheet:
					return GetCueSheetContents(verseFiles, publishingModel.PublishingInfoProvider, bookName, chapterNumber, outputPath);
				default:
					throw new InvalidEnumArgumentException(nameof(publishingModel.VerseIndexFormat),
						(int)publishingModel.VerseIndexFormat, typeof(PublishingModel.VerseIndexFormatType));
			}
		}

		internal static string GetCueSheetContents(string[] verseFiles, IPublishingInfoProvider infoProvider, string bookName,
			int chapterNumber, string outputPath)
		{
			var bldr = new StringBuilder();
			bldr.AppendFormat("FILE \"{0}\"", outputPath);
			bldr.AppendLine();

			TimeSpan indextime = new TimeSpan(0, 0, 0, 0);

			for (int i = 0; i < verseFiles.Length; i++)
			{
				bldr.AppendLine($"  TRACK {(i + 1):000} AUDIO");
				//    "  TRACK 0" + (i + 1) + " AUDIO");
				//else
				//    "  TRACK " + (i + 1) + " AUDIO";
				bldr.AppendLine("	TITLE 00000-" + bookName + chapterNumber + "-tnnC001 ");
				bldr.AppendLine("	INDEX 01 " + indextime);

				// get the length of the block
				using (var b = new NAudio.Wave.WaveFileReader(verseFiles[i]))
				{
					TimeSpan wavlength = b.TotalTime;

					//update the indextime for the verse
					indextime = indextime.Add(wavlength);
				}
			}
			return bldr.ToString();
		}

		internal static string GetAudacityLabelFileContents(string[] verseFiles, IPublishingInfoProvider infoProvider,
			string bookName, int chapterNumber, bool phraseLevel)
		{
			var audacityLabelFileBuilder = new AudacityLabelFileBuilder(verseFiles, infoProvider, bookName, chapterNumber,
				phraseLevel);
			return audacityLabelFileBuilder.ToString();
		}

		#region AudacityLabelFileBuilder class
		private class AudacityLabelFileBuilder
		{
			private readonly string[] verseFiles;
			private readonly IPublishingInfoProvider infoProvider;
			private readonly string bookName;
			private readonly int chapterNumber;
			private readonly bool phraseLevel;
			private readonly StringBuilder bldr = new StringBuilder();
			private readonly Dictionary<string, int> headingCounters = new Dictionary<string, int>();

			private ScriptLine block;
			private double startTime, endTime;
			private string prevVerse = null;
			private double accumClipTimeFromPrevBlocks = 0.0;
			private string currentVerse = null;
			private string nextVerse;
			private int subPhrase = -1;

			public AudacityLabelFileBuilder(string[] verseFiles, IPublishingInfoProvider infoProvider,
				string bookName, int chapterNumber, bool phraseLevel)
			{
				this.verseFiles = verseFiles;
				this.infoProvider = infoProvider;
				this.bookName = bookName;
				this.chapterNumber = chapterNumber;
				this.phraseLevel = phraseLevel;
			}

			public override string ToString()
			{
				WriteHeaderComments();

				for (int i = 0; i < verseFiles.Length; i++)
				{
					// get the length of the block
					double clipLength;
					using (var b = new NAudio.Wave.WaveFileReader(verseFiles[i]))
					{
						clipLength = b.TotalTime.TotalSeconds;
						//update the endTime for the verse
						endTime = endTime + clipLength;
					}

					// REVIEW: Use TryParse to avoid failure for extraneous filename?
					int lineNumber = Parse(GetFileNameWithoutExtension(verseFiles[i]));
					block = GetUnfilteredBlock(lineNumber);
					if (block == null)
						break;

					nextVerse = null;

					string label;
					if (block.Heading)
					{
						subPhrase = -1;
						label = GetHeadingBlockLabel();
					}
					else
					{
						if (chapterNumber == 0)
						{
							// Intro material
							subPhrase++;
							label = Empty;
						}
						else
						{
							ScriptLine nextBlock = null;
							if (i < verseFiles.Length - 1)
							{
								// Check next block
								int nextLineNumber = Parse(GetFileNameWithoutExtension(verseFiles[i + 1]));
								nextBlock = GetUnfilteredBlock(nextLineNumber);
								if (nextBlock != null)
								{
									nextVerse = nextBlock.CrossesVerseBreak
										? nextBlock.Verse.Substring(0, nextBlock.Verse.IndexOf('~'))
										: nextBlock.Verse;
								}
							}

							if (block.CrossesVerseBreak)
							{
								MakeLabelsForApproximateVerseLocationsInBlock(clipLength);
								continue;
							}

							// Current block is a normal verse or explicit verse bridge
							currentVerse = block.Verse;

							if (nextBlock != null)
							{
								Debug.Assert(currentVerse != null);

								if (phraseLevel)
								{
									// If this is the same as the next verse but different from the previous one, start
									// a new sub-verse sequence.
									if (!nextBlock.Heading && prevVerse != currentVerse &&
										(currentVerse == nextBlock.Verse ||
										(nextBlock.CrossesVerseBreak &&
										currentVerse == nextBlock.Verse.Substring(0, nextBlock.Verse.IndexOf('~')))))
									{
										subPhrase = 0;
									}
								}
								else if (!nextBlock.Heading && currentVerse == nextVerse)
								{
									// Same verse number.
									// For verse-level highlighting, postpone appending until we have the whole verse.
									prevVerse = currentVerse;
									accumClipTimeFromPrevBlocks += endTime - startTime;
									continue;
								}
							}

							label = currentVerse;
							UpdateSubPhrase();
						}
					}

					AppendLabel(startTime, endTime, label);

					// update start time for the next verse
					startTime = endTime;
					prevVerse = currentVerse;
				}

				return bldr.ToString();
			}

			private ScriptLine GetUnfilteredBlock(int lineNumber)
			{
				try
				{
					return infoProvider.GetUnfilteredBlock(bookName, chapterNumber, lineNumber);
				}
				catch (Exception)
				{
					return null;
				}
			}

			private void MakeLabelsForApproximateVerseLocationsInBlock(double clipLength)
			{
// Unless/until SAB can handle implicit verse bridges, we want to create a label
				// at approximately the right place (based on verse number offsets in text) for
				// each verse in the block.
				int ichVerse = 0;
				var verseOffsets = block.VerseOffsets.ToList();
				var textLen = block.Text.Length;
				verseOffsets.Add(textLen);
				int prevOffset = 0;
				double start = 0.0;
				foreach (var verseOffset in verseOffsets)
				{
					int ichVerseLim = block.Verse.IndexOf('~', ichVerse);
					if (ichVerseLim == -1)
					{
						currentVerse = block.Verse.Substring(ichVerse);
					}
					else
					{
						Debug.Assert(ichVerseLim <= block.Verse.Length - 2);
						currentVerse = block.Verse.Substring(ichVerse, ichVerseLim - ichVerse);
						ichVerse = ichVerseLim + 1;
					}
					double end = FindEndOfVerse(clipLength, start, prevOffset, verseOffset, block.Text);
					if (phraseLevel || currentVerse != prevVerse || currentVerse != nextVerse)
					{
						if (!phraseLevel && currentVerse == nextVerse)
						{
							accumClipTimeFromPrevBlocks += end - start;
							prevVerse = currentVerse;
							continue;
						}
						UpdateSubPhrase();
						end += accumClipTimeFromPrevBlocks;
						AppendLabel(startTime + start, startTime + end, currentVerse);
					}
					prevVerse = currentVerse;
					start = end;
					prevOffset = verseOffset;
				}
				startTime = endTime - accumClipTimeFromPrevBlocks;
			}

			private string GetHeadingBlockLabel()
			{
				var headingType = block.HeadingType.TrimEnd('1', '2', '3', '4');

				if (headingType == kChapter || headingType == kMainTitle)
					return headingType;

				if (!headingCounters.TryGetValue(headingType, out var headingCounter))
					headingCounter = 1;
				else
					headingCounter++;

				headingCounters[headingType] = headingCounter;
				return headingType + headingCounter;
			}

			private double FindEndOfVerse(double clipLength, double start, int prevOffset, int verseOffset, string text)
			{
				double percentage = (verseOffset - prevOffset) / (double) text.Length;
				return start + clipLength * percentage;
			}

			private void UpdateSubPhrase()
			{
				if (subPhrase >= 0 && prevVerse == currentVerse)
					subPhrase++;
				// if (!block.Heading && currentVerse == prevVerseEnd)
				//    return 1;
				else if (subPhrase > 0 && prevVerse != currentVerse)
					subPhrase = -1;
				if (subPhrase == -1 && currentVerse == nextVerse)
					subPhrase = 0;
			}

			private void WriteHeaderComments()
			{
				var bibleInfo = infoProvider.VersificationInfo;
				var bookCode = bibleInfo.GetBookCode(bibleInfo.GetBookNumber(bookName))
					.ToUpperInvariant();
				bldr.AppendLine($"\\id {bookCode}");
				bldr.AppendLine($"\\{kChapter} {chapterNumber}");
				bldr.AppendLine("\\level " + (phraseLevel ? "phrase" : "verse"));
				if (phraseLevel)
					bldr.AppendLine("\\separators " + infoProvider.BlockBreakCharacters);
			}

			private void AppendLabel(double start, double end, string label)
			{
				var timeRange = start.ToString("0.######", CultureInfo.InvariantCulture) + "\t" +
						end.ToString("0.######", CultureInfo.InvariantCulture) + "\t";
				bldr.AppendLine(timeRange + label + (subPhrase >= 0 ? ((char)('a' + subPhrase)).ToString() : Empty));
				accumClipTimeFromPrevBlocks = 0.0;
			}
		}
		#endregion //AudacityLabelFileBuilder class

		#endregion
	}
}
