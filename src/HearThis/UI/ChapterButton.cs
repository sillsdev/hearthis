// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using HearThis.Script;

namespace HearThis.UI
{
	public partial class ChapterButton : UnitNavigationButton
	{
		private readonly Func<int, ChapterInfo> _getUpdatedChapterInfo;
		private int _percentageRecorded;
		private bool _hasTranslatedContent;

		private static int s_minWidth;

		protected override bool DisplayLabels => DisplayLabelsWhenPaintingButtons;

		public static bool DisplayLabelsWhenPaintingButtons { get; set; }

		public event EventHandler RecordingCompleteChanged;

		public ChapterButton(ChapterInfo chapterInfo, Func<int, ChapterInfo> getChapterInfo)
		{
			_getUpdatedChapterInfo = getChapterInfo;
			ChapterInfo = chapterInfo;
			InitializeComponent();
			if (s_minWidth == 0)
			{
				using (var g = CreateGraphics())
					s_minWidth = Math.Max(15, TextRenderer.MeasureText(g,
						BookButton.kMaxChapters.ToString(CultureInfo.CurrentCulture), Font).Width);
			}
			Width = s_minWidth;
			Text = ChapterInfo.ChapterNumber1Based == 0 ? "i" : ChapterInfo.ChapterNumber1Based.ToString(CultureInfo.CurrentCulture);

			// We're doing ThreadPool instead of the more convenient (now deprecated) BackgroundWorker based on experimentation
			// and the advice on the web; we are doing relatively a lot of little threads here, that don't really have to
			// interact much with the UI until they are complete.
			var waitCallback = new WaitCallback(GetStatsInBackground);
			ThreadPool.QueueUserWorkItem(waitCallback, this);
		}

		private static void GetStatsInBackground(object stateInfo)
		{
			ChapterButton button = (ChapterButton)stateInfo;
			button._hasTranslatedContent = button.ChapterInfo.CalculatePercentageTranslated() > 0;
			button.RecalculatePercentageRecorded();
		}

		public void RecalculatePercentageRecorded()
		{
			PercentageRecorded = ChapterInfo.CalculatePercentageRecorded();
			InvalidateOnUIThread();
		}

		public override void UpdateProblemState()
		{
			ChapterInfo = _getUpdatedChapterInfo(ChapterInfo.ChapterNumber1Based);
			base.UpdateProblemState();
		}

		protected override ProblemType WorstProblem => ChapterInfo.WorstProblemInChapter;

		public ChapterInfo ChapterInfo { get; private set; }

		private int PercentageRecorded
		{
			set
			{
				var chapterCompleteChanged = _percentageRecorded >= 100 && value < 100 ||
					_percentageRecorded < 100 && value >= 100;
				_percentageRecorded = value;
				if (chapterCompleteChanged)
					RecordingCompleteChanged?.Invoke(this, new EventArgs());
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			DrawButton(e.Graphics, _hasTranslatedContent, _percentageRecorded);
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
			PercentageRecorded = 100;
			Invalidate();
		}

		private void OnRemoveRecordingsClick(object sender, EventArgs e)
		{
			ChapterInfo.RemoveRecordings();
			Invalidate();
		}
	}
}
