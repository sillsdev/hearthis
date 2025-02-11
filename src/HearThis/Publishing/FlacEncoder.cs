// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2024-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using L10NSharp;
using SIL.IO;
using SIL.Progress;

namespace HearThis.Publishing
{
	public class FlacEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress, int timeoutInSeconds)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("FlacEncoder.Progress", "Converting to flac", "Appears in progress indicator"));
			//-f overwrite if already exists
			string arguments = $"\"{sourcePath}\" -f -o \"{destPathWithoutExtension}.flac\"";
			ClipRepository.RunCommandLine(progress, FileLocationUtilities.GetFileDistributedWithApplication(false, "flac.exe"), arguments, timeoutInSeconds);
		}

		public string FormatName => "FLAC";
	}
}
