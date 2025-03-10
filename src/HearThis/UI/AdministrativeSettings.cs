// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using Paratext.Data;
using SIL.Extensions;
using SIL.Reporting;
using SIL.Scripture;
using static System.Reflection.Assembly;
using static System.String;
using static System.StringComparison;

namespace HearThis.UI
{
	public partial class AdministrativeSettings : Form, ILocalizable
	{
		private const string kWav = "WAV";

		public enum UiElement
		{
			ShiftClipsMenu,
			CheckForProblemsView,
		}
		private readonly Project _project;
		private readonly Func<UiElement, string> _getUiString;
		private readonly ExternalClipEditorInfo _clipEditorInfo;
#if MULTIPLEMODES
		private CheckBox _defaultMode;
		private readonly Image _defaultImage;
#endif
		private bool _userElectedToDeleteSkips;
		private readonly List<ScriptureRange> _origScriptureRangesToBreakByVerse = new List<ScriptureRange>();
		private readonly SortedDictionary<ScriptureRange, bool> _updatedScriptureRangesToBreakByVerse = new SortedDictionary<ScriptureRange, bool>();
		private bool _waitingForDetectRecordingsForNewRange = false;

		public AdministrativeSettings(Project project, Func<UiElement, string> getUiString, ExternalClipEditorInfo clipEditorInfo)
		{
			_project = project;
			_getUiString = getUiString;
			_clipEditorInfo = clipEditorInfo.Clone();
			InitializeComponent();

			var baseFontSize = _txtAdditionalBlockSeparators.Font.Size;
			_txtAdditionalBlockSeparators.Font = _txtClauseSeparatorCharacters.Font =
				new Font(project.FontName, baseFontSize * Settings.Default.ZoomFactor, FontStyle.Regular);

			var neededAdditionalTopMargin = _txtClauseSeparatorCharacters.Height - _cboPauseWhitespace.Height;
			if (neededAdditionalTopMargin > 0)
			{
				var margin = _cboPauseWhitespace.Margin;
				_cboPauseWhitespace.Margin = new Padding(margin.Left,
					margin.Top + neededAdditionalTopMargin, margin.Right, margin.Bottom);
			}

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

			// Initialize Record by verse tab
			if (project.ScriptProvider is ParatextScriptProvider paratextScript)
				InitializeRecordByVerseTab(paratextScript);
			else
				tabControl1.TabPages.Remove(tabPageRecordByVerse);

			// Initialize Clip Editor verse tab
			if (_clipEditorInfo.IsSpecified)
			{
				_lblPathToWAVFileEditor.Text = _clipEditorInfo.ApplicationPath;
				_txtEditingApplicationName.Text = _clipEditorInfo.ApplicationName;
				try
				{
					if (!File.Exists(_clipEditorInfo.ApplicationPath))
						_txtEditingApplicationName.ForeColor = Color.Red;
				}
				catch
				{
					_txtEditingApplicationName.ForeColor = Color.Red;
				}
				if (!IsNullOrEmpty(_clipEditorInfo.CommandLineParameters))
				{
					Debug.Assert(_rdoUseSpecifiedEditor.Checked);
					_chkWAVEditorCommandLineArguments.Checked = true;
					_txtCommandLineArguments.Text = _clipEditorInfo.CommandLineParameters;
				}

				SetWAVEditorControlsVisibility(true);
				UpdateDisplayOfCommandLineControls();
			}
			else
			{
				_rdoUseDefaultAssociatedApplication.Checked = true;
				_lblPathToWAVFileEditor.Text = "";
			}
			_txtEditingApplicationName.TextChanged += delegate
				{ _clipEditorInfo.ApplicationName = _txtEditingApplicationName.Text; };

			_lblWAVEditorCommandLineExample.Text = Format(_lblWAVEditorCommandLineExample.Text,
				ExternalClipEditorInfo.kClipPathPlaceholder);

			Program.RegisterLocalizable(this);
			HandleStringsLocalized();
		}

