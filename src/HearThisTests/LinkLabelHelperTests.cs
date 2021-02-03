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
			Assert.AreEqual(text, linkLabel.Text);
			Assert.AreEqual(0, linkLabel.LinkArea.Start);
			Assert.AreEqual(text.Length, linkLabel.LinkArea.Length);
			Assert.AreEqual("don't lose this", linkLabel.Links[0].LinkData);
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
			Assert.AreEqual(text, linkLabel.Text);
			Assert.AreEqual(0, linkLabel.LinkArea.Start);
			Assert.AreEqual(text.Length, linkLabel.LinkArea.Length);
			Assert.AreEqual("don't lose this", linkLabel.Links[0].LinkData);
			// That was too easy. The default is to set the link area to the whole
			// thing when the Text is set. Let's try again with the link area set
			// to something else;
			linkLabel.Links[0].Start = 8;
			linkLabel.Links[0].Length = 4;
			linkLabel.SetLinkRegions();
			Assert.AreEqual(0, linkLabel.LinkArea.Start);
			Assert.AreEqual(text.Length, linkLabel.LinkArea.Length);
			Assert.AreEqual("don't lose this", linkLabel.Links[0].LinkData);
		}

		[Test]
		public void SetLinkRegions_SquareBracketsAroundEntireText_LinkRegionSetToEntireTextWithSquareBracketsRemoved()
		{
			var linkLabel = new LinkLabel { Text = "[This whole thing is the link]",
				LinkArea = new LinkArea(6, 7)}; // Arbitrary non-default LinkArea
			linkLabel.SetLinkRegions();
			Assert.AreEqual("This whole thing is the link", linkLabel.Text);
			Assert.AreEqual(0, linkLabel.LinkArea.Start);
			Assert.AreEqual(linkLabel.Text.Length, linkLabel.LinkArea.Length);
		}

		[Test]
		public void SetLinkRegions_SquareBracketsAroundStartOfText_LinkRegionSetToPartOfTextInSquareBrackets()
		{
			var linkLabel = new LinkLabel { Text = "[This] was the link",
				LinkArea = new LinkArea(6, 7)}; // Arbitrary non-default LinkArea
			linkLabel.SetLinkRegions();
			Assert.AreEqual("This was the link", linkLabel.Text);
			Assert.AreEqual(0, linkLabel.LinkArea.Start);
			Assert.AreEqual(4, linkLabel.LinkArea.Length);
		}

		[Test]
		public void SetLinkRegions_SquareBracketsAroundEndOfText_LinkRegionSetToPartOfTextInSquareBrackets()
		{
			var linkLabel = new LinkLabel { Text = "This is the [link to use]",
				LinkArea = new LinkArea(3, 5)}; // Arbitrary non-default LinkArea
			linkLabel.SetLinkRegions();
			Assert.AreEqual("This is the link to use", linkLabel.Text);
			Assert.AreEqual(12, linkLabel.LinkArea.Start);
			Assert.AreEqual(11, linkLabel.LinkArea.Length);
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
			Assert.AreEqual(text.Replace("[", "").Replace("]", ""), linkLabel.Text);
			Assert.AreEqual(expectedStart, linkLabel.LinkArea.Start);
			Assert.AreEqual(expectedLength, linkLabel.LinkArea.Length);
			Assert.AreEqual(1, linkLabel.Links.Count);
			Assert.AreEqual("frog", linkLabel.Links[0].LinkData as string);
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
			Assert.AreEqual("Is this the [link to use]?", linkLabel.Text);
			Assert.AreEqual(3, linkLabel.LinkArea.Start);
			Assert.AreEqual(4, linkLabel.LinkArea.Length);
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
			Assert.AreEqual("I guess this is the link to use for stuff.", linkLabel.Text);
			Assert.AreEqual(3, linkLabel.Links.Count);
			Assert.AreEqual(8, linkLabel.Links[0].Start);
			Assert.AreEqual(4, linkLabel.Links[0].Length);
			Assert.AreEqual("first", linkLabel.Links[0].LinkData);
			Assert.AreEqual(20, linkLabel.Links[1].Start);
			Assert.AreEqual(4, linkLabel.Links[1].Length);
			Assert.AreEqual("second", linkLabel.Links[1].LinkData);
			Assert.AreEqual(28, linkLabel.Links[2].Start);
			Assert.AreEqual(3, linkLabel.Links[2].Length);
			Assert.AreEqual("third", linkLabel.Links[2].LinkData);
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
			Assert.AreEqual("I guess this is the link to use for English.", linkLabel.Text);
			Assert.AreEqual(4, linkLabel.Links.Count);
			Assert.AreEqual(2, linkLabel.Links[0].Start);
			Assert.AreEqual(10, linkLabel.Links[0].Length);
			Assert.AreEqual("first", linkLabel.Links[0].LinkData);
			Assert.AreEqual(16, linkLabel.Links[1].Start);
			Assert.AreEqual(8, linkLabel.Links[1].Length);
			Assert.AreEqual("second", linkLabel.Links[1].LinkData);
			Assert.AreEqual(28, linkLabel.Links[2].Start);
			Assert.AreEqual(3, linkLabel.Links[2].Length);
			Assert.AreEqual("third", linkLabel.Links[2].LinkData);
			Assert.AreEqual(36, linkLabel.Links[3].Start);
			Assert.AreEqual(7, linkLabel.Links[3].Length);
			Assert.AreEqual("fourth", linkLabel.Links[3].LinkData);

			linkLabel.Text = "[Supongo que esto] es [lo que se necesita].";
			linkLabel.SetLinkRegions();
			Assert.AreEqual("Supongo que esto es lo que se necesita.", linkLabel.Text);
			Assert.AreEqual(2, linkLabel.Links.Count);
			Assert.AreEqual(0, linkLabel.Links[0].Start);
			Assert.AreEqual(16, linkLabel.Links[0].Length);
			Assert.AreEqual("first", linkLabel.Links[0].LinkData);
			Assert.AreEqual(20, linkLabel.Links[1].Start);
			Assert.AreEqual(18, linkLabel.Links[1].Length);

			// Ensure that setting it back to English restores the original text and links
			linkLabel.Text = kEnglishTextWithBrackets;
			linkLabel.SetLinkRegions();
			Assert.AreEqual("I guess this is the link to use for English.", linkLabel.Text);
			Assert.AreEqual(4, linkLabel.Links.Count);
			Assert.AreEqual(2, linkLabel.Links[0].Start);
			Assert.AreEqual(10, linkLabel.Links[0].Length);
			Assert.AreEqual("first", linkLabel.Links[0].LinkData);
			Assert.AreEqual(16, linkLabel.Links[1].Start);
			Assert.AreEqual(8, linkLabel.Links[1].Length);
			Assert.AreEqual("second", linkLabel.Links[1].LinkData);
			Assert.AreEqual(28, linkLabel.Links[2].Start);
			Assert.AreEqual(3, linkLabel.Links[2].Length);
			Assert.AreEqual("third", linkLabel.Links[2].LinkData);
			Assert.AreEqual(36, linkLabel.Links[3].Start);
			Assert.AreEqual(7, linkLabel.Links[3].Length);
			Assert.AreEqual("fourth", linkLabel.Links[3].LinkData);
		}
	}
}
