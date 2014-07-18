using System;
using System.Xml.Serialization;

namespace HearThis.Script
{
	[Serializable]
	public class ScriptLine // This really should be called ScriptBlock, but it'll be a pain to migrate the XML files.
	{
		public const char kLineBreak = '\u2028';
		private bool _skipped;

		public event SkippedFlagChangedHandler OnSkippedChanged;
		public delegate void SkippedFlagChangedHandler(ScriptLine sender);

		[XmlElement("LineNumber")] // This really should be called Number, but it'll be a pain to migrate the XML files.
		public int Number;
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
		[XmlIgnore]
		public bool Skipped
		{
			get { return _skipped; }
			set
			{
				if (_skipped == value)
					return;
				_skipped = value;
				if (OnSkippedChanged == null)
					throw new Exception("Programming error: the OnSkippedChanged event must have a handler set before it is valid to set the Skipped flag.");
				OnSkippedChanged(this);
			}
		}

		public ScriptLine()
		{
		}

		public ScriptLine(string text)
		{
			Text = text;
			Number = 1;
		}
	}
}