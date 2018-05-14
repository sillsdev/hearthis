// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International. All Rights Reserved.
// <copyright from='2011' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
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
			_pathToLAME = FileLocationUtilities.GetFileDistributedWithApplication("lame", "lame.exe");
		}

		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			var destPath = destPathWithoutExtension + ".mp3";
			if (File.Exists(destPath))
				File.Delete(destPath);

			progress.WriteMessage(LocalizationManager.GetString("LameEncoder.Progress"," Converting to mp3", "Appears in progress indicator"));

			//-a downmix to mono
			string arguments = string.Format($"-a \"{sourcePath}\" \"{destPath}\"");
			ClipRepository.RunCommandLine(progress, _pathToLAME, arguments);
		}

		public string FormatName => "mp3";
	}
}