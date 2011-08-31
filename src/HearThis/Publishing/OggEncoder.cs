using System.IO;
using Palaso.CommandLineProcessing;
using Palaso.IO;
using Palaso.Progress.LogBox;

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
			string args = string.Format("{0} -c 1 -t ogg {1}.ogg", sourcePath, destPathWithoutExtension);
			string exePath = FileLocator.GetFileDistributedWithApplication("sox.exe");
			progress.WriteVerbose(exePath + " " + args);
			CommandLineRunner.Run(exePath, args, "", 60, progress);
		}

		public string FormatName
		{
			get { return "ogg"; }
		}
	}
}