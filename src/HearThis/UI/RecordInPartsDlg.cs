// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2016-2025, SIL Global.
// <copyright from='2016' to='2025' company='SIL Global'>
//		Copyright (c) 2016-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Publishing;
using L10NSharp;
using SIL.IO;
using SIL.Progress;
using SIL.Reporting;
using SIL.Windows.Forms.PortableSettingsProvider;
using static System.String;
using static HearThis.SafeSettings;

namespace HearThis.UI
{
	public partial class RecordInPartsDlg : Form, IMessageFilter
	{
		private readonly TempFile _tempFile1 = TempFile.WithExtension("wav");
		readonly TempFile _tempFile2 = TempFile.WithExtension("wav");
		private readonly TempFile _tempFileJoined = TempFile.WithExtension("wav");
		private readonly Color _scriptSecondHalfColor = AppPalette.SecondPartTextColor;
		private AudioButtonsControl _audioButtonCurrent;
		private readonly Color _defaultForegroundColorForInstructions;
		public event EventHandler RecordingAttemptAbortedBecauseOfNoMic;

		public RecordInPartsDlg()
		{
			// TempFile creates empty files, but we don't want them to exist until there is a real
			// recording to play, because it undesirably enables the play buttons.
			RobustFile.Delete(_tempFile1.Path);
			RobustFile.Delete(_tempFile2.Path);
			RobustFile.Delete(_tempFileJoined.Path);

			InitializeComponent();
			_defaultForegroundColorForInstructions = _instructionsLabel.ForeColor;
			if (Get(() => Settings.Default.RecordInPartsFormSettings) == null)
				Set(() => Settings.Default.RecordInPartsFormSettings = FormSettings.Create(this));
			_audioButtonCurrent = _audioButtonsFirst;
			
			_audioButtonsFirst.Path = _tempFile1.Path;
			_audioButtonsSecond.Path = _tempFile2.Path;
			_audioButtonsBoth.Path = _tempFileJoined.Path;
			UpdateDisplay();

			_recordTextBox.ForeColor = AppPalette.ScriptFocusTextColor;
			BackColor = AppPalette.Background;
			_recordTextBox.BackColor = AppPalette.Background;
			Closing += (sender, args) => Application.RemoveMessageFilter(this);

			_audioButtonsFirst.AlternatePlayButtonBaseToolTip = LocalizationManager.GetString(
				"RecordingControl.RecordInPartsDlg.PlayFirstPartButton_ToolTip_Base",
				"Play the clip for the first part");
			_audioButtonsFirst.AlternateRecordButtonBaseToolTip = LocalizationManager.GetString(
				"RecordingControl.RecordInPartsDlg.RecordFirstPartButton_ToolTip_Base",
				"Record the first part");

			_audioButtonsSecond.AlternatePlayButtonBaseToolTip = LocalizationManager.GetString(
				"RecordingControl.RecordInPartsDlg.PlaySecondPartButton_ToolTip_Base",
				"Play the clip for the second part");
			_audioButtonsSecond.AlternateRecordButtonBaseToolTip = LocalizationManager.GetString(
				"RecordingControl.RecordInPartsDlg.RecordSecondPartButton_ToolTip_Base",
				"Record the second part");
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			Application.AddMessageFilter(this);
		}

		/// <summary>
		/// The audio buttons control that should be used to initiate a new recording or
		/// that is currently being used for recording by pressing and holding the space bar
		/// </summary>
		/// <remarks>If the user is trying to record but the control with no visible record is
		/// active, presume he is wanting another go at recording the second segment.</remarks>
		private AudioButtonsControl CurrentAudioButtonForRecordingViaSpace =>
			_audioButtonCurrent == _audioButtonsBoth ? _audioButtonsSecond : _audioButtonCurrent;

		private void RecordingStarting(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (sender != _audioButtonsFirst)
			{
				_audioButtonsFirst.StopPlaying();
				if (_audioButtonCurrent == _audioButtonsFirst)
					_audioButtonCurrent = _audioButtonsSecond;
			}

			if (sender != _audioButtonsSecond)
				_audioButtonsSecond.StopPlaying();

			_audioButtonsBoth.StopPlaying();

			Logger.WriteEvent("RecordInPartsDlg.RecordingStarting for " + ((AudioButtonsControl)sender).Name);
			// Although the instructions are not actually script context, their proximity to the
			// text to be recorded could be confusing, so we'll mute them during the recording.
			_instructionsLabel.ForeColor = AppPalette.ScriptContextTextColorDuringRecording;
		}

