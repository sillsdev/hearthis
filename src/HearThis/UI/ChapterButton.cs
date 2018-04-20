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
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using HearThis.Script;

namespace HearThis.UI
{
	public partial class ChapterButton : UnitNavigationButton
	{
		private bool _selected;
		private int _percentageRecorded;
		private bool _hasTranslatedContent;

		private static int s_minWidth;

		protected override bool DisplayLabels => DisplayLabelsWhenPaintingButons;

		public static bool DisplayLabelsWhenPaintingButons { get; set; }

		public event EventHandler OnRecordingCompleteChanged;

		public ChapterButton(ChapterInfo chapterInfo)
		{
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

			//We'r'e doing ThreadPool instead of the more convenient BackgroundWorker based on experimentation and the advice on the web; we are doing relatively a lot of little threads here,
			//that don't really have to interact much with the UI until they are complete.
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
			lock (this)
			{
				if (IsHandleCreated && !IsDisposed)
					Invoke(new Action(Invalidate));
			}
		}

		public ChapterInfo ChapterInfo { get; }

		public int PercentageRecorded
		{
			get => _percentageRecorded;
			set
			{
				var chapterCompleteChanged = _percentageRecorded >= 100 && value < 100 ||
					_percentageRecorded < 100 && value >= 100;
				_percentageRecorded = value;
				if (chapterCompleteChanged)
					OnRecordingCompleteChanged?.Invoke(this, new EventArgs());
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
