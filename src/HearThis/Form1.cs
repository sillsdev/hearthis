using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HearThis
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.MinimumSize = recordControl1.MinimumSize;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Project project = new Project();
			recordControl1.SetProject(project);
		}
	}
}
