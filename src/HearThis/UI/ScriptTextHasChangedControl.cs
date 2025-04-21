// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020-2025, SIL Global.
// <copyright from='2020' to='2025' company='SIL Global'>
//		Copyright (c) 2020-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using HearThis.Script;
using HearThis.StringDifferences;
using L10NSharp;
using SIL.Reporting;
using SIL.Windows.Forms.SettingProtection;
using static System.String;
using static HearThis.UI.AdministrativeSettings;
using DateTime = System.DateTime;

namespace HearThis.UI
{
	/// <summary>
	/// This class holds the text that the user is supposed to be reading (the main pane on the bottom left of the UI).
	/// </summary>
	public partial class ScriptTextHasChangedControl : UserControl, ILocalizable
	{
		private Project _project;
		private bool _updatingDisplay;
		private bool _haveDisplayedExternalEditorSelection;
		private static float s_zoomFactor;
		private string _standardProblemText;
		private string _okayResolutionText;
		private string _standardDeleteExplanationText;
		private string _fmtRecordedDate;
		private string _fmtEditingInstructions;
		private ScriptLine _lastNullScriptLineIgnored;
		/// <summary>
		/// Information about the current text of the script (from the project)
		/// </summary>
		private ScriptLine CurrentScriptLine { get; set; }
		private StringDifferenceFinder _currentThenNowDifferences;
		public event EventHandler ProblemIgnoreStateChanged;
		public event EventHandler NextClick;
		public delegate void DisplayUpdatedHandler(ScriptTextHasChangedControl sender, bool displayingOptions);
		public event DisplayUpdatedHandler DisplayUpdated;
		public delegate void DisplayedWithClippedControlsHandler(ScriptTextHasChangedControl sender, ushort increasedVerticalSizeNeeded);
		public event DisplayedWithClippedControlsHandler DisplayedWithClippedControls;
		private ShiftClipsViewModel _shiftClipsViewModel;
		private Dictionary<Control, Action<Control>> _actionsToSetLocalizedTextForCtrls;

		private ExternalClipEditorInfo ExtClipEditorInfo => ExternalClipEditorInfo.PersistedSingleton;

		internal Func<UiElement, string> GetUIString { get; set; }

		public ScriptTextHasChangedControl()
		{
			InitializeComponent();

			Program.RegisterLocalizable(this);
			HandleStringsLocalized();

			_txtThen.BackColor = AppPalette.Background;
			_txtNow.BackColor = AppPalette.Background;
			_txtThen.ForeColor = AppPalette.TitleColor;
			_txtNow.ForeColor = AppPalette.TitleColor;
			_lblProblemSummary.ForeColor = _tableProblem.ForeColor =
				_nextButton.RoundedBorderColor = AppPalette.HilightColor;
			_btnPlayClip.SetUnconditionalFlatBackgroundColor(AppPalette.Background);
			_btnEditClip.SetUnconditionalFlatBackgroundColor(AppPalette.Background);

			_btnAskLater.CorrespondingRadioButton = _rdoAskLater;
			_btnUseExisting.CorrespondingRadioButton = _rdoUseExisting;
			_btnDelete.CorrespondingRadioButton = _rdoReRecord;

			_editSoundFile.Tag = _editSoundFile.Image;

			OnSettingsProtectionChanged(null, null);
			SettingsProtectionSettings.Default.PropertyChanged += OnSettingsProtectionChanged;

			ExtClipEditorInfo.SettingsChanged += OnExternalClipEditorSettingsChanged;
		}

		private void OnExternalClipEditorSettingsChanged(ExternalClipEditorInfo sender)
		{
			if (_pnlPlayClip.Visible)
				UpdateEditClipButton();
		}

		public void HandleStringsLocalized()
		{
			_standardProblemText = _lblProblemSummary.Text;
			_okayResolutionText = _lblResolution.Text;
			_standardDeleteExplanationText = _btnDelete.Text;
			_fmtRecordedDate = _lblBefore.Text;
			_fmtEditingInstructions = _lblEditingCompleteInstructions.Text;
			if (_actionsToSetLocalizedTextForCtrls != null)
			{
				foreach (var action in _actionsToSetLocalizedTextForCtrls)
					action.Value(action.Key);
			}
			_btnEditClip.Tag = _btnEditClip.Text;
			if (IsHandleCreated)
				UpdateEditClipButton();
		}

