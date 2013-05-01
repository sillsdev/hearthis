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
		public void Publish_PublishRootPathHasOneNewDirectory_DirectoryCreated()
		{
			LineRecordingRepository library = new LineRecordingRepository();
			var m = new PublishingModel(library, "foo");
			var newRandomPart = Guid.NewGuid().ToString();
			m.PublishRootPath = Path.Combine(Path.GetTempPath(), newRandomPart);
			var progress = new Palaso.Progress.StringBuilderProgress();
			m.Publish(progress);
			Assert.IsFalse(progress.ErrorEncountered);
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
		}

		[Test]
		public void Publish_PublishRootPathHasMoreThanOneNewDirectory_DirectoryCreated()
		{
			LineRecordingRepository library = new LineRecordingRepository();
			var m = new PublishingModel(library, "foo");
			m.PublishRootPath = "c:/1/2/3";
			m.Publish(new NullProgress());
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
		}

		[Test]
		public void Publish_PublishRootPathIsNull_PublishRootPathIsAtSomeDefaultPlace()
		{
			LineRecordingRepository library = new LineRecordingRepository();
			var m = new PublishingModel(library, "foo");
			m.PublishRootPath = null;
			m.Publish(new NullProgress());
			Assert.IsFalse(string.IsNullOrEmpty(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
		}


		[Test]
		public void Publish_PublishRootPathIsNull_PublishThisProjectPathUnderneathMyDocuments()
		{
			LineRecordingRepository library = new LineRecordingRepository();
			var m = new PublishingModel(library, "foo");
			m.PublishRootPath = null;
			m.Publish(new NullProgress());
			Assert.IsTrue(m.PublishThisProjectPath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)), m.PublishThisProjectPath);
			Assert.IsTrue(m.PublishThisProjectPath.EndsWith("HearThis-foo"), m.PublishThisProjectPath);
		}
	}
}
