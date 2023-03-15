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

		public void AddRange(BCVRef start, BCVRef end)
		{
			var newRange = new ScriptureRange { Start = start, End = end };
			if (ScriptureRanges == null)
			{
				ScriptureRanges = new List<ScriptureRange> { newRange };
				return;
			}

			int i = 0;
			bool foundOverlap = false;
			for (; i< ScriptureRanges.Count; i++)
			{
				var curr = ScriptureRanges[i];
				if (curr.End < start)
					continue;
				if (foundOverlap)
				{
					// Remove any subsequent ranges now covered by the expanded range, further
					// extending the end as needed
					if (ScriptureRanges[i - 1].ExpandIfOverlapping(curr))
					{
						ScriptureRanges.RemoveAt(i);
						i--;
					}
					else
						break;
				}
				else if (curr.ExpandIfOverlapping(newRange))
				{
					foundOverlap = true;
				}
				else if (curr.Start > end)
					break;
			}
			if (!foundOverlap)
			{
				ScriptureRanges.Insert(i, newRange);
			}
		}

		public bool Includes(BCVRef verse) =>
			ScriptureRanges != null &&
			ScriptureRanges.Any(r => r.StartRef <= verse && r.EndRef >= verse);
	}
}
