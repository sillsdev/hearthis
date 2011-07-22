using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using HearThis.Properties;
using NAudio.Wave;
using Palaso.Media;
using Palaso.Reporting;
using VoiceRecorder.Audio;

namespace HearThis
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
			_recordButton.Enabled = _recorder != null && (_player.PlaybackState==PlaybackState.Stopped && (_recorder.RecordingState == RecordingState.Monitoring || _recorder.RecordingState == RecordingState.Stopped));
			_playButton.Enabled = _recorder != null && _recorder.RecordingState != RecordingState.Recording  && !string.IsNullOrEmpty(Path) && File.Exists(Path) && _player.PlaybackState==PlaybackState.Stopped;
			_playButton.Invalidate();
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
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(SystemBrushes.ControlLightLight, e.ClipRectangle);
			base.OnPaint(e);
			e.Graphics.FillRectangle(Brushes.Gray, _levelRectangle);
		}

		private void _playButton_Click_1(object sender, EventArgs e)
		{
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
			}
			//_recorder.BeginMonitoring(0);
			UpdateDisplay();
		}
	}
}
