// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2016, SIL International. All Rights Reserved.
// <copyright from='2015' to='2016' company='SIL International'>
//		Copyright (c) 2016, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using Paratext;
using SIL.DblBundle;
using SIL.DblBundle.Text;

namespace HearThis.Script
{
	public class ParatextProjectProxy : IProjectInfo
	{
		private readonly ScrText m_scrText;
		private readonly DblMetadataLanguage m_language;

		public ParatextProjectProxy(ScrText scrText)
		{
			m_scrText = scrText;
			m_language = new DblMetadataLanguage();
			m_language.Iso = scrText.Settings.LanguageID.Id;
			m_language.Name = scrText.DisplayLanguageName;
			m_language.Script = scrText.DefaultFont;
			m_language.ScriptDirection = scrText.RightToLeft ? "RTL" : "LTR";
		}

		public ScrText ScrText { get { return m_scrText; } }

		public string Name { get { return m_scrText.Name; } }

		public string Id { get { return m_scrText.Settings.DBLId ?? m_scrText.Name;} }

		public DblMetadataLanguage Language
		{
			get { return m_language; }
		}
	}
}
