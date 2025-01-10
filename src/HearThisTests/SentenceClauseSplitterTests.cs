using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HearThis.Script;
using NUnit.Framework;

namespace HearThisTests
{
	/// <summary>
	/// Test the tricks we do to limit preceding context when short of space
	/// </summary>
	[TestFixture]
	public class SentenceClauseSplitterTests
	{
		[Test]
		public void BreakIntoChunks_EmptyString_YieldsNothing()
		{
			var splitter = new SentenceClauseSplitter(null);
			Assert.That(splitter.BreakIntoChunks(String.Empty), Is.Empty);
		}

		[Test]
		public void BreakIntoChunks_SinglePeriod_YieldsSingleChunkWithPeriod()
		{
			var splitter = new SentenceClauseSplitter(null);
			var result = splitter.BreakIntoChunks(".");
			Assert.That(result.Single().Text, Is.EqualTo("."));
		}

		[Test]
		public void BreakIntoChunks_TwoSimpleSentences_YieldsTwoChunks()
		{
			var charactersEncountered = new HashSet<char>(1);
			var splitter = new SentenceClauseSplitter(null);
			splitter.SentenceFinalPunctuationEncountered += delegate(SentenceClauseSplitter sender, char character)
			{
				Assert.That(sender, Is.EqualTo(splitter));
				charactersEncountered.Add(character);
			};
			var result = splitter.BreakIntoChunks("This is a cat. Why is it here?");
			Assert.That(result.Select(c => c.Text), Is.EqualTo(new []
			{
				"This is a cat.",
				"Why is it here?"
			}));
			Assert.That(charactersEncountered, Is.EquivalentTo(new [] { '.', '?'}));
		}

		[Test]
		public void BreakIntoChunks_DoNotBreakQuotations_YieldsChunksWithIncludedQuoteMarks()
		{
			var charactersEncountered = new HashSet<char>(1);
			var splitter = new SentenceClauseSplitter(null);
			splitter.SentenceFinalPunctuationEncountered += delegate(SentenceClauseSplitter sender, char character)
			{
				Assert.That(sender, Is.EqualTo(splitter));
				charactersEncountered.Add(character);
			};
			var result = splitter.BreakIntoChunks("“I'm pretty happy,” said John. “Me, too,” mumbled Alice.");
			Assert.That(result.Select(c => c.Text), Is.EqualTo(new []
			{
				"“I'm pretty happy,” said John.",
				"“Me, too,” mumbled Alice."
			}));
			Assert.That(charactersEncountered, Is.EquivalentTo(new [] { '.'}));
		}

		[Test]
		public void BreakIntoChunks_BreakQuotations_YieldsQuotationsAsSeparateChunks()
		{
			var charactersEncountered = new HashSet<char>(1);
			var splitter = new SentenceClauseSplitter(null, true, new CurlyQuotesProject());
			splitter.SentenceFinalPunctuationEncountered += delegate(SentenceClauseSplitter sender, char character)
				{
					Assert.That(sender, Is.EqualTo(splitter));
					charactersEncountered.Add(character);
				};
			var result = splitter.BreakIntoChunks("“I'm pretty happy,” said John. “Me, too,” mumbled Alice.");
			Assert.That(result.Select(c => c.Text), Is.EqualTo(new []
			{
				"“I'm pretty happy,”",
				"said John.",
				"“Me, too,”",
				"mumbled Alice."
			}));
			Assert.That(charactersEncountered, Is.EquivalentTo(new [] { '.'}));
		}

		[Test]
		public void BreakIntoChunks_DoNotBreakSentencesWithQuotationsContainingQuestionsAndExclamations_YieldsChunksWithIncludedQuoteMarks()
		{
			var charactersEncountered = new HashSet<char>(1);
			var splitter = new SentenceClauseSplitter(null);
			splitter.SentenceFinalPunctuationEncountered += delegate(SentenceClauseSplitter sender, char character)
			{
				Assert.That(sender, Is.EqualTo(splitter));
				charactersEncountered.Add(character);
			};
			var result = splitter.BreakIntoChunks("“Do you want to go to the zoo?” asked John. “No way!” shouted the twins.");
			Assert.That(result.Select(c => c.Text), Is.EqualTo(new []
			{
				"“Do you want to go to the zoo?” asked John.",
				"“No way!” shouted the twins."
			}));
			Assert.That(charactersEncountered, Is.EquivalentTo(new [] { '.'}));
		}

