using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
			int index = 0;
			_activeBlocks = _blocks = _blockElements.SelectMany(b => GetBlocks(provider, b, ref index)).ToArray();
			// Enhance: do we need to check this at runtime? If so how should we report failure?
			for (int i = 0; i < _blocks.Length; i++)
				Debug.Assert(i+1 == _blocks[i].Block.Number, "Blocks not numbered sequentially from 1");
		}

		private static MultiVoiceBlock[] GetBlocks(MultiVoiceScriptProvider provider, XElement block, ref int index)
		{
			string text;
			switch (provider.Version?.Major)
			{
				// If greater than max allowable in this version, we already handled that in MultiVoiceScriptProvider constructor
				case 2:
					text = String.Join("", block.Element("vernacularText")?.Elements("text").Select(textElement => textElement?.Value ?? "") ?? new string[0]);
					break;
				default:
					text = block.Element("text")?.Value.Trim() ?? "";
					break;
			}

			var chunks = provider.Splitter.BreakIntoChunks(text);
			MultiVoiceBlock[] result;
			if (chunks.Any())
			{
				int indexLocal = index;
				// It's important that the enumeration is evaluated exactly once, and fully, so all the increment
				// operations have occurred before we pass index back to the caller.
				result = chunks.Select(chunk => new MultiVoiceBlock(block, chunk.Text, ++indexLocal, provider)).ToArray();
				index = indexLocal;
			}
			else
			{
				// We want at least one. (Review: do we? Will we ever get empty blocks from Glyssen?)
				result = new[] {new MultiVoiceBlock(block, "", ++index, provider)};
			}
			return result;
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

		public int GetUnfilteredScriptBlockCount()
		{
			return _blocks.Length;
		}
		public int GetUnskippedScriptBlockCount()
		{
			return _blocks.Count(b => !b.Block.Skipped);
		}

		public int GetTranslatedVerseCount(bool filtered)
		{
			return (filtered ? _activeBlocks : _blocks).Count(b => !string.IsNullOrEmpty(b.Block.Text));
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

		public bool IsBlockInCharacter(int lineNbr0Based, string actor, string character)
		{
			if (_blocks.Length <= lineNbr0Based) // HT-493: extra file
				return false;
			var block = _blocks[lineNbr0Based];
			return block.Actor == actor && block.Character == character;
		}
	}
}
