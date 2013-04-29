using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HearThis.Script;

namespace HearThisTests
{
	[TestFixture]
	public class ParatextScriptProviderTests
	{
		[Test]
		public void GetScriptLineCountOnUnloadedBookReturnsZero()
		{
			var psp = new ParatextScriptProvider(new ScriptureStub());
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(0));
		}

		[Test]
		public void LoadBookZero()
		{
			var psp = new ParatextScriptProvider(new ScriptureStub());
			psp.LoadBook(0); // load Genesis
			Assert.That(psp.GetScriptLineCount(0, 1), Is.EqualTo(3));
		}
	}
}
