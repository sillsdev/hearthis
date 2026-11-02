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
	/// Tests for ClipRepository.ConstrainOuterEdge, used to constrain the leading/trailing
	/// silence of an audio file that has no neighboring file to combine with (a book's first
	/// chapter start, or last chapter end). These require FFmpeg from DistFiles.
	/// </summary>
	[TestFixture]
	public class ClipRepositoryConstrainOuterEdgeTests
	{
		private const double kToleranceInSeconds = 0.1;
		private string _tempFolder;

		[SetUp]
		public void SetUp()
		{
			LocalizationManager.StrictInitializationMode = false;
			_tempFolder = Path.Combine(Path.GetTempPath(),
				"HearThis ConstrainOuterEdge tests " + Guid.NewGuid());
			Directory.CreateDirectory(_tempFolder);
		}

		[TearDown]
		public void TearDown()
		{
			try { Directory.Delete(_tempFolder, true); }
			catch (IOException) { }
		}

		[Test]
		public void ConstrainOuterEdge_LeadingSilenceBelowMin_AddsBlankSpaceAndReturnsPositiveAdjustment()
		{
			using (var wavFile = TempFile.WithExtension(".wav"))
			{
				ClipRepositorySilenceDetectionTests.WriteWavFile(wavFile.Path, (0.2, true), (1.0, false));
				var progress = new StringBuilderProgress();

				var adjustment = ClipRepository.ConstrainOuterEdge(wavFile.Path, true, 0.6, 1.5,
					_tempFolder, progress);

				Assert.That(adjustment, Is.EqualTo(0.4).Within(kToleranceInSeconds));
				var newLeadingSilence = ClipRepository.GetDurationOfLeadingSilence(wavFile.Path, progress);
				Assert.That(newLeadingSilence, Is.EqualTo(0.6).Within(kToleranceInSeconds));
			}
		}

		[Test]
		public void ConstrainOuterEdge_TrailingSilenceAboveMax_RemovesBlankSpaceAndReturnsNegativeAdjustment()
		{
			using (var wavFile = TempFile.WithExtension(".wav"))
			{
				ClipRepositorySilenceDetectionTests.WriteWavFile(wavFile.Path, (1.0, false), (1.0, true));
				var progress = new StringBuilderProgress();

				var adjustment = ClipRepository.ConstrainOuterEdge(wavFile.Path, false, 0, 0.3,
					_tempFolder, progress);

				Assert.That(adjustment, Is.EqualTo(-0.7).Within(kToleranceInSeconds));
				var newTrailingSilence = ClipRepository.GetDurationOfTrailingSilence(wavFile.Path, progress);
				Assert.That(newTrailingSilence, Is.EqualTo(0.3).Within(kToleranceInSeconds));
			}
		}

		[Test]
		public void ConstrainOuterEdge_WithinRange_LeavesFileUnchangedAndReturnsZero()
		{
			using (var wavFile = TempFile.WithExtension(".wav"))
			{
				ClipRepositorySilenceDetectionTests.WriteWavFile(wavFile.Path, (0.5, true), (1.0, false));
				var progress = new StringBuilderProgress();

				var adjustment = ClipRepository.ConstrainOuterEdge(wavFile.Path, true, 0.2, 0.8,
					_tempFolder, progress);

				Assert.That(adjustment, Is.EqualTo(0));
			}
		}

		[Test]
		public void ConstrainOuterEdge_TrailingSilenceAboveMax_ReturnedAdjustmentMatchesActualMeasuredDurationChange()
		{
			// Same rationale as ClipRepositoryConstrainBoundaryTests.
			// ConstrainBoundary_CombinedSilenceAboveMax_ReturnedAdjustmentsMatchActualMeasuredDurationChange:
			// ffmpeg's -acodec copy trim rounds to whole demuxer packets, so the amount
			// actually removed is not always exactly the requested diff. kToleranceInSeconds
			// (0.1) is too loose to catch that drift, so this measures the real duration
			// change directly.
			using (var wavFile = TempFile.WithExtension(".wav"))
			{
				ClipRepositorySilenceDetectionTests.WriteWavFile(wavFile.Path, (1.0, false), (1.0, true));
				var originalDuration = GetWavDurationInSeconds(wavFile.Path);
				var progress = new StringBuilderProgress();

				var adjustment = ClipRepository.ConstrainOuterEdge(wavFile.Path, false, 0, 0.3,
					_tempFolder, progress);

				var actualAdjustment = GetWavDurationInSeconds(wavFile.Path) - originalDuration;
				Assert.That(adjustment, Is.EqualTo(actualAdjustment).Within(0.001));
			}
		}

		private static double GetWavDurationInSeconds(string path)
		{
			using (var reader = new WaveFileReader(path))
				return reader.TotalTime.TotalSeconds;
		}
	}
}
