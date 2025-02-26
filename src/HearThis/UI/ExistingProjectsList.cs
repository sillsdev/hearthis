// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global. All Rights Reserved.
// <copyright from='2015' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public partial class ExistingProjectsList : ProjectsListBase<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage>, ILocalizable
	{
		private bool _listIncludesBundleProjects = false;
		private readonly Dictionary<string, string> m_paratextProjectIds = new Dictionary<string, string>();

		public ExistingProjectsList()
		{
			InitializeComponent();
			Program.RegisterLocalizable(this); // HandleStringsLocalized gets called in OnLoad
		}

		public void HandleStringsLocalized()
		{
			if (_listIncludesBundleProjects)
				OverrideColumnHeaderText(2, LocalizationManager.GetString("ChooseProject.ProjectNameOrIdColumn", "Short Name/Id",
					"Name of second column in Choose Project dialog box, used when the list contains some projects based on text release bundles."));
			else
				OverrideColumnHeaderText(2, LocalizationManager.GetString("ChooseProject.ProjectShortName", "Short Name",
					"Name of second column in Choose Project dialog box, used when the list contains no projects based on text release bundles."));
		}

		public Func<IEnumerable<ScrText>> GetParatextProjects { private get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IProjectInfo SampleProjectInfo { get; set; }

		public string IdentifierForParatextProject => SelectedProject == null ? null :
			m_paratextProjectIds.TryGetValue(SelectedProject, out var id) ? id : null;

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
				yield return LocalizationManager.GetDynamicString(Program.kProduct,
					"ChooseProject.Type.Sample", SampleScriptProvider.kProjectUiName);
			}
			else
			{
				yield return TextBundleScripture.GetBestName((DblTextMetadata<DblMetadataLanguage>)projectInfo);
				yield return LocalizationManager.GetString("ChooseProject.Type.TextReleaseBundle",
					"Text Release Bundle");
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
					{
						var proxy = new ParatextProjectProxy(scrText);
						m_paratextProjectIds[proxy.Name] = scrText.Guid.ToString();
						yield return new Tuple<string, IProjectInfo>(proxy.Name, proxy);
					}
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