		private static bool RecordingExists(string path)
		{
			return File.Exists(path) && new FileInfo(path).Length > 0;
		}

		private bool _handlingSelChanged = false;

		private void RecordTextBoxOnSelectionChanged(object sender, EventArgs eventArgs)
		{
			// Checking selectionStart stops it from firing when the dialog first comes up.
			// Incidentally prevents it ALL changing color.
			if (_handlingSelChanged || _recordTextBox.SelectionLength > 0 || _recordTextBox.SelectionStart == 0)
				return;
			_handlingSelChanged = true;
			int originalStart = _recordTextBox.SelectionStart;
			_recordTextBox.SelectionStart = 0;
			_recordTextBox.SelectionLength = originalStart;
			_recordTextBox.SelectionColor = AppPalette.ScriptFocusTextColor;

			if (originalStart < _recordTextBox.Text.Length)
			{
				_recordTextBox.SelectionStart = originalStart;
				_recordTextBox.SelectionLength = _recordTextBox.TextLength - originalStart;
				_recordTextBox.SelectionColor = _scriptSecondHalfColor;
				_labelBothOne.ForeColor = _labelOne.ForeColor = AppPalette.ScriptFocusTextColor;
				_labelBothTwo.ForeColor = _labelTwo.ForeColor = _scriptSecondHalfColor;
			}
			else
			{
				_labelBothOne.ForeColor = _labelOne.ForeColor = _labelBothTwo.ForeColor =
					_labelTwo.ForeColor = SystemColors.ControlLight;
			}
			_recordTextBox.SelectionLength = 0;
			_handlingSelChanged = false;
		}

		private void UpdateDisplay()
		{
			// trick to disable recording second part until 1st is done
			_audioButtonsSecond.HaveSomethingToRecord = RecordingExists(_tempFile1.Path);
			bool haveBothPartsRecorded = RecordingExists(_tempFile2.Path);
			_audioButtonsBoth.HaveSomethingToRecord = haveBothPartsRecorded; // trick to disable play until 2nd done
			// Next buttons are hidden, so this is a way to have nothing highlighted.
			_audioButtonsFirst.ButtonHighlightMode =
				_audioButtonsSecond.ButtonHighlightMode =
					_audioButtonsBoth.ButtonHighlightMode = AudioButtonsControl.ButtonHighlightModes.Next;
			_audioButtonCurrent.ButtonHighlightMode = RecordingExists(_audioButtonCurrent.Path) ?
				AudioButtonsControl.ButtonHighlightModes.Play : AudioButtonsControl.ButtonHighlightModes.Record;
			_audioButtonsFirst.ShowKeyboardShortcutsInTooltips = _audioButtonsFirst == _audioButtonCurrent;
			_audioButtonsSecond.ShowKeyboardShortcutsInTooltips = _audioButtonsSecond == _audioButtonCurrent;
			_audioButtonsBoth.ShowKeyboardShortcutsInTooltips = _audioButtonsBoth == _audioButtonCurrent;
			_audioButtonsFirst.UpdateDisplay();
			_audioButtonsSecond.UpdateDisplay();
			_audioButtonsBoth.UpdateDisplay();
			//the default disabled text color is not different enough from enabled, when the background color of the button is not
			//white. So instead it's always enabled but we control the text color here.
			//_useRecordingsButton.Enabled = haveBothPartsRecorded;
			if (haveBothPartsRecorded)
			{
				_useRecordingsButton.DialogResult = DialogResult.OK;
				AcceptButton = _useRecordingsButton;
				_useRecordingsButton.Click -= _useRecordingsButton_PrematureClick;
			}

			_useRecordingsButton.ForeColor = haveBothPartsRecorded
				? SystemColors.ControlText
				: SystemColors.ControlDark;
		}

		void AdvanceCurrent()
		{
			if (_audioButtonCurrent == _audioButtonsFirst && RecordingExists(_audioButtonsFirst.Path))
				_audioButtonCurrent = _audioButtonsSecond;
			else if (_audioButtonCurrent == _audioButtonsSecond && RecordingExists(_audioButtonsSecond.Path))
				_audioButtonCurrent = _audioButtonsBoth;
			Logger.WriteEvent("Advanced current to " + _audioButtonCurrent.Name);
			UpdateDisplay();
		}

		void GoBack()
		{
			if (_audioButtonCurrent == _audioButtonsSecond)
				_audioButtonCurrent = _audioButtonsFirst;
			else if (_audioButtonCurrent == _audioButtonsBoth)
				_audioButtonCurrent = _audioButtonsSecond;
			Logger.WriteEvent("Set current back to " + _audioButtonCurrent.Name);
			UpdateDisplay();
		}