		public string SingleTabToShow
		{
			get => tabControl1.TabPages.Count == 1 ? tabControl1.TabPages[0].Tag as string : null;
			set
			{
				TabPage tabPageToKeep = null;
				for (var index = 0; index < tabControl1.TabPages.Count; index++)
				{
					TabPage tabPage = tabControl1.TabPages[index];
					if (tabPage.Tag as string == value)
					{
						tabPageToKeep = tabPage;
						break;
					}
				}

				if (tabPageToKeep == null)
					throw new ArgumentException("Caller requested a non-existent tab page.");

				tabControl1.TabPages.Clear();
				tabControl1.TabPages.Add(tabPageToKeep);
			}
		}

		private void InitializeRecordByVerseTab(ParatextScriptProvider paratextScript)
		{
			_verseCtrlRecordByVerseRangeStart.VerseRefChanged += VerseControlOnVerseRefChanged;
			_verseCtrlRecordByVerseRangeEnd.VerseRefChanged += VerseControlOnVerseRefChanged;

			var books = paratextScript.BookSet;
			if (books.Count == 0)
			{
				tabControl1.TabPages.Remove(tabPageRecordByVerse);
				return;
			}
			_verseCtrlRecordByVerseRangeStart.BooksPresentSet =
				_verseCtrlRecordByVerseRangeEnd.BooksPresentSet = books;
			_verseCtrlRecordByVerseRangeStart.ShowEmptyBooks = false;
			_verseCtrlRecordByVerseRangeEnd.ShowEmptyBooks = false;
			ResetRecordByVerseRangeControls(paratextScript);

			_gridVerseRanges.AddRemoveRowColumn(null, null, // Use default icons
				() => LocalizationManager.GetString("AdministrativeSettings.RecordByVerseRange.DeleteColumn_ToolTip_", "Delete this range"),
				DeleteRange, alwaysShowIcon:true);
			if (_project.ProjectSettings.RangesToBreakByVerse != null)
			{
				_origScriptureRangesToBreakByVerse.AddRange(_project.ProjectSettings.RangesToBreakByVerse.ScriptureRanges);
				foreach (var range in _origScriptureRangesToBreakByVerse)
					_gridVerseRanges.Rows.Add(range.StartRef.AsString, range.EndRef.AsString);
				_updatedScriptureRangesToBreakByVerse.AddRange(_origScriptureRangesToBreakByVerse
					.Select(r => new KeyValuePair<ScriptureRange, bool>(r, false)));
			}
		}

		private void ResetRecordByVerseRangeControls(ParatextScriptProvider paratextScript)
		{
			_verseCtrlRecordByVerseRangeStart.VerseRef = paratextScript.FirstAvailableScriptureRef;
			_verseCtrlRecordByVerseRangeEnd.VerseRef = paratextScript.LastAvailableScriptureRef;
		}

