// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International. All Rights Reserved.
// <copyright from='2011' to='2024' company='SIL International'>
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
	public class AudiBibleEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress, int timeoutInSeconds)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("AudiBibleEncoder.Progress", "Converting to AudiBible format", "Appears in progress indicator"));
			string args = string.Format("-c 1 {0} -r 22500 \"{1}.wav\"", sourcePath, destPathWithoutExtension);
			string exePath = FileLocationUtilities.GetFileDistributedWithApplication("sox", "sox.exe");
			progress.WriteVerbose(exePath + " " + args);
			var result = CommandLineRunner.Run(exePath, args, "", timeoutInSeconds, progress);
			if (result.StandardError.Contains("FAIL"))
				progress.WriteError(result.StandardError);
		}

		public string FormatName => "AudiBible";
	}
}
