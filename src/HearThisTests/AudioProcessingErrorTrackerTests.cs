using System.IO;
using HearThis.Publishing;
using L10NSharp;
using NUnit.Framework;
using SIL.IO;

namespace HearThisTests
{
	[TestFixture]
	public class AudioProcessingErrorTrackerTests
	{
		[SetUp]
		public void SetUp()
		{
			LocalizationManager.StrictInitializationMode = false;
		}

		[Test]
		public void ShouldAttempt_NoFailuresRecordedYet_IsTrue()
		{
			Assert.That(new AudioProcessingErrorTracker().ShouldAttempt(AudioProcessingStep.NoiseReduction),
				Is.True);
		}

		[Test]
		public void ShouldAttempt_FewerThanMaxConsecutiveFailures_IsTrue()
		{
			var tracker = new AudioProcessingErrorTracker { AlwaysCountFailures = true };
			for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures - 1; i++)
				tracker.RecordFailure(AudioProcessingStep.NoiseReduction);

			Assert.That(tracker.ShouldAttempt(AudioProcessingStep.NoiseReduction), Is.True);
		}

		[Test]
		public void ShouldAttempt_MaxConsecutiveFailuresReached_IsFalse()
		{
			var tracker = new AudioProcessingErrorTracker { AlwaysCountFailures = true };
			for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures; i++)
				tracker.RecordFailure(AudioProcessingStep.NoiseReduction);

			Assert.That(tracker.ShouldAttempt(AudioProcessingStep.NoiseReduction), Is.False);
		}

		[Test]
		public void ShouldAttempt_MoreThanMaxConsecutiveFailures_StaysFalse()
		{
			var tracker = new AudioProcessingErrorTracker { AlwaysCountFailures = true };
			for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures + 5; i++)
				tracker.RecordFailure(AudioProcessingStep.NoiseReduction);

			Assert.That(tracker.ShouldAttempt(AudioProcessingStep.NoiseReduction), Is.False);
		}

		[Test]
		public void RecordSuccess_AfterFailures_ResetsStreakSoFullRetryBudgetIsAvailableAgain()
		{
			var tracker = new AudioProcessingErrorTracker { AlwaysCountFailures = true };
			for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures - 1; i++)
				tracker.RecordFailure(AudioProcessingStep.NoiseReduction);

			tracker.RecordSuccess(AudioProcessingStep.NoiseReduction);

			// A success resets the streak, so the tracker tolerates a fresh run of up to
			// MaxConsecutiveFailures - 1 more failures.
			for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures - 1; i++)
				tracker.RecordFailure(AudioProcessingStep.NoiseReduction);

			Assert.That(tracker.ShouldAttempt(AudioProcessingStep.NoiseReduction), Is.True);
		}

		[Test]
		public void RecordFailure_TwoSteps_EachStepsStreakTrackedIndependently()
		{
			var tracker = new AudioProcessingErrorTracker { AlwaysCountFailures = true };
			for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures; i++)
				tracker.RecordFailure(AudioProcessingStep.NoiseReduction);

			Assert.That(tracker.ShouldAttempt(AudioProcessingStep.NoiseReduction), Is.False);
			Assert.That(tracker.ShouldAttempt(AudioProcessingStep.PauseNormalization), Is.True);
		}

		[Test]
		public void RecordFailure_ClipPathIsInvalidWavFile_DoesNotCountTowardStreak()
		{
			var tracker = new AudioProcessingErrorTracker();
			using (var invalidClip = TempFile.WithExtension(".wav"))
			{
				File.WriteAllBytes(invalidClip.Path, new byte[] { 1, 2, 3 });

				for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures + 5; i++)
					tracker.RecordFailure(AudioProcessingStep.NoiseReduction, invalidClip.Path);

				Assert.That(tracker.ShouldAttempt(AudioProcessingStep.NoiseReduction), Is.True);
				// Checking a clip's validity for tracking purposes must never have the side
				// effect of deleting it.
				Assert.That(invalidClip.Path, Does.Exist);
			}
		}

		[Test]
		public void RecordFailure_ClipPathIsValidWavFile_CountsTowardStreak()
		{
			var tracker = new AudioProcessingErrorTracker();
			using (var validClip = TempFile.WithExtension(".wav"))
			{
				ClipRepositorySilenceDetectionTests.WriteWavFile(validClip.Path, (1.0, false));

				for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures; i++)
					tracker.RecordFailure(AudioProcessingStep.NoiseReduction, validClip.Path);

				Assert.That(tracker.ShouldAttempt(AudioProcessingStep.NoiseReduction), Is.False);
			}
		}

		[Test]
		public void RecordFailure_AlwaysCountFailuresTrue_CountsEvenWhenClipIsInvalid()
		{
			var tracker = new AudioProcessingErrorTracker { AlwaysCountFailures = true };
			using (var invalidClip = TempFile.WithExtension(".wav"))
			{
				File.WriteAllBytes(invalidClip.Path, new byte[] { 1, 2, 3 });

				for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures; i++)
					tracker.RecordFailure(AudioProcessingStep.NoiseReduction, invalidClip.Path);

				Assert.That(tracker.ShouldAttempt(AudioProcessingStep.NoiseReduction), Is.False);
			}
		}

		[Test]
		public void RecordFailure_NoClipPathProvided_CountsTowardStreak()
		{
			var tracker = new AudioProcessingErrorTracker();
			for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures; i++)
				tracker.RecordFailure(AudioProcessingStep.NoiseReduction);

			Assert.That(tracker.ShouldAttempt(AudioProcessingStep.NoiseReduction), Is.False);
		}

		[Test]
		public void RecordFailure_ClipPathDoesNotExist_CountsTowardStreak()
		{
			// If the file is gone, there's no way to check whether it was a bad clip, so the
			// failure must still count.
			var tracker = new AudioProcessingErrorTracker();
			var nonexistentPath = Path.Combine(Path.GetTempPath(), "does-not-exist-" + Path.GetRandomFileName());
			for (int i = 0; i < AudioProcessingErrorTracker.MaxConsecutiveFailures; i++)
				tracker.RecordFailure(AudioProcessingStep.NoiseReduction, nonexistentPath);

			Assert.That(tracker.ShouldAttempt(AudioProcessingStep.NoiseReduction), Is.False);
		}
	}
}
