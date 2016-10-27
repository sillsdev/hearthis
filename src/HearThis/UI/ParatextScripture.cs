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
using Paratext;
using Paratext.Checking;
using Paratext.ProjectSettingsAccess;

namespace HearThis.Script
{
	/// <summary>
	/// This is the real implementation of IScripture which implements the interface by
	/// wrapping a real ScrText.
	/// </summary>
	internal class ParatextScripture : IScripture
	{
		private ScrText _scrText;

		public ParatextScripture(ScrText text)
		{
			_scrText = text;
		}

		#region IScripture Members

		public ScrVers Versification
		{
			get { return _scrText.Settings.Versification; }
		}

		public List<UsfmToken> GetUsfmTokens(VerseRef verseRef)
		{
			var parser = _scrText.Parser;
			return parser.GetUsfmTokens(verseRef, false /* single chapter */, true);
		}

		public IScrParserState CreateScrParserState(VerseRef verseRef)
		{
			return new ParserState(new ScrParserState(_scrText, verseRef));
		}

		public string DefaultFont
		{
			get { return _scrText.DefaultFont; }
		}

		public bool RightToLeft
		{
			get { return _scrText.RightToLeft; }
		}

		public string EthnologueCode
		{
			get
			{
				var result = _scrText.Settings.LanguageID.Id;
				if (result != null)
					return result;
				// Seems like the above SHOULD return Paratext's idea of the language identifier.
				// In some cases it does. But in others it does not.
				// In at least some cases where it does not, it really does know the identifier,
				// and seems to fairly consistently put it in the FullName,
				// surrounded by square brackets.
				var name = _scrText.Settings.FullName;
				int openBracket = name.IndexOf('[');
				int closeBracket = name.IndexOf(']');
				if (closeBracket > openBracket && openBracket > 0)
					return name.Substring(openBracket + 1, closeBracket - openBracket - 1);
				return result;
			}
		}

		public string Name
		{
			get { return _scrText.Name; }
		}

		public string FirstLevelStartQuotationMark => _scrText.Settings.Quotes.Begin;

		public string FirstLevelEndQuotationMark => _scrText.Settings.Quotes.End;

		public string SecondLevelStartQuotationMark => _scrText.Settings.InnerQuotes.Begin;

		public string SecondLevelEndQuotationMark => _scrText.Settings.InnerQuotes.End;

		public string ThirdLevelStartQuotationMark => _scrText.Settings.InnerInnerQuotes.Begin;

		public string ThirdLevelEndQuotationMark => _scrText.Settings.InnerInnerQuotes.End;

		/// <summary>
		/// Gets whether first-level quotation marks are used unambiguously to indicate first-level quotations.
		/// If the same marks are used for 2nd or 3rd level quotations, then this should return false.
		/// </summary>
		public bool FirstLevelQuotesAreUnique
		{
			get
			{
				return FirstLevelStartQuotationMark != SecondLevelStartQuotationMark &&
					FirstLevelStartQuotationMark != ThirdLevelStartQuotationMark &&
					FirstLevelStartQuotationMark != SecondLevelEndQuotationMark &&
					FirstLevelStartQuotationMark != ThirdLevelEndQuotationMark &&
					FirstLevelEndQuotationMark != SecondLevelStartQuotationMark &&
					FirstLevelEndQuotationMark != ThirdLevelStartQuotationMark &&
					FirstLevelEndQuotationMark != SecondLevelEndQuotationMark &&
					FirstLevelEndQuotationMark != ThirdLevelEndQuotationMark;
			}
		}
		#endregion
	}
}
