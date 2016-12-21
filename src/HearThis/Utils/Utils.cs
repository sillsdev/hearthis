// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2016, SIL International. All Rights Reserved.
// <copyright from='2015' to='2016' company='SIL International'>
//		Copyright (c) 2016, SIL International. All Rights Reserved.
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
		public static string CreateDirectory(params string[] pathparts)
		{
			return Directory.CreateDirectory(Path.Combine(pathparts)).FullName;
		}

#if DEBUG
		// This regex is intended to match any sequence of spaces or punctuation that is not word medial. It isn't quite right
		// yet (see commented out AreWordsIdentical tests).
		static readonly Regex s_replacePunctuationAndWhitespaceRegex = new Regex(@"((^|\s)[^\w']*)|([^\w']*($|\s))", RegexOptions.Compiled);

		/// <summary>
		/// This is not currently used in HearThis. It was written with the intention of using it in
		/// RecordingToolControl.GetIsRecordingInSynchWithText instead of a simple string equality check.
		/// However, it's not very clear that this optimization is actually safe, given all the ways punctuation
		/// might actually influence intonation, pauses, etc. in the recording.
		/// </summary>
		public static bool AreWordsIdentical(string str1, string str2)
		{
			str1 = s_replacePunctuationAndWhitespaceRegex.Replace(str1, " ").Trim();
			str2 = s_replacePunctuationAndWhitespaceRegex.Replace(str2, " ").Trim();
			return str1 == str2;
		}
#endif
	}
}
