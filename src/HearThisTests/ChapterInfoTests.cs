using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using HearThis.Script;
using Paratext;

namespace HearThisTests
{
	[TestFixture]
	public class ChapterInfoTests
	{
		private ScriptureStub _scriptureStub;
		private ParatextScriptProvider _psp;
		private BookInfo _bookInfo;
		private bool _chapterInfoCreated;

		public class DummyBookInfo : BookInfo
		{
			private static string s_folder = Path.GetTempPath();

			public DummyBookInfo(string projectName, int number, IScriptProvider scriptProvider) : base(projectName, number, scriptProvider)
			{
			}

			public override string GetChapterFolder(int chapterNumber)
			{
				return Path.Combine(s_folder, chapterNumber.ToString());
			}

			public override int GetCountOfRecordingsForChapter(int chapterNumber)
			{
				return Directory.GetFiles(GetChapterFolder(chapterNumber), "*.wav").Length;
			}
		}

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			_scriptureStub = new ScriptureStub();
			_scriptureStub.UsfmTokens = new List<UsfmToken>();
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "RUT"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "is", null, null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Intro to Ruth", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "ip", null, null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is nice. It's good, too.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:1.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:2.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is the first sentence in verse 1:3. This is the second sentence in verse 1:3.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "4"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:4.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "5"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:5.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "6"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:6.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "7"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:7.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "8"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:8.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "9"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:9.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 2:1.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 2:2.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 2:3.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "4"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 2:4, alone in last paragraph.", null));
			// Chapter 3 not translated
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "4"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 3:1.", null));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			_scriptureStub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is verse 3:2.", null));

			_psp = new ParatextScriptProvider(_scriptureStub);
			_bookInfo = new DummyBookInfo("frog", 7, _psp);
			_psp.LoadBook(7);

			for (int i = 0; i <= 4; i++)
				Directory.CreateDirectory(_bookInfo.GetChapterFolder(i));
		}

		[TestFixtureTearDown]
		public void TestFixtureTeardown()
		{
			for (int i = 0; i <= 4; i++)
				Directory.Delete(_bookInfo.GetChapterFolder(i), true);
		}

		[SetUp]
		public void Setup()
		{
			_chapterInfoCreated = false;
			for (int i = 0; i <= 4; i++)
				Assert.AreEqual(0, Directory.GetFiles(_bookInfo.GetChapterFolder(i)).Length);
		}

		[TearDown]
		public void Teardown()
		{
			for (int i = 0; i <= 4; i++)
			{
				foreach (var file in Directory.EnumerateFiles(_bookInfo.GetChapterFolder(i)))
					File.Delete(file);
			}
		}

		[Test]
		public void Create_NoExistingInfoFileOrRecordings_DoesNotCreateInfoFile()
		{
			const int kChapter = 1;

			string chapterInfoFilePath = Path.Combine(_bookInfo.GetChapterFolder(kChapter), ChapterInfo.kChapterInfoFilename);

			var chapterInfo = CreateChapterInfo(kChapter);
			Assert.IsFalse(File.Exists(chapterInfoFilePath));
			Assert.AreEqual(ChapterInfo.DataMigrationStatus.Completed, chapterInfo.VersesInChapterStatus);
		}

		[Test]
		public void Create_NoExistingInfoFileForIntroChapterWithRecordings_CreatesInfoFileWithMigrationCompletedWithoutMovingRecordings()
		{
			const int kChapter = 0;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Intro Head");
			WriteWavFile(chapterFolder, 1, "Intro Line 1");
			WriteWavFile(chapterFolder, 2, "Intro Line 2");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			var info = CreateChapterInfo(kChapter);

			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			Assert.AreEqual(ChapterInfo.DataMigrationStatus.Completed, info.VersesInChapterStatus);
			VerifyWavFile(chapterFolder, 0, "Intro Head");
			VerifyWavFile(chapterFolder, 1, "Intro Line 1");
			VerifyWavFile(chapterFolder, 2, "Intro Line 2");

			for (int i = 1; i <= 4; i++)
				Assert.AreEqual(0, Directory.GetFiles(_bookInfo.GetChapterFolder(i)).Length);
		}

		[Test]
		public void Create_NoExistingInfoFileForFinalChapterWithRecordings_CreatesInfoFileWithMigrationCompletedWithoutMovingRecordings()
		{
			const int kChapter = 4;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 4");
			WriteWavFile(chapterFolder, 1, "Verse 1");
			WriteWavFile(chapterFolder, 2, "Verse 2");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);
			Assert.IsFalse(File.Exists(chapterInfoFilePath));

			var info = CreateChapterInfo(kChapter);

			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			Assert.AreEqual(ChapterInfo.DataMigrationStatus.Completed, info.VersesInChapterStatus);
			VerifyWavFile(chapterFolder, 0, "Chapter 4");
			VerifyWavFile(chapterFolder, 1, "Verse 1");
			VerifyWavFile(chapterFolder, 2, "Verse 2");

			for (int i = 0; i < 4; i++)
				Assert.AreEqual(0, Directory.GetFiles(_bookInfo.GetChapterFolder(i)).Length);
		}

		[Test]
		public void Create_NoExistingInfoFileForFirstChapterWithAllRecordings_CreatesInfoFileWithMigrationCompletedAfterMovingRecordings()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			string chapter2Folder = _bookInfo.GetChapterFolder(kChapter + 1);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");
			WriteWavFile(chapterFolder, 2, "Verse 2");
			WriteWavFile(chapterFolder, 3, "Verse 3A");
			WriteWavFile(chapterFolder, 4, "Verse 3B");
			WriteWavFile(chapterFolder, 5, "Verse 4");
			WriteWavFile(chapterFolder, 6, "Verse 5");
			WriteWavFile(chapterFolder, 7, "Verse 6");
			WriteWavFile(chapterFolder, 8, "Verse 7");
			WriteWavFile(chapter2Folder, 0, "Verse 8");
			WriteWavFile(chapter2Folder, 1, "Verse 9");
			WriteWavFile(chapter2Folder, 2, "Chapter 2");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			var info = CreateChapterInfo(kChapter);

			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			Assert.AreEqual(ChapterInfo.DataMigrationStatus.Completed, info.VersesInChapterStatus);
			VerifyWavFile(chapterFolder, 0, "Chapter 1");
			VerifyWavFile(chapterFolder, 1, "Verse 1");
			VerifyWavFile(chapterFolder, 2, "Verse 2");
			VerifyWavFile(chapterFolder, 3, "Verse 3A");
			VerifyWavFile(chapterFolder, 4, "Verse 3B");
			VerifyWavFile(chapterFolder, 5, "Verse 4");
			VerifyWavFile(chapterFolder, 6, "Verse 5");
			VerifyWavFile(chapterFolder, 7, "Verse 6");
			VerifyWavFile(chapterFolder, 8, "Verse 7");
			VerifyWavFile(chapterFolder, 9, "Verse 8");
			VerifyWavFile(chapterFolder, 10, "Verse 9");
			Assert.AreEqual(12, Directory.GetFiles(chapterFolder).Length);
			VerifyWavFile(chapter2Folder, 0, "Chapter 2");
			Assert.AreEqual(1, Directory.GetFiles(chapter2Folder).Length);
		}

		[Test]
		public void Create_NoExistingInfoFileForFirstChapterWithNoRecordingsForLastPara_CreatesInfoFileWithMigrationCompletedAfterShiftingRecordingsInFollowingChapter()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			string chapter2Folder = _bookInfo.GetChapterFolder(kChapter + 1);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");
			WriteWavFile(chapterFolder, 2, "Verse 2");
			WriteWavFile(chapter2Folder, 2, "Chapter 2");
			WriteWavFile(chapter2Folder, 3, "Verse 1");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			var info = CreateChapterInfo(kChapter);

			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			Assert.AreEqual(ChapterInfo.DataMigrationStatus.Completed, info.VersesInChapterStatus);
			VerifyWavFile(chapterFolder, 0, "Chapter 1");
			VerifyWavFile(chapterFolder, 1, "Verse 1");
			VerifyWavFile(chapterFolder, 2, "Verse 2");
			Assert.AreEqual(4, Directory.GetFiles(chapterFolder).Length);
			VerifyWavFile(chapter2Folder, 0, "Chapter 2");
			VerifyWavFile(chapter2Folder, 1, "Verse 1");
			Assert.AreEqual(2, Directory.GetFiles(chapter2Folder).Length);
		}

		[Test]
		public void Create_NoExistingInfoFileForSecondChapterWithSomeRecordings_CreatesInfoFileWithMigrationCompletedAfterMovingRecordingsFromChapterFour()
		{
			const int kChapter = 2;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			string chapter3Folder = _bookInfo.GetChapterFolder(kChapter + 1);
			string chapter4Folder = _bookInfo.GetChapterFolder(kChapter + 2);
			WriteWavFile(chapterFolder, 0, "Chapter 2");
			WriteWavFile(chapterFolder, 1, "Verse 1");
			WriteWavFile(chapter4Folder, 0, "Verse 4");
			WriteWavFile(chapter4Folder, 3, "Verse 2");

			var info = CreateChapterInfo(kChapter);

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);
			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			Assert.AreEqual(ChapterInfo.DataMigrationStatus.Completed, info.VersesInChapterStatus);
			VerifyWavFile(chapterFolder, 0, "Chapter 2");
			VerifyWavFile(chapterFolder, 1, "Verse 1");
			VerifyWavFile(chapterFolder, 4, "Verse 4");
			Assert.AreEqual(4, Directory.GetFiles(chapterFolder).Length);
			Assert.AreEqual(0, Directory.GetFiles(chapter3Folder).Length);
			VerifyWavFile(chapter4Folder, 2, "Verse 2");
			Assert.AreEqual(1, Directory.GetFiles(chapter4Folder).Length);
		}

		[Test]
		public void Create_WorstCaseScenarioWhereTextHasChangedResultingInConflicts_CreatesInfoFileWithMigrationCompletedAfterMovingRecordings()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			string chapter2Folder = _bookInfo.GetChapterFolder(kChapter + 1);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");
			WriteWavFile(chapterFolder, 2, "Verse 2");
			WriteWavFile(chapterFolder, 3, "Verse 3A");
			WriteWavFile(chapterFolder, 4, "Verse 3B");
			WriteWavFile(chapterFolder, 5, "Verse 4");
			WriteWavFile(chapterFolder, 6, "Verse 5");
			WriteWavFile(chapterFolder, 7, "Verse 6");
			WriteWavFile(chapterFolder, 8, "Verse 7A");
			WriteWavFile(chapterFolder, 9, "Verse 7B");
			WriteWavFile(chapter2Folder, 0, "Verse 8A");
			WriteWavFile(chapter2Folder, 1, "Verse 8B-9A");
			WriteWavFile(chapter2Folder, 2, "Verse 9B");
			WriteWavFile(chapter2Folder, 3, "Chapter 2");
			WriteWavFile(chapter2Folder, 4, "Verse 1");
			WriteWavFile(chapter2Folder, 5, "Verse 2");
			WriteWavFile(chapter2Folder, 6, "Verse 3");
			WriteWavFile(chapter2Folder, 7, "Verse 4");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			var info = CreateChapterInfo(kChapter);

			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			Assert.AreEqual(ChapterInfo.DataMigrationStatus.ManualCheckingRequired, info.VersesInChapterStatus);
			VerifyWavFile(chapterFolder, 0, "Chapter 1");
			VerifyWavFile(chapterFolder, 1, "Verse 1");
			VerifyWavFile(chapterFolder, 2, "Verse 2");
			VerifyWavFile(chapterFolder, 3, "Verse 3A");
			VerifyWavFile(chapterFolder, 4, "Verse 3B");
			VerifyWavFile(chapterFolder, 5, "Verse 4");
			VerifyWavFile(chapterFolder, 6, "Verse 5");
			VerifyWavFile(chapterFolder, 7, "Verse 6");
			VerifyWavFile(chapterFolder, 8, "Verse 7A");
			VerifyBakFile(chapterFolder, 9, "Verse 7B");
			VerifyWavFile(chapterFolder, 9, "Verse 8A");
			VerifyWavFile(chapterFolder, 10, "Verse 8B-9A");
			Assert.AreEqual(13, Directory.GetFiles(chapterFolder).Length);
			VerifyWavFile(chapter2Folder, 0, "Verse 9B"); // Doesn't get moved because text has changed so that last para only has two lines
			VerifyWavFile(chapter2Folder, 1, "Chapter 2");
			VerifyWavFile(chapter2Folder, 2, "Verse 1");
			VerifyWavFile(chapter2Folder, 3, "Verse 2");
			VerifyWavFile(chapter2Folder, 4, "Verse 3");
			VerifyWavFile(chapter2Folder, 5, "Verse 4");
			Assert.AreEqual(6, Directory.GetFiles(chapter2Folder).Length);
		}

		[Test]
		public void Create_ExistingInfoFileShowingMigrationCompleted_DoesNotReattemptMigration()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			string chapter2Folder = _bookInfo.GetChapterFolder(kChapter + 1);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");
			WriteWavFile(chapterFolder, 2, "Verse 2");
			WriteWavFile(chapterFolder, 3, "Verse 3A");
			WriteWavFile(chapterFolder, 4, "Verse 3B");
			WriteWavFile(chapterFolder, 5, "Verse 4");
			WriteWavFile(chapterFolder, 6, "Verse 5");
			WriteWavFile(chapterFolder, 7, "Verse 6");
			WriteWavFile(chapterFolder, 8, "Verse 7");
			WriteWavFile(chapterFolder, 9, "Verse 8");
			WriteWavFile(chapterFolder, 10, "Verse 9");
			WriteWavFile(chapter2Folder, 0, "Chapter 2");
			WriteWavFile(chapter2Folder, 1, "Verse 1");
			WriteWavFile(chapter2Folder, 2, "Verse 2");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			ChapterInfo info = new ChapterInfo();
			info.ChapterNumber1Based = kChapter;
			info.VersesInChapterStatus = ChapterInfo.DataMigrationStatus.Completed;
			info.Recordings = new List<ScriptLine>();
			File.WriteAllText(chapterInfoFilePath, info.ToXmlString());
			Assert.IsTrue(File.Exists(chapterInfoFilePath));

			info = CreateChapterInfo(kChapter);

			Assert.AreEqual(ChapterInfo.DataMigrationStatus.Completed, info.VersesInChapterStatus);
			VerifyWavFile(chapterFolder, 0, "Chapter 1");
			VerifyWavFile(chapterFolder, 1, "Verse 1");
			VerifyWavFile(chapterFolder, 2, "Verse 2");
			VerifyWavFile(chapterFolder, 3, "Verse 3A");
			VerifyWavFile(chapterFolder, 4, "Verse 3B");
			VerifyWavFile(chapterFolder, 5, "Verse 4");
			VerifyWavFile(chapterFolder, 6, "Verse 5");
			VerifyWavFile(chapterFolder, 7, "Verse 6");
			VerifyWavFile(chapterFolder, 8, "Verse 7");
			VerifyWavFile(chapterFolder, 9, "Verse 8");
			VerifyWavFile(chapterFolder, 10, "Verse 9");
			Assert.AreEqual(12, Directory.GetFiles(chapterFolder).Length);
			VerifyWavFile(chapter2Folder, 0, "Chapter 2");
			VerifyWavFile(chapter2Folder, 1, "Verse 1");
			VerifyWavFile(chapter2Folder, 2, "Verse 2");
			Assert.AreEqual(3, Directory.GetFiles(chapter2Folder).Length);
		}

		[Test]
		public void Create_ExistingInfoFileShowingMigrationIncomplete_DoesNotReattemptMigration()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			string chapter2Folder = _bookInfo.GetChapterFolder(kChapter + 1);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 8, "Verse 7");
			WriteWavFile(chapterFolder, 9, "Verse 8");
			WriteWavFile(chapter2Folder, 0, "Verse 9");
			WriteWavFile(chapter2Folder, 1, "Chapter 2");
			WriteWavFile(chapter2Folder, 2, "Verse 2");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			ChapterInfo info = new ChapterInfo();
			info.ChapterNumber1Based = kChapter;
			info.VersesInChapterStatus = ChapterInfo.DataMigrationStatus.Incomplete;
			info.Recordings = new List<ScriptLine>();
			File.WriteAllText(chapterInfoFilePath, info.ToXmlString());
			Assert.IsTrue(File.Exists(chapterInfoFilePath));

			info = CreateChapterInfo(kChapter);

			Assert.AreEqual(ChapterInfo.DataMigrationStatus.Incomplete, info.VersesInChapterStatus);
			VerifyWavFile(chapterFolder, 0, "Chapter 1");
			VerifyWavFile(chapterFolder, 8, "Verse 7");
			VerifyWavFile(chapterFolder, 9, "Verse 8");
			Assert.AreEqual(4, Directory.GetFiles(chapterFolder).Length);
			VerifyWavFile(chapter2Folder, 0, "Verse 9");
			VerifyWavFile(chapter2Folder, 1, "Chapter 2");
			VerifyWavFile(chapter2Folder, 2, "Verse 2");
			Assert.AreEqual(3, Directory.GetFiles(chapter2Folder).Length);
		}

		private ChapterInfo CreateChapterInfo(int chapterNumber)
		{
			_chapterInfoCreated = true;
			return ChapterInfo.Create(_bookInfo, chapterNumber);
		}

		private void WriteWavFile(string chapterFolder, int fileNumber, string contents)
		{
			Assert.IsFalse(_chapterInfoCreated, "This test is attempting to write a WAV file after creating the ChapterInfo. You probably meant to call VerifyWavFile.");
			var path = Path.Combine(chapterFolder, string.Format("{0}.wav", fileNumber));
			Assert.IsFalse(File.Exists(path), "This test is attempting to write a WAV file that already exists: " + path + ". Check to ensure that you are not accidentally supplying a duplicate file number.");
			File.WriteAllBytes(path, Encoding.UTF8.GetBytes(contents));
		}

		private static void VerifyWavFile(string chapterFolder, int fileNumber, string contents)
		{
			var path = Path.Combine(chapterFolder, string.Format("{0}.wav", fileNumber));
			Assert.AreEqual(Encoding.UTF8.GetBytes(contents), File.ReadAllBytes(path));
		}

		private static void VerifyBakFile(string chapterFolder, int fileNumber, string contents)
		{
			var path = Path.Combine(chapterFolder, string.Format("{0}.wav.bak", fileNumber));
			Assert.AreEqual(Encoding.UTF8.GetBytes(contents), File.ReadAllBytes(path));
		}
	}
}
