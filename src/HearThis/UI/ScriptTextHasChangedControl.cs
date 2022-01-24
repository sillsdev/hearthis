// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2020' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using HearThis.StringDifferences;
using L10NSharp;
using SIL.Reporting;
using SIL.Windows.Forms.Widgets;
using static System.Int32;
using static System.IO.File;
using static System.String;
using DateTime = System.DateTime;
using FileInfo = System.IO.FileInfo;

namespace HearThis.UI
{
	/// <summary>
	/// This class holds the text that the user is supposed to be reading (the main pane on the bottom left of the UI).
	/// </summary>
	public partial class ScriptTextHasChangedControl : UserControl, IMessageFilter
	{
		private Project _project;
		private bool _inUpdateDisplay;
		private bool _inUpdateState;
		private static float s_zoomFactor;
		private string _standardProblemText;
		private string _standardDeleteExplanationText;
		private string _fmtRecordedDate;
		private int _indexIntoExtraRecordings;
		private ScriptLine _lastNullScriptLineIgnored;
		private ScriptLine CurrentScriptLine { get; set; }
		private IReadOnlyList<ExtraRecordingInfo> ExtraRecordings { get; set; }
		private ChapterInfo CurrentChapterInfo { get; set; }
		public event EventHandler ProblemIgnoreStateChanged;
		public event EventHandler NextClick;
		private ShiftClipsViewModel _shiftClipsViewModel;

		public ScriptTextHasChangedControl()
		{
			DoubleBuffered = true;
			InitializeComponent();
			_txtThen.BackColor = AppPalette.Background;
			_txtNow.BackColor = AppPalette.Background;
			_txtThen.ForeColor = AppPalette.TitleColor;
			_txtNow.ForeColor = AppPalette.TitleColor;
			_lblProblemSummary.ForeColor = AppPalette.HilightColor;
			_lblProblemSummary.FlatAppearance.MouseDownBackColor =
				_lblProblemSummary.FlatAppearance.MouseOverBackColor =
				_btnAskLater.FlatAppearance.MouseOverBackColor =
				_btnUseExisting.FlatAppearance.MouseOverBackColor =
				_btnDelete.FlatAppearance.MouseOverBackColor =
				_btnShiftClips.FlatAppearance.MouseOverBackColor =
				_btnShiftClips.FlatAppearance.MouseOverBackColor =
				_btnPlayClip.FlatAppearance.MouseOverBackColor =
				_btnPlayClip.FlatAppearance.MouseOverBackColor =
					AppPalette.Background;
			_nextButton.BorderColor = AppPalette.HilightColor;
			Program.RegisterStringsLocalized(HandleStringsLocalized);

			_btnAskLater.Tag = new Tuple<RadioButton, Image, Image>(_rdoAskLater, _btnAskLater.Image, Resources.Later_MouseOver);
			_btnUseExisting.Tag = new Tuple<RadioButton, Image, Image>(_rdoUseExisting, _btnUseExisting.Image, Resources.OK_MouseOver);
			_btnDelete.Tag = new Tuple<RadioButton, Image, Image>(_rdoReRecord, _btnDelete.Image, Resources.Delete_MouseOver);

			HandleStringsLocalized();
		}

		private void HandleStringsLocalized()
		{
			_standardProblemText = _lblProblemSummary.Text;
			_standardDeleteExplanationText = _btnDelete.Text;
			_fmtRecordedDate = _lblRecordedDate.Text;
			UpdateDisplay();
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (Visible && !_inUpdateDisplay)
				UpdateState();
		}

		private void OnNextButton(object sender, EventArgs e)
		{
			NextClick?.Invoke(this, e);
		}

		public void SetData(Project project, IReadOnlyList<ExtraRecordingInfo> extraClips)
		{
			_project = project;
			CurrentScriptLine = _project.ScriptOfSelectedBlock;
			SetScriptFonts();
			ExtraRecordings = extraClips;
			CurrentChapterInfo = _project.SelectedChapterInfo;
			_indexIntoExtraRecordings = _project.SelectedScriptBlock - _project.LineCountForChapter;
			UpdateState();
		}

		private void SetScriptFonts()
		{
			if (_project == null)
				return;

			_txtNow.Font = _txtThen.Font = new Font(_project.FontNameForSelectedBlock,
				_project.FontSizeForSelectedBlock * ZoomFactor);
		}

