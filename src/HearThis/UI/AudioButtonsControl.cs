// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
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
using static System.String;
using static System.Windows.Forms.DialogResult;
using static HearThis.SafeSettings;
using static System.Windows.Forms.MessageBoxIcon;
using static HearThis.Program;
using Timer = System.Timers.Timer;

namespace HearThis.UI
{
	public partial class AudioButtonsControl : UserControl
	{
		/// <summary>
		/// The one and only recorder shared among all <see cref="AudioButtonsControl"/>
		/// instances used in HearThis. This prevents interference when recording using
		/// one control while other controls remain in existence (with recorders listening
		/// on the same device).
		/// </summary>
		/// <remarks>See HT-402</remarks>
		public static AudioRecorder Recorder { get; }

		private string _path;
		private ISimpleAudioSession _player;

		public enum ButtonHighlightModes {Default=0, Record, Play, Next, SkipRecording};
		public event EventHandler NextClick;
		public delegate void RecordingEventHandler(AudioButtonsControl sender, bool error);
		public event RecordingEventHandler SoundFileRecordingComplete;
		public event CancelEventHandler RecordingStarting;
		public delegate void ButtonStateChangedHandler(object sender, BtnState newState);
		public event ButtonStateChangedHandler RecordButtonStateChanged;
		public event ButtonStateChangedHandler PlayButtonStateChanged;
		public event EventHandler RecordingAttemptAbortedBecauseOfNoMic;

		private readonly string _backupPath;
		private DateTime _startRecording;
		private bool _suppressTooShortWarning;
		private const int kMinMilliseconds = 500;
		private bool _showKeyboardShortcutsInTooltips = true;

		/// <summary>
		/// We're using this system timer rather than a normal form timer because with the latter, when the button "captured" the mouse, the timer refused to fire.
		/// </summary>
		private readonly Timer _startRecordingTimer;

		private static readonly int MaxRecordingMinutes = Get(() => Settings.Default.MaxRecordingMinutes);

		static AudioButtonsControl()
		{
			Recorder = new AudioRecorder(MaxRecordingMinutes);
			Recorder.SelectedDevice = RecordingDevice.Devices.FirstOrDefault() as RecordingDevice;
			if (Recorder.SelectedDevice != null)
				Recorder.BeginMonitoring();
		}

		public AudioButtonsControl()
		{
			InitializeComponent();

			if (DesignMode)
				Path = System.IO.Path.GetTempFileName();
			_startRecordingTimer = new Timer(300);
			_startRecordingTimer.Elapsed += OnStartRecordingTimer_Elapsed;

			_recordButton.CancellableMouseDownCall = TryStartRecord;
			_recordButton.ButtonStateChanged += OnRecordButtonStateChanged;
			_playButton.ButtonStateChanged += OnPlayButtonStateChanged;
			_backupPath = System.IO.Path.GetTempFileName();
		}
		
		protected bool ReallyDesignMode => (DesignMode || GetService(typeof (IDesignerHost)) != null) ||
			(LicenseManager.UsageMode == LicenseUsageMode.Designtime);

		private void OnRecordButtonStateChanged(object sender, EventArgs args)
		{
			RecordButtonStateChanged?.Invoke(this, _recordButton.State);
		}

		private void OnPlayButtonStateChanged(object sender, EventArgs args)
		{
			PlayButtonStateChanged?.Invoke(this, _recordButton.State);
		}

		public bool IsPlaying
		{
			get
			{
				lock(_player)
					return _player.IsPlaying;
			}
		}

		public bool ShowRecordButton
		{
			get => _recordButton.Visible;
			set => _recordButton.Visible = value;
		}

		public bool ShowNextButton
		{
			get => _nextButton.Visible;
			set => _nextButton.Visible = value;
		}

		public void SimulateMouseOverPlayButton(bool mouseOver = true)
		{
			if (mouseOver && _playButton.State == BtnState.Normal)
				_playButton.State = BtnState.MouseOver;
			else if (!mouseOver && _playButton.State == BtnState.MouseOver)
				_playButton.State = BtnState.Normal;
		}

