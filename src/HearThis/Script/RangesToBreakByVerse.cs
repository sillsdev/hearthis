// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International. All Rights Reserved.
// <copyright from='2023' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using SIL.Scripture;

namespace HearThis.Script
{
	[XmlRoot(ElementName="RangesToBreakByVerse")]
	public class RangesToBreakByVerse {
		[XmlElement(ElementName="ScriptureRange")]
		public List<ScriptureRange> ScriptureRanges { get; set; }
		
		public bool Includes(BCVRef verse) =>
			ScriptureRanges != null &&
			ScriptureRanges.Any(r => r.StartRef <= verse && r.EndRef >= verse);
	}
}
