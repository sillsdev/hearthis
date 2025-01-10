using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using HearThis.Publishing;
using NUnit.Framework;
using HearThis.Script;
using Paratext.Data;
using SIL.IO;
using static System.Text.Encoding;

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
			_scriptureStub.UsfmTokens = new List<UsfmToken>
			{
				new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "RUT"),
				new UsfmToken(UsfmTokenType.Paragraph, "is", null, null),
				new UsfmToken(UsfmTokenType.Text, null, "Intro to Ruth", null),
				new UsfmToken(UsfmTokenType.Paragraph, "ip", null, null),
				new UsfmToken(UsfmTokenType.Text, null, "This is nice. It's good, too.", null),
				new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"),
				new UsfmToken(UsfmTokenType.Paragraph, "p", null, null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:1.", null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:2.", null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"),
				new UsfmToken(UsfmTokenType.Text, null, "This is the first sentence in verse 1:3. This is the second sentence in verse 1:3.", null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "4"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:4.", null),
				new UsfmToken(UsfmTokenType.Paragraph, "p", null, null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "5"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:5.", null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "6"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:6.", null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "7"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:7.", null),
				new UsfmToken(UsfmTokenType.Paragraph, "p", null, null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "8"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:8.", null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "9"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 1:9.", null),
				new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"),
				new UsfmToken(UsfmTokenType.Paragraph, "p", null, null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 2:1.", null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 2:2.", null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 2:3.", null),
				new UsfmToken(UsfmTokenType.Paragraph, "p", null, null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "4"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 2:4, alone in last paragraph.", null),
				// Chapter 3 not translated
				new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "4"),
				new UsfmToken(UsfmTokenType.Paragraph, "p", null, null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 3:1.", null),
				new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"),
				new UsfmToken(UsfmTokenType.Text, null, "This is verse 3:2.", null)
			};

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
				Assert.That(Directory.GetFiles(_bookInfo.GetChapterFolder(i)), Is.Empty);
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

			Assert.That(CreateChapterInfo(kChapter).ChapterNumber1Based, Is.EqualTo(kChapter));
			Assert.That(chapterInfoFilePath, Does.Not.Exist);
		}

		[Test]
		public void Create_ExistingInfoFile_PreservesExistingInfoFromFile()
		{
			const int kChapter = 1;
			string chapterFolder = _bookInfo.GetChapterFolder(kChapter);
			WriteWavFile(chapterFolder, 0, "Chapter 1");
			WriteWavFile(chapterFolder, 1, "Verse 1");

			string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

			ChapterInfo info = new ChapterInfo
			{
				ChapterNumber1Based = kChapter,
				Recordings = new List<ScriptLine>()
			};
			var scriptBlock = new ScriptLine
			{
				Number = 1,
				Text = "Chapter 1",
				Heading = true
			};
			info.Recordings.Add(scriptBlock);
			scriptBlock = new ScriptLine
			{
				Number = 2,
				Verse = "1",
				Text = "Verse 1",
				Actor = "Fred",
				Character = "Jairus"
			};
			var dateRecorded = DateTime.UtcNow;
			scriptBlock.RecordingTime = dateRecorded;
			scriptBlock.Heading = false;
			info.Recordings.Add(scriptBlock);
			scriptBlock = new ScriptLine
			{
				Number = 3,
				Verse = "2",
				Text = "Verse 2",
				Heading = false
			};
			info.Recordings.Add(scriptBlock);
			File.WriteAllText(chapterInfoFilePath, info.ToXmlString());
			Assert.That(chapterInfoFilePath, Does.Exist);

			var realChapterFolder = Path.GetDirectoryName(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 0));
			Directory.CreateDirectory(realChapterFolder);
			try
			{
				WriteWavFile(realChapterFolder, 0, "dummy wav file contents");
				WriteWavFile(realChapterFolder, 1, "dummy wav file contents");
				WriteWavFile(realChapterFolder, 2, "dummy wav file contents");
				info = CreateChapterInfo(kChapter);

				Assert.That(info.Recordings.Count, Is.EqualTo(3));
				Assert.That(info.Recordings[0].Verse, Is.Null);
				Assert.That(info.Recordings[0].Text, Is.EqualTo("Chapter 1"));
				Assert.That(info.Recordings[1].Verse, Is.EqualTo("1"));
				Assert.That(info.Recordings[1].Text, Is.EqualTo("Verse 1"));
				Assert.That(info.Recordings[1].Actor, Is.EqualTo("Fred"));
				Assert.That(info.Recordings[1].Character, Is.EqualTo("Jairus"));
				Assert.That(info.Recordings[1].RecordingTime, Is.EqualTo(dateRecorded));
				Assert.That(info.Recordings[1].RecordingTime.Kind, Is.EqualTo(DateTimeKind.Utc));
				Assert.That(info.Recordings[0].Actor, Is.Null);

				Assert.That(chapterInfoFilePath, Does.Exist);
				VerifyWavFile(chapterFolder, 0, "Chapter 1");
				VerifyWavFile(chapterFolder, 1, "Verse 1");
				Assert.That(Directory.GetFiles(chapterFolder).Length, Is.EqualTo(3));
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

			ChapterInfo info = new ChapterInfo
			{
				ChapterNumber1Based = kChapter,
				Recordings = new List<ScriptLine>()
			};
			var scriptBlock = new ScriptLine
			{
				Number = 1,
				Text = "Chapter 1",
				Heading = true
			};
			info.Recordings.Add(scriptBlock);
			scriptBlock = new ScriptLine
			{
				Number = 2,
				Verse = "1",
				Text = "Verse 1",
				Heading = false
			};
			info.Recordings.Add(scriptBlock);
			scriptBlock = new ScriptLine
			{
				Number = 2,
				Verse = "2",
				Text = "Verse 2",
				Heading = false
			};
			info.Recordings.Add(scriptBlock);
			File.WriteAllText(chapterInfoFilePath, info.ToXmlString());
			Assert.That(chapterInfoFilePath, Does.Exist);

			var realChapterFolder = Path.GetDirectoryName(ClipRepository.GetPathToLineRecording(_bookInfo.ProjectName, _bookInfo.Name, kChapter, 0));
			Directory.CreateDirectory(realChapterFolder);
			try
			{
				WriteWavFile(realChapterFolder, 0, "dummy wav file contents");
				WriteWavFile(realChapterFolder, 1, "dummy wav file contents");
				info = CreateChapterInfo(kChapter);
				Assert.That(chapterInfoFilePath, Does.Exist);
				Assert.That(Path.ChangeExtension(chapterInfoFilePath, "corrupt"), Does.Exist);
				Assert.That(info.Recordings.Count, Is.EqualTo(2));
				Assert.That(info.Recordings.Last().Text, Is.EqualTo("Verse 1"));
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

			ChapterInfo info = new ChapterInfo
			{
				ChapterNumber1Based = kChapter,
				Recordings = new List<ScriptLine>()
			};
			var scriptBlock = new ScriptLine
			{
				Number = 0,
				Text = "Chapter 1",
				Heading = true
			};
			info.Recordings.Add(scriptBlock);
			scriptBlock = new ScriptLine
			{
				Number = 1,
				Verse = "1",
				Text = "Verse 1",
				Heading = false
			};
			info.Recordings.Add(scriptBlock);
			File.WriteAllText(chapterInfoFilePath, info.ToXmlString());
			Assert.That(chapterInfoFilePath, Does.Exist);

			info = CreateChapterInfo(kChapter);
			Assert.That(chapterInfoFilePath, Does.Exist);
			Assert.That(Path.ChangeExtension(chapterInfoFilePath, "corrupt"), Does.Exist);
			Assert.That(info.Recordings, Is.Empty);
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
					Assert.That(chapterInfoFilePath, Does.Not.Exist);

					var scriptBlock = new ScriptLine
					{
						Number = 1,
						Text = "Chapter 1",
						Heading = true
					};

					info.OnScriptBlockRecorded(scriptBlock);
					Assert.That(chapterInfoFilePath, Does.Exist);
					Assert.That(info.Recordings.Single().Text, Is.EqualTo("Chapter 1"));
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
					File.WriteAllBytes(fileC1.Path, UTF8.GetBytes(ClipRepositoryCharacterFilterTests.kRiffWavHeader));

					string chapterInfoFilePath = Path.Combine(chapterFolder, ChapterInfo.kChapterInfoFilename);

					ChapterInfo info = CreateChapterInfo(kChapter);
					Assert.That(chapterInfoFilePath, Does.Not.Exist);

					var scriptBlock = new ScriptLine
					{
						Number = 1,
						Text = "Chapter 1",
						Heading = true
					};

					info.OnScriptBlockRecorded(scriptBlock);
					Assert.That(fileC1.Path, Does.Not.Exist);
					Assert.That(chapterInfoFilePath, Does.Not.Exist);
					Assert.That(info.Recordings.Count, Is.EqualTo(0));
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
					var scriptBlock = new ScriptLine
					{
						Number = 1,
						Text = "Chapter 1",
						Heading = true
					};
					info.OnScriptBlockRecorded(scriptBlock);

					scriptBlock = new ScriptLine
					{
						Number = 4,
						Text = "This is the first sentence in verse 1:3.",
						Heading = false
					};
					info.OnScriptBlockRecorded(scriptBlock);

					Assert.That(info.Recordings.Count, Is.EqualTo(2));
					Assert.That(info.Recordings[1].Number, Is.EqualTo(4));
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
					var scriptBlock = new ScriptLine
					{
						Number = 1,
						Text = "Chapter 1",
						Heading = true
					};
					info.OnScriptBlockRecorded(scriptBlock);

					scriptBlock = new ScriptLine
					{
						Number = 2,
						Text = "Verse 1.",
						Heading = false
					};
					info.OnScriptBlockRecorded(scriptBlock);

					scriptBlock = new ScriptLine
					{
						Number = 3,
						Text = "Verse 2.",
						Heading = false
					};
					info.OnScriptBlockRecorded(scriptBlock);

					scriptBlock = new ScriptLine
					{
						Number = 2,
						Text = "Changed text for verse 1.",
						Heading = false
					};
					info.OnScriptBlockRecorded(scriptBlock);

					Assert.That(info.Recordings.Select(r => r.Number), Is.EqualTo(new [] {1, 2, 3}));
					Assert.That(info.Recordings[1].Text, Is.EqualTo("Changed text for verse 1."));
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
					var scriptBlock = new ScriptLine
					{
						Number = 1,
						Text = "Chapter 1",
						Heading = true
					};
					info.OnScriptBlockRecorded(scriptBlock);

					scriptBlock = new ScriptLine
					{
						Number = 3,
						Text = "Verse 2.",
						Heading = false
					};
					info.OnScriptBlockRecorded(scriptBlock);

					scriptBlock = new ScriptLine
					{
						Number = 2,
						Text = "Verse 1.",
						Heading = false
					};
					info.OnScriptBlockRecorded(scriptBlock);

					Assert.That(info.Recordings.Select(r => r.Number),
						Is.EqualTo(new [] {1, 2, 3}));
					Assert.That(info.Recordings.Select(r => r.Text),
						Is.EqualTo(new [] {"Chapter 1", "Verse 1.", "Verse 2."}));
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
			Assert.That(info.CalculatePercentageRecorded(), Is.EqualTo(0));
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
			Assert.That(info.CalculatePercentageRecorded(), Is.EqualTo(27));
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
				Assert.That(info.CalculatePercentageRecorded(), Is.EqualTo(36));
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
				Assert.That(info.CalculatePercentageRecorded(), Is.EqualTo(0));
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
				Assert.That(info.CalculatePercentageRecorded(), Is.EqualTo(100));
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
			Assert.That(info.HasUnresolvedProblem, Is.False);
		}

		[Test]
		public void HasUnresolvedProblem_AllRecordingsMatch_ReturnsFalse()
		{
			const int kChapter = 2;
			ChapterInfo info = CreateChapterInfo(kChapter);
			info.Recordings.Add(_bookInfo.ScriptProvider.GetUnfilteredBlock(_bookInfo.BookNumber, kChapter, 0));
			info.Recordings.Add(_bookInfo.ScriptProvider.GetUnfilteredBlock(_bookInfo.BookNumber, kChapter, 1));
			Assert.That(info.HasUnresolvedProblem, Is.False);
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
			Assert.That(info.HasUnresolvedProblem, Is.True);
		}

		[Test]
		public void HasUnresolvedProblem_HasRecordingWithoutFileBeyondCurrentScript_ReturnsFalse()
		{
			var info = CreateChapterInfoWithOneExtraRecording();
			Assert.That(info.HasUnresolvedProblem, Is.Not.True);
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
				Assert.That(info.HasUnresolvedProblem, Is.True);
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
				Assert.That(extra.RecordingInfo, Is.EqualTo(info.Recordings.Last()));
				Assert.That(extra.ClipFile, Is.EqualTo(expectedPath));
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
				Assert.That(extras[i].RecordingInfo, Is.Null);
				Assert.That(extras[i].ClipFile, Is.EqualTo(expectedPaths[i++]));
				Assert.That(extras[i].RecordingInfo, Is.Null);
				Assert.That(extras[i].ClipFile, Is.EqualTo(expectedPaths[i++]));
				Assert.That(extras.Count, Is.EqualTo(i));
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
				Assert.That(extras[i].RecordingInfo, Is.Null);
				Assert.That(extras[i++].ClipFile, Is.EqualTo(expectedPaths[0]));
				Assert.That(extras[i].RecordingInfo.Text, Is.EqualTo("Last extra"));
				Assert.That(extras[i++].ClipFile, Is.EqualTo(expectedPaths[1]));
				Assert.That(extras.Count, Is.EqualTo(i));
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
				Assert.That(i, Is.EqualTo(_bookInfo.ScriptProvider.GetUnfilteredScriptBlockCount(
					_bookInfo.BookNumber, info.ChapterNumber1Based)));
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
				Assert.That(info.GetIndexOfNextUnfilteredBlockWithProblem(blockCount),
					Is.EqualTo(-1));
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
			Assert.That(_chapterInfoCreated, Is.Not.True,
				"This test is attempting to write a WAV file after creating the ChapterInfo. " +
				"You probably meant to call VerifyWavFile.");
			var path = Path.Combine(chapterFolder, $"{fileNumber}.wav");
			Assert.That(path, Does.Not.Exist, "This test is attempting to write a WAV file that " +
				$"already exists: {path}. Check to ensure that you are not accidentally " +
				"supplying a duplicate file number.");
			File.WriteAllBytes(path, UTF8.GetBytes(ClipRepositoryCharacterFilterTests.kRiffWavHeader + contents));
			return path;
		}

		private static void VerifyWavFile(string chapterFolder, int fileNumber, string contents)
		{
			var path = Path.Combine(chapterFolder, $"{fileNumber}.wav");
			Assert.That(File.ReadAllBytes(path), Is.EqualTo(
				UTF8.GetBytes(ClipRepositoryCharacterFilterTests.kRiffWavHeader + contents)));
		}
	}
}
