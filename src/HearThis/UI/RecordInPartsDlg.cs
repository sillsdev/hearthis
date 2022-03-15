// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2016' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
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
using HearThis.Properties;
using HearThis.Publishing;
using L10NSharp;
using SIL.IO;
using SIL.Media;
using SIL.Media.Naudio;
using SIL.Progress;
using SIL.Reporting;
using SIL.Windows.Forms.PortableSettingsProvider;

namespace HearThis.UI
{
	public partial class RecordInPartsDlg : Form, IMessageFilter
	{
		private TempFile _tempFile1 = TempFile.WithExtension("wav");
		TempFile _tempFile2 = TempFile.WithExtension("wav");
		private TempFile _tempFileJoined = TempFile.WithExtension("wav");
		private Color _scriptSecondHalfColor = AppPalette.SecondPartTextColor;
		private AudioButtonsControl _audioButtonCurrent;
		private Color _defaultForegroundColorForInstructions;

		public delegate void RecorderChangedHandler(AudioRecorder newActiveRecorder);
		public event RecorderChangedHandler ActiveRecorderChanged;

		public RecordInPartsDlg(RecordingDevice recordingDevice = null)
		{
			// TempFile creates empty files, but we don't want them to exist until there is a real
			// recording to play, because it undesirably enables the play buttons.
			RobustFile.Delete(_tempFile1.Path);
			RobustFile.Delete(_tempFile2.Path);
			RobustFile.Delete(_tempFileJoined.Path);

			InitializeComponent();
			_defaultForegroundColorForInstructions = _instructionsLabel.ForeColor;
			if (Settings.Default.RecordInPartsFormSettings == null)
				Settings.Default.RecordInPartsFormSettings = FormSettings.Create(this);
			_audioButtonCurrent = _audioButtonsFirst;
			
			_audioButtonsFirst.Path = _tempFile1.Path;
			_audioButtonsSecond.Path = _tempFile2.Path;
			_audioButtonsBoth.Path = _tempFileJoined.Path;
			UpdateDisplay();
			Debug.Assert(_audioButtonsFirst.RecordingDevice == null);
			_audioButtonsFirst.RecordingDevice = recordingDevice;

			_recordTextBox.ForeColor = AppPalette.ScriptFocusTextColor;
			BackColor = AppPalette.Background;
			_recordTextBox.BackColor = AppPalette.Background;
			Application.AddMessageFilter(this);
			Closing += (sender, args) => Application.RemoveMessageFilter(this);
		}

		private AudioButtonsControl AudioButtonCurrent
		{
			get => _audioButtonCurrent;
			set
			{
				_audioButtonCurrent = value;
				if (_audioButtonCurrent.ShowRecordButton)
					SetRecordingDevice(_audioButtonCurrent);
			}
		}

		private void RecordingStarting(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (sender != _audioButtonsFirst)
				_audioButtonsFirst.ReleaseFile();
			if (sender != _audioButtonsSecond)
				_audioButtonsSecond.ReleaseFile();
			_audioButtonsBoth.ReleaseFile();

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
			// Incidentally prevents it ALL going red.
			if (_handlingSelChanged || _recordTextBox.SelectionLength > 0 || _recordTextBox.SelectionStart == 0)
				return;
			_handlingSelChanged = true;
			int originalStart = _recordTextBox.SelectionStart;
			_recordTextBox.SelectionStart = 0;
			_recordTextBox.SelectionLength = originalStart;
			_recordTextBox.SelectionColor = AppPalette.ScriptFocusTextColor;

			_recordTextBox.SelectionStart = originalStart;
			_recordTextBox.SelectionLength = _recordTextBox.TextLength - originalStart;
			_recordTextBox.SelectionColor = _scriptSecondHalfColor;
			_recordTextBox.SelectionLength = 0;
			_handlingSelChanged = false;
			_labelBothOne.ForeColor = _labelOne.ForeColor = AppPalette.ScriptFocusTextColor;
			_labelBothTwo.ForeColor = _labelTwo.ForeColor = _scriptSecondHalfColor;
		}

