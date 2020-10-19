// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
namespace HearThis.Script
{
	public interface IBibleStats
	{
		int BookCount { get; }
		/// <summary>Gets the 0-based book index, given the English name</summary>
		int GetBookNumber(string bookName);
		string GetBookCode(int bookNumber0Based);
		/// <summary>Gets the English name of the book</summary>
		string GetBookName(int bookNumber0Based);
		int GetChaptersInBook(int bookNumber0Based);
	}
}