		public void HandleStringsLocalized()
		{
			_lblSkippingInstructions.Text = Format(_lblSkippingInstructions.Text, _project.Name);
			var shiftClipsMenuName = _getUiString(UiElement.ShiftClipsMenu);
			_chkEnableClipShifting.Text = Format(_chkEnableClipShifting.Text, shiftClipsMenuName);
			var checkForProblemsView = _getUiString(UiElement.CheckForProblemsView);
			_chkShowCheckForProblems.Text = Format(_chkShowCheckForProblems.Text, checkForProblemsView);
			var part2 = Format(LocalizationManager.GetString("\"AdministrativeSettings.WarningExistingRecordingsPart2",
				"This is because the way {0} separates text into blocks will no longer match how" +
				" it separated them when the original recordings were made. You can use {1} to" +
				" help you check to make sure everything is recorded properly.",
				"This is used as the second part of a couple warning messages in the " +
				"Administrative Settings dialog box." +
				"Param 0: \"HearThis\" (product name); " +
				"Param 1: \"Check for Problems\" view title"),
				Program.kProduct,
				checkForProblemsView);

			_lblShiftClipsMenuWarning.Text = Format(_lblShiftClipsMenuWarning.Text, shiftClipsMenuName, ProductName);
			_lblWarningExistingRecordings.Text = Format(_lblWarningExistingRecordings.Text, part2);
			_lblWarningRecordByVerse.Text = Format(_lblWarningExistingRecordings.Text, part2);

			_lblInstructions.Text = Format(_lblInstructions.Text, kWav, Program.kProduct,
				"Audacity", _getUiString(UiElement.CheckForProblemsView));
			_lblCommandLineArgumentsInstructions.Text = Format(
				_lblCommandLineArgumentsInstructions.Text, Program.kProduct, ExternalClipEditorInfo.kClipPathPlaceholder);
			_rdoUseSpecifiedEditor.Text = Format(_rdoUseSpecifiedEditor.Text, kWav);
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

			if (_updatedScriptureRangesToBreakByVerse.Count > 0)
			{
				if (projSettings.RangesToBreakByVerse == null)
					projSettings.RangesToBreakByVerse = new RangesToBreakByVerse();
				projSettings.RangesToBreakByVerse.ScriptureRanges =
					_updatedScriptureRangesToBreakByVerse.Keys.ToList();
			}
			else
			{
				projSettings.RangesToBreakByVerse = null;
			}

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

			// Save settings on the Clip Editor Tab
			if (_rdoUseSpecifiedEditor.Checked)
			{
				Debug.Assert(_clipEditorInfo.IsSpecified);
				_clipEditorInfo.CommandLineParameters = _chkWAVEditorCommandLineArguments.Checked?
					_txtCommandLineArguments.Text : null;
			}
			else
			{
				_clipEditorInfo.ApplicationPath = null;
				_clipEditorInfo.CommandLineParameters = null;
			}

			ExternalClipEditorInfo.PersistedSingleton.UpdateSettings(_clipEditorInfo);

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
		/// John thought this button added unnecessary complexity and wasn't worth it, so I made it
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
		
		private void DeleteRange(int row)
		{
			if (_gridVerseRanges.SelectedRows.Count != 1 ||
			    _gridVerseRanges.SelectedRows[0].Index != row ||
			    _gridVerseRanges.SelectedRows[0].IsNewRow)
				return;

			var rangeToRemove = new ScriptureRange(
				new BCVRef(_gridVerseRanges.Rows[row].Cells[colFrom.Index].Value.ToString()),
				new BCVRef(_gridVerseRanges.Rows[row].Cells[colTo.Index].Value.ToString()));
			_updatedScriptureRangesToBreakByVerse.Remove(rangeToRemove);

			UpdateDisplayWarningRecordByVerse();

			_gridVerseRanges.Rows.RemoveAt(row);
		}

		private void VerseControlOnVerseRefChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateAddRecordByVerseRangeButtonEnabledState();
		}

		private void VerseCtrlRecordByVerseRangeChanged(object sender, EventArgs e)
		{
			UpdateAddRecordByVerseRangeButtonEnabledState();
		}

		private void UpdateAddRecordByVerseRangeButtonEnabledState()
		{
			var startRef = _verseCtrlRecordByVerseRangeStart.VerseRef;
			var endRef = _verseCtrlRecordByVerseRangeEnd.VerseRef;
			var startBCV = startRef.ToBCV();
			var endBCV = endRef.ToBCV();
			_btnAddRecordByVerseRange.Enabled = !_waitingForDetectRecordingsForNewRange && startBCV <= endBCV &&
				!_updatedScriptureRangesToBreakByVerse.ContainsKey(new ScriptureRange(startBCV, endBCV));
		}

		private bool DoRecordingsExistForNewRange(ScriptureRange newRange)
		{
			var paratextScript = (ParatextScriptProvider)_project.ScriptProvider;

			return newRange.GetNewRangesToBreakByVerse(_origScriptureRangesToBreakByVerse)
				.SelectMany(range => paratextScript.GetAllChaptersInExistingBooksInRange(range))
				.Any(bookChapterTuple => ClipRepository.GetDoAnyClipsExistInChapter(_project.Name,
					bookChapterTuple.BookName, bookChapterTuple.Chapter));
		}

