using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SIL.Windows.Forms.Progress;

namespace HearThis.UI
{
	/// <summary>
	/// This dialog is used during HearThisPack merging to report progress.
	/// Todo: make localizable.
	/// </summary>
	public partial class MergeProgressDialog : Form
	{
		public MergeProgressDialog()
		{
			InitializeComponent();

			// Would be neater to do this in designer, but it's more trouble than it's worth to figure out how
			// to make designer know about SIL.Windows.Forms.Progress.LogBox.
			LogBox = new LogBox();
			LogBox.Location = new Point(10, 10 + label1.Bottom);
			LogBox.Size = new Size(this.ClientSize.Width - 20, _okButton.Top - label1.Bottom - 20);
			Controls.Add(LogBox);
		}

		/// <summary>
		/// Where clients write progress messages.
		/// </summary>
		public LogBox LogBox { get; }

		/// <summary>
		/// Client should call this when progress is complete.
		/// </summary>
		public void SetDone()
		{
			// Once the process we're reporting on is done, the user can close the dialog.
			// We don't do it for them because they might want to see what's in the log.
			_okButton.Enabled = true;
		}

		/// <summary>
		/// Clients should call this to fill in the message saying where we're copying from.
		/// </summary>
		/// <param name="source"></param>
		public void SetSource(string source)
		{
			label1.Text = string.Format(label1.Text, source);
		}

		/// <summary>
		/// Not sure why this is necessary in addition to setting OK to be the cancel button and the button's
		/// DialogResult to cancel. Possibly because it's not running Modal?
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _okButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
