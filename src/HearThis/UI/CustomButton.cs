using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HearThis.UI
{
	public class RecordButton : CustomButton
	{
		private bool _waiting;

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
					if (IsDefault)
						g.DrawEllipse(_highlightPen, 1, 1, dim - 1, dim - 1);
					break;
				case BtnState.Pushed:
					g.FillEllipse(Waiting ? AppPallette.ButtonWaitingBrush : AppPallette.ButtonRecordingBrush, 1, 1, dim, dim);
					break;
				case BtnState.Inactive:
					g.FillEllipse(AppPallette.DisabledBrush, 1, 1, dim, dim);
					break;
				case BtnState.MouseOver:
					g.FillEllipse(AppPallette.BlueBrush, 1, 1, dim, dim);
					g.DrawEllipse(AppPallette.ButtonMouseOverPen, 1, 1, dim - 1, dim - 1);
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
			vertices[0] = new Point(0, 0);
			vertices[1] = new Point(0, Height - 1);
			vertices[2] = new Point(Width - 1, Height / 2 - 1);

			g.SmoothingMode = SmoothingMode.AntiAlias;

			if (Playing)
			{
				var pushedVertices = GetPushedPoints(vertices);
				g.FillPolygon(AppPallette.DisabledBrush, pushedVertices);
				g.DrawPolygon(AppPallette.ButtonMouseOverPen, vertices);
			}
			else
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

		public bool Playing
		{
			get { return _playing; }
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
			vertices[0] = new Point(0, Height / 2 - thick / 2); // upper left corner of stem
			vertices[1] = new Point(0, Height / 2 + thick / 2); // lower left corner of stem
			vertices[2] = new Point(stem, Height / 2 + thick / 2); // lower junction of stem and arrow
			vertices[3] = new Point(stem, Height); // lower point of arrow
			vertices[4] = new Point(Width - 1, Height / 2); // tip of arrow
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

		internal static Point[] GetPushedPoints(Point[] vertices)
		{
			Point[] pushed = new Point[vertices.Length];
			for (int i = 0; i < pushed.Length; i++)
			{
				pushed[i].X = vertices[i].X + 1;
				pushed[i].Y = vertices[i].Y + 1;
			}
			return pushed;
		}

		internal static PointF[] GetPushedPoints(PointF[] vertices)
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

