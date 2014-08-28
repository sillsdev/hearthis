using HearThis.Publishing;
using NUnit.Framework;

namespace HearThisTests
{
	/// <summary>
	/// Tests the ScriptureAppBuilderPublishingMethod class
	/// </summary>
	[TestFixture]
	public class ScriptureAppBuilderPublishingMethodTests
	{
		[Test]
		public void GetFilePathWithoutExtension_RepeatedAndOutOfOrderCalls_NamesBasedOnLanguageBookAndChapter()
		{
			var testMethod = new ScriptureAppBuilderPublishingMethod("xkal");
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\XKAL-GEN-001",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 1));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\XKAL-GEN-003",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 3));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\XKAL-GEN-003",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 3));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\XKAL-GEN-002",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 2));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\XKAL-GEN-004",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 4));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Leviticus-003\XKAL-LEV-001",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Leviticus", 1));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\XKAL-GEN-002",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 2));
		}
	}
}
