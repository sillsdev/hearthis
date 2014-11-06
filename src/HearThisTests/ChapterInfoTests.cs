using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
			if (ParatextScriptProvider.ParatextIsInstalled)
				ScrTextCollection.Initialize();

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
			_scriptureStub.Dispose();
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
			var scriptBlock = new ScriptLine();
			scriptBlock.Number = 1;
			scriptBlock.Text = "Chapter 1";
			scriptBlock.Heading = true;
			info.Recordings.Add(scriptBlock);
			scriptBlock = new ScriptLine();
			scriptBlock.Number = 2;
			scriptBlock.Verse = "1";
			scriptBlock.Text = "Verse 1";
			scriptBlock.Heading = false;
			info.Recordings.Add(scriptBlock);
			scriptBlock = new ScriptLine();
			scriptBlock.Number = 3;
			scriptBlock.Verse = "2";
			scriptBlock.Text = "Verse 2";
			scriptBlock.Heading = false;
			info.Recordings.Add(scriptBlock);
			File.WriteAllText(chapterInfoFilePath, info.ToXmlString());
			Assert.IsTrue(File.Exists(chapterInfoFilePath));

			info = CreateChapterInfo(kChapter);

			Assert.AreEqual(3, info.Recordings.Count);
			Assert.IsNull(info.Recordings[0].Verse);
			Assert.AreEqual("Chapter 1", info.Recordings[0].Text);
			Assert.AreEqual("1", info.Recordings[1].Verse);
			Assert.AreEqual("Verse 1", info.Recordings[1].Text);

			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			VerifyWavFile(chapterFolder, 0, "Chapter 1");
			VerifyWavFile(chapterFolder, 1, "Verse 1");
			Assert.AreEqual(3, Directory.GetFiles(chapterFolder).Length);
		}

		[Test]
		public void Create_ExistingInfoFileWithDuplicateRecordings_SavesBackupAndTruncatesListOfRecordings()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			ChapterInfo info = new ChapterInfo();
			info.ChapterNumber1Based = kChapter;
			info.Recordings = new List<ScriptLine>();
			var scriptBlock = new ScriptLine();
			scriptBlock.Number = 1;
			scriptBlock.Text = "Chapter 1";
			scriptBlock.Heading = true;
			info.Recordings.Add(scriptBlock);
			scriptBlock = new ScriptLine();
			scriptBlock.Number = 2;
			scriptBlock.Verse = "1";
			scriptBlock.Text = "Verse 1";
			scriptBlock.Heading = false;
			info.Recordings.Add(scriptBlock);
			scriptBlock = new ScriptLine();
			scriptBlock.Number = 2;
			scriptBlock.Verse = "2";
			scriptBlock.Text = "Verse 2";
			scriptBlock.Heading = false;
			info.Recordings.Add(scriptBlock);
			File.WriteAllText(chapterInfoFilePath, info.ToXmlString());
			Assert.IsTrue(File.Exists(chapterInfoFilePath));

			info = CreateChapterInfo(kChapter);
			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			Assert.IsTrue(File.Exists(Path.ChangeExtension(chapterInfoFilePath, "corrupt")));
			Assert.AreEqual(2, info.Recordings.Count);
			Assert.AreEqual("Verse 1", info.Recordings.Last().Text);
		}

		[Test]
		public void Create_ExistingInfoFileWithRecordingForLine0_SavesBackupAndTruncatesListOfRecordings()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			ChapterInfo info = new ChapterInfo();
			info.ChapterNumber1Based = kChapter;
			info.Recordings = new List<ScriptLine>();
			var scriptBlock = new ScriptLine();
			scriptBlock.Number = 0;
			scriptBlock.Text = "Chapter 1";
			scriptBlock.Heading = true;
			info.Recordings.Add(scriptBlock);
			scriptBlock = new ScriptLine();
			scriptBlock.Number = 1;
			scriptBlock.Verse = "1";
			scriptBlock.Text = "Verse 1";
			scriptBlock.Heading = false;
			info.Recordings.Add(scriptBlock);
			File.WriteAllText(chapterInfoFilePath, info.ToXmlString());
			Assert.IsTrue(File.Exists(chapterInfoFilePath));

			info = CreateChapterInfo(kChapter);
			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			Assert.IsTrue(File.Exists(Path.ChangeExtension(chapterInfoFilePath, "corrupt")));
			Assert.AreEqual(0, info.Recordings.Count);
		}

		[Test]
		public void OnRecordingSaved_NoExistingInfoFile_SavesWithNewlyAddedRecording()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			ChapterInfo info = CreateChapterInfo(kChapter);
			Assert.IsFalse(File.Exists(chapterInfoFilePath));

			var scriptBlock = new ScriptLine();
			scriptBlock.Number = 1;
			scriptBlock.Text = "Chapter 1";
			scriptBlock.Heading = true;

			info.OnScriptBlockRecorded(scriptBlock);
			Assert.IsTrue(File.Exists(chapterInfoFilePath));
			Assert.AreEqual(1, info.Recordings.Count);
			Assert.AreEqual("Chapter 1", info.Recordings[0].Text);
		}

		[Test]
		public void OnRecordingSaved_RecordingForNewLineNumberGreaterThanAnyExisting_AddsRecordingToEnd()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");

			ChapterInfo info = CreateChapterInfo(kChapter);
			var scriptBlock = new ScriptLine();
			scriptBlock.Number = 1;
			scriptBlock.Text = "Chapter 1";
			scriptBlock.Heading = true;
			info.OnScriptBlockRecorded(scriptBlock);

			scriptBlock = new ScriptLine();
			scriptBlock.Number = 4;
			scriptBlock.Text = "This is the first sentence in verse 1:3.";
			scriptBlock.Heading = false;
			info.OnScriptBlockRecorded(scriptBlock);

			Assert.AreEqual(2, info.Recordings.Count);
			Assert.AreEqual(4, info.Recordings[1].Number);
		}

		[Test]
		public void OnRecordingSaved_RecordingForExisting_ReplacesRecording()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");

			ChapterInfo info = CreateChapterInfo(kChapter);
			var scriptBlock = new ScriptLine();
			scriptBlock.Number = 1;
			scriptBlock.Text = "Chapter 1";
			scriptBlock.Heading = true;
			info.OnScriptBlockRecorded(scriptBlock);

			scriptBlock = new ScriptLine();
			scriptBlock.Number = 2;
			scriptBlock.Text = "Verse 1.";
			scriptBlock.Heading = false;
			info.OnScriptBlockRecorded(scriptBlock);

			scriptBlock = new ScriptLine();
			scriptBlock.Number = 3;
			scriptBlock.Text = "Verse 2.";
			scriptBlock.Heading = false;
			info.OnScriptBlockRecorded(scriptBlock);

			scriptBlock = new ScriptLine();
			scriptBlock.Number = 2;
			scriptBlock.Text = "Changed text for verse 1.";
			scriptBlock.Heading = false;
			info.OnScriptBlockRecorded(scriptBlock);

			Assert.AreEqual(3, info.Recordings.Count);
			Assert.AreEqual(1, info.Recordings[0].Number);
			Assert.AreEqual(2, info.Recordings[1].Number);
			Assert.AreEqual("Changed text for verse 1.", info.Recordings[1].Text);
			Assert.AreEqual(3, info.Recordings[2].Number);
		}


		[Test]
		public void OnRecordingSaved_RecordingForPreviouslyUnRecordedBlock_InsertsRecording()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			ChapterInfo info = CreateChapterInfo(kChapter);
			var scriptBlock = new ScriptLine();
			scriptBlock.Number = 1;
			scriptBlock.Text = "Chapter 1";
			scriptBlock.Heading = true;
			info.OnScriptBlockRecorded(scriptBlock);

			scriptBlock = new ScriptLine();
			scriptBlock.Number = 3;
			scriptBlock.Text = "Verse 2.";
			scriptBlock.Heading = false;
			info.OnScriptBlockRecorded(scriptBlock);

			scriptBlock = new ScriptLine();
			scriptBlock.Number = 2;
			scriptBlock.Text = "Verse 1.";
			scriptBlock.Heading = false;
			info.OnScriptBlockRecorded(scriptBlock);

			Assert.AreEqual(3, info.Recordings.Count);
			Assert.AreEqual(1, info.Recordings[0].Number);
			Assert.AreEqual("Chapter 1", info.Recordings[0].Text);
			Assert.AreEqual(2, info.Recordings[1].Number);
			Assert.AreEqual("Verse 1.", info.Recordings[1].Text);
			Assert.AreEqual(3, info.Recordings[2].Number);
			Assert.AreEqual("Verse 2.", info.Recordings[2].Text);
		}

		[Test]
		public void CalculatePercentageRecorded_NoRecordingsOrSkippedLines_Zero()
		{
			const int kChapter = 1;
			ChapterInfo info = CreateChapterInfo(kChapter);
			Assert.AreEqual(0, info.CalculatePercentageRecorded());
		}

		[Test]
		public void CalculatePercentageRecorded_ThreeRecordingsNoSkippedLines_TwentySevenPercent()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");
			WriteWavFile(chapterFolder, 2, "Verse 2");

			ChapterInfo info = CreateChapterInfo(kChapter);
			Assert.AreEqual(27, info.CalculatePercentageRecorded());
		}

		[Test]
		public void CalculatePercentageRecorded_ThreeRecordingsOneSkippedLines_ThirtySixPercent()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");
			WriteWavFile(chapterFolder, 2, "Verse 2");
			ChapterInfo info = CreateChapterInfo(kChapter);

			_psp.GetBlock(7, kChapter, 3).Skipped = true;

			Assert.AreEqual(36, info.CalculatePercentageRecorded());
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
