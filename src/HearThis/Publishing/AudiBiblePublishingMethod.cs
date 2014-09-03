// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.IO;

namespace HearThis.Publishing
{
	public class AudiBiblePublishingMethod : BunchOfFilesPublishingMethod
	{
		private string _ethnologueCode;
		public AudiBiblePublishingMethod(IAudioEncoder encoder, string ethnologueCode) : base(encoder)
		{
			_ethnologueCode = ethnologueCode.ToUpperInvariant();
		}

		public override string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			var bookNumber = _statistics.GetBookNumber(bookName);
			string bookIndex = bookNumber.ToString("00");
			var bookAbbr = _statistics.GetBookCode(bookNumber);
			string chapFormat = "00";
			if (bookName.ToLowerInvariant() == "psalms")
				chapFormat = "000";
			string chapterIndex = chapterNumber.ToString(chapFormat);
			var folderName = string.Format("{0}_{1}_{2}", bookIndex, _ethnologueCode, bookAbbr);
			string folderPath = Path.Combine(rootFolderPath, folderName);
			string fileName = folderName + "_" + chapterIndex;
			EnsureDirectory(folderPath);

			return Path.Combine(folderPath, fileName);
		}
	}
}
