// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using L10NSharp;
using Palaso.Progress;

namespace HearThis.Publishing
{
	public class LameEncoder : IAudioEncoder
	{
		private static string _pathToLAME;

		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			LocateAndRememberLAMEPath();

			if (File.Exists(destPathWithoutExtension + ".mp3"))
				File.Delete(destPathWithoutExtension + ".mp3");

			progress.WriteMessage(LocalizationManager.GetString("LameEncoder.Progress"," Converting to mp3", "Appears in progress indicator"));

			//-a downmix to mono
			string arguments = string.Format("-a \"{0}\" \"{1}.mp3\"", sourcePath, destPathWithoutExtension);
			ClipRepository.RunCommandLine(progress, _pathToLAME, arguments);
		}

		public string FormatName
		{
			get { return "mp3"; }
		}

		public static bool IsAvailable(out string message)
		{
			if (string.IsNullOrEmpty(LocateAndRememberLAMEPath()))
			{
				message = LocalizationManager.GetString("LameEncoder.Installation","To Make MP3s, first install \"Lame For Audacity\", if it is legal in your country.  Google \"Lame For Audacity\" to get an up-to-date link", "");
				return false;
			}
			message = "";
			return true;
		}

		/// <summary>
		/// Find the path to LAME)
		/// </summary>
		/// <returns></returns>
		private static string LocateAndRememberLAMEPath()
		{
			if (null != _pathToLAME) // string.empty means we looked for LAME previously and didn't find it)
				return _pathToLAME;
			_pathToLAME = LocateLAME();
			return _pathToLAME;
		}

		/// <summary>
		/// </summary>
		/// <returns>the path, if found, else null</returns>
		static private string LocateLAME()
		{
#if !MONO
			//nb: this is sensitive to whether we are compiled against win32 or not,
			//not just the host OS, as you might guess.
			var pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);


			var progFileDirs = new List<string>
				{
					pf.Replace(" (x86)", ""),			//native (win32 or 64, depending)
					pf.Replace(" (x86)", "")+" (x86)"	//win32
				};


			foreach (var path in progFileDirs)
			{
				var exePath = (Path.Combine(path, "LAME for Audacity/lame.exe"));
				if (File.Exists(exePath))
					return exePath;
			}
			return string.Empty;
#endif
		}
	}
}