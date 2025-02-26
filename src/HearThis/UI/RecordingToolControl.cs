// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
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
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using SIL.Code;
using SIL.Media.Naudio;
using SIL.ObjectModel;
using SIL.Reporting;
using SIL.Windows.Forms.SettingProtection;
using static System.String;
using static HearThis.UI.AdministrativeSettings;

namespace HearThis.UI
{
	public partial class RecordingToolControl : UserControl, IMessageFilter, ISupportInitialize, ILocalizable
	{
		private const string kEmDashToIndicateExtraRecording = "\u2014";
		private Project _project;
		private Mode _currentMode;
		private int _previousLine = -1;
		private readonly int _numberOfColumnsThatScriptControlSpans;
		private string _lineCountLabelFormat;
		private bool _changingChapter;
		private SynchronizationContext _uiSynchronizationContext;
		private Thread _uiSynchronizationThread;

		private readonly string _endOfBook = LocalizationManager.GetString("RecordingControl.EndOf", "End of {0}",
			"{0} is typically a book name");
		private readonly string _chapterFinished = LocalizationManager.GetString("RecordingControl.Finished", "{0} Finished",
			"{0} is a chapter number");
		private readonly string _gotoLink = LocalizationManager.GetString("RecordingControl.GoTo", "Go To {0}",
			"{0} is a chapter number");
		private bool _showingSkipButton;

