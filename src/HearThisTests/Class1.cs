using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Paratext;

namespace HearThisTests
{
	[TestFixture]
	public sealed class TypeToTestTests
	{
		[Test]
		public void JustWalkAround()
		{
			ScrTextCollection.Initialize();
			foreach(var text in Paratext.ScrTextCollection.ScrTexts)
			{
				if(!text.IsResourceText)
				{
					Debug.WriteLine(text);
					Debug.WriteLine(text.BooksPresentSet);
					Debug.WriteLine(text.GetVerseText(new VerseRef(1, 1,1, text.Versification), true));
					text.Versification.LastBook();
				}
			}
		}

	}
}
