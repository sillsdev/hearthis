// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2015-2025, SIL Global.
// <copyright from='2015' to='2025' company='SIL Global'>
//		Copyright (c) 2015-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using HearThis.Properties;
using L10NSharp;
using SIL.Windows.Forms.DblBundle;
using static HearThis.SafeSettings;
using static HearThis.UI.ExistingProjectsList;

namespace HearThis.UI
{
	public class SelectBundleDlg : SelectProjectDlgBase
	{
		protected override string DefaultBundleDirectory
		{
			get => Get(() => Settings.Default.DefaultBundleDirectory);
			set => Set(() => Settings.Default.DefaultBundleDirectory = value);
		}

		protected override string ProjectFileExtension => kProjectFileExtension;

		protected override string Title => LocalizationManager.GetString(
			"DialogBoxes.SelectBundleDlg.Title", "Create New Project from Text Release Bundle");

		protected override string ProductName => Program.kProduct;
	}
}
