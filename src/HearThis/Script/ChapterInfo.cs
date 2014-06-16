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
		public const string kChapterInfoFilename = "info.xml";

		private string _projectName;
		private string _bookName;
		private int _bookNumber;
		private IScriptProvider _scriptProvider;

		[XmlAttribute("Number")]
		public int ChapterNumber1Based;

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

			return chapterInfo;
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