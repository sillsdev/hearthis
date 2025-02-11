// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022-2025, SIL Global.
// <copyright from='2018' to='2025' company='SIL Global'>
//		Copyright (c) 2022-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Drawing;
using System.Windows.Forms;

namespace HearThis.UI
{
	public class ExclamationIcon : UserControl
	{
		private const string kDefaultIconText = "!";
		public override Color BackColor { get; set; } = AppPalette.Background;
		public override string Text { get; set; } = kDefaultIconText;

		public void ResetIcon()
		{
			Text = kDefaultIconText;
			Visible = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var rect = new Rectangle(ClientRectangle.X + Padding.Left, ClientRectangle.Y + Padding.Top,
				ClientRectangle.Width - Padding.Horizontal, ClientRectangle.Height - Padding.Vertical);
			base.OnPaint(e);
			e.Graphics.FillEllipse(AppPalette.HighlightBrush, rect);
			if (rect.Height - 3 <= Font.SizeInPoints && Text == kDefaultIconText)
				rect.DrawExclamation(e.Graphics, AppPalette.BackgroundBrush);
			else
			{
				rect.Inflate(-6, -1);
				//rect.Y = 0;
				rect.X += 1;
				rect.Y -= 1; // Annoying magic fudge factor because otherwise it is too low.

				TextRenderer.DrawText(e.Graphics, Text, Font, rect, AppPalette.Background, AppPalette.HilightColor,
					TextFormatFlags.HorizontalCenter | TextFormatFlags.Top);
			}
		}
	}
}
