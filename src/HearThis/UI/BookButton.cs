// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International. All Rights Reserved.
// <copyright from='2011' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using HearThis.Script;
using SIL.Scripture;
using System;
using System.Drawing;
using System.Windows.Forms;
using HearThis.Properties;

namespace HearThis.UI
{
	public partial class BookButton : UnitNavigationButton
	{
		internal const int kMaxChapters = 150; // Psalms

		private readonly BookInfo _model;

		private static int s_minWidth;
		private static bool s_displayLabels;
		private static readonly Font s_labelFont;

		protected override bool DisplayLabels => s_displayLabels;
		protected override Font LabelFont => s_labelFont;

		public static bool DisplayLabelsWhenPaintingButons { set => s_displayLabels = value; }

		static BookButton()
		{
			s_displayLabels = true;
			s_labelFont = ChapterButton.AttemptToCreateLabelFont("Segoe UI", 8) ??
				ChapterButton.AttemptToCreateLabelFont("Arial", 8);
		}

		public BookButton(BookInfo model)
		{
			_model = model;
			InitializeComponent();
			Text = BCVRef.NumberToBookCode(_model.BookNumber);
			if (s_minWidth == 0)
			{
				using (var g = CreateGraphics())
					s_minWidth = TextRenderer.MeasureText(g, "WW", LabelFont).Width;
			}
			Width = (int) (s_minWidth + (model.ChapterCount / (double) kMaxChapters) * 33.0);
		}

		public int BookNumber => _model.BookNumber;

		protected override void OnPaint(PaintEventArgs e)
		{
			var percentageTranslated = _model.CalculatePercentageTranslated();

			DrawButton(e.Graphics, percentageTranslated, _model.CalculatePercentageRecorded());
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
		}
	}
}
