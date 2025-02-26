using System.Linq;
using HearThis.Script;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Paratext.Data.DBLServices;
using SIL.DblBundle;
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
				Assert.That(tokens.Select(t => t.ToString()), Is.EqualTo(new[]
				{
					"[id] MAT",
					"- Test Bundle Publication ",
					"[h] ",
					"Matthew ",
					"[toc1] ",
					"Matthew ",
					"[toc2] ",
					"Matthew ",
					"[toc3] ",
					"Mt ",
					"[mt1] ",
					"Matthew ",
					"[c] 1",
					"[s] ",
					"Section Header ",
					"[p] ",
					"[v] 1",
					"Verse One Text. ",
					"[v] 2",
					"Verse Two Text."
				}));
			}
			finally
			{
				RobustFile.Delete(path);
			}
		}
	}
}
