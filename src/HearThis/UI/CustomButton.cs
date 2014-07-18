using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace HearThis.UI
{
	public class RecordButton : CustomButton
	{
		private bool _waiting;


		public RecordButton()
		{
		}

		public bool Waiting
		{
			get { return _waiting; }
			set
			{
				_waiting = value;
				Invalidate();
			}
		}


		protected override void Draw(Graphics g)
		{
			int dim = Math.Min(Width, Height) - 2;

			g.SmoothingMode = SmoothingMode.AntiAlias;

			switch (State)
			{
				case BtnState.Normal:
					g.FillEllipse(AppPallette.BlueBrush, 1, 1, dim, dim);
					if(IsDefault)
						g.DrawEllipse(_highlightPen, 1, 1, dim-1, dim-1);
					break;
				case BtnState.Pushed:
					g.FillEllipse(Waiting ? AppPallette.ButtonWaitingBrush : AppPallette.ButtonRecordingBrush, 1, 1, dim, dim);
					break;
				case BtnState.Inactive:
					g.FillEllipse(AppPallette.DisabledBrush, 1, 1, dim, dim);
					break;
				case BtnState.MouseOver:
					g.FillEllipse(AppPallette.BlueBrush, 1, 1, dim, dim);
					g.DrawEllipse(AppPallette.ButtonMouseOverPen, 1, 1, dim-1, dim-1);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}


	public class PlayButton : CustomButton
	{
		private bool _playing;

		protected override void Draw(Graphics g)
		{
			var vertices = new Point[3];
			vertices[0] = new Point(0,0);
			vertices[1] = new Point(0, Height-1);
			vertices[2] = new Point(Width-1, Height/2-1);

			g.SmoothingMode = SmoothingMode.AntiAlias;

			if (Playing)
			{
				var pushedVertices = GetPushedPoints(vertices);
				g.FillPolygon(AppPallette.DisabledBrush, pushedVertices);
				g.DrawPolygon(AppPallette.ButtonMouseOverPen, vertices);
			}
			else switch (State)
			{
				 case BtnState.Normal:
					g.FillPolygon(AppPallette.BlueBrush, vertices);
					if (IsDefault)
						g.DrawPolygon(_highlightPen, vertices);
					break;
				case BtnState.Pushed:
					var pushedVertices = GetPushedPoints(vertices);
					 g.FillPolygon(AppPallette.BlueBrush, pushedVertices);
					 break;
				case BtnState.Inactive:
					 g.FillPolygon(AppPallette.DisabledBrush, vertices);
					break;
				case BtnState.MouseOver:
					g.FillPolygon(AppPallette.BlueBrush, vertices);
					g.DrawPolygon(AppPallette.ButtonMouseOverPen, vertices);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public bool Playing
		{
			get
			{
				return _playing;
			}
			set
			{
				_playing = value;
				Invalidate();
			}
		}
	}

	public class ArrowButton : CustomButton
	{
		protected override void Draw(Graphics g)
		{
			var thick = 11;
			var stem = 12;
			var vertices = new Point[7];
			vertices[0] = new Point(0, Height/2 - thick/2); // upper left corner of stem
			vertices[1] = new Point(0, Height/2 + thick/2); // lower left corner of stem
			vertices[2] = new Point(stem, Height/2 + thick/2); // lower junction of stem and arrow
			vertices[3] = new Point(stem, Height); // lower point of arrow
			vertices[4] = new Point(Width - 1, Height/2); // tip of arrow
			vertices[5] = new Point(stem, 0); // upper point of arrow
			vertices[6] = new Point(stem, Height / 2 - thick / 2); // upper junction of stem and arrow

			g.SmoothingMode = SmoothingMode.AntiAlias;

			switch (State)
			{
				case BtnState.Normal:
					g.FillPolygon(AppPallette.BlueBrush, vertices);
					if (IsDefault)
						g.DrawPolygon(_highlightPen, vertices);
					break;
				case BtnState.Pushed:
					var pushedVertices = GetPushedPoints(vertices);
					g.FillPolygon(AppPallette.BlueBrush, pushedVertices);
					break;
				case BtnState.Inactive:
					g.FillPolygon(AppPallette.DisabledBrush, vertices);
					break;
				case BtnState.MouseOver:
					g.FillPolygon(AppPallette.BlueBrush, vertices);
				   g.DrawPolygon(AppPallette.ButtonMouseOverPen, vertices);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public class SkipButton : CustomButton
	{
		private float height;
		private float width;
		private float triangleHeight;
		private float right;
		private float top;
		private float bottom;
		private float dyMiddleOfCurve;

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			height = Height - Padding.Bottom - Padding.Top + 2 * AppPallette.ButtonMouseOverPen.Width;
			width = Width - Padding.Left - Padding.Right - 2 * AppPallette.ButtonMouseOverPen.Width;
			triangleHeight = height / 7F;
			right = Width - Padding.Right - AppPallette.ButtonMouseOverPen.Width;
			top = Padding.Top + AppPallette.ButtonMouseOverPen.Width;
			bottom = Height - Padding.Bottom - AppPallette.ButtonMouseOverPen.Width;
			dyMiddleOfCurve = top + bottom / 2F;
		}

		protected override void Draw(Graphics g)
		{
			float lineThickness = Math.Min(6F, width / 4F);
			if (State == BtnState.MouseOver)
				lineThickness += 2F;
			float triangleWidth = lineThickness * 3;
			float lineRight = right - triangleWidth / 2F;
			float left = Padding.Left + lineThickness / 2F + AppPallette.ButtonMouseOverPen.Width;

			PointF startPt = new PointF(lineRight, top);
			PointF midPt = new PointF(left, dyMiddleOfCurve);
			PointF endPt = new PointF(lineRight, bottom - triangleHeight /2F);
			Color lineColor;
			Brush fillBrush;
			if (State == BtnState.Inactive)
			{
				lineColor = AppPallette.EmptyBoxColor;
				fillBrush = AppPallette.DisabledBrush;
			}
			else
			{
				lineColor = AppPallette.Blue;
				fillBrush = AppPallette.BlueBrush;
			}

			g.SmoothingMode = SmoothingMode.AntiAlias;

			using (var pen = new Pen(lineColor, lineThickness))
			{
				// Draw the curved line.
				PointF control1 = new PointF(lineRight, top + Height / 3F);
				PointF control2 = new PointF(left, dyMiddleOfCurve - Height / 4F);
				PointF control3 = new PointF(left, dyMiddleOfCurve + Height / 4F);
				PointF control4 = new PointF(lineRight, endPt.Y - Height / 3F);
				PointF[] bezierPoints =
				{
					startPt, control1, control2, midPt,
					control3, control4, endPt
				};

				if (State == BtnState.Pushed)
					bezierPoints = GetPushedPoints(bezierPoints);

				g.DrawBeziers(pen, bezierPoints);

				if (State == BtnState.MouseOver)
				{
					float adj = (lineThickness + AppPallette.ButtonMouseOverPen.Width) / 2F;
					g.DrawLine(AppPallette.ButtonMouseOverPen, new PointF(startPt.X - adj, top),
						new PointF(startPt.X + adj, top));

					for (int index = 0; index < bezierPoints.Length; index++)
					{
						PointF pt = bezierPoints[index];
						bezierPoints[index] = new PointF(pt.X - lineThickness / 2,pt.Y);
					}
					g.DrawBeziers(AppPallette.ButtonMouseOverPen, bezierPoints);
					for (int index = 0; index < bezierPoints.Length; index++)
					{
						PointF pt = bezierPoints[index];
						bezierPoints[index] = new PointF(pt.X + lineThickness, pt.Y);
					}
					g.DrawBeziers(AppPallette.ButtonMouseOverPen, bezierPoints);
				}

				// Draw the triangle
				var vertices = new PointF[3];
				vertices[0] = new PointF(lineRight - triangleWidth / 2F, bottom - triangleHeight); // left corner
				vertices[1] = new PointF(right, bottom - triangleHeight); // right corner
				vertices[2] = new PointF(lineRight, bottom); // point

				if (State == BtnState.Pushed)
					vertices = GetPushedPoints(vertices);

				g.FillPolygon(fillBrush, vertices);
				if (State == BtnState.MouseOver)
				{
					g.DrawPolygon(AppPallette.ButtonMouseOverPen, vertices);
					// Fix the little piece where the stem connects to the triangle.
					g.SmoothingMode = SmoothingMode.None;

					float middleOfTriangleBase = vertices[0].X + (vertices[1].X - vertices[0].X) / 2F;
					float adj = AppPallette.ButtonMouseOverPen.Width / 2 + 1;
					g.FillPolygon(AppPallette.BlueBrush, new[]
					{
						new PointF(middleOfTriangleBase - 1, vertices[0].Y - adj),
						new PointF(middleOfTriangleBase - lineThickness - 1, vertices[0].Y + adj),
						new PointF(middleOfTriangleBase + lineThickness, vertices[0].Y + adj)
					});

					g.SmoothingMode = SmoothingMode.AntiAlias;
				}
			}

			lineThickness = lineThickness / 2;
			const float thinLineWidth = 1F;
			if (State == BtnState.MouseOver)
				lineThickness = lineThickness - thinLineWidth;

			using (var pen = new Pen(State == BtnState.Pushed ? AppPallette.Red : lineColor, lineThickness))
			{
				// Draw the text lines
				float dyTopLine = dyMiddleOfCurve - 1 - lineThickness / 2F;
				float leftEdge = Padding.Left + width / 2F;
				if (State == BtnState.MouseOver)
					leftEdge += AppPallette.ButtonMouseOverPen.Width;
				g.DrawLine(pen, new PointF(leftEdge, dyTopLine), new PointF(right, dyTopLine));
				float dyBottomLine = dyMiddleOfCurve + 1 + lineThickness / 2F;
				g.DrawLine(pen, new PointF(leftEdge, dyBottomLine), new PointF(right - lineThickness, dyBottomLine));

				if (State == BtnState.MouseOver)
				{
					using (var thinMouseOverPen = new Pen(AppPallette.ButtonMouseOverPen.Color, thinLineWidth))
					{
						// Highlight the text lines
						g.SmoothingMode = SmoothingMode.None;
						dyTopLine -= lineThickness;
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyTopLine), new PointF(right, dyTopLine));
						dyTopLine += lineThickness * 2 - 2 * thinLineWidth;
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyTopLine), new PointF(right, dyTopLine));
						dyBottomLine -= (lineThickness - 2 * thinLineWidth);
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyBottomLine), new PointF(right - lineThickness, dyBottomLine));
						dyBottomLine += lineThickness * 2 - 2 * thinLineWidth;
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyBottomLine), new PointF(right - lineThickness, dyBottomLine));
					}
				}
			}
		}
	}

	public enum BtnState
	{
		/// <summary>
		/// The button is disabled.
		/// </summary>
		Inactive = 0,
		/// <summary>
		/// The button is in it normal unpressed state
		/// </summary>
		Normal = 1,
		/// <summary>
		/// The location of the mouse is over the button
		/// </summary>
		MouseOver = 2,
		/// <summary>
		/// The button is currently being pressed
		/// </summary>
		Pushed = 3,
	}

	public class CustomButton : Control
	{
		private bool CapturingMouse;
		private BtnState _state = BtnState.Normal;
		protected Pen _highlightPen;
		private bool _isDefault;

		public CustomButton()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Opaque, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			BackColor = AppPallette.Background;

			_highlightPen = AppPallette.ButtonSuggestedPen;
		}

		public bool IsDefault
		{
			get { return _isDefault; }
			set
			{
				_isDefault = value;
				Invalidate();
			}
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			if (DesignMode)
				return;
			if (Enabled)
			{
				if (_state == BtnState.Inactive)
					State = BtnState.Normal;
			}
			else
				State = BtnState.Inactive;
			Invalidate();
		}

		public BtnState State
		{
			get { return !Enabled ? BtnState.Inactive : _state; }
			set
			{
				if (_state == value)
					return;
				_state = value;
				if (_state == BtnState.Inactive && Enabled)
					Enabled = false;
				else if (_state != BtnState.Inactive && !Enabled)
					Enabled = true;
				Invalidate();
			}
		}

		 public Func<bool> CancellableMouseDownCall { get; set; }

		#region Events Methods
		/// <summary>
		/// Mouse Down Event:
		/// set BtnState to Pushed and Capturing mouse to true
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (CancellableMouseDownCall != null)
			{
				if (!CancellableMouseDownCall())
					return;
			}
			base.OnMouseDown(e);
			Capture = true;
			CapturingMouse = true;
			State = BtnState.Pushed;
		}
		/// <summary>
		/// Mouse Up Event:
		/// Set BtnState to Normal and set CapturingMouse to false
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (Enabled)
				State = BtnState.Normal;
			CapturingMouse = false;
			Capture = false;
			Invalidate();
		}
		/// <summary>
		/// Mouse Leave Event:
		/// Set BtnState to normal if we CapturingMouse = true
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (Enabled && !CapturingMouse)
				State = BtnState.Normal;
		}
		/// <summary>
		/// Mouse Move Event:
		/// If CapturingMouse = true and mouse coordinates are within button region,
		/// set BtnState to Pushed, otherwise set BtnState to Normal.
		/// If CapturingMouse = false, then set BtnState to MouseOver
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (CapturingMouse)
			{
				Rectangle rect = new Rectangle(0, 0, Width, Height);
				if (Enabled)
				{
					State = BtnState.Normal;
					if ((e.X >= rect.Left) && (e.X <= rect.Right) && (e.Y >= rect.Top) && (e.Y <= rect.Bottom))
						State = BtnState.Pushed;
					Capture = true;
					Invalidate();
				}
			}
			else
			{
				if (Enabled)
				{
					if (State != BtnState.MouseOver)
						State = BtnState.MouseOver;
				}
			}
		}

		/// <summary>
		/// Lose Focus Event:
		/// set btnState to Normal
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			if (Enabled)
				State = BtnState.Normal;
			Invalidate();
		}
		#endregion

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			Graphics g = pevent.Graphics;
			g.FillRectangle(AppPallette.BackgroundBrush, 0, 0, Width, Height);
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			OnPaintBackground(pevent);
			Draw(pevent.Graphics);
		}

		protected virtual void Draw(Graphics g)
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.FillEllipse(AppPallette.BlueBrush, 0, 0, Width, Width);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				const int WS_EX_TRANSPARENT = 0x20;
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= WS_EX_TRANSPARENT;
				return cp;
			}
		}

		protected Point[] GetPushedPoints(Point[] vertices)
		{
			Point[] pushed = new Point[vertices.Length];
			for (int i = 0; i < pushed.Length; i++)
			{
				pushed[i].X = vertices[i].X + 1;
				pushed[i].Y = vertices[i].Y + 1;
			}
			return pushed;
		}

		protected PointF[] GetPushedPoints(PointF[] vertices)
		{
			PointF[] pushed = new PointF[vertices.Length];
			for (int i = 0; i < pushed.Length; i++)
			{
				pushed[i].X = vertices[i].X + 1F;
				pushed[i].Y = vertices[i].Y + 1F;
			}
			return pushed;
		}
	}
}

