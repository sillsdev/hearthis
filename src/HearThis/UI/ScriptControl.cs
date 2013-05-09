using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using HearThis.Properties;
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
		private Pen _focusPen;
		//private Brush _obfuscatedTextBrush;
		private bool _showContext;
		private bool _lockShowContext;
		private Rectangle _reducedMouseZone;

		public ScriptControl()
		{
			InitializeComponent();
			_reducedMouseZone = new Rectangle(0, 0, 10, 10); // We'll adjust this in OnSizeChanged();
			CurrentData = new PaintData();
			// Review JohnH (JohnT): not worth setting up for localization?
			CurrentData.Script = new ScriptLine(
				"The king’s scribes were summoned at that time, in the third month, which is the month of Sivan, on the twenty-third day. And an edict was written, according to all that Mordecai commanded concerning the Jews, to the satraps and the governors and the officials of the provinces from India to Ethiopia, 127 provinces");
			SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
			ZoomFactor = 1.0f;
			_scriptFocusTextBrush = new SolidBrush(AppPallette.ScriptFocusTextColor);


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

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			_reducedMouseZone = new Rectangle(0, 0, Bounds.Width / 2, Bounds.Height);
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
				DrawScriptWithContext(e.Graphics, CurrentData, r);
			}
			else
			{
				if (_direction == Direction.Forwards)
				{
					int virtualLeft = Animator.GetValue(_animationPoint.X, 0,
														0 - Bounds.Width);
					r = new RectangleF(virtualLeft, 0, Bounds.Width, Bounds.Height * 2);
					DrawScriptWithContext(e.Graphics, _outgoingData, r);

					virtualLeft = Animator.GetValue(_animationPoint.X, Bounds.Width, 0);
					r = new RectangleF(virtualLeft, 0, Bounds.Width*2, Bounds.Height);
					DrawScriptWithContext(e.Graphics, CurrentData, r);
				}
				else
				{
					int virtualLeft = Animator.GetValue(_animationPoint.X, 0,
														0 + Bounds.Width);
					r = new RectangleF(virtualLeft, 0, Bounds.Width, Bounds.Height*2);
					DrawScriptWithContext(e.Graphics, _outgoingData, r);

					virtualLeft = Animator.GetValue(_animationPoint.X, 0 - Bounds.Width, 0);
					r = new RectangleF(virtualLeft,0, Bounds.Width, Bounds.Height*2);
					DrawScriptWithContext(e.Graphics, CurrentData, r);
				}
			}
		}

		/// <summary>
		/// Draw the specified data in the specified rectangle. This is used to draw both the current data
		/// and the previous data (as part of animating the move from one line to another).
		/// </summary>
		private void DrawScriptWithContext(Graphics graphics, PaintData data, RectangleF rectangle)
		{
			const int verticalPadding = 10;
			const int kfocusIndent = 14;

			if (data.Script == null)
				return;
			var top = rectangle.Top;
			var currentRect = rectangle;
			int whiteSpace = 3; // pixels of space between context lines.
			top += DrawOneScriptLine(graphics, data.PreviousLine, currentRect, data.Script.FontSize, true) + whiteSpace;
			top += verticalPadding;
			currentRect = new RectangleF(currentRect.Left + kfocusIndent, top, currentRect.Width, currentRect.Bottom - top);
			var focusTop = top;
			var focusHeight = DrawOneScriptLine(graphics, data.Script, currentRect, data.Script.FontSize, false) + whiteSpace;
			top += focusHeight;
			graphics.DrawLine(_focusPen, rectangle.Left, focusTop, rectangle.Left, focusTop+focusHeight);

			top += verticalPadding;
			currentRect = new RectangleF(currentRect.Left - kfocusIndent, top, currentRect.Width, currentRect.Bottom - top);
			DrawOneScriptLine(graphics, data.NextLine, currentRect, data.Script.FontSize, true);
		}

		readonly char[] clauseSeparators = new char[] {',', ';', ':'};

		/// <summary>
		/// Draw one script line. It may be the main line (context is false)
		/// or a context line (context is true).
		/// </summary>
		private float DrawOneScriptLine(Graphics graphics, ScriptLine script, RectangleF rectangle, int mainFontSize, bool context)
		{
			if (script == null || mainFontSize == 0) // mainFontSize guard enables Shell designer mode
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
			// Also don't let main font get too tiny...for example it comes up 0 in the designer.
			var fontSize = context ? 12 : Math.Max(mainFontSize, 8);
			using (var font = new Font(script.FontName, fontSize*zoom, fontStyle))
			{
				if (Settings.Default.BreakLinesAtClauses && !context)
				{
					// Draw each 'clause' on a line.
					float offset = 0;
					foreach (var chunk in SentenceClauseSplitter.BreakIntoChunks(script.Text,clauseSeparators))
					{
						var text = chunk.Text.Trim();
						var lineRect = new RectangleF(rectangle.X, rectangle.Y + offset, rectangle.Width,
							rectangle.Height - offset);
						graphics.DrawString(text, font, _scriptFocusTextBrush,
							lineRect, alignment);
						offset += graphics.MeasureString(text, font, rectangle.Size).Height;
					}
					return offset;
				}
				else
				{
					// Normal behavior: draw it all as one string.
					graphics.DrawString(script.Text, font, context ? CurrentScriptContextBrush : _scriptFocusTextBrush,
						rectangle, alignment);
					return graphics.MeasureString(script.Text, font, rectangle.Size).Height;
				}
			}
		}

		protected Brush CurrentScriptContextBrush
		{
			get
			{
				if (_showContext)
					return AppPallette.ScriptContextTextBrush;
				return AppPallette.ObfuscatedTextContextBrush;
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

		private bool _mouseIsInReducedZone;

		private void ScriptControl_MouseMove(object sender, MouseEventArgs e)
		{
			var newMouseLocIsInTheZone = _reducedMouseZone.Contains(e.Location);
			if ((!_mouseIsInReducedZone && !newMouseLocIsInTheZone) ||
				(_mouseIsInReducedZone && newMouseLocIsInTheZone))
			{
				return; // do nothing (as quick as possible)
			}
			if (_mouseIsInReducedZone)
			{
				_mouseIsInReducedZone = false;
				_showContext = _lockShowContext;
			}
			else
			{
				_mouseIsInReducedZone = true;
				_showContext = true;
			}
			this.Invalidate();
		}

		private void ScriptControl_MouseLeave(object sender, EventArgs e)
		{
			if (!_mouseIsInReducedZone)
				return;
			_mouseIsInReducedZone = false;
			_showContext = _lockShowContext;
			this.Invalidate();
		}

		private void ScriptControl_Click(object sender, EventArgs e)
		{
			_lockShowContext = !_lockShowContext;

			//this tweak makes it more obvious that your click when context was locked on is going to turn it off
			if (_showContext & !_lockShowContext)
				_showContext = false;
			this.Invalidate();
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
