using System.Collections.Generic;
using System.Linq;
using HearThis.Script;
using NUnit.Framework;
using SIL.Scripture;

namespace HearThisTests.Script
{
	[TestFixture]
	public class RangesToBreakByVerseTests
	{
		[Test]
		public void AddRange_ScriptureRangeIsNull_InitializesAndAddsRange()
		{
			var ranges = new RangesToBreakByVerse();
			var start = new BCVRef(1, 1, 1);
			var end = new BCVRef(1, 50, 26);
			ranges.AddRange(start, end);
			Assert.That(ranges.ScriptureRanges.Single().Start, Is.EqualTo(start.BBCCCVVV));
			Assert.That(ranges.ScriptureRanges.Single().End, Is.EqualTo(end.BBCCCVVV));
		}

		[Test]
		public void AddRange_ScriptureRangeEmpty_AddsRange()
		{
			var ranges = new RangesToBreakByVerse();
			var start = new BCVRef(1, 1, 1);
			var end = new BCVRef(1, 50, 26);
			ranges.ScriptureRanges = new List<ScriptureRange>();
			ranges.AddRange(start, end);
			Assert.That(ranges.ScriptureRanges.Single().Start, Is.EqualTo(start.BBCCCVVV));
			Assert.That(ranges.ScriptureRanges.Single().End, Is.EqualTo(end.BBCCCVVV));
		}

		[TestCase(001001001, 001001001)]
		[TestCase(001001001, 001004001)]
		[TestCase(001001001, 002004001)]
		[TestCase(001001001, 003004001)]
		[TestCase(001001001, 003004010)]
		public void AddRange_ContainsPrecedingRange_AddsRange(int precedingRangeStart, int precedingRangeEnd)
		{
			var ranges = new RangesToBreakByVerse();
			ranges.AddRange(precedingRangeStart, precedingRangeEnd);
			var start = new BCVRef(3, 4, 11);
			var end = new BCVRef(3, 4, 12);
			ranges.AddRange(start, end);
			Assert.That(ranges.ScriptureRanges.Count, Is.EqualTo(2));
			Assert.That(ranges.ScriptureRanges[1].Start, Is.EqualTo(start.BBCCCVVV));
			Assert.That(ranges.ScriptureRanges[1].End, Is.EqualTo(end.BBCCCVVV));
		}

		[TestCase(002003013, 002003014)]
		[TestCase(002003015, 002004015)]
		[TestCase(002004001, 002004006)]
		[TestCase(003003013, 003004001)]
		public void AddRange_ContainsFollowingRange_InsertsRange(int followingRangeStart, int followingRangeEnd)
		{
			var ranges = new RangesToBreakByVerse();
			ranges.AddRange(followingRangeStart, followingRangeEnd);
			var start = new BCVRef(1, 1, 1);
			var end = new BCVRef(2, 3, 12);
			ranges.AddRange(start, end);
			Assert.That(ranges.ScriptureRanges.Count, Is.EqualTo(2));
			Assert.That(ranges.ScriptureRanges[0].Start, Is.EqualTo(start.BBCCCVVV));
			Assert.That(ranges.ScriptureRanges[0].End, Is.EqualTo(end.BBCCCVVV));
		}

		[TestCase(002003003, 002003014)]
		[TestCase(002003001, 002003014)]
		[TestCase(002003003, 002003016)]
		[TestCase(002002001, 002003014)]
		[TestCase(002003003, 003004014)]
		[TestCase(001003003, 005001001)]
		public void AddRange_ContainsSingleSupersetRange_NoChange(int supersetRangeStart, int supersetRangeEnd)
		{
			var ranges = new RangesToBreakByVerse();
			ranges.AddRange(supersetRangeStart, supersetRangeEnd);
			ranges.AddRange(005002001, 006002006);
			var start = new BCVRef(002003003);
			var end = new BCVRef(002003014);
			ranges.AddRange(start, end);
			Assert.That(ranges.ScriptureRanges.Count, Is.EqualTo(2));
			Assert.That(ranges.ScriptureRanges[0].Start, Is.EqualTo(supersetRangeStart));
			Assert.That(ranges.ScriptureRanges[0].End, Is.EqualTo(supersetRangeEnd));
			Assert.That(ranges.ScriptureRanges[1].Start, Is.EqualTo(005002001));
			Assert.That(ranges.ScriptureRanges[1].End, Is.EqualTo(006002006));
		}

