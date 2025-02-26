using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HearThis;
using HearThis.Communication;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;
using SIL.IO;
using SIL.Progress;

namespace HearThisTests
{
	/// <summary>
	/// Tests the HearThisPackMaker (and Reader).
	/// </summary>
	[TestFixture]
	public class HearThisPackMakerTests
	{
		private string _testFolderPath;
		private string _projectName;
		private string _infoXmlFilePath;
		private string _infoXml;
		private StringBuilderProgress _progress;
		private List<string> _expectedEntries;

		[OneTimeSetUp]
		public void MakeTestData()
		{
			var folderNameBase = "fakePackProject";
			int i = 0;
			while (Directory.Exists(Path.Combine(Program.ApplicationDataBaseFolder, folderNameBase + i)))
				i++;
			_projectName = folderNameBase + i;
			_testFolderPath = Program.GetApplicationDataFolder(_projectName);
			var ex1Folder = ClipRepository.GetChapterFolder(_projectName, "Exodus", 1);

			SimulateWaveFiles(ex1Folder, new [] {1,2,3,4});
			var info = new ChapterInfo
			{
				ChapterNumber1Based = 1,
				Recordings = new List<ScriptLine>
				{
					new ScriptLine { Text = "this is line 1", Number = 1, Actor = "Fred", Character = "Jairus" },
					new ScriptLine { Text = "this is line 2", Number = 2, Actor = "Joe", Character = "Peter" },
					new ScriptLine { Text = "this is line 3", Number = 3, Actor = "Fred", Character = "Stephen" },
					new ScriptLine { Text = "this is line 4", Number = 4, Actor = "Sally", Character = "Mary" }
				}
			};

			_infoXml = info.ToXmlString();
			_infoXmlFilePath = Path.Combine(ex1Folder, ChapterInfo.kChapterInfoFilename);
			File.WriteAllText(_infoXmlFilePath, _infoXml);
		}

		[OneTimeTearDown]
		public void DestroySampleFolder()
		{
			Directory.Delete(_testFolderPath, true);
		}

		[TearDown]
		public void Teardown()
		{
			_progress = null;
			_expectedEntries = null;
		}

		[TestCase(true)]
		[TestCase(false)]
		public void MakePack_FilesExist_PackCreatedWithExpectedContent(bool useProgress)
		{
			if (useProgress)
				SetUpProgress();
			var maker = new HearThisPackMaker(_testFolderPath);
			using (var temp = TempFile.WithExtension(HearThisPackMaker.kHearThisPackExtension))
			{
				Assert.That(maker.Pack(temp.Path, _progress), Is.True);
				using (var reader = new HearThisPackReader(temp.Path))
				{
					var link = reader.GetLink();
					VerifyFileContent(link, "/Exodus/1/0.wav", "this is a fake wave file - 1");
					VerifyFileContent(link, "/Exodus/1/1.wav", "this is a fake wave file - 2");
					VerifyFileContent(link, "/Exodus/1/2.wav", "this is a fake wave file - 3");
					VerifyFileContent(link, "/Exodus/1/3.wav", "this is a fake wave file - 4");
					VerifyFileContent(link, "/Exodus/1/" + ChapterInfo.kChapterInfoFilename, _infoXml);
				}
			}

			if (useProgress)
				VerifyProgress();
		}

		[TestCase(true)]
		[TestCase(false)]
		public void MakeFilteredPack_SomeFilesExistForActor_PackCreatedWithExpectedContent(bool useProgress)
		{
			if (useProgress)
				SetUpProgress();
			var maker = new HearThisPackMaker(_testFolderPath);
			maker.Actor = "Fred";
			using (var temp = TempFile.WithExtension(HearThisPackMaker.kHearThisPackExtension))
			{
				Assert.That(maker.Pack(temp.Path, _progress), Is.True);
				using (var reader = new HearThisPackReader(temp.Path))
				{
					var link = reader.GetLink();
					VerifyFileContent(link, "/Exodus/1/0.wav", "this is a fake wave file - 1");
					VerifyFileContent(link, "/Exodus/1/2.wav", "this is a fake wave file - 3");
					VerifyFileContent(link, "/Exodus/1/" + ChapterInfo.kChapterInfoFilename, _infoXml);
					VerifyNoContent(link, "/Exodus/1/1.wav"); // Joe's line, not Fred's
					VerifyNoContent(link, "/Exodus/1/3.wav"); // Sally's line, not Fred's
				}
			}

			if (useProgress)
				VerifyProgress();
		}

		[TestCase(true)]
		[TestCase(false)]
		public void MakeFilteredPack_NoFilesExistForActorAndNoInfoOrSkipFilesExist_NoPackCreated(bool useProgress)
		{
			if (useProgress)
				SetUpProgress();
			try
			{
				File.Move(_infoXmlFilePath, _infoXmlFilePath + ".bak");
				var maker = new HearThisPackMaker(_testFolderPath);
				maker.Actor = "Ward";
				using (var temp = TempFile.WithExtension(HearThisPackMaker.kHearThisPackExtension))
				{
					Assert.That(maker.Pack(temp.Path, _progress), Is.False);
					Assert.That(File.Exists(temp.Path), Is.False);
				}
			}
			finally
			{
				File.Move(_infoXmlFilePath + ".bak", _infoXmlFilePath);
			}

			if (useProgress)
				Assert.That(_progress.Text, Is.EqualTo("Warning: There were no relevant clips" +
					" or other files in this project to include in the HearThis Pack." +
					Environment.NewLine));
		}

		private void VerifyFileContent(IAndroidLink link, string path, string expectedContent)
		{
			Assert.That(link.TryGetData(_projectName + path, out var output), Is.True);
			var content = Encoding.UTF8.GetString(output);
			Assert.That(content, Is.EqualTo(expectedContent));
			if (_progress != null)
			{
				_expectedEntries.Add(_projectName + path);
			}
		}

		private void VerifyNoContent(IAndroidLink link, string path)
		{
			Assert.That(link.TryGetData(_projectName + path, out var _), Is.False);
		}

		private static void SimulateWaveFiles(string folderPath, int[] blocks)
		{
			foreach (var block in blocks)
			{
				File.WriteAllText(Path.Combine(folderPath, (block - 1) + ".wav"),
					"this is a fake wave file - " + block);
			}
		}

		private void SetUpProgress()
		{
			_progress = new StringBuilderProgress();
			_expectedEntries = new List<string>();

		}
		
		private void VerifyProgress()
		{
			Assert.That(_progress, Is.Not.Null);
			Assert.That(_progress.Text.Split(new [] {"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries),
				Is.EquivalentTo(_expectedEntries));
		}
	}
}
