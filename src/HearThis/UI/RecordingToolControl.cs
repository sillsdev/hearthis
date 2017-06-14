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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using SIL.Code;
using SIL.Media.Naudio;
using SIL.Reporting;
using SIL.Windows.Forms.SettingProtection;
using static System.String;

namespace HearThis.UI
{
	public partial class RecordingToolControl : UserControl, IMessageFilter
	{
		private Project _project;
		private int _previousLine = -1;
		private bool _alreadyShutdown;
		private string _lineCountLabelFormat;
		private bool _changingChapter = false;
		private Stopwatch _tempStopwatch = new Stopwatch();

		private readonly string _endOfBook = LocalizationManager.GetString("RecordingControl.EndOf", "End of {0}",
			"{0} is typically a book name");
		private readonly string _chapterFinished = LocalizationManager.GetString("RecordingControl.Finished", "{0} Finished",
			"{0} is a chapter number");
		private readonly string _gotoLink = LocalizationManager.GetString("RecordingControl.GoTo", "Go To {0}",
			"{0} is a chapter number");
		private bool _hidingSkippedBlocks;

		public RecordingToolControl()
		{
			_tempStopwatch.Start();

			InitializeComponent();
			SetZoom(Settings.Default.ZoomFactor); // do after InitializeComponent sets it to 1.
			SettingsProtectionSettings.Default.PropertyChanged += OnSettingsProtectionChanged;
			_lineCountLabelFormat = _lineCountLabel.Text;
			BackColor = AppPallette.Background;
			_bookLabel.ForeColor = AppPallette.TitleColor;
			_chapterLabel.ForeColor = AppPallette.TitleColor;
			_segmentLabel.ForeColor = AppPallette.TitleColor;
			_lineCountLabel.ForeColor = AppPallette.TitleColor;
			_segmentLabel.BackColor = AppPallette.Background;
			_lineCountLabel.BackColor = AppPallette.Background;
			_smallerButton.ForeColor = AppPallette.TitleColor;
			_largerButton.ForeColor = AppPallette.TitleColor;

			//_upButton.Initialize(Resources.up, Resources.upDisabled);
			//_nextButton.Initialize(Resources.down, Resources.downDisabled);

			if (DesignMode)
				return;

			_peakMeter.Start(33); //the number here is how often it updates
			_peakMeter.ColorMedium = AppPallette.Blue;
			_peakMeter.ColorNormal = AppPallette.EmptyBoxColor;
			_peakMeter.ColorHigh = AppPallette.Red;
			_peakMeter.SetRange(5, 80, 100);
			_audioButtonsControl.Recorder.PeakLevelChanged += ((s, e) => _peakMeter.PeakLevel = e.Level);
			_audioButtonsControl.RecordingDevice = RecordingDevice.Devices.FirstOrDefault();
			if (_audioButtonsControl.RecordingDevice == null)
			{
				_audioButtonsControl.ReportNoMicrophone();
				Environment.Exit(1);
			}
			recordingDeviceButton1.Recorder = _audioButtonsControl.Recorder;
			MouseWheel += OnRecordingToolControl_MouseWheel;

			_endOfUnitMessage.ForeColor = AppPallette.Blue;
			_nextChapterLink.ActiveLinkColor = AppPallette.HilightColor;
			_nextChapterLink.DisabledLinkColor = AppPallette.NavigationTextColor;
			_nextChapterLink.LinkColor = AppPallette.HilightColor;

			_audioButtonsControl.SoundFileCreated += OnSoundFileCreated;
			_audioButtonsControl.RecordingStarting += OnAudioButtonsControlRecordingStarting;

			UpdateBreakClausesImage();
			_longLineButton.Image = AppPallette.RecordInPartsImage;

			_lineCountLabel.ForeColor = AppPallette.NavigationTextColor;
		}

