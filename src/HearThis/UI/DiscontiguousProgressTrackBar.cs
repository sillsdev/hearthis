// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using SIL.Code;

// Thanks to Tom Holt's "TimeSlider" for trick of switching user-draw off and on
// This is still kludgy. If you start over, look at http://social.msdn.microsoft.com/Forums/en-US/csharplanguage/thread/1ca64f79-a5aa-40e2-85be-30e3934ab6ac/

namespace HearThis.UI
{
	/// <summary>
	/// This control is a trackbar, except draws indicators showing the status of each point represented by the bar.
	/// </summary>
	[ToolboxBitmap(typeof (TrackBar))]
	public class DiscontiguousProgressTrackBar : Control
	{
		private int _thumbWidth = 20;
		private int _minGapWidth = 1;
		private int _value;

		private bool _capturedMouse;
		private Func<SegmentPaintInfo[]> _getSegmentBrushes;
		private SegmentPaintInfo[] _currentSegmentBrushes;

		/// <summary>
		/// Client should provide this.
		/// </summary>
		public Func<SegmentPaintInfo[]> GetSegmentBrushesDelegate
		{
			set
			{
				_getSegmentBrushes = value;
				_currentSegmentBrushes = null;
			}
		}

		public override void Refresh()
		{
			base.Refresh(); // This forces an immediate re-paint, so the current brushes array will be repopulated if necessary.
			Enabled = SegmentCount != 0;
		}

		private void PopulateSegmentBrushes()
		{
			if (_getSegmentBrushes != null)
				_currentSegmentBrushes = _getSegmentBrushes?.Invoke();
			else if (_currentSegmentBrushes == null)
				SegmentCount = 0;
			Guard.AgainstNull(_currentSegmentBrushes, "_currentSegmentBrushes");
		}

