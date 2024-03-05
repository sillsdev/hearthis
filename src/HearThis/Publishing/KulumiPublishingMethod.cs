using System;
using System.IO;
using System.Linq;
using SIL.IO;

namespace HearThis.Publishing
{
	public class KulumiPublishingMethod : PublishingMethodBase
	{
		private const string kFilenameFormat = "{0}_{1}_{2}.mp3"; // Format: Date_BookName_ChapterNumber.mp3

		private readonly string[] oldTestamentBooks = {
			"Genesis", "Exodus", "Leviticus", "Numbers", "Deuteronomy",
			"Joshua", "Judges", "Ruth", "1 Samuel", "2 Samuel",
			"1 Kings", "2 Kings", "1 Chronicles", "2 Chronicles", "Ezra",
			"Nehemiah", "Esther", "Job", "Psalms", "Proverbs",
			"Ecclesiastes", "Song of Solomon", "Isaiah", "Jeremiah", "Lamentations",
			"Ezekiel", "Daniel", "Hosea", "Joel", "Amos",
			"Obadiah", "Jonah", "Micah", "Nahum", "Habakkuk",
			"Zephaniah", "Haggai", "Zechariah", "Malachi"
		};

		private readonly string[] newTestamentBooks = {
			"Matthew", "Mark", "Luke", "John", "Acts",
			"Romans", "1 Corinthians", "2 Corinthians", "Galatians", "Ephesians",
			"Philippians", "Colossians", "1 Thessalonians", "2 Thessalonians", "1 Timothy",
			"2 Timothy", "Titus", "Philemon", "Hebrews", "James",
			"1 Peter", "2 Peter", "1 John", "2 John", "3 John",
			"Jude", "Revelation"
		};

		private readonly string[] allBooks;

		public KulumiPublishingMethod() : base(new LameEncoder())
		{
			allBooks = oldTestamentBooks.Concat(newTestamentBooks).ToArray();
		}

		public override void DeleteExistingPublishedFiles(string rootFolderPath, string bookName)
		{
			if (!Directory.Exists(rootFolderPath))
				return;

			string testamentFolder = GetTestamentFolder(bookName);
			string bookFolderPath = Path.Combine(rootFolderPath, testamentFolder, bookName);

			if (Directory.Exists(bookFolderPath))
				Directory.Delete(bookFolderPath, recursive: true);
		}

		public override string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			string testamentFolder = GetTestamentFolder(bookName);
			string chapterFileName = string.Format(kFilenameFormat, DateTime.Now.ToString("yyyyMMddHHmmss"), bookName, chapterNumber.ToString("D3"));
			string bookFolderPath = Path.Combine(rootFolderPath, testamentFolder, bookName);
			string fullPath = Path.Combine(bookFolderPath, chapterFileName);

			EnsureDirectory(bookFolderPath);
			return fullPath;
		}

		public override string RootDirectoryName => "DATAPACK";

		private void EnsureDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		private string GetTestamentFolder(string bookName)
		{
			return oldTestamentBooks.Contains(bookName) ? "01 Old Testament" : newTestamentBooks.Contains(bookName) ? "02 New Testament" : "Unknown";
		}

		// Additional functionalities to meet Kulumi structure requirements.

		// Method to populate a test structure. Implement your actual logic for populating MP3 files.
		public void PopulateStructure(string rootFolderPath, string[][] chaptersPerBook, string htgCmdPath, string macExcludeTxtPath)
		{
			EnsureDirectory(rootFolderPath);

			for (int i = 0; i < allBooks.Length; i++)
			{
				string bookName = allBooks[i];
				string testament = GetTestamentFolder(bookName);
				string testamentFolderPath = Path.Combine(rootFolderPath, testament);
				string bookFolderPath = Path.Combine(testamentFolderPath, bookName);

				EnsureDirectory(bookFolderPath);

				// Example for copying HTGv4.cmd and Mac_Exclude.txt into each book folder
				RobustFile.Copy(htgCmdPath, Path.Combine(bookFolderPath, "HTGv4.cmd"), true);
				RobustFile.Copy(macExcludeTxtPath, Path.Combine(bookFolderPath, "Mac_Exclude.txt"), true);

				foreach (string chapter in chaptersPerBook[i])
				{
					string chapterFilePath = GetFilePathWithoutExtension(rootFolderPath, bookName, int.Parse(chapter));
					// Assume chapters are named simply by their number; adapt as needed.
					// This is where you would actually copy or create the MP3 files.
				}
			}
		}
	}
}
