// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;

namespace HearThis.UI
{
	public partial class AdministrativeSettings : Form
	{
		private readonly Project _project;
#if MULTIPLEMODES
		private CheckBox _defaultMode;
		private readonly Image _defaultImage;
#endif
		private bool _userElectedToDeleteSkips;

		public AdministrativeSettings(Project project)
		{
			_project = project;
			InitializeComponent();


			// Original idea was to have a Modes tab that would allow the administrator to select which modes would be
			// available to the user. Since we didn't get around to creating all the desired modes and the only thing
			// that distinguished Admin mode for normal recording mode was the visibility of the Skip button, John
			// suggested that for now we go back to a single check box that determines whether that button would be
			// displayed. If MULITPLEMODES is defined, some changes will also be needed on the Skipping page in Designer
			// (and, of course, the other modes will need to be added on the Modes page).
			// Initialize Modes tab
#if MULTIPLEMODES
			Administrator.Checked = Settings.Default.AllowAdministrativeMode;
			NormalRecording.Checked = Settings.Default.AllowNormalRecordingMode;
			_defaultImage = Administrator.Image;
			_defaultMode = Administrator;
			LinkLabel defaultModeLink = lnkAdministrativeModeSetAsDefault;
			for (int i = 0; i < _tableLayoutModes.RowCount; i++)
			{
				var lnk = (LinkLabel)_tableLayoutModes.GetControlFromPosition(1, i);
				var modeBtn = (CheckBox)_tableLayoutModes.GetControlFromPosition(0, i);
				lnk.Tag = modeBtn;
				modeBtn.Tag = lnk;
				lnk.Click += HandleDefaultModeChange;
				modeBtn.CheckedChanged += ModeCheckedChanged;
				modeBtn.TextImageRelation = TextImageRelation.TextBeforeImage;
				if (Settings.Default.ActiveMode == modeBtn.Name)
					defaultModeLink = lnk;
			}

			if (defaultModeLink != lnkAdministrativeModeSetAsDefault)
				HandleDefaultModeChange(defaultModeLink, new EventArgs());
#else
			tabControl1.TabPages.Remove(tabPageModes);

			// Initialize Skipping tab
			_chkShowSkipButton.Checked = (Settings.Default.ActiveMode == Administrator.Name);
#endif

			// Initialize Skipping tab
			_lblSkippingInstructions.Text = String.Format(_lblSkippingInstructions.Text, _project.Name);
			foreach (var styleName in _project.AllEncounteredParagraphStyleNames)
			{
				_lbSkippedStyles.Items.Add(styleName, _project.IsSkippedStyle(styleName));
			}

			// Initialize Punctuation tab
			_chkBreakAtQuotes.Checked = Settings.Default.BreakQuotesIntoBlocks;
			_txtClauseSeparatorCharacters.Text = Settings.Default.ClauseBreakCharacters;
			_lblWarningExistingRecordings.Visible = ClipRepository.GetDoAnyClipsExistForProject(project.Name);
			_lblWarningExistingRecordings.ForeColor = _chkBreakAtQuotes.ForeColor;
		}

		private void HandleOkButtonClick(object sender, EventArgs e)
		{
#if MULTIPLEMODES
			// Save settings on Modes tab
			Settings.Default.AllowAdministrativeMode = Administrator.Checked;
			Settings.Default.AllowNormalRecordingMode = NormalRecording.Checked;
			Settings.Default.ActiveMode = _defaultMode.Name;
#else
			Settings.Default.ActiveMode = _chkShowSkipButton.Checked ? Administrator.Name : NormalRecording.Name;
#endif

			// Save settings on Skipping tab
			if (_userElectedToDeleteSkips)
				_project.ClearAllSkippedBlocks();

			for (int i = 0; i < _lbSkippedStyles.Items.Count; i++)
				_project.SetSkippedStyle((string)_lbSkippedStyles.Items[i], _lbSkippedStyles.GetItemCheckState(i) == CheckState.Checked);

			// Save settings on Punctuation tab
			Settings.Default.BreakQuotesIntoBlocks = _chkBreakAtQuotes.Checked;
			Settings.Default.ClauseBreakCharacters = _txtClauseSeparatorCharacters.Text.Replace("  ", " ").Trim();
		}

#if MULTIPLEMODES
		private void HandleDefaultModeChange(object sender, EventArgs e)
		{
			var btn = (LinkLabel) sender;

			var newDefaultMode = ((CheckBox) btn.Tag);
			var oldDefaultMode = _defaultMode;
			ChangeDefault(newDefaultMode, oldDefaultMode);
			newDefaultMode.Checked = true;
		}

		private void ChangeDefault(CheckBox newDefaultMode, CheckBox oldDefaultMode)
		{
			SetDefault(newDefaultMode);
			oldDefaultMode.Image = null;
			((LinkLabel) oldDefaultMode.Tag).Enabled = true;
		}

		private void SetDefault(CheckBox newDefaultMode)
		{
			_defaultMode = newDefaultMode;
			newDefaultMode.Image = _defaultImage;
			((LinkLabel)newDefaultMode.Tag).Enabled = false;
		}

		private void ModeCheckedChanged(object sender, EventArgs e)
		{
			var btn = (CheckBox)sender;

			if (btn.Checked)
			{
				if (_defaultMode == null)
				{
					SetDefault(btn);
					_btnOk.Enabled = true;
				}
				return;
			}

			for (int i = 0; i < _tableLayoutModes.RowCount; i++)
			{
				var modeBtn = (CheckBox)_tableLayoutModes.GetControlFromPosition(0, i);
				if (modeBtn.Checked)
				{
					ChangeDefault(modeBtn, btn);
					return;
				}
			}

			btn.Image = null;
			((LinkLabel)btn.Tag).Enabled = true;
			_defaultMode = null;
			_btnOk.Enabled = false;
		}
#endif

		/// <summary>
		/// John thought this button added unnecessary complexity and wasn't worth it so I made it
		/// invisible, but I'm leaving the code here in case we decide it's needed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleClearAllSkipInfo_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show(this,
				LocalizationManager.GetString("AdministrativeSettings.ClearAllSkipsConfirmationMsg",
					"This will permanently delete information about any blocks you have skipped for this project and also clear all skipped styles. Are you sure you want to do this?"),
				ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			if (result == DialogResult.No)
				return;
			_userElectedToDeleteSkips = true;
			for (int i = 0; i < _lbSkippedStyles.Items.Count; i++)
				_lbSkippedStyles.SetItemCheckState(i, CheckState.Unchecked);
		}

		private void _chkBreakAtQuotes_CheckedChanged(object sender, EventArgs e)
		{
			_lblWarningExistingRecordings.ForeColor = ( _chkBreakAtQuotes.Checked == Settings.Default.BreakQuotesIntoBlocks) ?
				_chkBreakAtQuotes.ForeColor : AppPallette.Red;
		}
	}
}
