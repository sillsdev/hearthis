// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;

namespace HearThis.Script
{
	public interface IScriptProvider
	{
		/// <summary>
		/// The "block" is a bit of script that can be recorded as a single clip; text is generally broken into blocks based on
		/// paragraph breaks and sentence-final punctuation, not verse breaks.
		/// </summary>
		ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based);
		ScriptLine GetUnfilteredBlock(int bookNumber, int chapterNumber, int lineNumber0Based);

		int GetScriptBlockCount(int bookNumber, int chapter1Based);
		int GetUnfilteredScriptBlockCount(int bookNumber, int chapter1Based);
		int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based);
		int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based); // also unfiltered
		/// <summary>
		/// Despite its name, if called for chapter 0 (Book title/into), it will return a positive
		/// number if there is a title or intro material.
		/// </summary>
		int GetTranslatedVerseCount(int bookNumber0Based, int chapterNumber1Based);
		int GetUnfilteredTranslatedVerseCount(int bookNumber0Based, int chapterNumber1Based);
		int GetScriptBlockCount(int bookNumber);
		void LoadBook(int bookNumber0Based);
		string EthnologueCode { get; }
		bool RightToLeft { get; }
		string FontName { get; }
		string ProjectFolderName { get; }
		IEnumerable<string> AllEncounteredParagraphStyleNames { get; }
		IEnumerable<char> AllEncounteredSentenceEndingCharacters { get; }
		IBibleStats VersificationInfo { get; }
		/// <summary>
		/// Indicates whether in the course of parsing the text, there was *ever* any instance where a first-level quotation
		/// mark was encountered nested inside an existing quotation.
		/// </summary>
		bool NestedQuotesEncountered { get; }

		void UpdateSkipInfo();
	}
}
