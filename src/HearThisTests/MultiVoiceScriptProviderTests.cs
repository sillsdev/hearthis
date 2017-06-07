using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearThis.Script;
using NUnit.Framework;

namespace HearThisTests
{
	/// <summary>
	/// Tests for the MultiVoiceScriptProvider
	/// </summary>
	public class MultiVoiceScriptProviderTests
	{
		private MultiVoiceScriptProvider _sp1;
		private MultiVoiceScriptProvider _sp2;

		/// <summary>
		/// Make some instances on which (since they are largely immutable objects) we can make many independent tests efficiently.
		/// </summary>
		[TestFixtureSetUp]
		public void MakeSamples()
		{
			var input = @"<?xml version='1.0' encoding='UTF-8'?>
<glyssenscript id='3b9fdc679b9319c3' modifieddate='2017-04-12T12:17:10.2259345-04:00'>
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
		  <vern size ='20'>
			Exodus
		  </vern>
		  <primaryref xml:lang='es'>
			EXEO
		  </primaryref>
		  <secondaryref xml:lang='en'>
			EXODUS
		  </secondaryref>
		</block>
	  </chapter>
	</book>
	<book id='MAT'>
	  <chapter id='0'>
		<block id='1' actor='David' tag='mt' verse='0' character='book title or chapter (MAT)' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00001_MAT_000_000.wav'>
		  <vern size ='20'>
			JIRI ma MATAYO ocoyo
		  </vern>
		  <primaryref xml:lang='es'>
			MATEO
		  </primaryref>
		  <secondaryref xml:lang='en'>
			MATTHEW
		  </secondaryref>
		</block>
	  </chapter>
	  <chapter id='3'>
		<block id='1' actor='Buck' character='John the Baptist'>
		  <vern size ='4'>
			test
		  </vern>
		</block>
		<block id='2' actor='Buck' character='Peter'>
		  <vern size ='5'>
			test2
		  </vern>
		</block>
		<block id='3' actor='Buck' tag='p' verse='7' character='John the Baptist' delivery='rebuking' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00070_MAT_003_007.wav'>
		  <vern size ='101'>
			“Wun litino twol ororo, aŋa ma owaco botwu ni myero wuriŋ woko ki i akemo pa Lubaŋa ma mito bino-ni?
		  </vern>
		  <primaryref xml:lang='es'>
			!!Generación de víboras! ¿Quién os enseñó a huir de la ira venidera?
		  </primaryref>
		  <secondaryref xml:lang='en'>
			“You offspring of vipers, who warned you to flee from the wrath to come?
		  </secondaryref>
		</block>
		<block id='4' actor='Buck' character='Peter'>
		  <vern size ='5'>
			test3
		  </vern>
		</block>
	  </chapter>
	</book>
  </script>
</glyssenscript>
";
			_sp1 = new MultiVoiceScriptProvider(input);
			var skippedBlock =_sp1.GetBlock(39, 3, 1);
			skippedBlock.OnSkippedChanged += sender => { }; // code requires us to have a handler before we can set it.
			skippedBlock.Skipped = true;
			ScriptLine.SkippedStyleInfoProvider = _sp1;

			var input2 = @"<?xml version='1.0' encoding='UTF-8'?>
<glyssenscript id='3b9fdc679b9319c3' modifieddate='2017-04-12T12:17:10.2259345-04:00'>
  <language>
    <iso>ac</iso>
    <name>Acholi</name>
    <ldml>act</ldml>
    <script>Latin</script>
    <fontFamily>Andika</fontFamily>
    <fontSizeInPoints>12</fontSizeInPoints>
	<rightToLeft>true</rightToLeft>
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
		  <vern size ='20'>
			Genesis
		  </vern>
		  <primaryref xml:lang='es'>
			GENEO
		  </primaryref>
		  <secondaryref xml:lang='en'>
			GENESIS
		  </secondaryref>
		</block>
	  </chapter>
	</book>
	<book id='MRK'>
	  <chapter id='0'>
		<block id='1' actor='David' tag='mt' verse='0' character='book title or chapter (MAT)' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00001_MAT_000_000.wav'>
		  <vern size ='20'>
			JIRI ma MARK ocoyo
		  </vern>
		  <primaryref xml:lang='es'>
			MARKO
		  </primaryref>
		  <secondaryref xml:lang='en'>
			MARK
		  </secondaryref>
		</block>
	  </chapter>
	  <chapter id='4'>
		<block id='1' tag='q' actor='David' character='Peter'>
		  <vern size ='4'>
			test
		  </vern>
		</block>
		<block id='2'>
		</block>
		<block id='3' actor='Fred' tag='p' verse='10' character='John the Baptist' delivery='rebuking' file='C:\Users\bogle\Documents\Acholi New Testament 1985 Audio (1) Recording Script Clips\MAT\Acholi_New_Testament_1985_Audio_(1)_00070_MAT_003_007.wav'>
		  <vern size ='101'>
			“A translation of the offspring of vipers?
		  </vern>
		  <primaryref xml:lang='es'>
			!!Generación de víboras! ¿Quién os enseñó a huir de la ira venidera?
		  </primaryref>
		  <secondaryref xml:lang='en'>
			“You offspring of vipers, who warned you to flee from the wrath to come?
		  </secondaryref>
		</block>
	  </chapter>
	</book>
  </script>
</glyssenscript>
";
			_sp2 = new MultiVoiceScriptProvider(input2);
		}

