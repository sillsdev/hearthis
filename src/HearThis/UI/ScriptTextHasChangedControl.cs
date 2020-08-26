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
using System.IO;
using System.Linq;
using System.Threading;
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
		public event EventHandler ProblemIgnored;

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

		public void SetData(ScriptLine block)
		{
			CurrentScriptLine = block;
			UpdateDisplay();
		}
		
		public void SetProject(Project project)
		{
			_project = project;
			UpdateDisplay();
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
			Show();

			// TODO: Some of these scenarios are Waiting for design decisions. See https://docs.google.com/document/d/1JpBvo5hkHSNAZAMno_YdW6AWZGufAMAgDgWkfc4Xj2c/edit#bookmark=id.4wuun7f6ogjw

			CurrentChapterInfo = _project.SelectedChapterInfo;
			var currentRecordingInfo = CurrentRecordingInfo;
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
							"Problem: The clip recorded for this block was made before {0} started saving the script text.", "Parameter is \"HearThis\""), ProductName);
					}

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
				if (_txtNow.Text != currentRecordingInfo.Text)
				{
					CurrentCleanupAction = CleanupAction.UpdateInfo;
					_lblProblemSummary.Text = _standardProblemText;
				}
				else
				{
					_lblNow.Visible = _txtNow.Visible = false;
					if (!CheckForExtraRecordings())
					{
						_flowLayoutPanelThen.Visible = false;
						_lblProblemSummary.Text = Format(LocalizationManager.GetString("ScriptTextHasChangedControl.NoProblem",
							"The script text has not changed for this block since the clip was recorded ({0}).", "Parameter is recorded date"), ActualFileRecordingDateForUI);
						_chkIgnoreProblem.Enabled = false;
					}
				}

				_txtThen.Text = currentRecordingInfo.Text;
				_lblRecordedDate.Text = Format(_fmtRecordedDate, currentRecordingInfo.RecordingTime.ToLocalTime().ToShortDateString());
			}
			if (_txtThen.Visible)
				tableLayoutPanel1.RowStyles[tableLayoutPanel1.GetRow(_txtThen)].SizeType = SizeType.Percent;
			else
				tableLayoutPanel1.RowStyles[tableLayoutPanel1.GetRow(_txtThen)].SizeType = SizeType.AutoSize;

			do
			{
				if (Monitor.TryEnter(_audioButtonsControl, 10))
				{
					_audioButtonsControl.Path = _project.GetPathToRecordingForSelectedLine();
					_audioButtonsControl.ContextForAnalytics = new Dictionary<string, string>
					{
						{"book", _project.SelectedBook.Name},
						{"chapter", _project.SelectedChapterInfo.ChapterNumber1Based.ToString()},
						{"scriptBlock", _project.SelectedScriptBlock.ToString()},
						{"wordsInLine", CurrentScriptLine.ApproximateWordCount.ToString()}
					};
					break;
				}
			} while (true);
			if (_chkIgnoreProblem.Enabled)
				_chkIgnoreProblem.Focus();
			else if (_audioButtonsControl.Enabled)
				_audioButtonsControl.Focus();

			_audioButtonsControl.UpdateDisplay();
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
				ProblemIgnored?.Invoke(this, new EventArgs());
				UpdateDisplay();
			}
		}
	}
}
