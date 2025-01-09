// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global. All Rights Reserved.
// <copyright from='2017' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.IO;
using System.Text;
using System.Windows.Forms;
using DesktopAnalytics;
using ICSharpCode.SharpZipLib.Zip;
using L10NSharp;
using SIL.Progress;

namespace HearThis.Script
{

	/// <summary>
	/// Used to make HearThisPack files, basically a zip of the folders containing the wav files and info.xml files.
	/// Can optionally filter which wav files are included based on which actor recorded them.
	/// </summary>
	public class HearThisPackMaker
	{
		public const string kHearThisPackExtension = ".hearthispack";
		private readonly string _rootFolder;
		private readonly string _basePath; // part of _rootFolder's path not to include in zip structure
		private IProgress _progress;
		private int _addedEntries = 0;
		public HearThisPackMaker(string rootFolder)
		{
			_rootFolder = rootFolder;
			_basePath = Path.GetDirectoryName(_rootFolder);
		}

		public bool Pack(string destPath, IProgress progress)
		{
			_progress = progress;
			using (var zip = ZipFile.Create(destPath))
			{
				zip.BeginUpdate();
				ZipUpWavAndInfoFiles(zip, _rootFolder);
				// And we want this one more file besides the .wav and the info.xml files, so we can transfer
				// information about which lines are skipped.
				var skipPath = Path.Combine(_rootFolder, ScriptProviderBase.kSkippedLineInfoFilename);
				if (File.Exists(skipPath))
					AddToZip(zip, skipPath, Path.GetFileName(_rootFolder));
				if (_addedEntries == 0)
				{
					WriteProgressMessage(LocalizationManager.GetString("HearThisPack.Nothing",
						"There were no relevant clips or other files in this project to include " +
						"in the HearThis Pack."), true);
					zip.AbortUpdate();
				}
				else
				{
					zip.CommitUpdate();
					Analytics.Track("HearThisPack created");
				}
			}

			if (_addedEntries == 0)
			{
				File.Delete(destPath);
				return false;
			}

			return true;
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
					AddToZip(zip, file, directoryPathInArchive);
			}
			else // restrict to actor
			{
				if (!File.Exists(infoPath))
					return; // need info file for restricted export; ignore anything in folder without one
				// We don't care about book and chapter number, just the Recordings information.
				var chapInfo = ChapterInfo.Create(null, 1, File.ReadAllText(infoPath, Encoding.UTF8), true);
				foreach (var file in Directory.EnumerateFiles(folder, "*.wav"))
				{
					if (!int.TryParse(Path.GetFileNameWithoutExtension(file), out var blockNo))
						continue; // not a proper HearThis wav file.
					blockNo++;
					if (DoesBlockHaveActor(chapInfo, blockNo))
						AddToZip(zip, file, directoryPathInArchive);
				}
			}
		}

		private void AddToZip(ZipFile zip, string path, string directoryPathInArchive)
		{
			var entryName = Path.Combine(directoryPathInArchive, Path.GetFileName(path)).Replace("\\", "/");
			zip.Add(path, entryName);
			_addedEntries++;
			WriteProgressMessage(entryName);
		}

		private void WriteProgressMessage(string message, bool warning = false)
		{
			if (_progress != null)
			{
				if (warning)
					_progress.WriteWarning(message);
				else
					_progress.WriteMessage(message);
				// Feels like the LogBox should handle this itself, but currently it doesn't.
				// Probably I should be running this task in a background thread.
				if (_progress is Control progressControl)
					progressControl.Update();
			}
		}

		private bool DoesBlockHaveActor(ChapterInfo chapInfo, int blockNo)
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