		private void UpdateDisplay()
		{
			// trick to disable recording second part until 1st is done
			_audioButtonsSecond.HaveSomethingToRecord = RecordingExists(_tempFile1.Path);
			_audioButtonsBoth.HaveSomethingToRecord = RecordingExists(_tempFile2.Path); // trick to disable play until 2nd done
			// Next buttons are hidden, so this is a way to have nothing highlighted.
			_audioButtonsFirst.ButtonHighlightMode =
				_audioButtonsSecond.ButtonHighlightMode =
					_audioButtonsBoth.ButtonHighlightMode = AudioButtonsControl.ButtonHighlightModes.Next;
			if (RecordingExists(AudioButtonCurrent.Path))
				AudioButtonCurrent.ButtonHighlightMode = AudioButtonsControl.ButtonHighlightModes.Play;
			else
				AudioButtonCurrent.ButtonHighlightMode = AudioButtonsControl.ButtonHighlightModes.Record;
			_audioButtonsFirst.UpdateDisplay();
			_audioButtonsSecond.UpdateDisplay();
			_audioButtonsBoth.UpdateDisplay();
			//the default disabled text color is not different enough from enabled, when the background color of the button is not
			//white. So instead it's always enabled but we control the text color here.
			//_useRecordingsButton.Enabled = RecordingExists(_tempFile2.Path);
			_useRecordingsButton.ForeColor = RecordingExists(_tempFile2.Path)
				? SystemColors.ControlText
				: SystemColors.ControlDark;
		}

		void AdvanceCurrent()
		{
			if (AudioButtonCurrent == _audioButtonsFirst && RecordingExists(_audioButtonsFirst.Path))
				AudioButtonCurrent = _audioButtonsSecond;
			else if (AudioButtonCurrent == _audioButtonsSecond && RecordingExists(_audioButtonsSecond.Path))
				AudioButtonCurrent = _audioButtonsBoth;
			Logger.WriteEvent("Advanced current to " + AudioButtonCurrent.Name);
			UpdateDisplay();
		}

