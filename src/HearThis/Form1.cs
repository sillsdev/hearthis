using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using HearThis.Properties;
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

			SetWindowText("");
		}

		void OnSelectProjectClick(object sender, EventArgs e)
		{
			var paratextProject = ((ScrText)((MenuItem)sender).Tag);
			LoadProject(paratextProject.Name);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(Settings.Default.Project))
			{
				LoadProject(Settings.Default.Project);
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.Default.Save();
		}

		private void LoadProject(string name)
		{
			try
			{
				var project = new Project(Paratext.ScrTextCollection.Get(name));
				_recordingToolControl1.SetProject(project);
				SetWindowText(name);
				Settings.Default.Project = name;
				Settings.Default.Save();
			}
			catch (Exception e)
			{
				Palaso.Reporting.ErrorReport.NotifyUserOfProblem(e, "Could not open " + Settings.Default.Project);
			}
		}

		private void SetWindowText(string projectName)
		{
			var ver = Assembly.GetExecutingAssembly().GetName().Version;
			Text = string.Format("{3} -- HearThis {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build, projectName);
		}

	}
}