		public void SetMode(Mode newValue)
		{
			if (_currentMode == newValue)
				return;

			_currentMode = newValue;

			var s = _tableLayoutScript.RowStyles[_tableLayoutScript.GetRow(_scriptControl)];

			switch (newValue)
			{
				case Mode.ReadAndRecord:
					_scriptTextHasChangedControl.Hide();
					tableLayoutPanel1.SetColumnSpan(_tableLayoutScript, _numberOfColumnsThatScriptControlSpans);
					s.SizeType = SizeType.Percent;
					s.Height = 1;
					_scriptControl.GoToScript(GetDirection(), PreviousScriptBlock, CurrentScriptLine, NextScriptBlock);
					_scriptControl.Show();
					_audioButtonsControl.Show();
					_peakMeter.Show();
					_recordInPartsButton.Show();
					_breakLinesAtCommasButton.Show();
					_project.RefreshExtraClips();
					UpdateDisplay();
					RefreshBookAndChapterButtonProblemState(false);
					break;
				case Mode.CheckForProblems:
					_scriptControl.Hide();
					_audioButtonsControl.Hide();
					_peakMeter.Hide();
					_scriptTextHasChangedControl.SetData(_project);
					tableLayoutPanel1.SetColumnSpan(_tableLayoutScript, tableLayoutPanel1.ColumnCount);
					s.SizeType = SizeType.AutoSize;
					_recordInPartsButton.Hide();
					_breakLinesAtCommasButton.Hide();
					_deleteRecordingButton.Hide();
					foreach (BookButton bookBtn in _bookFlow.Controls)
						bookBtn.Visible = true;
					if (!_project.DoesCurrentSegmentHaveProblem() &&
					    // On initial reload after changing the color scheme, we don't want to move
					    // the user off the segment they were on.
					    !Program.RestartedToChangeColorScheme)
					{
						if (TrySelectFirstChapterWithProblem())
							UpdateSelectedChapter();
						else if (TrySelectFirstBookWithProblem())
							UpdateSelectedBook();
						else
						{
							MessageBox.Show(this, Format(
									LocalizationManager.GetString("RecordingControl.NoProblemsInProject",
										"{0} did not detect any problems in this project.",
										"Param 0: \"HearThis\" (program name)"),
									Program.kProduct),
								Program.kProduct);
						}
					}

					RefreshBookAndChapterButtonProblemState(true);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			_scriptSlider.Invalidate();
			UpdateScriptAndMessageControls();
		}

		public RecordingToolControl()
		{
			InitializeComponent();
			_numberOfColumnsThatScriptControlSpans = tableLayoutPanel1.GetColumnSpan(_tableLayoutScript);
			SetZoom(Settings.Default.ZoomFactor); // do after InitializeComponent sets it to 1.
			SettingsProtectionSettings.Default.PropertyChanged += OnSettingsProtectionChanged;
			Program.RegisterLocalizable(this);
			HandleStringsLocalized();
			BackColor = AppPalette.Background;
			_bookLabel.ForeColor = AppPalette.TitleColor;
			_chapterLabel.ForeColor = AppPalette.TitleColor;
			_segmentLabel.ForeColor = AppPalette.TitleColor;
			_lineCountLabel.ForeColor = AppPalette.TitleColor;
			_segmentLabel.BackColor = AppPalette.Background;
			_lineCountLabel.BackColor = AppPalette.Background;
			_skipButton.ForeColor = AppPalette.HilightColor; // Only used (for border) when UseForeColorForBorder
			_panelRecordingDeviceButton.BackColor =
				_panelRecordingDeviceBorder.BackColor = BackColor;

			recordingDeviceButton1.NoAudioDeviceImage = Resources.Audio_NoAudioDevice;
			recordingDeviceButton1.WebcamImage = Resources.Audio_Webcam;
			recordingDeviceButton1.ComputerInternalImage = Resources.Audio_Computer;
			recordingDeviceButton1.UsbAudioDeviceImage = Resources.Audio_Headset;
			recordingDeviceButton1.LineImage = Resources.Audio_Line;
			recordingDeviceButton1.MicrophoneImage = Resources.Audio_Microphone;
			recordingDeviceButton1.RecorderImage = Resources.Audio_Recorder;
			recordingDeviceButton1.KnownHeadsetImage = Resources.Audio_Headset;

			//_upButton.Initialize(Resources.up, Resources.upDisabled);
			//_nextButton.Initialize(Resources.down, Resources.downDisabled);

			if (DesignMode)
				return;

			_peakMeter.Start(33); //the number here is how often it updates
			_peakMeter.ColorMedium = AppPalette.Blue;
			_peakMeter.ColorNormal = AppPalette.EmptyBoxColor;
			_peakMeter.ColorHigh = AppPalette.Red;
			_peakMeter.SetRange(5, 80, 100);
			AudioButtonsControl.Recorder.PeakLevelChanged += ((s, e) => _peakMeter.PeakLevel = e.Level);
			recordingDeviceButton1.Recorder = AudioButtonsControl.Recorder;

			MouseWheel += OnRecordingToolControl_MouseWheel;

			_nextChapterLink.ActiveLinkColor = AppPalette.HilightColor;
			_nextChapterLink.DisabledLinkColor = AppPalette.NavigationTextColor;
			_nextChapterLink.LinkColor = AppPalette.HilightColor;

			_audioButtonsControl.SoundFileRecordingComplete += OnSoundFileCreated;
			_audioButtonsControl.RecordingStarting += OnAudioButtonsControlRecordingStarting;

			_breakLinesAtCommasButton.Checked = Settings.Default.BreakLinesAtClauses;

			_lineCountLabel.ForeColor = AppPalette.NavigationTextColor;

			_btnUndelete.Location = _deleteRecordingButton.Location;
		}

		public void HandleStringsLocalized()
		{
			_lineCountLabelFormat = _lineCountLabel.Text;
			SetChapterLabelIfIntroduction();
			UpdateUiStringsForCurrentScriptLine();
		}

		internal void SetGetUIString(Func<UiElement, string> func)
		{
			_scriptTextHasChangedControl.GetUIString = func;
		}

		private void OnAudioButtonsControlRecordingStarting(object sender, CancelEventArgs cancelEventArgs)
		{
			if (SelectedBlockHasSkippedStyle)
			{
				var fmt = LocalizationManager.GetString("RecordingControl.CannotRecordClipForSkippedStyle",
					"The settings for this project prevent recording this block because its paragraph style is {0}. If " +
					"you intend to record blocks having this style, in the Settings dialog box, select the Skipping page, " +
					"and then clear the selection for this style.");
				MessageBox.Show(this, Format(fmt, _project.ScriptOfSelectedBlock.ParagraphStyle), Program.kProduct);
				cancelEventArgs.Cancel = true;
			}
			else if (CurrentScriptLine.Skipped)
			{
				var fmt = LocalizationManager.GetString("RecordingControl.CannotRecordSkippedClip",
					"This block has been skipped. If you want to record a clip for this block, first click the Skip button " +
					"so that it is no longer selected.");
				MessageBox.Show(this, Format(fmt, _project.ScriptOfSelectedBlock.ParagraphStyle), Program.kProduct);
				cancelEventArgs.Cancel = true;
			}
			else
				_scriptControl.RecordingInProgress = true;
		}

		private void OnRecordButtonStateChanged(object sender, BtnState newState)
		{
			if (!_scriptControl.RecordingInProgress)
				_scriptControl.UserPreparingToRecord = newState == BtnState.MouseOver &&
					!SelectedBlockHasSkippedStyle && !CurrentScriptLine.Skipped;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			if (recordingDeviceButton1.Recorder.SelectedDevice == null)
			{
				ReportNoMicrophone(null, null);
				// Note: We used to immediately kill HearThis if no recording device was found,
				// but this seems unnecessarily extreme. Give them a chance to hook up a mic. Even
				// if they don't, they could publish existing stuff or review existing recordings.
				//Environment.Exit(1);
			}

			if (FindForm() is Shell shell)
			{
				shell.ProjectChanged += (sender, args) => SetProject(((Shell)sender).Project);
				shell.ModeChanged += (sender, mode) => SetMode(mode);
			}

			_uiSynchronizationContext = SynchronizationContext.Current;
			_uiSynchronizationThread = Thread.CurrentThread;
			Application.AddMessageFilter(this);
		}

		private void OnSettingsProtectionChanged(object sender, PropertyChangedEventArgs e)
		{
			//when we need to use Ctrl+Shift to display stuff, we don't want it also firing up the localization dialog (which shouldn't be done by a user under settings protection anyhow)
			LocalizationManager.EnableClickingOnControlToBringUpLocalizationDialog =
				!SettingsProtectionSettings.Default.NormallyHidden;
		}

		private void OnSoundFileCreated(AudioButtonsControl sender, Exception error)
		{
			_scriptControl.RecordingInProgress = false;
			if (error == null)
			{
				try
				{
					_project.HandleSoundFileCreated();
				}
				catch (FileNotFoundException)
				{
					ErrorReport.NotifyUserOfProblem(LocalizationManager.GetString("RecordingControl.InvalidWavFile",
						"Something went wrong recording that clip. Please try again. If this continues to happen, contact support."));
				}

				OnSoundFileCreatedOrDeleted();

				if (_currentMode == Mode.CheckForProblems)
					_scriptTextHasChangedControl.UpdateState();
			}
		}

		private void OnSoundFileCreatedOrDeleted()
		{
			_scriptSlider.Refresh();
			// deletion is done in LineRecordingRepository and affects audioButtons
			_chapterFlow.Controls.Cast<ChapterButton>().FirstOrDefault(b => b.Selected)?.RecalculatePercentageRecorded();
			if (_currentMode == Mode.ReadAndRecord)
				UpdateDisplay();
		}

		public void UpdateAfterMerge()
		{
			_scriptSlider.Refresh();
			foreach (ChapterButton chapterButton in _chapterFlow.Controls)
			{
				chapterButton.RecalculatePercentageRecorded();
			}
			UpdateDisplay();
			foreach (BookButton bookButton in _bookFlow.Controls)
			{
				bookButton.Invalidate();
			}
			_scriptSlider.Refresh();
		}

		private void OnRecordingToolControl_MouseWheel(object sender, MouseEventArgs e)
		{
			//the minus here is because down (negative) on the wheel equates to addition on the horizontal slider
			_scriptSlider.Value += e.Delta / -120;
		}

		private void SetProject(Project project)
		{
			if (_project != null)
			{
				_project.ScriptBlockRecordingRestored -= HandleScriptBlockRecordingRestored;
				_project.SkippedStylesChanged -= HandleSkippedStylesChanged;
				_project.SelectedBookChanged -= HandleSelectedBookChanged;
				_project.ExtraClipsCollectionChanged -= HandleExtraClipCollectionChanged;
			}

			_project = project;

			_scriptControl.SetFont(_project.FontName);
			_scriptControl.SetClauseSeparators(_project.ProjectSettings.ClauseBreakCharacterSet);

			_project.ScriptBlockRecordingRestored += HandleScriptBlockRecordingRestored;
			_project.SkippedStylesChanged += HandleSkippedStylesChanged;
			_project.ExtraClipsCollectionChanged += HandleExtraClipCollectionChanged;

			_bookFlow.Controls.Clear();
			foreach (BookInfo bookInfo in project.Books)
			{
				var x = new BookButton(bookInfo, Settings.Default.DisplayNavigationButtonLabels);
				_instantToolTip.SetToolTip(x, bookInfo.LocalizedName);
				_bookFlow.Controls.Add(x);
				BookInfo bookInfoToAvoidClosureProblem = bookInfo;
				x.Click += delegate { _project.SelectedBook = bookInfoToAvoidClosureProblem; };
				x.MouseEnter += HandleNavigationArea_MouseEnter;
				x.MouseLeave += HandleNavigationArea_MouseLeave;
				if (bookInfo.BookNumber == BibleStatsBase.kCountOfOTBooks - 1)
					_bookFlow.SetFlowBreak(x, true);

				if (bookInfo == _project.SelectedBook)
					x.Selected = true;
			}
			_project.LoadBook(_project.SelectedBook.BookNumber);
			_scriptSlider.GetSegmentBrushesDelegate = GetSegmentBrushes;
			UpdateSelectedBook();
			_project.SelectedBookChanged += HandleSelectedBookChanged;
		}

		/// <summary>
		/// Select the first block (for the current actor/character) that is not already recorded.
		/// Currently does not change anything if all are recorded; eventually, we may want it
		/// to return a boolean indicating failure so we can consider switching to the next character.
		/// </summary>
		private void SelectFirstUnrecordedBlock()
		{
			foreach (var bookInfo in _project.Books)
			{
				for (int chapter = 0; chapter < bookInfo.ChapterCount; chapter++)
				{
					var blockCount = bookInfo.ScriptProvider.GetScriptBlockCount(bookInfo.BookNumber, chapter);
					for (int blockNum = 0; blockNum < blockCount; blockNum++)
					{
						var block = bookInfo.ScriptProvider.GetBlock(bookInfo.BookNumber, chapter, blockNum);
						if (block.Skipped)
							continue;
						if (!ClipRepository.HasClip(_project.Name, bookInfo.Name, chapter, blockNum, _project.ScriptProvider))
						{
							_project.SelectedBook = bookInfo;
							_project.SelectedChapterInfo = bookInfo.GetChapter(chapter);
							Settings.Default.Block = block.Number - 1; // want index into unfiltered list
							_previousLine = -1; // Allows Settings.Default.Block to take effect
							return;
						}
					}
				}
			}
		}

		/// <summary>
		/// There has been a change in the actor/character selection in the script provider.
		/// </summary>
		internal void UpdateForActorCharacter()
		{
			// See https://community.scripture.software.sil.org/t/not-all-of-the-text-is-visible/4116/3
			ShowBookButtonsOnlyForTestamentsWithContent();
			SelectFirstUnrecordedBlock();
			UpdateSelectedBook();
			Invalidate(); // makes book labels update.
		}

		/// <summary>
		/// Show/hide the book buttons for the OT and/or NT conditionally based on whether any book
		/// in that testament has content. In the case of a multivoice script (which is currently
		/// the only situation where we call this), it only considers whether there is content for
		/// the currently selected actor/character. The point of doing this is to make a little more
		/// screen space available for displaying the script, but note that this can have a mildly
		/// jarring visual effect.
		/// </summary>
		/// <remarks>See also the comment in the <see cref="Project"/> constructor.</remarks>
		private void ShowBookButtonsOnlyForTestamentsWithContent()
		{
			var showOTBooks = _project.Books.Take(BibleStatsBase.kCountOfOTBooks).Any(b => b.HasTranslatedContent);
			for (var i = 0; i < Math.Min(_bookFlow.Controls.Count, BibleStatsBase.kCountOfOTBooks); i++)
				((BookButton)_bookFlow.Controls[i]).Visible = showOTBooks;
			var showNTBooks = !showOTBooks ||
				_project.Books.Skip(BibleStatsBase.kCountOfOTBooks).Any(b => b.HasTranslatedContent);
			for (var i = BibleStatsBase.kCountOfOTBooks; i < _bookFlow.Controls.Count; i++)
				((BookButton)_bookFlow.Controls[i]).Visible = showNTBooks;
		}

		public bool MicCheckingEnabled
		{
			set
			{
				if (recordingDeviceButton1 != null)
					recordingDeviceButton1.MicCheckingEnabled = value;
			}
		}
		private BookButton SelectedBookButton => _bookFlow.Controls.OfType<BookButton>().SingleOrDefault(b => b.BookNumber == _project.SelectedBook.BookNumber);

		private void HandleSelectedBookChanged(object sender, EventArgs e)
		{
			Debug.Assert(_project == sender);
			BookButton selected = SelectedBookButton;

			foreach (BookButton button in _bookFlow.Controls)
				button.Selected = button == selected;
			
			UpdateSelectedBook();
		}

		private void HandleScriptBlockRecordingRestored(Project sender, int bookNumber, int chapterNumber, ScriptLine scriptBlock)
		{
			if (bookNumber == _project.SelectedBook.BookNumber && chapterNumber == _project.SelectedChapterInfo.ChapterNumber1Based)
				OnSoundFileCreatedOrDeleted();
		}

		private void HandleSkippedStylesChanged(Project sender, string stylename, bool newskipvalue)
		{
			_scriptSlider.Invalidate();
		}

		private int DisplayedSegmentCount
		{
			get
			{
				Guard.AgainstNull(_project, "project");
				return _project.TotalCountOfBlocksAndExtraClipsForChapter;
			}
		}

		private SegmentPaintInfo[] GetSegmentBrushes()
		{
			var results = new SegmentPaintInfo[DisplayedSegmentCount];
			int iBrush = 0;
			for (var i = 0; i < results.Length; i++)
			{
				var scriptLine = _project.GetUnfilteredBlock(i);
				if (scriptLine == null)
				{
					var hasRecordedClip = HasRecordedClip(i);
					results[iBrush++] = new SegmentPaintInfo
					{
						MainBrush = AppPalette.DisabledBrush,
						UnderlineBrush = hasRecordedClip ? AppPalette.HighlightBrush : AppPalette.DisabledBrush,
						PaintIconDelegate = hasRecordedClip ? (Action<Graphics, Rectangle, bool>)
							((g, r, selected) => r.DrawExclamation(g, selected ? AppPalette.BlueBrush : AppPalette.HighlightBrush)) :
							(g, r, selected) => r.DrawDot(g, IconBrush(selected)),
					};
				}
				else
				{
					var isLineCurrentlyRecordable = _project.IsLineCurrentlyRecordable(_project.SelectedBook.BookNumber,
						_project.SelectedChapterInfo.ChapterNumber1Based, i);
					// The main bar will be drawn blue if there is something to record; otherwise leave the background
					// bar color showing.
					var mainBrush = isLineCurrentlyRecordable ? AppPalette.BlueBrush : AppPalette.DisabledBrush;
					if (scriptLine.Skipped)
					{
						// NB: Skipped segments only get entries in the array of brushes if they are being shown(currently always, previously in "Admin" mode).
						// If we are ever again hiding skipped segments, then we need to avoid putting these segments into the collection.
						if (!HidingSkippedBlocks)
						{
							if (_currentMode == Mode.CheckForProblems && HasRecordedClip(i))
								results[iBrush++] = new SegmentPaintInfo
								{
									MainBrush = mainBrush,
									PaintIconDelegate = (g, r, selected) => r.DrawExclamation(g, AppPalette.HighlightBrush)
								};
							else
								results[iBrush++] = new SegmentPaintInfo {MainBrush = mainBrush, Symbol = "/"};
						}
					}
					else
					{
						var seg = new SegmentPaintInfo {MainBrush = mainBrush};
						results[iBrush++] = seg;
						if (isLineCurrentlyRecordable && HasRecordedClip(i))
						{
							seg.UnderlineBrush = AppPalette.HighlightBrush;
							if (_currentMode == Mode.CheckForProblems)
							{
								if (_project.DoesSegmentHaveProblems(i))
								{
									seg.PaintIconDelegate = (g, r, selected) => r.DrawExclamation(g, AppPalette.HighlightBrush);
								}
								else if (_project.DoesSegmentHaveIgnoredProblem(i))
								{
									seg.PaintIconDelegate = (g, r, selected) => r.DrawDot(g, IconBrush(selected));
								}
							}
						}
					}
				}
			}
			return results;
		}

		private Brush IconBrush(bool selected) => selected ? AppPalette.HighlightBrush : AppPalette.DisabledBrush;

		private bool HasRecordedClip(int i) => _project.HasRecordedClip(i);

		private void UpdateDisplay()
		{
			_skipButton.Enabled = HaveScript &&
				(!(_project.ScriptProvider is ISkippedStyleInfoProvider skippedStyleInfoProvider) ||
					!skippedStyleInfoProvider.IsSkippedStyle(CurrentScriptLine.ParagraphStyle));

			// We need to update the _audioButtonsControl (following two lines of code) even in
			// Check for problems mode because the state of that control affects whether or not the
			// user can use the space bar to attempt to record.
			// Technically in overview mode we have something to record but we're not allowed to record it.
			// Pretending we don't have something produces the desired effect of disabling the Record button.
			// Similarly if the current block is not recordable.
			_audioButtonsControl.HaveSomethingToRecord = HaveScript && !InOverviewMode
				&& _project.IsLineCurrentlyRecordable(_project.SelectedBook.BookNumber, _project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock);
			_audioButtonsControl.UpdateDisplay();

			if (_currentMode != Mode.ReadAndRecord)
				return;
			_scriptControl.RecordingInProgress = _audioButtonsControl.Recording;
			_deleteRecordingButton.Visible = HaveRecording;
			_btnUndelete.Visible = !_deleteRecordingButton.Visible && _project.HasBackupFileForSelectedBlock();

			_recordInPartsButton.Enabled = HaveScript && !_skipButton.Checked;

			_audioButtonsControl.ButtonHighlightMode = _skipButton.Checked ?
				AudioButtonsControl.ButtonHighlightModes.SkipRecording :
				AudioButtonsControl.ButtonHighlightModes.Default;
		}

		// We're in 'overview' mode if we're dealing with actor/character information but haven't chosen one.
		private bool InOverviewMode => _project.ActorCharacterProvider != null && _project.ActorCharacterProvider.Character == null;

		private bool SelectedBlockHasSkippedStyle => ScriptLine.SkippedStyleInfoProvider.IsSkippedStyle(
			_project.ScriptOfSelectedBlock.ParagraphStyle);

		private bool HaveRecording => _project.HasRecordedClipForSelectedScriptLine();

		// This method is much more reliable for single line sections than comparing slider max & min
		private bool HaveScript => CurrentScriptLine?.Text?.Length > 0;

		/// <summary>
		/// Filter out all keystrokes except the few that we want to handle.
		/// We handle Space, TAB, PageUp, PageDown, Delete and Arrow keys.
		/// </summary>
		/// <remarks>This is invoked because we implement IMessageFilter and call Application.AddMessageFilter(this)</remarks>
		public bool PreFilterMessage(ref Message m)
		{
			const int WM_KEYDOWN = 0x100;
			const int WM_KEYUP = 0x101;

			if ((m.Msg != WM_KEYDOWN && m.Msg != WM_KEYUP) || !(FindForm()?.ContainsFocus ?? false))
				return false;

			if (m.Msg == WM_KEYUP && (Keys) m.WParam != Keys.Space)
				return false;

			var keys = (Keys)m.WParam;

			if (_currentMode == Mode.CheckForProblems &&
			    _scriptTextHasChangedControl.PreFilterKey(keys))
			{
				return true;
			}

			switch (keys)
			{
				case Keys.OemPeriod:
				case Keys.Decimal:
					if (_audioButtonsControl.HaveSomethingToRecord)
						MessageBox.Show(LocalizationManager.GetString("RecordingControl.PushTabToPlay",
						"To play the clip, press the Tab key."));
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
					// If we open the dialog directly from within this method, it doesn't work to
					// install a new Message Filter.
					InvokeLaterOnUIThread(() => longLineButton_Click(this, EventArgs.Empty));
					break;

				case Keys.Delete:
					if (_deleteRecordingButton.Visible && _deleteRecordingButton.Enabled)
						DeleteRecording();
					break;

				default:
					return false;
			}

			return true;
		}

		private void UpdateSelectedBook()
		{
			_bookLabel.Text = _project.SelectedBook.LocalizedName;

			_chapterFlow.SuspendLayout();
			foreach (ChapterButton btn in _chapterFlow.Controls)
				btn.RecordingCompleteChanged -= HandleChapterRecordingsCompleteChanged;
			_chapterFlow.Controls.Clear();

			var buttons = new List<ChapterButton>();

			//note: we're using chapter 0 to mean the material at the start of the book
			for (int i = 0; i <= _project.SelectedBook.ChapterCount; i++)
			{
				var chapterInfo = _project.SelectedBook.GetChapter(i);
				if (i == 0 && chapterInfo.IsEmpty && !chapterInfo.GetExtraClips().Any())
					continue;

				var button = new ChapterButton(chapterInfo, _project.SelectedBook.GetChapter) {ShowProblems = _currentMode == Mode.CheckForProblems};
				button.Click += OnChapterClick;
				button.MouseEnter += HandleNavigationArea_MouseEnter;
				button.MouseLeave += HandleNavigationArea_MouseLeave;
				button.RecordingCompleteChanged += HandleChapterRecordingsCompleteChanged;
				buttons.Add(button);
				_instantToolTip.SetToolTip(button, i == 0 ? GetIntroductionString() : Format(GetChapterNumberString(), i));
			}
			_chapterFlow.Controls.AddRange(buttons.ToArray());
			_chapterFlow.ResumeLayout(true);
			if (_currentMode == Mode.CheckForProblems)
			{
				TrySelectFirstChapterWithProblem();
			}
			else if (_project.CurrentCharacter != null)
				_project.SelectedChapterInfo = GetFirstUnrecordedChapter();

			UpdateSelectedChapter();
		}

		private void HandleChapterRecordingsCompleteChanged(object sender, EventArgs e)
		{
			SelectedBookButton?.RecalculatePercentageRecorded();
		}

		private ChapterInfo GetFirstUnrecordedChapter()
		{
			var bookInfo = _project.SelectedBook;
			var provider = _project.ActorCharacterProvider;
			if (bookInfo == null || provider == null)
				return _project.SelectedChapterInfo;
			var id = provider.GetNextUnrecordedChapterForCharacter(bookInfo.BookNumber, bookInfo.GetFirstChapter().ChapterNumber1Based);
			return bookInfo.GetChapter(id);
		}

		private bool TrySelectFirstChapterWithProblem(BookInfo book = null)
		{
			if (book == null)
				book = _project.SelectedBook;

			// Find first chapter with an unresolved problem.
			for (var i = 0; i < book.ChapterCount; i++)
			{
				var chapterInfo = book.GetChapter(i);
				if (chapterInfo.HasUnresolvedProblem)
				{
					if (_project.SelectedBook?.BookNumber != book.BookNumber)
						_project.SelectedBook = book;
					_project.SelectedChapterInfo = chapterInfo;
					return true;
				}
			}

			return false;
		}

		private bool TrySelectFirstBookWithProblem() => _project.Books.Any(TrySelectFirstChapterWithProblem);

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
			var newChapter = ((ChapterButton)sender).ChapterInfo;
			if (newChapter.ChapterNumber1Based == _project.SelectedChapterInfo.ChapterNumber1Based)
				return;
			_project.SelectedChapterInfo = newChapter;
			UpdateSelectedChapter();
		}

