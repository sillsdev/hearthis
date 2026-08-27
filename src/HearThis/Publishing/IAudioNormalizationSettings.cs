namespace HearThis.Publishing
{
	/// <summary>
	/// Audio Normalization and Noise Reduction options.
	/// </summary>
	public interface IAudioNormalizationSettings
	{
		bool NormalizeVolume { get; set; }
		bool ReduceNoise { get; set; }
		PauseData ClipPause { get; set; }
		PauseData ParagraphPause { get; set; }
		PauseData SectionPause { get; set; }
		PauseData ChapterPause { get; set; }
	}
}
