using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Palaso.Code;

// Thanks to Tom Holt's "TimeSlider" for trick of switching user-draw off and on
// This is still kludgy. If you start over, look at http://social.msdn.microsoft.com/Forums/en-US/csharplanguage/thread/1ca64f79-a5aa-40e2-85be-30e3934ab6ac/

namespace HearThis.UI {
	/// <summary>
	/// This control is a trackbar, except draws indicators showing the status of each point represented by the bar.
	/// </summary>
	[ToolboxBitmap(typeof(TrackBar))]
	public class DiscontiguousProgressTrackBar : Control, ISupportInitialize
	{
		private const int RightMargin = 7;
		private const int LeftMargin = 0;

		/// <summary>
		/// Client should provided this. It should return an array of size 1+Maximum-Minimum
		/// </summary>
		public Func<Brush[]> GetSegmentBrushesMethod;

		/// <summary>
		/// the graphics object; the one that comes with the paint even doesn't work, I supposed because of the I'll-draw-it-no-you-draw-it trickery
		/// </summary>
		private Graphics _graphics = null;



		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private int _value;
		private int _maximum;
		private int _minimum;
		private bool CapturedMouse;

		public DiscontiguousProgressTrackBar()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint |
						  ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			InitializeComponent();
			GetSegmentBrushesMethod = GetSegmentBrushesTest;

			MouseClick+=new MouseEventHandler(OnMouseClick);
		}

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			GetValueFromMouseEvent(e);
		}

		public int Value
		{
			get {
				return _value;
			}
			set
			{
				var oldValue = _value;
				_value = value;
				if (oldValue != value && ValueChanged!=null)
					ValueChanged(this, null);

				Invalidate();
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			CapturedMouse = ThumbRectangle.Contains(e.X, e.Y);
			if (!CapturedMouse)
			{
				return;
			}
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			CapturedMouse = false;
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.Left && CapturedMouse)
			{
				GetValueFromMouseEvent(e);
				Invalidate();
			}
		}

		private void GetValueFromMouseEvent(MouseEventArgs e)
		{
			var v = GetValueFromPosition(e.X);
			Value = Math.Max(Minimum, Math.Min(Maximum, v));
		}


		/// <summary>
		/// OnPaint event.
		/// We are kind of tricking the system, because I want to paint
		/// on top of the trackbar. The system either wants to draw it all
		/// and not send OnPaint events or let me do everything. So, we say
		/// I'll-do-it-no-you-do-it-okay-I'll-do-it.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{

			if(_graphics==null)
				_graphics = Graphics.FromHwnd(base.Handle);

			const int top = 8;
			const int height = 4;

			//erase
			e.Graphics.FillRectangle(AppPallette.BackgroundBrush, new Rectangle(0,0,Width,25));

			//draw the bar
			e.Graphics.FillRectangle(AppPallette.DisabledBrush, LeftMargin,top, Width - RightMargin - LeftMargin,3);

			Brush[] brushes = GetSegmentBrushesMethod();

			try
			{
				int barWidth = Width - LeftMargin - RightMargin;
				int segmentCount = 1 + Maximum - Minimum;
				// Do NOT make this an int, or rounding error mounts up as we multiply it by integers up to Maximum
				float segmentLength = (barWidth)/(float) segmentCount;
				if (Maximum > Minimum) // review this special case... currently max=min means it's empty
				{
					Guard.Against(brushes.Length != segmentCount,
								  string.Format(
									  "Expected number of brushes to equal the 1 + maximum-minimum value of the trackBar (1+{0}-{1}={2}) but it was {3}.",
									  Maximum, Minimum, segmentCount, brushes.Length));
					int segmentLeft = LeftMargin;
					for (int i = Minimum; i <= Maximum; i++)
					{
						// It's important to compute this with floats, to avoid accumulating rounding errors.
						int segmentRight = LeftMargin + (int)((i - Minimum + 1)*segmentLength);
						int segmentWidth = Math.Max(segmentRight - segmentLeft - 1, 1); // leave 1-pixel gap between, unless that makes it vanish
						//           if (SafeValue != i) // don't draw under the button
						_graphics.FillRectangle(brushes[i - Minimum], segmentLeft, top, segmentWidth, height);
						segmentLeft = segmentRight;
					}
					//draw the thumbThingy, making it the same color as the indicator underneath at this point
					if (brushes.Length > Value)
						e.Graphics.FillRectangle(brushes[Value]==Brushes.Transparent ? AppPallette.DisabledBrush : brushes[Value], ThumbRectangle);


				}
			}
			catch (Exception)
				{
#if DEBUG
					throw;
#endif
				}
		   // base.SetStyle(ControlStyles.UserPaint, true);
		}

		private Rectangle ThumbRectangle
		{
			get
			{
				int usableWidth = Width- LeftMargin - RightMargin;
				int halfThumbWidth = (int) (kTHumbWidth/2.0);
				int segWidth = (int) ((float) usableWidth/(float) (Maximum - Minimum + 1));
				int halfSegWidth = segWidth/2;
				int center;
				if (segWidth >= kTHumbWidth)
				{
					// When segments are wider than the thumb, it looks good to center the thumb in the segment.
					float proportion = Value == 0 ? Minimum : ((float)Value / (float)(Maximum - Minimum + 1));
					center = LeftMargin + halfSegWidth + (int)((proportion * usableWidth));
				}
				else
				{
					// thumb is wider than a segment. If we center it on segment centers, it gets clipped
					// at the edges. Better to divide evenly the space between its extreme positions.
					usableWidth -= kTHumbWidth;
					float proportion = Value == 0 ? Minimum : ((float)Value / (float)(Maximum - Minimum));
					center = LeftMargin + halfThumbWidth + (int)((proportion * usableWidth));
				}
				var r = new Rectangle((int) (center - halfThumbWidth), 0, kTHumbWidth, 20);
				return r;
			}
		}

		protected int kTHumbWidth = 20;

		private int GetValueFromPosition(int x)
		{
			return (int) (((float) (x - LeftMargin)/(float) (Width - RightMargin - LeftMargin)*(Maximum - Minimum + 1)) + Minimum);
		}


		public int Minimum
		{
			get {
				return _minimum;
			}
			set {
				_minimum = value;
				Invalidate();
			}
		}

		public int Maximum
		{
			get {
				return _maximum;
			}
			set {
				_maximum = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Merely reading "Value" from the OnPaint causes OnValueChanged to never be called again!  So we use this instead.
		/// </summary>
		//private int SafeValue { get; set; }

		private Brush[] GetSegmentBrushesTest()
		{
			return GetSegsTest().ToArray();
		}
		private IEnumerable<Brush> GetSegsTest()
		{
			for (int i = Minimum; i <= Maximum; i++)
			{
				if (i == Minimum)
					yield return Brushes.Red;
				else if (i == Maximum)
					yield return Brushes.Orange;
				else if (i < 10 || Math.Round((double)(i / 3.0)) == i / 3.0)
					yield return Brushes.Blue;
				else
				{
					yield return Brushes.Transparent;
				}
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if ( disposing && ( components != null ) )
			{
				if(_graphics!=null)
				{
					_graphics.Dispose();
					_graphics = null;
				}
			}
			base.Dispose(disposing);
		}


		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		public event EventHandler ValueChanged;
		public void BeginInit()
		{

		}

		public void EndInit()
		{

		}
	}
}