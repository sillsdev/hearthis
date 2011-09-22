using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

		protected override void OnPaint(PaintEventArgs pevent)
		{
			Graphics g = pevent.Graphics;
			int dim = Math.Min(Width, Height);

			g.FillRectangle(AppPallette.BackgroundBrush, 0, 0, Width, Height);
			g.SmoothingMode = SmoothingMode.AntiAlias;

			switch (State)
			{
				case BtnState.Normal:

					g.FillEllipse(AppPallette.BlueBrush, 0, 0, dim, dim);
						 if(IsDefault)
							 g.DrawEllipse(_highlightPen, 0, 0, dim-1, dim-1);
						 break;
				case BtnState.Pushed:
					if (Waiting)
						g.FillEllipse(AppPallette.ButtonWaitingBrush, 0, 0, dim, dim);
					else
					{
						g.FillEllipse(AppPallette.ButtonRecordingBrush, 0, 0, dim, dim);
					}
					break;
				case BtnState.Inactive:
					g.FillEllipse(AppPallette.DisabledBrush, 0, 0, dim, dim);
					break;
				case BtnState.MouseOver:
					 g.FillEllipse(AppPallette.BlueBrush, 0, 0, dim, dim);
						 g.DrawEllipse(AppPallette.ButtonMouseOverPen, 0, 0, dim-1, dim-1);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}



	public class PlayButton : CustomButton
	{
		private bool _playing;

		protected override void OnPaint(PaintEventArgs pevent)
		{
			Graphics g = pevent.Graphics;
			int dim = Math.Min(Width, Height);
			var vertices = new Point[3];
			vertices[0]=new Point(0,0);
			vertices[1] = new Point(0, Height-1);
			vertices[2] = new Point(Width-1, Height/2-1);


			g.FillRectangle(AppPallette.BackgroundBrush, 0, 0, Width, Height);
			g.SmoothingMode = SmoothingMode.AntiAlias;

			if(Playing)
			{
					var pushedVertices = GetPushedPoints(vertices);
					 g.FillPolygon(AppPallette.BlueBrush, pushedVertices);
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
			get {
				return _playing;
			}
			set {
				_playing = value;
				Invalidate();
			}
		}
	}

	public class ArrowButton : CustomButton
	{
		override public bool Enabled
		{
			get { return State != BtnState.Inactive; }
			set
			{
				this.State = value ? BtnState.Normal : BtnState.Inactive;
				this.Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			Graphics g = pevent.Graphics;
			int dim = Math.Min(Width, Height);
			var thick = 11;
			var stem = 12;
			var vertices = new Point[7];
			vertices[0] = new Point(0, Height/2 - thick/2);
			vertices[1] = new Point(0, Height/2 + thick/2);
			vertices[2] = new Point(stem, Height/2 + thick/2);
			vertices[3] = new Point(stem, Height);
			vertices[4] = new Point(Width, Height/2);
			vertices[5] = new Point(stem, 0);
			vertices[6] = new Point(stem, Height / 2 - thick / 2);

			g.FillRectangle(AppPallette.BackgroundBrush, 0, 0, Width, Height);
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
		protected bool _enabled;

		public CustomButton()
			{
				SetStyle(ControlStyles.SupportsTransparentBackColor, true);
				SetStyle(ControlStyles.Opaque, true);
				SetStyle(ControlStyles.ResizeRedraw, true);
				this.BackColor = Color.Transparent;
			_highlightPen = new Pen(AppPallette.HilightColor, 2);

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


		virtual public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				//todo: remove the entangline with button state
				this.State = value ? BtnState.Normal : BtnState.Inactive;
				this.Invalidate();
			}
		}

		 public BtnState State
		{
			get { return _state; }
			set
			{
				if (!_enabled)
				{
					_state = value; // FIX: the enabled to false gets lost if it happens while we're pushing; enabled should be separate
				}
				else
					_state = BtnState.Inactive;
				Invalidate();
			}
		}
		#region Events Methods
		/// <summary>
		/// Mouse Down Event:
		/// set BtnState to Pushed and Capturing mouse to true
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this.Capture = true;
			this.CapturingMouse = true;
			State = BtnState.Pushed;
			this.Invalidate();
		}
		/// <summary>
		/// Mouse Up Event:
		/// Set BtnState to Normal and set CapturingMouse to false
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			State = BtnState.Normal;
			this.Invalidate();
			this.CapturingMouse = false;
			this.Capture = false;
			this.Invalidate();
		}
		/// <summary>
		/// Mouse Leave Event:
		/// Set BtnState to normal if we CapturingMouse = true
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (!CapturingMouse)
			{
				State = BtnState.Normal;
				this.Invalidate();
			}
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
				System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);
				State = BtnState.Normal;
				if ((e.X >= rect.Left) && (e.X <= rect.Right))
				{
					if ((e.Y >= rect.Top) && (e.Y <= rect.Bottom))
					{
						State = BtnState.Pushed;
					}
				}
				this.Capture = true;
				this.Invalidate();
			}
			else
			{
				if(Enabled)
				{
					if (State != BtnState.MouseOver)
					{
						State = BtnState.MouseOver;
						this.Invalidate();
					}
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
			if (this.Enabled)
			{
				this.State = BtnState.Normal;
			}
			this.Invalidate();
		}


		#endregion

		protected override void OnPaint(PaintEventArgs pevent)
			{
				Graphics g = pevent.Graphics;
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.FillEllipse(AppPallette.BlueBrush, 0, 0, Width, Width);
			}


			protected override void OnPaintBackground(PaintEventArgs pevent)
			{
				// don't call the base class
				//base.OnPaintBackground(pevent);
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
					pushed[i].Y = vertices[i].Y+ 1;
				}
				return pushed;
			}

		}

	}
