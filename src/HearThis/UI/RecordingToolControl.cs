using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using Palaso.Reporting;

namespace HearThis.UI
{
	public partial class RecordingToolControl : UserControl, IMessageFilter
	{
		private Project _project;
		private int _previousLine;
		public event EventHandler LineSelectionChanged;
		private bool _alreadyShutdown;
		public event EventHandler ChooseProject;

		public RecordingToolControl()
		{
			InitializeComponent();
			_upButton.Initialize(Resources.up, Resources.upDisabled);
			_downButton.Initialize(Resources.down, Resources.downDisabled);
			_soundLibrary = new SoundLibrary();
			Application.AddMessageFilter(this);
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
				x.Click += new EventHandler(OnBookButtonClick);
				_bookFlow.Controls.Add(x);
				if(bookInfo.BookNumber==38)
					_bookFlow.SetFlowBreak(x,true);
			}
			UpdateSelectedBook();
		}

		private void UpdateDisplay()
		{
			_recordAndPlayControl.UpdateDisplay();
			_upButton.Enabled = _project.SelectedScriptLine > 0;
			_downButton.Enabled = _project.SelectedScriptLine < (_project.GetLineCountForChapter()-1);
		   // this.Focus();//to get keys
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
					_recordAndPlayControl.OnPlay(this, null);
					break;

				case Keys.Right:
				case Keys.PageDown:
				case Keys.Down:
					OnLineDownButton(this,null);
					break;

				case Keys.Left:
				case Keys.PageUp:
				case Keys.Up:
					OnLineUpButton(this, null);
					break;

				case Keys.Space:
						if (m.Msg == WM_KEYDOWN)
							_recordAndPlayControl.SpaceGoingDown();
						if (m.Msg == WM_KEYUP)
							_recordAndPlayControl.SpaceGoingUp();
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
					OnLineUpButton(this, null);
					break;
				case Keys.Enter:
				case Keys.PageDown:
					OnLineDownButton(this, null);
					break;
				case Keys.Right:
					_recordAndPlayControl.OnPlay(this, null);
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
			for (int i=0; i < _project.SelectedBook.ChapterCount ; i++)
			{
				var chapterInfo = _project.SelectedBook.GetChapter(i);
				var button = new ChapterButton(chapterInfo);
				button.Width = 15;
				button.Click += new EventHandler(OnChapterClick);
				buttons.Add(button);
			 }
			_chapterFlow.Controls.AddRange(buttons.ToArray());
			_chapterFlow.ResumeLayout(true);
			UpdateSelectedChapter();
		}

		void OnChapterClick(object sender, EventArgs e)
		{

			_project.SelectedChapter = ((ChapterButton) sender).ChapterInfo;
			UpdateSelectedChapter();
		}

		private void UpdateSelectedChapter()
		{
			foreach (ChapterButton chapterButton in _chapterFlow.Controls)
			{
				chapterButton.Selected = false;
			}
			_chapterLabel.Text = string.Format("Chapter {0}", _project.SelectedChapter.ChapterNumber);

			ChapterButton button = (ChapterButton) (from ChapterButton control in _chapterFlow.Controls
													  where control.ChapterInfo.ChapterNumber == _project.SelectedChapter.ChapterNumber
													  select control).FirstOrDefault();

			button.Selected = true;

			_scriptLineSlider.Minimum = 0;
			_scriptLineSlider.Maximum = _project.GetLineCountForChapter() - 1;
			_maxScriptLineLabel.Text = _scriptLineSlider.Maximum.ToString();
			_project.SelectedScriptLine = 0;
		   UpdateSelectedScriptLine();
		}

		private void OnLineSlider_ValueChanged(object sender, EventArgs e)
		{
			_project.SelectedScriptLine = _scriptLineSlider.Value;
			UpdateSelectedScriptLine();
		}

		private SoundLibrary _soundLibrary;

		private void UpdateSelectedScriptLine()
		{
			_segmentLabel.Text = String.Format("Line {0}", _project.SelectedScriptLine+1);
			if (_project.SelectedScriptLine <= _scriptLineSlider.Maximum)//todo: what causes this?
			{
				_scriptLineSlider.Value = _project.SelectedScriptLine;

				_scriptControl.GoToScript(
					_previousLine < _project.SelectedScriptLine
						? ScriptControl.Direction.Down
						: ScriptControl.Direction.Up,
					CurrentScriptLine);
				_previousLine = _project.SelectedScriptLine;
				_recordAndPlayControl.Path = _soundLibrary.GetPath(_project.Name, _project.SelectedBook.Name,
																   _project.SelectedChapter.ChapterNumber,
																   _project.SelectedScriptLine, ".wav");
			}
			UpdateDisplay();
		}


		public ScriptLine CurrentScriptLine
		{
			get
			{
				if( _project.SelectedBook.GetLineMethod !=null)
					return _project.SelectedBook.GetLineMethod(_project.SelectedChapter.ChapterNumber, _project.SelectedScriptLine);
				return new ScriptLine("No project yet. Line number " + _project.SelectedScriptLine.ToString() + "  The king’s scribes were summoned at that time, in the third month, which is the month of Sivan, on the twenty-third day. And an edict was written, according to all that Mordecai commanded concerning the Jews, to the satraps and the governors and the officials of the provinces from India to Ethiopia, 127 provinces..");
			}
		}

		private void OnLineDownButton(object sender, EventArgs e)
		{
			if (_downButton.Enabled)//could be fired by keyboard
				_scriptLineSlider.Value++;
		}

		private void OnLineUpButton(object sender, EventArgs e)
		{
			if (_upButton.Enabled)//could be fired by keyboard
					_scriptLineSlider.Value--;
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			MessageBox.Show(
				"HearThis automatically saves your work, while you use it. This button is just here to tell you that :-)  To create sound files for playing your recordings, click on the Publish button.");
		}

		private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			using (var dlg = new AboutDialog())
			{
				dlg.ShowDialog();
			}
		}

		private void _generateFiles_Click(object sender, EventArgs e)
		{
			using(var dlg = new PublishDialog(new PublishingModel(_soundLibrary, _project.Name)))
			{
				dlg.ShowDialog();
			}
		}

		private void OnChangeProjectButton_Click(object sender, EventArgs e)
		{
			if (ChooseProject != null)
				ChooseProject(this, null);
		}


	}
}
