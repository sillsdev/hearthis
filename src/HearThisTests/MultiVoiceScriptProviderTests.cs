// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2017-2025, SIL Global.
// <copyright from='2017' to='2025' company='SIL Global'>
//		Copyright (c) 2017-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using HearThis.Publishing;
using HearThis.Script;
using NUnit.Framework;
using SIL.ObjectModel;

namespace HearThisTests
{
	/// <summary>
	/// Tests for the MultiVoiceScriptProvider
	/// </summary>
	public class MultiVoiceScriptProviderTests
	{
		private MultiVoiceScriptProvider _sp1;
		private MultiVoiceScriptProvider _sp2;
		private string _input3;

		/// <summary>
		/// Make some instances on which (since they are largely immutable objects) we can make many independent tests efficiently.
		/// </summary>
		[OneTimeSetUp]
		public void MakeSamples()
		{
			var input = @"<?xml version='1.0' encoding='UTF-8'?>
<glyssenscript id='3b9fdc679b9319c3' modifieddate='2017-04-12T12:17:10.2259345-04:00' projectName='My Project' uniqueProjectId='abc123'>
  <language>
    <iso>ach</iso>
    <name>Acholi</name>
    <ldml>ach</ldml>
    <script>Latin</script>
    <fontFamily>Doulos SIL</fontFamily>
    <fontSizeInPoints>14</fontSizeInPoints>
  </language>
  <identification>
    <name>Acholi New Testament 1985</name>
    <nameLocal>Acoli Baibul 1985</nameLocal>
    <systemId type='tms'>b9236acd-66f3-44d0-98fc-3970b3d017cd</systemId>
    <systemId type='paratext'>3b9fdc679b9319c3ee45ab86cc1c0c42930c2979</systemId>
  </identification>
  <copyright>
    <statement contentType='xhtml'>
      <p>© 1985 The Bible Society of Uganda</p>
    </statement>
  </copyright>
  <script>
	<book id='EXO'>
	  <chapter id='0'>
		<block id='1' actor='David' tag='mt' verse='0' character='book title or chapter (MAT)' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00001_MAT_000_000.wav'>
		  <text>
			Exodus
		  </text>
		</block>
	  </chapter>
	</book>
	<book id='MAT'>
	  <chapter id='0'>
		<block id='1' actor='David' tag='mt' verse='0' character='book title or chapter (MAT)' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00001_MAT_000_000.wav'>
		  <text>
			JIRI ma MATAYO ocoyo
		  </text>
		</block>
	  </chapter>
	  <chapter id='3'>
		<block id='1' actor='Buck' character='John the Baptist'>
		  <text>
			test
		  </text>
		</block>
		<block id='2' actor='Buck' character='Peter'>
		  <text>
			test2
		  </text>
		</block>
		<block id='3' actor='Buck' tag='p' verse='7' character='John the Baptist' delivery='rebuking' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00070_MAT_003_007.wav'>
		  <text>
			“Wun litino twol ororo, aŋa ma owaco botwu ni myero wuriŋ woko ki i akemo pa Lubaŋa ma mito bino-ni?
		  </text>
		</block>
		<block id='4' actor='Buck' character='Peter'>
		  <text>
			test3
		  </text>
		</block>
	  </chapter>
	</book>
  </script>
</glyssenscript>
";
			_sp1 = new MultiVoiceScriptProvider(input);
			var skippedBlock =_sp1.GetBlock(39, 3, 1);
			skippedBlock.SkippedChanged += sender => { }; // code requires us to have a handler before we can set it.
			skippedBlock.Skipped = true;
			ScriptLine.SkippedStyleInfoProvider = _sp1;

			var input2 = @"<?xml version='1.0' encoding='UTF-8'?>
<glyssenscript id='3b9fdc679b9319c3' modifieddate='2017-04-12T12:17:10.2259345-04:00' projectName='My Project 2' uniqueProjectId='xyz321'>
  <language>
    <iso>ac</iso>
    <name>Acholi</name>
    <ldml>act</ldml>
    <script>Latin</script>
    <fontFamily>Andika</fontFamily>
    <fontSizeInPoints>12</fontSizeInPoints>
	<scriptDirection>RTL</scriptDirection>
  </language>
  <identification>
    <name>A test project</name>
    <nameLocal>Acoli Baibul 1985</nameLocal>
    <systemId type='tms'>b9236acd-66f3-44d0-98fc-3970b3d017cd</systemId>
    <systemId type='paratext'>3b9fdc679b9319c3ee45ab86cc1c0c42930c2979</systemId>
  </identification>
  <copyright>
    <statement contentType='xhtml'>
      <p>© 1985 The Bible Society of Uganda</p>
    </statement>
  </copyright>
  <script>
	<book id='GEN'>
	  <chapter id='0'>
		<block id='1' actor='Sally' tag='mt' verse='0' character='book title or chapter (MAT)' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00001_MAT_000_000.wav'>
		  <text>
			Genesis
		  </text>
		</block>
	  </chapter>
	</book>
	<book id='MRK'>
	  <chapter id='0'>
		<block id='1' actor='David' tag='mt' verse='0' character='book title or chapter (MAT)' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00001_MAT_000_000.wav'>
		  <text>
			JIRI ma MARK ocoyo
		  </text>
		</block>
	  </chapter>
	  <chapter id='4'>
		<block id='1' tag='q' actor='David' character='Peter'>
		  <text>
			test
		  </text>
		</block>
		<block id='2'>
		</block>
		<block id='3' actor='Fred' tag='p' verse='10' character='John the Baptist' delivery='rebuking' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00070_MAT_003_007.wav'>
		  <text>
			“A translation of the offspring of vipers?
		  </text>
		</block>
	  </chapter>
	</book>
  </script>
</glyssenscript>
";
			_sp2 = new MultiVoiceScriptProvider(input2);

			_input3 = @"<?xml version='1.0' encoding='UTF-8'?>
<glyssenscript id='3b9fdc679b9319c3' modifieddate='2017-04-12T12:17:10.2259345-04:00' projectName='My Project 3' uniqueProjectId='lmnop987'>
  <language>
    <iso>ac</iso>
    <name>Acholi</name>
    <ldml>act</ldml>
    <script>Latin</script>
    <fontFamily>Andika</fontFamily>
    <fontSizeInPoints>12</fontSizeInPoints>
	<scriptDirection>RTL</scriptDirection>
  </language>
  <identification>
    <name>A test project</name>
    <nameLocal>Acoli Baibul 1985</nameLocal>
    <systemId type='tms'>b9236acd-66f3-44d0-98fc-3970b3d017cd</systemId>
    <systemId type='paratext'>3b9fdc679b9319c3ee45ab86cc1c0c42930c2979</systemId>
  </identification>
  <copyright>
    <statement contentType='xhtml'>
      <p>© 1985 The Bible Society of Uganda</p>
    </statement>
  </copyright>
  <script>
	<book id='LUK'>
	  <chapter id='0'>
		<block id='1' actor='David' tag='mt' verse='0' character='book title or chapter (MAT)' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00001_MAT_000_000.wav'>
		  <text>
			Block 1 has two sentences. The second, this one, has commas; and also a semi-colon.
		  </text>
		</block>
		<block id='2' actor='David' tag='mt' verse='0' character='book title or chapter (MAT)' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00001_MAT_000_000.wav'>
		  <text>
			Block 2 has three sentences. This one has a semi-colons; this makes it divisible! It also has an exclamation point.
		  </text>
		</block>
	  </chapter>
	</book>
  </script>
</glyssenscript>
";
		}

