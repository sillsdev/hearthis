using System.Collections.Generic;
using System.IO;

namespace HearThis.Publishing
{
	/// <summary>
	/// Identifies which audio-processing step a failure happened in, so a single
	/// <see cref="AudioProcessingErrorTracker"/> can track each one's consecutive-failure streak
	/// independently.
	/// </summary>
	public enum AudioProcessingStep
	{
		NoiseReduction,
		PauseNormalization
	}

	/// <summary>
	/// Tracks consecutive failures of one or more audio-processing steps across a single publish
	/// run, so a caller can stop retrying a step after persistent failure without giving up on an
	/// isolated one. A failure is far more likely to be caused by one bad input (e.g., a corrupt
	/// or unusually short clip) than by a systemic problem (e.g., a broken ffmpeg install), so a
	/// single failure should not disable a step for the rest of the run -- but repeated,
	/// consecutive failures with no success in between are good evidence of a systemic problem,
	/// not worth re-attempting (and re-warning about) for every remaining chapter. Any success
	/// resets that step's streak: it is strong evidence the earlier failure(s) were specific to
	/// the input that failed, not systemic.
	/// </summary>
	// Public only because it appears in PublishAllBooks/PublishAllChapters's public signatures;
	// its members stay internal -- this type is not meant to be used outside this assembly.
	public sealed class AudioProcessingErrorTracker
	{
		internal const int MaxConsecutiveFailures = 3;

		private readonly Dictionary<AudioProcessingStep, int> _consecutiveFailures =
			new Dictionary<AudioProcessingStep, int>();

		/// <summary>
		/// Tests can set this to force every failure to count against the streak, bypassing the
		/// real clip-validity check in <see cref="RecordFailure"/>, which otherwise requires a
		/// real malformed WAV file on disk to exercise.
		/// </summary>
		internal bool AlwaysCountFailures { get; set; }

		internal bool ShouldAttempt(AudioProcessingStep step) =>
			GetConsecutiveFailures(step) < MaxConsecutiveFailures;

		internal void RecordSuccess(AudioProcessingStep step) => _consecutiveFailures[step] = 0;

		/// <summary>
		/// Records a failure of <paramref name="step"/>. When <paramref name="clipPath"/>
		/// identifies the specific file being processed at the point of failure and that file is
		/// itself an invalid clip (see <see cref="ClipRepository.IsInvalidClipFile"/>), the
		/// failure is attributed to that clip rather than counted toward the step's consecutive-
		/// failure streak: a clip known to be bad is not evidence of a systemic problem.
		/// </summary>
		internal void RecordFailure(AudioProcessingStep step, string clipPath = null)
		{
			// File.Exists guards against IsInvalidClipFile throwing for a path that no longer
			// exists by the time the failure is handled; a missing file can't be confirmed as
			// merely a bad clip, so it falls through and counts toward the streak.
			// deleteIfInvalid: false -- this check is incidental to tracking a failure that
			// already happened; it must not have the side effect of deleting the user's clip.
			if (!AlwaysCountFailures && clipPath != null && File.Exists(clipPath) &&
				ClipRepository.IsInvalidClipFile(clipPath, deleteIfInvalid: false))
				return;

			_consecutiveFailures[step] = GetConsecutiveFailures(step) + 1;
		}

		private int GetConsecutiveFailures(AudioProcessingStep step) =>
			_consecutiveFailures.TryGetValue(step, out var count) ? count : 0;
	}
}
