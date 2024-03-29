using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using HearThis.Publishing;
using NUnit.Framework;
using HearThis.Script;
using Paratext.Data;
using SIL.IO;

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

		[OneTimeSetUp]
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

		[OneTimeTearDown]
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
			scriptBlock.Actor = "Fred";
			scriptBlock.Character = "Jairus";
			var dateRecorded = DateTime.UtcNow;
			scriptBlock.RecordingTime = dateRecorded;
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

			var realChapterFolder = Path.GetDirectoryName(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 0));
			Directory.CreateDirectory(realChapterFolder);
			try
			{
				WriteWavFile(realChapterFolder, 0, "dummy wav file contents");
				WriteWavFile(realChapterFolder, 1, "dummy wav file contents");
				WriteWavFile(realChapterFolder, 2, "dummy wav file contents");
				info = CreateChapterInfo(kChapter);

				Assert.AreEqual(3, info.Recordings.Count);
				Assert.IsNull(info.Recordings[0].Verse);
				Assert.AreEqual("Chapter 1", info.Recordings[0].Text);
				Assert.AreEqual("1", info.Recordings[1].Verse);
				Assert.AreEqual("Verse 1", info.Recordings[1].Text);
				Assert.That(info.Recordings[1].Actor, Is.EqualTo("Fred"));
				Assert.That(info.Recordings[1].Character, Is.EqualTo("Jairus"));
				Assert.That(info.Recordings[1].RecordingTime, Is.EqualTo(dateRecorded));
				Assert.That(info.Recordings[1].RecordingTime.Kind, Is.EqualTo(DateTimeKind.Utc));
				Assert.That(info.Recordings[0].Actor, Is.Null);

				Assert.IsTrue(File.Exists(chapterInfoFilePath));
				VerifyWavFile(chapterFolder, 0, "Chapter 1");
				VerifyWavFile(chapterFolder, 1, "Verse 1");
				Assert.AreEqual(3, Directory.GetFiles(chapterFolder).Length);
			}
			finally
			{
				Directory.Delete(realChapterFolder, true);
			}
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

			var realChapterFolder = Path.GetDirectoryName(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 0));
			Directory.CreateDirectory(realChapterFolder);
			try
			{
				WriteWavFile(realChapterFolder, 0, "dummy wav file contents");
				WriteWavFile(realChapterFolder, 1, "dummy wav file contents");
				info = CreateChapterInfo(kChapter);
				Assert.IsTrue(File.Exists(chapterInfoFilePath));
				Assert.IsTrue(File.Exists(Path.ChangeExtension(chapterInfoFilePath, "corrupt")));
				Assert.AreEqual(2, info.Recordings.Count);
				Assert.AreEqual("Verse 1", info.Recordings.Last().Text);
			}
			finally
			{
				Directory.Delete(realChapterFolder, true);
			}
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
		public void OnScriptBlockRecorded_NoExistingInfoFile_SavesWithNewlyAddedRecording()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);

			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var fileC1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 0)))
				using (var fileC1_1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 1)))
				{
					File.Copy(mono.Path, fileC1.Path, true);
					File.Copy(mono.Path, fileC1_1.Path, true);

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
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(_bookInfo.ProjectName));
			}
		}

		[Test]
		public void OnScriptBlockRecorded_CorruptWavFileAndNoExistingInfoFile_FileDeletedAndInfoFileNotCreated()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);

			try
			{
				using (var fileC1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 0)))
				{
					File.WriteAllBytes(fileC1.Path, Encoding.UTF8.GetBytes(ClipRepositoryCharacterFilterTests.kRiffWavHeader));

					string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

					ChapterInfo info = CreateChapterInfo(kChapter);
					Assert.IsFalse(File.Exists(chapterInfoFilePath));

					var scriptBlock = new ScriptLine();
					scriptBlock.Number = 1;
					scriptBlock.Text = "Chapter 1";
					scriptBlock.Heading = true;

					info.OnScriptBlockRecorded(scriptBlock);
					Assert.IsFalse(File.Exists(fileC1.Path));
					Assert.IsFalse(File.Exists(chapterInfoFilePath));
					Assert.AreEqual(0, info.Recordings.Count);
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(_bookInfo.ProjectName));
			}
		}

		[Test]
		public void OnScriptBlockRecorded_RecordingForNewLineNumberGreaterThanAnyExisting_AddsRecordingToEnd()
		{
			const int kChapter = 1;

			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var fileC1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 0)))
				using (var fileC1_3 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 3)))
				{
					File.Copy(mono.Path, fileC1.Path, true);
					File.Copy(mono.Path, fileC1_3.Path, true);

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
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(_bookInfo.ProjectName));
			}
		}

		[Test]
		public void OnScriptBlockRecorded_RecordingForExisting_ReplacesRecording()
		{
			const int kChapter = 1;

			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var fileC1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 0)))
				using (var fileC1_1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 1)))
				using (var fileC1_2 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 2)))
				using (var fileC1_3 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 3)))
				{
					File.Copy(mono.Path, fileC1.Path, true);
					File.Copy(mono.Path, fileC1_1.Path, true);
					File.Copy(mono.Path, fileC1_2.Path, true);
					File.Copy(mono.Path, fileC1_3.Path, true);

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
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(_bookInfo.ProjectName));
			}
		}
		[Test]
		public void OnScriptBlockRecorded_RecordingForPreviouslyUnRecordedBlock_InsertsRecording()
		{
			const int kChapter = 1;
			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var fileC1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 0)))
				using (var fileC1_1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 1)))
				using (var fileC1_2 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 2)))
				using (var fileC1_3 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 3)))
				{
					File.Copy(mono.Path, fileC1.Path, true);
					File.Copy(mono.Path, fileC1_1.Path, true);
					File.Copy(mono.Path, fileC1_2.Path, true);
					File.Copy(mono.Path, fileC1_3.Path, true);

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
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(_bookInfo.ProjectName));
			}
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
		public void CalculatePercentageRecorded_ThreeRecordingsOneSkippedLine_ThirtySixPercent()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");
			WriteWavFile(chapterFolder, 2, "Verse 2");
			ChapterInfo info = CreateChapterInfo(kChapter);

			_psp.GetBlock(7, kChapter, 3).Skipped = true;

			try
			{
				Assert.AreEqual(36, info.CalculatePercentageRecorded());
			}
			finally
			{
				_psp.GetBlock(7, kChapter, 3).Skipped = false;
			}
		}

		[Test]
		public void CalculatePercentageRecorded_NoRecordingsOneSkippedLine_ZeroPercent()
		{
			const int kChapter = 1;
			ChapterInfo info = CreateChapterInfo(kChapter);

			_psp.GetBlock(7, kChapter, 3).Skipped = true;

			try
			{
				Assert.AreEqual(0, info.CalculatePercentageRecorded());
			}
			finally
			{
				_psp.GetBlock(7, kChapter, 3).Skipped = false;
			}
		}

		[Test]
		public void CalculatePercentageRecorded_AllLinesSkipped_100Percent()
		{
			const int kChapter = 1;
			ChapterInfo info = CreateChapterInfo(kChapter);

			for (int i = 0; i < _psp.GetScriptBlockCount(7, kChapter); i++)
				_psp.GetBlock(7, kChapter, i).Skipped = true;

			try
			{
				Assert.AreEqual(100, info.CalculatePercentageRecorded());
			}
			finally
			{
				for (int i = 0; i < _psp.GetScriptBlockCount(7, kChapter); i++)
					_psp.GetBlock(7, kChapter, i).Skipped = false;
			}
		}

		[Test]
		public void HasUnresolvedProblem_NoRecordings_ReturnsFalse()
		{
			const int kChapter = 1;
			ChapterInfo info = CreateChapterInfo(kChapter);
			Assert.IsFalse(info.HasUnresolvedProblem);
		}

		[Test]
		public void HasUnresolvedProblem_AllRecordingsMatch_ReturnsFalse()
		{
			const int kChapter = 2;
			ChapterInfo info = CreateChapterInfo(kChapter);
			info.Recordings.Add(_bookInfo.ScriptProvider.GetUnfilteredBlock(_bookInfo.BookNumber, kChapter, 0));
			info.Recordings.Add(_bookInfo.ScriptProvider.GetUnfilteredBlock(_bookInfo.BookNumber, kChapter, 1));
			Assert.IsFalse(info.HasUnresolvedProblem);
		}

		[Test]
		public void HasUnresolvedProblem_SecondRecordingHasDifferentText_ReturnsTrue()
		{
			const int kChapter = 2;
			ChapterInfo info = CreateChapterInfo(kChapter);
			info.Recordings.Add(_bookInfo.ScriptProvider.GetUnfilteredBlock(_bookInfo.BookNumber, kChapter, 0));
			var scriptLine = _bookInfo.ScriptProvider.GetUnfilteredBlock(_bookInfo.BookNumber, kChapter, 1);
			var modified = new ScriptLine("Then the hungry wolf climbed into grandmother's bed to wait.")
			{
				Number = scriptLine.Number,
				Actor = scriptLine.Actor,
				Character = scriptLine.Character,
				OriginalBlockNumber = scriptLine.OriginalBlockNumber,
				RecordingTime = DateTime.Now,
				Verse = scriptLine.Verse,
				Heading = scriptLine.Heading,
				HeadingType = scriptLine.HeadingType,
			};
			info.Recordings.Add(modified);
			Assert.IsTrue(info.HasUnresolvedProblem);
		}

		[Test]
		public void HasUnresolvedProblem_HasRecordingWithoutFileBeyondCurrentScript_ReturnsFalse()
		{
			var info = CreateChapterInfoWithOneExtraRecording();
			Assert.IsFalse(info.HasUnresolvedProblem);
		}

		[Test]
		public void HasUnresolvedProblem_HasRecordingWithFileBeyondCurrentScript_ReturnsTrue()
		{
			const int kChapter = 2;
			string chapterFolder = ClipRepository.GetChapterFolder(_bookInfo.ProjectName, _bookInfo.Name, kChapter);
			Directory.Delete(ClipRepository.GetProjectFolder(_bookInfo.ProjectName), true);
			Directory.CreateDirectory(chapterFolder);
			int blockCount = _bookInfo.ScriptProvider.GetUnfilteredScriptBlockCount(_bookInfo.BookNumber, kChapter);
			try
			{
				WriteWavFile(chapterFolder, blockCount, "extra");
				var info = CreateChapterInfoWithOneExtraRecording();
				Assert.IsTrue(info.HasUnresolvedProblem);
			}
			finally
			{
				Directory.Delete(ClipRepository.GetProjectFolder(_bookInfo.ProjectName), true);
			}
		}

		[Test]
		public void GetExtraRecordings_HasRecordingInfoWithoutClipBeyondCurrentScript_ReturnsEmpty()
		{
			var info = CreateChapterInfoWithOneExtraRecording();
			Assert.That(info.GetExtraClips(), Is.Empty);
		}

		[Test]
		public void GetExtraRecordings_HasRecordingInfoAndClipBeyondCurrentScript_ReturnsSingleExtraRecordingWithInfoAndClipFile()
		{
			const int kChapter = 2;
			string chapterFolder = ClipRepository.GetChapterFolder(_bookInfo.ProjectName, _bookInfo.Name, kChapter);
			int blockCount = _bookInfo.ScriptProvider.GetUnfilteredScriptBlockCount(_bookInfo.BookNumber, kChapter);
			try
			{
				var expectedPath = WriteWavFile(chapterFolder, blockCount, "extra");
				var info = CreateChapterInfoWithOneExtraRecording(kChapter);
				var extra = info.GetExtraClips().Single();
				Assert.AreEqual(info.Recordings.Last(), extra.RecordingInfo);
				Assert.AreEqual(expectedPath, extra.ClipFile);
			}
			finally
			{
				Directory.Delete(ClipRepository.GetProjectFolder(_bookInfo.ProjectName), true);
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GetExtraRecordings_HasOrphanClipsBeyondCurrentScript_ReturnsExtraRecordingsInOrderWithNullInfo(bool addRecordingInfoForRealBlocks)
		{
			const int kChapter = 2;
			string chapterFolder = ClipRepository.GetChapterFolder(_bookInfo.ProjectName, _bookInfo.Name, kChapter);
			int blockCount = _bookInfo.ScriptProvider.GetUnfilteredScriptBlockCount(_bookInfo.BookNumber, kChapter);
			try
			{
				var expectedPaths = new[]
				{
					WriteWavFile(chapterFolder, blockCount, "extra"),
					WriteWavFile(chapterFolder, blockCount + 2, "another extra")
				};
				var info = CreateChapterInfo(kChapter);

				if (addRecordingInfoForRealBlocks)
				{
					using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
					{
						for (var r = 0; r < blockCount; r++)
						{
							using (var fileC1 = TempFile.WithFilename(
								ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, r)))
							{
								File.Copy(mono.Path, fileC1.Path, true);
								info.OnScriptBlockRecorded(new ScriptLine($"Line {r}") {Number = r + 1});
							}
						}
					}
				}

				var extras = info.GetExtraClips().ToList();
				int i = 0;
				Assert.IsNull(extras[i].RecordingInfo);
				Assert.AreEqual(expectedPaths[i], extras[i++].ClipFile);
				Assert.IsNull(extras[i].RecordingInfo);
				Assert.AreEqual(expectedPaths[i], extras[i++].ClipFile);
				Assert.AreEqual(i, extras.Count);
			}
			finally
			{
				Directory.Delete(ClipRepository.GetProjectFolder(_bookInfo.ProjectName), true);
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GetExtraRecordings_HasOrphanRecordingInfoAndClipsBeyondCurrentScript_ReturnsExtraClipsInOrder(bool addRecordingInfoForRealBlocks)
		{
			const int kChapter = 2;
			string chapterFolder = ClipRepository.GetChapterFolder(_bookInfo.ProjectName, _bookInfo.Name, kChapter);
			Directory.Delete(ClipRepository.GetProjectFolder(_bookInfo.ProjectName), true);
			Directory.CreateDirectory(chapterFolder);
			int blockCount = _bookInfo.ScriptProvider.GetUnfilteredScriptBlockCount(_bookInfo.BookNumber, kChapter);
			try
			{
				var expectedPaths = new[]
				{
					WriteWavFile(chapterFolder, blockCount + 1, "extra"), // This one will not have a corresponding Recording info entry
					WriteWavFile(chapterFolder, blockCount + 2, "last extra"), // This one will have a corresponding Recording info entry
				};

				var info = CreateChapterInfoWithOneExtraRecording(); // This extra does not correspond to a wav file

				if (addRecordingInfoForRealBlocks)
				{
					using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
					{
						for (var r = 0; r < blockCount; r++)
						{
							using (var fileC1 = TempFile.WithFilename(
								ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, r)))
							{
								File.Copy(mono.Path, fileC1.Path, true);
								info.OnScriptBlockRecorded(new ScriptLine($"Line {r}") {Number = r + 1});
							}
						}
					}
				}

				info.Recordings.Add(new ScriptLine("Last extra") {Number = blockCount + 3});

				var extras = info.GetExtraClips().ToList();
				int i = 0;
				Assert.IsNull(extras[i].RecordingInfo);
				Assert.AreEqual(expectedPaths[0], extras[i++].ClipFile);
				Assert.AreEqual("Last extra", extras[i].RecordingInfo.Text);
				Assert.AreEqual(expectedPaths[1], extras[i++].ClipFile);
				Assert.AreEqual(i, extras.Count);
			}
			finally
			{
				Directory.Delete(ClipRepository.GetProjectFolder(_bookInfo.ProjectName), true);
			}
		}

		[Test]
		public void IndexOfFirstUnfilteredBlockWithProblem_HasClipBeyondCurrentScript_ReturnsExpectedIndex()
		{
			const int kChapter = 2;
			string chapterFolder = ClipRepository.GetChapterFolder(_bookInfo.ProjectName, _bookInfo.Name, kChapter);
			Directory.Delete(ClipRepository.GetProjectFolder(_bookInfo.ProjectName), true);
			Directory.CreateDirectory(chapterFolder);
			int blockCount = _bookInfo.ScriptProvider.GetUnfilteredScriptBlockCount(_bookInfo.BookNumber, kChapter);
			try
			{
				WriteWavFile(chapterFolder, blockCount, "extra");

				var info = CreateChapterInfoWithOneExtraRecording();
				int i = info.IndexOfFirstUnfilteredBlockWithProblem;
				Assert.AreEqual(_bookInfo.ScriptProvider.GetUnfilteredScriptBlockCount(_bookInfo.BookNumber, info.ChapterNumber1Based), i);
			}
			finally
			{
				Directory.Delete(ClipRepository.GetProjectFolder(_bookInfo.ProjectName), true);
			}
		}

		[Test]
		public void GetIndexOfNextUnfilteredBlockWithProblem_StartingFromLastExtra_ReturnsNegativeOne()
		{
			const int kChapter = 2;
			string chapterFolder = ClipRepository.GetChapterFolder(_bookInfo.ProjectName, _bookInfo.Name, kChapter);
			Directory.Delete(ClipRepository.GetProjectFolder(_bookInfo.ProjectName), true);
			Directory.CreateDirectory(chapterFolder);
			int blockCount = _bookInfo.ScriptProvider.GetUnfilteredScriptBlockCount(_bookInfo.BookNumber, kChapter);
			try
			{
				WriteWavFile(chapterFolder, blockCount, "extra");
				var info = CreateChapterInfoWithOneExtraRecording();
				Assert.AreEqual(-1, info.GetIndexOfNextUnfilteredBlockWithProblem(blockCount));
			}
			finally
			{
				Directory.Delete(ClipRepository.GetProjectFolder(_bookInfo.ProjectName), true);
			}
		}

		private ChapterInfo CreateChapterInfo(int chapterNumber)
		{
			_chapterInfoCreated = true;
			return ChapterInfo.Create(_bookInfo, chapterNumber);
		}

		private ChapterInfo CreateChapterInfoWithOneExtraRecording(int chapter = 2)
		{
			ChapterInfo info = CreateChapterInfo(chapter);
			info.Recordings.Add(_bookInfo.ScriptProvider.GetUnfilteredBlock(_bookInfo.BookNumber, chapter, 0));
			DateTime.TryParse("01/01/2018", out var recordedDate);
			var count = _bookInfo.ScriptProvider.GetScriptBlockCount(_bookInfo.BookNumber, chapter);
			var extra = new ScriptLine("Then the hungry wolf climbed into grandmother's bed to wait.")
			{
				Number = count + 1,
				RecordingTime = recordedDate,
				Verse = _bookInfo.ScriptProvider.GetUnfilteredBlock(_bookInfo.BookNumber, chapter, count - 1).Verse,
			};
			info.Recordings.Add(extra);
			return info;
		}

		private string WriteWavFile(string chapterFolder, int fileNumber, string contents)
		{
			Assert.IsFalse(_chapterInfoCreated, "This test is attempting to write a WAV file after creating the ChapterInfo. You probably meant to call VerifyWavFile.");
			var path = Path.Combine(chapterFolder, $"{fileNumber}.wav");
			Assert.IsFalse(File.Exists(path), "This test is attempting to write a WAV file that already exists: " + path + ". Check to ensure that you are not accidentally supplying a duplicate file number.");
			File.WriteAllBytes(path, Encoding.UTF8.GetBytes(ClipRepositoryCharacterFilterTests.kRiffWavHeader + contents));
			return path;
		}

		private static void VerifyWavFile(string chapterFolder, int fileNumber, string contents)
		{
			var path = Path.Combine(chapterFolder, $"{fileNumber}.wav");
			Assert.AreEqual(Encoding.UTF8.GetBytes(ClipRepositoryCharacterFilterTests.kRiffWavHeader + contents), File.ReadAllBytes(path));
		}
	}
}
