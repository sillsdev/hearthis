using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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
			_highlightBoxBrush = new SolidBrush(AppPallette.HilightColor);

			//We'r'e doing ThreadPool instead of the more convenient BackgroundWorker based on experimentation and the advice on the web; we are doing relatively a lot of little threads here,
			//that don't really have to interact much with the UI until they are complete.
			var waitCallback = new WaitCallback(GetStatsInBackground);
			ThreadPool.QueueUserWorkItem(waitCallback, this);
		}

		private static void GetStatsInBackground(object stateInfo)
		{

			ChapterButton button = stateInfo as ChapterButton;
			button._percentageRecorded = button.ChapterInfo.CalculatePercentageRecorded();
			button._percentageTranslated = button.ChapterInfo.CalculatePercentageTranslated();
			lock (button)
			{
				if (button.IsHandleCreated && !button.IsDisposed)
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
				if (_selected != value)
				{
					_selected = value;
					Invalidate();
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			int fillWidth = Width - 4;
			int fillHeight = Height - 5;
			var r = new Rectangle(2, 3, fillWidth, fillHeight);
			if (Selected)
			{
				e.Graphics.FillRectangle(_highlightBoxBrush, 0, 0, Width, Height);
			}

			DrawBox(e.Graphics, r, Selected, _percentageTranslated, _percentageRecorded);
		}

		/// <summary>
		/// NB: used by both chapter and book buttons
		/// </summary>
		public static void DrawBox(Graphics g, Rectangle bounds, bool selected, int percentageTranslated,
			int percentageRecorded)
		{
			using (Brush fillBrush = new SolidBrush(percentageTranslated > 0 ? AppPallette.Blue : AppPallette.EmptyBoxColor))
			{
				g.FillRectangle(fillBrush, bounds);
			}

			g.SmoothingMode = SmoothingMode.AntiAlias;
			if (percentageRecorded > 0 && percentageRecorded < 100)
			{
				using (var pen = new Pen(AppPallette.HilightColor, 1))
				{
					g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
				}
			}
			else if (percentageRecorded == 100)
			{
				int v1 = bounds.Height / 2 + 3;
				int v2 = bounds.Height / 2 + 7;
				int v3 = bounds.Height / 2 - 2;

				Pen progressPen = AppPallette.CompleteProgressPen;


				if (percentageRecorded == 100)
				{
					//draw the first stroke of a check mark
					g.DrawLine(progressPen, 4, v1, 7, v2);
					//complete the checkmark
					g.DrawLine(progressPen, 7, v2, 10, v3);
				}
			}
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && Control.ModifierKeys == Keys.Control)
			{
				_dangerousMenu.Show(this, e.Location);
			}
		}

		private void _makeDummyRecordings_Click(object sender, EventArgs e)
		{
			ChapterInfo.MakeDummyRecordings();
			Invalidate();
		}


		private void _dangerousMenu_Opening(object sender, CancelEventArgs e)
		{

		}

		private void OnRemoveRecordingsClick(object sender, EventArgs e)
		{
			ChapterInfo.RemoveRecordings();
			Invalidate();
		}
	}
}
