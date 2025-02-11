// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022-2025, SIL Global.
// <copyright from='2020' to='2025' company='SIL Global'>
//		Copyright (c) 2022-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Windows.Forms;
using L10NSharp;
using static System.String;

namespace HearThis.UI
{
	/// <summary>
	/// This simple dialog alerts the user to a data migration step that was unable to completely
	/// migrate everything with absolute certainty so that manual cleanup can be done if needed.
	/// </summary>
	public partial class DataMigrationReportNagDlg : Form, ILocalizable
	{
		private readonly string _incompleteMigrationVersion;

		public DataMigrationReportNagDlg(string incompleteMigrationVersion,
			string dataMigrationReportFile, string urlForHelp)
		{
			_incompleteMigrationVersion = incompleteMigrationVersion;
			InitializeComponent();
			_linkReport.Links[0].LinkData = new Action(() => Process.Start(dataMigrationReportFile));
			_linkHelp.Links[0].LinkData = new Action(() => Process.Start(urlForHelp));

			Program.RegisterLocalizable(this);
			HandleStringsLocalized();
		}

		public void HandleStringsLocalized()
		{
			_txtSummary.Text = Format(_txtSummary.Text, _incompleteMigrationVersion);
			_linkHelp.SetLinkRegions();
		}

		public bool StopNagging => _chkDoNotNagAnymore.Checked;
		public bool DeleteReportFile => _chkDelete.Checked;

		private void _chkDoNotNagAnymore_CheckedChanged(object sender, EventArgs e)
		{
			_chkDelete.Enabled = _chkDoNotNagAnymore.Checked;
			if (!_chkDelete.Enabled)
				_chkDelete.Checked = false;
		}

		private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			((Action)e.Link.LinkData).Invoke();
		}
	}
}
