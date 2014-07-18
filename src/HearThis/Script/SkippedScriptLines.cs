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
		private Dictionary<int, Dictionary<int, Dictionary<int, ScriptLineIdentifier>>> _skippedLines = new Dictionary<int, Dictionary<int, Dictionary<int, ScriptLineIdentifier>>>();
		private List<ScriptLineIdentifier> _skippedLineList;
		private string _filePath;

		/// <summary>
		/// This list is public to enable XML serialization, but use PopulateSkippedFlag and AddSkippedLine
		/// to keep things fast and error-free.
		/// </summary>
		public List<ScriptLineIdentifier> SkippedLinesList
		{
			get
			{
				if (_skippedLineList == null)
					_skippedLineList = new List<ScriptLineIdentifier>();
				else
					_skippedLineList.Clear();
				foreach (var book in _skippedLines.Keys)
				{
					foreach (var chapter in _skippedLines[book].Keys)
					{
						foreach (var line in _skippedLines[book][chapter].Keys)
						{
							_skippedLineList.Add(_skippedLines[book][chapter][line]);
						}
					}
				}
				return _skippedLineList;
			}
			set { _skippedLineList = value; }
		}


		/// <summary>
		/// Use this instead of the default constructor to instantiate an instance of this class
		/// </summary>
		/// <param name="book">Info about the book containing this chapter</param>
		/// <param name="chapterNumber1Based">[0] == intro, [1] == chapter 1, etc.</param>
		public static SkippedScriptLines Create(string filePath)
		{
			SkippedScriptLines deserializedClass = null;

			if (File.Exists(filePath))
			{
				try
				{
					deserializedClass = XmlSerializationHelper.DeserializeFromFile<SkippedScriptLines>(filePath);
					foreach (var skippedLine in deserializedClass._skippedLineList)
						deserializedClass.AddSkippedLine(skippedLine);
				}
				catch (Exception e)
				{
					Analytics.ReportException(e);
					Debug.Fail(e.Message);
				}
			}
			if (deserializedClass == null)
			{
				deserializedClass = new SkippedScriptLines();
				deserializedClass._skippedLines = new Dictionary<int, Dictionary<int, Dictionary<int, ScriptLineIdentifier>>>();
			}

			deserializedClass._filePath = filePath;

			return deserializedClass;
		}

		/// <summary>
		/// Given a list of script lines in a particular book and chapter, this method will
		/// set the Skipped flag set if at some point in the past, AddSkippedLine was
		/// called for it and the verse and text still match.
		/// </summary>
		/// <param name="book">1-based book number</param>
		/// <param name="chapter">1-based chapter number (where 0 represents the introduction)</param>
		/// <param name="scriptLines">Object representing the script line as provided by an
		/// IScriptProvider. This object's Skipped flag will (potentially) be modified by this
		/// method.</param>
		public void PopulateSkippedFlag(int book, int chapter, List<ScriptLine> scriptLines)
		{
			foreach (var scriptBlock in scriptLines)
				scriptBlock.Skipped = false;

			Dictionary<int, Dictionary<int, ScriptLineIdentifier>> chapters;
			if (_skippedLines.TryGetValue(book, out chapters))
			{
				Dictionary<int, ScriptLineIdentifier> lines;
				if (chapters.TryGetValue(chapter, out lines))
				{
					ScriptLineIdentifier id;
					foreach (var scriptBlock in scriptLines)
					{
						if (lines.TryGetValue(scriptBlock.Number, out id))
						{
							scriptBlock.Skipped = (id.Verse == scriptBlock.Verse && id.Text == scriptBlock.Text);
						}
					}
				}
			}
		}

		public void AddSkippedLine(int book, int chapter, ScriptLine scriptBlock)
		{
			Debug.Assert(scriptBlock.Skipped);
			AddSkippedLine(new ScriptLineIdentifier(book, chapter, scriptBlock));
		}

		private void AddSkippedLine(ScriptLineIdentifier skippedLine)
		{
			Dictionary<int, Dictionary<int, ScriptLineIdentifier>> chapters;
			if (!_skippedLines.TryGetValue(skippedLine.BookNumber, out chapters))
			{
				chapters = new Dictionary<int, Dictionary<int, ScriptLineIdentifier>>();
				_skippedLines[skippedLine.BookNumber] = chapters;
			}
			Dictionary<int, ScriptLineIdentifier> lines;
			if (!chapters.TryGetValue(skippedLine.ChapterNumber, out lines))
			{
				lines = new Dictionary<int, ScriptLineIdentifier>();
				chapters[skippedLine.ChapterNumber] = lines;
			}

			lines[skippedLine.LineNumber] = skippedLine;

			Save();
		}

		public void RemoveSkippedLine(int book, int chapter, ScriptLine scriptBlock)
		{
			Debug.Assert(!scriptBlock.Skipped);
			Dictionary<int, Dictionary<int, ScriptLineIdentifier>> chapters;
			if (!_skippedLines.TryGetValue(book, out chapters))
				throw new KeyNotFoundException("Attempting to remove skipped line for non-existent book: " + book);
			Dictionary<int, ScriptLineIdentifier> lines;
			if (!chapters.TryGetValue(chapter, out lines))
				throw new KeyNotFoundException("Attempting to remove skipped line for non-existent book: " + book);
			lines.Remove(scriptBlock.Number);

			Save();
		}

		private void Save()
		{
			if (_filePath != null) // This will be null when doing initial deserialization.
				XmlSerializationHelper.SerializeToFile(_filePath, this);
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