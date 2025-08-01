// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2019-2025, SIL Global.
// <copyright from='2019' to='2025' company='SIL Global'>
//		Copyright (c) 2019-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HearThis.Script;
using L10NSharp;
using SIL.Media;
using SIL.Reporting;

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

		private readonly ShiftClipsViewModel _model;
		private ISimpleAudioSession _player = null;
		private CellAddress _cellCurrentlyPlaying = null;

		/// <summary>
		/// Dialog box to help a project administrator shift a set of clips forward or backward one
		/// position relative to the blocks in order to bring them back into alignment
		/// </summary>
		/// <param name="model">Object that contains the data about lines that can be shifted (forward and/or backward).</param>
		public ShiftClipsDlg(ShiftClipsViewModel model)
		{
			Debug.Assert(model.CanShift);
			_model = model;
			InitializeComponent();

			if (_model.CanShiftForward)
			{
				_radioShiftLeft.Enabled = _model.CanShiftBackward;
			}
			else
			{
				// The logic here is a little weird, but since we know that at least one of the two lists will have
				// something in it, we don't have to examine linesToShiftBackward if linesToShiftForward is empty.
				_radioShiftLeft.Checked = true;
				_radioShiftRight.Enabled = false;
			}
			ScriptLine exampleLine = _model.CurrentLines.First();
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

		private void _gridScriptLines_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _model.CurrentLines.Count)
				return;

			if (e.ColumnIndex == colScriptBlockText.Index)
			{
				e.Value = _model.CurrentLines[e.RowIndex].Text;
			}
			else if (_cellCurrentlyPlaying != null && _cellCurrentlyPlaying == e)
			{
				e.Value = Properties.Resources.StopClip;
			}
			else if (e.ColumnIndex == colExistingRecording.Index)
			{
				if (_model.ShiftingForward && e.RowIndex < _model.CurrentLines.Count - 1 || !_model.ShiftingForward && e.RowIndex > 0)
					e.Value = Properties.Resources.PlayClip;
			}
			else if (e.ColumnIndex == colNewRecording.Index)
			{
				if (_model.ShiftingForward && e.RowIndex > 0 || !_model.ShiftingForward && GetFilePathForCell(e.RowIndex, e.ColumnIndex) != null)
					e.Value = Properties.Resources.PlayClip;
			}
		}

		private void OnShiftDirectionChanged(object sender, EventArgs e)
		{
			DisposePlayer();
			_model.ShiftingForward = _radioShiftRight.Checked;
			_gridScriptLines.RowCount = _model.CurrentLines.Count;
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
						((ISimpleAudioWithEvents)_player).PlaybackStopped += ShiftClipsDlg_PlaybackStopped;
						_player.Play();
						InvalidateCurrentlyPlayingCell();
					}
				}
			}
		}

		private void ShiftClipsDlg_PlaybackStopped(object sender, EventArgs e)
		{
			if (_player is ISimpleAudioWithEvents player) // Should always be true unless _player has been disposed
				player.PlaybackStopped -= ShiftClipsDlg_PlaybackStopped;
			InvalidateCurrentlyPlayingCell();
			_cellCurrentlyPlaying = null;
		}

		private void DisposePlayer()
		{
			if (_player != null)
			{
				if (_player.IsPlaying)
					_player.StopPlaying();
				var disposablePlayer = _player as IDisposable;
				_player = null; // Setting to null before disposing avoids race condition.
				if (disposablePlayer != null)
					disposablePlayer.Dispose();
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

			if (col == colNewRecording.Index)
			{
				if (_model.ShiftingForward)
					line--;
				else
					line++;
			}
			var fileNumber = line >= _model.CurrentLines.Count ? _model.CurrentLines.Last().Number + (line - _model.CurrentLines.Count) : _model.CurrentLines[line].Number - 1;

			return _model.GetFilePath(fileNumber);
		}

		private void _btnOk_Click(object sender, EventArgs e)
		{
			DisposePlayer();

			var result = _model.ShiftClips();

			if (result.Error != null)
			{
				if (result.Attempted > result.SuccessfulMoves)
				{
					ErrorReport.NotifyUserOfProblem(result.Error,
						LocalizationManager.GetString("RecordingControl.FailedToShiftClips",
							"There was a problem renaming clip\r\n{0}\r\nto\r\n{1}\r\n{2} of {3} clips shifted successfully.",
							"Param 0: Original clip file path; " +
							"Param 1: Intended new clip file path; " +
							"Param 2: Number of clips that were shifted before this error occurred; " +
							"Param 3: Total number of clips that HearThis intended to shift"),
						result.LastAttemptedMove.FilePath, result.LastAttemptedMove.GetIntendedDestinationPath(_model.Offset),
						result.SuccessfulMoves, result.Attempted);
				}
				else
				{
					ErrorReport.NotifyUserOfProblem(result.Error,
						LocalizationManager.GetString("RecordingControl.FailedToUpdateChapterInfo",
							"There was a problem updating chapter information for {0}, chapter {1}.",
							"Param 0: Scripture book name in English; " +
							"Param 1: chapter number"),
						_model.BookName, _model.ChapterInfo.ChapterNumber1Based);
				}
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
