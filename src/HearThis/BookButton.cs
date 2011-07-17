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
	public partial class BookButton : UserControl
	{
		private bool _selected;
		private SolidBrush _highlightBoxBrush;
		private Brush _fillBrush;

		public BookButton(BookInfo bookInfo)
		{
			InitializeComponent();
			int kMaxChapters = 150;//psalms
			Width = (int) (Width + ((double)bookInfo.ChapterCount / (double)kMaxChapters) * 25.0);

			//small gap for start of new testament
//            if(bookInfo.BookNumber == 40)
//            {
//                Margin = new Padding(Margin.Left + 5, Margin.Top, Margin.Right, Margin.Bottom);
//            }
			_highlightBoxBrush = new SolidBrush(Color.FromArgb(255,168,0));

			if (bookInfo.HasAllRecordings)
				_fillBrush = new SolidBrush(Color.FromArgb(32,74,135));
			else if(bookInfo.HasSomeRecordings)
				_fillBrush = new SolidBrush( Color.FromArgb(215,2,0));
			else
				_fillBrush = new SolidBrush(bookInfo.HasVerses ? Color.FromArgb(115,115,115) : Color.WhiteSmoke);
		}

		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (_selected != value)
				{
					_selected = value;
					Invalidate();
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var r = new Rectangle(e.ClipRectangle.Left + 3, e.ClipRectangle.Top + 3, e.ClipRectangle.Width - 6, e.ClipRectangle.Height - 3);

			e.Graphics.FillRectangle(_fillBrush, r);
			if (Selected)
			{
				e.Graphics.FillRectangle(_highlightBoxBrush, e.ClipRectangle.Left + 3, e.ClipRectangle.Top, e.ClipRectangle.Width - 6, 3); ;
			}
		}
	}
}