		[TestCase(002003003, 002003013)]
		[TestCase(002003003, 002003014)]
		[TestCase(002003001, 002003014)]
		[TestCase(002003003, 002003016)]
		[TestCase(001050003, 005001001)]
		[TestCase(001030002, 005001001)]
		public void AddRange_ContainsSingleRangeThatOverlapsAtStart_OverlappedRangeStartChanged(int supersetRangeStart, int supersetRangeEnd)
		{
			const int newStart = 001030001; // GEN 30:1
			const int newEnd = 002003013; // EXO 3:13
			Assert.That(newStart, Is.LessThanOrEqualTo(supersetRangeStart), "SETUP problem");
			Assert.That(newEnd, Is.GreaterThanOrEqualTo(supersetRangeStart), "SETUP problem");
			Assert.That(newEnd, Is.LessThanOrEqualTo(supersetRangeEnd), "SETUP problem");

			var ranges = new RangesToBreakByVerse();
			ranges.AddRange(supersetRangeStart, supersetRangeEnd);
			ranges.AddRange(005002001, 006002006);
			ranges.AddRange(newStart, newEnd);
			Assert.That(ranges.ScriptureRanges.Count, Is.EqualTo(2));
			Assert.That(ranges.ScriptureRanges[0].Start, Is.EqualTo(newStart));
			Assert.That(ranges.ScriptureRanges[0].End, Is.EqualTo(supersetRangeEnd));
			Assert.That(ranges.ScriptureRanges[1].Start, Is.EqualTo(005002001));
			Assert.That(ranges.ScriptureRanges[1].End, Is.EqualTo(006002006));
		}

		[TestCase(002020001, 003003011)]
		[TestCase(002020002, 002020003)]
		[TestCase(002020002, 002021002)]
		[TestCase(002010001, 003002011)]
		[TestCase(001020001, 003003010)]
		[TestCase(001020002, 003003011)]
		public void AddRange_ContainsSingleRangeThatOverlapsAtEnd_OverlappedRangeEndChanged(int supersetRangeStart, int supersetRangeEnd)
		{
			const int newStart = 002020002; // EXO 20:2
			const int newEnd = 003003011; // LEV 3:11
			Assert.That(newStart, Is.GreaterThanOrEqualTo(supersetRangeStart), "SETUP problem");
			Assert.That(newStart, Is.LessThanOrEqualTo(supersetRangeEnd), "SETUP problem");
			Assert.That(newEnd, Is.GreaterThanOrEqualTo(supersetRangeEnd), "SETUP problem");

			var ranges = new RangesToBreakByVerse();
			ranges.AddRange(001001001, 001001004);
			ranges.AddRange(supersetRangeStart, supersetRangeEnd);
			ranges.AddRange(005002001, 006002006);
			ranges.AddRange(newStart, newEnd);
			Assert.That(ranges.ScriptureRanges.Count, Is.EqualTo(3));
			Assert.That(ranges.ScriptureRanges[0].Start, Is.EqualTo(001001001));
			Assert.That(ranges.ScriptureRanges[0].End, Is.EqualTo(001001004));
			Assert.That(ranges.ScriptureRanges[1].Start, Is.EqualTo(supersetRangeStart));
			Assert.That(ranges.ScriptureRanges[1].End, Is.EqualTo(newEnd));
			Assert.That(ranges.ScriptureRanges[2].Start, Is.EqualTo(005002001));
			Assert.That(ranges.ScriptureRanges[2].End, Is.EqualTo(006002006));
		}

		[TestCase(002020003, 003003010)]
		[TestCase(002021020, 003002012)]
		public void AddRange_ContainsSingleRangeThatIsSubset_Expanded(int subsetRangeStart, int subsetRangeEnd)
		{
			const int newStart = 002020002; // EXO 20:2
			const int newEnd = 003003011; // LEV 3:11
			Assert.That(newStart, Is.LessThan(subsetRangeStart), "SETUP problem");
			Assert.That(newEnd, Is.GreaterThan(subsetRangeEnd), "SETUP problem");

			var ranges = new RangesToBreakByVerse();
			ranges.AddRange(001001001, 001001004);
			ranges.AddRange(subsetRangeStart, subsetRangeEnd);
			ranges.AddRange(005002001, 006002006);
			ranges.AddRange(newStart, newEnd);
			Assert.That(ranges.ScriptureRanges.Count, Is.EqualTo(3));
			Assert.That(ranges.ScriptureRanges[0].Start, Is.EqualTo(001001001));
			Assert.That(ranges.ScriptureRanges[0].End, Is.EqualTo(001001004));
			Assert.That(ranges.ScriptureRanges[1].Start, Is.EqualTo(newStart));
			Assert.That(ranges.ScriptureRanges[1].End, Is.EqualTo(newEnd));
			Assert.That(ranges.ScriptureRanges[2].Start, Is.EqualTo(005002001));
			Assert.That(ranges.ScriptureRanges[2].End, Is.EqualTo(006002006));
		}
		
