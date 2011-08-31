using System.IO;
using HearThis.Script;
using Palaso.Progress.LogBox;

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
		public MegaVoicePublishingMethod()
		{
			_statistics = new BibleStats();
			_encoder = new WavEncoder();
		}
		public string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			string bookIndex = (1 + _statistics.GetBookNumber(bookName)).ToString("000");
			string chapterIndex = chapterNumber.ToString("000");
			string fileName = string.Format("{0}-{1}",  bookName, chapterIndex);

			var dir = CreateDirectoryIfNeeded(rootFolderPath, GetFolderName(bookName, bookIndex));

			return Path.Combine(dir, fileName);
		}

		private string GetFolderName(string bookName, string bookIndex)
		{
			return string.Format("{0}-{1}", bookName, bookIndex);
		}

		public string GetRootDirectoryName()
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

	}
}