		[Test]
		public void BreakIntoChunks_BreakSentencesWithQuotationsContainingQuestionsAndExclamations_YieldsChunksWithIncludedQuoteMarks()
		{
			var charactersEncountered = new HashSet<char>(1);
			var splitter = new SentenceClauseSplitter(null, true, new CurlyQuotesProject());
			splitter.SentenceFinalPunctuationEncountered += delegate(SentenceClauseSplitter sender, char character)
			{
				Assert.That(sender, Is.EqualTo(splitter));
				charactersEncountered.Add(character);
			};
			var result = splitter.BreakIntoChunks("“Do you want to go to the zoo?” asked John. “No way!” shouted the twins.");
			Assert.That(result.Select(c => c.Text), Is.EqualTo(new []
			{
				"“Do you want to go to the zoo?”",
				"asked John.",
				"“No way!”",
				"shouted the twins."
			}));

			Assert.That(charactersEncountered, Is.EquivalentTo(new [] { '.'}),
				"The question mark and exclamation mark should not be in this list because they" +
				" are inside quotes that are not sentence-ending.");
		}

		[Test]
		public void BreakIntoChunks_BreakSentencesWithNestedQuotations_YieldsChunksForTopLevelQuotesOnly()
		{
			var splitter = new SentenceClauseSplitter(null, true, new CurlyQuotesProject());
			var result = splitter.BreakIntoChunks("Then God said, “Do not say, ‘Why did the Lord say, “You have sinned,” when we did what was right in our own eyes,’ or I will pluck you from this good land and hurl you into the desert!”");
			Assert.That(result.Select(c => c.Text), Is.EqualTo(new []
			{
				"Then God said,",
				"“Do not say, ‘Why did the Lord say, “You have sinned,” when we did what was right in our own eyes,’ or I will pluck you from this good land and hurl you into the desert!”"
			}));
		}

		[Test]
		public void BreakIntoChunks_BreakSentencesWithMultiCharacterQuotationMarks_YieldsChunksWithIncludedQuoteMarks()
		{
			var splitter = new SentenceClauseSplitter(null, true, new ChevronQuotesProject());
			var result = splitter.BreakIntoChunks("Then God said, <<Do not say, <Why did the Lord say, <<You have sinned,>> when we did what was right in our own eyes,> or I will pluck you from this good land.>>");
			Assert.That(result.Select(c => c.Text), Is.EqualTo(new []
			{
				"Then God said,",
				"<<Do not say, <Why did the Lord say, <<You have sinned,>> when we did what was right in our own eyes,> or I will pluck you from this good land.>>"
			}));
		}

		[Test]
		public void BreakIntoChunks_MultiSentenceQuotes_YieldsChunksForIndividualSentencesWithinQuotation()
		{
			var splitter = new SentenceClauseSplitter(null, true, new ChevronQuotesProject());
			var result = splitter.BreakIntoChunks("<<This is fine. This is nice. This is good,>> said Fred.");
			Assert.That(result.Select(c => c.Text), Is.EqualTo(new []
			{
				"<<This is fine.",
				"This is nice.",
				"This is good,>>",
				"said Fred."
			}));
		}

		[Test]
		public void BreakIntoChunks_ParagraphStartingInMiddleOfQuoteWithNoOpeningQuotes_YieldsChunksForIndividualSentencesWithinQuotation()
		{
			var splitter = new SentenceClauseSplitter(null, true, new CurlyQuotesProject());
			var result = splitter.BreakIntoChunks("This is fine.” Let's go fishing.");
			Assert.That(result.Select(c => c.Text), Is.EqualTo(new []
			{
				"This is fine.”",
				"Let's go fishing."
			}));
		}

		[Category("SkipOnTeamCity")]
		[Test]
		public void BreakIntoChunks_SpeedTest()
		{
			var splitter = new SentenceClauseSplitter(null);
			const string textToBreak = "This is a sentence. So is this. Why not? Go for it! Is this a little question\uFE56 What\u203D hoo-rah\u2047 ";
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < 5000; i++)
				splitter.BreakIntoChunks(textToBreak).ToList();
			stopwatch.Stop();
			Debug.WriteLine("Elapsed milliseconds: " + stopwatch.ElapsedMilliseconds);
			Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(30));
		}
	}

	internal class CurlyQuotesProject : IScrProjectSettings
	{
		public string FirstLevelStartQuotationMark => "“";
		public string FirstLevelEndQuotationMark => "”";
		public string SecondLevelStartQuotationMark => "‘";
		public string SecondLevelEndQuotationMark => "’";
		public string ThirdLevelStartQuotationMark => "“";
		public string ThirdLevelEndQuotationMark => "”";
		public bool FirstLevelQuotesAreUnique => false;
	}

	internal class ChevronQuotesProject : IScrProjectSettings
	{
		public string FirstLevelStartQuotationMark => "<<";
		public string FirstLevelEndQuotationMark => ">>";
		public string SecondLevelStartQuotationMark => "<";
		public string SecondLevelEndQuotationMark => ">";
		public string ThirdLevelStartQuotationMark => "<<";
		public string ThirdLevelEndQuotationMark => ">>";
		public bool FirstLevelQuotesAreUnique => false;
	}
}
