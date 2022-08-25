using System;
using HearThis.StringDifferences;
using NUnit.Framework;
using System.Linq;
using System.Text;
using SIL.WritingSystems;

namespace HearThisTests.Utils.StringDifferences
{
	[TestFixture]
	class StringDifferenceFinderTests
	{
		[OneTimeSetUp]
		public void SetUpFixture()
		{
			// Used to initialize SLDR first, then ICU, but that seems to be the cause of test
			// failures n this fixture on ONE of the available TeamCity agents. Not sure why.
			// JasonN tipped me off to this possible cause because FLEx was having similar issues.
			Icu.Wrapper.ConfineIcuVersions(70);
			Icu.Wrapper.Init();
			Sldr.Initialize();
			// Sanity check to make sure we have a version of ICU that will work
			Assert.That(double.Parse(Icu.Wrapper.UnicodeVersion), Is.GreaterThanOrEqualTo(13.0));
			Assert.That(Icu.Character.GetCharType(0x16FF0), Is.EqualTo(Icu.Character.UCharCategory.COMBINING_SPACING_MARK));
		}

		[TestCase("This is the same string.")]
		[TestCase("wow wow wow")]
		public void ComputeDifferences_IdenticalStrings_SingleSameString(string s)
		{
			var d = new StringDifferenceFinder(s, s);
			var origDiff = d.OriginalStringDifferences.Single(); 
			var newDiff = d.NewStringDifferences.Single(); 
			Assert.That(origDiff.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newDiff.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origDiff.Text, Is.EqualTo(s));
			Assert.That(newDiff.Text, Is.EqualTo(s));
		}

		[TestCase("This is a different string.", "xyz")]
		[TestCase("abcdefghijklklklklkl", "lzxywvutsrqpabge")]
		[TestCase("a", "z")]
		[TestCase("\uD800\uDC00\ud803\ude6d\udbff\udfff", "\udbff\udfef")] // Surrogate pairs
		[TestCase("\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u1793\u17c5\u200b\u178f\u17d2\u179a\u1784\u17cb\u200b\u1791\u17b8\u200b\u1785\u17b6\u17c6\u200b\u1799\u17b6\u1798 \u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u17a1\u17be\u1784\u200b\u1791\u17c5\u200b\u179b\u17be\u200b\u1794\u17c9\u1798 \u17a0\u17be\u1799\u200b\u1781\u17c6\u200b\u1798\u17be\u179b\u200b\u1791\u17c5 \u178a\u17be\u1798\u17d2\u1794\u17b8\u200b\u17b2\u17d2\u1799\u200b\u178a\u17b9\u1784\u200b\u1787\u17b6\u200b\u1791\u17d2\u179a\u1784\u17cb\u200b\u1793\u17b9\u1784\u200b\u1798\u17b6\u1793\u200b\u1796\u17d2\u179a\u17c7\u1794\u1793\u17d2\u1791\u17bc\u179b\u200b\u1798\u1780\u200b\u178a\u17bc\u1785\u200b\u1798\u17d2\u178f\u17c1\u1785 \u17a0\u17be\u1799\u200b\u1793\u17b9\u1784\u200b\u1786\u17d2\u179b\u17be\u1799\u200b\u1796\u17b8\u200b\u178a\u17c6\u178e\u17be\u179a\u200b\u178a\u17c2\u179b\u200b\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1785\u17c4\u1791\u200b\u1794\u17d2\u179a\u1780\u17b6\u1793\u17cb\u200b\u1787\u17b6\u200b\u1799\u17c9\u17b6\u1784\u200b\u178e\u17b6",
		     "new")]
		public void ComputeDifferences_NothingUsefulInCommon_SingleDeletionAndSingleAddition(string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			var origDiff = d.OriginalStringDifferences.Single(); 
			var newDiff = d.NewStringDifferences.Single(); 
			Assert.That(origDiff.Type, Is.EqualTo(DifferenceType.Deletion));
			Assert.That(newDiff.Type, Is.EqualTo(DifferenceType.Addition));
			Assert.That(origDiff.Text, Is.EqualTo(o));
			Assert.That(newDiff.Text, Is.EqualTo(n));
		}

		[TestCase(null, "new")]
		[TestCase("", "new")]
		[TestCase("old", null)]
		[TestCase("old", "")]
		[TestCase(null, "")]
		public void Constructor_NullOrEmptyString_ThrowsArgumentException(string o, string n)
		{
			Assert.That(() => new StringDifferenceFinder(o, n), Throws.ArgumentException);
		}

