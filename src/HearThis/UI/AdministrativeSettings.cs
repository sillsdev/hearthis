// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International. All Rights Reserved.
// <copyright from='2011' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using static System.String;

namespace HearThis.UI
{
	public partial class AdministrativeSettings : Form, ILocalizable
	{
		public enum UiElement
		{
			ShiftClipsMenu,
			CheckForProblemsView,
		}
		private readonly Project _project;
		private readonly Func<UiElement, string> _getUiString;
#if MULTIPLEMODES
		private CheckBox _defaultMode;
		private readonly Image _defaultImage;
#endif
		private bool _userElectedToDeleteSkips;

		public AdministrativeSettings(Project project, Func<UiElement, string> getUiString)
		{
			_project = project;
			_getUiString = getUiString;
			InitializeComponent();

			_txtAdditionalBlockSeparators.Font = _txtClauseSeparatorCharacters.Font =
				new Font(project.FontName, 12 * Settings.Default.ZoomFactor, FontStyle.Regular);

			// Original idea was to have a Modes tab that would allow the administrator to select which modes would be
			// available to the user. Since we didn't get around to creating all the desired modes and the only thing
			// that distinguished Admin mode from normal recording mode was the visibility of the Skip button, John
			// suggested that for now we go back to a single check box that determines whether that button would be
			// displayed. If MULITPLEMODES is defined, some changes will also be needed on the Skipping page in Designer
			// (and, of course, the other modes will need to be added on the Modes page).
			// Note: With HT-359, there is now a second distinction. The checkbox _chkShowCheckForProblems controls that;
			// if we ever go back to having a Modes tab, that checkbox should be moved to that tab.
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
			foreach (var styleName in _project.AllEncounteredParagraphStyleNames)
			{
				_lbSkippedStyles.Items.Add(styleName, _project.IsSkippedStyle(styleName));
			}

			// Initialize Punctuation tab
			var scrProjectSettings = _project.ScrProjectSettings;
			if (scrProjectSettings?.FirstLevelStartQuotationMark == null ||
			    scrProjectSettings.FirstLevelEndQuotationMark == null)
			{
				// This project is not based on a Paratext project or Text Release Bundle with
				// first-level quotes defined, so there are no quotation mark settings for HearThis
				// to access and no reason for it to want to try to parse quotes.
				_chkBreakAtQuotes.Checked = false;
				_chkBreakAtQuotes.Visible = false;
			}
			else
			{
				_chkBreakAtQuotes.Checked = _project.ProjectSettings.BreakQuotesIntoBlocks;
			}
			_txtAdditionalBlockSeparators.Text = _project.ProjectSettings.AdditionalBlockBreakCharactersExcludingWhitespace;

			_cboSentenceEndingWhitespace.DisplayMember = _cboPauseWhitespace.DisplayMember =
				WhitespaceCharacter.LongNameMember;
			_cboSentenceEndingWhitespace.SummaryDisplayMember = _cboPauseWhitespace.SummaryDisplayMember =
				WhitespaceCharacter.CodePointMember;

			foreach (var wsChar in WhitespaceCharacter.AllWhitespaceCharacters)
			{
				_cboSentenceEndingWhitespace.Items.Add(wsChar,
					_project.ProjectSettings.AdditionalBlockBreakCharacterSet.Contains(wsChar));

				_cboPauseWhitespace.Items.Add(wsChar,
					_project.ProjectSettings.ClauseBreakCharacterSet.Contains(wsChar));
			}

			_chkBreakAtParagraphBreaks.Checked = _project.ProjectSettings.BreakAtParagraphBreaks;
			_txtClauseSeparatorCharacters.Text = _project.ProjectSettings.ClauseBreakCharactersExcludingWhitespace;
			_lblWarningExistingRecordings.Visible = ClipRepository.GetDoAnyClipsExistForProject(project.Name);
			_lblWarningExistingRecordings.ForeColor = _chkBreakAtQuotes.ForeColor;

			// Initialize Interface tab
			_chkShowBookAndChapterLabels.Checked = Settings.Default.DisplayNavigationButtonLabels;
			_cboColorScheme.DisplayMember = "Value";
			_cboColorScheme.ValueMember = "Key";
			_cboColorScheme.DataSource = new BindingSource(AppPalette.AvailableColorSchemes, null);
			_cboColorScheme.SelectedValue = Settings.Default.UserColorScheme;
			_chkShowCheckForProblems.Checked = Settings.Default.EnableCheckForProblemsViewInProtectedMode;
			if (_chkEnableClipShifting.Enabled)
				_chkEnableClipShifting.Checked = Settings.Default.AllowDisplayOfShiftClipsMenu;

			Program.RegisterLocalizable(this);
			HandleStringsLocalized();
		}

