using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;
using Palaso.IO;

namespace HearThisTests
{
	[TestFixture]
	public class ClipRepositoryTests
	{
		private const double kMonoSampleDuration = 0.062;

		private class TestPublishingModel : PublishingModel
		{
			public TestPublishingModel(VerseIndexFormatType verseIndexFormat) : base(new DummyInfoProvider(), null)
			{
				AudioFormat = "megaVoice";
				VerseIndexFormat = verseIndexFormat;
				SetPublishingMethod();
			}
		}

		private class DummyInfoProvider : IPublishingInfoProvider
		{
			private IBibleStats _stats = new BibleStats();
			public List<string> Verses = new List<string>();
			public readonly List<string> BooksNotToPublish = new List<string>();
			public string Name { get { return "Dummy"; } }
			public string EthnologueCode { get { return "xdum"; } }
			public string CurrentBookName { get { throw new NotImplementedException(); } }

			public bool IncludeBook(string bookName)
			{
				return !BooksNotToPublish.Contains(bookName);
			}

			public ScriptLine GetBlock(string bookName, int chapterNumber, int lineNumber0Based)
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
					heading = true;
					headingType = "s";
				}
				return new ScriptLine
				{
					Number = scriptLineNumber,
					Verse = verse,
					Heading = heading,
					HeadingType = headingType,
				};
			}

			public IBibleStats VersificationInfo { get { return _stats; } }
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
				var filesToJoin = new List<string>();
				filesToJoin.Add(mono.Path);
				filesToJoin.Add(stereo.Path);
				var progress = new Palaso.Progress.StringBuilderProgress();

