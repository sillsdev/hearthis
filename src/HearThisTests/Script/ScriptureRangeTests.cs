using HearThis.Script;
using NUnit.Framework;
using Paratext.Data;
using SIL.Scripture;
using System.Collections.Generic;
using System.Linq;

namespace HearThisTests.Script
{
	internal class ScriptureRangeTests
	{
		[Test]
		public void ConstructorWithStartAndEnd_StartIsLessThanEnd_SetsRefsAppropriately()
		{
			var bcvStart = new BCVRef(001002003);
			var bcvEnd = new BCVRef(002003004);
			var r = new ScriptureRange(bcvStart, bcvEnd);
			Assert.That(r.StartRef, Is.EqualTo(bcvStart));
			Assert.That(r.EndRef, Is.EqualTo(bcvEnd));
			Assert.That(r.Start, Is.EqualTo(bcvStart.BBCCCVVV));
			Assert.That(r.End, Is.EqualTo(bcvEnd.BBCCCVVV));
		}

		[Test]
		public void ConstructorWithStartAndEnd_StartIsGreaterThanEnd_ThrowsArgumentException()
		{
			// ReSharper disable once ObjectCreationAsStatement
			Assert.That(() => { new ScriptureRange(23002006, 23002004); }, Throws.ArgumentException);
		}

		[Test]
		public void SetStart_StartIsLessThanEnd_SetsRefsAppropriately()
		{
			var bcvStart = new BCVRef(001002003);
			var bcvEnd = new BCVRef(002003004);
			var r = new ScriptureRange();
			r.End = bcvEnd.BBCCCVVV;
			r.Start = bcvStart.BBCCCVVV;
			Assert.That(r.StartRef, Is.EqualTo(bcvStart));
			Assert.That(r.EndRef, Is.EqualTo(bcvEnd));
		}

		[Test]
		public void SetStart_StartIsGreaterThanEnd_ThrowsArgumentException()
		{
			var r = new ScriptureRange();
			r.End = 23002004;
			Assert.That(() => { r.Start = 23002006; }, Throws.ArgumentException);
		}

		[TestCase(001001001, 001001001, 001001002, 001001002)]
		[TestCase(001001001, 001001001, 002002001, 002002001)]
		[TestCase(001001001, 001001001, 001001002, 066022021)]
		[TestCase(001001001, 001001001, 066022021, 066022021)]
		[TestCase(002020002, 003003011, 002020003, 002020003)]
		[TestCase(002020002, 003003011, 002020003, 003003010)]
		[TestCase(002020002, 003003011, 002020003, 003003011)]
		[TestCase(066022020, 066022020, 066022021, 066022021)]
		[TestCase(001001001, 066022020, 066022021, 066022021)]
		public void ComparisonOperators_NotEqual_ReturnsExpectedResult(int bcvLessStart, int bcvLessEnd, int bcvGreaterStart, int bcvGreaterEnd)
		{
			var less = new ScriptureRange(new BCVRef(bcvLessStart), new BCVRef(bcvLessEnd));
			var greater = new ScriptureRange(new BCVRef(bcvGreaterStart), new BCVRef(bcvGreaterEnd));
				
			Assert.That(less < greater, Is.True);
			Assert.That(less <= greater, Is.True);
			Assert.That(less > greater, Is.False);
			Assert.That(less >= greater, Is.False);
			Assert.That(less, Is.Not.EqualTo(greater));
			Assert.That(less.CompareTo(greater), Is.LessThan(0));
			Assert.That(greater.CompareTo(less), Is.GreaterThan(0));
			Assert.That(less.CompareTo((object)greater), Is.LessThan(0));
			Assert.That(greater.CompareTo((object)less), Is.GreaterThan(0));
		}

