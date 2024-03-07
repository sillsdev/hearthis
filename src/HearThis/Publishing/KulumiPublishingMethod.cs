using System;
using System.IO;
using System.Linq;
using SIL.IO;

namespace HearThis.Publishing
{
	public class KulumiPublishingMethod : SaberPublishingMethod
	{

		public KulumiPublishingMethod() : base()
		{
		}

		public override string RootDirectoryName
		{
			get { return "Kulumi"; }
		}
	}
}


