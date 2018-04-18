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
	public partial class BookButton : UserControl
	{
		internal const int kMaxChapters = 150; // Psalms

		private readonly BookInfo _model;
		private bool _selected;

		private static int s_minWidth;
		public static bool DisplayLabels { get; set; }
		internal static Font LabelFont { get; }

		static BookButton()
		{
			DisplayLabels = true;
			LabelFont = new Font("Segoe UI", 8, FontStyle.Bold);
		}

		public BookButton(BookInfo model)
		{
			_model = model;
			InitializeComponent();
			if (s_minWidth == 0)
			{
				using (var g = CreateGraphics())
					s_minWidth = TextRenderer.MeasureText(g, "WW", LabelFont).Width;
			}
			Width = (int) (s_minWidth + (model.ChapterCount / (double) kMaxChapters) * 33.0);
		}

		public int BookNumber
		{
			get { return _model.BookNumber; }
		}

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
			var r = new Rectangle(2, 2, Width - 4, Height - 4);

			if (Selected)
			{
				e.Graphics.FillRectangle(AppPallette.HighlightBrush, 0, 0, Width, Height);
			}
			var percentageTranslated = _model.CalculatePercentageTranslated();
			ChapterButton.DrawBox(e.Graphics, r, Selected, percentageTranslated,
				_model.CalculatePercentageRecorded());
			
			if (DisplayLabels && Settings.Default.DisplayNavigationButtonLabels && percentageTranslated > 0)
				ChapterButton.DrawLabel(e.Graphics, r, LabelFont, SilBooks.Codes_3Letter[_model.BookNumber],
					SilBooks.Codes_2Letter[_model.BookNumber]);
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
