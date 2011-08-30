using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Palaso.IO;
using Palaso.Progress.LogBox;

namespace HearThis.Publishing
{
	public interface IPublisher
	{
	}

	public interface IAudioEncoder
	{
		void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress);
		string FormatName { get; }
	}

	public class FlacEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			//-f overwrite if already exists
			string arguments =string.Format("\"{0}\" -f -o \"{1}.flac\"", sourcePath, destPathWithoutExtension);
			SoundLibrary.RunCommandLine(progress, FileLocator.GetFileDistributedWithApplication(false, "flac.exe"), arguments);
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

	public class LameEncoder : IAudioEncoder
	{
		private static string _pathToLAME;

		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			LocateAndRememberLAMEPath();

			if(File.Exists(destPathWithoutExtension+".mp3"))
				File.Delete(destPathWithoutExtension+".mp3");

			//-a downmix to mono
			string arguments = string.Format("-a \"{0}\" \"{1}.mp3\"", sourcePath, destPathWithoutExtension);
			SoundLibrary.RunCommandLine(progress, _pathToLAME, arguments);
		}

		public string FormatName
		{
			get { return "mp3"; }
		}

		public static bool IsAvailable(out string message)
		{
			if(string.IsNullOrEmpty(LocateAndRememberLAMEPath()))
			{
				message = "To Make MP3s, first install \"Lame For Audacity\", if it is legal in your country.  Google \"Lame For Audacity\" to get an up-to-date link";
				return false;
			}
			message = "";
			return true;
		}

		/// <summary>
		/// Find the path to ffmpeg, and remember it (some apps (like SayMore) call ffmpeg a lot)
		/// </summary>
		/// <returns></returns>
		private static string LocateAndRememberLAMEPath()
		{
			if (null != _pathToLAME) //NO! string.empty means we looked and didn't find: string.IsNullOrEmpty(s_ffmpegLocation))
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
			string withApplicationDirectory;

			//nb: this is sensitive to whether we are compiled against win32 or not,
			//not just the host OS, as you might guess.
			var pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);


			var progFileDirs = new List<string>()
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

	public class BunchOfFilesPublisher : IPublisher
	{
		private readonly IAudioEncoder _encoder;

		public BunchOfFilesPublisher(IAudioEncoder encoder)
		{
			_encoder = encoder;
		}
	}



}