		private void OnSettingsProtectionChanged(object sender, PropertyChangedEventArgs e)
		{
			// If an administrator has not set up an external clip editing program and settings
			// are locked, then we won't let the user edit clips in an external program even if
			// they have access to the Check for Problems mode.
			_btnEditClip.Visible = _editSoundFile.Visible =
				!SettingsProtectionSettings.Default.NormallyHidden || ExtClipEditorInfo.IsSpecified;
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (Visible)
			{
				if (!_updatingDisplay)
					UpdateState();
			}
			else
			{
				_audioButtonsControl.StopPlaying();
			}
		}

		private void OnNextButton(object sender, EventArgs e)
		{
			NextClick?.Invoke(this, e);
		}

		public void SetData(Project project)
		{
			_project = project;

			CurrentScriptLine = _project.SelectedBook.HasTranslatedContent ? _project.ScriptOfSelectedBlock : null;
			SetScriptFonts();
			UpdateState();
		}

		private void SetScriptFonts()
		{
			if (_project == null || !_project.SelectedBook.HasTranslatedContent)
				return;

			_txtNow.Font = _txtThen.Font = new Font(_project.FontNameForSelectedBlock,
				_project.FontSizeForSelectedBlock * ZoomFactor);

			if (_currentThenNowDifferences != null)
			{
				SetRichText(_txtThen, _currentThenNowDifferences.OriginalStringDifferences);
				SetRichText(_txtNow, _currentThenNowDifferences.NewStringDifferences);
			}
		}

		public void UpdateState()
		{
			_lastNullScriptLineIgnored = null;

			if (InvokeRequired)
				Invoke(new Action(UpdateDisplay));
			else
				UpdateDisplay();

			Task.Run(UpdateAudioButtonsControl);
		}

		private void UpdateAudioButtonsControl()
		{
			// We do this asynchronously to prevent deadlock on the control.
			// Nothing should keep the control locked for long, so it should always get
			// done very quickly. Since we don't lock _project, it is conceivable that
			// it could change in the middle of this operation, but it's practically
			// impossible. If it were to change, another background worker would get
			// kicked off right away. That one wouldn't be able to enter until we
			// released the lock on _audioButtonsControl, so everything would end up
			// in a consistent state.
			lock (_audioButtonsControl)
			{
				if (Visible && _project != null)
				{
					_audioButtonsControl.Path = _project.ClipFilePathForSelectedLine;
					_audioButtonsControl.ContextForAnalytics = _project.GetAudioContextInfoForAnalytics(CurrentScriptLine);
				}
				else
					_audioButtonsControl.Path = null;
			}

			if (InvokeRequired)
				Invoke(new Action(() => _audioButtonsControl.UpdateDisplay()));
			else
				_audioButtonsControl.UpdateDisplay();
		}

		private bool HaveScript => CurrentScriptLine?.Text?.Length > 0;

		private ChapterInfo CurrentChapterInfo => _project.SelectedChapterInfo;

		private ScriptLine CurrentRecordingInfo => CurrentScriptLine == null ? null :
			_project.GetCurrentOrDeletedRecordingInfo(CurrentScriptLine.Number);

		private DateTime FileRecordingTime { get; set; }

		private void SetBeforeDateLabel(DateTime recordingTime)
		{
			FileRecordingTime = recordingTime;
			void SetBeforeText(Control c) => c.Text = Format(_fmtRecordedDate,
				FileRecordingTime.ToLocalTime().ToShortDateString());
			SetBeforeText(_lblBefore);
			_actionsToSetLocalizedTextForCtrls[_lblBefore] = SetBeforeText;
		}

		private string ReadyToReRecordText => 
			LocalizationManager.GetString("ScriptTextHasChangedControl.DecisionReRecord",
				"Decision: Need to re-record this block.");

