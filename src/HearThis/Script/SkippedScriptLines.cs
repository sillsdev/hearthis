using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using DesktopAnalytics;
using Palaso.Xml;
using System.IO;

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
		// of skipping something that mighht be needed.
		public string Text { get; set; }
		public string Verse { get; set; }
	}
}