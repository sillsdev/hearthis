// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International. All Rights Reserved.
// <copyright from='2011' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Script;
using SIL.ObjectModel;
using SIL.Windows.Forms.Widgets.Flying;

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
		private bool _brightenContext;
		private bool _lockContextBrightness;
		private Rectangle _brightenContextMouseZone;
		private bool _recordingInProgress;
		private bool _userPreparingToRecord;
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
			ZoomFactor = 1.0f; // Typically overridden by later initialization in RecordingToolControl constructor
		}

		private void ScriptControl_Load(object sender, EventArgs e)
		{

		}

		protected bool ReallyDesignMode
		{
			get
			{
				return (DesignMode || GetService(typeof (IDesignerHost)) != null) ||
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

			if (CurrentData.Script == null || ScriptLine.SkippedStyleInfoProvider == null)
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
					r = new RectangleF(virtualLeft, 0, Bounds.Width * 2, Bounds.Height);
					DrawScriptWithContext(e.Graphics, CurrentData, r);
				}
				else
				{
					int virtualLeft = Animator.GetValue(_animationPoint.X, 0,
						0 + Bounds.Width);
					r = new RectangleF(virtualLeft, 0, Bounds.Width, Bounds.Height * 2);
					DrawScriptWithContext(e.Graphics, _outgoingData, r);

					virtualLeft = Animator.GetValue(_animationPoint.X, 0 - Bounds.Width, 0);
					r = new RectangleF(virtualLeft, 0, Bounds.Width, Bounds.Height * 2);
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
			const int kFocusIndent = 0; // 14;
			const int whiteSpace = 3; // pixels of space between context lines.

			if (data.Script == null)
				return;
			var mainPainter = new ScriptBlockPainter(this, graphics, data.Script, rectangle, data.Script.FontSize, false);
			var mainHeight = mainPainter.Measure();
			var maxPrevContextHeight = rectangle.Height - mainHeight - whiteSpace;
			var prevContextPainter = new ScriptBlockPainter(this, graphics, data.PreviousBlock, rectangle, data.Script.FontSize,
				true);
			var top = rectangle.Top;
			var currentRect = rectangle;
			var heightOfPrecedingContext = prevContextPainter.PaintMaxHeight(maxPrevContextHeight);
			if (heightOfPrecedingContext > 0) // No need to add padding if there was no context or we couldn't fit anything.
				top += heightOfPrecedingContext + whiteSpace + verticalPadding;
			currentRect = new RectangleF(currentRect.Left + kFocusIndent, top, currentRect.Width, currentRect.Bottom - top);
			mainPainter.BoundsF = currentRect;
			var mainActualPaintedHeight = mainPainter.Paint();
			top += mainActualPaintedHeight + whiteSpace + verticalPadding;
			currentRect = new RectangleF(currentRect.Left - kFocusIndent, top, currentRect.Width, currentRect.Bottom - top);
			new ScriptBlockPainter(this, graphics, data.NextBlock, currentRect, data.Script.FontSize, true).Paint();
		}

		internal class ScriptBlockPainter
		{
			private const int minMainFontSize = 8;
			private const int contextFontSize = 12; // before applying context zoom.

			private readonly Graphics _graphics;
			private readonly ScriptLine _script;
			public RectangleF BoundsF { get; set; }
			private readonly int _mainFontSize;
			private readonly bool _context; // true to paint context lines, false for the main text.
			private readonly float _zoom;
			private readonly Color _paintColor;

			internal static SentenceClauseSplitter ClauseSplitter;

			public ScriptBlockPainter(ScriptControl control, Graphics graphics, ScriptLine script, RectangleF boundsF,
				int mainFontSize, bool context)
			{
				ClauseSplitter = control.ClauseSplitter;
				_context = context;
				_script = script;

				if (_script != null && _script.Skipped)
				{
					if (!context)
					{
						_paintColor = control.RecordingInProgress ? AppPalette.ScriptFocusTextColor : AppPalette.SkippedLineColor;
					}
					else if (control.ShowSkippedBlocks) // currently always false
					{
						if ((control.RecordingInProgress || control.UserPreparingToRecord) && control.BrightenContext)
							_paintColor = ControlPaint.Light(AppPalette.SkippedLineColor, .9f);
						else
							_paintColor = AppPalette.SkippedLineColor;
					}
					else
						_script = null;
				}
				else
				{
					_paintColor = _context ? (control.RecordingInProgress || control.UserPreparingToRecord ? AppPalette.ScriptContextTextColorDuringRecording :
							(control.BrightenContext ? ControlPaint.Light(AppPalette.ScriptContextTextColor, .9f) :
								AppPalette.ScriptContextTextColor)) :
						AppPalette.ScriptFocusTextColor;
				}

				_graphics = graphics;
				BoundsF = boundsF;
				_mainFontSize = mainFontSize;
				_zoom = control.ZoomFactor;
			}

			internal ScriptBlockPainter(float zoom, Color paintColor, Graphics graphics, ScriptLine script,
				RectangleF boundsF, int mainFontSize, bool context)
			{
				_graphics = graphics;
				_script = script;
				BoundsF = boundsF;
				_mainFontSize = mainFontSize;
				_context = context;
				_zoom = zoom;
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
					int trySplit = (goodSplit + badSplit) / 2;
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
			/// but between the limits.
			/// </summary>
			/// <returns><c>true</c> if we find a suitable spot to split;
			/// <c>false</c> otherwise.</returns>
			private bool MoveToBreak(ref int trySplit, int min, int max)
			{
				if (IsGoodBreak(trySplit))
					return true;

				int maxDelta = (max - min) / 2; // rounded
				for (int delta = 1; delta < maxDelta; delta++)
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
			private bool IsGoodBreak(int index)
			{
				if (!Char.IsLetterOrDigit(_script.Text[index]))
					return true; // break before non-word-forming is good.
				return !Char.IsLetterOrDigit(_script.Text[index - 1]);
			}

			private string TextAtSplit(int split)
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
				return LayoutString(input, false);
			}

			public float Measure()
			{
				if (_script == null)
					return 0;
				return
					LayoutString(_script.Text, true);
			}

			private float Measure(string input)
			{
				return LayoutString(input, true);
			}

			// Measure the height it will take to paint the given input string with the current settings and/or
			// actually draw it.
			private float LayoutString(string input, bool measureOnly)
			{
				if (_script == null || _mainFontSize == 0) // mainFontSize guard enables Shell designer mode
					return 0;

				TextFormatFlags alignment = TextFormatFlags.WordBreak;
				if (_script.Centered)
					alignment |= TextFormatFlags.HorizontalCenter;

				// If the language is right-to-left, set the alignment flags for rendering in RTL
				if (_script.RightToLeft)
				{
					alignment |= TextFormatFlags.RightToLeft;
					alignment |= TextFormatFlags.Right;
				}

				double contextZoom = 0.9;

				// Base the size on the main Script line, not the context's own size. Otherwise, a previous or following
				// heading line may dominate what we really want read.
				var defaultZoom = (float)(_zoom * (_context ? contextZoom : 1.0));
				var zoom = defaultZoom;

				var label = _script.Character;
				if (!string.IsNullOrWhiteSpace(label))
					label = label.ToUpperInvariant(); // Enhance: do we know a locale we can use?

				float height = label == null ? 0f :
					// For measurement purposes, we have to treat the label specially (as explained in GetLabelHeight)
					GetLabelHeight(contextZoom, label);

				void MeasureText(string text, Font font, Rectangle lineRect, Color _)
				{
					var size = TextRenderer.MeasureText(_graphics, text, font, lineRect.Size, alignment);
					if (size.Width > lineRect.Width)
						height += lineRect.Height; // We don't know how big it really would have been, but it definitely didn't fit.
					height += size.Height;
				}

				Color labelColor = _context ? _paintColor : AppPalette.ScriptContextTextColor;
				Color paintColor = _paintColor;

				// First get the natural measurement.
				PerformStringLayout(zoom, null, input, labelColor, paintColor, contextZoom,
					alignment, MeasureText);

				// If not drawing, just return the natural height. If actually drawing,
				// we have to try to fit it in the space available.
				if (!measureOnly)
				{
					bool suppressClauseBreaking = false;

					// If drawing and the result is greater than the available height, try knocking
					// the font size down a couple times. If still too big, suppress label (first
					// with normal font and then with reduced font sizes). If still too big and
					// breaking lines into clauses, turn that option off and try again (first with
					// normal font and then with reduced font sizes). If it still doesn't fit,
					// then what?
					// REVIEW: For now, I'm drawing it in red, but I don't know if it will be
					// clear to the user why it is red. Might be better to throw some kind of
					// catchable exception or set a flag to know that we need to alert the user
					// with an actual warning message.
					while (height > BoundsF.Height)
					{
						contextZoom -= 0.1;
						zoom -= 0.1f;

						if ((_context && contextZoom < 0.4) || (!_context && zoom < 0.75))
						{
							if (!string.IsNullOrEmpty(label))
								label = null;
							else if (!suppressClauseBreaking)
								suppressClauseBreaking = true;
							else
							{
								// We don't want it to turn red if the only thing cut off is some
								// bottom padding (or maybe a tiny bit of a descender).
								const int fudgeFactor = 13;
								if (!_context && height > BoundsF.Height + fudgeFactor)
									paintColor = Color.Red;
								break;
							}

							contextZoom = 0.9;
							zoom = defaultZoom;
						}
						height = label == null ? 0f : GetLabelHeight(contextZoom, label);

						PerformStringLayout(zoom, null, input, labelColor, paintColor,
							contextZoom, alignment, MeasureText, suppressClauseBreaking);
					}

					PerformStringLayout(zoom, label, input, labelColor, paintColor, contextZoom,
						alignment, (text, font, color, lineRect) =>
						{ TextRenderer.DrawText(_graphics, text, font, color, lineRect, alignment); },
						suppressClauseBreaking);
				}

				return height;
			}

			private void PerformStringLayout(float zoom, string label, string input,
				Color labelColor, Color textColor, double contextZoom, TextFormatFlags formatFlags,
				Action<string, Font, Rectangle, Color> action, bool suppressClauseBreaking = false)
			{
				// We don't let the context get big... for fear of a big heading standing out so that it doesn't look *ignorable* anymore.
				// Also don't let main font get too tiny...for example it comes up 0 in the designer.
				var fontSize = _context ? contextFontSize : Math.Max(_mainFontSize, minMainFontSize);

				FontStyle fontStyle = default;
				if (_script.Bold)
					fontStyle = FontStyle.Bold;

				int labelHeight = 0;

				if (!string.IsNullOrWhiteSpace(label))
				{
					labelHeight = GetLabelHeight(contextZoom, label);
					using (var font = GetLabelFont(contextZoom, out _))
					{
						var lineRect = new Rectangle((int)BoundsF.X, (int)BoundsF.Y, (int)BoundsF.Width,
							(int)BoundsF.Height);
						action(label, font, lineRect, labelColor);
					}
				}

				using (var font = new Font(_script.FontName, fontSize * zoom, fontStyle))
				{
					if (!suppressClauseBreaking && (Settings.Default.BreakLinesAtClauses || _script.ForceHardLineBreakSplitting) && !_context)
					{
						// Draw each 'clause' on a line.
						float offset = labelHeight;
						foreach (var chunk in ClauseSplitter.BreakIntoChunks(input))
						{
							var text = chunk.Text.Trim();
							var lineRect = new Rectangle((int)BoundsF.X, (int)(BoundsF.Y + offset), (int)BoundsF.Width,
								(int)(BoundsF.Height - offset));
							action(text, font, lineRect, textColor);
							offset += TextRenderer.MeasureText(_graphics, text, font, lineRect.Size, formatFlags).Height;
						}
					}
					else
					{
						// Normal behavior: draw it all as one string.
						Rectangle bounds = new Rectangle((int)BoundsF.X, (int)BoundsF.Y + labelHeight, (int)BoundsF.Width, (int)BoundsF.Height - labelHeight);
						action(input, font, bounds, textColor);
					}
				}
			}

			private Font GetLabelFont(double contextZoom, out float labelZoom)
			{
				// Use the context font size, unless perversely the main font is smaller, then use that.
				var labelFontSize = (float)Math.Min(Math.Max(_mainFontSize, minMainFontSize), contextFontSize);
				labelZoom = (float)(_zoom * contextZoom); // zoom used for context.
				return new Font(_script.FontName, labelFontSize * labelZoom, FontStyle.Regular);
			}

			private int GetLabelHeight(double contextZoom, string label)
			{
				using (var font = GetLabelFont(contextZoom, out var labelZoom))
				{
					// This is the obvious thing to do, but especially with all-caps, it seems to leave too much gap.
					// Also, I am inclined to truncate the label to one line, even if it is somehow longer than that.
					//labelHeight = TextRenderer.MeasureText(_graphics, characterLabelText, font, new Size((int)BoundsF.Width, (int)BoundsF.Height), alignment).Height;

					// According to https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/how-to-obtain-font-metrics,
					// this is the way to get the ascent/descent of a font. It's counterintuitive that the EmHeight would be different from the ascent,
					// but it's described as "the height of the em square" which may well mean something like a square as high as M is wide.
					// For Roman fonts without descenders, we'd really like to leave out the Descent, but that can cause collisions (e.g,
					// upper-case Q sometimes has a descender). Including the descent height still gives a height less than the standard line spacing,
					// but SHOULD always prevent overlap. But fonts like Charis have a LOT of ascent and descent that most characters don't use.
					// This didn't work much better than MeasureText
					//var fontFamily = new FontFamily(_script.FontName);
					//labelHeight = (int)(font.Size * (fontFamily.GetCellAscent(FontStyle.Regular) + fontFamily.GetCellDescent(FontStyle.Regular))
					//	/ fontFamily.GetEmHeight(FontStyle.Regular) + 5 * labelZoom);

					// This approach really measures the actual label we will draw. By experiment, adding 6*labelZoom prevents overlap
					// for a problem Arabic text (the Arabic diacritics on the next line must paint CONSIDERABLY above what is supposed
					// to be the top of the line). It gives a nice small space in ordinary Roman text.
					var path = new GraphicsPath();
					// HT-230: The following throws an ArgumentException if the requested font
					// is not installed. The above constructor for Font will already have
					// discovered the problem and done its best to fall back to some other
					// font. It might look wrong, but at least we'll avoid crashing in layout
					// code. Ideally, we should probably look at the font right away when
					// loading the script and let the user know then they they might need to
					// install the font in order to get things to look right. But for 99.9%
					// of users, they will already have the needed font.
					//var fontFamily = new FontFamily(_script.FontName);
					path.AddString(label, font.FontFamily, (int)FontStyle.Regular, font.Size, PointF.Empty, StringFormat.GenericDefault);
					return (int)Math.Ceiling(path.GetBounds().Height + 6 * labelZoom);
				}
			}
		}



		public float ZoomFactor
		{
			get => _zoomFactor;
			set
			{
				if (_zoomFactor.Equals(value))
					return;
				_zoomFactor = value;
				Invalidate();
			}
		}

		public void SetFont(string name)
		{
			Font = new Font(name, 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
		}

		internal SentenceClauseSplitter ClauseSplitter { get; private set;  }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RecordingInProgress
		{
			get => _recordingInProgress;
			set
			{
				if (_recordingInProgress != value)
				{
					_recordingInProgress = value;
					Invalidate();
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UserPreparingToRecord
		{
			get => _userPreparingToRecord;
			set
			{
				Debug.Assert(!RecordingInProgress);
				if (_userPreparingToRecord != value)
				{
					_userPreparingToRecord = value;
					Invalidate();
				}
			}
		}

		internal bool BrightenContext => _brightenContext;

		public void SetClauseSeparators(IReadOnlySet<char> clauseBreakCharacters)
		{
			// Whenever a new project is set or the project's clause-break settings are changed, this should get called to set the
			// clause break characters stored in the project settings.
			var setWithLineBreakChar = new HashSet<char>(clauseBreakCharacters);
			setWithLineBreakChar.Add(ScriptLine.kLineBreak);
			ClauseSplitter = new SentenceClauseSplitter(new ReadOnlySet<char>(setWithLineBreakChar));
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
			_animator.Finished += (x, y) =>
			{
				_animator = null;
				_outgoingData = null;
			};
			_animator.Duration = 300;
			_animator.Start();
			CurrentData = new PaintData {Script = script, PreviousBlock = previous, NextBlock = next};
			Invalidate();
		}

		private void animator_Animate(object sender, Animator.AnimatorEventArgs e)
		{
			_animationPoint = e.Point;
			Invalidate();
		}

		private void ScriptControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (_recordingInProgress)
				return;
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
			if (!_brightenContext || _lockContextBrightness || _recordingInProgress)
				return;
			_brightenContext = false;
			Invalidate();
		}

		private void ScriptControl_Click(object sender, MouseEventArgs e)
		{
			if (_recordingInProgress || !_brightenContextMouseZone.Contains(e.Location))
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
	internal class PaintData
	{
		public ScriptLine Script { get; set; }
		// Preceding context; may be null.
		public ScriptLine PreviousBlock { get; set; }
		// Following context; may be null.
		public ScriptLine NextBlock { get; set; }
	}
}
