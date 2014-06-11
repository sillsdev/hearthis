using System.Collections.Generic;
using NUnit.Framework;
using HearThis.Script;
using Paratext;

namespace HearThisTests
{
	[TestFixture]
	public class ParatextScriptProviderTests
	{
		#region Utility methods

		public List<UsfmToken> CreateTestGenesis()
		{
			var tokens = new List<UsfmToken>();
			tokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			tokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "In the beginning, God.", null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Formless and void.", null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "John's favorite verse.", null));
			return tokens; // Should generate 4 script lines.
		}

		public List<UsfmToken> CreateGenesisWithParagraphBreakInVerse()
		{
			var tokens = new List<UsfmToken>();
			tokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			tokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "In the beginning, God", null));
			tokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "created the heavens and the earth.", null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Formless and void.", null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "John's favorite verse.", null));
			return tokens;
		}

		public List<UsfmToken> CreateGenesisWithEmptyVerse()
		{
			var tokens = new List<UsfmToken>();
			tokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			tokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "In the beginning, God.", null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "John's favorite verse.", null));
			return tokens;
		}

		#endregion

		[Test]
		public void GetScriptLineCountOnUnloadedBookReturnsZero()
		{
			var psp = new ParatextScriptProvider(new ScriptureStub());
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(0));
		}

		[Test]
		public void LoadBookZero()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(4));
		}

		[Test]
		public void LoadBook_ParagraphBreakInVerse()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateGenesisWithParagraphBreakInVerse();
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5));
		}

		[Test]
		public void ScriptLinesHaveCorrectVerses()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateGenesisWithParagraphBreakInVerse();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "2"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "God created a garden.", null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5));
			Assert.That(psp.GetScriptLineCount(0, 2), Is.EqualTo(2));
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
			Assert.That(psp.GetLine(0, 1, 1).Text, Is.EqualTo("In the beginning, God"));
			Assert.That(psp.GetLine(0, 1, 2).Text, Is.EqualTo("created the heavens and the earth."));

			Assert.That(psp.GetLine(0, 2, 0).Text, Is.EqualTo("Chapter 2"));
			Assert.That(psp.GetLine(0, 2, 1).Text, Is.EqualTo("God created a garden."));
			Assert.That(psp.GetLine(0, 1, 0).Verse, Is.EqualTo("0"));
			Assert.That(psp.GetLine(0, 1, 1).Verse, Is.EqualTo("1"));
			Assert.That(psp.GetLine(0, 1, 2).Verse, Is.EqualTo("1"));
			Assert.That(psp.GetLine(0, 1, 3).Verse, Is.EqualTo("2"));
			Assert.That(psp.GetLine(0, 1, 4).Verse, Is.EqualTo("3"));
			Assert.That(psp.GetLine(0, 2, 0).Verse, Is.EqualTo("0"));
			Assert.That(psp.GetLine(0, 2, 1).Verse, Is.EqualTo("1"));

			Assert.That(psp.GetScriptLineCountFromLastParagraph(0, 0), Is.EqualTo(0)); // In reality, this should never get called for chapter 0
			Assert.That(psp.GetScriptLineCountFromLastParagraph(0, 1), Is.EqualTo(3));
			Assert.That(psp.GetScriptLineCountFromLastParagraph(0, 2), Is.EqualTo(1)); // In reality, this should never get called for the last chapter
		}

		[Test]
		public void HandleTextOnIdLine()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Reina Valera", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "mt", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Genesis", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "s", null, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 0), Is.EqualTo(1));
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(1));
			Assert.That(psp.GetLine(0, 0, 0).Text, Is.EqualTo("Genesis"));
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
		}

		[Test]
		public void LoadBook_EmptyVerse()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateGenesisWithEmptyVerse();
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(3));
		}

		[Test]
		public void LoadBook_TwoVersesMergeToOneLineAndIgnoreNote()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "In the beginning, God ", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "created the heavens and the earth.", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Note, "f", null, "f*", "+"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "ft", null, "ft*"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Some next text.", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "f*", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "John's favorite verse.", null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(3));
		}

		[Test]
		public void LoadBook_FootnotesAreNotMadeIntoScriptLines()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Note, "f", null, "f*", "+"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "ft", null, "ft*"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Footnote text here.", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "f*", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " Second half of verse 3 here.", null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5));
		}

		[Test]
		public void LoadBook_CharStyleBkDoesntRemoveSpaces()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "The name ", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "bk", null, "bk*"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Genesis", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "bk*", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " means 'beginnings'.", null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5)); // Assuming the above text gets all on one line.
			Assert.That(psp.GetLine(0, 1, 4).Text, Is.EqualTo("The name Genesis means 'beginnings'."));
		}

		[Test]
		public void LoadBook_CharStyleFigGetsSkipped()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "We will ignore ", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "fig", null, "fig*"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "light|SomePic.jpg|col||2013 Gordon|aawa|1:1", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "fig*", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " the picture.", null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5)); // Assuming the above text gets all on one line.
			Assert.That(psp.GetLine(0, 1, 4).Text, Is.EqualTo("We will ignore the picture."));
		}

		[Test]
		public void LoadBook_EnsureSpacesBetweenSegments()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "4"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Tamatei tol lanu lan mama,", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "5"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "'Ik Petlehem,", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Note, "ft", null, "ft*", "*"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Aupan ikin Jutia.", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "ft*", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " ko kasanien.", null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5)); // Assuming the above text gets all on one line.
			Assert.That(psp.GetLine(0, 1, 4).Text, Is.EqualTo("Tamatei tol lanu lan mama, 'Ik Petlehem, ko kasanien."));
		}

		[Test]
		public void LoadBook_TestThatq1Works()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Quoted text here.", null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5));
		}

		[Test]
		public void LoadBook_TestThatSubsequentChapterWorks()
		{
			const string quoteText = "Quoted text here.";
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, quoteText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(4));
			Assert.That(psp.GetScriptLineCount(0, 2), Is.EqualTo(2));
			// works until 'Chapter' gets localized; should still be the test default
			Assert.That(psp.GetLine(0, 2, 0).Text, Is.EqualTo("Chapter 2"));
			Assert.That(psp.GetLine(0, 2, 1).Text, Is.EqualTo(quoteText));
		}

		[Test]
		public void LoadBook_TestThatSectionsWorks()
		{
			const string verseText = "Verse text here.";
			const string sectionText = "Section heading text";
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "s", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, sectionText, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(4),
				"Chapter 1 should still have 4 script lines.");
			Assert.That(psp.GetScriptLineCount(0, 2), Is.EqualTo(3),
				"Chapter 2 should have 3 script lines.");
			// works until 'Chapter' gets localized; should still be the test default
			Assert.That(psp.GetLine(0, 2, 0).Text, Is.EqualTo("Chapter 2"));
			Assert.That(psp.GetLine(0, 2, 1).Text, Is.EqualTo(sectionText));
			Assert.That(psp.GetLine(0, 2, 2).Text, Is.EqualTo(verseText));
		}

		[Test]
		public void LoadBook_TestThatRemIsIgnored()
		{
			const string verseText = "Verse text here.";
			const string remarkText = "some remark";
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "rem", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, remarkText, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5),
				"Chapter 1 should now have 5 script lines.");
			Assert.That(psp.GetLine(0, 1, 4).Text, Is.EqualTo(verseText));
		}

		[Test]
		public void LoadBook_TestThatIdIsIgnored()
		{
			const string verseText = "Verse text here.";
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(2),
				"'id' should not be counted in the script lines.");
			Assert.That(psp.GetLine(0, 1, 1).Text, Is.EqualTo(verseText));
		}

		[Test]
		public void DontShowIdHeaderOrTOCText()
		{
			const string verseText = "Verse text here.";
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "h", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Header text", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "toc1", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Table of Contents text", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(2),
				"'id', 'h' and 'toc1' should not be counted in the script lines.");
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
			Assert.That(psp.GetLine(0, 1, 1).Text, Is.EqualTo(verseText));
			// But what about chapter 0!?
			Assert.That(psp.GetScriptLineCount(0, 0), Is.EqualTo(0),
				"Shouldn't have any chapter 0 stuff.");
		}

		[Test]
		public void GetNothingForNonExistentBook()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "EXO"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "blah", null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis, which doesn't exist
			Assert.That(psp.GetScriptLineCount(0, 0), Is.EqualTo(0));
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(0));
		}

		[Test]
		public void IntroParagraphsLoadAsChapter0()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "h", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Header text", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "is", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Intro to Genesis", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "ip", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This is nice. It's good, too.", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Whatever.", null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			// Intro
			Assert.That(psp.GetScriptLineCount(0, 0), Is.EqualTo(3));
			Assert.That(psp.GetLine(0, 0, 0).Text, Is.EqualTo("Intro to Genesis"));
			Assert.That(psp.GetLine(0, 0, 1).Text, Is.EqualTo("This is nice."));
			Assert.That(psp.GetLine(0, 0, 2).Text, Is.EqualTo("It's good, too."));

			// Chapter 1
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(2));
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
			Assert.That(psp.GetLine(0, 1, 1).Text, Is.EqualTo("Whatever."));
		}
		[Test]
		public void DontShowParallelPassageReferenceText()
		{
			const string verseText = "Verse text here.";
			const string sectionText = "Section heading text";
			const string parallelRef = "(Mt 3:20)";
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "s", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, sectionText, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "r", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, parallelRef, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(3),
				"'id' and 'r' should not be counted in the script lines.");
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
			Assert.That(psp.GetLine(0, 1, 1).Text, Is.EqualTo(sectionText));
			Assert.That(psp.GetLine(0, 1, 2).Text, Is.EqualTo(verseText));
		}

		[Test]
		public void DontShowInlineReferenceText()
		{
			const string verseText = "Verse text here.";
			const string refText = "Mt 3:20";
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "rq", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, refText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(2),
				"'rq' inline stuff should be stripped from the script lines.");
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
			Assert.That(psp.GetLine(0, 1, 1).Text, Is.EqualTo(verseText));
		}

		[Test]
		public void LoadBook_ClMarkerBeforeFirstChapter()
		{
			const string verseText = "Verse text here.";
			const string verse2Text = "Second verse text.";
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "cl", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Psalm", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verse2Text, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(2),
				"Should be 2 script lines per chapter.");
			Assert.That(psp.GetScriptLineCount(0, 2), Is.EqualTo(2),
				"Should be 2 script lines per chapter.");
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Psalm 1"));
			Assert.That(psp.GetLine(0, 2, 0).Text, Is.EqualTo("Psalm 2"));
			Assert.That(psp.GetLine(0, 2, 1).Text, Is.EqualTo(verse2Text));
		}

		[Test]
		public void LoadBook_ClMarkerAfterChapter()
		{
			const string verseText = "Verse text here.";
			const string psalmTwo = "Psalm Two";
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "cl", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, psalmTwo, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "3"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(2),
				"Should be 2 script lines per chapter.");
			Assert.That(psp.GetScriptLineCount(0, 2), Is.EqualTo(2),
				"Should be 2 script lines per chapter.");
			Assert.That(psp.GetScriptLineCount(0, 3), Is.EqualTo(2),
				"Should be 2 script lines per chapter.");
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
			Assert.That(psp.GetLine(0, 2, 0).Text, Is.EqualTo(psalmTwo));
			// Make sure the next chapter reverts from no \cl marker.
			Assert.That(psp.GetLine(0, 3, 0).Text, Is.EqualTo("Chapter 3"));
		}

		[Test]
		public void LoadBook_CpMarkerAfterChapter()
		{
			const string verseText = "Verse text here.";
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "cp", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "A", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(2),
				"Should be 2 script lines per chapter.");
			Assert.That(psp.GetScriptLineCount(0, 2), Is.EqualTo(2),
				"Should be 2 script lines per chapter.");
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
			// currently PT7's Print Draft function shows BOTH the "A" and the "2" in this situation.
			Assert.That(psp.GetLine(0, 2, 0).Text, Is.EqualTo("Chapter A"));
		}

		[Test]
		public void LoadBook_ClThenCpMarker()
		{
			const string verseText = "Verse text here.";
			const string psalmTwo = "Psalm Two";
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "cl", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, psalmTwo, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "cp", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "B", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(2),
				"Should be 2 script lines per chapter.");
			Assert.That(psp.GetScriptLineCount(0, 2), Is.EqualTo(2),
				"Should be 2 script lines for this chapter.");
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
			Assert.That(psp.GetLine(0, 2, 0).Text, Is.EqualTo(psalmTwo + " B"));
		}

		[Test]
		public void LoadBook_CpThenClMarker()
		{
			const string verseText = "Verse text here.";
			const string psalmTwo = "Psalm Two";
			var stub = new ScriptureStub();
			stub.UsfmTokens = new List<UsfmToken>();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "cp", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "B", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "cl", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, psalmTwo, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(2),
				"Should be 2 script lines per chapter.");
			Assert.That(psp.GetScriptLineCount(0, 2), Is.EqualTo(2),
				"Should be 2 script lines for this chapter.");
			Assert.That(psp.GetLine(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
			Assert.That(psp.GetLine(0, 2, 0).Text, Is.EqualTo(psalmTwo + " B"));
		}

		[Test]
		public void DefaultFontTakenFromScrText()
		{
			var stub = new ScriptureStub();
			stub.SetDefaultFont("MyFont");
			stub.UsfmTokens = CreateTestGenesis();
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetLine(0,1,0).FontName, Is.EqualTo("MyFont"));
		}
	}
}