		private void UpdateSelectedChapter()
		{
			foreach (ChapterButton chapterButton in _chapterFlow.Controls)
				chapterButton.Selected = false;

			if (!SetChapterLabelIfIntroduction())
				_chapterLabel.Text = Format(GetChapterNumberString(), _project.SelectedChapterInfo.ChapterNumber1Based);

			ChapterButton button = (from ChapterButton control in _chapterFlow.Controls
				where control.ChapterInfo.ChapterNumber1Based == _project.SelectedChapterInfo.ChapterNumber1Based
				select control).Single();

			button.Selected = true;
			_project.RefreshExtraClips();
			UpdateControlsForSelectedChapter();
			_changingChapter = true;
			if (HidingSkippedBlocks)
				_project.SelectedScriptBlock = GetScriptBlockIndexFromSliderValueByAccountingForPrecedingHiddenBlocks(0);

			// If this is the initial load of the project, try to return to the block where user left off.
			int targetBlock = (_previousLine == -1 && Settings.Default.Block >= 0 && Settings.Default.Block < DisplayedSegmentCount) ?
				Settings.Default.Block : 0;

			if (targetBlock == 0 && _currentMode == Mode.CheckForProblems)
			{
				targetBlock = _project.SelectedChapterInfo.IndexOfFirstUnfilteredBlockWithProblem;
				if (targetBlock < 0)
					targetBlock = 0; // REVIEW: Do we want to bounce out to look at other chapters in the book or even other books in the project?
			}
			else if (_project.CurrentCharacter != null)
				targetBlock = GetFirstUnrecordedBlock(targetBlock);

			if (_scriptSlider.Value == targetBlock)
				UpdateForSelectedBlock();
			else
				_scriptSlider.Value = targetBlock;

			_changingChapter = false;
		}

