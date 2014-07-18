using System.Linq;
using HearThis.Script;
using NUnit.Framework;
using Paratext;

namespace HearThisTests
{
	public class ParatextParagraphTests
	{
		[Test]
		public void NewInstance_HasNoText()
		{
			Assert.That(new ParatextParagraph().HasData, Is.False);
		}

		[Test]
		public void AddingText_MakesHasDataTrue()
		{
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag();
			var pp = new ParatextParagraph();
			pp.StartNewParagraph(stateStub, true);
			Assert.That(pp.HasData, Is.False);
			pp.Add("this is text");
			Assert.That(pp.HasData, Is.True);
		}

		[Test]
		public void AddingEmptyString_LeavesHasDataFalse()
		{
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag();
			var pp = new ParatextParagraph();
			pp.StartNewParagraph(stateStub, true);
			Assert.That(pp.HasData, Is.False);
			pp.Add("");
			Assert.That(pp.HasData, Is.False);
		}

		[Test]
		public void StartingParagraph_ClearsText()
		{
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag();
			var pp = new ParatextParagraph();
			pp.StartNewParagraph(stateStub, true);
			pp.Add("this is text");
			Assert.That(pp.BreakIntoBlocks().First().Text, Is.EqualTo("this is text")); // This prevents debug assertion failure.
			pp.StartNewParagraph(new ParserStateStub(), true);
			Assert.That(pp.HasData, Is.False);
		}

		[Test]
		public void BlocksTakeFontInfoFromState()
		{
			var pp = new ParatextParagraph();
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag() {Bold=true, JustificationType = ScrJustificationType.scCenter,
				FontSize = 49,Fontname = "myfont"};
			pp.StartNewParagraph(stateStub, true);
			pp.Add("this is text");
			var block = pp.BreakIntoBlocks().First();

			Assert.That(block.Bold, Is.True);
			Assert.That(block.Centered, Is.False, "For now Centered is supposed to be ignored");
			Assert.That(block.FontName, Is.EqualTo("myfont"));
			Assert.That(block.FontSize, Is.EqualTo(49));
			Assert.That(block.Number, Is.EqualTo(1));
		}

		[Test]
		public void BlocksTakeFontInfoFromDefaultIfStateBlank()
		{
			var pp = new ParatextParagraph();
			pp.DefaultFont = "SomeFont";
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag()
			{
				Bold = true,
				JustificationType = ScrJustificationType.scCenter,
				FontSize = 49
			};
			pp.StartNewParagraph(stateStub, true);
			pp.Add("this is text");
			var block = pp.BreakIntoBlocks().First();

			Assert.That(block.Bold, Is.True);
			Assert.That(block.Centered, Is.False, "For now Centered is supposed to be ignored");
			Assert.That(block.FontName, Is.EqualTo("SomeFont"));
			Assert.That(block.FontSize, Is.EqualTo(49));
			Assert.That(block.Number, Is.EqualTo(1));
		}

		[Test]
		public void LineNumberContinuesIncreasingIfNotReset()
		{
			var pp = new ParatextParagraph();
			pp.DefaultFont = "SomeFont";
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag();
			pp.StartNewParagraph(stateStub, true);
			pp.Add("This is text. So is This.");
			Assert.That(pp.BreakIntoBlocks().First().Number, Is.EqualTo(1));
			Assert.That(pp.BreakIntoBlocks().Last().Number, Is.EqualTo(2));

			pp.StartNewParagraph(stateStub, false);
			pp.Add("This is more text.");
			var block = pp.BreakIntoBlocks().First();
			Assert.That(block.Number, Is.EqualTo(3));
		}

		void SetDefaultState(ParatextParagraph pp)
		{
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag();
			pp.StartNewParagraph(stateStub, true);
		}

