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
	public partial class OldChapterControl : UserControl
	{
		Brush _fullBrush = new SolidBrush(Color.FromArgb(32, 74, 135));
		Brush _emptyBrush = new SolidBrush(Color.FromArgb(115, 115, 115));
		private BookInfo _book;

		public OldChapterControl()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint, true);
			_book= new DummyBookInfo();//so we show something at  design-time
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			const int kMaxWidth = 50;
			const int kMaxRows = 3;
			int count = _book.ChapterCount;
			int widthOfEachInPixels = kMaxWidth;
			int rows = 1;

			while(widthOfEachInPixels * (count/rows) > e.ClipRectangle.Width && rows < kMaxRows)
			{
				++rows;
			}

			//shrink them if still not enough room
			if(rows==kMaxRows)
			{
				widthOfEachInPixels = e.ClipRectangle.Width/(count/rows);
			}
			int chapter = 0;

			int topOfCurrentRow = e.ClipRectangle.Top;
			int kRowHeight = 10;

			for (int row = 0; row < rows; row++)
			{
				var leftEdge = e.ClipRectangle.Left + 10;
				for (; chapter < _book.ChapterCount && leftEdge < e.ClipRectangle.Right; chapter++)
				{
					int pixelsOfFullness = (int)(widthOfEachInPixels * _book.GetChapter(chapter).PercentDone);
					e.Graphics.FillRectangle(_fullBrush, leftEdge + 3, topOfCurrentRow, pixelsOfFullness, kRowHeight);
					e.Graphics.FillRectangle(_emptyBrush, leftEdge + 3 + pixelsOfFullness, topOfCurrentRow, widthOfEachInPixels - pixelsOfFullness, kRowHeight);
					leftEdge += widthOfEachInPixels + 5;
				}
				topOfCurrentRow += kRowHeight+3;
			}
		}

		public void SetBook(BookInfo  book)
		{
			_book = book;
			Invalidate();
		}
	}
}
