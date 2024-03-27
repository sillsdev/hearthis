// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International. All Rights Reserved.
// <copyright from='2011' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International. All Rights Reserved.
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