		private void UpdateDisplay()
		{
			if (_project == null)
			{
				Hide();
				return; // Not ready yet
			}

			_actionsToSetLocalizedTextForCtrls = new Dictionary<Control, Action<Control>>();

			var currentRecordingInfo = CurrentRecordingInfo;

			_updatingDisplay = true;

			_lblEditingCompleteInstructions.Visible = _copyPathToClipboard.Visible = false;

			if (!HaveScript)
			{
				if (_project.IsExtraClipSelected)
				{
					UpdateDisplayForExtraRecording();
					DeterminePossibleClipShifts();
					ShowNextButtonIfThereAreMoreProblemsInChapter();
				}
				else
					Hide(); // Not ready yet
				_updatingDisplay = false;
				return;
			}

			if (currentRecordingInfo != null && currentRecordingInfo.Number - 1 != _project.SelectedScriptBlock)
				return; // Initializing during restart to change color scheme... not ready yet

			ResetDisplayToProblemState();
			var haveRecording = _project.HasRecordedClipForSelectedScriptLine();
			var haveBackup = !haveRecording && _project.HasBackupFileForSelectedBlock();
			_pnlPlayClip.Visible = haveRecording;
			if (haveRecording)
				UpdateEditClipButton();

			_btnUseExisting.Visible = _btnUseExisting.Enabled = _btnDelete.Visible = 
				_lblBefore.Visible = _txtThen.Visible = 
				 haveRecording || haveBackup;
			void SetDeleteButtonText(Control b) => b.Text = _standardDeleteExplanationText;
			SetDeleteButtonText(_btnDelete);
			_actionsToSetLocalizedTextForCtrls[_btnDelete] = SetDeleteButtonText;
			_tableOptions.Visible = _lblNow.Visible = _txtNow.Visible = true;
			ShowNextButtonIfThereAreMoreProblemsInChapter();

			_txtNow.Text = CurrentScriptLine.Text;
			_currentThenNowDifferences = null;

			DeterminePossibleClipShifts();

			if (CurrentScriptLine.Skipped)
			{
				if (haveRecording)
				{
					SetThenInfo(currentRecordingInfo);

					void SetProblemSummaryTextToBlockSkippedButHasClip(Control b) => b.Text =
						LocalizationManager.GetString("ScriptTextHasChangedControl.BlockSkippedButHasClip",
						"This block has been skipped, but it has a clip recorded.");
					SetProblemSummaryTextToBlockSkippedButHasClip(_lblProblemSummary);
					_actionsToSetLocalizedTextForCtrls[_lblProblemSummary] = SetProblemSummaryTextToBlockSkippedButHasClip;

					SetDisplayForDeleteCleanupAction();
				}
				else
				{
					ShowNoProblemState();
					void SetProblemSummaryTextToBlockSkipped(Control b) => b.Text =
						LocalizationManager.GetString("ScriptTextHasChangedControl.BlockSkipped",
							"This block has been skipped.");
					SetProblemSummaryTextToBlockSkipped(_lblProblemSummary);
					_actionsToSetLocalizedTextForCtrls[_lblProblemSummary] = SetProblemSummaryTextToBlockSkipped;
					_txtThen.Visible = false;
				}
			}
			else if (!haveRecording && (!haveBackup || (currentRecordingInfo != null && (currentRecordingInfo.OriginalText == null ||
				currentRecordingInfo.OriginalText == CurrentScriptLine.Text))))
			{
				ShowNoProblemState();
				_lblBefore.Visible = _txtThen.Visible = false;
				void SetProblemSummaryTextToNotRecorded(Control b) => b.Text =
					LocalizationManager.GetString("ScriptTextHasChangedControl.NotRecorded",
						"This block has not yet been recorded.");
				SetProblemSummaryTextToNotRecorded(_lblProblemSummary);
				_actionsToSetLocalizedTextForCtrls[_lblProblemSummary] = SetProblemSummaryTextToNotRecorded;
			}
			else if (currentRecordingInfo == null)
			{
				void SetProblemSummaryTextToScriptTextAtTimeOfRecordingUnknown(Control b) => b.Text =
					Format(LocalizationManager.GetString("ScriptTextHasChangedControl.ScriptTextAtTimeOfRecordingUnknown",
						"The clip for this block was recorded using an older version of {0} that did not save the version of the text at the time of recording.",
						"Param 0: \"HearThis\" (product name)"), ProductName);
				SetProblemSummaryTextToScriptTextAtTimeOfRecordingUnknown(_lblProblemSummary);
				_actionsToSetLocalizedTextForCtrls[_lblProblemSummary] = SetProblemSummaryTextToScriptTextAtTimeOfRecordingUnknown;

				_txtThen.Visible = false;

				if (haveRecording)
				{
					// We have a clip, but we don't know anything about the script at the time it was recorded.
					_problemIcon.Image = AppPalette.ScriptUnknownIcon;
					SetBeforeDateLabel(_project.GetActualClipRecordingTime(_project.SelectedScriptBlock));
				}
				else
				{
					SetBeforeDateLabel(_project.GetActualClipBackupRecordingTimeForSelectedBlock());
					ShowResolution(_btnDelete, () => ReadyToReRecordText);
				}
			}
			else
			{
				SetThenInfo(currentRecordingInfo);

				if (_txtNow.Text != _txtThen.Text ||
				    (currentRecordingInfo.OriginalText != null && currentRecordingInfo.OriginalText != CurrentScriptLine.Text) ||
				    haveBackup)
				{
					if (_txtNow.Text == _txtThen.Text)
					{
						_lblNow.Visible = _lblBefore.Visible = false;
						Debug.Fail("I think this is impossible.");
					}
					void SetProblemSummaryToStandardProblemText(Control b) =>
						b.Text =_standardProblemText;
					SetProblemSummaryToStandardProblemText(_lblProblemSummary);
					_actionsToSetLocalizedTextForCtrls[_lblProblemSummary] = SetProblemSummaryToStandardProblemText;

					if (haveBackup)
						ShowResolution(_btnDelete, () => ReadyToReRecordText);
					else if (currentRecordingInfo.OriginalText != null && _txtNow.Text != currentRecordingInfo.OriginalText &&
					         // Have to check this because the text might have changed AGAIN since the last time the user said
							 // it was okay:
					         _txtNow.Text == currentRecordingInfo.Text)
						ShowResolution(_btnUseExisting);
				}
				else
				{
					_lblNow.Visible = _lblBefore.Visible = false;

					Debug.Assert(_lastNullScriptLineIgnored == null);

					ShowNoProblemState();
					void SetProblemSummaryTextToNoProblem(Control b) => b.Text =
						LocalizationManager.GetString("ScriptTextHasChangedControl.NoProblem",
							"No problems");
					SetProblemSummaryTextToNoProblem(_lblProblemSummary);
					_actionsToSetLocalizedTextForCtrls[_lblProblemSummary] = SetProblemSummaryTextToNoProblem;
					_txtNow.Visible = false;
				}
			}

			Show();
			UpdateThenVsNowTableLayout();
			Focus();

			_audioButtonsControl.UpdateDisplay();
			_updatingDisplay = false;

			DisplayUpdated?.Invoke(this, _tableOptions.Visible);
		}

