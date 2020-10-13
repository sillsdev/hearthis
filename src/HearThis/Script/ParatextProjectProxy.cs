// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2015' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
//
// This class is nearly identical to the one in Glyssen, though with a different namespace.
// If improvements are made here, they should also be made there if applicable.
using Paratext.Data;
using SIL.DblBundle;
using SIL.DblBundle.Text;

namespace HearThis.Script
{
	public class ParatextProjectProxy : IProjectInfo
	{
		public ParatextProjectProxy(ScrText scrText)
		{
			ScrText = scrText;
			Language = new DblMetadataLanguage();
			Language.Iso = scrText.Settings.LanguageID.Id;
			Language.Name = scrText.DisplayLanguageName;
			Language.Script = scrText.Language.FontName;
			Language.ScriptDirection = scrText.RightToLeft ? "RTL" : "LTR";
		}

		public ScrText ScrText { get; }

		public string Name => ScrText.Name;

		public string Id => ScrText.Settings.DBLId ?? ScrText.Name;

		public DblMetadataLanguage Language { get; }
	}
}
