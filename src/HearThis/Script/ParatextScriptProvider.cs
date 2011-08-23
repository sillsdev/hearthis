using System;
using System.Collections.Generic;
using System.Diagnostics;
using Paratext;
using System.Linq;

namespace HearThis.Script
{
	public class ParatextScriptProvider : IScriptProvider
	{
		private readonly ScrText _paratextProject;
		private int _currentBook=-1;
		private int _currentChapter=-1;
		private string[] _lines;

		public ParatextScriptProvider(ScrText paratextProject)
		{
			_paratextProject = paratextProject;
		}

		/// <summary>
		/// The "line" is a bit of script (Book name, chapter #, section headings, etc.)
		/// </summary>
		public string GetLine(int bookNumber, int chapterNumber, int lineNumber)
		{
			LoadIfNecessary(bookNumber, chapterNumber);

			return _lines[lineNumber];
//                _paratextProject.GetVerseText(
//                    new VerseRef(bookNumber + 1, chapterNumber, lineNumber, _paratextProject.Versification), true);
		}

		private void LoadIfNecessary(int bookNumber, int chapterNumber)
		{
			if (bookNumber != _currentBook || chapterNumber != _currentChapter)
			{
				_lines = GetLineEnumeration(bookNumber, chapterNumber).ToArray();
				_currentBook = bookNumber;
				_currentChapter = chapterNumber;
			}
		}

		public int GetLineCountForChapter(int bookNumber, int chapterNumber)
		{
			LoadIfNecessary(bookNumber, chapterNumber);
			return _lines.Length;
		}

//            if(lineNumber == 1 && bookNumber ==0 && lineNumber == 1)
//            {
//                foreach (var line in GetLines(bookNumber,chapterNumber))
//                {
//                    Debug.WriteLine(": " + line);
//                }
//            }

		private IEnumerable<string> GetLineEnumeration(int bookNumber, int chapterNumber)
		{
			var verseRef = new VerseRef(bookNumber + 1, chapterNumber, 0 /*verse*/, _paratextProject.Versification);

			var parser = new ScrParser(_paratextProject, true);
			List<UsfmToken> tokens = parser.GetUsfmTokens(verseRef, false, true);

			var paragraphMarkersOfInterest = new List<string>(new string[] {"mt1", "mt2", "ip", "im", "ms", "imt", "s", "s1", "c", "p" });
			ScrParserState state = new ScrParserState(_paratextProject, verseRef);
			bool inTargetChapter = false;

			string bunch="";
			for (int i = 0; i < tokens.Count; ++i)
			{
				if (tokens[i].Marker == "c")
				{
					if (inTargetChapter) //we're done with our current chapter
						break;
					if(tokens[i].Data[0].Trim() == chapterNumber.ToString())
					{
						inTargetChapter = true;
					}
					else
					{
						continue; //not to our chapter yet
					}
				}
				else if(chapterNumber != 1 && !inTargetChapter)
				{
					continue;
				}

				state.UpdateState(tokens, i);

				if (state.NoteTag != null)
					continue; // skip note text tokens
				if (state.CharTag != null && state.CharTag.Marker == "fig")
					continue; // skip figure tokens
				if (state.ParaTag != null && !paragraphMarkersOfInterest.Contains(state.ParaTag.Marker))
					continue; // skip any undesired paragraph types

				if (state.ParaStart)
					if (!string.IsNullOrEmpty(bunch))
					{
						foreach(string line in BreakUpBunch(bunch))
						{
							yield return line;
						}
						bunch = "";
						//... do something at the start of a new paragraph

						//    ... tokens[i].Text is paragraph text you want to do something with
					}

				if(!string.IsNullOrEmpty(tokens[i].Text))
				{
					 bunch+= tokens[i].Text;
				}


				if (tokens[i].Marker == "c" && tokens[i].HasData)
				{
					bunch += "Chapter " + tokens[i].Data[0];//TODO: Localize
				}
			}

			// emit the last line
			if (!string.IsNullOrEmpty(bunch))
			{
				foreach (string line in BreakUpBunch(bunch))
				{
					yield return line;
				}
			}
		}

		private IEnumerable<string> BreakUpBunch(string bunch)
		{
			//TODO: this doesn't really parse well enough... e.g. "hello." will leave the quote to the next line.
			var separators = new char[]{'.','?','!'};
			if (bunch.IndexOfAny(separators) > 0)
			{

				foreach (var sentence in bunch.Split(separators))
				{
					//todo: this will replace the actual punctuation marks with periods.
					var trimSentence = sentence.Trim();
					if (!string.IsNullOrEmpty(trimSentence))
						yield return trimSentence + ".";
				}
			}
			else
			{
				yield return bunch;
			}
		}
	}
}