using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HearThis
{
	public partial class ImageButton : UserControl
	{
		public Image EnabledImage;
		public Image DisabledImage;
		//public event EventHandler Click;

		public ImageButton()
		{
			InitializeComponent();
			EnabledImage = DisabledImage = button.Image;
		}

		private void ImageButton_EnabledChanged(object sender, EventArgs e)
		{
		   if(EnabledImage!=null)
			   button.Image = Enabled ? EnabledImage : DisabledImage;
			button.Enabled = Enabled;
		}

		private void ImageButton_Load(object sender, EventArgs e)
		{
			button.Image = Enabled ? EnabledImage : DisabledImage;
		}

		private void button_Click(object sender, EventArgs e)
		{
		   InvokeOnClick(this, e);
		}
	}
}
