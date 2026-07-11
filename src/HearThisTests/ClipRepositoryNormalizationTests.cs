using System;
using System.Collections.Generic;
using System.IO;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using NAudio.Wave;
using NUnit.Framework;
using SIL.IO;
using SIL.Media;
using SIL.Progress;
using static SIL.IO.FileLocationUtilities;

namespace HearThisTests
{
	internal class StubAudioNormalizationSettings : IAudioNormalizationSettings
	{
		public bool NormalizeVolume { get; set; }
		public bool ReduceNoise { get; set; }
		public PauseData ClipPause { get; set; }
		public PauseData ParagraphPause { get; set; }
		public PauseData SectionPause { get; set; }
		public PauseData ChapterPause { get; set; }
	}

	/// <summary>
	/// Tests for the pause-selection logic used by ClipRepository.MergeAudioFiles to decide
	/// which pause settings (clip, paragraph, or section) apply between two consecutive clips.
	/// These are pure unit tests; no audio processing is involved.
	/// </summary>
	[TestFixture]
	public class GetPauseToApplyTests
	{
		private static readonly PauseData s_clipPause = new PauseData(true, 0, 10);
		private static readonly PauseData s_paragraphPause = new PauseData(true, 0, 10);
		private static readonly PauseData s_sectionPause = new PauseData(true, 0, 10);

		[SetUp]
		public void SetUp()
		{
			LocalizationManager.StrictInitializationMode = false;
		}

		private static Func<int, ScriptLine> LineProvider(params ScriptLine[] lines) =>
			i => lines[i];

		private static ScriptLine Line(bool paragraphStart = false, bool heading = false) =>
			new ScriptLine { ParagraphStart = paragraphStart, Heading = heading };

		[Test]
		public void GetPauseToApply_OnlyClipPauseEnabled_ReturnsClipPause()
		{
			var settings = new StubAudioNormalizationSettings { ClipPause = s_clipPause };

			var pause = ClipRepository.GetPauseToApply(settings, LineProvider(Line(), Line()), 1);

			Assert.That(pause, Is.SameAs(s_clipPause));
		}

		[Test]
		public void GetPauseToApply_ParagraphStart_ParagraphPauseOverridesClipPause()
		{
			var settings = new StubAudioNormalizationSettings
			{
				ClipPause = s_clipPause,
				ParagraphPause = s_paragraphPause,
			};

			var pause = ClipRepository.GetPauseToApply(settings,
				LineProvider(Line(), Line(paragraphStart: true)), 1);

			Assert.That(pause, Is.SameAs(s_paragraphPause));
		}

		[Test]
		public void GetPauseToApply_ParagraphPauseEnabledButNotParagraphStart_ReturnsClipPause()
		{
			var settings = new StubAudioNormalizationSettings
			{
				ClipPause = s_clipPause,
				ParagraphPause = s_paragraphPause,
			};

			var pause = ClipRepository.GetPauseToApply(settings, LineProvider(Line(), Line()), 1);

			Assert.That(pause, Is.SameAs(s_clipPause));
		}

		[Test]
		public void GetPauseToApply_Heading_SectionPauseOverridesClipPause()
		{
			var settings = new StubAudioNormalizationSettings
			{
				ClipPause = s_clipPause,
				SectionPause = s_sectionPause,
			};

			var pause = ClipRepository.GetPauseToApply(settings,
				LineProvider(Line(), Line(heading: true)), 1);

			Assert.That(pause, Is.SameAs(s_sectionPause));
		}

		[Test]
		public void GetPauseToApply_HeadingThatIsAlsoParagraphStart_SectionPauseWins()
		{
			var settings = new StubAudioNormalizationSettings
			{
				ClipPause = s_clipPause,
				ParagraphPause = s_paragraphPause,
				SectionPause = s_sectionPause,
			};

			var pause = ClipRepository.GetPauseToApply(settings,
				LineProvider(Line(), Line(paragraphStart: true, heading: true)), 1);

			Assert.That(pause, Is.SameAs(s_sectionPause));
		}

