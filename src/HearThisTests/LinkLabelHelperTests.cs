// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2020' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using HearThis;
using NUnit.Framework;
using System.Windows.Forms;

namespace HearThisTests
{
	[TestFixture]
	class LinkLabelHelperTests
	{
		[TestCase("")]
		[TestCase(" ")]
		public void SetLinkRegions_TextEmptyOrWhitespace_LinkRegionSetToEntireText(string text)
		{
			var linkLabel = new LinkLabel {Text = text };
			linkLabel.Links[0].LinkData = "don't lose this";
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo(text));
			Assert.That(linkLabel.LinkArea.Start, Is.EqualTo(0));
			Assert.That(linkLabel.LinkArea.Length, Is.EqualTo(text.Length));
			Assert.That(linkLabel.Links[0].LinkData, Is.EqualTo("don't lose this"));
		}

		[TestCase("This is some text without square brackets")]
		[TestCase("This is some text with opening [square bracket")]
		[TestCase("This is some text with closing] square bracket")]
		[TestCase("This is some text with] mismatched [square brackets")]
		[TestCase("This is some text with [] empty square brackets.")]
		public void SetLinkRegions_TextWithoutPairedSquareBracketsContainingText_LinkRegionSetToEntireText(string text)
		{
			var linkLabel = new LinkLabel {Text = text };
			linkLabel.Links[0].LinkData = "don't lose this";
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo(text));
			Assert.That(linkLabel.LinkArea.Start, Is.EqualTo(0));
			Assert.That(linkLabel.LinkArea.Length, Is.EqualTo(text.Length));
			Assert.That(linkLabel.Links[0].LinkData, Is.EqualTo("don't lose this"));
			// That was too easy. The default is to set the link area to the whole
			// thing when the Text is set. Let's try again with the link area set
			// to something else;
			linkLabel.Links[0].Start = 8;
			linkLabel.Links[0].Length = 4;
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.LinkArea.Start, Is.EqualTo(0));
			Assert.That(linkLabel.LinkArea.Length, Is.EqualTo(text.Length));
			Assert.That(linkLabel.Links[0].LinkData, Is.EqualTo("don't lose this"));
		}

		[Test]
		public void SetLinkRegions_SquareBracketsAroundEntireText_LinkRegionSetToEntireTextWithSquareBracketsRemoved()
		{
			var linkLabel = new LinkLabel { Text = "[This whole thing is the link]",
				LinkArea = new LinkArea(6, 7)}; // Arbitrary non-default LinkArea
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo("This whole thing is the link"));
			Assert.That(linkLabel.LinkArea.Start, Is.EqualTo(0));
			Assert.That(linkLabel.LinkArea.Length, Is.EqualTo(linkLabel.Text.Length));
		}

		[Test]
		public void SetLinkRegions_SquareBracketsAroundStartOfText_LinkRegionSetToPartOfTextInSquareBrackets()
		{
			var linkLabel = new LinkLabel { Text = "[This] was the link",
				LinkArea = new LinkArea(6, 7)}; // Arbitrary non-default LinkArea
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo("This was the link"));
			Assert.That(linkLabel.LinkArea.Start, Is.EqualTo(0));
			Assert.That(linkLabel.LinkArea.Length, Is.EqualTo(4));
		}

		[Test]
		public void SetLinkRegions_SquareBracketsAroundEndOfText_LinkRegionSetToPartOfTextInSquareBrackets()
		{
			var linkLabel = new LinkLabel { Text = "This is the [link to use]",
				LinkArea = new LinkArea(3, 5)}; // Arbitrary non-default LinkArea
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo("This is the link to use"));
			Assert.That(linkLabel.LinkArea.Start, Is.EqualTo(12));
			Assert.That(linkLabel.LinkArea.Length, Is.EqualTo(11));
		}

		[TestCase("This is the [link to use].", 12, 11)]
		[TestCase("This is a short [l]ink.", 16, 1)]
		public void SetLinkRegions_SquareBracketsAroundPartOfText_LinkRegionSetToPartOfTextInSquareBracketsAndLinkDataPreserved(
			string text, int expectedStart, int expectedLength)
		{
			var linkLabel = new LinkLabel { Text = text,
				LinkArea = new LinkArea(0, 1)}; // Arbitrary non-default LinkArea
			// Since the normal behavior of a LinkLabel is to reset the LinkData, we
			// also need to make sure that we preserve this when dynamically setting
			// the link region.
			linkLabel.Links[0].LinkData = "frog";
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo(text.Replace("[", "").Replace("]", "")));
			Assert.That(linkLabel.LinkArea.Start, Is.EqualTo(expectedStart));
			Assert.That(linkLabel.LinkArea.Length, Is.EqualTo(expectedLength));
			Assert.That(linkLabel.Links.Count, Is.EqualTo(1));
			Assert.That(linkLabel.Links[0].LinkData as string, Is.EqualTo("frog"));
		}

		/// <summary>
		/// This is a weird and somewhat arbitrary case.
		/// </summary>
		[Test]
		public void SetLinkRegions_SquareBracketsAroundMoreBitsOfTextThanInLinksCollection_LinkRegionSetToFirstTextInSquareBrackets()
		{
			var linkLabel = new LinkLabel { Text = "Is [this] the [link to use]?",
				LinkArea = new LinkArea(12, 11)}; // Arbitrary non-default LinkArea
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo("Is this the [link to use]?"));
			Assert.That(linkLabel.LinkArea.Start, Is.EqualTo(3));
			Assert.That(linkLabel.LinkArea.Length, Is.EqualTo(4));
		}

		[Test]
		public void SetLinkRegions_MultipleLinks_LinkRegionsMatchedToTextInSquareBrackets()
		{
			var linkLabel = new LinkLabel { Text = "I guess [this] is the [link] to [use] for stuff." };
			// These linked regions are arbitrary, but LinkLabel won't allow them to be set in a way that
			// causes them to overlap or go beyond the text.
			linkLabel.Links.Add(0, 2, "first");
			linkLabel.Links.Add(8, 3, "second");
			linkLabel.Links.Add(16, 7, "third");

			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo("I guess this is the link to use for stuff."));
			Assert.That(linkLabel.Links.Count, Is.EqualTo(3));
			Assert.That(linkLabel.Links[0].Start, Is.EqualTo(8));
			Assert.That(linkLabel.Links[0].Length, Is.EqualTo(4));
			Assert.That(linkLabel.Links[0].LinkData, Is.EqualTo("first"));
			Assert.That(linkLabel.Links[1].Start, Is.EqualTo(20));
			Assert.That(linkLabel.Links[1].Length, Is.EqualTo(4));
			Assert.That(linkLabel.Links[1].LinkData, Is.EqualTo("second"));
			Assert.That(linkLabel.Links[2].Start, Is.EqualTo(28));
			Assert.That(linkLabel.Links[2].Length, Is.EqualTo(3));
			Assert.That(linkLabel.Links[2].LinkData, Is.EqualTo("third"));
		}

		[Test]
		public void SetLinkRegions_FewerSquareBracketsThanOriginalLinks_LinkRegionsToWhateverIsThere()
		{
			const string kEnglishTextWithBrackets = "I [guess this] is [the link] to [use] for [English].";
			var linkLabel = new LinkLabel { Text = kEnglishTextWithBrackets };
			linkLabel.Links.Add(2, 1, "first");
			linkLabel.Links.Add(3, 1, "second");
			linkLabel.Links.Add(4, 1, "third");
			linkLabel.Links.Add(5, 46, "fourth"); // Looks weird, but tests some edge-case logic
			// Baseline (English)
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo("I guess this is the link to use for English."));
			Assert.That(linkLabel.Links.Count, Is.EqualTo(4));
			Assert.That(linkLabel.Links[0].Start, Is.EqualTo(2));
			Assert.That(linkLabel.Links[0].Length, Is.EqualTo(10));
			Assert.That(linkLabel.Links[0].LinkData, Is.EqualTo("first"));
			Assert.That(linkLabel.Links[1].Start, Is.EqualTo(16));
			Assert.That(linkLabel.Links[1].Length, Is.EqualTo(8));
			Assert.That(linkLabel.Links[1].LinkData, Is.EqualTo("second"));
			Assert.That(linkLabel.Links[2].Start, Is.EqualTo(28));
			Assert.That(linkLabel.Links[2].Length, Is.EqualTo(3));
			Assert.That(linkLabel.Links[2].LinkData, Is.EqualTo("third"));
			Assert.That(linkLabel.Links[3].Start, Is.EqualTo(36));
			Assert.That(linkLabel.Links[3].Length, Is.EqualTo(7));
			Assert.That(linkLabel.Links[3].LinkData, Is.EqualTo("fourth"));

			linkLabel.Text = "[Supongo que esto] es [lo que se necesita].";
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo("Supongo que esto es lo que se necesita."));
			Assert.That(linkLabel.Links.Count, Is.EqualTo(2));
			Assert.That(linkLabel.Links[0].Start, Is.EqualTo(0));
			Assert.That(linkLabel.Links[0].Length, Is.EqualTo(16));
			Assert.That(linkLabel.Links[0].LinkData, Is.EqualTo("first"));
			Assert.That(linkLabel.Links[1].Start, Is.EqualTo(20));
			Assert.That(linkLabel.Links[1].Length, Is.EqualTo(18));

			// Ensure that setting it back to English restores the original text and links
			linkLabel.Text = kEnglishTextWithBrackets;
			linkLabel.SetLinkRegions();
			Assert.That(linkLabel.Text, Is.EqualTo("I guess this is the link to use for English."));
			Assert.That(linkLabel.Links.Count, Is.EqualTo(4));
			Assert.That(linkLabel.Links[0].Start, Is.EqualTo(2));
			Assert.That(linkLabel.Links[0].Length, Is.EqualTo(10));
			Assert.That(linkLabel.Links[0].LinkData, Is.EqualTo("first"));
			Assert.That(linkLabel.Links[1].Start, Is.EqualTo(16));
			Assert.That(linkLabel.Links[1].Length, Is.EqualTo(8));
			Assert.That(linkLabel.Links[1].LinkData, Is.EqualTo("second"));
			Assert.That(linkLabel.Links[2].Start, Is.EqualTo(28));
			Assert.That(linkLabel.Links[2].Length, Is.EqualTo(3));
			Assert.That(linkLabel.Links[2].LinkData, Is.EqualTo("third"));
			Assert.That(linkLabel.Links[3].Start, Is.EqualTo(36));
			Assert.That(linkLabel.Links[3].Length, Is.EqualTo(7));
			Assert.That(linkLabel.Links[3].LinkData, Is.EqualTo("fourth"));
		}
	}
}
