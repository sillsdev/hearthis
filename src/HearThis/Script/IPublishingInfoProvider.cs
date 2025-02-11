// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2022-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using HearThis.Script;

namespace HearThis.Publishing
{
	public interface IPublishingInfoProvider
	{
		string Name { get; }
		string EthnologueCode { get; }
		string CurrentBookName { get; }
		bool IncludeBook(string bookName);
		ScriptLine GetUnfilteredBlock(string bookName, int chapterNumber, int lineNumber0Based);
		IBibleStats VersificationInfo { get; }
		int BookNameComparer(string x, string y);
		bool BreakQuotesIntoBlocks { get; }
		string BlockBreakCharacters { get; }
		bool HasProblemNeedingAttention(string bookName = null);
	}
}
