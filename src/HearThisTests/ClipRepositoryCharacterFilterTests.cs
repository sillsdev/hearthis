using System;
using System.Collections.Generic;
using System.IO;
using HearThis;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;

namespace HearThisTests
{
	/// <summary>
	/// Test the ClipRepository methods that have to do with counting recordings,
	/// especially the when restricted by actor/character.
	/// </summary>
	public class ClipRepositoryCharacterFilterTests
	{
		internal const string kRiffWavHeader = "RIFF1234WAVEfmt 12345678901234567890data";
		private string _testFolderPath;
		private ClipFakeProvider _scriptProvider;
		private string _ex1Folder;
		private string _mat5Folder;
		private string _projectName;

		[OneTimeSetUp]
		public void MakeSampleFolder()
		{
			var folderNameBase = "fakeTestProject";
			int i = 0;
			while (Directory.Exists(Path.Combine(Program.ApplicationDataBaseFolder, folderNameBase + i)))
				i++;
			_projectName = folderNameBase + i;
			_testFolderPath = Program.GetApplicationDataFolder(_projectName);
			Directory.CreateDirectory(_testFolderPath);
			_ex1Folder = ClipRepository.GetChapterFolder(_projectName, "Exodus", 1);
			SimulateWaveFiles(_ex1Folder, new [] { 1, 2, 4 });
			_mat5Folder = ClipRepository.GetChapterFolder(_projectName, "Matthew", 5);
			SimulateWaveFiles(_mat5Folder, new[] { 1, 2, 3, 4 });
			_scriptProvider = new ClipFakeProvider();
			_scriptProvider.SimulateBlockInCharacter(1,1,0,0, "ex1.1"); // Block 1 of Exodus 1 is in character
			_scriptProvider.SimulateBlockInCharacter(39, 5, 1, 0, "mat 5.2"); // Block 2 of Mat 5 is in character (and will be filtered block 0)
			_scriptProvider.SimulateBlockInCharacter(39, 5, 3, 1, "mat 5.4"); // Block 4 of Mat 5 is in character (and will be filtered block 1)
		}

		void SimulateWaveFiles(string folderPath, int[] blocks)
		{
			foreach (var block in blocks)
			{
				File.WriteAllText(Path.Combine(folderPath, (block - 1).ToString() + ".wav"), kRiffWavHeader + "0000");
			}
		}

		[OneTimeTearDown]
		public void DestroySampleFolder()
		{
			Directory.Delete(_testFolderPath, true);
		}

		[Test]
		public void GetCountOfRecordingsInFolder_OrdinaryProvider_FullCounts()
		{
			Assert.That(ClipRepository.GetCountOfRecordingsInFolder(_ex1Folder, new FakeProvider()), Is.EqualTo(3));
			Assert.That(ClipRepository.GetCountOfRecordingsInFolder(_mat5Folder, new FakeProvider()), Is.EqualTo(4));
		}

		[Test]
		public void GetCountOfRecordingsInFolder_CharacterRestricted()
		{
			Assert.That(ClipRepository.GetCountOfRecordingsInFolder(_ex1Folder, _scriptProvider), Is.EqualTo(1));
			Assert.That(ClipRepository.GetCountOfRecordingsInFolder(_mat5Folder, _scriptProvider), Is.EqualTo(2));
		}

		[Test]
		public void GetCountOfRecordingsForBook_OrdinaryProvider_FullCounts()
		{
			Assert.That(ClipRepository.GetCountOfRecordingsForBook(_testFolderPath, "Exodus", new FakeProvider()), Is.EqualTo(3));
			Assert.That(ClipRepository.GetCountOfRecordingsForBook(_testFolderPath, "Matthew", new FakeProvider()), Is.EqualTo(4));
		}

		[Test]
		public void GetCountOfRecordingsForBook_CharacterRestricted()
		{
			Assert.That(ClipRepository.GetCountOfRecordingsForBook(_testFolderPath, "Exodus", _scriptProvider), Is.EqualTo(1));
			Assert.That(ClipRepository.GetCountOfRecordingsForBook(_testFolderPath, "Matthew", _scriptProvider), Is.EqualTo(2));
		}

