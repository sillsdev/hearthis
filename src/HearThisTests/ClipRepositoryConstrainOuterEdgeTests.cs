using System;
using System.IO;
using HearThis.Publishing;
using L10NSharp;
using NUnit.Framework;
using SIL.IO;
using SIL.Media;
using SIL.Progress;
using static SIL.IO.FileLocationUtilities;

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
			FFmpegRunner.FFmpegLocation = GetFileDistributedWithApplication("FFmpeg", "ffmpeg.exe");
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
	}
}
