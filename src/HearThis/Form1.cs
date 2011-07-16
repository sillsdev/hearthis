using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Paratext;

namespace HearThis
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			_recordingToolControl1.ContextMenu = new ContextMenu();

			ScrTextCollection.Initialize();
			foreach (var text in Paratext.ScrTextCollection.ScrTexts)
			{
				if (!text.IsResourceText)
				{
					MenuItem x = new MenuItem(text.Name);
					x.Tag = text;
					x.Click += new EventHandler(OnSelectProjectClick);
					_recordingToolControl1.ContextMenu.MenuItems.Add(x);
				}
			}
		}

		void OnSelectProjectClick(object sender, EventArgs e)
		{
			var paratextProject = ((ScrText)((MenuItem)sender).Tag);
			var project = new Project(paratextProject);
			_recordingToolControl1.SetProject(project);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Project project = new Project();
			_recordingToolControl1.SetProject(project);
		}
	}
}
