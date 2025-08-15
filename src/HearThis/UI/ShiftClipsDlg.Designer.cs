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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShiftClipsDlg));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this._labelDirection = new System.Windows.Forms.Label();
            this._labelExplanation = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridScriptLines)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _labelDirection
            // 
            this._labelDirection.AutoSize = true;
            this._labelDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l10NSharpExtender1.SetLocalizableToolTip(this._labelDirection, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._labelDirection, null);
            this.l10NSharpExtender1.SetLocalizingId(this._labelDirection, "ShiftClipsDlg._labelDirection");
            this._labelDirection.Location = new System.Drawing.Point(425, 0);
            this._labelDirection.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this._labelDirection.Name = "_labelDirection";
            this._labelDirection.Size = new System.Drawing.Size(62, 13);
            this._labelDirection.TabIndex = 2;
            this._labelDirection.Text = "Direction:";
            // 
            // _labelExplanation
            // 
            this._labelExplanation.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._labelExplanation, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._labelExplanation, "Param 0: The name of the \"Shift Clips\" command (see `ShiftClipsDlg.WindowTitle`);" +
        " Param 1: \"HearThis\" (product name)");
            this.l10NSharpExtender1.SetLocalizingId(this._labelExplanation, "ShiftClipsDlg._labelExplanation");
            this._labelExplanation.Location = new System.Drawing.Point(3, 0);
            this._labelExplanation.Name = "_labelExplanation";
            this.tableLayoutPanel1.SetRowSpan(this._labelExplanation, 3);
            this._labelExplanation.Size = new System.Drawing.Size(411, 52);
            this._labelExplanation.TabIndex = 4;
            this._labelExplanation.Text = resources.GetString("_labelExplanation.Text");
            // 
            // _btnOk
            // 
            this._btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._btnOk, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._btnOk, null);
            this.l10NSharpExtender1.SetLocalizingId(this._btnOk, "ShiftClipsDlg._btnShiftClips");
            this._btnOk.Location = new System.Drawing.Point(468, 450);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(75, 23);
            this._btnOk.TabIndex = 1;
            this._btnOk.Text = "Shift Clips";
            this._btnOk.UseVisualStyleBackColor = true;
            this._btnOk.Click += new System.EventHandler(this._btnOk_Click);
            // 
            // l10NSharpExtender1
            // 
            this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
            this.l10NSharpExtender1.PrefixForNewItems = "";
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._btnCancel, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._btnCancel, null);
            this.l10NSharpExtender1.SetLocalizationPriority(this._btnCancel, L10NSharp.LocalizationPriority.High);
            this.l10NSharpExtender1.SetLocalizingId(this._btnCancel, "Common.Cancel");
            this._btnCancel.Location = new System.Drawing.Point(549, 450);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 2;
            this._btnCancel.Text = "&Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // _radioShiftRight
            // 
            this._radioShiftRight.AutoSize = true;
            this._radioShiftRight.Checked = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._radioShiftRight, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._radioShiftRight, null);
            this.l10NSharpExtender1.SetLocalizingId(this._radioShiftRight, "ShiftClipsDlg._radioShiftRight");
            this._radioShiftRight.Location = new System.Drawing.Point(430, 22);
            this._radioShiftRight.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
            this._radioShiftRight.Name = "_radioShiftRight";
            this._radioShiftRight.Size = new System.Drawing.Size(173, 17);
            this._radioShiftRight.TabIndex = 1;
            this._radioShiftRight.TabStop = true;
            this._radioShiftRight.Text = "Shift clips to the following block";
            this._radioShiftRight.UseVisualStyleBackColor = true;
            this._radioShiftRight.CheckedChanged += new System.EventHandler(this.OnShiftDirectionChanged);
            // 
            // _radioShiftLeft
            // 
            this._radioShiftLeft.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._radioShiftLeft, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._radioShiftLeft, null);
            this.l10NSharpExtender1.SetLocalizingId(this._radioShiftLeft, "ShiftClipsDlg._radioShiftLeft");
            this._radioShiftLeft.Location = new System.Drawing.Point(430, 45);
            this._radioShiftLeft.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
            this._radioShiftLeft.Name = "_radioShiftLeft";
            this._radioShiftLeft.Size = new System.Drawing.Size(179, 17);
            this._radioShiftLeft.TabIndex = 3;
            this._radioShiftLeft.Text = "Shift clips to the preceding block";
            this._radioShiftLeft.UseVisualStyleBackColor = true;
            // 
            // _gridScriptLines
            // 
            this._gridScriptLines.AllowUserToAddRows = false;
            this._gridScriptLines.AllowUserToDeleteRows = false;
            this._gridScriptLines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gridScriptLines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this._gridScriptLines.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._gridScriptLines.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._gridScriptLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._gridScriptLines.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colScriptBlockText,
            this.colExistingRecording,
            this.colNewRecording});
            this.tableLayoutPanel1.SetColumnSpan(this._gridScriptLines, 2);
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._gridScriptLines.DefaultCellStyle = dataGridViewCellStyle4;
            this._gridScriptLines.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._gridScriptLines, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._gridScriptLines, null);
            this.l10NSharpExtender1.SetLocalizingId(this._gridScriptLines, "ShiftClipsDlg.GridScriptLines");
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
            this.colScriptBlockText.HeaderText = "_L10N_:ShiftClipsDlg.GridScriptLines.ScriptText!Script Text";
            this.colScriptBlockText.MinimumWidth = 50;
            this.colScriptBlockText.Name = "colScriptBlockText";
            this.colScriptBlockText.ReadOnly = true;
            this.colScriptBlockText.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colExistingRecording
            // 
            this.colExistingRecording.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colExistingRecording.DefaultCellStyle = dataGridViewCellStyle2;
            this.colExistingRecording.HeaderText = "_L10N_:ShiftClipsDlg.GridScriptLines.Existing!Existing";
            this.colExistingRecording.MinimumWidth = 18;
            this.colExistingRecording.Name = "colExistingRecording";
            this.colExistingRecording.ReadOnly = true;
            this.colExistingRecording.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colNewRecording
            // 
            this.colNewRecording.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNewRecording.DefaultCellStyle = dataGridViewCellStyle3;
            this.colNewRecording.HeaderText = "_L10N_:ShiftClipsDlg.GridScriptLines.AfterShifting!After Shifting";
            this.colNewRecording.MinimumWidth = 18;
            this.colNewRecording.Name = "colNewRecording";
            this.colNewRecording.ReadOnly = true;
            this.colNewRecording.Resizable = System.Windows.Forms.DataGridViewTriState.True;
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
            this.tableLayoutPanel1.Controls.Add(this._labelDirection, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this._radioShiftLeft, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._labelExplanation, 0, 0);
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
            this.MinimumSize = new System.Drawing.Size(550, 300);
            this.Name = "ShiftClipsDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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
		private System.Windows.Forms.RadioButton _radioShiftRight;
		private System.Windows.Forms.RadioButton _radioShiftLeft;
		private System.Windows.Forms.DataGridViewTextBoxColumn colScriptBlockText;
		private System.Windows.Forms.DataGridViewImageColumn colExistingRecording;
		private System.Windows.Forms.DataGridViewImageColumn colNewRecording;
		private System.Windows.Forms.Label _labelDirection;
		private System.Windows.Forms.Label _labelExplanation;
	}
}
