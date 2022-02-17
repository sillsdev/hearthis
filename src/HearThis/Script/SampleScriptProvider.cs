// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HearThis.Publishing;
using L10NSharp;
using SIL.DblBundle;
using SIL.DblBundle.Text;
using SIL.IO;
using SIL.Reporting;
using SIL.Scripture;
using SIL.Xml;

namespace HearThis.Script
{
	public class SampleScriptProvider : ScriptProviderBase, IProjectInfo
	{
		public const string kProjectUiName = "Sample";
		public const string kProjectFolderName = "sample";
		private readonly BibleStats _stats;
		private readonly List<string> _paragraphStyleNames;
		private bool _allowExtraScriptLines;

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
			_paragraphStyleNames.Add(LocalizationManager.GetString("Sample.NormalParagraphStyleName", "Normal Paragraph", "Only for sample data"));
			_paragraphStyleNames.Add(LocalizationManager.GetString("Sample.SectionHeadParagraphStyleName", "Section Head", "Only for sample data"));
			Initialize();
		}

		protected override void Initialize(Action preDataMigrationInitializer = null)
		{
			base.Initialize();
			// Changing the color scheme forces a restart, but in that case we don't want to
			// re-initialize the sample project because that would confuse the user.
			if (!Program.RestartedToChangeColorScheme)
			{
				try
				{
					CreateSampleRecordingsInfoAndProblems();
				}
				catch (Exception ex)
				{
					ErrorReport.ReportNonFatalExceptionWithMessage(ex,
						LocalizationManager.GetString("Sample.ErrorGeneratingData", "An error occurred setting up the sample project."));
				}
			}

			SetSkippedStyle(_paragraphStyleNames[3], true);
		}

		private void CreateSampleRecordingsInfoAndProblems()
		{
			_allowExtraScriptLines = true;

			var initializationInfo = XmlSerializationHelper.DeserializeFromString<Recordings>(Properties.Resources.SampleDataRecordngInfo);
			foreach (var book in initializationInfo.Books)
			{
				var bookNum = BCVRef.BookToNumber(book.Id) - 1;
				var bookInfo = new BookInfo(Name, bookNum, this);
				foreach (var chapter in book.Chapters)
				{
					ChapterInfo info = null;
					foreach (var recording in chapter.Recordings)
					{
						var scriptLine = GetBlock(bookNum, chapter.Number, recording.Block);
						var wavFileName = ClipRepository.GetPathToLineRecording(Name, bookInfo.Name, chapter.Number, recording.Block);
						using (var ms = new MemoryStream())
						{
							string wavStreamName = recording.Type == SampleRecordingType.ChapterAnnouncement ?
								"sample" + recording.Type + (bookNum == BCVRef.BookToNumber("PSA") - 1 ? "Psalm" : "Chapter") + chapter.Number :
								"sampleSentence" + recording.Type;
							Properties.Resources.ResourceManager.GetStream(wavStreamName).CopyTo(ms);
							using (var fs = new FileStream(wavFileName, FileMode.Create, FileAccess.Write))
								ms.WriteTo(fs);
						}

						var backupClipFileName = Path.ChangeExtension(wavFileName, ClipRepository.kBackupFileExtension);
						RobustFile.Delete(backupClipFileName);
						var skipClipFileName = Path.ChangeExtension(wavFileName, ClipRepository.kSkipFileExtension);
						RobustFile.Delete(skipClipFileName);

						if (recording.OmitInfo)
						{
							var infoFilePath = ChapterInfo.GetFilePath(bookInfo, chapter.Number);
							RobustFile.Delete(infoFilePath);
						}
						else
						{
							if (info == null)
							{
								info = ChapterInfo.Create(bookInfo, chapter.Number);
								info.DeletedRecordings = null;
							}

							if (!string.IsNullOrEmpty(recording.Text))
								scriptLine.Text = recording.Text;
							scriptLine.RecordingTime = DateTime.Parse("2019-10-29 13:23:10");
							info.OnScriptBlockRecorded(scriptLine);
						}
					}
				}
			}

			_allowExtraScriptLines = false;
		}

		// Nothing to do, sample script provider doesn't have cached script lines to update.
		public override void UpdateSkipInfo()
		{
		}

