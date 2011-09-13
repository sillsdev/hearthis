using System;
using System.Collections.Generic;
using Palaso.Code;
using Paratext;

namespace HearThis.Script
{
	public class ParatextScriptProvider : IScriptProvider
	{
		private readonly ScrText _paratextProject;
		private Dictionary<int, Dictionary<int, List<ScriptLine>>> _script; // book <chapter, lines>
		private Dictionary<int, int[]>  _chapterVerseCount;//book <chapter, verseCount>

		public ParatextScriptProvider(ScrText paratextProject)
		{
			Guard.AgainstNull(paratextProject,"paratextProject");
			_paratextProject = paratextProject;
			_chapterVerseCount = new Dictionary<int, int[]>();
			_script = new Dictionary<int, Dictionary<int, List<ScriptLine>>>();
		}

		/// <summary>
		/// The "line" is a bit of script (Book name, chapter #, section headings, etc.)
		/// </summary>
		public ScriptLine GetLine(int bookNumber0Based, int chapterNumber, int lineNumber)
		{
			try
			{
				if (!_script.ContainsKey(bookNumber0Based)
					|| !_script[bookNumber0Based].ContainsKey(chapterNumber)
					|| _script[bookNumber0Based][chapterNumber].Count - 1 < lineNumber)
					return null;
				return _script[bookNumber0Based][chapterNumber][lineNumber];
			}
			catch(Exception)
			{
				return null;
			}
		}

		public int GetScriptLineCount(int bookNumber0Based, int chapter1Based)
		{
			try
			{
				if (!_script.ContainsKey(bookNumber0Based)
					 || !_script[bookNumber0Based].ContainsKey(chapter1Based))
					return 0;
				return _script[bookNumber0Based][chapter1Based].Count;
			}
			catch(Exception)
			{
				return 0;
			}
		}


		public int GetScriptLineCount(int bookNumber0Based)
		{
			int r = 0;
			try
			{
				if (!_script.ContainsKey(bookNumber0Based))
					return 0;
				foreach (var chapter in _script[bookNumber0Based])
				{
					r += GetScriptLineCount(bookNumber0Based, chapter.Key);
				}
			}
			catch (Exception)
			{
				return 0;
			}
			return r;
		}


		public int GetTranslatedVerseCount(int bookNumber, int chapterNumber1Based)
		{
			try
			{
				if (_chapterVerseCount == null || !_chapterVerseCount.ContainsKey(bookNumber) || _chapterVerseCount[bookNumber].Length < chapterNumber1Based - 1)
					return 0;

				return _chapterVerseCount[bookNumber][chapterNumber1Based];
			}
			catch(Exception)
			{
				return 0;
			}
		}



//        public void LoadBible(Palaso.Progress.ProgressState progress)
//        {
//            progress.TotalNumberOfSteps = 67;
//            var parser = new ScrParser(_paratextProject, true);
//            for (int bookNumber0Based = 0; bookNumber0Based < 66; bookNumber0Based++)
//            {
//                LoadBook(parser, bookNumber0Based, progress);
//                progress.NumberOfStepsCompleted++;
//            }
//        }

		public void LoadBook(int bookNumber0Based)
		{
			if(_script.ContainsKey(bookNumber0Based))
			{
				return;//already loaded
			}
			var parser = new ScrParser(_paratextProject, true);

			Dictionary<int, List<ScriptLine>> bookScript = new Dictionary<int, List<ScriptLine>>();//chapter, lines
			_script.Add(bookNumber0Based, bookScript);

			var paragraphMarkersOfInterest =
				new List<string>(new string[] {"mt", "mt1", "mt2", "ip", "im", "ms", "imt", "s", "s1", "c", "p"});

			var verseRef = new VerseRef(bookNumber0Based+1, 1, 0 /*verse*/,
										_paratextProject.Versification);

			var tokens = parser.GetUsfmTokens(verseRef, false, true);
			ScrParserState state = new ScrParserState(_paratextProject, verseRef);
			ParatextParagraph paragraph = new ParatextParagraph();
			var versesPerChapter = GetArrayForVersesPerChapter(bookNumber0Based);

			//Introductory lines, before the start of the chapter, will be in chapter 0
			int currentChapter1Based = 0;
			var chapterLines = GetNewChapterLines(bookNumber0Based, currentChapter1Based);

			for (int i = 0; i < tokens.Count; i++)
			{
				UsfmToken t = tokens[i];
				if (t.Marker == "c")
				{
					var chapterString = t.Data[0].Trim();
					currentChapter1Based = int.Parse(chapterString);
					chapterLines = GetNewChapterLines(bookNumber0Based, currentChapter1Based);
				}
				state.UpdateState(tokens, i);

				if (t.Marker == "v") //todo: don't be fulled by empty \v markers
					versesPerChapter[currentChapter1Based]++;

				if (state.NoteTag != null)
					continue; // skip note text tokens
				if (state.CharTag != null && state.CharTag.Marker == "fig")
					continue; // skip figure tokens
				if (state.ParaTag != null && !paragraphMarkersOfInterest.Contains(state.ParaTag.Marker))
					continue; // skip any undesired paragraph types

				if (state.ParaStart)
				{
					if (paragraph.HasData)
					{
						chapterLines.AddRange((IEnumerable<ScriptLine>) paragraph.BreakIntoLines());
					}
					paragraph.StartNewParagraph(state);
					if (currentChapter1Based == 0)
						versesPerChapter[0]++;// this helps to show that there is some content in the intro
				}

				if (!string.IsNullOrEmpty(tokens[i].Text))
				{
					paragraph.Add(tokens[i].Text);
				}

				if (tokens[i].Marker == "c" && tokens[i].HasData)
				{
					paragraph.Add("Chapter " + tokens[i].Data[0]); //TODO: Localize
				}

			}
			// emit the last line
			if (paragraph.HasData)
			{
				chapterLines.AddRange((IEnumerable<ScriptLine>)paragraph.BreakIntoLines());
			}
		}

		private List<ScriptLine> GetNewChapterLines(int bookNumber1Based, int currentChapter1Based)
		{
			List<ScriptLine> chapterLines = new List<ScriptLine>();
			_script[bookNumber1Based][currentChapter1Based] = chapterLines;
			return chapterLines;
		}

		private int[] GetArrayForVersesPerChapter(int bookNumber1Based)
		{
			int[] versesPerChapter;
			if (!_chapterVerseCount.TryGetValue(bookNumber1Based, out versesPerChapter))
			{
				versesPerChapter = new int[200];
				_chapterVerseCount[bookNumber1Based] = versesPerChapter;
			}
			return versesPerChapter;
		}
	}
}