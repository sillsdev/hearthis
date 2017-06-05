using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HearThis.Script
{
	/// <summary>
	/// Represents one block element from a Glyssen script file
	/// </summary>
	class MultiVoiceBlock
	{
		public XElement BlockElement { get; private set; }

		public MultiVoiceBlock(XElement block, MultiVoiceScriptProvider provider)
		{
			BlockElement = block;
			Block = new ScriptLine();
			Block.Text = block.Element("vern")?.Value.Trim()??"";
			int num;
			Int32.TryParse(block.Attribute("id")?.Value, out num);
			Block.Number = num;
			Block.ParagraphStyle = block.Attribute("tag")?.Value;
			Block.FontSize = provider.FontSize;
			Block.FontName = provider.FontName;
			Block.Verse = block.Attribute("verse")?.Value ?? "0";
			Block.RightToLeft = provider.RightToLeft;
		}

		public ScriptLine Block { get; private set; }
	}
}
