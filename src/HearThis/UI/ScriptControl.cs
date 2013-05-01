using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using HearThis.Script;
using Palaso.UI.WindowsForms.Widgets.Flying;

namespace HearThis.UI
{
	/// <summary>
	/// This class holds the text that the user is supposed to be reading (the main pane on the bottom left of the UI).
	/// </summary>
	public partial class ScriptControl : UserControl
	{
		private Animator _animator;
		private PointF _animationPoint;
		private Direction _direction;
		private static float _zoomFactor;
		private PaintData CurrentData { get; set; }
		private PaintData _outgoingData;
		private Brush _scriptFocusTextBrush;
		private Brush _scriptContextTextBrush;
		private Pen _focusPen;

		public ScriptControl()
		{
			InitializeComponent();
			CurrentData = new PaintData();
			CurrentData.Script = new ScriptLine(
				"The king’s scribes were summoned at that time, in the third month, which is the month of Sivan, on the twenty-third day. And an edict was written, according to all that Mordecai commanded concerning the Jews, to the satraps and the governors and the officials of the provinces from India to Ethiopia, 127 provinces");
			SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
			ZoomFactor = 1.0f;
			_scriptFocusTextBrush = new SolidBrush(AppPallette.ScriptFocusTextColor);
			_scriptContextTextBrush = new SolidBrush(AppPallette.ScriptContextTextColor);
			_focusPen = new Pen(AppPallette.HilightColor,6);
		}


