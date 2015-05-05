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
using HearThis.Script;
using SIL.Progress;

namespace HearThis.Publishing
{
	public abstract class PublishingMethodBase : IPublishingMethod
	{
		protected readonly BibleStats _statistics;
		protected readonly IAudioEncoder _encoder;

		public PublishingMethodBase(IAudioEncoder encoder)
		{
			_statistics = new BibleStats();
			_encoder = encoder;
		}

		public abstract void DeleteExistingPublishedFiles(string rootFolderPath, string bookName);

		protected string GetBookIndex(string bookName)
		{
			return (1 + _statistics.GetBookNumber(bookName)).ToString("000");
		}

		public abstract string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber);

		public abstract string RootDirectoryName { get; }

		public void PublishChapter(string rootPath, string bookName, int chapterNumber, string pathToIncomingChapterWav,
			IProgress progress)
		{
			var outputPath = GetFilePathWithoutExtension(rootPath, bookName, chapterNumber);
			_encoder.Encode(pathToIncomingChapterWav, outputPath, progress);
		}

		/// <summary>
		/// Virtual so we can override in tests
		/// </summary>
		/// <param name="path"></param>
		protected virtual void EnsureDirectory(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}
	}

	public abstract class HierarchicalPublishingMethodBase : PublishingMethodBase
	{
		protected virtual string FolderFormat
		{
			get { return "{0}{1}"; }
		}

		protected HierarchicalPublishingMethodBase(IAudioEncoder encoder) : base(encoder)
		{
		}

		protected string GetFolderPath(string rootFolderPath, string bookName)
		{
			return GetFolderPath(rootFolderPath, bookName, GetBookIndex(bookName));
		}

		protected string GetFolderPath(string rootFolderPath, string bookName, string bookIndex)
		{
			return Path.Combine(rootFolderPath, string.Format(FolderFormat, bookIndex, bookName));
		}

		public override void DeleteExistingPublishedFiles(string rootFolderPath, string bookName)
		{
			var path = GetFolderPath(rootFolderPath, bookName, GetBookIndex(bookName));
			if (!Directory.Exists(path))
				return;

			foreach (var file in Directory.GetFiles(path))
				File.Delete(file);
		}
	}
}
