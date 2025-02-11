// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2020-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.IO;

namespace HearThis.Publishing
{
	public class AudiBiblePublishingMethod : HierarchicalPublishingMethodBase
	{
		private readonly string _ethnologueCode;
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
			var folderName = $"{bookIndex}_{_ethnologueCode}_{bookAbbr}";
			string folderPath = Path.Combine(rootFolderPath, folderName);
			string fileName = folderName + "_" + chapterIndex;
			EnsureDirectory(folderPath);

			return Path.Combine(folderPath, fileName);
		}

		public override string RootDirectoryName => _encoder.FormatName;
	}
}
