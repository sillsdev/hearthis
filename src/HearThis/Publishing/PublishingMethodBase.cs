// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using HearThis.Script;
using L10NSharp;
using SIL.IO;
using SIL.Progress;

namespace HearThis.Publishing
{
	public abstract class PublishingMethodBase : IPublishingMethod
	{
		protected readonly BibleStats _statistics;
		protected readonly IAudioEncoder _encoder;
		private readonly string _pathToFFMPEG;

		protected PublishingMethodBase(IAudioEncoder encoder)
		{
			_statistics = new BibleStats();
			_encoder = encoder;
			_pathToFFMPEG = FileLocationUtilities.GetFileDistributedWithApplication("FFmpeg", "ffmpeg.exe");
		}

		public abstract void DeleteExistingPublishedFiles(string rootFolderPath, string bookName);

		protected string GetBookIndex(string bookName)
		{
			return (1 + _statistics.GetBookNumber(bookName)).ToString("000");
		}

		public abstract string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber);

		public abstract string RootDirectoryName { get; }

		public virtual IEnumerable<string> GetFinalInformationalMessages(PublishingModel model)
		{
			yield return LocalizationManager.GetString("PublishDialog.Done", "Done");
		}

		public virtual int ChapterTimeoutInSeconds => 10 * 60;

		public void PublishChapter(string rootPath, string bookName, int chapterNumber, string pathToIncomingChapterWav,
			IProgress progress, PublishingModel publishingModel = null)
		{
			// Audio Post-Processing Functionality
			if (publishingModel != null)
			{
				if (publishingModel.NormalizeVolume)
				{
					// normalize volume of the merged chapter audio file
					NormalizeAudio(rootPath, rootPath, progress);
				}

				if (publishingModel.ReduceNoise)
				{
					// reduce the noise of the merged chapter audio file
					ReduceNoise(rootPath, rootPath, progress);
				}

				// TODO: normalize duration of pauses (these might be moved somewhere else)
				if (publishingModel.NormalizeVolume)
				{
					// constrain pauses between sentences
				}
				if (publishingModel.NormalizeVolume)
				{
					// constrain pauses between paragraphs
				}
				if (publishingModel.NormalizeVolume)
				{
					// constrain pauses between sections
				}
				if (publishingModel.NormalizeVolume)
				{
					// constrain pauses between chapters
				}
			}

			var outputPath = GetFilePathWithoutExtension(rootPath, bookName, chapterNumber);
			_encoder.Encode(pathToIncomingChapterWav, outputPath, progress, ChapterTimeoutInSeconds);
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

		protected void NormalizeAudio(string sourcePath, string destPath, IProgress progress, int timeoutInSeconds = 600)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("NormalizeAudio.Progress", "Normalizing Audio File", "Appears in progress indicator"));

			//-a down-mix to mono
			string arguments = string.Format($"-a \"{sourcePath}\" \"{destPath}\"");
			ClipRepository.RunCommandLine(progress, _pathToFFMPEG, arguments, timeoutInSeconds);
		}

		protected void ReduceNoise(string sourcePath, string destPath, IProgress progress, int timeoutInSeconds = 600)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("ReduceNoise.Progress", "Reducing Noise in Audio File", "Appears in progress indicator"));

			//-a down-mix to mono
			string arguments = string.Format($"-a \"{sourcePath}\" \"{destPath}\"");
			ClipRepository.RunCommandLine(progress, _pathToFFMPEG, arguments, timeoutInSeconds);
		}
	}

	public abstract class HierarchicalPublishingMethodBase : PublishingMethodBase
	{
		protected virtual string FolderFormat => "{0}{1}";

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
				RobustFile.Delete(file);
		}
	}
}
