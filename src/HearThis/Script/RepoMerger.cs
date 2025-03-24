// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2015-2025, SIL Global.
// <copyright from='2015' to='2025' company='SIL Global'>
//		Copyright (c) 2015-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using HearThis.Communication;
using SIL.Progress;
using SIL.Reporting;
using SIL.Xml;

namespace HearThis.Script
{
	/// <summary>
	/// This class handles the overall merging of two repos, each represented as an IAndroidLink, given the current state
	/// of Scripture represented as an IScriptProvider.
	/// </summary>
	public class RepoMerger
	{
		private readonly Project _project;
		private readonly IAndroidLink _mine;
		private readonly IAndroidLink _theirs;

		public RepoMerger(Project project, IAndroidLink mine, IAndroidLink theirs)
		{
			_project = project;
			_mine = mine;
			_theirs = theirs;
		}

		/// <summary>
		/// The master method to merge everything in the project.
		/// </summary>
		public void Merge(IReadOnlyList<string> defaultSkippedStyles, IProgress progress)
		{
			foreach (var book in _project.Books)
			{
				for(int iChap = 0; iChap <= book.ChapterCount; iChap++)
				{
					if (progress.CancelRequested)
						return;
					if (book.GetChapter(iChap).UnfilteredScriptBlockCount != 0)
					{
						progress.WriteMessage("syncing {0} chapter {1}", book.Name, iChap.ToString());
						// Feels like the LogBox should handle this itself, but currently it doesn't.
						// ENHANCE: Should be running this task in a background thread.
						if (progress is Control progressControl)
							progressControl.Update();

						MergeChapter(book.BookNumber, iChap);
					}
				}
			}

			MergeSkippedData(defaultSkippedStyles);
		}

		// Enhance: when we implement skipping on Android, we need to write the merged file to _theirs also.
		private void MergeSkippedData(IReadOnlyList<string> defaultSkippedStyles)
		{
			string skippedLinePath = Path.Combine(_project.Name, ScriptProviderBase.kSkippedLineInfoFilename);
			if (!_theirs.TryGetData(skippedLinePath, out var theirSkipData))
				return; // nothing to merge.
			if (!_mine.TryGetData(skippedLinePath, out var ourSkipData))
			{
				// just copy theirs.
				_mine.PutFile(skippedLinePath, theirSkipData);
				return;
			}
			var theirSkipLines = SkippedScriptLines.Create(theirSkipData, defaultSkippedStyles);
			var ourSkipLines = SkippedScriptLines.Create(ourSkipData, defaultSkippedStyles);
			foreach (var skipLine in theirSkipLines.SkippedLinesList)
			{
				var index = ourSkipLines.SkippedLinesList.FindIndex(sli => sli.IsSameLine(skipLine));
				if (index < 0)
					ourSkipLines.SkippedLinesList.Add(skipLine);
				else
					ourSkipLines.SkippedLinesList[index] = skipLine;
			}

			var output = XmlSerializationHelper.SerializeToByteArray(ourSkipLines);
			_mine.PutFile(skippedLinePath, output);
		}

		/// <summary>
		/// Set this to false to prevent writing info.xml files to 'theirs'.
		/// This is pointless when merging from a HearThisPack rather than sharing with another device.
		/// </summary>
		public bool SendData = true;