		private string NormalSampleTextLine => LocalizationManager.GetString("Sample.WouldBeSentence",
			"Here if we were using a real project, there would be a sentence for you to read.", "Only for sample data");

		public override ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based)
		{
			if (!_allowExtraScriptLines && lineNumber0Based >= GetScriptBlockCount(bookNumber, chapterNumber))
			{
				throw new ArgumentOutOfRangeException(nameof(lineNumber0Based), lineNumber0Based,
					"Sample script provider cannot supply the requested block.");
			}

			string line;
			int iStyle;
			if (chapterNumber == 0)
			{
				line = LocalizationManager.GetString("Sample.Introductory", "Some introductory material about ", "Only for sample data") +
					_stats.GetBookName(bookNumber);
				iStyle = 1;
			}
			else if (lineNumber0Based == 0)
			{
				if (bookNumber == BCVRef.BookToNumber("PSA") - 1)
				{
					line = String.Format(LocalizationManager.GetString("Sample.PsalmFormat", "Psalm {0}", "Only for sample data; Param 0: Psalm number"),
						chapterNumber);
				}
				else
				{
					line = String.Format(LocalizationManager.GetString("Sample.BookAndChapterFormat", "{0} Chapter {1}", "Only for sample data; Param 0: Book name; Param 1: Chapter number"),
						_stats.GetBookName(bookNumber), chapterNumber);
				}

				iStyle = 0;
			}
			else if (IsEphesiansChapter3(bookNumber, chapterNumber) && lineNumber0Based == 4)
			{
				// In Ephesians 3, we throw in a section head before the block representing verse 4
				// in order to illustrate an example of a block that uses a skipped style. Note that
				// this corresponds to an entry in SampleDataRecordingInfo.xml because we wanted to
				// be able to illustrate the problem case where a recording exists for a block that
				// is supposed to be skipped. Ideally, that XML file and the classes that process it
				// should be enhanced to allow for this special case to be represented fully rather
				// than hard-coding it in this class.
				line = LocalizationManager.GetString("Sample.SectionHeadInEph3", "The Mystery", "Only for sample data");
				iStyle = 3;
			}
			else
			{
				line = NormalSampleTextLine;
				iStyle = 2;
			}

			var scriptLine = new ScriptLine()
				{
					Number = lineNumber0Based + 1,
					Text = line,
					FontName = "Arial",
					FontSize = 12,
					ParagraphStyle = _paragraphStyleNames[iStyle],
					Heading = iStyle == 0 || iStyle == 3,
					Verse = chapterNumber > 0 ? (lineNumber0Based).ToString() : null,
				};
			if (ClipRepository.SkipFileExists(Name, _stats.GetBookName(bookNumber), chapterNumber, lineNumber0Based))
			{
				scriptLine.SkippedChanged += sl => { /* no-op */ };
				scriptLine.Skipped = true;
			}

			var bookInfo = new BookInfo(Name, bookNumber, this);
			if (File.Exists(ChapterInfo.GetFilePath(bookInfo, chapterNumber)))
			{
				var chapterInfo = ChapterInfo.Create(bookInfo, chapterNumber);
				if (chapterInfo.RecordingInfo.Count > lineNumber0Based)
				{
					var recordingInfo = chapterInfo.RecordingInfo[lineNumber0Based];
					scriptLine.RecordingTime = recordingInfo.RecordingTime;
					scriptLine.OriginalText = recordingInfo.OriginalText;
				}
			}

			return scriptLine;
		}

		public override int GetScriptBlockCount(int bookNumber0Based, int chapter1Based)
		{
			if (chapter1Based == 0)//introduction
				return 1;

			// For most chapters, we just want one "extra" block for the chapter number.
			// But for Ephesians 3, we throw in a section head in order to illustrate an
			// example of a block that uses a skipped style.
			var extra = IsEphesiansChapter3(bookNumber0Based, chapter1Based) ? 2 : 1;

			return _stats.GetVersesInChapter(bookNumber0Based, chapter1Based) + extra;
		}

		// In Ephesians 3, we throw in a section head in order to illustrate an
		// example of a block that uses a skipped style.
		private bool IsEphesiansChapter3(int bookNumber0Based, int chapterNumber) =>
			bookNumber0Based == BCVRef.BookToNumber("EPH") - 1 && chapterNumber == 3;

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
