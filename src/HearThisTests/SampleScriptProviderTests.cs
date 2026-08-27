// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2026, SIL Global.
// <copyright from='2026' to='2026' company='SIL Global'>
//		Copyright (c) 2026, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Linq;
using HearThis.Script;
using L10NSharp;
using NUnit.Framework;
using SIL.Scripture;

namespace HearThisTests
{
	[TestFixture]
	public class SampleScriptProviderTests
	{
		private SampleScriptProvider _provider;
		private int _genesis;

		[OneTimeSetUp]
		public void MakeSample()
		{
			// SampleScriptProvider's constructor calls LocalizationManager.GetString;
			// relax strict mode so it falls back to the English text instead of
			// throwing when no LocalizationManager has been created (see the
			// analogous setup in ClipRepositoryTests.SetUpFixture).
			LocalizationManager.StrictInitializationMode = false;
			_provider = new SampleScriptProvider(suppressFullRefresh: true);
			_genesis = BCVRef.BookToNumber("GEN") - 1;
		}

		[Test]
		public void TracksParagraphStarts_ReturnsTrue()
		{
			Assert.That(_provider.TracksParagraphStarts, Is.True);
		}

		[Test]
		public void GetBlock_FirstBlockOfChapter_ParagraphStartIsTrue()
		{
			Assert.That(_provider.GetBlock(_genesis, 1, 0).ParagraphStart, Is.True);
		}

		[Test]
		public void GetBlock_ConsecutiveBlocks_ParagraphStartVaries()
		{
			var paragraphStartValues = Enumerable.Range(0, 20)
				.Select(i => _provider.GetBlock(_genesis, 1, i).ParagraphStart)
				.ToArray();

			// Since this provider simulates paragraph boundaries every 2-8 blocks
			// (rather than marking every block as a paragraph start), we should see
			// a genuine mix of true/false over a run of 20 consecutive blocks.
			Assert.That(paragraphStartValues, Has.Some.True);
			Assert.That(paragraphStartValues, Has.Some.False);
		}

		[Test]
		public void GetBlock_SameBlockRequestedTwice_ParagraphStartIsConsistent()
		{
			var first = _provider.GetBlock(_genesis, 1, 5).ParagraphStart;
			var second = _provider.GetBlock(_genesis, 1, 5).ParagraphStart;
			Assert.That(second, Is.EqualTo(first));
		}
	}
}
