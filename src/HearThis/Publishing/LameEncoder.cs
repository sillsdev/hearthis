// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International. All Rights Reserved.
// <copyright from='2011' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.IO;
using L10NSharp;
using SIL.IO;
using SIL.Progress;

namespace HearThis.Publishing
{
	public class LameEncoder : IAudioEncoder
	{
		private static readonly string _pathToLAME;

		static LameEncoder()
		{
			// If this fails when running unit tests, make sure you've followed the instructions in
			// build\readme - making getDependencies script.txt
			// in order to download lame.exe
			_pathToLAME = FileLocationUtilities.GetFileDistributedWithApplication("lame", "lame.exe");
		}

		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress, int timeoutInSeconds)
		{
			var destPath = destPathWithoutExtension + ".mp3";
			if (File.Exists(destPath))
				RobustFile.Delete(destPath);

			progress.WriteMessage("   " + LocalizationManager.GetString("LameEncoder.Progress", "Converting to mp3", "Appears in progress indicator"));

			//-a down-mix to mono
			string arguments = string.Format($"-a \"{sourcePath}\" \"{destPath}\"");
			ClipRepository.RunCommandLine(progress, _pathToLAME, arguments, timeoutInSeconds);
		}

		public string FormatName => "mp3";
	}
}
