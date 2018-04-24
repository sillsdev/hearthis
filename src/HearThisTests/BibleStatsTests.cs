using System.IO;
using HearThis.Script;
using HearThisTests.Properties;
using NUnit.Framework;
using Paratext.Data;
using SIL.Scripture;

namespace HearThisTests
{
	[TestFixture]
	public sealed class BibleStatsTests
	{
		[Test]
		public void BibleStats_GetsInfoAsShippedWithHearThis()
		{
			var stats = new BibleStats();
			Assert.AreEqual(66, stats.BookCount);
			Assert.AreEqual("Rev", stats.GetBookCode(65));
			Assert.AreEqual("Revelation", stats.GetBookName(65));
			Assert.AreEqual(30, stats.GetVersesInChapter(stats.GetBookNumber("Daniel"), 3));
			Assert.AreEqual(10, stats.GetChaptersInBook(stats.GetBookNumber("Esther")));
		}
	}

	[TestFixture]
	public sealed class ParatextVersificationInfoTests
	{
		[Test]
		public void ParatextVersificationInfo_English_GetsInfoFromParatextResourcesOrLocalEngVrsFile()
		{
			var stats = new ParatextVersificationInfo(ScrVers.English);
			Assert.AreEqual(66, stats.BookCount);
			Assert.AreEqual("Rev", stats.GetBookCode(65));
			Assert.AreEqual("Revelation", stats.GetBookName(65));
			Assert.AreEqual(10, stats.GetChaptersInBook(stats.GetBookNumber("Esther")));
			Assert.AreEqual(12, stats.GetChaptersInBook(stats.GetBookNumber("Daniel")));
		}

		[Test]
		public void ParatextVersificationInfo_Septuagint_GetsInfoFromVulVrs()
		{
			var tempVrsFile = Path.GetTempFileName();
			File.WriteAllText(tempVrsFile, Resources.SeptuagintVersification);
			try
			{
				var vers = Versification.Table.Implementation.Load(tempVrsFile, "customLxx");
				var stats = new ParatextVersificationInfo(vers);
				Assert.AreEqual(66, stats.BookCount);
				Assert.AreEqual("Mat", stats.GetBookCode(39));
				Assert.AreEqual("Revelation", stats.GetBookName(65));
				Assert.AreEqual(1, stats.GetChaptersInBook(stats.GetBookNumber("Esther")));
				Assert.AreEqual(12, stats.GetChaptersInBook(stats.GetBookNumber("Daniel")));
			}
			finally
			{
				File.Delete(tempVrsFile);
			}
		}

		[Test]
		public void ParatextVersificationInfo_Vulgate_GetsInfoFromLxxVrs()
		{
			var tempVrsFile = Path.GetTempFileName();
			File.WriteAllText(tempVrsFile, Resources.VulgateVersification);
			try
			{
				var vers = Versification.Table.Implementation.Load(tempVrsFile, "customVulgate");
				var stats = new ParatextVersificationInfo(vers);
				Assert.AreEqual(66, stats.BookCount);
				Assert.AreEqual("Mat", stats.GetBookCode(39));
				Assert.AreEqual("Revelation", stats.GetBookName(65));
				Assert.AreEqual(10, stats.GetChaptersInBook(stats.GetBookNumber("Esther")));
				Assert.AreEqual(14, stats.GetChaptersInBook(stats.GetBookNumber("Daniel")));
			}
			finally
			{
				File.Delete(tempVrsFile);
			}
		}
	}
}
