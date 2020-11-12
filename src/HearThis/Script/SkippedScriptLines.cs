// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
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
using HearThis.Properties;
using SIL.Reporting;

namespace HearThis.Script
{
	[Serializable]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class SkippedScriptLines
	{
		public List<string> SkippedParagraphStyles;
		public List<ScriptLineIdentifier> SkippedLinesList;
		private int _internalVersion;

		[XmlAttribute("version")]
		public int Version
		{
			get => Settings.Default.CurrentSkippedLinesVersion;
			set => _internalVersion = value;
		}

		/// <summary>
		/// Use this instead of the default constructor to instantiate an instance of this class
		/// </summary>
		public static SkippedScriptLines Create(string filePath, ISkippedStyleInfoProvider skippedStyleInfo)
		{
			if (File.Exists(filePath))
			{
				try
				{
					return XmlSerializationHelper.DeserializeFromFile<SkippedScriptLines>(filePath)
						.Migrate(skippedStyleInfo.StylesToSkipByDefault, filePath);
				}
				catch (Exception e)
				{
					Analytics.ReportException(e);
					Debug.Fail(e.Message);
				}
			}

			return Create(skippedStyleInfo.StylesToSkipByDefault);
		}

		public static SkippedScriptLines Create(byte[] data, IEnumerable<string> defaultSkippedStyles)
		{
			try
			{
				return XmlSerializationHelper.DeserializeFromString<SkippedScriptLines>(
					Encoding.UTF8.GetString(data)).Migrate(defaultSkippedStyles);
			}
			catch (Exception e)
			{
				Analytics.ReportException(e);
				Debug.Fail(e.Message);
			}

			return Create(defaultSkippedStyles);
		}

		private static SkippedScriptLines Create(IEnumerable<string> defaultSkippedStyles)
		{
			return new SkippedScriptLines
			{
				SkippedParagraphStyles = new List<string>(),
				SkippedLinesList = new List<ScriptLineIdentifier>(),
			}.Migrate(defaultSkippedStyles);
		}

		private SkippedScriptLines Migrate(IEnumerable<string> defaultSkippedStyles, string pathToSaveChanges = null)
		{
			var updated = false;
			while (_internalVersion < Settings.Default.CurrentSkippedLinesVersion)
			{
				switch (_internalVersion)
				{
					case 0:
						foreach (var style in defaultSkippedStyles)
						{
							if (!SkippedParagraphStyles.Contains(style))
								SkippedParagraphStyles.Add(style);
						}
						break;
				}

				_internalVersion++;
				updated = true;
			}

			if (updated && pathToSaveChanges != null)
			{
				try
				{
					XmlSerializationHelper.SerializeToFile(pathToSaveChanges, this);
				}
				catch (Exception e)
				{
					Logger.WriteError(e);
				}
			}

			return this;
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
