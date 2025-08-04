// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DesktopAnalytics;
using HearThis.Properties;
using L10NSharp;
using SIL.Progress;
using SIL.Reporting;
using static System.Environment;
using static System.String;
using static HearThis.SafeSettings;

namespace HearThis.Publishing
{
	public class PublishingModel
	{

		public enum VerseIndexFormatType
		{
			None,
			CueSheet,
			AudacityLabelFileVerseLevel,
			AudacityLabelFilePhraseLevel,
		}

		private readonly IPublishingInfoProvider _infoProvider;
		private readonly string _projectName;
		private string _audioFormat;
		private bool _publishOnlyCurrentBook;
		private bool _normalizeVolume;
		private bool _reduceNoise;
		private PauseData _sentencePause;
		private PauseData _paragraphPause;
		private PauseData _sectionPause;
		private PauseData _chapterPause;
		private bool _constrainPauseSentenceErrored = false;
		private bool _constrainPauseParagraghErrored = false;
		private bool _constrainPauseSectionErrored = false;
		private bool _constrainPauseChapterErrored = false;

		public IPublishingMethod PublishingMethod { get; private set; }
		public VerseIndexFormatType VerseIndexFormat { get; set; }
		internal int FilesInput { get; set; }
		internal int FilesOutput { get; set; }
		public string EthnologueCode { get; }

		public PublishingModel(string projectName, string ethnologueCode)
		{
			_projectName = projectName;
			EthnologueCode = ethnologueCode;
			_audioFormat = Get(() => Settings.Default.PublishAudioFormat);
			_publishOnlyCurrentBook = Get(() => Settings.Default.PublishCurrentBookOnly);
			_normalizeVolume = false;
			_reduceNoise = false;
		}

		public PublishingModel(IPublishingInfoProvider infoProvider) : this(infoProvider.Name, infoProvider.EthnologueCode)
		{
			_infoProvider = infoProvider;
		}

		internal bool PublishOnlyCurrentBook
		{
			get => _publishOnlyCurrentBook;
			set => _publishOnlyCurrentBook = Set(() => Settings.Default.PublishCurrentBookOnly = value);
		}

		internal bool NormalizeVolume
		{
			get => _normalizeVolume;
			set => _normalizeVolume /*= Settings.Default.NormalizeVolume*/ = value;
		}

		internal bool ReduceNoise
		{
			get => _reduceNoise;
			set => _reduceNoise /*= Settings.Default.ReduceNoise*/ = value;
		}

		internal PauseData SentencePause
		{
			get => _sentencePause;
			set => _sentencePause /*= Settings.Default.ReduceNoise*/ = value;
		}

		internal PauseData ParagraphPause
		{
			get => _paragraphPause;
			set => _paragraphPause /*= Settings.Default.ReduceNoise*/ = value;
		}

		internal PauseData SectionPause
		{
			get => _sectionPause;
			set => _sectionPause /*= Settings.Default.ReduceNoise*/ = value;
		}

		internal PauseData ChapterPause
		{
			get => _chapterPause;
			set => _chapterPause /*= Settings.Default.ReduceNoise*/ = value;
		}

		internal bool ConstrainPauseSentenceErrored
		{
			get => _constrainPauseSentenceErrored;
			set => _constrainPauseSentenceErrored = value;
		}

		internal bool ConstrainPauseParagraghErrored
		{
			get => _constrainPauseParagraghErrored;
			set => _constrainPauseParagraghErrored = value;
		}

		internal bool ConstrainPauseSectionErrored
		{
			get => _constrainPauseSectionErrored;
			set => _constrainPauseSectionErrored = value;
		}

		internal bool ConstrainPauseChapterErrored
		{
			get => _constrainPauseChapterErrored;
			set => _constrainPauseChapterErrored = value;
		}

		public string AudioFormat
		{
			get => _audioFormat;
			set
			{
				if (PublishingMethod != null)
					throw new InvalidOperationException("The audio format cannot be changed after Publish method has been called.");
				Set(() => Settings.Default.PublishAudioFormat = _audioFormat = value);
			}
		}
		/// <summary>
		/// Root shared by all projects (all languages). This is all we let the user specify. Just wraps the Settings "PublishRootPath"
		/// If specified path doesn't exist, silently falls back to default location in My Documents.
		/// </summary>
		public string PublishRootPath
		{
			get
			{
				var publishRootPath = Get(() => Settings.Default.PublishRootPath);
				if (IsNullOrEmpty(publishRootPath) || !Directory.Exists(publishRootPath))
					PublishRootPath = GetFolderPath(SpecialFolder.MyDocuments);
				return publishRootPath;
			}
			set
			{
				Set(() => Settings.Default.PublishRootPath = value, true);
			}
		}

