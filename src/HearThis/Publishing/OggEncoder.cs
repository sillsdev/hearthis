// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
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
	/// this is just to ensure that megavoice gets the precise bit/rate it wants
	/// </summary>
	public class OggEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress, int timeoutInSeconds)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("OggEncoder.Progress", "Converting to Ogg Vorbis format", "Appears in progress indicator"));
			string args = $"-c 1 \"{sourcePath}\" \"{destPathWithoutExtension}.ogg\"";
			string exePath = FileLocationUtilities.GetFileDistributedWithApplication("sox", "sox.exe");
			progress.WriteVerbose(exePath + " " + args);
			var result =CommandLineRunner.Run(exePath, args, "", timeoutInSeconds, progress);
			if (result.StandardError.Contains("FAIL"))
				progress.WriteError(result.StandardError);
		}

		public string FormatName => "ogg";
	}
}