		[TestCase("", new [] { "Block 1 has two sentences.", "The second, this one, has commas; and also a semi-colon.",
			"Block 2 has three sentences.", "This one has a semi-colons; this makes it divisible!",
			"It also has an exclamation point."},
			new[] {"1", "1", "2", "2", "2"})]
		[TestCase(";", new[] { "Block 1 has two sentences.", "The second, this one, has commas;", "and also a semi-colon.",
			"Block 2 has three sentences.", "This one has a semi-colons;", "this makes it divisible!",
			"It also has an exclamation point."},
			new[] { "1", "1", "1", "2", "2", "2", "2" })]
		[TestCase(";,", new[] { "Block 1 has two sentences.", "The second,", "this one,", "has commas;", "and also a semi-colon.",
			"Block 2 has three sentences.", "This one has a semi-colons;", "this makes it divisible!",
			"It also has an exclamation point."},
			new[] { "1", "1", "1", "1", "1", "2", "2", "2", "2" })]
		// Not trying a case where additional chars has white space. Caller is responsible to remove that.
		public void GetBlockWithBreaks(string additionalSeparators, string[] zeroOneLines, string[] zeroOneBlockNumbers) //, string[] fourOneLines)
		{
			var splitter = new SentenceClauseSplitter(new ReadOnlySet<char>(additionalSeparators.ToHashSet()), false);
			var sp = new MultiVoiceScriptProvider(_input3, splitter);
			Assert.That(sp.GetScriptBlockCount(41, 0), Is.EqualTo(zeroOneLines.Length));
			for (int i = 0; i < zeroOneLines.Length; i++)
			{
				var scriptLine = sp.GetBlock(41, 0, i);
				Assert.That(scriptLine.Text, Is.EqualTo(zeroOneLines[i]));
				Assert.That(scriptLine.Number, Is.EqualTo(i + 1));
				Assert.That(scriptLine.OriginalBlockNumber, Is.EqualTo(zeroOneBlockNumbers[i]));
			}

			Assert.That(sp.AllEncounteredSentenceEndingCharacters, Is.EquivalentTo(new[] { '.', '!' }));

			// Todo: verify OriginalBlockNumber can be retrieved
		}

