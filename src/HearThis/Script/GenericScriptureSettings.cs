using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearThis.Script
{
	/// <summary>
	/// This class is a general-purpose implementation of this interface.
	/// </summary>
	public class GenericScriptureSettings : IScrProjectSettings
	{
		public string FirstLevelStartQuotationMark { get; set; }
		public string FirstLevelEndQuotationMark { get; set; }
		public string SecondLevelStartQuotationMark { get; set; }
		public string SecondLevelEndQuotationMark { get; set; }
		public string ThirdLevelStartQuotationMark { get; set; }
		public string ThirdLevelEndQuotationMark { get; set; }
		public bool FirstLevelQuotesAreUnique { get; set; }
	}
}
