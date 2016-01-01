// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International. All Rights Reserved.
// <copyright from='2011' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.XPath;
using Paratext;
using SIL.DblBundle.Text;

namespace HearThis.Script
{
	/// <summary>
	/// This is the real implementation of IScripture which implements the interface by
	/// wrapping a DblTextBundle.
	/// </summary>
	internal class TextBundleScripture : IScripture
	{
		private TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage> _bundle;
		private ScrVers _versification;
		private readonly ScrStylesheet _stylesheet;
		private readonly Dictionary<string, List<UsfmToken>> _bookTokens = new Dictionary<string, List<UsfmToken>>();

		public TextBundleScripture(TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage> bundle)
		{
			_bundle = bundle;

			var stylesFile = Path.ChangeExtension(Path.GetTempFileName(), "sty");
			File.WriteAllBytes(stylesFile, Properties.Resources.usfm);
			_stylesheet = new ScrStylesheet(stylesFile);
			File.Delete(stylesFile);

			// TODO: Update stylesheet from the info in the bundle.
			//foreach (var s in _bundle.Stylesheet.Styles)
			//{
			//	var style = _stylesheet. GetTag(s.Id);
			//}

			var stopExpression = XPathExpression.Compile("*[false()]");
			Parallel.ForEach(_bundle.UsxBooksToInclude, book =>
			{
				string usfm;
				UsxFragmenter.FindFragments(book.XmlDocument.CreateNavigator(), stopExpression, out usfm);
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
					var vrsFile = Path.ChangeExtension(Path.GetTempFileName(), "vrs");
					_bundle.CopyVersificationFile(vrsFile);
					_versification = Paratext.Versification.Table.Load(vrsFile, "custom");
					File.Delete(vrsFile);
				}
				return _versification;
			}
		}

		public List<UsfmToken> GetUsfmTokens(VerseRef verseRef)
		{
			List<UsfmToken> tokens;
			if (!_bookTokens.TryGetValue(verseRef.Book, out tokens))
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

		public string DefaultFont
		{
			get { return _bundle.Stylesheet.FontFamily; }
		}

		public string EthnologueCode
		{
			get { return _bundle.LanguageIso; }
		}

		public string Name
		{
			get { return _bundle.Name; }
		}

		public string FirstLevelStartQuotationMark
		{
			get { return null; }
		}

		public string FirstLevelEndQuotationMark
		{
			get { return null; }
		}

		public string SecondLevelStartQuotationMark
		{
			get { return null; }
		}

		public string SecondLevelEndQuotationMark
		{
			get { return null; }
		}

		public string ThirdLevelStartQuotationMark
		{
			get { return null; }
		}

		public string ThirdLevelEndQuotationMark
		{
			get { return null; }
		}

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
