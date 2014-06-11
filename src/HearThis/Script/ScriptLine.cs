using System;
using System.Xml.Serialization;

namespace HearThis.Script
{
	[Serializable]
	public class ScriptLine
	{
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
		public string Verse;
		public bool Heading;

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