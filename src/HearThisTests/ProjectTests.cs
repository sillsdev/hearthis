// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2014' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;
using SIL.IO;
using SIL.ObjectModel;
using SIL.Xml;

namespace HearThisTests
{
	/// <summary>
	/// As yet very incomplete, this is a place to put tests of Project methods.
	/// </summary>
	[TestFixture]
	public class ProjectTests
	{
		[Test]
		public void GetProjectRecordingStatusInfoFileContent_ReturnsStringWithChapInfo()
		{
			var fakeScriptProvider = new TestScriptProvider();
			var project = new Project(fakeScriptProvider);
			var infoContent = project.GetProjectRecordingStatusInfoFileContent();
			Assert.That(infoContent, Is.EqualTo("Genesis;" + Environment.NewLine + "Matthew;6:0,3:2,7:3,2:2" + Environment.NewLine));
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("", '.')]
		[TestCase(null, '?', '!')]
		public void GetIPublishingInfoProvider_BlockBreakCharacters_NoAdditionalCharacters_ReturnsOnlySentenceEndingCharacters(
			string additionalBreakChars, params char[] simulatedSentenceEndingPunctuation)
		{
			var fakeScriptProvider = new TestScriptProvider(new ChevronQuotesProject(), simulatedSentenceEndingPunctuation);
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = additionalBreakChars;
			project.ProjectSettings.BreakQuotesIntoBlocks = false;
			Assert.That(((IPublishingInfoProvider)project).BlockBreakCharacters,
				Is.EquivalentTo(string.Join(" ", simulatedSentenceEndingPunctuation).Trim()));
		}

