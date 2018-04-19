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
	public partial class ChapterButton : UnitNavigationButton
	{
		private bool _selected;
		private int _percentageRecorded;
		private int _percentageTranslated;

		private static int s_minWidth;
		private static bool s_displayLabels;
		private static readonly Font s_labelFont;

		protected override bool DisplayLabels => s_displayLabels;
		protected override Font LabelFont => s_labelFont;

		public static bool DisplayLabelsWhenPaintingButons { set => s_displayLabels = value; }

		static ChapterButton()
		{
			s_displayLabels = true;
			s_labelFont = AttemptToCreateLabelFont("Segoe UI") ?? AttemptToCreateLabelFont("Arial");
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

		public ChapterInfo ChapterInfo { get; }

		protected override void OnPaint(PaintEventArgs e)
		{
			DrawButton(e.Graphics, _percentageTranslated, _percentageRecorded);
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
