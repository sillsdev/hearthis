using Palaso.Progress;


namespace HearThis.Publishing
{
	public interface IPublishingMethod
	{
		string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber);
		string GetRootDirectoryName();
		void PublishChapter(string rootPath, string bookName, int chapterNumber, string pathToIncomingChapterWav, IProgress progress);
	}
}
