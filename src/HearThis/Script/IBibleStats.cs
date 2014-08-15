namespace HearThis.Script
{
	public interface IBibleStats
	{
		int BookCount { get; }
		/// <summary>Gets the 0-based book index, given the English name</summary>
		int GetBookNumber(string bookName);
		string GetBookCode(int bookNumber0Based);
		/// <summary>Gets the English name of the book</summary>
		string GetBookName(int bookNumber0Based);
		int GetChaptersInBook(int bookNumber0Based);
	}
}