		public void UpdateState()
		{
			_lastNullScriptLineIgnored = null;

			if (InvokeRequired)
				Invoke(new Action(UpdateDisplay));
			else
				UpdateDisplay();

			if (_rdoAskLater.Visible && !_rdoAskLater.Checked)
			{
				_inUpdateState = true;
				_rdoAskLater.Checked = true;
			}

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
					_audioButtonsControl.Path = _indexIntoExtraRecordings >= 0 && _indexIntoExtraRecordings < ExtraRecordings.Count ?
						ExtraRecordings[_indexIntoExtraRecordings].ClipFile :
						_project.GetPathToRecordingForSelectedLine();
					_audioButtonsControl.ContextForAnalytics = new Dictionary<string, string>
					{
						{"book", _project.SelectedBook.Name},
						{"chapter", CurrentChapterInfo.ChapterNumber1Based.ToString()},
						{"scriptBlock", _project.SelectedScriptBlock.ToString()},
						{"wordsInLine", CurrentScriptLine?.ApproximateWordCount.ToString()},
						{"_indexIntoExtraRecordings", _indexIntoExtraRecordings.ToString()}
					};
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

		private ScriptLine CurrentRecordingInfo => CurrentScriptLine == null ? null :
			CurrentChapterInfo?.Recordings.FirstOrDefault(r => r.Number == CurrentScriptLine.Number);

		private DateTime ActualFileRecordingTime => new FileInfo(ClipRepository.GetPathToLineRecording(_project.Name, _project.SelectedBook.Name,
			CurrentChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock, _project.ScriptProvider)).CreationTime;

		private string ActualFileRecordingDateForUI => ActualFileRecordingTime.ToLocalTime().ToShortDateString();

		private void UpdateDisplay()
		{
			if (_project == null)
			{
				Hide();
				return; // Not ready yet
			}

			var currentRecordingInfo = CurrentRecordingInfo;

			if (!HaveScript)
			{
				if (ExtraRecordings.Count > _indexIntoExtraRecordings)
				{
					Debug.Assert(_indexIntoExtraRecordings >= 0, "Figure out when we can not have a scrip but be on a real block!");
					UpdateDisplayForExtraRecording();
					DeterminePossibleClipShifts();
					ShowNextButtonIfThereAreMoreProblemsInChapter();
				}
				else
					Hide(); // Not ready yet
				return;
			}

			if (currentRecordingInfo != null && currentRecordingInfo.Number - 1 != _project.SelectedScriptBlock)
				return; // Initializing during restart to change color scheme... not ready yet

			_inUpdateDisplay = true;
			SuspendLayout();
			_masterTableLayoutPanel.SuspendLayout();

			ResetProblemIcon();
			var haveRecording = GetHasRecordedClip(_project.SelectedScriptBlock);
			_audioButtonsControl.Visible = _btnUseExisting.Visible =
				_rdoUseExisting.Visible = _panelThen.Visible = _txtThen.Visible =
					_btnDelete.Visible = _rdoReRecord.Visible = haveRecording;
			_btnDelete.Text = _standardDeleteExplanationText;
			_tableOptions.Visible = _lblNow.Visible = _txtNow.Visible = true;
			ShowNextButtonIfThereAreMoreProblemsInChapter();

			_txtNow.Text = CurrentScriptLine.Text;

			DeterminePossibleClipShifts();

			if (CurrentScriptLine.Skipped)
			{
				if (haveRecording)
				{
					SetThenInfo(currentRecordingInfo);

					_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.BlockSkippedButHasRecording",
						"This block has been skipped, but it has a recording.");
					SetDisplayForDeleteCleanupAction();
				}
				else
				{
					_tableOptions.Visible = _lblNow.Visible = false;
					_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.BlockSkipped", "This block has been skipped.");
					_lblProblemSummary.Image = null;
				}
			}
			else if (currentRecordingInfo?.TextAsOriginallyRecorded == null || !haveRecording)
			{
				if (haveRecording)
				{
					// We have a recording, but we don't know anything about the script at the time it was recorded.
					_lblProblemSummary.Image = Resources.ScriptUnknownHC;
					_lblProblemSummary.Text = Format(LocalizationManager.GetString("ScriptTextHasChangedControl.ScriptTextAtTimeOfRecordingUnknown",
						"The clip for this block was recorded using an older version of {0} that did not save the version of the text at the time of recording.",
						"Param 0: \"HearThis\" (product name)"), ProductName);
					_lblRecordedDate.Text = Format(_fmtRecordedDate, ActualFileRecordingDateForUI);
				}
				else
				{
					_lblNow.Visible = false;
					_lblProblemSummary.Image = null;
					if (ClipRepository.GetHaveBackupFile(_project.Name, _project.SelectedBook.Name,
						CurrentChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock))
					{
						_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.ReadyForRerecording",
							"This block is ready to be re-recorded.");
					}
					else
					{
						_rdoAskLater.Visible = _btnAskLater.Visible = false;
						_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.NotRecorded",
							"This block has not yet been recorded.");
					}
				}
			}
			else
			{
				SetThenInfo(currentRecordingInfo);

				if (_txtNow.Text != currentRecordingInfo.Text)
				{
					_lblProblemSummary.Text = _standardProblemText;
				}
				else
				{
					if (_txtNow.Text != currentRecordingInfo.OriginalText && currentRecordingInfo.OriginalText != null)
					{
						_lblProblemSummary.Text = _standardProblemText;
					}
					else
					{
						_lblNow.Visible = _panelThen.Visible = false;

						if (_lastNullScriptLineIgnored == CurrentScriptLine)
						{
							_txtThen.Visible = _lblRecordedDate.Visible = false;
							_lblProblemSummary.Image = null;
							_lblProblemSummary.Text = Format(LocalizationManager.GetString("ScriptTextHasChangedControl.InfoUpdated",
								"Recording information updated for clip recorded on {0}",
								"Param is recording date"), ActualFileRecordingDateForUI);
						}
						else
						{
							_txtNow.Visible = _tableOptions.Visible = false;
							_lblProblemSummary.Image = null;
							_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.NoProblem", "No problems");
						}
					}
				}
			}

