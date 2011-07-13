using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HearThis
{
	public partial class ScriptureMapControl : UserControl
	{
		private Project _project;
		private int _selectedVerse;


		public ScriptureMapControl()
		{
			InitializeComponent();
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
			}
			UpdateSelectedBook();
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
			for (int i=1; i < _project.SelectedBook.ChapterCount+1 ; i++)
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
		}

		private void OnVerseSlider_ValueChanged(object sender, EventArgs e)
		{
			_segmentLabel.Text = String.Format("Verse {0}", SelectedVerse);
		}

		protected int SelectedVerse
		{
			get { return _selectedVerse; }
			set { _selectedVerse = value; }
		}
	}
}