		[TestCase(001001001, 001001001)]
		[TestCase(002020002, 003003011)]
		[TestCase(002020002, 003003011)]
		[TestCase(066022020, 066022020)]
		[TestCase(001001001, 066022020)]
		public void ComparisonOperators_Equal_ReturnsExpectedResult(int bcvStart, int bcvEnd)
		{
			var left = new ScriptureRange(new BCVRef(bcvStart), new BCVRef(bcvEnd));
			var right = new ScriptureRange(new BCVRef(bcvStart), new BCVRef(bcvEnd));
				
			Assert.That(left < right, Is.False);
			Assert.That(left <= right, Is.True);
			Assert.That(left > right, Is.False);
			Assert.That(left >= right, Is.True);
			Assert.That(left.Equals(right), Is.True);
			Assert.That(left.Equals((object)right), Is.True);
		}

		[TestCase(001001001)]
		[TestCase(001001002)]
		[TestCase(066022019)]
		[TestCase(066022020)]
		public void Includes_RangeDoesIncludeRef_ReturnsTrue(int bcvRef)
		{
			var rng = new ScriptureRange(new BCVRef(001001001), new BCVRef(066022020));
			Assert.That(rng.Includes(new BCVRef(bcvRef)));
		}

		[TestCase(001001001)]
		[TestCase(002002004)]
		[TestCase(040005011)]
		[TestCase(066022020)]
		public void Includes_RangeExcludesRef_ReturnsFalse(int bcvRef)
		{
			var rng = new ScriptureRange(new BCVRef(002002005), new BCVRef(040005010));
			Assert.That(rng.Includes(new BCVRef(bcvRef)), Is.False);
		}

		[Test]
		public void GetNewRangesToBreakByVerse_NoExistingRanges_ReturnsNewRange()
		{
			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = GetTestBookTokens("GEN").ToList();
				InitializeVerseIterationHelper(stub);

				var newStartBcv = new BCVRef(1, 1, 4);
				var newEndBcv = new BCVRef(1, 1, 6);

				var result = new ScriptureRange(newStartBcv, newEndBcv)
					.GetNewRangesToBreakByVerse(new List<ScriptureRange>()).Single();

				Assert.That(result.StartRef, Is.EqualTo(newStartBcv));
				Assert.That(result.EndRef, Is.EqualTo(newEndBcv));
			}
		}

