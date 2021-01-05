// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2016' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;
using SIL.IO;
using SIL.Scripture;
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

		#region Integration tests for HT-376
		[Test]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_NoParallelPassagesOrIntroTables_NoChaptersPotentiallyNeedingManualMigration()
		{
			using (var scriptProvider = new TestScriptProviderForMigrationTests((_, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				// This ensures that the initial call to DoDataMigration will be a no-op.
				SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
			}))
			{
				Assert.IsFalse(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null).Any());
				Assert.AreEqual(Settings.Default.CurrentDataVersion, scriptProvider.GetVersionNumberFromProjectInfoFile());
			}
		}
		
		[Test]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_ParallelPassageInFolderWithClipsAfterMigrationToHt203_ClipsNotShiftedAndNoChaptersPotentiallyNeedingManualMigration()
		{
			var presets = new Dictionary<int, string>
			{
				// This looks weird because Matthew should be book 40, but in the test VersificationInfo, it's book 1.
				[1002002] = "r - Heading - Parallel References"
			};

			using (var secretary = new ClipsShiftedSecretary())
			{
				var clipFilesThatShouldStillExist = new List<string>(2);
				using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
				{
					var skipInfo = SkippedScriptLines.Create(skippedLineInfoPath, self);
					var fileContentsWithVersionSetTo1 =
						XmlSerializationHelper.SerializeToString(skipInfo)
						.Replace($"<SkippedScriptLines version=\"{Settings.Default.CurrentSkippedLinesVersion}\">",
						"<SkippedScriptLines version=\"1\">");
					File.WriteAllText(skippedLineInfoPath, fileContentsWithVersionSetTo1);

					// This ensures that the initial call to DoDataMigration will be a no-op.
					SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
					Thread.Sleep(1001); // file times are to the second.
					clipFilesThatShouldStillExist.Add(CreateClipForBlock(projFolderPath, "Matthew", 2, 2));
					clipFilesThatShouldStillExist.Add(CreateClipForBlock(projFolderPath, "Matthew", 2, 3));
				}, presets))
				{
					Assert.IsFalse(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null).Any());
					Assert.AreEqual(Settings.Default.CurrentDataVersion, scriptProvider.GetVersionNumberFromProjectInfoFile());
					Assert.IsTrue(clipFilesThatShouldStillExist.All(File.Exists));
				}
				Assert.AreEqual(0, secretary.ClipsShiftedEvents.Count);
			}
		}
		
		[Test]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_ParallelPassageInFolderWithClipsBeforeMigrationToHt203_ClipsShiftedAndNoChaptersPotentiallyNeedingManualMigration()
		{
			var presets = new Dictionary<int, string>
			{
				// This looks weird because Matthew should be book 40, but in the test VersificationInfo, it's book 1.
				[1002002] = "r - Heading - Parallel References"
			};

			using (var secretary = new ClipsShiftedSecretary())
			{
				var clipFilesThatShouldBeShifted = new List<string>(2);
				using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
				{
					clipFilesThatShouldBeShifted.Add(CreateClipForBlock(projFolderPath, "Matthew", 2, 2));
					clipFilesThatShouldBeShifted.Add(CreateClipForBlock(projFolderPath, "Matthew", 2, 3));
					Thread.Sleep(1001); // file times are to the second.
					var skipInfo = SkippedScriptLines.Create(skippedLineInfoPath, self);
					var fileContentsWithVersionSetTo1 =
						XmlSerializationHelper.SerializeToString(skipInfo)
							.Replace($"<SkippedScriptLines version=\"{Settings.Default.CurrentSkippedLinesVersion}\">",
								"<SkippedScriptLines version=\"1\">");
					File.WriteAllText(skippedLineInfoPath, fileContentsWithVersionSetTo1);

					// This ensures that the initial call to DoDataMigration will be a no-op.
					SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
				}, presets))
				{
					Assert.IsFalse(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null).Any());
					Assert.AreEqual(Settings.Default.CurrentDataVersion, scriptProvider.GetVersionNumberFromProjectInfoFile());
					Assert.IsFalse(File.Exists(clipFilesThatShouldBeShifted.First()));
					Assert.IsTrue(File.Exists(clipFilesThatShouldBeShifted.Last()));
					Assert.IsTrue(File.Exists(clipFilesThatShouldBeShifted.Last().Replace("3.wav", "4.wav")));
				}
				Assert.AreEqual(1, secretary.ClipsShiftedEvents.Count);
				Assert.AreEqual("Matthew", secretary.ClipsShiftedEvents[0].BookName);
				Assert.AreEqual(2, secretary.ClipsShiftedEvents[0].ChapterNumber);
				Assert.AreEqual(2, secretary.ClipsShiftedEvents[0].LineNumberOfShiftedClip);
				Assert.AreEqual(1, secretary.ClipsShiftedEvents[0].ShiftedBy);
			}
		}
		
		[Test]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_TwoParallelPassageLinesInFolderWithClipsBeforeMigrationToHt203_ClipsShiftedAndNoChaptersPotentiallyNeedingManualMigration()
		{
			var presets = new Dictionary<int, string>
			{
				// This looks weird because Matthew should be book 40, but in the test VersificationInfo, it's book 1.
				[1002001] = "r - Heading - Parallel References",
				[1002004] = "r - Heading - Parallel References"
			};

			using (var secretary = new ClipsShiftedSecretary())
			{
				string clipFilesFolder = "";
				using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
				{
					clipFilesFolder = Path.GetDirectoryName(CreateClipForBlock(projFolderPath, "Matthew", 2, 0));
					CreateClipForBlock(projFolderPath, "Matthew", 2, 1);
					CreateClipForBlock(projFolderPath, "Matthew", 2, 2);
					CreateClipForBlock(projFolderPath, "Matthew", 2, 3);
					CreateClipForBlock(projFolderPath, "Matthew", 2, 4);
					Thread.Sleep(1001); // file times are to the second.
					var skipInfo = SkippedScriptLines.Create(skippedLineInfoPath, self);
					var fileContentsWithVersionSetTo1 =
						XmlSerializationHelper.SerializeToString(skipInfo)
							.Replace($"<SkippedScriptLines version=\"{Settings.Default.CurrentSkippedLinesVersion}\">",
								"<SkippedScriptLines version=\"1\">");
					File.WriteAllText(skippedLineInfoPath, fileContentsWithVersionSetTo1);

					// This ensures that the initial call to DoDataMigration will be a no-op.
					SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
				}, presets))
				{
					Assert.IsFalse(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null).Any());
					Assert.AreEqual(Settings.Default.CurrentDataVersion, scriptProvider.GetVersionNumberFromProjectInfoFile());
					var wavFiles = Directory.GetFiles(clipFilesFolder);
					Assert.AreEqual(5, wavFiles.Length);
					Assert.IsTrue(File.Exists(Path.Combine(clipFilesFolder, "0.wav")));
					Assert.IsTrue(File.Exists(Path.Combine(clipFilesFolder, "2.wav")));
					Assert.IsTrue(File.Exists(Path.Combine(clipFilesFolder, "3.wav")));
					Assert.IsTrue(File.Exists(Path.Combine(clipFilesFolder, "5.wav")));
					Assert.IsTrue(File.Exists(Path.Combine(clipFilesFolder, "6.wav")));
				}
				Assert.AreEqual(2, secretary.ClipsShiftedEvents.Count);
				Assert.IsTrue(secretary.ClipsShiftedEvents.All(c => c.BookName == "Matthew"));
				Assert.IsTrue(secretary.ClipsShiftedEvents.All(c => c.ChapterNumber == 2));
				Assert.IsTrue(secretary.ClipsShiftedEvents.All(c => c.ShiftedBy == 1));
				Assert.AreEqual(1, secretary.ClipsShiftedEvents[0].LineNumberOfShiftedClip);
				Assert.AreEqual(4, secretary.ClipsShiftedEvents[1].LineNumberOfShiftedClip);
			}
		}
		
		[TestCase(2, 3)]
		[TestCase(2, 3, 4, 5)]
		[TestCase(3, 5)]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_ConsecutiveIntroTableMarkersInFolderWithClipsBeforeMigrationToHt203_ClipsShiftedAndNoChaptersPotentiallyNeedingManualMigration(params int[] clipsToHaveRecordings)
		{
			var presets = new Dictionary<int, string>
			{
				// This looks weird because Matthew should be book 40, but in the test VersificationInfo, it's book 1.
				[1000002] = "iot - Introduction - Outline Title",
				[1000003] = "io1 - Introduction - Outline Level 1",
				[1000004] = "io2 - Introduction - Outline Level 2"
			};

			using (var secretary = new ClipsShiftedSecretary())
			{
				var clipFilesThatShouldBeShifted = new List<string>(2);
				using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
				{
					clipFilesThatShouldBeShifted.AddRange(clipsToHaveRecordings.Select(
						clipNbr => CreateClipForBlock(projFolderPath, "Matthew", 0, clipNbr)));
					Thread.Sleep(1001); // file times are to the second.
					var skipInfo = SkippedScriptLines.Create(skippedLineInfoPath, self);
					var fileContentsWithVersionSetTo1 =
						XmlSerializationHelper.SerializeToString(skipInfo)
							.Replace($"<SkippedScriptLines version=\"{Settings.Default.CurrentSkippedLinesVersion}\">",
								"<SkippedScriptLines version=\"1\">");
					File.WriteAllText(skippedLineInfoPath, fileContentsWithVersionSetTo1);

					// This ensures that the initial call to DoDataMigration will be a no-op.
					SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
				}, presets))
				{
					Assert.IsFalse(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null).Any());
					Assert.AreEqual(Settings.Default.CurrentDataVersion, scriptProvider.GetVersionNumberFromProjectInfoFile());
					var folder = Path.GetDirectoryName(clipFilesThatShouldBeShifted.First());
					var wavFiles = Directory.GetFiles(folder);
					Assert.AreEqual(clipsToHaveRecordings.Length, wavFiles.Length);
					foreach (var i in clipsToHaveRecordings)
						Assert.IsTrue(File.Exists(Path.Combine(folder, $"{i + 3}.wav")));
				}
				Assert.AreEqual(3, secretary.ClipsShiftedEvents.Count);
				Assert.AreEqual("Matthew", secretary.ClipsShiftedEvents[0].BookName);
				Assert.IsTrue(secretary.ClipsShiftedEvents.All(c => c.ChapterNumber == 0));
				Assert.IsTrue(secretary.ClipsShiftedEvents.All(c => c.ShiftedBy == 1));
				Assert.AreEqual(clipsToHaveRecordings.First(), secretary.ClipsShiftedEvents[0].LineNumberOfShiftedClip);
				Assert.AreEqual(clipsToHaveRecordings.First() + 1, secretary.ClipsShiftedEvents[1].LineNumberOfShiftedClip);
				Assert.AreEqual(clipsToHaveRecordings.First() + 2, secretary.ClipsShiftedEvents[2].LineNumberOfShiftedClip);
			}
		}
		
		[Test]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_ParallelPassagesInFolderWithClipsBothBeforeAndAfterMigrationToHt203_ClipsNotShiftedAndChaptersPotentiallyNeedingManualMigration()
		{
			var presets = new Dictionary<int, string>
			{
				// This looks weird because Matthew should be book 40, but in the test VersificationInfo, it's book 1.
				[1002001] = "r - Heading - Parallel References",
				[1002004] = "r - Heading - Parallel References"
			};

			using (var secretary = new ClipsShiftedSecretary())
			{
				var clipFilesThatShouldStillExist = new List<string>(2);
				using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
				{
					clipFilesThatShouldStillExist.Add(CreateClipForBlock(projFolderPath, "Matthew", 2, 2));
					CreateClipForBlock(projFolderPath, "Matthew", 2, 5);
					Thread.Sleep(1001); // file times are to the second.
					var skipInfo = SkippedScriptLines.Create(skippedLineInfoPath, self);
					var fileContentsWithVersionSetTo1 =
						XmlSerializationHelper.SerializeToString(skipInfo)
							.Replace($"<SkippedScriptLines version=\"{Settings.Default.CurrentSkippedLinesVersion}\">",
								"<SkippedScriptLines version=\"1\">");
					File.WriteAllText(skippedLineInfoPath, fileContentsWithVersionSetTo1);
					Thread.Sleep(1001); // file times are to the second.
					clipFilesThatShouldStillExist.Add(CreateClipForBlock(projFolderPath, "Matthew", 2, 3));

					// This ensures that the initial call to DoDataMigration will be a no-op.
					SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
				}, presets))
				{
					var manual = scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null);
					Assert.AreEqual(1, manual.Count);
					Assert.AreEqual(2, manual["Matthew"].Single());
					Assert.AreEqual(Settings.Default.CurrentDataVersion, scriptProvider.GetVersionNumberFromProjectInfoFile());
					Assert.IsTrue(clipFilesThatShouldStillExist.All(File.Exists));
					var folder = Path.GetDirectoryName(clipFilesThatShouldStillExist.First());
					var wavFiles = Directory.GetFiles(folder);
					Assert.AreEqual(3, wavFiles.Length);
					Assert.IsTrue(File.Exists(Path.Combine(folder, "6.wav")));
				}
				Assert.AreEqual(1, secretary.ClipsShiftedEvents.Count);
				Assert.AreEqual("Matthew", secretary.ClipsShiftedEvents[0].BookName);
				Assert.AreEqual(2, secretary.ClipsShiftedEvents[0].ChapterNumber);
				Assert.AreEqual(5, secretary.ClipsShiftedEvents[0].LineNumberOfShiftedClip);
				Assert.AreEqual(1, secretary.ClipsShiftedEvents[0].ShiftedBy);
			}
		}

		private class ClipsShiftedSecretary : IDisposable
		{
			internal class ClipsShiftedParameters
			{
				public string BookName { get; }
				public int ChapterNumber { get; }
				public int LineNumberOfShiftedClip { get; }
				public int ShiftedBy { get; }

				public ClipsShiftedParameters(string bookName, int chapterNumber, int lineNumberOfShiftedClip, int shiftedBy)
				{
					BookName = bookName;
					ChapterNumber = chapterNumber;
					LineNumberOfShiftedClip = lineNumberOfShiftedClip;
					ShiftedBy = shiftedBy;
				}
			}

			private string _projectName;
			private IScriptProvider _scriptProvider;
			internal readonly List<ClipsShiftedParameters> ClipsShiftedEvents = new List<ClipsShiftedParameters>();

			public ClipsShiftedSecretary()
			{
				ClipRepository.ClipsShifted += NoteClipsShifted;
			}

			private void NoteClipsShifted(string projectName, string bookName, IScriptProvider scriptProvider, int chapterNumber, int lineNumberOfShiftedClip, int shiftedBy)
			{
				if (_projectName == null)
					_projectName = projectName;
				else
					Assert.AreEqual(_projectName, projectName, "Got an event for some other project!");
				if (_scriptProvider == null)
					_scriptProvider = scriptProvider;
				else
					Assert.AreEqual(_scriptProvider, scriptProvider, "Got an event for some other script provider!");

				ClipsShiftedEvents.Add(new ClipsShiftedParameters(bookName, chapterNumber, lineNumberOfShiftedClip, shiftedBy));
			}

			public void Dispose()
			{
				ClipRepository.ClipsShifted -= NoteClipsShifted;
			}
		}
		#endregion // Integration tests for HT-376

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

		private string CreateClipForBlock(string projFolderPath, string bookName, int chapterIndex, int lineIndex)
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
			return path;
		}
	}

	class TestScriptProviderForMigrationTests : TestScriptProvider, IDisposable
	{
		private readonly Dictionary<int, string> _presetParagraphStyles;
		private string _projectFolderName;

		public TestScriptProviderForMigrationTests(Action<ISkippedStyleInfoProvider, string, string, string> setupData,
			Dictionary<int, string> presetParagraphStyles = null)
		{
			_presetParagraphStyles = presetParagraphStyles;
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
			if (_presetParagraphStyles != null &&
				_presetParagraphStyles.TryGetValue(new BCVRef(bookNumber, chapterNumber, lineNumber0Based).BBCCCVVV, out var style))
			{
				return new ScriptLine("Preset.")
				{
					Number = lineNumber0Based + 1,
					ParagraphStyle = style
				};
			}

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
