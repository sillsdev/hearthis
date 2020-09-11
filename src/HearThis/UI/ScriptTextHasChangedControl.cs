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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using SIL.Windows.Forms.Widgets.Flying;
using static System.String;

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
			DeleteExtraRecordings,
		}
		private Project _project;
		private Animator _animator;
		private PointF _animationPoint;
		//private Direction _direction;
		private static float _zoomFactor;
		private string _standardProblemText;
		private string _standardIgnoreText;
		private string _fmtRecordedDate;
		private ScriptLine CurrentScriptLine { get; set; }
		private ChapterInfo CurrentChapterInfo { get; set; }
		private CleanupAction CurrentCleanupAction { get; set; }
		private PaintData _outgoingData;
		public bool ShowSkippedBlocks { get; set; }
		public event EventHandler ProblemIgnoreStateChanged;

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
			_standardIgnoreText = _chkIgnoreProblem.Text;
			_fmtRecordedDate = _lblRecordedDate.Text;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if (FindForm() is Shell shell)
			{
				SetProject(shell.Project);
				shell.OnProjectChanged += delegate(object sender, EventArgs args)
				{
					SetProject(((Shell) sender).Project);
				};
			}
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			UpdateState();
		}

		public void SetData(ScriptLine block)
		{
			CurrentScriptLine = block;
			UpdateState();
		}

		private void SetProject(Project project)
		{
			_project = project;
			UpdateState();
		}

		private void UpdateState()
		{
			var worker = new BackgroundWorker();
			worker.DoWork += UpdateAudioButtonsControl;
			worker.RunWorkerAsync();
			UpdateDisplay();
		}

		private void UpdateAudioButtonsControl(object sender, DoWorkEventArgs e)
		{
			// We do this is a background worker to prevent deadlock on the control.
			// Nothing should keep the control locked for long, so it should always get
			// done very quickly. Since we don't lock _project, it is conceivable that
			// it could change in the middle of this operation, but it's practically
			// impossible. If it were to change, another background worker would get
			// kicked off right away. That one wouldn't be able to enter until we
			// released the lock on _audioButtonsControl, so everything would end up
			// in a consistent state.
			lock (_audioButtonsControl)
			{
				_audioButtonsControl.Path = Visible? _project?.GetPathToRecordingForSelectedLine() : null;
				if (Visible && _project != null)
				{
					_audioButtonsControl.ContextForAnalytics = new Dictionary<string, string>
					{
						{"book", _project.SelectedBook.Name},
						{"chapter", _project.SelectedChapterInfo.ChapterNumber1Based.ToString()},
						{"scriptBlock", _project.SelectedScriptBlock.ToString()},
						{"wordsInLine", CurrentScriptLine.ApproximateWordCount.ToString()}
					};
				}
			}
			if (InvokeRequired)
				Invoke(new Action(() => _audioButtonsControl.UpdateDisplay()));
			else
				_audioButtonsControl.UpdateDisplay();
		}

		private bool HaveScript => CurrentScriptLine != null && CurrentScriptLine.Text.Length > 0;

		private ScriptLine CurrentRecordingInfo => CurrentChapterInfo.Recordings.FirstOrDefault(r => r.Number == CurrentScriptLine.Number);

		private DateTime ActualFileRecordingTime => new FileInfo(ClipRepository.GetPathToLineRecording(_project.Name, _project.SelectedBook.Name,
			_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock, _project.ScriptProvider)).CreationTime;

		private string ActualFileRecordingDateForUI => ActualFileRecordingTime.ToLocalTime().ToShortDateString();

		public void UpdateDisplay()
		{
			if (_project == null || !HaveScript)
			{
				Hide();
				return; // Not ready yet
			}

			// TODO: Some of these scenarios are Waiting for design decisions. See https://docs.google.com/document/d/1JpBvo5hkHSNAZAMno_YdW6AWZGufAMAgDgWkfc4Xj2c/edit#bookmark=id.4wuun7f6ogjw

			CurrentChapterInfo = _project.SelectedChapterInfo;
			var currentRecordingInfo = CurrentRecordingInfo;
			if (currentRecordingInfo != null && currentRecordingInfo.Number - 1 != _project.SelectedScriptBlock)
				return; // Initializing during restart to change color scheme... not ready yet

			_chkIgnoreProblem.CheckedChanged -= _chkIgnoreProblem_CheckedChanged;

			Show();

			var haveRecording = ClipRepository.GetHaveClipUnfiltered(_project.Name, _project.SelectedBook.Name,
				_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock);
			_audioButtonsControl.Visible = _chkIgnoreProblem.Enabled = _flowLayoutPanelThen.Visible = _txtThen.Visible = haveRecording;
			_chkIgnoreProblem.Visible = _lblNow.Visible = _txtNow.Visible = true;
			_chkIgnoreProblem.Text = _standardIgnoreText;
			_txtThen.Enabled = true;
			_chkIgnoreProblem.Checked = false;
			CurrentCleanupAction = CleanupAction.None;
			_txtNow.Font = _txtThen.Font = new Font(CurrentScriptLine.FontName, CurrentScriptLine.FontSize * ZoomFactor);
			_txtNow.Text = CurrentScriptLine.Text;
			if (currentRecordingInfo == null)
			{
				CheckForExtraRecordings();
				if (haveRecording)
				{
					if (CurrentCleanupAction == CleanupAction.None)
					{
						CurrentCleanupAction = CleanupAction.UpdateInfo;
						// We have a recording, but we don't know anything about the script at the time it was recorded.
						_lblProblemSummary.Text = Format(LocalizationManager.GetString("ScriptTextHasChangedControl.ScriptTextAtTimeOfRecordingUnknown",
							"Problem: The clip for this block was recorded on {0}, which was before {1} started saving the version of the script text " +
							"at the time of recording.",
							"Param 0: recording date; Param 1: \"HearThis\""), ActualFileRecordingDateForUI, ProductName);
						_flowLayoutPanelThen.Visible = false;
					}
					else
						_lblRecordedDate.Text = Format(_fmtRecordedDate, ActualFileRecordingDateForUI);
				}
				else
				{
					_lblNow.Visible = false;
					_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.NotRecorded",
						"This block has not yet been recorded.");
				}
			}
			else
			{
				Debug.Assert(haveRecording); // If not, we need to remove the info about the recording!
				_txtThen.Text = currentRecordingInfo.TextAsOriginallyRecorded;

				if (_txtNow.Text != currentRecordingInfo.Text)
				{
					CurrentCleanupAction = CleanupAction.UpdateInfo;
					_lblProblemSummary.Text = _standardProblemText;
				}
				else
				{
					if (CheckForExtraRecordings())
						_lblNow.Visible = _txtNow.Visible = false;
					else
					{
						if (_txtNow.Text != currentRecordingInfo.OriginalText)
						{
							// REVIEW: _txtThen.Enabled = false;
							_chkIgnoreProblem.Checked = true;
							_lblProblemSummary.Text = _standardProblemText;
						}
						else
						{
							_lblNow.Visible = _txtNow.Visible = _flowLayoutPanelThen.Visible =
								_chkIgnoreProblem.Enabled = _chkIgnoreProblem.Visible = false;
							_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.NoProblem",
								"No problems");
						}
					}
				}

				_lblRecordedDate.Text = Format(_fmtRecordedDate, currentRecordingInfo.RecordingTime.ToLocalTime().ToShortDateString());
			}

			if (_chkIgnoreProblem.Enabled)
				_chkIgnoreProblem.Focus();
			else if (_audioButtonsControl.Enabled)
				_audioButtonsControl.Focus();

			_audioButtonsControl.UpdateDisplay();
			_chkIgnoreProblem.CheckedChanged += _chkIgnoreProblem_CheckedChanged;
		}

		private bool CheckForExtraRecordings()
		{
			if (CurrentScriptLine.Number == CurrentChapterInfo.GetUnfilteredScriptBlockCount() && CurrentChapterInfo.HasRecordingInfoBeyondExtentOfCurrentScript)
			{
				CurrentCleanupAction = CleanupAction.DeleteExtraRecordings;
				_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.ExtraRecordings",
					"Problem: There are extra recordings that go beyond the extent of the current script.");
				_chkIgnoreProblem.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.FixProblem",
					"Fix this problem.");
				// REVIEW: Should we change it so the play button plays any clips beyond the current block?
				return true;
			}

			return false;
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
			get => _zoomFactor;
			set
			{
				_zoomFactor = value;
				Invalidate();
			}
		}

		private void _chkIgnoreProblem_CheckedChanged(object sender, EventArgs e)
		{
			if (_chkIgnoreProblem.Checked)
			{
				switch (CurrentCleanupAction)
				{
					case CleanupAction.UpdateInfo:
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
						break;
					case CleanupAction.DeleteExtraRecordings:
						CurrentChapterInfo.RemoveRecordingInfoBeyondCurrentScriptExtent();
						ClipRepository.DeleteAllClipsAfterLine(_project.Name, _project.SelectedBook.Name,
							_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock);
						break;
					default:
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
			UpdateDisplay();
		}
	}
}
