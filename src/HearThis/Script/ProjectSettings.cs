// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2016, SIL International. All Rights Reserved.
// <copyright from='2016' to='2016' company='SIL International'>
//		Copyright (c) 2016, SIL International. All Rights Reserved.
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
	[XmlRoot(ElementName = "ProjectInfo", Namespace = "", IsNullable = false)]
	public class ProjectSettings
	{
		[XmlAttribute("version")]
		public int Version { get; set; }

		[XmlAttribute("breakAtParagraphBreaks")]
		public bool BreakAtParagraphBreaks { get; set; }
	}
}