		void GoBack()
		{
			if (AudioButtonCurrent == _audioButtonsSecond)
				AudioButtonCurrent = _audioButtonsFirst;
			else if (AudioButtonCurrent == _audioButtonsBoth)
				AudioButtonCurrent = _audioButtonsSecond;
			Logger.WriteEvent("Set current back to " + AudioButtonCurrent.Name);
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

			if (m.Msg == WM_KEYUP && (Keys) m.WParam != Keys.Space)
				return false;

			switch ((Keys) m.WParam)
			{
				case Keys.OemPeriod:
				case Keys.Decimal:
					MessageBox.Show("To play the clip, press the TAB key.");
					break;

				case Keys.Tab:
					if (RecordingExists(AudioButtonCurrent.Path))
						AudioButtonCurrent.OnPlay(this, null);
					else if (RecordingExists(_audioButtonsFirst.Path))
					{
						_audioButtonsFirst.OnPlay(this, null); // Play first while second current if second not recorded.
						AudioButtonCurrent = _audioButtonsFirst;
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
					AudioButtonCurrent = _audioButtonsFirst;
					UpdateDisplay();
					break;

				case Keys.D2:
					if (!RecordingExists(_audioButtonsFirst.Path))
						break;
					AudioButtonCurrent = _audioButtonsSecond;
					UpdateDisplay();
					break;

				case Keys.D3:
					if (!RecordingExists(_audioButtonsSecond.Path))
						break;
					AudioButtonCurrent = _audioButtonsBoth;
					UpdateDisplay();
					break;

				case Keys.Space:
					var recordButton = AudioButtonCurrent;
					// If the user is trying to record but the control with no visible record is active,
					// presume he is wanting another go at recording the second segment.
					if (AudioButtonCurrent == _audioButtonsBoth)
						recordButton = _audioButtonsSecond;
					if (m.Msg == WM_KEYDOWN)
					{
						SetRecordingDevice(recordButton);
						recordButton.SpaceGoingDown();
					}
					if (m.Msg == WM_KEYUP)
						recordButton.SpaceGoingUp();
					break;

				// Seems this should be unnecessary, since this is the OK button,
				// but if the rich text box has focus, without this the program thinks
				// we are trying to edit.
				case Keys.Enter:
					_useRecordingsButton_Click(null, null);
					break;
				case Keys.Escape:
					DialogResult = DialogResult.Cancel;
					Close();
					break;

				default:
					return false;
			}

			return true;
		}

		private void SetRecordingDevice(AudioButtonsControl recordButton)
		{
			if (recordButton.RecordingDevice == null)
			{
				var recordingDevice = RecordingDevice;
				if (recordButton != _audioButtonsFirst)
					_audioButtonsFirst.RecordingDevice = null;
				if (recordButton != _audioButtonsSecond)
					_audioButtonsSecond.RecordingDevice = null;
				recordButton.RecordingDevice = recordingDevice;
				Logger.WriteEvent($"Set RecordingDevice of {recordButton.Name} to {recordButton.RecordingDevice.ProductName}, Id: {recordButton.RecordingDevice.Id}");
				if (recordButton.Recorder.RecordingState == RecordingState.NotYetStarted)
				{
					Logger.WriteEvent($"Calling BeginMonitoring for {recordButton.Name}");
					recordButton.Recorder.BeginMonitoring();
					_audioButtonsFirst.UpdateDisplay();
					_audioButtonsSecond.UpdateDisplay();
				}

				ActiveRecorderChanged?.Invoke(recordButton.Recorder);
			}
		}

		private void AudioButtonsOnSoundFileCreated(object sender, ErrorEventArgs eventArgs)
		{
			Logger.WriteEvent($"RecordInPartsDlg.AudioButtonsOnSoundFileCreated raised for {((AudioButtonsControl)sender).Name}");
			if (eventArgs?.GetException() == null)
			{
				if (RecordingExists(_tempFile2.Path))
				{
					var inputFiles = new[] {_tempFile1.Path, _tempFile2.Path};
					ClipRepository.MergeAudioFiles(inputFiles, _tempFileJoined.Path, new NullProgress());
					// Don't advance current, default play is to play just this bit next.
				}
				else if (AudioButtonCurrent == _audioButtonsSecond)
					throw new ApplicationException("AudioButtonsOnSoundFileCreated after recording clip 2, but the recording does not exist or is of length 0!");
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
				if (Settings.Default.BreakLinesAtClauses)
				{
					var splitter = ScriptControl.ScriptBlockPainter.ClauseSplitter;
					var clauses = splitter.BreakIntoChunks(value);
					_recordTextBox.Text = string.Join("\n", clauses.Select(c => c.Text).ToArray());
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

		public RecordingDevice RecordingDevice =>
			_audioButtonsFirst.RecordingDevice ?? _audioButtonsSecond.RecordingDevice;

		public Dictionary<string, string> ContextForAnalytics
		{
			get { return _audioButtonsFirst.ContextForAnalytics; }
			set
			{
				_audioButtonsFirst.ContextForAnalytics =
					_audioButtonsSecond.ContextForAnalytics = _audioButtonsBoth.ContextForAnalytics = value;
			}
		}

		public void WriteCombinedAudio(string destPath)
		{
			if (!File.Exists(_tempFileJoined.Path))
				return;
			try
			{
				RobustFile.Copy(_tempFileJoined.Path, destPath, true);
			}
			catch (Exception err)
			{
				ErrorReport.NotifyUserOfProblem(err, String.Format(LocalizationManager.GetString("RecordInParts.ErrorMovingExistingRecording",
					"HearThis was unable to copy the combined recording to the correct destination:\r\n{0}\r\n" +
					"Please report this error. Restarting HearThis might solve this problem."), destPath));
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			Logger.WriteEvent("Recording in parts");
			Settings.Default.RecordInPartsFormSettings.InitializeForm(this);
			base.OnLoad(e);
			UpdateDisplay();
		}

		private void OnRecordButtonStateChanged(object sender, BtnState newState)
		{
			if (_audioButtonsFirst.Recording || _audioButtonsSecond.Recording)
				return;
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

		private void _useRecordingsButton_Click(object sender, EventArgs e)
		{
			// Can't use these recordings until we have both
			if (RecordingExists(_audioButtonsSecond.Path))
			{
				DialogResult = DialogResult.OK;
				Close();
			}
			else
			{
				//conceivably, they just did it all in one go and are happy, and this will make them unhappy!
				//we're weighing that against someone intending to do 2 but getting confused and clicking this button prematurely.
				MessageBox.Show(
					"HearThis needs two recordings in order to finish this task. Click 'Cancel' if you don't want to make two recordings.");
			}
		}
		
		private void _audioButton_MouseEnter(object sender, EventArgs e)
		{
			var mousedControl = (AudioButtonsControl)sender;
			// We don't want to activate recording of the second part unless the first part has been recorded.
			if (mousedControl == _audioButtonsFirst || RecordingExists(_audioButtonsFirst.Path))
				SetRecordingDevice(mousedControl);
		}

		// If the mouse was already over a control and the user used a keyboard shortcut to
		// make a different control the active one but then clicks the one they were hovering
		// on, we'll never get a mouse enter, so we need to make the change on the fly.
		private void _audioButton_MouseDown(object sender, MouseEventArgs e) =>
			_audioButton_MouseEnter(sender, e);
	}
}
