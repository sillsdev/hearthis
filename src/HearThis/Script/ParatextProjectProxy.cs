// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2015-2025, SIL Global.
// <copyright from='2015' to='2025' company='SIL Global'>
//		Copyright (c) 2015-2025, SIL Global.
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

		/// <summary>
		/// Gets whether Paratext can find this project using Find (vs. FindById). This
		/// is almost always true, but if two local projects have the same (short) Name,
		/// then this will be false for one of them.
		/// </summary>
		/// <remarks>ScrTextCollection.Find returns null if there is more than one project with the
		/// given name. (Of course, null can also be returned if the project can't be found at all,
		/// but since we have a ScrText object, that's presumably impossible.)</remarks>
		private bool CanBeFoundUsingShortName => ScrTextCollection.Find(ScrText.Name) != null;

		public string Name => CanBeFoundUsingShortName ? ScrText.Name : $"{ScrText.Name} ({ScrText.FullName})";

		public string Id => ScrText.Guid.ToString();

		public DblMetadataLanguage Language { get; }
	}
}