		[TestCase("These strings start the same.", "These strings end differently!")]
		[TestCase("Thisisgrand.", "Thosewerebad")] // Scriptio continua
		[TestCase("<Mine is awesome!", "<Yours were bad")]
		[TestCase("\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b, \u16c5\u201b\u178e\u17d1\u179d\u1794\u17ab\u201b\u1781\u16b8\u200d\u1795\u17a6\u17a5\u200a\u1798\u17b6\u1798 \u1782\u17d3\u1788\u17b8\u17c8\u2008\u1798\u17b8\u1788\u2008\u17a8\u17b8\u1788\u2008\u1798\u17c8\u2008\u1798\u17b8\u2008\u1798\u17c8\u1798\u17a8\u17b8\u1798\u2008\u1788\u17c8\u2008\u1797\u17b8\u1798\u2008\u1798\u17c8\u1788\u17b8\u1798\u17d8\u1798\u17b8\u2008\u17b8\u17d8\u1798\u2008\u1788\u17b8\u1788\u2008\u1788\u17b8\u2008\u1791\u17d8\u1793\u1783\u17c3\u2003\u1797\u17b7\u1787\u2007\u1797\u17b7\u1797\u2007\u1797\u17d7\u1797\u1777\u1797\u1797\u17d7\u1797\u177c\u1797\u2007\u1797\u1787\u2007\u1787\u177c\u1787\u2007\u1797\u17d7\u1787\u17c7\u1787\u17a7\u17b7\u1797\u2007\u1797\u17b7\u1786\u2006\u1766\u17d6\u176b\u17b6\u1796\u2006\u1766\u17b6\u206b\u176a\u1766\u176e\u17b6\u1796\u2006\u1786\u17c6\u1796\u2006\u1786",
			      "\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b; \u17c5\u200b\u178f\u17d2\u179a\u1784\u17cb\u200b\u1791\u17b8\u200b\u1785\u17b6\u17c6\u200b\u1799\u17a6\u1778 \u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u17a1\u17b6\u1784\u200b\u1791\u17c5\u200b\u179b\u17be\u200b\u1794\u17c9\u1798\u17a0\u17be\u1799\u200b\u1781\u17c6\u200b\u1798\u17be\u179b\u200b\u1791\u17c5\u178a\u17be\u1798\u17d2\u1794\u17b8\u200b\u17b2\u17d2\u1799\u200b\u178a\u17b9\u1784\u200b\u1787\u17b6\u200b\u1791\u17d2\u179a\u1784\u17cb\u200b\u1793\u17b9\u1784\u200b\u1798\u17b6\u1793\u200b\u1796\u17d2\u179a\u17c3\u1794\u1793\u17d2\u1791\u17bc\u179b\u200b\u1798\u1780\u200b\u178a\u17bc\u1785\u200b\u1798\u17d2\u178f\u17c1\u1785\u17a0\u17be\u1799\u200b\u1793\u17b9\u1784\u200b\u1786\u17d2\u179b\u17be\u1799\u200b\u1796\u17b8\u200b\u178a\u17c6\u178e\u17be\u179a\u200b\u178a\u17c2\u179b\u200b\u1781")] 
		public void ComputeDifferences_StringsStartTheSameButEndDifferently_SameStringFollowedByDifference(
			string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			Assert.That(d.OriginalStringDifferences.Count, Is.EqualTo(2));
			Assert.That(d.NewStringDifferences.Count, Is.EqualTo(2));
			var origSamePart = d.OriginalStringDifferences[0]; 
			var newSamePart = d.NewStringDifferences[0]; 
			Assert.That(origSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSamePart.Text, Is.EqualTo(newSamePart.Text));
			
			var origDeletion = d.OriginalStringDifferences[1]; 
			var newAddition = d.NewStringDifferences[1]; 
			Assert.That(origDeletion.Type, Is.EqualTo(DifferenceType.Deletion));
			Assert.That(newAddition.Type, Is.EqualTo(DifferenceType.Addition));
			Assert.That(o, Does.EndWith(origDeletion.Text));
			Assert.That(n, Does.EndWith(newAddition.Text));
			Assert.That(origDeletion.Text[0], Is.Not.EqualTo(newAddition.Text[0]));
			Assert.That(origSamePart.Text + origDeletion.Text, Is.EqualTo(o));
			Assert.That(newSamePart.Text + newAddition.Text, Is.EqualTo(n));
		}

