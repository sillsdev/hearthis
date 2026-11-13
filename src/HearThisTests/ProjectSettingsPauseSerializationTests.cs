using System.IO;
using System.Xml.Serialization;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;

namespace HearThisTests
{
	[TestFixture]
	public class ProjectSettingsPauseSerializationTests
	{
		[Test]
		public void PostHeadingPause_RoundTripsThroughXmlSerialization()
		{
			var settings = new ProjectSettings { PostHeadingPause = new PauseData(true, 1.45, 1.55) };
			var serializer = new XmlSerializer(typeof(ProjectSettings));
			string xml;
			using (var writer = new StringWriter())
			{
				serializer.Serialize(writer, settings);
				xml = writer.ToString();
			}

			ProjectSettings roundTripped;
			using (var reader = new StringReader(xml))
			{
				roundTripped = (ProjectSettings)serializer.Deserialize(reader);
			}

			Assert.That(roundTripped.PostHeadingPause, Is.Not.Null);
			Assert.That(roundTripped.PostHeadingPause.Apply, Is.True);
			Assert.That(roundTripped.PostHeadingPause.Min, Is.EqualTo(1.45));
			Assert.That(roundTripped.PostHeadingPause.Max, Is.EqualTo(1.55));
		}
	}
}
