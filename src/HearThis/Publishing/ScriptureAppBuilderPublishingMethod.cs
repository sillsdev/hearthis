using System.IO;

namespace HearThis.Publishing
{
	public class ScriptureAppBuilderPublishingMethod : BunchOfFilesPublishingMethod
	{
		private string _ethnologueCode;
		public ScriptureAppBuilderPublishingMethod(string ethnologueCode) : base(new LameEncoder())
		{
			_ethnologueCode = ethnologueCode.ToUpperInvariant();
		}

		public override string GetRootDirectoryName()
		{
			return "ScriptureAppBuilder";
		}

		public override string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			var bookNumber = _statistics.GetBookNumber(bookName);
			string bookIndex = (bookNumber + 1).ToString("000");
			var bookAbbr = _statistics.GetBookCode(bookNumber).ToUpperInvariant();
			string chapterIndex = chapterNumber.ToString("000");
			var folderName = string.Format("{0}-{1}", bookName, bookIndex);
			string folderPath = Path.Combine(rootFolderPath, folderName);
			string fileName = _ethnologueCode + "-" + bookAbbr + "-" + chapterIndex;
			EnsureDirectory(folderPath);

			return Path.Combine(folderPath, fileName);
		}
	}
}
