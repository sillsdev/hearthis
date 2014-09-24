// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Script;
using L10NSharp;
using Palaso.Linq;

namespace HearThis.Publishing
{
	public partial class PublishDialog : Form
	{
		private readonly PublishingModel _model;
		private readonly IScrProjectSettings _scrProjectSettings;
		private readonly bool _projectHasNestedQuotes;

		private enum State
		{
			Setup,
			Working,
			Success,
			Failure
		}

		private State _state;
		private BackgroundWorker _worker;

		private const char kAudioFormatRadioPrefix = '_';
		private const string kAudioFormatRadioSuffix = "Radio";

		public PublishDialog(Project project)
		{
			InitializeComponent();
			if (ReallyDesignMode)
				return;

			_scrProjectSettings = project.ScrProjectSettings;
			_projectHasNestedQuotes = project.HasNestedQuotes;

			var additionalBlockBreakCharacters = Settings.Default.AdditionalBlockBreakCharacters ?? string.Empty;
			if (Settings.Default.BreakQuotesIntoBlocks && !String.IsNullOrEmpty(_scrProjectSettings.FirstLevelStartQuotationMark))
			{
				additionalBlockBreakCharacters += " " + _scrProjectSettings.FirstLevelStartQuotationMark;
				if (_scrProjectSettings.FirstLevelStartQuotationMark != _scrProjectSettings.FirstLevelEndQuotationMark)
					additionalBlockBreakCharacters += " " + _scrProjectSettings.FirstLevelEndQuotationMark;
			}

			_model = new PublishingModel(project, additionalBlockBreakCharacters);
			_logBox.ShowDetailsMenuItem = true;
			_logBox.ShowCopyToClipboardMenuItem = true;

			var defaultAudioFormat = tableLayoutPanelAudioFormat.Controls.OfType<RadioButton>().FirstOrDefault(
				b => b.Name == kAudioFormatRadioPrefix + Settings.Default.PublishAudioFormat + kAudioFormatRadioSuffix);
			if (defaultAudioFormat != null)
				defaultAudioFormat.Checked = true;

			if (Settings.Default.PublishVerseIndexFormat == _includePhraseLevelLabels.Name)
			{
				_audacityLabelFile.Checked = true;
				_includePhraseLevelLabels.Enabled = true;
				_includePhraseLevelLabels.Checked = true;
			}
			else
			{
				var defaultVerseIndexFormat =
					tableLayoutPanelVerseIndexFormat.Controls.OfType<RadioButton>()
						.FirstOrDefault(b => b.Name == Settings.Default.PublishVerseIndexFormat);
				if (defaultVerseIndexFormat != null)
					defaultVerseIndexFormat.Checked = true;
			}

			_none.Tag = PublishingModel.VerseIndexFormatType.None;
			_cueSheet.Tag = PublishingModel.VerseIndexFormatType.CueSheet;
			_audacityLabelFile.Tag = PublishingModel.VerseIndexFormatType.AudacityLabelFileVerseLevel;

			_rdoCurrentBook.Checked = _model.PublishOnlyCurrentBook;
			_rdoCurrentBook.Text = string.Format(_rdoCurrentBook.Text, _model.PublishingInfoProvider.CurrentBookName);

			UpdateDisplay(State.Setup);
		}

