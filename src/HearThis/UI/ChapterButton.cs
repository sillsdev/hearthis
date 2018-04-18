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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Script;

namespace HearThis.UI
{
	public partial class ChapterButton : UserControl
	{
		private const int kHorizontalPadding = 4;
		private const int kVerticalPadding = 4;
		

		private bool _selected;
		private int _percentageRecorded;
		private int _percentageTranslated;

		private static int s_minWidth;
		public static bool DisplayLabels { get; set; }
		internal static Font LabelFont { get; }
		static ChapterButton()
		{
			DisplayLabels = true;
			LabelFont = new Font("Segoe UI", 7, FontStyle.Bold);
		}

		public ChapterButton(ChapterInfo chapterInfo)
		{
			ChapterInfo = chapterInfo;
			InitializeComponent();
			if (s_minWidth == 0)
			{
				using (var g = CreateGraphics())
					s_minWidth = Math.Max(15, TextRenderer.MeasureText(g,
						BookButton.kMaxChapters.ToString(CultureInfo.CurrentCulture), LabelFont).Width);
			}
			Width = s_minWidth;
			Text = ChapterInfo.ChapterNumber1Based == 0 ? "i" : ChapterInfo.ChapterNumber1Based.ToString(CultureInfo.CurrentCulture);

			//We'r'e doing ThreadPool instead of the more convenient BackgroundWorker based on experimentation and the advice on the web; we are doing relatively a lot of little threads here,
			//that don't really have to interact much with the UI until they are complete.
			var waitCallback = new WaitCallback(GetStatsInBackground);
			ThreadPool.QueueUserWorkItem(waitCallback, this);
		}

		private static void GetStatsInBackground(object stateInfo)
		{
			ChapterButton button = (ChapterButton)stateInfo;
			button._percentageTranslated = button.ChapterInfo.CalculatePercentageTranslated();
			button.RecalculatePercentageRecorded();
		}

		public void RecalculatePercentageRecorded()
		{
			_percentageRecorded = ChapterInfo.CalculatePercentageRecorded();
			lock (this)
			{
				if (IsHandleCreated && !IsDisposed)
				{
					Invoke(new Action(Invalidate));
				}
			}
		}

		public ChapterInfo ChapterInfo { get; private set; }

		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (_selected != value)
				{
					_selected = value;
					Invalidate();
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			int fillWidth = Width - kHorizontalPadding;
			int fillHeight = Height - kVerticalPadding;
			var r = new Rectangle(kHorizontalPadding / 2, kVerticalPadding / 2, fillWidth, fillHeight);
			if (Selected)
			{
				e.Graphics.FillRectangle(AppPallette.HighlightBrush, 0, 0, Width, Height);
			}

			DrawBox(e.Graphics, r, Selected, _percentageTranslated, _percentageRecorded);
			if (Settings.Default.DisplayNavigationButtonLabels && _percentageTranslated > 0)
				DrawLabel(e.Graphics, r, LabelFont, Text);
		}

		/// <summary>
		/// NB: used by both chapter and book buttons
		/// </summary>
		public static void DrawBox(Graphics g, Rectangle bounds, bool selected, int percentageTranslated,
			int percentageRecorded)
		{
			Brush fillBrush = percentageTranslated > 0 ? AppPallette.BlueBrush : AppPallette.EmptyBoxBrush;
			g.FillRectangle(fillBrush, bounds);

			g.SmoothingMode = SmoothingMode.AntiAlias;
			// if it is selected, drawing this line just makes the selection box look irregular.
			// Also, they can readily see what is translated in the selected book or chapter by
			// looking at the more detailed display of its components.
			if (percentageRecorded > 0 && percentageRecorded < 100 && !selected)
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

		/// <summary>
		/// NB: used by both chapter and book buttons
		/// </summary>
		public static void DrawLabel(Graphics g, Rectangle bounds, Font font, string preferredLabel, string fallbackLabel = null)
		{
			const TextFormatFlags positionFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

			bounds.Offset(0, -1);
			try
			{
				var label = preferredLabel;
				if (fallbackLabel != null)
				{
					if (TextRenderer.MeasureText(g, preferredLabel, font, bounds.Size, TextFormatFlags.NoPadding | positionFlags).Width > bounds.Width + 2)
						label = fallbackLabel;
				}
				if (label != null)
					TextRenderer.DrawText(g, label, font, bounds,
						DisplayLabels ? AppPallette.NavigationTextColor : AppPallette.MouseOverButtonBackColor, positionFlags);
			}
			catch (Exception)
			{
				// Font is probably missing. Skip label. Beats crashing.
			}
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && Control.ModifierKeys == Keys.Control)
			{
				_dangerousMenu.Show(this, e.Location);
			}
		}

		private void _makeDummyRecordings_Click(object sender, EventArgs e)
		{
			ChapterInfo.MakeDummyRecordings();
			Invalidate();
		}


		private void _dangerousMenu_Opening(object sender, CancelEventArgs e)
		{

		}

		private void OnRemoveRecordingsClick(object sender, EventArgs e)
		{
			ChapterInfo.RemoveRecordings();
			Invalidate();
		}
	}
}