		[TestCase(1001004, 1001006, 1001001, 1001009)]
		[TestCase(002003003, 002003014, 002003003, 002003014)]
		[TestCase(002003003, 002003014, /* extra one before: */ 002001002, 002002004, 002003001, 002003014)]
		[TestCase(002003003, 002003014, 002003003, 002003016)]
		[TestCase(002003003, 002003014, 002002001, 002003014)]
		[TestCase(002003003, 002003014, 002003003, 003004014)]
		[TestCase(002003003, 002003014, 001003003, 005001001, /* extra one after: */ 006001002, 006001004)]
		[TestCase(002020002 /* EXO 20:2 */, 003003011 /* LEV 3:11 */, 002020001, 003003011)]
		[TestCase(002020002 /* EXO 20:2 */, 003003011 /* LEV 3:11 */, 001020002, 003003011)]
		public void GetNewRangesToBreakByVerse_NewRangeCompletelyIncludedInExistingRange_ReturnsEmptyEnumeration(int newStartBcv, int newEndBcv, params int [] existingRanges)
		{
			var existingList = CreateExistingRanges(existingRanges);

			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = GetTestBookTokens("GEN").ToList();
				InitializeVerseIterationHelper(stub);

				var result = new ScriptureRange(new BCVRef(newStartBcv), new BCVRef(newEndBcv))
					.GetNewRangesToBreakByVerse(existingList);

				Assert.That(result, Is.Empty);
			}
		}

		[TestCase(001001018, 001002007, 001002005, 001002009)]
		[TestCase(001001018, 001002007, 001002005, 001002009, /* extra one after: */ 006001002, 006001004)]
		[TestCase(001030001 /* GEN 30:1 */, 002003013 /* EXO 3:13 */, 002003003, 002003013)]
		[TestCase(001030001 /* GEN 30:1 */, 002003013 /* EXO 3:13 */, 002003003, 002003014)]
		[TestCase(001030001 /* GEN 30:1 */, 002003013 /* EXO 3:13 */, 002003001, 002003014)]
		[TestCase(001030001 /* GEN 30:1 */, 002003013 /* EXO 3:13 */, 002003003, 002003016)]
		[TestCase(001030001 /* GEN 30:1 */, 002003013 /* EXO 3:13 */, 001050003, 005001001)]
		[TestCase(001030001 /* GEN 30:1 */, 002003013 /* EXO 3:13 */, 001030002, 005001001)]
		public void GetNewRangesToBreakByVerse_NewRangeOverlapsStartOfAnExistingRange_ReturnsSingleRangeCoveringNewVerses(
			int newStartBcv, int newEndBcv, params int [] existingRanges)
		{
			var existingList = CreateExistingRanges(existingRanges);

			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = GetTestBookTokens("GEN").Union(GetTestBookTokens("EXO", true)).ToList();
				InitializeVerseIterationHelper(stub);
				
				var newStartRef = new BCVRef(newStartBcv);

				var result = new ScriptureRange(newStartRef, new BCVRef(newEndBcv))
					.GetNewRangesToBreakByVerse(existingList).Single();

				Assert.That(result.StartRef, Is.EqualTo(newStartRef));
				var vEnd = new VerseRef(result.End, stub.Versification);
				Assert.That(vEnd.NextVerse(new BookSet(stub.BooksPresent)), Is.True);
				Assert.That(existingList.Count(r => r.Start == vEnd.BBBCCCVVV), Is.EqualTo(1),
					$"Expected exactly one existing range to start at {vEnd}");
			}
		}

		[TestCase(1002007, 1002016, 1002005, 1002009)]
		[TestCase(002020002 /* EXO 20:2 */, 003003011 /* LEV 3:11 */, 002020002, 002020003)]
		[TestCase(002020002 /* EXO 20:2 */, 003003011 /* LEV 3:11 */, 002020002, 002021002)]
		[TestCase(002020002 /* EXO 20:2 */, 003003011 /* LEV 3:11 */, 002010001, 003002011)]
		[TestCase(002020002 /* EXO 20:2 */, 003003011 /* LEV 3:11 */, 001020001, 003003010)]
		public void GetNewRangesToBreakByVerse_NewRangeOverlapsEndOfAnExistingRange_ReturnsSingleRangeCoveringNewVerses(
			int newStartBcv, int newEndBcv, params int[] existingRanges)
		{
			var existingList = CreateExistingRanges(existingRanges);

			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = GetTestBookTokens("GEN").Union(GetTestBookTokens("EXO", true))
					.Union(GetTestBookTokens("LEV")).ToList();
				InitializeVerseIterationHelper(stub);

				var newEndRef = new BCVRef(newEndBcv);

				var result = new ScriptureRange(new BCVRef(newStartBcv), newEndRef)
					.GetNewRangesToBreakByVerse(existingList).Single();

				var vStart = new VerseRef(result.Start, stub.Versification);
				Assert.That(vStart.PreviousVerse(new BookSet(stub.BooksPresent)), Is.True);
				Assert.That(existingList.Count(r => r.End == vStart.BBBCCCVVV), Is.EqualTo(1),
					$"Expected exactly one existing range to end at {vStart}");
				Assert.That(result.EndRef, Is.EqualTo(newEndRef));
			}
		}

		[TestCase(1002005, 1002009)]
		public void GetNewRangesToBreakByVerse_NewRangeStartsBeforeAndEndsAfterExistingRange_ReturnsTwoRangesCoveringNewVerses(params int [] existingRanges)
		{
			var existingList = CreateExistingRanges(existingRanges);

			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = GetTestBookTokens("GEN").ToList();
				InitializeVerseIterationHelper(stub);

				var newStartBcv = new BCVRef(1, 1, 18);
				var newEndBcv = new BCVRef(1, 2, 16);

				var result = new ScriptureRange(newStartBcv, newEndBcv)
					.GetNewRangesToBreakByVerse(existingList).ToList();

				Assert.That(result[0].StartRef, Is.EqualTo(newStartBcv));
				Assert.That(result[0].End, Is.EqualTo(1002004));
				Assert.That(result[1].Start, Is.EqualTo(1002010));
				Assert.That(result[1].EndRef, Is.EqualTo(newEndBcv));
			}
		}

		[TestCase(1002010, 1002018, /* Range before: */ 1002005, 1002009, /* Range after: */ 1020001, 1020012)]
		[TestCase(1003010, 1003018, /* Range before: */ 1002005, 1002009, /* Range after: */ 1020001, 1020012)]
		[TestCase(1019010, 1019038, /* Range before: */ 1002005, 1002009, /* Range after: */ 1020001, 1020012)]
		[TestCase(3004011, 3004012, /* Range before: */ 001001001, 001001001)]
		[TestCase(3004011, 3004012, /* Range before: */ 001001001, 001004001)]
		[TestCase(3004011, 3004012, /* Range before: */ 001001001, 002004001)]
		[TestCase(3004011, 3004012, /* Range before: */ 001001001, 003004001)]
		[TestCase(3004011, 3004012, /* Range before: */ 001001001, 003004010)]
		[TestCase(1001001, 2003012, /* Range after: */ 002003013, 002003014)]
		[TestCase(1001001, 2003012, /* Range after: */ 002003015, 002004015)]
		[TestCase(1001001, 2003012, /* Range after: */ 002004001, 002004006)]
		[TestCase(1001001, 2003012, /* Range after: */ 003003013, 003004001)]
		public void GetNewRangesToBreakByVerse_NewRangeDoesNotOverlapAnyExistingRange_ReturnsNewRange(int newStartBcv, int newEndBcv, params int [] existingRanges)
		{
			var existingList = CreateExistingRanges(existingRanges);

			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = GetTestBookTokens("GEN").ToList();
				InitializeVerseIterationHelper(stub);

				var newStartBcvRef = new BCVRef(newStartBcv);
				var newEndBcvRef = new BCVRef(newEndBcv);

				var result = new ScriptureRange(newStartBcvRef, newEndBcvRef)
					.GetNewRangesToBreakByVerse(existingList).Single();

				Assert.That(result.StartRef, Is.EqualTo(newStartBcvRef));
				Assert.That(result.EndRef, Is.EqualTo(newEndBcvRef));
			}
		}

		[TestCase(2, 002020002, 003003011, 0, 0, 002020003, 003003010)]
		[TestCase(2, 002020002, 003003011, 0, 0, 002021020, 003002012)]
		[TestCase(1, 002020004, 003003009, 0, 1, 002020002, 002020003, 003003010, 003003011)]
		[TestCase(3, 002020002, 003003011, 0, 1, 002020003, 002020003, 003003010, 003003010)]
		[TestCase(2, 002020004, 003003011, 0, 1, 002020001, 002020003, 003003010, 003003010, 003003020, 004001001)]
		[TestCase(2, 002020002, 003003009, 1, 2, 001001001, 001002003, 002020003, 002020003, 003003010, 003003016)]
		[TestCase(3, 002020002, 003003011, 1, 2, 001001001, 002002008, 002020003, 002020003, 003003010, 003003010, 003003020, 004001001)]
		[TestCase(3, 002020002, 003003011, 1, 2, 001001001, 001050003, 002020003, 002040003, 003001010, 003003001, 003007001, 004001001)]
		[TestCase(4, 002020002, 003003011, 2, 4, 001001001, 001030003, 001041001, 001049003, 002030003, 002034003, 002035001, 002035012, 003001010, 003003001)]
		public void GetNewRangesToBreakByVerse_ContainsSubsetRanges_ReturnsRangesForPortionsNotPreviouslyCovered(
			int expectedCount, int expectedStartOfFirstResult, int expectedEndOfLastResult,
			int indexOfFirstExistingCoveredRange, int indexOfLastExistingCoveredRange,
			params int[] existingRanges)
		{
			Assert.That(expectedCount, Is.GreaterThanOrEqualTo(
				indexOfLastExistingCoveredRange - indexOfFirstExistingCoveredRange),
				"Test Case expected results are incorrect");

			const int newStart = 002020002; // EXO 20:2
			const int newEnd = 003003011; // LEV 3:11

			var existingList = CreateExistingRanges(existingRanges);

			using (var stub = new ScriptureStub())
			{
				stub.UsfmTokens = GetTestBookTokens("GEN").Union(GetTestBookTokens("EXO", true))
					.Union(GetTestBookTokens("LEV")).ToList();
				InitializeVerseIterationHelper(stub);

				var newEndRef = new BCVRef(newEnd);

				var result = new ScriptureRange(new BCVRef(newStart), newEndRef)
					.GetNewRangesToBreakByVerse(existingList).ToList();

				Assert.That(result.Count, Is.EqualTo(expectedCount),
					"Unexpected number of new ranges");
				Assert.That(result.First().Start, Is.EqualTo(expectedStartOfFirstResult));
				Assert.That(result.Last().End, Is.EqualTo(expectedEndOfLastResult));

				if (expectedStartOfFirstResult == newStart)
				{
					var vEnd = new VerseRef(result[0].End, stub.Versification);
					Assert.That(vEnd.NextVerse(), Is.True);
					Assert.That(vEnd.BBBCCCVVV,
						Is.EqualTo(existingList[indexOfFirstExistingCoveredRange].Start));
				}

				for (int i = indexOfFirstExistingCoveredRange; i < indexOfLastExistingCoveredRange; i++)
				{
					var vStart = new VerseRef(existingList[i].End, stub.Versification);
					Assert.That(vStart.NextVerse(new BookSet(stub.BooksPresent)), Is.True);
					var vEnd = new VerseRef(existingList[i+1].Start, stub.Versification);
					Assert.That(vEnd.PreviousVerse(new BookSet(stub.BooksPresent)), Is.True);
					Assert.That(result.Single(r => r.Start == vStart.BBBCCCVVV).End,
						Is.EqualTo(vEnd.BBBCCCVVV),
						$"Expected exactly one result to be from {vStart} to {vEnd}");
				}
				
				if (expectedEndOfLastResult == newEnd)
				{
					var vStart = new VerseRef(result.Last().Start, stub.Versification);
					Assert.That(vStart.PreviousVerse(), Is.True);
					Assert.That(vStart.BBBCCCVVV,
						Is.EqualTo(existingList[indexOfLastExistingCoveredRange].End));
				}
			}
		}

		private IEnumerable<UsfmToken> GetTestBookTokens(string bookId, bool withIntro = false)
		{
			yield return new UsfmToken(UsfmTokenType.Book, "id", null, null, bookId);
			if (withIntro)
			{
				yield return new UsfmToken(UsfmTokenType.Paragraph, "ip", null, null);
				yield return new UsfmToken(UsfmTokenType.Text, null, $"This is the introduction to {bookId}", null);
			}

			yield return new UsfmToken(UsfmTokenType.Chapter, "c", null, null, "1");
			yield return new UsfmToken(UsfmTokenType.Paragraph, "p", null, null);
			yield return new UsfmToken(UsfmTokenType.Verse, "v", null, null, "1");
			yield return new UsfmToken(UsfmTokenType.Text, null, "First verse", null);
		}

		private static void InitializeVerseIterationHelper(ScriptureStub stub)
		{
			// The ParatextScriptProvider constructor actually does this assignment as well,
			// but this helps make it clear why we need to construct one.
			ScriptureRange.VerseIterationHelper = new ParatextScriptProvider(stub);
		}

		private List<ScriptureRange> CreateExistingRanges(int [] existingRanges)
		{
			Assert.That(existingRanges.Length, Is.GreaterThanOrEqualTo(2),
				"SETUP problem: must have at least one (start/end) pair of existing values");
			Assert.That(existingRanges.Length % 2, Is.EqualTo(0),
				"SETUP problem: must have even number of existing values (start/end pairs)");

			var existing = new List<ScriptureRange>();

			for (int i = 0; i < existingRanges.Length; i += 2)
			{
				var existingStart = existingRanges[i];
				Assert.That(existingStart, Is.GreaterThan(existing.LastOrDefault()?.End ?? 0),
					"SETUP problem: existing start/end pairs must be supplied in ascending order");
				var existingEnd = existingRanges[i + 1];
				Assert.That(existingStart, Is.LessThanOrEqualTo(existingEnd), "SETUP problem: invalid start/end pair");
				existing.Add(new ScriptureRange(existingStart, existingEnd));
			}

			return existing;
		}
	}
}
