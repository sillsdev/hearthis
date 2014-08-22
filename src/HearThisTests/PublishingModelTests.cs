using System;
using System.IO;
using HearThis.Publishing;
using NUnit.Framework;
using Palaso.Progress;

namespace HearThisTests
{
	[TestFixture]
	public class PublishingModelTests
	{
		[Test]
		// At one point this test was ignored because it failed on the (TeamCity) server.
		// I (JohnT) changed it to this category so it can be used on developer machines.
		// We don't recall how or why it fails on TeamCity, or even know for sure that it still does.
		// It MIGHT be something to do with whether Paratext is installed.
		//[Category("SkipOnTeamCity")]
		public void Publish_PublishRootPathDoesNotExist_UsesDefaultLocation()
		{
			var m = new PublishingModel("foo");
			var newRandomPart = Guid.NewGuid().ToString();
			m.PublishRootPath = Path.Combine(Path.GetTempPath(), newRandomPart);
			m.Publish(new NullProgress());
			Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HearThis-foo"), m.PublishThisProjectPath);
			Assert.IsTrue(m.PublishThisProjectPath.StartsWith(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishThisProjectPath));
		}

		[Test]
		// At one point this test was ignored because it failed on the (TeamCity) server.
		// I (JohnT) changed it to this category so it can be used on developer machines.
		// We don't recall how or why it fails on TeamCity, or even know for sure that it still does.
		// It MIGHT be something to do with whether Paratext is installed.
		//[Category("SkipOnTeamCity")]
		public void Publish_CustomPublishRootPathExists_UsesCustomLocation()
		{
			var m = new PublishingModel("foo");
			var customPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(customPath);
			m.PublishRootPath = customPath;
			m.Publish(new NullProgress());
			Assert.AreEqual(Path.Combine(customPath, "HearThis-foo"), m.PublishThisProjectPath);
			Assert.IsTrue(m.PublishThisProjectPath.StartsWith(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishThisProjectPath));
		}

		[Test]
		// At one point this test was ignored because it was thought that it might fail on the (TeamCity) server.
		// I (JohnT) changed it to this category so it can be used on developer machines.
		// We don't recall how or why it might fail on TeamCity, or even know for sure that it ever did.
		// It MIGHT be something to do with whether Paratext is installed.
		//[Category("SkipOnTeamCity")]
		public void Publish_PublishRootPathIsNull_PublishThisProjectPathUnderneathMyDocuments()
		{
			var m = new PublishingModel("foo");
			m.PublishRootPath = null;
			m.Publish(new NullProgress());
			Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HearThis-foo"), m.PublishThisProjectPath);
			Assert.IsTrue(m.PublishThisProjectPath.StartsWith(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishThisProjectPath));
		}
	}
}