		/// <summary>
		/// Filter out all keystrokes except the few that we want to handle.
		/// We handle Space, Enter, Period, PageUp, PageDown, Delete and Arrow keys.
		/// </summary>
		/// <remarks>This is invoked because we implement IMessageFilter and call Application.AddMessageFilter(this)</remarks>
		public bool PreFilterMessage(ref Message m)
		{
			const int WM_KEYDOWN = 0x100;
			const int WM_KEYUP = 0x101;

			if (m.Msg != WM_KEYDOWN && m.Msg != WM_KEYUP)
				return false;

			if (m.Msg == WM_KEYUP)
			{
				if ((Keys)m.WParam != Keys.Space)
					return false;
				CurrentAudioButtonForRecordingViaSpace.SpaceGoingUp();
				return true;
			}

			// Nothing else below makes sense to do while recording.
			if (AudioButtonsControl.Recorder.IsRecording)
				return false;

			switch ((Keys) m.WParam)
			{
				// REVIEW: The period has not been used to initiate playback for many years.
				// It's really unlikely that anyone would still be in the habit of doing that
				// or think to try it. We should probably have some kind of user-facing help
				// document that tells them what the unadvertised shortcut keys are. But we should
				// probably just rip out this code (here and in RecordingToolControl). If we do
				// keep it, it should be localizable.
				case Keys.OemPeriod:
				case Keys.Decimal:
					MessageBox.Show("To play the clip, press the TAB key.");
					break;

				case Keys.Tab:
					if (RecordingExists(_audioButtonCurrent.Path))
						_audioButtonCurrent.OnPlay(this, null);
					else if (RecordingExists(_audioButtonsFirst.Path))
					{
						_audioButtonsFirst.OnPlay(this, null); // Play first while second current if second not recorded.
						_audioButtonCurrent = _audioButtonsFirst;
					}
					UpdateDisplay();
					break;

				case Keys.Right:
				case Keys.PageDown:
				case Keys.Down:
					AdvanceCurrent();
					break;

				case Keys.Left:
				case Keys.PageUp:
				case Keys.Up:
					GoBack();
					break;

				case Keys.D1:
					_audioButtonCurrent = _audioButtonsFirst;
					UpdateDisplay();
					break;

				case Keys.D2:
					if (!RecordingExists(_audioButtonsFirst.Path))
						break;
					_audioButtonCurrent = _audioButtonsSecond;
					UpdateDisplay();
					break;

				case Keys.D3:
					if (!RecordingExists(_audioButtonsSecond.Path))
						break;
					_audioButtonCurrent = _audioButtonsBoth;
					// Since there's nothing else to do with this control but play it, might as
					// well go ahead and kick off playback now.
					_audioButtonsBoth.OnPlay(this, null);
					UpdateDisplay();
					break;

				case Keys.Space:
					CurrentAudioButtonForRecordingViaSpace.SpaceGoingDown();
					break;

				// Seems this should be unnecessary, since this is the OK button,
				// but if the rich text box has focus, without this the program thinks
				// we are trying to edit.
				case Keys.Enter:
					_useRecordingsButton.PerformClick();
					break;

				default:
					return false;
			}

			return true;
		}

		private void AudioButtonsOnSoundFileCreated(AudioButtonsControl sender, Exception error)
		{
			Logger.WriteEvent($"RecordInPartsDlg.AudioButtonsOnSoundFileCreated raised for {sender.Name}");
			if (error == null)
			{
				if (RecordingExists(_tempFile2.Path))
				{
					var inputFiles = new[] {_tempFile1.Path, _tempFile2.Path};
					ClipRepository.MergeAudioFiles(inputFiles, _tempFileJoined.Path, new NullProgress());
					// Don't advance current, default play is to play just this bit next.
				}
				else if (_audioButtonCurrent == _audioButtonsSecond)
					throw new ApplicationException("AudioButtonsOnSoundFileCreated after recording segment 2, but the recording does not exist or is of length 0!");
			}

			if (!_audioButtonsFirst.Recording && !_audioButtonsSecond.Recording)
				_instructionsLabel.ForeColor = _defaultForegroundColorForInstructions;

			UpdateDisplay();
		}

