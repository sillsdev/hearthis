using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using Palaso.Code;
using Palaso.Media.Naudio;
using Palaso.Reporting;
using System.Linq;

namespace HearThis.UI
{
	public partial class RecordingToolControl : UserControl, IMessageFilter
	{
		private Project _project;
		private int _previousLine;
		public event EventHandler LineSelectionChanged;
		private bool _alreadyShutdown;
		public event EventHandler ChooseProject;
		private readonly LineRecordingRepository _lineRecordingRepository;

		public RecordingToolControl()
		{
			InitializeComponent();
			BackColor = AppPallette.Background;

			//_upButton.Initialize(Resources.up, Resources.upDisabled);
			//_nextButton.Initialize(Resources.down, Resources.downDisabled);
			_lineRecordingRepository = new LineRecordingRepository();

			if (DesignMode)
				return;

			Application.AddMessageFilter(this);//get key presses

			_peakMeter.Start(33);//the number here is how often it updates
			_peakMeter.ColorMedium = AppPallette.Blue;
			_peakMeter.ColorNormal = AppPallette.EmptyBoxColor;
			_peakMeter.ColorHigh = AppPallette.Red;
			_peakMeter.SetRange(5, 80, 100);
			_audioButtonsControl.Recorder.PeakLevelChanged += ((s, e) => _peakMeter.PeakLevel = e.Level);
			_audioButtonsControl.RecordingDevice = RecordingDevice.Devices.First();
			recordingDeviceButton1.Recorder = _audioButtonsControl.Recorder;
			MouseWheel += new MouseEventHandler(OnRecordingToolControl_MouseWheel);

			_aboutButton.ForeColor = AppPallette.NavigationTextColor;
//
//            var map = new ColorMap[1];
//            map[0] =new ColorMap();
//            map[0].OldColor = Color.Black;
//            map[0].NewColor = AppPallette.Blue;
//            recordingDeviceButton1.ImageAttributes.SetGamma(2.2f);
////           recordingDeviceButton1.ImageAttributes.SetBrushRemapTable(map);
///

		}

		void OnRecordingToolControl_MouseWheel(object sender, MouseEventArgs e)
		{
			var change = e.Delta / -120;    //the minus here is because down (negative) on the wheel equateds to addition on the horizontal slider

			if (change > 0)
				_scriptLineSlider.Value = Math.Min(_scriptLineSlider.Maximum, _scriptLineSlider.Value + change);
			else
				_scriptLineSlider.Value = Math.Max(_scriptLineSlider.Minimum, _scriptLineSlider.Value + change);
		}

		public void SetProject(Project project)
		{
			_project = project;
			_bookFlow.Controls.Clear();
			foreach (BookInfo bookInfo in project.Books)
			{
				var x = new BookButton(bookInfo)
							{
								Tag = bookInfo

							};
				_instantToolTip.SetToolTip(x, bookInfo.LocalizedName);
				x.Click += new EventHandler(OnBookButtonClick);
				_bookFlow.Controls.Add(x);
				if(bookInfo.BookNumber==38)
					_bookFlow.SetFlowBreak(x,true);
				BookInfo bookInfoForInsideClosure = bookInfo;
				project.LoadBookAsync(bookInfo.BookNumber, new Action(delegate
																		  {
																			  if(x.IsHandleCreated && !x.IsDisposed)
																				  x.Invalidate();
																			  if(this.IsHandleCreated && !this.IsDisposed && project.SelectedBook == bookInfoForInsideClosure)
																			  {
//                                                                                  _project.SelectedChapterInfo = bookInfoForInsideClosure.GetFirstChapter();
//                                                                                  UpdateSelectedChapter();
																				  _project.GotoInitialChapter();
																				  UpdateSelectedBook();
																			  }
																		  }));
			}
			UpdateSelectedBook();
			_scriptLineSlider.GetSegmentBrushesMethod = GetSegmentBrushes;
		}

