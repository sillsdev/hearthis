using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		public void GetProjectInfoFileContent_ReturnsStringWithChapInfo()
		{
			var fakeScriptProvider = new TestScriptProvider();
			var project = new Project(fakeScriptProvider);
			var infoContent = project.GetProjectInfoFileContent();
			Assert.That(infoContent, Is.EqualTo("Genesis;" + Environment.NewLine + "Matthew;1:0,3:2,7:3,2:2" + Environment.NewLine));
		}
	}

	class TestScriptProvider : ScriptProviderBase
	{
		private FakeVerseInfo _verseInfo;
		public TestScriptProvider()
		{
			_verseInfo = new FakeVerseInfo();
		}
		public override ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
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
}
