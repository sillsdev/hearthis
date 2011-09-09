using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HearThis.UI
{
	public partial class UserControl1 : TrackBar
	{
		public UserControl1()
		{
			InitializeComponent();
			Maximum = 20;
			SetStyle(ControlStyles.UserPaint,true);
		}
		protected override void OnValueChanged(EventArgs e)
		{
			base.OnValueChanged(e);
		}
	}
}
