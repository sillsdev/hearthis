using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HearThis.Publishing;

namespace HearThis
{
	public partial class PublishDialog : Form
	{
		private readonly PublishingModel _model;

		public PublishDialog(PublishingModel model)
		{
			_model = model;
			InitializeComponent();
			_destinationLabel.Text = _model.PublishPath;
			_logBox.ShowDetailsMenuItem = true;
			_logBox.ShowCopyToClipboardMenuItem = true;
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void _publishButton_Click(object sender, EventArgs e)
		{
			if(!_cancelButton.Visible)//kind of a hack at the moment, the publish button does double-duty
			{
				Close();
				return;
			}

			_model.Publish(_logBox);
			_cancelButton.Visible = false;
			_publishButton.Text = "&Close";
			_openFolderLink.Text = _model.PublishPath;
			_openFolderLink.Visible = true;
		}

		private void _openFolderLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(_model.PublishPath);
		}
	}
}
