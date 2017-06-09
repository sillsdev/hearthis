using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;

namespace HearThis.Script
{

	/// <summary>
	/// Used to make HearThisPack files, basically a zip of the folders containing the wav files and info.xml files.
	/// Can optionally filter which wav files are included based on which actor recorded them.
	/// </summary>
	public class HearThisPackMaker
	{
		public const string HearThisPackExtension = ".hearthispack";
		private string _rootFolder;
		private string _basePath; // part of _rootFolder's path not to include in zip structure
		public HearThisPackMaker(string rootFolder)
		{
			_rootFolder = rootFolder;
			_basePath = Path.GetDirectoryName(_rootFolder);
		}

		public void Pack(string destPath)
		{
			using (var zip = new ZipFile(Encoding.UTF8))
			{
				ZipUp(zip, _rootFolder);
				zip.Save(destPath);
			}
		}

		public string Actor { get; set; }

		private void ZipUp(ZipFile zip, string folder)
		{
			foreach (var dir in Directory.EnumerateDirectories(folder))
				ZipUp(zip, dir);
			// strip off everything before the project name (including directory sep)
			var directoryPathInArchive = folder.Substring(_basePath.Length + 1);
			var infoPath = Path.Combine(folder, ChapterInfo.kChapterInfoFilename);
			if (File.Exists(infoPath))
				zip.AddFile(infoPath, directoryPathInArchive);
			if (string.IsNullOrEmpty(Actor))
			{
				// We want everything...just grab every wav file in the folder.
				foreach (var file in Directory.EnumerateFiles(folder, "*.wav"))
				{
					zip.AddFile(file, directoryPathInArchive);
				}
			}
			else // restrict to actor
			{
				if (!File.Exists(infoPath))
					return; // need info file for restricted export; ignore anything in folder without one
				// We don't care about book and chapter number, just the Recordings information.
				var chapInfo = ChapterInfo.Create(null, 1, File.ReadAllText(infoPath, Encoding.UTF8));
				foreach (var file in Directory.EnumerateFiles(folder, "*.wav"))
				{
					int blockNo;
					if (!int.TryParse(Path.GetFileNameWithoutExtension(file), out blockNo))
						continue; // not a proper HearThis wav file.
					blockNo++;
					if (DoesBlockHaveActor(chapInfo, blockNo))
						zip.AddFile(file, directoryPathInArchive);
				}
			}
		}

		bool DoesBlockHaveActor(ChapterInfo chapInfo, int blockNo)
		{
			// We can't use an index here because there may be missing recordings.
			foreach (var recording in chapInfo.Recordings)
			{
				if (recording.Number == blockNo)
					return recording.Actor == Actor;
			}
			return false; // no info about this recording, skip it.
		}
	}
}
