using System.IO;
using System.Xml.Serialization;
using HearThis.Script;
using NUnit.Framework;

namespace HearThisTests
{
	/// <summary>
	/// ScriptLine is persisted (via ChapterInfo) in each chapter's recording-info XML, so
	/// transient values that are always obtained from the live script provider must be
	/// excluded from serialization to keep those files from accumulating stale data.
	/// </summary>
	[TestFixture]
	public class ScriptLineSerializationTests
	{
		[Test]
		public void Serialize_ParagraphStartSet_ParagraphStartNotPersisted()
		{
			var line = new ScriptLine { Number = 1, ParagraphStart = true };

			var serializer = new XmlSerializer(typeof(ScriptLine));
			using (var writer = new StringWriter())
			{
				serializer.Serialize(writer, line);

				Assert.That(writer.ToString(), Does.Not.Contain("ParagraphStart"));
			}
		}
	}
}
