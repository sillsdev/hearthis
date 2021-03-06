// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using L10NSharp;
using SIL.IO;
using SIL.Media;
using SIL.Media.Naudio;
using SIL.Reporting;
using Timer = System.Timers.Timer;

namespace HearThis.UI
{
	public partial class AudioButtonsControl : UserControl
	{
		private string _path;
		public AudioRecorder Recorder { get; }
		private ISimpleAudioSession _player;

		public enum ButtonHighlightModes {Default=0, Record, Play, Next};
		public event EventHandler NextClick;
		public event ErrorEventHandler SoundFileRecordingComplete;
		public event CancelEventHandler RecordingStarting;
		public delegate void ButtonStateChangedHandler(object sender, BtnState newState);
		public event ButtonStateChangedHandler RecordButtonStateChanged;

		private readonly string _backupPath;
		private DateTime _startRecording;
		private bool _suppressTooShortWarning;
		private const int kMinMilliseconds = 500;

		/// <summary>
		/// We're using this system timer rather than a normal form timer because with the latter, when the button "captured" the mouse, the timer refused to fire.
		/// </summary>
		private readonly Timer _startRecordingTimer;

		public AudioButtonsControl()
		{
			InitializeComponent();

			Recorder = new AudioRecorder(Settings.Default.MaxRecordingMinutes);
			Recorder.Stopped += OnRecorder_Stopped;

			Path = System.IO.Path.GetTempFileName();
			_startRecordingTimer = new Timer(300);
			_startRecordingTimer.Elapsed += OnStartRecordingTimer_Elapsed;

			_recordButton.CancellableMouseDownCall = TryStartRecord;
			_recordButton.ButtonStateChanged += OnRecordButtonStateChanged;
			_backupPath = System.IO.Path.GetTempFileName();
		}

		private void OnRecordButtonStateChanged(object sender, EventArgs args)
		{
			RecordButtonStateChanged?.Invoke(this, _recordButton.State);
		}


		public ButtonHighlightModes ButtonHighlightMode
		{
			get => _buttonHighlightMode;
			set
			{
				_buttonHighlightMode = value;
				switch (value)
				{
					case ButtonHighlightModes.Play:
						_playButton.IsDefault = true;
						_recordButton.IsDefault = false;
						_nextButton.IsDefault = false;
						break;
					case ButtonHighlightModes.Record:
						_playButton.IsDefault = false;
						_recordButton.IsDefault = true;
						_nextButton.IsDefault = false;
						break;
					case ButtonHighlightModes.Next:
						_playButton.IsDefault = false;
						_recordButton.IsDefault = false;
						_nextButton.IsDefault = true;
						break;
					default:
						_playButton.IsDefault = false;
						_recordButton.IsDefault = false;
						_nextButton.IsDefault = false;
						break;
				}
			}
		}

		public void UpdateDisplay()
		{
			if (InvokeRequired)
			{
				Invoke(new Action(UpdateDisplay));
				return;
			}

			lock (this) // protect _player so we don't get a NullReferenceException
			{
				var isPlaying = _player != null && _player.IsPlaying;
				var canPlay = CanPlay;
				var canRecordNow = !isPlaying && Recorder.RecordingState == RecordingState.Monitoring || Recorder.RecordingState == RecordingState.Stopped;
				var canRecord = HaveSomethingToRecord && canRecordNow;

				if (ButtonHighlightMode == ButtonHighlightModes.Default)
					ButtonHighlightMode = ButtonHighlightModes.Record;

				_recordButton.Enabled = canRecord;
				//Console.WriteLine("record enabled: "+_recordButton.Enabled.ToString());
				_playButton.Enabled = canPlay;
				//            if (_playButton.Enabled)
				//                ButtonHighlightMode = ButtonHighlightModes.Play;
				//			else if(_recordButton.Enabled)
				//				ButtonHighlightMode = ButtonHighlightModes.Record;

				_playButton.Playing = isPlaying;
				_playButton.Invalidate();
			}
		}

		private bool CanPlay
		{
			get
			{
				lock (this) // protect _player so we don't get a NullReferenceException
				{
					/* this was when we were using the same object (naudio-derived) for both playback and recording
					 * (changed to irrklang 4/2013, but could go back if the playback file locking bug were fixed)
					 * return Recorder != null && Recorder.RecordingState != RecordingState.Recording && */
					return _player != null && !_player.IsPlaying && !string.IsNullOrEmpty(Path) && RecordingExists;
				}
			}
		}

		public bool HaveSomethingToRecord;
		private ButtonHighlightModes _buttonHighlightMode;

