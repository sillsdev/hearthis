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
using SIL.IO;

namespace HearThis.Publishing
{
	public class KulumiPublishingMethod : PublishingMethodBase
	{
		private const string kFilenameFormat = "{0}{1}{2} {3}";

		public KulumiPublishingMethod() : base(new LameEncoder())
		{
		}

		public override void DeleteExistingPublishedFiles(string rootFolderPath, string bookName)
		{
			if (!Directory.Exists(rootFolderPath))
				return;

			string searchPattern = string.Format(kFilenameFormat, GetBookIndex(bookName), "*", bookName, "*");
			foreach (var file in Directory.GetFiles(rootFolderPath, searchPattern))
				RobustFile.Delete(file);
		}

		public override string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			string chapterIndex = chapterNumber.ToString("000");
			string fileName = string.Format(kFilenameFormat, GetBookIndex(bookName), chapterIndex, bookName, chapterNumber);

			EnsureDirectory(rootFolderPath);

			return Path.Combine(rootFolderPath, fileName);
		}

		public override string RootDirectoryName => _encoder.FormatName;

	}
}
