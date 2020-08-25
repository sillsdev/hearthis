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
		private Project _project;
		private Animator _animator;
		private PointF _animationPoint;
		//private Direction _direction;
		private static float _zoomFactor;
		private string _standardProblemText;
		private string _fmtRecordedDate;
		private ScriptLine CurrentScriptLine { get; set; }
		private ChapterInfo CurrentChapterInfo { get; set; }
		private PaintData _outgoingData;
		public bool ShowSkippedBlocks { get; set; }

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

		public void UpdateDisplay()
		{
			if (_project == null || !HaveScript)
			{
				Hide();
				return; // Not ready yet
			}
			Show();

			CurrentChapterInfo = _project.SelectedChapterInfo;
			var currentRecordingInfo = CurrentRecordingInfo;
			var haveRecording = ClipRepository.GetHaveClipUnfiltered(_project.Name, _project.SelectedBook.Name,
				_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock);
			_audioButtonsControl.Visible = _chkIgnoreProblem.Enabled = _lblThen.Visible = _txtThen.Visible = _lblRecordedDate.Visible = haveRecording;
			_chkIgnoreProblem.Visible = _lblNow.Visible = _txtNow.Visible = true;
			_txtThen.Enabled = true;
			_chkIgnoreProblem.Checked = false;
			_txtNow.Font = _txtThen.Font = new Font(CurrentScriptLine.FontName, CurrentScriptLine.FontSize * ZoomFactor);
			_txtNow.Text = CurrentScriptLine.Text;
			if (currentRecordingInfo == null)
			{
				if (haveRecording)
				{
					// We have a recording, but we don't know anything about the script at the time it was recorded.
					_lblProblemSummary.Text = Format(LocalizationManager.GetString("ScriptTextHasChangedControl.ScriptTextAtTimeOfRecordingUnknown",
						"Problem: The clip recorded for this block was made before {0} started saving the script text.", "Parameter is \"HearThis\""), ProductName);
					_lblRecordedDate.Text = Format(_fmtRecordedDate, ActualFileRecordingTime.ToLocalTime().ToShortDateString());
				}
				else
				{
					_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.NotRecorded",
						"This block has not yet been recorded.");
				}
			}
			else
			{
				Debug.Assert(haveRecording); // If not, we need to remove the info about the recording!
				if (_txtNow.Text != currentRecordingInfo.Text)
				{
					_lblProblemSummary.Text = _standardProblemText;
				}
				else
				{
					// TODO: Waiting for design decision.
					_lblProblemSummary.Text = LocalizationManager.GetString("ScriptTextHasChangedControl.NoProblem",
						"The script text has not changed for this block since the clip was recorded.");
					_chkIgnoreProblem.Visible = _lblNow.Visible = _txtNow.Visible = false;
				}

				_txtThen.Text = currentRecordingInfo.Text;
				_lblRecordedDate.Text = Format(_fmtRecordedDate, currentRecordingInfo.RecordingTime.ToLocalTime().ToShortDateString());
			}
			if (_txtThen.Visible)
				tableLayoutPanel1.RowStyles[tableLayoutPanel1.GetRow(_txtThen)].SizeType = SizeType.Percent;
			else
				tableLayoutPanel1.RowStyles[tableLayoutPanel1.GetRow(_txtThen)].SizeType = SizeType.AutoSize;

			_audioButtonsControl.Path = _project.GetPathToRecordingForSelectedLine();
			_audioButtonsControl.ContextForAnalytics = new Dictionary<string, string>
			{
				{"book", _project.SelectedBook.Name},
				{"chapter", _project.SelectedChapterInfo.ChapterNumber1Based.ToString()},
				{"scriptBlock", _project.SelectedScriptBlock.ToString()},
				{"wordsInLine", CurrentScriptLine.ApproximateWordCount.ToString()}
			};
			_audioButtonsControl.UpdateDisplay();
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
				_chkIgnoreProblem.Enabled = false;
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
				_txtThen.Enabled = false;
				_audioButtonsControl.Focus();
			}
		}
	}
}
