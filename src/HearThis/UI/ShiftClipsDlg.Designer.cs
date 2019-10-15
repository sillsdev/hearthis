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
			if (disposing)
			{
				DisposePlayer();
				components?.Dispose();
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
			System.Windows.Forms.Label _labelDirection;
			System.Windows.Forms.Label _labelExplanation;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShiftClipsDlg));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this._btnOk = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._btnCancel = new System.Windows.Forms.Button();
			this._radioShiftRight = new System.Windows.Forms.RadioButton();
			this._radioShiftLeft = new System.Windows.Forms.RadioButton();
			this._gridScriptLines = new System.Windows.Forms.DataGridView();
			this.colScriptBlockText = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colExistingRecording = new System.Windows.Forms.DataGridViewImageColumn();
			this.colNewRecording = new System.Windows.Forms.DataGridViewImageColumn();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			_labelDirection = new System.Windows.Forms.Label();
			_labelExplanation = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._gridScriptLines)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _labelDirection
			// 
			_labelDirection.AutoSize = true;
			_labelDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(_labelDirection, null);
			this.l10NSharpExtender1.SetLocalizationComment(_labelDirection, null);
			this.l10NSharpExtender1.SetLocalizationPriority(_labelDirection, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(_labelDirection, "ShiftClips.ShiftClipsDlg._labelDirection");
			_labelDirection.Location = new System.Drawing.Point(362, 0);
			_labelDirection.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
			_labelDirection.Name = "_labelDirection";
			_labelDirection.Size = new System.Drawing.Size(62, 13);
			_labelDirection.TabIndex = 2;
			_labelDirection.Text = "Direction:";
			// 
			// _labelExplanation
			// 
			_labelExplanation.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(_labelExplanation, null);
			this.l10NSharpExtender1.SetLocalizationComment(_labelExplanation, null);
			this.l10NSharpExtender1.SetLocalizationPriority(_labelExplanation, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(_labelExplanation, "ShiftClips.ShiftClipsDlg._labelExplanation");
			_labelExplanation.Location = new System.Drawing.Point(3, 0);
			_labelExplanation.Name = "_labelExplanation";
			this.tableLayoutPanel1.SetRowSpan(_labelExplanation, 3);
			_labelExplanation.Size = new System.Drawing.Size(346, 65);
			_labelExplanation.TabIndex = 4;
			_labelExplanation.Text = resources.GetString("_labelExplanation.Text");
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
			// _radioShiftRight
			// 
			this._radioShiftRight.AutoSize = true;
			this._radioShiftRight.Checked = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._radioShiftRight, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._radioShiftRight, null);
			this.l10NSharpExtender1.SetLocalizingId(this._radioShiftRight, "ShiftClips.radioButton1");
			this._radioShiftRight.Location = new System.Drawing.Point(367, 22);
			this._radioShiftRight.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
			this._radioShiftRight.Name = "_radioShiftRight";
			this._radioShiftRight.Size = new System.Drawing.Size(236, 17);
			this._radioShiftRight.TabIndex = 1;
			this._radioShiftRight.TabStop = true;
			this._radioShiftRight.Text = "Shift recorded clips to the following sentence";
			this._radioShiftRight.UseVisualStyleBackColor = true;
			this._radioShiftRight.CheckedChanged += new System.EventHandler(this.OnShiftDirectionChanged);
			// 
			// _radioShiftLeft
			// 
			this._radioShiftLeft.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._radioShiftLeft, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._radioShiftLeft, null);
			this.l10NSharpExtender1.SetLocalizingId(this._radioShiftLeft, "ShiftClips.radioButton2");
			this._radioShiftLeft.Location = new System.Drawing.Point(367, 45);
			this._radioShiftLeft.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
			this._radioShiftLeft.Name = "_radioShiftLeft";
			this._radioShiftLeft.Size = new System.Drawing.Size(242, 17);
			this._radioShiftLeft.TabIndex = 3;
			this._radioShiftLeft.Text = "Shift recorded clips to the preceding sentence";
			this._radioShiftLeft.UseVisualStyleBackColor = true;
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
			this.tableLayoutPanel1.SetColumnSpan(this._gridScriptLines, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._gridScriptLines, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._gridScriptLines, null);
			this.l10NSharpExtender1.SetLocalizingId(this._gridScriptLines, "ShiftClipsDlg._gridScriptLines");
			this._gridScriptLines.Location = new System.Drawing.Point(3, 68);
			this._gridScriptLines.MultiSelect = false;
			this._gridScriptLines.Name = "_gridScriptLines";
			this._gridScriptLines.ReadOnly = true;
			this._gridScriptLines.RowHeadersVisible = false;
			this._gridScriptLines.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._gridScriptLines.ShowCellErrors = false;
			this._gridScriptLines.ShowRowErrors = false;
			this._gridScriptLines.Size = new System.Drawing.Size(606, 351);
			this._gridScriptLines.TabIndex = 0;
			this._gridScriptLines.VirtualMode = true;
			this._gridScriptLines.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.HandleCellContentClick);
			this._gridScriptLines.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this._gridScriptLines_CellValueNeeded);
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
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this._gridScriptLines, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this._radioShiftRight, 1, 1);
			this.tableLayoutPanel1.Controls.Add(_labelDirection, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this._radioShiftLeft, 1, 2);
			this.tableLayoutPanel1.Controls.Add(_labelExplanation, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(612, 422);
			this.tableLayoutPanel1.TabIndex = 3;
			// 
			// ShiftClipsDlg
			// 
			this.AcceptButton = this._btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._btnCancel;
			this.ClientSize = new System.Drawing.Size(636, 485);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this._btnOk);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "ShiftClipsDlg.WindowTitle");
			this.Name = "ShiftClipsDlg";
			this.Text = "Shift Clips";
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._gridScriptLines)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Button _btnOk;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.DataGridView _gridScriptLines;
		private System.Windows.Forms.DataGridViewTextBoxColumn colScriptBlockText;
		private System.Windows.Forms.DataGridViewImageColumn colExistingRecording;
		private System.Windows.Forms.DataGridViewImageColumn colNewRecording;
		private System.Windows.Forms.RadioButton _radioShiftRight;
		private System.Windows.Forms.RadioButton _radioShiftLeft;
	}
}