		public void HandleStringsLocalized()
		{
			_lblSkippingInstructions.Text = Format(_lblSkippingInstructions.Text, _project.Name);
			var shiftClipsMenuName = _getUiString(UiElement.ShiftClipsMenu);
			_chkEnableClipShifting.Text = Format(_chkEnableClipShifting.Text, shiftClipsMenuName);
			_chkShowCheckForProblems.Text = Format(_chkShowCheckForProblems.Text, _getUiString(UiElement.CheckForProblemsView));
			_lblShiftClipsMenuWarning.Text = Format(_lblShiftClipsMenuWarning.Text, shiftClipsMenuName, ProductName);
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
			var projSettings = _project.ProjectSettings;
			if (_chkBreakAtQuotes.Visible)
				projSettings.BreakQuotesIntoBlocks = _chkBreakAtQuotes.Checked;
			// Unfortunately, the Leave event doesn't get raised before the handling of the OK button click.
			RemoveDuplicateSeparatorCharactersFromAIfTheyAreInB(
				_txtAdditionalBlockSeparators.Focused ? _txtClauseSeparatorCharacters : _txtAdditionalBlockSeparators,
				_txtAdditionalBlockSeparators.Focused ? _txtAdditionalBlockSeparators : _txtClauseSeparatorCharacters);
			if (_cboSentenceEndingWhitespace.CheckedIndices.Count > 0 || _cboPauseWhitespace.CheckedIndices.Count > 0)
				Analytics.Track("Scriptio continua project");
			projSettings.AdditionalBlockBreakCharacters = _txtAdditionalBlockSeparators.Text.Replace("  ", " ").Trim() +
				_cboSentenceEndingWhitespace.Text;
			projSettings.ClauseBreakCharacters = _txtClauseSeparatorCharacters.Text.Replace("  ", " ").Trim() +
				_cboPauseWhitespace.Text;
			if (projSettings.BreakQuotesIntoBlocks || projSettings.AdditionalBlockBreakCharacters.Length > 0 ||
				projSettings.ClauseBreakCharacters != ", ; :")
			{
				var details = new Dictionary<string, string>(3)
				{
					["BreakQuotesIntoBlocks"] = projSettings.BreakQuotesIntoBlocks.ToString(),
					["AdditionalBlockBreakCharacters"] = projSettings.AdditionalBlockBreakCharacters,
					["ClauseBreakCharacters"] = projSettings.ClauseBreakCharacters
				};
				// REVIEW: We're firing this any time the user clicks OK and the values are not the
				// defaults, even if they didn't change anything. Is this what we want?
				Analytics.Track("Punctuation settings changed", details);
			}

			projSettings.BreakAtParagraphBreaks = _chkBreakAtParagraphBreaks.Checked;
			_project.SaveProjectSettings();

			// Save settings on Interface tab
			Settings.Default.DisplayNavigationButtonLabels = _chkShowBookAndChapterLabels.Checked;
			Settings.Default.AllowDisplayOfShiftClipsMenu = _chkEnableClipShifting.Checked;
			if (Settings.Default.UserColorScheme != (ColorScheme)_cboColorScheme.SelectedValue)
			{
				Settings.Default.RestartingToChangeColorScheme = true;
				Settings.Default.UserColorScheme = (ColorScheme)_cboColorScheme.SelectedValue;
				Settings.Default.Save();
				Application.Restart();
			}

			Settings.Default.EnableCheckForProblemsViewInProtectedMode = _chkShowCheckForProblems.Checked;
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

		private void UpdateWarningTextColor(object sender, EventArgs e)
		{
			var newAdditionalBlockSeparators = new HashSet<char>(ProjectSettings.StringToCharacterSet(_txtAdditionalBlockSeparators.Text));
			foreach (WhitespaceCharacter wsChar in _cboSentenceEndingWhitespace.CheckedItems)
				newAdditionalBlockSeparators.Add(wsChar);
			var projSettings = _project.ProjectSettings;
			_lblWarningExistingRecordings.ForeColor = ((!_chkBreakAtQuotes.Visible || _chkBreakAtQuotes.Checked == projSettings.BreakQuotesIntoBlocks) &&
				newAdditionalBlockSeparators.SetEquals(projSettings.AdditionalBlockBreakCharacterSet) &&
				_chkBreakAtParagraphBreaks.Checked == projSettings.BreakAtParagraphBreaks) ?
				_chkBreakAtQuotes.ForeColor : AppPalette.Red;
		}

		private void _txtAdditionalBlockSeparators_Leave(object sender, EventArgs e)
		{
			RemoveDuplicateSeparatorCharactersFromAIfTheyAreInB(_txtClauseSeparatorCharacters, _txtAdditionalBlockSeparators);
		}

		private void _txtClauseSeparatorCharacters_Leave(object sender, EventArgs e)
		{
			RemoveDuplicateSeparatorCharactersFromAIfTheyAreInB(_txtAdditionalBlockSeparators, _txtClauseSeparatorCharacters);
		}

		private void RemoveDuplicateSeparatorCharactersFromAIfTheyAreInB(TextBox a, TextBox b)
		{
			foreach (var ch in b.Text.Where(c => c != ' '))
				a.Text = a.Text.Replace(ch, ' ');
			a.Text = a.Text.Replace("   ", " ").Replace("  ", " ").Trim();
		}

		private void cboColorScheme_SelectedIndexChanged(object sender, EventArgs e)
		{
			lblColorSchemeChangeRestartWarning.Visible =
				Settings.Default.UserColorScheme != (ColorScheme)_cboColorScheme.SelectedValue;
		}

		private void chkEnableClipShifting_CheckedChanged(object sender, EventArgs e)
		{
			_lblShiftClipsExplanation.Visible = _lblShiftClipsMenuWarning.Visible = _chkEnableClipShifting.Checked;
		}
	}
}
