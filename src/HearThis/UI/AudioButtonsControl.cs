using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HearThis.Properties;
using L10NSharp;
using NAudio.Wave;
using Palaso.Media;
using Palaso.Media.Naudio;
using Palaso.Reporting;
using Segmentio;
using Timer = System.Timers.Timer;

namespace HearThis.UI
{
	public partial class AudioButtonsControl : UserControl
	{
		private string _path;
		public AudioRecorder Recorder { get; set; }
		private Palaso.Media.ISimpleAudioSession _player;

		public enum ButtonHighlightModes {Default=0, Record, Play, Next};
		public event EventHandler NextClick;

		/// <summary>
		/// We're using this system timer rather than a normal form timer becuase with the later, when the button "captured" the mouse, the timer refused to fire.
		/// </summary>
		private System.Timers.Timer _startRecordingTimer;

		public AudioButtonsControl()
		{
			InitializeComponent();

			Recorder = new AudioRecorder(1);
			Recorder.Stopped += new EventHandler(OnRecorder_Stopped);

			Path = System.IO.Path.GetTempFileName();
			_startRecordingTimer = new Timer(300);
			_startRecordingTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnStartRecordingTimer_Elapsed);

			_recordButton.CancellableMouseDownCall = new Func<bool>(() => TryStartRecord());
		}


		public ButtonHighlightModes ButtonHighlightMode
		{
			get { return _buttonHighlightMode; }
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
			if(ButtonHighlightMode==ButtonHighlightModes.Default)
				ButtonHighlightMode = ButtonHighlightModes.Record;

			_recordButton.Enabled = HaveSomethingToRecord && CanRecordNow;
			//Console.WriteLine("record enabled: "+_recordButton.Enabled.ToString());
			_playButton.Enabled = CanPlay;
//            if (_playButton.Enabled)
//                ButtonHighlightMode = ButtonHighlightModes.Play;
//			else if(_recordButton.Enabled)
//				ButtonHighlightMode = ButtonHighlightModes.Record;

			_playButton.Playing = _player.IsPlaying;
			_playButton.Invalidate();
		}

		private bool CanPlay
		{
			get
			{
				/* this was when we were using the same object (naudio-derived) for both playback and recording (changed to irrklang 4/2013, but could go back if the playback file locking bug were fixed)
				 * return Recorder != null && Recorder.RecordingState != RecordingState.Recording && */


				return !_player.IsPlaying &&
					   !string.IsNullOrEmpty(Path) && File.Exists(Path);
			}
		}

		public bool HaveSomethingToRecord;
		private ButtonHighlightModes _buttonHighlightMode;

		public bool CanRecordNow
		{
			get
			{
				if(Recorder == null)
					return false;
				if(_player.IsPlaying)
					return false;
				if(Recorder.RecordingState == RecordingState.Monitoring || Recorder.RecordingState == RecordingState.Stopped)
					return true;
				return false;
			}
		}


		public bool Recording
		{
			get
			{
				return Recorder.RecordingState == RecordingState.Recording ||
					   Recorder.RecordingState == RecordingState.RequestedStop;
			}
		}

		public bool Playing
		{
			get { return _player.IsPlaying; }
		}

		public string Path
		{
			get { return _path; }
			set
			{
				_path = value;
				if (_player!=null)
				{
					//Palaso.media.audiosession doesn't have a dispose (as of 4/2013), but at least we can unwire ourselves from it
					((ISimpleAudioWithEvents)_player).PlaybackStopped -= AudioButtonsControl_PlaybackStopped;
				}
				_player = Palaso.Media.AudioFactory.CreateAudioSession(_path);
				((ISimpleAudioWithEvents)_player).PlaybackStopped += AudioButtonsControl_PlaybackStopped;
			}
		}

