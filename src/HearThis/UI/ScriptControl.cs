// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
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
			const int kfocusIndent = 0; // 14;
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
			top += prevContextPainter.PaintMaxHeight(maxPrevContextHeight) + whiteSpace;
			top += verticalPadding;
			currentRect = new RectangleF(currentRect.Left + kfocusIndent, top, currentRect.Width, currentRect.Bottom - top);
			mainPainter.BoundsF = currentRect;
			var focusHeight = mainPainter.Paint() + whiteSpace;
			top += focusHeight;

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
						_paintColor = control.RecordingInProgress ? AppPallette.ScriptFocusTextColor : AppPallette.SkippedLineColor;
					}
					else if (control.ShowSkippedBlocks) // currently always false
					{
						if ((control.RecordingInProgress || control.UserPreparingToRecord) && control.BrightenContext)
							_paintColor = ControlPaint.Light(AppPallette.SkippedLineColor, .9f);
						else
							_paintColor = AppPallette.SkippedLineColor;
					}
					else
						_script = null;
				}
				else
				{
					_paintColor = _context ? (control.RecordingInProgress || control.UserPreparingToRecord ? AppPallette.ScriptContextTextColorDuringRecording :
							(control.BrightenContext ? ControlPaint.Light(AppPallette.ScriptContextTextColor, .9f) :
								AppPallette.ScriptContextTextColor)) :
						AppPallette.ScriptFocusTextColor;
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
			/// but between the limits. Return false if we can't find a suitable spot.
			/// </summary>
			/// <param name="trySplit"></param>
			/// <param name="min"></param>
			/// <param name="max"></param>
			/// <returns></returns>
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
				return LayoutString(input, LayoutAction.Measure | LayoutAction.Draw);
			}

			public float Measure()
			{
				if (_script == null)
					return 0;
				return
					LayoutString(_script.Text, LayoutAction.Measure);
			}

			private float Measure(string input)
			{
				return LayoutString(input, LayoutAction.Measure);
			}

			[Flags]
			private enum LayoutAction
			{
				Measure = 1,
				Draw = 2,
			}

			// Measure the height it will take to paint the given input string with the current settings and/or
			// actually draw it.
			private float LayoutString(string input, LayoutAction action)
			{
				if (_script == null || _mainFontSize == 0) // mainFontSize guard enables Shell designer mode
					return 0;

				FontStyle fontStyle = default(FontStyle);
				if (_script.Bold)
					fontStyle = FontStyle.Bold;

				TextFormatFlags alignment = TextFormatFlags.WordBreak;
				if (_script.Centered)
					alignment |= TextFormatFlags.HorizontalCenter;

				// If the language is right-to-left, set the alignment flags for rendering in RTL
				if (_script.RightToLeft)
				{
					alignment |= TextFormatFlags.RightToLeft;
					alignment |= TextFormatFlags.Right;
				}

				const double contextZoom = 0.9;
				const int minMainFontSize = 8;
				const int contextFontSize = 12; // before applying context zoom.

				// Base the size on the main Script line, not the context's own size. Otherwise, a previous or following
				// heading line may dominate what we really want read.
				var zoom = (float) (_zoom * (_context ? contextZoom : 1.0));

				// We don't let the context get big... for fear of a big heading standing out so that it doesn't look *ignorable* anymore.
				// Also don't let main font get too tiny...for example it comes up 0 in the designer.
				var fontSize = _context ? contextFontSize : Math.Max(_mainFontSize, minMainFontSize);
				int labelHeight = 0;
				if (!string.IsNullOrWhiteSpace(_script.Character))
				{
					// Use the context font size, unless perversely the main font is smaller, then use that.
					var labelFontSize = (float)(Math.Min(Math.Max(_mainFontSize, minMainFontSize), contextFontSize));
					var labelZoom = (float)(_zoom * contextZoom); // zoom used for context.
					var characterLabelText = _script.Character.ToUpperInvariant(); // Enhance: do we know a locale we can use?
					using (var font = new Font(_script.FontName, labelFontSize * labelZoom, FontStyle.Regular))
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
						var fontFamily = new FontFamily(_script.FontName);
						path.AddString(characterLabelText, fontFamily, (int)FontStyle.Regular, (Single)font.Size, PointF.Empty, StringFormat.GenericDefault);
						labelHeight = (int)Math.Ceiling(path.GetBounds().Height + 6 * labelZoom);

						var lineRect = new Rectangle((int)BoundsF.X, (int)(BoundsF.Y), (int)BoundsF.Width,
							(int)(BoundsF.Height));
						if ((action & LayoutAction.Draw) == LayoutAction.Draw)
							TextRenderer.DrawText(_graphics, characterLabelText, font, lineRect, AppPallette.ScriptContextTextColor, alignment);
					}
				}
				using (var font = new Font(_script.FontName, fontSize * zoom, fontStyle))
				{
					if ((Settings.Default.BreakLinesAtClauses || _script.ForceHardLineBreakSplitting) && !_context)
					{
						// Draw each 'clause' on a line.
						float offset = labelHeight;
						foreach (var chunk in ClauseSplitter.BreakIntoChunks(input))
						{
							var text = chunk.Text.Trim();
							var lineRect = new Rectangle((int)BoundsF.X, (int)(BoundsF.Y + offset), (int)BoundsF.Width,
								(int)(BoundsF.Height - offset));
							if ((action & LayoutAction.Draw) == LayoutAction.Draw)
								TextRenderer.DrawText(_graphics, text, font, lineRect, _paintColor, alignment);
							if ((action & LayoutAction.Measure) == LayoutAction.Measure)
								offset += TextRenderer.MeasureText(_graphics, text, font, lineRect.Size, alignment).Height;
						}
						return offset;
					}
					else
					{
						// Normal behavior: draw it all as one string.
						Rectangle bounds = new Rectangle((int) BoundsF.X, (int) BoundsF.Y + labelHeight, (int) BoundsF.Width, (int) BoundsF.Height - labelHeight);
						if ((action & LayoutAction.Draw) == LayoutAction.Draw)
							TextRenderer.DrawText(_graphics, input, font, bounds, _paintColor, alignment);

						if ((action & LayoutAction.Measure) == LayoutAction.Measure)
						{
							var size = TextRenderer.MeasureText(_graphics, input, font, bounds.Size, alignment);
							if (size.Width > bounds.Width)
								return bounds.Height; // We don't know how big it really would have been, but it definitely didn't fit.
							return size.Height + labelHeight;
						}
						return 0;
					}
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

		public void SetClauseSeparators(string clauseBreakCharacters)
		{
			// Whenever a new project is set or the project's clause-break settings are changed, this should get called to set the
			// clause break characters stored in the project settings.
			string clauseSeparatorCharacters = (clauseBreakCharacters ?? Settings.Default.ClauseBreakCharacters).Replace(" ", string.Empty);
			List<char> clauseSeparators = new List<char>(clauseSeparatorCharacters.ToCharArray());
			clauseSeparators.Add(ScriptLine.kLineBreak);
			ClauseSplitter = new SentenceClauseSplitter(clauseSeparators.ToArray());
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
