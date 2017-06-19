using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HearThis.UI
{
	/// <summary>
	/// This is apparently the only way we can make ToolStripDropDownButtons have non-black arrows.
	/// It makes such arrows use the forecolor as the arrow color.
	/// An instance should be created and set as the Renderer of the containing toolstrip.
	/// </summary>
	internal class ToolStripColorArrowRenderer : ToolStripRenderer
	{
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
		/// Giving a little color to the background provides some. The hightlight color may be too bright,
		/// but it seemed the most appropriate of the colors already in our palette.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			if (e.Item.Selected)
			{
				e.Graphics.FillRectangle(AppPallette.HighlightBrush, new Rectangle(Point.Empty, e.Item.Size));
				return;
			}
			base.OnRenderMenuItemBackground(e);
		}
	}
}
