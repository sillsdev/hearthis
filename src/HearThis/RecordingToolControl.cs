using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HearThis.Properties;

namespace HearThis
{
	public partial class RecordingToolControl : UserControl
	{
		private Project _project;
		private int _previousVerse;
		public event EventHandler VerseChanged;

		public RecordingToolControl()
		{
			InitializeComponent();
			_upButton.Initialize(Resources.up, Resources.upDisabled);
			_downButton.Initialize(Resources.down, Resources.downDisabled);
			_soundLibrary = new SoundLibrary();
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
			_upButton.Enabled = _project.SelectedVerse > 1;
			_downButton.Enabled = _project.SelectedVerse < _project.SelectedChapter.VersePotentialCount;
		}

//        private int CurrentVerseNumber
//        {
//            get { return 1+ _verseSlider.Maximum - _verseSlider.Value; }
//        }

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

			_verseSlider.Minimum = 1;
			_verseSlider.Maximum = _project.SelectedChapter.VersePotentialCount;
			_maxVerseLabel.Text = _verseSlider.Maximum.ToString();
			_project.SelectedVerse = 1;
		   UpdateSelectedVerse();
		}

		private void OnVerseSlider_ValueChanged(object sender, EventArgs e)
		{
			_project.SelectedVerse = 1 + _verseSlider.Maximum - _verseSlider.Value; //it's upside-down
			UpdateSelectedVerse();
		}

		private SoundLibrary _soundLibrary;

		private void UpdateSelectedVerse()
		{
			_segmentLabel.Text = String.Format("Verse {0}", _project.SelectedVerse);
			_verseSlider.Value = 1 + _verseSlider.Maximum - _project.SelectedVerse; // it's upside-down

			_scriptControl.GoToScript(_previousVerse<_project.SelectedVerse?ScriptControl.Direction.Down:ScriptControl.Direction.Up,
				SelectedVerseText);
			_previousVerse = _project.SelectedVerse;
			_recordAndPlayControl.Path = _soundLibrary.GetPath(_project.Name, _project.SelectedBook,
																  _project.SelectedChapter, _project.SelectedVerse);
			UpdateDisplay();
		}


		public string SelectedVerseText
		{
			get
			{
				if( _project.SelectedBook.GetVerse !=null)
					return _project.SelectedBook.GetVerse(_project.SelectedChapter.ChapterNumber, _project.SelectedVerse);
				return "No project yet. Verse number " + _project.SelectedVerse.ToString() + "  The king’s scribes were summoned at that time, in the third month, which is the month of Sivan, on the twenty-third day. And an edict was written, according to all that Mordecai commanded concerning the Jews, to the satraps and the governors and the officials of the provinces from India to Ethiopia, 127 provinces..";
			}
		}

		private void OnVerseDownButton(object sender, EventArgs e)
		{
				_verseSlider.Value--;
		}

		private void OnVerseUpButton(object sender, EventArgs e)
		{
				_verseSlider.Value++;
		}
	}
}
