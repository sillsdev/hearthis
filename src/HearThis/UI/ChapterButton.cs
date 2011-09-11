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
			if (Selected)
			{
				e.Graphics.FillRectangle(_highlightBoxBrush, e.ClipRectangle.Left, e.ClipRectangle.Top, Width, Height);
			}
			var r = new Rectangle(e.ClipRectangle.Left + 2, e.ClipRectangle.Top + 3, Width - 4, Height - 5);
			e.Graphics.FillRectangle(_fillBrush, r);
		}
	}
}
