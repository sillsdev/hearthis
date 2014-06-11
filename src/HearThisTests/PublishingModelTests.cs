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
		// It MIGHT be something to do with whether Paratext is installed, or whether a progress dialog can be shown.
		[Category("SkipOnTeamCity")]
		public void Publish_PublishRootPathHasOneNewDirectory_DirectoryCreated()
		{
			var m = new PublishingModel("foo");
			var newRandomPart = Guid.NewGuid().ToString();
			m.PublishRootPath = Path.Combine(Path.GetTempPath(), newRandomPart);
			var progress = new StringBuilderProgress();
			m.Publish(progress);
			Assert.IsFalse(progress.ErrorEncountered);
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
		}

		[Test]
		// At one point this test was ignored because it failed on the (TeamCity) server.
		// I (JohnT) changed it to this category so it can be used on developer machines.
		// We don't recall how or why it fails on TeamCity, or even know for sure that it still does.
		// It MIGHT be something to do with whether Paratext is installed, or whether a progress dialog can be shown.
		[Category("SkipOnTeamCity")]
		public void Publish_PublishRootPathHasMoreThanOneNewDirectory_DirectoryCreated()
		{
			var m = new PublishingModel("foo");
			m.PublishRootPath = "c:/1/2/3";
			m.Publish(new NullProgress());
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
		}

		[Test]
		// At one point this test was ignored because it failed on the (TeamCity) server.
		// I (JohnT) changed it to this category so it can be used on developer machines.
		// We don't recall how or why it fails on TeamCity, or even know for sure that it still does.
		// It MIGHT be something to do with whether Paratext is installed, or whether a progress dialog can be shown.
		[Category("SkipOnTeamCity")]
		public void Publish_PublishRootPathIsNull_PublishRootPathIsAtSomeDefaultPlace()
		{
			var m = new PublishingModel("foo");
			m.PublishRootPath = null;
			m.Publish(new NullProgress());
			Assert.IsFalse(string.IsNullOrEmpty(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
		}


		[Test]
		// At one point this test was ignored because it was thought that it might fail on the (TeamCity) server.
		// I (JohnT) changed it to this category so it can be used on developer machines.
		// We don't recall how or why it might fail on TeamCity, or even know for sure that it ever did.
		// It MIGHT be something to do with whether Paratext is installed, or whether a progress dialog can be shown.
		[Category("SkipOnTeamCity")]
		public void Publish_PublishRootPathIsNull_PublishThisProjectPathUnderneathMyDocuments()
		{
			var m = new PublishingModel("foo");
			m.PublishRootPath = null;
			m.Publish(new NullProgress());
			Assert.IsTrue(m.PublishThisProjectPath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)), m.PublishThisProjectPath);
			Assert.IsTrue(m.PublishThisProjectPath.EndsWith("HearThis-foo"), m.PublishThisProjectPath);
		}
	}
}
