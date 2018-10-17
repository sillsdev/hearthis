// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International. All Rights Reserved.
// <copyright from='2015' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.IO;
using SIL.IO;

namespace HearThis
{
	public static class Utils
	{
		public static string CreateDirectory(params string[] pathparts)
		{
			return Directory.CreateDirectory(Path.Combine(pathparts)).FullName;
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
