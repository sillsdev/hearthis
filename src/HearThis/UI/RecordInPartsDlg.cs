using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Publishing;
using SIL.IO;
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
		Timer _waitToJoinTimer = new Timer();
		private Color _scriptSecondHalfColor = AppPallette.SecondPartTextColor;
		private AudioButtonsControl _audioButtonCurrent;

		public RecordInPartsDlg()
		{
			// TempFile creates empty files, but we don't want them to exist until there is a real
			// recording to play, because it undesirably enables the play buttons.
			File.Delete(_tempFile1.Path);
			File.Delete(_tempFile2.Path);
			File.Delete(_tempFileJoined.Path);

			InitializeComponent();
			if (Settings.Default.RecordInPartsFormSettings == null)
				Settings.Default.RecordInPartsFormSettings = FormSettings.Create(this);
			_audioButtonCurrent = _audioButtonsFirst;

			_audioButtonsFirst.HaveSomethingToRecord = _audioButtonsSecond.HaveSomethingToRecord = true;
			_audioButtonsFirst.Path = _tempFile1.Path;
			_audioButtonsSecond.Path = _tempFile2.Path;
			_audioButtonsBoth.Path = _tempFileJoined.Path;
			_audioButtonsFirst.SoundFileCreated += AudioButtonsOnSoundFileCreated;
			_audioButtonsSecond.SoundFileCreated += AudioButtonsOnSoundFileCreated;
			_waitToJoinTimer.Interval = 200;
			_waitToJoinTimer.Tick += (sender, args) =>
			{
				_waitToJoinTimer.Stop();
				// It seems that logically it should be possible to put this code simply in AudioButtonsOnSoundFileCreated.
				// However when I do so the merge consistently fails; it behaves as if there is nothing in
				// the most recently created sound file.
				// It seems that the recording code is raising the event just barely before the recording is actually
				// available. A short delay is the only way I've found to make the join work.
				// Unfortunately I have no way of knowing how long the delay needs to be in general.
				// On my fast development computer, 80ms is enough and 60ms is not. Will need to test on some slower
				// machines.
				var inputFiles = new[] {_tempFile1.Path, _tempFile2.Path};
				if (RecordingExists(_tempFile2.Path))
				{
					ClipRepository.MergeAudioFiles(inputFiles, _tempFileJoined.Path, new NullProgress());
					// Don't advance current, default play is to play just this bit next.
					//_audioButtonCurrent = _audioButtonsBoth;
				}
				UpdateDisplay();
			};
			UpdateDisplay();
			_recordTextBox.ForeColor = AppPallette.ScriptFocusTextColor;
			_recordTextBox.SelectionChanged += RecordTextBoxOnSelectionChanged;
			_recordTextBox.ReadOnly = true;
			Application.AddMessageFilter(this);
			Closing += (sender, args) => Application.RemoveMessageFilter(this);
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
			_recordTextBox.SelectionColor = AppPallette.ScriptFocusTextColor;

			_recordTextBox.SelectionStart = originalStart;
			_recordTextBox.SelectionLength = _recordTextBox.TextLength - originalStart;
			_recordTextBox.SelectionColor = _scriptSecondHalfColor;
			_recordTextBox.SelectionLength = 0;
			_handlingSelChanged = false;
			_labelBothOne.ForeColor = _labelOne.ForeColor = AppPallette.ScriptFocusTextColor;
			_labelBothTwo.ForeColor = _labelTwo.ForeColor = _scriptSecondHalfColor;
		}

		private void UpdateDisplay()
		{
			_audioButtonsFirst.HaveSomethingToRecord = true;
			_audioButtonsSecond.HaveSomethingToRecord = RecordingExists(_tempFile1.Path);
				// trick to disable record until 1st done
			_audioButtonsBoth.HaveSomethingToRecord = RecordingExists(_tempFile2.Path); // trick to disable play until 2nd done
			// Next buttons are hidden, so this is a way to have nothing highlighted.
			_audioButtonsFirst.ButtonHighlightMode =
				_audioButtonsSecond.ButtonHighlightMode =
					_audioButtonsBoth.ButtonHighlightMode = AudioButtonsControl.ButtonHighlightModes.Next;
			if (RecordingExists(_audioButtonCurrent.Path))
				_audioButtonCurrent.ButtonHighlightMode = AudioButtonsControl.ButtonHighlightModes.Play;
			else
				_audioButtonCurrent.ButtonHighlightMode = AudioButtonsControl.ButtonHighlightModes.Record;
			_audioButtonsFirst.UpdateDisplay();
			_audioButtonsSecond.UpdateDisplay();
			_audioButtonsBoth.UpdateDisplay();
			//the default disabled text color is not different enough from enabled, when the background color of the button is not
			//white. So instead it's always enabled but we control the text color here.
			//_useRecordingsButton.Enabled = RecordingExists(_tempFile2.Path);
			_useRecordingsButton.ForeColor = RecordingExists(_tempFile2.Path)
				? SystemColors.ControlText
				: SystemColors.ControlDark;
			;
		}

		void AdvanceCurrent()
		{
			if (_audioButtonCurrent == _audioButtonsFirst && RecordingExists(_audioButtonsFirst.Path))
				_audioButtonCurrent = _audioButtonsSecond;
			else if (_audioButtonCurrent == _audioButtonsSecond && RecordingExists(_audioButtonsSecond.Path))
				_audioButtonCurrent = _audioButtonsBoth;
			UpdateDisplay();
		}

		void GoBack()
		{
			if (_audioButtonCurrent == _audioButtonsSecond)
				_audioButtonCurrent = _audioButtonsFirst;
			else if (_audioButtonCurrent == _audioButtonsBoth)
				_audioButtonCurrent = _audioButtonsSecond;
			UpdateDisplay();
		}

		/// <summary>
		/// Filter out all keystrokes except the few that we want to handle.
		/// We handle Space, Enter, Period, PageUp, PageDown, Delete and Arrow keys.
		/// </summary>
		/// <remarks>This is invoked because we implement IMessagFilter and call Application.AddMessageFilter(this)</remarks>
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
					UpdateDisplay();
					break;

				case Keys.Space:
					var recordButton = _audioButtonCurrent;
					// If the user is trying to record but the control with no visible record is active,
					// presume he is wanting another go at recording the second segment.
					if (_audioButtonCurrent == _audioButtonsBoth)
						recordButton = _audioButtonsSecond;
					if (m.Msg == WM_KEYDOWN)
						recordButton.SpaceGoingDown();
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

		private void AudioButtonsOnSoundFileCreated(object sender, EventArgs eventArgs)
		{
			_waitToJoinTimer.Start();
		}

		public Font VernacularFont
		{
			get { return _recordTextBox.Font; }
			set { _recordTextBox.Font = value; }
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
				_tempFile1.Dispose();
				_tempFile2.Dispose();
				_tempFileJoined.Dispose();
			}
			base.Dispose(disposing);
		}

		public string TextToRecord
		{
			get { return _recordTextBox.Text; }
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

		public RecordingDevice RecordingDevice
		{
			get { return _audioButtonsFirst.RecordingDevice; }
			set { _audioButtonsFirst.RecordingDevice = _audioButtonsSecond.RecordingDevice = value; }
		}

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
				File.Delete(destPath);
				File.Copy(_tempFileJoined.Path, destPath);
			}
			catch (Exception err)
			{
				// The corresponding problem in the AudioButtonsControl is not localizable, presumably because it was thought
				// to unlikely to happen, so I haven't done it for this one either.
				ErrorReport.NotifyUserOfProblem(err,
					String.Format(
						"Sigh. HearThis was unable to copy the combined recording to {0} where it belongs. Restarting HearThis might solve this problem.",
						destPath));
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			Settings.Default.RecordInPartsFormSettings.InitializeForm(this);
			base.OnLoad(e);
			UpdateDisplay();
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
	}
}
