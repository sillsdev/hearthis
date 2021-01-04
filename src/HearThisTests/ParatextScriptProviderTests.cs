using HearThis.Script;
using NUnit.Framework;
using Paratext.Data;
using SIL.IO;
using SIL.Xml;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HearThisTests
{
	[TestFixture]
	public class ParatextScriptProviderTests
	{
		#region Utility methods
		private List<UsfmToken> CreateTestGenesis()
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
			return tokens; // Should generate 4 script blocks.
		}

		private List<UsfmToken> CreateGenesisWithParagraphBreakInVerse()
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

		private List<UsfmToken> CreateGenesisWithEmptyVerse()
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

		[TestCase(true)]
		[TestCase(false)]
		public void GetScriptLineCountOnUnloadedBookReturnsZero(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(0));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBookZero(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateTestGenesis();
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(4));
			}
		}

		[TestCase(true, 5)]
		[TestCase(false, 4)]
		public void LoadBook_ParagraphBreakInVerse(bool breakAtParagraphBreaks, int result)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateGenesisWithParagraphBreakInVerse();
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(result));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void AdditionalBlockBreakCharacters(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Reina Valera", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null,
					"Sentence One. Sentence Two$ Sentence Three: Sentence Four?", null));

				ParatextScriptProvider psp;
				var projectSettingsFilePath = Path.Combine(HearThis.Program.GetApplicationDataFolder(stub.Name), ScriptProviderBase.kProjectInfoFilename);

				try
				{
					var projectSettings = new ProjectSettings();
					projectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
					projectSettings.AdditionalBlockBreakCharacters = ": $";
					XmlSerializationHelper.SerializeToFile(projectSettingsFilePath, projectSettings);
					psp = new ParatextScriptProvider(stub);
				}
				finally
				{
					RobustFile.Delete(projectSettingsFilePath);
				}
				psp.LoadBook(0); // load Genesis

				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(5));
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(0, 1, 1).Text, Is.EqualTo("Sentence One."));
				Assert.That(psp.GetBlock(0, 1, 2).Text, Is.EqualTo("Sentence Two$"));
				Assert.That(psp.GetBlock(0, 1, 3).Text, Is.EqualTo("Sentence Three:"));
				Assert.That(psp.GetBlock(0, 1, 4).Text, Is.EqualTo("Sentence Four?"));
			}
		}

		[TestCase(true)]
		public void ScriptLinesHaveCorrectVerses(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateGenesisWithParagraphBreakInVerse();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "2"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "God created a garden.", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(5));
				Assert.That(psp.GetScriptBlockCount(0, 2), Is.EqualTo(2));
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(0, 1, 1).Text, Is.EqualTo("In the beginning, God"));
				Assert.That(psp.GetBlock(0, 1, 2).Text, Is.EqualTo("created the heavens and the earth."));

				Assert.That(psp.GetBlock(0, 2, 0).Text, Is.EqualTo("Chapter 2"));
				Assert.That(psp.GetBlock(0, 2, 1).Text, Is.EqualTo("God created a garden."));
				Assert.That(psp.GetBlock(0, 1, 0).Verse, Is.EqualTo("0"));
				Assert.That(psp.GetBlock(0, 1, 1).Verse, Is.EqualTo("1"));
				Assert.That(psp.GetBlock(0, 1, 2).Verse, Is.EqualTo("1"));
				Assert.That(psp.GetBlock(0, 1, 3).Verse, Is.EqualTo("2"));
				Assert.That(psp.GetBlock(0, 1, 4).Verse, Is.EqualTo("3"));
				Assert.That(psp.GetBlock(0, 2, 0).Verse, Is.EqualTo("0"));
				Assert.That(psp.GetBlock(0, 2, 1).Verse, Is.EqualTo("1"));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void HandleTextOnIdLine(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Reina Valera", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "mt", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Genesis", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "s", null, null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 0), Is.EqualTo(1));
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(1));
				Assert.That(psp.GetBlock(0, 0, 0).Text, Is.EqualTo("Genesis"));
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_EmptyVerse(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateGenesisWithEmptyVerse();
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(3));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_TwoVersesMergeToOneLineAndIgnoreNote(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
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
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(3));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_FootnotesAreNotMadeIntoScriptLines(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateTestGenesis();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Note, "f", null, "f*", "+"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "ft", null, "ft*"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Footnote text here.", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "f*", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " Second half of verse 3 here.", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(5));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_CharStyleBkDoesNotRemoveSpaces(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateTestGenesis();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "The name ", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "bk", null, "bk*"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Genesis", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "bk*", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " means 'beginnings'.", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(5)); // Assuming the above text is a single block.
				Assert.That(psp.GetBlock(0, 1, 4).Text, Is.EqualTo("The name Genesis means 'beginnings'."));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_CharStyleFigGetsSkipped(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateTestGenesis();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "We will ignore ", null));
				NamedAttribute[] figureAttributes = { new NamedAttribute(AttributeName.Reference, "GEN 1:1"), new NamedAttribute(AttributeName.Copyright, "bla") };
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "fig", null, "fig*") { Attributes = figureAttributes });
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Picture caption", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "fig*", null, null) { Attributes = figureAttributes });
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "the picture.", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(5)); // Assuming the above text is a single block.
				Assert.That(psp.GetBlock(0, 1, 4).Text, Is.EqualTo("We will ignore the picture."));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_EnsureSpacesBetweenSegments(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateTestGenesis();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "4"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Tamatei tol lanu lan mama, ", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "5"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "'Ik Petlehem,", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Note, "ft", null, "ft*", "*"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Aupan ikin Jutia.", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "ft*", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " ko kasanien.", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(5)); // Assuming the above text is a single block.
				Assert.That(psp.GetBlock(0, 1, 4).Text, Is.EqualTo("Tamatei tol lanu lan mama, 'Ik Petlehem, ko kasanien."));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_TestThatq1Works(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateTestGenesis();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Quoted text here.", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(5));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_TestThatSubsequentChapterWorks(bool breakAtParagraphBreaks)
		{
			const string quoteText = "Quoted text here.";
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateTestGenesis();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, quoteText, null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(4));
				Assert.That(psp.GetScriptBlockCount(0, 2), Is.EqualTo(2));
				// works until 'Chapter' gets localized; should still be the test default
				Assert.That(psp.GetBlock(0, 2, 0).Text, Is.EqualTo("Chapter 2"));
				Assert.That(psp.GetBlock(0, 2, 1).Text, Is.EqualTo(quoteText));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_TestThatSectionsWorks(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			const string sectionText = "Section heading text";
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateTestGenesis();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "s", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, sectionText, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis

				Debug.WriteLine(psp.ToString());

				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(4),
					"Chapter 1 should still have 4 script blocks.");
				Assert.That(psp.GetScriptBlockCount(0, 2), Is.EqualTo(3),
					"Chapter 2 should have 3 script blocks.");
				// works until 'Chapter' gets localized; should still be the test default
				Assert.That(psp.GetBlock(0, 2, 0).Text, Is.EqualTo("Chapter 2"));
				Assert.That(psp.GetBlock(0, 2, 1).Text, Is.EqualTo(sectionText));
				Assert.That(psp.GetBlock(0, 2, 2).Text, Is.EqualTo(verseText));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_TestThatRemIsIgnored(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			const string remarkText = "some remark";
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = CreateTestGenesis();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "rem", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, remarkText, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(5),
					"Chapter 1 should now have 5 script blocks.");
				Assert.That(psp.GetBlock(0, 1, 4).Text, Is.EqualTo(verseText));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_TestThatIdIsIgnored(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(2),
					"'id' should not be counted in the script blocks.");
				Assert.That(psp.GetBlock(0, 1, 1).Text, Is.EqualTo(verseText));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void DontShowIdHeaderOrTOCText(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "h", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Header text", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "h1", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Header one text", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "h2", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Header two text", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "h3", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Header three text", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "toc1", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "The amazing book of Genesis", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "toc2", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Genesis", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "toc3", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "GEN", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(2),
					"'id', 'h', 'h#', and 'toc#' should not be counted in the script blocks.");
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(0, 1, 1).Text, Is.EqualTo(verseText));
				// But what about chapter 0!?
				Assert.That(psp.GetScriptBlockCount(0, 0), Is.EqualTo(0),
					"Shouldn't have any chapter 0 stuff.");
			}
		}

		[Test]
		public void DontExcludeTransliteratedText()
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "This ", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "tl", null, "tl*"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "word", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "tl*", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, " is transliterated.", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = false;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(2));
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(0, 1, 1).Text, Is.EqualTo("This word is transliterated."));;
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void MultipleUnnestedQuotesInSameParagraph(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "58"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null,
					"Long pepa ia oli raetem wan toktok olsem, “Yu mas aot mo folem wan gudfala rod we yu no save mekem God i kros long yu.” Taem we hem i ridim pepa ia hem i talem long Ivanjelis se, “Be bae mi aot mi go long wanem ples?” (Sam 139:7).",
					null));

				ParatextScriptProvider psp;
				var projectSettingsFilePath = Path.Combine(HearThis.Program.GetApplicationDataFolder(stub.Name), ScriptProviderBase.kProjectInfoFilename);

				try
				{
					var projectSettings = new ProjectSettings();
					projectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
					projectSettings.BreakQuotesIntoBlocks = true;
					XmlSerializationHelper.SerializeToFile(projectSettingsFilePath, projectSettings);
					psp = new ParatextScriptProvider(stub);
				}
				finally
				{
					RobustFile.Delete(projectSettingsFilePath);
				}

				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 2), Is.EqualTo(6));
				Assert.That(psp.GetBlock(0, 2, 0).Text, Is.EqualTo("Chapter 2"));
				Assert.That(psp.GetBlock(0, 2, 1).Text, Is.EqualTo("Long pepa ia oli raetem wan toktok olsem,"));
				Assert.That(psp.GetBlock(0, 2, 2).Text,
					Is.EqualTo("“Yu mas aot mo folem wan gudfala rod we yu no save mekem God i kros long yu.”"));
				Assert.That(psp.GetBlock(0, 2, 3).Text, Is.EqualTo("Taem we hem i ridim pepa ia hem i talem long Ivanjelis se,"));
				Assert.That(psp.GetBlock(0, 2, 4).Text, Is.EqualTo("“Be bae mi aot mi go long wanem ples?”"));
				Assert.That(psp.GetBlock(0, 2, 5).Text, Is.EqualTo("(Sam 139:7)."));
			}
		}

		[Test]
		public void BreakOnParagraphBreakIsFalse_ParagraphsWithoutSentenceEndingPunctuationAreCombined()
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>
				{
					new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "PSA"),
					new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"),
					new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null),
					new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"),
					new UsfmToken(UsfmTokenType.Text, null, "Blessed is the man who doesn’t walk in the counsel of the wicked, ", null),
					new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "nor stand on the path of sinners, ", null),
					new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "nor sit in the seat of scoffers; ", null),
					new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"),
					new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "but his delight is in Yahweh’s law. ", null)
				};
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = false;
				psp.LoadBook(18); // load Psalms
				Assert.That(psp.GetScriptBlockCount(18, 1), Is.EqualTo(2));
				Assert.That(psp.GetBlock(18, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(18, 1, 1).Text, Is.EqualTo("Blessed is the man who doesn’t walk in the counsel of the wicked, nor stand on the path of sinners, nor sit in the seat of scoffers; but his delight is in Yahweh’s law."));
			}
		}

		[Test]
		public void BreakOnParagraphBreakIsFalse_ParagraphsWithSentenceEndingPunctuationAreNotCombined()
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>
				{
					new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "PSA"),
					new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"),
					new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null),
					new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"),
					new UsfmToken(UsfmTokenType.Text, null, "He will be like a tree planted by the streams of water, ", null),
					new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "that produces its fruit in its season, ", null),
					new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "whose leaf also does not wither. ", null),
					new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "Whatever he does shall prosper. ", null),
					new UsfmToken(UsfmTokenType.Verse, "v", null, null, "4"),
					new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "The wicked are not so, ", null)
				};
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = false;
				psp.LoadBook(18); // load Psalms
				Assert.That(psp.GetScriptBlockCount(18, 1), Is.EqualTo(4));
				Assert.That(psp.GetBlock(18, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(18, 1, 1).Text, Is.EqualTo("He will be like a tree planted by the streams of water, that produces its fruit in its season, whose leaf also does not wither."));
				Assert.That(psp.GetBlock(18, 1, 2).Text, Is.EqualTo("Whatever he does shall prosper."));
				Assert.That(psp.GetBlock(18, 1, 3).Text, Is.EqualTo("The wicked are not so,"));
			}
		}

		[Test]
		public void BreakOnParagraphBreakIsTrue_ParagraphsWithoutSentenceEndingPunctuationAreNotCombined()
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>
				{
					new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "PSA"),
					new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"),
					new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null),
					new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"),
					new UsfmToken(UsfmTokenType.Text, null, "Blessed is the man who doesn’t walk in the counsel of the wicked, ", null),
					new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "nor stand on the path of sinners, ", null),
					new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "nor sit in the seat of scoffers; ", null),
					new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"),
					new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "but his delight is in Yahweh’s law. ", null)
				};
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = true;
				psp.LoadBook(18); // load Psalms
				Assert.That(psp.GetScriptBlockCount(18, 1), Is.EqualTo(5));
				Assert.That(psp.GetBlock(18, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(18, 1, 1).Text,
					Is.EqualTo("Blessed is the man who doesn’t walk in the counsel of the wicked,"));
				Assert.That(psp.GetBlock(18, 1, 2).Text, Is.EqualTo("nor stand on the path of sinners,"));
				Assert.That(psp.GetBlock(18, 1, 3).Text, Is.EqualTo("nor sit in the seat of scoffers;"));
				Assert.That(psp.GetBlock(18, 1, 4).Text, Is.EqualTo("but his delight is in Yahweh’s law."));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GetNothingForNonExistentBook(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "EXO"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "blah", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis, which doesn't exist
				Assert.That(psp.GetScriptBlockCount(0, 0), Is.EqualTo(0));
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(0));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void IntroParagraphsLoadAsChapter0(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
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
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				// Intro
				Assert.That(psp.GetScriptBlockCount(0, 0), Is.EqualTo(3));
				Assert.That(psp.GetBlock(0, 0, 0).Text, Is.EqualTo("Intro to Genesis"));
				Assert.That(psp.GetBlock(0, 0, 1).Text, Is.EqualTo("This is nice."));
				Assert.That(psp.GetBlock(0, 0, 2).Text, Is.EqualTo("It's good, too."));

				// Chapter 1
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(2));
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(0, 1, 1).Text, Is.EqualTo("Whatever."));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void IncludeParallelPassageReferenceText(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			const string sectionText = "Section heading text";
			const string parallelRef = "(Mt 3:20)";
			using (var stub = new ScriptureStub())
			{
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
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(4),
					"'id' should not be counted in the script blocks.");
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(0, 1, 1).Text, Is.EqualTo(sectionText));
				Assert.That(psp.GetBlock(0, 1, 2).Text, Is.EqualTo(parallelRef));
				Assert.That(psp.GetBlock(0, 1, 3).Text, Is.EqualTo(verseText));
				Assert.That(psp.GetSkippedScriptBlockCount(0, 1), Is.EqualTo(1),
					"The parallel passage block should be skipped by default");
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void DontShowInlineReferenceText(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			const string refText = "Mt 3:20";
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "rq", null, "rq*"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, refText, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "*rq", null, null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(2),
					"'rq' inline stuff should be stripped from the script blocks.");
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(0, 1, 1).Text, Is.EqualTo(verseText));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_WordInTextDifferentFromWordInGlossary_OnlyWordInTextIncludedInScript(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "id", null, null, "GAL"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "5"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "22"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null,
					"But the fruit of the Spirit is love, joy, peace, patience, kindness, goodness, ", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "w", null, "w*"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "faithfulness|faith", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "w*", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, ", ", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "23"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "gent|eness, ", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "w", null, "w*"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "self-control", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "w*", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "; against such things there is no |aw. ", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(47); // load Galatians
				Assert.That(psp.GetScriptBlockCount(47, 5), Is.EqualTo(2));
				Assert.That(psp.GetBlock(47, 5, 0).Text, Is.EqualTo("Chapter 5"));
				Assert.That(psp.GetBlock(47, 5, 1).Text,
					Is.EqualTo(
						"But the fruit of the Spirit is love, joy, peace, patience, kindness, goodness, faithfulness, gent|eness, self-control; against such things there is no |aw."));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_ClMarkerBeforeFirstChapter(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			const string verse2Text = "Second verse text.";
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "cl", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Psalm ", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verseText, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "2"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, verse2Text, null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetScriptBlockCount(0, 2), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Psalm 1"));
				Assert.That(psp.GetBlock(0, 2, 0).Text, Is.EqualTo("Psalm 2"));
				Assert.That(psp.GetBlock(0, 2, 1).Text, Is.EqualTo(verse2Text));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_ClMarkerAfterChapter(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			const string psalmTwo = "Psalm Two";
			using (var stub = new ScriptureStub())
			{
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
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetScriptBlockCount(0, 2), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetScriptBlockCount(0, 3), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(0, 2, 0).Text, Is.EqualTo(psalmTwo));
				// Make sure the next chapter reverts from no \cl marker.
				Assert.That(psp.GetBlock(0, 3, 0).Text, Is.EqualTo("Chapter 3"));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_CpMarkerAfterChapter(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			using (var stub = new ScriptureStub())
			{
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
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetScriptBlockCount(0, 2), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				// currently PT7's Print Draft function shows BOTH the "A" and the "2" in this situation.
				Assert.That(psp.GetBlock(0, 2, 0).Text, Is.EqualTo("Chapter A"));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_ClThenCpMarker(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			const string psalmTwo = "Psalm Two ";
			using (var stub = new ScriptureStub())
			{
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
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetScriptBlockCount(0, 2), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(0, 2, 0).Text, Is.EqualTo(psalmTwo + "B"));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LoadBook_CpThenClMarker(bool breakAtParagraphBreaks)
		{
			const string verseText = "Verse text here.";
			const string psalmTwo = "Psalm Two ";
			using (var stub = new ScriptureStub())
			{
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
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetScriptBlockCount(0, 1), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetScriptBlockCount(0, 2), Is.EqualTo(2),
					"Should be 2 script blocks for this chapter.");
				Assert.That(psp.GetBlock(0, 1, 0).Text, Is.EqualTo("Chapter 1"));
				Assert.That(psp.GetBlock(0, 2, 0).Text, Is.EqualTo(psalmTwo + "B"));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void SkippedBlockPersistedWhenReloaded(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "verse 1 text will be skipped. ", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "verse 2 text.", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				psp.GetBlock(0, 1, 1).Skipped = true;
				psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.IsFalse(psp.GetBlock(0, 1, 0).Skipped);
				Assert.IsTrue(psp.GetBlock(0, 1, 1).Skipped);
				Assert.IsFalse(psp.GetBlock(0, 1, 2).Skipped);
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void UnskippedBlockPersistedWhenReloaded(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "verse 1 text will be skipped. ", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "verse 2 text.", null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				psp.GetBlock(0, 1, 1).Skipped = true;
				psp.GetBlock(0, 1, 1).Skipped = false;
				psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.IsFalse(psp.GetBlock(0, 1, 0).Skipped);
				Assert.IsFalse(psp.GetBlock(0, 1, 1).Skipped);
				Assert.IsFalse(psp.GetBlock(0, 1, 2).Skipped);
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void SkippedStylePersistedWhenReloaded(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "GEN"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "s", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "The Beginning", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null,
					"In the beginning God created the heavens and the earth.", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null,
					"And the earth was formless and void and the Spirit of God moved over the face of the waters.", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "s", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "First day of creation", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "p", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null,
					"And God said, \"Let there be light,\" and there was light.", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "4"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null,
					"God saw that the light was good, and He separated the light from the darkness.", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "5"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null,
					"God called the light \"day\" and the darkness \"night\". And there was evening and morning - the first day.",
					null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				psp.GetBlock(0, 1, 1).SkipAllBlocksOfThisStyle(true); // section head
				psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis

				Debug.WriteLine(psp);

				Assert.IsFalse(psp.GetBlock(0, 1, 0).Skipped); // chapter number
				Assert.IsTrue(psp.GetBlock(0, 1, 1).Skipped); // section head
				Assert.IsFalse(psp.GetBlock(0, 1, 2).Skipped); // verse 1
				Assert.IsFalse(psp.GetBlock(0, 1, 3).Skipped); // verse 2
				Assert.AreEqual("s - Heading - Section Level 1", psp.GetBlock(0, 1, 4).ParagraphStyle);
				Assert.IsTrue(psp.GetBlock(0, 1, 4).Skipped); // section head
				Assert.IsFalse(psp.GetBlock(0, 1, 5).Skipped); // verse 3
				Assert.IsFalse(psp.GetBlock(0, 1, 6).Skipped); // verse 4
				Assert.IsFalse(psp.GetBlock(0, 1, 7).Skipped); // verse 5a
				Assert.IsFalse(psp.GetBlock(0, 1, 8).Skipped); // verse 5b
				// Now test un-skipping
				psp.GetBlock(0, 1, 4).SkipAllBlocksOfThisStyle(false); // section head
				psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.IsFalse(psp.GetBlock(0, 1, 0).Skipped); // chapter number
				Assert.IsFalse(psp.GetBlock(0, 1, 1).Skipped); // section head
				Assert.IsFalse(psp.GetBlock(0, 1, 2).Skipped); // verse 1
				Assert.IsFalse(psp.GetBlock(0, 1, 3).Skipped); // verse 2
				Assert.IsFalse(psp.GetBlock(0, 1, 4).Skipped); // section head
				Assert.IsFalse(psp.GetBlock(0, 1, 5).Skipped); // verse 3
				Assert.IsFalse(psp.GetBlock(0, 1, 6).Skipped); // verse 4
				Assert.IsFalse(psp.GetBlock(0, 1, 7).Skipped); // verse 5a
				Assert.IsFalse(psp.GetBlock(0, 1, 8).Skipped); // verse 5b
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void SelahTreatedAsParagraphStyle(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>();
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Book, "id", null, null, "PSA"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "3"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Verse 1, line 1.", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Verse 1, line 2.", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "2"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Verse 2, line 1", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Verse 2, line 2.", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "qs", null, "qs*"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Selah", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "qs*", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Verse, "v", null, null, "3"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Verse 3, line 1", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Character, "qs", null, "qs*"));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.Text, null, "Selah", null));
				stub.UsfmTokens.Add(new UsfmToken(UsfmTokenType.End, "qs*", null, null));
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(18); // load Psalms

				Debug.WriteLine(psp);

				var selah = psp.GetBlock(18, 3, breakAtParagraphBreaks ? 5 : 4);
				Assert.AreEqual("Selah", selah.Text);
				Assert.IsTrue(selah.ParagraphStyle.StartsWith("qs...qs*"));
				Assert.IsTrue(psp.AllEncounteredParagraphStyleNames.Any(s => s.StartsWith("qs...qs*")));
				selah.SkipAllBlocksOfThisStyle(true); // selah
				psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(18); // load Psalms

				Debug.WriteLine(psp);

				if (breakAtParagraphBreaks)
				{
					Assert.IsTrue(psp.GetBlock(18, 3, 5).Skipped); // selah in verse 2
					Assert.IsTrue(psp.GetBlock(18, 3, 7).Skipped); // selah in verse 3
				}
				else
				{
					Assert.IsTrue(psp.GetBlock(18, 3, 4).Skipped); // selah in verse 2
					Assert.IsTrue(psp.GetBlock(18, 3, 6).Skipped); // selah in verse 3
				}
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void HebrewTitleTreatedAsSeparateBlockRegardlessOfPunctuation(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = new List<UsfmToken>
				{
					new UsfmToken(UsfmTokenType.Book, "id", null, null, "PSA"),
					new UsfmToken(UsfmTokenType.Paragraph, "c", null, null, "119"),
					new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null),
					new UsfmToken(UsfmTokenType.Verse, "v", null, null, "8"),
					new UsfmToken(UsfmTokenType.Text, null, "I will observe your statutes. ", null),
					new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "Don’t utterly forsake me. ", null),
					new UsfmToken(UsfmTokenType.Paragraph, "d", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "BET", null),
					new UsfmToken(UsfmTokenType.Verse, "v", null, null, "9"),
					new UsfmToken(UsfmTokenType.Paragraph, "q1", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "How can a young man keep his way pure? ", null),
					new UsfmToken(UsfmTokenType.Paragraph, "q2", null, null),
					new UsfmToken(UsfmTokenType.Text, null, "By living according to your word. ", null)
				};
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(18); // load Psalms

				Debug.WriteLine(psp);

				Assert.That(psp.GetScriptBlockCount(18, 119), Is.EqualTo(6));
				Assert.That(psp.GetBlock(18, 119, 0).Text, Is.EqualTo("Chapter 119"));
				Assert.That(psp.GetBlock(18, 119, 1).Text, Is.EqualTo("I will observe your statutes."));
				Assert.That(psp.GetBlock(18, 119, 2).Text, Is.EqualTo("Don’t utterly forsake me."));
				Assert.That(psp.GetBlock(18, 119, 3).Text, Is.EqualTo("BET"));
				Assert.That(psp.GetBlock(18, 119, 4).Text, Is.EqualTo("How can a young man keep his way pure?"));
				Assert.That(psp.GetBlock(18, 119, 5).Text, Is.EqualTo("By living according to your word."));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void DefaultFontTakenFromScrText(bool breakAtParagraphBreaks)
		{
			using (var stub = new ScriptureStub())
			{
				stub.SetDefaultFont("MyFont");
				stub.UsfmTokens = CreateTestGenesis();
				var psp = new ParatextScriptProvider(stub);
				psp.ProjectSettings.BreakAtParagraphBreaks = breakAtParagraphBreaks;
				psp.LoadBook(0); // load Genesis
				Assert.That(psp.GetBlock(0, 1, 0).FontName, Is.EqualTo("MyFont"));
			}
		}
	}
}
