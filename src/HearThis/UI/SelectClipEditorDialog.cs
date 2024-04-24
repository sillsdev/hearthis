// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International. All Rights Reserved.
// <copyright from='2024' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using L10NSharp;
using SIL.Reporting;
using SIL.Windows.Forms.PortableSettingsProvider;
using static System.String;
using static HearThis.UI.AdministrativeSettings;

namespace HearThis.UI
{
	public partial class SelectClipEditorDialog : Form
	{
		private readonly ExternalClipEditorInfo _model;
		private readonly Func<UiElement, string> _getUIString;

		public SelectClipEditorDialog(ExternalClipEditorInfo model, Func<UiElement, string> getUIString)
		{
			_model = model;
			_getUIString = getUIString;
			InitializeComponent();
			if (Settings.Default.SelectClipEditorFormSettings == null)
				Settings.Default.SelectClipEditorFormSettings = FormSettings.Create(this);
			if (_model.IsSpecified)
			{
				_lblPathToWAVFileEditor.Text = _model.ApplicationPath;
				_txtEditingApplicationName.Text = _model.ApplicationName;
				_txtCommandLineArguments.Text = _model.CommandLineParameters;
				_rdoUseDefaultAssociatedApplication.Checked = _model.UseAssociatedDefaultApplication;
			}
			else
			{
				_lblPathToWAVFileEditor.Text = "";
			}

			_lblWAVEditorCommandLineExample.Text = Format(_lblWAVEditorCommandLineExample.Text,
				ExternalClipEditorInfo.kClipPathPlaceholder);
			HandleStringsLocalized();
		}

		public void HandleStringsLocalized()
		{
			const string kWav = "WAV";
			_lblInstructions.Text = Format(_lblInstructions.Text, kWav, Program.kProduct,
				"Adobe  Audacity", _getUIString(UiElement.CheckForProblemsView));
			_lblCommandLineArgumentsInstructions.Text = Format(
				_lblCommandLineArgumentsInstructions.Text, Program.kProduct, ExternalClipEditorInfo.kClipPathPlaceholder);
			_rdoUseSpecifiedEditor.Text = Format(_rdoUseSpecifiedEditor.Text, kWav);
		}

		protected override void OnLoad(EventArgs e)
		{
			Logger.WriteEvent("Showing Select Clip Editor dialog box.");
			Analytics.Track("Selecting Clip Editor");
			Settings.Default.SelectClipEditorFormSettings.InitializeForm(this);
			base.OnLoad(e);
		}

		private void _chkWAVEditorCommandLineArguments_CheckedChanged(object sender, EventArgs e)
		{
			UpdateDisplayOfCommandLineControls();
		}

		private void _btnOpenFileChooser_Click(object sender, EventArgs e)
		{
			const string kExe = "exe";
			using (var dlg = new OpenFileDialog())
			{
				dlg.Title = LocalizationManager.GetString("DialogBoxes.ExportDlg.SaveFileDialog.Title", "Choose File Location");
				if (_lblPathToWAVFileEditor.Text.Length > 0)
				{
					dlg.InitialDirectory = Path.GetDirectoryName(_lblPathToWAVFileEditor.Text);
					dlg.FileName = _lblPathToWAVFileEditor.Text;
				}
				else
				{
					dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				}

				dlg.Filter = Format("{0} ({1})|{1}|{2} ({3})|{3}|{4} ({5})|{5}",
					LocalizationManager.GetString("SelectClipEditorDialog.OpenFile.ApplicationFileTypeLabel", "Applications"), "*." + kExe,
					LocalizationManager.GetString("SelectClipEditorDialog.OpenFile.ExecutableFileTypeLabel", "All Executable files"), "*." + kExe + "; *.bat; *.cmd; *.com",
					LocalizationManager.GetString("SelectClipEditorDialog.OpenFile.AllFilesLabel", "All Files",
						"Label used in open file dialog for \"*.*\""), "*.*");
				dlg.DefaultExt = kExe;
				
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					_model.ApplicationPath = dlg.FileName;
					_lblPathToWAVFileEditor.Text = _model.IsSpecified ? dlg.FileName : "";
					if (_model.ApplicationName != null)
						_txtEditingApplicationName.Text = _model.ApplicationName;
					_rdoUseSpecifiedEditor.Checked = true;
					UpdateDisplayOfWAVEditorControls();
				}
			}
		}

		private void HandleWAVEditoOptionChanged(object sender, EventArgs e)
		{
			if (sender is RadioButton rdo && rdo.Checked)
			{
				if (rdo == _rdoUseSpecifiedEditor)
					_rdoUseDefaultAssociatedApplication.Checked = false;
				else
					_rdoUseSpecifiedEditor.Checked = false;

				UpdateDisplayOfWAVEditorControls();
			}
		}

		private void UpdateDisplayOfWAVEditorControls()
		{
			_model.UseAssociatedDefaultApplication = _rdoUseDefaultAssociatedApplication.Checked;
			_btnOk.Enabled = _model.IsSpecified;
			var editorSpecified = _rdoUseSpecifiedEditor.Checked && _lblPathToWAVFileEditor.Text.Length > 0;
			_lblPathToWAVFileEditor.Visible = _chkWAVEditorCommandLineArguments.Enabled = editorSpecified;
			if (_lblWAVFileEditingApplicationName.Visible)
			{
				// If the name-related controls have already been displayed, we don't want them flashing on and off,
				// so just disable them if the user changes back to using the default associated application
				_lblWAVFileEditingApplicationName.Enabled = _txtEditingApplicationName.Enabled = editorSpecified;
			}
			else
			{
				_lblWAVFileEditingApplicationName.Visible = _txtEditingApplicationName.Visible = editorSpecified;
			}
			UpdateDisplayOfCommandLineControls();
		}

		private void UpdateDisplayOfCommandLineControls()
		{
			if (!_lblCommandLineArgumentsInstructions.Visible)
			{
				_lblCommandLineArgumentsInstructions.Visible =
					_lblWAVEditorCommandLineExample.Visible =
					_txtCommandLineArguments.Visible =
					_chkWAVEditorCommandLineArguments.Checked;
			}
			else
			{
				_lblCommandLineArgumentsInstructions.Enabled =
					_lblWAVEditorCommandLineExample.Enabled =
						_txtCommandLineArguments.Enabled =
							_chkWAVEditorCommandLineArguments.Checked;
			}

			// Unfortunately, there does not seem to be a way to auto-size the dialog. It always makes itself
			// big enough to show everything, even if stuff is hidden.
			var lastVisibleInstrLabel = _lblWAVEditorCommandLineExample.Visible ? _lblWAVEditorCommandLineExample :
				_lblInstructions;
			if (lastVisibleInstrLabel.Bottom > _rdoUseSpecifiedEditor.Top)
			{
				Height += lastVisibleInstrLabel.Bottom - _rdoUseSpecifiedEditor.Top + _rdoUseSpecifiedEditor.Margin.Top +
					lastVisibleInstrLabel.Margin.Bottom;
				MinimumSize = Size;
			}
		}

		private void _btnOk_Click(object sender, EventArgs e)
		{
			_model.UpdateSettings();
		}
	}
}
