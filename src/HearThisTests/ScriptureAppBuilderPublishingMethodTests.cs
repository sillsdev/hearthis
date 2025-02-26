using System.IO;
using HearThis.Publishing;
using NUnit.Framework;
using SIL.IO;

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
			using (var tempFile = TempFile.WithFilenameInTempFolder("tempFile"))
			{
				string dir = Path.GetDirectoryName(tempFile.Path);
				var testMethod = new ScriptureAppBuilderPublishingMethod("xkal");
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 1),
					Is.EqualTo(dir + @"\Genesis-001\XKAL-GEN-001"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 3),
					Is.EqualTo(dir + @"\Genesis-001\XKAL-GEN-003"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 3),
					Is.EqualTo(dir + @"\Genesis-001\XKAL-GEN-003"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 2),
					Is.EqualTo(dir + @"\Genesis-001\XKAL-GEN-002"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 4),
					Is.EqualTo(dir + @"\Genesis-001\XKAL-GEN-004"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Leviticus", 1),
					Is.EqualTo(dir + @"\Leviticus-003\XKAL-LEV-001"));
				Assert.That(testMethod.GetFilePathWithoutExtension(dir, "Genesis", 2),
					Is.EqualTo(dir + @"\Genesis-001\XKAL-GEN-002"));
			}
		}
	}
}
