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
		// Fails on TeamCity because MyDocuments is not a valid location.
		//[Category("SkipOnTeamCity")]
		public void SanityCheck1()
		{
			Assert.IsTrue(Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
		}

		[Test]
		// Fails on TeamCity because MyDocuments is not a valid location.
		//[Category("SkipOnTeamCity")]
		public void SanityCheck2()
		{
			Assert.IsTrue(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).EndsWith("ocuments"));
		}

		[Test]
		// Fails on TeamCity because MyDocuments is not a valid location.
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
		// Fails on TeamCity because MyDocuments is not a valid location.
		//[Category("SkipOnTeamCity")]
		public void Publish_PublishRootPathIsNull_UsesDefaultLocation()
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
