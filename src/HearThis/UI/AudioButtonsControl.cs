using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;
using Palaso.Media.Naudio;
using Palaso.Reporting;
using Timer = System.Timers.Timer;

namespace HearThis.UI
{
	public partial class AudioButtonsControl : UserControl
	{
		private string _path;
		public AudioRecorder Recorder { get; set; }
		private AudioPlayer _player;

		public enum ButtonHighlightModes {Default=0, Record, Play, Next};
		public event EventHandler NextClick;

		/// <summary>
		/// We're using this system timer rather than a normal form timer becuase with the later, when the button "captured" the mouse, the timer refused to fire.
		/// </summary>
		private System.Timers.Timer _timer;

		public AudioButtonsControl()
		{
			InitializeComponent();

			Recorder = new AudioRecorder();
			Recorder.Stopped += new EventHandler(_recorder_Stopped);

			_player = new AudioPlayer();
			_player.Stopped += new EventHandler(_player_Stopped);

			Path = System.IO.Path.GetTempFileName();
			_timer = new Timer(300);
			_timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);

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
			_playButton.Enabled = CanPlay;
			if (_playButton.Enabled)
				ButtonHighlightMode = ButtonHighlightModes.Play;

			_playButton.Playing = _player.PlaybackState == PlaybackState.Playing;
			_playButton.Invalidate();
		}

		private bool CanPlay
		{
			get
			{
				return Recorder != null && Recorder.RecordingState != RecordingState.Recording &&
					   !string.IsNullOrEmpty(Path) && File.Exists(Path);
			}
		}

		public bool HaveSomethingToRecord;
		private ButtonHighlightModes _buttonHighlightMode;

		public bool CanRecordNow
		{
			get { return Recorder != null && (_player.PlaybackState==PlaybackState.Stopped && (Recorder.RecordingState == RecordingState.Monitoring || Recorder.RecordingState == RecordingState.Stopped)); }
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
			get { return _player.PlaybackState == PlaybackState.Playing; }
		}

		public string Path
		{
			get { return _path; }
			set
			{
				_path = value;
			}
		}

		public RecordingDevice RecordingDevice
		{
			get { return Recorder.SelectedDevice; }
			set { Recorder.SelectedDevice = value; }
		}

		public bool CanGoNext
		{
			set { _nextButton.Enabled = value; }
		}

//
//        private void _playButton_Click(object sender, EventArgs e)
//        {
//            var session = AudioFactory.AudioSession(Path);
//            session.Play();
//        }


		private void OnRecordDown(object sender, MouseEventArgs e)
		{
			if (!_recordButton.Enabled)
				return; //could be fired by keyboard

			if (Recording)
				return;

			if (File.Exists(Path))
			{
				try
				{
					File.Delete(Path);
				}
				catch (Exception err)
				{
					ErrorReport.NotifyUserOfProblem(err,
													"Sigh. The old copy of that file is locked up, so we can't record over it at the moment. Yes, this problem will need to be fixed.");
					return;
				}
				UsageReporter.SendNavigationNotice("ReRecord");
			}
			else
			{
				UsageReporter.SendNavigationNotice("Record");
			}
			//_startDelayTimer.Enabled = true;
			//_startDelayTimer.Start();
			_timer.Start();
			//_recordButton.ImagePressed = Resources.recordActive;
			_recordButton.Waiting = true;
			UpdateDisplay();
		}

		private void OnRecordUp(object sender, MouseEventArgs e)
		{
			//_recordButton.ImagePressed = Resources.recordActive;
			_recordButton.Waiting = false;
			_recordButton.State = BtnState.Normal;
			ButtonHighlightMode = ButtonHighlightModes.Next;

			Debug.WriteLine("changing press image back to red");

			if (Recorder.RecordingState != RecordingState.Recording)
				return;
			try
			{
				Debug.WriteLine("Stop recording");
				Recorder.Stop(); //.StopRecordingAndSaveAsWav();
			}
			catch (Exception)
			{
				//swallow it review: initial reason is that they didn't hold it down long enough, could detect and give message
			}

			UpdateDisplay();
		}

		void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_timer.Stop();
			Invoke(new Action(delegate {
				Debug.WriteLine("Start recording");
				Recorder.BeginRecording(Path);
			   // _recordButton.ImagePressed = Resources.recordActive1;
				_recordButton.Waiting = false;

			}));
		}

		private void OnStartDelayTimerTick(object sender, EventArgs e)
		{
			_timer.Stop();
			Debug.WriteLine("Start recording");
			Recorder.BeginRecording(Path);

			_recordButton.Waiting = false;
		}

		private void RecordAndPlayControl_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;

			Recorder.BeginMonitoring();
		}



		public void OnPlay(object sender, EventArgs e)
		{
			if (!_playButton.Enabled)
				return; //could be fired by keyboard

			try
			{
				_playButton.Playing = true;

				_player.LoadFile(_path);

				UpdateDisplay();
				_player.Play();
				UpdateDisplay();
				UsageReporter.SendNavigationNotice("Play");
			}
			catch (EndOfStreamException err)
			{
				 ErrorReport.NotifyUserOfProblem(err,
								"Sigh. That recording has a problem. It will now be removed, if possible.");
				try
				{
					File.Delete(_path);
				}
				catch (Exception)
				{
					ErrorReport.NotifyUserOfProblem(err,
								   "Nope, couldn't delete it.");
				}

			}
			catch(Exception err)
			{
				_playButton.Playing = false; //normally, this is done in the stopped event handler
				ErrorReport.NotifyUserOfProblem(err,
								"Sigh. There was a problem reading that file. Try again later.");
			}
			UpdateDisplay();
		}
		void _player_Stopped(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		void _recorder_Stopped(object sender, EventArgs e)
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
														"The record button wasn't down long engough, but that file is locked up, so we can't remove it. Yes, this problem will need to be fixed.");
					}
				}
				//_hint.Text = "Hold down the record button while talking.";
				MessageBox.Show("Hold down the record button (or the space bar) while talking, and only let it go when you're done.");
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

			_recordButton.State = BtnState.Pushed;
			_recordButton.Invalidate();
			OnRecordDown(this, null);
		}

		public void SpaceGoingUp()
		{
			_recordButton.State = BtnState.Normal;
			_recordButton.Invalidate();
			OnRecordUp(this, null);
		}

		private void OnNextClick(object sender, EventArgs e)
		{
			ButtonHighlightMode = ButtonHighlightModes.Record;//todo (or play)

			if(NextClick !=null)
				NextClick(sender, e);

			if(CanPlay) // if we already have a recording, don't encourage re-recording, encourage playing
				ButtonHighlightMode = ButtonHighlightModes.Play;
		}
	}

}
