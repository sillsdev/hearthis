using System;
using System.IO;
using HearThis.Publishing;
using L10NSharp;
using NAudio.Wave;
using NUnit.Framework;
using SIL.IO;
using SIL.Progress;

namespace HearThisTests
{
	/// <summary>
	/// Tests for ClipRepository.ConstrainBoundary, the shared algorithm used to constrain the
	/// silence at a boundary between two adjacent audio files (clips, or -- once wired up in a
	/// later task -- chapters). These require FFmpeg from DistFiles.
	/// </summary>
	[TestFixture]
	public class ClipRepositoryConstrainBoundaryTests
	{
		private const double kToleranceInSeconds = 0.1;
		private string _tempFolder;

		[SetUp]
		public void SetUp()
		{
			LocalizationManager.StrictInitializationMode = false;
			_tempFolder = Path.Combine(Path.GetTempPath(),
				"HearThis ConstrainBoundary tests " + Guid.NewGuid());
			Directory.CreateDirectory(_tempFolder);
		}

		[TearDown]
		public void TearDown()
		{
			try { Directory.Delete(_tempFolder, true); }
			catch (IOException) { }
		}

		[Test]
		public void ConstrainBoundary_CombinedSilenceBelowMin_AddsBlankSpaceToStartOfCurrentAndReturnsAdjustment()
		{
			using (var previous = TempFile.WithExtension(".wav"))
			using (var current = TempFile.WithExtension(".wav"))
			{
				// previous: 1.0s tone, 0.6s trailing silence
				ClipRepositorySilenceDetectionTests.WriteWavFile(previous.Path, (1.0, false), (0.6, true));
				// current: 0.4s leading silence, 1.0s tone
				ClipRepositorySilenceDetectionTests.WriteWavFile(current.Path, (0.4, true), (1.0, false));

				var pause = new PauseData(true, 1.5, 3.0); // combined 1.0s is below min 1.5s
				var progress = new StringBuilderProgress();

				var (previousAdjustment, currentAdjustment) = ClipRepository.ConstrainBoundary(
					previous.Path, current.Path, pause, _tempFolder, progress);

				Assert.That(previousAdjustment, Is.EqualTo(0));
				Assert.That(currentAdjustment, Is.EqualTo(0.5).Within(kToleranceInSeconds));
				var newLeadingSilence = ClipRepository.GetDurationOfLeadingSilence(current.Path, progress);
				Assert.That(newLeadingSilence, Is.EqualTo(0.9).Within(kToleranceInSeconds));
			}
		}

		[Test]
		public void ConstrainBoundary_CombinedSilenceAboveMax_TrimsBothSidesProportionallyAndReturnsAdjustments()
		{
			using (var previous = TempFile.WithExtension(".wav"))
			using (var current = TempFile.WithExtension(".wav"))
			{
				// previous: 1.0s tone, 0.6s trailing silence
				ClipRepositorySilenceDetectionTests.WriteWavFile(previous.Path, (1.0, false), (0.6, true));
				// current: 0.4s leading silence, 1.0s tone
				ClipRepositorySilenceDetectionTests.WriteWavFile(current.Path, (0.4, true), (1.0, false));

				var pause = new PauseData(true, 0, 0.5); // combined 1.0s is above max 0.5s
				var progress = new StringBuilderProgress();

				var (previousAdjustment, currentAdjustment) = ClipRepository.ConstrainBoundary(
					previous.Path, current.Path, pause, _tempFolder, progress);

				// takeOffAll = 0.5; ratioPreviousToCurrent = 0.6 / (0.6 + 0.4) = 0.6
				Assert.That(previousAdjustment, Is.EqualTo(-0.3).Within(kToleranceInSeconds));
				Assert.That(currentAdjustment, Is.EqualTo(-0.2).Within(kToleranceInSeconds));
				var newTrailingSilence = ClipRepository.GetDurationOfTrailingSilence(previous.Path, progress);
				Assert.That(newTrailingSilence, Is.EqualTo(0.3).Within(kToleranceInSeconds));
				var newLeadingSilence = ClipRepository.GetDurationOfLeadingSilence(current.Path, progress);
				Assert.That(newLeadingSilence, Is.EqualTo(0.2).Within(kToleranceInSeconds));
			}
		}

