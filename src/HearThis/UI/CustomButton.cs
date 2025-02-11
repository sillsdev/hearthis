// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2022-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HearThis.UI
{
	public class RecordButton : CustomButton
	{
		private bool _waiting;
		private bool _blocked;

		public bool Waiting
		{
			get { return _waiting; }
			set
			{
				_waiting = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Flag indicating that normal recording is not possible (e.g., block is skipped).
		/// Therefore the display is different. This allows the button to remain enabled (so that
		/// tool tips can be displayed and a message can be shown if the user clicks it).
		/// </summary>
		public bool Blocked
		{
			get => _blocked;
			set
			{
				_blocked = value;
				Invalidate();
			}
		}

		public void RecordingWasAborted()
		{
			State = BtnState.Normal;
			Capture = false;
			CapturingMouse = false;
		}

		protected override void Draw(Graphics g)
		{
			int dim = Math.Min(Width, Height) - 2;

			g.SmoothingMode = SmoothingMode.AntiAlias;

			switch (State)
			{
				case BtnState.Normal:
					g.FillEllipse(Blocked? AppPalette.SkippedBrush : AppPalette.BlueBrush, 1, 1, dim, dim);
					if (IsDefault)
						g.DrawEllipse(_highlightPen, 1, 1, dim - 1, dim - 1);
					break;
				case BtnState.Pushed:
					g.FillEllipse(Blocked ? AppPalette.RedBrush :
						(Waiting ? AppPalette.ButtonWaitingBrush : AppPalette.ButtonRecordingBrush), 1, 1, dim, dim);
					break;
				case BtnState.Inactive:
					g.FillEllipse(AppPalette.DisabledBrush, 1, 1, dim, dim);
					break;
				case BtnState.MouseOver:
					g.FillEllipse(Blocked? AppPalette.RedBrush : AppPalette.BlueBrush, 1, 1, dim, dim);
					g.DrawEllipse(AppPalette.ButtonMouseOverPen, 1, 1, dim - 1, dim - 1);
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
				g.FillPolygon(AppPalette.DisabledBrush, pushedVertices);
				g.DrawPolygon(AppPalette.ButtonMouseOverPen, vertices);
			}
			else
				switch (State)
				{
					case BtnState.Normal:
						g.FillPolygon(AppPalette.BlueBrush, vertices);
						if (IsDefault)
							g.DrawPolygon(_highlightPen, vertices);
						break;
					case BtnState.Pushed:
						var pushedVertices = GetPushedPoints(vertices);
						g.FillPolygon(AppPalette.BlueBrush, pushedVertices);
						break;
					case BtnState.Inactive:
						g.FillPolygon(AppPalette.DisabledBrush, vertices);
						break;
					case BtnState.MouseOver:
						g.FillPolygon(AppPalette.BlueBrush, vertices);
						g.DrawPolygon(AppPalette.ButtonMouseOverPen, vertices);
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
			if (!Visible)
				return;

			var rect = ClientRectangle;

			rect.Inflate(-1, -1);

			rect.Y += Padding.Top;
			rect.X += Padding.Left;
			rect.Height -= Padding.Vertical;
			rect.Width -= Padding.Horizontal;

			Rectangle iconRect = new Rectangle(rect.Location, rect.Size);

			var thick = iconRect.Height / 3;
			var stem = 12;
			var vertices = new Point[7];
			vertices[0] = new Point(iconRect.Left, iconRect.Top + iconRect.Height / 2 - thick / 2); // upper left corner of stem
			vertices[1] = new Point(iconRect.Left, iconRect.Top + iconRect.Height / 2 + thick / 2); // lower left corner of stem
			vertices[2] = new Point(iconRect.Left + stem, iconRect.Top + iconRect.Height / 2 + thick / 2); // lower junction of stem and arrow
			vertices[3] = new Point(iconRect.Left + stem, iconRect.Top + iconRect.Height); // lower point of arrow
			vertices[4] = new Point(iconRect.Left + iconRect.Width - 1, iconRect.Top + iconRect.Height / 2); // tip of arrow
			vertices[5] = new Point(iconRect.Left + stem, iconRect.Top); // upper point of arrow
			vertices[6] = new Point(iconRect.Left + stem, iconRect.Top + iconRect.Height / 2 - thick / 2); // upper junction of stem and arrow

			g.SmoothingMode = SmoothingMode.AntiAlias;

			using (var brush = new SolidBrush(ForeColor))
			{
				switch (State)
				{
					case BtnState.Normal:
						g.FillPolygon(brush, vertices);
						if (IsDefault)
							g.DrawPolygon(_highlightPen, vertices);
						break;
					case BtnState.Pushed:
						var pushedVertices = GetPushedPoints(vertices);
						g.FillPolygon(brush, pushedVertices);
						break;
					case BtnState.Inactive:
						g.FillPolygon(AppPalette.DisabledBrush, vertices);
						break;
					case BtnState.MouseOver:
						g.FillPolygon(brush, vertices);
						g.DrawPolygon(AppPalette.ButtonMouseOverPen, vertices);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			Invalidate();
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
		protected bool CapturingMouse { get; set; }
		private BtnState _state = BtnState.Normal;
		protected Pen _highlightPen;
		private bool _isDefault;
		public event EventHandler ButtonStateChanged;

		public CustomButton()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Opaque, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			BackColor = AppPalette.Background;

			_highlightPen = AppPalette.ButtonSuggestedPen;
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
				ButtonStateChanged?.Invoke(this, new EventArgs());
			}
		}

		public Func<bool> CancellableMouseDownCall { get; set; }

		#region Events Methods
		/// <summary>
		/// Mouse Down Event:
		/// set BtnState to Pushed and Capturing mouse to true
		/// </summary>
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
			g.FillRectangle(AppPalette.BackgroundBrush, 0, 0, Width, Height);
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			OnPaintBackground(pevent);
			Draw(pevent.Graphics);
		}

		protected virtual void Draw(Graphics g)
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.FillEllipse(AppPalette.BlueBrush, 0, 0, Width, Width);
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