		private void ScriptControl_Load(object sender, EventArgs e)
		{

		}
		protected new bool ReallyDesignMode
		{
			get
			{
				return (base.DesignMode || GetService(typeof(IDesignerHost)) != null) ||
					(LicenseManager.UsageMode == LicenseUsageMode.Designtime);
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (ReallyDesignMode)
				return;

			if (CurrentData.Script == null)
				return;

				RectangleF r;
				if (_animator == null)
				{
					r = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
					DrawData(e.Graphics, CurrentData, r);
				}
				else
				{
					if (_direction == Direction.Forwards)
					{
						int virtualLeft = Animator.GetValue(_animationPoint.X, 0,
														   0 - Bounds.Width);
						r = new RectangleF(virtualLeft, 0, Bounds.Width, Bounds.Height * 2);
						DrawData(e.Graphics, _outgoingData, r);

						virtualLeft = Animator.GetValue(_animationPoint.X, Bounds.Width, 0);
						r = new RectangleF(virtualLeft, 0, Bounds.Width*2, Bounds.Height);
						DrawData(e.Graphics, CurrentData, r);
					}
					else
					{
						int virtualLeft = Animator.GetValue(_animationPoint.X, 0,
														   0 + Bounds.Width);
						r = new RectangleF(virtualLeft, 0, Bounds.Width, Bounds.Height*2);
						DrawData(e.Graphics, _outgoingData, r);

						virtualLeft = Animator.GetValue(_animationPoint.X, 0 - Bounds.Width, 0);
						r = new RectangleF(virtualLeft,0, Bounds.Width, Bounds.Height*2);
						DrawData(e.Graphics, CurrentData, r);
					}
				}

		}

		/// <summary>
		/// Draw the specified data in the specified rectangle. This is used to draw both the current data
		/// and the previous data (as part of animating the move from one line to another).
		/// </summary>
		private void DrawData(Graphics graphics, PaintData data, RectangleF rectangle)
		{
			const int verticalPadding = 10;
			const int kfocusIndent = 14;

			if (data.Script == null)
				return;
			var top = rectangle.Top;
			var currentRect = rectangle;
			int whiteSpace = 3; // pixels of space between context lines.
			top += DrawScript(graphics, data.PreviousLine, currentRect, data.Script.FontSize, true) + whiteSpace;
			top += verticalPadding;
			currentRect = new RectangleF(currentRect.Left + kfocusIndent, top, currentRect.Width, currentRect.Bottom - top);
			var focusTop = top;
			var focusHeight = DrawScript(graphics, data.Script, currentRect, data.Script.FontSize, false) + whiteSpace;
			top += focusHeight;
			graphics.DrawLine(_focusPen, rectangle.Left, focusTop, rectangle.Left, focusTop+focusHeight);

			top += verticalPadding;
			currentRect = new RectangleF(currentRect.Left - kfocusIndent, top, currentRect.Width, currentRect.Bottom - top);
			DrawScript(graphics, data.NextLine, currentRect, data.Script.FontSize, true);
		}

		/// <summary>
		/// Draw one script line. It may be the main line (context is false)
		/// or a context line (context is true).
		/// </summary>
		private float DrawScript(Graphics graphics, ScriptLine script, RectangleF rectangle, int mainFontSize, bool context)
		{
			if (script == null)
				return 0;

			FontStyle fontStyle=default(FontStyle);
			if(script.Bold)
				fontStyle = FontStyle.Bold;
			StringFormat alignment = new StringFormat();
			if(script.Centered)
				alignment.Alignment = StringAlignment.Center;

			// Base the size on the main Script line, not the context's own size. Otherwise, a previous or following
			// heading line may dominate what we really want read.
			var zoom = (float) (ZoomFactor*(context ? 0.9 : 1.0));

			//We don't let the context get big... for fear of a big heading standing out so that it doesn't look *ignorable* anymore.
			var fontSize = context ? 12 : mainFontSize;
			using (var font = new Font(script.FontName, fontSize * zoom, fontStyle))
			{
				graphics.DrawString(script.Text, font, context ? _scriptContextTextBrush : _scriptFocusTextBrush, rectangle, alignment);
				return graphics.MeasureString(script.Text, font, rectangle.Size).Height;
			}
		}

		public float ZoomFactor
		{
			get {
				return _zoomFactor;
			}
			set {
				_zoomFactor = value;
				Invalidate();
			}
		}

		/* protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			RectangleF r;
			if(_animator ==null)
			{
				r =  new RectangleF(e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width, e.ClipRectangle.Height);
				e.Graphics.DrawString(Script, Font, Brushes.Black, r);
			}
			else
			{
				if (_direction == Direction.Down)
				{
					int virtualTop = Animator.GetValue(_animationPoint.X, e.ClipRectangle.Top,
													   e.ClipRectangle.Top - e.ClipRectangle.Height);
					r = new RectangleF(e.ClipRectangle.Left, virtualTop, e.ClipRectangle.Width, e.ClipRectangle.Height*2);
					e.Graphics.DrawString(_outgoingScript, Font, Brushes.Gray, r);

					virtualTop = Animator.GetValue(_animationPoint.X, e.ClipRectangle.Bottom, e.ClipRectangle.Top);
					r = new RectangleF(e.ClipRectangle.Left, virtualTop, e.ClipRectangle.Width, e.ClipRectangle.Height*2);
					e.Graphics.DrawString(Script, Font, Brushes.Black, r);
				}
				else
				{
					int virtualTop = Animator.GetValue(_animationPoint.X, e.ClipRectangle.Top,
													   e.ClipRectangle.Top + e.ClipRectangle.Height);
					r = new RectangleF(e.ClipRectangle.Left, virtualTop, e.ClipRectangle.Width, e.ClipRectangle.Height*2);
					e.Graphics.DrawString(_outgoingScript, Font, Brushes.Gray, r);

					virtualTop = Animator.GetValue(_animationPoint.X, e.ClipRectangle.Top - e.ClipRectangle.Height, e.ClipRectangle.Top);
					r = new RectangleF(e.ClipRectangle.Left, virtualTop, e.ClipRectangle.Width, e.ClipRectangle.Height*2);
					e.Graphics.DrawString(Script, Font, Brushes.Black, r);
				}
			}

		}
*/
		public enum Direction
		{
			Backwards,
			Forwards
		}

		public void GoToScript(Direction direction, ScriptLine previous, ScriptLine script, ScriptLine next)
		{
			_direction = direction;
			_outgoingData = CurrentData;
			_animator = new Animator();
			_animator.Animate += new Animator.AnimateEventDelegate(animator_Animate);
			_animator.Finished += new EventHandler((x, y) => { _animator = null;
																 _outgoingData = null;
			});
			_animator.Duration = 300;
			_animator.Start();
			CurrentData = new PaintData() {Script = script, PreviousLine = previous, NextLine = next};
			Invalidate();
		}

		void animator_Animate(object sender, Animator.AnimatorEventArgs e)
		{
			_animationPoint = e.Point;
			Invalidate();
		}
	}

	/// <summary>
	/// The data needed to call DrawScript. Used to paint both the current data and the animation.
	/// </summary>
	class PaintData
	{
		public ScriptLine Script { get; set; }
		// Preceding context; may be null.
		public ScriptLine PreviousLine { get; set; }
		// Following context; may be null.
		public ScriptLine NextLine { get; set; }
	}

}
