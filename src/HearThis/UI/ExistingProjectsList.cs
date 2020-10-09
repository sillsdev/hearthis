// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2015' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using HearThis.Script;
using L10NSharp;
using Paratext.Data;
using SIL.DblBundle;
using SIL.DblBundle.Text;
using SIL.Windows.Forms.DblBundle;

namespace HearThis.UI
{
	public partial class ExistingProjectsList : ProjectsListBase<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage>
	{
		private bool _listIncludesBundleProjects = false;

		public ExistingProjectsList()
		{
			InitializeComponent();
			Program.RegisterStringsLocalized(HandleStringsLocalized);
		}

		private void HandleStringsLocalized()
		{
			if (_listIncludesBundleProjects)
				OverrideColumnHeaderText(2, LocalizationManager.GetString("ChooseProject.ProjectNameOrIdColumn", "Short Name/Id",
					"Name of second column in Choose Project dialog box, used when the list contains some projects based on text release bundles."));
			else
				OverrideColumnHeaderText(2, LocalizationManager.GetString("ChooseProject.ProjectShortName", "Short Name",
					"Name of second column in Choose Project dialog box, used when the list contains no projects based on text release bundles."));
		}

		public Func<IEnumerable<ScrText>> GetParatextProjects { private get; set; }
		public IProjectInfo SampleProjectInfo { get; set; }

		protected override DataGridViewColumn FillColumn => colFullName;

		protected override IEnumerable<string> AllProjectFolders => Directory.GetDirectories(Program.ApplicationDataBaseFolder);

		public const string kProjectFileExtension = ".hearthis";

		protected override IEnumerable<object> GetAdditionalRowData(IProjectInfo projectInfo)
		{
			if (projectInfo is ParatextProjectProxy projectProxy)
			{
				yield return projectProxy.ScrText.Settings.FullName;
				yield return LocalizationManager.GetString("ChooseProject.Type.Paratext", "Paratext");
			}
			else if (projectInfo is SampleScriptProvider)
			{
				yield return SampleScriptProvider.kProjectUiName;
				yield return SampleScriptProvider.kProjectUiName;
			}
			else
			{
				yield return ((DblTextMetadata<DblMetadataLanguage>)projectInfo).Name;
				yield return LocalizationManager.GetString("ChooseProject.Type.TextReleaseBundle", "Text Release Bundle");
			}
		}

		protected override string ProjectFileExtension => kProjectFileExtension;

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			HandleStringsLocalized();
		}

		protected override IEnumerable<Tuple<string, IProjectInfo>> Projects
		{
			get
			{
				if (GetParatextProjects != null)
				{
					foreach (var scrText in GetParatextProjects())
						yield return new Tuple<string, IProjectInfo>(scrText.Name, new ParatextProjectProxy(scrText));
				}

				foreach (var project in base.Projects)
				{
					_listIncludesBundleProjects = true;
					yield return project;
				}

				yield return new Tuple<string, IProjectInfo>(SampleScriptProvider.kProjectUiName, SampleProjectInfo);
			}
		}

		protected override IProjectInfo GetProjectInfo(string path)
		{
			var bundle = new TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage>(path);
			return bundle.Metadata;
		}

		protected override string GetRecordingProjectName(Tuple<string, IProjectInfo> project)
		{
			if (project.Item2 is DblTextMetadata<DblMetadataLanguage>)
				return project.Item2.Id;
			return base.GetRecordingProjectName(project);
		}
	}
}