		void AudioButtonsControl_PlaybackStopped(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		public RecordingDevice RecordingDevice
		{
			get { return Recorder.SelectedDevice; }
			set { Recorder.SelectedDevice = value; }
		}

		// Use this after we are running (after BeginMonitoring, or when first mic plugged in)
		public void SwitchRecordingDevice(RecordingDevice device)
		{
			Recorder.SwitchDevice(device);
		}

		public bool CanGoNext
		{
			set { _nextButton.Enabled = value;

			}
		}

		public Segmentio.Model.Properties ContextForAnalytics;

//
//        private void _playButton_Click(object sender, EventArgs e)
//        {
//            var session = AudioFactory.AudioSession(Path);
//            session.Play();
//        }

		private DateTime _startRecording;

		/// <summary>
		/// Start the recording
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns>true if the recording started successfully</returns>
		private bool TryStartRecord()
		{
			if (Recorder.RecordingState == RecordingState.RequestedStop)
			{
				MessageBox.Show(
					LocalizationManager.GetString("AudioButtonsControl.BadState",
						"HearThis is in an unusual state, possibly caused by unplugging a microphone. You will need to restart."),
					LocalizationManager.GetString("AudioButtonsControl.BadStateCaption", "Cannot record"));
			}
			if (!_recordButton.Enabled)
				return false; //could be fired by keyboard

			// If someone unplugged the microphone we were planning to use switch to another.
			if (!RecordingDevice.Devices.Contains(RecordingDevice))
			{
				RecordingDevice = RecordingDevice.Devices.FirstOrDefault();
			}
			if (RecordingDevice == null)
			{
				ReportNoMicrophone();
				return false;
			}

			if (Recording)
				return false;

			if (File.Exists(Path))
			{
				try
				{
					File.Delete(Path);
					try
					{
						Analytics.Client.Track(Settings.Default.IdForAnalytics, "Re-recorded a Line", ContextForAnalytics);
					}
					catch (Exception)
					{

						throw;
					}
				}
				catch (Exception err)
				{
					ErrorReport.NotifyUserOfProblem(err,
													"Sigh. The old copy of that file is locked up, so we can't record over it at the moment. Yes, this problem will need to be fixed.");
					return false;
				}
				UsageReporter.SendNavigationNotice("ReRecord");
			}
			else
			{
				UsageReporter.SendNavigationNotice("Record");
			}
			_startRecording = DateTime.Now;
			//_startDelayTimer.Enabled = true;
			//_startDelayTimer.Start();
			_startRecordingTimer.Start();
			//_recordButton.ImagePressed = Resources.recordActive;
			_recordButton.Waiting = true;
			UpdateDisplay();
			return true;
		}

		private void OnRecordUp(object sender, MouseEventArgs e)
		{
			//_recordButton.ImagePressed = Resources.recordActive;
			_recordButton.Waiting = false;
			_recordButton.State = BtnState.Normal;
			ButtonHighlightMode = ButtonHighlightModes.Next;

			Debug.WriteLine("changing press image back to red");

			if (Recorder.RecordingState != RecordingState.Recording)
			{
				WarnPressTooShort();
				return;
			}
			try
			{
				Debug.WriteLine("Stop recording");
				 Recorder.Stop(); //.StopRecordingAndSaveAsWav();
				ReportSuccessfulRecordingAnalytics();
			}
			catch (Exception)
			{
				//swallow it review: initial reason is that they didn't hold it down long enough, could detect and give message
			}
			if (DateTime.Now - _startRecording < TimeSpan.FromSeconds(0.5))
				WarnPressTooShort();

			UpdateDisplay();
		}

		private void WarnPressTooShort()
		{
			MessageBox.Show(this, LocalizationManager.GetString("AudioButtonsControl.PleaseHold",
				"Please hold the record button down until you have finished recording", "Appears when the button is pressed very briefly"),
				 LocalizationManager.GetString("AudioButtonsControl.PressToRecord","Press to record", "Caption for PleaseHold message"));
		}

		private void ReportSuccessfulRecordingAnalytics()
		{
			var properties = new Segmentio.Model.Properties()
				{
					{"Length", Recorder.RecordedTime},
				};
			foreach (var property in ContextForAnalytics)
			{
				properties.Add(property.Key, property.Value);
			}
			Analytics.Client.Track(Settings.Default.IdForAnalytics, "Recorded A Line", properties);
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
				Debug.WriteLine("Start recording");
				Recorder.BeginRecording(Path);
			   // _recordButton.ImagePressed = Resources.recordActive1;
				_recordButton.Waiting = false;

			}));
		}

		internal void ReportNoMicrophone()
		{
			MessageBox.Show(this,
				LocalizationManager.GetString("AudioButtonsControl.NoMic", "This computer appears to have no sound recording device available. You will need one to use this program."),
				LocalizationManager.GetString("AudioButtonsControl.NoInput", "No input device"));
		}

		private void OnStartDelayTimerTick(object sender, EventArgs e)
		{
			_startRecordingTimer.Stop();
			Debug.WriteLine("Start recording");
			Recorder.BeginRecording(Path);

			_recordButton.Waiting = false;
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
			if (!_playButton.Enabled)
				return; //could be fired by keyboard

			try
			{
				_playButton.Playing = true;

				UpdateDisplay();
				_player.Play();
				UpdateDisplay();
				//_updateDisplayTimer.Enabled = true;//because the irrklang-based player has no events to tell us when it's done. It will evntually turn the play and record buttons back on
				UsageReporter.SendNavigationNotice("Play");
			}
			catch (EndOfStreamException err)
			{
				 ErrorReport.NotifyUserOfProblem(err,
								LocalizationManager.GetString("AudioButtonsControl.RecordingProblem","Sigh. That recording has a problem. It will now be removed, if possible."));
				try
				{
					File.Delete(_path);
				}
				catch (Exception)
				{
					ErrorReport.NotifyUserOfProblem(err,
								   LocalizationManager.GetString("AudioButtonsControl.DeleteProblem","Nope, couldn't delete it."));
				}

			}
			catch(Exception err)
			{
				_playButton.Playing = false; //normally, this is done in the stopped event handler
				ErrorReport.NotifyUserOfProblem(err,
								LocalizationManager.GetString("AudioButtonsControl.ReadingProblem", "Sigh. There was a problem reading that file. Try again later."));
			}
			UpdateDisplay();
		}

		void OnRecorder_Stopped(object sender, EventArgs e)
		{
			Debug.WriteLine("_recorder_Stopped: requesting begin monitoring");
			if (Recorder.RecordedTime.TotalMilliseconds < 500)
			{
				if (File.Exists(_path))
				{
					try
					{
						File.Delete(_path);
					}
					catch (Exception err)
					{
						ErrorReport.NotifyUserOfProblem(err,
														LocalizationManager.GetString("AudioButtonsControl.ShortRecordingProblem", "The record button wasn't down long engough, but that file is locked up, so we can't remove it. Yes, this problem will need to be fixed."));
					}
				}
				//_hint.Text = "Hold down the record button while talking.";
				MessageBox.Show(LocalizationManager.GetString("AudioButtonsControl.HoldButtonHint", "Hold down the record button (or the space bar) while talking, and only let it go when you're done."));

				try
				{
					Analytics.Client.Track(Settings.Default.IdForAnalytics, "Flubbed Record Press", new Segmentio.Model.Properties() {
							{ "Length", Recorder.RecordedTime },
							});
				}
				catch (Exception)
				{
				}
			}
			//_recorder.BeginMonitoring(0);
			UpdateDisplay();
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
			if(CanPlay) // if we already have a recording, don't encourage re-recording, encourage playing
				ButtonHighlightMode = ButtonHighlightModes.Play;
		}

		private void OnNextClick(object sender, EventArgs e)
		{
			if(NextClick !=null)
				NextClick(sender, e);
		}
	}

}
