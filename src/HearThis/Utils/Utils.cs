// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2015' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Windows.Forms;
using L10NSharp;
using SIL.IO;
using SIL.Media;
using static System.String;

namespace HearThis
{
	public static class Utils
	{
		public static string CreateDirectory(params string[] pathParts)
		{
			return Directory.CreateDirectory(Path.Combine(pathParts)).FullName;
		}

		public static ISimpleAudioSession GetPlayer(Form parent, string path)
		{
			while (true)
			{
				try
				{
					return AudioFactory.CreateAudioSession(path);
				}
				catch (Exception e)
				{
					string msg = Format(LocalizationManager.GetString("Program.FailedToCreateAudioSession",
						"The following error occurred while preparing an audio session to be able to play back recordings:\r\n{0}\r\n" +
						"{1} will not work correctly without speakers. Ensure that your speakers are enabled and functioning properly.\r\n" +
						"Would you like {1} to try again?"), e.Message, Program.kProduct);
					if (DialogResult.No == MessageBox.Show(parent, msg, Program.kProduct, MessageBoxButtons.YesNo))
						return null;
				}
			}
		}
	}

	// ENHANCE: Modify the Move method in Libpalaso's RobustFile class to take this optional third parameter
	// and then look at the myriad places where it's used that could be simplified by removing the code to
	// check for and delete the destination file.
	public static class RobustFileAddOn
	{
		public static void Move(string sourceFileName, string destFileName, bool overWrite = false)
		{
			if (overWrite && RobustFile.Exists(destFileName))
				RobustFile.Delete(destFileName);
			RobustFile.Move(sourceFileName, destFileName);
		}

		public static bool IsWritable(string file, out Exception error)
		{
			error = null;

			if (!File.Exists(file))
			{
				var dir = Path.GetDirectoryName(file);
				return IsDirectoryWritable(dir, out error);
			}

			try
			{
				using (var fs = new FileStream(file, FileMode.Open))
					return fs.CanWrite; // Always true
			}
			catch (Exception e)
			{
				error = e;
				return false;
			}
		}

		private static bool IsDirectoryWritable(string dirPath, out Exception error)
		{
			if (dirPath == null)
			{
				// Though perhaps not strictly true, for our purposes if the file does not have
				// a containing directory, we consider it not writable.
				error = new ArgumentNullException(nameof(dirPath));
				return false;
			}

			if (!Directory.Exists(dirPath))
			{
				// Though perhaps not strictly true, for our purposes if the file's containing
				// directory does not exist, we consider it not writable.
				error = new DirectoryNotFoundException($"Directory {dirPath} does not exist.");
				return false;
			}

			error = null;
			try
			{
				using (File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
					return true;
			}
			catch (Exception e)
			{
				error = e;
				return false;
			}
		}
	}
}
