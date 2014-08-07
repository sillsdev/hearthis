namespace HearThis.Script
{
	public interface ISkippedStyleInfoProvider
	{
		void SetSkippedStyle(string style, bool skipped);
		bool IsSkippedStyle(string style);
	}
}