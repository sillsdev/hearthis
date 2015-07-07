using System.IO;
using HearThis.Properties;
using L10NSharp;
using SIL.Windows.Forms.DblBundle;

namespace HearThis.UI
{
	public class SelectBundleDlg : SelectProjectDlgBase
	{
		protected override string DefaultBundleDirectory
		{
			get { return Settings.Default.DefaultBundleDirectory; }
			set { Settings.Default.DefaultBundleDirectory = value; }
		}

		protected override string ProjectFileExtension
		{
			get { return ExistingProjectsList.kProjectFileExtension; }
		}

		protected override string Title
		{
			get { return LocalizationManager.GetString("DialogBoxes.SelectBundleDlg.Title", "Create New Project from Text Release Bundle"); }
		}

		protected override string ProductName
		{
			get { return Program.kProduct; }
		}
	}
}
