// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2017' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Xml.Linq;

namespace HearThis.Script
{
	/// <summary>
	/// Represents one block element from a Glyssen script file
	/// </summary>
	class MultiVoiceBlock
	{
		public XElement BlockElement { get; }

		public MultiVoiceBlock(XElement block, string text, int num, MultiVoiceScriptProvider provider)
		{
			BlockElement = block;
			Block = new ScriptLine();
			Block.Text = text;
			Block.Number = num;
			Block.ParagraphStyle = block.Attribute("tag")?.Value;
			Block.FontSize = provider.FontSize;
			Block.FontName = provider.FontName;
			Block.Verse = block.Attribute("verse")?.Value ?? "0";
			Block.RightToLeft = provider.RightToLeft;
			Block.OriginalBlockNumber = BlockElement.Attribute("id")?.Value.Trim() ?? "";

			Block.Actor = Actor = BlockElement.Attribute("actor")?.Value ?? "";
			Block.Character = Character = BlockElement.Attribute("character")?.Value ?? "";
		}

		public ScriptLine Block { get; }

		public string Actor { get; }

		public string Character { get; }

	}
}
