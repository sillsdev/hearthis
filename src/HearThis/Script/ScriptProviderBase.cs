using System;
using System.Collections.Generic;
using System.IO;
using HearThis.Publishing;

namespace HearThis.Script
{
	public abstract class ScriptProviderBase : IScriptProvider
	{
		private SkippedScriptLines _skippedLines;
		private Tuple<int, int> _chapterHavingSkipFlagPopulated;

		public abstract ScriptLine GetBlock(int bookNumber, int chapterNumber, int lineNumber0Based);
		public abstract int GetScriptBlockCount(int bookNumber, int chapter1Based);
		public abstract int GetSkippedScriptBlockCount(int bookNumber, int chapter1Based);
		public abstract int GetUnskippedScriptBlockCount(int bookNumber, int chapter1Based);
		public abstract int GetTranslatedVerseCount(int bookNumberDelegateSafe, int chapterNumber1Based);
		public abstract int GetScriptBlockCount(int bookNumber);
		public abstract void LoadBook(int bookNumber0Based);
		public abstract string EthnologueCode { get; }
		public abstract string ProjectFolderName { get; }

		protected string ProjectFolderPath
		{
			get { return ClipRecordingRepository.GetApplicationDataFolder(ProjectFolderName); }
		}

		protected void LoadSkipInfo()
		{
			_skippedLines = SkippedScriptLines.Create(Path.Combine(ProjectFolderPath, "SkippedLineInfo.xml"));
		}

		protected void PopulateSkippedFlag(int bookNumber, int chapterNumber, List<ScriptLine> scriptLines)
		{
			foreach (var scriptBlock in scriptLines)
				scriptBlock.OnSkippedChanged += (line) => HandleSkippedFlagChanged(bookNumber, chapterNumber, line);

			lock (_skippedLines)
			{
				_chapterHavingSkipFlagPopulated = new Tuple<int, int>(bookNumber, chapterNumber);
				_skippedLines.PopulateSkippedFlag(bookNumber, chapterNumber, scriptLines);
				_chapterHavingSkipFlagPopulated = null;
			}
		}

		private void HandleSkippedFlagChanged(int bookNumber, int chapterNumber, ScriptLine line)
		{
			lock (_skippedLines)
			{
				if (_chapterHavingSkipFlagPopulated != null && _chapterHavingSkipFlagPopulated.Item1 == bookNumber && _chapterHavingSkipFlagPopulated.Item2 == chapterNumber)
					return;

				if (line.Skipped)
					_skippedLines.AddSkippedLine(bookNumber, chapterNumber, line);
				else
					_skippedLines.RemoveSkippedLine(bookNumber, chapterNumber, line);
			}
		}
	}
}