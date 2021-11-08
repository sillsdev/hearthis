// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2018' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using HearThis.Script;

namespace HearThis.UI
{
	public abstract class UnitNavigationButton : UserControl
	{
		private const int kHorizontalPadding = 4;
		private const int kVerticalPadding = 4;
		const TextFormatFlags kTextPositionFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

		private bool _selected;
		private bool _showProblems;
		private ProblemType _worstProblem;

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

		public virtual void UpdateProblemState()
		{
			var prevValue = _worstProblem;
			_worstProblem = WorstProblem;
			if (prevValue != ProblemType.None || _worstProblem != ProblemType.None)
				InvalidateOnUIThread();
		}

		protected abstract ProblemType WorstProblem { get; }

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

			var problem = ShowProblems && _worstProblem != ProblemType.None;
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
					if ((_worstProblem & ProblemType.Unresolved) == ProblemType.Unresolved)
						r.DrawExclamation(g);
					else
						r.DrawDot(g);
				}
			}
		}

		private void DrawProgressIndicators(Graphics g, Rectangle bounds, int percentageRecorded, bool problem)
		{
			// If it is selected, drawing this line just makes the selection box look irregular.
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
					bounds.DrawExclamation(g, AppPallette.HighlightBrush);
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
				//complete the check mark
				g.DrawLine(progressPen, xBottom, v2, xTop, v3);
			}
		}

		private void DrawLabel(Graphics g, Rectangle bounds)
		{
			DrawButtonText(g, bounds, AppPallette.NavigationTextColor);
		}

		private void DrawButtonText(Graphics g, Rectangle bounds, Color color, string text = null, Font font = null)
		{
			bounds.Offset(0, -1);
			TextRenderer.DrawText(g, text ?? Text, font ?? Font, bounds, color, kTextPositionFlags);
		}
	}
}