		[Test]
		public void GetPauseToApply_ConsecutiveHeadings_SecondHeadingDoesNotGetSectionPause()
		{
			var settings = new StubAudioNormalizationSettings
			{
				ClipPause = s_clipPause,
				SectionPause = s_sectionPause,
			};
			var provider = LineProvider(Line(), Line(heading: true), Line(heading: true));

			Assert.That(ClipRepository.GetPauseToApply(settings, provider, 1),
				Is.SameAs(s_sectionPause), "First heading follows a non-heading clip");
			Assert.That(ClipRepository.GetPauseToApply(settings, provider, 2),
				Is.SameAs(s_clipPause), "Consecutive heading should fall back to the clip pause");
		}

		[Test]
		public void GetPauseToApply_NullGetScriptLine_ReturnsClipPause()
		{
			var settings = new StubAudioNormalizationSettings
			{
				ClipPause = s_clipPause,
				ParagraphPause = s_paragraphPause,
				SectionPause = s_sectionPause,
			};

			var pause = ClipRepository.GetPauseToApply(settings, null, 1);

			Assert.That(pause, Is.SameAs(s_clipPause));
		}

		[Test]
		public void GetPauseToApply_ScriptLineNotFound_ReturnsClipPause()
		{
			// An extraneous clip has no script block, so the provider returns null for it.
			var settings = new StubAudioNormalizationSettings
			{
				ClipPause = s_clipPause,
				ParagraphPause = s_paragraphPause,
				SectionPause = s_sectionPause,
			};

			var pause = ClipRepository.GetPauseToApply(settings, i => null, 1);

			Assert.That(pause, Is.SameAs(s_clipPause));
		}

		[Test]
		public void GetPauseToApply_NoPauseTypeEnabled_ReturnsNull()
		{
			var settings = new StubAudioNormalizationSettings
			{
				ClipPause = new PauseData(false, 0, 10),
				ParagraphPause = new PauseData(false, 0, 10),
				SectionPause = new PauseData(false, 0, 10),
			};

			var pause = ClipRepository.GetPauseToApply(settings,
				LineProvider(Line(), Line(paragraphStart: true, heading: true)), 1);

			Assert.That(pause, Is.Null);
		}

		[Test]
		public void GetPauseToApply_AllPauseSettingsNull_ReturnsNull()
		{
			var settings = new StubAudioNormalizationSettings();

			var pause = ClipRepository.GetPauseToApply(settings,
				LineProvider(Line(), Line(paragraphStart: true, heading: true)), 1);

			Assert.That(pause, Is.Null);
		}
	}

	/// <summary>
	/// Integration tests for the pause-normalization block in ClipRepository.MergeAudioFiles.
	/// These require FFmpeg and shntool from DistFiles.
	/// </summary>
	[TestFixture]
	public class MergeAudioFilesNormalizationTests
	{
		[SetUp]
		public void SetUp()
		{
			LocalizationManager.StrictInitializationMode = false;
			// Normally set by the PublishingMethodBase constructor, which is not involved
			// when calling MergeAudioFiles directly.
			FFmpegRunner.FFmpegLocation = GetFileDistributedWithApplication("FFmpeg", "ffmpeg.exe");
		}

