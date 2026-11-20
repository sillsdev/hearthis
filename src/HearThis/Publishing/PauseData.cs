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

		/// <summary>
		/// The source of truth for each pause tier's shipped default Min/Max.
		/// </summary>
		/// <remarks>
		/// Designer literals must contain literal decimals and cannot reference this, so
		/// tests ensure they stay in sync.
		/// </remarks>
		public static class Defaults
		{
			public static PauseData Clip => new PauseData(false, 0.40, 0.75);
			public static PauseData Paragraph => new PauseData(false, 0.75, 0.90);
			public static PauseData Section => new PauseData(false, 1.15, 1.25);
			public static PauseData Chapter => new PauseData(false, 1.25, 2.00);
			public static PauseData PostHeading => new PauseData(false, 1.45, 1.55);
		}
	}
}
