// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2014' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;
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
		private IScripture _scriptureStub;

		public TestScriptProvider()
		{
			_verseInfo = new FakeVerseInfo();
		}

		public TestScriptProvider(IScrProjectSettings scrProjectSettings = null)
		{
			ScrProjectSettings = scrProjectSettings;
			_verseInfo = new FakeVerseInfo();
			SetVersionNumberBeforeInitialize();
			Initialize();
		}

		private void SetVersionNumberBeforeInitialize()
		{
			Directory.CreateDirectory(ProjectFolderPath);
			ProjectSettings projectSettings = new ProjectSettings { Version = HearThis.Properties.Settings.Default.CurrentDataVersion};
			XmlSerializationHelper.SerializeToFile(Path.Combine(ProjectFolderPath, kProjectInfoFilename), projectSettings);
		}

		public override ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
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