		private void UpdateEditClipButton()
		{
			if (ExtClipEditorInfo.ApplicationName != null)
			{
				_btnEditClip.Text = Format(LocalizationManager.GetString(
						"ScriptTextHasChangedControl.PlayClipButtonFmt", "Open clip in {0}",
						"Param is the name of the WAV editor selected by the user"),
					ExtClipEditorInfo.ApplicationName);
			}
			else if (_btnEditClip.Tag is string origText)
			{
				_btnEditClip.Text = origText;
			}
		}

		private void ShowNoProblemState()
		{
			_tableOptions.Visible = _lblNow.Visible = false;
			_problemIcon.Image = null;
		}

		private void ResetDisplayToProblemState()
		{
			_problemIcon.Image = AppPalette.AlertCircleIcon;
			HideResolution();
			_rdoAskLater.Checked = true;
		}

		private void SetThenInfo(ScriptLine recordingInfo)
		{
			if (recordingInfo?.TextAsOriginallyRecorded == null)
				_txtThen.Visible = _lblBefore.Visible = false;
			else
			{
				// If the two texts are the same, we will be hiding the "now" text in the calling code.
				// We do this to turn \u2028 into \n. Otherwise, the comparison fails.
				_txtThen.Text = recordingInfo.TextAsOriginallyRecorded;
				if (_txtNow.Text.Length > 0 && _txtNow.Text != _txtThen.Text)
				{
					_currentThenNowDifferences = new StringDifferenceFinder(recordingInfo.TextAsOriginallyRecorded, _txtNow.Text);
					SetRichText(_txtThen, _currentThenNowDifferences.OriginalStringDifferences);
					SetRichText(_txtNow, _currentThenNowDifferences.NewStringDifferences);
				}

				SetBeforeDateLabel(recordingInfo.RecordingTime);
				_txtThen.Visible = true;
			}
		}

