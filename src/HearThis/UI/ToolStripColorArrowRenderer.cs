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
	}
}
