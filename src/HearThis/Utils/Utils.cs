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

namespace HearThis
{
	public static class Utils
	{
		public static string CreateDirectory(params string[] pathparts)
		{
			return Directory.CreateDirectory(Path.Combine(pathparts)).FullName;
		}
	}
}