		[Test]
		public void GetIPublishingInfoProvider_BlockBreakCharacters_OnlyExplicitAdditionalCharacters_ReturnsOnlyExplicitAdditionalBreakCharacters()
		{
			var fakeScriptProvider = new TestScriptProvider(new ChevronQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = "^ + @";
			project.ProjectSettings.BreakQuotesIntoBlocks = false;
			Assert.That(((IPublishingInfoProvider)project).BlockBreakCharacters, Is.EqualTo("^ + @"));
		}

		[Test]
		public void GetIPublishingInfoProvider_BlockBreakCharacters_AdditionalCharactersAreWhitespaceCharacters_ReturnsStringWithUnicodeCodepoints()
		{
			var fakeScriptProvider = new TestScriptProvider(new ChevronQuotesProject());
			var project = new Project(fakeScriptProvider);
			var breakingWhitespaceChars = new HashSet<char>(new [] {' ', '\u3000'});
			project.ProjectSettings.AdditionalBlockBreakCharacterSet = new ReadOnlySet<char>(breakingWhitespaceChars);
			project.ProjectSettings.BreakQuotesIntoBlocks = false;
			Assert.That(((IPublishingInfoProvider)project).BlockBreakCharacters, Is.EqualTo("\\s \\u3000"));
		}

		[Test]
		public void GetIPublishingInfoProvider_BlockBreakCharacters_EncounteredAndExplicitAdditionalCharacters_ReturnsEncounteredAndExplicitAdditionalBreakCharacters()
		{
			var fakeScriptProvider = new TestScriptProvider(new ChevronQuotesProject(), '\u1AA8', '\u1AA9', '\u1AAA', '\u1AAB');
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = "^ + @";
			project.ProjectSettings.BreakQuotesIntoBlocks = false;
			Assert.That(((IPublishingInfoProvider)project).BlockBreakCharacters,
				Is.EqualTo("\u1AA8 \u1AA9 \u1AAA \u1AAB ^ + @"));
		}

		[Test]
		public void GetIPublishingInfoProvider_BlockBreakCharacters_ExplicitAdditionalCharactersQuoteBreakWithSameStartAndEndQuote_ReturnsExplicitAdditionalBreakCharactersPlusStartQuote()
		{
			var fakeScriptProvider = new TestScriptProvider(new StraightQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = ";";
			project.ProjectSettings.BreakQuotesIntoBlocks = true;
			Assert.That(((IPublishingInfoProvider)project).BlockBreakCharacters, Is.EqualTo("; \""));
		}

		[TestCase(null)]
		[TestCase("")]
		public void GetIPublishingInfoProvider_BlockBreakCharacters_NoExplicitAdditionalCharactersQuoteBreakWithSameStartAndEndQuote_ReturnsOnlyAndStartQuote(string additionalBreakChars)
		{
			var fakeScriptProvider = new TestScriptProvider(new StraightQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = additionalBreakChars;
			project.ProjectSettings.BreakQuotesIntoBlocks = true;
			Assert.That(((IPublishingInfoProvider)project).BlockBreakCharacters, Is.EqualTo("\""));
		}

		[Test]
		public void GetIPublishingInfoProvider_BlockBreakCharacters_ExplicitAdditionalCharactersQuoteBreakWithDifferentStartAndEndQuote_ReturnsExplicitAdditionalBreakCharactersPlusStartAndEndQuotes()
		{
			var fakeScriptProvider = new TestScriptProvider(new ChevronQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = ";";
			project.ProjectSettings.BreakQuotesIntoBlocks = true;
			Assert.That(((IPublishingInfoProvider)project).BlockBreakCharacters, Is.EqualTo("; << >>"));
		}

		[TestCase(null)]
		[TestCase("")]
		public void GetIPublishingInfoProvider_BlockBreakCharacters_NoExplicitAdditionalCharactersQuoteBreakWithDifferentStartAndEndQuote_ReturnsOnlyStartAndEndQuotes(string additionalBreakChars)
		{
			var fakeScriptProvider = new TestScriptProvider(new CurlyQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = additionalBreakChars;
			project.ProjectSettings.BreakQuotesIntoBlocks = true;
			Assert.That(((IPublishingInfoProvider)project).BlockBreakCharacters, Is.EqualTo("“ ”"));
		}

		[Test]
		public void DeleteClipForSelectedBlock__OriginalScriptLineNotModified()
		{
			var fakeScriptProvider = new TestScriptProvider(new CurlyQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.SelectedBook = project.Books.Last();
			project.SelectedChapterInfo = project.SelectedBook.GetChapter(1);
			var origScriptLine = project.ScriptOfSelectedBlock;
			var origText = origScriptLine.Text;

			try
			{
				using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
				using (var recFile = TempFile.WithFilename(project.GetPathToRecordingForSelectedLine()))
				{
					File.Copy(mono.Path, recFile.Path, true);
					project.HandleSoundFileCreated();
					Assert.That(project.SelectedLineHasClip, Is.True);
					project.DeleteClipForSelectedBlock();
					Assert.That(project.ScriptOfSelectedBlock.Text, Is.EqualTo(origText));
					var deletedLine = project.SelectedChapterInfo.DeletedRecordings.Single();
					Assert.That(deletedLine.Text, Is.Null);
					Assert.That(deletedLine.OriginalText, Is.EqualTo(origText));
					Assert.That(deletedLine, Is.Not.EqualTo(origScriptLine));
					VerifyMiscPropertiesAreEqual(deletedLine, origScriptLine);
				}
			}
			finally
			{
				RobustIO.DeleteDirectoryAndContents(ClipRepository.GetProjectFolder(project.Name));
			}
		}

		private void VerifyMiscPropertiesAreEqual(ScriptLine lineToVerify, ScriptLine otherLine)
		{
			Assert.That(lineToVerify.Number, Is.EqualTo(otherLine.Number));
			Assert.That(lineToVerify.OriginalBlockNumber, Is.EqualTo(otherLine.OriginalBlockNumber));
			Assert.That(lineToVerify.Verse, Is.EqualTo(otherLine.Verse));
			Assert.That(lineToVerify.Actor, Is.EqualTo(otherLine.Actor));
			Assert.That(lineToVerify.Character, Is.EqualTo(otherLine.Character));
			Assert.That(lineToVerify.Bold, Is.EqualTo(otherLine.Bold));
			Assert.That(lineToVerify.Centered, Is.EqualTo(otherLine.Centered));
			Assert.That(lineToVerify.FontName, Is.EqualTo(otherLine.FontName));
			Assert.That(lineToVerify.FontSize, Is.EqualTo(otherLine.FontSize));
			Assert.That(lineToVerify.Heading, Is.EqualTo(otherLine.Heading));
			Assert.That(lineToVerify.HeadingType, Is.EqualTo(otherLine.HeadingType));
			Assert.That(lineToVerify.ParagraphStyle, Is.EqualTo(otherLine.ParagraphStyle));
			Assert.That(lineToVerify.RightToLeft, Is.EqualTo(otherLine.RightToLeft));
			Assert.That(lineToVerify.RecordingTime, Is.EqualTo(otherLine.RecordingTime));
			Assert.That(lineToVerify.Skipped, Is.EqualTo(otherLine.Skipped));
			Assert.That(lineToVerify.ForceHardLineBreakSplitting, Is.EqualTo(otherLine.ForceHardLineBreakSplitting));
		}
	}

	internal class TestScriptProvider : ScriptProviderBase, IScrProjectSettingsProvider
	{
		private readonly FakeVerseInfo _verseInfo;
		private IScripture _scriptureStub;
		private readonly ScriptLine _scriptLineForMat0_Block0 = new ScriptLine
		{
			Number = 1,
			Text = "The intro to Matthew",
			Actor = "Terry",
			Character = "intro-MAT",
			FontName = "Arial Bold",
			FontSize = 28,
			Heading = true,
			HeadingType = "imt"
		};
		private readonly ScriptLine _scriptLineForMat1_Block0 = new ScriptLine
		{
			Number = 1,
			Verse = "1",
			Text = "This is the genealogy of Jesus.",
			Actor = "Marlon",
			Character = "narrator-MAT",
			Bold = true,
			FontName = "Comic Sans",
			FontSize = 22,
			ForceHardLineBreakSplitting = true
		};

		public TestScriptProvider()
		{
			_verseInfo = new FakeVerseInfo();
		}

		public TestScriptProvider(IScrProjectSettings scrProjectSettings = null,
			params char[] simulatedCharactersEncountered)
		{
			ScrProjectSettings = scrProjectSettings;
			_verseInfo = new FakeVerseInfo();
			SetVersionNumberBeforeInitialize();
			Initialize();
			foreach (var c in simulatedCharactersEncountered)
				AddEncounteredSentenceEndingCharacter(c);
		}

		private void SetVersionNumberBeforeInitialize()
		{
			Directory.CreateDirectory(ProjectFolderPath);
			ProjectSettings projectSettings = new ProjectSettings { Version = HearThis.Properties.Settings.Default.CurrentDataVersion};
			XmlSerializationHelper.SerializeToFile(Path.Combine(ProjectFolderPath, kProjectInfoFilename), projectSettings);
		}

		public override ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			if (bookNumber == 1 && chapterNumber == 0 && lineNumber0Based == 0)
				return _scriptLineForMat0_Block0;
			if (bookNumber == 1 && chapterNumber == 1 && lineNumber0Based == 0)
				return _scriptLineForMat1_Block0;
			throw new NotImplementedException();
		}

		public override void UpdateSkipInfo()
		{
			throw new NotImplementedException();
		}

		private readonly int[] matthewBlockCounts = {6, 3, 7, 2};
		private readonly int[] matthewTransCounts = {0, 2, 3, 2};
		public override int GetScriptBlockCount(int bookNumber, int chapter1Based)
		{
			if (bookNumber == 0)
				return 0; // no content in Genesis in this test
			return matthewBlockCounts[chapter1Based];
		}

		public override int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based)
		{
			throw new NotImplementedException();
		}

		public override int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based)
		{
			throw new NotImplementedException();
		}

		public override int GetTranslatedVerseCount(int bookNumberDelegateSafe, int chapterNumber1Based)
		{
			if (bookNumberDelegateSafe == 0)
				return 0;
			return matthewTransCounts[chapterNumber1Based];
		}

		public override int GetScriptBlockCount(int bookNumber)
		{
			throw new NotImplementedException();
		}

		public override void LoadBook(int bookNumber0Based)
		{
		}

		public override string EthnologueCode => throw new NotImplementedException();

		public override bool RightToLeft => throw new NotImplementedException();

		public override string FontName => throw new NotImplementedException();

		public override string ProjectFolderName => "Fake";

		public override IEnumerable<string> AllEncounteredParagraphStyleNames => throw new NotImplementedException();

		public override IBibleStats VersificationInfo => _verseInfo;

		public IScrProjectSettings ScrProjectSettings { get; }

		protected override IStyleInfoProvider StyleInfo
		{
			get
			{
				if (_scriptureStub == null)
					_scriptureStub = new ScriptureStub();
				return _scriptureStub.StyleInfo;
			}
		}
	}

	class FakeVerseInfo : IBibleStats
	{
		public int BookCount => 2;

		private readonly string[] Books = {"Genesis", "Matthew"};
		private readonly int[] chapCounts = {0, 3};

		public int GetBookNumber(string bookName)
		{
			if (bookName == "Genesis")
				return 0;
			if (bookName == "Matthew")
				return 1; // This is a really reduced "fake" canon, consisting of only two books, so Matthew is book 1.
			throw new NotImplementedException();
		}

		public string GetBookCode(int bookNumber0Based)
		{
			switch (bookNumber0Based)
			{
				case 0: return "GEN";
				case 1: return "MAT";
				default: throw new NotImplementedException();
			}
		}

		public string GetBookName(int bookNumber0Based)
		{
			return Books[bookNumber0Based];
		}

		public int GetChaptersInBook(int bookNumber0Based)
		{
			return chapCounts[bookNumber0Based];
		}
	}

	internal class StraightQuotesProject : IScrProjectSettings
	{
		public string FirstLevelStartQuotationMark => "\"";
		public string FirstLevelEndQuotationMark => "\"";
		public string SecondLevelStartQuotationMark => "'";
		public string SecondLevelEndQuotationMark => "'";
		public string ThirdLevelStartQuotationMark => "\"";
		public string ThirdLevelEndQuotationMark => "\"";
		public bool FirstLevelQuotesAreUnique => false;
	}
}
