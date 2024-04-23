// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International. All Rights Reserved.
// <copyright from='2024' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using L10NSharp;
using SIL.CommandLineProcessing;
using SIL.IO;
using SIL.Progress;

namespace HearThis.Publishing
{
	/// <summary>
	/// This encoder converts audio files to OGG Opus. Opus is great for transmitting speech and
	/// music with minimal latency. 
	/// </summary>
	/// <remarks>The opusenc.exe command-line tool was downloaded from
	/// https://opus-codec.org/downloads/</remarks>
	public class OpusEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress, int timeoutInSeconds)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("OpusEncoder.Progress", "Converting to Ogg Opus format", "Appears in progress indicator"));

			string exePath = FileLocationUtilities.GetFileDistributedWithApplication("opusenc", "opusenc.exe");

			string args = $"--bitrate 64 \"{sourcePath}\" \"{destPathWithoutExtension}.opus\"";

			progress.WriteVerbose(exePath + " " + args);

			var result = CommandLineRunner.Run(exePath, args, "", timeoutInSeconds, progress);
			if (result.StandardError.Contains("FAIL"))
				progress.WriteError(result.StandardError);
		}
		public string FormatName => "opus";
	}
}
