// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International. All Rights Reserved.
// <copyright from='2011' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HearThis.Properties;

namespace HearThis.UI
{
	public class UnitNavigationButton : UserControl
	{
		private const int kHorizontalPadding = 4;
		private const int kVerticalPadding = 4;

		private bool _selected;

		private static int s_minWidth;
		// The following should probably be overridden in derived classes, but
		// this class is not abstract because it messes up Designer.
		protected virtual bool DisplayLabels { get; }
		protected virtual Font LabelFont => Font;

		protected static Font AttemptToCreateLabelFont(string fontFace, int size = 7)
		{
			try
			{
				return new Font(fontFace, size, FontStyle.Bold);
			}
			catch
			{
				// font face is probably missing.
				return null;
			}
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

		protected void DrawButton(Graphics g, int percentageTranslated, int percentageRecorded)
		{
			if (Selected)
				g.FillRectangle(AppPallette.HighlightBrush, 0, 0, Width, Height);

			int fillWidth = Width - kHorizontalPadding;
			int fillHeight = Height - kVerticalPadding;
			var r = new Rectangle(kHorizontalPadding / 2, kVerticalPadding / 2, fillWidth, fillHeight);

			Brush fillBrush = percentageTranslated > 0 ? AppPallette.BlueBrush : AppPallette.EmptyBoxBrush;
			g.FillRectangle(fillBrush, r);

			g.SmoothingMode = SmoothingMode.AntiAlias;

			if (Settings.Default.DisplayNavigationButtonLabels && percentageTranslated > 0 && LabelFont != null)
				DrawLabel(g, r);
			else
				DrawProgressIndicators(g, r, percentageRecorded);
		}

		protected void DrawProgressIndicators(Graphics g, Rectangle bounds, int percentageRecorded)
		{
			// if it is selected, drawing this line just makes the selection box look irregular.
			// Also, they can readily see what is translated in the selected book or chapter by
			// looking at the more detailed display of its components.
			if (percentageRecorded > 0 && percentageRecorded < 100 && !Selected)
			{
				g.DrawLine(AppPallette.CompleteProgressPen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
			}
			else if (percentageRecorded >= 100)
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
							TextRenderer.DrawText(g, "!", font, bounds, AppPallette.HilightColor,
								TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
						return;
					}
					catch (Exception)
					{
						// Arial is probably missing. Just let it draw the check mark. Beats crashing.
					}
				}

				Pen progressPen = AppPallette.CompleteProgressPen;
				//draw the first stroke of a check mark
				g.DrawLine(progressPen, 4, v1, 7, v2);
				//complete the checkmark
				g.DrawLine(progressPen, 7, v2, 10, v3);
			}
		}

		protected void DrawLabel(Graphics g, Rectangle bounds)
		{
			const TextFormatFlags positionFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

			bounds.Offset(0, -1);
			TextRenderer.DrawText(g, Text, LabelFont, bounds,
				DisplayLabels ? AppPallette.NavigationTextColor : AppPallette.MouseOverButtonBackColor, positionFlags);
		}
	}
}