				ClipRepository.MergeAudioFiles(filesToJoin, output.Path, progress);
				Assert.IsFalse(progress.ErrorEncountered);
				Assert.IsTrue(File.Exists(output.Path));
			}
		}

		[Test]
		public void MergeAudioFiles_SingleFile_CopiedToOutputPath()
		{
			using (var output = new TempFile())
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var filesToJoin = new List<string>();
				filesToJoin.Add(mono.Path);
				var progress = new Palaso.Progress.StringBuilderProgress();

				ClipRepository.MergeAudioFiles(filesToJoin, output.Path, progress);
				Assert.IsFalse(progress.ErrorEncountered);
				Assert.IsTrue(File.Exists(output.Path));
				Assert.AreEqual(File.ReadAllBytes(mono.Path), File.ReadAllBytes(output.Path));
			}
		}

		/// <summary>
		/// This tests the case where some recordings are done for a book, but then the book is deleted (e.g., in Paratext)
		/// </summary>
		[Test]
		public void PublishAllBooks_RecordingsExistForMissingBook_MissingBookIsSkipped()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			var projectName = publishingInfoProvider.Name;
			var publishingModel = new PublishingModel(publishingInfoProvider, null);
			publishingModel.AudioFormat = "megaVoice";
			publishingModel.PublishOnlyCurrentBook = false;
			publishingInfoProvider.BooksNotToPublish.Add("Proverbs");
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var fileInProverbs = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(projectName, "Proverbs", 1, 1)))
			using (var fileInJohn = TempFile.WithFilename(ClipRepository.GetPathToLineRecording(projectName, "John", 1, 1)))
			{
				File.Copy(mono.Path, fileInProverbs.Path, true);
				File.Copy(mono.Path, fileInJohn.Path, true);
				var progress = new Palaso.Progress.StringBuilderProgress();
				try
				{
					publishingModel.Publish(progress);
					Assert.IsFalse(progress.ErrorEncountered);
					Assert.AreEqual(1, publishingModel.FilesInput);
					Assert.AreEqual(1, publishingModel.FilesOutput);
					var megavoicePublishRoot = Path.Combine(publishingModel.PublishThisProjectPath, "MegaVoice");
					Assert.IsTrue(File.Exists(publishingModel.PublishingMethod.GetFilePathWithoutExtension(megavoicePublishRoot, "John", 1) + ".wav"));
					Assert.IsFalse(File.Exists(publishingModel.PublishingMethod.GetFilePathWithoutExtension(megavoicePublishRoot, "Proverbs", 1) + ".wav"));
					// Encoding process actually trims off a byte for some reason (probably because it's garbage), so we can't simply compare
					// entire byte stream.
					var encodedFileContents =
						File.ReadAllBytes(publishingModel.PublishingMethod.GetFilePathWithoutExtension(megavoicePublishRoot, "John", 1) + ".wav");
					var originalFileContents = File.ReadAllBytes(mono.Path);
					Assert.IsTrue(encodedFileContents.Length == originalFileContents.Length - 1);
					Assert.IsTrue(encodedFileContents.SequenceEqual(originalFileContents.Take(encodedFileContents.Length)));
				}
				finally
				{
					Directory.Delete(publishingModel.PublishThisProjectPath, true);
				}
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
				var progress = new Palaso.Progress.StringBuilderProgress();

				File.Create(outputPath).Close();
				Assert.IsTrue(File.Exists(outputPath));
				ClipRepository.PublishVerseIndexFiles(rootPath, bookName, chapterNumber, new string[0], publishingModel, progress);
				Assert.IsFalse(progress.ErrorEncountered);
				Assert.IsFalse(File.Exists(outputPath));
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
				var progress = new Palaso.Progress.StringBuilderProgress();

				Assert.IsFalse(File.Exists(outputPath));
				ClipRepository.PublishVerseIndexFiles(rootPath, bookName, chapterNumber, filesToJoin, publishingModel, progress);
				Assert.IsFalse(progress.ErrorEncountered);
				Assert.IsTrue(File.Exists(outputPath));
				Assert.IsTrue(File.ReadAllText(outputPath).Length > 0);
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
				var progress = new Palaso.Progress.StringBuilderProgress();

				Assert.IsFalse(File.Exists(outputPath));
				ClipRepository.PublishVerseIndexFiles(rootPath, bookName, chapterNumber, filesToJoin, publishingModel, progress);
				Assert.IsFalse(progress.ErrorEncountered);
				Assert.IsTrue(File.Exists(outputPath));
				Assert.IsTrue(File.ReadAllText(outputPath).Length > 0);
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
				var filesToJoin = new[] { file0.Path, file1.Path, file2.Path, file3.Path, file4.Path };

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration);
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
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var file0 = TempFile.WithFilename("0.wav"))
			using (var file1 = TempFile.WithFilename("1.wav"))
			{
				File.Copy(mono.Path, file0.Path, true);
				File.Copy(mono.Path, file1.Path, true);
				var filesToJoin = new[] { file0.Path, file1.Path};

				Assert.IsNull(ClipRepository.GetVerseIndexFileContents("Psalms", 0, filesToJoin,
					PublishingModel.VerseIndexFormatType.AudacityLabelFileVerseLevel, publishingInfoProvider, "dummy.txt"));
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
				var filesToJoin = new[] { file0.Path, file1.Path, file2.Path, file3.Path, file4.Path };

				var result = ClipRepository.GetVerseIndexFileContents("Psalms", 0, filesToJoin,
					PublishingModel.VerseIndexFormatType.AudacityLabelFilePhraseLevel, publishingInfoProvider, "dummy.txt");
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration);
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
				var filesToJoin = new[] { file1.Path, file2.Path, file5.Path };

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration);
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("1");
				verifier.AddExpectedLine("4");
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
				var filesToJoin = new[] { file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path };

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration);
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine(2, "1");
				verifier.AddExpectedLine(3, "2-3");
				verifier.AddExpectedLine("3");
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
				var filesToJoin = new[] { file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path };

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, true);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration);
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("1a");
				verifier.AddExpectedLine("1b");
				verifier.AddExpectedLine("2a");
				verifier.AddExpectedLine("2b"); // Ideally, we want 2b-3a, but SAB doesn't support this (yet)
				verifier.AddExpectedLine("3b");
				verifier.AddExpectedLine("3c");
				verifier.Verify();
			}
		}


		[Test]
		public void GetAudacityLabelFileContents_ByPhraseMultipleClipsInExplicitVerseBridge_LabelIndicatesPhraseesInVerseBridge()
		{
			var publishingInfoProvider = new DummyInfoProvider();
			publishingInfoProvider.Verses.Add("s"); // 0
			publishingInfoProvider.Verses.Add("v1"); // 1
			publishingInfoProvider.Verses.Add("v2-4"); // 2
			publishingInfoProvider.Verses.Add("v2-4"); // 3
			publishingInfoProvider.Verses.Add("v2-4"); // 4
			publishingInfoProvider.Verses.Add("v2-4~5"); // 5 Unlikely scenario: Sentence starts in bridge but caontinues into following verse
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
				var filesToJoin = new[] { file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path };

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, true);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration);
				verifier.AddExpectedLine("s1");
				verifier.AddExpectedLine("1");
				verifier.AddExpectedLine("2-4a");
				verifier.AddExpectedLine("2-4b");
				verifier.AddExpectedLine("2-4c");
				verifier.AddExpectedLine("2-4d"); // Ideally, we want 2-4d-5 (???), but SAB doesn't support this (yet)
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
				var filesToJoin = new[] { file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path };

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, false);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration);
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
				var filesToJoin = new[] { file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path, file7.Path };

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 5, true);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration);
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
			using (var file0 = TempFile.WithFilename("0.wav"))   // mt -> mt
			using (var file1 = TempFile.WithFilename("1.wav"))   // c  -> c
			using (var file2 = TempFile.WithFilename("2.wav"))   // ms -> ms1
			using (var file3 = TempFile.WithFilename("3.wav"))   // s  -> s1
			using (var file4 = TempFile.WithFilename("4.wav"))   // d  -> d1
			using (var file5 = TempFile.WithFilename("5.wav"))   // v1 -> 1
			using (var file6 = TempFile.WithFilename("6.wav"))   // v2 -> 2
			using (var file7 = TempFile.WithFilename("7.wav"))   // sp -> sp1
			using (var file8 = TempFile.WithFilename("8.wav"))   // v3 -> 3
			using (var file9 = TempFile.WithFilename("9.wav"))   // s2 -> s2
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
				var filesToJoin = new[] {
					file0.Path, file1.Path, file2.Path, file3.Path, file4.Path,
					file5.Path, file6.Path, file7.Path, file8.Path, file9.Path,
					file10.Path, file11.Path, file12.Path, file13.Path, file14.Path,
					file15.Path};

				var result = ClipRepository.GetAudacityLabelFileContents(filesToJoin, publishingInfoProvider, "Psalms", 1, true);
				var verifier = new AudacityLabelFileLineVerifier(result, kMonoSampleDuration);
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
				var filesToJoin = new[] { file0.Path, file1.Path, file2.Path, file3.Path, file4.Path };

				var result = ClipRepository.GetCueSheetContents(filesToJoin, publishingInfoProvider, "Psalms", 5, "PSA5.wav");
				var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
				int i = 0;
				Assert.AreEqual(14, lines.Length);
				Assert.AreEqual("FILE \"PSA5.wav\"", lines[i++]);
				Assert.AreEqual("TRACK 01 AUDIO", lines[i++]);
				Assert.AreEqual("  TITLE \"psa005-xdum-s1\"", lines[i++]);
				Assert.AreEqual("  INDEX 01 00:00:00", lines[i++]);
				Assert.AreEqual("TRACK 02 AUDIO", lines[i++]);
				Assert.AreEqual("  TITLE \"00000-psa005-xdum-V001\"", lines[i++]);
				Assert.AreEqual("  INDEX 01 00:00:05", lines[i++]);
				Assert.AreEqual("TRACK 03 AUDIO", lines[i++]);
				Assert.AreEqual("  TITLE \"00000-psa005-xdum-V002-003\"", lines[i++]);
				Assert.AreEqual("  INDEX 01 00:00:09", lines[i++]);
				Assert.AreEqual("0.062\t0.124\t1", lines[i++]);
				Assert.AreEqual("0.124\t0.186\t2-3", lines[i++]);
				Assert.AreEqual("0.186\t0.248\ts2", lines[i++]);
				Assert.AreEqual("0.248\t0.31\t4", lines[i]);
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
				var filesToJoin = new[] { file0.Path, file1.Path, file4.Path };

				var result = ClipRepository.GetCueSheetContents(filesToJoin, publishingInfoProvider, "Psalms", 5, "PSA5.wav");
				var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
				int i = 0;
				Assert.AreEqual(4, lines.Length);
				Assert.AreEqual("FILE \"PSA5.wav\"", lines[i++]);
				Assert.AreEqual("0\t0.062\ts1", lines[i++]);
				Assert.AreEqual("0.062\t0.124\t1", lines[i++]);
				Assert.AreEqual("0.124\t0.186\t4", lines[i]);
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
				var filesToJoin = new[] { file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path };

				var result = ClipRepository.GetCueSheetContents(filesToJoin, publishingInfoProvider, "Psalms", 5, "PSA5.wav");
				var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
				int i = 0;
				Assert.AreEqual(5, lines.Length);
				Assert.AreEqual("FILE \"PSA5.wav\"", lines[i++]);
				Assert.AreEqual("0\t0.062\ts1", lines[i++]);
				Assert.AreEqual("0.062\t0.186\t1", lines[i++]); // Includes 2 clips
				Assert.AreEqual("0.186\t0.372\t2-3", lines[i++]); // Includes 3 clips
				Assert.AreEqual("0.372\t0.434\t3", lines[i]);
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
				var filesToJoin = new[] { file0.Path, file1.Path, file2.Path, file3.Path, file4.Path, file5.Path, file6.Path };

				var result = ClipRepository.GetCueSheetContents(filesToJoin, publishingInfoProvider, "Psalms", 5, "PSA5.wav");
				var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
				int i = 0;
				Assert.AreEqual(5, lines.Length);
				Assert.AreEqual("FILE \"PSA5.wav\"", lines[i++]);
				Assert.AreEqual("0\t0.186\ts1", lines[i++]); // Includes 3 clips
				Assert.AreEqual("0.186\t0.248\t1-2", lines[i++]);
				Assert.AreEqual("0.248\t0.372\ts2", lines[i++]); // Includes 2 clips
				Assert.AreEqual("0.372\t0.434\t3", lines[i]);
			}
		}

		internal class AudacityLabelFileLineVerifier
		{
			internal class LabelLineInfo
			{
				public int NumberOfClips { get; set; }
				public string ExpectedLabel { get; set; }

				internal LabelLineInfo(int numberOfClips, string expectedLabel)
				{
					NumberOfClips = numberOfClips;
					ExpectedLabel = expectedLabel;
				}
			}

			private readonly double _sampleClipDuration;
			private readonly string[] _actualLabels;
			private readonly List<LabelLineInfo> _expectedLabelLines = new List<LabelLineInfo>();

			internal AudacityLabelFileLineVerifier(string actualFileContents, double sampleClipDuration)
			{
				_actualLabels = actualFileContents.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				_sampleClipDuration = sampleClipDuration;
			}

			internal void AddExpectedLine(string expectedLabel)
			{
				AddExpectedLine(1, expectedLabel);
			}

			internal void AddExpectedLine(int numberOfClips, string expectedLabel)
			{
				_expectedLabelLines.Add(new LabelLineInfo(numberOfClips, expectedLabel));
			}

			internal void AddExpectedSkippedClips(int numberOfClips)
			{
				_expectedLabelLines.Add(new LabelLineInfo(numberOfClips, null));
			}

			internal void Verify()
			{
				Assert.AreEqual(_expectedLabelLines.Count(l => l.ExpectedLabel != null), _actualLabels.Length);
				double start = 0;
				int iActual = 0;
				for (int i = 0; i < _expectedLabelLines.Count; i++)
				{
					var fields = _actualLabels[iActual].Split('\t');
					Assert.AreEqual(3, fields.Length, string.Format("Bogus line ({0}): {1}", i, _actualLabels[iActual]));

					var end = start + _expectedLabelLines[i].NumberOfClips * _sampleClipDuration;
					var expectedLabel = _expectedLabelLines[i].ExpectedLabel;
					if (expectedLabel != null)
					{
						var failMsg =
							string.Format(
								"Line {0} was expected to go from {1:0.######} to {2:0.######} and have label \"{3}\", but was \"{4}\"",
								iActual, start, end, expectedLabel, _actualLabels[iActual]);
						Assert.AreEqual(Math.Round(start, 6), Math.Round(double.Parse(fields[0]), 6), failMsg);
						Assert.AreEqual(Math.Round(end, 6), Math.Round(double.Parse(fields[1]), 6), failMsg);
						Assert.AreEqual(expectedLabel, fields[2], failMsg);
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
		public void AudacityLabelFileLineVerifier_Verify_CorrectWithSkippedClips_NoException()
		{
			string actual = "0\t0.062\tc" + Environment.NewLine +
				"0.186\t0.248\twhatever";
			var verifier = new AudacityLabelFileLineVerifier(actual, 0.062);
			verifier.AddExpectedLine(1, "c");
			verifier.AddExpectedSkippedClips(2);
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
			Assert.IsTrue(msg.Trim().StartsWith("Bogus line (0): 0\t0.062"));
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
			Assert.IsTrue(msg.Trim().StartsWith("Line 2 was expected to go from 0.18 to 0.24 and have label \"whatever\", but was \"0.181\t0.24\twhatever\""));
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
			Assert.IsTrue(msg.Trim().StartsWith("Line 1 was expected to go from 0.06 to 0.18 and have label \"2-3\", but was \"0.06\t0.143\t2-3\""));
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
			Assert.IsTrue(msg.Trim().StartsWith("Line 2 was expected to go from 0.18 to 0.24 and have label \"whatever\", but was \"0.18\t0.24\ts1\""));
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
	}
}
