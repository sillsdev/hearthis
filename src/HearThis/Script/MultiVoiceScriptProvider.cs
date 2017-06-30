using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using HearThis.Properties;
using HearThis.Publishing;
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
		private SentenceClauseSplitter _splitter;
		private IRecordingAvailability _recordingAvailabilitySource;
		private FullyRecordedStatus _mostRecentFullyRecordedCharacters;
		private Action<FullyRecordedStatus> _doWhenFullyRecordedCharactersAvailable;
		private object _syncLock = new object();

		/// <summary>
		/// The font size in points indicated in the language element of the script
		/// </summary>
		public int FontSize { get; private set; }

		public static BibleStats Stats = new BibleStats();

		/// <summary>
		///  This constructor takes the XML as a string (and is mainly used for testing)
		/// </summary>
		/// <param name="xmlInput"></param>
		/// <param name="splitter"></param>
		public MultiVoiceScriptProvider(string xmlInput, SentenceClauseSplitter splitter = null): this (XDocument.Parse(xmlInput), splitter)
		{
		}

		// If revision in file is greater than this, we don't know how to read it.
		private const int kCurrentMaxFileVersion = 1;
		/// <summary>
		/// The main constructor, takes an XDocument in the glyssenscript format.
		/// </summary>
		/// <param name="script"></param>
		/// <param name="splitter"></param>
		public MultiVoiceScriptProvider(XDocument script, SentenceClauseSplitter splitter = null)
		{
			_splitter = splitter;
			if (_splitter == null)
			{
				char[] separators = null;
				string additionalBreakCharacters = Settings.Default.AdditionalBlockBreakCharacters.Replace(" ", string.Empty);
				if (additionalBreakCharacters.Length > 0)
					separators = additionalBreakCharacters.ToArray();
				// We never need to break at quotes with a glyssen script, since quotes are always a separate block already.
				// The constructor needs a non-null value for scripture settings, but we don't actually use anything from it
				// when breakAtFirstLevelQuotes is false, so we just pass an empty one.
				_splitter = new SentenceClauseSplitter(separators, false, new GenericScriptureSettings());
			}
			_script = script;
			var fileVersion = _script.Root.Attribute("version")?.Value??"1.0";
			if (string.IsNullOrEmpty(fileVersion))
				fileVersion = "1.0";
			if (!fileVersion.Contains('.'))
				fileVersion += ".0"; // Version.Parse doesn't allow plain "2"
			Version version = Version.Parse(fileVersion); // go ahead and throw...can't handle this file...if we can't parse.
			if (version.Major > kCurrentMaxFileVersion)
				throw new ArgumentException("File is a later major version than current HearThis can handle");

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
			// AFTER initialize loads skip info, apply it to our lines.
			foreach (var book in _books.Values)
			{
				foreach (var chap in book.Chapters)
					PopulateSkippedFlag(book.BookNumber, chap.Id, chap.Blocks.Select(b => b.Block).ToList());
			}
		}

		/// <summary>
		/// Creates a MultiVoiceScriptProvider by loading a file and making a default sentence splitter from settings. This is the main way HearThis creates a real one.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static MultiVoiceScriptProvider Load(string path)
		{
			var script = XDocument.Load(path);
			var result = new MultiVoiceScriptProvider(script);
			// start computing what is recorded in the background. Hopefully it will be all ready when needed.
			var worker = new BackgroundWorker();
			FullyRecordedStatus fullyRecordedCharacters = null;
			worker.DoWork += (sender, args) =>
			{
				fullyRecordedCharacters = result.FullyRecordedCharacters;
			};

			worker.RunWorkerCompleted += (sender, args) =>
			{
				lock (result._syncLock)
				{
					result._doWhenFullyRecordedCharactersAvailable?.Invoke(fullyRecordedCharacters);
				}
			};
			worker.RunWorkerAsync();
			return result;
		}

		public void DoWhenFullyRecordedCharactersAvailable(Action<FullyRecordedStatus> action)
		{
			// guard against a race condition between this thread setting _doWhenFullyRecordedCharactersAvailable
			// and the thread creating it checking the result.
			// The only way _mostRecentFullyRecordedCharacters can NOT be null is if the background
			// thread already completed; once that happens, we don't need to sync.
			lock (_syncLock)
			{
				if (_mostRecentFullyRecordedCharacters == null)
				{
					_doWhenFullyRecordedCharactersAvailable = action;
					return;
				}
			}
			// We already, have it, but it may be out of date. By returning the property rather than
			// the member variable, we get the benefit of the getter code that checks recordings
			// of the current character, if any.
			action(FullyRecordedCharacters);
		}

		internal SentenceClauseSplitter Splitter => _splitter;

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

		public override int GetUnfilteredScriptBlockCount(int bookNumber, int chapter1Based)
		{
			return GetBook(bookNumber)?.GetUnfilteredScriptBlockCount(chapter1Based) ?? 0;
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
			return GetBook(bookNumber)?.GetTranslatedVerseCount(chapterNumber1Based, true) ?? 0;
		}
		public override int GetUnfilteredTranslatedVerseCount(int bookNumber, int chapterNumber1Based)
		{
			return GetBook(bookNumber)?.GetTranslatedVerseCount(chapterNumber1Based, false) ?? 0;
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

		public IRecordingAvailability RecordingAvailabilitySource
		{
			get
			{
				// Except in testing, we obtain this information from the real ClipRepository, via this wrapper.
				if (_recordingAvailabilitySource == null)
					_recordingAvailabilitySource = new RecordingAvailability();
				return _recordingAvailabilitySource;
			}
			set { _recordingAvailabilitySource = value; }
		}

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
			// various things test for null actor and character, and they are persisted in
			// Settings.Default, which seems to yield an empty string when set to null.
			Actor = string.IsNullOrWhiteSpace(actor) ? null : actor;
			Character = string.IsNullOrWhiteSpace(character) ? null : character;
			_books.ForEach(kvp => kvp.Value.RestrictToCharacters(Actor, Character)); // be sure to use corrected fields, not args, here
		}

		public bool IsBlockInCharacter(int book, int chapter, int lineno0based)
		{
			if (string.IsNullOrEmpty(Actor) || string.IsNullOrEmpty(Character))
				return true; // all blocks are in character if none specified.
			return GetBook(book)?.IsBlockInCharacter(chapter, lineno0based, Actor, Character) ?? false;
		}

		public int GetNextUnrecordedLineInChapterForCharacter(int book, int chapter, int startLine)
		{
			if (Character == null)
				return startLine;
			var blockCount = GetUnfilteredScriptBlockCount(book, chapter);
			for (int blockNum = startLine; blockNum < blockCount; blockNum++)
			{
				var block = GetUnfilteredBlock(book, chapter, blockNum);
				if (block.Skipped)
					continue;
				if (block.Character != Character || block.Actor != Actor)
					continue;
				if (!RecordingAvailabilitySource.GetHaveClipUnfiltered(ProjectFolderName, VersificationInfo.GetBookName(book), chapter, blockNum))
				{
					return blockNum;
				}
			}
			// If we don't find one, go with what we already had
			return startLine;
		}

		public int GetNextUnrecordedChapterForCharacter(int book, int startChapter)
		{
			if (Character == null)
				return startChapter;
			MultiVoiceBook multiVoiceBook;
			if (!_books.TryGetValue(book, out multiVoiceBook))
				return startChapter;
			foreach (var chap in multiVoiceBook.Chapters)
			{
				if (chap.Id < startChapter)
					continue;
				var blockCount = GetUnfilteredScriptBlockCount(book, chap.Id);
				for (int blockNum = 0; blockNum < blockCount; blockNum++)
				{
					var block = GetUnfilteredBlock(book, chap.Id, blockNum);
					if (block.Skipped)
						continue;
					if (block.Character != Character || block.Actor != Actor)
						continue;
					if (!RecordingAvailabilitySource.GetHaveClipUnfiltered(ProjectFolderName, VersificationInfo.GetBookName(book), chap.Id, blockNum))
					{
						return chap.Id;
					}
				}
			}
			return startChapter;
		}

		/// <summary>
		/// for testing.
		/// </summary>
		internal void ClearFullyRecordedCharacters()
		{
			_mostRecentFullyRecordedCharacters = null;
		}

		/// <summary>
		/// Gather all information about fully recorded characters.
		/// This can take a while and is therefore sometimes run on a background thread.
		/// The results are cached in a member variable, but only when fully computed, so any call
		/// on a different thread won't get an incomplete result.
		/// When there is a current actor/character, any cached value is updated on each call
		/// to validate the information for that character.
		/// (There should be no way for any other character's data to get out of date.)
		/// </summary>
		public FullyRecordedStatus FullyRecordedCharacters {

			get
			{
				if (_mostRecentFullyRecordedCharacters == null)
				{
					var charsWithMissingRecordings = new HashSet<Tuple<string, string>>();
					var result = new FullyRecordedStatus(this);
					var availabilty = RecordingAvailabilitySource;
					foreach (var book in _books.Values)
					{
						var bookName = VersificationInfo.GetBookName(book.BookNumber);
						foreach (var chap in book.Chapters)
						{
							foreach (var block in chap.Blocks)
							{
								var key = Tuple.Create(block.Actor, block.Character);
								if (availabilty.GetHaveClipUnfiltered(ProjectFolderName, bookName, chap.Id, block.Block.Number - 1))
								{
									if (!charsWithMissingRecordings.Contains(key))
										result.Add(block.Actor, block.Character);
								}
								else
								{
									charsWithMissingRecordings.Add(key);
									result.Remove(block.Actor, block.Character);
								}
							}
						}
					}
					_mostRecentFullyRecordedCharacters = result;
				}
				else if (Character != null)
				{
					var availabilty = RecordingAvailabilitySource;
					foreach (var book in _books.Values)
					{
						var bookName = VersificationInfo.GetBookName(book.BookNumber);
						foreach (var chap in book.Chapters)
						{
							foreach (var block in chap.Blocks)
							{
								if (block.Actor != Actor || block.Character != Character)
									continue; // info about any other character should be correct.
								if (!availabilty.GetHaveClipUnfiltered(ProjectFolderName, bookName, chap.Id, block.Block.Number - 1))
								{
									_mostRecentFullyRecordedCharacters.Remove(Actor, Character);
									return _mostRecentFullyRecordedCharacters;
								}
							}
						}
					}
					// Completed our search without finding any unrecorded blocks for current charcter.
					_mostRecentFullyRecordedCharacters.Add(Actor, Character);
				}
				return _mostRecentFullyRecordedCharacters;
			}
		}
		#endregion
	}
}
