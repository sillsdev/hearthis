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
using System.Text.RegularExpressions;

namespace HearThis
{
	public static class Utils
	{
		//static readonly Regex s_replacePunctuationAndWhitespaceRegex = new Regex(@"\b[\w']+\b", RegexOptions.Compiled);
		static readonly Regex s_replacePunctuationAndWhitespaceRegex = new Regex(@"[^\w']+", RegexOptions.Compiled);

		public static string CreateDirectory(params string[] pathparts)
		{
			return Directory.CreateDirectory(Path.Combine(pathparts)).FullName;
		}

		public static bool AreWordsIdentical(string str1, string str2)
		{
			str1 = s_replacePunctuationAndWhitespaceRegex.Replace(str1, " ").Trim();
			str2 = s_replacePunctuationAndWhitespaceRegex.Replace(str2, " ").Trim();
			return str1 == str2;
		}
	}
}
