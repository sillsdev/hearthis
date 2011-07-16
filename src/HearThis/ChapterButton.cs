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
	public partial class ChapterButton : UserControl
	{
		private bool _selected;
		private Brush _highlightBoxBrush;

		public ChapterButton(ChapterInfo chapterInfo)
		{
			ChapterInfo = chapterInfo;
			InitializeComponent();
			_highlightBoxBrush = new SolidBrush(Color.FromArgb(255,168, 0));
		}

		public ChapterInfo ChapterInfo { get; private set; }

		public bool Selected
		{
			get { return _selected; }
			set
			{
				if(_selected !=value)
				{
					_selected = value;
					Invalidate();
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var r = new Rectangle(e.ClipRectangle.Left+3, e.ClipRectangle.Top + 3, e.ClipRectangle.Width-6, e.ClipRectangle.Height-3);

			e.Graphics.FillRectangle(Brushes.LightGray, r);
			if (Selected)
			{
				e.Graphics.FillRectangle(_highlightBoxBrush, e.ClipRectangle.Left+3,e.ClipRectangle.Top, e.ClipRectangle.Width-6, 3);;
			}
		}
	}
}