		[TestCase("This is grand.", "Those were bad")]
		[TestCase("\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u16c5\u201b\u178e\u17d1\u179d\u1794\u17ab\u201b\u1781\u16b8\u200d\u1795\u17a6\u17a5\u200a\u1798\u17b6\u1798 \u1782\u17d3\u1788\u17b8\u17c8\u2008\u1798\u17b8\u1788\u2008\u17a8\u17b8\u1788\u2008\u1798\u17c8\u2008\u1798\u17b8\u2008\u1798\u17c8\u1798\u17a8\u17b8\u1798\u2008\u1788\u17c8\u2008\u1797\u17b8\u1798\u2008\u1798\u17c8\u1788\u17b8\u1798\u17d8\u1798\u17b8\u2008\u17b8\u17d8\u1798\u2008\u1788\u17b8\u1788\u2008\u1788\u17b8\u2008\u1791\u17d8\u1793\u1783\u17c3\u2003\u1797\u17b7\u1787\u2007\u1797\u17b7\u1797\u2007\u1797\u17d7\u1797\u1777\u1797\u1797\u17d7\u1797\u177c\u1797\u2007\u1797\u1787\u2007\u1787\u177c\u1787\u2007\u1797\u17d7\u1787\u17c7\u1787\u17a7\u17b7\u1797\u2007\u1797\u17b7\u1786\u2006\u1766\u17d6\u176b\u17b6\u1796\u2006\u1766\u17b6\u206b\u176a\u1766\u176e\u17b6\u1796\u2006\u1786\u17c6\u1796\u2006\u1786",
			"\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u17c5\u200b\u178f\u17d2\u179a\u1784\u17cb\u200b\u1791\u17b8\u200b\u1785\u17b6\u17c6\u200b\u1799\u17a6\u1778 \u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u17a1\u17b6\u1784\u200b\u1791\u17c5\u200b\u179b\u17be\u200b\u1794\u17c9\u1798\u17a0\u17be\u1799\u200b\u1781\u17c6\u200b\u1798\u17be\u179b\u200b\u1791\u17c5\u178a\u17be\u1798\u17d2\u1794\u17b8\u200b\u17b2\u17d2\u1799\u200b\u178a\u17b9\u1784\u200b\u1787\u17b6\u200b\u1791\u17d2\u179a\u1784\u17cb\u200b\u1793\u17b9\u1784\u200b\u1798\u17b6\u1793\u200b\u1796\u17d2\u179a\u17c3\u1794\u1793\u17d2\u1791\u17bc\u179b\u200b\u1798\u1780\u200b\u178a\u17bc\u1785\u200b\u1798\u17d2\u178f\u17c1\u1785\u17a0\u17be\u1799\u200b\u1793\u17b9\u1784\u200b\u1786\u17d2\u179b\u17be\u1799\u200b\u1796\u17b8\u200b\u178a\u17c6\u178e\u17be\u179a\u200b\u178a\u17c2\u179b\u200b\u1781")] 
		public void ComputeDifferences_StringsStartWithSameLettersButEndDifferently_SameStringFollowedByDifference(
			string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			Assert.That(d.OriginalStringDifferences.Count, Is.EqualTo(1));
			Assert.That(d.NewStringDifferences.Count, Is.EqualTo(1));
			var origDeletion = d.OriginalStringDifferences[0]; 
			var newAddition = d.NewStringDifferences[0]; 
			Assert.That(origDeletion.Type, Is.EqualTo(DifferenceType.Deletion));
			Assert.That(newAddition.Type, Is.EqualTo(DifferenceType.Addition));
			Assert.That(origDeletion.Text, Is.EqualTo(o));
			Assert.That(newAddition.Text, Is.EqualTo(n));
		}

		[TestCase("This", "This is even more.")]
		[TestCase("Am I ", "Am I a substring?")]
		public void ComputeDifferences_OrigStringIsSubstringAtStartOfNewString_SameStringFollowedByAddition(
			string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			Assert.That(d.NewStringDifferences.Count, Is.EqualTo(2));
			var origSamePart = d.OriginalStringDifferences.Single(); 
			var newSamePart = d.NewStringDifferences[0]; 
			Assert.That(origSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSamePart.Text, Is.EqualTo(newSamePart.Text));
			
			var newAddition = d.NewStringDifferences[1]; 
			Assert.That(newAddition.Type, Is.EqualTo(DifferenceType.Addition));
			Assert.That(n, Does.EndWith(newAddition.Text));
			Assert.That(newSamePart.Text + newAddition.Text, Is.EqualTo(n));
		}

		[TestCase("is even", "This is even more.")]
		[TestCase(" I a ", "Am I a substring?")]
		[TestCase("\ud803\ude6d", "\uD800\uDC00 \ud803\ude6d \udbff\udfff")]
		public void ComputeDifferences_OrigStringIsSubstringInMiddleOfNewString_AdditionBeforeAndAfterSameString(
			string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			Assert.That(d.NewStringDifferences.Count, Is.EqualTo(3));
			var newStart = d.NewStringDifferences[0];
			Assert.That(newStart.Type, Is.EqualTo(DifferenceType.Addition));
			Assert.That(n, Does.StartWith(newStart.Text));

			var origSamePart = d.OriginalStringDifferences.Single(); 
			var newSamePart = d.NewStringDifferences[1]; 
			Assert.That(origSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSamePart.Text, Is.EqualTo(newSamePart.Text));
			
			var newEnd = d.NewStringDifferences[2]; 
			Assert.That(newEnd.Type, Is.EqualTo(DifferenceType.Addition));
			Assert.That(n, Does.EndWith(newEnd.Text));
			Assert.That(newStart.Text + newSamePart.Text + newEnd.Text, Is.EqualTo(n));
		}

