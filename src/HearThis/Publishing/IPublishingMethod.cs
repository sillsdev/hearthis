// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using SIL.Progress;
using System.Collections.Generic;

namespace HearThis.Publishing
{
	public interface IPublishingMethod
	{
		void DeleteExistingPublishedFiles(string rootFolderPath, string bookName);
		string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber);
		string RootDirectoryName { get; }
		IEnumerable<string> GetFinalInformationalMessages(PublishingModel model);
		void PublishChapter(string rootPath, string bookName, int chapterNumber, string pathToIncomingChapterWav, IProgress progress, PublishingModel publishingModel = null);
	}
}
