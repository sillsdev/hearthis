using System;
using System.Diagnostics;
using System.Linq;
using HearThis.Script;
using NUnit.Framework;
using SIL.Unicode;

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
			Assert.AreEqual(0, splitter.BreakIntoChunks(String.Empty).Count());
		}

		[Test]
		public void BreakIntoChunks_SinglePeriod_YieldsSingleChunkWithPeriod()
		{
			var splitter = new SentenceClauseSplitter(null);
			var result = splitter.BreakIntoChunks(".").ToList();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(".", result[0].Text);
		}

		[Test]
		public void BreakIntoChunks_TwoSimpleSentences_YieldsTwoChunks()
		{
			var splitter = new SentenceClauseSplitter(null);
			var result = splitter.BreakIntoChunks("This is a cat. Why is it here?").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("This is a cat.", result[0].Text);
			Assert.AreEqual("Why is it here?", result[1].Text);
		}

		[Test]
		public void BreakIntoChunks_DoNotBreakQuotations_YieldsChunksWithIncludedQuoteMarks()
		{
			var splitter = new SentenceClauseSplitter(null);
			var result = splitter.BreakIntoChunks("“I'm pretty happy,” said John. “Me, too,” mumbled Alice.").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("“I'm pretty happy,” said John.", result[0].Text);
			Assert.AreEqual("“Me, too,” mumbled Alice.", result[1].Text);
		}

		[Test]
		public void BreakIntoChunks_BreakQuotations_YieldsQuotationsAsSeparateChunks()
		{
			var splitter = new SentenceClauseSplitter(null, true, new CurlyQuotesProject());
			var result = splitter.BreakIntoChunks("“I'm pretty happy,” said John. “Me, too,” mumbled Alice.").ToList();
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual("“I'm pretty happy,”", result[0].Text);
			Assert.AreEqual("said John.", result[1].Text);
			Assert.AreEqual("“Me, too,”", result[2].Text);
			Assert.AreEqual("mumbled Alice.", result[3].Text);
		}

		[Test]
		public void BreakIntoChunks_DoNotBreakSentencesWithQuotationsContainingQuestionsAndExclamations_YieldsChunksWithIncludedQuoteMarks()
		{
			var splitter = new SentenceClauseSplitter(null);
			var result = splitter.BreakIntoChunks("“Do you want to go to the zoo?” asked John. “No way!” shouted the twins.").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("“Do you want to go to the zoo?” asked John.", result[0].Text);
			Assert.AreEqual("“No way!” shouted the twins.", result[1].Text);
		}

		[Test]
		public void BreakIntoChunks_BreakSentencesWithQuotationsContainingQuestionsAndExclamations_YieldsChunksWithIncludedQuoteMarks()
		{
			var splitter = new SentenceClauseSplitter(null, true, new CurlyQuotesProject());
			var result = splitter.BreakIntoChunks("“Do you want to go to the zoo?” asked John. “No way!” shouted the twins.").ToList();
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual("“Do you want to go to the zoo?”", result[0].Text);
			Assert.AreEqual("asked John.", result[1].Text);
			Assert.AreEqual("“No way!”", result[2].Text);
			Assert.AreEqual("shouted the twins.", result[3].Text);
		}

		[Test]
		public void BreakIntoChunks_BreakSentencesWithNestedQuotations_YieldsChunksForTopLevelQuotesOnly()
		{
			var splitter = new SentenceClauseSplitter(null, true, new CurlyQuotesProject());
			var result = splitter.BreakIntoChunks("Then God said, “Do not say, ‘Why did the Lord say, “You have sinned,” when we did what was right in our own eyes,’ or I will pluck you from this good land and hurl you into the desert!”").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("Then God said,", result[0].Text);
			Assert.AreEqual("“Do not say, ‘Why did the Lord say, “You have sinned,” when we did what was right in our own eyes,’ or I will pluck you from this good land and hurl you into the desert!”", result[1].Text);
		}

		[Test]
		public void BreakIntoChunks_BreakSentencesWithMultiCharacterQuotationMarks_YieldsChunksWithIncludedQuoteMarks()
		{
			var splitter = new SentenceClauseSplitter(null, true, new ChevronQuotesProject());
			var result = splitter.BreakIntoChunks("Then God said, <<Do not say, <Why did the Lord say, <<You have sinned,>> when we did what was right in our own eyes,> or I will pluck you from this good land.>>").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("Then God said,", result[0].Text);
			Assert.AreEqual("<<Do not say, <Why did the Lord say, <<You have sinned,>> when we did what was right in our own eyes,> or I will pluck you from this good land.>>", result[1].Text);
		}

		[Test]
		public void BreakIntoChunks_MultisentenceQuotes_YieldsChunksForIndividualSentencesWithinQuotation()
		{
			var splitter = new SentenceClauseSplitter(null, true, new ChevronQuotesProject());
			var result = splitter.BreakIntoChunks("<<This is fine. This is nice. This is good,>> said Fred.").ToList();
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual("<<This is fine.", result[0].Text);
			Assert.AreEqual("This is nice.", result[1].Text);
			Assert.AreEqual("This is good,>>", result[2].Text);
			Assert.AreEqual("said Fred.", result[3].Text);
		}

		[Test]
		public void BreakIntoChunks_ParagraphStartingInMiddleOfQuoteWithNoOpeningQuotes_YieldsChunksForIndividualSentencesWithinQuotation()
		{
			var splitter = new SentenceClauseSplitter(null, true, new CurlyQuotesProject());
			var result = splitter.BreakIntoChunks("This is fine.” Let's go fishing.").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("This is fine.”", result[0].Text);
			Assert.AreEqual("Let's go fishing.", result[1].Text);
		}

		[Category("SkipOnTeamCity")]
		[Test]
		public void BreakIntoChunks_SpeedTest()
		{
			var splitter = new SentenceClauseSplitter(null);
			const string textToBreak = "This is a sentence. So is this. Why not? Go for it! Is this a little question\uFE56 What\u203D hoo-rah\u2047 ";
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < 5000; i++)
				splitter.BreakIntoChunks(textToBreak).ToList();
			stopwatch.Stop();
			Debug.WriteLine("Elapsed milliseconds: " + stopwatch.ElapsedMilliseconds);
			Assert.Less(stopwatch.ElapsedMilliseconds, 30);
		}
	}

	internal class CurlyQuotesProject : IScrProjectSettings
	{
		public string FirstLevelStartQuotationMark { get { return "“"; } }
		public string FirstLevelEndQuotationMark { get { return "”"; } }
		public string SecondLevelStartQuotationMark { get { return "‘"; } }
		public string SecondLevelEndQuotationMark { get { return "’"; } }
		public string ThirdLevelStartQuotationMark { get { return "“"; } }
		public string ThirdLevelEndQuotationMark { get { return "”"; } }
		public bool FirstLevelQuotesAreUnique { get { return false; } }
	}

	internal class ChevronQuotesProject : IScrProjectSettings
	{
		public string FirstLevelStartQuotationMark { get { return "<<"; } }
		public string FirstLevelEndQuotationMark { get { return ">>"; } }
		public string SecondLevelStartQuotationMark { get { return "<"; } }
		public string SecondLevelEndQuotationMark { get { return ">"; } }
		public string ThirdLevelStartQuotationMark { get { return "<<"; } }
		public string ThirdLevelEndQuotationMark { get { return ">>"; } }
		public bool FirstLevelQuotesAreUnique { get { return false; } }
	}
}
