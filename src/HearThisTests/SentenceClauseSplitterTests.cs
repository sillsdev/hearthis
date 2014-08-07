using System;
using System.Diagnostics;
using System.Linq;
using HearThis.Script;
using NUnit.Framework;

namespace HearThisTests
{
	/// <summary>
	/// Test the tricks we do to limit preceding context when short of space
	/// </summary>
	[TestFixture]
	public class SentenceClauseSplitterTests
	{
		private static readonly char[] standardBreakCharacters = { '.', '?', '!', '।', '॥' };

		[Test]
		public void BreakIntoChunks_EmptyString_YieldsNothing()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters);
			Assert.AreEqual(0, splitter.BreakIntoChunks(String.Empty).Count());
		}

		[Test]
		public void BreakIntoChunks_SinglePeriod_YieldsSingleChunkWithPeriod()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters);
			var result = splitter.BreakIntoChunks(".").ToList();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(".", result[0].Text);
		}

		[Test]
		public void BreakIntoChunks_TwoSimpleSentences_YieldsTwoChunks()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters);
			var result = splitter.BreakIntoChunks("This is a cat. Why is it here?").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("This is a cat.", result[0].Text);
			Assert.AreEqual("Why is it here?", result[1].Text);
		}

		[Test]
		public void BreakIntoChunks_DoNotBreakQuotations_YieldsChunksWithIncludedQuoteMarks()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters);
			var result = splitter.BreakIntoChunks("“I'm pretty happy,” said John. “Me, too,” mumbled Alice.").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("“I'm pretty happy,” said John.", result[0].Text);
			Assert.AreEqual("“Me, too,” mumbled Alice.", result[1].Text);
		}

		[Test]
		public void BreakIntoChunks_BreakQuotations_YieldsQuotationsAsSeparateChunks()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters, "“", "”");
			var result = splitter.BreakIntoChunks("“I'm pretty happy,” said John. “Me, too,” mumbled Alice.").ToList();
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual("“I'm pretty happy,”", result[0].Text);
			Assert.AreEqual("said John.", result[1].Text);
			Assert.AreEqual("“Me, too,”", result[2].Text);
			Assert.AreEqual("mumbled Alice.", result[3].Text);
		}

		[Test]
		public void BreakIntoChunks_DoNotBreakSentencesWithQuotationsContainingQuestionsAndExclamations_YieldsChunksWithIncludedQuoteMarks()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters);
			var result = splitter.BreakIntoChunks("“Do you want to go to the zoo?” asked John. “No way!” shouted the twins.").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("“Do you want to go to the zoo?” asked John.", result[0].Text);
			Assert.AreEqual("“No way!” shouted the twins.", result[1].Text);
		}

		[Test]
		public void BreakIntoChunks_BreakSentencesWithQuotationsContainingQuestionsAndExclamations_YieldsChunksWithIncludedQuoteMarks()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters, "“", "”");
			var result = splitter.BreakIntoChunks("“Do you want to go to the zoo?” asked John. “No way!” shouted the twins.").ToList();
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual("“Do you want to go to the zoo?”", result[0].Text);
			Assert.AreEqual("asked John.", result[1].Text);
			Assert.AreEqual("“No way!”", result[2].Text);
			Assert.AreEqual("shouted the twins.", result[3].Text);
		}

		[Test]
		public void BreakIntoChunks_BreakSentencesWithNestedQuotations_YieldsChunksForTopLevelQuotesOnly()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters, "“", "”");
			var result = splitter.BreakIntoChunks("Then God said, “Do not say, ‘Why did the Lord say, “You have sinned,” when we did what was right in our own eyes,’ or I will pluck you from this good land and hurl you into the desert!”").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("Then God said,", result[0].Text);
			Assert.AreEqual("“Do not say, ‘Why did the Lord say, “You have sinned,” when we did what was right in our own eyes,’ or I will pluck you from this good land and hurl you into the desert!”", result[1].Text);
		}

		[Test]
		public void BreakIntoChunks_BreakSentencesWithMultiCharacterQuotationMarks_YieldsChunksWithIncludedQuoteMarks()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters, "<<", ">>");
			var result = splitter.BreakIntoChunks("Then God said, <<Do not say, <Why did the Lord say, <<You have sinned,>> when we did what was right in our own eyes,> or I will pluck you from this good land.>>").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("Then God said,", result[0].Text);
			Assert.AreEqual("<<Do not say, <Why did the Lord say, <<You have sinned,>> when we did what was right in our own eyes,> or I will pluck you from this good land.>>", result[1].Text);
		}

		[Test]
		public void BreakIntoChunks_MultisentenceQuotes_YieldsChunksForIndividualSentencesWithinQuotation()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters, "<<", ">>");
			var result = splitter.BreakIntoChunks("<<This is fine. This is nice. This is good,>> said Fred.").ToList();
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual("<<This is fine.", result[0].Text);
			Assert.AreEqual("This is nice.", result[1].Text);
			Assert.AreEqual("This is good,>>", result[2].Text);
			Assert.AreEqual("said Fred.", result[3].Text);
		}

		[Test]
		public void BreakIntoChunks_ParagraphStartingInMiddleOfQuoteWithNoOpeningQuotes_YieldsChunksForIndividualSentencesWithinQuotation()
		{
			var splitter = new SentenceClauseSplitter(standardBreakCharacters, "“", "”");
			var result = splitter.BreakIntoChunks("This is fine.” Let's go fishing.").ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("This is fine.”", result[0].Text);
			Assert.AreEqual("Let's go fishing.", result[1].Text);
		}

		[Category("SkipOnTeamCity")]
		[Test]
		public void BreakIntoChunks_SpeedTest()
		{
			var separators = new[] { '.', '?', '!',
				'\u0589', // ARMENIAN FULL STOP
				'\u061F', // ARABIC QUESTION MARK
				'\u06D4', // ARABIC FULL STOP
				'\u0700', // SYRIAC END OF PARAGRAPH
				'\u0701', // SYRIAC SUPRALINEAR FULL STOP
				'\u0702', // SYRIAC SUBLINEAR FULL STOP
				'\u07F9', // NKO EXCLAMATION MARK
				'\u0964', // DEVANAGARI DANDA
				'\u0965', // DEVANAGARI DOUBLE DANDA
				'\u104A', // MYANMAR SIGN LITTLE SECTION
				'\u104B', // MYANMAR SIGN SECTION
				'\u1362', // ETHIOPIC FULL STOP
				'\u1367', // ETHIOPIC QUESTION MARK
				'\u1368', // ETHIOPIC PARAGRAPH SEPARATOR
				'\u166E', // CANADIAN SYLLABICS FULL STOP
				'\u1803', // MONGOLIAN FULL STOP
				'\u1809', // MONGOLIAN MANCHU FULL STOP
				'\u1944', // LIMBU EXCLAMATION MARK
				'\u1945', // LIMBU QUESTION MARK
				'\u1AA8', // TAI THAM SIGN KAAN
				'\u1AA9', // TAI THAM SIGN KAANKUU
				'\u1AAA', // TAI THAM SIGN SATKAAN
				'\u1AAB', // TAI THAM SIGN SATKAANKUU
				'\u1B5A', // BALINESE PANTI
				'\u1B5B', // BALINESE PAMADA
				'\u1B5E', // BALINESE CARIK SIKI
				'\u1B5F', // BALINESE CARIK PAREREN
				'\u1C3B', // LEPCHA PUNCTUATION TA-ROL
				'\u1C3C', // LEPCHA PUNCTUATION NYET THYOOM TA-ROL
				'\u1C7E', // OL CHIKI PUNCTUATION MUCAAD
				'\u1C7F', // OL CHIKI PUNCTUATION DOUBLE MUCAAD
				'\u203C', // DOUBLE EXCLAMATION MARK
				'\u203D', // INTERROBANG
				'\u2047', // DOUBLE QUESTION MARK
				'\u2048', // QUESTION EXCLAMATION MARK
				'\u2049', // EXCLAMATION QUESTION MARK
				'\u2E2E', // REVERSED QUESTION MARK
				'\u3002', // IDEOGRAPHIC FULL STOP
				'\uA4FF', // LISU PUNCTUATION FULL STOP
				'\uA60E', // VAI FULL STOP
				'\uA60F', // VAI QUESTION MARK
				'\uA6F3', // BAMUM FULL STOP
				'\uA6F7', // BAMUM QUESTION MARK
				'\uA876', // PHAGS-PA MARK SHAD
				'\uA877', // PHAGS-PA MARK DOUBLE SHAD
				'\uA8CE', // SAURASHTRA DANDA
				'\uA8CF', // SAURASHTRA DOUBLE DANDA
				'\uA92F', // KAYAH LI SIGN SHYA
				'\uA9C8', // JAVANESE PADA LINGSA
				'\uA9C9', // JAVANESE PADA LUNGSI
				'\uAA5D', // CHAM PUNCTUATION DANDA
				'\uAA5E', // CHAM PUNCTUATION DOUBLE DANDA
				'\uAA5F', // CHAM PUNCTUATION TRIPLE DANDA
				'\uAAF0', // MEETEI MAYEK CHEIKHAN
				'\uAAF1', // MEETEI MAYEK AHANG KHUDAM
				'\uABEB', // MEETEI MAYEK CHEIKHEI
				'\uFE52', // SMALL FULL STOP
				'\uFE56', // SMALL QUESTION MARK
				'\uFE57', // SMALL EXCLAMATION MARK
				'\uFF01', // FULLWIDTH EXCLAMATION MARK
				'\uFF0E', // FULLWIDTH FULL STOP
				'\uFF1F', // FULLWIDTH QUESTION MARK
				'\uFF61', // HALFWIDTH IDEOGRAPHIC FULL STOP
				// These would require surrogate pairs
				//'\u11047', // BRAHMI DANDA
				//'\u11048', // BRAHMI DOUBLE DANDA
				//'\u110BE', // KAITHI SECTION MARK
				//'\u110BF', // KAITHI DOUBLE SECTION MARK
				//'\u110C0', // KAITHI DANDA
				//'\u110C1', // KAITHI DOUBLE DANDA
				//'\u11141', // CHAKMA DANDA
				//'\u11142', // CHAKMA DOUBLE DANDA
				//'\u11143', // CHAKMA QUESTION MARK
				//'\u111C5', // SHARADA DANDA
				//'\u111C6', // SHARADA DOUBLE DANDA
			};
			var splitter = new SentenceClauseSplitter(separators);
			const string textToBreak = "This is a sentence. So is this. Why not? Go for it! Is this a little question\uFE56 What\u203D hoo-rah\u2047 ";
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < 5000; i++)
				splitter.BreakIntoChunks(textToBreak).ToList();
			stopwatch.Stop();
			Debug.WriteLine("Elapsed milliseconds: " + stopwatch.ElapsedMilliseconds);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds < 30);
		}
	}
}
