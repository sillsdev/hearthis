using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void _publishButton_Click(object sender, EventArgs e)
		{
			_model.Publish(_logBox);
		}
	}
}
