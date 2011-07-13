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
	public partial class RecordControl : UserControl
	{
		private Project _project;

		public RecordControl()
		{
			InitializeComponent();
		}

		private void scriptureMapControl1_Load(object sender, EventArgs e)
		{

		}

		private void scriptureMapControl1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			PictureBox b = (PictureBox)sender;
			b.Tag = b.Location;
			b.Location= new Point(b.Location.X+1,b.Location.Y+1);
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			PictureBox b = (PictureBox)sender;
			b.Location = (Point) b.Tag;
		}

		public void SetProject(Project project)
		{
			_project = project;
			_scriptureMapControl.SetProject(project);
		}

	}
}
