using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace HearThis.Publishing
{
	public partial class PublishDialog : Form
	{
		private readonly PublishingModel _model;

		enum State
		{
			Setup,
			Working,
			Success,
			Failure
		}

		private State _state = State.Setup;

		public PublishDialog(PublishingModel model)
		{
			_model = model;
			InitializeComponent();
			_destinationLabel.Text = _model.RootPath;
			_logBox.ShowDetailsMenuItem = true;
			_logBox.ShowCopyToClipboardMenuItem = true;
			UpdateDisplay(State.Setup);
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
					_flacRadio.Enabled = _oggRadio.Enabled = _mp3Radio.Enabled = _saberRadio.Enabled = _megavoiceRadio.Enabled = false;
					break;
				case State.Success: _cancelButton.Visible = false;
					_flacRadio.Enabled = _oggRadio.Enabled = _mp3Radio.Enabled = _saberRadio.Enabled = _megavoiceRadio.Enabled = false;
					_publishButton.Text = "&Close";
					_openFolderLink.Text = _model.RootPath;
					_openFolderLink.Visible = true;
					break;
				case State.Failure:
					_publishButton.Text = "&Close";
					_flacRadio.Enabled = _oggRadio.Enabled = _mp3Radio.Enabled = _saberRadio.Enabled = _megavoiceRadio.Enabled = false;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void _publishButton_Click(object sender, EventArgs e)
		{
			if(!_cancelButton.Visible)//kind of a hack at the moment, the publish button does double-duty
			{
				Close();
				return;
			}


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


			//IAudioEncoder encoder = _mp3Radio.Enabled ? new LameEncoder() : new FlacEncoder();
			UpdateDisplay(State.Working);
			var state = _model.Publish(_logBox) ? State.Success : State.Failure;
			UpdateDisplay(state);
		}

		private void _openFolderLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(_model.RootPath);
		}

		private void _mp3Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			MessageBox.Show("Before or after installing 'Lame for Audacity', you'll need to restart HearThis");
			Process.Start("http://audacity.sourceforge.net/help/faq?s=install&i=lame-mp3");
		}
	}
}
