// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HearThis.UI
{
	public abstract class SkipButtonBase : CheckBox
	{
		protected abstract Brush FillBrush { get; }
		protected abstract Color LineColor { get; }
		protected abstract float PercentageOfAvailableWidthToUseForLine { get; }
		protected abstract float StandardLineThickness { get; }

		protected float UsableHeight { get; private set; }
		protected float UsableWidth { get; private set; }
		private float _triangleHeight;
		protected float UsableRight { get; private set; }
		private float _top;
		protected float UsableBottom { get; private set; }
		protected float MiddleOfCurve { get; private set; }
		private int _btnHeight;
		private float _extraRightPaddingForLine;

		public SkipButtonBase()
		{
			Appearance = Appearance.Button;
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Opaque, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		protected bool MouseIsOverButton
		{
			get { return ClientRectangle.Contains(PointToClient(MousePosition)); }
		}

		protected virtual bool HighlightLines
		{
			get { return MouseIsOverButton; }
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			_btnHeight = Height - Padding.Bottom - Padding.Top;
			UsableHeight = _btnHeight + 2 * AppPallette.ButtonMouseOverPen.Width; // REVIEW: Should we be subtracting?
			UsableWidth = Width - Padding.Left - Padding.Right - 2 * AppPallette.ButtonMouseOverPen.Width;
			_triangleHeight = UsableHeight / 7F;
			UsableRight = Width - Padding.Right - AppPallette.ButtonMouseOverPen.Width;
			_top = Padding.Top + AppPallette.ButtonMouseOverPen.Width;
			UsableBottom = Height - Padding.Bottom - AppPallette.ButtonMouseOverPen.Width;
			MiddleOfCurve = _top + UsableBottom / 2F;
			_extraRightPaddingForLine = UsableWidth * (100 - PercentageOfAvailableWidthToUseForLine) / 100;
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			OnPaintBackground(pevent);
			DrawCurvySkipLine(pevent.Graphics);
		}

		protected void DrawCurvySkipLine(Graphics g)
		{
			float triangleWidth = StandardLineThickness * 3;
			float lineRight = UsableRight - triangleWidth / 2F - _extraRightPaddingForLine;
			float left = Padding.Left + StandardLineThickness / 2F + AppPallette.ButtonMouseOverPen.Width;

			PointF startPt = new PointF(lineRight, _top);
			PointF midPt = new PointF(left, MiddleOfCurve);
			PointF endPt = new PointF(lineRight, UsableBottom - _triangleHeight / 2F);
			Color lineColor;
			Brush fillBrush;
			if (!Enabled)
			{
				lineColor = AppPallette.EmptyBoxColor;
				fillBrush = AppPallette.DisabledBrush;
			}
			else
			{
				lineColor = LineColor;
				fillBrush = FillBrush;
			}

			g.SmoothingMode = SmoothingMode.AntiAlias;

			using (var pen = new Pen(lineColor, StandardLineThickness))
			{
				// Draw the curved line.
				PointF control1 = new PointF(lineRight, _top + _btnHeight / 3F);
				PointF control2 = new PointF(left, MiddleOfCurve - _btnHeight / 4F);
				PointF control3 = new PointF(left, MiddleOfCurve + _btnHeight / 4F);
				PointF control4 = new PointF(lineRight, endPt.Y - _btnHeight / 3F);
				PointF[] bezierPoints =
				{
					startPt, control1, control2, midPt,
					control3, control4, endPt
				};

				if (Checked)
					bezierPoints = CustomButton.GetPushedPoints(bezierPoints);

				g.DrawBeziers(pen, bezierPoints);

				if (HighlightLines)
				{
					float adj = (StandardLineThickness + AppPallette.ButtonMouseOverPen.Width) / 2F;
					g.DrawLine(AppPallette.ButtonMouseOverPen, new PointF(bezierPoints[0].X - adj, _top),
						new PointF(bezierPoints[0].X + adj, _top));

					for (int index = 0; index < bezierPoints.Length; index++)
					{
						PointF pt = bezierPoints[index];
						bezierPoints[index] = new PointF(pt.X - StandardLineThickness / 2, pt.Y);
					}
					g.DrawBeziers(AppPallette.ButtonMouseOverPen, bezierPoints);
					for (int index = 0; index < bezierPoints.Length; index++)
					{
						PointF pt = bezierPoints[index];
						bezierPoints[index] = new PointF(pt.X + StandardLineThickness, pt.Y);
					}
					g.DrawBeziers(AppPallette.ButtonMouseOverPen, bezierPoints);
				}

				// Draw the triangle
				var vertices = new PointF[3];
				vertices[0] = new PointF(lineRight - triangleWidth / 2F, UsableBottom - _triangleHeight); // left corner
				vertices[1] = new PointF(UsableRight - _extraRightPaddingForLine, UsableBottom - _triangleHeight); // right corner
				vertices[2] = new PointF(lineRight, UsableBottom); // point

				if (Checked)
					vertices = CustomButton.GetPushedPoints(vertices);

				g.FillPolygon(fillBrush, vertices);
				if (HighlightLines)
				{
					g.DrawPolygon(AppPallette.ButtonMouseOverPen, vertices);
					// Fix the little piece where the stem connects to the triangle.
					g.SmoothingMode = SmoothingMode.None;

					float middleOfTriangleBase = vertices[0].X + (vertices[1].X - vertices[0].X) / 2F;
					float adj = AppPallette.ButtonMouseOverPen.Width / 2 + 1;
					g.FillPolygon(AppPallette.BlueBrush, new[]
					{
						new PointF(middleOfTriangleBase - 1, vertices[0].Y - adj),
						new PointF(middleOfTriangleBase - StandardLineThickness - 1, vertices[0].Y + adj),
						new PointF(middleOfTriangleBase + StandardLineThickness, vertices[0].Y + adj)
					});

					g.SmoothingMode = SmoothingMode.AntiAlias;
				}
			}
		}
	}

	public class SkipButton : SkipButtonBase
	{
		protected override Brush FillBrush
		{
			get { return AppPallette.BlueBrush; }
		}

		protected override Color LineColor
		{
			get { return AppPallette.Blue; }
		}

		protected Color TextLineColor
		{
			get
			{
				if (Checked)
					return AppPallette.SkippedLineColor;
				if (!Enabled)
					return AppPallette.EmptyBoxColor;
				return AppPallette.Blue;
			}
		}

		protected override float PercentageOfAvailableWidthToUseForLine
		{
			get { return 100; }
		}

		protected override float StandardLineThickness
		{
			get { return Math.Min(6F, UsableWidth / 4F); }
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			OnPaintBackground(pevent);

			Graphics g = pevent.Graphics;

			float lineThickness = Math.Min(6F, UsableWidth / 4F);
			if (MouseIsOverButton)
				lineThickness += 2F;

			DrawCurvySkipLine(g);

			lineThickness = lineThickness / 2;
			const float thinLineWidth = 1F;
			if (MouseIsOverButton)
				lineThickness = lineThickness - thinLineWidth;


			using (var pen = new Pen(TextLineColor, lineThickness))
			{
				// Draw the text lines
				float dyTopLine = MiddleOfCurve - 1 - lineThickness / 2F;
				float leftEdge = Padding.Left + UsableWidth / 2F;
				if (MouseIsOverButton)
					leftEdge += AppPallette.ButtonMouseOverPen.Width;
				g.DrawLine(pen, new PointF(leftEdge, dyTopLine), new PointF(UsableRight, dyTopLine));
				float dyBottomLine = MiddleOfCurve + 1 + lineThickness / 2F;
				g.DrawLine(pen, new PointF(leftEdge, dyBottomLine), new PointF(UsableRight - lineThickness, dyBottomLine));

				if (HighlightLines)
				{
					using (var thinMouseOverPen = new Pen(AppPallette.ButtonMouseOverPen.Color, thinLineWidth))
					{
						// Highlight the text lines
						g.SmoothingMode = SmoothingMode.None;
						dyTopLine -= lineThickness;
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyTopLine), new PointF(UsableRight, dyTopLine));
						dyTopLine += lineThickness * 2 - 2 * thinLineWidth;
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyTopLine), new PointF(UsableRight, dyTopLine));
						dyBottomLine -= (lineThickness - 2 * thinLineWidth);
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyBottomLine), new PointF(UsableRight - lineThickness, dyBottomLine));
						dyBottomLine += lineThickness * 2 - 2 * thinLineWidth;
						g.DrawLine(thinMouseOverPen, new PointF(leftEdge, dyBottomLine), new PointF(UsableRight - lineThickness, dyBottomLine));
					}
				}
			}
		}
	}

	public class ShowSkippedBlocksButton : SkipButtonBase
	{
		private Brush _brush = new SolidBrush(Color.DimGray);

		protected override Brush FillBrush
		{
			get { return _brush; }
		}
		protected override Color LineColor
		{
			get { return Color.DimGray; }
		}
		protected override float PercentageOfAvailableWidthToUseForLine
		{
			get { return 50; }
		}
		protected override float StandardLineThickness
		{
			get { return Math.Min(2F, UsableWidth / 4F); }
		}

		protected override bool HighlightLines
		{
			get { return false; }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				_brush.Dispose();
			base.Dispose(disposing);
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			OnPaintBackground(pevent);

			Graphics g = pevent.Graphics;

			if (MouseIsOverButton)
			{
				using (Brush brush = new SolidBrush(AppPallette.MouseOverButtonBackColor))
				{
					g.FillRectangle(brush, ClientRectangle);
				}
			}

			DrawCurvySkipLine(g);

			using (var pen = new Pen(Color.DimGray, StandardLineThickness))
			{
				// Draw the eye
				float leftEdge = Padding.Left + UsableWidth / 3.5F;
				float radiusOfEye = Math.Min(UsableWidth, UsableHeight) / 3F;
				const int degreesToOmit = 30;
				const float radians = (float)(degreesToOmit * 2 * Math.PI / 180);
				float dyAdj = radiusOfEye * (float)Math.Cos(radians);
				//g.DrawLine(pen, 0, dyMiddleOfCurve, Right, dyMiddleOfCurve);
				g.DrawArc(pen, leftEdge, MiddleOfCurve + dyAdj - radiusOfEye - StandardLineThickness, 2 * radiusOfEye, 2 * radiusOfEye, 180 + degreesToOmit, 90 + degreesToOmit);
				g.DrawArc(pen, leftEdge, MiddleOfCurve - dyAdj - radiusOfEye - StandardLineThickness, 2 * radiusOfEye, 2 * radiusOfEye, degreesToOmit, 90 + degreesToOmit);
				// Draw the eyball
				float radiusOfEyeball = radiusOfEye / 2F;
				//float dxAdj = radiusOfEye * (float)Math.Sin(radians);
				g.FillEllipse(_brush, leftEdge + radiusOfEye - radiusOfEyeball / 2,
					MiddleOfCurve - radiusOfEyeball / 2 - dyAdj / 2, radiusOfEyeball, radiusOfEyeball);

				if (!Checked)
					g.DrawLine(pen, new PointF(0, 0), new PointF(UsableRight, UsableBottom));
			}
		}
	}
}
