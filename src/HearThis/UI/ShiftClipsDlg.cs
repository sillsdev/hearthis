// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2019, SIL International. All Rights Reserved.
// <copyright from='2019' to='2019' company='SIL International'>
//		Copyright (c) 2019, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HearThis.Script;
using SIL.Media;

namespace HearThis.UI
{
	public partial class ShiftClipsDlg : Form
	{
		internal class CellAddress
		{
			public int RowIndex { get; }
			public int ColumnIndex { get; }

			private CellAddress(int row, int column)
			{
				RowIndex = row;
				ColumnIndex = column;
			}

			public static implicit operator CellAddress(DataGridViewCellValueEventArgs eventArgs)
			{
				return new CellAddress(eventArgs.RowIndex, eventArgs.ColumnIndex);
			}

			public static implicit operator CellAddress(DataGridViewCellEventArgs eventArgs)
			{
				return new CellAddress(eventArgs.RowIndex, eventArgs.ColumnIndex);
			}

			public override bool Equals(object obj)
			{
				if (obj is CellAddress other)
					return Equals(other);
				return base.Equals(obj);
			}

			protected bool Equals(CellAddress other)
			{
				return RowIndex == other.RowIndex && ColumnIndex == other.ColumnIndex;
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (RowIndex * 397) ^ ColumnIndex;
				}
			}

			public static bool operator ==(CellAddress left, CellAddress right)
			{
				return Equals(left, right);
			}

			public static bool operator !=(CellAddress left, CellAddress right)
			{
				return !Equals(left, right);
			}
		}

		private readonly Func<int, string> _clipPathProvider;
		private readonly List<ScriptLine> _linesToShiftForward;
		private readonly List<ScriptLine> _linesToShiftBackward;
		private ISimpleAudioSession _player = null;
		private CellAddress _cellCurrentlyPlaying = null;

		/// <summary>
		/// Dialog box to help a project administrator shift a set of clips forward or backward one position relative to the blocks in order
		/// to bring them back into alignment
		/// </summary>
		/// <param name="clipPathProvider">Delegate that, given a 1-based line number, returns the path to the corresponding (current) clip.</param>
		/// <param name="linesToShiftForward">Series of ScriptLines (in ascending order) which, if the user chooses to shift forward, will
		/// have their corresponding clips incremented by one.</param>
		/// <param name="linesToShiftBackward">Series of ScriptLines (in ascending order) which, if the user chooses to shift backward, will
		/// have their corresponding clips decremented by one.</param>
		public ShiftClipsDlg(Func<int, string> clipPathProvider, List<ScriptLine> linesToShiftForward, List<ScriptLine> linesToShiftBackward)
		{
			_clipPathProvider = clipPathProvider;
			_linesToShiftForward = linesToShiftForward;
			this._linesToShiftBackward = linesToShiftBackward;
			InitializeComponent();

			ScriptLine exampleLIne;
			if (linesToShiftForward.Any())
			{
				exampleLIne = linesToShiftForward.First();
				_radioShiftLeft.Enabled = linesToShiftBackward.Any();
			}
			else
			{
				exampleLIne = linesToShiftBackward.First();
				_radioShiftLeft.Checked = true;
				_radioShiftRight.Enabled = false;
			}
			colScriptBlockText.DefaultCellStyle.Font = new Font(exampleLIne.FontName, exampleLIne.FontSize);
			colNewRecording.DefaultCellStyle.NullValue = null;
			colExistingRecording.DefaultCellStyle.NullValue = null;
			OnShiftDirectionChanged(null, null);
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			DisposePlayer();
			base.OnFormClosed(e);
		}

		public bool ShiftingForward => _radioShiftRight.Checked;
		public IReadOnlyList<ScriptLine> CurrentLines => ShiftingForward ? _linesToShiftForward : _linesToShiftBackward;

		private void _gridScriptLines_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= CurrentLines.Count)
				return;

			if (e.ColumnIndex == colScriptBlockText.Index)
			{
				e.Value = CurrentLines[e.RowIndex].Text;
			}
			else if (_cellCurrentlyPlaying != null && _cellCurrentlyPlaying == e)
			{
				e.Value = Properties.Resources.StopClip;
			}
			else if (e.ColumnIndex == colExistingRecording.Index)
			{
				if (ShiftingForward && e.RowIndex < _linesToShiftForward.Count - 1 || !ShiftingForward && e.RowIndex > 0)
					e.Value = Properties.Resources.PlayClip;
			}
			else if (e.ColumnIndex == colNewRecording.Index)
			{
				if (ShiftingForward && e.RowIndex > 0 || !ShiftingForward && e.RowIndex < _linesToShiftBackward.Count - 1)
					e.Value = Properties.Resources.PlayClip;
			}
		}

		private void OnShiftDirectionChanged(object sender, EventArgs e)
		{
			DisposePlayer();
			_gridScriptLines.RowCount = CurrentLines.Count;
			_gridScriptLines.Invalidate();
		}

		private void Play(string path, CellAddress cell)
		{
			{
				DisposePlayer();
				if (!string.IsNullOrEmpty(path))
				{
					_player = Utils.GetPlayer(this, path);
					if (_player != null)
					{
						_cellCurrentlyPlaying = cell;
						_player.Play();
						InvalidateCurrentlyPlayingCell();
					}
				}
			}
		}

		private void DisposePlayer()
		{
			if (_player != null)
			{
				if (_player.IsPlaying)
					_player.StopPlaying();
				if (_player is IDisposable disposablePlayer)
					disposablePlayer.Dispose();
				_player = null;
			}

			InvalidateCurrentlyPlayingCell();
			_cellCurrentlyPlaying = null;
		}

		private void InvalidateCurrentlyPlayingCell()
		{
			if (_cellCurrentlyPlaying == null || !IsHandleCreated || IsDisposed)
				return;
			if (_gridScriptLines.Rows.Count > _cellCurrentlyPlaying.RowIndex)
			{
				var cells = _gridScriptLines.Rows[_cellCurrentlyPlaying.RowIndex].Cells;
				if (cells.Count > _cellCurrentlyPlaying.ColumnIndex)
					_gridScriptLines.InvalidateCell(cells[_cellCurrentlyPlaying.ColumnIndex]);
			}
		}

		private void HandleCellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;
			if (_gridScriptLines.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewImageCell)
			{
				var clickedCell = (CellAddress)e;
				if (_cellCurrentlyPlaying == clickedCell)
					DisposePlayer();
				else
				{
					int line = e.RowIndex;
					if (e.ColumnIndex == colNewRecording.Index)
					{
						if (ShiftingForward)
							line--;
						else
							line++;
					}
					Play(_clipPathProvider(CurrentLines[line].Number), clickedCell);
				}
			}
		}
	}
}