		[TestCase(1, 0, 002020002, 003003011, 002020002, 002020003, 003003010, 003003011)]
		[TestCase(1, 0, 002020002, 003003011, 002020003, 002020003, 003003010, 003003010)]
		[TestCase(2, 0, 002020001, 003003011, 002020001, 002020003, 003003010, 003003010, 003003020, 004001001)]
		[TestCase(2, 1, 002020002, 003003016, 001001001, 001002003, 002020003, 002020003, 003003010, 003003016)]
		[TestCase(3, 1, 002020002, 003003011, 001001001, 002002008, 002020003, 002020003, 003003010, 003003010, 003003020, 004001001)]
		[TestCase(3, 1, 002020002, 003003011, 001001001, 001050003, 002020003, 002040003, 003001010, 003003001, 003007001, 004001001)]
		[TestCase(3, 2, 002020002, 003003011, 001001001, 001030003, 001041001, 001049003, 002030003, 002034003, 002035001, 002035012, 003001010, 003003001)]
		public void AddRange_ContainsMultipleSubsetRanges_CoveredRangesCoalesced(int expectedCount,
			int expectedIndex, int expectedStart, int expectedEnd, params int [] existingRanges)
		{ 
			Assert.That(existingRanges.Length, Is.GreaterThanOrEqualTo(2),
				"SETUP problem: must at least one (start/end) pair of existing values");
			Assert.That(existingRanges.Length % 2, Is.EqualTo(0),
				"SETUP problem: must have even number of existing values (start/end pairs)");

			const int newStart = 002020002; // EXO 20:2
			const int newEnd = 003003011; // LEV 3:11
			var indexOfFirstExistingStartRefInOverlap = expectedIndex * 2;
			var indexOfFirstSurvivingRange = existingRanges.Length - (expectedCount - expectedIndex + 1) * 2;
			Assert.That(newStart, Is.LessThanOrEqualTo(existingRanges[indexOfFirstExistingStartRefInOverlap + 1]), "SETUP problem");

			var ranges = new RangesToBreakByVerse();
			for (int i = 0; i < existingRanges.Length; i += 2)
			{
				var existingStart = existingRanges[i];
				Assert.That(existingStart, Is.GreaterThan(ranges.ScriptureRanges?.LastOrDefault()?.End ?? 0),
					"SETUP problem: existing start/end pairs must be supplied in ascending order");
				var existingEnd = existingRanges[i + 1];
				Assert.That(existingStart, Is.LessThanOrEqualTo(existingEnd), "SETUP problem: invalid start/end pair");
				ranges.AddRange(existingStart, existingEnd);
			}

			ranges.AddRange(newStart, newEnd);

			Assert.That(ranges.ScriptureRanges.Count, Is.EqualTo(expectedCount));
			for (int i = 0; i < expectedIndex; i++)
			{
				Assert.That(ranges.ScriptureRanges[i].Start, Is.EqualTo(existingRanges[i * 2]));
				Assert.That(ranges.ScriptureRanges[i].End, Is.EqualTo(existingRanges[i * 2 + 1]));
			}

			Assert.That(ranges.ScriptureRanges[expectedIndex].Start, Is.EqualTo(expectedStart));
			Assert.That(ranges.ScriptureRanges[expectedIndex].End, Is.EqualTo(expectedEnd));

			int iExisting = existingRanges.Length - 2;
			for (int i = expectedCount - 1; i > expectedIndex; i--)
			{
				Assert.That(ranges.ScriptureRanges[i].Start, Is.EqualTo(existingRanges[iExisting]));
				Assert.That(ranges.ScriptureRanges[i].End, Is.EqualTo(existingRanges[iExisting + 1]));
				iExisting -= 2;
			}
		}
	}
}
