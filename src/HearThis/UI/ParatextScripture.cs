// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2011' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using Paratext.Data;
using SIL.Scripture;

namespace HearThis.Script
{
	/// <summary>
	/// This is the real implementation of IScripture which implements the interface by
	/// wrapping a real ScrText.
	/// </summary>
	internal class ParatextScripture : IScripture
	{
		private readonly ScrText _scrText;
		private StyleLookup _stylesheet;

		public ParatextScripture(ScrText text)
		{
			_scrText = text;
		}

		public IStyleInfoProvider StyleInfo =>
			_stylesheet ?? (_stylesheet = new StyleLookup(_scrText.DefaultStylesheet));

		#region IScripture Members

		public ScrVers Versification => _scrText.Settings.Versification;

		// HT-415: This is a hack to allow HearThis to load a Paratext book that
		// is missing paragraph markers after the \c. We can't really know
		// which paragraph style to use, but "\p" is the most common and since it
		// typically doesn't matter (unless the text was supposed to be a heading).
		public ScrTag DefaultScriptureParaTag => _scrText.DefaultStylesheet.GetTag("p");

		public List<UsfmToken> GetUsfmTokens(VerseRef verseRef)
		{
			var parser = _scrText.Parser;
			return parser.GetUsfmTokens(verseRef, false /* single chapter */, true);
		}

		public IScrParserState CreateScrParserState(VerseRef verseRef)
		{
			return new ParserState(new ScrParserState(_scrText, verseRef));
		}

		public string DefaultFont => _scrText.Language.FontName;

		public bool RightToLeft => _scrText.RightToLeft;

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

		/// <summary>
		/// while this will almost always be the "short" name of the underlying Paratext project,
		/// in cases where the local machine has more than one Paratext project with the same name,
		/// one or more of them will use name.Guid. So this property is suitable for
		/// getting a guaranteed unique folder name but is not ideal for displaying in the UI.
		/// </summary>
		public string Name => Path.GetFileName(_scrText.Directory);

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
