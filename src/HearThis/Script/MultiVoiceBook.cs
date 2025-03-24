using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SIL.Linq;

namespace HearThis.Script
{
	/// <summary>
	/// Represents one Book element from a Glyssen file
	/// </summary>
	class MultiVoiceBook
	{
		public XElement BookElement { get; private set; }

		private XElement[] _chapterElements;
		// Key is chapter number, where 0 typically indicates and introduction; but the ID
		// is simply taken from the attribute in the chapter, so there is no guarantee that
		// IDs form a complete list. Hence the use of a dictionary rather than an array.
		private Dictionary<int, MultiVoiceChapter> _chapters;

		public string Id { get; private set; }
		public int BookNumber { get; private set; }

		public MultiVoiceBook(XElement bookElement, MultiVoiceScriptProvider provider)
		{
			BookElement = bookElement;
			Id = BookElement.Attribute("id")?.Value;
			BookNumber = MultiVoiceScriptProvider.Stats.GetBookNumberFromCode(Id);
			_chapterElements = BookElement.Elements("chapter").ToArray();
			_chapters = new Dictionary<int, MultiVoiceChapter>();
			foreach (var chap in _chapterElements)
			{
				var chapter = new MultiVoiceChapter(chap, provider);
				_chapters[chapter.Id] = chapter;
			}
		}

		internal IEnumerable<MultiVoiceChapter> Chapters => _chapters.Values;

		public ScriptLine GetBlock(int chapterNumber, int lineNumber0Based)
		{
			return _chapters[chapterNumber].GetBlock(lineNumber0Based);
		}

		public ScriptLine GetUnfilteredBlock(int chapterNumber, int lineNumber0Based)
		{
			return _chapters[chapterNumber]?.GetUnfilteredBlock(lineNumber0Based);
		}

		public int GetScriptBlockCount()
		{
			return _chapters.Values.Sum(c => c.GetScriptBlockCount());
		}

		public int GetScriptBlockCount(int chapter1Based)
		{
			return GetChapter(chapter1Based)?.GetScriptBlockCount() ?? 0;
		}

		public int GetUnfilteredScriptBlockCount(int chapter1Based)
		{
			return GetChapter(chapter1Based)?.GetUnfilteredScriptBlockCount() ?? 0;
		}

		public int GetUnskippedScriptBlockCount(int chapter1Based)
		{
			return GetChapter(chapter1Based)?.GetUnskippedScriptBlockCount() ?? 0;
		}

		public int GetTranslatedVerseCount(int chapterNumber1Based, bool filtered)
		{
			return GetChapter(chapterNumber1Based)?.GetTranslatedVerseCount(filtered) ?? 0;
		}

		MultiVoiceChapter GetChapter(int chapter1Based)
		{
			MultiVoiceChapter chapter;
			_chapters.TryGetValue(chapter1Based, out chapter);
			return chapter; // may be null
		}
		public IEnumerable<MultiVoiceBlock> Blocks => _chapters.Values.SelectMany(c => c.Blocks);

		internal void CollectActors(HashSet<string> collector)
		{
			_chapters.ForEach(kvp => kvp.Value.CollectActors(collector));
		}

		internal void CollectCharacters(string actor, HashSet<string> collector)
		{
			_chapters.ForEach(kvp => kvp.Value.CollectCharacters(actor, collector));
		}

		public void RestrictToCharacters(string actor, string character)
		{
			_chapters.ForEach(kvp => kvp.Value.RestrictToCharacters(actor, character));
		}

		public bool IsBlockInCharacter(int chapter, int lineNbr0Based, string actor, string character)
		{
			return GetChapter(chapter)?.IsBlockInCharacter(lineNbr0Based, actor, character) ?? false;
		}
	}
}
