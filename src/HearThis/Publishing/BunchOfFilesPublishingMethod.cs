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
	public class BunchOfFilesPublishingMethod : PublishingMethodBase
	{
		private const string kFilenameFormat = "{0}{1}{2} {3}";

		public BunchOfFilesPublishingMethod(IAudioEncoder encoder) : base(encoder)
		{
		}

		public override void DeleteExistingPublishedFiles(string rootFolderPath, string bookName)
		{
			if (!Directory.Exists(rootFolderPath))
				return;

			string searchPattern = string.Format(kFilenameFormat, GetBookIndex(bookName), "*", bookName, "*");
			foreach (var file in Directory.GetFiles(rootFolderPath, searchPattern))
				File.Delete(file);
		}

		public override string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			string chapterIndex = chapterNumber.ToString("000");
			string fileName = string.Format(kFilenameFormat, GetBookIndex(bookName), chapterIndex, bookName, chapterNumber);

			EnsureDirectory(rootFolderPath);

			return Path.Combine(rootFolderPath, fileName);
		}

		public override string RootDirectoryName
		{
			get { return _encoder.FormatName; }
		}
	}
}