		private bool SetChapterLabelIfIntroduction()
		{
			if (_project?.SelectedChapterInfo == null || _project.SelectedChapterInfo.ChapterNumber1Based > 0)
				return false;
			_chapterLabel.Text = Format(GetIntroductionString());
			return true;
		}

		private int GetFirstUnrecordedBlock(int startLine)
		{
			var bookInfo = _project.SelectedBook;
			var chapterInfo = _project.SelectedChapterInfo;
			var provider = _project.ActorCharacterProvider;
			if (bookInfo == null || chapterInfo == null || provider == null)
				return startLine;
			return provider.GetNextUnrecordedLineInChapterForCharacter(bookInfo.BookNumber, chapterInfo.ChapterNumber1Based,
				startLine);
		}

		private void OnLineSlider_ValueChanged(object sender, EventArgs e)
		{
			Settings.Default.Block = _scriptSlider.Value;
			UpdateForSelectedBlock();
		}

		private void UpdateForSelectedBlock()
		{
			int sliderValue = _scriptSlider.Value;

			if (_scriptSlider.Finished)
				_project.SelectedScriptBlock = _project.LineCountForChapter;
			else
			{
				if (HidingSkippedBlocks)
					sliderValue = GetScriptBlockIndexFromSliderValueByAccountingForPrecedingHiddenBlocks(sliderValue);
				_project.SelectedScriptBlock = sliderValue;
				UpdateSelectedScriptLine();
			}
			UpdateScriptAndMessageControls();
		}