		public bool Recording => Recorder.RecordingState == RecordingState.Recording || Recorder.RecordingState == RecordingState.RequestedStop;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Path
		{
			get => _path;
			set
			{
				lock (this) // Don't want another thread checking _player while we're swapping it out.
				{
					_path = value;
					DisposePlayer();
					if (!string.IsNullOrEmpty(_path))
					{
						_player = Utils.GetPlayer(FindForm(), _path);
						if (_player is ISimpleAudioWithEvents simpleAudioWithEvents)
							simpleAudioWithEvents.PlaybackStopped += AudioButtonsControl_PlaybackStopped;
					}
				}
			}
		}

		private void DisposePlayer()
		{
			lock (this)// Don't want another thread checking _player while we're disposing it.
			{
				if (_player != null)
				{
					((ISimpleAudioWithEvents)_player).PlaybackStopped -= AudioButtonsControl_PlaybackStopped;
					if (_player.IsPlaying)
						_player.StopPlaying();
					_player?.Dispose();
					_player = null;
				}
			}
		}

		void AudioButtonsControl_PlaybackStopped(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		public RecordingDevice RecordingDevice
		{
			get => Recorder.SelectedDevice as RecordingDevice;
			set => Recorder.SelectedDevice = value;
		}

		public bool CanGoNext
		{
			set => _nextButton.Enabled = value;
		}

		public Dictionary<string,string> ContextForAnalytics;

//
//        private void _playButton_Click(object sender, EventArgs e)
//        {
//            var session = AudioFactory.AudioSession(Path);
//            session.Play();
//        }

		/// <summary>
		/// Start the recording
		/// </summary>
		/// <returns>true if the recording started successfully</returns>
		private bool TryStartRecord()
		{
			if (Recorder.RecordingState == RecordingState.RequestedStop)
			{
				MessageBox.Show(
					LocalizationManager.GetString("AudioButtonsControl.BadState",
						"HearThis is in an unusual state, possibly caused by unplugging a microphone. You will need to restart."),
					LocalizationManager.GetString("AudioButtonsControl.BadStateCaption", "Cannot record"));
				return false;
			}
			if (!_recordButton.Enabled)
				return false; //could be fired by keyboard

			if (RecordingStarting != null)
			{
				var e = new CancelEventArgs();
				RecordingStarting(this, e);
				if (e.Cancel)
					return false;
			}

			// If someone unplugged the microphone we were planning to use switch to another.
			if (!RecordingDevice.Devices.Contains(RecordingDevice))
			{
				RecordingDevice = RecordingDevice.Devices.FirstOrDefault() as RecordingDevice;
			}
			if (RecordingDevice == null)
			{
				ReportNoMicrophone();
				return false;
			}

			if (Recording)
			{
				Debug.WriteLine($"Recording is already true (Name = {Name})");
				return false;
			}
			else
				Debug.WriteLine($"Recording is false (Name = {Name})");

			if (RecordingExists)
			{
				try
				{
					// Can't use move because it doesn't allow overwrite
					RobustFileAddOn.Move(Path, _backupPath, true);
					Analytics.Track("Re-recorded a clip", ContextForAnalytics);
				}
				catch (IOException err)
				{
					ErrorReport.NotifyUserOfProblem(err, LocalizationManager.GetString("AudioButtonsControl.ErrorMovingExistingRecording",
						"The file with the existing recording can not be overwritten. We can't record over it at the moment. Please report this error. Restarting HearThis might solve this problem."));
					return false;
				}
			}
			else
			{
				RobustFile.Delete(_backupPath);
				Analytics.Track("Recording clip", ContextForAnalytics);
			}

			_suppressTooShortWarning = false;
			_startRecording = DateTime.Now;
			_recordButton.Waiting = true;
			Debug.WriteLine($"Calling _startRecordingTimer.Start() (Name = {Name})");
			_startRecordingTimer.Start();
			//_recordButton.ImagePressed = Resources.recordActive;
			UpdateDisplay();
			return true;
		}

		private void OnRecordUp(object sender, MouseEventArgs e)
		{
			_recordButton.Waiting = false;
			_recordButton.State = BtnState.Normal;
			ButtonHighlightMode = ButtonHighlightModes.Next;

			if (Recorder.RecordingState != RecordingState.Recording)
			{
				WarnPressTooShort();
				UpdateDisplay();
				return;
			}
			try
			{
				Debug.WriteLine("Stop recording");
				Recorder.Stop();
			}
			catch (Exception)
			{
				//swallow it review: initial reason is that they didn't hold it down long enough, could detect and give message
			}
			if (DateTime.Now - _startRecording < TimeSpan.FromMilliseconds(kMinMilliseconds))
				WarnPressTooShort();

			UpdateDisplay();
		}

		private bool BackupExists => File.Exists(_backupPath);
		private bool RecordingExists => File.Exists(Path);

		private string RecordingTooShortMessage => LocalizationManager.GetString("AudioButtonsControl.PleaseHold",
			"Hold down the record button (or the space bar) while talking, and only let it go when you're done.",
			"Appears when the button is pressed very briefly");

		private void WarnPressTooShort()
		{
			if (!_suppressTooShortWarning)
			{
				_suppressTooShortWarning = true;
				MessageBox.Show(this, RecordingTooShortMessage,
					LocalizationManager.GetString("AudioButtonsControl.PressToRecord", "Press to record", "Caption for HoldButtonHint message"));
			}

			// If we had a prior recording, restore it...button press may have been a mistake.
			AttemptRestoreFromBackup();
		}

		private void AttemptRestoreFromBackup()
		{
			try
			{
				if (BackupExists)
					RobustFileAddOn.Move(_backupPath, Path, true);
			}
			catch (IOException)
			{
				// if we can't restore it, we can't. Review: are there other exception types we should ignore? Should we bother the user?
			}
		}

		private void ReportSuccessfulRecordingAnalytics()
		{
			var properties = new Dictionary<string, string>()
				{
					{"Length", Recorder.RecordedTime.ToString()},
					{"BreakLinesAtClauses", Settings.Default.BreakLinesAtClauses.ToString()}
				};
			foreach (var property in ContextForAnalytics)
			{
				properties.Add(property.Key, property.Value);
			}
			Analytics.Track("Recorded A Line", properties);
		}

		void OnStartRecordingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_startRecordingTimer.Stop();
			if (_recordButton.State != BtnState.Pushed)
			{
				// User released the button/space key before we even got started. Don't start,
				// since there will be no reliable signal to finish.
				return;
			}
			Invoke(new Action(delegate {
				Debug.WriteLine($"Start recording (Name = {Name}; Path = {Path})");
				Recorder.BeginRecording(Path);
				_recordButton.Waiting = false;
			}));
		}