		/// <summary>
		/// We use a directory directly underneath the PublishRootPath, named for this project.
		/// The directory may or may not exist.
		/// </summary>
		public string PublishThisProjectPath => Path.Combine(PublishRootPath, "HearThis-" + _projectName);

		public IPublishingInfoProvider PublishingInfoProvider => _infoProvider;

		public bool IncludeBook(string bookName)
		{
			return _infoProvider.IncludeBook(bookName);
		}

		public bool Publish(IProgress progress)
		{
			SetPublishingMethod();

			try
			{
				if (!Directory.Exists(PublishThisProjectPath))
				{
					Directory.CreateDirectory(PublishThisProjectPath);
				}
				var path = Path.Combine(PublishThisProjectPath, PublishingMethod.RootDirectoryName);
				FilesInput = FilesOutput = 0;
				if (PublishOnlyCurrentBook)
				{
					PublishingMethod.DeleteExistingPublishedFiles(path, _infoProvider.CurrentBookName);
					ClipRepository.PublishAllChapters(this, _projectName, _infoProvider.CurrentBookName, path, progress);
				}
				else
					ClipRepository.PublishAllBooks(this, _projectName, path, progress);

				foreach (var message in PublishingMethod.GetFinalInformationalMessages(this))
					progress.WriteMessage(message);
			}
			catch (Exception error)
			{
				progress.WriteError(error.Message);
				ErrorReport.NotifyUserOfProblem(error,
					LocalizationManager.GetString("PublishDialog.Error", "Sorry, the program made some mistake... ") + error.Message);
				return false;
			}
			var properties = new Dictionary<string, string>
				{
					{"FilesInput", FilesInput.ToString()},
					{"FilesOutput", FilesOutput.ToString()},
					{"Type", PublishingMethod.GetType().Name}
				};
			Analytics.Track("Published", properties);
			return true;
		}


		/// <summary>
		/// In production code, this should only be called by Publish method, but it's exposed here to
		/// make it accessible for tests.
		/// </summary>
		protected void SetPublishingMethod()
		{
			Debug.Assert(PublishingMethod == null);
			// Note that the case labels are derived from the Name property of the RadioButton controls, so the
			// case of these strings must match the case used in the control names.
			switch (AudioFormat)
			{
				case "audiBible":
					PublishingMethod = new AudiBiblePublishingMethod(new AudiBibleEncoder(), EthnologueCode);
					break;
				case "saber":
					PublishingMethod = new SaberPublishingMethod();
					break;
				case "megaVoice":
					PublishingMethod = new MegaVoicePublishingMethod();
					break;
				case "scrAppBuilder":
					PublishingMethod = new ScriptureAppBuilderPublishingMethod(EthnologueCode);
					break;
				case "mp3":
					PublishingMethod = new BunchOfFilesPublishingMethod(new LameEncoder());
					break;
				// This is OGG Vorbus, we are keeping it as "ogg" because originally, it was the only OGG option
				// and if we change the name now, previous user settings may break
				case "ogg":
					PublishingMethod = new BunchOfFilesPublishingMethod(new OggEncoder());
					break;
				case "opus":
					PublishingMethod = new BunchOfFilesPublishingMethod(new OpusEncoder());
					break;
				case "kulumi":
					PublishingMethod = new KulumiPublishingMethod();
					break;
				default:
					PublishingMethod = new BunchOfFilesPublishingMethod(new FlacEncoder());
					break;
			}
		}

		public bool BooksToExportHaveProblemsNeedingAttention()
		{
			return _infoProvider != null &&
				_infoProvider.HasProblemNeedingAttention(PublishOnlyCurrentBook ? _infoProvider.CurrentBookName : null);
		}
	}

	public class PauseData
	{
		public PauseData(bool apply, double min, double max)
		{
			Apply = apply;
			Min = min;
			Max = max;
		}

		public bool Apply { get; }
		public double Min { get; }
		public double Max { get; }
	}
}
