using System.IO;
using HearThis.Script;
using Palaso.Progress;


namespace HearThis.Publishing
{
	public class SaberPublishingMethod : IPublishingMethod
	{
		private BibleStats _statistics;
		private IAudioEncoder _encoder;
		public SaberPublishingMethod()
		{
			_statistics = new BibleStats();
			_encoder = new LameEncoder();
		}
		public string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			string bookIndex = (1 + _statistics.GetBookNumber(bookName)).ToString("000");
			string chapterIndex = chapterNumber.ToString("000");
			string fileName = string.Format("{0}{1}{2} {3}", bookIndex, chapterIndex, bookName, chapterNumber);

			var dir = CreateDirectoryIfNeeded(rootFolderPath, GetFolderName(bookName, bookIndex));

			return Path.Combine(dir, fileName);
		}

		private string GetFolderName(string bookName, string bookIndex)
		{
			return string.Format("{0}{1}", bookIndex, bookName);
		}

		public string GetRootDirectoryName()
		{
			return "Saber";
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

	}
}