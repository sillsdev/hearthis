// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Xml.Serialization;

namespace HearThis.Script
{
	[Serializable]
	public class ScriptLine // This really should be called ScriptBlock, but it'll be a pain to migrate the XML files.
	{
		public const char kLineBreak = '\u2028';
		private bool _skipped;
		private string _headingType;

		public event ScriptBlockChangedHandler OnSkippedChanged;
		public delegate void ScriptBlockChangedHandler(ScriptLine sender);

		public static ISkippedStyleInfoProvider SkippedStyleInfoProvider { get; internal set; }

		[XmlElement("LineNumber")] // This really should be called Number, but it'll be a pain to migrate the XML files.
		public int Number;
		public string Text;
		[XmlIgnore]
		public string ParagraphStyle;
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
			get { return _skipped || SkippedStyleInfoProvider.IsSkippedStyle(ParagraphStyle); }
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

		public string HeadingType
		{
			get { return Heading ? _headingType : null; }
			set { _headingType = value; }
		}

		public bool CrossesVerseBreak
		{
			get { return Verse != null && Verse.Contains("~"); }
		}

		public ScriptLine()
		{
		}

		public ScriptLine(string text)
		{
			Text = text;
			Number = 1;
		}

		public void SkipAllBlocksOfThisStyle(bool skipped)
		{
			SkippedStyleInfoProvider.SetSkippedStyle(ParagraphStyle, skipped);
		}
	}
}