// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022-2025, SIL Global.
// <copyright from='2021' to='2025' company='SIL Global'>
//		Copyright (c) 2022-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;

namespace HearThis.UI
{
	public static class PaintingExtensions
	{
		private const int kProblemIconDotSize = 4;
		private const int kExclamationGap = 1;

		public static void DrawExclamation(this Rectangle bounds, Graphics g, Brush brush = null)
		{
			brush = brush ?? AppPalette.BlueBrush;
			var dotSize = Math.Max(2, Math.Min(kProblemIconDotSize, bounds.Height - 3 * kProblemIconDotSize));
			bounds.Height -= 2;
			var dotRect = bounds.DrawDot(g, brush, dotSize);
			var exclamationWidth = Math.Min(dotRect.Width * 3 / 2, bounds.Width);
			if (dotRect.Width % 2 != exclamationWidth % 2)
				exclamationWidth++; // if one is even and the other is odd, they don't center, and the result is ugly.
			var lowerDotRect = new Rectangle(dotRect.X, dotRect.Top - kExclamationGap - dotRect.Height, dotRect.Width, dotRect.Height).DrawDot(g,
				brush, dotRect.Height);
			var upperDotRect = new Rectangle(bounds.X, bounds.Top + 1, bounds.Width, exclamationWidth).DrawDot(g, brush, exclamationWidth);
			var lowerDotVerticalMiddle = lowerDotRect.Top + lowerDotRect.Height / 2;
			var leftSideOfLowerDot = new Point(lowerDotRect.X, lowerDotVerticalMiddle);
			var rightSideOfLowerDot = new Point(lowerDotRect.Right, lowerDotVerticalMiddle);
			var upperDotVerticalMiddle = upperDotRect.Top + (lowerDotRect.Height - 1) / 2;
			var leftSideOfUpperDot = new Point(upperDotRect.X, upperDotVerticalMiddle);
			var rightSideOfUpperDot = new Point(upperDotRect.Right, upperDotVerticalMiddle);
			g.FillPolygon(brush, new[] {leftSideOfLowerDot, leftSideOfUpperDot, rightSideOfUpperDot, rightSideOfLowerDot});
		}
		
		public static Rectangle DrawDot(this Rectangle bounds, Graphics g, Brush brush = null, int preferredSize = kProblemIconDotSize)
		{
			brush = brush ?? AppPalette.BlueBrush;
			var dotSize = Math.Min(preferredSize, Math.Min(bounds.Width, bounds.Height));
			var dotRect = new Rectangle(bounds.X + (bounds.Width - dotSize) / 2,
				bounds.Bottom - (dotSize), dotSize, dotSize);
			g.FillEllipse(brush, dotRect);
			return dotRect;
		}
	}
}
