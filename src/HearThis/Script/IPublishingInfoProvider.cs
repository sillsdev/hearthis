// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
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
		string AdditionalBlockBreakCharacters { get; }
	}
}