		/// <summary>
		/// Asynchronously determine whether any recordings exist for a newly added range of
		/// Scripture to be broken out verse by verse (rather than by sentence-ending punctuation)
		/// so that a warning can be displayed if necessary. On computers with fast drives this
		/// should be pretty fast for small ranges, but it could take a hot minute if the range is
		/// large (especially when no existing recordings are found, or one is not found until near
		/// the end of the range).
		/// </summary>
		private async Task DetectRecordingsForNewRange(ScriptureRange range)
		{
			Debug.Assert(_waitingForDetectRecordingsForNewRange);

			bool recordingsExist = await Task.Run(() => DoRecordingsExistForNewRange(range));

			_updatedScriptureRangesToBreakByVerse[range] = recordingsExist;

			if (InvokeRequired)
				Invoke((Action)UpdateDisplayWarningRecordByVerse);
			else
				UpdateDisplayWarningRecordByVerse();

			_waitingForDetectRecordingsForNewRange = false;

			if (InvokeRequired)
				Invoke((Action)UpdateAddRecordByVerseRangeButtonEnabledState);
			else
				UpdateAddRecordByVerseRangeButtonEnabledState();
		}

		private void UpdateDisplayWarningRecordByVerse()
		{
			_lblWarningRecordByVerse.Visible =
				_origScriptureRangesToBreakByVerse.Any(r => !_updatedScriptureRangesToBreakByVerse.ContainsKey(r)) ||
				_updatedScriptureRangesToBreakByVerse.Values.Any(k => k);
		}

		private void _btnAddRecordByVerseRange_Click(object sender, EventArgs e)
		{
			if (_waitingForDetectRecordingsForNewRange)
			{
				Debug.Fail("Add button should not be enabled while waiting for DetectRecordingsForNewRange to complete");
				return;
			}

			var startRef = _verseCtrlRecordByVerseRangeStart.VerseRef;
			var endRef = _verseCtrlRecordByVerseRangeEnd.VerseRef;
			var startBcv = new BCVRef(startRef.BookNum, startRef.ChapterNum, startRef.VerseNum);
			var endBvc = new BCVRef(endRef.BookNum, endRef.ChapterNum, endRef.VerseNum);
			Debug.Assert(startBcv <= endBvc);

			var newRange = new ScriptureRange(startBcv, endBvc);

			_waitingForDetectRecordingsForNewRange = true;
			_updatedScriptureRangesToBreakByVerse.Add(newRange, false);
			var i = _updatedScriptureRangesToBreakByVerse.Keys.IndexOf(newRange);

			Task.Run(() => DetectRecordingsForNewRange(newRange));

			_gridVerseRanges.Rows.Insert(i, startRef, endRef);

			ResetRecordByVerseRangeControls((ParatextScriptProvider)_project.ScriptProvider);

			UpdateAddRecordByVerseRangeButtonEnabledState();
		}

		private void _chkWAVEditorCommandLineArguments_CheckedChanged(object sender, EventArgs e)
		{
			UpdateDisplayOfCommandLineControls();
			if (_chkWAVEditorCommandLineArguments.Checked)
			{
				if (_txtCommandLineArguments.Tag is string restoreArgsText)
					_txtCommandLineArguments.Text = restoreArgsText;
				_txtCommandLineArguments.Focus();
			}
			else
			{
				_txtCommandLineArguments.Tag = _txtCommandLineArguments.Text;
				_txtCommandLineArguments.Text = "";
			}
		}

