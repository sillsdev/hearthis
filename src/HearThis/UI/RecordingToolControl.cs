using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using Palaso.Code;
using Palaso.Media.Naudio;

namespace HearThis.UI
{
	public partial class RecordingToolControl : UserControl, IMessageFilter
	{
		private Project _project;
		private int _previousLine;
		private bool _alreadyShutdown;
		private string _lineCountLabelFormat;
		public event EventHandler ChooseProject;

		private readonly string _endOfBook = LocalizationManager.GetString("RecordingControl.EndOf", "End of {0}", "{0} is typically a book name");
		private readonly string _chapterFinished = LocalizationManager.GetString("RecordingControl.Finished", "{0} Finished", "{0} is a chapter number");
		private readonly string _gotoLink = LocalizationManager.GetString("RecordingControl.GoTo","Go To {0}", "{0} is a chapter number");

		public RecordingToolControl()
		{
			InitializeComponent();
			_lineCountLabelFormat = _lineCountLabel.Text;
			BackColor = AppPallette.Background;

			//_upButton.Initialize(Resources.up, Resources.upDisabled);
			//_nextButton.Initialize(Resources.down, Resources.downDisabled);

			if (DesignMode)
				return;

			_peakMeter.Start(33);//the number here is how often it updates
			_peakMeter.ColorMedium = AppPallette.Blue;
			_peakMeter.ColorNormal = AppPallette.EmptyBoxColor;
			_peakMeter.ColorHigh = AppPallette.Red;
			_peakMeter.SetRange(5, 80, 100);
			_audioButtonsControl.Recorder.PeakLevelChanged += ((s, e) => _peakMeter.PeakLevel = e.Level);
			_audioButtonsControl.RecordingDevice = RecordingDevice.Devices.FirstOrDefault();
			if (_audioButtonsControl.RecordingDevice == null)
				_audioButtonsControl.ReportNoMicrophone();
			recordingDeviceButton1.Recorder = _audioButtonsControl.Recorder;
			MouseWheel += OnRecordingToolControl_MouseWheel;

			_toolStrip.Renderer = new NoBorderToolStripRenderer();
			toolStripButton4.ForeColor = AppPallette.NavigationTextColor;

			_endOfUnitMessage.ForeColor = AppPallette.Blue;
			_nextChapterLink.ActiveLinkColor = AppPallette.HilightColor;
			_nextChapterLink.DisabledLinkColor = AppPallette.NavigationTextColor;
			_nextChapterLink.LinkColor = AppPallette.HilightColor;

			_audioButtonsControl.SoundFileCreated += OnSoundFileCreated;

			SetupUILanguageMenu();
			UpdateBreakClausesImage();

			_lineCountLabel.ForeColor = AppPallette.NavigationTextColor;
		}

		private void OnSoundFileCreated(object sender, EventArgs eventArgs)
		{
			_project.SelectedChapterInfo.OnRecordingSaved(_project.SelectedScriptLine, CurrentScriptLine);
			OnSoundFileCreatedOrDeleted();
		}

