// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2022' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
namespace HearThis.UI
{
	/// <summary>
	/// Interface for windows/controls that need to do something special when on-the-fly
	/// localization occurs.
	/// </summary>
	public interface ILocalizable
	{
		/// <summary>
		/// Implement this to save localized format strings, reformat formatted strings displayed
		/// in the UI, and/or repopulate UI elements with dynamic localized strings.
		/// </summary>
		void HandleStringsLocalized();
	}
}
