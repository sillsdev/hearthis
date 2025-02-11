// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022-2025, SIL Global.
// <copyright from='2014' to='2025' company='SIL Global'>
//		Copyright (c) 2022-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------

namespace HearThis.Communication
{
	/// <summary>
	/// Defines the method that may be used to communicate with HearThisAndroid
	/// </summary>
	public interface IAndroidLink
	{
		string GetDeviceName();
		bool GetFile(string androidPath, string destPath);
		bool TryGetData(string androidPath, out byte[] data);
		bool PutFile(string androidPath, byte[] data);
		bool TryListFiles(string androidPath, out string list);
		void DeleteFile(string androidPath);
	}
}
