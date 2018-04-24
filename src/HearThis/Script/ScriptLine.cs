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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace HearThis.Script
{
	[Serializable]
	public class ScriptLine // This really should be called ScriptBlock, but it'll be a pain to migrate the XML files.
	{
		public const char kLineBreak = '\u2028';
		private bool _skipped;
		private string _headingType;
		private List<int> _verseOffsets;

		public event ScriptBlockChangedHandler OnSkippedChanged;
		public delegate void ScriptBlockChangedHandler(ScriptLine sender);

		public static ISkippedStyleInfoProvider SkippedStyleInfoProvider { get; internal set; }

		[XmlElement("LineNumber")] // This really should be called Number, but it'll be a pain to migrate the XML files.
		public int Number;
		public string Text;

		/// <summary>
		/// The actor who made the current recording for this block.
		/// Null if unrecorded, or not made using a multi-voice script provider, or recorded before we added this feature.
		/// </summary>
		public string Actor;

		/// <summary>
		/// The character that this block belongs to.
		/// Null if unrecorded, or not made using a multi-voice script provider, or recorded before we added this feature.
		/// </summary>
		public string Character;

		/// <summary>
		/// For multivoice recordings, the original (Glyssenscript) block number that this recording is part of.
		/// </summary>
		public string OriginalBlockNumber;

		/// <summary>
		/// The (UTC) time when the recording was made.
		/// If unrecorded, or recorded before we added this feature, it will be default(DateTime), a DateTime of
		/// Unspecified Kind with a value 1/1/0001 12:00:00 AM
		/// </summary>
		public DateTime RecordingTime;
		[XmlIgnore]
		public string ParagraphStyle;
		[XmlIgnore]
		public bool Bold;
		[XmlIgnore]
		public bool Centered;
		[XmlIgnore]
		public bool RightToLeft;
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
				{
					//TODO
					//made DEBUG only because the current version seems to have left SampleScriptProvider out of testing, and various things are broken
					//with it, including this.
					Debug.Fail("Programming error: the OnSkippedChanged event must have a handler set before it is valid to set the Skipped flag.");
				}
				else
				{
					OnSkippedChanged(this);
				}
			}
		}

		public IEnumerable<int> VerseOffsets
		{
			get { return _verseOffsets; }
		}

		public void AddVerseOffset(int offset)
		{
			if (_verseOffsets == null)
				_verseOffsets = new List<int>();
			else
			{
				if (_verseOffsets[_verseOffsets.Count - 1] > offset) // REVIEW >=
					throw new ArgumentException("Verse offsets must be added in ascending order.", "offset");
			}
			_verseOffsets.Add(offset);
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