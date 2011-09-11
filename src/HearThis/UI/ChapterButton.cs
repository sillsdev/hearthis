using System.Drawing;
using System.Windows.Forms;
using HearThis.Script;

namespace HearThis.UI
{
	public partial class ChapterButton : UserControl
	{
		private bool _selected;
		private Brush _highlightBoxBrush;
		private SolidBrush _fillBrush;
		private int _percentageRecorded;

		public ChapterButton(ChapterInfo chapterInfo)
		{
			ChapterInfo = chapterInfo;
			InitializeComponent();
			_highlightBoxBrush = new SolidBrush(AppPallette.Orange);


//            if (chapterInfo.HasAllRecordings)
//                _fillBrush = new SolidBrush(AppPallette.Blue);
//            else if (chapterInfo.HasSomeRecordings)
//                _fillBrush = new SolidBrush(AppPallette.Red);
//            else
				_fillBrush = new SolidBrush(chapterInfo.HasVerses ? AppPallette.DarkGray : Color.WhiteSmoke);

			//_fillBrush = new SolidBrush(AppPallette.DarkGray);

			RefreshStatistics();
		}

		private void RefreshStatistics()
		{
			_percentageRecorded =  ChapterInfo.CalculatePercentageRecorded();
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
			int greyWidth = Width - 4;
			int greyHeight = Height - 5;
			var r = new Rectangle( 2, 3, greyWidth, greyHeight);
			var shadow = new Rectangle(r.Left, r.Top,r.Width,r.Height);
			shadow.Offset(1,1);
			if (Selected)
			{
				e.Graphics.FillRectangle(_highlightBoxBrush, 0, 0, Width, Height);
			}
			else
			{
				e.Graphics.FillRectangle(Brushes.Black, shadow);
			}
			e.Graphics.FillRectangle(_fillBrush, r);
			if(_percentageRecorded >0)
			{
				int recordedColor = (int) (greyWidth/(100.0/(float)_percentageRecorded));
				r = new Rectangle(2,3, recordedColor, greyWidth);
				e.Graphics.FillRectangle(AppPallette.BlueBrush, r);
			}
		}
	}
}