		private void OnAudioButtonsControlRecordingStarting(object sender, CancelEventArgs cancelEventArgs)
		{
			if (SelectedBlockHasSkippedStyle)
			{
				var fmt = LocalizationManager.GetString("RecordingControl.CannotRecordClipForSkippedStyle",
					"The settings for this project prevent recording this block because its paragraph style is {0}. If " +
					"you intend to record blocks having this style, in the Settings dialog box, select the Skipping page, " +
					"and then clear the selection for this style.");
				MessageBox.Show(this, Format(fmt, GetScriptBlock(_project.SelectedScriptBlock).ParagraphStyle), Program.kProduct);
				cancelEventArgs.Cancel = true;
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			var shell = Parent as Shell;
			if (shell != null)
			{
				shell.OnProjectChanged += delegate(object sender, EventArgs args)
				{
					SetProject(((Shell) sender).Project);
				};
			}
		}

		private void OnSettingsProtectionChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			//when we need to use Ctrl+Shift to display stuff, we don't want it also firing up the localization dialog (which shouldn't be done by a user under settings protection anyhow)
			LocalizationManager.EnableClickingOnControlToBringUpLocalizationDialog =
				!SettingsProtectionSettings.Default.NormallyHidden;
		}

		private void OnSoundFileCreated(object sender, EventArgs eventArgs)
		{
			if (CurrentScriptLine.Skipped)
			{
				var skipPath = Path.ChangeExtension(_project.GetPathToRecordingForSelectedLine(), "skip");
				if (File.Exists(skipPath))
				{
					try
					{
						File.Delete(skipPath);
					}
					catch (Exception e)
					{
						// Bummer. But we can probably ignore this.
						Analytics.ReportException(e);
					}
				}
			}
			if (IsSelectedScriptBlockLastUnskippedInChapter())
				DeleteClipsBeyondLastClip();
			_project.SelectedChapterInfo.OnScriptBlockRecorded(CurrentScriptLine);
			OnSoundFileCreatedOrDeleted();
		}

		private bool IsSelectedScriptBlockLastUnskippedInChapter()
		{
			// DisplayedSegmentCount is 1-based
			// ScriptBlockIndex is 0-based
			return DisplayedSegmentCount == _scriptSlider.Value + 1;
		}

		private void DeleteClipsBeyondLastClip()
		{
			ClipRepository.DeleteAllClipsAfterLine(_project.Name, _project.SelectedBook.Name,
				_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock);
		}

		private void OnSoundFileCreatedOrDeleted()
		{
			_scriptSlider.Refresh();
			// deletion is done in LineRecordingRepository and affects audioButtons
			foreach (ChapterButton chapterButton in _chapterFlow.Controls)
			{
				if (chapterButton.Selected)
				{
					chapterButton.RecalculatePercentageRecorded();
					break;
				}
			}
			UpdateDisplay();
		}

		/// <summary>
		/// This invokes the message filter that allows the control to interpret various keystrokes as button presses.
		/// It is tempting to try to manage this from within this control, e.g., in the constructor and Dispose method.
		/// However, this fails to disable the message filter when dialogs (or the localization tool) are launched.
		/// The interception of the space key, especially, is disconcerting while some dialogs are active.
		/// So, instead, we arrange to call these methods from the OnActivated and OnDeactivate methods of the parent window.
		/// </summary>
		public void StartFilteringMessages()
		{
			Application.AddMessageFilter(this);
		}

		public void StopFilteringMessages()
		{
			Application.RemoveMessageFilter(this);
		}

		private void OnRecordingToolControl_MouseWheel(object sender, MouseEventArgs e)
		{
			//the minus here is because down (negative) on the wheel equates to addition on the horizontal slider
			_scriptSlider.Value += e.Delta / -120;
		}

