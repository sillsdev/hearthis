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
				Assert.AreEqual(dir + @"\Genesis-001\XKAL-GEN-001",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 1));
				Assert.AreEqual(dir + @"\Genesis-001\XKAL-GEN-003",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 3));
				Assert.AreEqual(dir + @"\Genesis-001\XKAL-GEN-003",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 3));
				Assert.AreEqual(dir + @"\Genesis-001\XKAL-GEN-002",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 2));
				Assert.AreEqual(dir + @"\Genesis-001\XKAL-GEN-004",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 4));
				Assert.AreEqual(dir + @"\Leviticus-003\XKAL-LEV-001",
					testMethod.GetFilePathWithoutExtension(dir, "Leviticus", 1));
				Assert.AreEqual(dir + @"\Genesis-001\XKAL-GEN-002",
					testMethod.GetFilePathWithoutExtension(dir, "Genesis", 2));
			}
		}
	}
}