		[TestCase(1, 0, 0, "Exodus", 1, "mt", "0", "David", "book title or chapter (MAT)")]
		[TestCase(39,0,0,"JIRI ma MATAYO ocoyo", 1, "mt", "0", "David", "book title or chapter (MAT)")]
		[TestCase(39, 3, 2, "“Wun litino twol ororo, aŋa ma owaco botwu ni myero wuriŋ woko ki i akemo pa Lubaŋa ma mito bino-ni?", 3, "p", "7", "Buck", "John the Baptist")]
		public void GetBlock(int book, int chapter, int blockNo, string text, int num, string style, string verse, string actor, string character)
		{
			var line =_sp1.GetBlock(book, chapter, blockNo);
			Assert.That(line.Text, Is.EqualTo(text));
			Assert.That(line.Number, Is.EqualTo(num));
			Assert.That(line.ParagraphStyle, Is.EqualTo(style));
			Assert.That(line.Bold, Is.False); // until something in the file can indicate otherwise
			Assert.That(line.Centered, Is.False); // likewise
			Assert.That(line.ForceHardLineBreakSplitting, Is.False); // likewise
			Assert.That(line.Heading, Is.False); // likewise
			Assert.That(line.RightToLeft, Is.False); // specified in language element, defaults to false.
			Assert.That(line.FontSize, Is.EqualTo(14)); // from language element
			Assert.That(line.FontName, Is.EqualTo("Doulos SIL")); // from language element
			Assert.That(line.Verse, Is.EqualTo(verse));
			Assert.That(line.Actor, Is.EqualTo(actor));
			Assert.That(line.Character, Is.EqualTo(character));
		}

		/// <summary>
		/// The main reason for this block using _sp2 is to check that the different values for font and RTL come through.
		/// </summary>
		/// <param name="book"></param>
		/// <param name="chapter"></param>
		/// <param name="blockNo"></param>
		/// <param name="text"></param>
		/// <param name="num"></param>
		/// <param name="style"></param>
		/// <param name="verse"></param>
		[TestCase(0, 0, 0, "Genesis", 1, "mt", "0")]
		[TestCase(40, 0, 0, "JIRI ma MARK ocoyo", 1, "mt", "0")]
		[TestCase(40, 4, 2, "“A translation of the offspring of vipers?", 3, "p", "10")]
		public void GetBlock2(int book, int chapter, int blockNo, string text, int num, string style, string verse)
		{
			var line = _sp2.GetBlock(book, chapter, blockNo);
			Assert.That(line.Text, Is.EqualTo(text));
			Assert.That(line.Number, Is.EqualTo(num));
			Assert.That(line.ParagraphStyle, Is.EqualTo(style));
			Assert.That(line.Bold, Is.False); // until something in the file can indicate otherwise
			Assert.That(line.Centered, Is.False); // likewise
			Assert.That(line.ForceHardLineBreakSplitting, Is.False); // likewise
			Assert.That(line.Heading, Is.False); // likewise
			Assert.That(line.RightToLeft, Is.True);
			Assert.That(line.FontSize, Is.EqualTo(12));
			Assert.That(line.FontName, Is.EqualTo("Andika"));
			Assert.That(line.Verse, Is.EqualTo(verse));
		}

