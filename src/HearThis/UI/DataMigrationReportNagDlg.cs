// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2020' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Windows.Forms;
using static System.String;

namespace HearThis.UI
{
	/// <summary>
	/// This simple dialog alerts the user to a data migration step that was unable to completely
	/// migrate everything with absolute certainty so that manual cleanup can be done if needed.
	/// </summary>
	public partial class DataMigrationReportNagDlg : Form
	{
		private readonly string _incompleteMigrationVersion;

		public DataMigrationReportNagDlg(string incompleteMigrationVersion,
			string dataMigrationReportFile, string urlForHelp)
		{
			_incompleteMigrationVersion = incompleteMigrationVersion;
			InitializeComponent();
			_linkReport.Links[0].LinkData = new Action(() => Process.Start(dataMigrationReportFile));
			_linkHelp.Links[0].LinkData = new Action(() => Process.Start(urlForHelp));

			Program.RegisterStringsLocalized(HandleStringsLocalized);
			HandleStringsLocalized();
		}

		private void HandleStringsLocalized()
		{
			_txtSummary.Text = Format(_txtSummary.Text, _incompleteMigrationVersion);
			// TODO: Set link label area to words in braces and remove the braces.
			//_linkHelp
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
