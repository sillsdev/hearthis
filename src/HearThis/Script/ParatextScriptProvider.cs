using System;
using System.Collections.Generic;
using System.Diagnostics;
using Palaso.Code;
using Paratext;

namespace HearThis.Script
{
	public class ParatextScriptProvider : IScriptProvider
	{
		private readonly IScripture _paratextProject;
		private Dictionary<int, Dictionary<int, List<ScriptLine>>> _script; // book <chapter, lines>
		private Dictionary<int, int[]>  _chapterVerseCount;//book <chapter, verseCount>

		public ParatextScriptProvider(IScripture paratextProject)
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

		public void LoadBook(int bookNumber0Based)
		{
			if(_script.ContainsKey(bookNumber0Based))
			{
				return;//already loaded
			}
			// review: this slows loading down;
			// it was added because I occasionaly got an error accessing the _script, on the following line:
			// _script.Add(bookNumber0Based, bookScript);
			lock (_script)
			{
				var bookScript = new Dictionary<int, List<ScriptLine>>(); //chapter, lines
				_script.Add(bookNumber0Based, bookScript);

				var verseRef = new VerseRef(bookNumber0Based + 1, 1, 0 /*verse*/, _paratextProject.Versification);

				var tokens = _paratextProject.GetUsfmTokens(verseRef, false, true);
				var state = _paratextProject.CreateScrParserState(verseRef);
				var paragraph = new ParatextParagraph();
				var versesPerChapter = GetArrayForVersesPerChapter(bookNumber0Based);

				//Introductory lines, before the start of the chapter, will be in chapter 0
				int currentChapter1Based = 0;
				var chapterLines = GetNewChapterLines(bookNumber0Based, currentChapter1Based);

				var paragraphMarkersOfInterest =
					new List<string>(new string[] {"mt", "mt1", "mt2", "ip", "im", "ms", "imt", "s", "s1", "c", "p"});

				bool lookingForVerseText = false;
				string space = " ";

				for (int i = 0; i < tokens.Count; i++)
				{
					UsfmToken t = tokens[i];
					state.UpdateState(tokens, i);

					if (t.Marker == "v")
					{
						// don't be fooled by empty \v markers
						if (lookingForVerseText)
						{
							paragraph.Add(space);
							versesPerChapter[currentChapter1Based]++;
						}
						lookingForVerseText = true;
					}

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
							chapterLines.AddRange(paragraph.BreakIntoLines());
						}
						paragraph.StartNewParagraph(state);
						if (currentChapter1Based == 0)
							versesPerChapter[0]++; // this helps to show that there is some content in the intro
					}
					if (t.Marker == "c")
					{
						lookingForVerseText = false;
						var chapterString = t.Data[0].Trim();
						currentChapter1Based = int.Parse(chapterString);
						chapterLines = GetNewChapterLines(bookNumber0Based, currentChapter1Based);
					}

					if (!string.IsNullOrEmpty(tokens[i].Text))
					{
						paragraph.Add(tokens[i].Text.Trim());
						if (lookingForVerseText)
						{
							lookingForVerseText = false;
							versesPerChapter[currentChapter1Based]++;
						}
					}

					if (tokens[i].Marker == "c" && tokens[i].HasData)
					{
						paragraph.Add("Chapter " + tokens[i].Data[0]); //TODO: Localize
					}

				}
				// emit the last line
				if (paragraph.HasData)
				{
					chapterLines.AddRange(paragraph.BreakIntoLines());
				}
			}
		}

		private List<ScriptLine> GetNewChapterLines(int bookNumber1Based, int currentChapter1Based)
		{
			var chapterLines = new List<ScriptLine>();
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