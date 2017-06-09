using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SIL.Extensions;
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

		/// <summary>
		/// The blocks that have the current actor and character.
		/// </summary>
		private MultiVoiceBlock[] _activeBlocks;

		public int Id { get; private set; }

		public MultiVoiceChapter(XElement chapter, MultiVoiceScriptProvider provider)
		{
			ChapterElement = chapter;
			int id;
			Int32.TryParse(chapter.Attribute("id")?.Value, out id);
			Id = id;
			_blockElements = chapter.Elements("block").ToArray();
			_activeBlocks = _blocks = _blockElements.Select(b => new MultiVoiceBlock(b, provider)).ToArray();
			// Enhance: do we need to check this at runtime? If so how should we report failure?
			for (int i = 0; i < _blocks.Length; i++)
				Debug.Assert(i+1 == _blocks[i].Block.Number, "Blocks not numbered sequentially from 1");
		}

		public ScriptLine GetBlock(int lineNumber0Based)
		{
			return _activeBlocks[lineNumber0Based].Block;
		}

		public ScriptLine GetUnfilteredBlock(int lineNumber0Based)
		{
			if (lineNumber0Based < 0 || lineNumber0Based >= _blocks.Length)
				return null;
			return _blocks[lineNumber0Based].Block;
		}

		public int GetScriptBlockCount()
		{
			return _activeBlocks.Length;
		}

		public int GetUnskippedScriptBlockCount()
		{
			return _activeBlocks.Count(b => !b.Block.Skipped);
		}

		public int GetTranslatedVerseCount()
		{
			return _activeBlocks.Count(b => !string.IsNullOrEmpty(b.Block.Text));
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

		public void RestrictToCharacters(string actor, string character)
		{
			if (actor == null)
				_activeBlocks = _blocks;
			else
				_activeBlocks = _blocks.Where(b => b.Actor == actor && b.Character == character).ToArray();
		}

		public bool IsBlockInCharacter(int lineno0Based, string actor, string character)
		{
			var block = _blocks[lineno0Based];
			return block.Actor == actor && block.Character == character;
		}
	}
}