		private void SetRichText(RichTextBox box, IEnumerable<StringDifferenceSegment> segments)
		{
			box.Text = Empty;
			int start = 0;

			foreach (var seg in segments)
			{
				// append the text to the RichTextBox control
				box.AppendText(seg.Text);
				int end = box.TextLength;

				// select the new text
				box.Select(start, end - start);
				// set the attributes of the new text
				box.SelectionColor = GetSegmentColor(seg.Type, box.ForeColor);
				// unselect
				box.Select(end, 0);
				start = end;
			}
		}

		private Color GetSegmentColor(DifferenceType segType, Color defaultColor)
		{
			switch (segType)
			{
				case DifferenceType.Same:
					return defaultColor;
				case DifferenceType.Addition:
					return AppPalette.Recording;
				case DifferenceType.Deletion:
					return AppPalette.Red;
				default:
					throw new ArgumentOutOfRangeException(nameof(segType), segType, null);
			}
		}

		private void ShowNextButtonIfThereAreMoreProblemsInChapter()
		{
			_nextButton.Visible = HasMoreProblemsInChapter;
			_nextButton.Invalidate();
		}

		private void SetDisplayForDeleteCleanupAction()
		{
			_rdoUseExisting.Visible = _btnUseExisting.Visible = false;

			void SetDeleteButtonText(Control b) => b.Text = LocalizationManager.GetString(
				"ScriptTextHasChangedControl.DeleteClipButtonText", "Delete clip");
			SetDeleteButtonText(_btnDelete);
			_actionsToSetLocalizedTextForCtrls[_btnDelete] = SetDeleteButtonText;

			_rdoReRecord.Visible = _btnDelete.Visible = true;
		}

		private void ShowResolution(RadioButtonHelperButton buttonWithIcon, Func<string> getResolutionText = null)
		{
			if (getResolutionText == null)
				getResolutionText = () => _okayResolutionText;
			_lblResolution.Visible = true;
			_lblResolution.Text = getResolutionText();
			_actionsToSetLocalizedTextForCtrls[_lblResolution] = l => l.Text = getResolutionText();
			// This will usually be the case, but if we are called from UpdateDisplay (i.e.,
			// initially setting up the state for the block) and the block is already in a
			// resolved state, this will ensure the correct option is selected.
			buttonWithIcon.CorrespondingRadioButton.Checked = true;
			_problemIcon.Image = AppPalette.CurrentColorScheme == ColorScheme.HighContrast ?
				buttonWithIcon.HighContrastMouseOverImage : buttonWithIcon.MouseOverImage;
			_pnlPlayClip.Visible = _project.SelectedLineHasClip;
			_lblEditingCompleteInstructions.Visible = _copyPathToClipboard.Visible = false;
			ProblemIgnoreStateChanged?.Invoke(this, EventArgs.Empty);
		}

		private void UpdateDisplayForExtraRecording()
		{
			_tableOptions.Visible = _btnAskLater.Visible = true;

			SetDisplayForDeleteCleanupAction();

			Show();

			if (_project.SelectedLineHasClip)
			{
				ResetDisplayToProblemState();
				_pnlPlayClip.Visible = true;
			}
			else
			{
				ShowDeleteResolution();
				_pnlPlayClip.Visible = false;
			}

			void SetProblemSummaryTextToExtraClip(Control b) => b.Text =
				LocalizationManager.GetString("ScriptTextHasChangedControl.ExtraClip",
					"The text has changed so that this clip does not match up to any block in the current script.");
			SetProblemSummaryTextToExtraClip(_lblProblemSummary);
			_actionsToSetLocalizedTextForCtrls[_lblProblemSummary] = SetProblemSummaryTextToExtraClip;

			_txtNow.Text = ""; // Need to do this before calling SetThenInfo to prevent bogus diff.
			SetThenInfo(_project.GetRecordingInfoOfSelectedExtraBlock);

			_lblNow.Visible = _txtNow.Visible = false;

			UpdateThenVsNowTableLayout();
			
			DisplayUpdated?.Invoke(this, _tableOptions.Visible);
		}

