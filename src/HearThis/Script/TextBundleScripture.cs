// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.XPath;
using Paratext.Data;
using SIL.DblBundle.Text;
using SIL.IO;
using SIL.Scripture;

namespace HearThis.Script
{
	/// <summary>
	/// This is the real implementation of IScripture which implements the interface by
	/// wrapping a DblTextBundle.
	/// </summary>
	internal class TextBundleScripture : IScripture
	{
		private readonly TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage> _bundle;
		private ScrVers _versification;
		private readonly ScrStylesheet _stylesheet;
		private StyleLookup _stylesheetWrapper;
		private readonly Dictionary<string, List<UsfmToken>> _bookTokens = new Dictionary<string, List<UsfmToken>>();

		public TextBundleScripture(TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage> bundle)
		{
			_bundle = bundle;

			var stylesFile = Path.ChangeExtension(Path.GetTempFileName(), "sty");
			RobustFile.WriteAllBytes(stylesFile, Properties.Resources.usfm);
			_stylesheet = new ScrStylesheet(stylesFile);
			RobustFile.Delete(stylesFile);

			//foreach (var s in _bundle.Stylesheet.Styles)
			//{
			//	var style = _stylesheet. GetTag(s.Id);
			// TODO: Update stylesheet from the info in the bundle.
			//}

			var stopExpression = XPathExpression.Compile("*[false()]");
			Parallel.ForEach(_bundle.UsxBooksToInclude, book =>
			{
				UsxFragmenter.FindFragments(_stylesheet, book.XmlDocument.CreateNavigator(), stopExpression, out var usfm);
				_bookTokens[book.BookId] = UsfmToken.Tokenize(_stylesheet, usfm, false);
			});
		}

		#region IScripture Members
		public ScrVers Versification
		{
			get
			{
				if (_versification == null)
				{
					using (var reader = _bundle.GetVersification())
						_versification = SIL.Scripture.Versification.Table.Implementation.Load(reader, "custom");
				}
				return _versification;
			}
		}

		public List<UsfmToken> GetUsfmTokens(VerseRef verseRef)
		{
			if (!_bookTokens.TryGetValue(verseRef.Book, out var tokens))
				return new List<UsfmToken>();
			UsfmParser parser = new UsfmParser(_stylesheet, tokens, verseRef, null);
			var list = new List<UsfmToken>();
			while (parser.ProcessToken())
				list.Add(parser.Token);
			return list;
		}

		public IScrParserState CreateScrParserState(VerseRef verseRef)
		{
			return new ParserState(new ScrParserState(_stylesheet, verseRef));
		}

		public string DefaultFont => _bundle.Stylesheet.FontFamily;

		public bool RightToLeft => false;

		public string EthnologueCode => _bundle.LanguageIso;

		public string Name => _bundle.Name;

		public IStyleInfoProvider StyleInfo =>
			_stylesheetWrapper ?? (_stylesheetWrapper = new StyleLookup(_stylesheet));

		public string FirstLevelStartQuotationMark => null;

		public string FirstLevelEndQuotationMark => null;

		public string SecondLevelStartQuotationMark => null;

		public string SecondLevelEndQuotationMark => null;

		public string ThirdLevelStartQuotationMark => null;

		public string ThirdLevelEndQuotationMark => null;

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
