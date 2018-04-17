using System;
using System.Windows.Forms;

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

		private void btnCheckForUpdates_Click(object sender, EventArgs e)
		{
			CheckForUpdatesClicked?.Invoke(this, e);
		}
	}
}
