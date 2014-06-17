using System;
using System.Xml.Serialization;

namespace HearThis.Script
{
	[Serializable]
	public class ScriptLine
	{
		public const char kLineBreak = '\u2028';
		public int LineNumber;
		public string Text;
		[XmlIgnore]
		public bool Bold;
		[XmlIgnore]
		public bool Centered;
		[XmlIgnore]
		public int FontSize;
		[XmlIgnore]
		public string FontName;
		[XmlIgnore]
		public bool ForceHardLineBreakSplitting;
		public string Verse;
		public bool Heading;
		public bool Skipped;

		public ScriptLine()
		{
		}

		public ScriptLine(string text)
		{
			Text = text;
			LineNumber = 1;
		}
	}
}