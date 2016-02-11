using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HearThis.Publishing;
using SIL.IO;
using SIL.Media.Naudio;
using SIL.Progress;

namespace HearThis.UI
{
	public partial class RecordInPartsDlg : Form
	{
		private TempFile _tempFile1 = TempFile.WithExtension("wav");
		TempFile _tempFile2 = TempFile.WithExtension("wav");
		private TempFile _tempFileJoined = TempFile.WithExtension("wav");
		Timer _waitToJoinTimer = new Timer();
		public RecordInPartsDlg()
		{
			// TempFile creates empty files, but we don't want them to exist until there is a real
			// recording to play, because it undesirably enables the play buttons.
			File.Delete(_tempFile1.Path);
			File.Delete(_tempFile2.Path);
			File.Delete(_tempFileJoined.Path);

			InitializeComponent();

			_audioButtonsFirst.HaveSomethingToRecord = _audioButtonsSecond.HaveSomethingToRecord = true;
			_audioButtonsFirst.Path = _tempFile1.Path;
			_audioButtonsSecond.Path = _tempFile2.Path;
			_audioButtonsBoth.Path = _tempFileJoined.Path;
			_audioButtonsFirst.SoundFileCreated += AudioButtonsOnSoundFileCreated;
			_audioButtonsSecond.SoundFileCreated += AudioButtonsOnSoundFileCreated;
			_waitToJoinTimer.Interval = 100;
			_waitToJoinTimer.Tick += (sender, args) =>
			{
				_waitToJoinTimer.Stop();
				var inputFiles = new List<string>();
				if (File.Exists(_tempFile1.Path) && new FileInfo(_tempFile1.Path).Length > 0)
					inputFiles.Add(_tempFile1.Path);
				if (File.Exists(_tempFile2.Path) && new FileInfo(_tempFile2.Path).Length > 0)
					inputFiles.Add(_tempFile2.Path);
				ClipRepository.MergeAudioFiles(inputFiles, _tempFileJoined.Path, new NullProgress());
				UpdateDisplay();
			};
			_audioButtonsFirst.HaveSomethingToRecord = _audioButtonsSecond.HaveSomethingToRecord = true;
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			_audioButtonsFirst.UpdateDisplay();
			_audioButtonsSecond.UpdateDisplay();
			_audioButtonsBoth.UpdateDisplay();
		}

		private void AudioButtonsOnSoundFileCreated(object sender, EventArgs eventArgs)
		{
			_waitToJoinTimer.Start();
		}

		public Font Font
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

		public string Text
		{
			get { return _recordTextBox.Text; }
			set { _recordTextBox.Text = value; }
		}

		public RecordingDevice RecordingDevice
		{
			get { return _audioButtonsFirst.RecordingDevice; }
			set
			{_audioButtonsFirst.RecordingDevice = _audioButtonsSecond.RecordingDevice = value;}
		}

		public Dictionary<string, string> ContextForAnalytics
		{
			get { return _audioButtonsFirst.ContextForAnalytics; }
			set { _audioButtonsFirst.ContextForAnalytics = _audioButtonsSecond.ContextForAnalytics = _audioButtonsBoth.ContextForAnalytics = value; }
		}

		public void WriteCombinedAudio(string destPath)
		{
			if (!File.Exists(_tempFileJoined.Path))
				return;
			File.Delete(destPath);
			File.Copy(_tempFileJoined.Path, destPath);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			UpdateDisplay();
		}
	}
}