		internal void ReportNoMicrophone()
		{
			MessageBox.Show(this,
				LocalizationManager.GetString("AudioButtonsControl.NoMic", "This computer appears to have no sound recording device available. You will need one to use this program."),
				LocalizationManager.GetString("AudioButtonsControl.NoInput", "No input device"));
		}

		private void RecordAndPlayControl_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;
			if (Recorder.SelectedDevice == null)
				return; // user has no input device; we warn of this elsewhere. But don't crash here.
			Recorder.BeginMonitoring();
		}

		public void OnPlay(object sender, EventArgs e)
		{
			// This lock ensures that another thread doesn't change out the player while we're in the process
			// of updating the display and kicking off playback. This was not added in response to a specific
			// bug, so it might not be needed (application logic might prevent a non-thread-safe scenario).
			lock (this)
			{
				// _player should never be null if _playButton.Enabled, but just in case there is a race
				// condition not adequately handled by the locks, we'll re-check it here and avoid a possible
				// NullReferenceException.
				if (!_playButton.Enabled || _player == null)
					return; //could be fired by keyboard

				try
				{
					_playButton.Playing = true;

					UpdateDisplay();
					_player.Play();
					UpdateDisplay();
					Analytics.Track("Play", ContextForAnalytics);
				}
				catch (EndOfStreamException err)
				{
					ErrorReport.NotifyUserOfProblem(err,
						LocalizationManager.GetString("AudioButtonsControl.RecordingProblem", "That recording has a problem. It will now be removed, if possible."));
					try
					{
						RobustFile.Delete(_path);
					}
					catch (Exception)
					{
						ErrorReport.NotifyUserOfProblem(err,
							LocalizationManager.GetString("AudioButtonsControl.DeleteProblem", "Failed to delete problem file."));
					}
				}
				catch (Exception err)
				{
					_playButton.Playing = false; //normally, this is done in the stopped event handler
					ErrorReport.NotifyUserOfProblem(err,
						LocalizationManager.GetString("AudioButtonsControl.ReadingProblem", "There was a problem reading that file. Try again later."));
				}
			}

			UpdateDisplay();
		}

		void OnRecorder_Stopped(IAudioRecorder audioRecorder, ErrorEventArgs errorEventArgs)
		{
			Debug.WriteLine($"_recorder_Stopped: requesting begin monitoring (Name = {Name})");

			if (errorEventArgs != null)
			{
				HandleRecordingError(errorEventArgs.GetException());
			}
			else
			{
				ProcessFinishedRecording();
				UpdateDisplay();
			}
		}

