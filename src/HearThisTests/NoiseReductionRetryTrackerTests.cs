using HearThis.Publishing;
using NUnit.Framework;

namespace HearThisTests
{
	[TestFixture]
	public class NoiseReductionRetryTrackerTests
	{
		[Test]
		public void ShouldAttempt_NoFailuresRecordedYet_IsTrue()
		{
			Assert.That(new NoiseReductionRetryTracker().ShouldAttempt, Is.True);
		}

		[Test]
		public void ShouldAttempt_FewerThanMaxConsecutiveFailures_IsTrue()
		{
			var tracker = new NoiseReductionRetryTracker();
			for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures - 1; i++)
				tracker.RecordFailure();

			Assert.That(tracker.ShouldAttempt, Is.True);
		}

		[Test]
		public void ShouldAttempt_MaxConsecutiveFailuresReached_IsFalse()
		{
			var tracker = new NoiseReductionRetryTracker();
			for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures; i++)
				tracker.RecordFailure();

			Assert.That(tracker.ShouldAttempt, Is.False);
		}

		[Test]
		public void ShouldAttempt_MoreThanMaxConsecutiveFailures_StaysFalse()
		{
			var tracker = new NoiseReductionRetryTracker();
			for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures + 5; i++)
				tracker.RecordFailure();

			Assert.That(tracker.ShouldAttempt, Is.False);
		}

		[Test]
		public void RecordSuccess_AfterFailures_ResetsStreakSoFullRetryBudgetIsAvailableAgain()
		{
			var tracker = new NoiseReductionRetryTracker();
			for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures - 1; i++)
				tracker.RecordFailure();

			tracker.RecordSuccess();

			// A success resets the streak to zero, so the tracker should tolerate a fresh run
			// of up to MaxConsecutiveFailures - 1 more failures before giving up again.
			for (int i = 0; i < NoiseReductionRetryTracker.MaxConsecutiveFailures - 1; i++)
				tracker.RecordFailure();

			Assert.That(tracker.ShouldAttempt, Is.True);
		}
	}
}
