// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2016' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using HearThis.Properties;

namespace HearThis.Script
{
	[Serializable]
	[XmlRoot(ElementName = "ProjectInfo", Namespace = "", IsNullable = false)]
	public class ProjectSettings
	{
		private bool _newlyCreatedSettingsForExistingProject;

		public ProjectSettings()
		{
			// Set defaults
			ClauseBreakCharacters = ", ; :";
			AdditionalBlockBreakCharacters = "";

			Version = Settings.Default.CurrentDataVersion;
		}

		/// <summary>
		/// This setting is not persisted, as it is only needed during data migration. It allows us to
		/// correctly handle two edge cases: 1) a project from an early version of HearThis is
		/// getting settings for the very first time or 2) the settings were corrupted in a way that
		/// prevented them from being deserialized. Note that in the latter case, we can't know for
		/// sure which data migration steps need to be done because we don't know which version the
		/// project was at when things went awry.
		/// </summary>
		[XmlIgnore]
		internal bool NewlyCreatedSettingsForExistingProject
		{
			get => _newlyCreatedSettingsForExistingProject;
			set
			{
				_newlyCreatedSettingsForExistingProject = value;
				Version = 0;
			}
		}

		[XmlAttribute("version")]
		public int Version { get; set; }

		[XmlAttribute("breakAtParagraphBreaks")]
		[DefaultValue(false)]
		public bool BreakAtParagraphBreaks { get; set; }

		[XmlAttribute("breakQuotesIntoBlocks")]
		[DefaultValue(false)]
		public bool BreakQuotesIntoBlocks { get; set; }

		[XmlAttribute("clauseBreakCharacters")]
		[DefaultValue(", ; :")]
		public string ClauseBreakCharacters { get; set; }

		[XmlAttribute("additionalBlockBreakCharacters")]
		[DefaultValue("")]
		public string AdditionalBlockBreakCharacters { get; set; }
	}
}