		[Test]
		public void MergeAudioFiles_ClipPauseEnabledWithWideRange_NormalizesWithoutError()
		{
			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(true, 0, 10),
				};
				var progress = new StringBuilderProgress();

				ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
					output.Path, progress, settings, i => new ScriptLine());

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(progress.Text, Does.Contain("Normalizing audio"));
				Assert.That(output.Path, Does.Exist);
			}
		}

		[Test]
		public void MergeAudioFiles_ClipPauseEnabledWithReduceNoise_NormalizesWithoutError()
		{
			// Exercises the per-clip noise reduction of the (mono) clips used to measure
			// silence, plus the join of the noise-reduced clips when
			// DOUBLE_PASS_NOISE_REDUCTION is defined.
			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var settings = new StubAudioNormalizationSettings
				{
					ReduceNoise = true,
					ClipPause = new PauseData(true, 0, 10),
				};
				var progress = new StringBuilderProgress();

				ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
					output.Path, progress, settings, i => new ScriptLine());

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(progress.Text,
					Does.Not.Contain("Error trying to normalize pauses"));
				Assert.That(output.Path, Does.Exist);
			}
		}

		[Test]
		public void MergeAudioFiles_AllPauseTypesDisabled_SkipsPauseNormalization()
		{
			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(false, 0, 10),
					ParagraphPause = new PauseData(false, 0, 10),
					SectionPause = new PauseData(false, 0, 10),
				};
				var progress = new StringBuilderProgress();

				ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
					output.Path, progress, settings);

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(progress.Text, Does.Not.Contain("Normalizing audio"));
				Assert.That(output.Path, Does.Exist);
			}
		}

		[Test]
		public void MergeAudioFiles_PauseSettingsNull_SkipsPauseNormalizationWithoutCrashing()
		{
			// The pause settings can be null, e.g., after an error in a previous chapter
			// caused them to be cleared. This must not throw.
			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var settings = new StubAudioNormalizationSettings();
				var progress = new StringBuilderProgress();

				Assert.That(() => ClipRepository.MergeAudioFiles(
					new List<string> { clip1.Path, clip2.Path },
					output.Path, progress, settings), Throws.Nothing);

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(output.Path, Does.Exist);
			}
		}

		[Test]
		public void MergeAudioFiles_ErrorNormalizingPauses_AllPauseSettingsClearedAndWarningWritten()
		{
			// Requires FFmpeg installed at the expected path. The invalid (empty) clip makes
			// FFmpeg fail when the pause loop tries to lengthen the pause (the impossibly
			// tight range forces that attempt), which must clear all three pause settings so
			// subsequent chapters do not fail the same way.
			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.WithExtension(".wav"))
			{
				File.WriteAllBytes(clip2.Path, new byte[0]);
				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(true, 99, 100),
					ParagraphPause = new PauseData(true, 99, 100),
					SectionPause = new PauseData(true, 99, 100),
				};
				var progress = new StringBuilderProgress();

				try
				{
					ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
						output.Path, progress, settings);
				}
				catch
				{
					// The processing that follows the pause-normalization block also fails
					// on the invalid clip; only the pause-error recovery is under test here.
				}

				Assert.That(settings.ClipPause, Is.Null);
				Assert.That(settings.ParagraphPause, Is.Null);
				Assert.That(settings.SectionPause, Is.Null);
				Assert.That(progress.Text,
					Does.Contain("Error trying to normalize pauses in combined audio file"));
			}
		}
	}

	/// <summary>
	/// Tests for the chapter-pause constraining block in PublishingMethodBase.PublishChapter.
	/// These require FFmpeg from DistFiles.
	/// </summary>
	[TestFixture]
	public class PublishingMethodBaseChapterPauseTests
	{
		private class MinimalPublishingInfo : IPublishingInfo
		{
			public string Name => "Dummy";
			public string EthnologueCode => "xdum";
			public string CurrentBookName => null;
			public bool IncludeBook(string bookName) => true;
			public ScriptLine GetUnfilteredBlock(string bookName, int chapterNumber, int lineNumber0Based) => null;
			public IBibleStats VersificationInfo { get; } = new BibleStats();
			public int BookNameComparer(string x, string y) => 0;
			public bool BreakQuotesIntoBlocks => false;
			public string BlockBreakCharacters => ". ?";
			public bool HasProblemNeedingAttention(string bookName = null) => false;
		}

		[SetUp]
		public void SetUp()
		{
			LocalizationManager.StrictInitializationMode = false;
		}

		private static PublishingModel CreateModel() =>
			new PublishingModel(new MinimalPublishingInfo());

		[Test]
		public void PublishChapter_OnlyChapterPauseEnabled_ChapterPauseBlockEntered()
		{
			// Regression test: the chapter-pause block must run even when neither volume
			// normalization nor noise reduction is enabled.
			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
				var model = CreateModel();
				model.ChapterPause = new PauseData(true, 0, 10);
				var progress = new StringBuilderProgress();

				publisher.PublishChapter(Path.GetTempPath(), "Genesis", 1, chapterWav.Path,
					progress, model);

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(progress.Text, Does.Contain("Constraining Pauses between Chapters"));
			}
		}

		[Test]
		public void PublishChapter_ChapterPauseDisabled_ChapterPauseBlockNotEntered()
		{
			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var encoder = new MockEncoder();
				var publisher = new AudiBiblePublishingMethod(encoder, "xyz");
				var model = CreateModel();
				model.ChapterPause = new PauseData(false, 0, 10);
				var progress = new StringBuilderProgress();

				publisher.PublishChapter(Path.GetTempPath(), "Genesis", 1, chapterWav.Path,
					progress, model);

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(progress.Text, Does.Not.Contain("Constraining Pauses"));
				Assert.That(encoder.SourcePaths, Is.EqualTo(new[] { chapterWav.Path }),
					"Chapter should still be encoded");
			}
		}

		[Test]
		public void PublishChapter_NormalizeVolumeWithNullChapterPause_DoesNotThrow()
		{
			// ChapterPause can be null if SaveAudioNormalizationSettings has not run.
			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
				var model = CreateModel();
				model.NormalizeVolume = true;
				var progress = new StringBuilderProgress();

				Assert.That(() => publisher.PublishChapter(Path.GetTempPath(), "Genesis", 1,
					chapterWav.Path, progress, model), Throws.Nothing);

				Assert.That(progress.Text, Does.Not.Contain("Constraining Pauses"));
			}
		}
	}

	/// <summary>
	/// Integration tests for ClipRepository.ReduceNoise. These require FFmpeg and the
	/// cb.rnnn noise-reduction model from DistFiles.
	/// </summary>
	[TestFixture]
	public class ReduceNoiseTests
	{
		[SetUp]
		public void SetUp()
		{
			LocalizationManager.StrictInitializationMode = false;
			// Normally set by the PublishingMethodBase constructor, which is not involved
			// when calling ReduceNoise directly.
			FFmpegRunner.FFmpegLocation = GetFileDistributedWithApplication("FFmpeg", "ffmpeg.exe");
		}

		private static double GetRmsDb(string wavPath)
		{
			using (var reader = new WaveFileReader(wavPath))
			{
				double sumOfSquares = 0;
				long sampleCount = 0;
				float[] frame;
				while ((frame = reader.ReadNextSampleFrame()) != null)
				{
					foreach (var sample in frame)
					{
						sumOfSquares += sample * (double)sample;
						sampleCount++;
					}
				}
				return 10 * Math.Log10(sumOfSquares / sampleCount);
			}
		}

		[Test]
		public void ReduceNoise_MonoSpeechClip_PreservesSpeechLevel()
		{
			// HearThis records mono clips, so noise reduction must handle mono input and
			// must not attenuate the speech it is supposed to preserve. (A filter graph
			// that leans on stereo-only filters like dialoguenhance loses several dB of
			// speech when fed mono audio.)
			var speechClip = GetFileDistributedWithApplication(
				"localization", "SampleAudio-es", "sampleSentenceMatchingText.wav");
			using (var output = TempFile.WithExtension(".wav"))
			{
				// ReduceNoise runs ffmpeg without -y, so the destination must not exist.
				File.Delete(output.Path);
				var progress = new StringBuilderProgress();

				ClipRepository.ReduceNoise(speechClip, output.Path, progress);

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(output.Path, Does.Exist);
				Assert.That(GetRmsDb(output.Path),
					Is.GreaterThan(GetRmsDb(speechClip) - 3),
					"Noise reduction should not significantly reduce the speech level");
			}
		}
	}
}
