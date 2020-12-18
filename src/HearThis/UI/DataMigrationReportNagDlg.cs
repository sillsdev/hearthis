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

namespace HearThis.UI
{
	/// <summary>
	/// This simple dialog provides a brief explanation of what HearThisPacks are for
	/// and a home for the control that allows choosing to limit the pack to the current
	/// actor.
	/// </summary>
	public partial class DataMigrationReportNagDlg : Form
	{
		public DataMigrationReportNagDlg(string reportFilePath, string urlForHelp)
		{
			InitializeComponent();
			_linkReport.Links[0].LinkData = new Action(() => Process.Start(reportFilePath));
			_linkHelp.Links[0].LinkData = new Action(() => Process.Start(urlForHelp));

			Program.RegisterStringsLocalized(HandleStringsLocalized);
			HandleStringsLocalized();
		}

		private void HandleStringsLocalized()
		{
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
