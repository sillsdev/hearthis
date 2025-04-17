﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;
using SIL.IO;
using SIL.Progress;
using SIL.Reporting;
using DateTime = System.DateTime;

namespace HearThisTests
{
	[TestFixture]
	public class ClipRepositoryTests
	{
		private const double kMonoSampleDuration = 0.062;
		private TestErrorReporter _errorReporter;

		private class TestPublishingModel : PublishingModel
		{
			public TestPublishingModel(VerseIndexFormatType verseIndexFormat) : base(new DummyInfoProvider())
			{
				AudioFormat = "megaVoice";
				VerseIndexFormat = verseIndexFormat;
				SetPublishingMethod();
			}
		}

		private class DummyInfoProvider : IPublishingInfoProvider
		{
			public readonly List<string> Verses = new List<string>();
			public readonly Dictionary<string, List<int>> VerseOffsets = new Dictionary<string, List<int>>();
			public readonly Dictionary<string, string> Text = new Dictionary<string, string>();
			public readonly List<string> BooksNotToPublish = new List<string>();
			public string Name => "Dummy";
			public string EthnologueCode => "xdum";
			public string CurrentBookName { get; set; }
			public bool Strict;

			public bool IncludeBook(string bookName)
			{
				return !BooksNotToPublish.Contains(bookName);
			}

			public ScriptLine GetUnfilteredBlock(string bookName, int chapterNumber, int lineNumber0Based)
			{
				var scriptLineNumber = lineNumber0Based + 1;
				bool heading;
				string verse = null;
				string headingType = null;
				if (lineNumber0Based < Verses.Count)
				{
					string verseNumberOrHeadingType = Verses[lineNumber0Based];
					if (verseNumberOrHeadingType[0] == 'v')
					{
						verse = verseNumberOrHeadingType.Substring(1);
						heading = false;
					}
					else if (verseNumberOrHeadingType.Length >= 2 && verseNumberOrHeadingType.Substring(0, 2) == "ip")
					{
						heading = false;
					}
					else
					{
						heading = true;
						headingType = verseNumberOrHeadingType;
					}
				}
				else
				{
					if (Strict)
						throw new ArgumentOutOfRangeException(nameof(lineNumber0Based));
					heading = true;
					headingType = "s";
				}

				var line = new ScriptLine
				{
					Number = scriptLineNumber,
					Verse = verse,
					Heading = heading,
					HeadingType = headingType,
				};
				if (verse != null && verse.Contains("~"))
				{
					if (VerseOffsets.TryGetValue(verse, out var offsets))
					{
						foreach (var offset in offsets)
							line.AddVerseOffset(offset);
					}

					if (Text.TryGetValue(verse, out var text))
						line.Text = text;
				}

				return line;
			}

			public IBibleStats VersificationInfo { get; } = new BibleStats();

			public int BookNameComparer(string x, string y)
			{
				return 0;
			}

			public bool BreakQuotesIntoBlocks => false;
			public string BlockBreakCharacters => ". ?";
			public bool HasProblemNeedingAttention(string bookName = null) => false;
		}

		[SetUp]
		public void SetUpFixture()
		{
			_errorReporter = new TestErrorReporter();
			ErrorReport.SetErrorReporter(_errorReporter);
		}

		/// <summary>
		/// regression
		/// </summary>
		[Test, Ignore("known to fail")]
		public void MergeAudioFiles_DifferentChannels_StillMerges()
		{
			using (var output = new TempFile())
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var stereo = TempFile.FromResource(Resource1._2Channel, ".wav"))
			{
				var filesToJoin = new List<string> {mono.Path, stereo.Path};
				var progress = new SIL.Progress.StringBuilderProgress();

				ClipRepository.MergeAudioFiles(filesToJoin, output.Path, progress);
				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(output.Path, Does.Exist);
			}
		}

