// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2022' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Drawing;
using System.Windows.Forms;
//using System.Windows.Forms.VisualStyles;

namespace HearThis.UI
{
	internal class ButtonWithoutFocusRectangle : Button
	{
		// Currently, the only place we use this control, the FlatStyle is set to Standard,
		// so we don't need all this code that is intended to fix the appearance if the
		// FlatStyle is Flat. But I'm leaving it here in case some future use requires it.

		//private static readonly VisualStyleRenderer Renderer;

		//static ButtonWithoutFocusRectangle()
		//{
		//	if (Application.RenderWithVisualStyles)
		//	{
		//		var elem = VisualStyleElement.Button.PushButton.Normal;
		//		Renderer = new VisualStyleRenderer(elem.ClassName, elem.Part, (int)PushButtonState.Normal);
		//	}
		//}

		public void SetUnconditionalFlatBackgroundColor(Color backColor)
		{
			FlatAppearance.MouseDownBackColor =
			FlatAppearance.MouseOverBackColor =
			FlatAppearance.MouseDownBackColor =
			FlatAppearance.MouseOverBackColor =
			FlatAppearance.BorderColor = backColor;
		}

		//protected override void OnPaint(PaintEventArgs e)
		//{
		//	base.OnPaint(e);
		//	if (Focused && Application.RenderWithVisualStyles && FlatStyle == FlatStyle.Standard)
		//	{
		//		Rectangle rc = Renderer.GetBackgroundContentRectangle(e.Graphics, new Rectangle(0, 0, Width, Height));
		//		rc.Height--;
		//		rc.Width--;
		//		using (Pen p = new Pen(FlatAppearance.BorderColor))
		//			e.Graphics.DrawRectangle(p, rc);
		//	}
		//}
	}
}
