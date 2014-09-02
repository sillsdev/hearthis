using HearThis.Script;
using NUnit.Framework;
using Paratext;

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
			Assert.AreEqual(100, stats.GetVersesInChapter(stats.GetBookNumber("Daniel"), 3));
			Assert.AreEqual(10, stats.GetChaptersInBook(stats.GetBookNumber("Esther")));
		}
	}

	[TestFixture]
	public sealed class ParatextVersificationInfoTests
	{
		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			ScrTextCollection.Initialize();
		}


		[Test]
		[Category("SkipOnTeamCity")]
		public void ParatextVersificationInfo_English_GetsInfoFromEngVrs()
		{
			var stats = new ParatextVersificationInfo(ScrVers.English);
			Assert.AreEqual(66, stats.BookCount);
			Assert.AreEqual("Rev", stats.GetBookCode(65));
			Assert.AreEqual("Revelation", stats.GetBookName(65));
			Assert.AreEqual(10, stats.GetChaptersInBook(stats.GetBookNumber("Esther")));
			Assert.AreEqual(12, stats.GetChaptersInBook(stats.GetBookNumber("Daniel")));
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void ParatextVersificationInfo_Vulgate_GetsInfoFromVulVrs()
		{
			var stats = new ParatextVersificationInfo(ScrVers.Vulgate);
			Assert.AreEqual(66, stats.BookCount);
			Assert.AreEqual("Mat", stats.GetBookCode(39));
			Assert.AreEqual("Revelation", stats.GetBookName(65));
			Assert.AreEqual(10, stats.GetChaptersInBook(stats.GetBookNumber("Esther")));
			Assert.AreEqual(14, stats.GetChaptersInBook(stats.GetBookNumber("Daniel")));
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void ParatextVersificationInfo_Septuagint_GetsInfoFromLxxVrs()
		{
			var stats = new ParatextVersificationInfo(ScrVers.Septuagint);
			Assert.AreEqual(66, stats.BookCount);
			Assert.AreEqual("Exo", stats.GetBookCode(1));
			Assert.AreEqual("Revelation", stats.GetBookName(65));
			Assert.AreEqual(10, stats.GetChaptersInBook(stats.GetBookNumber("Esther")));
			Assert.AreEqual(12, stats.GetChaptersInBook(stats.GetBookNumber("Daniel")));
		}
	}
}
