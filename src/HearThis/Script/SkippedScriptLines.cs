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
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using DesktopAnalytics;
using SIL.Xml;
using System.IO;
using System.Linq;
using System.Text;

namespace HearThis.Script
{
	[Serializable]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class SkippedScriptLines
	{
		public List<string> SkippedParagraphStyles;
		public List<ScriptLineIdentifier> SkippedLinesList;


		/// <summary>
		/// Use this instead of the default constructor to instantiate an instance of this class
		/// </summary>
		public static SkippedScriptLines Create(string filePath)
		{
			if (File.Exists(filePath))
			{
				try
				{
					return XmlSerializationHelper.DeserializeFromFile<SkippedScriptLines>(filePath);
				}
				catch (Exception e)
				{
					Analytics.ReportException(e);
					Debug.Fail(e.Message);
				}
			}

			return new SkippedScriptLines
			{
				SkippedParagraphStyles = new List<string>(),
				SkippedLinesList = new List<ScriptLineIdentifier>(),
			};
		}

		public static SkippedScriptLines Create(byte[] data)
		{
			try
			{
				return XmlSerializationHelper.DeserializeFromString<SkippedScriptLines>(Encoding.UTF8.GetString(data));
			}
			catch (Exception e)
			{
				Analytics.ReportException(e);
				Debug.Fail(e.Message);
			}
			return new SkippedScriptLines
			{
				SkippedParagraphStyles = new List<string>(),
				SkippedLinesList = new List<ScriptLineIdentifier>(),
			};
		}

		public ScriptLineIdentifier GetLine(int bookNumber, int chapNumber, int lineNumber)
		{
			return SkippedLinesList.FirstOrDefault(l =>
				l.BookNumber == bookNumber && l.ChapterNumber == chapNumber && l.LineNumber == lineNumber);
		}
	}

	[Serializable]
	public class ScriptLineIdentifier
	{
		/// <summary>
		/// Default constructor needed for XML deserialization
		/// </summary>
		public ScriptLineIdentifier()
		{
		}

		public ScriptLineIdentifier(int book, int chapter, ScriptLine scriptBlock)
		{
			BookNumber = book;
			ChapterNumber = chapter;
			LineNumber = scriptBlock.Number;
			Verse = scriptBlock.Verse;
			Text = scriptBlock.Text;
		}

		public int BookNumber { get; set; }
		public int ChapterNumber { get; set; }
		public int LineNumber { get; set; }
		// The following are not part of the "key" but are stored because, if they don't match, we
		// want to discard this skip info record and re-instate the text rather than taking the risk
		// of skipping something that might be needed.
		public string Text { get; set; }
		public string Verse { get; set; }

		public bool IsSameLine(ScriptLineIdentifier other)
		{
			return other.BookNumber == BookNumber && other.ChapterNumber == ChapterNumber && other.LineNumber == LineNumber;
		}
	}
}