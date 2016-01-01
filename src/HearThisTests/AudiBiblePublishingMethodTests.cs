using System.Collections.Generic;
using System.IO;
using HearThis.Publishing;
using NUnit.Framework;
using SIL.Progress;

namespace HearThisTests
{
	/// <summary>
	/// Tests the AudiBiblePublishingMethod class, particuarly the few ways it differs from BunchOfFilesPublishingMethod
	/// </summary>
	[TestFixture]
	public class AudiBiblePublishingMethodTests
	{
		[Test]
		public void InvokesEncoderWithCorrectArgs()
		{
			var mockEncoder = new MockEncoder();
			var publisher = new TestPublisher(mockEncoder, "xyz");
			var progress = new ConsoleProgress();

			publisher.PublishChapter("MyPath", "Genesis", 5, "inputPath", progress);
			publisher.PublishChapter("MyPath", "Genesis", 17, "inputPath2", progress);

			Assert.That(mockEncoder.Progress, Is.EqualTo(progress));
			Assert.That(mockEncoder.SourcePaths[0], Is.EqualTo("inputPath"));
			var sep = Path.DirectorySeparatorChar;
			Assert.That(mockEncoder.DestPaths[0], Is.EqualTo("MyPath" + sep + "00_XYZ_Gen" + sep + "00_XYZ_Gen_05"));
			Assert.That(publisher.EnsuredDirectories, Has.Member("MyPath" + sep + "00_XYZ_Gen"));

			Assert.That(mockEncoder.SourcePaths[1], Is.EqualTo("inputPath2"));
			Assert.That(mockEncoder.DestPaths[1], Is.EqualTo("MyPath" + sep + "00_XYZ_Gen" + sep + "00_XYZ_Gen_17"));
		}

		[Test]
		public void PsalmsHasExtraDigit()
		{
			var mockEncoder = new MockEncoder();
			var publisher = new TestPublisher(mockEncoder, "QED");
			var progress = new ConsoleProgress();

			publisher.PublishChapter("SomePath", "Psalms", 8, "startHere", progress);
			publisher.PublishChapter("SomePath", "Psalms", 128, "startHere3", progress);

			Assert.That(mockEncoder.Progress, Is.EqualTo(progress));
			Assert.That(mockEncoder.SourcePaths[0], Is.EqualTo("startHere"));
			var sep = Path.DirectorySeparatorChar;
			Assert.That(mockEncoder.DestPaths[0], Is.EqualTo("SomePath" + sep + "18_QED_Psa" + sep + "18_QED_Psa_008"));
			Assert.That(publisher.EnsuredDirectories, Has.Member("SomePath" + sep + "18_QED_Psa"));

			Assert.That(mockEncoder.SourcePaths[1], Is.EqualTo("startHere3"));
			Assert.That(mockEncoder.DestPaths[1], Is.EqualTo("SomePath" + sep + "18_QED_Psa" + sep + "18_QED_Psa_128"));
		}
	}

	class MockEncoder : IAudioEncoder
	{
		public List<string> SourcePaths = new List<string>();
		public List<string> DestPaths = new List<string>();
		public IProgress Progress;
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			SourcePaths.Add(sourcePath);
			DestPaths.Add(destPathWithoutExtension);
			Progress = progress;
		}

		public string FormatName { get { return "dummy"; } }
	}

	class TestPublisher : AudiBiblePublishingMethod
	{
		public TestPublisher(IAudioEncoder encoder, string ethnologueCode)
			: base(encoder, ethnologueCode)
		{
		}

		public List<string> EnsuredDirectories = new List<string>();

		protected override void EnsureDirectory(string path)
		{
		   EnsuredDirectories.Add(path);
		}
	}
}
