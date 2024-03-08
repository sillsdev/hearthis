using System;
using System.IO;
using L10NSharp;
using SIL.CommandLineProcessing;
using SIL.IO;
using SIL.Progress;

namespace HearThis.Publishing
{
	public class OpusEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("OpusEncoder.Progress", "Converting to OGG Opus format", "Appears in progress indicator"));

			// Modify this line to specify the full path to opusenc.exe
			string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HearThis.Publishing", "opusenc.exe");

			// Specify any additional arguments here
			string args = $"--bitrate 64 \"{sourcePath}\" \"{destPathWithoutExtension}.opus\"";

			progress.WriteVerbose(exePath + " " + args);
			var result = CommandLineRunner.Run(exePath, args, "", 60 * 10, progress);
			if (result.StandardError.Contains("FAIL"))
				progress.WriteError(result.StandardError);
		}

		public string FormatName => "opus";
	}
}
