using System;
using L10NSharp;

namespace HearThis.Script
{
	public class SampleScriptProvider : IScriptProvider
	{
		private BibleStats _stats;

		public SampleScriptProvider()
		{
			_stats = new BibleStats();
		}
		public ScriptLine GetLine(int bookNumber, int chapterNumber, int lineNumber0Based)
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
						LineNumber = lineNumber0Based + 1,
						Text =line,
						FontName = "Arial",
						FontSize = 12
					};
		}

		public int GetScriptLineCount(int book, int chapter1Based)
		{
			if(chapter1Based ==0)//introduction
				return 0;

			return _stats.GetPossibleVersesInChapter(book, chapter1Based);
		}

		public int GetTranslatedVerseCount(int bookNumberDelegateSafe, int chapterNumber1Based)
		{
			return 1;
		}

		public int GetScriptLineCount(int bookNumber)
		{
			return _stats.GetChaptersInBook(bookNumber) * 10;
		}

		public void LoadBook(int bookNumber0Based)
		{

		}

		public string EthnologueCode { get { return "KAL"; } }

		public int GetScriptLineCountFromLastParagraph(int bookNumber, int chapterNumber1Based)
		{
			// Since the SampleScriptProvider didn't have the bug that this property is designed to deal with
			// there's no need to implement it.
			throw new NotImplementedException();
		}
	}
}