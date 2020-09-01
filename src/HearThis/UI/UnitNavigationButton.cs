// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2018' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace HearThis.UI
{
	public abstract class UnitNavigationButton : UserControl
	{
		private const int kHorizontalPadding = 4;
		private const int kVerticalPadding = 4;
		const TextFormatFlags kTextPositionFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

		private bool _selected;
		private bool _showProblems;
		private bool _hasProblem;

		// The following property is intended to be overridden in derived classes, but
		// this class is not abstract because it messes up Designer.
		// ReSharper disable once UnassignedGetOnlyAutoProperty
		protected virtual bool DisplayLabels { get; }

		protected virtual float LabelFontSize => 7;

		protected UnitNavigationButton()
		{
			if (Font.SizeInPoints != LabelFontSize)
				Font = new Font(Font.FontFamily, LabelFontSize, FontStyle.Bold);
		}

		public bool Selected
		{
			get => _selected;
			set
			{
				if (_selected != value)
				{
					_selected = value;
					Invalidate();
				}
			}
		}

		public bool ShowProblems
		{
			get => _showProblems;
			set
			{
				_showProblems = value;
				if (value)
				{
					var waitCallback = new WaitCallback(GetProblemsInBackground);
					ThreadPool.QueueUserWorkItem(waitCallback, this);
				}
				else
					InvalidateOnUIThread();
			}
		}

		private static void GetProblemsInBackground(object stateInfo)
		{
			UnitNavigationButton button = (UnitNavigationButton)stateInfo;
			button.UpdateProblemState();
		}

		public void UpdateProblemState()
		{
			var prevValue = _hasProblem;
			_hasProblem = HasRecordingsThatDoNotMatchCurrentScript;
			// If it was false and is still false, no re-draw needed.
			if (prevValue || _hasProblem)
				InvalidateOnUIThread();
		}

		protected abstract bool HasRecordingsThatDoNotMatchCurrentScript { get; }

		protected void InvalidateOnUIThread()
		{
			lock (this)
			{
				if (IsHandleCreated && !IsDisposed)
					Invoke(new Action(Invalidate));
			}
		}

		protected void DrawButton(Graphics g, bool hasTranslatedContent, int percentageRecorded)
		{
			if (Selected)
				g.FillRectangle(AppPallette.HighlightBrush, 0, 0, Width, Height);

			int fillWidth = Width - kHorizontalPadding;
			int fillHeight = Height - kVerticalPadding;
			var r = new Rectangle(kHorizontalPadding / 2, kVerticalPadding / 2, fillWidth, fillHeight);

			var problem = ShowProblems && _hasProblem;
			Brush fillBrush = hasTranslatedContent ? (problem ? AppPallette.DisabledBrush : AppPallette.BlueBrush) :
				AppPallette.EmptyBoxBrush;
			g.FillRectangle(fillBrush, r);

			g.SmoothingMode = SmoothingMode.AntiAlias;

			if (hasTranslatedContent)
			{
				// REVIEW: If there is a problem, should we skip the label?
				if (DisplayLabels)
					DrawLabel(g, r);
				else if (percentageRecorded > 0)
					DrawProgressIndicators(g, r, percentageRecorded, problem);
				if (problem)
				{
					DrawButtonIcon(g, r, AppPallette.CurrentColorScheme == ColorScheme.Normal ?
						Properties.Resources.exclamation_normal_blue : Properties.Resources.exclamation_high_contrast_blue);
				}
			}
		}

		protected void DrawProgressIndicators(Graphics g, Rectangle bounds, int percentageRecorded, bool problem)
		{
			// if it is selected, drawing this line just makes the selection box look irregular.
			// Also, they can readily see what is translated in the selected book or chapter by
			// looking at the more detailed display of its components.
			if (percentageRecorded < 100)
			{
				if (!Selected)
					g.DrawLine(AppPallette.CompleteProgressPen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
			}
			else if (!problem)
			{
				if (percentageRecorded > 100)
				{
					DrawButtonIcon(g, bounds, Properties.Resources.exclamation_normal_highlight);
					return;
				}

				int v1 = bounds.Height / 2 + 3;
				int v2 = bounds.Height / 2 + 7;
				int v3 = bounds.Height / 2 - 2;

				var xLeft = bounds.Left + (bounds.Width - 6) / 2;
				var xBottom = xLeft + 3;
				var xTop = xLeft + 6;

				Pen progressPen = AppPallette.CompleteProgressPen;
				//draw the first stroke of a check mark
				g.DrawLine(progressPen, xLeft, v1, xBottom, v2);
				//complete the checkmark
				g.DrawLine(progressPen, xBottom, v2, xTop, v3);
			}
		}

		private void DrawButtonIcon(Graphics g, Rectangle bounds, Image image)
		{
			if (image.Width < bounds.Width)
			{
				bounds.X += (bounds.Width - image.Width) / 2;
				bounds.Width = image.Width;
			}
			if (image.Height < bounds.Height)
			{
				bounds.Y += (bounds.Height - image.Height) / 2;
				bounds.Height = image.Height;
			}
			g.DrawImage(image, bounds);

			// The following code can be used to try to dynamically re-color an image instead of
			// using a custom-color image for each situation. In my testing, doing this
			// (especially when combined with image scaling) didn't produce optimal results.
			//var imageAttributes = new ImageAttributes();
			//// The following is based on using Paint's eye-dropper tool:
			//var baseColorOfExclamationMark = Color.FromArgb(28, 14, 254);
			//var redAdjust = color.R / 256f - baseColorOfExclamationMark.R / 256f;
			//var greenAdjust = color.G / 256f - baseColorOfExclamationMark.G / 256f; ;
			//var blueAdjust = color.B / 256f - baseColorOfExclamationMark.B / 256f; ;
			//float[][] colorMatrixElements = {
			//	new [] {1f, 0, 0, 0.5f, 0},
			//	new [] {0, 1f, 0, 0.5f, 0},
			//	new [] {0, 0, 1f, 0.5f, 0},
			//	new [] {0f, 0f, 0, 1f, 0},    // alpha scaling factor of 1
			//	new [] {redAdjust, greenAdjust, blueAdjust, 0f, 1}};  // translations
			//var colorMatrix = new ColorMatrix(colorMatrixElements);
			//imageAttributes.SetColorMatrix(
			//	colorMatrix,
			//	ColorMatrixFlag.Default,
			//	ColorAdjustType.Bitmap);
			//g.DrawImage(image, bounds, 0, 0,
			//image.Width,
			//image.Height,
			//GraphicsUnit.Pixel,imageAttributes);

			// This was the original approach, to simply draw a bold exclamation point
			// of the desired size. JohnH though it was too hard to see.
			//using (var font = new Font("Arial", Math.Min(bounds.Height - 2, Math.Max(10.5f, LabelFontSize)), FontStyle.Bold))
			//	DrawButtonText(g, bounds, color, "!", font);
		}

		protected void DrawLabel(Graphics g, Rectangle bounds)
		{
			DrawButtonText(g, bounds, AppPallette.NavigationTextColor);
		}

		protected void DrawButtonText(Graphics g, Rectangle bounds, Color color, string text = null, Font font = null)
		{
			bounds.Offset(0, -1);
			TextRenderer.DrawText(g, text ?? Text, font ?? Font, bounds, color, kTextPositionFlags);
		}
	}
}
