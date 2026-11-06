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
		public void MergeAudioFiles_ReduceNoiseEnabledAllPauseTypesDisabled_StillAppliesDoublePassNoiseReduction()
		{
			// With no pause type enabled, the per-clip noise-reduction pass must still run
			// (double-pass is the default). An invalid (empty) second clip makes that pass
			// fail, which is observable: the catch block reports a noise-reduction error. If
			// the noise-reduction pass were skipped, it would not.
			Environment.SetEnvironmentVariable("SINGLE_PASS_NOISE_REDUCTION", null);
			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.WithExtension(".wav"))
			{
				File.WriteAllBytes(clip2.Path, new byte[0]);
				var settings = new StubAudioNormalizationSettings
				{
					ReduceNoise = true,
					ClipPause = new PauseData(false, 0, 10),
					ParagraphPause = new PauseData(false, 0, 10),
					SectionPause = new PauseData(false, 0, 10),
				};
				var progress = new StringBuilderProgress();

				try
				{
					ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
						output.Path, progress, settings, i => new ScriptLine());
				}
				catch
				{
					// Whatever happens to this chapter after the failure isn't under test --
					// only whether the noise-reduction pass ran and failed.
				}

				Assert.That(progress.Text, Does.Contain("Error trying to reduce noise in clips to be joined"));
				// A per-clip noise-reduction failure must not disable ReduceNoise: that flag is
				// also read independently by PrepareChapterAudio's own whole-chapter
				// noise-reduction pass, and clearing it here would silently disable that pass for
				// every remaining chapter (and subsequent book) in the publish run.
				Assert.That(settings.ReduceNoise, Is.True);
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
		public void MergeAudioFiles_ClipPauseEnabled_WorkingCopiesIsRemovedAfterCompletion()
		{
			// CopyAllFiles's copies in working_copies are only ever read during this same call
			// (either directly, or replaced by measure_silence's noise-reduced copies under
			// DOUBLE_PASS_NOISE_REDUCTION) -- nothing needs them to survive after the join
			// completes, so leaving them on disk indefinitely (previously only cleared at the
			// *start* of the next call) wastes space for no benefit. The whole folder is
			// removed, not just emptied -- RobustIO.DeleteDirectoryAndContents is the same
			// helper already used elsewhere in this file for deleting a stale output folder.
			var workingCopiesFolder = Path.Combine(Path.GetTempPath(), "working_copies");
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
				Assert.That(Directory.Exists(workingCopiesFolder), Is.False);
			}
		}

		[Test]
		public void MergeAudioFiles_ClipPauseAndReduceNoiseEnabled_MeasureSilenceIsRemovedAfterCompletion()
		{
			// measure_silence's noise-reduced copies (made solely to measure silence accurately)
			// are never read again once the pause-normalization loop finishes -- same
			// leftover-forever problem as working_copies.
			var measureSilenceFolder = Path.Combine(Path.GetTempPath(), "measure_silence");
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
				Assert.That(Directory.Exists(measureSilenceFolder), Is.False);
			}
		}

		[Test]
		public void MergeAudioFiles_ReduceNoiseEnabledDefaultDoublePass_NeverCallsCopyAllFiles()
		{
			// Under double-pass noise reduction (the default), the noise-reduced measureFiles
			// become pathsOfFilesToJoin -- CopyAllFiles's working_copies copies would never be
			// read, so CopyAllFiles must not get called at all. Proven by putting a plain FILE
			// at the working_copies path: CopyAllFiles's own CreateDirectory(working_copies)
			// call would throw immediately if it ever ran (checking folder state *after* the
			// call isn't enough -- MergeAudioFiles's own end-of-call cleanup removes
			// working_copies either way, whether or not it was ever used this call).
			var workingCopiesFolder = Path.Combine(Path.GetTempPath(), "working_copies");
			Environment.SetEnvironmentVariable("SINGLE_PASS_NOISE_REDUCTION", null);
			if (Directory.Exists(workingCopiesFolder))
				Directory.Delete(workingCopiesFolder, true);
			if (File.Exists(workingCopiesFolder))
				File.Delete(workingCopiesFolder);
			File.WriteAllText(workingCopiesFolder, "");
			try
			{
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

					Assert.That(() => ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
						output.Path, progress, settings, i => new ScriptLine()), Throws.Nothing);
					Assert.That(progress.ErrorEncountered, Is.False);
				}
			}
			finally
			{
				File.Delete(workingCopiesFolder);
			}
		}

		[Test]
		public void MergeAudioFiles_ReduceNoiseEnabledAllPauseTypesDisabledSinglePass_SkipsWastedNoiseReductionMeasurement()
		{
			// Regression test: under single-pass noise reduction with every pause type
			// unchecked, the per-clip noise-reduction measurement pass used to run anyway --
			// real ffmpeg work on every clip whose result nothing would ever read, since
			// double-pass is off (so it's never joined) and no pause type is enabled (so it's
			// never consulted for boundary constraining). Proven the same way as the
			// double-pass sibling test above: a plain FILE at the measure_silence path makes
			// its own CreateDirectory call throw immediately if the measurement loop ever
			// runs (checking folder state *after* the call isn't enough -- MergeAudioFiles's
			// own end-of-call cleanup removes measure_silence either way, whether or not it
			// was ever used this call).
			var measureSilenceFolder = Path.Combine(Path.GetTempPath(), "measure_silence");
			Environment.SetEnvironmentVariable("SINGLE_PASS_NOISE_REDUCTION", "yes");
			if (Directory.Exists(measureSilenceFolder))
				Directory.Delete(measureSilenceFolder, true);
			if (File.Exists(measureSilenceFolder))
				File.Delete(measureSilenceFolder);
			File.WriteAllText(measureSilenceFolder, "");
			try
			{
				using (var output = new TempFile())
				using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var clip2 = TempFile.FromResource(Resource1._1Channel, ".wav"))
				{
					var settings = new StubAudioNormalizationSettings { ReduceNoise = true };
					var progress = new StringBuilderProgress();

					Assert.That(() => ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
						output.Path, progress, settings, i => new ScriptLine()), Throws.Nothing);
					Assert.That(progress.ErrorEncountered, Is.False);
				}
			}
			finally
			{
				Environment.SetEnvironmentVariable("SINGLE_PASS_NOISE_REDUCTION", null);
				File.Delete(measureSilenceFolder);
			}
		}

		[Test]
		public void MergeAudioFiles_ReduceNoiseFailsPartwayThroughDoublePass_OriginalClipFileSurvivesUnmodified()
		{
			// An invalid (empty) second clip makes ffmpeg's noise-reduction pass fail when it
			// reaches that clip -- after clip1 has already been successfully noise-reduced.
			// This must not leave pathsOfFilesToJoin pointing at the ORIGINAL, unprotected clip
			// files: the "Fix Channel and Sample Rate" region that runs afterward edits every
			// path in pathsOfFilesToJoin in place (File.Move then re-encode), which would
			// corrupt/replace the user's actual permanent recording if it ran on clip1's real
			// path instead of a disposable copy.
			Environment.SetEnvironmentVariable("SINGLE_PASS_NOISE_REDUCTION", null);
			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.WithExtension(".wav"))
			{
				File.WriteAllBytes(clip2.Path, new byte[0]);
				var originalClip1Bytes = File.ReadAllBytes(clip1.Path);
				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(true, 0, 10),
					ReduceNoise = true,
				};
				var progress = new StringBuilderProgress();

				try
				{
					ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
						output.Path, progress, settings, i => new ScriptLine());
				}
				catch
				{
					// Whatever ultimately happens to this chapter's processing after the
					// failure isn't the point here -- only whether the original clip survives.
				}

				Assert.That(clip1.Path, Does.Exist,
					"The original clip file must never be moved, even when noise reduction fails partway through.");
				Assert.That(File.ReadAllBytes(clip1.Path), Is.EqualTo(originalClip1Bytes));
			}
		}

		[Test]
		public void MergeAudioFiles_ReduceNoiseFailsWithPauseTypeAlsoEnabled_WarnsAboutNoiseReductionNotPauseNormalization()
		{
			// An invalid (empty) second clip makes ffmpeg's noise-reduction pass fail when it
			// reaches that clip -- i.e., the failure happens in the per-clip noise-reduction
			// loop, BEFORE the pause-boundary loop even starts. With a pause type also enabled,
			// a single catch block covers both steps; it must attribute the failure to noise
			// reduction, not (misleadingly) to pause normalization.
			Environment.SetEnvironmentVariable("SINGLE_PASS_NOISE_REDUCTION", null);
			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.WithExtension(".wav"))
			{
				File.WriteAllBytes(clip2.Path, new byte[0]);
				var settings = new StubAudioNormalizationSettings
				{
					ClipPause = new PauseData(true, 0, 10),
					ReduceNoise = true,
				};
				var progress = new StringBuilderProgress();

				try
				{
					ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
						output.Path, progress, settings, i => new ScriptLine());
				}
				catch
				{
					// Whatever ultimately happens to this chapter's processing after the
					// failure isn't the point here -- only which message gets reported and
					// which settings get disabled as a result.
				}

				Assert.That(progress.Text, Does.Contain("Error trying to reduce noise in clips to be joined"));
				Assert.That(progress.Text, Does.Not.Contain("Error trying to normalize pauses in combined audio file"));

				// Guards against re-entering (and re-failing) the same broken pause-normalization
				// step for every remaining chapter in the book. ReduceNoise, by contrast, must NOT
				// be disabled here: PrepareChapterAudio's whole-chapter noise-reduction pass reads
				// that same shared setting independently, so clearing it would silently disable
				// that pass for every remaining chapter (and subsequent book) in the publish run.
				Assert.That(settings.ReduceNoise, Is.True);
				Assert.That(settings.ClipPause, Is.Null);
			}
		}

		[Test]
		public void MergeAudioFiles_NoiseReductionFails_RecordsFailureOnTracker()
		{
			Environment.SetEnvironmentVariable("SINGLE_PASS_NOISE_REDUCTION", null);
			var tracker = new NoiseReductionRetryTracker();

			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.WithExtension(".wav"))
			{
				File.WriteAllBytes(clip2.Path, new byte[0]);
				var settings = new StubAudioNormalizationSettings { ReduceNoise = true };
				var progress = new StringBuilderProgress();

				try
				{
					ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
						output.Path, progress, settings, i => new ScriptLine(), null, tracker);
				}
				catch
				{
					// Only whether the failure was recorded on the tracker is under test here.
				}

				Assert.That(progress.Text, Does.Contain("Error trying to reduce noise in clips to be joined"));
				// A single failure must not exhaust the tracker.
				Assert.That(tracker.ShouldAttempt, Is.True);
			}
		}

		[Test]
		public void MergeAudioFiles_NoiseReductionRetryTrackerExhausted_SkipsNoiseReductionAttempt()
		{
			// Regression test for the noise-reduction circuit breaker: once a tracker has
			// recorded MaxConsecutiveFailures consecutive failures, MergeAudioFiles must not
			// attempt noise reduction again -- regardless of how badly the next clip is
			// malformed -- until some other success resets the streak.
			Environment.SetEnvironmentVariable("SINGLE_PASS_NOISE_REDUCTION", null);
			var tracker = new NoiseReductionRetryTracker();
			for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures; i++)
				tracker.RecordFailure();

			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.WithExtension(".wav"))
			{
				File.WriteAllBytes(clip2.Path, new byte[0]);
				var settings = new StubAudioNormalizationSettings { ReduceNoise = true };
				var progress = new StringBuilderProgress();

				try
				{
					ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
						output.Path, progress, settings, i => new ScriptLine(), null, tracker);
				}
				catch
				{
					// Any failure here would have to come from some later, unrelated stage;
					// the only thing under test is whether noise reduction was even attempted.
				}

				Assert.That(progress.Text,
					Does.Not.Contain("Error trying to reduce noise in clips to be joined"));
				Assert.That(tracker.ShouldAttempt, Is.False);
			}
		}

		[Test]
		public void MergeAudioFiles_ThrowsPartwayThrough_CleansUpAllThreeScratchFolders()
		{
			// Regression test: a noise-reduction failure is handled internally (the pause-
			// normalization try/catch doesn't rethrow for it), but the same malformed clip then
			// makes the unguarded Fix Channel and Sample Rate region fail too, which DOES
			// propagate out of MergeAudioFiles. That used to skip two cleanups: the Fix Channel
			// and Sample Rate loop's own tmpPath in audio_processing was left stranded, and the
			// end-of-method cleanup of working_copies/measure_silence never ran at all, since it
			// sat after the throw. All three must now be gone, however the call ends.
			const string projectName = "MergeAudioFilesScratchCleanupOnFailureTest";
			Environment.SetEnvironmentVariable("SINGLE_PASS_NOISE_REDUCTION", null);

			using (var output = new TempFile())
			using (var clip1 = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var clip2 = TempFile.WithExtension(".wav"))
			{
				File.WriteAllBytes(clip2.Path, new byte[0]);
				var settings = new StubAudioNormalizationSettings { ReduceNoise = true };
				var progress = new StringBuilderProgress();

				try
				{
					ClipRepository.MergeAudioFiles(new List<string> { clip1.Path, clip2.Path },
						output.Path, progress, settings, i => new ScriptLine(), projectName);
				}
				catch
				{
					// The exception itself isn't under test -- only that it doesn't leave
					// scratch debris behind in a shared, machine-wide temp folder.
				}

				Assert.That(Directory.Exists(ClipRepository.AudioProcessingFolder(projectName)), Is.False);
				Assert.That(Directory.Exists(ClipRepository.WorkingCopiesFolder(projectName)), Is.False);
				Assert.That(Directory.Exists(ClipRepository.MeasureSilenceFolder(projectName)), Is.False);
			}
		}

		[Test]
		public void MergeAudioFiles_NoiseReductionSucceeds_ResetsTrackerFailureStreak()
		{
			var tracker = new NoiseReductionRetryTracker();
			for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures - 1; i++)
				tracker.RecordFailure();

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
					output.Path, progress, settings, i => new ScriptLine(), null, tracker);

				Assert.That(progress.ErrorEncountered, Is.False);

				// The streak must have been reset to zero, not merely left under the cap: it
				// must now tolerate a fresh run of MaxConsecutiveFailures - 1 more failures.
				for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures - 1; i++)
					tracker.RecordFailure();
				Assert.That(tracker.ShouldAttempt, Is.True);
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
			public MinimalPublishingInfo(string name = "Dummy")
			{
				Name = name;
			}

			public string Name { get; }
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

		private static PublishingModel CreateModel(string projectName = "Dummy") =>
			new PublishingModel(new MinimalPublishingInfo(projectName));

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

		[Test]
		public void FinalizeChapterAudio_NormalizeVolumeFails_RestoresChapterFileUnchanged()
		{
			// Bytes that are not audio make ffmpeg's loudnorm measurement pass fail after
			// FinalizeChapterAudio has already moved the chapter WAV aside to its temp path.
			// The failure must be swallowed (warned, not thrown), the chapter file must be
			// moved back to its original path with its content intact, and the encoder must
			// be handed that restored path rather than a missing file.
			using (var chapterWav = TempFile.WithExtension(".wav"))
			{
				var originalBytes = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x01, 0x02, 0x03, 0xFE, 0xFF };
				File.WriteAllBytes(chapterWav.Path, originalBytes);
				var encoder = new MockEncoder();
				var publisher = new AudiBiblePublishingMethod(encoder, "xyz");
				var model = CreateModel();
				model.NormalizeVolume = true;
				var progress = new StringBuilderProgress();

				Assert.That(() => publisher.FinalizeChapterAudio(Path.GetTempPath(), "Genesis", 1,
					chapterWav.Path, progress, model), Throws.Nothing);

				Assert.That(encoder.SourcePaths, Is.EqualTo(new[] { chapterWav.Path }));
				Assert.That(chapterWav.Path, Does.Exist);
				Assert.That(File.ReadAllBytes(chapterWav.Path), Is.EqualTo(originalBytes));
			}
		}

		[Test]
		public void PrepareChapterAudio_ReduceNoiseFails_RestoresChapterFileUnchanged()
		{
			// Bytes that are not audio make ffmpeg's noise-reduction step fail after
			// PrepareChapterAudio has already moved the chapter WAV aside to its temp path.
			// The failure must be swallowed (warned, not thrown), and the chapter file must be
			// moved back to its original path with its content intact.
			using (var chapterWav = TempFile.WithExtension(".wav"))
			{
				var originalBytes = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x01, 0x02, 0x03, 0xFE, 0xFF };
				File.WriteAllBytes(chapterWav.Path, originalBytes);
				var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
				var model = CreateModel();
				model.ReduceNoise = true;
				var progress = new StringBuilderProgress();

				var result = publisher.PrepareChapterAudio(chapterWav.Path, progress, model);

				Assert.That(result, Is.EqualTo(chapterWav.Path));
				Assert.That(chapterWav.Path, Does.Exist);
				Assert.That(File.ReadAllBytes(chapterWav.Path), Is.EqualTo(originalBytes));
			}
		}

		[Test]
		public void PrepareChapterAudio_PublishingModelHasProjectName_UsesProjectSpecificScratchFolder()
		{
			// Regression test: PrepareChapterAudio used to always scratch into a shared,
			// project-less %TEMP%\post_temp folder (a literal, independent of
			// ClipRepository's own project-specific scratch folders), so two concurrent
			// publishes -- even of different projects -- could collide there. It must now
			// use the same per-project folder ClipRepository's own passes use, derived from
			// the PublishingModel it already receives (PublishingInfo.Name) rather than a
			// separate parameter.
			const string projectName = "PrepareChapterAudioScratchFolderIsolationTest";
			var expectedFolder = ClipRepository.AudioProcessingFolder(projectName);
			if (Directory.Exists(expectedFolder))
				Directory.Delete(expectedFolder, true);

			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
				var model = CreateModel(projectName);
				model.ReduceNoise = true;
				var progress = new StringBuilderProgress();

				publisher.PrepareChapterAudio(chapterWav.Path, progress, model);

				Assert.That(expectedFolder, Does.Exist);
			}
		}

		[Test]
		public void FinalizeChapterAudio_PublishingModelHasProjectName_UsesProjectSpecificScratchFolder()
		{
			const string projectName = "FinalizeChapterAudioScratchFolderIsolationTest";
			var expectedFolder = ClipRepository.AudioProcessingFolder(projectName);
			if (Directory.Exists(expectedFolder))
				Directory.Delete(expectedFolder, true);

			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
				var model = CreateModel(projectName);
				model.NormalizeVolume = true;
				var progress = new StringBuilderProgress();

				publisher.FinalizeChapterAudio(Path.GetTempPath(), "Genesis", 1, chapterWav.Path,
					progress, model);

				Assert.That(expectedFolder, Does.Exist);
			}
		}

		[Test]
		public void PrepareChapterAudio_ReduceNoiseFailsConsecutivelyUpToCap_StopsAttemptingAfterCapIsReached()
		{
			// Regression test for the noise-reduction circuit breaker: a single publisher
			// instance (one publish run) must keep retrying noise reduction through up to
			// NoiseReductionRetryTracker.MaxConsecutiveFailures consecutive chapter failures,
			// then stop attempting it at all for the rest of the run.
			var originalBytes = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x01, 0x02, 0x03, 0xFE, 0xFF };
			var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
			var model = CreateModel();
			model.ReduceNoise = true;

			for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures; i++)
			{
				using (var chapterWav = TempFile.WithExtension(".wav"))
				{
					File.WriteAllBytes(chapterWav.Path, originalBytes);
					var progress = new StringBuilderProgress();

					publisher.PrepareChapterAudio(chapterWav.Path, progress, model);

					Assert.That(progress.Text, Does.Contain("Error when trying to Reduce Noise"),
						$"Attempt {i + 1} of {NoiseReductionRetryTracker.MaxConsecutiveFailures} should still have been tried.");
				}
			}

			// The breaker should now be open: a further attempt must not even try, no matter
			// how bad the input is.
			using (var chapterWav = TempFile.WithExtension(".wav"))
			{
				File.WriteAllBytes(chapterWav.Path, originalBytes);
				var progress = new StringBuilderProgress();

				var result = publisher.PrepareChapterAudio(chapterWav.Path, progress, model);

				Assert.That(result, Is.EqualTo(chapterWav.Path));
				Assert.That(progress.Text, Does.Not.Contain("Error when trying to Reduce Noise"));
			}
		}

		[Test]
		public void PrepareChapterAudio_ReduceNoiseSucceedsAfterFailures_ResetsStreakSoLaterFailuresAreStillRetried()
		{
			var originalBytes = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x01, 0x02, 0x03, 0xFE, 0xFF };
			var publisher = new AudiBiblePublishingMethod(new MockEncoder(), "xyz");
			var model = CreateModel();
			model.ReduceNoise = true;

			// One fewer than the cap, so the breaker has not opened yet.
			for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures - 1; i++)
			{
				using (var chapterWav = TempFile.WithExtension(".wav"))
				{
					File.WriteAllBytes(chapterWav.Path, originalBytes);
					publisher.PrepareChapterAudio(chapterWav.Path, new StringBuilderProgress(), model);
				}
			}

			// A real success in between.
			using (var chapterWav = TempFile.FromResource(Resource1._1Channel, ".wav"))
			{
				var progress = new StringBuilderProgress();
				publisher.PrepareChapterAudio(chapterWav.Path, progress, model);
				Assert.That(progress.ErrorEncountered, Is.False);
			}

			// The streak must have been reset to zero: another run of failures one short of
			// the cap must still be attempted, not silently skipped.
			for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures - 1; i++)
			{
				using (var chapterWav = TempFile.WithExtension(".wav"))
				{
					File.WriteAllBytes(chapterWav.Path, originalBytes);
					var progress = new StringBuilderProgress();
					publisher.PrepareChapterAudio(chapterWav.Path, progress, model);
					Assert.That(progress.Text, Does.Contain("Error when trying to Reduce Noise"));
				}
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
	/// Tests for ClipRepository.UseDoublePassNoiseReduction, the beta-testing override that
	/// lets a tester compare single- vs. double-pass noise reduction via an environment
	/// variable without a rebuild. Pure unit tests; no audio processing.
	/// </summary>
	[TestFixture]
	public class UseDoublePassNoiseReductionTests
	{
		private const string kEnvVar = "SINGLE_PASS_NOISE_REDUCTION";
		private string _origValue;

		[SetUp]
		public void SetUp()
		{
			_origValue = Environment.GetEnvironmentVariable(kEnvVar);
		}

		[TearDown]
		public void TearDown()
		{
			Environment.SetEnvironmentVariable(kEnvVar, _origValue);
		}

		[Test]
		public void UseDoublePassNoiseReduction_EnvironmentVariableNotSet_ReturnsTrue()
		{
			Environment.SetEnvironmentVariable(kEnvVar, null);

			Assert.That(ClipRepository.UseDoublePassNoiseReduction, Is.True);
		}

		[TestCase("yes")]
		[TestCase("YES")]
		[TestCase("true")]
		[TestCase("True")]
		public void UseDoublePassNoiseReduction_EnvironmentVariableRequestsSinglePass_ReturnsFalse(string value)
		{
			Environment.SetEnvironmentVariable(kEnvVar, value);

			Assert.That(ClipRepository.UseDoublePassNoiseReduction, Is.False);
		}

		[TestCase("")]
		[TestCase("no")]
		[TestCase("2")]
		public void UseDoublePassNoiseReduction_EnvironmentVariableSetToSomethingElse_ReturnsTrue(string value)
		{
			Environment.SetEnvironmentVariable(kEnvVar, value);

			Assert.That(ClipRepository.UseDoublePassNoiseReduction, Is.True);
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
