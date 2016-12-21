using HearThis;
using NUnit.Framework;

namespace HearThisTests
{
#if DEBUG
	[TestFixture]
	class UtilsTests
	{
		[Test]
		public void AreWordsIdentical_Identical_ReturnsTrue()
		{
			Assert.IsTrue(Utils.AreWordsIdentical("The cat", "The cat"));
		}

		[TestCase("The cat", "The  cat")]
		[TestCase("The  cat", "The cat")]
		[TestCase("   The  cat ", " The cat")]
		public void AreWordsIdentical_IdenticalExceptWhiteSpace_ReturnsTrue(string str1, string str2)
		{
			Assert.IsTrue(Utils.AreWordsIdentical(str1, str2));
		}

		[TestCase("The. cat", "The; cat")]
		[TestCase("This is a cat", "This is a cat.")]
		[TestCase("\"This is a cat\"", "This is a cat")]
		public void AreWordsIdentical_IdenticalExceptPunctuation_ReturnsTrue(string str1, string str2)
		{
			Assert.IsTrue(Utils.AreWordsIdentical(str1, str2));
		}

		//[TestCase("The.\" !cat", "The; cat")]
		//[TestCase("This   is . a cat", "This is a cat.")]
		//[TestCase("\"This is a cat\"", "This_ 'is a cat!!!")]
		//public void AreWordsIdentical_IdenticalExceptComplexPunctuationAndSpaceSequences_ReturnsTrue(string str1, string str2)
		//{
		//	Assert.IsTrue(Utils.AreWordsIdentical(str1, str2));
		//}

		[Test]
		public void AreWordsIdentical_IdenticalExceptNumber_ReturnsFalse()
		{
			Assert.IsFalse(Utils.AreWordsIdentical("There were 70 men.", "There were 700 men."));
		}

		[TestCase("The cat's meow", "The cats meow.")]
		[TestCase("The cats' meow", "The cats meow.")]
		[TestCase("The cat's meow", "The cats' meow.")]
		public void AreWordsIdentical_IdenticalExceptApostrophe_ReturnsFalse(string str1, string str2)
		{
			Assert.IsFalse(Utils.AreWordsIdentical(str1, str2));
		}

		[Test]
		public void AreWordsIdentical_DifferentWords_ReturnsFalse()
		{
			Assert.IsFalse(Utils.AreWordsIdentical("The cat", "The dog"));
		}

		[TestCase("Cats & dogs are strange bed-fellows", "Cats & dogs are strange bedfellows")]
		[TestCase("Cats & dogs are strange bed-fellows", "Cats & dogs are strange bed_fellows")]
		[TestCase("Cats&dogs are strange.", "Cats & dogs are strange.")]
		public void AreWordsIdentical_DifferentWordMedialPunctuation_ReturnsFalse(string str1, string str2)
		{
			Assert.IsFalse(Utils.AreWordsIdentical(str1, str2));
		}
	}
#endif
}