		[DefaultValue(true)]
		public bool ShowKeyboardShortcutsInTooltips
		{
			get => _showKeyboardShortcutsInTooltips;
			set
			{
				if (_showKeyboardShortcutsInTooltips == value)
					return;
				_showKeyboardShortcutsInTooltips = value;
				UpdatePlayAndRecordButtonToolTips();
			}
		}

		public ButtonHighlightModes ButtonHighlightMode
		{
			get => _buttonHighlightMode;
			set
			{
				_recordButton.Blocked = false;

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
						_recordButton.IsDefault = ShowRecordButton;
						_nextButton.IsDefault = false;
						break;
					case ButtonHighlightModes.SkipRecording:
						_recordButton.Blocked = true;
						goto case ButtonHighlightModes.Next;
					case ButtonHighlightModes.Next:
						_playButton.IsDefault = false;
						_recordButton.IsDefault = false;
						_nextButton.IsDefault = ShowNextButton;
						break;
					default:
						_playButton.IsDefault = false;
						_recordButton.IsDefault = false;
						_nextButton.IsDefault = false;
						break;
				}

				UpdatePlayAndRecordButtonToolTips();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string AlternateRecordButtonBaseToolTip { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string AlternatePlayButtonBaseToolTip { get; set; }

		private void UpdatePlayAndRecordButtonToolTips()
		{
			string AppendHowTo(string howTo, string s)
			{
				if (howTo.StartsWith("_"))
					howTo = howTo.Substring(1);
				else
					s += " ";

				s += howTo;
				return s;
			}

			string tooltip;

			if (_buttonHighlightMode == ButtonHighlightModes.SkipRecording)
				tooltip = LocalizationManager.GetString(
					"AudioButtonsControl.RecordButton.ToolTip_Skip", "Skipped block - Do not record",
					"Appears as tool tip on the record button when the current block is skipped.");
			else
			{
				tooltip = AlternateRecordButtonBaseToolTip ?? LocalizationManager.GetString(
					"AudioButtonsControl.RecordButton_ToolTip_Base",
					"Record this block.");
				string howTo;
				if (ShowKeyboardShortcutsInTooltips)
					howTo = LocalizationManager.GetString(
						"AudioButtonsControl.RecordButton_ToolTip_HowToMouseAndKeyboardShortcut",
						"(Press and hold the mouse or space bar.)",
						"This will be appended to the base tooltip (default or alternate). A " +
						"space will normally be added to separate these two. For languages " +
						"(e.g., scriptio continua languages) where a space would be " +
						"inappropriate, you can suppress the space by adding an underscore " +
						"character to the start of this string.");
				else
					howTo = LocalizationManager.GetString(
						"AudioButtonsControl.RecordButton_ToolTip_HowToMouseOnly",
						"(Press and hold the mouse.)",
						"This will be appended to the base tooltip (default or alternate). A " +
						"space will normally be added to separate these two. For languages " +
						"(e.g., scriptio continua languages) where a space would be " +
						"inappropriate, you can suppress the space by adding an underscore " +
						"character to the start of this string.");
				tooltip = AppendHowTo(howTo, tooltip);
			}

			toolTip1.SetToolTip(_recordButton, tooltip);

			tooltip = AlternatePlayButtonBaseToolTip ?? LocalizationManager.GetString("AudioButtonsControl.PlayButton_ToolTip_Base",
				"Play the clip for this block");

			if (ShowKeyboardShortcutsInTooltips)
			{
				string howTo = LocalizationManager.GetString("AudioButtonsControl.PlayButton_ToolTip_HowTo",
					"(Tab key)",
					"This will be appended to the base tooltip (default or alternate). A " +
					"space will normally be added to separate these two. For languages " +
					"(e.g., scriptio continua languages) where a space would be " +
					"inappropriate, you can suppress the space by adding an underscore " +
					"character to the start of this string.");
				if (howTo.StartsWith("_"))
					howTo = howTo.Substring(1);
				else
					tooltip += " ";
				tooltip += howTo;
			}
			toolTip1.SetToolTip(_playButton, tooltip);
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
				Debug.WriteLine("Recorder.RecordingState = " + Recorder.RecordingState);
				var canRecordNow = !isPlaying && Recorder.RecordingState == RecordingState.Monitoring || Recorder.RecordingState == RecordingState.Stopped;
				var canRecord = HaveSomethingToRecord && canRecordNow;

				if (ButtonHighlightMode == ButtonHighlightModes.Default)
					ButtonHighlightMode = ShowRecordButton ? ButtonHighlightModes.Record : ButtonHighlightModes.Play;

				_recordButton.Enabled = canRecord;
				Debug.WriteLine($"{Name} record enabled: {_recordButton.Enabled}");
				_recordButton.Invalidate();
				_playButton.Enabled = canPlay;

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
					/* this was when we were using the same object (NAudio-derived) for both playback and recording
					 * (changed to irrklang 4/2013, but could go back if the playback file locking bug were fixed)
					 * return Recorder != null && Recorder.RecordingState != RecordingState.Recording && */
					return _player != null && !_player.IsPlaying && !IsNullOrEmpty(Path) &&
					       ValidRecordingExists;
				}
			}
		}