		public Font VernacularFont
		{
			get => _recordTextBox.Font;
			set => _recordTextBox.Font = value;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				components?.Dispose();
				_tempFile1.Dispose();
				_tempFile2.Dispose();
				_tempFileJoined.Dispose();
				_audioButtonsFirst.SoundFileRecordingComplete -= AudioButtonsOnSoundFileCreated;
				_audioButtonsSecond.SoundFileRecordingComplete -= AudioButtonsOnSoundFileCreated;
				_recordTextBox.SelectionChanged -= RecordTextBoxOnSelectionChanged;
				_audioButtonsFirst.RecordingStarting -= RecordingStarting;
				_audioButtonsSecond.RecordingStarting -= RecordingStarting;
			}
			base.Dispose(disposing);
		}

		public string TextToRecord
		{
			get => _recordTextBox.Text;
			set
			{
				if (Get(() => Settings.Default.BreakLinesAtClauses))
				{
					var splitter = ScriptControl.ScriptBlockPainter.ClauseSplitter;
					var clauses = splitter.BreakIntoChunks(value);
					_recordTextBox.Text = Join("\n", clauses.Select(c => c.Text).ToArray());
					// Note: doing this means that the value we get may not match the value that was set.
					// Currently I don't think we actually use the getter, but if we ever do we might
					// want to fix this.
				}
				else
				{
					_recordTextBox.Text = value;
				}
			}
		}

		public Dictionary<string, string> ContextForAnalytics
		{
			get => _audioButtonsFirst.ContextForAnalytics;
			set => _audioButtonsFirst.ContextForAnalytics =
				_audioButtonsSecond.ContextForAnalytics = _audioButtonsBoth.ContextForAnalytics = value;
		}

		public bool WriteCombinedAudio(string destPath)
		{
			if (!File.Exists(_tempFileJoined.Path))
				throw new InvalidOperationException("Valid only if the user made two recordings and accepted them.");
			try
			{
				RobustFile.Copy(_tempFileJoined.Path, destPath, true);
			}
			catch (Exception err)
			{
				ErrorReport.NotifyUserOfProblem(err, Format(LocalizationManager.GetString("RecordingControl.RecordInPartsDlg.ErrorMovingExistingRecording",
					"{0} was unable to copy the combined recording to the correct destination:\r\n{1}\r\n" +
					"Please report this error. Restarting {0} might solve this problem.",
					"Param 0: \"HearThis\" (product name); Param 1: Destination filename"),
					ProductName, destPath));
				return false;
			}

			return true;
		}

		protected override void OnLoad(EventArgs e)
		{
			Logger.WriteEvent("Recording in parts");
			Get(() => Settings.Default.RecordInPartsFormSettings).InitializeForm(this);
			base.OnLoad(e);
			UpdateDisplay();
		}

		private void OnRecordButtonStateChanged(object sender, BtnState newState)
		{
			if (sender == _audioButtonsFirst)
			{
				_audioButtonsSecond.UpdateDisplay();
				if (_audioButtonsFirst.Recording)
					return;
			}

			if (sender == _audioButtonsSecond)
			{
				_audioButtonsFirst.UpdateDisplay();
				if (_audioButtonsSecond.Recording)
					return;
			}

			_instructionsLabel.ForeColor = (newState == BtnState.MouseOver) ?
				AppPalette.ScriptContextTextColorDuringRecording :
				_defaultForegroundColorForInstructions;
		}

		protected override void OnClosed(EventArgs e)
		{
			// Stop playback to release the temp files, and dispose the players, thus
			// preventing them from raising a PlaybackStopped event after the control
			// has been disposed.
			_audioButtonsFirst.Path = null;
			_audioButtonsSecond.Path = null;
			_audioButtonsBoth.Path = null;

			base.OnClosed(e);
		}

		private void _useRecordingsButton_PrematureClick(object sender, EventArgs e)
		{
			// Conceivably, they just did it all in one go and are happy, and this will make them unhappy!
			// We're weighing that against someone intending to do 2 but getting confused and clicking this button prematurely.
			MessageBox.Show(this, Format(LocalizationManager.GetString("RecordingControl.RecordInPartsDlg.PrematureRecordingsButtonClick",
				"{0} needs two recordings in order to finish this task. Click \"{1}\" if you don't want to make two recordings.",
				"Param 0: \"HearThis\" (product name); Param 1: Text on cancel button"),
				ProductName, _cancelButton.Text.Replace("&", Empty)),
				Text);
		}

		private void AudioButton_RecordingAttemptAbortedBecauseOfNoMic(object sender, EventArgs e)
		{
			RecordingAttemptAbortedBecauseOfNoMic?.Invoke(this, e);
		}
	}
}