		protected bool ReallyDesignMode
		{
			get
			{
				return (DesignMode || GetService(typeof (IDesignerHost)) != null) ||
						(LicenseManager.UsageMode == LicenseUsageMode.Designtime);
			}
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
					_megaVoiceRadio.Enabled = true;
					break;
				case State.Working:
					_publishButton.Enabled = false;
					_changeDestinationLink.Enabled = false;
					tableLayoutPanelAudioFormat.Controls.OfType<RadioButton>().ForEach(b => b.Enabled = false);
					tableLayoutPanelVerseIndexFormat.Controls.OfType<RadioButton>().ForEach(b => b.Enabled = false);
					break;
				case State.Success:
				case State.Failure:
					_cancelButton.Text = LocalizationManager.GetString("PublishDialog.Close", "&Close",
						"Cancel Button text changes to this after publishing.");
					_openFolderLink.Text = _model.PublishThisProjectPath;
					_openFolderLink.Visible = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void _publishButton_Click(object sender, EventArgs e)
		{
			_model.AudioFormat =
				tableLayoutPanelAudioFormat.Controls.OfType<RadioButton>().Single(b => b.Checked).Name.
					TrimStart(kAudioFormatRadioPrefix).Replace(kAudioFormatRadioSuffix, string.Empty);

			if (_includePhraseLevelLabels.Checked)
			{
				Settings.Default.PublishVerseIndexFormat = _includePhraseLevelLabels.Name;
				_model.VerseIndexFormat = PublishingModel.VerseIndexFormatType.AudacityLabelFilePhraseLevel;
			}
			else
			{
				var selectedVerseIndexButton =
					tableLayoutPanelVerseIndexFormat.Controls.OfType<RadioButton>().Single(b => b.Checked);
				Settings.Default.PublishVerseIndexFormat = selectedVerseIndexButton.Name;
				_model.VerseIndexFormat = (PublishingModel.VerseIndexFormatType) selectedVerseIndexButton.Tag;
			}

			_model.PublishOnlyCurrentBook = _rdoCurrentBook.Checked;

			UpdateDisplay(State.Working);
			_worker = new BackgroundWorker();
			_worker.DoWork += _worker_DoWork;
			_worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
			_worker.WorkerSupportsCancellation = true;
			_worker.RunWorkerAsync();
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
			if (_worker == null || !_worker.IsBusy)
			{
				Close();
				return;
			}

			_logBox.CancelRequested = true;

			if (_worker != null)
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

		private void _scrAppBuilderRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (_scrAppBuilderRadio.Checked)
				_audacityLabelFile.Checked = true;
		}

		private void _audacityLabelFile_CheckedChanged(object sender, EventArgs e)
		{
			_includePhraseLevelLabels.Enabled = _audacityLabelFile.Checked;
			if (!_includePhraseLevelLabels.Enabled)
				_includePhraseLevelLabels.Checked = false;
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			WarnAboutConflictBetweenQuoteBreakingAndSAB();
		}

		private void _includePhraseLevelLabels_CheckedChanged(object sender, EventArgs e)
		{
			if (IsHandleCreated)
				WarnAboutConflictBetweenQuoteBreakingAndSAB();
		}

		private void WarnAboutConflictBetweenQuoteBreakingAndSAB()
		{
			if (_includePhraseLevelLabels.Checked && _projectHasNestedQuotes && Settings.Default.BreakQuotesIntoBlocks &&
				_scrProjectSettings != null && !_scrProjectSettings.FirstLevelQuotesAreUnique)
			{
				var msg = string.Format(LocalizationManager.GetString("PublishDialog.PossibleIncompatibilityWithSAB",
					"This project has first-level quotes broken out into separate blocks, but it looks like the first-level" +
					" quotation marks may also be used for other levels (nested quotations). If you publish phrase-level labels," +
					" Scripture App Builder will need to be configured to include the first-level quotation marks ({0} and {1})" +
					" as phrase-ending punctuation, but Scripture App Builder might not be able to distinguish first-level quaotes" +
					" (which should be considered as separate phrases) from other levels (which shoud not)." +
					" Are you sure you want to publish phrase-level labels?", "Param 0 is first-level start quotation mark",
					"Param 1 is first-level ending quotation mark"), _scrProjectSettings.FirstLevelStartQuotationMark,
					_scrProjectSettings.FirstLevelEndQuotationMark);
				if (DialogResult.No == MessageBox.Show(this, msg, ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button1))
					_includePhraseLevelLabels.Checked = false;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			_openFolderLink.MaximumSize = new Size(_publishButton.Location.X - _openFolderLink.Location.X - _publishButton.Margin.Left - _openFolderLink.Margin.Right,
				_openFolderLink.MaximumSize.Height);
			_destinationLabel.MaximumSize = _openFolderLink.MaximumSize;
		}
	}
}
