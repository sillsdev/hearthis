// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
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
			ClipRepository.RunCommandLine(progress, FileLocator.GetFileDistributedWithApplication(false, "flac.exe"), arguments);
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