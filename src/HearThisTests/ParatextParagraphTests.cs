using System.Linq;
using HearThis.Script;
using NUnit.Framework;
using Paratext.Data;

namespace HearThisTests
{
	public class ParatextParagraphTests
	{
		readonly SentenceClauseSplitter _splitter = new SentenceClauseSplitter(null, false, new CurlyQuotesProject());
		[Test]
		public void NewInstance_HasNoText()
		{
			Assert.That(new ParatextParagraph(_splitter).HasData, Is.False);
		}

		[Test]
		public void AddingText_MakesHasDataTrue()
		{
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag();
			var pp = new ParatextParagraph(_splitter);
			pp.StartNewParagraph(stateStub, true);
			Assert.That(pp.HasData, Is.False);
			pp.Add("this is text");
			Assert.That(pp.HasData, Is.True);
		}

		[Test]
		public void AddingEmptyString_LeavesHasDataFalse()
		{
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			Assert.That(pp.HasData, Is.False);
			pp.Add("");
			Assert.That(pp.HasData, Is.False);
		}

		[Test]
		public void AddingWhitespaceString_LeavesHasDataFalse()
		{
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			Assert.That(pp.HasData, Is.False);
			pp.Add("        ");
			Assert.That(pp.HasData, Is.False);
		}

		[Test]
		public void Add_ReplaceChevronsWithQuotesWhenQuotesAreChevrons_NoChevronReplacementPerformed()
		{
			var splitter = new SentenceClauseSplitter(null, true, new ChevronQuotesProject());
			var pp = new ParatextParagraph(splitter);
			SetDefaultState(pp);
			pp.Add("Then God said, <<Do not say, <Why did the Lord say, <<You have sinned,>> when we did what was right in our own eyes,> or I will pluck you from this good land.>>");
			var result = pp.BreakIntoBlocks();
			Assert.That(result.Select(r => r.Text), Is.EqualTo(new []
			{
				"Then God said,",
				"<<Do not say, <Why did the Lord say, <<You have sinned,>> when we did what was right in our own eyes,> or I will pluck you from this good land.>>"
			}));
		}

		[Test]
		public void StartNewParagraph_ParagraphHasData_ClearsText()
		{
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.Add("this is text");
			Assert.That(pp.BreakIntoBlocks().First().Text, Is.EqualTo("this is text")); // This prevents debug assertion failure.
			Assert.That(pp.HasData, Is.True);
			pp.StartNewParagraph(new ParserStateStub(), true);
			Assert.That(pp.HasData, Is.False);
		}

		[Test]
		public void BlocksTakeFontInfoFromState()
		{
			var pp = new ParatextParagraph(_splitter);
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag {Marker = @"\s", Name = "Section Head", Bold = true,
				JustificationType = ScrJustificationType.scCenter, FontSize = 49, Fontname = "myfont",
				TextType = ScrTextType.scSection };
			pp.StartNewParagraph(stateStub, true);
			pp.Add("this is text");
			var block = pp.BreakIntoBlocks().First();

			Assert.That(block.Bold, Is.True);
			Assert.That(block.Centered, Is.False, "For now Centered is supposed to be ignored");
			Assert.That(block.FontName, Is.EqualTo("myfont"));
			Assert.That(block.FontSize, Is.EqualTo(49));
			Assert.That(block.Number, Is.EqualTo(1));
			Assert.That(block.ParagraphStyle, Is.EqualTo("Section Head"));
			Assert.That(block.Heading, Is.True);
			Assert.That(block.HeadingType, Is.EqualTo("s"));
		}

		[Test]
		public void BlocksTakeFontInfoFromDefaultIfStateBlank()
		{
			var pp = new ParatextParagraph(_splitter);
			pp.DefaultFont = "SomeFont";
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag()
			{
				Marker = @"\p",
				Name = "Paragraph",
				Bold = true,
				JustificationType = ScrJustificationType.scCenter,
				FontSize = 49,
				TextType = ScrTextType.scVerseText,
			};
			pp.StartNewParagraph(stateStub, true);
			pp.Add("this is text");
			var block = pp.BreakIntoBlocks().First();

			Assert.That(block.Bold, Is.True);
			Assert.That(block.Centered, Is.False, "For now Centered is supposed to be ignored");
			Assert.That(block.FontName, Is.EqualTo("SomeFont"));
			Assert.That(block.FontSize, Is.EqualTo(49));
			Assert.That(block.Number, Is.EqualTo(1));
			Assert.That(block.ParagraphStyle, Is.EqualTo("Paragraph"));
			Assert.That(block.Heading, Is.False);
			Assert.That(block.HeadingType, Is.Null);
		}

