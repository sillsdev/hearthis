// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2017' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HearThis.UI
{
	/// <summary>
	/// This is apparently the only way we can make ToolStripDropDownButtons have non-black arrows.
	/// It makes such arrows use the forecolor as the arrow color.
	/// An instance should be created and set as the Renderer of the containing toolstrip.
	/// </summary>
	internal class ToolStripColorArrowRenderer : ToolStripRenderer, IDisposable
	{
		private Pen _checkedItemUnderlinePen;
		public Color CheckedItemUnderlineColor
		{
			get => _checkedItemUnderlinePen?.Color ?? Color.Empty;
			set
			{
				Dispose();
				_checkedItemUnderlinePen = value == Color.Empty ? null : new Pen(value, 3);
			}
		}

		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			e.ArrowColor = e.Item.ForeColor; // why on earth isn't this the default??
			base.OnRenderArrow(e);
		}

		/// <summary>
		/// Without this, for some reason the menu item hovered over goes white and thus almost
		/// disappears against the very light grey background of the menu. (But, we need to exclude
		/// the top-level button from the fix, since it doesn't need to go black.)
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			if (e.Item.Selected && !(e.Item is ToolStripDropDownButton))
				e.TextColor = Color.Black;
			base.OnRenderItemText(e);
		}

		/// <summary>
		/// Since all the menu items are black, making the hovered one black doesn't give any feedback.
		/// Giving a little color to the background provides some. The highlight color may be too bright,
		/// but it seemed the most appropriate of the colors already in our palette.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			if (e.Item.Selected)
			{
				e.Graphics.FillRectangle(AppPalette.HighlightBrush, new Rectangle(Point.Empty, e.Item.Size));
				return;
			}
			base.OnRenderMenuItemBackground(e);

			if (CheckedItemUnderlineColor != Color.Empty && e.Item is ToolStripMenuItem menu &&
				menu.Checked)
			{
				var rect = menu.ContentRectangle;
				var yPos = rect.Bottom;
				e.Graphics.DrawLine(_checkedItemUnderlinePen, rect.X, yPos,
					rect.Right, yPos);
			}
		}
		/// <summary>
		/// Without this we get no separators (or perhaps their default color matches the background?)
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			// Way too big and dark
			// e.Graphics.FillRectangle(AppPalette.BackgroundBrush, new Rectangle(Point.Empty, e.Item.Size));

			// No effect
			//e.Item.ForeColor = e.Item.BackColor = AppPalette.Background;
			//base.OnRenderSeparator(e);

			// stack overflow (calls this method again)
			//e.Item.ForeColor = e.Item.BackColor = AppPalette.Background;
			//DrawSeparator(e);

			int mid = e.Item.Height / 2;
			// Found experimentally. In typical menus, the grey line does not extend into the icon/check area.
			// I cannot find any property of any object that tells me how wide that area is.
			int left = 32;
			using (var pen = new Pen(Color.LightGray))
				e.Graphics.DrawLine(pen, left, mid, e.Item.Width, mid);
		}

		public void Dispose()
		{
			_checkedItemUnderlinePen?.Dispose();
		}
	}
}
