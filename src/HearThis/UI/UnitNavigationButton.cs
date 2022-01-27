// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2018' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
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
	public class UnitNavigationButton : UserControl
	{
		private const int kHorizontalPadding = 4;
		private const int kVerticalPadding = 4;
		const TextFormatFlags kTextPositionFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

		private bool _selected;

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

		protected void DrawButton(Graphics g, bool hasTranslatedContent, int percentageRecorded, bool loadErrorOccurred = false)
		{
			if (Selected)
				g.FillRectangle(AppPalette.HighlightBrush, 0, 0, Width, Height);

			int fillWidth = Width - kHorizontalPadding;
			int fillHeight = Height - kVerticalPadding;
			var r = new Rectangle(kHorizontalPadding / 2, kVerticalPadding / 2, fillWidth, fillHeight);

			Brush fillBrush = hasTranslatedContent ? AppPalette.BlueBrush : AppPalette.EmptyBoxBrush;
			g.FillRectangle(fillBrush, r);

			g.SmoothingMode = SmoothingMode.AntiAlias;

			if (hasTranslatedContent)
			{
				if (DisplayLabels)
					DrawLabel(g, r);
				else if (percentageRecorded > 0)
					DrawProgressIndicators(g, r, percentageRecorded);
			}
			else if (loadErrorOccurred)
			{
				DrawErrorX(g, r);
			}
		}

		private void DrawProgressIndicators(Graphics g, Rectangle bounds, int percentageRecorded)
		{
			// If it is selected, drawing this line just makes the selection box look irregular.
			// Also, they can readily see what is translated in the selected book or chapter by
			// looking at the more detailed display of its components.
			if (percentageRecorded < 100)
			{
				if (!Selected)
					g.DrawLine(AppPalette.CompleteProgressPen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
			}
			else
			{
				int v1 = bounds.Height / 2 + 3;
				int v2 = bounds.Height / 2 + 7;
				int v3 = bounds.Height / 2 - 2;

				if (percentageRecorded > 100)
				{
					bounds.Offset(0, -1);
					try
					{
						using (var font = new Font("Arial", 9, FontStyle.Bold))
							DrawWarningIndicator(g, font, bounds);
						return;
					}
					catch (Exception)
					{
						// Arial is probably missing. Just use the default (label) font. Beats crashing.
						DrawWarningIndicator(g, Font, bounds);
					}
				}

				var xLeft = bounds.Left + (bounds.Width - 6) / 2;
				var xBottom = xLeft + 3;
				var xTop = xLeft + 6;

				Pen progressPen = AppPalette.CompleteProgressPen;
				//draw the first stroke of a check mark
				g.DrawLine(progressPen, xLeft, v1, xBottom, v2);
				//complete the check mark
				g.DrawLine(progressPen, xBottom, v2, xTop, v3);
			}
		}

		private void DrawWarningIndicator(Graphics g, Font font, Rectangle bounds)
		{
			TextRenderer.DrawText(g, "!", font, bounds, AppPalette.HilightColor, kTextPositionFlags);
		}

		private void DrawLabel(Graphics g, Rectangle bounds)
		{

			bounds.Offset(0, -1);
			TextRenderer.DrawText(g, Text, Font, bounds, AppPalette.NavigationTextColor, kTextPositionFlags);
		}

		private void DrawErrorX(Graphics g, Rectangle bounds)
		{
			using (var pen = new Pen(AppPalette.Red))
			{
				g.DrawLine(pen, new Point(bounds.Left, bounds.Top), new Point(bounds.Right, bounds.Bottom));
				g.DrawLine(pen, new Point(bounds.Left, bounds.Bottom), new Point(bounds.Right, bounds.Top));
			}
		}
	}
}
