// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2019' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HearThis.Publishing;
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

			private bool Equals(CellAddress other)
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

		private readonly Func<int, IClipFile> _clipFileProvider;
		private readonly List<ScriptLine> _linesToShiftForward;
		private readonly List<ScriptLine> _linesToShiftBackward;
		private ISimpleAudioSession _player = null;
		private CellAddress _cellCurrentlyPlaying = null;

		/// <summary>
		/// Dialog box to help a project administrator shift a set of clips forward or backward one
		/// position relative to the blocks in order to bring them back into alignment
		/// </summary>
		/// <param name="clipFileProvider">Delegate that, given a 0-based line number, returns an
		/// object representing the corresponding (current) clip.</param>
		/// <param name="linesToShiftForward">Series of ScriptLines (in ascending order) which, if
		/// the user chooses to shift forward, will have their corresponding clips incremented by one.</param>
		/// <param name="linesToShiftBackward">Series of ScriptLines (in ascending order) which, if
		/// the user chooses to shift backward, will have their corresponding clips decremented by one.</param>
		/// <remarks><paramref name="linesToShiftForward"/> and <paramref name="linesToShiftBackward"/> cannot both be empty.</remarks>
		public ShiftClipsDlg(Func<int, IClipFile> clipFileProvider, List<ScriptLine> linesToShiftForward,
			List<ScriptLine> linesToShiftBackward)
		{
			Debug.Assert(linesToShiftForward.Any() || linesToShiftBackward.Any());
			_clipFileProvider = clipFileProvider;
			_linesToShiftForward = linesToShiftForward;
			_linesToShiftBackward = linesToShiftBackward;
			InitializeComponent();

			if (linesToShiftForward.Any())
			{
				_radioShiftLeft.Enabled = linesToShiftBackward.Any();
			}
			else
			{
				// The logic here is a little weird, but since we know that at least one of the two lists will have
				// something in it, we don't have to examine linesToShiftBackward if linesToShiftForward is empty.
				_radioShiftLeft.Checked = true;
				_radioShiftRight.Enabled = false;
			}
			ScriptLine exampleLine = CurrentLines.First();
			colScriptBlockText.DefaultCellStyle.Font = new Font(exampleLine.FontName, exampleLine.FontSize);
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
				if (ShiftingForward && e.RowIndex > 0 || !ShiftingForward && GetFilePathForCell(e.RowIndex, e.ColumnIndex) != null)
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
					Play(GetFilePathForCell(e.RowIndex, e.ColumnIndex), clickedCell);
				}
			}
		}

		private string GetFilePathForCell(int row, int col)
		{
			var line = row;

			int fileNumber;
			if (col == colNewRecording.Index)
			{
				if (ShiftingForward)
					line--;
				else
					line++;
			}
			fileNumber = line >= CurrentLines.Count ? CurrentLines.Last().Number + (line - CurrentLines.Count) : CurrentLines[line].Number - 1;

			var path = _clipFileProvider(fileNumber).FilePath;
			return File.Exists(path) ? path : null;
		}
	}
}
