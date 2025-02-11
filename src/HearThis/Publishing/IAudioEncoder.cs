// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2024-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using SIL.Progress;

namespace HearThis.Publishing
{
	public interface IAudioEncoder
	{
		void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress, int timeoutInSeconds);
		string FormatName { get; }
	}
}