		private void UpdateSelectedScriptLine()
		{
			var currentScriptLine = CurrentScriptLine;
			_segmentLabel.Visible = true;
			_skipButton.CheckedChanged -= OnSkipButtonCheckedChanged;
			_skipButton.Checked = _skipButton.UseForeColorForBorder = currentScriptLine != null && currentScriptLine.Skipped;
			_skipButton.CheckedChanged += OnSkipButtonCheckedChanged;

			if (DisplayedSegmentCount == 0)
				_project.SelectedScriptBlock = 0; // This should already be true, but just make sure;

			if (_currentMode == Mode.ReadAndRecord)
				_scriptControl.GoToScript(GetDirection(), PreviousScriptBlock, currentScriptLine, NextScriptBlock);
			else
				_scriptTextHasChangedControl.SetData(_project);

			_previousLine = _project.SelectedScriptBlock;

			if (!_scriptSlider.IsFullyInitialized || _project.TotalCountOfBlocksAndExtraClipsForChapter == 0)
			{
				_audioButtonsControl.Path = null;
			}
			else
			{
				_audioButtonsControl.Path = _project.ClipFilePathForSelectedLine;
				_audioButtonsControl.ContextForAnalytics = _project.GetAudioContextInfoForAnalytics(currentScriptLine);
			}

			UpdateUiStringsForCurrentScriptLine();

			UpdateDisplay();
		}