		private void UpdateThenVsNowTableLayout()
		{
			if (!_txtThen.Visible)
				_txtThen.Text = "";
			if (!_txtNow.Visible)
				_txtNow.Text = "";

			if (_txtThen.Text.Length == 0)
			{
				if (_txtNow.Text.Length == 0)
					return;

				_tableBlockText.ColumnStyles[0].Width = 0;
			}
			else
			{
				_tableBlockText.ColumnStyles[0].Width = 50;
			}

			_tableBlockText.ColumnStyles[1].Width = _txtNow.Text.Length == 0 ?
				0 : 50;

			if (DisplayedWithClippedControls != null)
			{
				int needed = tableMaster.PreferredSize.Height - tableMaster.Height;
				if (needed > 0 && needed < ushort.MaxValue)
					DisplayedWithClippedControls.Invoke(this, (ushort)needed);
			}
		}

		/// <summary>
		/// This class has special handling for a few of the keys that RecordingToolControl
		/// normally handles: Tab and sometimes PageDown and Delete
		/// </summary>
		public bool PreFilterKey(Keys key)
		{
			switch (key)
			{
				case Keys.Tab:
					_audioButtonsControl.OnPlay(this, null);
					return true;

				case Keys.PageDown:
					if (_nextButton.Visible)
					{
						OnNextButton(this, null);
						return true;
					}
					break;

				case Keys.Delete:
					if (_rdoReRecord.Visible && !_rdoReRecord.Checked)
					{
						_rdoReRecord.Checked = true;
						return true;
					}
					break;

				case Keys.Space:
					if (HaveScript && !CurrentScriptLine.Skipped)
					{
						// REVIEW: Would it be better to tell them which view to use? To make
						// localization smooth, this would require exposing the name of that view
						// (in Shell) and accessing Shell using FindForm here, so that's a little
						// undesirable. Also, it has the vague downside that if we later add
						// another view where recording is possible, we might need/want to change
						// the message so as not to be misleading.
						MessageBox.Show(LocalizationManager.GetString(
							"ScriptTextHasChangedControl.CannotRecord",
							"You cannot record in this view."));
					}
					return true;
			}

			return false;
		}

		public float ZoomFactor
		{
			get => s_zoomFactor;
			set
			{
				s_zoomFactor = value;
				SetScriptFonts();
			}
		}

		public bool HasMoreProblemsInChapter => CurrentChapterInfo.GetIndexOfNextUnfilteredBlockWithProblem(
			_project.SelectedScriptBlock) > _project.SelectedScriptBlock;

		public bool DeleteClip()
		{
			try
			{
				_audioButtonsControl.StopPlaying();
				if (!_project.DeleteClipForSelectedBlock())
					return false;
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				ErrorReport.ReportNonFatalException(exception);
				if (_rdoAskLater.Visible)
				{
					_updatingDisplay = true; // Prevent side effect of selecting "Ask later"
					_rdoAskLater.Checked = true;
					ResetDisplayToProblemState();
					ProblemIgnoreStateChanged?.Invoke(this, EventArgs.Empty);
					_updatingDisplay = false;
				}
				DisplayUpdated?.Invoke(this, _tableOptions.Visible);
				return false;
			}

			if (_problemIcon.Image == null)
			{
				UpdateDisplay();
			}
			else
			{
				ShowDeleteResolution();
				DisplayUpdated?.Invoke(this, _tableOptions.Visible);
			}

			return true;
		}

		private void ShowDeleteResolution()
		{
			ShowResolution(_btnDelete, () =>
				(!_tableOptions.Visible || _rdoUseExisting.Visible) ? ReadyToReRecordText :
					LocalizationManager.GetString("ScriptTextHasChangedControl.DeletedClip",
						"This problem has been resolved (extra clip deleted)."));
		}

		private void UseExistingClip()
		{
			// "Ignore" this problem.
			var scriptLine = CurrentRecordingInfo;
			if (scriptLine == null)
			{
				// Just update the timestamp
				scriptLine = CurrentScriptLine;
				scriptLine.RecordingTime = FileRecordingTime;
				_lastNullScriptLineIgnored = CurrentScriptLine;
			}
			else
			{
				if (!_project.HasRecordedClipForSelectedScriptLine())
				{
					// Going from deleted state to "use existing" state.
					if (!_project.RestoreClipForSelectedBlock())
					{
						// This will probably be mildly jarring, but this should be extremely rare,
						// and anything less just leaves the UI in a confusing state that is more
						// difficult to recover from later.
						UpdateDisplay();
						return;
					}
				}

				_pnlPlayClip.Visible = true;

				if (scriptLine.OriginalText == null)
					scriptLine.OriginalText = scriptLine.Text;
			}

			scriptLine.Text = CurrentScriptLine.Text;

			Exception error = null;
			CurrentChapterInfo.OnScriptBlockRecorded(scriptLine, e =>
			{
				ErrorReport.ReportNonFatalException(e);
				error = e;
				return true;
			});
			Debug.Assert(CurrentRecordingInfo != null);
			if (error == null)
				ShowResolution(_btnUseExisting);
			else
				UpdateDisplay(); // Less-than-ideal, but we'll recover.
		}

