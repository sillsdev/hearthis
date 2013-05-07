using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L10NSharp;
using Palaso.CommandLineProcessing;
using Palaso.IO;
using Palaso.Progress;

namespace HearThis.Publishing
{
	public class AudiBibleEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			progress.WriteMessage(LocalizationManager.GetString("AudiBibleEncoder.Progress", "   Converting to AudiBible format", "Appears in progress indicator"));
			string args = string.Format("-c 1 {0} -r 22500 \"{1}.wav\"", sourcePath, destPathWithoutExtension);
			string exePath = FileLocator.GetFileDistributedWithApplication("sox", "sox.exe");
			progress.WriteVerbose(exePath + " " + args);
			var result = CommandLineRunner.Run(exePath, args, "", 60, progress);
			if (result.StandardError.Contains("FAIL"))
				progress.WriteError(result.StandardError);
		}

		public string FormatName
		{
			get { return "AudiBible"; }
		}
	}
}
