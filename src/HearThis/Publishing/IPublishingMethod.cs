// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using SIL.Progress;
using System.Collections.Generic;

namespace HearThis.Publishing
{
	public interface IPublishingMethod
	{
		/// <summary>
		/// Deletes any files left over from a previous publish of the given book, so this
		/// run doesn't mix stale output in with the new files.
		/// </summary>
		void DeleteExistingPublishedFiles(string rootFolderPath, string bookName);

		/// <summary>
		/// Gets the path (without extension) where the given chapter's published output file
		/// belongs.
		/// </summary>
		string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber);

		/// <summary>
		/// The name of the top-level folder (under the publish root) this method publishes into.
		/// </summary>
		string RootDirectoryName { get; }

		/// <summary>
		/// Any messages to present to the user once publishing has completed (e.g., where to
		/// find the output or how to use it).
		/// </summary>
		IEnumerable<string> GetFinalInformationalMessages(PublishingModel model);

		/// <summary>
		/// Given a chapter's merged clip audio (all of the chapter's clips joined into one
		/// WAV), performs whatever per-chapter processing this publishing method wants to do
		/// before chapter-boundary pauses are constrained. This is called once per chapter,
		/// independently of its neighbors, before any cross-chapter processing happens -- so
		/// it must not assume other chapters in the book have been prepared yet, and must not
		/// do anything that depends on the audio at the start/end of the chapter being in its
		/// final form (see FinalizeChapterAudio for that).
		/// </summary>
		/// <returns>The path to the prepared WAV file (may be the same path passed in, if this
		/// publishing method has nothing to do at this stage).</returns>
		string PrepareChapterAudio(string pathToIncomingChapterWav, IProgress progress,
			PublishingModel publishingModel = null);

		/// <summary>
		/// Given a chapter's prepared audio (the output of PrepareChapterAudio, with
		/// chapter-boundary pauses already constrained against its neighbors), performs any
		/// remaining processing and writes out the chapter's published output file.
		/// </summary>
		void FinalizeChapterAudio(string rootPath, string bookName, int chapterNumber,
			string preparedWavPath, IProgress progress, PublishingModel publishingModel = null);
	}
}
