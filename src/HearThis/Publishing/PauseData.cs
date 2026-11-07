using System.ComponentModel;
using System.Xml.Serialization;

namespace HearThis.Publishing
{
	[XmlRoot(ElementName= "PauseData")]
	public class PauseData
	{
		private const double kDefaultMin = 0.1;
		private const double kDefaultMax = 0.1;

		/// <summary>
		/// Required for serialization
		/// </summary>
		public PauseData()
		{
			Min = kDefaultMin;
			Max = kDefaultMax;
		}
		
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
		[DefaultValue(kDefaultMin)]
		public double Min { get; set; }

		[XmlAttribute("max")]
		[DefaultValue(kDefaultMax)]
		public double Max { get; set; }
	}
}
