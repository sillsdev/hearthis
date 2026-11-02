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
using SIL.Reporting;
using System;

namespace HearThis.Publishing
{
	public abstract class PublishingMethodBase : IPublishingMethod
	{
		protected readonly BibleStats _statistics;
		protected readonly IAudioEncoder _encoder;
		private const string _FFmpegFolder = "FFmpeg";
		private readonly string _pathToFFMPEG;
		private bool _hadErrorReducingNoise;
		private bool _hadErrorNormalizingVolumeToStandard;

		protected PublishingMethodBase(IAudioEncoder encoder)
		{
			_statistics = new BibleStats();
			_encoder = encoder;

			MediaInfo.FFprobeFolder = GetDirectoryDistributedWithApplication(false, _FFmpegFolder);
			FFmpegRunner.FFmpegLocation = GetFileDistributedWithApplication(_FFmpegFolder, "ffmpeg.exe");
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

		public string PrepareChapterAudio(string pathToIncomingChapterWav, IProgress progress,
			PublishingModel publishingModel = null)
		{
			// Volume normalization happens entirely in FinalizeChapterAudio now, as a single
			// real two-pass loudnorm operation on the final, fully-assembled chapter audio.
			// An earlier version also ran a separate dual_mono-only pass here, but that
			// wasn't measuring or targeting anything -- just reusing a stray leftover filter
			// chain -- so it added no accuracy, only a second, uncoordinated loudnorm pass.
			if (publishingModel != null && publishingModel.ReduceNoise && !_hadErrorReducingNoise)
			{
				// create other temp folder and ensure it is empty
				string tempFolderPath = GetTempPath() + "post_temp";
				EnsureDirectory(tempFolderPath);
				foreach (var file in Directory.GetFiles(tempFolderPath))
					RobustFile.Delete(file);

				try
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
				catch (Exception e)
				{
					_hadErrorReducingNoise = true;
					var msg = LocalizationManager.GetString("ReduceNoise.Error",
						"Error when trying to Reduce Noise. Exception details in Logger");
					var msgException = $"{msg}:\n {e.Message}";
					Logger.WriteEvent(msgException);
					progress?.WriteWarning(msg);
				}
			}

			return pathToIncomingChapterWav;
		}

		public void FinalizeChapterAudio(string rootPath, string bookName, int chapterNumber,
			string preparedWavPath, IProgress progress, PublishingModel publishingModel = null)
		{
			if (publishingModel != null && publishingModel.NormalizeVolume && !_hadErrorNormalizingVolumeToStandard)
			{
				// create other temp folder and ensure it is empty
				string tempFolderPath = GetTempPath() + "post_temp";
				EnsureDirectory(tempFolderPath);
				foreach (var file in Directory.GetFiles(tempFolderPath))
					RobustFile.Delete(file);

				try
				{
					// move current wav file
					string tempPath = tempFolderPath + "\\joined.wav";
					File.Move(preparedWavPath, tempPath);
					File.Delete(preparedWavPath);

					// normalize volume of the merged chapter audio file to the industry standard
					StandardNormalizeVolume(tempPath, preparedWavPath, progress);

					// delete temp file
					File.Delete(tempPath);
				}
				catch (Exception e)
				{
					_hadErrorNormalizingVolumeToStandard = true;
					var msg = LocalizationManager.GetString("NormalizeVolumeStandard.Error",
						"Error when trying to apply Standard Volume Normalization. Exception details in Logger");
					var msgException = $"{msg}:\n {e.Message}";
					Logger.WriteEvent(msgException);
					progress?.WriteWarning(msg);
				}
			}

			var outputPath = GetFilePathWithoutExtension(rootPath, bookName, chapterNumber);
			_encoder.Encode(preparedWavPath, outputPath, progress, ChapterTimeoutInSeconds);
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

		public void ReduceNoise(string sourcePath, string destPath, IProgress progress, int timeoutInSeconds = 600)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("ReduceNoise.Progress", "Reducing Noise in Audio File", "Appears in progress indicator"));

			ClipRepository.ReduceNoise(sourcePath, destPath, progress);
		}

		#region Volume Normalization (real two-pass loudnorm)

		// I=-16 LUFS integrated, LRA=7 LU loudness range, TP=-1 dBTP true peak: industry-
		// standard targets for spoken-word audio. dual_mono=true corrects a known loudnorm
		// quirk that otherwise over-measures true mono content (HearThis clips are always
		// mono) by several LU.
		private const string kLoudnormTargetArgs = "I=-16:LRA=7:TP=-1:dual_mono=true";

		protected void StandardNormalizeVolume(string sourcePath, string destPath, IProgress progress, int timeoutInSeconds = 600)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("NormalizeVolumeStandard.Progress", "Normalizing Volume of Audio File to Industry Standard", "Appears in progress indicator"));

			// Pass 1 (measure): analysis only, produces no output file. ffmpeg reports the
			// actual measured loudness as a JSON block on stderr.
			var measureResult = ClipRepository.RunCommandLine(progress, _pathToFFMPEG,
				BuildLoudnormMeasureArguments(sourcePath), timeoutInSeconds);
			var measureOutput = measureResult.StandardError + measureResult.StandardOutput;

			if (ClipRepository.TryParseLoudnormStats(measureOutput, out var stats))
			{
				// Pass 2 (apply): feeds pass 1's exact measured values back in with
				// linear=true -- real two-pass loudnorm, which reliably hits the target. A
				// single dynamic-mode pass (the previous implementation) estimates gain
				// on the fly without ever seeing the whole file's statistics, and can be off
				// by a full LU or more on real speech content.
				ClipRepository.RunCommandLine(progress, _pathToFFMPEG,
					BuildLoudnormApplyArguments(sourcePath, destPath, stats), timeoutInSeconds);
			}
			else
			{
				// Audio too short or too quiet for loudnorm to produce a usable measurement
				// (e.g. a one-word chapter; ffmpeg reports "-inf" in that case) -- fall back
				// to a single dynamic-mode pass rather than fail the whole chapter's publish
				// over an unmeasurable edge case.
				ClipRepository.RunCommandLine(progress, _pathToFFMPEG,
					BuildLoudnormDynamicArguments(sourcePath, destPath), timeoutInSeconds);
			}
		}

		internal static string BuildLoudnormMeasureArguments(string sourcePath) =>
			$@"-i ""{sourcePath}"" -af loudnorm={kLoudnormTargetArgs}:print_format=json -f null -";

		internal static string BuildLoudnormApplyArguments(string sourcePath, string destPath,
			(string measuredI, string measuredLra, string measuredTp, string measuredThresh, string targetOffset) stats) =>
			$@"-i ""{sourcePath}"" -af loudnorm={kLoudnormTargetArgs}:measured_I={stats.measuredI}:measured_LRA={stats.measuredLra}:measured_TP={stats.measuredTp}:measured_thresh={stats.measuredThresh}:offset={stats.targetOffset}:linear=true ""{destPath}""";

		internal static string BuildLoudnormDynamicArguments(string sourcePath, string destPath) =>
			$@"-i ""{sourcePath}"" -af loudnorm={kLoudnormTargetArgs} ""{destPath}""";

		#endregion
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
