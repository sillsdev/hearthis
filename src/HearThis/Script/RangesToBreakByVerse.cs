// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023-2025, SIL Global.
// <copyright from='2023' to='2025' company='SIL Global'>
//		Copyright (c) 2023-2025, SIL Global.
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
