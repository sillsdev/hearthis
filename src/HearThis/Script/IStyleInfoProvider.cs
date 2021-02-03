// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2020' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
namespace HearThis.Script
{
	/// <summary>
	/// This exposes the things we care about out of ScrStylesheet, providing an
	/// anti-corruption layer between Paratext and HearThis and allowing us to test the code
	/// that calls Paratext.
	/// </summary>
	public interface IStyleInfoProvider
	{
		bool IsParagraph(string tag);
		bool IsPublishableVernacular(string tag);
		string GetStyleName(string tag);
	}
}
