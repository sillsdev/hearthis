// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using HearThis.Properties;
using HearThis.Script;
using HearThis.UI;
using L10NSharp;
using SIL.Linq;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HearThis.Publishing
{
	public partial class PublishDialog : Form, ILocalizable
	{
		private readonly bool _checkForProblemsBeforePublishing;
		private readonly PublishingModel _model;
		private readonly IScrProjectSettings _scrProjectSettings;
		private readonly bool _projectHasNestedQuotes;

		private enum State
		{
			InitialDisplay,
			Working,
			Success,
			Failure
		}

		private State _state = State.InitialDisplay;
		private BackgroundWorker _worker;

		private const char kAudioFormatRadioPrefix = '_';
		private const string kAudioFormatRadioSuffix = "Radio";

		public PublishDialog(Project project, bool checkForProblemsBeforePublishing)
		{
			_checkForProblemsBeforePublishing = checkForProblemsBeforePublishing;
			InitializeComponent();
			if (ReallyDesignMode)
				return;

			_scrProjectSettings = project.ScrProjectSettings;
			_projectHasNestedQuotes = project.HasNestedQuotes;

			_model = new PublishingModel(project);
			_logBox.ShowDetailsMenuItem = true;
			_logBox.ShowCopyToClipboardMenuItem = true;

			var defaultAudioFormat = _tableLayoutPanelAudioFormat.Controls.OfType<RadioButton>().FirstOrDefault(
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
					_tableLayoutPanelVerseIndexFormat.Controls.OfType<RadioButton>()
						.FirstOrDefault(b => b.Name == Settings.Default.PublishVerseIndexFormat);
				if (defaultVerseIndexFormat != null)
					defaultVerseIndexFormat.Checked = true;
			}

			_none.Tag = PublishingModel.VerseIndexFormatType.None;
			_cueSheet.Tag = PublishingModel.VerseIndexFormatType.CueSheet;
			_audacityLabelFile.Tag = PublishingModel.VerseIndexFormatType.AudacityLabelFileVerseLevel;

			_rdoCurrentBook.Checked = _model.PublishOnlyCurrentBook;
			UpdateDisplay();

			Program.RegisterLocalizable(this);
			HandleStringsLocalized();
		}

		public void HandleStringsLocalized()
		{
			_rdoCurrentBook.Text = string.Format(_rdoCurrentBook.Text, _model.PublishingInfoProvider.CurrentBookName);
			_audacityLabelFile.Text = string.Format(_audacityLabelFile.Text, _scrAppBuilderRadio.Text, "Audacity");
		}

		private bool ReallyDesignMode =>
			DesignMode || GetService(typeof(IDesignerHost)) != null ||
			LicenseManager.UsageMode == LicenseUsageMode.Designtime;

		public bool ShowProblems { get; private set; }

		private void UpdateDisplay(State state)
		{
			_state = state;
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			switch (_state)
			{
				case State.InitialDisplay:
					_destinationLabel.Text = _model.PublishThisProjectPath;
					Debug.Assert(_publishButton.Enabled, "Button state should already be correct. Display should never revert to this state.");
					break;
				case State.Working:
					_publishButton.Enabled = false;
					_changeDestinationLink.Enabled = false;
					_tableLayoutPanelAudioFormat.Controls.OfType<RadioButton>().ForEach(b => b.Enabled = false);
					_tableLayoutPanelVerseIndexFormat.Controls.OfType<RadioButton>().ForEach(b => b.Enabled = false);
					_tableLayoutPanelBooksToPublish.Controls.OfType<RadioButton>().ForEach(b => b.Enabled = false);
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
				_tableLayoutPanelAudioFormat.Controls.OfType<RadioButton>().Single(b => b.Checked).Name.
					TrimStart(kAudioFormatRadioPrefix).Replace(kAudioFormatRadioSuffix, string.Empty);

			if (_includePhraseLevelLabels.Checked)
			{
				Settings.Default.PublishVerseIndexFormat = _includePhraseLevelLabels.Name;
				_model.VerseIndexFormat = PublishingModel.VerseIndexFormatType.AudacityLabelFilePhraseLevel;
			}
			else
			{
				var selectedVerseIndexButton =
					_tableLayoutPanelVerseIndexFormat.Controls.OfType<RadioButton>().Single(b => b.Checked);
				Settings.Default.PublishVerseIndexFormat = selectedVerseIndexButton.Name;
				_model.VerseIndexFormat = (PublishingModel.VerseIndexFormatType)selectedVerseIndexButton.Tag;
			}

			_model.PublishOnlyCurrentBook = _rdoCurrentBook.Checked;

			if (_checkForProblemsBeforePublishing &&
				_model.BooksToExportHaveProblemsNeedingAttention()
				&& DoesUserWantToSeeProblems())
			{
				ShowProblems = true;
				Close();
				return;
			}

			// save audio post-processing choices
			_model.NormalizeVolume = _chkNormalizeVolume.Checked;
			_model.ReduceNoise = _chkNoiseReduction.Checked;
			_model.SentencePause = new PauseData(_chkSentencePauses.Checked, Decimal.ToDouble(_numericSectionPauseMin.Value), Decimal.ToDouble(_numericSectionPauseMax.Value));
			_model.ParagraphPause = new PauseData(_chkParagraphPauses.Checked, Decimal.ToDouble(_numericSectionPauseMin.Value), Decimal.ToDouble(_numericSectionPauseMax.Value));
			_model.SectionPause = new PauseData(_chkSectionPauses.Checked, Decimal.ToDouble(_numericSectionPauseMin.Value), Decimal.ToDouble(_numericSectionPauseMax.Value));
			_model.ChapterPause = new PauseData(_chkChapterPauses.Checked, Decimal.ToDouble(_numericSectionPauseMin.Value), Decimal.ToDouble(_numericSectionPauseMax.Value));

			UpdateDisplay(State.Working);
			_worker = new BackgroundWorker();
			_worker.DoWork += _worker_DoWork;
			_worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
			_worker.WorkerSupportsCancellation = true;
			_worker.RunWorkerAsync();
		}

		private bool DoesUserWantToSeeProblems()
		{
			var msg = LocalizationManager.GetString("PublishDialog.ProblemsNeedingAttention",
				"There are potential problems with one or more of the recordings that you are about to export. " +
				"(For example, a clip might not match the current version of the text.) Would you like to " +
				"look at the problems before exporting?");
			return MessageBox.Show(this, msg, ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
				MessageBoxDefaultButton.Button1) == DialogResult.Yes;
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

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			if (_worker == null || !_worker.IsBusy)
			{
				Close();
				return;
			}

			_logBox.CancelRequested = true;

			_worker?.CancelAsync();
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
			if (_includePhraseLevelLabels.Checked && _projectHasNestedQuotes && _model.PublishingInfoProvider.BreakQuotesIntoBlocks &&
				_scrProjectSettings != null && !_scrProjectSettings.FirstLevelQuotesAreUnique)
			{
				var msg = string.Format(LocalizationManager.GetString("PublishDialog.PossibleIncompatibilityWithSAB",
					"This project has first-level quotes broken out into separate blocks, but it looks like the first-level" +
					" quotation marks may also be used for other levels (nested quotations). If you publish phrase-level labels," +
					" {2} will need to be configured to include the first-level quotation marks ({0} and {1})" +
					" as phrase-ending punctuation, but {2} might not be able to distinguish first-level quotes" +
					" (which should be considered as separate phrases) from other levels (which should not)." +
					" Are you sure you want to publish phrase-level labels?",
					"Param 0: first-level start quotation mark; " +
					"Param 1: first-level ending quotation mark; " +
					"Param 2: \"Scripture App Builder\" (product name)"),
					_scrProjectSettings.FirstLevelStartQuotationMark,
					_scrProjectSettings.FirstLevelEndQuotationMark,
					ScriptureAppBuilderPublishingMethod.kScriptureAppBuilder);
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

		private int minPauseNormalizationRow =>
			_tableLayoutPanelAudioNormalization.GetRow(_numericSentencePauseMin);

		private void MinPauseValueChanged(object sender, EventArgs e)
		{
			var min = (NumericUpDown)sender;
			var cellMin = _tableLayoutPanelAudioNormalization.GetCellPosition(min);
			var max = (NumericUpDown)_tableLayoutPanelAudioNormalization
				.GetControlFromPosition(cellMin.Column + 2, cellMin.Row);
			if (min.Value > max.Value)
				max.Value = Math.Min(min.Value + max.Increment, max.Maximum);
			if (_tableLayoutPanelAudioNormalization.RowCount > cellMin.Row + 1)
			{
				var nextMin = (NumericUpDown)_tableLayoutPanelAudioNormalization
					.GetControlFromPosition(cellMin.Column, cellMin.Row + 1);
				
				if (min.Value > nextMin.Value)
					nextMin.Value = Math.Max(min.Value, nextMin.Minimum);
			}
			if (cellMin.Row > minPauseNormalizationRow)
			{
				var prevMin = (NumericUpDown)_tableLayoutPanelAudioNormalization
					.GetControlFromPosition(cellMin.Column, cellMin.Row - 1);
				
				if (min.Value < prevMin.Value)
					prevMin.Value = Math.Max(min.Value, prevMin.Minimum);
			}
		}

		private void MaxPauseValueChanged(object sender, EventArgs e)
		{
			var max = (NumericUpDown)sender;
			var cellMax = _tableLayoutPanelAudioNormalization.GetCellPosition(max);
			var min = (NumericUpDown)_tableLayoutPanelAudioNormalization
				.GetControlFromPosition(cellMax.Column - 2, cellMax.Row);
			if (min.Value > max.Value)
				min.Value = Math.Max(max.Value - min.Increment, min.Minimum);
			if (_tableLayoutPanelAudioNormalization.RowCount > cellMax.Row + 1)
			{
				var nextMax = (NumericUpDown)_tableLayoutPanelAudioNormalization
					.GetControlFromPosition(cellMax.Column, cellMax.Row + 1);
				
				if (max.Value > nextMax.Value)
					nextMax.Value = Math.Min(max.Value, nextMax.Maximum);
			}
			if (cellMax.Row > minPauseNormalizationRow)
			{
				var prevMax = (NumericUpDown)_tableLayoutPanelAudioNormalization
					.GetControlFromPosition(cellMax.Column, cellMax.Row - 1);
				
				if (max.Value < prevMax.Value)
					prevMax.Value = Math.Max(max.Value, prevMax.Minimum);
			}
		}
	}
}