		[TestCase(1, 0, 0, "Exodus", 1, "mt", "0")]
		[TestCase(39,0,0,"JIRI ma MATAYO ocoyo", 1, "mt", "0")]
		[TestCase(39, 3, 2, "“Wun litino twol ororo, aŋa ma owaco botwu ni myero wuriŋ woko ki i akemo pa Lubaŋa ma mito bino-ni?", 3, "p", "7")]
		public void GetBlock(int book, int chapter, int blockNo, string text, int num, string style, string verse)
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

		[TestCase(0, 0, 0, 0)] // book not found
		[TestCase(27, 0, 0, 0)] // another book not found
		[TestCase(39, 10, 0, 0)] // chapter not found
		[TestCase(1, 0, 1, 1)]
		[TestCase(39, 0, 1,1)]
		[TestCase(39, 3, 4,3)]
		public void GetScriptBlockCountChapter(int book, int chapter, int count, int unskippedCount)
		{
			Assert.That(_sp1.GetScriptBlockCount(book, chapter), Is.EqualTo(count));
			Assert.That(_sp1.GetUnskippedScriptBlockCount(book, chapter), Is.EqualTo(unskippedCount));
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
			Assert.That(_sp1.ProjectFolderName, Is.EqualTo("Acholi New Testament 1985"));
			Assert.That(_sp2.ProjectFolderName, Is.EqualTo("A test project"));
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
		[TestCase("sp2", "Fred", "John the Baptist", 40, 4, 0, "“A translation of the offspring of vipers?")] // moved from 2 to 0
		public void RestrictToCharactersBlocks(string which, string actor, string character, int book, int chapter, int line, string blockContent)
		{
			var sp = (which == "sp1") ? _sp1 : _sp2;
			sp.RestrictToCharacters(actor, character);
			Assert.That(sp.GetBlock(book, chapter, line).Text, Is.EqualTo(blockContent));
			sp.RestrictToCharacters(null, null);
		}

		[TestCase("sp1", "David", "book title or chapter (MAT)", 1, 0, 1, 1, 1)] // no change (nothing filtered)
		[TestCase("sp1", "David", "book title or chapter (MAT)", 39, 0, 1, 1, 1)] // no change (nothing filtered)
		[TestCase("sp1", "Buck", "John the Baptist", 39, 3, 2, 2, 2)] // 2 blocks with this character
		[TestCase("sp1", "Buck", "Peter", 39, 3, 2, 1, 2)] // 2 blocks with this character (1 skipped)
		[TestCase("sp2", "David", "Peter", 40, 4, 1, 1, 1)] // 1 block with this character
		public void RestrictToCharactersChapters(string which, string actor, string character, int book, int chapter, int scriptBlockCount, int unskippedBlockCount, int transVerseCount)
		{
			var sp = (which == "sp1") ? _sp1 : _sp2;
			sp.RestrictToCharacters(actor, character);
			Assert.That(sp.GetScriptBlockCount(book, chapter), Is.EqualTo(scriptBlockCount));
			Assert.That(sp.GetUnskippedScriptBlockCount(book, chapter), Is.EqualTo(unskippedBlockCount));
			Assert.That(sp.GetTranslatedVerseCount(book, chapter), Is.EqualTo(transVerseCount));
			sp.RestrictToCharacters(null, null);
		}

		[TestCase("sp1", "David", "book title or chapter (MAT)", 1, 1)]
		[TestCase("sp1", "Buck", "John the Baptist", 39, 2)]
		[TestCase("sp1", "Buck", "Peter", 39, 2)]
		[TestCase("sp2", "David", "Peter", 40, 1)]
		public void RestrictToCharactersBooks(string which, string actor, string character, int book, int scriptBlockCount)
		{
			var sp = (which == "sp1") ? _sp1 : _sp2;
			sp.RestrictToCharacters(actor, character);
			Assert.That(sp.GetScriptBlockCount(book), Is.EqualTo(scriptBlockCount));
			sp.RestrictToCharacters(null, null);
		}

		[TestCase("sp1", "David", "book title or chapter (MAT)", 1, 0, 0, true)]
		[TestCase("sp1", "David", "Peter", 1, 0, 0, false)] // wrong character
		[TestCase("sp1", "Buck", "book title or chapter (MAT)", 1, 0, 0, false)] // wrong actor
		[TestCase("sp1", "Buck", "John the Baptist", 39, 3, 2, true)]
		public void IsBlockInCharacter(string which, string actor, string character, int book, int chapter, int block, bool expected)
		{
			var sp = (which == "sp1") ? _sp1 : _sp2;
			sp.RestrictToCharacters(actor, character);
			Assert.That(sp.IsBlockInCharacter(book, chapter, block), Is.EqualTo(expected));
			sp.RestrictToCharacters(null, null);
		}

		[Test]
		public void IsBlockInCharacter_NoActor()
		{
			Assert.That(_sp1.IsBlockInCharacter(1, 0, 0), Is.True);
			Assert.That(_sp1.IsBlockInCharacter(39, 3, 2), Is.True);
		}
	}
}
