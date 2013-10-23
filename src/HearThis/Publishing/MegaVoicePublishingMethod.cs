using System.Collections.Generic;
using System.IO;
using HearThis.Script;
using Palaso.Progress;

namespace HearThis.Publishing
{
	/// <summary>
	/// From the nscribe manual:
	/// All audio content that you wish to use with the nScribe™ software must be in the form of mono, 16 bit, 44.1 khz, WAV (.wav) files.
	/// If you plan to use ‘Auto-Arrange Nested’ function (see next section), make sure to give the audio tag file the same name as the tier folder name and add the three-letter prefix “TAG”. For example, for a tier named “Luke” create an audio tag named “TAGLuke.wav”. See Appendix B for more information.
	/// </summary>
	public class MegaVoicePublishingMethod : IPublishingMethod
	{
		private BibleStats _statistics;
		private IAudioEncoder _encoder;
		Dictionary<string, int> filesOutput = new Dictionary<string, int>();
		private Dictionary<string, int> bookSequenceForThisPublishing = new Dictionary<string, int>();

		public MegaVoicePublishingMethod(string projectName)
		{
			_statistics = new BibleStats();
			_encoder = new WavEncoder();
			polulateBookSequnecForThisPublishing(projectName);
		}

		public string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			// Megavoice requires files numbered sequentially from 001 for each book.
			int fileNumber;
			filesOutput.TryGetValue(bookName, out fileNumber); // if not found it will be zero.
			fileNumber++;
			filesOutput[bookName] = fileNumber;
			string chapterIndex = fileNumber.ToString("000");
			string fileName = string.Format("{0}-{1}",  bookName, chapterIndex);

			// Megavoice requires books numbered sequentially from 001.
			// The sequence numbers are pre populated in the dictionary bookSequenceForThisPublishing
			string bookSequence = bookSequenceForThisPublishing[bookName].ToString("000");
			var dir = CreateDirectoryIfNeeded(rootFolderPath, GetFolderName(bookName, bookSequence));

			return Path.Combine(dir, fileName);
		}

		private string GetFolderName(string bookName, string bookIndex)
		{
			return string.Format("{0}-{1}", bookName, bookIndex);
		}

		public virtual string GetRootDirectoryName()
		{
			return "MegaVoice";
		}

		public void PublishChapter(string rootPath, string bookName, int chapterNumber, string pathToIncomingChapterWav, IProgress progress)
		{
			var outputPath = GetFilePathWithoutExtension(rootPath, bookName, chapterNumber);
			_encoder.Encode(pathToIncomingChapterWav, outputPath, progress);
		}

		private string CreateDirectoryIfNeeded(string parent, string child)
		{
			var path = Path.Combine(parent, child);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}

		private void polulateBookSequnecForThisPublishing(string projectName)
		{
			Dictionary<string, bool> bookExists = new Dictionary<string, bool>();
			foreach (string bookPath in Directory.GetDirectories(LineRecordingRepository.GetApplicationDataFolder(projectName)))
			{
				string bookName = Path.GetFileName(bookPath);
				int bookIndex = _statistics.GetBookNumber(bookName);
				if (isTheBookRecorded(bookPath))
					bookExists[bookName] = true;
				else
					bookExists[bookName] = false;
			}
			int sequence= 0;
			foreach (string bookName in _statistics.BookNames)
			{
				if (bookExists[bookName])
					bookSequenceForThisPublishing[bookName] = ++sequence;
				else
					bookSequenceForThisPublishing[bookName] = -1;
			}
		}

		private bool isTheBookRecorded(string bookPath)
		{
			foreach (var chapterPath in Directory.GetDirectories(bookPath))
			{
				var verseFiles = Directory.GetFiles(chapterPath);
				if (verseFiles.Length > 0)
					return true;
			}
			return false;
		}
	}
}