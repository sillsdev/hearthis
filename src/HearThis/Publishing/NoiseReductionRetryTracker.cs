namespace HearThis.Publishing
{
	/// <summary>
	/// Tracks consecutive noise-reduction failures across a single publish run, so a caller can
	/// stop retrying after persistent failure without giving up on an isolated one. A noise-
	/// reduction failure is far more likely to be caused by one bad input (e.g., a corrupt or
	/// unusually short clip) than by a systemic problem (e.g., a broken ffmpeg install), so a
	/// single failure should not disable the feature for the rest of the run -- but repeated,
	/// consecutive failures with no success in between are good evidence of a systemic problem,
	/// not worth re-attempting (and re-warning about) for every remaining chapter. Any success
	/// resets the streak: it is strong evidence the earlier failure(s) were specific to the
	/// input that failed, not systemic.
	/// </summary>
	// Public only because it appears in PublishAllBooks/PublishAllChapters's public signatures;
	// its members stay internal -- this type is not meant to be used outside this assembly.
	public sealed class NoiseReductionRetryTracker
	{
		internal const int MaxConsecutiveFailures = 3;

		private int _consecutiveFailures;

		internal bool ShouldAttempt => _consecutiveFailures < MaxConsecutiveFailures;

		internal void RecordSuccess() => _consecutiveFailures = 0;

		internal void RecordFailure() => _consecutiveFailures++;
	}
}
