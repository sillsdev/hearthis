using System.IO;
using Palaso.CommandLineProcessing;
using Palaso.IO;
using Palaso.Progress;

namespace HearThis.Publishing
{
	/// <summary>
	/// this is just to ensure that megavoice gets the precise bit/rate it wants
	/// </summary>
	public class OggEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			progress.WriteMessage("   Converting to ogg format");
			string args = string.Format("-c 1 {0} \"{1}.ogg\"", sourcePath, destPathWithoutExtension);
			string exePath = FileLocator.GetFileDistributedWithApplication("sox","sox.exe");
			progress.WriteVerbose(exePath + " " + args);
			var result =CommandLineRunner.Run(exePath, args, "", 60, progress);
			if(result.StandardError.Contains("FAIL"))
				progress.WriteError(result.StandardError);
		}

		public string FormatName
		{
			get { return "ogg"; }
		}
	}
}