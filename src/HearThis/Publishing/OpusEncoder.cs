using System;
using System.IO;
using L10NSharp;
using SIL.CommandLineProcessing;
using SIL.IO;
using SIL.Progress;
using System.Net;

namespace HearThis.Publishing
{
	public class OpusEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("OpusEncoder.Progress", "Converting to OGG Opus format", "Appears in progress indicator"));

			// URL to download opusenc.exe
			string downloadUrl = "https://example.com/opusenc.exe";

			// Directory to save opusenc.exe
			string downloadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HearThis.Publishing");

			// Ensure the directory exists
			Directory.CreateDirectory(downloadDirectory);

			// Path to save opusenc.exe
			string exePath = Path.Combine(downloadDirectory, "opusenc.exe");

			// Download opusenc.exe if not already downloaded
			if (!File.Exists(exePath))
			{
				using (WebClient webClient = new WebClient())
				{
					progress.WriteMessage("Downloading opusenc.exe...");
					webClient.DownloadFile(downloadUrl, exePath);
				}
			}

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
