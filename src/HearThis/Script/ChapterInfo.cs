// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2011' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
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
using System.Xml.Serialization;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Publishing;
using SIL.IO;
using SIL.Reporting;
using SIL.Xml;
using static System.Int32;

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
	public class ChapterInfo : ChapterRecordingInfoBase
	{
		public const string kChapterInfoFilename = "info.xml";

		private string _projectName;
		private string _bookName;
		private int _bookNumber;
		private IScriptProvider _scriptProvider;
		private int _realScriptBlockCount;
		private string _filePath;
		private List<ExtraRecordingInfo> _extraRecordings;

		[XmlAttribute("Number")]
		public int ChapterNumber1Based;

		/// <summary>
		/// Information about the script and the recordings at the time the recordings were
		/// made.
		/// </summary>
		/// <remarks>
		/// Note the following:
		/// <li>Any recording made in HearThis before this was implemented will not be reflected
		/// in this list, so it should NOT be used to determine the actual number of recordings.</li>
		/// <li>When determining if clips are possibly out-of-date, be sure to handle the
		/// case where a clip exists but is not reflected here.</li>
		/// <li>To enable XML serialization, this is not a SortedList, but it is expected to be
		/// ordered by LineNumber. In production code, this collection should not be modified
		/// directly by other classes.</li>
		/// <li>This list is NOT filtered by current character.</li>
		/// </remarks>
		public List<ScriptLine> Recordings { get; set; }

		/// <summary>
		/// Information about the recorded clips that were deleted (in case the deletion is undone).
		/// </summary>
		public List<ScriptLine> DeletedRecordings { get; set; }

		public override IReadOnlyList<ScriptLine> RecordingInfo => Recordings;

		/// <summary>
		/// The lines we want to record for this chapter. May be out of date compared to
		/// current ScriptProvider; this is primarily output data for HearThisAndroid.
		/// May also be missing if file was created by an older version of HearThis.
		/// </summary>
		public List<ScriptLine> Source { get; set; }

		/// <summary>
		/// Use this instead of the default constructor to instantiate an instance of this class
		/// </summary>
		/// <param name="book">Info about the book containing this chapter</param>
		/// <param name="chapterNumber1Based">[0] == intro, [1] == chapter 1, etc.</param>
		public static ChapterInfo Create(BookInfo book, int chapterNumber1Based)
		{
			return Create(book, chapterNumber1Based, null, false);
		}

		internal static string GetFilePath(BookInfo book, int chapterNumber1Based) =>
			Path.Combine(book.GetChapterFolder(chapterNumber1Based), kChapterInfoFilename);

		/// <summary>
		/// This version allows creating a ChapterInfo from alternative file contents (when source is non-null)
		/// </summary>
		/// <param name="book">Info about the book containing this chapter; may be null when just loading file to get Recordings info</param>
		/// <param name="chapterNumber1Based">[0] == intro, [1] == chapter 1, etc.</param>
		/// <param name="source">If non-null, this will be used rather than the standard file as the source of chapter information.</param>
		/// <param name="doNotCleanUpRecordingsForMissingClips">Flag indicating that the cleanup step to remove recordings when wav files
		/// are not present should be skipped. (Needed during Android merge.)</param>
		public static ChapterInfo Create(BookInfo book, int chapterNumber1Based, string source, bool doNotCleanUpRecordingsForMissingClips)
		{
			ChapterInfo chapterInfo = null;
			string filePath = null;
			if (book != null)
				filePath = GetFilePath(book, chapterNumber1Based);
			if (File.Exists(filePath) || !string.IsNullOrEmpty(source))
			{
				try
				{
					if (string.IsNullOrEmpty(source))
						chapterInfo = XmlSerializationHelper.DeserializeFromFile<ChapterInfo>(filePath); // normal
					else
						chapterInfo = XmlSerializationHelper.DeserializeFromString<ChapterInfo>(source); // tests
					int prevLineNumber = 0;
					int countOfRecordings = chapterInfo.Recordings.Count;
					for (int i = 0; i < countOfRecordings; i++)
					{
						ScriptLine block = chapterInfo.Recordings[i];
						if (block.Number <= prevLineNumber)
						{
							var pathOfCorruptedFile = Path.ChangeExtension(filePath, "corrupt");
							Logger.WriteEvent("Backing up apparently corrupt chapter info file to " + pathOfCorruptedFile);
							RobustFile.Move(filePath, pathOfCorruptedFile, true);
							chapterInfo.Recordings.RemoveRange(i, countOfRecordings - i);
							chapterInfo.Save(filePath);
							break;
						}
						prevLineNumber = block.Number;

						if (!doNotCleanUpRecordingsForMissingClips &&
							!File.Exists(ClipRepository.GetPathToLineRecording(book.ProjectName, book.Name, chapterNumber1Based, block.Number - 1)) &&
							!ClipRepository.SkipFileExists(book.ProjectName, book.Name, chapterNumber1Based, block.Number - 1))
						{
							//Debug.Fail("This should only be possible if user has mucked with the files.");
							// Info is not "corrupt" but it contains recording info for a clip that no longer exists.
							chapterInfo.Recordings.RemoveAt(i);
							i--;
							countOfRecordings--;

							if (ClipRepository.HasBackupFile(book.ProjectName, book.Name, chapterNumber1Based, block.Number - 1) &&
								chapterInfo.DeletedRecordings == null ||
								!chapterInfo.DeletedRecordings.Any(r => r.Number == block.Number))
							{
								chapterInfo.NoteDeletedRecording(block);

								chapterInfo.Save(filePath);
							}
						}
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

			if (book != null)
			{
				chapterInfo._projectName = book.ProjectName;
				chapterInfo._bookName = book.Name;
				chapterInfo._bookNumber = book.BookNumber;
				chapterInfo._scriptProvider = book.ScriptProvider;
				chapterInfo.UpdateScriptBlockCount();
			}
			chapterInfo._filePath = filePath;

			return chapterInfo;
		}

		private void NoteDeletedRecording(ScriptLine block)
		{
			if (DeletedRecordings == null)
				DeletedRecordings = new List<ScriptLine>(1);
			DeletedRecordings.Add(block);
		}

		private void UpdateScriptBlockCount() =>
			_realScriptBlockCount = _scriptProvider.GetUnfilteredScriptBlockCount(_bookNumber, ChapterNumber1Based);

		internal void UpdateSource()
		{
			UpdateScriptBlockCount();
			Source = new List<ScriptLine>(_realScriptBlockCount);
			for (int i = 0; i < _realScriptBlockCount; i++)
				Source.Add(_scriptProvider.GetUnfilteredBlock(_bookNumber, ChapterNumber1Based, i));
		}

		/// <summary>
		/// Indicates whether it is REALLY empty, that is, has nothing even when no character filter applied.
		/// </summary>
		public bool IsEmpty => _realScriptBlockCount == 0;

		public int UnfilteredScriptBlockCount => _realScriptBlockCount;

		public int GetScriptBlockCount()
		{
			return _scriptProvider.GetScriptBlockCount(_bookNumber, ChapterNumber1Based);
		}

		/// <summary>
		/// "Recorded" actually means either recorded or skipped (unless nothing has been recorded
		/// or everything is skipped - we don't want to report partially recorded just because a
		/// block or two is skipped).
		/// It is filtered by current character.
		/// </summary>
		/// <returns>A percentage between 0 and 100%</returns>
		public int CalculatePercentageRecorded()
		{
			int skippedScriptLines = 0;
			int scriptLineCount = GetScriptBlockCount();
			if (scriptLineCount == 0)
				return 0;

			for (int i = 0; i < scriptLineCount; i++)
			{
				if (_scriptProvider.GetBlock(_bookNumber, ChapterNumber1Based, i).Skipped)
					skippedScriptLines++;
			}
			// ENHANCE: This was causing too many problems because the Recordings list isn't being maintained the case where
			// a settings change causes the block breaks to change. This can result in a clip whose block number is now
			// the same as that of a previously skipped block. This block will then be double-counted (once as a skip and once
			// as a recording). This isn't a very common scenario and it might be okay, but the performance gain here is modest,
			// and it's not clear whether it's worth this potential confusion. I think the ideal solution is to somehow fix up
			// the Recordings list when a settings change causes shifting of blocks, but that's more than I'm prepared to do at
			// this stage. In any case, until we have the pieces in place for the user to do the necessary clean-up (moving
			// recordings to align properly with new block divisions), it's not going to be possible to get things right.
			//// First check Recordings collection in memory - it's faster (though not guaranteed reliable since someone could delete a file).
			//if (Recordings.Count + skippedScriptLines == scriptLineCount)
			//    return 100;

			var cRecordings = ClipRepository.GetCountOfRecordingsInFolder(Path.GetDirectoryName(_filePath), _scriptProvider);
			if (cRecordings == 0 && skippedScriptLines < scriptLineCount)
				skippedScriptLines = 0;

			return (int)(100 * (cRecordings + skippedScriptLines) / (float)scriptLineCount);
		}

		public bool RecordingsFinished
		{
			get
			{
				int scriptLineCount = GetScriptBlockCount();
				// First check Recordings collection in memory - it's faster (though not guaranteed reliable since someone could delete a file).
				if (Recordings.Count == scriptLineCount)
					return true;
				// Older versions of HT didn't maintain in-memory recording info, so see if we have the right number of recordings.
				// ENHANCE: for maximum reliability, we should check for the existence of the exact file names we expect.
				return CalculatePercentageRecorded() >= 100;
			}
		}

		public bool HasUnresolvedProblem => GetProblems().Any(p => p.Type.NeedsAttention());

		/// <summary>
		/// Would have wanted to do this using a Tuple with named items, but it creates
		/// a ValueTuple that can't be used inside FirstOrDefault.
		/// </summary>
		private class Problem
		{
			public int LineNumber { get; }
			public ProblemType Type { get; }

			public Problem(int lineNumber, ProblemType type)
			{
				LineNumber = lineNumber;
				Type = type;
			}
		}

		public ProblemType WorstProblemInChapter => GetProblems().Select(p => p.Type).DefaultIfEmpty(ProblemType.None).Max();

		// 0-based
		internal int IndexOfFirstUnfilteredBlockWithProblem =>
			GetIndexOfNextUnfilteredBlockWithProblem(-1);

		// 0-based
		internal int GetIndexOfNextUnfilteredBlockWithProblem(int currentIndex) =>
			GetProblems(currentIndex + 1).FirstOrDefault(p => p.Type.NeedsAttention())?.LineNumber ?? -1;

		private IEnumerable<Problem> GetProblems(int minIndex = 0)
		{
			int prevBlockNumber = minIndex - 1;
			foreach (var recordedLine in Recordings.Where(r=> r.Number <= _realScriptBlockCount))
			{
				var blockNumber = recordedLine.Number - 1;

				if (blockNumber < minIndex)
					continue;

				var currentText = _scriptProvider.GetUnfilteredBlock(_bookNumber, ChapterNumber1Based, blockNumber).Text;

				// In rare instances, the text may be subsequently reverted back to the way it
				// was when the clip was originally recorded; this should not be treated as a problem.
				if (recordedLine.Text != currentText && recordedLine.OriginalText != currentText)
					yield return new Problem(blockNumber, ProblemType.TextChange | ProblemType.Unresolved);

				if (recordedLine.OriginalText != null && recordedLine.OriginalText != currentText)
					yield return new Problem(blockNumber, ProblemType.TextChange | ProblemType.Resolved);

				if (recordedLine.Skipped && ClipRepository.HasClip(_projectName, _bookName, ChapterNumber1Based, blockNumber))
					yield return new Problem(blockNumber, ProblemType.ClipForSkippedBlock);
				else if (prevBlockNumber < blockNumber - 1)
				{
					for (int i = prevBlockNumber + 1; i < blockNumber; i++)
					{
						if (_scriptProvider.GetUnfilteredBlock(_bookNumber, ChapterNumber1Based, i).Skipped &&
						    ClipRepository.HasClipUnfiltered(_projectName, _bookName, ChapterNumber1Based, i))
							yield return new Problem(i, ProblemType.ClipForSkippedBlock);
					}
				}

				prevBlockNumber = blockNumber;
			}

			for (int i = 0; i < GetExtraClips().Count; i++)
			{
				if (i + _realScriptBlockCount >= minIndex)
					yield return new Problem(i + _realScriptBlockCount, ProblemType.ExtraRecordings);
			}
		}

		private IEnumerable<string> ExcessClipFiles =>
			ClipRepository.GetAllExcessClipFiles(_realScriptBlockCount, _projectName,
				_bookName, ChapterNumber1Based);

		public int CalculatePercentageTranslated()
		{
			 return (_scriptProvider.GetTranslatedVerseCount(_bookNumber, ChapterNumber1Based));
		}

		public int CalculateUnfilteredPercentageTranslated()
		{
			return (_scriptProvider.GetUnfilteredTranslatedVerseCount(_bookNumber, ChapterNumber1Based));
		}

		public void MakeDummyRecordings()
		{
			using (TempFile sound = new TempFile())
			{
				byte[] buffer = new byte[Resources.think.Length];
				Resources.think.Read(buffer, 0, buffer.Length);
				RobustFile.WriteAllBytes(sound.Path, buffer);
				for (int line = 0; line < GetScriptBlockCount(); line++)
				{
					var path = ClipRepository.GetPathToLineRecording(_projectName, _bookName, ChapterNumber1Based, line, _scriptProvider);

					if (!File.Exists(path))
					{
						RobustFile.Copy(sound.Path, path);
						OnScriptBlockRecorded(_scriptProvider.GetBlock(_bookNumber, ChapterNumber1Based, line));
					}
				}
			}
		}

		private string Folder =>
			ClipRepository.GetChapterFolder(_projectName, _bookName, ChapterNumber1Based);

		public void RemoveRecordings()
		{
			Directory.Delete(Folder, true);
			Recordings = Recordings.Where(r => r.Skipped).ToList();
			Save();
		}

		public override void Save(bool preserveModifiedTime = false) => Save(_filePath, preserveModifiedTime);

		private void Save(string filePath, bool preserveModifiedTime = false)
		{
			if (_scriptProvider != null)
			{
				Recordings.RemoveAll(r => r.Number > _realScriptBlockCount &&
					!ExcessClipFiles.Select(Path.GetFileName)
					.Contains($"{r.Number - 1}.wav", StringComparer.InvariantCultureIgnoreCase));
			}

			var attempt = 0;
			var finfo = new FileInfo(filePath);
			var modified = finfo.LastWriteTimeUtc;
			Exception error = null;
			while (attempt < 2)
			{
				XmlSerializationHelper.SerializeToFileWithWriteThrough(filePath, this, out error);
				if (error == null)
					break;
				Logger.WriteError(error);

				if (attempt++ == 0 && finfo.IsReadOnly)
				{
					try
					{
						finfo.IsReadOnly = false;
					}
					catch (Exception e)
					{
						Logger.WriteError(e);
						attempt = MaxValue;
					}
				}
			}

			// Though HearThis can "survive" without this file existing or having
			// correct information in it, this is a core HearThis data file. If we
			// can't save it for some reason, the problem probably isn't going to
			// magically go away.
			if (attempt > 2)
				throw new Exception($"Unable to save {GetType().Name} file: " + filePath, error);

			if (preserveModifiedTime)
			{
				finfo.LastWriteTimeUtc = modified;
				finfo.Attributes |= FileAttributes.Archive;
			}
		}

		public string ToXmlString()
		{
			return XmlSerializationHelper.SerializeToString(this);
		}

		public override void OnScriptBlockRecorded(ScriptLine selectedScriptBlock)
		{
			var filename = ClipRepository.GetPathToLineRecording(_projectName, _bookName,
				ChapterNumber1Based, selectedScriptBlock.Number - 1);
			if (ClipRepository.IsInvalidClipFile(filename))
				return;
			selectedScriptBlock.Skipped = false;
			Debug.Assert(selectedScriptBlock.Number > 0);
			int iInsert = 0;
			for (int i = 0; i < Recordings.Count; i++)
			{
				var recording = Recordings[i];
				if (recording.Number == selectedScriptBlock.Number)
				{
					Recordings[i] = selectedScriptBlock;
					iInsert = -1;
					break;
				}
				if (recording.Number > selectedScriptBlock.Number)
				{
					break;
				}
				iInsert++;
			}
			if (iInsert >= 0)
				Recordings.Insert(iInsert, selectedScriptBlock);
			if (DeletedRecordings != null)
			{
				// There should never be more than one.
				DeletedRecordings.RemoveAll(s => s.Number == selectedScriptBlock.Number);
			}
			Save();
		}

		public void OnClipDeleted(ScriptLine selectedScriptBlock)
		{
			if (selectedScriptBlock != null)
				OnClipDeleted(selectedScriptBlock.Number, true);
		}

		public void OnExtraClipDeleted(int blockNumber) =>
			OnClipDeleted(blockNumber, false);

		private void OnClipDeleted(int blockNumber, bool saveTextAsOriginal)
		{
			var recording = Recordings.FirstOrDefault(r => r.Number == blockNumber);
			if (recording != null)
			{
				Recordings.Remove(recording);
				if (saveTextAsOriginal && recording.OriginalText == null)
					recording = recording.GetAsDeleted();
				NoteDeletedRecording(recording);
				Save();
			}
		}

		public void OnClipUndeleted(ScriptLine selectedScriptBlock)
		{
			if (selectedScriptBlock != null)
				OnClipUndeleted(selectedScriptBlock.Number);
		}

		private void OnClipUndeleted(int blockNumber)
		{
			var recording = DeletedRecordings?.SingleOrDefault(r => r.Number == blockNumber);
			if (recording != null)
			{
				DeletedRecordings.Remove(recording);
				if (!DeletedRecordings.Any())
					DeletedRecordings = null;
				if (recording.Text == null && recording.OriginalText != null)
				{
					recording.Text = recording.OriginalText;
					recording.OriginalText = null;
				}
				OnScriptBlockRecorded(recording);
			}
		}

		public IReadOnlyList<ExtraRecordingInfo> GetExtraClips(bool rescan = false)
		{
			if (_extraRecordings == null || rescan)
			{
				_extraRecordings = ExcessClipFiles.Select(file => new ExtraRecordingInfo(file,
					Recordings.FirstOrDefault(r => Parse(Path.GetFileNameWithoutExtension(file)) == r.Number - 1))).ToList();
			}
			return _extraRecordings;
		}

		public void RemoveRecordingInfo(ScriptLine line)
		{
			Recordings.Remove(line);
			Save();
		}
	}
}
