using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using HearThis.Script;

namespace HearThis.UI
{
	public partial class ChapterButton : UserControl
	{
		private bool _selected;
		private Brush _highlightBoxBrush;
		private int _percentageRecorded;
		private int _percentageTranslated;

		public ChapterButton(ChapterInfo chapterInfo)
		{
			ChapterInfo = chapterInfo;
			InitializeComponent();
			_highlightBoxBrush = new SolidBrush(AppPallette.Orange);

			//We'r'e doing ThreadPool instead of the more convenient BackgroundWorker based on experimentation and the advice on the web; we are doing relatively a lot of little threads here,
			//that don't really have to interact much with the UI until they are complete.
			var waitCallback = new WaitCallback(GetStatsInBackground);
			ThreadPool.QueueUserWorkItem(waitCallback, this);
		}

		static void GetStatsInBackground(object stateInfo)
		{

			ChapterButton button = stateInfo as ChapterButton;
			button._percentageRecorded = button.ChapterInfo.CalculatePercentageRecorded();
			button._percentageTranslated =  button.ChapterInfo.CalculatePercentageTranslated();
			lock(button)
			{
				if(button.IsHandleCreated && !button.IsDisposed)
				{
					button.Invoke(new Action(delegate { button.Invalidate(); }));
				}
			}
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
				e.Graphics.FillRectangle(Brushes.Gray, shadow);
			}
			using (Brush _fillBrush = new SolidBrush(_percentageTranslated > 0 ? AppPallette.DarkGray : Color.WhiteSmoke))
			{
				e.Graphics.FillRectangle(_fillBrush, r);
			}
			if(_percentageRecorded >0)
			{
				int recordedWidth = Math.Max(2, (int) (greyWidth/(100.0/(float)_percentageRecorded)));
				r = new Rectangle(2,3, recordedWidth, greyHeight);
				e.Graphics.FillRectangle(AppPallette.BlueBrush, r);
			}
		}
	}
}
