using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Forms;
using L10NSharp;

namespace HearThis.Publishing
{
	public partial class PublishDialog : Form
	{
		private readonly PublishingModel _model;

		private enum State
		{
			Setup,
			Working,
			Success,
			Failure
		}


		private State _state = State.Setup;
		private BackgroundWorker _worker;

		public PublishDialog(PublishingModel model)
		{
			InitializeComponent();
			if (ReallyDesignMode)
				return;
			_model = model;
			_logBox.ShowDetailsMenuItem = true;
			_logBox.ShowCopyToClipboardMenuItem = true;
			UpdateDisplay(State.Setup);
		}

		protected bool ReallyDesignMode
		{
			get
			{
				return (DesignMode || GetService(typeof(IDesignerHost)) != null) ||
					(LicenseManager.UsageMode == LicenseUsageMode.Designtime);
			}
		}


		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		private void UpdateDisplay(State state)
		{
			_state = state;
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			_destinationLabel.Text = _model.PublishThisProjectPath;

			switch (_state)
			{
				case State.Setup:
					string tooltip;
					_flacRadio.Enabled = true;
					_oggRadio.Enabled = FlacEncoder.IsAvailable(out tooltip);
					toolTip1.SetToolTip(_oggRadio, tooltip);
					_mp3Radio.Enabled = LameEncoder.IsAvailable(out tooltip);
					_saberRadio.Enabled = _mp3Radio.Enabled;
					toolTip1.SetToolTip(_mp3Radio, tooltip);
					_mp3Link.Visible = !_mp3Radio.Enabled;
					_saberLink.Visible = !_saberRadio.Enabled;
					_megavoiceRadio.Enabled = true;
				   break;
				case State.Working:
					_publishButton.Enabled = false;
					DisablePublishTypeRadios();
					break;
				case State.Success:
					 _cancelButton.Text = GetCloseTextForCancelButton();
					DisablePublishTypeRadios();
					_publishButton.Enabled = false;
					_openFolderLink.Text = _model.PublishThisProjectPath;
					_openFolderLink.Visible = true;
					break;
				case State.Failure:
					_cancelButton.Text = GetCloseTextForCancelButton();
					DisablePublishTypeRadios();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void DisablePublishTypeRadios()
		{
			_flacRadio.Enabled = _audiBibleRadio.Enabled = _oggRadio.Enabled =_mp3Radio.Enabled =
				_saberRadio.Enabled = _megavoiceRadio.Enabled = false;
		}

		private static string GetCloseTextForCancelButton()
		{
			return LocalizationManager.GetString("PublishDialog.Close", "&Close",
				"Cancel Button text changes to this after successful publish");
		}

		private void _publishButton_Click(object sender, EventArgs e)
		{

			if (_saberRadio.Checked)
				_model.PublishingMethod = new SaberPublishingMethod();
			else if(_megavoiceRadio.Checked)
				_model.PublishingMethod = new MegaVoicePublishingMethod();
			else if (_mp3Radio.Checked)
				_model.PublishingMethod = new BunchOfFilesPublishingMethod(new LameEncoder());
			else if (_flacRadio.Checked)
				_model.PublishingMethod = new BunchOfFilesPublishingMethod(new FlacEncoder());
			else if (_oggRadio.Checked)
				_model.PublishingMethod = new BunchOfFilesPublishingMethod(new OggEncoder());
			else if (_audiBibleRadio.Checked)
				_model.PublishingMethod = new AudiBiblePublishingMethod(new AudiBibleEncoder(), _model.EthnologueCode);

			if (_none.Checked)
				_model.verseIndexFormat = PublishingModel.VerseIndexFormat.None;
			else if (_cueSheet.Checked)
				_model.verseIndexFormat = PublishingModel.VerseIndexFormat.CueSheet;
			else if (_audacityLabelFile.Checked)
				_model.verseIndexFormat = PublishingModel.VerseIndexFormat.AudacityLabelFile;


			//IAudioEncoder encoder = _mp3Radio.Enabled ? new LameEncoder() : new FlacEncoder();
			UpdateDisplay(State.Working);
			_worker = new BackgroundWorker();
			_worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
			_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
			_worker.WorkerSupportsCancellation = true;
			_worker.RunWorkerAsync();

			UpdateDisplay(State.Working);
		}

		private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			 UpdateDisplay();
		}

		private void _worker_DoWork(object sender, DoWorkEventArgs e)
		{
			_state = _model.Publish(_logBox) ? State.Success : State.Failure;
		}

		private void _openFolderLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(_model.PublishThisProjectPath);
		}

		private void _mp3Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			MessageBox.Show(LocalizationManager.GetString("PublishDialog.Restart",
				"OK will open your browser to a page where you should be able to download a version of Lame.exe. It may NOT be the main Download button! Before or after installing 'Lame for Audacity', you'll need to restart HearThis"));
			Process.Start("http://lame1.buanzo.com.ar/#lamewindl");
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			if(_worker ==null || !_worker.IsBusy)
			{
				Close();
				return;
			}

			_logBox.CancelRequested = true;

			if(_worker!=null)
				_worker.CancelAsync();
		}

		private void _changeDestinationLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			using (var dlg = new FolderBrowserDialog())
			{
				dlg.SelectedPath = _model.PublishRootPath;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					_model.PublishRootPath = dlg.SelectedPath;
					UpdateDisplay();
				}
			}
		}

		private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
		{

		}

		private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
		{

		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

		private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}
	}
}
