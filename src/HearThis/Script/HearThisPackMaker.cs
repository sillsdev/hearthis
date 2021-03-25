using System.IO;
using System.Text;
using System.Windows.Forms;
using Ionic.Zip;
using SIL.Progress;

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
		private IProgress _progress;
		public HearThisPackMaker(string rootFolder)
		{
			_rootFolder = rootFolder;
			_basePath = Path.GetDirectoryName(_rootFolder);
		}

		public void Pack(string destPath, IProgress progress)
		{
			_progress = progress;
			using (var zip = new ZipFile(Encoding.UTF8))
			{
				ZipUpWavAndInfoFiles(zip, _rootFolder);
				// And we want this one more file besides the .wav and the info.xml files, so we can transfer
				// information about which lines are skipped.
				var skipPath = Path.Combine(_rootFolder, ScriptProviderBase.kSkippedLineInfoFilename);
				if (File.Exists(skipPath))
				{
					AddToZip(zip, skipPath, Path.GetFileName(_rootFolder));
				}
				zip.Save(destPath);
			}
		}

		public string Actor { get; set; }

		private void ZipUpWavAndInfoFiles(ZipFile zip, string folder)
		{
			foreach (var dir in Directory.EnumerateDirectories(folder))
				ZipUpWavAndInfoFiles(zip, dir);
			// strip off everything before the project name (including directory sep)
			var directoryPathInArchive = folder.Substring(_basePath.Length + 1);
			var infoPath = Path.Combine(folder, ChapterInfo.kChapterInfoFilename);
			if (File.Exists(infoPath))
				AddToZip(zip, infoPath, directoryPathInArchive);
			if (string.IsNullOrEmpty(Actor))
			{
				// We want everything...just grab every wav file in the folder.
				foreach (var file in Directory.EnumerateFiles(folder, "*.wav"))
				{
					AddToZip(zip, file, directoryPathInArchive);
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
						AddToZip(zip, file, directoryPathInArchive);
				}
			}
		}

		private void AddToZip(ZipFile zip, string path, string directoryPathInArchive)
		{
			var entry = zip.AddFile(path, directoryPathInArchive);
			if (_progress != null)
			{
				_progress.WriteMessage(entry.FileName);
				// Feels like the LogBox should handle this itself, but currently it doesn't.
				// Probably I should be running this task in a background thread.
				var progressControl = _progress as Control;
				if (progressControl != null)
					progressControl.Update();
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
