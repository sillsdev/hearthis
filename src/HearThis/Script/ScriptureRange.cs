// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International. All Rights Reserved.
// <copyright from='2023' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Xml.Serialization;
using SIL.Scripture;

namespace HearThis.Script
{
	[XmlRoot(ElementName="ScriptureRange")]
	public class ScriptureRange
	{
		private int _end;
		public BCVRef StartRef => new BCVRef(Start);
		public BCVRef EndRef => new BCVRef(End);

		[XmlAttribute(AttributeName="start")]
		public int Start { get; set; }

		[XmlAttribute(AttributeName = "end")]
		public int End
		{
			get => _end;
			set
			{
				if (value < Start)
				{
					throw new ArgumentOutOfRangeException(nameof(value),
						"End reference must be after start reference.");
				}
				_end = value;
			}
		}

		public bool ExpandIfOverlapping(ScriptureRange other)
		{
			if (End > other.Start && other.End > Start)
			{
				Start = Math.Min(Start, other.Start);
				End = Math.Max(End, other.End);
				return true;
			}
			return false;
		}
	}
}