		[Test]
		public void GetPathToLineRecording()
		{
			Assert.That(ClipRepository.GetPathToLineRecording(_projectName, "Matthew", 5, 1), Is.EqualTo(Path.Combine(_mat5Folder, "1.wav")));
			// It would be nice to test the behavior with a non-filtered script provider; but it's too much trouble to set up
			// (and wouldn't prove much unless we used a real script provider).
			//Assert.That(ClipRepository.GetPathToLineRecording(_projectName, "Matthew", 5, 1, new FakeProvider()), Is.EqualTo(Path.Combine(_mat5Folder, "1.wav")));
			// Block 1 in the filtered list is the second block for the character, block 4
			Assert.That(ClipRepository.GetPathToLineRecording(_projectName, "Matthew", 5, 1, _scriptProvider), Is.EqualTo(Path.Combine(_mat5Folder, "3.wav")));
		}
	}

	class ClipFakeProvider : IScriptProvider, IActorCharacterProvider
	{
		public ClipFakeProvider()
		{
			VersificationInfo = new BibleStats(); // need a real one to parse Exodus and Matthew.
		}
		public ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			_blocks.TryGetValue(Tuple.Create(bookNumber, chapterNumber, lineNumber0Based), out var result);
			return result;
		}

		public ScriptLine GetUnfilteredBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			throw new NotImplementedException();
		}

		public int GetScriptBlockCount(int bookNumber, int chapter1Based)
		{
			_blocksInChapterInCharacter.TryGetValue(Tuple.Create(bookNumber, chapter1Based), out var count);
			return count;
		}

		public int GetUnfilteredScriptBlockCount(int bookNumber, int chapter1Based)
		{
			return GetScriptBlockCount(bookNumber, chapter1Based);
		}

		public int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based)
		{
			throw new NotImplementedException();
		}

		public int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based)
		{
			throw new NotImplementedException();
		}

		public int GetTranslatedVerseCount(int bookNumber0Based, int chapterNumber1Based)
		{
			throw new NotImplementedException();
		}

		public int GetUnfilteredTranslatedVerseCount(int bookNumber0Based, int chapterNumber1Based)
		{
			throw new NotImplementedException();
		}

		public int GetScriptBlockCount(int bookNumber)
		{
			throw new NotImplementedException();
		}

		public void LoadBook(int bookNumber0Based)
		{
			throw new NotImplementedException();
		}

		public string EthnologueCode => null;
		public bool RightToLeft => false;
		public string FontName => null;
		public string ProjectFolderName => null;
		public IEnumerable<string> AllEncounteredParagraphStyleNames
		{
			get { yield break; }
		}
		public IEnumerable<char> AllEncounteredSentenceEndingCharacters
		{
			get { yield break; }
		}
		public IBibleStats VersificationInfo { get; }
		public bool NestedQuotesEncountered => false;

		public void UpdateSkipInfo()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<string> Actors { get; private set; }
		public IEnumerable<string> GetCharacters(string actor)
		{
			throw new NotImplementedException();
		}

		public void RestrictToCharacter(string actor, string character)
		{
			throw new NotImplementedException();
		}

		public string Actor { get; private set; }
		public string ActorForUI => Actor;
		public string Character { get; private set; }

		readonly Dictionary<Tuple<int, int, int>, bool> _blocksInCharacter = new Dictionary<Tuple<int, int, int>, bool>();
		readonly Dictionary<Tuple<int, int, int>, ScriptLine> _blocks = new Dictionary<Tuple<int, int, int>, ScriptLine>();
		readonly Dictionary<Tuple<int, int>, int> _blocksInChapterInCharacter = new Dictionary<Tuple<int, int>, int>();
		public void SimulateBlockInCharacter(int book, int chapter, int realLineNo, int fakeLineNo, string text)
		{
			var ccKey = Tuple.Create(book, chapter);
			_blocksInChapterInCharacter.TryGetValue(ccKey, out var count);
			_blocksInChapterInCharacter[ccKey] = count + 1;
			_blocksInCharacter[Tuple.Create(book, chapter, realLineNo)] = true;
			_blocks[Tuple.Create(book, chapter, fakeLineNo)] = new ScriptLine() { Number = realLineNo + 1, Text = text };
		}

		public bool IsBlockInCharacter(int book, int chapter, int lineNumber0Based)
		{
			if (!_blocksInCharacter.TryGetValue(Tuple.Create(book, chapter, lineNumber0Based), out var result))
				return false;
			return result;
		}

		public int GetNextUnrecordedLineInChapterForCharacter(int book, int chapter, int startLine)
		{
			throw new NotImplementedException();
		}

		public int GetNextUnrecordedChapterForCharacter(int book, int startChapter)
		{
			throw new NotImplementedException();
		}

		public FullyRecordedStatus FullyRecordedCharacters { get; private set; }
		public void DoWhenFullyRecordedCharactersAvailable(Action<FullyRecordedStatus> action)
		{
			throw new NotImplementedException();
		}
	}
}
