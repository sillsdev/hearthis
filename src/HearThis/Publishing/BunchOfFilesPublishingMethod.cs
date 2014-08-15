using System.IO;
using HearThis.Script;
using Palaso.Progress;

namespace HearThis.Publishing
{
	public class BunchOfFilesPublishingMethod : IPublishingMethod
	{
		private readonly IAudioEncoder _encoder;

		protected BibleStats _statistics;

		public BunchOfFilesPublishingMethod(IAudioEncoder encoder)
		{
			_encoder = encoder;
			_statistics = new BibleStats();
		}

		public virtual string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			string bookIndex = (1 + _statistics.GetBookNumber(bookName)).ToString("000");
			string chapterIndex = chapterNumber.ToString("000");
			string fileName = string.Format("{0}{1}{2} {3}", bookIndex, chapterIndex, bookName, chapterNumber);

			EnsureDirectory(rootFolderPath);

			return Path.Combine(rootFolderPath, fileName);
		}

		/// <summary>
		/// Virtual so we can override in tests
		/// </summary>
		/// <param name="path"></param>
		public virtual void EnsureDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
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