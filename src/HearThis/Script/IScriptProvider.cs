// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
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

		int GetScriptBlockCount(int bookNumber, int chapter1Based);
		int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based);
		int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based);
		int GetTranslatedVerseCount(int bookNumber0Based, int chapterNumber1Based);
		int GetScriptBlockCount(int bookNumber);
		void LoadBook(int bookNumber0Based);
		string EthnologueCode { get; }
		string ProjectFolderName { get; }
		IEnumerable<string> AllEncounteredParagraphStyleNames { get; }
		IBibleStats VersificationInfo { get; }
		/// <summary>
		/// Indicates whether in the course of parsing the text, there was *ever* any instance where a first-level quotation
		/// mark was encountered nested inside an existing quotation.
		/// </summary>
		bool NestedQuotesEncountered { get; }
	}
}