		private void UpdateUiStringsForCurrentScriptLine()
		{
			var currentScriptLine = CurrentScriptLine;
			string verse = currentScriptLine?.Verse;
			bool isRealVerseNumber = !IsNullOrEmpty(verse) && verse != "0";

			if (_project == null)
			{
				_lineCountLabel.Visible = false;
			}
			else
			{
				_lineCountLabel.Text = _scriptSlider.Value < _project.LineCountForChapter ?
					Format(_lineCountLabelFormat, _scriptSlider.Value + 1, _project.LineCountForChapter) :
					LocalizationManager.GetString("RecordingControl.UnexpectedRecording", "Extra clip");
				_lineCountLabel.Visible = true;
			}

			if (HaveScript)
			{
				Debug.Assert(currentScriptLine != null);
				if (currentScriptLine.Heading)
					_segmentLabel.Text = LocalizationManager.GetString("RecordingControl.Heading", "Heading");
				else if (isRealVerseNumber)
				{
					int firstBridgeChar = verse.IndexOfAny(new[] {'-', '~'});
					int lastBridgeChar = verse.LastIndexOfAny(new[] {'-', '~'});
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
					_segmentLabel.Text = currentScriptLine == null ?
						kEmDashToIndicateExtraRecording :
						LocalizationManager.GetString("RecordingControl.NotTranslated", "Not translated yet");
				}
			}
		}

		private bool HidingSkippedBlocks => false; // Currently we never want to do this. Some of the newer code doesn't handle it.

		public bool ShowingSkipButton
		{
			get => _showingSkipButton;
			set
			{
				_showingSkipButton = value;
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

		private ScriptLine CurrentScriptLine => _project?.GetUnfilteredBlock(_project.SelectedScriptBlock);

		/// <summary>
		/// Used for displaying context to the reader, this is the previous block in the actual (unfiltered) text.
		/// </summary>
		private ScriptLine PreviousScriptBlock => GetContextLine(true);

		/// <summary>
		/// Used for displaying context to the reader, this is the next block in the actual (unfiltered) text.
		/// </summary>
		private ScriptLine NextScriptBlock => GetContextLine(false);

		private ScriptLine GetContextLine(bool preceding)
		{
			var currentBlockNum = CurrentScriptLine?.Number;
			if (currentBlockNum == null)
				return null;
			var realIndex = (int)currentBlockNum - 1;
			return _project.ScriptProvider.GetUnfilteredBlock(_project.SelectedBook.BookNumber,
				_project.SelectedChapterInfo.ChapterNumber1Based, realIndex + (preceding ? -1 : 1));
		}

		private void OnNextButton(object sender, EventArgs e)
		{
			int newSliderValue = (sender is ScriptTextHasChangedControl s && s.HasMoreProblemsInChapter) ?
				_project.SelectedChapterInfo.GetIndexOfNextUnfilteredBlockWithProblem(_scriptSlider.Value) :
				_scriptSlider.Value + 1;
			if (_project.ActorCharacterProvider?.Character != null)
			{
				// Advance to the next block this character can record, or the end of the chapter
				// If we return to supporting HidingSkippedBlocks, this probably needs adjusting
				// to make sure the index we pass to IsLineCurrentlyRecordable allows for previous skipped lines.
				while (newSliderValue < DisplayedSegmentCount &&
					   !_project.IsLineCurrentlyRecordable(_project.SelectedBook.BookNumber,
						   _project.SelectedChapterInfo.ChapterNumber1Based, newSliderValue))
				{
					newSliderValue++;
				}
			}

			if (_currentMode == Mode.ReadAndRecord || _scriptSlider.SegmentCount > newSliderValue)
			{
				_scriptSlider.Value = newSliderValue;
				_audioButtonsControl.UpdateButtonStateOnNavigate();
			}
		}

		private void GoBack()
		{
			int newSliderValue = _scriptSlider.Value - 1;
			_scriptSlider.Value = newSliderValue;
			_audioButtonsControl.UpdateButtonStateOnNavigate();
		}

		private void _deleteRecordingButton_Click(object sender, EventArgs e) => DeleteRecording();

		private void btnUndelete_Click(object sender, EventArgs e)
		{
			if (_currentMode == Mode.ReadAndRecord)
			{
				if (_project.UndeleteClipForSelectedBlock())
					OnSoundFileCreatedOrDeleted();
			}
			else
				_scriptTextHasChangedControl.UndoDeleteOfClipWithoutProblems();
		}

		private void DeleteRecording()
		{
			if (_currentMode == Mode.ReadAndRecord)
			{
				if (!_project.DeleteClipForSelectedBlock())
					return;
			}
			else
			{
				if (!_scriptTextHasChangedControl.DeleteClip())
					return;
			}

			OnSoundFileCreatedOrDeleted();
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
								"There is already a clip recorded for this line.\r\nIf you skip it, this clip will be omitted when publishing.\r\n\r\nAre you sure you want to do this?"),
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
				if (_currentMode == Mode.ReadAndRecord)
					_scriptControl.Invalidate();
				else
					_scriptTextHasChangedControl.UpdateState();	
				_audioButtonsControl.ButtonHighlightMode = AudioButtonsControl.ButtonHighlightModes.Default;
			}
		}

		private void UpdateDisplayForAdminMode()
		{
			_skipButton.Visible = ShowingSkipButton;
#if EnableHidingSkippedBlocks
			// I think all the rest of this code relates to the obsolete behavior of hiding
			// skipped blocks when not in the mostly-obsolete admin mode (which used to control HidingSkippedBlocks).
			// Keeping it on the off chance that we want to re-enable hiding them.

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
						if (_project.GetUnfilteredBlock(i).Skipped)
							sliderValue--;
					}
					// We also need to subtract 1 for the selected block if it was skipped
					if (_project.ScriptOfSelectedBlock.Skipped)
						sliderValue--;
					if (sliderValue < 0)
					{
						// Look forward to find an unskipped block
						sliderValue = 0;
						while (sliderValue < segmentCount && _project.GetUnfilteredBlock(_project.SelectedScriptBlock + sliderValue + 1).Skipped)
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
#endif
		}

