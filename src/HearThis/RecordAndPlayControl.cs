using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using HearThis.Properties;
using Palaso.Media;
using Palaso.Reporting;

namespace HearThis
{
	public partial class RecordAndPlayControl : UserControl
	{
		private string _path;
		private ISimpleAudioSession _recorder;

		public RecordAndPlayControl()
		{
			InitializeComponent();
			_playButton.EnabledImage = Resources.play;
			_playButton.DisabledImage = Resources.playDisabled;
			_recordButton.EnabledImage = Resources.record;
			_recordButton.DisabledImage = Resources.recordDisabled;

			Path = System.IO.Path.GetTempFileName();
		}

		public void UpdateDisplay()
		{
			_recordButton.Enabled = true;
			_playButton.Enabled = _recorder != null && _recorder.CanPlay;// !string.IsNullOrEmpty(Path) && File.Exists(Path);
		}

		public string Path
		{
			get { return _path; }
			set
			{
				_path = value;
				_recorder = AudioFactory.AudioSession(Path);
				//_timer.Enabled = true;
			}
		}

		private void _playButton_Click(object sender, EventArgs e)
		{
			var session = AudioFactory.AudioSession(Path);
			session.Play();
		}


		private void OnRecordDown(object sender, MouseEventArgs e)
		{
			//allow owner one last chance to set a path (which may be sensitive to other ui controls)
//            if (BeforeStartingToRecord != null)
//                BeforeStartingToRecord.Invoke(this, null);
//
			if (File.Exists(Path))
				File.Delete(Path);

			_recorder.StartRecording();
			//UpdateScreen();
		}

		private void OnRecordUp(object sender, MouseEventArgs e)
		{
			if (!_recorder.IsRecording)
				return;
			try
			{
				_recorder.StopRecordingAndSaveAsWav();
			}
			catch (Exception)
			{
				//swallow it review: initial reason is that they didn't hold it down long enough, could detect and give message
			}

			if (_recorder.LastRecordingMilliseconds < 500)
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
		   //UpdateScreen();
//            if (SoundRecorded != null)
//            {
//                SoundRecorded.Invoke(this, null);
//                UsageReporter.SendNavigationNotice("AudioRecorded");
//            }
		}

	}
}
