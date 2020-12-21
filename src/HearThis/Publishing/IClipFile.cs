// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2020' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
namespace HearThis.Publishing
{
	public interface IClipFile
	{
		string FileName { get; }
		int Number { get; }
		/// <summary>
		/// Shift file the specified number of block positions
		/// </summary>
		/// <param name="positions">The number of positions forward (positive) or backward
		/// (negative) to move the file</param>
		void ShiftPosition(int positions);
	}
}
