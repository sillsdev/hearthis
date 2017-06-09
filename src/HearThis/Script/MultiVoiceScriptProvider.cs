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
	/// A script provider that works from a GlyssenPack file and also supports ICharacterGroupProvider
	/// </summary>
	public class MultiVoiceScriptProvider : ScriptProviderBase, IActorCharacterProvider
	{
		public const string MultiVoiceFileExtension = ".glyssenscript"; // must be all LC
		private XDocument _script;
		private XElement[] _bookElements;
		// Key is book number, in the canonical sequence where Genesis is zero and Matthew is 39.
		// We use a dictionary rather than an array because the source is often sparse; not all
		// books may occur at all in the source file.
		private Dictionary<int, MultiVoiceBook> _books;
		private XElement _languageElement;

		/// <summary>
		/// The font size in points indicated in the language element of the script
		/// </summary>
		public int FontSize { get; private set; }

		public static BibleStats Stats = new BibleStats();

		/// <summary>
		///  This constructor takes the XML as a string (and is mainly used for testing)
		/// </summary>
		/// <param name="xmlInput"></param>
		public MultiVoiceScriptProvider(string xmlInput): this (XDocument.Parse(xmlInput))
		{
		}

		/// <summary>
		/// The main constructor, takes an XDocument in the glyssenscript format.
		/// </summary>
		/// <param name="script"></param>
		public MultiVoiceScriptProvider(XDocument script)
		{
			_script = script;
			_languageElement = _script.Root.Element("language");
			// FontSize and family and RTL must be initialized before we create the books (and their embedded blocks).
			try
			{
				FontSize = Int32.Parse(_languageElement.Element("fontSizeInPoints").Value);
			}
			catch (Exception ex) when (ex is NullReferenceException || ex is ArgumentNullException || ex is FormatException)
			{
				FontSize = 11; // is this a good default? Should always be specified, anyway.
			}
			FontName = _languageElement?.Element("fontFamily")?.Value ?? ""; // Some default?
			RightToLeft = _languageElement?.Element("rightToLeft")?.Value?.ToLowerInvariant() == "true";
			EthnologueCode = _languageElement?.Element("ldml")?.Value ?? ""; // Review: do we want the iso field or the ldml?
			ProjectFolderName =
				_script.Root?.Element("identification")?.Element("name")?.Value ?? ""; // enhance: better default?

			_bookElements = _script.Root.Element("script").Elements("book").ToArray();
			_books = new Dictionary<int, MultiVoiceBook>();

			foreach (var bookElt in _bookElements)
			{
				var book = new MultiVoiceBook(bookElt, this);
				_books[book.BookNumber] = book;
			}

			// AFTER we have the content data!
			AllEncounteredParagraphStyleNames = Blocks.Select(b => b.Block.ParagraphStyle)
				.Distinct()
				.Where(x => !string.IsNullOrEmpty(x))
				.ToArray();
			Initialize(); // loads skip information and makes this the skip handler
		}

		/// <summary>
		/// Creates a MultiVoiceScriptProvider by loading a file. This is the main way HearThis creates a real one.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static MultiVoiceScriptProvider Load(string path)
		{
			var script = XDocument.Load(path);
			return new MultiVoiceScriptProvider(script);
		}

		/// <summary>
		/// Get a specified block of the file. Caller should have ensured that this block exists.
		/// </summary>
		/// <param name="bookNumber"></param>
		/// <param name="chapter1Based"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">Book or chapter not loaded or invalid number</exception>
		/// <exception cref="IndexOutOfRangeException">Block number out of range</exception>
		public override ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			return _books[bookNumber].GetBlock(chapterNumber, lineNumber0Based);
		}

		public override ScriptLine GetUnfilteredBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			return _books[bookNumber]?.GetUnfilteredBlock(chapterNumber, lineNumber0Based);
		}

		public override int GetScriptBlockCount(int bookNumber, int chapter1Based)
		{
			return GetBook(bookNumber)?.GetScriptBlockCount(chapter1Based) ?? 0;
		}

		// Required to be implemented by base class, but never used.
		public override int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based)
		{
			throw new NotImplementedException();
		}

		public override int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based)
		{
			return GetBook(bookNumber)?.GetUnskippedScriptBlockCount(chapter1Based) ?? 0;
		}

		/// <summary>
		/// As far as I can tell, all logic that uses this just cares whether it is zero or not, that is,
		/// it's really just an indication that SOMETHING in the chapter is translated and needs recording.
		/// Bizarrely, ChapterInfo.CalculatePercentageTranslated() simply returns it; that also is only
		/// used in contexts that care only whether it is zero.
		/// For this provider, I've made it return the number of non-empty blocks in the chapter.
		/// That's probably actually more useful than the number of verses, and one day we may rename it
		/// either to (bool) IsAnyOfChapterTranslated or GetTranslatedBlockCount() (which would enable
		/// a more meaningful CalculatePercentageTranslated()).
		/// </summary>
		/// <param name="bookNumberDelegateSafe"></param>
		/// <param name="chapterNumber1Based"></param>
		/// <returns></returns>
		public override int GetTranslatedVerseCount(int bookNumber, int chapterNumber1Based)
		{
			return GetBook(bookNumber)?.GetTranslatedVerseCount(chapterNumber1Based) ?? 0;
		}

		// Gets the specified book, or null if it's not in the file.
		internal MultiVoiceBook GetBook(int bookNumber)
		{
			MultiVoiceBook book;
			_books.TryGetValue(bookNumber, out book);
			return book;
		}

		public override int GetScriptBlockCount(int bookNumber)
		{
			return GetBook(bookNumber)?.GetScriptBlockCount() ?? 0;
		}

		public override void LoadBook(int bookNumber0Based)
		{
			// nothing to do for now
		}

		public override string EthnologueCode { get; }
		public override bool RightToLeft { get; }
		public override string FontName { get; }
		public override string ProjectFolderName { get; }
		public override IEnumerable<string> AllEncounteredParagraphStyleNames { get; }
		public override IBibleStats VersificationInfo => Stats;

		/// <summary>
		/// Currently this is NOT filtered by actor/character.
		/// </summary>
		private IEnumerable<MultiVoiceBlock> Blocks => _books.Values.SelectMany(b => b.Blocks);

		#region Implementation of IActorCharacterProvider

		/// <summary>
		/// Returns all the actors who have been associated with blocks in the script.
		/// (Not filtered by actor/character)
		/// </summary>
		public IEnumerable<string> Actors
		{
			get
			{
				return Collect((book, set) => book.CollectActors(set));
			}
		}

		/// <summary>
		/// Collect unique strings from all books and return them in alphabetical order.
		/// The action specifies which strings are wanted by adding all desired strings from one book to a set.
		/// </summary>
		/// <param name="collect"></param>
		/// <returns></returns>
		private IEnumerable<string> Collect(Action<MultiVoiceBook, HashSet<string>> collect)
		{
			var collector = new HashSet<string>();
			_books.ForEach(kvp => collect(kvp.Value, collector));
			collector.Remove(string.Empty);
			var result = new List<string>(collector);
			result.Sort();
			return result;
		}

		/// <summary>
		/// Returns all the characters who have been designated to be played by the indicated actor in the script.
		/// (Not filtered by actor/character)
		/// </summary>
		/// <param name="actor"></param>
		/// <returns></returns>
		public IEnumerable<string> GetCharacters(string actor)
		{
			return Collect((book, set) => book.CollectCharacters(actor, set));
		}

		public string Actor { get; private set; }
		public string Character { get; private set; }

		public void RestrictToCharacter(string actor, string character)
		{
			Actor = actor;
			Character = character;
			_books.ForEach(kvp => kvp.Value.RestrictToCharacters(actor, character));
		}

		public bool IsBlockInCharacter(int book, int chapter, int lineno0based)
		{
			if (string.IsNullOrEmpty(Actor) || string.IsNullOrEmpty(Character))
				return true; // all blocks are in character if none specified.
			return GetBook(book)?.IsBlockInCharacter(chapter, lineno0based, Actor, Character) ?? false;
		}

		#endregion
	}
}
