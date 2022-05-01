// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2022' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace HearThis.UI
{
	internal class ButtonWithoutFocusRectangle : Button
	{
		private static readonly VisualStyleRenderer Renderer;

		static ButtonWithoutFocusRectangle()
		{
			VisualStyleElement elem = VisualStyleElement.Button.PushButton.Normal;
			Renderer = new VisualStyleRenderer(elem.ClassName, elem.Part, (int)PushButtonState.Normal);
		}

		public void SetUnconditionalFlatBackgroundColor(Color backColor)
		{
			FlatAppearance.MouseDownBackColor =
			FlatAppearance.MouseOverBackColor =
			FlatAppearance.MouseDownBackColor =
			FlatAppearance.MouseOverBackColor =
			FlatAppearance.BorderColor = backColor;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Focused && Application.RenderWithVisualStyles && FlatStyle == FlatStyle.Standard)
			{
				Rectangle rc = Renderer.GetBackgroundContentRectangle(e.Graphics, new Rectangle(0, 0, Width, Height));
				rc.Height--;
				rc.Width--;
				using (Pen p = new Pen(FlatAppearance.BorderColor))
					e.Graphics.DrawRectangle(p, rc);
			}
		}
	}
}
