using System.IO;
using Palaso.CommandLineProcessing;
using Palaso.IO;
using Palaso.Progress.LogBox;

namespace HearThis.Publishing
{
	/// <summary>
	/// this is just to ensure that megavoice gets the precise bit/rate it wants
	/// </summary>
	public class WavEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			//todo: sox input.wav -b 16 -r 41k output.wav
			string args = string.Format("{0} -b 16 -r 44.1k {1}.wav", sourcePath, destPathWithoutExtension);
			string exePath = FileLocator.GetFileDistributedWithApplication("sox.exe");
			progress.WriteVerbose(exePath + " " + args);
			CommandLineRunner.Run(exePath, args, "", 60, progress);
		}

		public string FormatName
		{
			get { return "WAV"; }
		}
	}
}