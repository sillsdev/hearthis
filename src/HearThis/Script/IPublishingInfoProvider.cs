using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HearThis.Script;

namespace HearThis.Publishing
{
	public interface IPublishingInfoProvider
	{
		string Name { get; }
		string EthnologueCode { get; }
		ScriptLine GetBlock(string bookName, int chapterNumber, int lineNumber0Based);
	}
}
