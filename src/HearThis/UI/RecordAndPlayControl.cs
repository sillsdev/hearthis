using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using HearThis.Audio;
using NAudio.Wave;
using Palaso.Reporting;

namespace HearThis.UI
{
	public partial class RecordAndPlayControl : UserControl
	{
		private string _path;
		private AudioRecorder _recorder;
		private float _peakLevel;
		private Rectangle _levelRectangle;
		private AudioPlayer _player;

		public RecordAndPlayControl()
		{
			InitializeComponent();

			_recorder = new AudioRecorder();
			_recorder.Stopped += new EventHandler(_recorder_Stopped);

			_player = new AudioPlayer();
			_player.Stopped += new EventHandler(_player_Stopped);

			Path = System.IO.Path.GetTempFileName();
			SetStyle(ControlStyles.UserPaint,true);
		}

		public void UpdateDisplay()
		{
			_recordButton.Enabled = !Playing;
			_playButton.Enabled = _recorder != null && _recorder.RecordingState != RecordingState.Recording  && !string.IsNullOrEmpty(Path) && File.Exists(Path) && _player.PlaybackState==PlaybackState.Stopped;
			_playButton.Invalidate();
		}

		public bool CanRecord
		{
			get { return _recorder != null && (_player.PlaybackState==PlaybackState.Stopped && (_recorder.RecordingState == RecordingState.Monitoring || _recorder.RecordingState == RecordingState.Stopped)); }
		}


		public bool Recording
		{
			get
			{
				return _recorder.RecordingState == RecordingState.Recording ||
					   _recorder.RecordingState == RecordingState.RequestedStop;
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
//
//        private void _playButton_Click(object sender, EventArgs e)
//        {
//            var session = AudioFactory.AudioSession(Path);
//            session.Play();
//        }


		private void OnRecordDown(object sender, MouseEventArgs e)
		{
			//allow owner one last chance to set a path (which may be sensitive to other ui controls)
//            if (BeforeStartingToRecord != null)
//                BeforeStartingToRecord.Invoke(this, null);
//

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
			}

			_recorder.BeginRecording(Path);
			UpdateDisplay();
		}

		private void OnRecordUp(object sender, MouseEventArgs e)
		{
			_levelMeterTimer.Enabled = false;
			if (_recorder.RecordingState != RecordingState.Recording)
				return;
			try
			{
				_recorder.Stop(); //.StopRecordingAndSaveAsWav();
			}
			catch (Exception)
			{
				//swallow it review: initial reason is that they didn't hold it down long enough, could detect and give message
			}


			UpdateDisplay();
		}

		private void _levelMeterTimer_Tick(object sender, EventArgs e)
		{
			Invalidate(_levelRectangle);
		}

		private void RecordAndPlayControl_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;

			_recorder.SampleAggregator.MaximumCalculated += new EventHandler<MaxSampleEventArgs>(SampleAggregator_MaximumCalculated);
			_levelMeterTimer.Enabled = true;
			_recorder.BeginMonitoring(0);
		}

		private void ComputeLevelRectangle()
		{
			int height = (int)(_peakLevel * (Height - 10));
			Invalidate(_levelRectangle);
			_levelRectangle = new Rectangle(3, (Bottom-Top)-height, 6,  height);
		}

		void SampleAggregator_MaximumCalculated(object sender, MaxSampleEventArgs e)
		{
			_peakLevel = Math.Max(e.MaxSample, Math.Abs(e.MinSample));
			ComputeLevelRectangle();
			Invalidate();
			Debug.WriteLine("peaklevel="+_peakLevel);
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(SystemBrushes.ControlLightLight, this.Bounds);//, e.ClipRectangle);
			base.OnPaint(e);
			//e.Graphics.FillRectangle(Brushes.Gray, _levelRectangle);
			DrawVerticalMeter(e.Graphics);
		}

		private int _previousLevelMeterExtent=1000;

		private void DrawVerticalMeter(Graphics g)
		{
			var fullExtent = this.Height-20;

			// The first step involves painting the entire control so it looks maxed out.
			// After that, erase (using the control's background color) from the top of
			// the control, to a point along the Y coordinate that represents the peak level.

			// Draw green fading to red. The gradient green to red takes up 80% of the meter.
			var partialExtent = (int)(fullExtent * 0.80);

			var width = 5;

			var rc = new Rectangle(0, 0, width, partialExtent + 1);

			using (var br = new LinearGradientBrush(rc, Color.Red, Color.LightGreen, 90f))
			{
				var blend = new Blend();
//                blend.Positions = new[] { 0.0f, 0.4f, 0.9f, 1.0f };
//                blend.Factors = new[] { 0.0f, 0.5f, 1.0f, 1.0f };
				blend.Positions = new[] { 0.0f, 0.1f, 0.2f, 1.0f };
				blend.Factors = new[] { 0.0f, 0.0f, 1.0f, 1.0f };
				br.Blend = blend;
				g.FillRectangle(br, rc);
			}

			// Draw yellow fading to green. The gradient yellow to green take up 20% of the meter.
			rc.Y = partialExtent - 1;
			rc.Height = (int)(fullExtent * 0.20) + 2;

			using (var br = new LinearGradientBrush(rc, Color.LightGreen, Color.Yellow, 90f))
			{
				rc.Y++;
				var blend = new Blend();
				 blend.Positions = new[] { 0.0f, 0.1f, 0.2f, 1.0f };
				blend.Factors = new[] { 0.0f, 0.8f, 1.0f, 1.0f };
				br.Blend = blend;
				g.FillRectangle(br, rc);
			}


			// If the meter is maxed out, then we're done.
			if (_peakLevel.Equals(1f))
				return;

			// Now use the back ground color to erase the part of control
			// that represents what's above the peak level.

		   // _peakLevel = .5f;

			partialExtent = fullExtent -
				(int)(Math.Round(_peakLevel * fullExtent, MidpointRounding.AwayFromZero));

			partialExtent = fullExtent- _levelRectangle.Height;
//
//            if (_previousLevelMeterExtent == 1000)
//                _previousLevelMeterExtent = partialExtent;
//
//            if(partialExtent> _previousLevelMeterExtent)
//            {
//                partialExtent = _previousLevelMeterExtent-10;//fade
//            }
//            _previousLevelMeterExtent = partialExtent;

			//partialExtent = 0;
			rc = new Rectangle(0, 0, width, partialExtent);

			using (var br = new SolidBrush(BackColor))
				g.FillRectangle(br, rc);
		}

		public void OnPlay(object sender, EventArgs e)
		{
			if (!_playButton.Enabled)
				return; //could be fired by keyboard

			try
			{
				_player.LoadFile(_path);

				UpdateDisplay();
				_player.Play();
				UpdateDisplay();
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
			if (_recorder.RecordedTime.TotalMilliseconds < 500)
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
	}
}
