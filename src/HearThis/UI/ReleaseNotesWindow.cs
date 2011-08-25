using System;
using System.Diagnostics;
using System.Windows.Forms;
using Palaso.IO;

namespace HearThis.UI
{
	public partial class ReleaseNotesWindow : Form
	{
		public ReleaseNotesWindow()
		{
			InitializeComponent();
		}

		private void InfoWindow_Load(object sender, EventArgs e)
		{
			Activate(); //bring to front
			var path = FileLocator.GetFileDistributedWithApplication( "releaseNotes.htm");
			_webBrowser.Navigate(path);
		}

		private void OnExitClick(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Abort;
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void _webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			if (_webBrowser.Document != null)
			{
				e.Cancel = true; //we don't want to navigate away from here, we'll launch a browser instead
				Process.Start(e.Url.AbsoluteUri);
			}

		}

		private void _webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{

		}
	}
}