		[Test]
		public void MergeAudioFiles_SingleFile_CopiedToOutputPath()
		{
			using (var output = new TempFile())
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var filesToJoin = new List<string> {mono.Path};
				var progress = new SIL.Progress.StringBuilderProgress();

				ClipRepository.MergeAudioFiles(filesToJoin, output.Path, progress);
				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(output.Path, Does.Exist);
				Assert.That(File.ReadAllBytes(output.Path),
					Is.EqualTo(File.ReadAllBytes(mono.Path)));
			}
		}

		/// <summary>
		/// This tests the case where there is a valid recording in a chapter folder
		/// </summary>
		[Test]
		public void HasRecordingsForProject_WavFileInChapterFolder_ReturnsTrue()
		{
			const string projectName = "Dummy";
			var pathToJohn1_1 = ClipRepository.GetPathToLineRecording(projectName, "John", 1, 1);
			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var fileInBackupFolder = TempFile.WithFilename(pathToJohn1_1))
				{
					File.Copy(mono.Path, fileInBackupFolder.Path, true);
					Assert.That(ClipRepository.HasRecordingsForProject(projectName), Is.True);
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		/// <summary>
		/// This tests the case where the only recordings are in a bogus (non-numeric) "chapter" folder
		/// </summary>
		[Test]
		public void HasRecordingsForProject_WavFilesOnlyInNonNumericFolderInBookFolder_ReturnsFalseWithNoError()
		{
			const string projectName = "Dummy";
			var pathToJohn1_1 = ClipRepository.GetPathToLineRecording(projectName, "John", 1, 1);
			var wavFilenameForJohn1_1 = Path.GetFileName(pathToJohn1_1);
			Assert.That(wavFilenameForJohn1_1, Is.Not.Null);
			var pathToBackupFolder = Path.GetDirectoryName(pathToJohn1_1) + "_Backup";
			Directory.CreateDirectory(pathToBackupFolder);
			var pathToBackupJohn1_1 = Path.Combine(pathToBackupFolder, wavFilenameForJohn1_1);
			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var fileInBackupFolder = TempFile.WithFilename(pathToBackupJohn1_1))
				{
					File.Copy(mono.Path, fileInBackupFolder.Path, true);
					Assert.That(ClipRepository.HasRecordingsForProject(projectName), Is.False);
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		/// <summary>
		/// This tests the case where chapter folder contains a bogus (non-numeric) WAV file
		/// </summary>
		[Test]
		public void GetCountOfRecordingsInFolder_WithActorCharacterProvider_WavFilesOnlyInNonNumericFolderInBookFolder_ReturnsCountOfValidWavFiles()
		{
			const string projectName = "Dummy";

			var pathToJohn1_1 = ClipRepository.GetPathToLineRecording(projectName, "John", 1, 1);
			var pathToJohn1Folder = Path.GetDirectoryName(pathToJohn1_1);
			Assert.That(pathToJohn1Folder, Is.Not.Null);
			var pathToNonNumericWavFile = Path.Combine(pathToJohn1Folder, "bogusness.wav");
			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var bogusFile = TempFile.WithFilename(pathToNonNumericWavFile))
				using (var fileInJohn = TempFile.WithFilename(pathToJohn1_1))
				{
					File.Copy(mono.Path, bogusFile.Path, true);
					File.Copy(mono.Path, fileInJohn.Path, true);
					var provider = new ClipFakeProvider();
					provider.SimulateBlockInCharacter(42, 1, 1, 1, "This is a monkey. Time to make some soup!");
					Assert.That(ClipRepository.GetCountOfRecordingsInFolder(pathToJohn1Folder, provider), Is.EqualTo(1));
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		/// <summary>
		/// This tests the case where the "chapter" folder passed in is bogus (non-numeric)
		/// </summary>
		[Test]
		public void GetCountOfRecordingsInFolder_WithActorCharacterProvider_BogusChapterFolder_ReturnsZero()
		{
			const string projectName = "Dummy";

			var pathToJohn1_1 = ClipRepository.GetPathToLineRecording(projectName, "John", 1, 1);
			var wavFilenameForJohn1_1 = Path.GetFileName(pathToJohn1_1);
			Assert.That(wavFilenameForJohn1_1, Is.Not.Null);
			var pathToBackupFolder = Path.GetDirectoryName(pathToJohn1_1) + "_Backup";
			Directory.CreateDirectory(pathToBackupFolder);
			try
			{
				var pathToBackupJohn1_1 = Path.Combine(pathToBackupFolder, wavFilenameForJohn1_1);
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var fileInBackupFolder = TempFile.WithFilename(pathToBackupJohn1_1))
				{
					File.Copy(mono.Path, fileInBackupFolder.Path, true);
					var provider = new ClipFakeProvider();
					provider.SimulateBlockInCharacter(42, 1, 1, 1, "This is a monkey. Time to make some soup!");
					Assert.That(ClipRepository.GetCountOfRecordingsInFolder(pathToBackupFolder, provider), Is.EqualTo(0));
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		[TestCase(null)]
		[TestCase("")]
		public void IsInvalidClipFile_NullOrEmpty_ThrowsArgumentNullException(string path)
		{
			Assert.That(() => ClipRepository.IsInvalidClipFile(path), Throws.ArgumentNullException);
		}

		[Test]
		public void IsInvalidClipFile_NonexistentFileWithProgress_LogsWarningAndReturnsTrue()
		{
			const string kFilename = "some_nonexistent_file.wav";
			var progress = new StringBuilderProgress();
			Assert.That(ClipRepository.IsInvalidClipFile(kFilename, progress), Is.True);
			Assert.That(progress.Text.Trim('\r', '\n'), Is.EqualTo(
				$"Warning: Attempted to check validity of nonexistent file: {kFilename}"));
		}

		[Test]
		public void IsInvalidClipFile_NonexistentFileWithoutProgress_ThrowsFileNotFoundException()
		{
			const string kFilename = "some_nonexistent_file.wav";
			Assert.That(() => ClipRepository.IsInvalidClipFile(kFilename),
				Throws.TypeOf<FileNotFoundException>().With.Property("FileName").EqualTo(kFilename));
		}

		/// <summary>
		/// This tests the case where some recordings are done for a book, but then the book is deleted (e.g., in Paratext)
		/// </summary>
		[Test]
		public void PublishAllBooks_RecordingsExistForMissingBook_MissingBookIsSkipped()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			var projectName = publishingInfoProvider.Name;
			var publishingModel = new PublishingModel(publishingInfoProvider)
			{
				AudioFormat = "megaVoice",
				PublishOnlyCurrentBook = false
			};
			publishingInfoProvider.BooksNotToPublish.Add("Proverbs");
			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var fileInProverbs = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(projectName, "Proverbs", 1, 1)))
				using (var fileInJohn = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(projectName, "John", 1, 1)))
				{
					File.Copy(mono.Path, fileInProverbs.Path, true);
					File.Copy(mono.Path, fileInJohn.Path, true);
					var progress = new SIL.Progress.StringBuilderProgress();
					publishingModel.Publish(progress);
					Assert.That(progress.ErrorEncountered, Is.False);
					Assert.That(publishingModel.FilesInput, Is.EqualTo(1));
					Assert.That(publishingModel.FilesOutput, Is.EqualTo(1));
					var megavoicePublishRoot = Path.Combine(publishingModel.PublishThisProjectPath, "MegaVoice");
					Assert.That(publishingModel.PublishingMethod.GetFilePathWithoutExtension(
						megavoicePublishRoot, "John", 1) + ".wav", Does.Exist);
					Assert.That(publishingModel.PublishingMethod.GetFilePathWithoutExtension(megavoicePublishRoot, "Proverbs", 1) + ".wav", Does.Not.Exist);
					// Encoding process actually trims off a byte for some reason (probably because it's garbage), so we can't simply compare
					// entire byte stream.
					var encodedFileContents =
						File.ReadAllBytes(publishingModel.PublishingMethod.GetFilePathWithoutExtension(megavoicePublishRoot, "John", 1) + ".wav");
					var originalFileContents = File.ReadAllBytes(mono.Path);
					Assert.That(encodedFileContents.Length, Is.EqualTo(originalFileContents.Length - 1));
					Assert.That(encodedFileContents, Is.EqualTo(originalFileContents.Take(encodedFileContents.Length)));
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(publishingModel.PublishThisProjectPath);
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		/// <summary>
		/// This tests the case where user has made a backup copy of a chapter folder
		/// </summary>
		[Test]
		public void PublishAllBooks_NonNumericFolderInBookFolder_ExtraFolderIsIgnored()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			var projectName = publishingInfoProvider.Name;
			var publishingModel = new PublishingModel(publishingInfoProvider)
			{
				AudioFormat = "megaVoice",
				PublishOnlyCurrentBook = false
			};
			var pathToJohn1_1 = ClipRepository.GetPathToLineRecording(projectName, "John", 1, 1);
			var wavFilenameForJohn1_1 = Path.GetFileName(pathToJohn1_1);
			Assert.That(wavFilenameForJohn1_1, Is.Not.Null);
			var pathToBackupFolder = Path.GetDirectoryName(pathToJohn1_1) + "_Backup";
			Directory.CreateDirectory(pathToBackupFolder);
			var pathToBackupJohn1_1 = Path.Combine(pathToBackupFolder, wavFilenameForJohn1_1);
			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var fileInBackupFolder = TempFile.WithFilename(pathToBackupJohn1_1))
				using (var fileInJohn = TempFile.WithFilename(pathToJohn1_1))
				{
					File.Copy(mono.Path, fileInBackupFolder.Path, true);
					File.Copy(mono.Path, fileInJohn.Path, true);
					var progress = new SIL.Progress.StringBuilderProgress();
					publishingModel.Publish(progress);
					Assert.That(progress.ErrorEncountered, Is.False);
					Assert.That(publishingModel.FilesInput, Is.EqualTo(1));
					Assert.That(publishingModel.FilesOutput, Is.EqualTo(1));
					var megavoicePublishRoot = Path.Combine(publishingModel.PublishThisProjectPath, "MegaVoice");
					Assert.That(publishingModel.PublishingMethod.GetFilePathWithoutExtension(
						megavoicePublishRoot, "John", 1) + ".wav", Does.Exist);
					// Encoding process actually trims off a byte for some reason (probably because it's garbage), so we can't simply compare
					// entire byte stream.
					var encodedFileContents =
						File.ReadAllBytes(publishingModel.PublishingMethod.GetFilePathWithoutExtension(megavoicePublishRoot, "John", 1) + ".wav");
					var originalFileContents = File.ReadAllBytes(mono.Path);
					Assert.That(encodedFileContents.Length, Is.EqualTo(originalFileContents.Length - 1));
					Assert.That(encodedFileContents, Is.EqualTo(originalFileContents.Take(encodedFileContents.Length)));
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(publishingModel.PublishThisProjectPath);
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		[Test]
		public void PublishCurrentBook_MoreClipsThanBlocksInChapterOne_WarningNotedInLog()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("c");
			publishingInfoProvider.Verses.Add("v1");
			publishingInfoProvider.Strict = true;
			publishingInfoProvider.CurrentBookName = "Philemon";
			var projectName = publishingInfoProvider.Name;
			var publishingModel = new PublishingModel(publishingInfoProvider)
			{
				AudioFormat = "megaVoice",
				PublishOnlyCurrentBook = true
			};
			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var filePhmC1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(projectName, "Philemon", 1, 0)))
				using (var filePhm1_1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(projectName, "Philemon", 1, 1)))
				using (var filePhm1_2 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(projectName, "Philemon", 1, 2)))
				{
					File.Copy(mono.Path, filePhmC1.Path, true);
					File.Copy(mono.Path, filePhm1_1.Path, true);
					File.Copy(mono.Path, filePhm1_2.Path, true);
					var progress = new SIL.Progress.StringBuilderProgress();
					publishingModel.Publish(progress);
					Assert.That(progress.ErrorEncountered, Is.False);
					Assert.That(progress.Text, Does.Contain("Unexpected clips were encountered in the folder for Philemon 1."));
					Assert.That(publishingModel.FilesInput, Is.EqualTo(3));
					Assert.That(publishingModel.FilesOutput, Is.EqualTo(1));
					var megavoicePublishRoot = Path.Combine(publishingModel.PublishThisProjectPath, "MegaVoice");
					Assert.That(publishingModel.PublishingMethod.GetFilePathWithoutExtension(
						megavoicePublishRoot, "Philemon", 1) + ".wav", Does.Exist);
					// Encoding process actually trims off a byte for some reason (probably because it's garbage), so we can't simply compare
					// entire byte stream.
					var encodedFileContents =
						File.ReadAllBytes(publishingModel.PublishingMethod.GetFilePathWithoutExtension(megavoicePublishRoot, "Philemon", 1) + ".wav");
					var originalFileContents = File.ReadAllBytes(mono.Path);
					Assert.That(encodedFileContents.Length, Is.GreaterThan(originalFileContents.Length * 2));
					Assert.That(encodedFileContents.Length, Is.LessThanOrEqualTo(originalFileContents.Length * 3));
				}
			}
			finally
			{
				Directory.Delete(publishingModel.PublishThisProjectPath, true);
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		[Test]
		public void PublishCurrentBook_InvalidWavFile_ErrorNotedInLog()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("c");
			publishingInfoProvider.Verses.Add("v1");
			publishingInfoProvider.Strict = true;
			publishingInfoProvider.CurrentBookName = "Philemon";
			var projectName = publishingInfoProvider.Name;
			var publishingModel = new PublishingModel(publishingInfoProvider)
			{
				AudioFormat = "megaVoice",
				PublishOnlyCurrentBook = true
			};
			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var filePhmC1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(projectName, "Philemon", 1, 0)))
				using (var filePhm1_1 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(projectName, "Philemon", 1, 1)))
				using (var filePhm1_2 = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(projectName, "Philemon", 1, 2)))
				{
					File.Copy(mono.Path, filePhmC1.Path, true);
					File.WriteAllBytes(filePhm1_1.Path, Encoding.UTF8.GetBytes(ClipRepositoryCharacterFilterTests.kRiffWavHeader));
					File.Copy(mono.Path, filePhm1_2.Path, true);
					var progress = new SIL.Progress.StringBuilderProgress();
					publishingModel.Publish(progress);
					Assert.That(progress.ErrorEncountered, Is.True);
					Assert.That(progress.Text, Does.Contain($"Invalid WAV file {filePhm1_1.Path}"));
					Assert.That(progress.Text, Does.Contain("HearThis will attempt to delete it."));
					Assert.That(publishingModel.FilesInput, Is.EqualTo(2));
					Assert.That(publishingModel.FilesOutput, Is.EqualTo(1));
					var megavoicePublishRoot = Path.Combine(publishingModel.PublishThisProjectPath, "MegaVoice");
					Assert.That(publishingModel.PublishingMethod.GetFilePathWithoutExtension(
						megavoicePublishRoot, "Philemon", 1) + ".wav", Does.Exist);
					// Encoding process actually trims off a byte for some reason (probably because it's garbage), so we can't simply compare
					// entire byte stream.
					var encodedFileContents =
						File.ReadAllBytes(publishingModel.PublishingMethod.GetFilePathWithoutExtension(megavoicePublishRoot, "Philemon", 1) + ".wav");
					var originalFileContents = File.ReadAllBytes(mono.Path);
					Assert.That(encodedFileContents.Length, Is.GreaterThan(originalFileContents.Length));
					Assert.That(encodedFileContents.Length, Is.LessThanOrEqualTo(originalFileContents.Length * 2));
				}
			}
			finally
			{
				Directory.Delete(publishingModel.PublishThisProjectPath, true);
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		[Test]
		public void PublishVerseIndexFiles_VerseIndexFormatNone_ExistingFileGetsDeleted()
		{
			var publishingModel = new TestPublishingModel(PublishingModel.VerseIndexFormatType.None);
			var rootPath = Path.GetTempPath();
			const string bookName = "Psalms";
			const int chapterNumber = 5;
			var outputPath = Path.ChangeExtension(
				publishingModel.PublishingMethod.GetFilePathWithoutExtension(rootPath, bookName, chapterNumber), "txt");
			using (new TempFile(outputPath, true))
			{
				var progress = new SIL.Progress.StringBuilderProgress();

				File.Create(outputPath).Close();
				Assert.That(outputPath, Does.Exist);
				ClipRepository.PublishVerseIndexFiles(rootPath, bookName, chapterNumber, new string[0], publishingModel, progress);
				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(outputPath, Does.Not.Exist);
			}
		}

		[Test]
		public void PublishVerseIndexFiles_AudacityLabelFileDoesNotExist_Created()
		{
			var publishingModel = new TestPublishingModel(PublishingModel.VerseIndexFormatType.AudacityLabelFileVerseLevel);
			var rootPath = Path.GetTempPath();
			const string bookName = "Psalms";
			const int chapterNumber = 5;
			var outputPath = Path.ChangeExtension(
				publishingModel.PublishingMethod.GetFilePathWithoutExtension(rootPath, bookName, chapterNumber), "txt");
			using (new TempFile(outputPath, false))
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			{
				File.Copy(mono.Path, file1.Path, true);
				var filesToJoin = new string[1];
				filesToJoin[0] = file1.Path;
				var progress = new SIL.Progress.StringBuilderProgress();

				Assert.That(outputPath, Does.Not.Exist);
				ClipRepository.PublishVerseIndexFiles(rootPath, bookName, chapterNumber, filesToJoin, publishingModel, progress);
				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(outputPath, Does.Exist);
				Assert.That(File.ReadAllText(outputPath).Length, Is.GreaterThan(0));
			}
		}

		[Test]
		public void PublishVerseIndexFiles_CueSheetDoesNotExist_Created()
		{
			var publishingModel = new TestPublishingModel(PublishingModel.VerseIndexFormatType.CueSheet);
			var rootPath = Path.GetTempPath();
			const string bookName = "Psalms";
			const int chapterNumber = 5;
			var outputPath = Path.ChangeExtension(
				publishingModel.PublishingMethod.GetFilePathWithoutExtension(rootPath, bookName, chapterNumber), "txt");
			using (new TempFile(outputPath, false))
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			{
				File.Copy(mono.Path, file1.Path, true);
				var filesToJoin = new string[1];
				filesToJoin[0] = file1.Path;
				var progress = new SIL.Progress.StringBuilderProgress();

				Assert.That(outputPath, Does.Not.Exist);
				ClipRepository.PublishVerseIndexFiles(rootPath, bookName, chapterNumber, filesToJoin, publishingModel, progress);
				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(outputPath, Does.Exist);
				Assert.That(File.ReadAllText(outputPath).Length, Is.GreaterThan(0));
			}
		}

		[Test]
		public void GetAudacityLabelFileContents_ConsecutiveVerseClipsChapterWithNoSectionHead_ContentsAreCorrect()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("c");
			publishingInfoProvider.Verses.Add("v1");
			publishingInfoProvider.Verses.Add("v2-3");
			publishingInfoProvider.Verses.Add("s");
			publishingInfoProvider.Verses.Add("v4");
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 5, "verse", ". ?");
				// Note: SAB does not (currently, as of 1.0 Beta 1) highlight the chapter number, but since unexpected labels
				// are ignored, I'm going to go ahead and include a "c" label for it. It makes for a more "complete" set of
				// labels and may be useful for some other purpose. Plus, it will be there if SAB ever decides it's important
				// enough to support.
				verifier.AddExpectedLine("c");
				verifier.AddExpectedLine("1");
				verifier.AddExpectedLine("2-3");
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("4");
				verifier.Verify();
			}
		}

		/// <summary>
		/// Synchronized highlighting for book introductions is now supported, but only with "phrase-level" option
		/// (since there are no verses in introductions).
		/// </summary>
		[Test]
		public void GetVerseIndexFileContents_ByVerseIntro_ReturnsNull()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("is");
			publishingInfoProvider.Verses.Add("ip");
			var model = new PublishingModel(publishingInfoProvider)
			{
				VerseIndexFormat = PublishingModel.VerseIndexFormatType.AudacityLabelFileVerseLevel
			};
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path};

				Assert.That(ClipRepository.GetVerseIndexFileContents("Psalms", 0, filesToJoin,
					model, "dummy.txt"), Is.Null);
			}
		}

		[Test]
		public void GetVerseIndexFileContents_ByPhraseIntro_ReturnsPhraseLabels()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("mt");
			publishingInfoProvider.Verses.Add("is");
			publishingInfoProvider.Verses.Add("is");
			publishingInfoProvider.Verses.Add("ip");
			publishingInfoProvider.Verses.Add("ip");
			// ENHANCE: What about other intro styles?

			var model = new PublishingModel(publishingInfoProvider)
			{
				VerseIndexFormat = PublishingModel.VerseIndexFormatType.AudacityLabelFilePhraseLevel
			};

			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path};

				var result = ClipRepository.GetVerseIndexFileContents("Psalms", 0, filesToJoin,
					model, "dummy.txt");
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 0, "phrase", ". ?");
				// Note: SAB does not (currently, as of 1.0 Beta 1) highlight the book title, but since unexpected labels
				// are ignored, I'm going to go ahead and include an "mt" label for it. It makes for a more "complete" set of
				// labels and may be useful for some other purpose. Plus, it will be there if SAB ever decides it's important
				// enough to support.
				verifier.AddExpectedLine("mt");
				verifier.AddExpectedLine("is1");
				verifier.AddExpectedLine("is2");
				verifier.AddExpectedLine("a");
				verifier.AddExpectedLine("b");
				verifier.Verify();
			}
		}

		[Test]
		public void GetAudacityLabelFileContents_SkippedClips_UnrecordedVersesNotLabeled()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("c"); // Skipped
			publishingInfoProvider.Verses.Add("s");
			publishingInfoProvider.Verses.Add("v1");
			publishingInfoProvider.Verses.Add("v2-3"); // Skipped
			publishingInfoProvider.Verses.Add("s"); // Skipped
			publishingInfoProvider.Verses.Add("v4");
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file5 = TempFile.WithFilename("5.wav"))
			{
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				var filesToJoin = new[] {file1.Path, file2.Path, file5.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 5, "verse", ". ?");
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("1");
				verifier.AddExpectedLine("4");
				verifier.Verify();
			}
		}

		[Test]
		public void GetAudacityLabelFileContents_MoreClipsThanBlocks_NoLabelsGeneratedForExtraClips()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("c"); // Skipped
			publishingInfoProvider.Verses.Add("s");
			publishingInfoProvider.Verses.Add("v1");
			publishingInfoProvider.Verses.Add("v2");
			publishingInfoProvider.Strict = true; // This prevents it from treating anything extra as a Heading block
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			{
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				var filesToJoin = new[] {file1.Path, file2.Path, file3.Path, file4.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 5, "verse", ". ?");
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("1");
				verifier.AddExpectedLine("2");
				verifier.Verify();
			}
		}

		[Test]
		public void GetAudacityLabelFileContents_ByVerseMultipleClipsPerVerse_LabelIndicatesStartOfFirstClipForEachVerse()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("s"); // 0
			publishingInfoProvider.Verses.Add("v1"); // 1
			publishingInfoProvider.Verses.Add("v1"); // 2
			publishingInfoProvider.Verses.Add("v2-3"); // 3
			publishingInfoProvider.Verses.Add("v2-3"); // 4
			publishingInfoProvider.Verses.Add("v2-3"); // 5
			publishingInfoProvider.Verses.Add("v3"); // 6 - This probably represents a mistake in the text
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			using (var file5 = TempFile.WithFilename("5.wav"))
			using (var file6 = TempFile.WithFilename("6.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				File.Copy(mono.Path, file6.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 5, "verse", ". ?");
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine(2, "1");
				verifier.AddExpectedLine(3, "2-3");
				verifier.AddExpectedLine("3");
				verifier.Verify();
			}
		}

		/// <summary>
		/// HT-200
		/// </summary>
		[Test]
		public void GetAudacityLabelFileContents_ByVerseMultipleClipsInVerseFollowedByImplicitBridge_TimingsDoNotOverlap()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("s"); // 0 (skipped)
			publishingInfoProvider.Verses.Add("v1"); // 1
			publishingInfoProvider.Verses.Add("v1"); // 2
			publishingInfoProvider.Text["1"] = "Yesu Kɩrsaẁ fɩ́rrǝ́ gùú cɔ̃mmã nã. Dũnnũẁ hlã-yǝ nǝ̃-mã ǹ-pɩgaa wù mããcieraaba nǝ̃ dìí mãã gɩ-ń ce fiyãã dɩ̀ ji hi, nã̀ã tĩɛ̃ wù mɛlɛgɛw wù mããcier Yuhanãw nãã, ǹ-ce mã́ fǝ̃ǝ̃.";
			publishingInfoProvider.Verses.Add("v2"); // 3
			publishingInfoProvider.Text["2"] = "Wù-lǝ wù pɩ́gará-dǝ gbaa, wà wu díɛ́ kuu wo kuuw mãã, Dũnnũ cãwan-i, nǝ̃ Yesu Kɩrsa siɛrnãsǝri.";
			publishingInfoProvider.Verses.Add("v3"); // 4
			publishingInfoProvider.Text["3"] = "Wùú mãã wù kal sɛbɛ wáà, nǝ̃ bàá mãã bà nɔ̃ Dũnnũ tanhiil'n cãwanĩɛ̃ gáà, nã̀ã ba bà blǝ dìí mãã dɩ̀ yállá, bà yunɲã̀ dwállá, nã̀ã wa dɩ̀ bǝ̃ǝ̃gɩ̀ píɛlá nã!";
			publishingInfoProvider.Verses.Add("v4~5"); // 5
			publishingInfoProvider.Text["4~5"] = "Sɛbɛ wáà yállá ǹ-hlǝ Yuhanãw nã-i, ǹ-hã Igɩlisɩ taa'n kurɔn nɩrhǝ̃ǝ̃lw mãã Asii yiɛgu: Dũnnũw mãã wù yáá dìɛ, nã̀ã ba dìɛ, nã̀ã ba wù ga jo, bá nǝ̃ yuflaa nɩrhǝ̃ǝ̃lw mãã wù yuuntasǝ'n guujirakuu'n yigagɩ nã, bá ne ɲì yiɛgu, nã̀ã hã-ɲã́ã̀ nǝ̃ hyasɩrãgǝgu, bá nǝ̃ Yesu Kɩrsaw, wùú mãã sulamntiiw mãã yalntiiw, nǝ̃ wuudĩɛ̃lw ǹ-hlǝ kuugɩ nã, nǝ̃ ɲũũrũũ yuntaa'n yuuntiiw!";
			var lengthOfVerse45 = publishingInfoProvider.Text["4~5"].Length;
			var offsetOfVerse5 = publishingInfoProvider.Text["4~5"].IndexOf("bá nǝ̃ Yesu Kɩrsaw", StringComparison.InvariantCulture);
			publishingInfoProvider.VerseOffsets["4~5"] = new List<int>(new[] {offsetOfVerse5});
			publishingInfoProvider.Verses.Add("v5~6"); // 6
			publishingInfoProvider.Text["5~6"] = "Yì cɔ̃mmã̀ mãã mã̀ dwal wùú nã, wù ca yì nã ǹ-pyar yì gaacĩɛ̃yaga yì yuunã nǝ̃ wù twammã, nã̀ã ce yí ba yuntaaba, nǝ̃ puruɔntaaba wù Tǝ Dũnnũw saa; wù buusammã nǝ̃ fãngããgɩ́ ba wù saa fuwɔ fɩraa!";
			var lengthOfVerse56 = publishingInfoProvider.Text["5~6"].Length;
			var offsetOfVerse6 = publishingInfoProvider.Text["5~6"].IndexOf("nã̀ã ce yí ba yuntaaba", StringComparison.InvariantCulture);
			publishingInfoProvider.VerseOffsets["5~6"] = new List<int>(new[] {offsetOfVerse6});
			publishingInfoProvider.Verses.Add("v6"); // 7
			publishingInfoProvider.Text["6"] = "Kwaya!";
			publishingInfoProvider.Verses.Add("v7"); // 8
			publishingInfoProvider.Verses.Add("v7"); // 9
			publishingInfoProvider.Verses.Add("v7"); // 10
			publishingInfoProvider.Text["7"] = "Níyà, wùú jówà yiilunɲã nã nnĩĩ. Cwaaba min, bà ji da-yǝ, halle bàá nǝ̃n flã ǹ-kǝ̃ǝ̃l-yǝ, nǝ̃ cuu coplaaga min, gà ji pɩpɩrĩɛ̃ gà yunɲã nã ǹ-ba bà kaal wù cɔ̃mmã nã. Ũwũũ, kwaya!";
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				//using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			using (var file5 = TempFile.WithFilename("5.wav"))
			using (var file6 = TempFile.WithFilename("6.wav"))
			using (var file7 = TempFile.WithFilename("7.wav"))
			using (var file8 = TempFile.WithFilename("8.wav"))
			using (var file9 = TempFile.WithFilename("9.wav"))
			using (var file10 = TempFile.WithFilename("10.wav"))
			{
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				File.Copy(mono.Path, file6.Path, true);
				File.Copy(mono.Path, file7.Path, true);
				File.Copy(mono.Path, file8.Path, true);
				File.Copy(mono.Path, file9.Path, true);
				File.Copy(mono.Path, file10.Path, true);
				var filesToJoin = new[] {file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path, file7.Path, file8.Path, file9.Path, file10.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 5, "verse", ". ?");
				verifier.AddExpectedLine(2, "1");
				verifier.AddExpectedLine("2");
				verifier.AddExpectedLine("3");
				var percentageOfVerses45BelongingToVerse4 = (double)offsetOfVerse5 / lengthOfVerse45;
				var percentageOfVerses45BelongingToVerse5 = 1 - percentageOfVerses45BelongingToVerse4;
				var percentageOfVerses56BelongingToVerse5 = (double)offsetOfVerse6 / lengthOfVerse56;
				var percentageOfVerses56BelongingToVerse6 = 1 - percentageOfVerses56BelongingToVerse5;
				verifier.AddExpectedLine(kMonoSampleDuration * percentageOfVerses45BelongingToVerse4, "4");
				verifier.AddExpectedLine(kMonoSampleDuration * (percentageOfVerses45BelongingToVerse5 + percentageOfVerses56BelongingToVerse5), "5");
				verifier.AddExpectedLine(kMonoSampleDuration * (1 + percentageOfVerses56BelongingToVerse6), "6");
				verifier.AddExpectedLine(3, "7");
				verifier.Verify();
			}
		}

		// TODO: Write test (and check behavior in SAB) for case where a section head occurs in the middle of 1 Cor 12:31

		[Test]
		public void GetAudacityLabelFileContents_ByVerseBlocksCrossVerses_LabelsBasedOnEstimatedPositionForEachVerse()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("s"); // 0
			publishingInfoProvider.Verses.Add("v1"); // 1
			publishingInfoProvider.Verses.Add("v1~2"); // 2
			publishingInfoProvider.VerseOffsets["1~2"] = new List<int>(new[] {30});
			publishingInfoProvider.Text["1~2"] = "012345678 012345678 012345678 023456789."; // verse 2 occurs 3/4 of the way through the text of the sentence.
			publishingInfoProvider.Verses.Add("v2~3-4"); // 3 (sentence starts in verse 2 and continues into explicit bridge 3-4)
			publishingInfoProvider.VerseOffsets["2~3-4"] = new List<int>(new[] {20});
			publishingInfoProvider.Text["2~3-4"] = "012345678 012345678 023456789."; // verse bridge 3-4 occurs 2/3 of the way through the text of the sentence.
			publishingInfoProvider.Verses.Add("v3-4"); // 4
			publishingInfoProvider.Verses.Add("v3-4"); // 5
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			using (var file5 = TempFile.WithFilename("5.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 5, "verse", ". ?");
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine(kMonoSampleDuration * 1.75, "1"); // All of verse 1 and 3/4 of verse 2.
				verifier.AddExpectedLine(kMonoSampleDuration * (.25 + 2.0 / 3), "2"); // Final 1/4 of verse 2 + 2/3 of verse 3-4
				verifier.AddExpectedLine(kMonoSampleDuration * (2 + 1.0 / 3), "3-4");
				verifier.Verify();
			}
		}

		[Test]
		public void GetAudacityLabelFileContents_ByPhraseMultipleClipsPerVerse_LabelIndicatesStartOfFirstClipForEachVerse()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("s"); // 0
			publishingInfoProvider.Verses.Add("v1"); // 1
			publishingInfoProvider.Verses.Add("v1"); // 2
			publishingInfoProvider.Verses.Add("v2"); // 3
			publishingInfoProvider.Verses.Add("v2~3"); // 4 (bridge is not explicitly in text. Sentence just crosses verse break.)
			publishingInfoProvider.VerseOffsets["2~3"] = new List<int>(new[] {20});
			publishingInfoProvider.Text["2~3"] = "012345678 012345678 023456789."; // verse 3 occurs 2/3 of the way through the text of the sentence.
			publishingInfoProvider.Verses.Add("v3"); // 5
			publishingInfoProvider.Verses.Add("v3"); // 6
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			using (var file5 = TempFile.WithFilename("5.wav"))
			using (var file6 = TempFile.WithFilename("6.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				File.Copy(mono.Path, file6.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, true);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 5, "phrase", ". ?");
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("1a");
				verifier.AddExpectedLine("1b");
				verifier.AddExpectedLine("2a");
				verifier.AddExpectedLine(kMonoSampleDuration * 2 / 3, "2b"); // Ideally, I think we want 2b-3a, but SAB doesn't support this (yet)
				verifier.AddExpectedLine(kMonoSampleDuration / 3, "3a");
				verifier.AddExpectedLine("3b");
				verifier.AddExpectedLine("3c");
				verifier.Verify();
			}
		}


		[Test]
		public void GetAudacityLabelFileContents_ByPhraseMultipleClipsInExplicitAndImplicitVerseBridges_LabelIndicatesPhrasesInVerseBridges()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("s"); // 0
			publishingInfoProvider.Verses.Add("v1"); // 1
			publishingInfoProvider.Verses.Add("v2-4"); // 2
			publishingInfoProvider.Verses.Add("v2-4"); // 3
			publishingInfoProvider.Verses.Add("v2-4"); // 4
			publishingInfoProvider.Verses.Add("v2-4~5~6"); // 5 Unlikely scenario: Sentence starts in bridge but continues into following verses
			publishingInfoProvider.VerseOffsets["2-4~5~6"] = new List<int>(new[] {10, 20}); // Verse 5 starts 25% of the way through the text
			publishingInfoProvider.Text["2-4~5~6"] = "123456789 123456789 123456789 123456789."; // and verse 6 starts 50% of the way through the text
			publishingInfoProvider.Verses.Add("v6~7"); // 6
			publishingInfoProvider.VerseOffsets["6~7"] = new List<int>(new[] {10}); // Verse 7 starts 50% of the way through the text
			publishingInfoProvider.Text["6~7"] = "123456789 123456789.";
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			using (var file5 = TempFile.WithFilename("5.wav"))
			using (var file6 = TempFile.WithFilename("6.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				File.Copy(mono.Path, file6.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, true);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 5, "phrase", ". ?");
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("1");
				verifier.AddExpectedLine("2-4a");
				verifier.AddExpectedLine("2-4b");
				verifier.AddExpectedLine("2-4c");
				verifier.AddExpectedLine(kMonoSampleDuration / 4, "2-4d"); // Ideally, we want 2-4d-6 (???), but SAB doesn't support this (yet)
				verifier.AddExpectedLine(kMonoSampleDuration / 4, "5");
				verifier.AddExpectedLine(kMonoSampleDuration / 2, "6a");
				verifier.AddExpectedLine(kMonoSampleDuration / 2, "6b"); // Ideally, we want 6b-7 (???), but SAB doesn't support this (yet)
				verifier.AddExpectedLine(kMonoSampleDuration / 2, "7");
				verifier.Verify();
			}
		}

		[Test]
		public void GetAudacityLabelFileContents_ByVerseConsecutiveHeadingClips_HeadingsEnumerated()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("s1");
			publishingInfoProvider.Verses.Add("s2");
			publishingInfoProvider.Verses.Add("s3");
			publishingInfoProvider.Verses.Add("v1-2");
			publishingInfoProvider.Verses.Add("s2");
			publishingInfoProvider.Verses.Add("s4");
			publishingInfoProvider.Verses.Add("v3");
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav")) // Heading 1a
			using (var file1 = TempFile.WithFilename("1.wav")) // Heading 1b
			using (var file2 = TempFile.WithFilename("2.wav")) // Heading 1c
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav")) // Heading 2a
			using (var file5 = TempFile.WithFilename("5.wav")) // Heading 2b
			using (var file6 = TempFile.WithFilename("6.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				File.Copy(mono.Path, file6.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 5, "verse", ". ?");
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("s2");
				verifier.AddExpectedLine("s3");
				verifier.AddExpectedLine("1-2");
				verifier.AddExpectedLine("s4");
				verifier.AddExpectedLine("s5");
				verifier.AddExpectedLine("3");
				verifier.Verify();
			}
		}

		/// <summary>
		/// Per comment by Richard Margetts:
		/// "At the moment, there is no phrase-splitting within sub-headings.
		/// But if someone asks for it, it will not be difficult to add."
		/// </summary>
		[Test]
		public void GetAudacityLabelFileContents_ByPhraseConsecutiveHeadingClips_HeadingsEnumerated()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("c");
			publishingInfoProvider.Verses.Add("s1");
			publishingInfoProvider.Verses.Add("s2");
			publishingInfoProvider.Verses.Add("s3");
			publishingInfoProvider.Verses.Add("v1-2");
			publishingInfoProvider.Verses.Add("s2");
			publishingInfoProvider.Verses.Add("s4");
			publishingInfoProvider.Verses.Add("v3");
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav")) // Chapter
			using (var file1 = TempFile.WithFilename("1.wav")) // Heading 1a
			using (var file2 = TempFile.WithFilename("2.wav")) // Heading 1b
			using (var file3 = TempFile.WithFilename("3.wav")) // Heading 1c
			using (var file4 = TempFile.WithFilename("4.wav"))
			using (var file5 = TempFile.WithFilename("5.wav")) // Heading 2a
			using (var file6 = TempFile.WithFilename("6.wav")) // Heading 2b
			using (var file7 = TempFile.WithFilename("7.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				File.Copy(mono.Path, file6.Path, true);
				File.Copy(mono.Path, file7.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path, file7.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Exodus", 5, true);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"EXO", 5, "phrase", ". ?");
				// Note: SAB does not (currently, as of 1.0 Beta 1) highlight the chapter number, but since unexpected labels
				// are ignored, I'm going to go ahead and include a "c" label for it. It makes for a more "complete" set of
				// labels and may be useful for some other purpose. Plus, it will be there if SAB ever decides it's important
				// enough to support.
				verifier.AddExpectedLine("c");
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("s2");
				verifier.AddExpectedLine("s3");
				verifier.AddExpectedLine("1-2");
				verifier.AddExpectedLine("s4");
				verifier.AddExpectedLine("s5");
				verifier.AddExpectedLine("3");
				verifier.Verify();
			}
		}

		/// <summary>
		/// Per comment by Richard Margetts:
		/// "At the moment, there is no phrase-splitting within sub-headings.
		/// But if someone asks for it, it will not be difficult to add."
		/// </summary>
		[Test]
		public void GetAudacityLabelFileContents_ByPhraseDifferentHeadingTypes_HeadingsEnumeratedByType()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("mt");
			publishingInfoProvider.Verses.Add("c");
			publishingInfoProvider.Verses.Add("ms");
			publishingInfoProvider.Verses.Add("s");
			publishingInfoProvider.Verses.Add("d");
			publishingInfoProvider.Verses.Add("v1");
			publishingInfoProvider.Verses.Add("v2");
			publishingInfoProvider.Verses.Add("sp");
			publishingInfoProvider.Verses.Add("v3");
			publishingInfoProvider.Verses.Add("s2");
			publishingInfoProvider.Verses.Add("s3");
			publishingInfoProvider.Verses.Add("v4");
			publishingInfoProvider.Verses.Add("sp");
			publishingInfoProvider.Verses.Add("v5");
			publishingInfoProvider.Verses.Add("ms3");
			publishingInfoProvider.Verses.Add("v6");
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav")) // mt -> mt
			using (var file1 = TempFile.WithFilename("1.wav")) // c  -> c
			using (var file2 = TempFile.WithFilename("2.wav")) // ms -> ms1
			using (var file3 = TempFile.WithFilename("3.wav")) // s  -> s1
			using (var file4 = TempFile.WithFilename("4.wav")) // d  -> d1
			using (var file5 = TempFile.WithFilename("5.wav")) // v1 -> 1
			using (var file6 = TempFile.WithFilename("6.wav")) // v2 -> 2
			using (var file7 = TempFile.WithFilename("7.wav")) // sp -> sp1
			using (var file8 = TempFile.WithFilename("8.wav")) // v3 -> 3
			using (var file9 = TempFile.WithFilename("9.wav")) // s2 -> s2
			using (var file10 = TempFile.WithFilename("10.wav")) // s3 -> s3
			using (var file11 = TempFile.WithFilename("11.wav")) // v4 -> 4
			using (var file12 = TempFile.WithFilename("12.wav")) // sp -> sp2
			using (var file13 = TempFile.WithFilename("13.wav")) // v5 -> 5
			using (var file14 = TempFile.WithFilename("14.wav")) // ms -> ms2
			using (var file15 = TempFile.WithFilename("15.wav")) // v6 -> 6
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				File.Copy(mono.Path, file6.Path, true);
				File.Copy(mono.Path, file7.Path, true);
				File.Copy(mono.Path, file8.Path, true);
				File.Copy(mono.Path, file9.Path, true);
				File.Copy(mono.Path, file10.Path, true);
				File.Copy(mono.Path, file11.Path, true);
				File.Copy(mono.Path, file12.Path, true);
				File.Copy(mono.Path, file13.Path, true);
				File.Copy(mono.Path, file14.Path, true);
				File.Copy(mono.Path, file15.Path, true);
				var filesToJoin = new[]
				{
					file0.Path, file1.Path, file2.Path, file3.Path, file4.Path,
					file5.Path, file6.Path, file7.Path, file8.Path, file9.Path,
					file10.Path, file11.Path, file12.Path, file13.Path, file14.Path,
					file15.Path
				};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 1, true);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration,
					"PSA", 1, "phrase", ". ?");
				// Note: SAB does not (currently, as of 1.0 Beta 1) highlight the main title or chapter numbers, but since
				// unexpected labels are ignored, I'm going to go ahead and include a labels for these. This makes for a more
				// "complete" set of labels and may be useful for some other purpose. Plus, they will be there if SAB ever
				// decides it's important enough to support.
				verifier.AddExpectedLine("mt");
				verifier.AddExpectedLine("c");
				verifier.AddExpectedLine("ms1");
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("d1");
				verifier.AddExpectedLine("1");
				verifier.AddExpectedLine("2");
				verifier.AddExpectedLine("sp1");
				verifier.AddExpectedLine("3");
				verifier.AddExpectedLine("s2");
				verifier.AddExpectedLine("s3");
				verifier.AddExpectedLine("4");
				verifier.AddExpectedLine("sp2");
				verifier.AddExpectedLine("5");
				verifier.AddExpectedLine("ms2");
				verifier.AddExpectedLine("6");
				verifier.Verify();
			}
		}


		[Test]
		[Ignore("Cue sheet implementation could not be completed due to lack of design/use case")]
		public void GetCueSheetContents_ConsecutiveClipsTwoHeadingThreeVerses_ContentsAreCorrect()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("v2-3");
			publishingInfoProvider.Verses.Add("s");
			publishingInfoProvider.Verses.Add("v4");
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path};

				var result = ClipRepository.GetCueSheetContents(filesToJoin, publishingInfoProvider, "Psalms", 5, "PSA5.wav");
				var lines = result.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
				Assert.That(lines, Is.EqualTo(new []
				{
					"FILE \"PSA5.wav\"",
					"TRACK 01 AUDIO",
					"  TITLE \"psa005-xdum-s1\"",
					"  INDEX 01 00:00:00",
					"TRACK 02 AUDIO",
					"  TITLE \"00000-psa005-xdum-V001\"",
					"  INDEX 01 00:00:05",
					"TRACK 03 AUDIO",
					"  TITLE \"00000-psa005-xdum-V002-003\"",
					"  INDEX 01 00:00:09",
					"0.062\t0.124\t1",
					"0.124\t0.186\t2-3",
					"0.186\t0.248\ts2",
					"0.248\t0.31\t4"
				}));
			}
		}

		[Test]
		[Ignore("Cue sheet implementation could not be completed due to lack of design/use case")]
		public void GetCueSheetContents_SkippedClips_UnrecordedVersesNotLabeled()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("v2-3"); // Skipped
			publishingInfoProvider.Verses.Add("s"); // Skipped
			publishingInfoProvider.Verses.Add("v4");
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file4.Path};

				var result = ClipRepository.GetCueSheetContents(filesToJoin, publishingInfoProvider, "Psalms", 5, "PSA5.wav");
				var lines = result.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
				Assert.That(lines, Is.EqualTo(new []
				{
					"FILE \"PSA5.wav\"",
					"0\t0.062\ts1",
					"0.062\t0.124\t1",
					"0.124\t0.186\t4"
				}));
			}
		}

		[Test]
		[Ignore("Cue sheet implementation could not be completed due to lack of design/use case")]
		public void GetCueSheetContents_MultipleClipsPerVerse_LabelIndicatesStartOfFirstClipForEachVerse()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("v1");
			publishingInfoProvider.Verses.Add("v2-3");
			publishingInfoProvider.Verses.Add("v2-3");
			publishingInfoProvider.Verses.Add("v2-3");
			publishingInfoProvider.Verses.Add("v3"); // REVIEW: Desired result not yet certain
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			using (var file2 = TempFile.WithFilename("2.wav"))
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav"))
			using (var file5 = TempFile.WithFilename("5.wav"))
			using (var file6 = TempFile.WithFilename("6.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				File.Copy(mono.Path, file6.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path};

				var result = ClipRepository.GetCueSheetContents(filesToJoin, publishingInfoProvider, "Psalms", 5, "PSA5.wav");
				var lines = result.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
				Assert.That(lines, Is.EqualTo(new []
				{
					"FILE \"PSA5.wav\"",
					"0\t0.062\ts1",
					"0.062\t0.186\t1", // Includes 2 clips
					"0.186\t0.372\t2-3", // Includes 3 clips
					"0.372\t0.434\t3"
				}));
			}
		}

		[Test]
		[Ignore("Cue sheet implementation could not be completed due to lack of design/use case")]
		public void GetCueSheetContents_ConsecutiveHeadingClips_HeadingsCoalesced()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("s1");
			publishingInfoProvider.Verses.Add("s2");
			publishingInfoProvider.Verses.Add("s2"); // Three consecutive headings
			publishingInfoProvider.Verses.Add("v1-2");
			publishingInfoProvider.Verses.Add("s2");
			publishingInfoProvider.Verses.Add("s4"); // Two consecutive headings
			publishingInfoProvider.Verses.Add("v3");
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav")) // Heading 1a
			using (var file1 = TempFile.WithFilename("1.wav")) // Heading 1b
			using (var file2 = TempFile.WithFilename("2.wav")) // Heading 1c
			using (var file3 = TempFile.WithFilename("3.wav"))
			using (var file4 = TempFile.WithFilename("4.wav")) // Heading 2a
			using (var file5 = TempFile.WithFilename("5.wav")) // Heading 2b
			using (var file6 = TempFile.WithFilename("6.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				File.Copy(mono.Path, file2.Path, true);
				File.Copy(mono.Path, file3.Path, true);
				File.Copy(mono.Path, file4.Path, true);
				File.Copy(mono.Path, file5.Path, true);
				File.Copy(mono.Path, file6.Path, true);
				var filesToJoin = new[] {file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path};

				var result = ClipRepository.GetCueSheetContents(filesToJoin, publishingInfoProvider, "Psalms", 5, "PSA5.wav");
				var lines = result.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
				Assert.That(lines, Is.EqualTo(new []
				{
					"FILE \"PSA5.wav\"",
					"0\t0.186\ts1", // Includes 3 clips
					"0.186\t0.248\t1-2",
					"0.248\t0.372\ts2", // Includes 2 clips
					"0.372\t0.434\t3"
				}));
			}
		}

		internal class AudacityLabelFileLineVerifier
		{
			internal class LabelLineInfo
			{
				public double ClipDuration { get; }
				public string ExpectedLabel { get; }

				internal LabelLineInfo(double clipDuration, string expectedLabel)
				{
					ClipDuration = clipDuration;
					ExpectedLabel = expectedLabel;
				}
			}

			private readonly double _sampleClipDuration;
			private readonly string _bookCode;
			private readonly int _chapter;
			private readonly string _level;
			private readonly string _separators;
			private readonly string[] _actualHeaderCommentLines;
			private readonly string[] _actualLabels;
			private readonly List<LabelLineInfo> _expectedLabelLines = new List<LabelLineInfo>();

			internal AudacityLabelFileLineVerifier(string actualFileContents, double sampleClipDuration,
				string bookCode = null, int chapter = 0, string level = null, string separators = null)
			{
				var actualLines = actualFileContents.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
				_actualHeaderCommentLines = actualLines.TakeWhile(l => l.StartsWith("\\")).ToArray();
				_actualLabels = actualLines.Skip(_actualHeaderCommentLines.Length).ToArray();
				_sampleClipDuration = sampleClipDuration;
				_bookCode = bookCode;
				_chapter = chapter;
				_level = level;
				_separators = separators;
			}

			internal void AddExpectedLine(string expectedLabel)
			{
				AddExpectedLine(1, expectedLabel);
			}

			internal void AddExpectedLine(int numberOfClips, string expectedLabel)
			{
				AddExpectedLine(numberOfClips * _sampleClipDuration, expectedLabel);
			}

			internal void AddExpectedLine(double clipDuration, string expectedLabel)
			{
				_expectedLabelLines.Add(new LabelLineInfo(clipDuration, expectedLabel));
			}

			internal void Verify()
			{
				if (_bookCode == null)
				{
					Assert.That(_actualHeaderCommentLines, Is.Empty,
						$"No header comments expected, but {_actualHeaderCommentLines.Length} lines of comments found:" +
						Environment.NewLine + string.Join(Environment.NewLine, _actualHeaderCommentLines));
				}
				else
				{
					var expectedHeaderLineCount = _level == "phrase" ? 4 : 3;
					if (expectedHeaderLineCount > _actualHeaderCommentLines.Length)
						Assert.Fail("Missing header comments");
					else if (expectedHeaderLineCount < _actualHeaderCommentLines.Length)
					{
						Assert.Fail("There were more header comments than expected:" +
							Environment.NewLine + string.Join(Environment.NewLine, _actualHeaderCommentLines));
					}

					Assert.That(_actualHeaderCommentLines[0], Is.EqualTo($"\\id {_bookCode}"));
					Assert.That(_actualHeaderCommentLines[1], Is.EqualTo($"\\c {_chapter}"));
					Assert.That(_actualHeaderCommentLines[2], Is.EqualTo($"\\level {_level}"));
					if (_level == "phrase")
						Assert.That(_actualHeaderCommentLines[3], Is.EqualTo($"\\separators {_separators}"));
				}

				Assert.That(_actualLabels.Length, Is.EqualTo(_expectedLabelLines.Count(l => l.ExpectedLabel != null)));
				double start = 0;
				int iActual = 0;
				for (int i = 0; i < _expectedLabelLines.Count; i++)
				{
					var fields = _actualLabels[iActual].Split('\t');
					Assert.That(fields.Length, Is.EqualTo(3),
						$"Bogus line ({i}): {_actualLabels[iActual]}");

					var end = start + _expectedLabelLines[i].ClipDuration;
					var expectedLabel = _expectedLabelLines[i].ExpectedLabel;
					if (expectedLabel != null)
					{
						var sStart = start.ToString("0.######", CultureInfo.InvariantCulture);
						var sEnd = end.ToString("0.######", CultureInfo.InvariantCulture);
						var failMsg = $"Line {iActual} was expected to go from {sStart} to {sEnd} and" +
							$" have label \"{expectedLabel}\", but was \"{_actualLabels[iActual]}\"";
						Assert.That(Math.Round(double.Parse(fields[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture), 6),
							Is.EqualTo(Math.Round(start, 6)), failMsg);
						Assert.That(Math.Round(double.Parse(fields[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture), 6),
							Is.EqualTo(Math.Round(end, 6)), failMsg);
						Assert.That(fields[2], Is.EqualTo(expectedLabel), failMsg);
						iActual++;
					}

					start = end;
				}
			}
		}

		[Test]
		public void AudacityLabelFileLineVerifier_Verify_Correct_NoException()
		{
			string actual = "0\t0.062\tc" + Environment.NewLine +
				"0.062\t0.186\t2-3" + Environment.NewLine +
				"0.186\t0.248\twhatever";
			var verifier = new AudacityLabelFileLineVerifier(actual, 0.062);
			verifier.AddExpectedLine(1, "c");
			verifier.AddExpectedLine(2, "2-3");
			verifier.AddExpectedLine(1, "whatever");
			verifier.Verify();
		}

		[Test]
		public void AudacityLabelFileLineVerifier_Verify_BogusLine_Fails()
		{
			string actual = "0\t0.062" + Environment.NewLine +
				"0.062\t0.186\t2-3" + Environment.NewLine +
				"0.186\t0.248\twhatever";
			var verifier = new AudacityLabelFileLineVerifier(actual, 0.062);
			verifier.AddExpectedLine(1, "c");
			verifier.AddExpectedLine(2, "2-3");
			verifier.AddExpectedLine(1, "whatever");
			var msg = Assert.Throws<AssertionException>(verifier.Verify).Message;
			Assert.That(msg.Trim(), Does.StartWith("Bogus line (0): 0\t0.062"));
		}

		[Test]
		public void AudacityLabelFileLineVerifier_Verify_IncorrectStartTime_Fails()
		{
			string actual = "0\t0.06\tc" + Environment.NewLine +
				"0.06\t0.180\t2-3" + Environment.NewLine +
				"0.181\t0.24\twhatever";
			var verifier = new AudacityLabelFileLineVerifier(actual, 0.06);
			verifier.AddExpectedLine(1, "c");
			verifier.AddExpectedLine(2, "2-3");
			verifier.AddExpectedLine(1, "whatever");
			var msg = Assert.Throws<AssertionException>(verifier.Verify).Message;
			Assert.That(msg.Trim(), Does.StartWith(
				"Line 2 was expected to go from 0.18 to 0.24 and have label \"whatever\", but was \"0.181\t0.24\twhatever\""));
		}

		[Test]
		public void AudacityLabelFileLineVerifier_Verify_IncorrectEndTime_Fails()
		{
			string actual = "0\t0.06\tc" + Environment.NewLine +
				"0.06\t0.143\t2-3" + Environment.NewLine +
				"0.143\t0.348\twhatever";
			var verifier = new AudacityLabelFileLineVerifier(actual, 0.06);
			verifier.AddExpectedLine(1, "c");
			verifier.AddExpectedLine(2, "2-3");
			verifier.AddExpectedLine(1, "whatever");
			var msg = Assert.Throws<AssertionException>(verifier.Verify).Message;
			Assert.That(msg.Trim(), Does.StartWith(
				"Line 1 was expected to go from 0.06 to 0.18 and have label \"2-3\", but was \"0.06\t0.143\t2-3\""));
		}

		[Test]
		public void AudacityLabelFileLineVerifier_Verify_IncorrectLabel_Fails()
		{
			string actual = "0\t0.06\tc" + Environment.NewLine +
				"0.06\t0.180\t2-3" + Environment.NewLine +
				"0.18\t0.24\ts1";
			var verifier = new AudacityLabelFileLineVerifier(actual, 0.06);
			verifier.AddExpectedLine(1, "c");
			verifier.AddExpectedLine(2, "2-3");
			verifier.AddExpectedLine(1, "whatever");
			var msg = Assert.Throws<AssertionException>(verifier.Verify).Message;
			Assert.That(msg.Trim(), Does.StartWith(
				"Line 2 was expected to go from 0.18 to 0.24 and have label \"whatever\", but was \"0.18\t0.24\ts1\""));
		}

		[Test]
		public void AudacityLabelFileLineVerifier_Verify_ExtraBlankLine_NoException()
		{
			string actual = "0\t0.062\tc" + Environment.NewLine +
				"0.062\t0.186\t2-3" + Environment.NewLine +
				"0.186\t0.248\twhatever" + Environment.NewLine;
			var verifier = new AudacityLabelFileLineVerifier(actual, 0.062);
			verifier.AddExpectedLine(1, "c");
			verifier.AddExpectedLine(2, "2-3");
			verifier.AddExpectedLine(1, "whatever");
			verifier.Verify();
		}

		[Test]
		public void AudacityLabelFileLineVerifier_Verify_MissingHeaderLines_Fails()
		{
			string actual = "0\t0.062\tc" + Environment.NewLine +
				"0.062\t0.186\t2-3" + Environment.NewLine +
				"0.186\t0.248\twhatever" + Environment.NewLine;
			var verifier = new AudacityLabelFileLineVerifier(actual, 0.062, "MAT", 1, "phrase", ". !");
			verifier.AddExpectedLine(1, "c");
			verifier.AddExpectedLine(2, "2-3");
			verifier.AddExpectedLine(1, "whatever");
			var msg = Assert.Throws<AssertionException>(verifier.Verify).Message;
			Assert.That(msg.Trim(), Does.StartWith("Missing header comments"));

		}

		#region // HT-376 (ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate)
		[Test]
		public void ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate_NoFiles_ReturnsTrue()
		{
			var testProject = TestContext.CurrentContext.Test.ID;
			const string kTestBook = "Matthew";
			const int kTestChapter = 1;
			var info = new TestChapterInfo();

			var chapterFolder = ClipRepository.GetChapterFolder(testProject, kTestBook, kTestChapter);
			try
			{
				// SUT
				Assert.That(ClipRepository.ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate(
					testProject, kTestBook, kTestChapter, 0, DateTime.UtcNow, () => info), Is.True);
				Assert.That(Directory.GetFiles(chapterFolder), Is.Empty);
				Assert.That(info.SaveCallCount, Is.EqualTo(0));
			}
			finally
			{
				CleanUpTestFolder(chapterFolder, testProject);
			}
		}

		[Test]
		public void ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate_AllFilesAreAtOrBeforeGivenBlock_NoFilesChangedReturnsTrue()
		{
			var cutoff = DateTime.UtcNow;
			var testProject = TestContext.CurrentContext.Test.ID;
			const string kTestBook = "Matthew";
			const int kTestChapter = 1;
			var chapterFolder = ClipRepository.GetChapterFolder(testProject, kTestBook, kTestChapter);
			var info = new TestChapterInfo(1, 2);
			try
			{
				var file0 = Path.Combine(chapterFolder, "0.wav");
				File.Create(file0).Close();
				var file1 = Path.Combine(chapterFolder, "1.wav");
				File.Create(file1).Close();
				// SUT
				Assert.That(ClipRepository.ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate(
					testProject, kTestBook, kTestChapter, 2, cutoff, () => info), Is.True);
				Assert.That(Directory.GetFiles(chapterFolder).Length, Is.EqualTo(2));
				Assert.That(file0, Does.Exist);
				Assert.That(file1, Does.Exist);
				Assert.That(info.SaveCallCount, Is.EqualTo(0));
			}
			finally
			{
				CleanUpTestFolder(chapterFolder, testProject);
			}
		}

		[TestCase(0)]
		[TestCase(1)]
		public void ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate_SomeButNotAllFilesModifiedAfterCutoffDate_NoFilesChangedReturnsFalse(int block)
		{
			var testProject = TestContext.CurrentContext.Test.ID;
			const string kTestBook = "Matthew";
			const int kTestChapter = 1;
			var chapterFolder = ClipRepository.GetChapterFolder(testProject, kTestBook, kTestChapter);
			var info = new TestChapterInfo(1, 2, 3);
			try
			{
				var file0 = Path.Combine(chapterFolder, "0.wav");
				File.Create(file0).Close();
				var file2 = Path.Combine(chapterFolder, "2.wav");
				File.Create(file2).Close();
				System.Threading.Thread.Sleep(1001); // file times are to the second.
				var cutoff = DateTime.UtcNow;
				var file1 = Path.Combine(chapterFolder, "1.wav");
				File.Create(file1).Close();
				// SUT
				Assert.That(ClipRepository.ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate(
					testProject, kTestBook, kTestChapter, block, cutoff, () => info), Is.False);
				Assert.That(Directory.GetFiles(chapterFolder).Length, Is.EqualTo(3));
				Assert.That(file0, Does.Exist);
				Assert.That(file1, Does.Exist);
				Assert.That(file2, Does.Exist);
				Assert.That(info.SaveCallCount, Is.EqualTo(0));
			}
			finally
			{
				CleanUpTestFolder(chapterFolder, testProject);
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate_AllFilesModifiedAfterCutoffDate_NoFilesChangedAndReturnsTrue(bool includeClip0)
		{
			var testProject = TestContext.CurrentContext.Test.ID;
			const string kTestBook = "Matthew";
			const int kTestChapter = 1;
			var chapterFolder = ClipRepository.GetChapterFolder(testProject, kTestBook, kTestChapter);
			var info = new TestChapterInfo(1, 2, 3);
			try
			{
				var file0 = Path.Combine(chapterFolder, "0.wav");
				if (includeClip0)
					File.Create(file0).Close();
				var cutoff = DateTime.UtcNow;
				var file2 = Path.Combine(chapterFolder, "2.wav");
				File.Create(file2).Close();
				var file1 = Path.Combine(chapterFolder, "1.wav");
				File.Create(file1).Close();
				// SUT
				Assert.That(ClipRepository.ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate(
					testProject, kTestBook, kTestChapter, 1, cutoff, () => info), Is.True);
				Assert.That(Directory.GetFiles(chapterFolder).Length, Is.EqualTo(includeClip0 ? 3 : 2));
				Assert.That(File.Exists(file0), Is.EqualTo(includeClip0));
				Assert.That(file1, Does.Exist);
				Assert.That(file2, Does.Exist);
				Assert.That(info.SaveCallCount, Is.EqualTo(0));
			}
			finally
			{
				CleanUpTestFolder(chapterFolder, testProject);
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate_AllFilesModifiedBeforeCutoffDate_FilesAfterBlockShifted(bool includeClip0)
		{
			var testProject = TestContext.CurrentContext.Test.ID;
			const string kTestBook = "Matthew";
			const int kTestChapter = 1;

			var chapterFolder = ClipRepository.GetChapterFolder(testProject, kTestBook, kTestChapter);
			ChapterRecordingInfoBase info;
			if (includeClip0)
				info = new TestChapterInfo(1, 2, 3, 8); // Intentionally omitted 4, just to make sure the logic is okay with having one missing.
			else
				info = new TestChapterInfo(2, 3, 8); // Intentionally omitted 4, just to make sure the logic is okay with having one missing.
			info.RecordingInfo[1].SkippedChanged += sender => { }; // code requires us to have a handler before we can set it.
			info.RecordingInfo[1].Skipped = true;

			try
			{
				File.Create(Path.Combine(chapterFolder, "2.skip")).Close();
				File.Create(Path.Combine(chapterFolder, "1.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "7.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "3.wav")).Close();
				System.Threading.Thread.Sleep(1001); // file times are to the second.
				var file0 = Path.Combine(chapterFolder, "0.wav");
				if (includeClip0)
					File.Create(file0).Close();
				// SUT
				Assert.That(ClipRepository.ShiftClipsAtOrAfterBlockIfAllClipsAreBeforeDate(
					testProject, kTestBook, kTestChapter, 1, DateTime.UtcNow, () => info), Is.True);
				Assert.That(Directory.GetFiles(chapterFolder).Length, Is.EqualTo(includeClip0 ? 5 : 4));
				Assert.That(Path.Combine(chapterFolder, "8.wav"), Does.Exist);
				Assert.That(Path.Combine(chapterFolder, "4.wav"), Does.Exist);
				Assert.That(Path.Combine(chapterFolder, "3.skip"), Does.Exist);
				Assert.That(Path.Combine(chapterFolder, "2.wav"), Does.Exist);
				Assert.That(Path.Combine(chapterFolder, "1.wav"), Does.Not.Exist);
				Assert.That(File.Exists(file0), Is.EqualTo(includeClip0));

				int i = 0;
				if (includeClip0)
				{
					Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(1), "Should not have incremented this one");
					Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 1"));
				}
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(3));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 2"));
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(4));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 3"));
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(9));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 8"));
				Assert.That(info.RecordingInfo.Count, Is.EqualTo(i));
			}
			finally
			{
				CleanUpTestFolder(chapterFolder, testProject);
			}
		}
		#endregion // HT-376

		#region HT-384
		
		[TestCase(true, true, true)]
		[TestCase(true, true, false)]
		[TestCase(true, false, true)]
		[TestCase(true, false, false)]
		[TestCase(false, true, true)]
		[TestCase(false, true, false)]
		[TestCase(false, false, true, 3)]
		[TestCase(false, false, false, 3)]
		public void ShiftClips_ThreeClipsAndSkipsWithInfoShiftedForward_CorrectClipsShiftedAndInfoUpdated(
			bool includeClip0, bool includeClip5, bool makeFile2Skip, int offset = 1)
		{
			ScriptLine.SkippedStyleInfoProvider = new FakeProvider();
			if (includeClip5)
				Assert.That(offset, Is.EqualTo(1));
			var testProject = TestContext.CurrentContext.Test.ID;
			const string kTestBook = "Matthew";
			const int kTestChapter = 1;

			var chapterFolder = ClipRepository.GetChapterFolder(testProject, kTestBook, kTestChapter);
			var clipsWithInfo = new List<int>(5);
			if (includeClip0)
				clipsWithInfo.Add(1);
			clipsWithInfo.Add(2);
			clipsWithInfo.Add(3);
			clipsWithInfo.Add(4);
			if (includeClip5)
				clipsWithInfo.Add(6);
			var info = new TestChapterInfo(clipsWithInfo.ToArray());
			var clip2Index = includeClip0 ? 2 : 1;
			var file2Ext = "wav";
			if (makeFile2Skip)
			{
				info.RecordingInfo[clip2Index].SkippedChanged += sender => { }; // code requires us to have a handler before we can set it.
				info.RecordingInfo[clip2Index].Skipped = true;
				file2Ext = "skip";
			}

			try
			{
				File.Create(Path.Combine(chapterFolder, $"2.{file2Ext}")).Close();
				File.Create(Path.Combine(chapterFolder, "1.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "3.wav")).Close();
				var file5 = Path.Combine(chapterFolder, "5.wav");
				if (includeClip5)
					File.Create(file5).Close();
				var file0 = Path.Combine(chapterFolder, "0.wav");
				if (includeClip0)
					File.Create(file0).Close();

				// SUT
				var result = ClipRepository.ShiftClips(testProject, kTestBook, kTestChapter, 1, 3, offset, () => info);
				Assert.That(result.SuccessfulMoves, Is.EqualTo(result.Attempted));
				Assert.That(result.Error, Is.Null);
				Assert.That(Directory.GetFiles(chapterFolder).Length, Is.EqualTo(clipsWithInfo.Count));
				Assert.That(Path.Combine(chapterFolder, $"{1 + offset}.wav"), Does.Exist);
				Assert.That(Path.Combine(chapterFolder, $"{2 + offset}.{file2Ext}"), Does.Exist);
				Assert.That(Path.Combine(chapterFolder, $"{3 + offset}.wav"), Does.Exist);
				Assert.That(File.Exists(file0), Is.EqualTo(includeClip0));
				Assert.That(File.Exists(file5),
					Is.EqualTo(includeClip5 || (offset == 2 || offset == 4 || (offset == 3 && !makeFile2Skip))));

				int i = 0;
				if (includeClip0)
				{
					Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(1), "Should not have incremented this one");
					Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 1"));
				}

				Assert.That(info.SaveCallCount, Is.EqualTo(1));
				
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(2 + offset));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 2"));
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(3 + offset));
				Assert.That(info.RecordingInfo[i].Skipped, Is.EqualTo(makeFile2Skip));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 3"));
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(4 + offset));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 4"));
				if (includeClip5)
				{
					Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(6), "Should not have incremented this one");
					Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 6"));
				}
				Assert.That(info.RecordingInfo.Count, Is.EqualTo(i));
			}
			finally
			{
				CleanUpTestFolder(chapterFolder, testProject);
			}
		}

		[Test]
		public void ShiftClips_TwoClipsWithInfoShiftedBackward_CorrectClipsShiftedAndInfoUpdated()
		{
			const int offset = -1;
			ScriptLine.SkippedStyleInfoProvider = new FakeProvider();
			var testProject = TestContext.CurrentContext.Test.ID;
			const string kTestBook = "Acts";
			const int kTestChapter = 1;

			var chapterFolder = ClipRepository.GetChapterFolder(testProject, kTestBook, kTestChapter);
			var clipsWithInfo = new List<int> { 2, 3, 4 };
			var info = new TestChapterInfo(clipsWithInfo.ToArray());

			try
			{
				File.Create(Path.Combine(chapterFolder, "2.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "1.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "3.wav")).Close();

				// SUT
				var result = ClipRepository.ShiftClips(testProject, kTestBook, kTestChapter, 1, 2, offset, () => info);
				Assert.That(result.SuccessfulMoves, Is.EqualTo(result.Attempted));
				Assert.That(result.Error, Is.Null);
				Assert.That(Directory.GetFiles(chapterFolder).Length, Is.EqualTo(clipsWithInfo.Count));
				Assert.That(Path.Combine(chapterFolder, $"{1 + offset}.wav"), Does.Exist);
				Assert.That(Path.Combine(chapterFolder, $"{2 + offset}.wav"), Does.Exist);
				Assert.That(Path.Combine(chapterFolder, "3.wav"), Does.Exist);

				Assert.That(info.SaveCallCount, Is.EqualTo(1));

				int i = 0;
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(2 + offset));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 2"));
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(3 + offset));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 3"));
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(4), "Should not have decremented this one");
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 4"));
				Assert.That(info.RecordingInfo.Count, Is.EqualTo(i));
			}
			finally
			{
				CleanUpTestFolder(chapterFolder, testProject);
			}
		}

		[TestCase(-1)]
		[TestCase(-2)]
		public void ShiftClips_FourClipsShiftedBackward_FirstOneMissingInfo_CorrectClipsShiftedAndExistingInfoNumbersUpdated(int offset)
		{
			ScriptLine.SkippedStyleInfoProvider = new FakeProvider();
			var testProject = TestContext.CurrentContext.Test.ID;
			const string kTestBook = "Acts";
			const int kTestChapter = 1;

			var chapterFolder = ClipRepository.GetChapterFolder(testProject, kTestBook, kTestChapter);
			var clipsWithInfo = new List<int> { 4, 5, 6 };
			var info = new TestChapterInfo(clipsWithInfo.ToArray());

			try
			{
				File.Create(Path.Combine(chapterFolder, "2.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "5.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "4.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "3.wav")).Close();

				// SUT
				var result = ClipRepository.ShiftClips(testProject, kTestBook, kTestChapter, 2, 4, offset, () => info);
				Assert.That(result.SuccessfulMoves, Is.EqualTo(result.Attempted));
				Assert.That(result.Error, Is.Null);
				Assert.That(Directory.GetFiles(chapterFolder).Length, Is.EqualTo(4));
				for (var f = 2; f <= 5; f++)
					Assert.That(Path.Combine(chapterFolder, $"{f + offset}.wav"), Does.Exist);

				Assert.That(info.SaveCallCount, Is.EqualTo(1));

				int i = 0;
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(4 + offset));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 4"));
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(5 + offset));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 5"));
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(6 + offset));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 6"));
				Assert.That(info.RecordingInfo.Count, Is.EqualTo(i));
			}
			finally
			{
				CleanUpTestFolder(chapterFolder, testProject);
			}
		}

		[TestCase(-1, 3)]
		[TestCase(-2, 4)] // 4 is more than exist, but should not produce an error
		public void ShiftClips_ThreeClipsShiftedBackward_LastTwoMissingInfo_CorrectClipsShiftedAndExistingInfoNumberUpdated(
			int offset, int count)
		{
			ScriptLine.SkippedStyleInfoProvider = new FakeProvider();
			var testProject = TestContext.CurrentContext.Test.ID;
			const string kTestBook = "John";
			const int kTestChapter = 1;

			var chapterFolder = ClipRepository.GetChapterFolder(testProject, kTestBook, kTestChapter);
			var clipsWithInfo = new List<int> { 3 };
			var info = new TestChapterInfo(clipsWithInfo.ToArray());

			try
			{
				File.Create(Path.Combine(chapterFolder, "2.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "4.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "3.wav")).Close();

				// SUT
				var result = ClipRepository.ShiftClips(testProject, kTestBook, kTestChapter, 2, count, offset, () => info);
				Assert.That(result.SuccessfulMoves, Is.EqualTo(3));
				Assert.That(result.Attempted, Is.EqualTo(3));
				Assert.That(result.Error, Is.Null);
				Assert.That(Directory.GetFiles(chapterFolder).Length, Is.EqualTo(3));
				for (var f = 2; f <= 4; f++)
					Assert.That(Path.Combine(chapterFolder, $"{f + offset}.wav"), Does.Exist);

				Assert.That(info.SaveCallCount, Is.EqualTo(1));

				var onlyRecordingInfo = info.RecordingInfo.Single();
				Assert.That(onlyRecordingInfo.Number, Is.EqualTo(3 + offset));
				Assert.That(onlyRecordingInfo.Text, Is.EqualTo("Line 3"));
			}
			finally
			{
				CleanUpTestFolder(chapterFolder, testProject);
			}
		}

		[TestCase(1, 1)]
		[TestCase(2, 2)]
		[TestCase(800, -3)] // Meaningless, but shouldn't hurt
		[TestCase(-2, 300)] // Meaningless, but shouldn't hurt
		public void ShiftClips_ZeroOffset_NoChanges(int start, int count)
		{
			const int offset = 0;
			ScriptLine.SkippedStyleInfoProvider = new FakeProvider();
			var testProject = TestContext.CurrentContext.Test.ID;
			const string kTestBook = "Acts";
			const int kTestChapter = 1;

			var chapterFolder = ClipRepository.GetChapterFolder(testProject, kTestBook, kTestChapter);
			var clipsWithInfo = new List<int> { 2, 3, 4 };
			var info = new TestChapterInfo(clipsWithInfo.ToArray());

			try
			{
				File.Create(Path.Combine(chapterFolder, "2.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "1.wav")).Close();
				File.Create(Path.Combine(chapterFolder, "3.wav")).Close();

				// SUT
				var result = ClipRepository.ShiftClips(testProject, kTestBook, kTestChapter, start, count, offset, () => info);
				Assert.That(result.SuccessfulMoves, Is.EqualTo(0));
				Assert.That(result.Attempted, Is.EqualTo(0));
				Assert.That(result.Error, Is.Null);
				Assert.That(Directory.GetFiles(chapterFolder).Length, Is.EqualTo(clipsWithInfo.Count));
				Assert.That(Path.Combine(chapterFolder, "1.wav"), Does.Exist);
				Assert.That(Path.Combine(chapterFolder, "2.wav"), Does.Exist);
				Assert.That(Path.Combine(chapterFolder, "3.wav"), Does.Exist);

				Assert.That(info.SaveCallCount, Is.EqualTo(0));

				int i = 0;
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(2));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 2"));
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(3));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 3"));
				Assert.That(info.RecordingInfo[i].Number, Is.EqualTo(4));
				Assert.That(info.RecordingInfo[i++].Text, Is.EqualTo("Line 4"));
				Assert.That(info.RecordingInfo.Count, Is.EqualTo(i));
			}
			finally
			{
				CleanUpTestFolder(chapterFolder, testProject);
			}
		}
		#endregion // HT-384

		#region RestoreBackedUpClip
		[Test]
		public void RestoreBackedUpClip_OnlySkipFileExists_ReturnsTrue()
		{
			const string projectName = "Dummy";
			const string book = "Acts";
			const int chapter = 3;
			const int line = 4;
			var pathToActs3_4Clip = ClipRepository.GetPathToLineRecording(projectName, book, chapter, line);
			var pathToActs3_4Skip = Path.ChangeExtension(pathToActs3_4Clip, ClipRepository.kSkipFileExtension);
			try
			{
				using (var clipContents = TempFile.FromResource(Resource1._1Channel, ".wav"))
				{
					RobustFile.Copy(clipContents.Path, pathToActs3_4Skip, true);
					Assert.That(ClipRepository.RestoreBackedUpClip(projectName, book, chapter, line), Is.True);
					Assert.That(pathToActs3_4Clip, Does.Exist);
					Assert.That(pathToActs3_4Skip, Does.Not.Exist);
					Assert.That(_errorReporter.ReportedProblems, Is.Empty);
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		[Test]
		public void RestoreBackedUpClip_OnlyClipFileExists_ReturnsFalse()
		{
			const string projectName = "Dummy";
			const string book = "Acts";
			const int chapter = 3;
			const int line = 4;
			var pathToActs3_4Clip = ClipRepository.GetPathToLineRecording(projectName, book, chapter, line);
			var pathToActs3_4Skip = Path.ChangeExtension(pathToActs3_4Clip, ClipRepository.kSkipFileExtension);
			try
			{
				using (var clipContents = TempFile.FromResource(Resource1._1Channel, ".wav"))
				{
					RobustFile.Copy(clipContents.Path, pathToActs3_4Clip, true);
					Assert.That(ClipRepository.RestoreBackedUpClip(projectName, book, chapter, line), Is.False);
					Assert.That(pathToActs3_4Clip, Does.Exist);
					Assert.That(pathToActs3_4Skip, Does.Not.Exist);
					Assert.That(_errorReporter.ReportedProblems, Is.Empty);
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		[Test]
		public void RestoreBackedUpClip_NeitherFileExists_ReturnsFalse()
		{
			const string projectName = "Dummy";
			const string book = "Acts";
			const int chapter = 3;
			const int line = 4;
			var pathToActs3_4Clip = ClipRepository.GetPathToLineRecording(projectName, book, chapter, line);
			var pathToActs3_4Skip = Path.ChangeExtension(pathToActs3_4Clip, ClipRepository.kSkipFileExtension);
			try
			{
				Assert.That(ClipRepository.RestoreBackedUpClip(projectName, book, chapter, line), Is.False);
				Assert.That(pathToActs3_4Clip, Does.Not.Exist);
				Assert.That(pathToActs3_4Skip, Does.Not.Exist);
				Assert.That(_errorReporter.ReportedProblems, Is.Empty);
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		#region Tests to deal with the unexpected scenarios where both clip and skip files exist.
		// Not sure how this can happen, but see HT-465 for background. 

		[Test]
		public void RestoreBackedUpClip_IdenticalClipAndSkipExist_ReturnsFalseAndRetainsOnlyClip()
		{
			const string projectName = "Dummy";
			const string book = "Acts";
			const int chapter = 3;
			const int line = 4;
			var pathToActs3_4Clip = ClipRepository.GetPathToLineRecording(projectName, book, chapter, line);
			var pathToActs3_4Skip = Path.ChangeExtension(pathToActs3_4Clip, ClipRepository.kSkipFileExtension);
			try
			{
				using (var clipContents = TempFile.FromResource(Resource1._1Channel, ".wav"))
				{
					RobustFile.Copy(clipContents.Path, pathToActs3_4Clip, true);
					RobustFile.Copy(clipContents.Path, pathToActs3_4Skip, true);
					Assert.That(ClipRepository.RestoreBackedUpClip(projectName, book, chapter, line), Is.False);
					Assert.That(pathToActs3_4Clip, Does.Exist);
					Assert.That(pathToActs3_4Skip, Does.Not.Exist);
					Assert.That(_errorReporter.ReportedProblems, Is.Empty);
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		[Test]
		public void RestoreBackedUpClip_ExistingClipNewerThanSkipFile_ReturnsFalseAndReportsNonFatalProblem()
		{
			const string projectName = "Dummy";
			const string book = "Acts";
			const int chapter = 3;
			const int line = 4;
			var pathToActs3_4Clip = ClipRepository.GetPathToLineRecording(projectName, book, chapter, line);
			var pathToActs3_4Skip = Path.ChangeExtension(pathToActs3_4Clip, ClipRepository.kSkipFileExtension);
			var backedUpSkipFile = pathToActs3_4Skip + "old.wav";
			try
			{
				using (var skipContents = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var clipContents = TempFile.FromResource(Resource1._2Channel, ".wav"))
				{
					RobustFile.Copy(skipContents.Path, pathToActs3_4Skip, true);
					RobustFile.Copy(clipContents.Path, pathToActs3_4Clip, true);
					Assert.That(ClipRepository.RestoreBackedUpClip(projectName, book, chapter, line), Is.False);
					Assert.That(pathToActs3_4Clip, Does.Exist);
					Assert.That(pathToActs3_4Skip, Does.Not.Exist);
					var reportedProblem = _errorReporter.ReportedProblems.Single();
					Assert.That(reportedProblem.Exception, Is.Null);
					Assert.That(reportedProblem.Message, Is.EqualTo(
						"HearThis found an existing clip for a block that was marked as being " +
						"skipped. Because the existing clip is newer, that file is being kept, " +
						$"but the other version is being kept as {backedUpSkipFile}"));
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}

		[Test]
		public void RestoreBackedUpClip_SkipFileNewerThanExistingClip_ReturnsTrueAndReportsNonFatalProblem()
		{
			const string projectName = "Dummy";
			const string book = "Acts";
			const int chapter = 3;
			const int line = 4;
			var pathToActs3_4Clip = ClipRepository.GetPathToLineRecording(projectName, book, chapter, line);
			var pathToActs3_4Skip = Path.ChangeExtension(pathToActs3_4Clip, ClipRepository.kSkipFileExtension);
			var backedUpClipFile = Path.ChangeExtension(pathToActs3_4Clip, "oldclip.wav");
			try
			{
				using (var skipContents = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var clipContents = TempFile.FromResource(Resource1._2Channel, ".wav"))
				{
					RobustFile.Copy(clipContents.Path, pathToActs3_4Clip, true);
					RobustFile.Copy(skipContents.Path, pathToActs3_4Skip, true);
					Assert.That(ClipRepository.RestoreBackedUpClip(projectName, book, chapter, line), Is.True);
					Assert.That(pathToActs3_4Clip, Does.Exist);
					Assert.That(pathToActs3_4Skip, Does.Not.Exist);
					var reportedProblem = _errorReporter.ReportedProblems.Single();
					Assert.That(reportedProblem.Exception, Is.Null);
					Assert.That(reportedProblem.Message, Is.EqualTo(
						"HearThis found an existing clip for a block that was marked as being " +
						"skipped. Because the skip file is newer, that file is replacing the " +
						$"existing clip, but the other version is being kept as {backedUpClipFile}"));
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(projectName));
			}
		}
		#endregion
		#endregion

		private static void CleanUpTestFolder(string chapterFolder, string testProject)
		{
			var testProjectFolder = Path.GetDirectoryName(Path.GetDirectoryName(chapterFolder));
			Assert.That(testProjectFolder.EndsWith(testProject),
				"Uh-oh. the implementation of ClipRepository.GetChapterFolder must have changed!");
			RobustIO.DeleteDirectoryAndContents(testProjectFolder);
		}

		private class TestChapterInfo : ChapterRecordingInfoBase
		{
			private readonly List<ScriptLine> _recordings;

			public int SaveCallCount { get; private set; }

			public TestChapterInfo(params int[] scriptLineNumbers)
			{
				_recordings = scriptLineNumbers.Select(n => new ScriptLine($"Line {n}")
					{Number = n, RecordingTime = DateTime.Now}).ToList();
			}

			public override IReadOnlyList<ScriptLine> RecordingInfo => _recordings;

			/// <summary>
			/// This is not expected to be called in these tests, but it is required by the
			/// interface.
			/// </summary>
			/// <exception cref="NotImplementedException">Always</exception>
			public override void OnScriptBlockRecorded(ScriptLine scriptBlock,
				Func<Exception, bool> exceptionHandlerOverride = null)
			{
				throw new NotImplementedException();
			}

			public override void Save(bool preserveModifiedTime = false)
			{
				SaveCallCount++;
			}
		}
	}
}
