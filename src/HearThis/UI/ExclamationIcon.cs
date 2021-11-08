// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2018' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
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
		public override Color BackColor { get; set; } = AppPallette.Blue;
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
			if (rect.Height <= Font.SizeInPoints && Text == kDefaultIconText)
				rect.DrawExclamation(e.Graphics, AppPallette.HighlightBrush);
			else
			{
				rect.Y -= 2; // Annoying magic fudge factor because otherwise it is too low.

				TextRenderer.DrawText(e.Graphics, Text, Font, rect, AppPallette.HilightColor, BackColor,
					TextFormatFlags.HorizontalCenter | TextFormatFlags.Top);
			}
		}
	}
}
