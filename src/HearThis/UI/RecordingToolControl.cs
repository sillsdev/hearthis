// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
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
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using SIL.Code;
using SIL.IO;
using SIL.Media.Naudio;
using SIL.Reporting;
using SIL.Windows.Forms.SettingProtection;
using static System.String;

namespace HearThis.UI
{
	public enum Mode
	{
		ReadAndRecord,
		CheckForProblems,
	}

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
		private bool _showingSkipButton;
		private Mode _currentMode = Mode.ReadAndRecord;

		public Mode CurrentMode
		{
			get => _currentMode;
			set
			{
				_currentMode = value;
				switch (_currentMode)
				{
					case Mode.ReadAndRecord:
						_scriptTextHasChangedControl.Hide();
						tableLayoutPanel1.SetColumnSpan(_tableLayoutScript, 1);
						_scriptControl.GoToScript(GetDirection(), PreviousScriptBlock, CurrentScriptLine, NextScriptBlock);
						_scriptControl.Show();
						_audioButtonsControl.Show();
						_peakMeter.Show();
						_recordInPartsButton.Show();
						_breakLinesAtCommasButton.Show();
						UpdateDisplay();
						SetBookAndChapterButtonsToShowProblems(false);
						break;
					case Mode.CheckForProblems:
						_scriptControl.Hide();
						_audioButtonsControl.Hide();
						_peakMeter.Hide();
						_scriptTextHasChangedControl.SetData(CurrentScriptLine);
						tableLayoutPanel1.SetColumnSpan(_tableLayoutScript, 2);
						_recordInPartsButton.Hide();
						_breakLinesAtCommasButton.Hide();
						_deleteRecordingButton.Hide();
						if (!DoesSegmentHaveProblems(_project.SelectedScriptBlock, true) &&
							// On initial reload after changing the color scheme, this setting tells us we are
							// restarting in CheckForProblems mode, so we don't want to move the user off the
							// segment they were on.
							Settings.Default.CurrentMode != Mode.CheckForProblems)
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
											"Param is \"HearThis\" (program name)"),
										Program.kProduct),
									Program.kProduct);
							}
						}

						SetBookAndChapterButtonsToShowProblems(true);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				_scriptSlider.Invalidate();
				UpdateScriptAndMessageControls();
			}
		}

		public RecordingToolControl()
		{
			_tempStopwatch.Start();

			InitializeComponent();
			SetZoom(Settings.Default.ZoomFactor); // do after InitializeComponent sets it to 1.
			SettingsProtectionSettings.Default.PropertyChanged += OnSettingsProtectionChanged;
			HandleStringsLocalized();
			BackColor = AppPallette.Background;
			_bookLabel.ForeColor = AppPallette.TitleColor;
			_chapterLabel.ForeColor = AppPallette.TitleColor;
			_segmentLabel.ForeColor = AppPallette.TitleColor;
			_lineCountLabel.ForeColor = AppPallette.TitleColor;
			_segmentLabel.BackColor = AppPallette.Background;
			_lineCountLabel.BackColor = AppPallette.Background;
			_skipButton.ForeColor = AppPallette.HilightColor; // Only used (for border) when UseForeColorForBorder

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
			_peakMeter.ColorMedium = AppPallette.Blue;
			_peakMeter.ColorNormal = AppPallette.EmptyBoxColor;
			_peakMeter.ColorHigh = AppPallette.Red;
			_peakMeter.SetRange(5, 80, 100);
			_audioButtonsControl.Recorder.PeakLevelChanged += ((s, e) => _peakMeter.PeakLevel = e.Level);
			_audioButtonsControl.RecordingDevice = RecordingDevice.Devices.FirstOrDefault() as RecordingDevice;
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

			_audioButtonsControl.SoundFileRecordingComplete += OnSoundFileCreated;
			_audioButtonsControl.RecordingStarting += OnAudioButtonsControlRecordingStarting;

			_breakLinesAtCommasButton.Checked = Settings.Default.BreakLinesAtClauses;

			_lineCountLabel.ForeColor = AppPallette.NavigationTextColor;
			
			Program.RegisterStringsLocalized(HandleStringsLocalized);
		}

		private void HandleStringsLocalized()
		{
			_lineCountLabelFormat = _lineCountLabel.Text;
			SetChapterLabelIfIntroduction();
			UpdateUiStringsForCurrentScriptLine();
		}

		private void OnAudioButtonsControlRecordingStarting(object sender, CancelEventArgs cancelEventArgs)
		{
			if (SelectedBlockHasSkippedStyle)
			{
				var fmt = LocalizationManager.GetString("RecordingControl.CannotRecordClipForSkippedStyle",
					"The settings for this project prevent recording this block because its paragraph style is {0}. If " +
					"you intend to record blocks having this style, in the Settings dialog box, select the Skipping page, " +
					"and then clear the selection for this style.");
				MessageBox.Show(this, Format(fmt, GetUnfilteredScriptBlock(_project.SelectedScriptBlock).ParagraphStyle), Program.kProduct);
				cancelEventArgs.Cancel = true;
			}
			else if (CurrentScriptLine.Skipped)
			{
				var fmt = LocalizationManager.GetString("RecordingControl.CannotRecordSkippedClip",
					"This block has been skipped. If you want to record a clip for this block, first click the Skip button " +
					"so that it is no longer selected.");
				MessageBox.Show(this, Format(fmt, GetUnfilteredScriptBlock(_project.SelectedScriptBlock).ParagraphStyle), Program.kProduct);
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
			if (FindForm() is Shell shell)
			{
				shell.ProjectChanged += delegate(object sender, EventArgs args)
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

		private void OnSoundFileCreated(object sender, ErrorEventArgs eventArgs)
		{
			_scriptControl.RecordingInProgress = false;
			if (CurrentScriptLine.Skipped)
			{
				var skipPath = Path.ChangeExtension(_project.GetPathToRecordingForSelectedLine(), "skip");
				if (File.Exists(skipPath))
				{
					try
					{
						RobustFile.Delete(skipPath);
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
			if (_project.ActorCharacterProvider != null)
			{
				// We presume the recording just made was made by the current actor for the current character.
				// (Or if none has been set, they will correctly be null.)
				CurrentScriptLine.Actor = _project.ActorCharacterProvider.Actor;
				CurrentScriptLine.Character = _project.ActorCharacterProvider.Character;
			}
			else
			{
				// Probably redundant, but it MIGHT have been previously recorded with a known actor.
				CurrentScriptLine.Actor = CurrentScriptLine.Character = null;
			}
			CurrentScriptLine.RecordingTime = DateTime.UtcNow;
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

		private void SetProject(Project project)
		{
			if (_project != null)
			{
				_project.ScriptBlockRecordingRestored -= HandleScriptBlockRecordingRestored;
				_project.SelectedBookChanged -= HandleSelectedBookChanged;
			}

			_project = project;
			_scriptControl.SetFont(_project.FontName);
			_scriptControl.SetClauseSeparators(_project.ProjectSettings.ClauseBreakCharacters);

			_project.ScriptBlockRecordingRestored += HandleScriptBlockRecordingRestored;

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
				if (bookInfo.BookNumber == 38)
					_bookFlow.SetFlowBreak(x, true);

				if (bookInfo == _project.SelectedBook)
					x.Selected = true;
			}
			_project.LoadBook(_project.SelectedBook.BookNumber);
			UpdateSelectedBook();
			_scriptSlider.GetSegmentBrushesDelegate = GetSegmentBrushes;
			_project.SelectedBookChanged += HandleSelectedBookChanged;
		}

		/// <summary>
		/// Select the first block (for the current actor/character) that is not already recorded.
		/// Currently does not change anything if all are recorded; eventually, we may want it
		/// to return a boolean indicating failure so we can consider switching to the next character.
		/// </summary>
		void SelectFirstUnrecordedBlock()
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
						if (!ClipRepository.GetHaveClip(_project.Name, bookInfo.Name, chapter, blockNum, _project.ScriptProvider))
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
			SelectFirstUnrecordedBlock();
			UpdateSelectedBook();
			Invalidate(); // makes book labels update.
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

		private int DisplayedSegmentCount
		{
			get
			{
				Guard.AgainstNull(_project, "project");
				return _project.GetLineCountForChapter(!HidingSkippedBlocks);
			}
		}

		private SegmentPaintInfo[] GetSegmentBrushes()
		{
			var results = new SegmentPaintInfo[DisplayedSegmentCount];
			int iBrush = 0;
			for (var i = 0; i < results.Length; i++)
			{
				var isLineCurrentlyRecordable = _project.IsLineCurrentlyRecordable(_project.SelectedBook.BookNumber,
					_project.SelectedChapterInfo.ChapterNumber1Based, i);
				// The main bar will be drawn blue if there is something to record; otherwise leave the background
				// bar color showing.
				var mainBrush = isLineCurrentlyRecordable ? AppPallette.BlueBrush : AppPallette.DisabledBrush;
				if (GetUnfilteredScriptBlock(i).Skipped)
				{
					// NB: Skipped segments only get entries in the array of brushes if they are being shown(currently always, previously in "Admin" mode).
					// If we are ever again hiding skipped segments, then we need to avoid putting these segments into the collection.
					if (!HidingSkippedBlocks)
						results[iBrush++] = new SegmentPaintInfo {MainBrush = mainBrush, Symbol = "/"};
				}
				else
				{
					var seg = new SegmentPaintInfo {MainBrush = mainBrush};
					results[iBrush++] = seg;
					if (isLineCurrentlyRecordable &&
						ClipRepository.GetHaveClipUnfiltered(_project.Name, _project.SelectedBook.Name,
							_project.SelectedChapterInfo.ChapterNumber1Based, i))
					{
						seg.UnderlineBrush = AppPallette.HighlightBrush;
						if (CurrentMode == Mode.CheckForProblems)
						{
							if (DoesSegmentHaveProblems(i))
							{
								seg.PaintIconDelegate = (g, r, selected) => UnitNavigationButton.DrawExclamation(g, r, AppPallette.HighlightBrush);
							}
							else if (i == results.Length - 1 && _project.SelectedChapterInfo.HasRecordingInfoBeyondExtentOfCurrentScript)
							{
								seg.PaintIconDelegate = DrawExtraRecordingsIndicator;
							}
							else if (DoesSegmentHaveIgnoredProblem(i))
							{
								seg.PaintIconDelegate = (g, r, selected) => UnitNavigationButton.DrawDot(g, r,
									IconBrush(selected));
							}
						}
					}
				}
			}
			return results;
		}

		private Brush IconBrush(bool selected) => selected ? AppPallette.HighlightBrush : AppPallette.DisabledBrush;

		private void DrawExtraRecordingsIndicator(Graphics g, Rectangle r, bool selected)
		{
			var text = ">";
			var size = g.MeasureString(text, Font);
			var leftString = r.X + r.Width / 2 - size.Width / 2;
			var topString = r.Y + r.Height / 2 - size.Height / 2;
			g.DrawString(text, _scriptSlider.Font, IconBrush(selected), leftString, topString);
		}

		private ScriptLine GetRecordingInfo(int i) => _project.SelectedChapterInfo.Recordings.FirstOrDefault(r => r.Number == i + 1);
		private string GetCurrentScriptText(int i) => _project.SelectedBook.GetBlock(_project.SelectedChapterInfo.ChapterNumber1Based, i).Text;

		private bool DoesSegmentHaveProblems(int i, bool treatLackOfInfoAsProblem = false)
		{
			var recordingInfo = GetRecordingInfo(i);
			return recordingInfo == null ? treatLackOfInfoAsProblem && HaveRecording :
				recordingInfo.Text != GetCurrentScriptText(i);
		}

		private bool DoesSegmentHaveIgnoredProblem(int i)
		{
			var recordingInfo = GetRecordingInfo(i);
			return recordingInfo?.OriginalText != null && recordingInfo.OriginalText != GetCurrentScriptText(i);
		}

		private List<ScriptLine> GetRecordableBlocksUpThroughNextHoleToTheRight()
		{
			var indices = new List<int>();
			for (var i = _project.SelectedScriptBlock; i < _project.GetLineCountForChapter(true); i++)
				indices.Add(i);
			return GetRecordableBlocksUpThroughHole(indices);
		}

		private List<ScriptLine> GetRecordableBlocksAfterPreviousHoleToTheLeft()
		{
			var indices = new List<int>();
			for (var i = _project.SelectedScriptBlock; i >= 0; i--)
				indices.Add(i);
			return GetRecordableBlocksUpThroughHole(indices, true);
		}

		private List<ScriptLine> GetRecordableBlocksUpThroughHole(IEnumerable<int> indices, bool reverseList = false)
		{
			var bookInfo = _project.SelectedBook;
			var chapter = _project.SelectedChapterInfo.ChapterNumber1Based;
			var lines = new List<ScriptLine>();
			foreach (var i in indices)
			{
				if (!_project.IsLineCurrentlyRecordable(bookInfo.BookNumber, chapter, i))
					break;
				var block = bookInfo.ScriptProvider.GetBlock(bookInfo.BookNumber, chapter, i);
				if (reverseList)
					lines.Insert(0, block);
				else
					lines.Add(block);
				if (!block.Skipped && !ClipRepository.GetHaveClip(_project.Name, bookInfo.Name, chapter, i, _project.ScriptProvider))
					return lines;
			}
			return new List<ScriptLine>();
		}

		private void UpdateDisplay()
		{
			if (CurrentMode != Mode.ReadAndRecord)
				return;
			_scriptControl.RecordingInProgress = _audioButtonsControl.Recording;
			_skipButton.Enabled = HaveScript;
			// Technically in overview mode we have something to record but we're not allowed to record it.
			// Pretending we don't have something produces the desired effect of disabling the Record button.
			// Similarly if the current block is not recordable.
			_audioButtonsControl.HaveSomethingToRecord = HaveScript && !InOverviewMode
				&& _project.IsLineCurrentlyRecordable(_project.SelectedBook.BookNumber, _project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock);
			_audioButtonsControl.UpdateDisplay();
			_lineCountLabel.Visible = HaveScript;
			//_upButton.Enabled = _project.SelectedScriptLine > 0;
			//_audioButtonsControl.CanGoNext = _project.SelectedScriptBlock < (_project.GetLineCountForChapter()-1);
			_deleteRecordingButton.Visible = HaveRecording;
			_recordInPartsButton.Enabled = HaveScript && !_skipButton.Checked;

			_audioButtonsControl.ButtonHighlightMode = _skipButton.Checked ?
				AudioButtonsControl.ButtonHighlightModes.SkipRecording :
				AudioButtonsControl.ButtonHighlightModes.Default;
		}

		// We're in 'overview' mode if we're dealing with actor/character information but haven't chosen one.
		private bool InOverviewMode => _project.ActorCharacterProvider != null && _project.ActorCharacterProvider.Character == null;

		private bool SelectedBlockHasSkippedStyle => ScriptLine.SkippedStyleInfoProvider.IsSkippedStyle(
			GetUnfilteredScriptBlock(_project.SelectedScriptBlock).ParagraphStyle);

		private bool HaveRecording => !_scriptSlider.Finished &&
			ClipRepository.GetHaveClipUnfiltered(_project.Name, _project.SelectedBook.Name,
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
		/// <remarks>This is invoked because we implement IMessageFilter and call Application.AddMessageFilter(this)</remarks>
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
					ShowPlayShortcutMessage();
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
			foreach (ChapterButton btn in _chapterFlow.Controls)
				btn.RecordingCompleteChanged -= HandleChapterRecordingsCompleteChanged;
			_chapterFlow.Controls.Clear();

			var buttons = new List<ChapterButton>();

			//note: we're using chapter 0 to mean the material at the start of the book
			for (int i = 0; i <= _project.SelectedBook.ChapterCount; i++)
			{
				var chapterInfo = _project.SelectedBook.GetChapter(i);
				if (i == 0 && chapterInfo.IsEmpty)
					continue;

				var button = new ChapterButton(chapterInfo, _project.SelectedBook.GetChapter) {ShowProblems = CurrentMode == Mode.CheckForProblems};
				button.Click += OnChapterClick;
				button.MouseEnter += HandleNavigationArea_MouseEnter;
				button.MouseLeave += HandleNavigationArea_MouseLeave;
				button.RecordingCompleteChanged += HandleChapterRecordingsCompleteChanged;
				buttons.Add(button);
				_instantToolTip.SetToolTip(button, i == 0 ? GetIntroductionString() : Format(GetChapterNumberString(), i));
			}
			_chapterFlow.Controls.AddRange(buttons.ToArray());
			_chapterFlow.ResumeLayout(true);
			if (CurrentMode == Mode.CheckForProblems)
			{
				TrySelectFirstChapterWithProblem();
			}
			else if (_project.CurrentCharacter != null)
				_project.SelectedChapterInfo = GetFirstUnrecordedChapter();

			UpdateSelectedChapter();
		}

		private void SetBookAndChapterButtonsToShowProblems(bool show)
		{
			if (_chapterFlow.Controls.OfType<UnitNavigationButton>().FirstOrDefault()?.ShowProblems == show)
				return;

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

			for (var i = 0; i < book.ChapterCount; i++)
			{
				// TODO: Implement logic to find chapter with an unresolved problem.
				var chapterInfo = book.GetChapter(i);
				if (chapterInfo.HasRecordingsThatDoNotMatchCurrentScript)
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

		public static void ShowPlayShortcutMessage()
		{
			MessageBox.Show(LocalizationManager.GetString("RecordingControl.PushTabToPlay", "To play the clip, press the TAB key."));
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
			var newChapter = ((ChapterButton)sender).ChapterInfo;
			if (newChapter.ChapterNumber1Based == _project.SelectedChapterInfo.ChapterNumber1Based)
				return;
			_project.SelectedChapterInfo = newChapter;
			UpdateSelectedChapter();
		}

		private void UpdateSelectedChapter()
		{
			foreach (ChapterButton chapterButton in _chapterFlow.Controls)
			{
				chapterButton.Selected = false;
			}

			if (!SetChapterLabelIfIntroduction())
				_chapterLabel.Text = Format(GetChapterNumberString(), _project.SelectedChapterInfo.ChapterNumber1Based);

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

			if (targetBlock == 0 && CurrentMode == Mode.CheckForProblems)
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

			if (_tempStopwatch != null)
			{
				_tempStopwatch.Stop();
				Debug.WriteLine("Elapsed time: " + _tempStopwatch.ElapsedMilliseconds);
				_tempStopwatch = null;
			}
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

		private void ResetSegmentCount()
		{
			if (_project == null)
				return;
			_scriptSlider.Refresh();
			_audioButtonsControl.Enabled = DisplayedSegmentCount != 0;
		}

		private void OnLineSlider_ValueChanged(object sender, EventArgs e)
		{
			Settings.Default.Block = _scriptSlider.Value;
			UpdateForSelectedBlock();
		}

		private void UpdateForSelectedBlock()
		{
			int sliderValue = _scriptSlider.Value;

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
			_segmentLabel.Visible = true;
			_skipButton.CheckedChanged -= OnSkipButtonCheckedChanged;
			_skipButton.Checked = _skipButton.UseForeColorForBorder = currentScriptLine != null && currentScriptLine.Skipped;
			_skipButton.CheckedChanged += OnSkipButtonCheckedChanged;

			UpdateUiStringsForCurrentScriptLine();

			if (DisplayedSegmentCount == 0)
				_project.SelectedScriptBlock = 0; // This should already be true, but just make sure;

			if (CurrentMode == Mode.ReadAndRecord)
				_scriptControl.GoToScript(GetDirection(), PreviousScriptBlock, currentScriptLine, NextScriptBlock);
			else
				_scriptTextHasChangedControl.SetData(currentScriptLine);

			_previousLine = _project.SelectedScriptBlock;
			_audioButtonsControl.Path = _project.GetPathToRecordingForSelectedLine();

			var approximateWordCount = currentScriptLine == null ? 0 : currentScriptLine.ApproximateWordCount;

			_audioButtonsControl.ContextForAnalytics = new Dictionary<string, string>
			{
				{"book", _project.SelectedBook.Name},
				{"chapter", _project.SelectedChapterInfo.ChapterNumber1Based.ToString()},
				{"scriptBlock", _project.SelectedScriptBlock.ToString()},
				{"wordsInLine", approximateWordCount.ToString()}
			};

			UpdateDisplay();
		}

		private void UpdateUiStringsForCurrentScriptLine()
		{
			var currentScriptLine = CurrentScriptLine;
			string verse = currentScriptLine?.Verse;
			bool isRealVerseNumber = !IsNullOrEmpty(verse) && verse != "0";
			if (HaveScript)
			{
				int displayedBlockIndex = _scriptSlider.Value + 1;
				_lineCountLabel.Text = Format(_lineCountLabelFormat, displayedBlockIndex, DisplayedSegmentCount);

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
					_segmentLabel.Text = LocalizationManager.GetString("RecordingControl.NotTranslated", "Not translated yet");
				}
			}



		}

		public bool HidingSkippedBlocks
		{
			get { return false; } // Currently we never want to do this. Some of the newer code doesn't handle it.
			set
			{
				_hidingSkippedBlocks = value;
				UpdateDisplayForAdminMode();
			}
		}

		public bool ShowingSkipButton
		{
			get { return _showingSkipButton; }
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

		private ScriptLine CurrentScriptLine => _project != null ? GetUnfilteredScriptBlock(_project.SelectedScriptBlock) : null;

		/// <summary>
		/// Used for displaying context to the reader, this is the previous block in the actual (unfiltered) text.
		/// </summary>
		public ScriptLine PreviousScriptBlock
		{
			get
			{
				var current = GetUnfilteredScriptBlock(_project.SelectedScriptBlock);
				if (current == null)
					return null;
				var realIndex = current.Number - 1;
				return _project.ScriptProvider.GetUnfilteredBlock(_project.SelectedBook.BookNumber,
					_project.SelectedChapterInfo.ChapterNumber1Based, realIndex - 1);
			}
		}

		/// <summary>
		/// Used for displaying context to the reader, this is the next block in the actual (unfiltered) text.
		/// </summary>
		public ScriptLine NextScriptBlock
		{
			get
			{
				var current = GetUnfilteredScriptBlock(_project.SelectedScriptBlock);
				if (current == null)
					return null;
				var realIndex = current.Number - 1;
				return _project.ScriptProvider.GetUnfilteredBlock(_project.SelectedBook.BookNumber,
					_project.SelectedChapterInfo.ChapterNumber1Based, realIndex + 1);
			}
		}

		public ScriptLine GetUnfilteredScriptBlock(int index)
		{
			if (index < 0 || index >= _project.SelectedChapterInfo.GetUnfilteredScriptBlockCount())
				return null;
			return _project.SelectedBook.GetUnfilteredBlock(_project.SelectedChapterInfo.ChapterNumber1Based, index);
		}

		private void OnNextButton(object sender, EventArgs e)
		{
			int newSliderValue = _scriptSlider.Value + 1;
			if (_project.ActorCharacterProvider != null && _project.ActorCharacterProvider.Character != null)
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
			_deleteRecordingButton.Image = Resources.BottomToolbar_Delete;
		}

		private void _deleteRecordingButton_MouseLeave(object sender, EventArgs e)
		{
			_deleteRecordingButton.Image = Resources.BottomToolbar_Delete;
		}

		private void OnDeleteRecording()
		{
			if (ClipRepository.DeleteLineRecording(_project.Name, _project.SelectedBook.Name,
				_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptBlock, _project.ScriptProvider))
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
				_audioButtonsControl.ButtonHighlightMode = AudioButtonsControl.ButtonHighlightModes.Default;
			}
		}

		private void UpdateDisplayForAdminMode()
		{
			_skipButton.Visible = ShowingSkipButton;
#if EnableHidingSkippedBlocks
			// I think all the rest of this code relates to the obsolete behavior of hiding
			// skipped blocks when not in the mostly-obsolete admin mode (which used to control HidingSkippedBlocks).
			// Keeping it on the offchance that we want to re-enable hiding them.

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
						if (GetUnfilteredScriptBlock(i).Skipped)
							sliderValue--;
					}
					// We also need to subtract 1 for the selected block if it was skipped
					if (GetUnfilteredScriptBlock(_project.SelectedScriptBlock).Skipped)
						sliderValue--;
					if (sliderValue < 0)
					{
						// Look forward to find an unskipped block
						sliderValue = 0;
						while (sliderValue < segmentCount && GetUnfilteredScriptBlock(_project.SelectedScriptBlock + sliderValue + 1).Skipped)
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
			Settings.Default.ZoomFactor = zoom;
			Settings.Default.Save();
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
			if (CurrentMode == Mode.ReadAndRecord)
			{
				_scriptControl.Visible = true;
				_audioButtonsControl.CanGoNext = true;
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
			if (CurrentMode == Mode.ReadAndRecord)
				_scriptControl.Invalidate();
		}

		private int GetScriptBlockIndexFromSliderValueByAccountingForPrecedingHiddenBlocks(int sliderValue)
		{
			for (int i = 0; i <= sliderValue; i++)
			{
				var block = GetUnfilteredScriptBlock(i);
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
				var scriptLine = _project.GetUnfilteredBlock(_project.CurrentBookName, _project.SelectedChapterInfo.ChapterNumber1Based,
					_project.SelectedScriptBlock);
				dlg.TextToRecord = scriptLine.Text;
				dlg.RecordingDevice = _audioButtonsControl.RecordingDevice;
				dlg.RecordingDeviceIndicator = recordingDeviceButton1;
				dlg.ContextForAnalytics = _audioButtonsControl.ContextForAnalytics;
				dlg.VernacularFont = new Font(scriptLine.FontName, scriptLine.FontSize * _scriptControl.ZoomFactor);
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					dlg.WriteCombinedAudio(_project.GetPathToRecordingForSelectedLine());
					OnSoundFileCreated(this, null);
				}
			}
			recordingDeviceButton1.Recorder = _audioButtonsControl.Recorder;
		}

		public void SetClauseSeparators(string clauseBreakCharacters)
		{
			_scriptControl.SetClauseSeparators(clauseBreakCharacters);
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

		private void _scriptControl_LocationChanged(object sender, EventArgs e)
		{
			_endOfUnitMessage.Location = _scriptControl.Location;
		}

		public void HandleDisplayNavigationButtonLabelsChange()
		{
			foreach (BookButton btn in _bookFlow.Controls)
				btn.SetWidth(Settings.Default.DisplayNavigationButtonLabels);
			_chapterFlow.Invalidate(true);
		}

		private bool ExtraRecordingsExistRelativeToCurrentPosition
		{
			get
			{
				for (int i = _scriptSlider.Value + 1; i < _scriptSlider.SegmentCount; i++)
				{
					if (ClipRepository.GetHaveClipUnfiltered(_project.Name, _project.SelectedBook.Name,
						_project.SelectedChapterInfo.ChapterNumber1Based, i))
						return false;
				}

				// There is a special case when the user is on the last block and it does have a
				// recording. If there are extra blocks beyond it, we want to shift those in if
				// shifting clips to the left.
				if (_scriptSlider.Value < _scriptSlider.SegmentCount - 1 && HaveRecording)
					return false;

				return ClipRepository.GetHaveClipUnfiltered(_project.Name, _project.SelectedBook.Name,
					_project.SelectedChapterInfo.ChapterNumber1Based, _scriptSlider.SegmentCount);
			}
		}

		private void _scriptSlider_MouseClick(object sender, MouseEventArgs e)
		{
			if (Settings.Default.AllowDisplayOfShiftClipsMenu && e.Button == MouseButtons.Right &&
				(HaveRecording || ExtraRecordingsExistRelativeToCurrentPosition))
			{
				_contextMenuStrip.Show(_scriptSlider, e.Location);
			}
		}

		private void _mnuShiftClips_Click(object sender, EventArgs e)
		{
			var book = _project.SelectedBook;
			var chapterInfo = _project.SelectedChapterInfo;
			List<ScriptLine> linesToShiftForward, linesToShiftBackward;
			bool normalShifting = HaveRecording;
			if (normalShifting)
			{
				linesToShiftForward = GetRecordableBlocksUpThroughNextHoleToTheRight();
				linesToShiftBackward = GetRecordableBlocksAfterPreviousHoleToTheLeft();
			}
			else
			{
				linesToShiftForward = new List<ScriptLine>();
				linesToShiftBackward = new List<ScriptLine>();
				for (var i = _scriptSlider.Value; i < _scriptSlider.SegmentCount; i++)
				{
					linesToShiftBackward.Add(book.ScriptProvider.GetBlock(
						book.BookNumber, chapterInfo.ChapterNumber1Based, i));
				}
			}

			if (linesToShiftForward.Any() || linesToShiftBackward.Any())
			{
				IClipFile ClipPathProvider(int line) => ClipRepository.GetClipFile(_project.Name, book.Name,
					chapterInfo.ChapterNumber1Based, line, _project.ScriptProvider);
				using (var dlg = new ShiftClipsDlg(ClipPathProvider, linesToShiftForward, linesToShiftBackward))
				{
					if (dlg.ShowDialog(this) == DialogResult.OK)
					{
						try
						{
							int offset = dlg.ShiftingForward ? 1 : -1;
							var startLineNumber = dlg.CurrentLines.First().Number - (normalShifting ? 1 : 0);

							var result = ClipRepository.ShiftClips(_project.Name, book.Name,
								chapterInfo.ChapterNumber1Based, startLineNumber,
								normalShifting ? dlg.CurrentLines.Count - 1 : Int32.MaxValue,
								offset, () => chapterInfo);

							if (result.Error != null)
							{
								if (result.Attempted > result.SuccessfulMoves)
								{
									ErrorReport.NotifyUserOfProblem(result.Error,
										LocalizationManager.GetString("RecordingControl.FailedToShiftClips",
											"There was a problem renaming clip\r\n{0}\r\nto\r\n{1}\r\n{2} of {3} clips shifted successfully."),
										result.LastAttemptedMove.FilePath, result.LastAttemptedMove.GetIntendedDestinationPath(offset),
										result.SuccessfulMoves, result.Attempted);
								}
								else
								{
									ErrorReport.NotifyUserOfProblem(result.Error,
										LocalizationManager.GetString("RecordingControl.FailedToUpdateChapterInfo",
											"There was a problem updating chapter information for {0}, chapter {1}."),
										book.Name, chapterInfo.ChapterNumber1Based);
								}
							}
						}
						finally
						{
							_scriptSlider.Invalidate();
							_audioButtonsControl.Invalidate();
							OnSoundFileCreatedOrDeleted();
						}
					}
				}
			}
			else
			{
				MessageBox.Show(this, LocalizationManager.GetString("RecordingControl.CannotShiftClips",
					"All blocks already have recordings or are skipped. You would need to " +
					"delete a recording to make a \"hole\" in order to shift existing clips."), Program.kProduct);
			}
		}

		private void _scriptTextHasChangedControl_ProblemIgnoreStateChanged(object sender, EventArgs e)
		{
			_scriptSlider.Invalidate();
			_chapterFlow.Controls.OfType<ChapterButton>().Single(b => b.ChapterInfo.ChapterNumber1Based == _project.SelectedChapterInfo.ChapterNumber1Based).UpdateProblemState();
			_bookFlow.Controls.OfType<BookButton>().Single(b => b.BookNumber == _project.SelectedBook.BookNumber).UpdateProblemState();
		}
	}
}
