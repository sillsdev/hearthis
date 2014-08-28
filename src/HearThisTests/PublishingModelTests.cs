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
		// Fails on TeamCity because MyDocuments is not a valid location.
		[Test]
		[Category("SkipOnTeamCity")]
		public void Publish_PublishRootPathDoesNotExist_UsesDefaultLocation()
		{
			var m = new PublishingModel("foo", null);
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
			var m = new PublishingModel("foo", null);
			var customPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(customPath);
			m.PublishRootPath = customPath;
			m.Publish(new NullProgress());
			Assert.AreEqual(Path.Combine(customPath, "HearThis-foo"), m.PublishThisProjectPath);
			Assert.IsTrue(m.PublishThisProjectPath.StartsWith(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishThisProjectPath));
		}

		// Fails on TeamCity because MyDocuments is not a valid location.
		[Test]
		[Category("SkipOnTeamCity")]
		public void Publish_PublishRootPathIsNull_UsesDefaultLocation()
		{
			var m = new PublishingModel("foo", null);
			m.PublishRootPath = null;
			m.Publish(new NullProgress());
			Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HearThis-foo"), m.PublishThisProjectPath);
			Assert.IsTrue(m.PublishThisProjectPath.StartsWith(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishRootPath));
			Assert.IsTrue(Directory.Exists(m.PublishThisProjectPath));
		}

		[Test]
		public void PublishOnlyCurrentBook_Set_Persists()
		{
			var m = new PublishingModel("foo", null);
			m.PublishOnlyCurrentBook = true;
			Assert.IsTrue(m.PublishOnlyCurrentBook);
			m = new PublishingModel("fee", null);
			Assert.IsTrue(m.PublishOnlyCurrentBook);
			m.PublishOnlyCurrentBook = false;
			Assert.IsFalse(m.PublishOnlyCurrentBook);
			m = new PublishingModel("fee", null);
			Assert.IsFalse(m.PublishOnlyCurrentBook);
		}


		[Test]
		public void PublishingMethod_AudioFormatIsAudiBible_GetsAudiBiblePublishingMethod()
		{
			VerifyPublishingMethod<AudiBiblePublishingMethod>("audiBible");
		}

		[Test]
		public void PublishingMethod_AudioFormatIsSaber_GetsSaberPublishingMethod()
		{
			VerifyPublishingMethod<SaberPublishingMethod>("saber");
		}

		[Test]
		public void PublishingMethod_AudioFormatIsMegaVoice_GetsMegaVoicePublishingMethod()
		{
			VerifyPublishingMethod<MegaVoicePublishingMethod>("megaVoice");
		}

		[Test]
		public void PublishingMethod_AudioFormatIsMp3_GetsBunchOfFilesPublishingMethodWithLameEncoder()
		{
			var method = VerifyPublishingMethod<BunchOfFilesPublishingMethod>("mp3");
			Assert.AreEqual("mp3", method.GetRootDirectoryName());
		}

		[Test]
		public void PublishingMethod_AudioFormatIsFlac_GetsBunchOfFilesPublishingMethodWithLameEncoder()
		{
			var method = VerifyPublishingMethod<BunchOfFilesPublishingMethod>("flac");
			Assert.AreEqual("FLAC", method.GetRootDirectoryName());
		}

		[Test]
		public void PublishingMethod_AudioFormatIsOgg_GetsBunchOfFilesPublishingMethodWithLameEncoder()
		{
			var method = VerifyPublishingMethod<BunchOfFilesPublishingMethod>("ogg");
			Assert.AreEqual("ogg", method.GetRootDirectoryName());
		}

		[Test]
		public void PublishingMethod_AudioFormatIsScrAppBuilder_GetsScriptureAppBuilderPublishingMethod()
		{
			VerifyPublishingMethod<ScriptureAppBuilderPublishingMethod>("scrAppBuilder");
		}

		private T VerifyPublishingMethod<T>(string audioFormat) where T: class
		{
			var m = new PublishingModel("foo", "xkal");
			m.AudioFormat = audioFormat;
			m.Publish(new NullProgress());
			T method = m.PublishingMethod as T;
			Assert.IsNotNull(method);
			Assert.AreEqual(method, m.PublishingMethod, "Should get exact same publishing method.");
			return method;
		}
	}
}