		public void SetProject(Project project)
		{
			// ENHANCE: Need to use some kind of semaphore to ensure that project doesn't change until books are done loading.

			if (_project != null)
			{
				_project.OnScriptBlockRecordingRestored -= HandleScriptBlockRecordingRestored;
				_project.OnSelectedBookChanged -= HandleSelectedBookChanged;
			}

			_project = project;
			_scriptControl.SetFont(_project.FontName);

			_project.OnScriptBlockRecordingRestored += HandleScriptBlockRecordingRestored;

			_bookFlow.Controls.Clear();
			foreach (BookInfo bookInfo in project.Books)
			{
				var x = new BookButton(bookInfo);
				_instantToolTip.SetToolTip(x, bookInfo.LocalizedName);
				_bookFlow.Controls.Add(x);
				BookInfo bookInfoToAvoidClosureProblem = bookInfo;
				x.Click += delegate { _project.SelectedBook = bookInfoToAvoidClosureProblem; };
				if (bookInfo.BookNumber == 38)
					_bookFlow.SetFlowBreak(x, true);

				if (bookInfo == _project.SelectedBook)
					x.Selected = true;
			}
			_project.LoadBook(_project.SelectedBook.BookNumber);
			UpdateSelectedBook();
			_scriptSlider.GetSegmentBrushesDelegate = GetSegmentBrushes;

			LoadBooksAsync(_project.SelectedBook);
		}

		private void LoadBooksAsync(BookInfo selectedBook)
		{
			var worker = new BackgroundWorker();
			worker.DoWork += delegate
			{
				Parallel.ForEach(_bookFlow.Controls.OfType<BookButton>(), x =>
				{
					_project.LoadBook(x.BookNumber);
					if (x.IsHandleCreated && !x.IsDisposed)
						x.Invalidate();
				});
			};

			worker.RunWorkerCompleted += (sender, args) =>
			{
				// There is an ever-so-slight possibility that the user has selected another book while the books were loading
				// (before subscribing to the Click event for the button they clicked). In this case, the project will have
				// been notified but HandleSelectedBookChanged will not have been called because we intentionally don't want to
				// subscribe to OnSelectedBookChanged until we're really ready. So now we check for this situation.
				if (_project.SelectedBook != selectedBook)
				{
					HandleSelectedBookChanged(_project, new EventArgs());
				}
				_project.OnSelectedBookChanged += HandleSelectedBookChanged;
			};

			worker.RunWorkerAsync();
		}

		private void HandleSelectedBookChanged(object sender, EventArgs e)
		{
			Debug.Assert(_project == sender);
			BookButton selected =
				_bookFlow.Controls.OfType<BookButton>().Single(b => b.BookNumber == _project.SelectedBook.BookNumber);

			foreach (BookButton button in _bookFlow.Controls)
				button.Selected = false;

			selected.Selected = true;

			UpdateSelectedBook();
		}

		private void HandleScriptBlockRecordingRestored(Project sender, int bookNumber, int chapterNumber, ScriptLine scriptBlock)
		{
			if (bookNumber == _project.SelectedBook.BookNumber && chapterNumber == _project.SelectedChapterInfo.ChapterNumber1Based)
				OnSoundFileCreatedOrDeleted();
		}

		private int DisplayedSegmentCount
		{
			get
			{
				Guard.AgainstNull(_project, "project");
				return _project.GetLineCountForChapter(!HidingSkippedBlocks);
			}
		}

		private Brush[] GetSegmentBrushes()
		{
			var brushes = new Brush[DisplayedSegmentCount];
			int iBrush = 0;
			for (var i = 0; i < _project.GetLineCountForChapter(true); i++)
			{
				if (GetScriptBlock(i).Skipped)
				{
					// NB: Skipped segments only get entries in the array of brushes if they are being shown(i.e., in "Admin" mode).
					// If we are hiding skipped segments (as would be typical when HearThis is being used for recording), then we
					// need to avoid putting these (orange) brushes into the collection.
					if (!HidingSkippedBlocks)
						brushes[iBrush++] = AppPallette.SkippedSegmentBrush;
				}
				else if (ClipRepository.GetHaveClip(_project.Name, _project.SelectedBook.Name,
					_project.SelectedChapterInfo.ChapterNumber1Based, i))
				{
					brushes[iBrush++] = AppPallette.BlueBrush;
				}
				else
					brushes[iBrush++] = Brushes.Transparent;
			}
			return brushes;
		}

