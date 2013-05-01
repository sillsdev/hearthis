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
			tokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "In the beginning, God.", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Formless and void.", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "John's favorite verse.", null, null));
			return tokens; // Should generate 4 script lines.
		}

		public List<UsfmToken> CreateGenesisWithParagraphBreakInVerse()
		{
			var tokens = new List<UsfmToken>();
			tokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			tokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "In the beginning, God", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "created the heavens and the earth.", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Formless and void.", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "John's favorite verse.", null, null));
			return tokens;
		}

		public List<UsfmToken> CreateGenesisWithEmptyVerse()
		{
			var tokens = new List<UsfmToken>();
			tokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			tokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "In the beginning, God.", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "John's favorite verse.", null, null));
			return tokens;
		}

		public List<UsfmToken> CreateGenesisWithNotes()
		{
			var tokens = new List<UsfmToken>();
			tokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
			tokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "In the beginning, God ", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "created the heavens and the earth.", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Note, "nt", null, "nt*", null));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Some next text.", null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Note, "nt*", null, null, null));
			tokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
			tokens.Add(new UsfmToken(UsfmTokenType.Text, null, "John's favorite verse.", null, null));
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
			stub.UsfmTokens = CreateGenesisWithNotes();
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(3));
		}

		[Test]
		public void LoadBook_FootnotesAreNotMadeIntoScriptLines()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Note, "ft", null, "ft*", "*"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Footnote text here.", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Note, "ft*", null, null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " Second half of verse 3 here.", null, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5));
		}

		[Test]
		public void LoadBook_CharStyleBkDoesntRemoveSpaces()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "The name ", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "bk", null, "bk*", null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Genesis", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Note, "bk*", null, null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " means 'beginnings'.", null, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5)); // Assuming the above text gets all on one line.
			Assert.That(psp.GetLine(0, 1, 4).Text, Is.EqualTo("The name Genesis means 'beginnings'."));
		}

		[Test]
		public void LoadBook_EnsureSpacesBetweenSegments()
		{
			var stub = new ScriptureStub();
			stub.UsfmTokens = CreateTestGenesis();
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "4"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Tamatei tol lanu lan mama,", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "5"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "'Ik Petlehem,", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Note, "ft", null, "ft*", "*"));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Aupan ikin Jutia.", null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Note, "ft*", null, null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " ko kasanien.", null, null));
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
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null, null));
			stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Quoted text here.", null, null));
			var psp = new ParatextScriptProvider(stub);
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(5));
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
