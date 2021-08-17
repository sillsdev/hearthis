using System;
using System.Windows.Forms;
using SIL.Reporting;

namespace HearThis.UI
{
	public partial class UpgradeNeededDialog : Form
	{
		public event EventHandler CheckForUpdatesClicked;

		public string Description { set => _description.Text = value; }

		public UpgradeNeededDialog()
		{
			InitializeComponent();
		}

		protected override void OnShown(EventArgs e)
		{
			Logger.WriteEvent("Showing Upgrade Needed dialog box.");
			base.OnShown(e);
		}

		private void btnCheckForUpdates_Click(object sender, EventArgs e)
		{
			CheckForUpdatesClicked?.Invoke(this, e);
		}
	}
}