		private void _btnOpenFileChooser_Click(object sender, EventArgs e)
		{
			Logger.WriteEvent("Selecting Clip Editor");
			Analytics.Track("Selecting Clip Editor");

			const string kExe = "exe";
			using (var dlg = new OpenFileDialog())
			{
				dlg.Title = Format(LocalizationManager.GetString(
					"AdministrativeSettings.ClipEditor.SelectEditorDialog.Title",
					"Select a {0} file editing application",
					"Param is \"WAV\" (file format)"),
					kWav);
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
					LocalizationManager.GetString("AdministrativeSettings.ClipEditor.SelectEditorDialog.ApplicationFileTypeLabel",
						"Applications", "File type label"), "*." + kExe,
					LocalizationManager.GetString("AdministrativeSettings.ClipEditor.SelectEditorDialog.ExecutableFileTypeLabel",
						"All Executable files", "File type label"), "*." + kExe + "; *.bat; *.cmd; *.com",
					LocalizationManager.GetString("AdministrativeSettings.ClipEditor.SelectEditorDialog.AllFilesLabel", "All Files",
						"File type label used for \"*.*\""), "*.*");
				dlg.DefaultExt = kExe;
				dlg.CheckFileExists = true;

				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					// If the user selected HearThis, we don't want to allow that.
					if (string.Equals(GetEntryAssembly()?.Location, dlg.FileName, OrdinalIgnoreCase))
						return; // Could/should show a message box here, but this is a rare case and not worth the effort.
					_clipEditorInfo.ApplicationPath = dlg.FileName;
					if (_clipEditorInfo.IsSpecified)
					{
						_lblPathToWAVFileEditor.Text = _clipEditorInfo.ApplicationPath;
						_lblPathToWAVFileEditor.ForeColor = _txtCommandLineArguments.ForeColor;
						if (_clipEditorInfo.ApplicationName != null)
							_txtEditingApplicationName.Text = _clipEditorInfo.ApplicationName;
						_rdoUseSpecifiedEditor.Checked = true;
						UpdateDisplayOfWAVEditorControls();
					}
				}
			}
		}

		private void HandleWavEditorOptionChanged(object sender, EventArgs e)
		{
			if (sender is RadioButton rdo && rdo.Checked)
			{
				if (rdo == _rdoUseSpecifiedEditor)
				{
					_clipEditorInfo.ApplicationPath = _lblPathToWAVFileEditor.Text;
					if (!_clipEditorInfo.IsSpecified)
						_btnOpenFileChooser_Click(_btnOpenFileChooser, EventArgs.Empty);
					_rdoUseDefaultAssociatedApplication.Checked = !_clipEditorInfo.IsSpecified;
				}
				else
				{
					Analytics.Track("Using Default App for Clip Editing");
					_rdoUseSpecifiedEditor.Checked = false;
				}

				UpdateDisplayOfWAVEditorControls();
			}
		}

		private void UpdateDisplayOfWAVEditorControls()
		{
			if (_rdoUseDefaultAssociatedApplication.Checked)
				_clipEditorInfo.UseAssociatedDefaultApplication();
			var editorSpecified = _rdoUseSpecifiedEditor.Checked && _lblPathToWAVFileEditor.Text.Length > 0;
			if (_lblWAVFileEditingApplicationName.Visible)
			{
				// If the name-related controls have already been displayed, we don't want them flashing on and off,
				// so just disable them if the user changes back to using the default associated application
				_lblPathToWAVFileEditor.Enabled = _lblWAVFileEditingApplicationName.Enabled = _txtEditingApplicationName.Enabled =
					_chkWAVEditorCommandLineArguments.Enabled = editorSpecified;
			}
			else
			{
				SetWAVEditorControlsVisibility(editorSpecified);
			}
			UpdateDisplayOfCommandLineControls();
		}

		private void SetWAVEditorControlsVisibility(bool visible)
		{
			_lblPathToWAVFileEditor.Visible = _lblWAVFileEditingApplicationName.Visible =
				_txtEditingApplicationName.Visible = _chkWAVEditorCommandLineArguments.Visible =
					visible;
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
							_chkWAVEditorCommandLineArguments.Enabled &&
							_chkWAVEditorCommandLineArguments.Checked;
			}

			//// Unfortunately, there does not seem to be a way to auto-size the dialog. It always makes itself
			//// big enough to show everything, even if stuff is hidden.
			//var lastVisibleInstrLabel = _lblWAVEditorCommandLineExample.Visible ? _lblWAVEditorCommandLineExample :
			//	_lblInstructions;
			//if (lastVisibleInstrLabel.Bottom > _rdoUseSpecifiedEditor.Top)
			//{
			//	Height += lastVisibleInstrLabel.Bottom - _rdoUseSpecifiedEditor.Top + _rdoUseSpecifiedEditor.Margin.Top +
			//		lastVisibleInstrLabel.Margin.Bottom;
			//	MinimumSize = Size;
			//}
		}
	}
}
