// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using L10NSharp;
using SIL.DblBundle;
using SIL.DblBundle.Text;

namespace HearThis.Script
{
	public class SampleScriptProvider : ScriptProviderBase, IProjectInfo
	{
		public const string kProjectUiName = "Sample";
		public const string kProjectFolderName = "sample";
		private readonly BibleStats _stats;
		private readonly List<string> _paragraphStyleNames;

		public override string ProjectFolderName
		{
			get { return kProjectFolderName; }
		}
		public override IEnumerable<string> AllEncounteredParagraphStyleNames
		{
			get { return _paragraphStyleNames; }
		}
		public override IBibleStats VersificationInfo
		{
			get { return _stats; }
		}

		public SampleScriptProvider()
		{
			_stats = new BibleStats();
			_paragraphStyleNames = new List<string>(3);
			_paragraphStyleNames.Add(LocalizationManager.GetString("Sample.ChapterStyleName", "Chapter", "Only for sample data"));
			_paragraphStyleNames.Add(LocalizationManager.GetString("Sample.IntroductionParagraphStyleName", "Introduction", "Only for sample data"));
			_paragraphStyleNames.Add(LocalizationManager.GetString("Sample.NormalParagraphtyleName", "Normal Paragraph", "Only for sample data"));
			Initialize();
		}

		public override ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			string line;
			int iStyle;
			if (lineNumber0Based == 0)
			{
				line = _stats.GetBookName(bookNumber) +
						LocalizationManager.GetString("Sample.Chapter", " Chapter ", "Only for sample data") + chapterNumber;
				iStyle = 0;
			}
			else
			{
				if (chapterNumber == 1 && lineNumber0Based == 1) // REVIEW: shouldn't this be 0 AND 0?
				{
					line = LocalizationManager.GetString("Sample.Introductory", "Some introductory material about ", "Only for sample data") +
						_stats.GetBookName(bookNumber);
					iStyle = 1;
				}
				else
				{
					line = LocalizationManager.GetString("Sample.WouldBeSentence",
						"Here if we were using a real project, there would be a sentence for you to read.", "Only for sample data");
					iStyle = 2;
				}
			}

			return new ScriptLine()
					{
						Number = lineNumber0Based + 1,
						Text =line,
						FontName = "Arial",
						FontSize = 12,
						ParagraphStyle = _paragraphStyleNames[iStyle],
						Verse = (lineNumber0Based+1).ToString()
					};
		}

		public override int GetScriptBlockCount(int bookNumber0Based, int chapter1Based)
		{
			if (chapter1Based == 0)//introduction
				return 0;

			return _stats.GetVersesInChapter(bookNumber0Based, chapter1Based);
		}

		public override int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based)
		{
			return GetScriptLines(bookNumber, chapter1Based).Count(s => s.Skipped);
		}

		public override int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based)
		{
			return GetScriptLines(bookNumber, chapter1Based).Count(s => !s.Skipped);
		}

		private IEnumerable<ScriptLine> GetScriptLines(int bookNumber, int chapter1Based)
		{
			List<ScriptLine> lines = new List<ScriptLine>();
			for (int i = 0; i < GetScriptBlockCount(bookNumber, chapter1Based); i++)
			{
				lines.Add(GetBlock(bookNumber, chapter1Based, i));
				PopulateSkippedFlag(bookNumber, chapter1Based, lines);
			}
			return lines;
		}

		public override int GetTranslatedVerseCount(int bookNumber0Based, int chapterNumber1Based)
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

		public override bool RightToLeft { get { return false; } }

		public override string FontName { get { return "Microsoft Sans Serif"; } }

		public string Name { get { return kProjectUiName; } }
		public string Id { get { return kProjectUiName; } }
		public DblMetadataLanguage Language { get { return new DblMetadataLanguage { Iso="en", Name="English"}; } }
	}
}