		[Test]
		public void InputWithNoSeparators_YieldsOneLine()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("this is text");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(1));
			Assert.That(blocks.First().Text, Is.EqualTo("this is text"));
		}

		[Test]
		public void InputWithCommonSeparators_YieldsMultipleLines_WithCorrectPunctuation()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("this is text; this is more. Is this good text? You decide! It makes a test, anyway.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(4));
			Assert.That(blocks[0].Text, Is.EqualTo("this is text; this is more."));
			Assert.That(blocks[1].Text, Is.EqualTo("Is this good text?"));
			Assert.That(blocks[2].Text, Is.EqualTo("You decide!"));
			Assert.That(blocks[3].Text, Is.EqualTo("It makes a test, anyway."));
		}

		[Test]
		public void BreakIntoBlocks_DevenagriInput_SeparatesCorrectly()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add(
				"विराम अवस्था में किसी वस्तु की ऊर्जा का मान mc2 होता है जहां m वस्तु का द्रव्यमान है। ऊर्जा सरंक्षण के नियम से किसी भी क्रिया में द्रव्यमान में कमी क्रिया के पश्चात इसकी गतिज ऊर्जा में वृद्धि के तुल्य होनी चाहिए। इसी प्रकार, किसी वस्तु का द्रव्यमान को इसकी गतिज ऊर्जा को इसमें लेकर बढाया जा सकता है।");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(3));
			Assert.That(blocks[0].Text,
						Is.EqualTo("विराम अवस्था में किसी वस्तु की ऊर्जा का मान mc2 होता है जहां m वस्तु का द्रव्यमान है।"));
			Assert.That(blocks[1].Text,
						Is.EqualTo(
							"ऊर्जा सरंक्षण के नियम से किसी भी क्रिया में द्रव्यमान में कमी क्रिया के पश्चात इसकी गतिज ऊर्जा में वृद्धि के तुल्य होनी चाहिए।"));
		}

		[Test]
		public void InputWithAngleBrackets_YieldsProperQuotes()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("He said, <<Is this good text?>> She said, <<I'm not sure. >> “You decide”! It makes a <<test>>, anyway");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(4));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, “Is this good text?”"));
			Assert.That(blocks[1].Text, Is.EqualTo("She said, “I'm not sure. ”"));
			Assert.That(blocks[2].Text, Is.EqualTo("“You decide”!"));
			Assert.That(blocks[3].Text, Is.EqualTo("It makes a “test”, anyway"));
		}

		[Test]
		public void InputWithAngleBrackets_YieldsProperQuotes_ForSingleSentence()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("He said, <<This is good text>>");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(1));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, “This is good text”"));
		}

		[Test]
		public void InputWithFinalClosingQuoteAfterPunctuation()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("He said, <<This is good text!>>");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(1));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, “This is good text!”"));
		}

		[Test]
		public void SingleClosingQuoteGoesToPreviousSegment()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("He said, <This is good text!> <Here is some more>");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(2));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, ‘This is good text!’"));
			Assert.That(blocks[1].Text, Is.EqualTo("‘Here is some more’"));
		}

		[Test]
		public void TripleAngleBracketsHandledCorrectly()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("He said, <<She said, <This is good text!>>> <<<Here is some more!> she said.>>");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(3));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, “She said, ‘This is good text!’”")); // note that the output is a single close followed by a double
			Assert.That(blocks[1].Text, Is.EqualTo("“‘Here is some more!’")); // output starts with double followed by single
			Assert.That(blocks[2].Text, Is.EqualTo("she said.”")); // might be even smarter to attach to previous, but we aren't there yet.
		}

		/// <summary>
		/// This one tries out all four common closing quote characters. Several others (all of Unicode class closing quote) are also included,
		/// but this is not currently considered important behavior for HearThis.
		/// </summary>
		[Test]
		public void ClosingQuoteSpaceCombination_AttachesToPreviousSentence()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("He said, «She said, <This is good text!>  › >>» «Here is some more!» she said.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(3));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, «She said, ‘This is good text!’  › ”»"));
			Assert.That(blocks[1].Text, Is.EqualTo("«Here is some more!»"));
			Assert.That(blocks[2].Text, Is.EqualTo("she said."));
		}

		[Test]
		public void ClosingQuoteAndParen_AttachesToPreviousSentence()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("He said, «She said, (This is good text!)  › >>» [Here is some more! ] she said.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(3));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, «She said, (This is good text!)  › ”»"));
			Assert.That(blocks[1].Text, Is.EqualTo("[Here is some more! ]"));
			Assert.That(blocks[2].Text, Is.EqualTo("she said."));

		}

		[Test]
		public void LeadingQuestionMarkDoesNotPreventSegmentation()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("?Hello? This is a sentence. This is another.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(3));
			Assert.That(blocks[0].Text, Is.EqualTo("?Hello?"));
			Assert.That(blocks[1].Text, Is.EqualTo("This is a sentence."));
			Assert.That(blocks[2].Text, Is.EqualTo("This is another."));
		}

		[Test]
		public void LeadingSegBreakInLaterSegment_GetsAttachedToFollowing()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("This is a test. !This is emphasised! This is another.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(3));
			Assert.That(blocks[0].Text, Is.EqualTo("This is a test."));
			Assert.That(blocks[1].Text, Is.EqualTo("!This is emphasised!"));
			Assert.That(blocks[2].Text, Is.EqualTo("This is another."));
		}
	}
}