		private void OnSmallerClick(object sender, EventArgs e)
		{
			SetZoom(_scriptControl.ZoomFactor - 0.2f);
			SetZoom(_scriptTextHasChangedControl.ZoomFactor - 0.2f);
		}

		private void OnLargerClick(object sender, EventArgs e)
		{
			SetZoom(_scriptControl.ZoomFactor + 0.2f);
			SetZoom(_scriptTextHasChangedControl.ZoomFactor + 0.2f);
		}

		private void SetZoom(float newZoom)
		{
			var zoom = Math.Max(Math.Min(newZoom, 2.0f), 1.0f);
			if (Settings.Default.ZoomFactor != zoom)
			{
				Settings.Default.ZoomFactor = zoom;
				Settings.Default.Save();
			}

			_scriptControl.ZoomFactor = zoom;
			_scriptTextHasChangedControl.ZoomFactor = zoom;
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
				ShowEndOfUnit(Format(_chapterFinished, _chapterLabel.Text));
			else
			{
				_endOfUnitMessage.Visible = false;
				_audioButtonsControl.CanGoNext = false;
			}

			_nextChapterLink.Text = Format(_gotoLink, GetNextChapterLabel());
			_nextChapterLink.Visible = true;
			_recordInPartsButton.Hide();
		}

		private void ShowEndOfUnit(string text)
		{
			_endOfUnitMessage.Text = text;
			_endOfUnitMessage.Visible = true;
			_audioButtonsControl.CanGoNext = false;
		}

		private string GetNextChapterLabel() =>
			Format(GetChapterNumberString(), _project.GetNextChapterNum());

		private void ShowEndOfBook() => ShowEndOfUnit(Format(_endOfBook, _bookLabel.Text));

		private void ShowScriptLines()
		{
			_endOfUnitMessage.Visible = false;
			_nextChapterLink.Visible = false;
			if (_currentMode == Mode.ReadAndRecord)
			{
				_scriptControl.Visible = true;
				_audioButtonsControl.CanGoNext = true;
				_recordInPartsButton.Show();
			}
			else
			{
				_scriptTextHasChangedControl.Visible = true;
			}
		}

		private void HideScriptLines()
		{
			_scriptControl.Visible = false;
			_scriptTextHasChangedControl.Visible = false;
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
			if (_currentMode == Mode.ReadAndRecord)
				_scriptControl.Invalidate();
		}

		private int GetScriptBlockIndexFromSliderValueByAccountingForPrecedingHiddenBlocks(int sliderValue)
		{
			for (int i = 0; i <= sliderValue; i++)
			{
				var block = _project.GetUnfilteredBlock(i);
				if (block == null) // passed the end of the list. All were skipped.
					return sliderValue;
				if (block.Skipped)
					sliderValue++;
			}
			return sliderValue;
		}

		internal void ReportNoMicrophone(object sender, EventArgs e)
		{
			MessageBox.Show(this,
				LocalizationManager.GetString("RecordingToolControl.NoMic", "This computer appears to have no sound recording device available. You will need one to record with this program."),
				LocalizationManager.GetString("RecordingToolControl.NoInput", "No input device"));
		}

		public void StopPlaying() =>
			_audioButtonsControl.StopPlaying();
		
		private void longLineButton_Click(object sender, EventArgs e)
		{
			StopPlaying();

			if (recordingDeviceButton1.Recorder.SelectedDevice == null)
			{
				recordingDeviceButton1.Recorder.SelectedDevice = RecordingDevice.Devices.FirstOrDefault() as RecordingDevice;
				if (recordingDeviceButton1.Recorder.SelectedDevice == null)
				{
					ReportNoMicrophone(null, null);
					return;
				}
			}

			using (var dlg = new RecordInPartsDlg())
			{
				var scriptLine = _project.GetUnfilteredBlock(_project.SelectedScriptBlock);
				dlg.TextToRecord = scriptLine.Text;
				dlg.Activated += (s, args) => recordingDeviceButton1.MicCheckingEnabled = true;
				dlg.Deactivate += (s, args) => recordingDeviceButton1.MicCheckingEnabled = false;
				dlg.RecordingAttemptAbortedBecauseOfNoMic += ReportNoMicrophone;
				dlg.ContextForAnalytics = _audioButtonsControl.ContextForAnalytics;
				dlg.VernacularFont = new Font(scriptLine.FontName, scriptLine.FontSize * _scriptControl.ZoomFactor);
				var result = dlg.ShowDialog(this);
				if (result == DialogResult.OK)
				{
					if (dlg.WriteCombinedAudio(_project.GetPathToRecordingForSelectedLine()))
						OnSoundFileCreated(null, null);
				}
				else
				{
					Trace.WriteLine("Dialog result: " + result);
				}
			}
		}
		
		public void RefreshBookAndChapterButtonProblemState(bool show = true)
		{
			_bookFlow.SuspendLayout();
			_chapterFlow.SuspendLayout();
			try
			{
				foreach (var button in _chapterFlow.Controls.OfType<UnitNavigationButton>()
					.Concat(_bookFlow.Controls.OfType<UnitNavigationButton>()))
				{
					button.ShowProblems = show;
				}
			}
			finally
			{
				_chapterFlow.ResumeLayout(true);
				_bookFlow.ResumeLayout();
			}
		}

		public void SetClauseSeparators(IReadOnlySet<char> clauseBreakCharacterSet)
		{
			_scriptControl.SetClauseSeparators(clauseBreakCharacterSet);
		}

		private void HandleNavigationArea_MouseEnter(object sender, EventArgs e)
		{
			ShowOrHideNavigationLabels(sender);
		}

		private void HandleNavigationArea_MouseLeave(object sender, EventArgs e)
		{
			ShowOrHideNavigationLabels(sender);
		}

		private void ShowOrHideNavigationLabels(object sender)
		{
			if (!Settings.Default.DisplayNavigationButtonLabels)
				return;

			var panel = sender as FlowLayoutPanel;
			if (panel == null)
			{
				panel = (sender as UnitNavigationButton)?.Parent as FlowLayoutPanel;
				if (panel == null || panel.Controls.Count == 0)
					return;
			}
			var showLabels = panel.ClientRectangle.Contains(panel.PointToClient(MousePosition));
			bool changed = false;
			if (panel == _bookFlow)
			{
				if (BookButton.DisplayLabelsWhenPaintingButtons != showLabels)
				{
					changed = true;
					BookButton.DisplayLabelsWhenPaintingButtons = showLabels;
				}
			}
			else
			{
				if (ChapterButton.DisplayLabelsWhenPaintingButtons != showLabels)
				{
					changed = true;
					ChapterButton.DisplayLabelsWhenPaintingButtons = showLabels;
				}
			}

			if (changed)
				panel.Invalidate(true);
		}

