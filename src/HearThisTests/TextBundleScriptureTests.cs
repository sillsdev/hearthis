using System.Linq;
using HearThis.Script;
using NUnit.Framework;
using SIL.DblBundle.Tests.Text;
using SIL.DblBundle.Text;
using SIL.IO;
using SIL.Scripture;
using static System.String;

namespace HearThisTests
{
	[TestFixture]
	class TextBundleScriptureTests
	{
		[Test]
		public void GetUsfmTokens_Bundle_GetsExpectedTokens()
		{
			var bundle = TextBundleTests.CreateZippedTextBundleFromResources();
			var path = bundle.Path;
			try
			{
				IScripture scr = new TextBundleScripture(new TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage>(path));
				var tokens = scr.GetUsfmTokens(new VerseRef(040_001_001));
				Assert.AreEqual("[id] MAT\n- Test Bundle Publication \n[h] \nMatthew \n[toc1] \nMatthew \n[toc2] \nMatthew \n[toc3] \nMt \n[mt1] \nMatthew \n[c] 1\n[s] \nSection Header \n[p] \n[v] 1\nVerse One Text. \n[v] 2\nVerse Two Text.",
					Join("\n", tokens));
			}
			finally
			{
				RobustFile.Delete(path);
			}
		}
	}
}
