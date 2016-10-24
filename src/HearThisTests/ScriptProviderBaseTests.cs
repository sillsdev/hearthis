using System;
using System.IO;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;
using SIL.IO;
using SIL.Xml;

namespace HearThisTests
{
	[TestFixture]
	public class ScriptProviderBaseTests
	{
		private byte[] m_wavFileBytes;
		private byte[] WavFileBytes
		{
			get
			{
				if (m_wavFileBytes == null)
				{
					var lengthOfWavFileInBytes = Resource1._2Channel.Length;
					m_wavFileBytes = new byte[lengthOfWavFileInBytes + 1L];
					Resource1._2Channel.Read(m_wavFileBytes, 0, (int)lengthOfWavFileInBytes);
				}
				return m_wavFileBytes;
			}
		}

		[Test]
		public void Initialize_ProjectInfoNull_NoSkippedStyles_NoClipsBackedUpAndProjectInfoSavedWithCurrentVersion()
		{
			using (var scriptProvider = new TestScriptProviderForMigrationTests((projFolderPath, skippedLineInfoPath) =>
			{
				CreateClipForBlock(projFolderPath, "Matthew", 1, 1);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 2);
				CreateClipForBlock(projFolderPath, "Matthew", 3, 1);
			}))
			{
				Assert.IsTrue(ClipRepository.GetHaveClip(scriptProvider.ProjectFolderName, "Matthew", 1, 1));
				Assert.IsTrue(ClipRepository.GetHaveClip(scriptProvider.ProjectFolderName, "Matthew", 2, 2));
				Assert.IsTrue(ClipRepository.GetHaveClip(scriptProvider.ProjectFolderName, "Matthew", 3, 1));
				Assert.AreEqual(Settings.Default.CurrentDataVersion, scriptProvider.GetVersionNumberFromProjectInfoFile());
			}
		}

		[Test]
		public void Initialize_ProjectInfoNull_SkippedStyles_ClipsFrorSkippedStylesBackedUpAndProjectInfoSavedWithCurrentVersion()
		{
			using (var scriptProvider = new TestScriptProviderForMigrationTests((projFolderPath, skippedLineInfoPath) =>
			{
				var skipInfo = SkippedScriptLines.Create(skippedLineInfoPath);
				skipInfo.SkippedParagraphStyles.Add("s");
				XmlSerializationHelper.SerializeToFile(skippedLineInfoPath, skipInfo);
				CreateClipForBlock(projFolderPath, "Matthew", 1, 1);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 0);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 1);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 2);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 3);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 4);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 5);
				CreateClipForBlock(projFolderPath, "Matthew", 3, 1);
			}))
			{
				// Note: All the even numbered blocks have a style of "s", which is skipped.
				Assert.IsTrue(ClipRepository.GetHaveClip(scriptProvider.ProjectFolderName, "Matthew", 1, 1));
				VerifyClipWasBackedUp(scriptProvider.ProjectFolderName, "Matthew", 2, 0);
				Assert.IsTrue(ClipRepository.GetHaveClip(scriptProvider.ProjectFolderName, "Matthew", 2, 1));
				VerifyClipWasBackedUp(scriptProvider.ProjectFolderName, "Matthew", 2, 2);
				Assert.IsTrue(ClipRepository.GetHaveClip(scriptProvider.ProjectFolderName, "Matthew", 2, 3));
				VerifyClipWasBackedUp(scriptProvider.ProjectFolderName, "Matthew", 2, 4);
				Assert.IsTrue(ClipRepository.GetHaveClip(scriptProvider.ProjectFolderName, "Matthew", 2, 5));
				Assert.IsTrue(ClipRepository.GetHaveClip(scriptProvider.ProjectFolderName, "Matthew", 3, 1));
				Assert.AreEqual(Settings.Default.CurrentDataVersion, scriptProvider.GetVersionNumberFromProjectInfoFile());
			}
		}

		private void VerifyClipWasBackedUp(string projFolderPath, string bookName, int chapterIndex, int lineIndex)
		{
			Assert.IsFalse(ClipRepository.GetHaveClip(projFolderPath, bookName, chapterIndex, lineIndex));
			var recordingPath = ClipRepository.GetPathToLineRecording(projFolderPath, bookName, chapterIndex, lineIndex);
			var skipPath = Path.ChangeExtension(recordingPath, "skip");
			Assert.IsTrue(File.Exists(skipPath));
		}

		private void CreateClipForBlock(string projFolderPath, string bookName, int chapterIndex, int lineIndex)
		{
			var path = projFolderPath;
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			path = Path.Combine(path, bookName);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			path = Path.Combine(path, chapterIndex.ToString());
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			path = Path.Combine(path, lineIndex.ToString());
			path = Path.ChangeExtension(path, ".wav");
			File.WriteAllBytes(path, WavFileBytes);
		}
	}

	class TestScriptProviderForMigrationTests : TestScriptProvider, IDisposable
	{
		private readonly string m_projectFolderName;

		public TestScriptProviderForMigrationTests(Action<string, string> setupData)
		{
			m_projectFolderName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
			Directory.CreateDirectory(ProjectFolderPath);
			setupData(ProjectFolderPath, Path.Combine(ProjectFolderPath, kSkippedLineInfoFilename));
			Initialize();
		}

		public override string ProjectFolderName
		{
			get { return m_projectFolderName; }
		}

		public override ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			if (bookNumber == 0)
				throw new NotImplementedException();

			return new ScriptLine("Blah")
			{
				Number = lineNumber0Based,
				ParagraphStyle = lineNumber0Based % 2 == 0 ? "s" : "p"
			};
		}

		internal int GetVersionNumberFromProjectInfoFile()
		{
			var path = Path.Combine(ProjectFolderPath, kProjectInfoFilename);
			if (File.Exists(path))
				return XmlSerializationHelper.DeserializeFromFile<ProjectInfo>(path).Version;
			Assert.Fail("File not found: " + path);
			return -1;
		}

		public void Dispose()
		{
			DirectoryUtilities.DeleteDirectoryRobust(ProjectFolderPath);
		}
	}
}
