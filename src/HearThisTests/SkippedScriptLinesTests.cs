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
			Assert.That(_stylesToSkipByDefault.Count, Is.EqualTo(5),
				"Sanity check - See ScriptProviderBase.StylesToSkipByDefault");
		}

		[Test]
		public void Create_NonExistentPath_CorrectlyInitializedWithDefaultSkippedStyles()
		{
			var result = SkippedScriptLines.Create(Path.GetRandomFileName(), new TestScriptProvider());
			Assert.That(result.Version, Is.EqualTo(Settings.Default.CurrentSkippedLinesVersion));
			var skippedStyles = result.SkippedParagraphStyles;
			Assert.That(skippedStyles, Is.EquivalentTo(_stylesToSkipByDefault));
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
			Assert.That(result.Version, Is.EqualTo(Settings.Default.CurrentSkippedLinesVersion));
			var skippedStyles = result.SkippedParagraphStyles;
			Assert.That(skippedStyles.Count, Is.EqualTo(_stylesToSkipByDefault.Count));
			Assert.That(skippedStyles, Is.EquivalentTo(_stylesToSkipByDefault));
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
			Assert.That(result.Version, Is.EqualTo(Settings.Default.CurrentSkippedLinesVersion));
			var skippedStyles = result.SkippedParagraphStyles;
			Assert.That(skippedStyles.Count, Is.EqualTo(_stylesToSkipByDefault.Count + 1));
			Assert.That(skippedStyles, Does.Contain("qs...qs* - Poetry Text - Selah"));
			Assert.That(skippedStyles, Is.SupersetOf(_stylesToSkipByDefault));
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
			Assert.That(result.Version, Is.EqualTo(Settings.Default.CurrentSkippedLinesVersion));
			var skippedStyles = result.SkippedParagraphStyles;
			Assert.That(skippedStyles.Count, Is.EqualTo(3));
			Assert.That(skippedStyles, Is.EquivalentTo(mySkippedStyles));
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
