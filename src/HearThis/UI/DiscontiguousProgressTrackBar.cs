// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2016, SIL International. All Rights Reserved.
// <copyright from='2011' to='2016' company='SIL International'>
//		Copyright (c) 2016, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
		private const int kRightMargin = 7;
		private const int kLeftMargin = 0;
		private const int kThumbWidth = 20;
		private const int kGapWidth = 1;
		private const int kHalfThumbWidth = kThumbWidth / 2;

		private int _value;

		private bool _capturedMouse;
		private Func<Brush[]> _getSegmentBrushes;
		private Brush[] _currentSegmentBrushes;

		/// <summary>
		/// Client should provide this.
		/// </summary>
		public Func<Brush[]> GetSegmentBrushesDelegate
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
			const int top = 8;
			const int height = 4;

			//erase
			e.Graphics.FillRectangle(AppPallette.BackgroundBrush, new Rectangle(0, 0, Width, 25));

			//draw the bar
			e.Graphics.FillRectangle(AppPallette.DisabledBrush, kLeftMargin, top, BarWidth, 3);

			PopulateSegmentBrushes();
			try
			{
				// Do NOT make this an int, or rounding error mounts up as we multiply it by integers up to Maximum
				float segmentLength = BarWidth / (float) SegmentCount;
				if (SegmentCount > 0) // review this special case... currently max=min means it's empty
				{
					int segmentLeft = kLeftMargin;
					for (int i = 0; i < SegmentCount; i++)
					{
						// It's important to compute this with floats, to avoid accumulating rounding errors.
						int segmentRight = kLeftMargin + (int) ((i + 1) * segmentLength);
						int segmentWidth = Math.Max(segmentRight - segmentLeft - kGapWidth, 1);
							// leave gap between, unless that makes it vanish
						e.Graphics.FillRectangle(_currentSegmentBrushes[i], segmentLeft, top, segmentWidth, height);
						segmentLeft = segmentRight;
					}
					// If not showing the "finished" state, draw the thumbThingy, making it the same color as the indicator underneath at this point
					if (SegmentCount > Value)
						e.Graphics.FillRectangle(_currentSegmentBrushes[Value] == Brushes.Transparent ? AppPallette.DisabledBrush : _currentSegmentBrushes[Value],
							ThumbRectangle);
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
			get { return Width - kLeftMargin - kRightMargin; }
			set { Width = value + kLeftMargin + kRightMargin; } // setter used for testing
		}

		internal Rectangle ThumbRectangle
		{
			get
			{
				if (Finished || SegmentCount == 0)
					return new Rectangle();

				int usableWidth = BarWidth;
				float segWidth = (float) usableWidth / (SegmentCount); // This includes the gap width
				int left = kLeftMargin;
				if (segWidth == kThumbWidth)
				{
					// When segments (including gap) are the same width as the thumb, the thumb should always
					// align with the semgent's left edge.
					left += RoundTowardClosestEdge(Value * segWidth);
				}
				else if (segWidth >= kThumbWidth)
				{
					// When segments are wider than the thumb, it looks good to "center" the thumb in the segment,
					// adjusted proportionately based on where it is in the overall sequence
					float halfSegWidth = segWidth / 2;
					left += RoundTowardClosestEdge(Value * segWidth + halfSegWidth) - kHalfThumbWidth;
				}
				else
				{
					// thumb is wider than a segment. If we center it on segment centers, it gets clipped
					// at the edges. Better to divide evenly the space between its extreme positions.
					usableWidth -= kThumbWidth;
					float proportion = (float) Value / (SegmentCount - 1);
					left += RoundTowardClosestEdge(proportion * usableWidth);
				}
				var r = new Rectangle(left, 0, kThumbWidth, 20);
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
			int val = (int) ((x - kLeftMargin) / (float) BarWidth * (SegmentCount));
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
		private IEnumerable<Brush> GetSegBrushesProvisional(int segmentCount)
		{
			for (int i = 0; i < segmentCount; i++)
			{
				if (i == 0)
					yield return Brushes.Red;
				else if (i == segmentCount - 1)
					yield return Brushes.Orange;
				else if (i < 10 || i % 3 == 0)
					yield return Brushes.Blue;
				else
				{
					yield return Brushes.Transparent;
				}
			}
		}

		public event EventHandler ValueChanged;
	}
}