		private void UpdateDisplay()
		{
			_skipButton.Enabled = HaveScript;
			_audioButtonsControl.HaveSomethingToRecord = HaveScript;
			_audioButtonsControl.UpdateDisplay();
			_lineCountLabel.Visible = HaveScript;
			//_upButton.Enabled = _project.SelectedScriptLine > 0;
			//_audioButtonsControl.CanGoNext = _project.SelectedScriptBlock < (_project.GetLineCountForChapter()-1);
			_deleteRecordingButton.Visible = HaveRecording;
			_longLineButton.Enabled = HaveScript && !SelectedBlockHasSkippedStyle;
		}

		private bool SelectedBlockHasSkippedStyle => ScriptLine.SkippedStyleInfoProvider.IsSkippedStyle(
			GetScriptBlock(_project.SelectedScriptBlock).ParagraphStyle);

		private bool HaveRecording => ClipRepository.GetHaveClip(_project.Name, _project.SelectedBook.Name,
			_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock);

		private bool HaveScript
		{
			// This method is much more reliable for single line sections than comparing slider max & min
			get { return CurrentScriptLine != null && CurrentScriptLine.Text.Length > 0; }
		}


		/// <summary>
		/// Filter out all keystrokes except the few that we want to handle.
		/// We handle Space, TAB, PageUp, PageDown, Delete and Arrow keys.
		/// </summary>
		/// <remarks>This is invoked because we implement IMessagFilter and call Application.AddMessageFilter(this)</remarks>
		public bool PreFilterMessage(ref Message m)
		{
			const int WM_KEYDOWN = 0x100;
			const int WM_KEYUP = 0x101;

			if (m.Msg != WM_KEYDOWN && m.Msg != WM_KEYUP)
				return false;

			if (m.Msg == WM_KEYUP && (Keys) m.WParam != Keys.Space)
				return false;

			switch ((Keys) m.WParam)
			{
				case Keys.OemPeriod:
				case Keys.Decimal:
					MessageBox.Show("To play the clip, press the TAB key.");
					break;

				case Keys.Tab:
					_audioButtonsControl.OnPlay(this, null);
					break;

				case Keys.Right:
				case Keys.PageDown:
				case Keys.Down:
					OnNextButton(this, null);
					break;

				case Keys.Left:
				case Keys.PageUp:
				case Keys.Up:
					GoBack();
					break;

				case Keys.Space:
					if (m.Msg == WM_KEYDOWN)
						_audioButtonsControl.SpaceGoingDown();
					if (m.Msg == WM_KEYUP)
						_audioButtonsControl.SpaceGoingUp();
					break;

				case Keys.P:
					longLineButton_Click(this, new EventArgs());
					break;

				case Keys.Delete:
					OnDeleteRecording();
					break;

				default:
					return false;
			}

			return true;
		}