		public void UndoDeleteOfClipWithoutProblems()
		{
			if (!_project.RestoreClipForSelectedBlock())
			{
				Debug.Fail("Either we are in a bad state, or the Move failed.");
				return;
			}

			UpdateState();

			HideResolution();
			_problemIcon.Image = null;
			_pnlPlayClip.Visible = true;

			CurrentChapterInfo.OnScriptBlockRecorded(CurrentRecordingInfo);

			DisplayUpdated?.Invoke(this, _tableOptions.Visible);
		}

		private void HideResolution()
		{
			_lblResolution.Visible = false;
			// The following isn't technically necessary since we ALWAYS set the label
			// to have the appropriate text before re-displaying it, but it's cleaner.
			_actionsToSetLocalizedTextForCtrls.Remove(_lblResolution);
		}

		private void RevertToProblemState()
		{
			ResetDisplayToProblemState();

			if (_project.RestoreClipForSelectedBlock())
			{
				_pnlPlayClip.Visible = true;
			}
			else
			{

				// Un-ignore.
				if (_lastNullScriptLineIgnored == CurrentScriptLine)
				{
					CurrentChapterInfo.RemoveRecordingInfo(_lastNullScriptLineIgnored);
					_lastNullScriptLineIgnored = null;
				}
				else
				{
					var scriptLine = CurrentRecordingInfo;
					scriptLine.Text = scriptLine.OriginalText;
					scriptLine.OriginalText = null;
					CurrentChapterInfo.OnScriptBlockRecorded(scriptLine);
				}
			}
			ProblemIgnoreStateChanged?.Invoke(this, EventArgs.Empty);
		}

		private void DeterminePossibleClipShifts()
		{
			if (!_project.HasRecordedClip(_project.SelectedScriptBlock))
			{
				_shiftClipsViewModel = null;
				_btnShiftClips.Visible = _iconShiftClips.Visible = _lblShiftClips.Visible = false;
				return;
			}

			_shiftClipsViewModel = new ShiftClipsViewModel(_project);
			_btnShiftClips.Visible = _iconShiftClips.Visible = _lblShiftClips.Visible = _shiftClipsViewModel.CanShift;
		}

