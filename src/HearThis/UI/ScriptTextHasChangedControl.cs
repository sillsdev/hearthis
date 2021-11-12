// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2020' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
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
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using SIL.IO;
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
		private enum CleanupAction
		{
			None,
			UpdateInfo,
			DeleteExtraRecording,
			DeleteRecordingForSkippedLine,
		}
		private Project _project;
		private static float s_zoomFactor;
		private string _standardProblemText;
		private string _standardDeleteExplanationText;
		private string _fmtRecordedDate;
		private int _indexIntoExtraRecordings;
		private ScriptLine CurrentScriptLine { get; set; }
		private IReadOnlyList<ExtraRecordingInfo> ExtraRecordings { get; set; }
		private ChapterInfo CurrentChapterInfo { get; set; }
		private CleanupAction CurrentCleanupAction { get; set; }
		public event EventHandler ProblemIgnoreStateChanged;
		public event EventHandler NextClick;

		public ScriptTextHasChangedControl()
		{
			InitializeComponent();
			_txtThen.BackColor = AppPallette.Background;
			_txtNow.BackColor = AppPallette.Background;
			_txtThen.ForeColor = AppPallette.TitleColor;
			_txtNow.ForeColor = AppPallette.TitleColor;
			Program.RegisterStringsLocalized(HandleStringsLocalized);
			HandleStringsLocalized();
		}
		
		private void HandleStringsLocalized()
		{
			_standardProblemText = _lblProblemSummary.Text;
			_standardDeleteExplanationText = _lblDelete.Text;
			_fmtRecordedDate = _lblRecordedDate.Text;
			UpdateDisplay();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if (FindForm() is Shell shell)
			{
				SetProject(shell.Project);
				shell.ProjectChanged += delegate(object sender, EventArgs args)
				{
					SetProject(((Shell) sender).Project);
				};
			}
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (Visible)
				UpdateState();
		}
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateRelativeThenAndNowRowSizes();
		}

		private void OnNextButton(object sender, EventArgs e)
		{
			NextClick?.Invoke(this, e);
		}

		public void SetData(ScriptLine block, IReadOnlyList<ExtraRecordingInfo> extraClips)
		{
			CurrentScriptLine = block;
			ExtraRecordings = extraClips;
			UpdateState();
		}

		private void SetProject(Project project)
		{
			_project = project;
			UpdateState();
		}

		public void UpdateState()
		{
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
				_audioButtonsControl.Path = Visible? _project?.GetPathToRecordingForSelectedLine() ??
					ExtraRecordings[_indexIntoExtraRecordings].ClipFile : null;
				if (Visible && _project != null)
				{
					_audioButtonsControl.ContextForAnalytics = new Dictionary<string, string>
					{
						{"book", _project.SelectedBook.Name},
						{"chapter", _project.SelectedChapterInfo.ChapterNumber1Based.ToString()},
						{"scriptBlock", _project.SelectedScriptBlock.ToString()},
						{"wordsInLine", CurrentScriptLine?.ApproximateWordCount.ToString()}
					};
				}
			}
			if (InvokeRequired)
				Invoke(new Action(() => _audioButtonsControl.UpdateDisplay()));
			else
				_audioButtonsControl.UpdateDisplay();
		}

		private bool HaveScript => CurrentScriptLine?.Text?.Length > 0;

		private ScriptLine CurrentRecordingInfo => CurrentChapterInfo.Recordings.FirstOrDefault(r => r.Number == CurrentScriptLine.Number);

		private DateTime ActualFileRecordingTime => new FileInfo(ClipRepository.GetPathToLineRecording(_project.Name, _project.SelectedBook.Name,
			_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock, _project.ScriptProvider)).CreationTime;

		private string ActualFileRecordingDateForUI => ActualFileRecordingTime.ToLocalTime().ToShortDateString();

		private void UpdateDisplay()
		{
			if (_project == null)
			{
				Hide();
				return; // Not ready yet
			}
			if (!HaveScript)
			{
				_indexIntoExtraRecordings = _project.SelectedScriptBlock - _project.GetLineCountForChapter(true);
				if (ExtraRecordings.Count > _indexIntoExtraRecordings)
					UpdateDisplayForExtraRecording();
				else
					Hide(); // Not ready yet
				return;
			}

			// TODO: Some of these scenarios are Waiting for design decisions. See https://docs.google.com/document/d/1JpBvo5hkHSNAZAMno_YdW6AWZGufAMAgDgWkfc4Xj2c/edit#bookmark=id.4wuun7f6ogjw

			CurrentChapterInfo = _project.SelectedChapterInfo;
			var currentRecordingInfo = CurrentRecordingInfo;
			if (currentRecordingInfo != null && currentRecordingInfo.Number - 1 != _project.SelectedScriptBlock)
				return; // Initializing during restart to change color scheme... not ready yet

			_chkIgnoreProblem.CheckedChanged -= _chkIgnoreProblem_CheckedChanged;

			SuspendLayout();
			Show();

			_problemIcon.ResetIcon();
			var haveRecording = ClipRepository.GetHaveClipUnfiltered(_project.Name, _project.SelectedBook.Name,
				_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock);
			_audioButtonsControl.Visible = _chkIgnoreProblem.Enabled = _lblIgnoreProblem.Enabled =
				_flowLayoutPanelThen.Visible = _txtThen.Visible = _btnDelete.Visible =
				_lblDelete.Visible = haveRecording;
			_lblDelete.Text = _standardDeleteExplanationText;
			_chkIgnoreProblem.Visible = _lblIgnoreProblem.Visible = _lblNow.Visible = _txtNow.Visible = true;
			_txtThen.Enabled = true;
			_chkIgnoreProblem.Checked = _nextButton.Visible = false;
			CurrentCleanupAction = CleanupAction.None;
			_txtNow.Font = _txtThen.Font = new Font(CurrentScriptLine.FontName, CurrentScriptLine.FontSize * ZoomFactor);
			_txtNow.Text = CurrentScriptLine.Text;
			if (CurrentScriptLine.Skipped)
			{
				if (haveRecording)
				{
					SetThenInfo(currentRecordingInfo);

					_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.BlockSkippedButHasRecording",
						"This block has been skipped, but it has a recording.");
					CurrentCleanupAction = CleanupAction.DeleteRecordingForSkippedLine;
					SetDisplayForDeleteCleanupAction();
				}
				else
				{
					_txtThen.Visible = _flowLayoutPanelThen.Visible = _chkIgnoreProblem.Enabled =
						_chkIgnoreProblem.Visible = _lblIgnoreProblem.Visible = _lblNow.Visible = false;
					_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.BlockSkipped", "This block has been skipped.");
					_problemIcon.Visible = false;
				}
			}
			else if (currentRecordingInfo?.TextAsOriginallyRecorded == null)
			{
				if (haveRecording)
				{
					if (CurrentCleanupAction == CleanupAction.None)
					{
						CurrentCleanupAction = CleanupAction.UpdateInfo;
						// We have a recording, but we don't know anything about the script at the time it was recorded.
						ShowNextButtonIfThereAreMoreProblemsInChapter();
						_problemIcon.Text = "?";
						_lblProblemSummary.Text = Format(LocalizationManager.GetString("ScriptTextHasChangedControl.ScriptTextAtTimeOfRecordingUnknown",
							"The clip for this block was recorded on {0}, which was before {1} started saving the version of the script text " +
							"at the time of recording.",
							"Param 0: recording date; Param 1: \"HearThis\""), ActualFileRecordingDateForUI, ProductName);
						_flowLayoutPanelThen.Visible = false;
						_txtThen.Visible = false;
					}
					else
						_lblRecordedDate.Text = Format(_fmtRecordedDate, ActualFileRecordingDateForUI);
				}
				else
				{
					ShowNextButtonIfThereAreMoreProblemsInChapter();
					_lblNow.Visible = _chkIgnoreProblem.Visible = _lblIgnoreProblem.Visible =
						_problemIcon.Visible = false;
					_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.NotRecorded",
						"This block has not yet been recorded.");
				}
			}
			else
			{
				SetThenInfo(currentRecordingInfo);

				Debug.Assert(haveRecording); // If not, we need to remove the info about the recording!

				if (_txtNow.Text != currentRecordingInfo.Text)
				{
					CurrentCleanupAction = CleanupAction.UpdateInfo;
					_lblProblemSummary.Text = _standardProblemText;
				}
				else
				{
					if (_txtNow.Text != currentRecordingInfo.OriginalText && currentRecordingInfo.OriginalText != null)
					{
						// REVIEW: _txtThen.Enabled = false;
						_chkIgnoreProblem.Checked = true;
						_lblProblemSummary.Text = _standardProblemText;
					}
					else
					{
						ShowNextButtonIfThereAreMoreProblemsInChapter();
						_lblNow.Visible = _txtNow.Visible = _flowLayoutPanelThen.Visible =
							_chkIgnoreProblem.Enabled = _chkIgnoreProblem.Visible =
							_lblIgnoreProblem.Visible = _problemIcon.Visible = false;
						_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.NoProblem", "No problems");
					}
				}
			}

			ResumeLayout(true);

			UpdateRelativeThenAndNowRowSizes();

			if (_chkIgnoreProblem.Enabled)
				_chkIgnoreProblem.Focus();
			else if (_audioButtonsControl.Enabled)
				_audioButtonsControl.Focus();

			_audioButtonsControl.UpdateDisplay();
			_chkIgnoreProblem.CheckedChanged += _chkIgnoreProblem_CheckedChanged;
		}

		private void SetThenInfo(ScriptLine recordingInfo)
		{
			if (recordingInfo?.TextAsOriginallyRecorded == null)
				_txtThen.Visible = _flowLayoutPanelThen.Visible = false;
			else
			{
				_txtThen.Text = recordingInfo.TextAsOriginallyRecorded;
				_lblRecordedDate.Text = Format(_fmtRecordedDate, recordingInfo.RecordingTime.ToLocalTime().ToShortDateString());
			}
		}

		private void ShowNextButtonIfThereAreMoreProblemsInChapter()
		{
			if (_project.SelectedChapterInfo.GetIndexOfNextUnfilteredBlockWithProblem(
				_project.SelectedScriptBlock) > _project.SelectedScriptBlock)
			{
				_nextButton.Visible = true;
			}
		}

		private void SetDisplayForDeleteCleanupAction()
		{
			_lblDelete.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.DeleteExtraClipExplanation",
				"Delete recording.");
			_chkIgnoreProblem.Visible = _lblIgnoreProblem.Visible = false;
		}

		private void UpdateDisplayForExtraRecording()
		{
			_problemIcon.ResetIcon();

			Show();

			var extraRecording = ExtraRecordings[_indexIntoExtraRecordings];

			if (Exists(extraRecording.ClipFile))
			{
				_audioButtonsControl.Visible = _btnDelete.Visible = true;
				CurrentCleanupAction = CleanupAction.DeleteExtraRecording;
				SetDisplayForDeleteCleanupAction();
				_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.ExtraRecording", 
					"This is an extra recording that does not correspond to any block in the current script.");
			}
			else
			{
				_problemIcon.Visible = _audioButtonsControl.Visible = _btnDelete.Visible =
					_lblDelete.Visible = _chkIgnoreProblem.Visible = _lblIgnoreProblem.Visible = false;
				CurrentCleanupAction = CleanupAction.None;
				_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.DeletedExtraRecording",
					"This problem has been resolved (extra file deleted).");
			}

			SetThenInfo(extraRecording.RecordingInfo);

			_lblNow.Visible = _txtNow.Visible = false;
		}

		private void UpdateRelativeThenAndNowRowSizes()
		{
			if (_txtThen.Text.Length == 0 && _txtNow.Text.Length == 0)
				return;

			var thenRowIndex = tableLayoutPanel1.GetRow(_txtThen);
			var nowRowIndex = tableLayoutPanel1.GetRow(_txtNow);
			var thenRow = tableLayoutPanel1.RowStyles[thenRowIndex];
			var nowRow = tableLayoutPanel1.RowStyles[nowRowIndex];
			float availableHeight = tableLayoutPanel1.Height;
			var rowHeights = tableLayoutPanel1.GetRowHeights();
			for (int row = 0; row < rowHeights.Length; row++)
			{
				if (row != thenRowIndex && row != nowRowIndex)
					availableHeight -= rowHeights[row];
			}
			thenRow.SizeType = _txtThen.Visible ? SizeType.Percent : SizeType.AutoSize;
			nowRow.SizeType = _txtNow.Visible ? SizeType.Percent : SizeType.AutoSize;
			if (thenRow.SizeType == SizeType.Percent && nowRow.SizeType == SizeType.Percent)
			{
				using (var g = CreateGraphics())
				{
					float GetLayoutHeight(TextBox t) => TextRenderer.MeasureText(g, t.Text, t.Font, new Size(t.Width, MaxValue), TextFormatFlags.WordBreak).Height;
					var singleLineHeight = TextRenderer.MeasureText(g, "A", _txtThen.Font, new Size(MaxValue, MaxValue)).Height;
					var thenHeight = GetLayoutHeight(_txtThen);
					var nowHeight = GetLayoutHeight(_txtNow);
					var total = thenHeight + nowHeight + _txtThen.Margin.Vertical + _txtNow.Margin.Vertical;
					if (total < availableHeight || thenHeight <= singleLineHeight)
					{
						_txtThen.Height = (int)Math.Round(thenHeight, MidpointRounding.AwayFromZero);
						thenRow.SizeType = SizeType.AutoSize;
					}
					else if (nowHeight <= singleLineHeight)
					{
						_txtNow.Height = (int)Math.Round(nowHeight, MidpointRounding.AwayFromZero);
						nowRow.SizeType = SizeType.AutoSize;
					}
					else
					{
						thenRow.Height = thenHeight;
						nowRow.Height = nowHeight;
					}
				}
			}
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

			switch ((Keys) m.WParam)
			{
				case Keys.OemPeriod:
				case Keys.Decimal:
					RecordingToolControl.ShowPlayShortcutMessage();
					break;

				case Keys.Tab:
					_audioButtonsControl.OnPlay(this, null);
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
				UpdateDisplay();
			}
		}

		private void _btnDelete_Click(object sender, EventArgs e)
		{
			if (CurrentCleanupAction == CleanupAction.DeleteExtraRecording)
				RobustFile.Delete(ExtraRecordings[_indexIntoExtraRecordings].ClipFile);
			else
				_project.DeleteClipForSelectedBlock();

			ProblemIgnoreStateChanged?.Invoke(this, new EventArgs());
			if (InvokeRequired)
				Invoke(new Action(UpdateDisplay));
			else
				UpdateDisplay();
		}

		private void _chkIgnoreProblem_CheckedChanged(object sender, EventArgs e)
		{
			if (_chkIgnoreProblem.Checked)
			{
				if (CurrentCleanupAction == CleanupAction.UpdateInfo)
				{
					var scriptLine = CurrentRecordingInfo;
					if (scriptLine == null)
					{
						scriptLine = CurrentScriptLine;
						scriptLine.RecordingTime = ActualFileRecordingTime;
					}
					else
					{
						scriptLine.OriginalText = scriptLine.Text;
						scriptLine.Text = CurrentScriptLine.Text;
					}

					CurrentChapterInfo.OnScriptBlockRecorded(scriptLine);
				}
				else
				{
					throw new InvalidOperationException("_chkIgnoreProblem should not have been enabled!");
				}
			}
			else
			{
				// Un-ignore.
				var scriptLine = CurrentRecordingInfo;
				scriptLine.Text = scriptLine.OriginalText;
				scriptLine.OriginalText = null;
				CurrentChapterInfo.OnScriptBlockRecorded(scriptLine);
			}

			ProblemIgnoreStateChanged?.Invoke(this, new EventArgs());
			if (InvokeRequired)
				Invoke(new Action(UpdateDisplay));
			else
				UpdateDisplay();
		}
	}
}