		[Test]
		public void ConstrainBoundary_CombinedSilenceWithinRange_LeavesBothFilesUnchangedAndReturnsZero()
		{
			using (var previous = TempFile.WithExtension(".wav"))
			using (var current = TempFile.WithExtension(".wav"))
			{
				ClipRepositorySilenceDetectionTests.WriteWavFile(previous.Path, (1.0, false), (0.6, true));
				ClipRepositorySilenceDetectionTests.WriteWavFile(current.Path, (0.4, true), (1.0, false));

				var pause = new PauseData(true, 0.5, 1.5); // combined 1.0s is within [0.5, 1.5]
				var progress = new StringBuilderProgress();

				var (previousAdjustment, currentAdjustment) = ClipRepository.ConstrainBoundary(
					previous.Path, current.Path, pause, _tempFolder, progress);

				Assert.That(previousAdjustment, Is.EqualTo(0));
				Assert.That(currentAdjustment, Is.EqualTo(0));
			}
		}

		[Test]
		public void ConstrainBoundary_SeparateMeasurePathsProvided_MeasuresFromMeasurePathsNotJoinPaths()
		{
			using (var previousJoin = TempFile.WithExtension(".wav"))
			using (var currentJoin = TempFile.WithExtension(".wav"))
			using (var previousMeasure = TempFile.WithExtension(".wav"))
			using (var currentMeasure = TempFile.WithExtension(".wav"))
			{
				// The "join" files look like they need no adjustment (combined 2.0s, within
				// [1.5, 3.0])...
				ClipRepositorySilenceDetectionTests.WriteWavFile(previousJoin.Path, (1.0, false), (1.0, true));
				ClipRepositorySilenceDetectionTests.WriteWavFile(currentJoin.Path, (1.0, true), (1.0, false));
				// ...but the "measure" files (e.g. noise-reduced copies) report much less
				// silence, which should be what actually drives the decision.
				ClipRepositorySilenceDetectionTests.WriteWavFile(previousMeasure.Path, (1.0, false), (0.2, true));
				ClipRepositorySilenceDetectionTests.WriteWavFile(currentMeasure.Path, (0.2, true), (1.0, false));

				var pause = new PauseData(true, 1.5, 3.0);
				var progress = new StringBuilderProgress();

				var (previousAdjustment, currentAdjustment) = ClipRepository.ConstrainBoundary(
					previousJoin.Path, currentJoin.Path, pause, _tempFolder, progress,
					previousMeasure.Path, currentMeasure.Path);

				// Combined measured silence is ~0.4s, below min 1.5s, so 1.1s should be added
				// to the start of the current *join* file.
				Assert.That(previousAdjustment, Is.EqualTo(0));
				Assert.That(currentAdjustment, Is.EqualTo(1.1).Within(kToleranceInSeconds));
			}
		}

		[Test]
		public void ConstrainBoundary_CombinedSilenceAboveMax_ReturnedAdjustmentsMatchActualMeasuredDurationChange()
		{
			// ffmpeg's -acodec copy trim (used by RemoveEndingBlankSpace/RemoveBeginningBlankSpace)
			// rounds to whole demuxer packets, so the amount actually removed from a file is not
			// always exactly the requested amount. The returned adjustment must reflect what was
			// actually applied -- otherwise anything built from it (verse-index timing) drifts
			// by the rounding error. kToleranceInSeconds (0.1) is too loose to catch that drift
			// (typically tens of milliseconds), so this test measures each file's real duration
			// change directly instead of relying on the requested takeOff amounts.
			using (var previous = TempFile.WithExtension(".wav"))
			using (var current = TempFile.WithExtension(".wav"))
			{
				ClipRepositorySilenceDetectionTests.WriteWavFile(previous.Path, (1.0, false), (0.6, true));
				ClipRepositorySilenceDetectionTests.WriteWavFile(current.Path, (0.4, true), (1.0, false));

				var originalPreviousDuration = GetWavDurationInSeconds(previous.Path);
				var originalCurrentDuration = GetWavDurationInSeconds(current.Path);

				var pause = new PauseData(true, 0, 0.5); // combined 1.0s is above max 0.5s
				var progress = new StringBuilderProgress();

				var (previousAdjustment, currentAdjustment) = ClipRepository.ConstrainBoundary(
					previous.Path, current.Path, pause, _tempFolder, progress);

				var actualPreviousAdjustment = GetWavDurationInSeconds(previous.Path) - originalPreviousDuration;
				var actualCurrentAdjustment = GetWavDurationInSeconds(current.Path) - originalCurrentDuration;

				Assert.That(previousAdjustment, Is.EqualTo(actualPreviousAdjustment).Within(0.001));
				Assert.That(currentAdjustment, Is.EqualTo(actualCurrentAdjustment).Within(0.001));
			}
		}

		private static double GetWavDurationInSeconds(string path)
		{
			using (var reader = new WaveFileReader(path))
				return reader.TotalTime.TotalSeconds;
		}
	}
}