		public void Shutdown()
		{
			if (_alreadyShutdown)
				return;
			_alreadyShutdown = true;
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnHandleDestroyed(EventArgs e)
		{
			Shutdown();
			base.OnHandleDestroyed(e);
		}

		private void UpdateSelectedBook()
		{
			_bookLabel.Text = _project.SelectedBook.LocalizedName;

			_chapterFlow.SuspendLayout();
			_chapterFlow.Controls.Clear();

			var buttons = new List<ChapterButton>();

			//note: we're using chapter 0 to mean the material at the start of the book
			for (int i = 0; i <= _project.SelectedBook.ChapterCount; i++)
			{
				var chapterInfo = _project.SelectedBook.GetChapter(i);
				if (i == 0 && chapterInfo.IsEmpty)
					continue;

				var button = new ChapterButton(chapterInfo);
				button.Width = 15;
				button.Click += OnChapterClick;
				buttons.Add(button);
				_instantToolTip.SetToolTip(button, i == 0 ? GetIntroductionString() : Format(GetChapterNumberString(), i));
			}
			_chapterFlow.Controls.AddRange(buttons.ToArray());
			_chapterFlow.ResumeLayout(true);
			UpdateSelectedChapter();
		}

		private static string GetIntroductionString()
		{
			return LocalizationManager.GetString("RecordingControl.Introduction", "Introduction");
		}

		private static string GetChapterNumberString()
		{
			return LocalizationManager.GetString("RecordingControl.Chapter", "Chapter {0}");
		}

		private void OnChapterClick(object sender, EventArgs e)
		{
			_project.SelectedChapterInfo = ((ChapterButton)sender).ChapterInfo;
			UpdateSelectedChapter();
		}

		private void UpdateSelectedChapter()
		{
			foreach (ChapterButton chapterButton in _chapterFlow.Controls)
			{
				chapterButton.Selected = false;
			}
			if (_project.SelectedChapterInfo.ChapterNumber1Based > 0)
				_chapterLabel.Text = Format(GetChapterNumberString(), _project.SelectedChapterInfo.ChapterNumber1Based);
			else
				_chapterLabel.Text = Format(GetIntroductionString());

			ChapterButton button = (from ChapterButton control in _chapterFlow.Controls
				where control.ChapterInfo.ChapterNumber1Based == _project.SelectedChapterInfo.ChapterNumber1Based
				select control).Single();

			button.Selected = true;
			ResetSegmentCount();
			_changingChapter = true;
			if (HidingSkippedBlocks)
				_project.SelectedScriptBlock = GetScriptBlockIndexFromSliderValueByAccountingForPrecedingHiddenBlocks(0);

			// If this is the initial load of the project, try to return to the block where user left off.
			int targetBlock = (_previousLine == -1 && Settings.Default.Block >= 0 && Settings.Default.Block < DisplayedSegmentCount) ?
				Settings.Default.Block : 0;

			if (_scriptSlider.Value == targetBlock)
				UpdateSelectedScriptLine();
			else
				_scriptSlider.Value = targetBlock;
			_changingChapter = false;
			UpdateScriptAndMessageControls();

			if (_tempStopwatch != null)
			{
				_tempStopwatch.Stop();
				Debug.WriteLine("Elapsed time: " + _tempStopwatch.ElapsedMilliseconds);
				_tempStopwatch = null;
			}
		}

		private void ResetSegmentCount()
		{
			if (_project == null)
				return;
			_scriptSlider.Refresh();
			_audioButtonsControl.Enabled = DisplayedSegmentCount != 0;
		}

		private void OnLineSlider_ValueChanged(object sender, EventArgs e)
		{
			int sliderValue = _scriptSlider.Value;

			Settings.Default.Block = sliderValue;

			UpdateScriptAndMessageControls();
			if (_scriptSlider.Finished)
				_project.SelectedScriptBlock = _project.GetLineCountForChapter(true);
			else
			{
				if (HidingSkippedBlocks)
					sliderValue = GetScriptBlockIndexFromSliderValueByAccountingForPrecedingHiddenBlocks(sliderValue);
				_project.SelectedScriptBlock = sliderValue;
				UpdateSelectedScriptLine();
			}
		}

		private void UpdateSelectedScriptLine()
		{
			var currentScriptLine = CurrentScriptLine;
			string verse = currentScriptLine != null ? currentScriptLine.Verse : null;
			bool isRealVerseNumber = !IsNullOrEmpty(verse) && verse != "0";
			_segmentLabel.Visible = true;
			_skipButton.CheckedChanged -= OnSkipButtonCheckedChanged;
			_skipButton.Checked = currentScriptLine != null && currentScriptLine.Skipped;
			_skipButton.CheckedChanged += OnSkipButtonCheckedChanged;
			if (HaveScript)
			{
				int displayedBlockIndex = _scriptSlider.Value + 1;
				_lineCountLabel.Text = Format(_lineCountLabelFormat, displayedBlockIndex, DisplayedSegmentCount);

				if (currentScriptLine.Heading)
					_segmentLabel.Text = LocalizationManager.GetString("RecordingControl.Heading", "Heading");
				else if (isRealVerseNumber)
				{
					int firstBridgeChar = verse.IndexOfAny(new[] { '-', '~' });
					int lastBridgeChar = verse.LastIndexOfAny(new[] { '-', '~' });
					if (firstBridgeChar > 0)
					{
						verse = verse.Substring(0, firstBridgeChar) + "-" + verse.Substring(lastBridgeChar + 1);
						_segmentLabel.Text = Format(LocalizationManager.GetString("RecordingControl.ScriptVerseBridge", "Verses {0}"), verse);
					}
					else
						_segmentLabel.Text = Format(LocalizationManager.GetString("RecordingControl.Script", "Verse {0}"), verse);
				}
				else
					_segmentLabel.Text = Empty;
			}
			else
			{
				if (isRealVerseNumber)
				{
					_segmentLabel.Text =
						Format(LocalizationManager.GetString("RecordingControl.VerseNotTranslated", "Verse {0} not translated yet"), verse);
				}
				else
				{
					_segmentLabel.Text = LocalizationManager.GetString("RecordingControl.NotTranslated", "Not translated yet");
				}
			}

			if (DisplayedSegmentCount == 0)
				_project.SelectedScriptBlock = 0; // This should already be true, but just make sure;

			_scriptControl.GoToScript(GetDirection(), PreviousScriptBlock, currentScriptLine, NextScriptBlock);
			_previousLine = _project.SelectedScriptBlock;
			_audioButtonsControl.Path = _project.GetPathToRecordingForSelectedLine();

			char[] delimiters = {' ', '\r', '\n'};

			var approximateWordCount = 0;
			if (currentScriptLine != null)
				approximateWordCount = currentScriptLine.Text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;

			_audioButtonsControl.ContextForAnalytics = new Dictionary<string, string>
			{
				{"book", _project.SelectedBook.Name},
				{"chapter", _project.SelectedChapterInfo.ChapterNumber1Based.ToString()},
				{"scriptBlock", _project.SelectedScriptBlock.ToString()},
				{"wordsInLine", approximateWordCount.ToString()}
			};
			UpdateDisplay();
		}

		public bool HidingSkippedBlocks
		{
			get { return _hidingSkippedBlocks; }
			set
			{
				_hidingSkippedBlocks = value;
				UpdateDisplayForAdminMode();
			}
		}

		private ScriptControl.Direction GetDirection()
		{
			if (_changingChapter)
				return ScriptControl.Direction.Forwards;

			return _previousLine < _project.SelectedScriptBlock
				? ScriptControl.Direction.Forwards
				: ScriptControl.Direction.Backwards;
		}

		public ScriptLine CurrentScriptLine
		{
			get { return GetScriptBlock(_project.SelectedScriptBlock); }
		}

		public ScriptLine PreviousScriptBlock
		{
			get { return GetScriptBlock(_project.SelectedScriptBlock - 1); }
		}

		public ScriptLine NextScriptBlock
		{
			get { return GetScriptBlock(_project.SelectedScriptBlock + 1); }
		}

		public ScriptLine GetScriptBlock(int index)
		{
			if (index < 0 || index >= _project.SelectedChapterInfo.GetScriptBlockCount())
				return null;
			return _project.SelectedBook.GetBlock(_project.SelectedChapterInfo.ChapterNumber1Based, index);
		}

		private void OnNextButton(object sender, EventArgs e)
		{
			int newSliderValue = _scriptSlider.Value + 1;
			_scriptSlider.Value = newSliderValue;
			_audioButtonsControl.UpdateButtonStateOnNavigate();
		}

		private void GoBack()
		{
			int newSliderValue = _scriptSlider.Value - 1;
			_scriptSlider.Value = newSliderValue;
			_audioButtonsControl.UpdateButtonStateOnNavigate();
		}

		private void _deleteRecordingButton_Click(object sender, EventArgs e)
		{
			OnDeleteRecording();
		}

		private void _deleteRecordingButton_MouseEnter(object sender, EventArgs e)
		{
			_deleteRecordingButton.Image = Resources.deleteHighlighted;
		}

		private void _deleteRecordingButton_MouseLeave(object sender, EventArgs e)
		{
			_deleteRecordingButton.Image = Resources.deleteNormal;
		}

		private void OnDeleteRecording()
		{
			if (ClipRepository.DeleteLineRecording(_project.Name, _project.SelectedBook.Name,
				_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock))
			{
				_project.SelectedChapterInfo.OnClipDeleted(CurrentScriptLine);
				OnSoundFileCreatedOrDeleted();
			}
		}

		private void OnSkipButtonCheckedChanged(object sender, EventArgs e)
		{
			if (_skipButton.Checked)
			{
				if (HaveRecording)
				{
					if (DialogResult.No ==
						MessageBox.Show(this,
							LocalizationManager.GetString("RecordingControl.ConfirmSkip",
								"There is already a recording for this line.\r\nIf you skip it, this recording will be omitted when publishing.\r\n\r\nAre you sure you want to do this?"),
							ProductName,
							MessageBoxButtons.YesNo))
						return;
					ClipRepository.BackUpRecordingForSkippedLine(_project.Name, _project.CurrentBookName,
						_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock);
				}
				CurrentScriptLine.Skipped = true;
				OnNextButton(sender, e);
			}
			else
			{
				CurrentScriptLine.Skipped = false;
				_scriptSlider.Refresh();
				_scriptControl.Invalidate();
			}
		}

		private void UpdateDisplayForAdminMode()
		{
			_scriptControl.ShowSkippedBlocks = _skipButton.Visible = !HidingSkippedBlocks;

			if (_project == null)
				return;

			int sliderValue = _scriptSlider.Value;
			var segmentCount = DisplayedSegmentCount;
			bool alreadyFinished = (sliderValue == segmentCount);
			ResetSegmentCount();

			if (alreadyFinished)
			{
				sliderValue = segmentCount;
			}
			else if (segmentCount == 0)
			{
				// Unusual case where all segments were skipped and are now being hidden
				sliderValue = 0;
			}
			else
			{
				if (HidingSkippedBlocks)
				{
					for (int i = 0; i < _project.SelectedScriptBlock; i++)
					{
						if (GetScriptBlock(i).Skipped)
							sliderValue--;
					}
					// We also need to subtract 1 for the selected block if it was skipped
					if (GetScriptBlock(_project.SelectedScriptBlock).Skipped)
						sliderValue--;
					if (sliderValue < 0)
					{
						// Look forward to find an unskipped block
						sliderValue = 0;
						while (sliderValue < segmentCount && GetScriptBlock(_project.SelectedScriptBlock + sliderValue + 1).Skipped)
							sliderValue++;
					}
				}
				else
				{
					sliderValue = GetScriptBlockIndexFromSliderValueByAccountingForPrecedingHiddenBlocks(sliderValue);
				}
			}
			if (_scriptSlider.Value == sliderValue)
			{
				UpdateScriptAndMessageControls();
				if (!_scriptSlider.Finished)
					UpdateSelectedScriptLine();
			}
			else
				_scriptSlider.Value = sliderValue;
		}

		private void OnSmallerClick(object sender, EventArgs e)
		{
			SetZoom(_scriptControl.ZoomFactor - 0.2f);
		}

		private void OnLargerClick(object sender, EventArgs e)
		{
			SetZoom(_scriptControl.ZoomFactor + 0.2f);
		}

		private void SetZoom(float newZoom)
		{
			var zoom = Math.Max(Math.Min(newZoom, 2.0f), 1.0f);
			Settings.Default.ZoomFactor = zoom;
			Settings.Default.Save();
			_scriptControl.ZoomFactor = zoom;
		}

		/// <summary>
		/// Shows or hides controls as appropriate based on whether user has advanced through all blocks in this chapter:
		/// responsible for the "End of (book)" messages and "Go To Chapter x" links.
		/// </summary>
		private void UpdateScriptAndMessageControls()
		{
			if (_scriptSlider.Finished)
			{
				HideScriptLines();
				// '>' is just paranoia
				if (_project.SelectedChapterInfo.ChapterNumber1Based >= _project.SelectedBook.ChapterCount)
				{
					ShowEndOfBook();
				}
				else
				{
					ShowEndOfChapter();
				}
				_skipButton.Enabled = false;
				_audioButtonsControl.HaveSomethingToRecord = false;
				_audioButtonsControl.UpdateDisplay();
				_segmentLabel.Visible = false;
				_lineCountLabel.Visible = false;
			}
			else
				ShowScriptLines();
		}

		private void ShowEndOfChapter()
		{
			if (_project.SelectedChapterInfo.RecordingsFinished)
			{
				_endOfUnitMessage.Text = Format(_chapterFinished, _chapterLabel.Text);
				_endOfUnitMessage.Visible = true;
			}
			else
				_endOfUnitMessage.Visible = false;
			_nextChapterLink.Text = Format(_gotoLink, GetNextChapterLabel());
			_nextChapterLink.Visible = true;
			_audioButtonsControl.CanGoNext = false;
		}

		private string GetNextChapterLabel()
		{
			return Format(GetChapterNumberString(), _project.GetNextChapterNum());
		}

		private void ShowEndOfBook()
		{
			_endOfUnitMessage.Text = Format(_endOfBook, _bookLabel.Text);
			_endOfUnitMessage.Visible = true;
		}

		private void ShowScriptLines()
		{
			_endOfUnitMessage.Visible = false;
			_nextChapterLink.Visible = false;
			_scriptControl.Visible = true;
			_audioButtonsControl.CanGoNext = true;
		}

		private void HideScriptLines()
		{
			_scriptControl.Visible = false;
		}

		private void OnNextChapterLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			_project.SelectedChapterInfo = _project.GetNextChapterInfo();
			UpdateSelectedChapter();
		}

