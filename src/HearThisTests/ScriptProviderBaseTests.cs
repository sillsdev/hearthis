// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2016-2025, SIL Global.
// <copyright from='2016' to='2025' company='SIL Global'>
//		Copyright (c) 2016-2025, SIL Global.
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
				Assert.That(ClipRepository.HasClip(scriptProvider.ProjectFolderName, "Matthew", 1, 1), Is.True);
				Assert.That(ClipRepository.HasClip(scriptProvider.ProjectFolderName, "Matthew", 2, 2), Is.True);
				Assert.That(ClipRepository.HasClip(scriptProvider.ProjectFolderName, "Matthew", 3, 1), Is.True);
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(), Is.EqualTo(Settings.Default.CurrentDataVersion));
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
				Assert.That(ClipRepository.HasClip(scriptProvider.ProjectFolderName, "Matthew", 1, 1), Is.True);
				VerifyClipWasBackedUp(scriptProvider.ProjectFolderName, "Matthew", 2, 0);
				Assert.That(ClipRepository.HasClip(scriptProvider.ProjectFolderName, "Matthew", 2, 1), Is.True);
				VerifyClipWasBackedUp(scriptProvider.ProjectFolderName, "Matthew", 2, 2);
				Assert.That(ClipRepository.HasClip(scriptProvider.ProjectFolderName, "Matthew", 2, 3), Is.True);
				VerifyClipWasBackedUp(scriptProvider.ProjectFolderName, "Matthew", 2, 4);
				Assert.That(ClipRepository.HasClip(scriptProvider.ProjectFolderName, "Matthew", 2, 5), Is.True);
				Assert.That(ClipRepository.HasClip(scriptProvider.ProjectFolderName, "Matthew", 3, 1), Is.True);
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(),
					Is.EqualTo(Settings.Default.CurrentDataVersion));
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

				Assert.That(scriptProvider.ProjectSettings.BreakAtParagraphBreaks, Is.EqualTo(expectedResult));
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(),
					Is.EqualTo(Settings.Default.CurrentDataVersion));
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
				Assert.That(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null), Is.Empty);
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(), Is.EqualTo(Settings.Default.CurrentDataVersion));
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

			var clipFilesThatShouldStillExist = new List<string>(2);
			using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				SimulateRunOfVersion2_2_3(skippedLineInfoPath, self);

				// This ensures that the initial call to DoDataMigration will be a no-op.
				SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
				Thread.Sleep(1001); // file times are to the second.
				clipFilesThatShouldStillExist.Add(CreateClipForBlock(projFolderPath, "Matthew", 2, 2));
				clipFilesThatShouldStillExist.Add(CreateClipForBlock(projFolderPath, "Matthew", 2, 3));
			}, presets))
			{
				Assert.That(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null), Is.Empty);
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(), Is.EqualTo(Settings.Default.CurrentDataVersion));
				Assert.That(clipFilesThatShouldStillExist, Is.All.Exist);
				if (scriptProvider.TryGetChapterInfo(scriptProvider.VersificationInfo.GetBookNumber("Matthew"), 2, out var chapterInfo))
					Assert.That(chapterInfo.SaveCallCount, Is.EqualTo(0));
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

			var clipFilesThatShouldBeShifted = new List<string>(2);
			using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				clipFilesThatShouldBeShifted.Add(CreateClipForBlock(projFolderPath, "Matthew", 2, 2));
				clipFilesThatShouldBeShifted.Add(CreateClipForBlock(projFolderPath, "Matthew", 2, 3));
				Thread.Sleep(1001); // file times are to the second.
				SimulateRunOfVersion2_2_3(skippedLineInfoPath, self);

				// This ensures that the initial call to DoDataMigration will be a no-op.
				SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
			}, presets))
			{
				var bookNum = scriptProvider.VersificationInfo.GetBookNumber("Matthew");
				var chapterInfo = new TestScriptProviderForMigrationTests.TestChapterInfo();
				var origFile2CreateTime = File.GetCreationTime(clipFilesThatShouldBeShifted[0]);
				chapterInfo.OnScriptBlockRecorded(new ScriptLine("This is the text for block #2.")
					{Number = 3, RecordingTime = origFile2CreateTime});
				var origFile3CreateTime = File.GetCreationTime(clipFilesThatShouldBeShifted[1]);
				chapterInfo.OnScriptBlockRecorded(new ScriptLine("This is the text for block #3.")
					{Number = 4, RecordingTime = origFile3CreateTime});
				scriptProvider.AddTestChapterInfo(bookNum, 2, chapterInfo);

				// SUT
				Assert.That(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null), Is.Empty);
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(), Is.EqualTo(Settings.Default.CurrentDataVersion));
				Assert.That(clipFilesThatShouldBeShifted.First(), Does.Not.Exist);
				Assert.That(clipFilesThatShouldBeShifted.Last(), Does.Exist);
				Assert.That(clipFilesThatShouldBeShifted.Last().Replace("3.wav", "4.wav"), Does.Exist);
				Assert.That(chapterInfo.SaveCallCount, Is.EqualTo(1));
				int i = 0;
				Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(4));
				Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(origFile2CreateTime));
				Assert.That(chapterInfo.RecordingInfo[i++].Text, Is.EqualTo("This is the text for block #2."));
				Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(5));
				Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(origFile3CreateTime));
				Assert.That(chapterInfo.RecordingInfo[i++].Text, Is.EqualTo("This is the text for block #3."));
				Assert.That(chapterInfo.RecordingInfo.Count, Is.EqualTo(i));
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

			string clipFilesFolder = "";
			using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				clipFilesFolder = Path.GetDirectoryName(CreateClipForBlock(projFolderPath, "Matthew", 2, 0));
				CreateClipForBlock(projFolderPath, "Matthew", 2, 1);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 2);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 3);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 4);
				Thread.Sleep(1001); // file times are to the second.
				SimulateRunOfVersion2_2_3(skippedLineInfoPath, self);

				// This ensures that the initial call to DoDataMigration will be a no-op.
				SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
			}, presets))
			{
				var bookNum = scriptProvider.VersificationInfo.GetBookNumber("Matthew");
				var chapterInfo = new TestScriptProviderForMigrationTests.TestChapterInfo();
				var fileTimes = new List<DateTime>();
				fileTimes.Add(File.GetCreationTime(Path.Combine(clipFilesFolder, "0.wav")));
				chapterInfo.OnScriptBlockRecorded(new ScriptLine("Text for block #0.")
					{Number = 1, RecordingTime = fileTimes.Last()});
				fileTimes.Add(File.GetCreationTime(Path.Combine(clipFilesFolder, "1.wav")));
				chapterInfo.OnScriptBlockRecorded(new ScriptLine("Text for block #1.")
					{Number = 2, RecordingTime = fileTimes.Last()});
				fileTimes.Add(File.GetCreationTime(Path.Combine(clipFilesFolder, "2.wav")));
				chapterInfo.OnScriptBlockRecorded(new ScriptLine("Text for block #2.")
					{Number = 3, RecordingTime = fileTimes.Last()});
				fileTimes.Add(File.GetCreationTime(Path.Combine(clipFilesFolder, "3.wav")));
				chapterInfo.OnScriptBlockRecorded(new ScriptLine("Text for block #3.")
					{Number = 4, RecordingTime = fileTimes.Last()});
				fileTimes.Add(File.GetCreationTime(Path.Combine(clipFilesFolder, "4.wav")));
				chapterInfo.OnScriptBlockRecorded(new ScriptLine("Text for block #4.")
					{Number = 5, RecordingTime = fileTimes.Last()});
				scriptProvider.AddTestChapterInfo(bookNum, 2, chapterInfo);

				// SUT
				Assert.That(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null), Is.Empty);
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(), Is.EqualTo(Settings.Default.CurrentDataVersion));
				var wavFiles = Directory.GetFiles(clipFilesFolder);
				Assert.That(wavFiles.Length, Is.EqualTo(5));
				Assert.That(Path.Combine(clipFilesFolder, "0.wav"), Does.Exist);
				Assert.That(Path.Combine(clipFilesFolder, "2.wav"), Does.Exist);
				Assert.That(Path.Combine(clipFilesFolder, "3.wav"), Does.Exist);
				Assert.That(Path.Combine(clipFilesFolder, "5.wav"), Does.Exist);
				Assert.That(Path.Combine(clipFilesFolder, "6.wav"), Does.Exist);

				Assert.That(chapterInfo.SaveCallCount, Is.EqualTo(2));
				int i = 0;
				Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(1));
				Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(fileTimes[i]));
				Assert.That(chapterInfo.RecordingInfo[i].Text, Is.EqualTo($"Text for block #{i++}."));
				Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(3));
				Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(fileTimes[i]));
				Assert.That(chapterInfo.RecordingInfo[i].Text, Is.EqualTo($"Text for block #{i++}."));
				Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(4));
				Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(fileTimes[i]));
				Assert.That(chapterInfo.RecordingInfo[i].Text, Is.EqualTo($"Text for block #{i++}."));
				Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(6));
				Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(fileTimes[i]));
				Assert.That(chapterInfo.RecordingInfo[i].Text, Is.EqualTo($"Text for block #{i++}."));
				Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(7));
				Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(fileTimes[i]));
				Assert.That(chapterInfo.RecordingInfo[i].Text, Is.EqualTo($"Text for block #{i++}."));
				Assert.That(chapterInfo.RecordingInfo.Count, Is.EqualTo(i));
			}
		}

		[TestCase(2, 3)]
		[TestCase(2, 3, 4, 5)]
		[TestCase(3, 5)]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_ConsecutiveIntroTableMarkersInFolderWithClipsBeforeMigrationToHt203_ClipsShiftedAndNoChaptersPotentiallyNeedingManualMigration(
			params int[] clipsToHaveRecordings)
		{
			const int kChapter = 0;
			var presets = new Dictionary<int, string>
			{
				// This looks weird because Matthew should be book 40, but in the test VersificationInfo, it's book 1.
				[1_000_002] = "iot - Introduction - Outline Title",
				[1_000_003] = "io1 - Introduction - Outline Level 1",
				[1_000_004] = "io2 - Introduction - Outline Level 2"
			};

			var clipFilesThatShouldBeShifted = new List<string>(4);
			using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				clipFilesThatShouldBeShifted.AddRange(clipsToHaveRecordings.Select(
					clipNbr => CreateClipForBlock(projFolderPath, "Matthew", kChapter, clipNbr)));
				Thread.Sleep(1001); // file times are to the second.
				SimulateRunOfVersion2_2_3(skippedLineInfoPath, self);

				// This ensures that the initial call to DoDataMigration will be a no-op.
				SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
			}, presets))
			{
				var bookNum = scriptProvider.VersificationInfo.GetBookNumber("Matthew");
				var chapterInfo = new TestScriptProviderForMigrationTests.TestChapterInfo();
				List<DateTime> fileTimes = new List<DateTime>();
				for (var index = 0; index < clipsToHaveRecordings.Length; index++)
				{
					var fileTime = File.GetCreationTime(clipFilesThatShouldBeShifted[index]);
					fileTimes.Add(fileTime);
					int clipNbr = clipsToHaveRecordings[index];
					chapterInfo.OnScriptBlockRecorded(new ScriptLine($"Text for block #{clipNbr}.")
						{Number = clipNbr + 1, RecordingTime = fileTime});
				}
				scriptProvider.AddTestChapterInfo(bookNum, kChapter, chapterInfo);

				// SUT
				Assert.That(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null), Is.Empty);
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(), Is.EqualTo(Settings.Default.CurrentDataVersion));
				var folder = Path.GetDirectoryName(clipFilesThatShouldBeShifted.First());
				var wavFiles = Directory.GetFiles(folder);
				Assert.That(wavFiles.Length, Is.EqualTo(clipsToHaveRecordings.Length));
				foreach (var i in clipsToHaveRecordings)
					Assert.That(Path.Combine(folder, $"{i + 3}.wav"), Does.Exist);

				Assert.That(chapterInfo.SaveCallCount, Is.EqualTo(3));
				Assert.That(chapterInfo.RecordingInfo.Count, Is.EqualTo(clipsToHaveRecordings.Length));

				for (var i = 0; i < clipsToHaveRecordings.Length; i++)
				{
					int clipNbr = clipsToHaveRecordings[i];
					Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(clipNbr + 1 + presets.Count));
					Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(fileTimes[i]));
					Assert.That(chapterInfo.RecordingInfo[i].Text, Is.EqualTo($"Text for block #{clipNbr}."));
				}
			}
		}

		[Test]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_ParallelPassagesInFolderWithClipsBothBeforeAndAfterMigrationToHt203_ClipsNotShiftedAndChaptersPotentiallyNeedingManualMigration()
		{
			const int kChapter = 2;
			var presets = new Dictionary<int, string>
			{
				// This looks weird because Matthew should be book 40, but in the test VersificationInfo, it's book 1.
				[1002001] = "r - Heading - Parallel References",
				[1002004] = "r - Heading - Parallel References"
			};

			var clipFilesThatShouldStillExist = new List<string>(2);
			var clipFileToShift = "";
			using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				clipFilesThatShouldStillExist.Add(CreateClipForBlock(projFolderPath, "Matthew", kChapter, 2));
				clipFileToShift = CreateClipForBlock(projFolderPath, "Matthew", kChapter, 5);
				Thread.Sleep(1001); // file times are to the second.
				SimulateRunOfVersion2_2_3(skippedLineInfoPath, self);
				Thread.Sleep(1001); // file times are to the second.
				clipFilesThatShouldStillExist.Add(CreateClipForBlock(projFolderPath, "Matthew", kChapter, 3));

				// This ensures that the initial call to DoDataMigration will be a no-op.
				SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
			}, presets))
			{
				var bookNum = scriptProvider.VersificationInfo.GetBookNumber("Matthew");
				var chapterInfo = new TestScriptProviderForMigrationTests.TestChapterInfo();
				var origFile2CreateTime = File.GetCreationTime(clipFilesThatShouldStillExist[0]);
				chapterInfo.OnScriptBlockRecorded(new ScriptLine("This is the text for block #2.")
					{Number = 3, RecordingTime = origFile2CreateTime});
				var origFile3CreateTime = File.GetCreationTime(clipFilesThatShouldStillExist[1]);
				chapterInfo.OnScriptBlockRecorded(new ScriptLine("This is the text for block #3.")
					{Number = 4, RecordingTime = origFile3CreateTime});
				var origFile5CreateTime = File.GetCreationTime(clipFileToShift);
				chapterInfo.OnScriptBlockRecorded(new ScriptLine("This is the text for block #5.")
					{Number = 6, RecordingTime = origFile5CreateTime});
				scriptProvider.AddTestChapterInfo(bookNum, kChapter, chapterInfo);

				// SUT
				var manual = scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null);
				Assert.That(manual.Count, Is.EqualTo(1));
				Assert.That(manual["Matthew"].Single(), Is.EqualTo(2));
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(), Is.EqualTo(Settings.Default.CurrentDataVersion));
				Assert.That(clipFilesThatShouldStillExist, Is.All.Exist);
				var folder = Path.GetDirectoryName(clipFilesThatShouldStillExist.First());
				var wavFiles = Directory.GetFiles(folder);
				Assert.That(wavFiles.Length, Is.EqualTo(3));
				Assert.That(Path.Combine(folder, "6.wav"), Does.Exist);

				Assert.That(chapterInfo.SaveCallCount, Is.EqualTo(1));
				int i = 0;
				Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(3));
				Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(origFile2CreateTime));
				Assert.That(chapterInfo.RecordingInfo[i++].Text, Is.EqualTo("This is the text for block #2."));
				Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(4));
				Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(origFile3CreateTime));
				Assert.That(chapterInfo.RecordingInfo[i++].Text, Is.EqualTo("This is the text for block #3."));
				Assert.That(chapterInfo.RecordingInfo[i].Number, Is.EqualTo(7));
				Assert.That(chapterInfo.RecordingInfo[i].RecordingTime, Is.EqualTo(origFile5CreateTime));
				Assert.That(chapterInfo.RecordingInfo[i++].Text, Is.EqualTo("This is the text for block #5."));
				Assert.That(chapterInfo.RecordingInfo.Count, Is.EqualTo(i));
			}
		}

		[Test]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_SimulatedMidChapterInterruption_ClipsShiftedInRemainingChapterAndInterruptedChapterPotentiallyNeedingManualMigration()
		{
			var presets = new Dictionary<int, string>
			{
				// This looks weird because Matthew should be book 40, but in the test VersificationInfo, it's book 1.
				[1000002] = "iot - Introduction - Outline Title",
				[1000003] = "io1 - Introduction - Outline Level 1",
				[1000004] = "io2 - Introduction - Outline Level 2",
				[1001001] = "r - Heading - Parallel References",
				[1002002] = "r - Heading - Parallel References"
			};

			var folderForMatthew = "";
			using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				// This first clip represents one that was successfully migrated in the initial attempt
				folderForMatthew = Path.GetDirectoryName(Path.GetDirectoryName(CreateClipForBlock(projFolderPath, "Matthew", 0, 5)));
				// This second clip represents one that is in an unknown state because the initial migration was interrupted.
				CreateClipForBlock(projFolderPath, "Matthew", 1, 1);
				// This final clip is in a chapter that was never reached during the initial migration.
				CreateClipForBlock(projFolderPath, "Matthew", 2, 2);
				Thread.Sleep(1001); // file times are to the second.
				SimulateRunOfVersion2_2_3(skippedLineInfoPath, self);

				// This ensures that the initial call to DoDataMigration will be a no-op.
				SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
			}, presets))
			{
				var bookNum = scriptProvider.VersificationInfo.GetBookNumber("Matthew");

				var chapter0Info = new TestScriptProviderForMigrationTests.TestChapterInfo();
				chapter0Info.OnScriptBlockRecorded(new ScriptLine("This is the text for block 0:5.")
					{Number = 6});
				scriptProvider.AddTestChapterInfo(bookNum, 0, chapter0Info);
				
				var chapter1Info = new TestScriptProviderForMigrationTests.TestChapterInfo();
				chapter1Info.OnScriptBlockRecorded(new ScriptLine("This is the text for block 1:1.")
					{Number = 2});
				scriptProvider.AddTestChapterInfo(bookNum, 1, chapter1Info);

				var chapter2Info = new TestScriptProviderForMigrationTests.TestChapterInfo();
				chapter2Info.OnScriptBlockRecorded(new ScriptLine("This is the text for block 2:2.")
					{Number = 3});
				scriptProvider.AddTestChapterInfo(bookNum, 2, chapter2Info);

				// Simulate interrupted migration
				var tracker = MigrationProgressTracker.Create(Path.GetDirectoryName(folderForMatthew), b => throw new Exception("Unexpected call"));
				tracker.Start(bookNum, 1);

				// SUT
				var manual = scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null);
				Assert.That(manual.Count, Is.EqualTo(1));
				Assert.That(manual["Matthew"].Single(), Is.EqualTo(1), "The interrupted chapter");
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(), Is.EqualTo(Settings.Default.CurrentDataVersion));
				Assert.That(Path.GetFileName(Directory.GetFiles(Path.Combine(folderForMatthew, "0")).Single()), Is.EqualTo("5.wav"));
				Assert.That(Path.GetFileName(Directory.GetFiles(Path.Combine(folderForMatthew, "1")).Single()), Is.EqualTo("1.wav"));
				Assert.That(Path.GetFileName(Directory.GetFiles(Path.Combine(folderForMatthew, "2")).Single()), Is.EqualTo("3.wav"));

				// Ensure that the migration progress tracker is not left in a state
				// that would imply that a migration was incomplete.
				var newTracker = MigrationProgressTracker.Create(Path.GetDirectoryName(folderForMatthew), b => throw new Exception("Unexpected call"));
				Assert.That(newTracker.PreviousMigrationWasInterrupted, Is.False);

				Assert.That(chapter0Info.SaveCallCount, Is.EqualTo(0));
				Assert.That(chapter1Info.SaveCallCount, Is.EqualTo(0));
				Assert.That(chapter2Info.SaveCallCount, Is.EqualTo(1));
				Assert.That(chapter2Info.RecordingInfo[0].Number, Is.EqualTo(4));
				Assert.That(chapter2Info.RecordingInfo[0].Text, Is.EqualTo("This is the text for block 2:2."));
				Assert.That(chapter2Info.RecordingInfo.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_SimulatedInterChapterInterruption_ClipsShiftedInRemainingChaptersAndNoChaptersPotentiallyNeedingManualMigration()
		{
			var presets = new Dictionary<int, string>
			{
				// This looks weird because Matthew should be book 40, but in the test VersificationInfo, it's book 1.
				[1000002] = "iot - Introduction - Outline Title",
				[1000003] = "io1 - Introduction - Outline Level 1",
				[1000004] = "io2 - Introduction - Outline Level 2",
				[1001001] = "r - Heading - Parallel References",
				[1002002] = "r - Heading - Parallel References"
			};

			var folderForMatthew = "";
			using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				// This first clip represents one that was successfully migrated in the initial attempt
				folderForMatthew = Path.GetDirectoryName(Path.GetDirectoryName(CreateClipForBlock(projFolderPath, "Matthew", 0, 5)));
				// Remaining clips represent ones in chapters never reached during the initial migration.
				CreateClipForBlock(projFolderPath, "Matthew", 1, 1);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 2);
				Thread.Sleep(1001); // file times are to the second.
				SimulateRunOfVersion2_2_3(skippedLineInfoPath, self);

				// This ensures that the initial call to DoDataMigration will be a no-op.
				SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
			}, presets))
			{
				var bookNum = scriptProvider.VersificationInfo.GetBookNumber("Matthew");

				var chapter0Info = new TestScriptProviderForMigrationTests.TestChapterInfo();
				chapter0Info.OnScriptBlockRecorded(new ScriptLine("This is the text for block 0:5.")
					{Number = 6});
				scriptProvider.AddTestChapterInfo(bookNum, 0, chapter0Info);
				
				var chapter1Info = new TestScriptProviderForMigrationTests.TestChapterInfo();
				chapter1Info.OnScriptBlockRecorded(new ScriptLine("This is the text for block 1:1.")
					{Number = 2});
				scriptProvider.AddTestChapterInfo(bookNum, 1, chapter1Info);

				var chapter2Info = new TestScriptProviderForMigrationTests.TestChapterInfo();
				chapter2Info.OnScriptBlockRecorded(new ScriptLine("This is the text for block 2:2.")
					{Number = 3});
				scriptProvider.AddTestChapterInfo(bookNum, 2, chapter2Info);

				// Simulate interrupted migration
				var tracker = MigrationProgressTracker.Create(Path.GetDirectoryName(folderForMatthew), b => throw new Exception("Unexpected call"));
				tracker.Start(bookNum, 0);
				tracker.NoteCompletedCurrentBookAndChapter();
				// SUT
				Assert.That(scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null), Is.Empty);
				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(), Is.EqualTo(Settings.Default.CurrentDataVersion));
				Assert.That(Path.GetFileName(Directory.GetFiles(Path.Combine(folderForMatthew, "0")).Single()), Is.EqualTo("5.wav"));
				Assert.That(Path.GetFileName(Directory.GetFiles(Path.Combine(folderForMatthew, "1")).Single()), Is.EqualTo("2.wav"));
				Assert.That(Path.GetFileName(Directory.GetFiles(Path.Combine(folderForMatthew, "2")).Single()), Is.EqualTo("3.wav"));

				// Ensure that the migration progress tracker is not left in a state
				// that would imply that a migration was incomplete.
				var newTracker = MigrationProgressTracker.Create(Path.GetDirectoryName(folderForMatthew), b => throw new Exception("Unexpected call"));
				Assert.That(newTracker.PreviousMigrationWasInterrupted, Is.False);

				Assert.That(chapter0Info.SaveCallCount, Is.EqualTo(0));
				Assert.That(chapter1Info.SaveCallCount, Is.EqualTo(1));
				Assert.That(chapter2Info.SaveCallCount, Is.EqualTo(1));

				Assert.That(chapter0Info.RecordingInfo[0].Number, Is.EqualTo(6));

				Assert.That(chapter1Info.RecordingInfo[0].Number, Is.EqualTo(3));
				Assert.That(chapter1Info.RecordingInfo[0].Text, Is.EqualTo("This is the text for block 1:1."));
				Assert.That(chapter1Info.RecordingInfo.Count, Is.EqualTo(1));

				Assert.That(chapter2Info.RecordingInfo[0].Number, Is.EqualTo(4));
				Assert.That(chapter2Info.RecordingInfo[0].Text, Is.EqualTo("This is the text for block 2:2."));
				Assert.That(chapter2Info.RecordingInfo.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void MigrateDataToVersion4ByShiftingClipsAsNeeded_ChapterBeforeInterruptionNeededManualMigration_ClipsShiftedInRemainingChaptersAndChaptersPotentiallyNeedingManualMigrationIncludesPreviouslyEncounteredChapter()
		{
			var presets = new Dictionary<int, string>
			{
				// This looks weird because Matthew should be book 40, but in the test VersificationInfo, it's book 1.
				[1000002] = "iot - Introduction - Outline Title",
				[1000003] = "io1 - Introduction - Outline Level 1",
				[1000004] = "io2 - Introduction - Outline Level 2",
				[1001001] = "r - Heading - Parallel References",
				[1002002] = "r - Heading - Parallel References"
			};

			var folderForMatthew = "";
			using (var scriptProvider = new TestScriptProviderForMigrationTests((self, projFolderPath, skippedLineInfoPath, projectSettingsPath) =>
			{
				// This first clip is in a chapter encountered the initial attempt - required manual migration.
				folderForMatthew = Path.GetDirectoryName(Path.GetDirectoryName(CreateClipForBlock(projFolderPath, "Matthew", 0, 5)));
				// This second clip represents one that is in an unknown state because the initial migration was interrupted.
				CreateClipForBlock(projFolderPath, "Matthew", 1, 1);
				// This final clip is in a chapter that was never reached during the initial migration.
				CreateClipForBlock(projFolderPath, "Matthew", 2, 2);
				Thread.Sleep(1001); // file times are to the second.
				SimulateRunOfVersion2_2_3(skippedLineInfoPath, self);
				Thread.Sleep(1001); // file times are to the second.
									// This is why chapter 0 required manual migration: some clips recorded before and some after
				CreateClipForBlock(projFolderPath, "Matthew", 0, 2);
				CreateClipForBlock(projFolderPath, "Matthew", 0, 3);
				CreateClipForBlock(projFolderPath, "Matthew", 2, 3);

				// This ensures that the initial call to DoDataMigration will be a no-op.
				SetVersionNumberBeforeInitialize(projectSettingsPath, 4);
			}, presets))
			{
				var bookNum = scriptProvider.VersificationInfo.GetBookNumber("Matthew");

				var chapter0Info = new TestScriptProviderForMigrationTests.TestChapterInfo();
				chapter0Info.OnScriptBlockRecorded(new ScriptLine("This is the text for block 0:5.")
					{Number = 6});
				scriptProvider.AddTestChapterInfo(bookNum, 0, chapter0Info);
				
				var chapter1Info = new TestScriptProviderForMigrationTests.TestChapterInfo();
				chapter1Info.OnScriptBlockRecorded(new ScriptLine("This is the text for block 1:1.")
					{Number = 2});
				scriptProvider.AddTestChapterInfo(bookNum, 1, chapter1Info);

				var chapter2Info = new TestScriptProviderForMigrationTests.TestChapterInfo();
				chapter2Info.OnScriptBlockRecorded(new ScriptLine("This is the text for block 2:2.")
					{Number = 3});
				scriptProvider.AddTestChapterInfo(bookNum, 2, chapter2Info);

				// Simulate interrupted migration
				var tracker = MigrationProgressTracker.Create(Path.GetDirectoryName(folderForMatthew), b => throw new Exception("Unexpected call"));
				// This test doesn't have any clips for Genesis, but just to be sure that if a previous
				// migration added something for Genesis, it isn't lost:
				tracker.ChaptersPotentiallyNeedingManualMigration.Add("Genesis", new List<int>(new[] { 6 }));
				tracker.ChaptersPotentiallyNeedingManualMigration.Add("Matthew", new List<int>(new[] { 0 }));
				tracker.Start(bookNum, 1);

				// SUT
				var manual = scriptProvider.MigrateDataToVersion4ByShiftingClipsAsNeeded(null);
				Assert.That(manual.Count, Is.EqualTo(2));
				Assert.That(manual["Genesis"].Single(), Is.EqualTo(6));
				Assert.That(manual["Matthew"], Is.EqualTo(new [] { 0, 1, 2 }),
					"Should be: 0)the one from the initial migration, " +
					"1) the interrupted chapter, " +
					"2) the one from the resumed migration");

				Assert.That(scriptProvider.GetVersionNumberFromProjectInfoFile(), Is.EqualTo(Settings.Default.CurrentDataVersion));
				var chapter0Folder = Path.Combine(folderForMatthew, "0");
				Assert.That(Directory.GetFiles(chapter0Folder).Length, Is.EqualTo(3));
				Assert.That(Path.Combine(chapter0Folder, "5.wav"), Does.Exist);
				Assert.That(Path.Combine(chapter0Folder, "2.wav"), Does.Exist);
				Assert.That(Path.Combine(chapter0Folder, "3.wav"), Does.Exist);
				Assert.That(Path.GetFileName(Directory.GetFiles(Path.Combine(folderForMatthew, "1")).Single()), Is.EqualTo("1.wav"));
				var chapter2Folder = Path.Combine(folderForMatthew, "0");
				Assert.That(Directory.GetFiles(chapter2Folder).Length, Is.EqualTo(3));
				Assert.That(Path.Combine(chapter2Folder, "2.wav"), Does.Exist);
				Assert.That(Path.Combine(chapter2Folder, "3.wav"), Does.Exist);

				// Ensure that the migration progress tracker is not left in a state
				// that would imply that a migration was incomplete.
				var newTracker = MigrationProgressTracker.Create(Path.GetDirectoryName(folderForMatthew), b => throw new Exception("Unexpected call"));
				Assert.That(newTracker.PreviousMigrationWasInterrupted, Is.False);

				Assert.That(chapter0Info.SaveCallCount, Is.EqualTo(0));
				Assert.That(chapter1Info.SaveCallCount, Is.EqualTo(0));
				Assert.That(chapter2Info.SaveCallCount, Is.EqualTo(0));

				Assert.That(chapter0Info.RecordingInfo[0].Number, Is.EqualTo(6));
				Assert.That(chapter1Info.RecordingInfo[0].Number, Is.EqualTo(2));
				Assert.That(chapter2Info.RecordingInfo[0].Number, Is.EqualTo(3));
			}
		}

		private static void SimulateRunOfVersion2_2_3(string skippedLineInfoPath, ISkippedStyleInfoProvider self)
		{
			var skipInfo = SkippedScriptLines.Create(skippedLineInfoPath, self);
			var fileContentsWithVersionSetTo1 =
				XmlSerializationHelper.SerializeToString(skipInfo)
					.Replace($"<SkippedScriptLines version=\"{Settings.Default.CurrentSkippedLinesVersion}\">",
						"<SkippedScriptLines version=\"1\">");
			File.WriteAllText(skippedLineInfoPath, fileContentsWithVersionSetTo1);
		}

		#endregion // Integration tests for HT-376

		private void SetVersionNumberBeforeInitialize(string projectSettingsPath, int version)
		{
			ProjectSettings projectSettings = new ProjectSettings { Version = version };
			XmlSerializationHelper.SerializeToFile(projectSettingsPath, projectSettings);
		}

		private void VerifyClipWasBackedUp(string projFolderPath, string bookName, int chapterIndex, int lineIndex)
		{
			Assert.That(ClipRepository.HasClip(projFolderPath, bookName, chapterIndex, lineIndex), Is.False);
			var recordingPath = ClipRepository.GetPathToLineRecording(projFolderPath, bookName, chapterIndex, lineIndex);
			var skipPath = Path.ChangeExtension(recordingPath, "skip");
			Assert.That(skipPath, Does.Exist);
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

	internal class TestScriptProviderForMigrationTests : TestScriptProvider, IDisposable
	{
		private readonly Dictionary<int, string> _presetParagraphStyles;
		private readonly Dictionary<int, Dictionary<int, TestChapterInfo>> _chapterInfoRepo =
			new Dictionary<int, Dictionary<int, TestChapterInfo>>();
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

		public void AddTestChapterInfo(int bookNum, int chapterNum, TestChapterInfo info)
		{
			if (!_chapterInfoRepo.TryGetValue(bookNum, out var book))
				_chapterInfoRepo[bookNum] = book = new Dictionary<int, TestChapterInfo>();

			book.Add(chapterNum, info);
		}

		public bool TryGetChapterInfo(int bookNum, int chapterNum, out TestChapterInfo info)
		{
			info = null;
			return _chapterInfoRepo.TryGetValue(bookNum, out var book) &&
				book.TryGetValue(chapterNum, out info);
		}

		protected override ChapterRecordingInfoBase GetChapterInfo(int bookNum, int chapterNum)
		{
			if (!_chapterInfoRepo.TryGetValue(bookNum, out var book))
				_chapterInfoRepo[bookNum] = book = new Dictionary<int, TestChapterInfo>();

			if (!book.TryGetValue(chapterNum, out var chapter))
				book[chapterNum] = chapter = new TestChapterInfo();
			return chapter;
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

		public class TestChapterInfo : ChapterRecordingInfoBase
		{
			private readonly List<ScriptLine> _recordings = new List<ScriptLine>();

			public int SaveCallCount { get; private set; }

			public override IReadOnlyList<ScriptLine> RecordingInfo => _recordings;

			public override void OnScriptBlockRecorded(ScriptLine scriptBlock,
				Func<Exception, bool> exceptionHandlerOverride = null)
			{
				if (_recordings.Any(r => r.Number >= scriptBlock.Number))
					throw new InvalidOperationException("For these tests, simulate recording blocks in order");
				_recordings.Add(scriptBlock);
				Assert.That(exceptionHandlerOverride, Is.Null,
					"For these tests, no special exception handling is expected");
			}

			public override void Save(bool preserveModifiedTime = false)
			{
				SaveCallCount++;
			}
		}
	}
}
