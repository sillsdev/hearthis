using HearThis.Properties;
using HearThis.Script;
using NUnit.Framework;
using SIL.Extensions;
using SIL.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HearThisTests
{
	[TestFixture]
	class SkippedScriptLinesTests
	{
		private List<string> _stylesToSkipByDefault;
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_stylesToSkipByDefault = new TestScriptProvider().StylesToSkipByDefault.ToList();
			Assert.AreEqual(5, _stylesToSkipByDefault.Count,
				"Sanity check - See ScriptProviderBase.StylesToSkipByDefault");
		}

		[Test]
		public void Create_NonExistentPath_CorrectlyInitializedWithDefaultSkippedStyles()
		{
			var result = SkippedScriptLines.Create(Path.GetRandomFileName(), new TestScriptProvider());
			Assert.AreEqual(Settings.Default.CurrentSkippedLinesVersion, result.Version);
			var skippedStyles = result.SkippedParagraphStyles;
			Assert.IsTrue(skippedStyles.SetEquals(_stylesToSkipByDefault));
		}

		[Test]
		public void Create_DataAtVersion0WithNoSkippedStyles_MigratedToCurrentVersionWithDefaultSkippedStyles()
		{
			var orig = new SkippedScriptLines
			{
				SkippedParagraphStyles = new List<string>(),
				SkippedLinesList = new List<ScriptLineIdentifier>(),
			};

			var result = SkippedScriptLines.Create(GetAsBytesWithNoVersionSpecified(orig), _stylesToSkipByDefault);
			Assert.AreEqual(Settings.Default.CurrentSkippedLinesVersion, result.Version);
			var skippedStyles = result.SkippedParagraphStyles;
			Assert.AreEqual(_stylesToSkipByDefault.Count, skippedStyles.Count);
			Assert.IsTrue(skippedStyles.SetEquals(_stylesToSkipByDefault));
		}

		[TestCase("qs...qs* - Poetry Text - Selah")]
		// This could never happen in real life since in version 0, "r" and "io1" were never
		// presented to the user as styles that could be skipped.
		[TestCase("qs...qs* - Poetry Text - Selah", "r - Heading - Parallel References", "io1 - Introduction - Outline Level 1")]
		public void Create_DataAtVersion0WithSkippedStyles_MigratedToCurrentVersionWithExistingAndDefaultSkippedStyles(params string [] existing)
		{
			var orig = new SkippedScriptLines
			{
				SkippedParagraphStyles = new List<string>(existing),
				SkippedLinesList = new List<ScriptLineIdentifier>(),
			};

			var result = SkippedScriptLines.Create(GetAsBytesWithNoVersionSpecified(orig), _stylesToSkipByDefault);
			Assert.AreEqual(Settings.Default.CurrentSkippedLinesVersion, result.Version);
			var skippedStyles = result.SkippedParagraphStyles;
			Assert.AreEqual(_stylesToSkipByDefault.Count + 1, skippedStyles.Count);
			Assert.IsTrue(skippedStyles.Contains("qs...qs* - Poetry Text - Selah"));
			Assert.IsTrue(skippedStyles.ToHashSet().IsProperSupersetOf(_stylesToSkipByDefault));
		}

		[Test]
		public void Create_DataAtVersion1WithSomeDefaultSkippedStylesNotSkipped_MigrateDoesNotAlterSkippedStyles()
		{
			var mySkippedStyles = new[] {_stylesToSkipByDefault[4],
				_stylesToSkipByDefault[0], _stylesToSkipByDefault[2]};
			var orig = new SkippedScriptLines
			{
				SkippedParagraphStyles = new List<string>(mySkippedStyles),
				SkippedLinesList = new List<ScriptLineIdentifier>(),
			};

			var result = SkippedScriptLines.Create(XmlSerializationHelper.SerializeToByteArray(orig), _stylesToSkipByDefault);
			Assert.AreEqual(Settings.Default.CurrentSkippedLinesVersion, result.Version);
			var skippedStyles = result.SkippedParagraphStyles;
			Assert.AreEqual(3, skippedStyles.Count);
			Assert.IsTrue(skippedStyles.SetEquals(mySkippedStyles));
		}

		private static byte[] GetAsBytesWithNoVersionSpecified(SkippedScriptLines orig)
		{
			var utf16 = XmlSerializationHelper.SerializeToString(orig);
			var className = orig.GetType().Name;
			var rootElementWithCurrentVersion = $"<{className} version=\"{Settings.Default.CurrentSkippedLinesVersion}\">";
			Assert.That(utf16.Contains(rootElementWithCurrentVersion), "SETUP error: " +
				$"{className} root element not formatted as expected. Was an attribute added?");
			var bytes = Encoding.UTF8.GetBytes(utf16.Replace(rootElementWithCurrentVersion,
				$"<{className}>"));
			return bytes;
		}
	}
}
