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
			using (var scriptProvider = new TestScriptProviderForMigrationTests((_, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
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
		public void Initialize_ProjectInfoNull_SkippedStyles_ClipsForSkippedStylesBackedUpAndProjectInfoSavedWithCurrentVersion()
		{
			using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				var skipInfo = SkippedScriptLines.Create(skippedLineInfoPath, self);
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

		[TestCase(true, 0, true)]
		[TestCase(true, 1, true)]
		[TestCase(true, 2, false)]
		[TestCase(false, 0, false)]
		[TestCase(false, 1, false)]
		[TestCase(false, 2, false)]
		public void Initialize_DataVersion2_BreakAtParagraphBreaksSetCorrectly(bool createClip, int dataVersionNumber, bool expectedResult)
		{
			using (var scriptProvider = new TestScriptProviderForMigrationTests((_, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				SetVersionNumberBeforeInitialize(projectSettingsPath, dataVersionNumber);
				if (createClip)
					CreateClipForBlock(projFolderPath, "Matthew", 1, 1);
			}))
			{

				Assert.AreEqual(expectedResult, scriptProvider.ProjectSettings.BreakAtParagraphBreaks);
				Assert.AreEqual(Settings.Default.CurrentDataVersion, scriptProvider.GetVersionNumberFromProjectInfoFile());
			}
		}

		private void SetVersionNumberBeforeInitialize(string projectSettingsPath, int version)
		{
			ProjectSettings projectSettings = new ProjectSettings { Version = version };
			XmlSerializationHelper.SerializeToFile(projectSettingsPath, projectSettings);
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
		private string _projectFolderName;

		public TestScriptProviderForMigrationTests(Action<ISkippedStyleInfoProvider, string, string, string> setupData)
		{
			_projectFolderName = ProjectFolderName;
			Directory.CreateDirectory(ProjectFolderPath);
			setupData(this, ProjectFolderPath,
				Path.Combine(ProjectFolderPath, kSkippedLineInfoFilename),
				Path.Combine(ProjectFolderPath, kProjectInfoFilename));
			Initialize();
		}

		public override string ProjectFolderName
		{
			get
			{
				if (_projectFolderName == null)
					_projectFolderName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
				return _projectFolderName;
			}
		}

		public override ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			if (bookNumber == 0)
				throw new NotImplementedException();

			return new ScriptLine("Blah")
			{
				Number = lineNumber0Based + 1,
				ParagraphStyle = lineNumber0Based % 2 == 0 ? "s" : "p"
			};
		}

		internal int GetVersionNumberFromProjectInfoFile()
		{
			var path = Path.Combine(ProjectFolderPath, kProjectInfoFilename);
			if (File.Exists(path))
				return XmlSerializationHelper.DeserializeFromFile<ProjectSettings>(path).Version;
			Assert.Fail("File not found: " + path);
			return -1;
		}

		public void Dispose()
		{
			RobustIO.DeleteDirectoryAndContents(ProjectFolderPath);
		}
	}
}
