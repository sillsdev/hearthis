// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2020' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Linq;
using Paratext.Data;

namespace HearThis.Script
{
	public class StyleLookup : IStyleInfoProvider
	{
		private readonly ILookup<string, ScrTag> _lookup;

		public StyleLookup(ScrStylesheet stylesheet)
		{
			_lookup = stylesheet.Tags.ToLookup(t => t.Marker, t => t);
		}

		private ScrTag GetExistingTag(string marker) => _lookup[marker].SingleOrDefault();

		public bool IsParagraph(string marker) =>
			GetExistingTag(marker)?.StyleType == ScrStyleType.scParagraphStyle;

		public bool IsPublishableVernacular(string marker)
		{
			var style = GetExistingTag(marker);
			return style != null &&
				style.HasTextProperty(TextProperties.scPublishable) &&
				style.HasTextProperty(TextProperties.scVernacular);
		}

		public string GetStyleName(string marker) => GetExistingTag(marker).Name;
	}
}
