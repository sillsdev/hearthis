// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020-2025, SIL Global.
// <copyright from='2020' to='2025' company='SIL Global'>
//		Copyright (c) 2020-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Xml.Serialization;
using System.Collections.Generic;

namespace HearThis.Script
{
	public enum SampleRecordingType
	{
		MatchingText,
		NotMatchingText,
		BeyondScriptExtent,
		ChapterAnnouncement,
	}

	[XmlRoot(ElementName = "Recordings")]
	public class Recordings
	{
		[XmlElement(ElementName = "Book")]
		public List<Book> Books { get; set; }
	}

	[XmlRoot(ElementName = "Book")]
	public class Book
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "Chapter")]
		public  List<Chapter> Chapters { get; set; }
	}

	[XmlRoot(ElementName = "Chapter")]
	public class Chapter
	{
		[XmlAttribute(AttributeName = "number")]
		public int Number { get; set; }

		[XmlElement(ElementName = "Recording")]
		public List<Recording> Recordings { get; set; }
	}

	[XmlRoot(ElementName = "Recording")]
	public class Recording
	{
		[XmlAttribute(AttributeName = "type")]
		public SampleRecordingType Type { get; set; }

		[XmlAttribute(AttributeName = "block")]
		public int Block { get; set; }

		[XmlAttribute(AttributeName = "text")]
		public string Text { get; set; }

		[XmlAttribute(AttributeName = "omitInfo")]
		public bool OmitInfo { get; set; }
	}
}
