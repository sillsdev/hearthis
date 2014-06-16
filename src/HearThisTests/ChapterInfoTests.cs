using System.Collections.Generic;
using System.Globalization;
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
			private static readonly string s_folder = Path.GetTempPath();

			public DummyBookInfo(string projectName, int number, IScriptProvider scriptProvider) : base(projectName, number, scriptProvider)
			{
			}

			public override string GetChapterFolder(int chapterNumber)
			{
				return Path.Combine(s_folder, chapterNumber.ToString(CultureInfo.InvariantCulture));
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
		public void Create_NoExistingInfoFile_DoesNotCreateInfoFile()
		{
			const int kChapter = 1;

			string chapterInfoFilePath = Path.Combine(_bookInfo.GetChapterFolder(kChapter), ChapterInfo.kChapterInfoFilename);

			Assert.AreEqual(kChapter, CreateChapterInfo(kChapter).ChapterNumber1Based);
			Assert.IsFalse(File.Exists(chapterInfoFilePath));
		}

		[Test]
		public void Create_ExistingInfoFile_PreservesExistingInfoFromFile()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			ChapterInfo info = new ChapterInfo();
			info.ChapterNumber1Based = kChapter;
			info.Recordings = new List<ScriptLine>();
			var scriptLine = new ScriptLine();
			scriptLine.Verse = "0";
			scriptLine.Text = "Chapter 1";
			scriptLine.Heading = false;
			info.Recordings.Add(scriptLine);
			scriptLine = new ScriptLine();
			scriptLine.Verse = "1";
			scriptLine.Text = "Verse 1";
			scriptLine.Heading = false;
			info.Recordings.Add(scriptLine);
			File.WriteAllText(chapterInfoFilePath, info.ToXmlString());
			Assert.IsTrue(File.Exists(chapterInfoFilePath));

			info = CreateChapterInfo(kChapter);

			Assert.AreEqual(2, info.Recordings.Count);
			Assert.AreEqual("0", info.Recordings[0].Verse);
			Assert.AreEqual("Chapter 1", info.Recordings[0].Text);
			Assert.AreEqual("1", info.Recordings[1].Verse);
			Assert.AreEqual("Verse 1", info.Recordings[1].Text);

			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			VerifyWavFile(chapterFolder, 0, "Chapter 1");
			VerifyWavFile(chapterFolder, 1, "Verse 1");
			Assert.AreEqual(3, Directory.GetFiles(chapterFolder).Length);
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
	}
}
