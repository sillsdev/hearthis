using System.Collections.Generic;

namespace HearThis.Script
{
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Class with information about the lines in a chapter
	/// </summary>
	/// ------------------------------------------------------------------------------------
	public class ChapterLines
	{
		private List<ScriptLine> _lines;
		public int NumberOfLinesInLastParagraph { get; private set; }

		public ChapterLines()
		{
			_lines = new List<ScriptLine>();
			NumberOfLinesInLastParagraph = 0;
		}

		public int Count { get {return _lines.Count ; } }

		public ScriptLine this[int x]
		{
			get { return _lines[x]; }
		}

		public void AddParagraphLines(IEnumerable<ScriptLine> lines)
		{
			var before = _lines.Count;
			_lines.AddRange(lines);
			NumberOfLinesInLastParagraph = _lines.Count - before;
		}
	}
}