using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;

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
			Assert.That(infoContent, Is.EqualTo("Genesis;" + Environment.NewLine + "Matthew;1:0,3:2,7:3,2:2" + Environment.NewLine));
		}

		[TestCase(null)]
		[TestCase("")]
		public void GetIPublishingInfoProvider_AdditionalBlockBreakCharacters_NoAdditionalCharacters_ReturnsNullOrEmpty(string additionalBreakChars)
		{
			var fakeScriptProvider = new TestScriptProvider(new ChevronQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = additionalBreakChars;
			project.ProjectSettings.BreakQuotesIntoBlocks = false;
			Assert.That(String.IsNullOrEmpty(((IPublishingInfoProvider)project).AdditionalBlockBreakCharacters));
		}

		[Test]
		public void GetIPublishingInfoProvider_AdditionalBlockBreakCharacters_OnlyExplicitAdditionalCharacters_ReturnsExplicitAdditionalBreakCharacters()
		{
			var fakeScriptProvider = new TestScriptProvider(new ChevronQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = "^ + @";
			project.ProjectSettings.BreakQuotesIntoBlocks = false;
			Assert.AreEqual("^ + @", ((IPublishingInfoProvider)project).AdditionalBlockBreakCharacters);
		}

		[Test]
		public void GetIPublishingInfoProvider_AdditionalBlockBreakCharacters_ExplicitAdditionalCharactersQuoteBreakWithSameStartAndEndQuote_ReturnsExplicitAdditionalBreakCharactersPlusStartQuote()
		{
			var fakeScriptProvider = new TestScriptProvider(new StraightQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = ";";
			project.ProjectSettings.BreakQuotesIntoBlocks = true;
			Assert.AreEqual("; \"", ((IPublishingInfoProvider)project).AdditionalBlockBreakCharacters);
		}

		[TestCase(null)]
		[TestCase("")]
		public void GetIPublishingInfoProvider_AdditionalBlockBreakCharacters_NoExplicitAdditionalCharactersQuoteBreakWithSameStartAndEndQuote_ReturnsOnlyStartQuote(string additionalBreakChars)
		{
			var fakeScriptProvider = new TestScriptProvider(new StraightQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = additionalBreakChars;
			project.ProjectSettings.BreakQuotesIntoBlocks = true;
			Assert.AreEqual("\"", ((IPublishingInfoProvider)project).AdditionalBlockBreakCharacters);
		}

		[Test]
		public void GetIPublishingInfoProvider_AdditionalBlockBreakCharacters_ExplicitAdditionalCharactersQuoteBreakWithDifferentStartAndEndQuote_ReturnsExplicitAdditionalBreakCharactersPlusStartAndEndQuotes()
		{
			var fakeScriptProvider = new TestScriptProvider(new ChevronQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = ";";
			project.ProjectSettings.BreakQuotesIntoBlocks = true;
			Assert.AreEqual("; << >>", ((IPublishingInfoProvider)project).AdditionalBlockBreakCharacters);
		}

		[TestCase(null)]
		[TestCase("")]
		public void GetIPublishingInfoProvider_AdditionalBlockBreakCharacters_NoExplicitAdditionalCharactersQuoteBreakWithDifferentStartAndEndQuote_ReturnsOnlyStartAndEndQuotes(string additionalBreakChars)
		{
			var fakeScriptProvider = new TestScriptProvider(new CurlyQuotesProject());
			var project = new Project(fakeScriptProvider);
			project.ProjectSettings.AdditionalBlockBreakCharacters = additionalBreakChars;
			project.ProjectSettings.BreakQuotesIntoBlocks = true;
			Assert.AreEqual("“ ”", ((IPublishingInfoProvider)project).AdditionalBlockBreakCharacters);
		}
	}

	class TestScriptProvider : ScriptProviderBase, IScrProjectSettingsProvider
	{
		private readonly FakeVerseInfo _verseInfo;

		public TestScriptProvider()
		{
			_verseInfo = new FakeVerseInfo();
		}

		public TestScriptProvider(IScrProjectSettings scrProjectSettings = null)
		{
			ScrProjectSettings = scrProjectSettings;
			_verseInfo = new FakeVerseInfo();
			Initialize();
		}

		public override ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			throw new NotImplementedException();
		}

		public override void UpdateSkipInfo()
		{
			throw new NotImplementedException();
		}

		private int[] matthewBlockCounts = new[] {1, 3, 7, 2};
		private int[] matthewTransCounts = new[] {0, 2, 3, 2};
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

		public override string EthnologueCode
		{
			get { throw new NotImplementedException(); }
		}

		public override bool RightToLeft
		{
			get { throw new NotImplementedException(); }
		}

		public override string FontName
		{
			get { throw new NotImplementedException(); }
		}

		public override string ProjectFolderName
		{
			get { return "Fake"; }
		}

		public override IEnumerable<string> AllEncounteredParagraphStyleNames
		{
			get { throw new NotImplementedException(); }
		}

		public override IBibleStats VersificationInfo
		{
			get { return _verseInfo; }
		}

		public IScrProjectSettings ScrProjectSettings { get; }
	}

	class FakeVerseInfo : IBibleStats
	{
		public int BookCount
		{
			get { return 2; }
		}

		private string[] Books = {"Genesis", "Matthew"};
		private int[] chapCounts = {0, 3};

		public int GetBookNumber(string bookName)
		{
			if (bookName == "Genesis")
				return 0;
			if (bookName == "Matthew")
				return 39;
			throw new NotImplementedException();
		}

		public string GetBookCode(int bookNumber0Based)
		{
			throw new NotImplementedException();
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