		private void _btnShiftClips_Click(object sender, EventArgs e)
		{
			ActiveControl = null; // Prevent drawing focus rectangle
			_audioButtonsControl.StopPlaying();
			var clipRestored = false;
			if (_rdoReRecord.Checked)
			{
				// We suppress side effects because we don't want to update the UI until we see
				// whether the user decides to go through with this or not.
				clipRestored = _project.RestoreClipForSelectedBlock(suppressSideEffects:true);
				if (!clipRestored)
					return;
			}
			using (var dlg = new ShiftClipsDlg(_shiftClipsViewModel))
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					if (_project.IsExtraBlockSelected)
					{
						_project.HandleExtraBlocksShifted();
					}
					else
					{
						ProblemIgnoreStateChanged?.Invoke(this, EventArgs.Empty);
						UpdateState();
					}
				}
				else if (clipRestored)
				{
					_project.RestoreClipForSelectedBlock();
				}
			}
		}

		private void PaintRoundedBorder(object sender, PaintEventArgs e)
		{
			var ctrl = (Control)sender;
			DrawRoundedRectangle(ctrl, e, ctrl.ForeColor);
		}

		private void _pnlPlayClip_Paint(object sender, PaintEventArgs e)
		{
			DrawRoundedRectangle(_pnlPlayClip, e, _btnPlayClip.ForeColor);
		}

		private void DrawRoundedRectangle(Control ctrl, PaintEventArgs e, Color color)
		{
			var rect = ctrl.ClientRectangle;
			rect.Width--;
			rect.Height--;
			e.Graphics.DrawRoundedRectangle(color, rect, 8);
		}

		private void _rdoAskLater_CheckedChanged(object sender, EventArgs e)
		{
			if (_rdoAskLater.Checked && !_updatingDisplay)
				RevertToProblemState();
		}

		private void _rdoUseExisting_CheckedChanged(object sender, EventArgs e)
		{
			if (_rdoUseExisting.Checked && !_updatingDisplay)
				UseExistingClip();
		}

		private void _rdoReRecord_CheckedChanged(object sender, EventArgs e)
		{
			if (_rdoReRecord.Checked && !_updatingDisplay)
				DeleteClip();
		}

		private void _btnPlayClip_Click(object sender, EventArgs e)
		{
			_audioButtonsControl.OnPlay(this, null);
			RemovePlayButtonHighlight();
		}

		private void PlayClip_MouseEnter(object sender, EventArgs e)
		{
			_audioButtonsControl.SimulateMouseOverPlayButton();
			HighlightPlayClipButtonIfNotPlaying();
			_audioButtonsControl.PlayButtonStateChanged += AudioButtonsControlOnPlayButtonStateChanged;
		}

		private void HighlightPlayClipButtonIfNotPlaying()
		{
			if (!_audioButtonsControl.IsPlaying)
			{
				_btnPlayClip.ForeColor = AppPalette.HilightColor;
				_pnlPlayClip.Invalidate();
			}
		}

		private void AudioButtonsControlOnPlayButtonStateChanged(object sender, BtnState newState) =>
			HighlightPlayClipButtonIfNotPlaying();

		private void PlayClip_MouseLeave(object sender, EventArgs e)
		{
			_audioButtonsControl.PlayButtonStateChanged -= AudioButtonsControlOnPlayButtonStateChanged;

			_audioButtonsControl.SimulateMouseOverPlayButton(false);
			RemovePlayButtonHighlight();
		}

		private void RemovePlayButtonHighlight()
		{
			_btnPlayClip.ForeColor = Color.DarkGray;
			_pnlPlayClip.Invalidate();
		}

		private void EditClip_MouseEnter(object sender, EventArgs e)
		{
			_btnEditClip.ForeColor = AppPalette.HilightColor;
			_editSoundFile.Image = AppPalette.CurrentColorScheme == ColorScheme.HighContrast ?
				_editSoundFile.HighContrastMouseOverImage : _editSoundFile.MouseOverImage;
		}

		private void EditClip_MouseLeave(object sender, EventArgs e)
		{
			// Get the current mouse position relative to the form
			var mousePosition = PointToClient(MousePosition);

			// Check if the mouse is over either control
			bool isMouseOverControls = _btnEditClip.Bounds.Contains(mousePosition) ||
				_editSoundFile.Bounds.Contains(mousePosition);

			// If the mouse is not over either control, return to default state
			if (!isMouseOverControls)
			{
				_btnEditClip.ForeColor = Color.DarkGray;
				_editSoundFile.Image = (Image)_editSoundFile.Tag;
			}
		}

		private void _btnShiftClips_MouseEnter(object sender, EventArgs e) =>
			_btnShiftClips.ForeColor = AppPalette.HilightColor;

		private void _btnShiftClips_MouseLeave(object sender, EventArgs e) =>
			_btnShiftClips.ForeColor = Color.DarkGray;
			
		private void _editSoundFile_Click(object sender, EventArgs e)
		{
			if (!ExtClipEditorInfo.IsSpecified && !_haveDisplayedExternalEditorSelection)
			{
				using (var dlg = new AdministrativeSettings(_project, GetUIString, ExtClipEditorInfo))
				{
					dlg.SingleTabToShow = "ClipEditor";
					if (dlg.ShowDialog(this) == DialogResult.Cancel)
					{
						return;
					}

					_haveDisplayedExternalEditorSelection = true;
				}
			}

			var command = ExtClipEditorInfo.GetCommandToOpen(_audioButtonsControl.Path, out var arguments);
			try
			{
				Process.Start(command, arguments);
			}
			catch (Exception ex)
			{
				Logger.WriteError($"Problem starting {command} with {arguments}", ex);
				ErrorReport.ReportNonFatalException(ex);
				return;
			}

			_lblEditingCompleteInstructions.Text = Format(_fmtEditingInstructions, _audioButtonsControl.Path);
			_lblEditingCompleteInstructions.Visible = _copyPathToClipboard.Visible = true;
		}

		private void _copyPathToClipboard_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(_audioButtonsControl.Path);
		}
	}
}
