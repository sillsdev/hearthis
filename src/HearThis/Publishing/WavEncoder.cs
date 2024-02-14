// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International. All Rights Reserved.
// <copyright from='2011' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using SIL.CommandLineProcessing;
using SIL.IO;
using SIL.Progress;

namespace HearThis.Publishing
{
	/// <summary>
	/// This is just to ensure that megavoice gets the precise bit/rate it wants
	/// </summary>
	public class WavEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			//todo: sox input.wav -b 16 -r 41k output.wav
			string args = $"\"{sourcePath}\" -b 16 -c 1 -r 44.1k \"{destPathWithoutExtension}.wav\"";
			string exePath = FileLocationUtilities.GetFileDistributedWithApplication("sox","sox.exe");
			progress.WriteVerbose(exePath + " " + args);
			CommandLineRunner.Run(exePath, args, "", 60 * 10, progress);
		}

		public string FormatName => "WAV";
	}
}