		private Brush[] GetSegmentBrushes()
		{
			Guard.AgainstNull(_project,"project");

			int lineCountForChapter = _project.GetLineCountForChapter();
			var brushes = new Brush[lineCountForChapter];
			for (int i = 0; i < lineCountForChapter; i++)
			{
				if(_lineRecordingRepository.GetHaveScriptLineFile(_project.Name, _project.SelectedBook.Name,
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
		  //  _upButton.Enabled = _project.SelectedScriptLine > 0;
		   _audioButtonsControl.CanGoNext =  _project.SelectedScriptLine < (_project.GetLineCountForChapter()-1);

		   // this.Focus();//to get keys

			/* skip this for now... disabled looks more enabled than our enabled look!
			 * _smallerButton.Enabled = _scriptControl.ZoomFactor > 1;
			_largerButton.Enabled = _scriptControl.ZoomFactor <2;
			 */
		}

		private bool HaveScript
		{
			get { return _scriptLineSlider.Maximum > _scriptLineSlider.Minimum; }
		}


		/// <summary>
		///
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
				case Keys.Enter:
					_audioButtonsControl   .OnPlay(this, null);
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
			Application.RemoveMessageFilter(this);
			_alreadyShutdown = true;
		}

		void RecordAndPlayControl_MouseWheel(object sender, MouseEventArgs e)
		{
			for (int i = 0; i < Math.Abs(e.Delta); i++)
			{

			}
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnHandleDestroyed(EventArgs e)
		{
			Shutdown();
			base.OnHandleDestroyed(e);
		}
		private void RecordingToolControl_KeyPress(object sender, KeyPressEventArgs e)
		{

		}

		protected override bool IsInputKey(Keys keyData)
		{
			return base.IsInputKey(keyData);
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
			 _project.SelectedBook = (BookInfo) ((BookButton) sender).Tag;
			UpdateSelectedBook();
		}

		private void UpdateSelectedBook()
		{
			_bookLabel.Text = _project.SelectedBook.LocalizedName;
			//_bookFlow.Invalidate();

			foreach (BookButton button in _bookFlow.Controls)
			{
				button.Selected = false;
			}

			BookButton selected = (from BookButton control in _bookFlow.Controls
								where control.Tag == _project.SelectedBook
								select control).FirstOrDefault();

			selected.Selected = true;

			_chapterFlow.SuspendLayout();
			_chapterFlow.Controls.Clear();

			var buttons = new List<ChapterButton>();

			//note: we're using chapter 0 to mean the material at the start of the book
			for (int i=0; i <= _project.SelectedBook.ChapterCount ; i++)
			{
				var chapterInfo = _project.SelectedBook.GetChapter(i);
				if (i == 0 && chapterInfo.IsEmpty)
					continue;

				var button = new ChapterButton(chapterInfo);
				button.Width = 15;
				button.Click += new EventHandler(OnChapterClick);
				buttons.Add(button);
				if(i==0)
				{
						_instantToolTip.SetToolTip(button, "Introduction");
				}
				else
				{
					_instantToolTip.SetToolTip(button, "Chapter "+(i).ToString());
				}
			 }
			_chapterFlow.Controls.AddRange(buttons.ToArray());
			_chapterFlow.ResumeLayout(true);
			UpdateSelectedChapter();
		}

		void OnChapterClick(object sender, EventArgs e)
		{

			_project.SelectedChapterInfo = ((ChapterButton) sender).ChapterInfo;
			UpdateSelectedChapter();
		}

		private void UpdateSelectedChapter()
		{
			foreach (ChapterButton chapterButton in _chapterFlow.Controls)
			{
				chapterButton.Selected = false;
			}
			if(_project.SelectedChapterInfo.ChapterNumber1Based>0)
				_chapterLabel.Text = string.Format("Chapter {0}", _project.SelectedChapterInfo.ChapterNumber1Based);
			else
			{
				_chapterLabel.Text = string.Format("Introduction");
			}

			ChapterButton button = (ChapterButton) (from ChapterButton control in _chapterFlow.Controls
													  where control.ChapterInfo.ChapterNumber1Based == _project.SelectedChapterInfo.ChapterNumber1Based
													  select control).FirstOrDefault();

			button.Selected = true;
			_scriptLineSlider.Maximum = Math.Max(0, _project.GetLineCountForChapter() - 1);
			_scriptLineSlider.Minimum = 0;
			if(_scriptLineSlider.Maximum ==0)
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
			_lineCountLabel.Text = _scriptLineSlider.Maximum.ToString();
		   UpdateSelectedScriptLine();
		}

		private void OnLineSlider_ValueChanged(object sender, EventArgs e)
		{
			_project.SelectedScriptLine = _scriptLineSlider.Value;
			UpdateSelectedScriptLine();
		}


		private void UpdateSelectedScriptLine()
		{
			if (HaveScript)
			{
				_segmentLabel.Text = String.Format("Line {0}", _project.SelectedScriptLine + 1);
			}
			else
			{
				_segmentLabel.Text = String.Format("Not translated yet");
			}
			if (_project.SelectedScriptLine <= _scriptLineSlider.Maximum)//todo: what causes this?
			{
				_scriptLineSlider.Value = _project.SelectedScriptLine;

				_scriptControl.GoToScript(
					_previousLine < _project.SelectedScriptLine
						? ScriptControl.Direction.Next
						: ScriptControl.Direction.Up,
					CurrentScriptLine);
				_previousLine = _project.SelectedScriptLine;
				_audioButtonsControl.Path = _lineRecordingRepository.GetPathToLineRecording(_project.Name, _project.SelectedBook.Name,
																   _project.SelectedChapterInfo.ChapterNumber1Based,
																   _project.SelectedScriptLine);
			}
			UpdateDisplay();
		}


		public ScriptLine CurrentScriptLine
		{
			get
			{
				if( _project.SelectedBook.GetLineMethod !=null)
					return _project.SelectedBook.GetLineMethod(_project.SelectedChapterInfo.ChapterNumber1Based, _project.SelectedScriptLine);
				return new ScriptLine("No project yet. Line number " + _project.SelectedScriptLine.ToString() + "  The king’s scribes were summoned at that time, in the third month, which is the month of Sivan, on the twenty-third day. And an edict was written, according to all that Mordecai commanded concerning the Jews, to the satraps and the governors and the officials of the provinces from India to Ethiopia, 127 provinces..");
			}
		}

		private void OnNextButton(object sender, EventArgs e)
		{
			if (/*_nextButton.Enabled &&*/ _scriptLineSlider.Value < _scriptLineSlider.Maximum)//could be fired by keyboard
				_scriptLineSlider.Value++;
		}

		private void GoBack()
		{
			if ( _scriptLineSlider.Value > _scriptLineSlider.Minimum)//could be fired by keyboard
					_scriptLineSlider.Value--;
		}

		private void OnSaveClick(object sender, EventArgs e)
		{
			MessageBox.Show(
				"HearThis automatically saves your work, while you use it. This button is just here to tell you that :-)  To create sound files for playing your recordings, click on the Publish button.");
		}

		private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

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
			using(var dlg = new PublishDialog(new PublishingModel(_lineRecordingRepository, _project.Name)))
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


	}
}
