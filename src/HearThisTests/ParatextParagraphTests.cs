using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var pp = new ParatextParagraph();
			pp.Add("this is text");
			Assert.That(pp.HasData, Is.True);
		}

		[Test]
		public void AddingEmptyString_LeavesHasDataFalse()
		{
			var pp = new ParatextParagraph();
			pp.Add("");
			Assert.That(pp.HasData, Is.False);
		}

		[Test]
		public void StartingParagraph_ClearsText()
		{
			var pp = new ParatextParagraph();
			pp.Add("this is text");
			pp.StartNewParagraph(new ParserStateStub());
			Assert.That(pp.HasData, Is.False);
		}

		[Test]
		public void LinesTakeFontInfoFromState()
		{
			var pp = new ParatextParagraph();
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag() {Bold=true, JustificationType = ScrJustificationType.scCenter,
				FontSize = 49,Fontname = "myfont"};
			pp.StartNewParagraph(stateStub);
			pp.Add("this is text");
			var line = pp.BreakIntoLines().First();

			Assert.That(line.Bold, Is.True);
			Assert.That(line.Centered, Is.True);
			Assert.That(line.FontName, Is.EqualTo("myfont"));
			Assert.That(line.FontSize, Is.EqualTo(49));
		}

		void SetDefaultState(ParatextParagraph pp)
		{
			var stateStub = new ParserStateStub();
			stateStub.ParaTag = new ScrTag();
			pp.StartNewParagraph(stateStub);
		}

		[Test]
		public void InputWithNoSeparators_YieldsOneLine()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("this is text");
			var lines = pp.BreakIntoLines().ToList();
			Assert.That(lines, Has.Count.EqualTo(1));
			Assert.That(lines.First().Text, Is.EqualTo("this is text"));
		}

		[Test]
		public void InputWithCommonSeparators_YieldsMultipleLines_WithCorrectPunctuation()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("this is text; this is more. Is this good text? You decide! It makes a test, anyway.");
			var lines = pp.BreakIntoLines().ToList();
			Assert.That(lines, Has.Count.EqualTo(4));
			Assert.That(lines[0].Text, Is.EqualTo("this is text; this is more."));
			Assert.That(lines[1].Text, Is.EqualTo("Is this good text?"));
			Assert.That(lines[2].Text, Is.EqualTo("You decide!"));
			Assert.That(lines[3].Text, Is.EqualTo("It makes a test, anyway."));
		}

		[Test]
		public void InputWithAngleBrackets_YieldsProperQuotes()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("He said, <<Is this good text?>> She said, <<I'm not sure. >> “You decide”! It makes a <<test>>, anyway");
			var lines = pp.BreakIntoLines().ToList();
			Assert.That(lines, Has.Count.EqualTo(4));
			Assert.That(lines[0].Text, Is.EqualTo("He said, “Is this good text?”"));
			Assert.That(lines[1].Text, Is.EqualTo("She said, “I'm not sure. ”"));
			Assert.That(lines[2].Text, Is.EqualTo("“You decide”!"));
			Assert.That(lines[3].Text, Is.EqualTo("It makes a “test”, anyway"));
		}

		[Test]
		public void InputWithAngleBrackets_YieldsProperQuotes_ForSingleSentence()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("He said, <<This is good text>>");
			var lines = pp.BreakIntoLines().ToList();
			Assert.That(lines, Has.Count.EqualTo(1));
			Assert.That(lines[0].Text, Is.EqualTo("He said, “This is good text”"));
		}

		[Test]
		public void InputWithFinalClosingQuoteAfterPunctuation()
		{
			var pp = new ParatextParagraph();
			SetDefaultState(pp);
			pp.Add("He said, <<This is good text!>>");
			var lines = pp.BreakIntoLines().ToList();
			Assert.That(lines, Has.Count.EqualTo(1));
			Assert.That(lines[0].Text, Is.EqualTo("He said, “This is good text!”"));
		}
	}
}
