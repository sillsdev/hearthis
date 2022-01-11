using System.Linq;
using HearThis.StringDifferences;
using NUnit.Framework;

namespace HearThisTests.Utils.StringDifferences
{
	[TestFixture]
	class StringDifferenceFinderTests
	{
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
		[TestCase("This is grand.", "Those were bad")]
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

		[TestCase("This", "This is even more.")]
		[TestCase("Am I ", "Am I a substring?")]
		public void ComputeDifferences_NewStringIsSubstringAtStartOfOrigString_SameStringFollowedByDeletion(
			string n, string o)
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

		[TestCase("is even", "This is even more.")]
		[TestCase(" I a ", "Am I a substring?")]
		public void ComputeDifferences_NewStringIsSubstringInMiddleOfOrigString_DeletionBeforeAndAfterSameString(
			string n, string o)
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

		[TestCase("even more.", "This is even more.")]
		[TestCase(" a substring?", "Am I a substring?")]
		public void ComputeDifferences_NewStringIsSubstringAtEndOfOrigString_DeletionFollowedBySameString(
			string n, string o)
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
	}
}