		/// <summary>
		/// This method does the main merge task. It has the following responsibilities:
		/// - if 'mine' lacks info.xml, generate one.
		/// - if 'mine' has the wrong 'Source' data, update it
		/// - if 'mine' has .wav files with no corresponding data in 'Recordings' in info.xml, add info reflecting their presence
		/// - if 'theirs' lacks info.xml, copy updated info.xml there and stop.
		/// - retrieve 'theirs' info.xml
		/// - get directory listings for both chapter directories
		/// - merge recordings appropriately.
		/// </summary>
		public virtual void MergeChapter(int iBook, int iChap1Based)
		{
			var book = _project.Books.FirstOrDefault(b => b.BookNumber == iBook);
			if (book == null)
			{
				Logger.WriteEvent($"Attempted to merge a chapter for a non-existent book: {iBook}");
				return;
			}
			var ourInfo = GetXmlInfo(_mine, Path.Combine(GetOurChapterPath(_project.Name, book.Name, iChap1Based), ChapterInfo.kChapterInfoFilename));
			var chapInfo = string.IsNullOrEmpty(ourInfo) ? book.GetChapter(iChap1Based) :
				ChapterInfo.Create(book, iChap1Based, ourInfo, true);
			chapInfo.UpdateSource();
			ourInfo = chapInfo.ToXmlString();
			var ourChapPath = GetOurChapterPath(_project.Name, book.Name, iChap1Based);
			var ourFiles = GetFileInfo(_mine, ourChapPath);
			var ourInfoElt = XElement.Parse(ourInfo);
			var ourRecordings = ourInfoElt.Element("Recordings");
			foreach (var fileInfo in ourFiles)
			{
				if (fileInfo.IsDirectory)
					continue;
				var extension = Path.GetExtension(fileInfo.Name).ToLowerInvariant();
				if (extension != ".wav" && extension != ".mp4")
					continue;
				if (!int.TryParse(Path.GetFileNameWithoutExtension(fileInfo.Name), out var blockOfFile))
					continue;
				EnsureScriptLinePresent(ourRecordings, blockOfFile + 1, () => GetMissingScriptLine(blockOfFile));
			}
			var theirInfo = GetXmlInfo(_theirs, GetTheirChapterPath(_project.Name, book.Name, iChap1Based) + "/" + ChapterInfo.kChapterInfoFilename);
			IEnumerable<XElement> theirRecordings = new XElement[0];
			if (!string.IsNullOrEmpty(theirInfo))
			{
				var theirInfoElt = XElement.Parse(theirInfo);
				theirRecordings = theirInfoElt.Element("Recordings").Elements("ScriptLine");
			}
			var sourceElt = ourInfoElt.Element("Source");
			var theirChapPath = GetTheirChapterPath(_project.Name, book.Name, iChap1Based);
			var theirFiles = GetFileInfo(_theirs, theirChapPath);
			foreach (var theirLine in theirRecordings)
			{
				var block = int.Parse(theirLine.Element("LineNumber").Value);
				var ourLine = GetScriptLine(ourRecordings, block);
				var theirRecording = theirLine.Element("Text").Value;
				string ourRecording = "";
				if (ourLine != null)
					ourRecording = ourLine.Element("Text").Value;
				var sourceLine = GetScriptLine(sourceElt, block);
				if (sourceLine == null)
					continue; // ignore any recording they have for a line that does not exist.
				var source = sourceLine.Element("Text").Value;
				var ourModifyTime = GetModifyTime(ourFiles, block, out var ext);
				var theirModifyTime = GetModifyTime(theirFiles, block, out ext);
				var safeLine = theirLine; // using theirLine (a foreach variable) in closure is not reliable.
				if (MergeBlock(iBook, iChap1Based, block, source, ourRecording, theirRecording, ourModifyTime, theirModifyTime, ext))
				{
					if (ourLine != null)
						ourLine.ReplaceWith(theirLine);
					else
						EnsureScriptLinePresent(ourRecordings, block, () => safeLine);
				}
			}
			ourInfo = ourInfoElt.ToString();
			var bytes = Encoding.UTF8.GetBytes(ourInfo);
			_mine.PutFile(Path.Combine(ourChapPath, ChapterInfo.kChapterInfoFilename), bytes);
			if (SendData)
				_theirs.PutFile(_project.Name + "/" + book.Name + "/" + iChap1Based + "/" + ChapterInfo.kChapterInfoFilename, bytes);
		}

		string GetXmlInfo(IAndroidLink link, string path)
		{
			link.TryGetData(path, out var infoBytes);
			return Encoding.UTF8.GetString(infoBytes ?? new byte[0]);
		}

		string GetBlockWavFileName(int block, string ext)
		{
			return Path.ChangeExtension((block - 1).ToString(), ext);
		}

		DateTime GetModifyTime(List<FileDetails> details, int block, out string ext)
		{
			var wavFileName = GetBlockWavFileName(block, ".wav");
			var mp4FileName = GetBlockWavFileName(block, ".mp4");
			var result = DateTime.MinValue; // Any file we find will be more recent (and if no file, will be older than anything else)
			ext = "";
			foreach (var row in details)
			{
				if (row.Name == wavFileName  && row.Modified > result)
				{
					ext = ".wav";
					result = row.Modified;
				}
				if (row.Name == mp4FileName && row.Modified >  result)
				{
					ext = ".mp4";
					result = row.Modified;
				}
			}
			return result;
		}

		XElement GetScriptLine(XElement parent, int blockNo)
		{
			return parent.XPathSelectElement("ScriptLine[LineNumber='" + blockNo + "']");
		}

