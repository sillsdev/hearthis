using L10NSharp;
using Palaso.IO;
using Palaso.Progress;


namespace HearThis.Publishing
{
	public class FlacEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			progress.WriteMessage(LocalizationManager.GetString("FlacEncoder.Progress","   Converting to flac", "Appears in progress indicator"));
			//-f overwrite if already exists
			string arguments = string.Format("\"{0}\" -f -o \"{1}.flac\"", sourcePath, destPathWithoutExtension);
			LineRecordingRepository.RunCommandLine(progress, FileLocator.GetFileDistributedWithApplication(false, "flac.exe"), arguments);
		}

		public string FormatName
		{
			get { return "FLAC"; }
		}

		static public bool IsAvailable(out string message)
		{
			message = "";
			return true;
		}
	}
}