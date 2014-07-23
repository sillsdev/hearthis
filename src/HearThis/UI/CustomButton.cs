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

	public class SkipButtonPainter : IDisposable
	{
		private readonly Control _button;
		private readonly Brush _fillBrush;
		private readonly Color _lineColor;
		private readonly float _percentageOfAvailableWidthToUseForLine;

		private float _height;
		private float _width;
		private float _triangleHeight;
		private float _right;
		private float _top;
		private float _bottom;
		private float _dyMiddleOfCurve;
		private int _btnHeight;
		private float _extraRightPaddingForLine;

		public float MiddleOfCurve { get { return _dyMiddleOfCurve; } }
		public float Right { get { return _right; } }
		public float Width { get { return _width; } }
		public float Height { get { return _height; } }
		public float Bottom { get { return _bottom; } }

		public SkipButtonPainter(Control button, Brush fillBrush, Color lineColor, float percentageOfAvailableWidthToUseForLine)
		{
			_button = button;
			_fillBrush = fillBrush;
			_lineColor = lineColor;
			_percentageOfAvailableWidthToUseForLine = percentageOfAvailableWidthToUseForLine;

			_button.Resize += HandleButtonResize;
		}

		private void HandleButtonResize(object sender, EventArgs args)
		{
			var padding = _button.Padding;
			_btnHeight = _button.Height - padding.Bottom - padding.Top;
			_height = _btnHeight + 2 * AppPallette.ButtonMouseOverPen.Width; // REVIEW: Should we be subtracting?
			_width = _button.Width - padding.Left - padding.Right - 2 * AppPallette.ButtonMouseOverPen.Width;
			_triangleHeight = _height / 7F;
			_right = _button.Width - padding.Right - AppPallette.ButtonMouseOverPen.Width;
			_top = padding.Top + AppPallette.ButtonMouseOverPen.Width;
			_bottom = _button.Height - padding.Bottom - AppPallette.ButtonMouseOverPen.Width;
			_dyMiddleOfCurve = _top + _bottom / 2F;
			_extraRightPaddingForLine = _width * (100 - _percentageOfAvailableWidthToUseForLine) / 100;
		}

		public void Draw(Graphics g, float lineThickness, BtnState state)
		{
			float triangleWidth = lineThickness * 3;
			float lineRight = _right - triangleWidth / 2F - _extraRightPaddingForLine;
			float left = _button.Padding.Left + lineThickness / 2F + AppPallette.ButtonMouseOverPen.Width;

			PointF startPt = new PointF(lineRight, _top);
			PointF midPt = new PointF(left, _dyMiddleOfCurve);
			PointF endPt = new PointF(lineRight, _bottom - _triangleHeight / 2F);
			Color lineColor;
			Brush fillBrush;
			if (state == BtnState.Inactive)
			{
				lineColor = AppPallette.EmptyBoxColor;
				fillBrush = AppPallette.DisabledBrush;
			}
			else
			{
				lineColor = _lineColor;
				fillBrush = _fillBrush;
			}

			g.SmoothingMode = SmoothingMode.AntiAlias;

			using (var pen = new Pen(lineColor, lineThickness))
			{
				// Draw the curved line.
				PointF control1 = new PointF(lineRight, _top + _btnHeight / 3F);
				PointF control2 = new PointF(left, _dyMiddleOfCurve - _btnHeight / 4F);
				PointF control3 = new PointF(left, _dyMiddleOfCurve + _btnHeight / 4F);
				PointF control4 = new PointF(lineRight, endPt.Y - _btnHeight / 3F);
				PointF[] bezierPoints =
				{
					startPt, control1, control2, midPt,
					control3, control4, endPt
				};

				if (state == BtnState.Pushed)
					bezierPoints = CustomButton.GetPushedPoints(bezierPoints);

				g.DrawBeziers(pen, bezierPoints);

				if (state == BtnState.MouseOver)
				{
					float adj = (lineThickness + AppPallette.ButtonMouseOverPen.Width) / 2F;
					g.DrawLine(AppPallette.ButtonMouseOverPen, new PointF(startPt.X - adj, _top),
						new PointF(startPt.X + adj, _top));

					for (int index = 0; index < bezierPoints.Length; index++)
					{
						PointF pt = bezierPoints[index];
						bezierPoints[index] = new PointF(pt.X - lineThickness / 2, pt.Y);
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
				vertices[0] = new PointF(lineRight - triangleWidth / 2F, _bottom - _triangleHeight); // left corner
				vertices[1] = new PointF(_right - _extraRightPaddingForLine, _bottom - _triangleHeight); // right corner
				vertices[2] = new PointF(lineRight, _bottom); // point

				if (state == BtnState.Pushed)
					vertices = CustomButton.GetPushedPoints(vertices);

				g.FillPolygon(fillBrush, vertices);
				if (state == BtnState.MouseOver)
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
		}

		public void Dispose()
		{
			_button.Resize -= HandleButtonResize;
		}
	}

	public class SkipButton : CustomButton
	{
		private SkipButtonPainter _painter;

		public SkipButton()
		{
			_painter = new SkipButtonPainter(this, AppPallette.BlueBrush, AppPallette.Blue, 100);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				_painter.Dispose();

			base.Dispose(disposing);
		}

		private Color LineColor
		{
			get
			{
				switch (State)
				{
					case BtnState.Pushed:
						return AppPallette.Red;
					case BtnState.Inactive:
						return AppPallette.EmptyBoxColor;
					default:
						return AppPallette.Blue;
				}
			}
		}

		protected override void Draw(Graphics g)
		{
			float lineThickness = Math.Min(6F, _painter.Width / 4F);
			if (State == BtnState.MouseOver)
				lineThickness += 2F;

			_painter.Draw(g, lineThickness, State);

			lineThickness = lineThickness / 2;
			const float thinLineWidth = 1F;
			if (State == BtnState.MouseOver)
				lineThickness = lineThickness - thinLineWidth;


			using (var pen = new Pen(LineColor, lineThickness))
			{
				// Draw the text lines
				float dyTopLine = _painter.MiddleOfCurve - 1 - lineThickness / 2F;
				float leftEdge = Padding.Left + _painter.Width / 2F;
				if (State == BtnState.MouseOver)
					leftEdge += AppPallette.ButtonMouseOverPen.Width;
				g.DrawLine(pen, new PointF(leftEdge, dyTopLine), new PointF(_painter.Right, dyTopLine));
				float dyBottomLine = _painter.MiddleOfCurve + 1 + lineThickness / 2F;
				g.DrawLine(pen, new PointF(leftEdge, dyBottomLine), new PointF(_painter.Right - lineThickness, dyBottomLine));

				if (State == BtnState.MouseOver)
				{
					using (var thinMouseOverPen = new Pen(AppPallette.ButtonMouseOverPen.Color, thinLineWidth))
					{
						// Highlight the text lines
						g.SmoothingMode = SmoothingMode.None;
						dyTopLine -= lineThickness;
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyTopLine), new PointF(_painter.Right, dyTopLine));
						dyTopLine += lineThickness * 2 - 2 * thinLineWidth;
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyTopLine), new PointF(_painter.Right, dyTopLine));
						dyBottomLine -= (lineThickness - 2 * thinLineWidth);
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyBottomLine), new PointF(_painter.Right - lineThickness, dyBottomLine));
						dyBottomLine += lineThickness * 2 - 2 * thinLineWidth;
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyBottomLine), new PointF(_painter.Right - lineThickness, dyBottomLine));
					}
				}
			}
		}
	}

	public class ShowSkippedBlocksButton : CheckBox
	{
		private Brush _brush;
		private SkipButtonPainter _painter;

		public ShowSkippedBlocksButton()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Opaque, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			_brush = new SolidBrush(Color.DimGray);
			_painter = new SkipButtonPainter(this, _brush, Color.DimGray, 50);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_painter.Dispose();
				_brush.Dispose();
			}

			base.Dispose(disposing);
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			base.OnPaintBackground(pevent);

			Graphics g = pevent.Graphics;

			if (ClientRectangle.Contains(PointToClient(MousePosition)))
			{
				using (Brush brush = new SolidBrush(AppPallette.MouseOverButtonBackColor))
				{
					g.FillRectangle(brush, ClientRectangle);
				}
			}

			float lineThickness = Math.Min(2F, _painter.Width / 4F);

			_painter.Draw(g, lineThickness, BtnState.Normal);

			using (var pen = new Pen(Color.DimGray, lineThickness))
			{
				// Draw the eye
				float leftEdge = Padding.Left + _painter.Width / 3.5F;
				float radiusOfEye = Math.Min(_painter.Width, _painter.Height) / 3F;
				const int degreesToOmit = 30;
				const float radians = (float)(degreesToOmit * 2 * Math.PI / 180);
				float dyAdj = radiusOfEye * (float)Math.Cos(radians);
				//g.DrawLine(pen, 0, dyMiddleOfCurve, Right, dyMiddleOfCurve);
				g.DrawArc(pen, leftEdge, _painter.MiddleOfCurve + dyAdj - radiusOfEye - lineThickness, 2 * radiusOfEye, 2 * radiusOfEye, 180 + degreesToOmit, 90 + degreesToOmit);
				g.DrawArc(pen, leftEdge, _painter.MiddleOfCurve - dyAdj - radiusOfEye - lineThickness, 2 * radiusOfEye, 2 * radiusOfEye, degreesToOmit, 90 + degreesToOmit);
				// Draw the eyball
				float radiusOfEyeball = radiusOfEye / 2F;
				//float dxAdj = radiusOfEye * (float)Math.Sin(radians);
				g.FillEllipse(_brush, leftEdge + radiusOfEye - radiusOfEyeball / 2,
					_painter.MiddleOfCurve - radiusOfEyeball / 2 - dyAdj / 2, radiusOfEyeball, radiusOfEyeball);

				if (!Checked)
					g.DrawLine(pen, new PointF(0, 0), new PointF(_painter.Right, _painter.Bottom));
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

		static internal Point[] GetPushedPoints(Point[] vertices)
		{
			Point[] pushed = new Point[vertices.Length];
			for (int i = 0; i < pushed.Length; i++)
			{
				pushed[i].X = vertices[i].X + 1;
				pushed[i].Y = vertices[i].Y + 1;
			}
			return pushed;
		}

		static internal PointF[] GetPushedPoints(PointF[] vertices)
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

