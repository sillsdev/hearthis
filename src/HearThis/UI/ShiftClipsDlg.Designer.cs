namespace HearThis.UI
{
	partial class ShiftClipsDlg
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this._gridScriptLines = new System.Windows.Forms.DataGridView();
			this._btnOk = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._btnCancel = new System.Windows.Forms.Button();
			this.colScriptBlockText = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colExistingRecording = new System.Windows.Forms.DataGridViewImageColumn();
			this.colNewRecording = new System.Windows.Forms.DataGridViewImageColumn();
			((System.ComponentModel.ISupportInitialize)(this._gridScriptLines)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// _gridScriptLines
			// 
			this._gridScriptLines.AllowUserToAddRows = false;
			this._gridScriptLines.AllowUserToDeleteRows = false;
			this._gridScriptLines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._gridScriptLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this._gridScriptLines.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colScriptBlockText,
            this.colExistingRecording,
            this.colNewRecording});
			this.l10NSharpExtender1.SetLocalizableToolTip(this._gridScriptLines, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._gridScriptLines, null);
			this.l10NSharpExtender1.SetLocalizingId(this._gridScriptLines, "ShiftClipsDlg._gridScriptLines");
			this._gridScriptLines.Location = new System.Drawing.Point(12, 12);
			this._gridScriptLines.MultiSelect = false;
			this._gridScriptLines.Name = "_gridScriptLines";
			this._gridScriptLines.ReadOnly = true;
			this._gridScriptLines.RowHeadersVisible = false;
			this._gridScriptLines.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._gridScriptLines.ShowCellErrors = false;
			this._gridScriptLines.ShowRowErrors = false;
			this._gridScriptLines.Size = new System.Drawing.Size(612, 424);
			this._gridScriptLines.TabIndex = 0;
			this._gridScriptLines.VirtualMode = true;
			this._gridScriptLines.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this._gridScriptLines_CellValueNeeded);
			// 
			// _btnOk
			// 
			this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._btnOk, L10NSharp.LocalizationPriority.High);
			this.l10NSharpExtender1.SetLocalizingId(this._btnOk, "Common.OK");
			this._btnOk.Location = new System.Drawing.Point(468, 450);
			this._btnOk.Name = "_btnOk";
			this._btnOk.Size = new System.Drawing.Size(75, 23);
			this._btnOk.TabIndex = 1;
			this._btnOk.Text = "OK";
			this._btnOk.UseVisualStyleBackColor = true;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = null;
			this.l10NSharpExtender1.PrefixForNewItems = "ShiftClips";
			// 
			// _btnCancel
			// 
			this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnCancel, "ShiftClips.button1");
			this._btnCancel.Location = new System.Drawing.Point(549, 450);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 2;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			// 
			// colScriptBlockText
			// 
			this.colScriptBlockText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colScriptBlockText.HeaderText = "Script Text";
			this.colScriptBlockText.Name = "colScriptBlockText";
			this.colScriptBlockText.ReadOnly = true;
			this.colScriptBlockText.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colExistingRecording
			// 
			this.colExistingRecording.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.colExistingRecording.DefaultCellStyle = dataGridViewCellStyle1;
			this.colExistingRecording.HeaderText = "Existing";
			this.colExistingRecording.Name = "colExistingRecording";
			this.colExistingRecording.ReadOnly = true;
			this.colExistingRecording.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colExistingRecording.Width = 73;
			// 
			// colNewRecording
			// 
			this.colNewRecording.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.colNewRecording.DefaultCellStyle = dataGridViewCellStyle2;
			this.colNewRecording.HeaderText = "After Shifting";
			this.colNewRecording.Name = "colNewRecording";
			this.colNewRecording.ReadOnly = true;
			this.colNewRecording.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colNewRecording.Width = 73;
			// 
			// ShiftClipsDlg
			// 
			this.AcceptButton = this._btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._btnCancel;
			this.ClientSize = new System.Drawing.Size(636, 485);
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this._btnOk);
			this.Controls.Add(this._gridScriptLines);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "ShiftClipsDlg.WindowTitle");
			this.Name = "ShiftClipsDlg";
			this.Text = "Shift Clips";
			((System.ComponentModel.ISupportInitialize)(this._gridScriptLines)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView _gridScriptLines;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Button _btnOk;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.DataGridViewTextBoxColumn colScriptBlockText;
		private System.Windows.Forms.DataGridViewImageColumn colExistingRecording;
		private System.Windows.Forms.DataGridViewImageColumn colNewRecording;
	}
}