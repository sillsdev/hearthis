// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2020' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
namespace HearThis.Publishing
{
	public interface IClipFile
	{
		string FilePath { get; }
		int Number { get; }
		/// <summary>
		/// Shift file the specified number of block positions
		/// </summary>
		/// <param name="positions">The number of positions forward (positive) or backward
		/// (negative) to move the file</param>
		void ShiftPosition(int positions);
		/// <summary>
		/// This is a companion to the above method. Really just intended
		/// for use in error reporting if something goes wrong with the move.
		/// </summary>
		/// <param name="positions">The number of positions forward (positive) or backward
		/// (negative) to move the file</param>
		/// <returns>The full path to the intended destination if the <see cref="FilePath"/>
		/// is moved the indicated number of positions.</returns>
		string GetIntendedDestinationPath(int positions);
	}
}
