using System.IO;
using HearThis.Script;
using Palaso.Progress.LogBox;

namespace HearThis.Publishing
{
	public class BunchOfFilesPublishingMethod : IPublishingMethod
	{
		private readonly IAudioEncoder _encoder;

		private BibleStats _statistics;

		public BunchOfFilesPublishingMethod(IAudioEncoder encoder)
		{
			_encoder = encoder;

			_statistics = new BibleStats();
		}

		public string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			string bookIndex = (1+_statistics.GetBookNumber(bookName)).ToString("000");
			string chapterIndex = chapterNumber.ToString("000");
			string fileName = string.Format("{0}{1}{2} {3}", bookIndex, chapterIndex, bookName, chapterNumber);

			if(!Directory.Exists(rootFolderPath))
			{
				Directory.CreateDirectory(rootFolderPath);
			}

			return Path.Combine(rootFolderPath, fileName);
		}

		public string GetRootDirectoryName()
		{
			return _encoder.FormatName;
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