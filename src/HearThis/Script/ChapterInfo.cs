// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
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
		private string _filePath;

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
		/// <li>When determining if recordings are possibly out-of-date, be sure to handle the
		/// case where a recording exists but is not reflected here.</li>
		/// <li>To enable XML serialization, this is not a SortedList, but it is expected to be
		/// ordered by LineNumber.</li>
		/// <li>This list is NOT filtered by current character.</li>
		/// </remarks>
		public List<ScriptLine> Recordings { get; set; }

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
			return Create(book, chapterNumber1Based, null);
		}

		internal static string GetFilePath(BookInfo book, int chapterNumber1Based) =>
			Path.Combine(book.GetChapterFolder(chapterNumber1Based), kChapterInfoFilename);

		/// <summary>
		/// This version allows creating a ChapterInfo from alternative file contents (when source is non-null)
		/// </summary>
		/// <param name="book">Info about the book containing this chapter; may be null when just loading file to get Recordings info</param>
		/// <param name="chapterNumber1Based">[0] == intro, [1] == chapter 1, etc.</param>
		/// <param name="source">If non-null, this will be used rather than the standard file as the source of chapter information.</param>
		public static ChapterInfo Create(BookInfo book, int chapterNumber1Based, string source)
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
							RobustFileAddOn.Move(filePath, pathOfCorruptedFile, true);
							chapterInfo.Recordings.RemoveRange(i, countOfRecordings - i);
							chapterInfo.Save(filePath);
							break;
						}
						prevLineNumber = block.Number;
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
			}
			chapterInfo._filePath = filePath;

			return chapterInfo;
		}

		internal void UpdateSource()
		{
			var scriptBlockCount = _scriptProvider.GetUnfilteredScriptBlockCount(_bookNumber, ChapterNumber1Based);
			Source = new List<ScriptLine>(scriptBlockCount);
			for (int i = 0; i < scriptBlockCount; i++)
			{
				Source.Add(_scriptProvider.GetUnfilteredBlock(_bookNumber, ChapterNumber1Based, i));
			}
		}

		/// <summary>
		/// Indicates whether it is REALLY empty, that is, has nothing even when no character filter applied.
		/// </summary>
		public bool IsEmpty => GetUnfilteredScriptBlockCount() == 0;

		public int GetUnfilteredScriptBlockCount()
		{
			return _scriptProvider.GetUnfilteredScriptBlockCount(_bookNumber, ChapterNumber1Based);
		}

		public int GetScriptBlockCount()
		{
			return _scriptProvider.GetScriptBlockCount(_bookNumber, ChapterNumber1Based);
		}

		/// <summary>
		/// "Recorded" actually means either recorded or skipped.
		/// It is filtered by current character.
		/// </summary>
		/// <returns></returns>
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
			// a settings change causes the block breaks to change. This can result in a recording whose block number is now
			// the same as that of a previously skipped block. This block will then be double-counted (once as a skip and once
			// as a recording). This isn't a very common scenario and it might be okay, but the performance gain here is modest,
			// and it's not clear whether it's worth this potential confusion. I think the ideal solution is to somehow fix up
			// the Recordings list when a settings change causes shifting of blocks, but that's more than I'm prepared to do at
			// this stage. In any case, until we have the pieces in place for the user to do the necessary clean-up (moving
			// recordings to align properly with new block divisions), it's not going to be possible to get things right.
			//// First check Recordings collection in memory - it's faster (though not guaranteed reliable since someone could delete a file).
			//if (Recordings.Count + skippedScriptLines == scriptLineCount)
			//    return 100;

			return (int)(100 * (ClipRepository.GetCountOfRecordingsInFolder(Path.GetDirectoryName(_filePath), _scriptProvider) + skippedScriptLines)/
				(float)(scriptLineCount));
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

		public bool HasRecordingsThatDoNotMatchCurrentScript => GetProblems().Any(p => p.Item2.NeedsAttention());

		public ProblemType WorstProblemInChapter => GetProblems().Select(p => p.Item2).DefaultIfEmpty(ProblemType.None).Max();

		internal int IndexOfFirstUnfilteredBlockWithProblem =>
			GetProblems().FirstOrDefault(p => p.Item2.NeedsAttention())?.Item1 ?? -1;

		internal IEnumerable<Tuple<int, ProblemType>> GetProblems()
		{
			var blockCount = _scriptProvider.GetUnfilteredScriptBlockCount(_bookNumber, ChapterNumber1Based);
			foreach (var recordedLine in Recordings)
			{
				if (recordedLine.Number > blockCount)
				{
					// This is a special case where the number of blocks in the script has
					// been reduced since the recording was done.
					yield return new Tuple<int, ProblemType>(blockCount - 1, ProblemType.ExtraRecordings);
					break;
				}

				var currentText = _scriptProvider.GetUnfilteredBlock(_bookNumber, ChapterNumber1Based, recordedLine.Number - 1).Text;

				if (recordedLine.Text != currentText)
					yield return new Tuple<int, ProblemType>(recordedLine.Number - 1, ProblemType.TextChange | ProblemType.Unresolved);

				if (recordedLine.OriginalText != null && recordedLine.OriginalText != currentText)
					yield return new Tuple<int, ProblemType>(recordedLine.Number - 1, ProblemType.TextChange | ProblemType.Ignored);
			}

			if (CalculatePercentageRecorded() > 100)
			{
				// Like the aforementioned special case, but these recordings pre-date the
				// feature where HearThis stored info in Recordings.
				yield return new Tuple<int, ProblemType>(blockCount - 1, ProblemType.ExtraRecordings);
			}
		}

		public bool HasRecordingInfoBeyondExtentOfCurrentScript =>
			Recordings.LastOrDefault()?.Number > _scriptProvider.GetUnfilteredScriptBlockCount(_bookNumber, ChapterNumber1Based) ||
			CalculatePercentageRecorded() > 100;

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

		private String Folder
		{
			get { return ClipRepository.GetChapterFolder(_projectName, _bookName, ChapterNumber1Based); }
		}

		public void RemoveRecordings()
		{
			Directory.Delete(Folder, true);
			Recordings = Recordings.Where(r => r.Skipped).ToList();
			Save();
		}

		protected override void Save()
		{
			Save(_filePath);
		}

		private void Save(string filePath)
		{
			if (!XmlSerializationHelper.SerializeToFile(filePath, this, out var error))
			{
				Logger.WriteError(error);
				// Though HearThis can "survive" without this file existing or having
				// correct information in it, this is a core HearThis data file. If we
				// can't save it for some reason, the problem probably isn't going to
				// magically go away.
				throw new Exception("Unable to save file: " + filePath, error);
			}
		}

		public string ToXmlString()
		{
			return XmlSerializationHelper.SerializeToString(this);
		}

		public override void OnScriptBlockRecorded(ScriptLine selectedScriptBlock)
		{
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
			Save();
		}

		public void OnClipDeleted(ScriptLine selectedScriptBlock) =>
			OnClipDeleted(selectedScriptBlock.Number);

		public void OnClipDeleted(int blockNumber)
		{
			var recording = Recordings.FirstOrDefault(r => r.Number == blockNumber);
			if (recording != null)
				Recordings.Remove(recording);
			Save();
		}

		public void RemoveRecordingInfoBeyondCurrentScriptExtent()
		{
			if (Recordings.RemoveAll(r => r.Number > GetUnfilteredScriptBlockCount()) > 0)
				Save();
		}
	}
}
