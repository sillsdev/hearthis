// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using Palaso.Progress;

namespace HearThis.Publishing
{
	public interface IAudioEncoder
	{
		void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress);
		string FormatName { get; }
	}
}