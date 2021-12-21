// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HearThis.Publishing;
using static System.Int32;

namespace HearThis.Script
{
	public class ShiftClipsViewModel
	{
		public string ProjectName { get; }
		public string BookName { get; }
		public ChapterInfo ChapterInfo { get; }

		private readonly Func<int, IClipFile> _clipFileProvider;

		// _linesToShiftForward is a series of ScriptLines (in ascending order) which, if
		// the user chooses to shift forward, will have their corresponding clips incremented by one.
		// _linesToShiftBackward is a series of ScriptLines (in ascending order) which, if
		// the user chooses to shift backward, will have their corresponding clips decremented by one.
		private readonly List<ScriptLine> _linesToShiftForward, _linesToShiftBackward;
		private bool _shiftingForward;

		public ShiftClipsViewModel(Project project)
		{
			ProjectName = project.Name;
			BookName = project.SelectedBook.Name;
			ChapterInfo = project.SelectedChapterInfo;

			_clipFileProvider = line => ClipRepository.GetClipFile(ProjectName, BookName,
				ChapterInfo.ChapterNumber1Based, line, project.ScriptProvider);

			var realBlockCountForChapter = project.LineCountForChapter;
			NormalShifting = project.SelectedScriptBlock < realBlockCountForChapter;

			if (NormalShifting)
			{
				_linesToShiftForward = project.GetRecordableBlocksUpThroughNextHoleToTheRight();
				_linesToShiftBackward = project.GetRecordableBlocksAfterPreviousHoleToTheLeft();
			}
			else // Shifting extra clips backward.
			{
				_linesToShiftForward = new List<ScriptLine>();
				_linesToShiftBackward = new List<ScriptLine>();
				// First, need to see if there is a "hole" someplace earlier in the chapter.
				int lastHole;
				for (lastHole = realBlockCountForChapter - 1; lastHole >= 0; lastHole--)
				{
					if (!GetHasRecordedClip(lastHole))
						break;
				}

				if (lastHole >= 0)
				{
					var book = project.SelectedBook;
					var chapterInfo = project.SelectedChapterInfo;
					// REVIEW: Do we want to generate "empty" script lines to represent all the extras?
					// (i.e., when i >= realBlockCount && < _scriptSlider.SegmentCount)
					for (var i = lastHole; i < realBlockCountForChapter; i++)
					{
						_linesToShiftBackward.Add(book.ScriptProvider.GetBlock(
							book.BookNumber, chapterInfo.ChapterNumber1Based, i));
					}
				}
			}
		}

		public string GetFilePath(int fileNumber)
		{
			var path = _clipFileProvider(fileNumber).FilePath;
			return File.Exists(path) ? path : null;
		}

		public ClipRepository.ClipShiftingResult ShiftClips()
		{
			var startLineNumber = CurrentLines.First().Number - (NormalShifting ? 1 : 0);

			return ClipRepository.ShiftClips(ProjectName, BookName,
				ChapterInfo.ChapterNumber1Based, startLineNumber,
				NormalShifting ? CurrentLines.Count - 1 : MaxValue,
				Offset, () => ChapterInfo);
		}

		private bool GetHasRecordedClip(int i) => ClipRepository.GetHaveClipUnfiltered(ProjectName, BookName,
			ChapterInfo.ChapterNumber1Based, i);

		private bool NormalShifting { get; }

		public bool CanShift => CanShiftForward || CanShiftBackward;

		public bool CanShiftForward => _linesToShiftForward.Any();

		public bool CanShiftBackward => _linesToShiftBackward.Any();

		public IReadOnlyList<ScriptLine> CurrentLines => ShiftingForward ? _linesToShiftForward : _linesToShiftBackward;

		public bool ShiftingForward
		{
			get => _shiftingForward;
			set
			{
				if (value && !CanShiftForward)
					throw new InvalidOperationException("Cannot shift lines forward.");
				if (!value && !CanShiftBackward)
					throw new InvalidOperationException("Cannot shift lines backward.");

				_shiftingForward = value;
			}
		}

		public int Offset => ShiftingForward ? 1 : -1;

	}
}
