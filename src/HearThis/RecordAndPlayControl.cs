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
			_playButton.Initialize(Resources.play, Resources.playDisabled);
			_recordButton.Initialize(Resources.record,Resources.recordDisabled);

			_recorder = new AudioRecorder();

			_player = new AudioPlayer();

			Path = System.IO.Path.GetTempFileName();
			SetStyle(ControlStyles.UserPaint,true);
		}

		public void UpdateDisplay()
		{
			_recordButton.Enabled = true;
			_playButton.Enabled = _recorder != null && _recorder.RecordingState != RecordingState.Recording  && !string.IsNullOrEmpty(Path) && File.Exists(Path);
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
				File.Delete(Path);

			_recorder.BeginRecording(Path);
			//WaveCallbackInfo cb;c
			//var x = new WaveRecorder(new WaveInProvider(new WaveIn(cb)), Path);

			//_recorder.StartRecording();
			//_levelMeterTimer.Enabled = true;
			//UpdateScreen();
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

			if (_recorder.RecordedTime.TotalMilliseconds < 500)
			{
				if (File.Exists(_path))
				{
					File.Delete(_path);
				}
				//_hint.Text = "Hold down the record button while talking.";
			}
			else
			{
				// _hint.Text = "";
			}
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
			_player.LoadFile(_path);
			_player.Play();
		}

	}
}
