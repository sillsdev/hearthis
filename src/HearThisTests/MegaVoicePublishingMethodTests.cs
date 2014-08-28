using HearThis.Publishing;
using NUnit.Framework;

namespace HearThisTests
{
	/// <summary>
	/// Tests the MegaVoicePublishingMethod class
	/// </summary>
	[TestFixture]
	public class MegaVoicePublishingMethodTests
	{
		[Test]
		public void GetFilePathWithoutExtension_RepeatedAndOutOfOrderCalls_NumbersAssignedSequentiallyAndReturnedConsistentlyForRepeatCalls()
		{
			var testMethod = new MegaVoicePublishingMethod();
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\Genesis-001",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 1));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\Genesis-002",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 3));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\Genesis-002",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 3));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\Genesis-003",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 2));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\Genesis-004",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 4));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Leviticus-003\Leviticus-001",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Leviticus", 1));
			Assert.AreEqual(@"C:\Users\pan\Desktop\Genesis-001\Genesis-003",
				testMethod.GetFilePathWithoutExtension(@"C:\Users\pan\Desktop", "Genesis", 2));
		}
	}
}
