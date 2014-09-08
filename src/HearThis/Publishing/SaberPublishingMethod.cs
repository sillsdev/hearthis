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
	public class SaberPublishingMethod : HierarchicalPublishingMethodBase
	{
		public SaberPublishingMethod() : base(new LameEncoder())
		{
		}

		public override string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			string bookIndex = GetBookIndex(bookName);
			string chapterIndex = chapterNumber.ToString("000");
			string fileName = string.Format("{0}{1}{2} {3}", bookIndex, chapterIndex, bookName, chapterNumber);

			var path = GetFolderPath(rootFolderPath, bookName, bookIndex);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return Path.Combine(path, fileName);
		}

		public override string RootDirectoryName
		{
			get { return "Saber"; }
		}
	}
}