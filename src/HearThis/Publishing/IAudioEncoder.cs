using Palaso.Progress;

namespace HearThis.Publishing
{
	public interface IAudioEncoder
	{
		void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress);
		string FormatName { get; }
	}
}