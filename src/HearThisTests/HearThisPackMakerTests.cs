using System.Collections.Generic;
using System.IO;
using System.Text;
using HearThis;
using HearThis.Communication;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;
using SIL.IO;

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
		private string _ex1Folder;
		private string _infoXml;

		[OneTimeSetUp]
		public void MakeTestData()
		{
			var folderNameBase = "fakePackProject";
			int i = 0;
			while (Directory.Exists(Path.Combine(Program.ApplicationDataBaseFolder, folderNameBase + i)))
				i++;
			_projectName = folderNameBase + i;
			_testFolderPath = Program.GetApplicationDataFolder(_projectName);
			_ex1Folder = ClipRepository.GetChapterFolder(_projectName, "Exodus", 1);

			SimulateWaveFiles(_ex1Folder, new [] {1,2,3,4});
			var info = new ChapterInfo();
			info.ChapterNumber1Based = 1;
			info.Recordings = new List<ScriptLine>();
			info.Recordings.Add(new ScriptLine() {Text="this is line 1", Number = 1, Actor = "Fred", Character = "Jairus"});
			info.Recordings.Add(new ScriptLine() { Text = "this is line 2", Number = 2, Actor = "Joe", Character = "Peter" });
			info.Recordings.Add(new ScriptLine() { Text = "this is line 3", Number = 3, Actor = "Fred", Character = "Stephen" });
			info.Recordings.Add(new ScriptLine() { Text = "this is line 4", Number = 4, Actor = "Sally", Character = "Mary" });
			_infoXml = info.ToXmlString();
			var infoPath = Path.Combine(_ex1Folder, ChapterInfo.kChapterInfoFilename);
			File.WriteAllText(infoPath, _infoXml);
		}

		[OneTimeTearDown]
		public void DestroySampleFolder()
		{
			Directory.Delete(_testFolderPath, true);
		}

		[Test]
		public void MakePack()
		{
			var maker = new HearThisPackMaker(_testFolderPath);
			using (var temp = TempFile.WithExtension(HearThisPackMaker.HearThisPackExtension))
			{
				maker.Pack(temp.Path, null);
				using (var reader = new HearThisPackReader(temp.Path))
				{
					var link = reader.GetLink();
					VerifyFileContent(link, "/Exodus/1/0.wav","this is a fake wave file - 1");
					VerifyFileContent(link, "/Exodus/1/1.wav", "this is a fake wave file - 2");
					VerifyFileContent(link, "/Exodus/1/" + ChapterInfo.kChapterInfoFilename, _infoXml);
				}
			}
		}


		[Test]
		public void MakeFilteredPack()
		{
			var maker = new HearThisPackMaker(_testFolderPath);
			maker.Actor = "Fred";
			using (var temp = TempFile.WithExtension(HearThisPackMaker.HearThisPackExtension))
			{
				maker.Pack(temp.Path, null);
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
		}

		void VerifyFileContent(IAndroidLink link, string path, string expectedContent)
		{
			byte[] output;
			Assert.That(link.TryGetData(_projectName + path, out output), Is.True);
			var content = Encoding.UTF8.GetString(output);
			Assert.That(content, Is.EqualTo(expectedContent));
		}

		void VerifyNoContent(IAndroidLink link, string path)
		{
			byte[] output;
			Assert.That(link.TryGetData(_projectName + path, out output), Is.False);
		}

		void SimulateWaveFiles(string folderPath, int[] blocks)
		{
			foreach (var block in blocks)
			{
				File.WriteAllText(Path.Combine(folderPath, (block - 1).ToString() + ".wav"), "this is a fake wave file - " + block);
			}
		}
	}
}
