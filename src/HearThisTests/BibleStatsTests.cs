using System.IO;
using HearThis.Script;
using HearThisTests.Properties;
using NUnit.Framework;
using SIL.Scripture;
using static HearThis.Script.BibleStatsBase;

namespace HearThisTests
{
	[TestFixture]
	public sealed class BibleStatsTests
	{
		[Test]
		public void BibleStats_GetsInfoAsShippedWithHearThis()
		{
			var stats = new BibleStats();
			Assert.That(stats.BookCount, Is.EqualTo(66));
			Assert.That(stats.GetBookCode(65), Is.EqualTo("Rev"));
			Assert.That(stats.GetBookName(65), Is.EqualTo("Revelation"));
			Assert.That(stats.GetVersesInChapter(stats.GetBookNumber("Daniel"), 3), Is.EqualTo(30));
			Assert.That(stats.GetChaptersInBook(stats.GetBookNumber("Esther")), Is.EqualTo(10));
		}
	}

	[TestFixture]
	public sealed class ParatextVersificationInfoTests
	{
		[Test]
		public void ParatextVersificationInfo_English_GetsInfoFromParatextResourcesOrLocalEngVrsFile()
		{
			var stats = new ParatextVersificationInfo(ScrVers.English);
			Assert.That(stats.BookCount, Is.EqualTo(66));
			Assert.That(stats.GetBookCode(65), Is.EqualTo("Rev"));
			Assert.That(stats.GetBookName(65), Is.EqualTo("Revelation"));
			Assert.That(stats.GetChaptersInBook(stats.GetBookNumber("Esther")), Is.EqualTo(10));
			Assert.That(stats.GetChaptersInBook(stats.GetBookNumber("Daniel")), Is.EqualTo(12));
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
				Assert.That(stats.BookCount, Is.EqualTo(kCanonicalBookCount));
				Assert.That(stats.GetBookCode(kCountOfOTBooks), Is.EqualTo("Mat"));
				Assert.That(stats.GetBookName(kCanonicalBookCount - 1), Is.EqualTo("Revelation"));
				Assert.That(stats.GetChaptersInBook(stats.GetBookNumber("Esther")), Is.EqualTo(1));
				Assert.That(stats.GetChaptersInBook(stats.GetBookNumber("Daniel")), Is.EqualTo(12));
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
				Assert.That(stats.BookCount, Is.EqualTo(66));
				Assert.That(stats.GetBookCode(39), Is.EqualTo("Mat"));
				Assert.That(stats.GetBookName(65), Is.EqualTo("Revelation"));
				Assert.That(stats.GetChaptersInBook(stats.GetBookNumber("Esther")), Is.EqualTo(10));
				Assert.That(stats.GetChaptersInBook(stats.GetBookNumber("Daniel")), Is.EqualTo(14));
			}
			finally
			{
				File.Delete(tempVrsFile);
			}
		}
	}
}
