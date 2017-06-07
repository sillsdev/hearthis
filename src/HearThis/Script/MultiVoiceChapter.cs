using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SIL.Linq;

namespace HearThis.Script
{
	/// <summary>
	/// Represents one chapter from a Glyssen file
	/// </summary>
	class MultiVoiceChapter
	{
		public XElement ChapterElement { get; private set; }
		private XElement[] _blockElements;
		// The blocks that occur in this chapter. It is expected that the block at index i has Number i+1
		// (that is, the first block in each chapter is expected to have ID/Number 1, though HearThis usually
		// identifies it by index (0), and the corresponding wave file also uses 0.wav).
		private MultiVoiceBlock[] _blocks;

		public int Id { get; private set; }

		public MultiVoiceChapter(XElement chapter, MultiVoiceScriptProvider provider)
		{
			ChapterElement = chapter;
			int id;
			Int32.TryParse(chapter.Attribute("id")?.Value, out id);
			Id = id;
			_blockElements = chapter.Elements("block").ToArray();
			_blocks = _blockElements.Select(b => new MultiVoiceBlock(b, provider)).ToArray();
			// Enhance: do we need to check this at runtime? If so how should we report failure?
			for (int i = 0; i < _blocks.Length; i++)
				Debug.Assert(i+1 == _blocks[i].Block.Number, "Blocks not numbered sequentially from 1");
		}

		public ScriptLine GetBlock(int lineNumber0Based)
		{
			return _blocks[lineNumber0Based].Block;
		}

		public int GetScriptBlockCount()
		{
			return _blocks.Length;
		}

		public int GetUnskippedScriptBlockCount()
		{
			return _blocks.Count(b => !b.Block.Skipped);
		}

		public int GetTranslatedVerseCount()
		{
			return _blocks.Count(b => !string.IsNullOrEmpty(b.Block.Text));
		}

		public IEnumerable<MultiVoiceBlock> Blocks => _blocks;

		internal void CollectActors(HashSet<string> collector)
		{
			_blocks.ForEach(b => collector.Add(b.Actor));
		}

		internal void CollectCharacters(string actor, HashSet<string> collector)
		{
			foreach (var block in _blocks)
			{
				if (block.Actor == actor)
					collector.Add(block.Character);
			}
		}
	}
}
