using System;
using System.Drawing;
using System.Windows.Forms;
using HearThis.Script;

namespace HearThis.UI
{
	public partial class BookButton : UserControl
	{
		private readonly BookInfo _model;
		private bool _selected;
		private Brush _highlightBoxBrush;

		public BookButton(BookInfo model)
		{
			_model = model;
			InitializeComponent();
			int kMaxChapters = 150;//psalms
			Width = (int) (Width + ((double)model.ChapterCount / (double)kMaxChapters) * 33.0);
			_highlightBoxBrush = new SolidBrush(AppPallette.HilightColor);
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
			var r = new Rectangle(2, 3, Width - 4, Height - 5);

			if (Selected)
			{
				e.Graphics.FillRectangle(_highlightBoxBrush, 0, 0, Width, Height);
			}
			ChapterButton.DrawBox(e.Graphics, r, Selected, _model.CalculatePercentageTranslated(), _model.CalculatePercentageRecorded());
		}

	}
}
