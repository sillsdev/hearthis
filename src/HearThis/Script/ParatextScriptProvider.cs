using System;
using System.Collections.Generic;
using System.Diagnostics;
using Palaso.Code;
using Paratext;
using System.Linq;

namespace HearThis.Script
{
	public class ParatextScriptProvider : IScriptProvider
	{
		private readonly ScrText _paratextProject;
		private int _currentBook=-1;
		private int _currentChapter=-1;
		private ScriptLine[] _lines;

		public ParatextScriptProvider(ScrText paratextProject)
		{
			Guard.AgainstNull(paratextProject,"paratextProject");
			_paratextProject = paratextProject;
		}

		/// <summary>
		/// The "line" is a bit of script (Book name, chapter #, section headings, etc.)
		/// </summary>
		public ScriptLine GetLine(int bookNumber, int chapterNumber, int lineNumber)
		{
			LoadIfNecessary(bookNumber, chapterNumber);
			if (lineNumber >= _lines.Length)
				return null;

			return _lines[lineNumber];
//                _paratextProject.GetVerseText(
//                    new VerseRef(bookNumber + 1, chapterNumber, lineNumber, _paratextProject.Versification), true);
		}

		private void LoadIfNecessary(int bookNumber, int chapterNumber)
		{
			if (bookNumber != _currentBook || chapterNumber != _currentChapter)
			{
				_lines = GetScriptLines(bookNumber, chapterNumber).ToArray();
				_currentBook = bookNumber;
				_currentChapter = chapterNumber;
			}
		}

		public int GetLineCountForChapter(int bookNumber, int chapterNumber)
		{
			LoadIfNecessary(bookNumber, chapterNumber);
			return _lines.Length;
		}

		/// <summary>
		/// THis would need a lot of work... what we'd really like a percentage of verses that are non-empty.  And we could more rapidly do it for the whole book in one go.
		/// </summary>
		/// <param name="bookNumber"></param>
		/// <param name="chapterNumber"></param>
		/// <returns></returns>
		public bool HasVerses(int bookNumber, int chapterNumber)
		{
			var verseRef = new VerseRef(bookNumber + 1, chapterNumber, 0 /*verse*/, _paratextProject.Versification);
			var parser = new ScrParser(_paratextProject, true);
			List<UsfmToken> tokens = parser.GetUsfmTokens(verseRef, false, true);

			ScrParserState state = new ScrParserState(_paratextProject, verseRef);
			bool inTargetChapter = false;
			int verseCount = 0;

			for (int i = 0; i < tokens.Count; ++i)
			{
				if (tokens[i].Marker == "c")
				{
					if (inTargetChapter) //we're done with our current chapter
						break;
					if (tokens[i].Data[0].Trim() == chapterNumber.ToString())
					{
						inTargetChapter = true;
					}
					else
					{
						continue; //not to our chapter yet
					}
				}
				else if (chapterNumber != 1 && !inTargetChapter)
				{
					continue;
				}

				state.UpdateState(tokens, i);
				if (state.ParaTag != null && state.ParaTag.Marker != "v")
					++verseCount;
			}
			return verseCount > 0;
		}

//            if(lineNumber == 1 && bookNumber ==0 && lineNumber == 1)
//            {
//                foreach (var line in GetLines(bookNumber,chapterNumber))
//                {
//                    Debug.WriteLine(": " + line);
//                }
//            }

	  private IEnumerable<ScriptLine> GetScriptLines(int bookNumber, int chapterNumber)
		{
			var verseRef = new VerseRef(bookNumber + 1, chapterNumber, 0 /*verse*/, _paratextProject.Versification);
			//_paratextProject
			var parser = new ScrParser(_paratextProject, true);
			List<UsfmToken> tokens = parser.GetUsfmTokens(verseRef, false, true);
			if (tokens.Count == 0)
			{
				yield break;
			}

			var paragraphMarkersOfInterest = new List<string>(new string[] {"mt", "mt1", "mt2", "ip", "im", "ms", "imt", "s", "s1", "c", "p" });
			ScrParserState state = new ScrParserState(_paratextProject, verseRef);
			bool inTargetChapter = false;

			ParatextParagraph paragraph = new ParatextParagraph();


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
				{
					if (paragraph.HasData)
					{
						foreach(ScriptLine line in paragraph.BreakIntoLines())
						{
							yield return line;
						}
					}
					paragraph.StartNewParagraph(state);
				}

				if(!string.IsNullOrEmpty(tokens[i].Text))
				{
					 paragraph.Add(tokens[i].Text);
				}

				if (tokens[i].Marker == "c" && tokens[i].HasData)
				{
					paragraph.Add("Chapter " + tokens[i].Data[0]);//TODO: Localize
				}
			}

			// emit the last line
			if (paragraph.HasData)
			{
				foreach (ScriptLine line in paragraph.BreakIntoLines())
				{
					yield return line;
				}
			}
		}

	  private class ParatextParagraph
	  {
		  //this was unreliable as teh inner format stuff it apparently a reference, so it would change unintentionally
				//public ScrParserState State { get; private set; }
		  public ScrTag State { get; private set; }
		  public string text { get; private set; }

		  public bool HasData
		  {
			  get { return !string.IsNullOrEmpty(text); }
		  }

		  public void Add(string s)
		  {
			  text += s;
			  Debug.WriteLine("Add " + s + " : " + State.Marker + " bold=" + State.Bold + " center=" + State.JustificationType);
		  }

		  public void StartNewParagraph(ScrParserState scrParserState)
		  {
			  text = "";
			  State = scrParserState.ParaTag;
			  Debug.WriteLine("Start " + State.Marker + " bold=" + State.Bold + " center=" + State.JustificationType);
		  }

		  public IEnumerable<ScriptLine> BreakIntoLines()
		  {
			  //TODO: this doesn't really parse well enough... e.g. "hello." will leave the quote to the next line.
			  var separators = new char[] { '.', '?', '!' };
			  if (text.IndexOfAny(separators) > 0)
			  {

				  foreach (var sentence in text.Split(separators))
				  {
					  //todo: this will replace the actual punctuation marks with periods.
					  var trimSentence = sentence.Trim();
					  if (!string.IsNullOrEmpty(trimSentence))
					  {
						  trimSentence = trimSentence.Replace("<<", "“");
						  trimSentence = trimSentence.Replace(">>", "”");
						  trimSentence= trimSentence.TrimStart('”');
						 var x = GetScriptLine(trimSentence + ".");

						  yield return x;
					  }
				  }
			  }
			  else
			  {
				  yield return GetScriptLine(text);
			  }
		  }

		  private ScriptLine GetScriptLine(string s)
		  {
			  Debug.WriteLine("Emitting "+s+" bold="+State.Bold+" center="+State.JustificationType);
			  return new ScriptLine()
			  {
				  Text = s,
				  Bold = State.Bold,
				  Centered = State.JustificationType == ScrJustificationType.scCenter,
				  FontSize = State.FontSize,
				  FontName = State.Fontname
			  };
		  }

	  }
	}
}