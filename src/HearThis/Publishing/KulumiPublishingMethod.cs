// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International. All Rights Reserved.
// <copyright from='2024' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------

using L10NSharp;
using System.Collections.Generic;

namespace HearThis.Publishing
{
	/// <summary>
	/// Represents a publishing method specific to Kulumi.
	/// </summary>
	public class KulumiPublishingMethod : SaberPublishingMethod
	{
		/// <summary>
		/// Gets the root directory name specific to Kulumi publishing.
		/// </summary>
		public override string RootDirectoryName => "Kulumi";

		public override IEnumerable<string> GetFinalInformationalMessages(PublishingModel model)
		{
			foreach (var message in base.GetFinalInformationalMessages(model))
				yield return message;

			if (model.VerseIndexFormat == PublishingModel.VerseIndexFormatType.AudacityLabelFilePhraseLevel)
			{
				yield return ""; // blank line
				yield return LocalizationManager.GetString(
					"PublishDialog.KulumiPublishingInstructions",
					"For Kulumi X and Mini, the root folder can be renamed as needed to " +
					"distinguish from other top-level publications on device and the entire " +
					"folder structure should be copied over. For the Kulumi Sheep, which " +
					"supports only 2 levels of navigation, only the contents of the Kulumi " +
					"folder should be copied to the device");
			}
		}
	}
}