		public bool HaveSomethingToRecord { get; set; }

		private ButtonHighlightModes _buttonHighlightMode;

		public bool Recording => Recorder.IsRecording;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Path
		{
			get => _path;
			set
			{
				lock (this) // Don't want another thread checking _player while we're swapping it out.
				{
					_path = value;

					if (ReallyDesignMode)
						return;

					if (Recorder.IsRecording)
					{
						// What we really want is to abort, but there doesn't seem to be a good way
						// to do that. In practice, though, this should be fine.
						Recorder.Stop();
					}

					DisposePlayer();
					if (Visible && IsHandleCreated)
						SetUpPlayer();
				}
			}
		}

		private void SetUpPlayer()
		{
			lock (this) // Don't want another thread checking _player while we're swapping it out.
			{
				if (_player?.FilePath == _path)
					return; // Visible is true the first time before OnVisibleChanged gets called.
				Debug.Assert(_player == null, "Failed to call DisposePlayer before setting it up.");
				if (!IsNullOrEmpty(_path))
				{
					_player = Utils.GetPlayer(FindForm(), _path);
					if (_player is ISimpleAudioWithEvents simpleAudioWithEvents)
						simpleAudioWithEvents.PlaybackStopped += AudioButtonsControl_PlaybackStopped;
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

		protected override void OnVisibleChanged(EventArgs e)
		{
			lock (this)
			{
				base.OnVisibleChanged(e);

				if (!Visible)
				{
					DisposePlayer();
				}
				else
				{
					SetUpPlayer();
					UpdateDisplay();
				}
			}
		}

		void AudioButtonsControl_PlaybackStopped(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		private static RecordingDevice RecordingDevice =>
			Recorder.SelectedDevice as RecordingDevice;

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

			// If someone unplugged the microphone we were planning to use, switch to another.
			if (!RecordingDevice.Devices.Contains(RecordingDevice))
				Recorder.SelectedDevice = RecordingDevice.Devices.FirstOrDefault() as RecordingDevice;

			if (RecordingDevice == null)
			{
				RecordingAttemptAbortedBecauseOfNoMic?.Invoke(this, EventArgs.Empty);
				return false;
			}

			if (Recording)
			{
				Logger.WriteEvent($"Recording is already true (Name = {Name})");
				return false;
			}

			Debug.WriteLine($"Recording is false (Name = {Name})");

			if (ClipFileExists)
			{
				try
				{
					RobustFile.Move(Path, _backupPath, true);
				}
				catch (Exception err)
				{
					ErrorReport.NotifyUserOfProblem(err,
						LocalizationManager.GetString(
						"AudioButtonsControl.ErrorMovingExistingRecording",
						"The existing recording file cannot be moved or overwritten:") +
						Environment.NewLine + Path + Environment.NewLine +
						ManualFileDeletionInstructionsFmt, kProduct);
					return false;
				}
				Analytics.Track("Re-recorded a clip", ContextForAnalytics);
			}
			else
			{
				try
				{
					RobustFile.Delete(_backupPath);
				}
				catch (Exception e)
				{
					Logger.WriteError(e);
					ErrorReport.NotifyUserOfProblem(new ShowOncePerSessionBasedOnExactMessagePolicy(),
						e, LocalizationManager.GetString(
						"AudioButtonsControl.ErrorDeletingClipFileBackup",
						"The backup file for the clip you are about to record could not be " +
						"deleted. Although this will not prevent you from attempting to record " +
						"the clip now, it could pose a problem later."));
				}
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
			ButtonHighlightMode = ShowNextButton ? ButtonHighlightModes.Next : ButtonHighlightModes.Play;

			if (Recorder.RecordingState != RecordingState.Recording)
			{
				Recorder.Stopped -= OnRecorder_Stopped;
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
		private bool ClipFileExists => File.Exists(Path);
		private bool ValidRecordingExists
		{
			get
			{
				if (!ClipFileExists)
					return false;
				try
				{
					return new FileInfo(Path).Length > 0;
				}
				catch (Exception e)
				{
					Logger.WriteError(e);
					return false;
				}
			}
		}

		private string RecordingTooShortMessage => LocalizationManager.GetString("AudioButtonsControl.PleaseHold",
			"Hold down the record button (or the space bar) while talking, and only let it go when you're done.",
			"Appears when the button is pressed very briefly");

		private void WarnPressTooShort()
		{
			Logger.WriteEvent($"Button press too short (Name = {Name})");
			if (!_suppressTooShortWarning)
			{
				_suppressTooShortWarning = true;
				MessageBox.Show(this, RecordingTooShortMessage,
					LocalizationManager.GetString("AudioButtonsControl.PressToRecord",
						"Press to record", "Caption for HoldButtonHint message"));
			}

			// If we had a prior recording, restore it...button press may have been a mistake.
			AttemptRestoreFromBackup();
		}

		private void AttemptRestoreFromBackup()
		{
			try
			{
				if (BackupExists)
					RobustFile.Move(_backupPath, Path, true);
			}
			catch (Exception e)
			{
				// If we can't restore it, we can't.
				Logger.WriteError(e);
				Analytics.ReportException(e);
				// REVIEW: Should we bother the user?
			}
		}

		private void ReportSuccessfulRecordingAnalytics()
		{
			var properties = new Dictionary<string, string>
				{
					{"Length", Recorder.RecordedTime.ToString()},
					{"BreakLinesAtClauses", Get(() => Settings.Default.BreakLinesAtClauses.ToString())}
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

			Recorder.Stopped += OnRecorder_Stopped;

			Invoke(new Action(delegate {
				Logger.WriteEvent($"Start recording (Name = {Name}; Path = {Path}) using" +
					$"{RecordingDevice?.ProductName} (Id:{RecordingDevice?.Id})");
				Recorder.BeginRecording(Path);
				_recordButton.Waiting = false;
			}));
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
				if (!_playButton.Enabled || !_playButton.Visible || _player == null)
					return; //could be fired by keyboard

				// Avoid confusion by stopping playback of any other controls (with the same
				// parent). This is really for the benefit of the Record In Parts dialog.
				foreach (var ctrl in Parent.Controls)
				{
					if (ctrl is AudioButtonsControl audioButtonsControl && ctrl != this)
						audioButtonsControl.StopPlaying();
				}

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
					var clipNum = System.IO.Path.GetFileNameWithoutExtension(Path);
					var folder = System.IO.Path.GetDirectoryName(Path);
					var chapter = System.IO.Path.GetFileName(folder);
					folder = System.IO.Path.GetDirectoryName(folder);
					var book = System.IO.Path.GetFileName(folder);
					ErrorReport.NotifyUserOfProblem(err,
						LocalizationManager.GetString("AudioButtonsControl.PlaybackProblem",
							"There was a problem playing clip {0} (block {1}) for {2} {3}.",
							"Param 0: clip file number; " +
							"Param 1: block number; " +
							"Param 2: Scripture book name; " +
							"Param 3: chapter number"), clipNum, int.Parse(clipNum) + 1, book, chapter);
				}
			}

			UpdateDisplay();
		}

		/// <summary>
		/// Stops playback or recording of the associated file. This frees up the file to be
		/// overwritten or deleted if so desired. It can also be used to interrupt playback
		/// of a file so another file can be played or recorded without interference.
		/// </summary>
		public void StopPlaying()
		{
			lock (this)
			{
				if (_player == null)
					return;
				if (_player.IsPlaying)
					_player.StopPlaying();
				else if (_player.IsRecording)
				{
					Debug.Fail("We are not using this player to do recording, so we should not be " +
						"able to get into this state!");
					_player.StopRecordingAndSaveAsWav();
				}
			}
		}

		private void OnRecorder_Stopped(IAudioRecorder audioRecorder, ErrorEventArgs errorEventArgs)
		{
			Recorder.Stopped -= OnRecorder_Stopped;

			Logger.WriteEvent($"_recorder_Stopped: (Name = {Name})");

			if (errorEventArgs != null)
				HandleRecordingError(errorEventArgs.GetException());
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
			if (BackupExists && ValidRecordingExists && Recorder.RecordedTime.TotalMilliseconds >= kMinMilliseconds)
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

			if (MessageBox.Show(this, msg, ProductName, msgBoxButtons, Warning) == Yes ||
				!ValidRecordingExists)
			{
				AttemptRestoreFromBackup();
			}

			_recordButton.RecordingWasAborted();

			Logger.WriteError(ex);
		}

		private void ProcessFinishedRecording()
		{
			var error = false;
			if (_recordButton.State == BtnState.Pushed)
			{
				// Looks like the recording exceeded the maximum length.
				// Note: I don't think Waiting could ever be true here, but if it is, it's apparently some other scenario than what we're trying to handle.
				Debug.Assert(!_recordButton.Waiting);
				_recordButton.State = BtnState.Normal;
				if (Recorder.RecordedTime.TotalMilliseconds >= 6000 * MaxRecordingMinutes)
				{
					var msg = Format(LocalizationManager.GetString(
							"AudioButtonsControl.MaximumRecordingLength",
							"{0} currently limits clips to {1} minutes. If you need to record " +
							"longer clips, please contact support.",
							"Param 0: \"HearThis\" (product name); " +
							"Param 1: maximum number of minutes"),
						ProductName, MaxRecordingMinutes);
					MessageBox.Show(msg,
						LocalizationManager.GetString(
							"AudioButtonsControl.RecordingStoppedMsgCaption",
							"Recording Stopped",
							"MessageBox caption when a clip exceeds the maximum number of minutes allowed."));
					Analytics.Track("Recording Exceeded Max Time", new Dictionary<string, string>
						{{"Length", Recorder.RecordedTime.ToString()},});
					error = true;
				}
			}

			if (Recorder.RecordedTime.TotalMilliseconds < kMinMilliseconds)
			{
				WarnPressTooShort();
				error = true;
				
				lock (this)
				{
					try
					{
						RobustFile.Delete(Path);
					}
					catch (Exception err)
					{
						ErrorReport.NotifyUserOfProblem(err,
							FileDeleteFailedWithCleanupInstructions);
					}
				}

				Analytics.Track("Flubbed Record Press", new Dictionary<string, string>
					{{"Length", Recorder.RecordedTime.ToString()},});
			}
			else if (ValidRecordingExists)
				ReportSuccessfulRecordingAnalytics();
			else
			{
				HandleInvalidRecording();
				error = true;
			}

			SoundFileRecordingComplete?.Invoke(this, error);
		}

		private string FileDeleteFailedWithCleanupInstructions =>
			Format(LocalizationManager.GetString("AudioButtonsControl.UnableToDeleteClip",
				"{0} was unable to delete the file:",
				"Param is \"HearThis\" (product name)") +
			    Environment.NewLine + Path + Environment.NewLine +
			    ManualFileDeletionInstructionsFmt, kProduct);

		internal static string ManualFileDeletionInstructionsFmt =>
			LocalizationManager.GetString("AudioButtonsControl.ManualFileDeletionInstructions",
				"If the file is locked/open (which is likely), you might need to restart {0} or " +
				"your computer. You can also try to delete it yourself when {0} is not running. " +
				"If the problem persists, please contact support.",
				"Param is \"HearThis\" (product name)");

		private void HandleInvalidRecording()
		{
			Dictionary<string, string> additionalProperties;
			try
			{
				additionalProperties = new Dictionary<string, string>
				{
					{"SelectedDevice", Recorder.SelectedDevice.ToString()},
					{"RecordingState", Recorder.RecordingState.ToString()}
				};
			}
			catch (Exception e)
			{
				Logger.WriteError(e);
				// Oh, well. we'll just track the event without details.
				additionalProperties = new Dictionary<string, string>
				{
					{"ErrorGatheringDiagnostics", e.Message}
				};
			}

			Analytics.Track("Invalid Recording", additionalProperties);
			
			// Probably not strictly necessary to obtain this lock, but it ensures that Path does
			// not change while trying to delete it and report the problem.
			lock (this) 
			{
				var msg = LocalizationManager.GetString(
					"AudioButtonsControl.InvalidRecording",
					"The clip you just recorded could not be saved properly. {0} If you " +
					"continue to see this error, it might indicate a problem with your " +
					"hardware (e.g., a faulty mic) or the audio driver you are using to record.",
					"Param 0: An embedded message with details about whether or not the invalid " +
					"clip was deleted.");
				try
				{
					RobustFile.Delete(Path);
				}
				catch (Exception e)
				{
					ErrorReport.NotifyUserOfProblem(e, msg,
						FileDeleteFailedWithCleanupInstructions);
					return;
				}

				ErrorReport.NotifyUserOfProblem(msg,
					LocalizationManager.GetString("AudioButtonsControl.ClipDeleted",
						"It has been discarded, and you will need to try again.",
						"This is a message that provides additional details and will be " +
						"embedded (as param 0) within the message " +
						"`AudioButtonsControl.InvalidRecording`. It should be localized so that " +
						"it fits naturally within that context."));
			}
		}

		public void SpaceGoingDown()
		{
			if (!_recordButton.Enabled)
				return;
			if (_recordButton.State == BtnState.Pushed)
				return;

			Logger.WriteEvent("SpaceGoingDown for " + Name);

			if (TryStartRecord())
			{
				_recordButton.State = BtnState.Pushed;
				_recordButton.Invalidate();
			}
		}

		public void SpaceGoingUp()
		{
			if (!_recordButton.Enabled)
				return;

			Logger.WriteEvent("SpaceGoingUp for " + Name);

			_recordButton.State = BtnState.Normal;
			_recordButton.Invalidate();
			OnRecordUp(this, null);
		}

		public void UpdateButtonStateOnNavigate()
		{
			// if we already have a clip recorded, don't encourage re-recording, encourage playing
			ButtonHighlightMode = CanPlay ? ButtonHighlightModes.Play : ButtonHighlightModes.Record;
		}

		private void OnNextClick(object sender, EventArgs e)
		{
			NextClick?.Invoke(sender, e);
		}
	}
}
