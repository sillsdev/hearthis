using L10NSharp;
using SIL.CommandLineProcessing;
using SIL.IO;
using SIL.Progress;

namespace HearThis.Publishing
{
	/// <summary>
	/// this is just to ensure that megavoice gets the precise bit/rate it wants
	/// </summary>
	public class OpusEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("OpusEncoder.Progress", "Converting to opus format", "Appears in progress indicator"));
			string args = $"--bitrate 64 \"{sourcePath}\" \"{destPathWithoutExtension}.opus\"";
			string exePath = FileLocationUtilities.GetFileDistributedWithApplication("opusenc.1", "opusenc.exe");
			progress.WriteVerbose(exePath + " " + args);
			var result = CommandLineRunner.Run(exePath, args, "", 60 * 10, progress);
			if (result.StandardError.Contains("FAIL"))
				progress.WriteError(result.StandardError);
		}

		public string FormatName => "opus";
	}
}