		[TestCase(0, 0)] // book not found
		[TestCase(27, 0)] // another book not found
		[TestCase(1, 1)]
		[TestCase(39, 5)]
		public void GetScriptBlockCountBook(int book, int count)
		{
			Assert.That(_sp1.GetScriptBlockCount(book), Is.EqualTo(count));
		}

		[TestCase(0, 0, 0, 0, 0)] // book not found
		[TestCase(27, 0, 0, 0, 0)] // another book not found
		[TestCase(39, 10, 0, 0, 0)] // chapter not found
		[TestCase(1, 0, 1, 1, 1)] // nothing filtered
		[TestCase(39, 0, 1, 1, 1)] // nothing filtered
		[TestCase(39, 3, 4, 3, 4)]
		public void GetScriptBlockCountChapter(int book, int chapter, int count, int unskippedCount, int unfilteredCount)
		{
			Assert.That(_sp1.GetScriptBlockCount(book, chapter), Is.EqualTo(count));
			Assert.That(_sp1.GetUnskippedScriptBlockCount(book, chapter), Is.EqualTo(unskippedCount));
			Assert.That(_sp1.GetUnfilteredScriptBlockCount(book, chapter), Is.EqualTo(unfilteredCount));
		}

		[TestCase(0, 0, 0)] // book not found
		[TestCase(27, 0, 0)] // another book not found
		[TestCase(39, 10, 0)] // chapter not found
		[TestCase(1, 0, 1)]
		[TestCase(39, 0, 1)]
		[TestCase(39, 3, 4)]
		public void GetTranslatedVerseCount(int book, int chapter, int count)
		{
			Assert.That(_sp1.GetTranslatedVerseCount(book, chapter), Is.EqualTo(count));
		}

		/// <summary>
		/// Tests that EthnologueCode is taken from language.ldml, not language.iso. Is that what we want??
		/// </summary>
		[Test]
		public void EthnologueCode()
		{
			Assert.That(_sp1.EthnologueCode, Is.EqualTo("ach"));
			Assert.That(_sp2.EthnologueCode, Is.EqualTo("act"));
		}

		[Test]
		public void ProjectFolderName()
		{
			Assert.That(_sp1.ProjectFolderName, Is.EqualTo("My Project abc123"));
			Assert.That(_sp2.ProjectFolderName, Is.EqualTo("My Project 2 xyz321"));
		}

		/// <summary>
		/// At this point we're treating block tags as paragraph styles. Not completely sure that's right.
		/// </summary>
		[Test]
		public void AllEncounteredParagraphStyleNames()
		{
			Assert.That(_sp1.AllEncounteredParagraphStyleNames, Is.EquivalentTo(new[] {"mt", "p"}));
			Assert.That(_sp2.AllEncounteredParagraphStyleNames, Is.EquivalentTo(new[] { "mt", "p", "q" }));
		}

		[Test]
		public void Actors()
		{
			Assert.That(_sp1.Actors, Is.EquivalentTo(new[] { "David", "Buck" }));
			Assert.That(_sp2.Actors, Is.EquivalentTo(new[] { "Sally", "David", "Fred" }));
		}

