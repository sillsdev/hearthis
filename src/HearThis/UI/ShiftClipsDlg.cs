using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HearThis.Script;

namespace HearThis.UI
{
	public partial class ShiftClipsDlg : Form
	{
		private readonly List<ScriptLine> _linesToShift;

		public ShiftClipsDlg(List<ScriptLine> linesToShift)
		{
			_linesToShift = linesToShift;
			InitializeComponent();

			colScriptBlockText.DefaultCellStyle.Font = new Font(linesToShift[0].FontName, linesToShift[0].FontSize);
			colNewRecording.DefaultCellStyle.NullValue = null;
			colExistingRecording.DefaultCellStyle.NullValue = null;
			_gridScriptLines.RowCount = linesToShift.Count;
		}

		private void _gridScriptLines_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _linesToShift.Count)
				return;

			if (e.ColumnIndex == colScriptBlockText.Index)
			{
				e.Value = _linesToShift[e.RowIndex].Text;
			}
			else if (e.ColumnIndex == colExistingRecording.Index)
			{
				if (e.RowIndex < _linesToShift.Count - 1)
					e.Value = Properties.Resources.PlayClip;
			}
			else if (e.ColumnIndex == colNewRecording.Index)
			{
				if (e.RowIndex > 0)
					e.Value = Properties.Resources.PlayClip;
			}
		}
	}
}