		private void HandleRecordingError(Exception ex)
		{
			_suppressTooShortWarning = true;
			UpdateDisplay();

			MessageBoxButtons msgBoxButtons;
			string msg;
			// If the recording isn't long enough, we don't need to bother asking about restoring the backup.
			// In that case, we can assume the error is the cause of the short recording, so we'll just report
			// the error and restore the backup.
			if (BackupExists && RecordingExists && Recorder.RecordedTime.TotalMilliseconds >= kMinMilliseconds)
			{
				msg = ex.Message + Environment.NewLine +
					LocalizationManager.GetString("AudioButtonsControl.RecordingProblemRestoreFromBackup",
						"A backup of the previous recording is available. Would you like to restore it?");
				msgBoxButtons = MessageBoxButtons.YesNo;
			}
			else
			{
				msg = ex.Message;
				msgBoxButtons = MessageBoxButtons.OK;
			}

			if (MessageBox.Show(this, msg, ProductName, msgBoxButtons, MessageBoxIcon.Warning) == DialogResult.Yes ||
				BackupExists && !RecordingExists)
			{
				AttemptRestoreFromBackup();
			}

			_recordButton.RecordingWasAborted();

			Logger.WriteError(ex);
		}

		private void ProcessFinishedRecording()
		{
			ErrorEventArgs errorEventArgs = null;
			if (_recordButton.State == BtnState.Pushed)
			{
				// Looks like the recording exceeded the maximum length.
				// Note: I don't think Waiting could ever be true here, but if it is, it's apparently some other scenario than what we're trying to handle.
				Debug.Assert(!_recordButton.Waiting);
				_recordButton.State = BtnState.Normal;
				if (Recorder.RecordedTime.TotalMilliseconds >= 6000 * Settings.Default.MaxRecordingMinutes)
				{
					var msg = String.Format(LocalizationManager.GetString("AudioButtonsControl.MaximumRecordingLength",
							"{0} currently limits recorded clips to {1} minutes. If you need to record longer clips, please contact support.",
							"Param 0: \"HearThis\" (product name); Param 1: maximum number of minutes"),
						ProductName, Settings.Default.MaxRecordingMinutes);
					MessageBox.Show(msg,
						LocalizationManager.GetString("AudioButtonsControl.RecordingStoppedMsgCaption", "Recording Stopped",
							"Displayed as the MessageBox caption when a clip recording exceeds the maximum number of minutes allowed."));
					errorEventArgs = new ErrorEventArgs(new Exception(msg));
				}
			}

			if (Recorder.RecordedTime.TotalMilliseconds < kMinMilliseconds)
			{
				if (RecordingExists)
				{
					try
					{
						RobustFile.Delete(_path);
					}
					catch (Exception err)
					{
						ErrorReport.NotifyUserOfProblem(err,
							LocalizationManager.GetString("AudioButtonsControl.ShortRecordingProblem", "The record button wasn't down long enough, but that file is locked up, so we can't remove it. Yes, this problem will need to be fixed."));
						errorEventArgs = new ErrorEventArgs(err);
					}
				}

				if (errorEventArgs == null)
				{
					WarnPressTooShort();
					errorEventArgs = new ErrorEventArgs(new Exception(RecordingTooShortMessage));
				}

				Analytics.Track("Flubbed Record Press", new Dictionary<string, string>
					{{"Length", Recorder.RecordedTime.ToString()},});
			}
			else if (RecordingExists)
				ReportSuccessfulRecordingAnalytics();

			SoundFileRecordingComplete?.Invoke(this, errorEventArgs);
		}

		public void SpaceGoingDown()
		{
			if (!_recordButton.Enabled)
				return;
			if (_recordButton.State == BtnState.Pushed)
				return;

			Debug.WriteLine("SpaceGoingDown");

			if (TryStartRecord())
			{
				_recordButton.State = BtnState.Pushed;
				_recordButton.Invalidate();
			}
		}

		public void SpaceGoingUp()
		{
			_recordButton.State = BtnState.Normal;
			_recordButton.Invalidate();
			OnRecordUp(this, null);
		}

		public void UpdateButtonStateOnNavigate()
		{
			ButtonHighlightMode = ButtonHighlightModes.Record;//todo (or play)
			if (CanPlay) // if we already have a recording, don't encourage re-recording, encourage playing
				ButtonHighlightMode = ButtonHighlightModes.Play;
		}

		private void OnNextClick(object sender, EventArgs e)
		{
			NextClick?.Invoke(sender, e);
		}
	}

}
