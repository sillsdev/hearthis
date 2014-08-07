#define USETEXTRENDERER

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
		private readonly Brush _scriptFocusTextBrush;
		//private Pen _focusPen;
		//private Brush _obfuscatedTextBrush;
		private bool _brightenContext;
		private bool _lockContextBrightness;
		private Rectangle _brightenContextMouseZone;
		public bool ShowSkippedBlocks { get; set; }

		public ScriptControl()
		{
			InitializeComponent();
			_brightenContextMouseZone = new Rectangle(0, 0, 10, 10); // We'll adjust this in OnSizeChanged();
			CurrentData = new PaintData();
			// Review JohnH (JohnT): not worth setting up for localization?
			CurrentData.Script = new ScriptLine(
				"The king’s scribes were summoned at that time, in the third month, which is the month of Sivan, on the twenty-third day. And an edict was written, according to all that Mordecai commanded concerning the Jews, to the satraps and the governors and the officials of the provinces from India to Ethiopia, 127 provinces");
			SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
			ZoomFactor = 1.0f;
			_scriptFocusTextBrush = new SolidBrush(AppPallette.ScriptFocusTextColor);

			//_focusPen = new Pen(AppPallette.HilightColor,6);
		}

		private void ScriptControl_Load(object sender, EventArgs e)
		{

		}

		protected bool ReallyDesignMode
		{
			get
			{
				return (DesignMode || GetService(typeof(IDesignerHost)) != null) ||
					(LicenseManager.UsageMode == LicenseUsageMode.Designtime);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			_brightenContextMouseZone = new Rectangle(0, 0, Bounds.Width / 2, Bounds.Height);
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
					r = new RectangleF(virtualLeft, 0, Bounds.Width, Bounds.Height);
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
			const int kfocusIndent = 0;// 14;
			const int whiteSpace = 3; // pixels of space between context lines.

			if (data.Script == null)
				return;
			var mainPainter = new ScriptBlockPainter(this, graphics, data.Script, rectangle, data.Script.FontSize, false);
			var mainHeight = mainPainter.Measure();
			var maxPrevContextHeight = rectangle.Height - mainHeight - whiteSpace;
			var prevContextPainter = new ScriptBlockPainter(this, graphics, data.PreviousBlock, rectangle, data.Script.FontSize, true);
			var top = rectangle.Top;
			var currentRect = rectangle;
			top += prevContextPainter.PaintMaxHeight(maxPrevContextHeight) + whiteSpace;
			top += verticalPadding;
			currentRect = new RectangleF(currentRect.Left + kfocusIndent, top, currentRect.Width, currentRect.Bottom - top);
			mainPainter.BoundsF = currentRect;
			var focusHeight = mainPainter.Paint() + whiteSpace;
			top += focusHeight;
			//graphics.DrawLine(_focusPen, rectangle.Left, focusTop, rectangle.Left, focusTop+focusHeight);

			top += verticalPadding;
			currentRect = new RectangleF(currentRect.Left - kfocusIndent, top, currentRect.Width, currentRect.Bottom - top);
			(new ScriptBlockPainter(this, graphics, data.NextBlock, currentRect, data.Script.FontSize, true)).Paint();
		}

		internal class ScriptBlockPainter
		{
			private readonly Graphics _graphics;
			private readonly ScriptLine _script;
			public RectangleF BoundsF { get; set; }
			private readonly int _mainFontSize;
			private readonly bool _context; // true to paint context lines, false for the main text.
			private readonly float _zoom;
			private Brush _brush; // currently only used if USETEXTRENDERER is false
			private readonly Color _paintColor; // currently only used if USETEXTRENDERER is true

			private static SentenceClauseSplitter _clauseSplitter;

			static ScriptBlockPainter()
			{
				SetClauseSeparators();
			}

			public static void SetClauseSeparators()
			{
				string clauseSeparatorCharacters = Settings.Default.ClauseBreakCharacters.Replace(" ", string.Empty);
				List<char> clauseSeparators = new List<char>(clauseSeparatorCharacters.ToCharArray());
				clauseSeparators.Add(ScriptLine.kLineBreak);
				_clauseSplitter = new SentenceClauseSplitter(clauseSeparators.ToArray());
			}

			public ScriptBlockPainter(ScriptControl control, Graphics graphics, ScriptLine script, RectangleF boundsF, int mainFontSize, bool context)
			{
				_context = context;
				_script = script;

				if (_script != null && _script.Skipped)
				{
					if (control.ShowSkippedBlocks)
					{
						_brush = AppPallette.SkippedSegmentBrush;
						_paintColor = AppPallette.SkippedLineColor;
					}
					else
					{
						_script = null;
					}
				}
				else
				{
					if (_context)
					{

						_brush = control.CurrentScriptContextBrush;
						_paintColor = AppPallette.ScriptContextTextColor;
					}
					else
					{
						_brush = control._scriptFocusTextBrush;
						_paintColor = AppPallette.ScriptFocusTextColor;
					}
				}
				_graphics = graphics;
				BoundsF = boundsF;
				_mainFontSize = mainFontSize;
				_zoom = control.ZoomFactor;
			}

			internal ScriptBlockPainter(float zoom, Brush brush, Color paintColor, Graphics graphics, ScriptLine script, RectangleF boundsF, int mainFontSize, bool context)
			{
				_graphics = graphics;
				_script = script;
				BoundsF = boundsF;
				_mainFontSize = mainFontSize;
				_context = context;
				_zoom = zoom;
				_brush = brush;
				_paintColor = paintColor;
			}

			public float PaintMaxHeight(float maxHeight)
			{
				if (maxHeight <= 10 || _script == null || string.IsNullOrEmpty(_script.Text))
					return 0; // assume we can't draw any context in less than 10 pixels.
				return DoMaxHeight(maxHeight, Paint);
			}

			// Perform the paint task (typically, Paint(...) on as much of the text of _script as will fit in the specified height.
			// We use the Func so we can more readily test this function.
			// Return 0 if nothing fits, otherwise whatever the paint() call returns, typically the height actually used to paint the text.
			internal float DoMaxHeight(float maxHeight, Func<string, float> paint)
			{
				if (Measure() < maxHeight)
					return paint(_script.Text); // it all fit.
				// Figure out how much to truncate at the start of the text so that what is left will fit in the available space.
				int badSplit = 0; // unsatisfactory place to split: text from here on is too long to fit in the space available.
				int goodSplit = _script.Text.Length; // possible place to split: we have room to paint everything after this.
				while (badSplit < goodSplit - 1)
				{
					int trySplit = (goodSplit + badSplit)/2;
					// try to split at non-letter
					if (!MoveToBreak(ref trySplit, badSplit, goodSplit))
						break;
					if (Measure(TextAtSplit(trySplit)) <= maxHeight)
					{
						goodSplit = trySplit;
					}
					else
					{
						badSplit = trySplit;
					}
				}
				if (goodSplit >= _script.Text.Length)
					return 0; // can't fit any context.
				return paint(TextAtSplit(goodSplit));
			}

			/// <summary>
			/// Try to move trySplit to a place that is not in the middle of a word,
			/// but between the limits. Return false if we can't find a suitable spot.
			/// </summary>
			/// <param name="trySplit"></param>
			/// <param name="min"></param>
			/// <param name="max"></param>
			/// <returns></returns>
			bool MoveToBreak(ref int trySplit, int min, int max)
			{
				if (IsGoodBreak(trySplit))
					return true;

				int maxDelta = (max - min)/2;  // rounded
				for (int delta = 1; delta < maxDelta; delta++ )
				{
					if (trySplit - delta > min && IsGoodBreak(trySplit - delta))
					{
						trySplit -= delta;
						return true;
					}
					if (trySplit + delta < max && IsGoodBreak(trySplit + delta))
					{
						trySplit += delta;
						return true;
					}
				}
				return false; // can't find any other good break point.
			}

			// Is it good to break at index? Don't pass 0 or length.
			bool IsGoodBreak(int index)
			{
				if (!Char.IsLetterOrDigit(_script.Text[index]))
					return true; // break before non-word-forming is good.
				return !Char.IsLetterOrDigit(_script.Text[index - 1]);
			}

			string TextAtSplit(int split)
			{
				return "..." + _script.Text.Substring(split);
			}

			public float Paint()
			{
				if (_script == null)
					return 0;
				return
					Paint(_script.Text);
			}

			public float Paint(string input)
			{
				return
					MeasureAndDo(input,
						(text, font, lineRect, alignment) =>
							{
#if USETEXTRENDERER
								Rectangle bounds;
								var alignment1 = GetTextRendererBoundsAndAlignment(lineRect, alignment, out bounds);
								TextRenderer.DrawText(_graphics, text, font, bounds, _paintColor, alignment1);
#else
								_graphics.DrawString(text, font, _brush, lineRect, alignment);
#endif
							});
			}

			/// <summary>
			/// Figure out the kind of bounds and alignment that the USETEXTRENDERER approach needs, given the originals.
			/// Note: if we settle on the USETEXTRENDERER approach, we can change the arguments of the drawString argument
			/// to MeasureAndDo so it takes a TextFormatFlags and Rectangle instead of StringFormat and RectangleF.
			/// Then we can just figure them out once where we call drawString.
			/// </summary>
			/// <param name="lineRect"></param>
			/// <param name="alignment"></param>
			/// <param name="bounds"></param>
			/// <returns></returns>
			private static TextFormatFlags GetTextRendererBoundsAndAlignment(RectangleF lineRect, StringFormat alignment,
																			 out Rectangle bounds)
			{
				TextFormatFlags alignment1 = TextFormatFlags.WordBreak;
				if (alignment.Alignment == StringAlignment.Center)
					alignment1 &= TextFormatFlags.HorizontalCenter;
				bounds = new Rectangle((int) lineRect.Left, (int) lineRect.Top, (int) lineRect.Width, (int) lineRect.Height);
				return alignment1;
			}

			public float Measure()
			{
				if (_script == null)
					return 0;
				return
					MeasureAndDo(_script.Text,
						(text, font, lineRect, alignment) => { });
			}

			public float Measure(string input)
			{
				return
					MeasureAndDo(input,
						(text, font, lineRect, alignment) => { });
			}

			// Measure the height it will take to paint our script with all the current settings.
			// Also does the drawString action with the arguments needed to actually draw the text.
			internal float MeasureAndDo(string input, Action<string, Font, RectangleF, StringFormat> drawString)
			{
				if (_script == null || _mainFontSize == 0) // mainFontSize guard enables Shell designer mode
					return 0;

				FontStyle fontStyle = default(FontStyle);
				if (_script.Bold)
					fontStyle = FontStyle.Bold;

				StringFormat alignment = new StringFormat();
				if (_script.Centered)
					alignment.Alignment = StringAlignment.Center;

				// Base the size on the main Script line, not the context's own size. Otherwise, a previous or following
				// heading line may dominate what we really want read.
				var zoom = (float) (_zoom*(_context ? 0.9 : 1.0));

				// We don't let the context get big... for fear of a big heading standing out so that it doesn't look *ignorable* anymore.
				// Also don't let main font get too tiny...for example it comes up 0 in the designer.
				var fontSize = _context ? 12 : Math.Max(_mainFontSize, 8);
				using (var font = new Font(_script.FontName, fontSize*zoom, fontStyle))
				{
					if ((Settings.Default.BreakLinesAtClauses || _script.ForceHardLineBreakSplitting) && !_context)
					{
						// Draw each 'clause' on a line.
						float offset = 0;
						foreach (var chunk in _clauseSplitter.BreakIntoChunks(input))
						{
							var text = chunk.Text.Trim();
							var lineRect = new RectangleF(BoundsF.X, BoundsF.Y + offset, BoundsF.Width,
														  BoundsF.Height - offset);
							drawString(text, font,
									   lineRect, alignment);
#if USETEXTRENDERER
							Rectangle bounds;
							var alignment1 = GetTextRendererBoundsAndAlignment(lineRect, alignment, out bounds);
							offset += TextRenderer.MeasureText(_graphics, text, font, bounds.Size, alignment1).Height;
#else
							offset += _graphics.MeasureString(text, font, BoundsF.Size).Height;
#endif
						}
						return offset;
					}
					else
					{
						// Normal behavior: draw it all as one string.
						drawString(input, font, BoundsF, alignment);
						return _graphics.MeasureString(input, font, BoundsF.Size).Height;
					}
				}
			}
		}

		/// <summary>
		/// Draw one script line. It may be the main line (context is false)
		/// or a context line (context is true).
		/// </summary>
		private float DrawOneScriptLine(Graphics graphics, ScriptLine script, RectangleF boundsF, int mainFontSize, bool context)
		{
			return new ScriptBlockPainter(this, graphics, script, boundsF, mainFontSize, context).Paint();
		}

		protected Brush CurrentScriptContextBrush
		{
			get
			{
				if (_brightenContext)
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
			_animator.Animate += animator_Animate;
			_animator.Finished += (x, y) => {
				_animator = null;
				_outgoingData = null;
			};
			_animator.Duration = 300;
			_animator.Start();
			CurrentData = new PaintData {Script = script, PreviousBlock = previous, NextBlock = next};
			Invalidate();
		}

		void animator_Animate(object sender, Animator.AnimatorEventArgs e)
		{
			_animationPoint = e.Point;
			Invalidate();
		}

		private void ScriptControl_MouseMove(object sender, MouseEventArgs e)
		{
			var newMouseLocIsInTheZone = _lockContextBrightness || _brightenContextMouseZone.Contains(e.Location);
			if (_brightenContext == newMouseLocIsInTheZone)
				return; // do nothing (as quickly as possible)

			var oldBrightenContext = _brightenContext;
			_brightenContext = !_brightenContext || _lockContextBrightness;
			if (oldBrightenContext != _brightenContext)
				Invalidate();
		}

		private void ScriptControl_MouseLeave(object sender, EventArgs e)
		{
			if (!_brightenContext || _lockContextBrightness)
				return;
			_brightenContext = false;
			Invalidate();
		}

		private void ScriptControl_Click(object sender, MouseEventArgs e)
		{
			if (!_brightenContextMouseZone.Contains(e.Location))
				return;
			_lockContextBrightness = !_lockContextBrightness;

			//this tweak makes it more obvious that your click when context was locked on is going to turn it off
			if (_brightenContext & !_lockContextBrightness)
				_brightenContext = false;
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
		public ScriptLine PreviousBlock { get; set; }
		// Following context; may be null.
		public ScriptLine NextBlock { get; set; }
	}
}
