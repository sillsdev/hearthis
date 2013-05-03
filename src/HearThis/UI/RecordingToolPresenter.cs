using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HearThis.Script;

namespace HearThis.UI
{
	public class RecordingToolPresenter : IRecordingPresenter
	{
		public int MaxBooks { get; set; }
		public int MaxChapters { get; set; }
		public int MaxLines { get; set; }
		public int CurrentBook { get; set; }
		public int CurrentChapter { get; set; }
		public int CurrentLine { get; set; }

		public void GotoNextLine()
		{
			if (CurrentLine + 1 > MaxLines)
			{
				HideScriptLines();
				ShowEndSectionMessages();
			}
			else
			{
				CurrentLine++;
				HideEndSectionMessages();
				ShowScriptLines();
			}
		}

		private void ShowScriptLines()
		{
			throw new NotImplementedException();
		}

		private void HideEndSectionMessages()
		{
			throw new NotImplementedException();
		}

		private void ShowEndSectionMessages()
		{
			throw new NotImplementedException();
		}

		private void HideScriptLines()
		{
			throw new NotImplementedException();
		}
	}
}
