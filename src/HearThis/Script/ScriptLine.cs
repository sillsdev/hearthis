// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
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
		private string _text;

		public event ScriptBlockChangedHandler SkippedChanged;
		public delegate void ScriptBlockChangedHandler(ScriptLine sender);

		public static ISkippedStyleInfoProvider SkippedStyleInfoProvider { get; internal set; }

		/// <summary>
		/// 1-based line number of block (i.e., line)
		/// </summary>
		[XmlElement("LineNumber")] // This really should be called Number, but it'll be a pain to migrate the XML files.
		public int Number;
		public string Text
		{
			get => _text;
			set
			{
				if (_text != null && value == null)
					throw new InvalidOperationException("Text cannot be cleared!");
				_text = value;
			}
		}

		/// <summary>
		/// In cases where the text is recorded but then later changed and the user "ignores"
		/// the difference (i.e., accepts that the existing recording is a valid representation
		/// of the current version of the text), the original recorded version of the text will
		/// be saved in this field in case the user later changes his mind and wants to
		/// un-ignore the problem.
		/// </summary>
		public string OriginalText;

		public string TextAsOriginallyRecorded => OriginalText ?? Text;

		/// <summary>
		/// The actor who recorded the current clip for this block.
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
		/// The (UTC) time when the clip was recorded.
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
				if (SkippedChanged == null)
				{
					//TODO
					//made DEBUG only because the current version seems to have left SampleScriptProvider out of testing, and various things are broken
					//with it, including this.
					Debug.Fail("Programming error: the OnSkippedChanged event must have a handler set before it is valid to set the Skipped flag.");
				}
				else
				{
					SkippedChanged(this);
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
			get => Heading ? _headingType : null;
			set => _headingType = value;
		}

		public bool CrossesVerseBreak => Verse != null && Verse.Contains("~");

		public int ApproximateWordCount => Text.Split(new [] {' ', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).Length;

		public ScriptLine()
		{
		}

		public ScriptLine(string text)
		{
			Text = text;
			Number = 1;
		}

		/// <summary>
		/// Gets a "clone" of this object, but with the OriginalText set instead of the Text.
		/// Note that this is not thread-safe!
		/// </summary>
		public ScriptLine GetAsDeleted()
		{
			ScriptLine clone = (ScriptLine)MemberwiseClone();
			clone.OriginalText = Text;
			clone._text = null;
			return clone;
		}

		public void SkipAllBlocksOfThisStyle(bool skipped)
		{
			SkippedStyleInfoProvider.SetSkippedStyle(ParagraphStyle, skipped);
		}
	}
}