		[TestCase("sp1", "David", "book title or chapter (MAT)")]
		[TestCase("sp1", "Buck", "John the Baptist;Peter")]
		[TestCase("sp2", "David", "book title or chapter (MAT);Peter")]
		public void GetCharacters(string which, string actor, string characters)
		{
			var sp = (which == "sp1") ? _sp1 : _sp2;
			Assert.That(string.Join(";", sp.GetCharacters(actor)), Is.EqualTo(characters));
		}

		// In sp1 Mat 3 we have originally
		//	0: Buck/John the Baptist/test
		//	1: Buck/Peter/test2
		//	2: Buck/John the Baptist/“Wun litino twol ororo...
		//	3: Buck/Peter/test3
		[TestCase("sp1", "David", "book title or chapter (MAT)", 1, 0, 0, "Exodus")] // no change (nothing prior filtered)
		[TestCase("sp1", "David", "book title or chapter (MAT)", 39, 0, 0, "JIRI ma MATAYO ocoyo")] // no change (nothing prior filtered)
		[TestCase("sp1", "Buck", "John the Baptist", 39, 3, 0,
			"test")] // no change
		[TestCase("sp1", "Buck", "John the Baptist", 39, 3, 1,
			"“Wun litino twol ororo, aŋa ma owaco botwu ni myero wuriŋ woko ki i akemo pa Lubaŋa ma mito bino-ni?")] // was at index 1, now 1 since earlier block in chapter not this character
		[TestCase("sp1", "Buck", "Peter", 39, 3, 0,
			"test2")] // was at index 1, now 0 since earlier blocks in chapter not this character
		[TestCase("sp1", "Buck", "Peter", 39, 3, 1,
			"test3")] // was at index 3, now 1 since earlier blocks in chapter not this character
		[TestCase("sp2", "David", "Peter", 40, 4, 0, "test")] // no change
		[TestCase("sp2", "Fred", "John the Baptist", 40, 4, 0,
			"“A translation of the offspring of vipers?")] // moved from 2 to 0
		public void RestrictToCharactersBlocks(string which, string actor, string character, int book, int chapter, int line,
			string blockContent)
		{
			var sp = (which == "sp1") ? _sp1 : _sp2;
			sp.RestrictToCharacter(actor, character);
			Assert.That(sp.GetBlock(book, chapter, line).Text, Is.EqualTo(blockContent));
			// This is a quick check that this routine is actually getting the unfiltered block.
			Assert.That(sp.GetUnfilteredBlock(book, chapter, line).Number, Is.EqualTo(line + 1));
			sp.RestrictToCharacter(null, null);
		}

		[TestCase("sp1", "David", "book title or chapter (MAT)", 1, 0, 1, 1, 1, 1,1)] // no change (nothing filtered)
		[TestCase("sp1", "David", "book title or chapter (MAT)", 39, 0, 1, 1, 1, 1,1)] // no change (nothing filtered)
		[TestCase("sp1", "Buck", "John the Baptist", 39, 3, 2, 3, 2, 4, 4)] // 2 blocks with this character (1/4 skipped)
		[TestCase("sp1", "Buck", "Peter", 39, 3, 2, 3, 2, 4, 4)] // 2 blocks with this character (1 skipped)
		[TestCase("sp2", "David", "Peter", 40, 4, 1, 3, 1, 3, 2)] // 1 block with this character (0/3 skipped) (2/3 translated, including the character one)
		public void RestrictToCharactersChapters(string which, string actor, string character, int book, int chapter,
			int scriptBlockCount, int unskippedBlockCount, int transVerseCount, int unfilteredCount, int unfilteredTransVerseCount)
		{
			var sp = (which == "sp1") ? _sp1 : _sp2;
			sp.RestrictToCharacter(actor, character);
			Assert.That(sp.GetScriptBlockCount(book, chapter), Is.EqualTo(scriptBlockCount));
			Assert.That(sp.GetUnskippedScriptBlockCount(book, chapter), Is.EqualTo(unskippedBlockCount));
			Assert.That(sp.GetTranslatedVerseCount(book, chapter), Is.EqualTo(transVerseCount));
			Assert.That(sp.GetUnfilteredTranslatedVerseCount(book, chapter), Is.EqualTo(unfilteredTransVerseCount));
			Assert.That(sp.GetUnfilteredScriptBlockCount(book, chapter), Is.EqualTo(unfilteredCount));
			sp.RestrictToCharacter(null, null);
		}

