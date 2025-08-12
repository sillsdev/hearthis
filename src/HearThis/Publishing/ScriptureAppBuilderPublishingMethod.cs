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
using L10NSharp;

namespace HearThis.Publishing
{
	public class ScriptureAppBuilderPublishingMethod : HierarchicalPublishingMethodBase
	{
		public const string kScriptureAppBuilder = "Scripture App Builder";
		private readonly string _ethnologueCode;
		public ScriptureAppBuilderPublishingMethod(string ethnologueCode) : base(new LameEncoder())
		{
			_ethnologueCode = ethnologueCode.ToUpperInvariant();
		}

		protected override string FolderFormat => "{0}-{1}";

		public override string RootDirectoryName => "ScriptureAppBuilder";

		public override IEnumerable<string> GetFinalInformationalMessages(PublishingModel model)
		{
			foreach (var message in base.GetFinalInformationalMessages(model))
				yield return message;

			if (model.VerseIndexFormat == PublishingModel.VerseIndexFormatType.AudacityLabelFilePhraseLevel)
			{
				yield return ""; // blank line
				yield return string.Format(LocalizationManager.GetString(
						"PublishDialog.ScriptureAppBuilderInstructionsAboutBlockBreakChars",
						"When building the app using {2}, in order for " +
						"the audio to synchronize with the text highlighting make sure that " +
						"the recording phrase-ending characters specified on the 'Audio - " +
						"Audio Synchronization' page in {3} has the same characters that " +
						"{1} uses to break the text into recording blocks in your project: {0}.",
						"Param 0: list of characters; " +
						"Param 1: \"HearThis\" (product name); " +
						"Param 2: \"Scripture App Builder\" (product name); " +
						"Param 3: \"SAB\" (product name abbreviated)"),
					model.PublishingInfo.BlockBreakCharacters, Program.kProduct, kScriptureAppBuilder, "SAB");
			}
		}

		public override string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			var bookNumber = _statistics.GetBookNumber(bookName);
			string bookIndex = (bookNumber + 1).ToString("000");
			var bookAbbr = _statistics.GetBookCode(bookNumber).ToUpperInvariant();
			string chapterIndex = chapterNumber.ToString("000");
			var folderName = $"{bookName}-{bookIndex}";
			string folderPath = Path.Combine(rootFolderPath, folderName);
			string fileName = _ethnologueCode + "-" + bookAbbr + "-" + chapterIndex;
			EnsureDirectory(folderPath);

			return Path.Combine(folderPath, fileName);
		}
	}
}