		[TestCase("even more.", "This is even more.")]
		[TestCase(" a substring?", "Am I a substring?")]
		public void ComputeDifferences_OrigStringIsSubstringAtEndOfNewString_AdditionFollowedBySameString(
			string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			Assert.That(d.NewStringDifferences.Count, Is.EqualTo(2));
			var newAddition = d.NewStringDifferences[0];
			Assert.That(newAddition.Type, Is.EqualTo(DifferenceType.Addition));
			Assert.That(n, Does.StartWith(newAddition.Text));

			var origSamePart = d.OriginalStringDifferences.Single(); 
			var newSamePart = d.NewStringDifferences[1]; 
			Assert.That(origSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSamePart.Text, Is.EqualTo(newSamePart.Text));
			
			Assert.That(newAddition.Text + newSamePart.Text , Is.EqualTo(n));
		}

		[TestCase("This is even more.", "This")]
		[TestCase("Am I a substring?", "Am I ")]
		public void ComputeDifferences_NewStringIsSubstringAtStartOfOrigString_SameStringFollowedByDeletion(
			string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			Assert.That(d.OriginalStringDifferences.Count, Is.EqualTo(2));
			var newSamePart = d.NewStringDifferences.Single(); 
			var origSamePart = d.OriginalStringDifferences[0]; 
			Assert.That(origSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSamePart.Text, Is.EqualTo(newSamePart.Text));
			
			var origDeletion = d.OriginalStringDifferences[1]; 
			Assert.That(origDeletion.Type, Is.EqualTo(DifferenceType.Deletion));
			Assert.That(o, Does.EndWith(origDeletion.Text));
			Assert.That(newSamePart.Text + origDeletion.Text, Is.EqualTo(o));
		}

		[TestCase("This is even more.", "is even")]
		[TestCase("Am I a substring?", " I a ")]
		public void ComputeDifferences_NewStringIsSubstringInMiddleOfOrigString_DeletionBeforeAndAfterSameString(
			string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			Assert.That(d.OriginalStringDifferences.Count, Is.EqualTo(3));
			var origStart = d.OriginalStringDifferences[0];
			Assert.That(origStart.Type, Is.EqualTo(DifferenceType.Deletion));
			Assert.That(o, Does.StartWith(origStart.Text));

			var newSamePart = d.NewStringDifferences.Single(); 
			var origSamePart = d.OriginalStringDifferences[1]; 
			Assert.That(origSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSamePart.Text, Is.EqualTo(newSamePart.Text));
			
			var origEnd = d.OriginalStringDifferences[2]; 
			Assert.That(origEnd.Type, Is.EqualTo(DifferenceType.Deletion));
			Assert.That(o, Does.EndWith(origEnd.Text));
			Assert.That(origStart.Text + newSamePart.Text + origEnd.Text, Is.EqualTo(o));
		}

		[TestCase("This is even more.", "even more.")]
		[TestCase("Am I a substring?", " a substring?")]
		[TestCase("\uD800\uDC00\ud803\ude6d\udbff\udfff", "\ud803\ude6d\udbff\udfff")] // Surrogate pairs
		public void ComputeDifferences_NewStringIsSubstringAtEndOfOrigString_DeletionFollowedBySameString(
			string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			Assert.That(d.OriginalStringDifferences.Count, Is.EqualTo(2));
			var origDeletion = d.OriginalStringDifferences[0];
			Assert.That(origDeletion.Type, Is.EqualTo(DifferenceType.Deletion));
			Assert.That(o, Does.StartWith(origDeletion.Text));

			var newSamePart = d.NewStringDifferences.Single(); 
			var origSamePart = d.OriginalStringDifferences[1]; 
			Assert.That(origSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSamePart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSamePart.Text, Is.EqualTo(newSamePart.Text));
			
			Assert.That(origDeletion.Text + origSamePart.Text , Is.EqualTo(o));
		}

