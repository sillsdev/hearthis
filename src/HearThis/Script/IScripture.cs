// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2023-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using Paratext.Data;
using SIL.Scripture;

namespace HearThis.Script
{
	/// <summary>
	/// This exposes the things we care about out of ScrText, providing an
	/// anti-corruption layer between Paratext and HearThis and allowing us to test the code
	/// that calls Paratext.
	/// </summary>
	public interface IScripture : IScrProjectSettings
	{
		ScrVers Versification { get; }
		List<UsfmToken> GetUsfmTokens(VerseRef verseRef);
		IScrParserState CreateScrParserState(VerseRef verseRef);
		string DefaultFont { get; }
		bool RightToLeft { get; }
		string EthnologueCode { get; }
		string Name { get; }
		IStyleInfoProvider StyleInfo { get; }
		/// <summary>
		/// List of book numbers (1-based) with content
		/// </summary>
		IEnumerable<int> BooksPresent { get; }
	}

	/// <summary>
	/// This exposes the things we care about out of ScrText, providing an
	/// anti-corruption layer between Paratext and HearThis and allowing us to test the code
	/// that calls Paratext.
	/// </summary>
	public interface IScrProjectSettings
	{
		string FirstLevelStartQuotationMark { get; }
		string FirstLevelEndQuotationMark { get; }
		string SecondLevelStartQuotationMark { get; }
		string SecondLevelEndQuotationMark { get; }
		string ThirdLevelStartQuotationMark { get; }
		string ThirdLevelEndQuotationMark { get; }
		/// <summary>
		/// Gets whether first-level quotation marks are used unambiguously to indicate first-level quotations.
		/// If the same marks are used for 2nd or 3rd level quotations, then this should return false.
		/// </summary>
		bool FirstLevelQuotesAreUnique { get; }
	}
}