		public void HandleDisplayNavigationButtonLabelsChange()
		{
			foreach (BookButton btn in _bookFlow.Controls)
				btn.SetWidth(Settings.Default.DisplayNavigationButtonLabels);
			_chapterFlow.Invalidate(true);
		}

		public string ShiftClipsMenuName => _mnuShiftClips.Text.TrimEnd('.');

		private void _scriptSlider_MouseClick(object sender, MouseEventArgs e)
		{
			if (Settings.Default.AllowDisplayOfShiftClipsMenu &&
				_project.LineCountForChapter > 1 &&
				e.Button == MouseButtons.Right && _project.SelectedLineHasClip)
			{
				_contextMenuStrip.Show(_scriptSlider, e.Location);
			}
		}

		private void _scriptSlider_MouseEnterSegment(DiscontiguousProgressTrackBar sender, int value)
		{
			string tip = null;
			if (value >= _project.LineCountForChapter)
			{
				tip = LocalizationManager.GetString("RecordingControl.DeleteExtraClipTooltip",
					"If this extra clip does not correspond to any block, delete it.");
				if (Settings.Default.AllowDisplayOfShiftClipsMenu && _project.IsHole(value - 1))
				{
					tip += " " + Format(LocalizationManager.GetString(
						"RecordingControl.ShiftClipsHintTooltip",
						"Otherwise, right click for {0} command.",
						"Param 0: the name of the \"Shift Clips\" menu command"), ShiftClipsMenuName);
				}
			}
			toolTip1.SetToolTip(_scriptSlider, tip);
		}

		private void _mnuShiftClips_Click(object sender, EventArgs e)
		{
			var model = new ShiftClipsViewModel(_project);
			if (model.CanShift)
			{
				using (var dlg = new ShiftClipsDlg(model))
				{
					if (dlg.ShowDialog(this) == DialogResult.OK)
					{
						_scriptSlider.Invalidate();
						_audioButtonsControl.Invalidate();
						OnSoundFileCreatedOrDeleted();
					}
				}
			}
			else
			{
				MessageBox.Show(this, LocalizationManager.GetString("RecordingControl.CannotShiftClips",
					"All blocks already have clips recorded or are skipped. You would need to " +
					"delete a clip to make a \"hole\" in order to shift existing clips."), Program.kProduct);
			}
		}

		private void _scriptTextHasChangedControl_ProblemIgnoreStateChanged(object sender, EventArgs e)
		{
			_scriptSlider.Invalidate();
			UpdateChapterAndBookProblemStates();
		}

		private void _scriptTextHasChangedControl_DisplayUpdated(ScriptTextHasChangedControl sender, bool displayingOptions)
		{
			_deleteRecordingButton.Visible = !displayingOptions && HaveRecording;
			_btnUndelete.Visible = !displayingOptions && !_deleteRecordingButton.Visible &&
				_project.HasBackupFileForSelectedBlock();
		}

		private void _scriptTextHasChangedControl_DisplayedWithClippedControls(
			ScriptTextHasChangedControl sender, ushort increasedHeight)
		{
			var form = FindForm();
			if (form != null)
			{
				// Don't expand off the screen
				var screenBottom = Screen.FromControl(form).WorkingArea.Bottom;
				if (form.Bottom + increasedHeight > screenBottom)
					increasedHeight = (ushort)Math.Max(0, screenBottom - form.Bottom);
				form.Size = new Size(form.Width, form.Height + increasedHeight);
			}

			// This is a first-time-only adjustment. After that, we'll let the user resize as desired.
			_scriptTextHasChangedControl.DisplayedWithClippedControls -= _scriptTextHasChangedControl_DisplayedWithClippedControls;
		}

		private void UpdateControlsForSelectedChapter()
		{
			_scriptSlider.Refresh();
			_audioButtonsControl.Enabled = DisplayedSegmentCount != 0;
		}

		private void HandleExtraClipCollectionChanged(object sender, EventArgs e)
		{
			UpdateControlsForSelectedChapter();
			_scriptSlider.Value = _project.SelectedScriptBlock;
			_scriptTextHasChangedControl.SetData(_project);
			UpdateChapterAndBookProblemStates();
		}

		private void UpdateChapterAndBookProblemStates()
		{
			var currentChapterButton = _chapterFlow.Controls.OfType<ChapterButton>()
				.Single(b => b.ChapterInfo.ChapterNumber1Based == _project.SelectedChapterInfo.ChapterNumber1Based);
			var currentBookButton = _bookFlow.Controls.OfType<BookButton>()
				.Single(b => b.BookNumber == _project.SelectedBook.BookNumber);

			currentChapterButton.RecalculatePercentageRecorded();

			currentChapterButton.UpdateProblemState();
			currentBookButton.UpdateProblemState();
		}

		#region ISupportInitialize implementation
		public void BeginInit()
		{
			SuspendLayout(); // See HT-4111
		}

		public void EndInit()
		{
			ResumeLayout(false); // See HT-4111
		}
		#endregion

		private void _panelRecordingDeviceBorder_MouseEnter(object sender, EventArgs e)
		{
			_panelRecordingDeviceBorder.BackColor = AppPalette.CommonMuted;
		}

		private void _panelRecordingDeviceBorder_MouseLeave(object sender, EventArgs e)
		{
			if (!_panelRecordingDeviceBorder.ClientRectangle.Contains(
				_panelRecordingDeviceBorder.PointToClient(Cursor.Position)))
			{
				_panelRecordingDeviceBorder.BackColor = BackColor;
			}
		}

		// This is a very stripped-down version of the code from PtxUtils. In our case, we
		// never have multiple invokes getting queued up, we don't need the advanced logging, etc.
		private void InvokeLaterOnUIThread(ThreadStart action)
		{
			if (action == null)
				return;

			_uiSynchronizationContext.Post(Callback, action);
		}

		private void Callback(object state)
		{
			Debug.Assert(Thread.CurrentThread == _uiSynchronizationThread, "Invoked on wrong thread");

			ThreadStart cbAction = (ThreadStart)state;
			if (IsTargetDisposed(cbAction))
				return;
			cbAction();
		}

		private static bool IsTargetDisposed(ThreadStart action)
		{
			if (action?.Target == null)
				return false;

			// check to see if form related to action has been disposed
			return (action.Target as Form ?? (action.Target as Control)?.FindForm())?.IsDisposed ?? true;
		}
	}
}