		[TestCase("sp1", "David", "book title or chapter (MAT)", 1, 1)]
		[TestCase("sp1", "Buck", "John the Baptist", 39, 2)]
		[TestCase("sp1", "Buck", "Peter", 39, 2)]
		[TestCase("sp2", "David", "Peter", 40, 1)]
		public void RestrictToCharactersBooks(string which, string actor, string character, int book, int scriptBlockCount)
		{
			var sp = (which == "sp1") ? _sp1 : _sp2;
			sp.RestrictToCharacter(actor, character);
			Assert.That(sp.GetScriptBlockCount(book), Is.EqualTo(scriptBlockCount));
			sp.RestrictToCharacter(null, null);
		}

		[TestCase("sp1", "David", "book title or chapter (MAT)", 1, 0, 0, true)]
		[TestCase("sp1", "David", "book title or chapter (MAT)", 1, 0, 1, false)] // block out of range
		[TestCase("sp1", "David", "Peter", 1, 0, 0, false)] // wrong character
		[TestCase("sp1", "Buck", "book title or chapter (MAT)", 1, 0, 0, false)] // wrong actor
		[TestCase("sp1", "Buck", "John the Baptist", 39, 3, 2, true)]
		public void IsBlockInCharacter(string which, string actor, string character, int book, int chapter, int block, bool expected)
		{
			var sp = (which == "sp1") ? _sp1 : _sp2;
			sp.RestrictToCharacter(actor, character);
			Assert.That(sp.IsBlockInCharacter(book, chapter, block), Is.EqualTo(expected));
			sp.RestrictToCharacter(null, null);
		}

		[Test]
		public void IsBlockInCharacter_NoActorSpecified_AlwaysTrue()
		{
			Assert.That(_sp1.IsBlockInCharacter(1, 0, 0), Is.True);
			Assert.That(_sp1.IsBlockInCharacter(39, 3, 2), Is.True);
		}