		public DiscontiguousProgressTrackBar()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			MouseClick += OnMouseClick;
		}

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			SetValueFromMouseEvent(e);
		}

		public bool Finished => _value == SegmentCount && SegmentCount > 0;

		[DefaultValue(20)]
		public int ThumbWidth
		{
			get => _thumbWidth;
			set
			{
				if (value >= 0 && value < (Width - Padding.Horizontal) / 2)
					_thumbWidth = value;
			}
		}

		private int HalfThumbWidth => ThumbWidth / 2;

		[DefaultValue(1)]
		public int MinimumGapWidth
		{
			get => _minGapWidth;
			set
			{
				if (value >= 0 && value < (Width - Padding.Horizontal) / 2)
					_minGapWidth = value;
			}
		}

		/// <summary>
		/// 0-based
		/// </summary>
		[DefaultValue(0)]
		public int Value
		{
			get { return _value; }
			set
			{
				// Prevent value from going negative or exceeding SegmentCount.
				var oldValue = _value;
				_value = Math.Min(Math.Max(value, 0), SegmentCount);
				if (oldValue != _value && ValueChanged != null)
					ValueChanged(this, null);

				Invalidate();
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (Finished)
				return;

			_capturedMouse = ThumbRectangle.Contains(e.X, e.Y);
			if (!_capturedMouse)
				return;
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_capturedMouse = false;
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.Left && _capturedMouse)
			{
				SetValueFromMouseEvent(e);
			}
		}

		private void SetValueFromMouseEvent(MouseEventArgs e)
		{
			Value = GetValueFromPosition(e.X);
			Invalidate();
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
			int top = ThumbTop + ThumbHeightAboveBar;
			int height = ThumbHeight / 5;

			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			//erase
			e.Graphics.FillRectangle(AppPallette.BackgroundBrush, new Rectangle(0, 0, Width, Height));

			PopulateSegmentBrushes();
			try
			{
				// Do NOT make this an int, or rounding error mounts up as we multiply it by integers up to Maximum
				float segmentLength = BarWidth / (float) SegmentCount;
				if (SegmentCount > 0) // review this special case... currently max=min means it's empty
				{
					int segmentLeft = Padding.Left;
					for (int i = 0; i < SegmentCount; i++)
					{
						// It's important to compute this with floats, to avoid accumulating rounding errors.
						int segmentRight = Padding.Left + (int) ((i + 1) * segmentLength);
						// Leave a gap between, unless that makes it vanish
						int segmentWidth = Math.Max(segmentRight - segmentLeft - MinimumGapWidth, 1);
						// When the segments are very wide relative to the gap, the gap is hard to notice.
						if (segmentWidth >= MinimumGapWidth * 80)
							segmentWidth -= segmentWidth / 80;
						e.Graphics.FillRectangle(_currentSegmentBrushes[i].MainBrush, segmentLeft, top, segmentWidth, height);
						if (_currentSegmentBrushes[i].UnderlineBrush != null)
						{
							int underlineThickness = Math.Max(height/3, 1);
							e.Graphics.FillRectangle(_currentSegmentBrushes[i].UnderlineBrush, segmentLeft, top + height - underlineThickness, segmentWidth, underlineThickness);
						}
						if (i != Value)
						{
							if (_currentSegmentBrushes[i].Symbol != null)
							{
								var text = _currentSegmentBrushes[i].Symbol;
								var size = e.Graphics.MeasureString(text, Font);
								var leftString = segmentLeft + segmentWidth / 2 - size.Width / 2;
								var topString = top + height / 2 - size.Height / 2;
								e.Graphics.DrawString(text, Font, AppPallette.DisabledBrush, leftString, topString);
							}

							_currentSegmentBrushes[i].PaintIconDelegate?.Invoke(
								e.Graphics, new Rectangle(segmentLeft, Padding.Top, segmentWidth, top), false);
						}
						segmentLeft = segmentRight;
					}
					// If not showing the "finished" state, draw the thumbThingy, making it the same color as the indicator underneath at this point
					if (SegmentCount > Value)
					{
						var rect = ThumbRectangle;
						var overlayText = _currentSegmentBrushes[Value].Symbol;
						var fillBrush = _currentSegmentBrushes[Value].MainBrush;
						if (fillBrush == Brushes.Transparent || overlayText != null)
							fillBrush = AppPallette.DisabledBrush;

						e.Graphics.FillRectangle(fillBrush, rect);

						if (overlayText != null || _currentSegmentBrushes[Value].PaintIconDelegate != null)
						{
							var boundingRect = new Rectangle(rect.Location, new Size(rect.Width, rect.Height - 1));
							e.Graphics.DrawRectangle(AppPallette.ProblemHighlightPen, boundingRect);
							if (overlayText != null)
							{
								var size = e.Graphics.MeasureString(overlayText, Font);
								var leftString = rect.Left + rect.Width / 2 - size.Width / 2;
								var topString = rect.Top + rect.Height / 2 - size.Height / 2;
								e.Graphics.DrawString(overlayText, Font, _currentSegmentBrushes[Value].MainBrush, leftString, topString);
							}

							boundingRect.Y++;
							boundingRect.Height--;
							_currentSegmentBrushes[Value].PaintIconDelegate?.Invoke(
								e.Graphics, boundingRect, true);
						}
					}
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

		public int SegmentCount
		{
			get
			{
				if (_currentSegmentBrushes == null)
					PopulateSegmentBrushes();
				return _currentSegmentBrushes.Length;
			}
			set
			{
				if (_getSegmentBrushes != null)
					throw new InvalidOperationException("Once GetSegmentBrushesDelegate has been set, this setter should not be used. Only valid in Designer or in tests.");
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", "The value of SegmentCount must not be negative.");
				_currentSegmentBrushes = GetSegBrushesProvisional(value).ToArray();
				Invalidate();
			}
		}

		internal int BarWidth
		{
			get => Width - Padding.Left - Padding.Right;
			set => Width = value + Padding.Left + Padding.Right;
// setter used for testing
		}

		private int ThumbTop => Math.Max(Padding.Top, FontHeight - ThumbHeightAboveBar);
		private int ThumbHeight => Height - Padding.Vertical;
		private int ThumbHeightAboveBar => ThumbHeight * 2 / 5;

		internal Rectangle ThumbRectangle
		{
			get
			{
				if (Finished || SegmentCount == 0)
					return new Rectangle();

				int usableWidth = BarWidth;
				float segWidth = (float) usableWidth / (SegmentCount); // This includes the gap width
				int left = Padding.Left;
				if (segWidth == ThumbWidth)
				{
					// When segments (including gap) are the same width as the thumb, the thumb should always
					// align with the semgent's left edge.
					left += RoundTowardClosestEdge(Value * segWidth);
				}
				else if (segWidth >= ThumbWidth)
				{
					// When segments are wider than the thumb, it looks good to "center" the thumb in the segment,
					// adjusted proportionately based on where it is in the overall sequence
					float halfSegWidth = segWidth / 2;
					left += RoundTowardClosestEdge(Value * segWidth + halfSegWidth) - HalfThumbWidth;
				}
				else
				{
					// thumb is wider than a segment. If we center it on segment centers, it gets clipped
					// at the edges. Better to divide evenly the space between its extreme positions.
					usableWidth -= ThumbWidth;
					float proportion = (float) Value / (SegmentCount - 1);
					left += RoundTowardClosestEdge(proportion * usableWidth);
				}
				var r = new Rectangle(left, ThumbTop, ThumbWidth, ThumbHeight);
				return r;
			}
		}

		private int RoundTowardClosestEdge(double val)
		{
			if (Value < SegmentCount / 2)
			{
				int truncatedValue = (int) Math.Truncate(val);
				var frac = val - truncatedValue;
				if (Equals(frac, 0.5))
				{
					return truncatedValue;
				}
			}
			return (int) Math.Round(val, MidpointRounding.AwayFromZero);
		}

		private int GetValueFromPosition(int x)
		{
			int val = (int) ((x - Padding.Left) / (float) BarWidth * (SegmentCount));
			// Deal with special case where user clicks to the right or left of the thumb, even if that position is actually associated
			// with a segment other than the immediately adjacent one.
			if (val == Value)
			{
				if (x < ThumbRectangle.Left)
					val--;
				if (x > ThumbRectangle.Right)
					val++;
			}
			return val;
		}

		// This is only used for tests and in Designer. In production, client should call InitializeClientDelegates
		// to provide a real-life implementation of GetSegmentBrushes.
		private IEnumerable<SegmentPaintInfo> GetSegBrushesProvisional(int segmentCount)
		{
			for (int i = 0; i < segmentCount; i++)
			{
				var result = Brushes.Transparent;
				if (i == 0)
					result = Brushes.Red;
				else if (i == segmentCount - 1)
					result = Brushes.Orange;
				else if (i < 10 || i % 3 == 0)
					result = Brushes.Blue;
				yield return new SegmentPaintInfo() {MainBrush = result};
			}
		}

		public event EventHandler ValueChanged;
	}

	public class SegmentPaintInfo
	{
		public Brush MainBrush;
		public Brush UnderlineBrush;
		public string Symbol;
		public Action<Graphics, Rectangle, bool> PaintIconDelegate;
	}
}
