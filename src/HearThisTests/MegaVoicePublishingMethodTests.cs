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
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 1),
					Is.EqualTo(dir + @"\Genesis-001\Genesis-001"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 3),
					Is.EqualTo(dir + @"\Genesis-001\Genesis-002"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 3),
					Is.EqualTo(dir + @"\Genesis-001\Genesis-002"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 2),
					Is.EqualTo(dir + @"\Genesis-001\Genesis-003"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 4),
					Is.EqualTo(dir + @"\Genesis-001\Genesis-004"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Leviticus", 1),
					Is.EqualTo(dir + @"\Leviticus-003\Leviticus-001"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 2),
					Is.EqualTo(dir + @"\Genesis-001\Genesis-003"));
			}
		}
	}
}
