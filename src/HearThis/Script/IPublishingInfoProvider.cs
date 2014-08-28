using HearThis.Script;

namespace HearThis.Publishing
{
	public interface IPublishingInfoProvider
	{
		string Name { get; }
		string EthnologueCode { get; }
		string CurrentBookName { get; }
		ScriptLine GetBlock(string bookName, int chapterNumber, int lineNumber0Based);
		IBibleStats VersificationInfo { get; }
	}
}
