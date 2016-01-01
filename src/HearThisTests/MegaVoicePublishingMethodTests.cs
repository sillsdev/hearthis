using System.IO;
using HearThis.Publishing;
using NUnit.Framework;
using SIL.IO;

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
			using (var tempFile = TempFile.WithFilenameInTempFolder("tempFile"))
			{
				string dir = Path.GetDirectoryName(tempFile.Path);
				var testMethod = new MegaVoicePublishingMethod();
				Assert.AreEqual(dir + @"\Genesis-001\Genesis-001",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 1));
				Assert.AreEqual(dir + @"\Genesis-001\Genesis-002",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 3));
				Assert.AreEqual(dir + @"\Genesis-001\Genesis-002",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 3));
				Assert.AreEqual(dir + @"\Genesis-001\Genesis-003",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 2));
				Assert.AreEqual(dir + @"\Genesis-001\Genesis-004",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 4));
				Assert.AreEqual(dir + @"\Leviticus-003\Leviticus-001",
					testMethod.GetFilePathWithoutExtension(dir, "Leviticus", 1));
				Assert.AreEqual(dir + @"\Genesis-001\Genesis-003",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 2));
			}
		}
	}
}