		// HT-444
		[TestCase("This is evenmore embarrassing.", "This is even more embarrassing.")]
		public void ComputeDifferences_SpaceAddedBetweenWords_AdditionShowsSurroundingWords(
			string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			Assert.That(d.OriginalStringDifferences.Count, Is.EqualTo(3));
			Assert.That(d.OriginalStringDifferences[0].Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(d.NewStringDifferences[0].Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(d.OriginalStringDifferences[1].Type, Is.EqualTo(DifferenceType.Deletion));
			Assert.That(d.NewStringDifferences[1].Type, Is.EqualTo(DifferenceType.Addition));
			Assert.That(d.OriginalStringDifferences[2].Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(d.NewStringDifferences[2].Type, Is.EqualTo(DifferenceType.Same));

			var origDeletion = d.OriginalStringDifferences[1];
			var newAddition = d.NewStringDifferences[1];
			var delText = origDeletion.Text;
			var addText = newAddition.Text;
			Assert.That(o, Does.Contain(delText));
			Assert.That(n, Does.Contain(addText));
			Assert.That(delText, Does.Not.Contain(" "));
			var indexOfSpace = newAddition.Text.IndexOf(" ", StringComparison.Ordinal);
			Assert.That(indexOfSpace, Is.GreaterThan(0));
			Assert.That(newAddition.Text.Length, Is.EqualTo(origDeletion.Text.Length + 1));
			Assert.That(delText, Is.EqualTo(addText.Substring(0, indexOfSpace) +
				addText.Substring(indexOfSpace + 1)));

			Assert.That(d.OriginalStringDifferences[0].Text + delText + d.OriginalStringDifferences[2].Text, Is.EqualTo(o));
		}

		[TestCase("No puede el mundo aborreceros a vosotros; mas a mí me aborrece, porque yo testifico de él, que sus obras son malas.",
			"No puede el mundo aborrezeros a vozotros, mas a mí me aborrece porque yo testifico de él, que sus hobras son malas.")]
		public void ComputeDifferences_MultipleSmallSpellingChanges_SameStartAndEndAndLongestCommonMiddle(
			string o, string n)
		{
			var d = new StringDifferenceFinder(o, n);
			Assert.That(d.OriginalStringDifferences.Count, Is.EqualTo(5));
			Assert.That(d.NewStringDifferences.Count, Is.EqualTo(5));

			var origSameStart = d.OriginalStringDifferences[0];
			var newSameStart = d.NewStringDifferences[0];
			Assert.That(origSameStart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSameStart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSameStart.Text, Is.EqualTo(newSameStart.Text));

			var origDeletion1 = d.OriginalStringDifferences[1];
			Assert.That(origDeletion1.Type, Is.EqualTo(DifferenceType.Deletion));

			var newAddition1 = d.NewStringDifferences[1];
			Assert.That(newAddition1.Type, Is.EqualTo(DifferenceType.Addition));

			var origSameMiddle = d.OriginalStringDifferences[2];
			var newSameMiddle = d.NewStringDifferences[2];
			Assert.That(origSameMiddle.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSameMiddle.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSameMiddle.Text, Is.EqualTo(newSameMiddle.Text));

			var origDeletion3 = d.OriginalStringDifferences[3];
			Assert.That(origDeletion3.Type, Is.EqualTo(DifferenceType.Deletion));

			var newAddition3 = d.NewStringDifferences[3];
			Assert.That(newAddition3.Type, Is.EqualTo(DifferenceType.Addition));

			var origSameEnd = d.OriginalStringDifferences.Last(); 
			var newSameEnd = d.NewStringDifferences.Last(); 
			Assert.That(origSameEnd.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSameEnd.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSameEnd.Text, Is.EqualTo(newSameEnd.Text));
			
			Assert.That(origSameStart.Text + origDeletion1.Text + origSameMiddle.Text +
				origDeletion3.Text + origSameEnd.Text, Is.EqualTo(o));
			Assert.That(newSameStart.Text + newAddition1.Text + newSameMiddle.Text +
				newAddition3.Text + newSameEnd.Text, Is.EqualTo(n));
		}
		
		[TestCase("No puede el mundo aborreceros a vosotros; mas a mi me aborrece, porque yo testifico de él, que sus obras son malas.",
			"No puede el mundo aborreceros a vosotros; mas a mí me aborrece, porque yo testifico de el, que sus obras son malas.",
			"mi", "mí", "él", "el", NormalizationForm.FormC)]
		[TestCase("No puede el mundo aborreceros a vosotros; mas a mi me aborrece, porque yo testifico de él, que sus obras son malas.",
			"No puede el mundo aborreceros a vosotros; mas a mí me aborrece, porque yo testifico de el, que sus obras son malas.",
			"mi", "mí", "él", "el")]
		// Multiple single-character diacritics and diacritics that apply to two base characters
		// U+035C : Combining Double Breve Below -- applies to leading and trailing character
		// U+0360 : Combining Double Tilde -- applies to leading and trailing character
		// U+0308 : Combining Diaeresis (double dot above, umlaut, etc.) -- applies to single leading character
		// U+0324 : Latin Small Letter N with Acute (precomposed)
		// U+20DD : Combining Enclosing Circle  -- applies to single leading character
		[TestCase("No puede el mundo aborreceros a vosotros; mas a mi me aborrece, porque yo testifico de el, que su\u0308\u0324s\u20DD obras son malas.",
			"No puede el mundo aborreceros a vosotros; ma\u035Cs a m\u0360i me aborrece, porque yo testifico de el, que sus obras son malas.",
			"mas a mi", "ma\u035Cs a m\u0360i", "su\u0308\u0324s\u20DD", "sus")]
		// overlaying diacritic and zero-width non-joiner (U+200C)
		[TestCase("No puede el mundo aborreceros a vosotros; mask a mi me aborrece, porque yo testifico de el, que sus o\u20E6bras son malas.",
			"No puede el mundo aborreceros a vosotros; mas\u200C\u035C a mi me aborrece, porque yo testifico de el, que sus obras son malas.",
			"mask", "mas\u200C\u035C", "o\u20E6bras", "obras")]
		// zero-width joiner (U+200D) between two base characters
		[TestCase("No puede el mundo aborreceros a vosotros; mas a mi me aborrece, porque yo testifico de el, que sus o\u200Db\u200Dr\u200Da\u200Ds son malas.",
			"No puede el mundo aborreceros a vosotros; ma\u200Ds a mi me aborrece, porque yo testifico de el, que sus obras son malas.",
			"mas", "ma\u200Ds", "o\u200Db\u200Dr\u200Da\u200Ds", "obras")]
		// Remainder of text cases are for combining spacing marks - treat whole word as different
		// Khmer vowel differences (text also has viramas)
		// Here are the differences (*):                                                                                                                                                                                                                                 *                                                                                                                                                                                                                                                                                                                                                                         *
		[TestCase("\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u1793\u17c5\u200b\u178f\u17d2\u179a\u1784\u17cb\u200b\u1791\u17b8\u200b\u1785\u17b6\u17c6\u200b\u1799\u17b6\u1798 \u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u17a1\u17be\u1784\u200b\u1791\u17c5\u200b\u179b\u17be\u200b\u1794\u17c9\u1798 \u17a0\u17be\u1799\u200b\u1781\u17c6\u200b\u1798\u17be\u179b\u200b\u1791\u17c5 \u178a\u17be\u1798\u17d2\u1794\u17b8\u200b\u17b2\u17d2\u1799\u200b\u178a\u17b9\u1784\u200b\u1787\u17b6\u200b\u1791\u17d2\u179a\u1784\u17cb\u200b\u1793\u17b9\u1784\u200b\u1798\u17b6\u1793\u200b\u1796\u17d2\u179a\u17c7\u1794\u1793\u17d2\u1791\u17bc\u179b\u200b\u1798\u1780\u200b\u178a\u17bc\u1785\u200b\u1798\u17d2\u178f\u17c1\u1785 \u17a0\u17be\u1799\u200b\u1793\u17b9\u1784\u200b\u1786\u17d2\u179b\u17be\u1799\u200b\u1796\u17b8\u200b\u178a\u17c6\u178e\u17be\u179a\u200b\u178a\u17c2\u179b\u200b\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1785\u17c4\u1791\u200b\u1794\u17d2\u179a\u1780\u17b6\u1793\u17cb\u200b\u1787\u17b6\u200b\u1799\u17c9\u17b6\u1784\u200b\u178e\u17b6",
			      "\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u1793\u17c5\u200b\u178f\u17d2\u179a\u1784\u17cb\u200b\u1791\u17b8\u200b\u1785\u17b6\u17c6\u200b\u1799\u17b6\u1798 \u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u17a1\u17b6\u1784\u200b\u1791\u17c5\u200b\u179b\u17be\u200b\u1794\u17c9\u1798 \u17a0\u17be\u1799\u200b\u1781\u17c6\u200b\u1798\u17be\u179b\u200b\u1791\u17c5 \u178a\u17be\u1798\u17d2\u1794\u17b8\u200b\u17b2\u17d2\u1799\u200b\u178a\u17b9\u1784\u200b\u1787\u17b6\u200b\u1791\u17d2\u179a\u1784\u17cb\u200b\u1793\u17b9\u1784\u200b\u1798\u17b6\u1793\u200b\u1796\u17d2\u179a\u17c3\u1794\u1793\u17d2\u1791\u17bc\u179b\u200b\u1798\u1780\u200b\u178a\u17bc\u1785\u200b\u1798\u17d2\u178f\u17c1\u1785 \u17a0\u17be\u1799\u200b\u1793\u17b9\u1784\u200b\u1786\u17d2\u179b\u17be\u1799\u200b\u1796\u17b8\u200b\u178a\u17c6\u178e\u17be\u179a\u200b\u178a\u17c2\u179b\u200b\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1785\u17c4\u1791\u200b\u1794\u17d2\u179a\u1780\u17b6\u1793\u17cb\u200b\u1787\u17b6\u200b\u1799\u17c9\u17b6\u1784\u200b\u178e\u17b6",
			//                                                                      *
			"\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u17a1\u17be\u1784\u200b\u1791\u17c5\u200b\u179b\u17be\u200b\u1794\u17c9\u1798",
			"\u1781\u17d2\u1789\u17bb\u17c6\u200b\u1793\u17b9\u1784\u200b\u17a1\u17b6\u1784\u200b\u1791\u17c5\u200b\u179b\u17be\u200b\u1794\u17c9\u1798",
			//                                                                                                                                                                                                                      *
			"\u178a\u17be\u1798\u17d2\u1794\u17b8\u200b\u17b2\u17d2\u1799\u200b\u178a\u17b9\u1784\u200b\u1787\u17b6\u200b\u1791\u17d2\u179a\u1784\u17cb\u200b\u1793\u17b9\u1784\u200b\u1798\u17b6\u1793\u200b\u1796\u17d2\u179a\u17c7\u1794\u1793\u17d2\u1791\u17bc\u179b\u200b\u1798\u1780\u200b\u178a\u17bc\u1785\u200b\u1798\u17d2\u178f\u17c1\u1785",
			"\u178a\u17be\u1798\u17d2\u1794\u17b8\u200b\u17b2\u17d2\u1799\u200b\u178a\u17b9\u1784\u200b\u1787\u17b6\u200b\u1791\u17d2\u179a\u1784\u17cb\u200b\u1793\u17b9\u1784\u200b\u1798\u17b6\u1793\u200b\u1796\u17d2\u179a\u17c3\u1794\u1793\u17d2\u1791\u17bc\u179b\u200b\u1798\u1780\u200b\u178a\u17bc\u1785\u200b\u1798\u17d2\u178f\u17c1\u1785")]
		// Devanagari virama differences (text also has vowels which are combining marks)
		[TestCase("का आगाज मार्च के आखिरी हँसी सदाऽऽत्मा गगा लगभग.", "का आगाज मार्च के आखिरी हसॲ सदाऽऽत्मा गंगा लगभग.",
			"हँसी", "हसॲ", "गगा", "गंगा")]
		// Lontara script (combining marks but no viramas)
		// Here are the differences (*):                      *                                                                                                          *
		[TestCase("\u1a00\u1a17\u1a08\u1a08, \u1a00\u1a11\u1a19\u1a05-\u1a06\u1a09\u1a19\u1a0c\u1a19\u1a0a\u1a01, \u1a00\u1a11\u1a19\u1a05, \u1a06\u1a09\u1a19\u1a0c\u1a1A\u1a0a\u1a01, \u1a05\u1a14-\u1a15\u1a18\u1a01\u1a17, \u1a15\u1a18\u1a01\u1a17, \u1a12\u1a1a\u1a08\u1a11, \u1a15\u1a18\u1a11\u1a18\u1a04\u1a18-\u1a14\u1a18\u1a12\u1a04-\u1a15\u1a1b\u1a04",
			      "\u1a00\u1a17\u1a08\u1a08, \u1a00\u1a11\u1a1A\u1a05-\u1a06\u1a09\u1a19\u1a0c\u1a19\u1a0a\u1a01, \u1a00\u1a11\u1a19\u1a05, \u1a06\u1a09\u1a19\u1a0c\u1a19\u1a0a\u1a01, \u1a05\u1a14-\u1a15\u1a18\u1a01\u1a17, \u1a15\u1a18\u1a01\u1a17, \u1a12\u1a1a\u1a08\u1a11, \u1a15\u1a18\u1a11\u1a18\u1a04\u1a18-\u1a14\u1a18\u1a12\u1a04-\u1a15\u1a1b\u1a04",
			// ENHANCE: In a perfect world, we would probably prefer not to include the identical
			// trailing punctuation as part of the difference, but GetLongestUsefulCommonSubstring
			// currently treats that as part of the preceding word.
			//                *
			"\u1a00\u1a11\u1a19\u1a05-\u1a06\u1a09\u1a19\u1a0c\u1a19\u1a0a\u1a01,",
			"\u1a00\u1a11\u1a1A\u1a05-\u1a06\u1a09\u1a19\u1a0c\u1a19\u1a0a\u1a01,",
			//                            *
			"\u1a06\u1a09\u1a19\u1a0c\u1a1A\u1a0a\u1a01",
			"\u1a06\u1a09\u1a19\u1a0c\u1a19\u1a0a\u1a01")]
		// CJK - Surrogate pairs
		// Here are the differences (*):                                                 *------------------*                                *------------------*
		[TestCase("\U00020000\U00020001\U00020007\U00020037\U000200D1\U000200C7\U00020056\U000200D9\U000201D9 \U00020167\U00020193\U000200C7 \U00020177\U000201A1\U000202C8\U0002023A\U0002024F",
			      "\U00020000\U00020001\U00020007\U00020037\U000200D1\U000200C7\U00020056\U000200F8 \U00020167\U00020193\U000200C7 \U00020177\U00020193\U000202C8\U0002023A\U0002024F",
			"\U000200D9\U000201D9", "\U000200F8", "\U00020177\U000201A1", "\U00020177\U00020193")]
		// CJK + Han Reading Combining Class (U+16FF0 & U+16FF1) - Surrogate pairs
		// Here are the differences (*):                                                            *                                                   *
		[TestCase("\U00020000\U00020001\U00020007 \U00020037\U000200D1\U000200C7 \U00020056\U000200D9 \U00020167\U00020193\U000200C7 \U00020167\U00016FF1\U00020193\U000200C7 \U0002023A\U0002024F",
			      "\U00020000\U00020001\U00020007 \U00020037\U000200D1\U000200C7 \U00020056\U000200D9\U00016FF0 \U00020167\U00020193\U000200C7 \U00020167\U00020193\U000200C7 \U0002023A\U0002024F",
			"\U00020056\U000200D9", "\U00020056\U000200D9\U00016FF0", "\U00020167\U00016FF1\U00020193\U000200C7", "\U00020167\U00020193\U000200C7")]
		public void ComputeDifferences_DiacriticChanges_BaseCharactersAreKeptTogetherWithDiacritics(
			string o, string n, string del1, string add1, string del2, string add2,
			NormalizationForm normalization = NormalizationForm.FormD)
		{
			var d = new StringDifferenceFinder(o.Normalize(normalization), n.Normalize(normalization));
			Assert.That(d.OriginalStringDifferences.Count, Is.EqualTo(5));
			Assert.That(d.NewStringDifferences.Count, Is.EqualTo(5));

			var origSameStart = d.OriginalStringDifferences[0];
			var newSameStart = d.NewStringDifferences[0];
			Assert.That(origSameStart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSameStart.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSameStart.Text, Is.EqualTo(newSameStart.Text));

			// We use Trim here because the current algorithm can include
			// surrounding whitespace. It could be argued that that is incorrect,
			// but for our purposes we really don't care.
			var origDeletion1 = d.OriginalStringDifferences[1];
			Assert.That(origDeletion1.Type, Is.EqualTo(DifferenceType.Deletion));
			Assert.That(origDeletion1.Text.Trim(), Is.EqualTo(del1));

			var newAddition1 = d.NewStringDifferences[1];
			Assert.That(newAddition1.Type, Is.EqualTo(DifferenceType.Addition));
			Assert.That(newAddition1.Text.Trim(), Is.EqualTo(add1.Normalize(normalization)));

			var origSameMiddle = d.OriginalStringDifferences[2];
			var newSameMiddle = d.NewStringDifferences[2];
			Assert.That(origSameMiddle.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSameMiddle.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSameMiddle.Text, Is.EqualTo(newSameMiddle.Text));

			// We use Trim here because the current algorithm can include
			// surrounding whitespace. It could be argued that that is incorrect,
			// but for our purposes we really don't care.
			var origDeletion3 = d.OriginalStringDifferences[3];
			Assert.That(origDeletion3.Type, Is.EqualTo(DifferenceType.Deletion));
			Assert.That(origDeletion3.Text.Trim(), Is.EqualTo(del2.Normalize(normalization)));

			var newAddition3 = d.NewStringDifferences[3];
			Assert.That(newAddition3.Type, Is.EqualTo(DifferenceType.Addition));
			Assert.That(newAddition3.Text.Trim(), Is.EqualTo(add2));

			var origSameEnd = d.OriginalStringDifferences.Last(); 
			var newSameEnd = d.NewStringDifferences.Last(); 
			Assert.That(origSameEnd.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(newSameEnd.Type, Is.EqualTo(DifferenceType.Same));
			Assert.That(origSameEnd.Text, Is.EqualTo(newSameEnd.Text));
			
			Assert.That((origSameStart.Text + origDeletion1.Text + origSameMiddle.Text +
				origDeletion3.Text + origSameEnd.Text).Normalize(normalization),
				Is.EqualTo(o.Normalize(normalization)));
			Assert.That((newSameStart.Text + newAddition1.Text + newSameMiddle.Text +
				newAddition3.Text + newSameEnd.Text).Normalize(normalization),
				Is.EqualTo(n.Normalize(normalization)));
		}
	}
}
