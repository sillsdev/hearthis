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

		[Test]
		public void MergeAudioFiles_ClipPauseAddsBlankSpace_ReturnsPositiveAdjustmentOnSubsequentClip()
		{
			using (var output = new TempFile())
			using (var clip1 = TempFile.WithExtension(".wav"))
			using (var clip2 = TempFile.WithExtension(".wav"))
			{
				// Tone-only clips (no explicit silence segment) measure ~0 leading/trailing
				// silence, so a min well above that forces ConstrainBoundary to add blank
				// space to clip2's start.
				ClipRepositorySilenceDetectionTests.WriteWavFile(clip1.Path, (1.0, false));
				ClipRepositorySilenceDetectionTests.WriteWavFile(clip2.Path, (1.0, false));
				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(true, 1.0, 2.0),
				};
				var progress = new StringBuilderProgress();

				var adjustments = ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
					output.Path, progress, settings, i => new ScriptLine());

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(adjustments.Count, Is.EqualTo(2));
				Assert.That(adjustments[0], Is.EqualTo(0));
				Assert.That(adjustments[1], Is.EqualTo(1.0).Within(0.1));
			}
		}

		[Test]
		public void MergeAudioFiles_ClipPauseTrimsBlankSpace_ReturnsNegativeAdjustmentsOnBothClips()
		{
			using (var output = new TempFile())
			using (var clip1 = TempFile.WithExtension(".wav"))
			using (var clip2 = TempFile.WithExtension(".wav"))
			{
				ClipRepositorySilenceDetectionTests.WriteWavFile(clip1.Path, (1.0, false), (0.6, true));
				ClipRepositorySilenceDetectionTests.WriteWavFile(clip2.Path, (0.4, true), (1.0, false));
				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(true, 0, 0.5),
				};
				var progress = new StringBuilderProgress();

				var adjustments = ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
					output.Path, progress, settings, i => new ScriptLine());

				Assert.That(progress.ErrorEncountered, Is.False);
				// Combined 1.0s silence, max 0.5s: takeOffAll = 0.5, split 0.6/0.4 -> -0.3/-0.2
				// (same math as ClipRepositoryConstrainBoundaryTests. ConstrainBoundary_CombinedSilenceAboveMax...).
				Assert.That(adjustments[0], Is.EqualTo(-0.3).Within(0.1));
				Assert.That(adjustments[1], Is.EqualTo(-0.2).Within(0.1));
			}
		}

		[Test]
		public void MergeAudioFiles_MiddleClipHasAdjustmentsFromBothBoundaries_SumsRatherThanOverwrites()
		{
			// Three clips, two boundaries, both touching the middle clip (clip2): boundary 1
			// (clip1-clip2) is an "add" case that lengthens clip2's start; boundary 2
			// (clip2-clip3) is a "remove" case that shortens clip2's end. clip2's returned
			// adjustment must be the SUM of both (proving ClipRepository.MergeAudioFiles uses
			// `+=`, not `=`, when both boundary loop iterations touch the same array slot) --
			// not just whichever boundary ran last.
			using (var output = new TempFile())
			using (var clip1 = TempFile.WithExtension(".wav"))
			using (var clip2 = TempFile.WithExtension(".wav"))
			using (var clip3 = TempFile.WithExtension(".wav"))
			{
				// clip1: 1.0s tone, 0.1s trailing silence.
				ClipRepositorySilenceDetectionTests.WriteWavFile(clip1.Path, (1.0, false), (0.1, true));
				// clip2: 0.1s leading silence, 1.0s tone, 5.0s trailing silence.
				ClipRepositorySilenceDetectionTests.WriteWavFile(clip2.Path, (0.1, true), (1.0, false), (5.0, true));
				// clip3: 0.5s leading silence, 1.0s tone.
				ClipRepositorySilenceDetectionTests.WriteWavFile(clip3.Path, (0.5, true), (1.0, false));

				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(true, 1.0, 3.0),
				};
				var progress = new StringBuilderProgress();

				var adjustments = ClipRepository.MergeAudioFiles(
					new List<string> { clip1.Path, clip2.Path, clip3.Path },
					output.Path, progress, settings, i => new ScriptLine());

				Assert.That(progress.ErrorEncountered, Is.False);
				// Boundary 1: combined 0.2s is below min 1.0s -> +0.8s added to clip2's start.
				// Boundary 2: combined 5.5s is above max 3.0s -> takeOffAll = 2.5s, split
				// 5.0/5.5 : 0.5/5.5 -> -2.2727s off clip2's end, -0.2273s off clip3's start.
				// clip2's net: +0.8 - 2.2727 = -1.4727 (sign and magnitude only make sense as a
				// sum of both contributions -- neither boundary alone produces this value).
				Assert.That(adjustments[0], Is.EqualTo(0));
				Assert.That(adjustments[1], Is.EqualTo(-1.4727).Within(0.1));
				Assert.That(adjustments[2], Is.EqualTo(-0.2273).Within(0.1));
			}
		}

		[Test]
		public void MergeAudioFiles_SingleFile_ReturnsSingleZeroAdjustment()
		{
			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(true, 0, 10),
				};
				var progress = new StringBuilderProgress();

				var adjustments = ClipRepository.MergeAudioFiles(new List<string> { clip1.Path },
					output.Path, progress, settings, i => new ScriptLine());

				Assert.That(adjustments.Count, Is.EqualTo(1));
				Assert.That(adjustments[0], Is.EqualTo(0));
			}
		}

		[Test]
		public void MergeAudioFiles_ClipPauseEnabled_CopyTempIsEmptyAfterCompletion()
		{
			// CopyAllFiles's copies in copy_temp are only ever read during this same call
			// (either directly, or replaced by measure_temp's noise-reduced copies under
			// DOUBLE_PASS_NOISE_REDUCTION) -- nothing needs them to survive after the join
			// completes, so leaving them on disk indefinitely (previously only cleared at the
			// *start* of the next call) wastes space for no benefit.
			var copyTempFolder = Path.Combine(Path.GetTempPath(), "copy_temp");
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
				Assert.That(Directory.Exists(copyTempFolder), Is.True,
					"Test assumption violated: copy_temp should have been created by this call.");
				Assert.That(Directory.GetFiles(copyTempFolder), Is.Empty);
			}
		}

		[Test]
		public void MergeAudioFiles_ClipPauseAndReduceNoiseEnabled_MeasureTempIsEmptyAfterCompletion()
		{
			// measure_temp's noise-reduced copies (made solely to measure silence accurately)
			// are never read again once the pause-normalization loop finishes -- same
			// leftover-forever problem as copy_temp.
			var measureTempFolder = Path.Combine(Path.GetTempPath(), "measure_temp");
			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(true, 0, 10),
					ReduceNoise = true,
				};
				var progress = new StringBuilderProgress();

				ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
					output.Path, progress, settings, i => new ScriptLine());

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(Directory.Exists(measureTempFolder), Is.True,
					"Test assumption violated: measure_temp should have been created by this call.");
				Assert.That(Directory.GetFiles(measureTempFolder), Is.Empty);
			}
		}
	}

	/// <summary>
	/// Tests for PublishingMethodBase.TryParseLoudnormStats, which extracts the
	/// measured-loudness JSON block ffmpeg's loudnorm filter writes to stderr (amid other,
	/// unrelated ffmpeg log output) when invoked with print_format=json. Pure unit tests; no
	/// audio processing.
	/// </summary>
	[TestFixture]
	public class TryParseLoudnormStatsTests
	{
		// A representative (trimmed) excerpt of real ffmpeg stderr output: version banner and
		// stream info lines before the JSON block, a progress line after it.
		private const string kRealisticSurroundingNoise =
			"ffmpeg version 6.0 Copyright (c) 2000-2023 the FFmpeg developers\r\n" +
			"  built with gcc 12.2.0\r\n" +
			"Input #0, wav, from 'chapter.wav':\r\n" +
			"  Duration: 00:00:08.00, bitrate: 706 kb/s\r\n" +
			"[Parsed_loudnorm_0 @ 000001c8] \r\n";

		private const string kTrailingNoise =
			"size=     689kB time=00:00:07.98 bitrate= 706.8kbits/s speed= 839x\r\n";

		[Test]
		public void TryParseLoudnormStats_ValidJsonAmidOtherFFmpegOutput_ExtractsAllFiveValues()
		{
			var output = kRealisticSurroundingNoise +
				"{\r\n" +
				"\t\"input_i\" : \"-23.71\",\r\n" +
				"\t\"input_tp\" : \"-6.54\",\r\n" +
				"\t\"input_lra\" : \"16.90\",\r\n" +
				"\t\"input_thresh\" : \"-34.08\",\r\n" +
				"\t\"output_i\" : \"-16.01\",\r\n" +
				"\t\"output_tp\" : \"-1.00\",\r\n" +
				"\t\"output_lra\" : \"7.00\",\r\n" +
				"\t\"output_thresh\" : \"-26.33\",\r\n" +
				"\t\"normalization_type\" : \"dynamic\",\r\n" +
				"\t\"target_offset\" : \"0.01\"\r\n" +
				"}\r\n" + kTrailingNoise;

			var succeeded = PublishingMethodBase.TryParseLoudnormStats(output, out var stats);

			Assert.That(succeeded, Is.True);
			Assert.That(stats.measuredI, Is.EqualTo("-23.71"));
			Assert.That(stats.measuredLra, Is.EqualTo("16.90"));
			Assert.That(stats.measuredTp, Is.EqualTo("-6.54"));
			Assert.That(stats.measuredThresh, Is.EqualTo("-34.08"));
			Assert.That(stats.targetOffset, Is.EqualTo("0.01"));
		}

		[Test]
		public void TryParseLoudnormStats_PositiveTargetOffset_ExtractsWithoutSign()
		{
			// target_offset can be positive (needs more gain) or negative (needs less);
			// ffmpeg's JSON only adds a "-" prefix for negative values, never a "+".
			var output = "{ \"input_i\" : \"-30.00\", \"input_tp\" : \"-10.00\", " +
				"\"input_lra\" : \"5.00\", \"input_thresh\" : \"-40.00\", \"target_offset\" : \"2.50\" }";

			var succeeded = PublishingMethodBase.TryParseLoudnormStats(output, out var stats);

			Assert.That(succeeded, Is.True);
			Assert.That(stats.targetOffset, Is.EqualTo("2.50"));
		}

		[Test]
		public void TryParseLoudnormStats_AudioTooShortOrQuietToMeasure_ReturnsFalse()
		{
			// ffmpeg reports "-inf"/"inf" instead of a number when it can't produce a usable
			// measurement (e.g. HearThisTests' own ~62ms mono sample clip).
			var output = "{\r\n" +
				"\t\"input_i\" : \"-inf\",\r\n" +
				"\t\"input_tp\" : \"-45.30\",\r\n" +
				"\t\"input_lra\" : \"0.00\",\r\n" +
				"\t\"input_thresh\" : \"-70.00\",\r\n" +
				"\t\"output_i\" : \"-inf\",\r\n" +
				"\t\"target_offset\" : \"inf\"\r\n" +
				"}\r\n";

			var succeeded = PublishingMethodBase.TryParseLoudnormStats(output, out _);

			Assert.That(succeeded, Is.False);
		}

		[Test]
		public void TryParseLoudnormStats_NoJsonBlockAtAll_ReturnsFalse()
		{
			var succeeded = PublishingMethodBase.TryParseLoudnormStats(kRealisticSurroundingNoise, out _);

			Assert.That(succeeded, Is.False);
		}

		[Test]
		public void TryParseLoudnormStats_OneFieldMissing_ReturnsFalseRatherThanPartialStats()
		{
			// All five fields must parse, or the caller should fall back entirely rather than
			// apply a mix of real and missing measured values.
			var output = "{ \"input_i\" : \"-23.71\", \"input_tp\" : \"-6.54\", " +
				"\"input_lra\" : \"16.90\", \"target_offset\" : \"0.01\" }"; // input_thresh missing

			var succeeded = PublishingMethodBase.TryParseLoudnormStats(output, out _);

			Assert.That(succeeded, Is.False);
		}
	}

	/// <summary>
	/// Tests for PublishingMethodBase.PrepareChapterAudio and FinalizeChapterAudio. These
	/// require FFmpeg from DistFiles. Chapter-boundary pause constraining no longer happens
	/// here -- see ClipRepositoryTests for coverage of that, now in
	/// ClipRepository.PublishAllChapters.
	/// </summary>
	[TestFixture]
	public class PublishingMethodBaseChapterAudioTests
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
		public void PrepareChapterAudio_NoPostProcessingRequested_ReturnsInputPathUnchanged()
		{
			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
				var model = CreateModel();
				var progress = new StringBuilderProgress();

				var result = publisher.PrepareChapterAudio(chapterWav.Path, progress, model);

				Assert.That(result, Is.EqualTo(chapterWav.Path));
				Assert.That(progress.ErrorEncountered, Is.False);
			}
		}

		[Test]
		public void PrepareChapterAudio_NullPublishingModel_ReturnsInputPathUnchanged()
		{
			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
				var progress = new StringBuilderProgress();

				var result = publisher.PrepareChapterAudio(chapterWav.Path, progress);

				Assert.That(result, Is.EqualTo(chapterWav.Path));
			}
		}

		[Test]
		public void PrepareChapterAudio_NormalizeVolumeEnabledButReduceNoiseDisabled_ReturnsInputPathUnchanged()
		{
			// Volume normalization now happens entirely in FinalizeChapterAudio (as a single
			// real two-pass loudnorm operation on the final, fully-assembled chapter audio) --
			// PrepareChapterAudio no longer does anything for NormalizeVolume on its own.
			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
				var model = CreateModel();
				model.NormalizeVolume = true;
				var progress = new StringBuilderProgress();

				var result = publisher.PrepareChapterAudio(chapterWav.Path, progress, model);

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(result, Is.EqualTo(chapterWav.Path));
			}
		}

		[Test]
		public void FinalizeChapterAudio_NoPostProcessingRequested_EncodesPreparedPathDirectly()
		{
			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var encoder = new MockEncoder();
				var publisher = new AudiBiblePublishingMethod(encoder, "xyz");
				var model = CreateModel();
				var progress = new StringBuilderProgress();

				publisher.FinalizeChapterAudio(Path.GetTempPath(), "Genesis", 1, chapterWav.Path,
					progress, model);

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(encoder.SourcePaths, Is.EqualTo(new[] { chapterWav.Path }));
			}
		}

		[Test]
		public void FinalizeChapterAudio_NormalizeVolumeEnabled_DoesNotThrow()
		{
			// Resource1._1Channel is only ~62ms -- too short for ffmpeg's loudnorm to produce
			// a usable measurement (it reports "-inf"), so this exercises
			// StandardNormalizeVolume's single-dynamic-pass fallback path. See
			// FinalizeChapterAudio_NormalizeVolumeEnabledWithMeasurableAudio_CompletesSuccessfully
			// below for the real (measurable) two-pass path.
			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
				var model = CreateModel();
				model.NormalizeVolume = true;

				Assert.That(() => publisher.FinalizeChapterAudio(Path.GetTempPath(), "Genesis", 1,
					chapterWav.Path, new StringBuilderProgress(), model), Throws.Nothing);
			}
		}

		[Test]
		public void FinalizeChapterAudio_NormalizeVolumeEnabledWithMeasurableAudio_CompletesSuccessfully()
		{
			using (var chapterWav = TempFile.WithExtension(".wav"))
			{
				// Long enough for ffmpeg's loudnorm to produce a real measurement (unlike the
				// ~62ms clip used above, which is too short and exercises the dynamic-mode
				// fallback instead).
				ClipRepositorySilenceDetectionTests.WriteWavFile(chapterWav.Path, (8.0, false));

				var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
				var model = CreateModel();
				model.NormalizeVolume = true;
				var progress = new StringBuilderProgress();

				publisher.FinalizeChapterAudio(Path.GetTempPath(), "Genesis", 1, chapterWav.Path, progress, model);

				Assert.That(progress.ErrorEncountered, Is.False);
				Assert.That(chapterWav.Path, Does.Exist);
				// Confirm the output is still a valid, readable WAV -- not corrupted by the
				// two-step (measure, then apply) process.
				using (var reader = new WaveFileReader(chapterWav.Path))
					Assert.That(reader.TotalTime.TotalSeconds, Is.GreaterThan(0));
			}
		}
	}

	/// <summary>
	/// Tests for the pure ffmpeg-argument-building helpers PublishingMethodBase.
	/// StandardNormalizeVolume uses for real two-pass loudnorm. No ffmpeg involved.
	/// </summary>
	[TestFixture]
	public class LoudnormArgumentBuilderTests
	{
		private static readonly (string measuredI, string measuredLra, string measuredTp,
			string measuredThresh, string targetOffset) kSampleStats =
			("-23.71", "16.90", "-6.54", "-34.08", "0.01");

		[Test]
		public void BuildLoudnormMeasureArguments_IncludesSourcePathAndAnalysisOnlyFlags()
		{
			var args = PublishingMethodBase.BuildLoudnormMeasureArguments("C:\\temp\\source.wav");

			Assert.That(args, Does.Contain("\"C:\\temp\\source.wav\""));
			Assert.That(args, Does.Contain("print_format=json"));
			Assert.That(args, Does.Contain("-f null -"));
			Assert.That(args, Does.Contain("dual_mono=true"));
			// An analysis-only pass must never specify a destination file -- the source path
			// should be the only ".wav" reference in the whole command.
			Assert.That(args.Split(new[] { ".wav" }, StringSplitOptions.None).Length - 1, Is.EqualTo(1));
		}

		[Test]
		public void BuildLoudnormApplyArguments_EmbedsExactMeasuredValuesAndLinearMode()
		{
			var args = PublishingMethodBase.BuildLoudnormApplyArguments(
				"C:\\temp\\source.wav", "C:\\temp\\dest.wav", kSampleStats);

			Assert.That(args, Does.Contain("\"C:\\temp\\source.wav\""));
			Assert.That(args, Does.Contain("\"C:\\temp\\dest.wav\""));
			Assert.That(args, Does.Contain("measured_I=-23.71"));
			Assert.That(args, Does.Contain("measured_LRA=16.90"));
			Assert.That(args, Does.Contain("measured_TP=-6.54"));
			Assert.That(args, Does.Contain("measured_thresh=-34.08"));
			Assert.That(args, Does.Contain("offset=0.01"));
			Assert.That(args, Does.Contain("linear=true"));
			Assert.That(args, Does.Contain("dual_mono=true"));
		}

		[Test]
		public void BuildLoudnormDynamicArguments_HasNoMeasuredValues()
		{
			// The fallback path for audio too short/quiet to measure must not reference any
			// measured_* parameter -- those values don't exist in that case.
			var args = PublishingMethodBase.BuildLoudnormDynamicArguments(
				"C:\\temp\\source.wav", "C:\\temp\\dest.wav");

			Assert.That(args, Does.Contain("\"C:\\temp\\source.wav\""));
			Assert.That(args, Does.Contain("\"C:\\temp\\dest.wav\""));
			Assert.That(args, Does.Contain("dual_mono=true"));
			Assert.That(args, Does.Not.Contain("measured_"));
			Assert.That(args, Does.Not.Contain("linear=true"));
		}
	}

	/// <summary>
	/// Tests that ClipRepository's own ffmpeg-invoking helpers work correctly even when
	/// nothing has set FFmpegRunner.FFmpegLocation beforehand -- e.g. a caller that uses
	/// MergeAudioFiles or its silence-detection/pause helpers directly, without ever
	/// constructing a PublishingMethodBase subclass first (which is what normally sets this
	/// static field, as a side effect of its own constructor, before any publish-related work
	/// happens).
	/// </summary>
	[TestFixture]
	public class ClipRepositoryFFmpegLocationTests
	{
		[SetUp]
		public void SetUp()
		{
			LocalizationManager.StrictInitializationMode = false;
		}

		[TearDown]
		public void TearDown()
		{
			// Other fixtures assume a valid FFmpegLocation is already set by the time their own
			// SetUp runs -- restore it rather than leaving it null, so test run order can't
			// make an unrelated fixture fail.
			FFmpegRunner.FFmpegLocation = GetFileDistributedWithApplication("FFmpeg", "ffmpeg.exe");
		}

		[Test]
		public void GetDurationOfLeadingSilence_FFmpegLocationNeverSet_StillWorks()
		{
			// Deliberately do NOT set FFmpegRunner.FFmpegLocation -- simulates a caller that
			// never constructed a PublishingMethodBase subclass.
			FFmpegRunner.FFmpegLocation = null;

			using (var wavFile = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var progress = new StringBuilderProgress();

				Assert.That(() => ClipRepository.GetDurationOfLeadingSilence(wavFile.Path, progress),
					Throws.Nothing);
				Assert.That(progress.ErrorEncountered, Is.False);
			}
		}

		[Test]
		public void MergeAudioFiles_FFmpegLocationNeverSet_StillWorks()
		{
			FFmpegRunner.FFmpegLocation = null;

			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(true, 0, 10),
				};
				var progress = new StringBuilderProgress();

				Assert.That(() => ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
					output.Path, progress, settings, i => new ScriptLine()), Throws.Nothing);
				Assert.That(progress.ErrorEncountered, Is.False);
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