		private void OnSoundFileCreatedOrDeleted()
		{
			_scriptLineSlider.Invalidate();
			// deletion is done in LineRecordingRepository and affects audioButtons
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

		void OnRecordingToolControl_MouseWheel(object sender, MouseEventArgs e)
		{
			//the minus here is because down (negative) on the wheel equates to addition on the horizontal slider
			_scriptLineSlider.Value += e.Delta / -120;
		}

		public void SetProject(Project project)
		{
			_project = project;
			_bookFlow.Controls.Clear();
			_scriptLineSlider.ValueChanged -= OnLineSlider_ValueChanged; // update later when we have a correct value
			foreach (BookInfo bookInfo in project.Books)
			{
				var x = new BookButton(bookInfo) { Tag = bookInfo };
				_instantToolTip.SetToolTip(x, bookInfo.LocalizedName);
				x.Click += OnBookButtonClick;
				_bookFlow.Controls.Add(x);
				if (bookInfo.BookNumber==38)
					_bookFlow.SetFlowBreak(x,true);
				BookInfo bookInfoForInsideClosure = bookInfo;
				project.LoadBookAsync(bookInfo.BookNumber, delegate
					{
						if (x.IsHandleCreated && !x.IsDisposed)
							x.Invalidate();
						if (IsHandleCreated && !IsDisposed && project.SelectedBook == bookInfoForInsideClosure)
						{
							//_project.SelectedChapterInfo = bookInfoForInsideClosure.GetFirstChapter();
							//UpdateSelectedChapter();
							_project.GotoInitialChapter();
							UpdateSelectedBook();
						}
					});
			}
			UpdateSelectedBook();
			_scriptLineSlider.ValueChanged += OnLineSlider_ValueChanged;
			_scriptLineSlider.GetSegmentBrushesMethod = GetSegmentBrushes;
		}

		private Brush[] GetSegmentBrushes()
		{
			Guard.AgainstNull(_project,"project");

			int lineCountForChapter = _project.GetLineCountForChapter();
			var brushes = new Brush[lineCountForChapter];
			for (int i = 0; i < lineCountForChapter; i++)
			{
				if (LineRecordingRepository.GetHaveScriptLineFile(_project.Name, _project.SelectedBook.Name,
					 _project.SelectedChapterInfo.ChapterNumber1Based, i))
				{
					brushes[i] = AppPallette.BlueBrush;
				}
				else
				{
					brushes[i] = Brushes.Transparent;
				}
			}
			return brushes;
		}

		private void UpdateDisplay()
		{
			_audioButtonsControl.HaveSomethingToRecord = HaveScript;
			_audioButtonsControl.UpdateDisplay();
			_lineCountLabel.Visible = HaveScript;
			//_upButton.Enabled = _project.SelectedScriptLine > 0;
			_audioButtonsControl.CanGoNext = _project.SelectedScriptLine < (_project.GetLineCountForChapter()-1);
			_deleteRecordingButton.Visible = HaveRecording;
		}

		private bool HaveRecording
		{
			get
			{
				return LineRecordingRepository.GetHaveScriptLineFile(_project.Name, _project.SelectedBook.Name,
				_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptLine);
			}
		}

		private bool HaveScript
		{
			// This method is much more reliable for single line sections than comparing slider max & min
			get { return CurrentScriptLine != null && CurrentScriptLine.Text.Length > 0; }
		}


		/// <summary>
		/// Filter out all keystrokes except the few that we want to handle.
		/// We handle Space, Enter, Period, PageUp, PageDown, Delete and Arrow keys.
		/// </summary>
		/// <remarks>This is invoked because we implement IMessagFilter and call Application.AddMessageFilter(this)</remarks>
		public bool PreFilterMessage(ref Message m)
		{
			const int WM_KEYDOWN = 0x100;
			const int WM_KEYUP = 0x101;

			if (m.Msg != WM_KEYDOWN && m.Msg != WM_KEYUP)
				return false;

			if (m.Msg == WM_KEYUP && (Keys)m.WParam != Keys.Space)
				return false;

			switch ((Keys)m.WParam)
			{
				case Keys.OemPeriod:
				case Keys.Decimal:
				case Keys.Enter:
					_audioButtonsControl.OnPlay(this, null);
					break;

				case Keys.Right:
				case Keys.PageDown:
				case Keys.Down:
					OnNextButton(this,null);
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

				case Keys.Delete:
					OnDeleteRecording();
					break;

				case Keys.Tab:

					// Eat these.
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

		private void RecordingToolControl_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			e.SuppressKeyPress = true;
			switch (e.KeyCode)
			{
				case Keys.PageUp:
					GoBack();
					break;
				case Keys.Enter:
				case Keys.PageDown:
					OnNextButton(this, null);
					break;
				case Keys.Right:
					_audioButtonsControl.OnPlay(this, null);
					break;
				default:
					e.Handled = false;
					e.SuppressKeyPress = false;
					break;
			}
		}
		void OnBookButtonClick(object sender, EventArgs e)
		{
			 _project.SelectedBook = (BookInfo)((BookButton)sender).Tag;
			UpdateSelectedBook();
			UpdateScriptAndMessageControls(0);
		}

		private void UpdateSelectedBook()
		{
			_bookLabel.Text = _project.SelectedBook.LocalizedName;

			foreach (BookButton button in _bookFlow.Controls)
				button.Selected = false;

			BookButton selected = (from BookButton control in _bookFlow.Controls
								where control.Tag == _project.SelectedBook
								select control).Single();

			selected.Selected = true;

			_chapterFlow.SuspendLayout();
			_chapterFlow.Controls.Clear();

			var buttons = new List<ChapterButton>();

			//note: we're using chapter 0 to mean the material at the start of the book
			for (int i = 0; i <= _project.SelectedBook.ChapterCount ; i++)
			{
				var chapterInfo = _project.SelectedBook.GetChapter(i);
				if (i == 0 && chapterInfo.IsEmpty)
					continue;

				var button = new ChapterButton(chapterInfo);
				button.Width = 15;
				button.Click += OnChapterClick;
				buttons.Add(button);
				_instantToolTip.SetToolTip(button, i == 0 ? GetIntroductionString() : string.Format(GetChapterNumberString(), i));
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

		void OnChapterClick(object sender, EventArgs e)
		{
			_project.SelectedChapterInfo = ((ChapterButton)sender).ChapterInfo;
			UpdateSelectedChapter();
			UpdateScriptAndMessageControls(0);
		}

		private void UpdateSelectedChapter()
		{
			foreach (ChapterButton chapterButton in _chapterFlow.Controls)
			{
				chapterButton.Selected = false;
			}
			if (_project.SelectedChapterInfo.ChapterNumber1Based>0)
				_chapterLabel.Text = string.Format(GetChapterNumberString(), _project.SelectedChapterInfo.ChapterNumber1Based);
			else
			{
				_chapterLabel.Text = string.Format(GetIntroductionString());
			}

			ChapterButton button = (from ChapterButton control in _chapterFlow.Controls
				where control.ChapterInfo.ChapterNumber1Based == _project.SelectedChapterInfo.ChapterNumber1Based
				select control).Single();

			button.Selected = true;
			var lineCount = _project.GetLineCountForChapter();
			_scriptLineSlider.SegmentCount = Math.Max(0, lineCount);
			if (_scriptLineSlider.SegmentCount == 0 && lineCount == 0) // Fixes case where lineCount = 0 (Introduction)
			{
				_audioButtonsControl.Enabled = false;
				_scriptLineSlider.Enabled = false;
				//_maxScriptLineLabel.Text = "";
			}
			else
			{
				_audioButtonsControl.Enabled = true;
				_scriptLineSlider.Enabled = true;
				//_maxScriptLineLabel.Text = _scriptLineSlider.Maximum.ToString();
			}
			_project.SelectedScriptLine = 0;
		   UpdateSelectedScriptLine(true);
		}

		private void OnLineSlider_ValueChanged(object sender, EventArgs e)
		{
			if (UpdateScriptAndMessageControls(_scriptLineSlider.Value))
				_project.SelectedScriptLine = _scriptLineSlider.Maximum;
			else
			{
				_project.SelectedScriptLine = _scriptLineSlider.Value;
				UpdateSelectedScriptLine(false);
			}
		}

		private void UpdateSelectedScriptLine(bool changingChapter)
		{
			var currentScriptLine = CurrentScriptLine;
			string verse = currentScriptLine != null ? currentScriptLine.Verse : null;
			bool isRealVerseNumber = !string.IsNullOrEmpty(verse) && verse != "0";
			if (HaveScript)
			{
				_lineCountLabel.Text = string.Format(_lineCountLabelFormat, _project.SelectedScriptLine + 1, _scriptLineSlider.SegmentCount);

				if (currentScriptLine.Heading)
					_segmentLabel.Text = LocalizationManager.GetString("RecordingControl.Heading", "Heading");
				else if (isRealVerseNumber)
					_segmentLabel.Text = String.Format(LocalizationManager.GetString("RecordingControl.Script", "Verse {0}"), verse);
				else
					_segmentLabel.Text = String.Empty;
			}
			else
			{
				if (isRealVerseNumber)
					_segmentLabel.Text = String.Format(LocalizationManager.GetString("RecordingControl.VerseNotTranslated", "Verse {0} not translated yet"), CurrentScriptLine.Verse);
				else
					_segmentLabel.Text = LocalizationManager.GetString("RecordingControl.NotTranslated", "Not translated yet"); // Can this happen?
			}
			if (_scriptLineSlider.SegmentCount == 0)
				_project.SelectedScriptLine = 0; // This should already be true, but just make sure;

			if (_project.SelectedScriptLine < _scriptLineSlider.SegmentCount || _scriptLineSlider.SegmentCount == 0) // REVIEW: what can cause us to go over the limit?
			{
				_scriptLineSlider.Value = _project.SelectedScriptLine;

				_scriptControl.GoToScript(GetDirection(changingChapter), PreviousScriptLine, currentScriptLine, NextScriptLine);
				_previousLine = _project.SelectedScriptLine;
				_audioButtonsControl.Path = _project.GetPathToRecordingForSelectedLine();

				char[] delimiters = {' ', '\r', '\n' };

				var approximateWordCount = 0;
				if (currentScriptLine != null)
					approximateWordCount = currentScriptLine.Text.Split(delimiters,StringSplitOptions.RemoveEmptyEntries).Length;

				_audioButtonsControl.ContextForAnalytics = new Dictionary<string, string>
					{
						{"book", _project.SelectedBook.Name},
						{"chapter", _project.SelectedChapterInfo.ChapterNumber1Based.ToString()},
						{"scriptLine", _project.SelectedScriptLine.ToString()},
						{"wordsInLine", approximateWordCount.ToString()}
					};
			}
			UpdateDisplay();
		}

		private ScriptControl.Direction GetDirection(bool changingChapter)
		{
			if (changingChapter)
				return ScriptControl.Direction.Forwards;

			return _previousLine < _project.SelectedScriptLine
					   ? ScriptControl.Direction.Forwards
					   : ScriptControl.Direction.Backwards;
		}

		public ScriptLine CurrentScriptLine
		{
			get { return GetScriptLine(_project.SelectedScriptLine); }
		}

		public ScriptLine PreviousScriptLine
		{
			get { return GetScriptLine(_project.SelectedScriptLine - 1); }
		}

		public ScriptLine NextScriptLine
		{
			get { return GetScriptLine(_project.SelectedScriptLine + 1); }
		}

		public ScriptLine GetScriptLine(int index)
		{
			if (index < 0 || index >= _project.SelectedChapterInfo.GetScriptLineCount())
				return null;
			return _project.SelectedBook.GetLineMethod(_project.SelectedChapterInfo.ChapterNumber1Based, index);
		}

		private void OnNextButton(object sender, EventArgs e)
		{
			if (UpdateScriptAndMessageControls(_scriptLineSlider.Value + 1))
				return;
			_scriptLineSlider.Value++;
			_audioButtonsControl.UpdateButtonStateOnNavigate();
		}

		private void GoBack()
		{
			_scriptLineSlider.Value--;
			_audioButtonsControl.UpdateButtonStateOnNavigate();
		}

		private void OnSaveClick(object sender, EventArgs e)
		{
			MessageBox.Show(
				LocalizationManager.GetString("RecordingControl.SaveAutomatically", "HearThis automatically saves your work, while you use it. This button is just here to tell you that :-)  To create sound files for playing your recordings, click on the Publish button."),
				LocalizationManager.GetString("Common.Save", "Save"));
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
			if (LineRecordingRepository.DeleteLineRecording(_project.Name, _project.SelectedBook.Name,
				_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptLine))
			{
				OnSoundFileCreatedOrDeleted();
			}
		}

		private void OnAboutClick(object sender, EventArgs e)
		{
			using (var dlg = new AboutDialog())
			{
				dlg.ShowDialog();
			}
		}

		private void OnPublishClick(object sender, EventArgs e)
		{
			using(var dlg = new PublishDialog(new PublishingModel(_project.Name, _project.EthnologueCode)))
			{
				dlg.ShowDialog();
			}
		}

		private void OnChangeProjectButton_Click(object sender, EventArgs e)
		{
			if (ChooseProject != null)
				ChooseProject(this, null);
		}

		private void OnSmallerClick(object sender, EventArgs e)
		{
			if (_scriptControl.ZoomFactor > 1)
				_scriptControl.ZoomFactor -= 0.2f;
		}

		private void OnLargerClick(object sender, EventArgs e)
		{
		   if (_scriptControl.ZoomFactor <2)
				_scriptControl.ZoomFactor += 0.2f;
		}

		/// <summary>
		/// Responsable for the "End of (book)" messages and "Go To Chapter x" links.
		/// </summary>
		/// <param name="newSliderValue"></param>
		/// <returns>true, if scriptlines gets hidden and a message displayed</returns>
		public bool UpdateScriptAndMessageControls(int newSliderValue)
		{
			if (newSliderValue > _scriptLineSlider.Maximum)
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
				_audioButtonsControl.HaveSomethingToRecord = false;
				_audioButtonsControl.UpdateDisplay();
				_lineCountLabel.Visible = false;
				return true;
			}
			ShowScriptLines();
			return false;
		}

		private void ShowEndOfChapter()
		{
			_endOfUnitMessage.Text = string.Format(_chapterFinished, _chapterLabel.Text);
			_nextChapterLink.Text = string.Format(_gotoLink, GetNextChapterLabel());
			_endOfUnitMessage.Visible = true;
			_nextChapterLink.Visible = true;
		}

		private string GetNextChapterLabel()
		{
			return string.Format(GetChapterNumberString(), _project.GetNextChapterNum());
		}

		private void ShowEndOfBook()
		{
			_endOfUnitMessage.Text = string.Format(_endOfBook, _bookLabel.Text);
			_endOfUnitMessage.Visible = true;
		}

		private void ShowScriptLines()
		{
			_endOfUnitMessage.Visible = false;
			_nextChapterLink.Visible = false;
			_scriptControl.Visible = true;
		}

		private void HideScriptLines()
		{
			_scriptControl.Visible = false;
		}

		private void OnNextChapterLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			_project.SelectedChapterInfo = _project.GetNextChapterInfo();
			UpdateSelectedChapter();
			UpdateScriptAndMessageControls(0);
		}

		private void SetupUILanguageMenu()
		{
			_uiLanguageMenu.DropDownItems.Clear();
			foreach (var lang in LocalizationManager.GetUILanguages(true))
			{
				var item = _uiLanguageMenu.DropDownItems.Add(lang.NativeName);
				item.Tag = lang;
				item.Click += new EventHandler((a, b) =>
				{
					LocalizationManager.SetUILanguage(((CultureInfo)item.Tag).IetfLanguageTag, true);
					Settings.Default.UserInterfaceLanguage = ((CultureInfo)item.Tag).IetfLanguageTag;
					item.Select();
					_uiLanguageMenu.Text = ((CultureInfo)item.Tag).NativeName;
				});
				if (((CultureInfo)item.Tag).IetfLanguageTag == Settings.Default.UserInterfaceLanguage)
				{
					_uiLanguageMenu.Text = ((CultureInfo)item.Tag).NativeName;
				}
			}


			_uiLanguageMenu.DropDownItems.Add(new ToolStripSeparator());
			var menu = _uiLanguageMenu.DropDownItems.Add(LocalizationManager.GetString("RecordingControl.MoreMenuItem",
				"More...", "Last item in menu of UI languages"));
			menu.Click += new EventHandler((a, b) =>
			{
				LocalizationManager.ShowLocalizationDialogBox(this);
				SetupUILanguageMenu();
			});
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
				Settings.Default.BreakLinesAtClauses ? Resources.Icon_LineBreak_Comma_Active : Resources.Icon_LineBreak_Comma;
		}
	}

	public class NoBorderToolStripRenderer : ToolStripProfessionalRenderer
	{
		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
	}
}
