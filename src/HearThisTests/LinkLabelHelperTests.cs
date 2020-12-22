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
		[TestCase("This is some text without curly braces")]
		[TestCase("This is some text with opening {curly brace")]
		[TestCase("This is some text with closing} curly brace")]
		[TestCase("This is some text with} mismatched {curly braces")]
		public void SetLinkRegions_TextWithNoCurlyBraces_LinkRegionSetToEntireText(string text)
		{
			var linkLabel = new LinkLabel {Text = text };
			linkLabel.Links[0].LinkData = "don't lose this";
			linkLabel.SetLinkRegions();
			Assert.AreEqual(text, linkLabel.Text);
			Assert.AreEqual(0, linkLabel.LinkArea.Start);
			Assert.AreEqual(text.Length, linkLabel.LinkArea.Length);
			Assert.AreEqual("don't lose this", linkLabel.Links[0].LinkData);
			if (text.StartsWith("This is some text"))
			{
				linkLabel.Links[0].Start = 8;
				linkLabel.Links[0].Length = 4;
				linkLabel.SetLinkRegions();
				Assert.AreEqual(0, linkLabel.LinkArea.Start);
				Assert.AreEqual(text.Length, linkLabel.LinkArea.Length);
				Assert.AreEqual("don't lose this", linkLabel.Links[0].LinkData);
			}
		}

		[Test]
		public void SetLinkRegions_CurlyBracesAroundEntireText_LinkRegionSetToEntireTextWithCurlyBracesRemoved()
		{
			var linkLabel = new LinkLabel { Text = "{This whole thing is the link}",
				LinkArea = new LinkArea(6, 7)};
			linkLabel.SetLinkRegions();
			Assert.AreEqual("This whole thing is the link", linkLabel.Text);
			Assert.AreEqual(0, linkLabel.LinkArea.Start);
			Assert.AreEqual(linkLabel.Text.Length, linkLabel.LinkArea.Length);
		}

		[Test]
		public void SetLinkRegions_CurlyBracesAroundStartOfText_LinkRegionSetToPartOfTextInCurlyBraces()
		{
			var linkLabel = new LinkLabel { Text = "{This} was the link",
				LinkArea = new LinkArea(6, 7)};
			linkLabel.SetLinkRegions();
			Assert.AreEqual("This was the link", linkLabel.Text);
			Assert.AreEqual(0, linkLabel.LinkArea.Start);
			Assert.AreEqual(4, linkLabel.LinkArea.Length);
		}

		[Test]
		public void SetLinkRegions_CurlyBracesAroundEndOfText_LinkRegionSetToPartOfTextInCurlyBraces()
		{
			var linkLabel = new LinkLabel { Text = "This is the {link to use}",
				LinkArea = new LinkArea(3, 5)};
			linkLabel.SetLinkRegions();
			Assert.AreEqual("This is the link to use", linkLabel.Text);
			Assert.AreEqual(12, linkLabel.LinkArea.Start);
			Assert.AreEqual(11, linkLabel.LinkArea.Length);
		}

		[TestCase("This is the {link to use}.", 12, 11)]
		[TestCase("This is a short {l}ink.", 16, 1)]
		public void SetLinkRegions_CurlyBracesAroundPartOfText_LinkRegionSetToPartOfTextInCurlyBraces(
			string text, int expectedStart, int expectedEnd)
		{
			var linkLabel = new LinkLabel { Text = text,
				LinkArea = new LinkArea(0, 1)};
			// Since the normal behavior of a LinkLabel is to reset thw LinkData, we
			// also need to make sure that we preserve this when dynamically setting
			// the link region.
			linkLabel.Links[0].LinkData = "frog";
			linkLabel.SetLinkRegions();
			Assert.AreEqual(text.Replace("{", "").Replace("}", ""), linkLabel.Text);
			Assert.AreEqual(expectedStart, linkLabel.LinkArea.Start);
			Assert.AreEqual(expectedEnd, linkLabel.LinkArea.Length);
			Assert.AreEqual(1, linkLabel.Links.Count);
			Assert.AreEqual("frog", linkLabel.Links[0].LinkData as string);
		}

		/// <summary>
		/// This is a weird and somewhat arbitrary case.
		/// </summary>
		[Test]
		public void SetLinkRegions_CurlyBracesAroundMoreBitsOfTextThanInLinksCollection_LinkRegionSetToFirstTextInCurlyBraces()
		{
			var linkLabel = new LinkLabel { Text = "Is {this} the {link to use}?",
				LinkArea = new LinkArea(12, 11)};
			linkLabel.SetLinkRegions();
			Assert.AreEqual("Is this the {link to use}?", linkLabel.Text);
			Assert.AreEqual(3, linkLabel.LinkArea.Start);
			Assert.AreEqual(4, linkLabel.LinkArea.Length);
		}

		[Test]
		public void SetLinkRegions_MultipleLinks_LinkRegionsMatchedToTextInCurlyBraces()
		{
			var linkLabel = new LinkLabel { Text = "I guess {this} is the {link} to {use} for stuff." };
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
		public void SetLinkRegions_FewerCurlyBracesThanOriginalLinks_LinkRegionsToWhateverIsThere()
		{
			var linkLabel = new LinkLabel { Text = "I {guess this} is {the link} to {use} for {English}." };
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

			linkLabel.Text = "{Supongo que esto} es {lo que se necesita}.";
			linkLabel.SetLinkRegions();
			Assert.AreEqual("Supongo que esto es lo que se necesita.", linkLabel.Text);
			Assert.AreEqual(2, linkLabel.Links.Count);
			Assert.AreEqual(0, linkLabel.Links[0].Start);
			Assert.AreEqual(16, linkLabel.Links[0].Length);
			Assert.AreEqual("first", linkLabel.Links[0].LinkData);
			Assert.AreEqual(20, linkLabel.Links[1].Start);
			Assert.AreEqual(18, linkLabel.Links[1].Length);

			// Ensure that setting it bakc to English restores the original text and links
			linkLabel.Text = "I {guess this} is {the link} to {use} for {English}.";
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
