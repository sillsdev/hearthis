using System;

namespace HearThis.Audio
{
	public interface IAudioPlayer : IDisposable
	{
		void LoadFile(string path);
		void Play();
		void Stop();
		TimeSpan CurrentPosition { get; set; }
		TimeSpan StartPosition { get; set; }
		TimeSpan EndPosition { get; set; }
	}
}
