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
		private Dictionary<string, int> filesOutput = new Dictionary<string, int>();

		Dictionary<string, List<int>> hashTable = new Dictionary<string, List<int>>();
		List<int> list;

		public MegaVoicePublishingMethod()
		{
			_statistics = new BibleStats();
			_encoder = new WavEncoder();
		}

		public string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			// Megavoice requires files numbered sequentially from 001 for each book.
			int fileNumber;
			filesOutput.TryGetValue(bookName, out fileNumber); // if not found it will be zero.

			fileNumber = GetUniqueNameForChapter(bookName, chapterNumber);

			filesOutput[bookName] = fileNumber;
			string bookIndex = (1 + _statistics.GetBookNumber(bookName)).ToString("000");
			string chapterIndex = fileNumber.ToString("000");
			string fileName = string.Format("{0}-{1}",  bookName, chapterIndex);

			var dir = CreateDirectoryIfNeeded(rootFolderPath, GetFolderName(bookName, bookIndex));

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

		public void PublishChapter(string rootPath, string bookName, int chapterNumber, string pathToIncomingChapterWav,
			IProgress progress)
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

		/// <summary>
		/// get the unique file name for Megavoice sequential naming.
		/// </summary>
		/// <param name="bookName"></param>
		/// <param name="chapterNumber"></param>
		/// <returns></returns>
		private int GetUniqueNameForChapter(string bookName, int chapterNumber)
		{
			//if book already exists
		   if (hashTable.TryGetValue(bookName, out list))
		   {
			   // if the chapter already exists
				if (list.Contains(chapterNumber))
					return list.IndexOf(chapterNumber)+1;
				else
				{
					list.Add(chapterNumber);
					return list.IndexOf(chapterNumber)+1;
				}
	}
			else
			{
				list = new List<int>();
				list.Add(chapterNumber);
				hashTable.Add(bookName, list);
				return list.IndexOf(chapterNumber)+1;
			}
		}

	}

}
