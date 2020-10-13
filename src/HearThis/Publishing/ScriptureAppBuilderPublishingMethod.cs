// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.IO;

namespace HearThis.Publishing
{
	public class ScriptureAppBuilderPublishingMethod : HierarchicalPublishingMethodBase
	{
		private readonly string _ethnologueCode;
		public ScriptureAppBuilderPublishingMethod(string ethnologueCode) : base(new LameEncoder())
		{
			_ethnologueCode = ethnologueCode.ToUpperInvariant();
		}

		protected override string FolderFormat
		{
			get { return "{0}-{1}"; }
		}

		public override string RootDirectoryName
		{
			get { return "ScriptureAppBuilder"; }
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
