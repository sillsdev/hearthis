using System.ComponentModel;
using System.Xml.Serialization;

namespace HearThis.Publishing
{
	[XmlRoot(ElementName= "PauseData")]
	public class PauseData
	{
		public PauseData(bool apply, double min, double max)
		{
			Apply = apply;
			Min = min;
			Max = max;
		}

		[XmlAttribute("apply")]
		[DefaultValue(false)]
		public bool Apply { get; set; }

		[XmlAttribute("min")]
		[DefaultValue(0.1)]
		public double Min { get; set; }

		[XmlAttribute("max")]
		[DefaultValue(0.7)]
		public double Max { get; set; }
	}
}
