// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using HearThis.Script;
using SIL.Scripture;
using System;
using System.Windows.Forms;

namespace HearThis.UI
{
	public partial class BookButton : UnitNavigationButton
	{
		internal const int kMaxChapters = 150; // Psalms

		private readonly BookInfo _model;

		private static int s_widthForDisplayingLabels;
		private static int s_minProportionalWidth;
		private int _percentageRecorded;

		protected override bool DisplayLabels => DisplayLabelsWhenPaintingButtons;
		protected virtual float LabelFontSize => 8;

		public static bool DisplayLabelsWhenPaintingButtons { get; set; }

		public BookButton(BookInfo model, bool useFixedWidthForLabels)
		{
			_model = model;
			InitializeComponent();
			Text = BCVRef.NumberToBookCode(_model.BookNumber + 1);
			// Ideally this would be done in the static constructor, but we can't call CreateGraphics or access the font there.
			if (s_widthForDisplayingLabels == 0)
			{
				using (var g = CreateGraphics())
					s_widthForDisplayingLabels = TextRenderer.MeasureText(g, "NAM", Font).Width; // Nahum happens to have the widest 3-letter code.
				s_minProportionalWidth = Width;
			}
			SetWidth(useFixedWidthForLabels);
		}

		public void SetWidth(bool useFixedWidthForLabels)
		{
			Width = useFixedWidthForLabels ? s_widthForDisplayingLabels :
				(int)(s_minProportionalWidth + (_model.ChapterCount / (double)kMaxChapters) * 33.0);
		}

		public int BookNumber => _model.BookNumber;

		private int PercentageRecorded
		{
			get
			{
				if (_percentageRecorded == -1)
					_percentageRecorded = _model.CalculatePercentageRecorded();
				return _percentageRecorded;
			}
		}

		public void RecalculatePercentageRecorded()
		{
			_percentageRecorded = _model.CalculatePercentageRecorded();
			lock (this)
			{
				if (IsHandleCreated && !IsDisposed)
					Invoke(new Action(Invalidate));
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			DrawButton(e.Graphics, _model.HasTranslatedContent, PercentageRecorded);
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && ModifierKeys == Keys.Control)
			{
				_dangerousMenu.Show(this, e.Location);
			}
		}

		private void _makeDummyRecordings_Click(object sender, EventArgs e)
		{
			_model.MakeDummyRecordings();
			_percentageRecorded = 100;
			Invalidate();
		}
	}
}
