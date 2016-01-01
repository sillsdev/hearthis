using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using HearThis.Communication;
using HearThis.Publishing;

namespace HearThis.Script
{
	/// <summary>
	/// This class handles the overall merging of two repos, each represented as an IAndroidLink, given the current state
	/// of Scripture represented as an IScriptProvider.
	/// </summary>
	public class RepoMerger
	{
		private Project _project;
		private IAndroidLink _mine;
		private IAndroidLink _theirs;
		public RepoMerger(Project project, IAndroidLink mine, IAndroidLink theirs)
		{
			_project = project;
			_mine = mine;
			_theirs = theirs;
		}

		/// <summary>
		/// The master method to merge everything in the project.
		/// </summary>
		public void Merge()
		{
			foreach (var book in _project.Books)
			{
				for(int ichap = 0; ichap <= book.ChapterCount; ichap++)
				{
					if (book.GetChapter(ichap).GetScriptBlockCount() != 0)
					{
						MergeChapter(book.BookNumber, ichap);
					}
				}
			}
		}
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
		/// <param name="ibook"></param>
		/// <param name="ichap1based"></param>
		public virtual void MergeChapter(int ibook, int ichap1based)
		{
			var book = _project.Books[ibook];
			var ourInfo = GetXmlInfo(_mine, Path.Combine(GetOurChapterPath(_project.Name, book.Name, ichap1based), ChapterInfo.kChapterInfoFilename));
			ChapterInfo chapInfo;
			if (string.IsNullOrEmpty(ourInfo))
			{
				chapInfo = book.GetChapter(ichap1based);
			}
			else
			{
				chapInfo = ChapterInfo.Create(book, ichap1based, ourInfo);
			}
			chapInfo.UpdateSource();
			ourInfo = chapInfo.ToXmlString();
			var ourChapPath = GetOurChapterPath(_project.Name, book.Name, ichap1based);
			var ourFiles = GetFileInfo(_mine, ourChapPath);
			var ourInfoElt = XElement.Parse(ourInfo);
			var ourRecordings = ourInfoElt.Element("Recordings");
			foreach (var fileInfo in ourFiles)
			{
				if (fileInfo.IsDirectory)
					continue;
				if (Path.GetExtension(fileInfo.Name) != ".wav")
					continue;
				int blockOfFile;
				if (!int.TryParse(Path.GetFileNameWithoutExtension(fileInfo.Name), out blockOfFile))
					continue;
				EnsureScriptLinePresent(ourRecordings, blockOfFile, () => GetMissingScriptLine(blockOfFile));
			}
			var theirInfo = GetXmlInfo(_theirs, GetTheirChapterPath(_project.Name, book.Name, ichap1based) + "/" + ChapterInfo.kChapterInfoFilename);
			XElement theirInfoElt = null;
			IEnumerable<XElement> theirRecordings = new XElement[0];
			if (!string.IsNullOrEmpty(theirInfo))
			{
				theirInfoElt = XElement.Parse(theirInfo);
				theirRecordings = theirInfoElt.Element("Recordings").Elements("ScriptLine");
			}
			var sourceElt = ourInfoElt.Element("Source");
			var theirChapPath = GetTheirChapterPath(_project.Name, book.Name, ichap1based);
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
				var ourModifyTime = GetModifyTime(ourFiles, block);
				var theirModifyTime = GetModifyTime(theirFiles, block);
				var safeLine = theirLine; // using theirLine (a foreach varaible) in closure is not reliable.
				if (MergeBlock(ibook, ichap1based, block, source, ourRecording, theirRecording, ourModifyTime, theirModifyTime))
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
			_theirs.PutFile(_project.Name + "/" + book.Name + "/" + ichap1based + "/" + ChapterInfo.kChapterInfoFilename, bytes);
		}

		string GetXmlInfo(IAndroidLink link, string path)
		{
			byte[] infoBytes;
			link.TryGetData(path, out infoBytes);
			return Encoding.UTF8.GetString(infoBytes ?? new byte[0]);
		}

		DateTime GetModifyTime(List<FileDetails> details, int block)
		{
			var fileName = Path.ChangeExtension(block.ToString(), ".wav");
			foreach (var row in details)
			{
				if (row.Name == fileName)
					return row.Modified;
			}
			return DateTime.MinValue; // In case it matters, make the nonexistent file seem very old.
		}

		XElement GetScriptLine(XElement parent, int blockNo)
		{
			return parent.XPathSelectElement("ScriptLine[LineNumber='" + blockNo + "']");
		}

		void EnsureScriptLinePresent(XElement recordings, int blockNo, Func<XElement> getLineElement )
		{
			foreach (var scriptLine in recordings.Elements("ScriptLine"))
			{
				var thisBlockNoElt = scriptLine.Element("LineNumber");
				if (thisBlockNoElt != null) // should never be null...anything better we can do?
				{
					int thisLineNo;
					if (int.TryParse(thisBlockNoElt.Value, out thisLineNo))
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
		/// On the other hand, if someone is looking for recordings which need to be checked or redone, it could be very bad
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
			return projName + "/" + bookName + "/" + chapter.ToString();
		}

		class FileDetails
		{
			public string Name;
			public DateTime Modified;
			public bool IsDirectory;
		}

		List<FileDetails> GetFileInfo(IAndroidLink link, string source)
		{
			var result = new List<FileDetails>();
			string listing;
			if (!link.TryListFiles(source, out listing))
				return result;
			foreach (var line in listing.Split('\n'))
			{
				var parts = line.Split(';');
				if (parts.Length != 3)
					continue; // ignore info we don't expect
				DateTime modified;
				if (!DateTime.TryParse(parts[1], out modified))
					continue;
				result.Add(new FileDetails() {Name = parts[0], Modified = modified, IsDirectory = parts[2] == "d"});
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
		/// <param name="ibook"></param>
		/// <param name="ichap1based"></param>
		/// <param name="iblock"></param>
		/// <param name="source"></param>
		/// <param name="myRecording"></param>
		/// <param name="theirRecording"></param>
		/// <returns></returns>
		public virtual bool MergeBlock(int ibook, int ichap1based, int iblock, string source, string myRecording,
			string theirRecording, DateTime myModTime, DateTime theirModTime)
		{
			if (string.IsNullOrEmpty(theirRecording))
				return false; // they don't have one, nothing to do.
			if (string.IsNullOrEmpty(myRecording))
			{
				// We don't have one (but they do), copy theirs.
				CopyTheirs(ibook, ichap1based, iblock);
				return true;
			}
			if (myRecording == source)
			{
				if (theirRecording == source && theirModTime > myModTime)
				{
					// both are current; theirs is newer; copy theirs
					CopyTheirs(ibook, ichap1based, iblock);
					return true;
				}
				return false;
			}
			if (theirRecording == source || theirModTime > myModTime)
			{
				// Theirs is for the right text...ours is not. Or, neither is correct, and theirs is newer. Copy theirs.
				CopyTheirs(ibook, ichap1based, iblock);
				return true;
			}
			return false;
		}

		private void CopyTheirs(int ibook, int ichap1based, int iblock)
		{
			var book = _project.VersificationInfo.GetBookName(ibook);
			var recordingName = iblock.ToString() + ".wav";
			var destPath = Path.Combine(Program.GetApplicationDataFolder(_project.Name), book, ichap1based.ToString(),recordingName);
			var sourcePath = _project.Name + "/" + book + "/" + ichap1based + "/" + recordingName;
			_theirs.GetFile(sourcePath, destPath);
		}
	}
}
