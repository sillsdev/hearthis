using System.Collections.Generic;
using System.Linq;
using L10NSharp;

namespace HearThis.Script
{
	public class SampleScriptProvider : ScriptProviderBase
	{
		public const string kProjectUiName = "Sample";
		public const string kProjectFolderName = "sample";
		private readonly BibleStats _stats;

		public override string ProjectFolderName
		{
			get { return kProjectFolderName; }
		}

		public SampleScriptProvider()
		{
			_stats = new BibleStats();
			LoadSkipInfo();
		}

		public override ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			string line;
			if (lineNumber0Based == 0)
				line = _stats.GetBookName(bookNumber) + LocalizationManager.GetString("Sample.Chapter", " Chapter ", "Only for sample data") + chapterNumber;
			else
			{
				line =  LocalizationManager.GetString("Sample.WouldBeSentence", "Here if we were using a real project, there would be a sentence for you to read.", "Only for sample data");

				if (chapterNumber == 1)
				{
					if (lineNumber0Based == 1)
						line = LocalizationManager.GetString("Sample.Introductory", "Some introductory material about ", "Only for sample data") + _stats.GetBookName(bookNumber);
				}
			}

			return new ScriptLine()
					{
						Number = lineNumber0Based + 1,
						Text =line,
						FontName = "Arial",
						FontSize = 12
					};
		}

		public override int GetScriptBlockCount(int bookNumber, int chapter1Based)
		{
			if (chapter1Based == 0)//introduction
				return 0;

			return _stats.GetPossibleVersesInChapter(bookNumber, chapter1Based);
		}

		public override int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based)
		{
			return GetScriptLines(bookNumber, chapter1Based).Count(s => s.Skipped);
		}

		public override int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based)
		{
			return GetScriptLines(bookNumber, chapter1Based).Count(s => !s.Skipped);
		}

		private List<ScriptLine> GetScriptLines(int bookNumber, int chapter1Based)
		{
			List<ScriptLine> lines = new List<ScriptLine>();
			for (int i = 0; i < GetScriptBlockCount(bookNumber, chapter1Based); i++)
			{
				lines.Add(GetBlock(bookNumber, chapter1Based, i));
				PopulateSkippedFlag(bookNumber, chapter1Based, lines);
			}
			return lines;
		}

		public override int GetTranslatedVerseCount(int bookNumberDelegateSafe, int chapterNumber1Based)
		{
			return 1;
		}

		public override int GetScriptBlockCount(int bookNumber)
		{
			return _stats.GetChaptersInBook(bookNumber) * 10;
		}

		public override void LoadBook(int bookNumber0Based)
		{

		}

		public override string EthnologueCode { get { return "KAL"; } }
	}
}