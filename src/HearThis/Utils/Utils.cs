// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2019, SIL International. All Rights Reserved.
// <copyright from='2015' to='2019' company='SIL International'>
//		Copyright (c) 2019, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
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
		public static string CreateDirectory(params string[] pathparts)
		{
			return Directory.CreateDirectory(Path.Combine(pathparts)).FullName;
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
	}
}