		private void _breakLinesAtCommasButton_Click(object sender, EventArgs e)
		{
			Settings.Default.BreakLinesAtClauses = !Settings.Default.BreakLinesAtClauses;
			Settings.Default.Save();
			UpdateBreakClausesImage();
			_scriptControl.Invalidate();
		}

		private void UpdateBreakClausesImage()
		{
			_breakLinesAtCommasButton.Image =
				Settings.Default.BreakLinesAtClauses ? AppPallette.LineBreakCommaActiveImage : Resources.linebreakComma;
		}

		private void _breakLinesAtCommasButton_MouseEnter(object sender, EventArgs e)
		{
			_breakLinesAtCommasButton.BackColor = AppPallette.MouseOverButtonBackColor;
		}

		private void _breakLinesAtCommasButton_MouseLeave(object sender, EventArgs e)
		{
			_breakLinesAtCommasButton.BackColor = AppPallette.Background;
		}

		private void _longLineButton_MouseLeave(object sender, EventArgs e)
		{
			_longLineButton.BackColor = AppPallette.Background;
		}

		private void _longLineButton_MouseEnter(object sender, EventArgs e)
		{
			_longLineButton.BackColor = AppPallette.MouseOverButtonBackColor;
		}

		public class NoBorderToolStripRenderer : ToolStripProfessionalRenderer
		{
			protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
			{
			}
		}

		private int GetScriptBlockIndexFromSliderValueByAccountingForPrecedingHiddenBlocks(int sliderValue)
		{
			for (int i = 0; i <= sliderValue; i++)
			{
				var block = GetScriptBlock(i);
				if (block == null) // passed the end of the list. All were skipped.
					return sliderValue;
				if (block.Skipped)
					sliderValue++;
			}
			return sliderValue;
		}

		private void longLineButton_Click(object sender, EventArgs e)
		{
			using (var dlg = new RecordInPartsDlg())
			{
				var scriptLine = _project.GetBlock(_project.CurrentBookName, _project.SelectedChapterInfo.ChapterNumber1Based,
					_project.SelectedScriptBlock);
				dlg.TextToRecord = scriptLine.Text;
				dlg.RecordingDevice = _audioButtonsControl.RecordingDevice;
				dlg.ContextForAnalytics = _audioButtonsControl.ContextForAnalytics;
				dlg.VernacularFont = new Font(scriptLine.FontName, scriptLine.FontSize * _scriptControl.ZoomFactor);
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					dlg.WriteCombinedAudio(_project.GetPathToRecordingForSelectedLine());
					OnSoundFileCreated(this, new EventArgs());
				}
			}
		}
	}
}