			ResumeLayout(true);
			Show();

			UpdateThenVsNowTableLayout();
			_masterTableLayoutPanel.ResumeLayout();

			// REVIEW: Focus a specific control?
			if (_tableOptions.Visible)
				_tableOptions.Focus();
			else if (_audioButtonsControl.Enabled)
				_audioButtonsControl.Focus();

			_audioButtonsControl.UpdateDisplay();
			_inUpdateDisplay = false;
		}

		private void ResetProblemIcon()
		{
			_lblProblemSummary.Image = Settings.Default.UserColorScheme == ColorScheme.Normal ?
				Resources.AlertCircle : Resources.AlertCircleHC;
		}

		private void SetThenInfo(ScriptLine recordingInfo)
		{
			if (recordingInfo?.TextAsOriginallyRecorded == null)
				_txtThen.Visible = _panelThen.Visible = false;
			else
			{
				// If the two texts are the same, we will be hiding the "now" text in the calling code.
				if (_txtNow.Text.Length > 0 && _txtNow.Text != recordingInfo.TextAsOriginallyRecorded)
				{
					var diffs = new StringDifferenceFinder(recordingInfo.TextAsOriginallyRecorded, _txtNow.Text);
					SetRichText(_txtThen, diffs.OriginalStringDifferences);
					SetRichText(_txtNow, diffs.NewStringDifferences);
				}
				else
					_txtThen.Text = recordingInfo.TextAsOriginallyRecorded;
				_lblRecordedDate.Text = Format(_fmtRecordedDate, recordingInfo.RecordingTime.ToLocalTime().ToShortDateString());
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
					return Color.FromArgb(144, 238, 144);
				case DifferenceType.Deletion:
					return Color.FromArgb(255, 192, 203);
				default:
					throw new ArgumentOutOfRangeException(nameof(segType), segType, null);
			}
		}

		private void ShowNextButtonIfThereAreMoreProblemsInChapter()
		{
			_nextButton.Visible = CurrentChapterInfo.GetIndexOfNextUnfilteredBlockWithProblem(
				_project.SelectedScriptBlock) > _project.SelectedScriptBlock;
			_nextButton.Invalidate();
		}

		private void SetDisplayForDeleteCleanupAction()
		{
			_rdoUseExisting.Visible = _btnUseExisting.Visible = false;
			_btnDelete.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.DeleteExtraClipExplanation",
				"Delete recording.");
			_rdoReRecord.Visible = _btnDelete.Visible = true;
		}

		private void UpdateDisplayForExtraRecording()
		{
			SetDisplayForDeleteCleanupAction();

			Show();

			var extraRecording = ExtraRecordings[_indexIntoExtraRecordings];

			if (Exists(extraRecording.ClipFile))
			{
				_audioButtonsControl.Visible = _btnPlayClip.Visible = true;
				ResetProblemIcon();
				_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.ExtraRecording",
					"This is an extra recording that does not correspond to any block in the current script.");
			}
			else
			{
				_audioButtonsControl.Visible = _btnPlayClip.Visible = false;
				_lblProblemSummary.Image = null;
				_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.DeletedExtraRecording",
					"This problem has been resolved (extra file deleted).");
			}

			SetThenInfo(extraRecording.RecordingInfo);

			_lblNow.Visible = _txtNow.Visible = false;

			UpdateThenVsNowTableLayout();
		}

		private void UpdateThenVsNowTableLayout()
		{
			if (!_txtThen.Visible)
				_txtThen.Text = "";
			if (!_txtNow.Visible)
				_txtNow.Text = "";

			// TODO: temporary
			return;

			if (_txtThen.Text.Length == 0)
			{
				if (_txtNow.Text.Length == 0)
					return;
				_masterTableLayoutPanel.ColumnStyles[0].Width = 0;
			}
			else
			{
				_masterTableLayoutPanel.ColumnStyles[0].Width = 50;
			}

			_masterTableLayoutPanel.ColumnStyles[1].Width = _txtNow.Text.Length == 0 ?
				0 : 50;

			var thenVsNowRowIndex = _masterTableLayoutPanel.GetRow(_txtThen);

			float availableHeight = _masterTableLayoutPanel.Height - _masterTableLayoutPanel.Padding.Vertical;
			var rowHeights = _masterTableLayoutPanel.GetRowHeights();
			for (int row = 0; row < rowHeights.Length; row++)
			{
				if (row < thenVsNowRowIndex || row > thenVsNowRowIndex + 1)
					availableHeight -= rowHeights[row];
			}

			var thenVsNowHeaderHeight = _masterTableLayoutPanel.GetRowHeights()[thenVsNowRowIndex - 1];
			availableHeight -= thenVsNowHeaderHeight;
			availableHeight -= _masterTableLayoutPanel.Margin.Vertical;
			availableHeight -= _masterTableLayoutPanel.Padding.Vertical;

			using (var g = CreateGraphics())
			{
				float GetLayoutHeight(RichTextBox t) => TextRenderer.MeasureText(g, t.Text, t.Font, new Size(t.Width, MaxValue), TextFormatFlags.WordBreak).Height;
				var minHeight = TextRenderer.MeasureText(g, "A", _txtThen.Font, new Size(MaxValue, MaxValue)).Height;
				var thenHeight = GetLayoutHeight(_txtThen);
				var nowHeight = GetLayoutHeight(_txtNow);
				if (thenHeight > minHeight || nowHeight > minHeight)
					minHeight *= 2; // Show a minimum of two lines if needed.
				minHeight += _panelThen.Height + _panelThen.Margin.Vertical;
				availableHeight = Math.Max(availableHeight, minHeight);
				var neededHeight = Math.Max(thenHeight + _txtThen.Margin.Vertical, nowHeight + _txtNow.Margin.Vertical);
				int txtBoxHeight;
				if (neededHeight > availableHeight)
				{
					_txtThen.ScrollBars = _txtNow.ScrollBars = RichTextBoxScrollBars.Vertical;
					_masterTableLayoutPanel.LayoutSettings.RowStyles[thenVsNowRowIndex].Height = availableHeight;
					txtBoxHeight = (int)Math.Ceiling(availableHeight);
				}
				else
				{
					// Turning the scroll bars off (and on, above) should not be necessary, but
					// there's a slight discrepancy between our estimate of the height needed and
					// the way the control lays out the text and determines if the scroll bar
					// should show. So we can end up with the scroll bar displayed when it really
					// only shifts the text up and down by two pixels (within the line leading)
					// when we can just barely fit the text.
					_txtThen.ScrollBars = _txtNow.ScrollBars = RichTextBoxScrollBars.None;
					txtBoxHeight = (int)Math.Ceiling(neededHeight);
				}
				_masterTableLayoutPanel.Height = thenVsNowHeaderHeight +
					Math.Max(txtBoxHeight, _audioButtonsControl.Height + _audioButtonsControl.Margin.Vertical) +
					_masterTableLayoutPanel.Padding.Vertical;
			}
		}

		/// <summary>
		/// This invokes the message filter that allows the control to interpret various keystrokes as button presses.
		/// </summary>
		public void StartFilteringMessages()
		{
			Application.AddMessageFilter(this);
		}

		public void StopFilteringMessages()
		{
			Application.RemoveMessageFilter(this);
		}

		/// <summary>
		/// Filter out all keystrokes except the few that we want to handle.
		/// We handle Space, TAB, PageUp, PageDown, Delete and Arrow keys.
		/// </summary>
		/// <remarks>This is invoked because we implement IMessageFilter and call Application.AddMessageFilter(this)</remarks>
		public bool PreFilterMessage(ref Message m)
		{
			const int WM_KEYDOWN = 0x100;

			if (m.Msg != WM_KEYDOWN)
				return false;

			switch ((Keys)m.WParam)
			{
				case Keys.OemPeriod:
				case Keys.Decimal:
					RecordingToolControl.ShowPlayShortcutMessage();
					break;

				case Keys.Tab:
					_audioButtonsControl.OnPlay(this, null);
					break;

				case Keys.Delete:
					if (_rdoReRecord.Visible && !_rdoReRecord.Checked)
						_rdoReRecord.Checked = true;
					break;

				default:
					return false;
			}

			return true;
		}

		public float ZoomFactor
		{
			get => s_zoomFactor;
			set
			{
				s_zoomFactor = value;
				SetScriptFonts();
				UpdateDisplay();
			}
		}

		private void DeleteClip()
		{
			try
			{
				_audioButtonsControl.ReleaseFile();
				_project.DeleteClipForSelectedBlock(ExtraRecordings);
				RefreshAfterClipDeletionOrUndo();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				ErrorReport.ReportNonFatalException(exception);
			}
		}

		private bool UndoDelete()
		{
			if (_project.UndeleteClipForSelectedBlock())
			{
				RefreshAfterClipDeletionOrUndo();
				return true;
			}

			return false;
		}

		private void RefreshAfterClipDeletionOrUndo()
		{
			ProblemIgnoreStateChanged?.Invoke(this, new EventArgs());
			if (InvokeRequired)
				Invoke(new Action(UpdateDisplay));
			else
				UpdateDisplay();
		}

		private void UseExistingClip()
		{
			// "Ignore" this problem.
			var scriptLine = CurrentRecordingInfo;
			if (scriptLine == null)
			{
				scriptLine = CurrentScriptLine;
				scriptLine.RecordingTime = ActualFileRecordingTime;
				_lastNullScriptLineIgnored = CurrentScriptLine;
			}
			else
			{
				scriptLine.OriginalText = scriptLine.Text;
				scriptLine.Text = CurrentScriptLine.Text;
			}

			CurrentChapterInfo.OnScriptBlockRecorded(scriptLine);

			OnIgnoreStateChanged();
		}

		private void RevertToProblemState()
		{
			if (UndoDelete())
				return;

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

			OnIgnoreStateChanged();
		}

		private void OnIgnoreStateChanged()
		{
			ProblemIgnoreStateChanged?.Invoke(this, new EventArgs());
			if (InvokeRequired)
				Invoke(new Action(UpdateDisplay));
			else
				UpdateDisplay();
		}

		private void BitmapButtonMouseEnter(object sender, EventArgs e)
		{
			if (sender is BitmapButton btn)
			{

			}
		}

		private void BitmapButtonMouseLeave(object sender, EventArgs e)
		{
			if (sender is BitmapButton btn)
			{
				btn.FlatAppearance.BorderSize = 0;
				btn.Invalidate();
			}
		}

		private bool GetHasRecordedClip(int i) => ClipRepository.GetHaveClipUnfiltered(_project.Name, _project.SelectedBook.Name,
			CurrentChapterInfo.ChapterNumber1Based, i);

		private void DeterminePossibleClipShifts()
		{
			if (!GetHasRecordedClip(_project.SelectedScriptBlock))
			{
				_shiftClipsViewModel = null;
				_btnShiftClips.Visible = _lblShiftClips.Visible = false;
				return;
			}

			_shiftClipsViewModel = new ShiftClipsViewModel(_project);
			_btnShiftClips.Visible = _lblShiftClips.Visible = _shiftClipsViewModel.CanShift;
		}

		private void _btnShiftClips_Click(object sender, EventArgs e)
		{
			using (var dlg = new ShiftClipsDlg(_shiftClipsViewModel))
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					RefreshAfterClipDeletionOrUndo();
					_audioButtonsControl.Invalidate();
				}
			}
		}

		private void _masterTableLayoutPanel_Resize(object sender, EventArgs e)
		{
			UpdateThenVsNowTableLayout();
			_nextButton.Invalidate();
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

		private void SelectRadioButton(object sender, EventArgs e)
		{
			var radioBtn = (RadioButton)((Button)sender).Tag;
			radioBtn.Checked = !radioBtn.Checked;
		}

		private void _rdoAskLater_CheckedChanged(object sender, EventArgs e)
		{
			if (_rdoAskLater.Checked && !_inUpdateState)
				RevertToProblemState();
		}

		private void _rdoUseExisting_CheckedChanged(object sender, EventArgs e)
		{
			if (_rdoUseExisting.Checked)
				UseExistingClip();
		}

		private void _rdoReRecord_CheckedChanged(object sender, EventArgs e)
		{
			if (_rdoReRecord.Checked)
				DeleteClip();
		}

		private void _btnPlayClip_Click(object sender, EventArgs e)
		{
			_audioButtonsControl.OnPlay(this, null);
		}
		private void HandleMouseEnterRadioButton(object sender, EventArgs e)
		{
			HandleMouseEnterButton(GetCorrespondingButton((RadioButton)sender), e);
		}
		private void HandleMouseLeaveRadioButton(object sender, EventArgs e)
		{
			HandleMouseLeaveButton(GetCorrespondingButton((RadioButton)sender), e);
		}

		private Button GetCorrespondingButton(RadioButton rdoBtn) =>
			rdoBtn.Parent.Controls.OfType<Button>().Single(b => b.Tag is Tuple<RadioButton, Image, Image> tuple && tuple.Item1 == rdoBtn);

		private void HandleMouseEnterButton(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.ForeColor = SystemColors.ActiveCaption;
			if (btn.Tag is Tuple<RadioButton, Image, Image> tuple)
				btn.Image = tuple.Item3;
			btn.Invalidate();
		}
		private void HandleMouseLeaveButton(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.ForeColor = Color.DarkGray;
			if (btn.Tag is Tuple<RadioButton, Image, Image> tuple)
				btn.Image = tuple.Item2;
			btn.Invalidate();
		}

		private void _nextButton_MouseEnter(object sender, EventArgs e)
		{
			_nextButton.Font = new Font(_nextButton.Font, FontStyle.Bold);
		}
		private void _nextButton_MouseLeave(object sender, EventArgs e)
		{
			_nextButton.Font = new Font(_nextButton.Font, FontStyle.Regular);
		}

		private void _btnPlayClip_MouseEnter(object sender, EventArgs e)
		{
			_audioButtonsControl.SimulateMouseOverPlayButton();
			HandleMouseEnterButton(sender, e);
			_pnlPlayClip.Invalidate();
		}
		private void _btnPlayClip_MouseLeave(object sender, EventArgs e)
		{
			_audioButtonsControl.SimulateMouseOverPlayButton(false);
			HandleMouseLeaveButton(sender, e);
			_pnlPlayClip.Invalidate();
		}

		/// <summary>
		/// This doesn't feel like it should be necessary, but for some odd reason, without this,
		/// the next button is not getting repainted when the mouse moves over some parts of the UI.
		/// Unfortunately, this constant repainting causes it to flicker annoyingly.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _masterTableLayoutPanel_Paint(object sender, PaintEventArgs e)
		{
			_nextButton.Invalidate();
		}
	}
}
