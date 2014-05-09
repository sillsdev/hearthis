using System.Diagnostics;
using NUnit.Framework;
using Paratext;

namespace HearThisTests
{
	[TestFixture]
	public sealed class TypeToTestTests
	{
		[Test, Ignore("breaks on server, which has no Paratext")]
		public void JustWalkAround()
		{
			ScrTextCollection.Initialize();
			foreach (var text in ScrTextCollection.ScrTexts(false, false))
			{
				Debug.WriteLine(text);
				Debug.WriteLine(text.BooksPresentSet);
				Debug.WriteLine(text.GetVerseText(new VerseRef(1, 1, 1, text.Versification)));
				text.Versification.LastBook();
			}
		}

	}
}