		[Test]
		public void GetNextUnrecordedLineInChapterForCharacter()
		{
			var availableRecordings = new FakeRecordingAvailability();
			_sp1.RecordingAvailabilitySource = availableRecordings;
			_sp1.RestrictToCharacter(null, null);
			// No character, no change (return startLine)
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39,3, 0), Is.EqualTo(0));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 1), Is.EqualTo(1));

			_sp1.RestrictToCharacter("Buck", "Peter");
			// We haven't told it anything is recorded yet, but 1 is skipped
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 0), Is.EqualTo(3));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 1), Is.EqualTo(3));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 3), Is.EqualTo(3)); // no change if start block is valid choice

			// Buck has two Peter recordings in Mat 3. Pretend the first is recorded
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Matthew", 3, 1);
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 0), Is.EqualTo(3));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 1), Is.EqualTo(3));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 3), Is.EqualTo(3));

			// Now all of Buck/Peter is recorded. Since we can't find an unrecorded block for the character,
			// we won't alter startLine.
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Matthew", 3, 3);
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 0), Is.EqualTo(0));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 1), Is.EqualTo(1));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 3), Is.EqualTo(3));

			// Make sure we can skip an unrecorded line and find a recorded one.
			_sp1.RestrictToCharacter("Buck", "John the Baptist");
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Matthew", 3, 0);
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 0), Is.EqualTo(2));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 1), Is.EqualTo(2));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 2), Is.EqualTo(2));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 3), Is.EqualTo(3)); // nothing found, no change.

			// Check that nothing happens if we ask for nonexistent books or chapters
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(0, 3, 7), Is.EqualTo(7));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 10, 11), Is.EqualTo(11));

			// Even though there are both recorded and unrecorded lines in Mat 3, with no character
			// selected we don't alter the start line.
			_sp1.RestrictToCharacter(null, null);
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 0), Is.EqualTo(0));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 1), Is.EqualTo(1));
			Assert.That(_sp1.GetNextUnrecordedLineInChapterForCharacter(39, 3, 3), Is.EqualTo(3));

			_sp1.RecordingAvailabilitySource = null; // return to default state.
		}

		[Test]
		public void GetNextUnrecordedChapterForCharacter()
		{
			var input = @"<?xml version='1.0' encoding='UTF-8'?>
<glyssenscript id='3b9fdc679b9319c3' modifieddate='2017-04-12T12:17:10.2259345-04:00' projectName='My Project for Next Unrecorded Chapter' uniqueProjectId='987654321zyx'>
  <language>
    <iso>ac</iso>
    <name>Acholi</name>
    <ldml>act</ldml>
    <script>Latin</script>
    <fontFamily>Andika</fontFamily>
    <fontSizeInPoints>12</fontSizeInPoints>
	<scriptDirection>RTL</scriptDirection>
  </language>
  <identification>
    <name>A test project</name>
    <nameLocal>Acoli Baibul 1985</nameLocal>
    <systemId type='tms'>b9236acd-66f3-44d0-98fc-3970b3d017cd</systemId>
    <systemId type='paratext'>3b9fdc679b9319c3ee45ab86cc1c0c42930c2979</systemId>
  </identification>
  <copyright>
    <statement contentType='xhtml'>
      <p>© 1985 The Bible Society of Uganda</p>
    </statement>
  </copyright>
  <script>
	<book id='MRK'>
	  <chapter id='0'>
		<block id='1' actor='David' tag='mt' verse='0' character='book title or chapter (MAT)' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00001_MAT_000_000.wav'>
		  <text>
			JIRI ma MARK ocoyo
		  </text>
		</block>
	  </chapter>
	  <chapter id='1'>
		<block id='1' actor='Susan' tag='mt' verse='0' character='Elizabeth'>
		  <text>
			JIRI ma MARK ocoyo
		  </text>
		</block>
		<block id='2' actor='David' tag='mt' verse='0' character='Peter'>
		  <text>
			JIRI ma MARK ocoyo
		  </text>
		</block>
      </chapter>
	  <chapter id='2'>
		<block id='1' actor='Susan' tag='mt' verse='0' character='Elizabeth'>
		  <text>
			JIRI ma MARK ocoyo
		  </text>
		</block>
		<block id='2' actor='David' tag='mt' verse='0' character='Peter'>
		  <text>
			JIRI ma MARK ocoyo
		  </text>
		</block>
      </chapter>
      <chapter id='4'>
		<block id='1' tag='q' actor='David' character='Peter'>
		  <text>
			test
		  </text>
		</block>
		<block id='2'>
		</block>
		<block id='3' actor='Fred' tag='p' verse='10' character='John the Baptist' delivery='rebuking' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00070_MAT_003_007.wav'>
		  <text>
			“A translation of the offspring of vipers?
		  </text>
		</block>
	  </chapter>
	</book>
  </script>
</glyssenscript>
";
			var sp = new MultiVoiceScriptProvider(input);
			var availableRecordings = new FakeRecordingAvailability();
			sp.RecordingAvailabilitySource = availableRecordings;
			sp.RestrictToCharacter(null, null);
			// No character, no change (return startChapter)
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 0), Is.EqualTo(0));
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 1), Is.EqualTo(1));

			sp.RestrictToCharacter("Susan", "Elizabeth");
			// We haven't told it anything is recorded yet, but chapter 0 has nothing for this character
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 0), Is.EqualTo(1));
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 1), Is.EqualTo(1)); // no change, that chapter has unrecorded stuff

			// Pretend that first item of Elizabeth's is recorded. Her next is in chapter 2.
			availableRecordings.SetHaveClip(sp.ProjectFolderName, "Mark", 1, 0);
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 0), Is.EqualTo(2));
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 1), Is.EqualTo(2));
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 2), Is.EqualTo(2));

			// Now all of Elizabeth is recorded. Since we can't find an unrecorded block for the character,
			// we won't alter startChapter.
			availableRecordings.SetHaveClip(sp.ProjectFolderName, "Mark", 2, 0);
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 0), Is.EqualTo(0));
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 1), Is.EqualTo(1));

			// Check that nothing happens if we ask for nonexistent books
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(0, 7), Is.EqualTo(7));

			// Even though there are both recorded and unrecorded lines in Mark 2, with no character
			// selected we don't alter the start line.
			sp.RestrictToCharacter(null, null);
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 0), Is.EqualTo(0));
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 1), Is.EqualTo(1));
			Assert.That(sp.GetNextUnrecordedChapterForCharacter(40, 3), Is.EqualTo(3));
		}

		[Test]
		public void FullyRecordedCharacters()
		{
			var availableRecordings = new FakeRecordingAvailability();
			_sp1.RecordingAvailabilitySource = availableRecordings;
			_sp1.RestrictToCharacter(null,null);
			Assert.That(_sp1.FullyRecordedCharacters, Has.Count.EqualTo(0));

			_sp1.ClearFullyRecordedCharacters();
			// Both of David's recordings exist
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Exodus", 0, 0);
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Matthew", 0, 0);
			// Only one of Buck's John the Baptist recordings exists
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Matthew", 0, 0);
			Assert.That(_sp1.FullyRecordedCharacters, Has.Count.EqualTo(1));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("David", "book title or chapter (MAT)"));

			_sp1.ClearFullyRecordedCharacters();
			// Both of Buck's Peter recordings
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Matthew", 3, 1);
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Matthew", 3, 3);
			Assert.That(_sp1.FullyRecordedCharacters, Has.Count.EqualTo(2)); // This should verify that no others are allrecorded.
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("David", "book title or chapter (MAT)"));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck", "Peter"));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck", "John the Baptist"), Is.False);
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Joe", "the nonexistent"), Is.False);
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck"), Is.False); // since his John the Baptist lines aren't all done.

			_sp1.ClearFullyRecordedCharacters();
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Matthew", 3, 0);
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Matthew", 3, 2);
			Assert.That(_sp1.FullyRecordedCharacters, Has.Count.EqualTo(3));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("David", "book title or chapter (MAT)"));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck", "Peter"));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck", "John the Baptist"));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck"));

			// For this test we don't clear. Therefore, to notice changes, we're depending
			// on the check-current-character code.
			_sp1.RestrictToCharacter("Buck", "Peter");
			availableRecordings.RemoveHaveClip(_sp1.ProjectFolderName, "Matthew", 3, 3);
			Assert.That(_sp1.FullyRecordedCharacters, Has.Count.EqualTo(2));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("David", "book title or chapter (MAT)"));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck", "John the Baptist"));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck"), Is.False);

			// now put it back and check THAT is noticed.
			availableRecordings.SetHaveClip(_sp1.ProjectFolderName, "Matthew", 3, 3);
			Assert.That(_sp1.FullyRecordedCharacters, Has.Count.EqualTo(3));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("David", "book title or chapter (MAT)"));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck", "Peter"));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck", "John the Baptist"));
			Assert.That(_sp1.FullyRecordedCharacters.AllRecorded("Buck"));

			_sp1.RecordingAvailabilitySource = null; // return to default state.
			_sp1.RestrictToCharacter(null, null);
		}
	}

	class FakeRecordingAvailability : IRecordingAvailability
	{
		HashSet<Tuple<string, string, int, int>> availableRecordings = new HashSet<Tuple<string, string, int, int>>();

		public void SetHaveClip(string projectName, string bookName, int chapterNumber1Based, int lineNumberZeroBased)
		{
			availableRecordings.Add(Tuple.Create(projectName, bookName, chapterNumber1Based, lineNumberZeroBased));
		}

		public void RemoveHaveClip(string projectName, string bookName, int chapterNumber1Based, int lineNumberZeroBased)
		{
			availableRecordings.Remove(Tuple.Create(projectName, bookName, chapterNumber1Based, lineNumberZeroBased));
		}

		public bool HasClipUnfiltered(string projectName, string bookName, int chapterNumber1Based, int lineNumberZeroBased)
		{
			return availableRecordings.Contains(Tuple.Create(projectName, bookName, chapterNumber1Based, lineNumberZeroBased));
		}
	}
}
