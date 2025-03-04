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
using SIL.Media;
using static System.IO.Path;
using static SIL.IO.FileLocationUtilities;

namespace HearThis.Publishing
{
	public abstract class PublishingMethodBase : IPublishingMethod
	{
		protected readonly BibleStats _statistics;
		protected readonly IAudioEncoder _encoder;
		private const string _kFFmpegFolder = "FFmpeg";
		private readonly string _pathToFFMPEG;

		protected PublishingMethodBase(IAudioEncoder encoder)
		{
			_statistics = new BibleStats();
			_encoder = encoder;

			MediaInfo.FFprobeFolder = GetDirectoryDistributedWithApplication(false, _kFFmpegFolder);
			FFmpegRunner.FFmpegLocation = GetFileDistributedWithApplication(_kFFmpegFolder, "ffmpeg.exe");
			_pathToFFMPEG = FFmpegRunner.FFmpegLocation;
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
			if (publishingModel != null && (publishingModel.NormalizeVolume || publishingModel.ReduceNoise))
			{
				// create other temp folder and ensure it is empty
				string tempFolderPath = GetTempPath() + "post_temp";
				EnsureDirectory(tempFolderPath);
				foreach (var file in Directory.GetFiles(tempFolderPath))
					RobustFile.Delete(file);

				// normalize volume
				if (publishingModel.NormalizeVolume)
				{
					// move current wav file
					string tempPath = tempFolderPath + "\\joined.wav";
					File.Move(pathToIncomingChapterWav, tempPath);
					File.Delete(pathToIncomingChapterWav);

					// normalize volume of the merged chapter audio file
					NormalizeVolume(tempPath, pathToIncomingChapterWav, progress);

					// delete temp file
					File.Delete(tempPath);
				}

				// reduce noise
				if (publishingModel.ReduceNoise)
				{
					// move current wav file
					string tempPath = tempFolderPath + "\\joined.wav";
					File.Move(pathToIncomingChapterWav, tempPath);
					File.Delete(pathToIncomingChapterWav);

					// reduce the noise of the merged chapter audio file
					ReduceNoise(tempPath, pathToIncomingChapterWav, progress);

					// delete temp file
					File.Delete(tempPath);
				}

				// TODO: normalize duration of pauses (these might be moved somewhere else)
				if (publishingModel.SentencePause.apply)
				{
					// constrain pauses between sentences
				}
				if (publishingModel.ParagraphPause.apply)
				{
					// constrain pauses between paragraphs
				}
				if (publishingModel.SectionPause.apply)
				{
					// constrain pauses between sections
				}
				if (publishingModel.ChapterPause.apply)
				{
					// constrain pauses between chapters
				}

				// normalize volume a second time
				if (publishingModel.NormalizeVolume)
				{
					// move current wav file
					string tempPath = tempFolderPath + "\\joined.wav";
					File.Move(pathToIncomingChapterWav, tempPath);
					File.Delete(pathToIncomingChapterWav);

					// normalize volume of the merged chapter audio file a second time
					NormalizeVolume(tempPath, pathToIncomingChapterWav, progress);

					// delete temp file
					File.Delete(tempPath);
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

		protected void NormalizeVolume(string sourcePath, string destPath, IProgress progress, int timeoutInSeconds = 600)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("NormalizeVolume.Progress", "Normalizing Volume of Audio File", "Appears in progress indicator"));

			string arguments = string.Format($"-i {sourcePath} -af lowpass=5000,highpass=200,afftdn=nf=-25 {destPath}");
			ClipRepository.RunCommandLine(progress, _pathToFFMPEG, arguments, timeoutInSeconds);
		}

		protected void ReduceNoise(string sourcePath, string destPath, IProgress progress, int timeoutInSeconds = 600)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("ReduceNoise.Progress", "Reducing Noise in Audio File", "Appears in progress indicator"));

			string arguments = string.Format($"-i {sourcePath} -af loudnorm=dual_mono=true -ar 48k {destPath}");
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
