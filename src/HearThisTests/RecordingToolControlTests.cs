using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HearThis.Script;
using HearThis.UI;


namespace HearThisTests
{
	[TestFixture]
	class RecordingToolControlTests
	{
		[SetUp]
		public void CreateTestData()
		{
			var presenter = new HearThis.UI.RecordingToolPresenter();
		}
		[Test]
		public void PressNextButton_FirstOfSeveral()
		{

		}

		[Test]
		public void PressNextButton_LastOfSeveral()
		{

		}

	}
}