		[Test]
		public void LineNumberContinuesIncreasingIfNotReset()
		{
			var pp = new ParatextParagraph(_splitter);
			pp.DefaultFont = "SomeFont";
			var stateStub = SetDefaultState(pp);
			pp.Add("This is text. So is This.");
			Assert.That(pp.BreakIntoBlocks().First().Number, Is.EqualTo(1));
			Assert.That(pp.BreakIntoBlocks().Last().Number, Is.EqualTo(2));

			pp.StartNewParagraph(stateStub, false);
			pp.Add("This is more text.");
			var block = pp.BreakIntoBlocks().First();
			Assert.That(block.Number, Is.EqualTo(3));
		}

		ParserStateStub SetDefaultState(ParatextParagraph pp)
		{
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag();
			pp.StartNewParagraph(stateStub, true);
			return stateStub;
		}

		[Test]
		public void InputWithNoSeparators_YieldsOneLine()
		{
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.Add("this is text");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(1));
			Assert.That(blocks.First().Text, Is.EqualTo("this is text"));
		}

		[Test]
		public void BreakIntoBlocks_InputWithCommonSeparators_YieldsMultipleLinesWithCorrectPunctuation()
		{
			var pp = new ParatextParagraph(_splitter);
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
		public void BreakIntoBlocks_KeepTogether_YieldsSingleLine()
		{
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.Add("(Lk 2.23; Mk 5.8)");
			var blocks = pp.BreakIntoBlocks(true).ToList();
			Assert.That(blocks, Has.Count.EqualTo(1));
			Assert.That(blocks[0].Text, Is.EqualTo("(Lk 2.23; Mk 5.8)"));
		}

		[Test]
		public void BreakIntoBlocks_TwoSentencesWithinSingleVerse_YieldsTwoBlocksWithSameVerseNumber()
		{
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.NoteVerseStart("2");
			pp.Add("Verse two.");
			pp.NoteVerseStart("3");
			pp.Add("This is the first sentence. This is the second sentence.");
			pp.NoteVerseStart("4");
			pp.Add("Verse four.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(4));
			Assert.That(blocks[0].Text, Is.EqualTo("Verse two."));
			Assert.That(blocks[0].Verse, Is.EqualTo("2"));
			Assert.That(blocks[1].Text, Is.EqualTo("This is the first sentence."));
			Assert.That(blocks[1].Verse, Is.EqualTo("3"));
			Assert.That(blocks[2].Text, Is.EqualTo("This is the second sentence."));
			Assert.That(blocks[2].Verse, Is.EqualTo("3"));
			Assert.That(blocks[3].Text, Is.EqualTo("Verse four."));
			Assert.That(blocks[3].Verse, Is.EqualTo("4"));
		}

		[Test]
		public void BreakIntoBlocks_SentencesCrossVerseBreaks_YieldsBlocksWithImplicitVerseBridge()
		{
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.NoteVerseStart("2");
			const string kV2Text = "If some people don't believe; ";
			pp.Add(kV2Text);
			pp.NoteVerseStart("3");
			const string kV3Text = "they will find out they're wrong ";
			pp.Add(kV3Text);
			pp.NoteVerseStart("4");
			pp.Add("in a loving way. But I say, ");
			pp.NoteVerseStart("5");
			pp.Add("try not to freak out!");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(2));
			Assert.That(blocks[0].Text, Is.EqualTo(kV2Text + kV3Text + "in a loving way."));
			Assert.That(blocks[0].Verse, Is.EqualTo("2~3~4"));
			Assert.That(blocks[0].CrossesVerseBreak, Is.True);
			var verseOffsets = blocks[0].VerseOffsets.ToList();
			Assert.That(verseOffsets.Count, Is.EqualTo(2));
			Assert.That(verseOffsets[0], Is.EqualTo(kV2Text.Length));
			Assert.That(verseOffsets[1], Is.EqualTo(kV2Text.Length + kV3Text.Length));
			Assert.That(blocks[1].Text, Is.EqualTo("But I say, try not to freak out!"));
			Assert.That(blocks[1].Verse, Is.EqualTo("4~5"));
			Assert.That(blocks[1].CrossesVerseBreak, Is.True);
			verseOffsets = blocks[1].VerseOffsets.ToList();
			Assert.That(verseOffsets.Count, Is.EqualTo(1));
			Assert.That(verseOffsets[0], Is.EqualTo("But I say, ".Length));
		}

		[Test]
		public void BreakIntoBlocks_DeeplyNestedChevrons_YieldsBlocksWithCorrectVerseNumber()
		{
			var pp = new ParatextParagraph(new SentenceClauseSplitter(null, true, new CurlyQuotesProject()));
			SetDefaultState(pp);
			pp.NoteVerseStart("9");
			pp.Add("<<You are a <martian>,>> noted John. ");
			pp.NoteVerseStart("10");
			pp.Add("<<You say, <You are a <<martian,>>> but I think you are from Pluto!>> rebutted his friend Wally.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(4));
			Assert.That(blocks[0].Text, Is.EqualTo("“You are a ‘martian’,”"));
			Assert.That(blocks[0].Verse, Is.EqualTo("9"));
			Assert.That(blocks[1].Text, Is.EqualTo("noted John."));
			Assert.That(blocks[1].Verse, Is.EqualTo("9"));
			Assert.That(blocks[2].Text, Is.EqualTo("“You say, ‘You are a “martian,”’ but I think you are from Pluto!”"));
			Assert.That(blocks[2].Verse, Is.EqualTo("10"));
			Assert.That(blocks[3].Text, Is.EqualTo("rebutted his friend Wally."));
			Assert.That(blocks[3].Verse, Is.EqualTo("10"));
		}

		[Test]
		public void BreakIntoBlocks_VeryDeeplyNestedChevronsWithNoQuoteLevelsDefined_NoReplacements()
		{
			var pp = new ParatextParagraph(new SentenceClauseSplitter(null, true, new NoQuotesProject()));
			SetDefaultState(pp);
			pp.NoteVerseStart("10");
			pp.Add("<<You say: <You think, <<A martian says, <You don't think I know the word for <<dog,>>>>>> but you are wrong,>> rebutted Wally.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(1));
			Assert.That(blocks[0].Text, Is.EqualTo("<<You say: <You think, <<A martian says, <You don't think I know the word for <<dog,>>>>>> but you are wrong,>> rebutted Wally."));
			Assert.That(blocks[0].Verse, Is.EqualTo("10"));
		}

		[Test]
		public void BreakIntoBlocks_VeryDeeplyNestedChevronsWithThreeDistinctLevels_FirstAndThirdLevelQuotesAreCorrect()
		{
			var pp = new ParatextParagraph(new SentenceClauseSplitter(null, true, new ThreeLevelDistinctQuotesProject()));
			SetDefaultState(pp);
			pp.NoteVerseStart("10");
			pp.Add("<<You say: <You think, <<A martian says, <You don't think I know the word for <<dog,>>>>>> but you are wrong!>> rebutted Wally.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(2));
			Assert.That(blocks[0].Text, Is.EqualTo("“You say: ‘You think, {+A martian says, “You don't think I know the word for ‘dog,’”+}’ but you are wrong!”"));
			Assert.That(blocks[0].Verse, Is.EqualTo("10"));
			Assert.That(blocks[1].Text, Is.EqualTo("rebutted Wally."));
			Assert.That(blocks[1].Verse, Is.EqualTo("10"));
		}

		[Test]
		public void BreakIntoBlocks_NestedChevronsInTextButOnlyTwoLevelsOfQuotesDefinedInProject_NestedChevronsConvertedCorrectly()
		{
			var pp = new ParatextParagraph(new SentenceClauseSplitter(null, true, new TwoLevelCurlyQuotesProject()));
			SetDefaultState(pp);
			pp.NoteVerseStart("9");
			pp.Add("<<You are a <martian>,>> noted John. ");
			pp.NoteVerseStart("10");
			pp.Add("<<You say, <You are a <<martian,>>> but I think you are from Pluto!>> rebutted his friend Wally.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(4));
			Assert.That(blocks[0].Text, Is.EqualTo("“You are a ‘martian’,”"));
			Assert.That(blocks[0].Verse, Is.EqualTo("9"));
			Assert.That(blocks[1].Text, Is.EqualTo("noted John."));
			Assert.That(blocks[1].Verse, Is.EqualTo("9"));
			Assert.That(blocks[2].Text, Is.EqualTo("“You say, ‘You are a “martian,”’ but I think you are from Pluto!”"));
			Assert.That(blocks[2].Verse, Is.EqualTo("10"));
			Assert.That(blocks[3].Text, Is.EqualTo("rebutted his friend Wally."));
			Assert.That(blocks[3].Verse, Is.EqualTo("10"));
		}

		[Test]
		public void BreakIntoBlocks_SingleClosingChevronInTextButOnlyOneLevelOfQuotesDefinedInProject_SingleChevronsNotConverted()
		{
			var pp = new ParatextParagraph(new SentenceClauseSplitter(null, true, new OneLevelCurlyQuotesProject()));
			SetDefaultState(pp);
			pp.NoteVerseStart("9");
			pp.Add("<<You are a <martian>,>> noted John. ");
			pp.NoteVerseStart("10");
			pp.Add("<<You say, <You are a <<martian,>>> but I think you are from Pluto!>> rebutted his friend Wally.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(4));
			Assert.That(blocks[0].Text, Is.EqualTo("“You are a <martian>,”"));
			Assert.That(blocks[0].Verse, Is.EqualTo("9"));
			Assert.That(blocks[1].Text, Is.EqualTo("noted John."));
			Assert.That(blocks[1].Verse, Is.EqualTo("9"));
			Assert.That(blocks[2].Text, Is.EqualTo("“You say, <You are a “martian,”> but I think you are from Pluto!”"));
			Assert.That(blocks[2].Verse, Is.EqualTo("10"));
			Assert.That(blocks[3].Text, Is.EqualTo("rebutted his friend Wally."));
			Assert.That(blocks[3].Verse, Is.EqualTo("10"));
		}

		[Test]
		public void BreakIntoBlocks_SentenceBeginsInVerseFollowingEmptyVerse_YieldsBlocksWithCorrectVerseNumber()
		{
			var pp = new ParatextParagraph(new SentenceClauseSplitter(null, true, new CurlyQuotesProject()));
			SetDefaultState(pp);
			pp.NoteVerseStart("9");
			pp.Add("<<You are a martian,>> noted John. ");
			pp.NoteVerseStart("10");
			pp.NoteVerseStart("11");
			pp.Add("<<You say, <You are a martian,> but I think you are from Pluto!>> rebutted his friend Wally.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(4));
			Assert.That(blocks[0].Text, Is.EqualTo("“You are a martian,”"));
			Assert.That(blocks[0].Verse, Is.EqualTo("9"));
			Assert.That(blocks[0].CrossesVerseBreak, Is.False);
			Assert.That(blocks[1].Text, Is.EqualTo("noted John."));
			Assert.That(blocks[1].Verse, Is.EqualTo("9"));
			Assert.That(blocks[1].CrossesVerseBreak, Is.False);
			Assert.That(blocks[2].Text, Is.EqualTo("“You say, ‘You are a martian,’ but I think you are from Pluto!”"));
			Assert.That(blocks[2].Verse, Is.EqualTo("11"));
			Assert.That(blocks[2].CrossesVerseBreak, Is.False);
			Assert.That(blocks[3].Text, Is.EqualTo("rebutted his friend Wally."));
			Assert.That(blocks[3].Verse, Is.EqualTo("11"));
			Assert.That(blocks[3].CrossesVerseBreak, Is.False);
		}

		[Test]
		public void BreakIntoBlocks_DevanagariInput_SeparatesCorrectly()
		{
			var pp = new ParatextParagraph(_splitter);
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
			var pp = new ParatextParagraph(_splitter);
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
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.Add("He said, <<This is good text>>");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(1));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, “This is good text”"));
		}

		[Test]
		public void InputWithFinalClosingQuoteAfterPunctuation()
		{
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.Add("He said, <<This is good text!>>");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(1));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, “This is good text!”"));
		}

		[Test]
		public void SingleClosingQuoteGoesToPreviousSegment()
		{
			var pp = new ParatextParagraph(_splitter);
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
			// This is really essentially a test of the SentenceClauseSplitter class
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.Add("He said, <<She said, <This is good text!>>> <<<Here is some more!> she said.>>");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(2));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, “She said, ‘This is good text!’”")); // note that the output is a single close followed by a double
			Assert.That(blocks[1].Text, Is.EqualTo("“‘Here is some more!’ she said.”")); // output starts with double followed by single
		}

		/// <summary>
		/// This one tries out all four common closing quote characters (real Chevrons don't get replaced. GT and LT signs get replaced with curly quotes).
		/// The production code checks for any character having Unicode class closing quote, so this test doesn't test every possibility.
		/// </summary>
		[Test]
		public void ClosingQuoteSpaceCombination_AttachesToPreviousSentence()
		{
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.Add("He said, «She said, <This is good text!>  › ”» «Here is some more!» Bill said.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(3));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, «She said, ‘This is good text!’  › ”»"));
			Assert.That(blocks[1].Text, Is.EqualTo("«Here is some more!»"));
			Assert.That(blocks[2].Text, Is.EqualTo("Bill said."));
		}

		[Test]
		public void ClosingQuoteAndParen_AttachesToPreviousSentence()
		{
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.Add("He said, «She said, (This is good text!)  › ”» [Here is some more! ] Bill said.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(3));
			Assert.That(blocks[0].Text, Is.EqualTo("He said, «She said, (This is good text!)  › ”»"));
			Assert.That(blocks[1].Text, Is.EqualTo("[Here is some more! ]"));
			Assert.That(blocks[2].Text, Is.EqualTo("Bill said."));
		}

		[Test]
		public void LeadingQuestionMarkDoesNotPreventSegmentation()
		{
			var pp = new ParatextParagraph(_splitter);
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
			var pp = new ParatextParagraph(_splitter);
			SetDefaultState(pp);
			pp.Add("This is a test. !This is emphasized! This is another.");
			var blocks = pp.BreakIntoBlocks().ToList();
			Assert.That(blocks, Has.Count.EqualTo(3));
			Assert.That(blocks[0].Text, Is.EqualTo("This is a test."));
			Assert.That(blocks[1].Text, Is.EqualTo("!This is emphasized!"));
			Assert.That(blocks[2].Text, Is.EqualTo("This is another."));
		}
	}

	internal class OneLevelCurlyQuotesProject : IScrProjectSettings
	{
		public string FirstLevelStartQuotationMark => "“";
		public string FirstLevelEndQuotationMark => "”";
		public string SecondLevelStartQuotationMark => "";
		public string SecondLevelEndQuotationMark => "";
		public string ThirdLevelStartQuotationMark => "";
		public string ThirdLevelEndQuotationMark => "";
		public bool FirstLevelQuotesAreUnique => true;
	}

	internal class TwoLevelCurlyQuotesProject : IScrProjectSettings
	{
		public string FirstLevelStartQuotationMark => "“";
		public string FirstLevelEndQuotationMark => "”";
		public string SecondLevelStartQuotationMark => "‘";
		public string SecondLevelEndQuotationMark => "’";
		public string ThirdLevelStartQuotationMark => "";
		public string ThirdLevelEndQuotationMark => "";
		public bool FirstLevelQuotesAreUnique => true;
	}

	internal class ThreeLevelDistinctQuotesProject : IScrProjectSettings
	{
		public string FirstLevelStartQuotationMark => "“";
		public string FirstLevelEndQuotationMark => "”";
		public string SecondLevelStartQuotationMark => "‘";
		public string SecondLevelEndQuotationMark => "’";

		// Not aware of any real quote system that has three levels that are fully distinct,
		// but since Paratext UI allows for it, this serves as an example to prove HT
		// handles it correctly.
		public string ThirdLevelStartQuotationMark => "{+";
		public string ThirdLevelEndQuotationMark => "+}";
		public bool FirstLevelQuotesAreUnique => true;
	}

	internal class NoQuotesProject : IScrProjectSettings
	{
		public string FirstLevelStartQuotationMark => "";
		public string FirstLevelEndQuotationMark => "";
		public string SecondLevelStartQuotationMark => "";
		public string SecondLevelEndQuotationMark => "";
		public string ThirdLevelStartQuotationMark => "";
		public string ThirdLevelEndQuotationMark => "";
		public bool FirstLevelQuotesAreUnique => false;
	}
}