		static void EnsureScriptLinePresent(XElement recordings, int blockNo, Func<XElement> getLineElement )
		{
			foreach (var scriptLine in recordings.Elements("ScriptLine"))
			{
				var thisBlockNoElt = scriptLine.Element("LineNumber");
				if (thisBlockNoElt != null) // should never be null...anything better we can do?
				{
					if (int.TryParse(thisBlockNoElt.Value, out var thisLineNo))
					{
						if (thisLineNo == blockNo)
							return; // required info is already present
						if (thisLineNo > blockNo)
						{
							// insert missing info here
							thisBlockNoElt.AddBeforeSelf(GetMissingScriptLine(blockNo));
						}
					}
				}
			}
			// no match, add at end
			recordings.Add(getLineElement());
		}

		/// <summary>
		/// Generate a script line for a recording which exists but doesn't have an entry in the Recordings element of the info.xml.
		/// It's not obvious what text it is best to give this ScriptLine. If it is going to end up being used without revision,
		/// it would probably be best to give it the current text for that block, since in many cases that is what was recorded
		/// and it will provide accurate information for clients such as readers which display the text being spoken.
		/// On the other hand, if someone is looking for clips which need to be checked or redone, it could be very bad
		/// to explicitly indicate that this recording was made from the correct text, when we don't know for sure that it
		/// was not made from an earlier revision. It seems safest to set the text to something that indicates we don't know
		/// what text was recorded.
		/// </summary>
		/// <param name="lineNo"></param>
		/// <returns></returns>
		private static XElement GetMissingScriptLine(int lineNo)
		{
			return new XElement("ScriptLine",
				new XElement("LineNumber", lineNo.ToString()),
				new XElement("Text", "---"));
		}

		string GetOurChapterPath(string projName, string bookName, int chapter)
		{
			return Path.Combine(Program.GetApplicationDataFolder(projName), bookName, chapter.ToString());
		}

		string GetTheirChapterPath(string projName, string bookName, int chapter)
		{
			return projName + "/" + bookName + "/" + chapter;
		}

		class FileDetails
		{
			public string Name;
			public DateTime Modified;
			public bool IsDirectory;
		}

		static List<FileDetails> GetFileInfo(IAndroidLink link, string source)
		{
			var result = new List<FileDetails>();
			if (!link.TryListFiles(source, out var listing))
				return result;
			foreach (var line in listing.Split('\n'))
			{
				var parts = line.Split(';');
				if (parts.Length != 3)
					continue; // ignore info we don't expect
				if (!DateTime.TryParse(parts[1], out var modified))
					continue;
				result.Add(new FileDetails {Name = parts[0], Modified = modified, IsDirectory = parts[2] == "d"});
			}
			return result;
		}

		/// <summary>
		/// Merge (if necessary) the recording (if any) that we already have for the specified block with the one (if any)
		/// that they have. Return true if we chose their recording.
		/// Currently the algorithm is rather simple:
		/// - if either recording is missing, indicated by either an empty string for xRecording, choose the other.
		/// - if their recording exists and mine does not, or if theirs is preferable, copy theirs to mine.
		/// </summary>
		/// <returns>A value indicating whether theirs was copied.</returns>
		public virtual bool MergeBlock(int iBook, int iChap1Based, int iBlock, string source, string myRecording,
			string theirRecording, DateTime myModTime, DateTime theirModTime, string ext)
		{
			if (string.IsNullOrEmpty(theirRecording))
				return false; // they don't have one, nothing to do.

			if (string.IsNullOrEmpty(myRecording))
			{
				// We don't have one (but they do), copy theirs.
				return CopyTheirs(iBook, iChap1Based, iBlock, ext);
			}
			if (myRecording == source)
			{
				// If both are current; theirs is newer; copy theirs
				return theirRecording == source && theirModTime > myModTime &&
					CopyTheirs(iBook, iChap1Based, iBlock, ext);
			}
			if (theirRecording == source || theirModTime > myModTime)
			{
				// Theirs is for the right text...ours is not. Or, neither is correct, and theirs is newer. Copy theirs.
				return CopyTheirs(iBook, iChap1Based, iBlock, ext);
			}
			return false;
		}

		private bool CopyTheirs(int iBook, int iChap1Based, int iBlock, string ext)
		{
			var book = _project.VersificationInfo.GetBookName(iBook);
			var recordingName = GetBlockWavFileName(iBlock, ext);
			var destPath = Path.Combine(Program.GetApplicationDataFolder(_project.Name), book, iChap1Based.ToString(),recordingName);
			var sourcePath = _project.Name + "/" + book + "/" + iChap1Based + "/" + recordingName;
			if (!_theirs.GetFile(sourcePath, destPath))
				return false;
			// Get rid of any competing recording with the other extension.
			_mine.DeleteFile(Path.ChangeExtension(destPath, ext == ".wav" ? ".mp4" : ".wav"));
			return true;
		}
	}
}
