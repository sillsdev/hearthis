namespace HearThis.UI
{
	partial class DataMigrationReportNagDlg
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
				Program.UnregisterStringsLocalized(HandleStringsLocalized);
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
			this.aboutHearThisPack = new System.Windows.Forms.Label();
			this._chkDoNotNagAnymore = new System.Windows.Forms.CheckBox();
			this._okButton = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._chkDelete = new System.Windows.Forms.CheckBox();
			this._linkHelp = new System.Windows.Forms.LinkLabel();
			this._linkReport = new System.Windows.Forms.LinkLabel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// aboutHearThisPack
			// 
			this.aboutHearThisPack.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.aboutHearThisPack, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.aboutHearThisPack, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.aboutHearThisPack, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.aboutHearThisPack, "DataMigrationReportNagDlg.aboutHearThisPack");
			this.aboutHearThisPack.Location = new System.Drawing.Point(3, 0);
			this.aboutHearThisPack.Name = "aboutHearThisPack";
			this.aboutHearThisPack.Size = new System.Drawing.Size(495, 13);
			this.aboutHearThisPack.TabIndex = 0;
			this.aboutHearThisPack.Text = "During the last data migration, HearThis encountered some things that it was not " +
    "able to migrate cleanly.";
			// 
			// _chkDoNotNagAnymore
			// 
			this._chkDoNotNagAnymore.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkDoNotNagAnymore, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkDoNotNagAnymore, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._chkDoNotNagAnymore, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._chkDoNotNagAnymore, "DataMigrationReportNagDlg._chkDoNotNagAnymore");
			this._chkDoNotNagAnymore.Location = new System.Drawing.Point(3, 78);
			this._chkDoNotNagAnymore.Name = "_chkDoNotNagAnymore";
			this._chkDoNotNagAnymore.Size = new System.Drawing.Size(272, 17);
			this._chkDoNotNagAnymore.TabIndex = 2;
			this._chkDoNotNagAnymore.Text = "I have taken care of this. Do not show this anymore.";
			this._chkDoNotNagAnymore.UseVisualStyleBackColor = true;
			this._chkDoNotNagAnymore.CheckedChanged += new System.EventHandler(this._chkDoNotNagAnymore_CheckedChanged);
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._okButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._okButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._okButton, "Common.OK");
			this._okButton.Location = new System.Drawing.Point(455, 179);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 3;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "";
			// 
			// _chkDelete
			// 
			this._chkDelete.AutoSize = true;
			this._chkDelete.Enabled = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkDelete, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkDelete, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._chkDelete, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._chkDelete, "DataMigrationReportNagDlg._chkDelete");
			this._chkDelete.Location = new System.Drawing.Point(3, 101);
			this._chkDelete.Name = "_chkDelete";
			this._chkDelete.Size = new System.Drawing.Size(105, 17);
			this._chkDelete.TabIndex = 3;
			this._chkDelete.Text = "Delete the report";
			this._chkDelete.UseVisualStyleBackColor = true;
			// 
			// _linkHelp
			// 
			this._linkHelp.AutoSize = true;
			this._linkHelp.LinkArea = new System.Windows.Forms.LinkArea(91, 96);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._linkHelp, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._linkHelp, "Put curly braces around the text that should appear as the hyperlink.");
			this.l10NSharpExtender1.SetLocalizingId(this._linkHelp, "DataMigrationReportNagDlg._linkHelp");
			this._linkHelp.Location = new System.Drawing.Point(3, 42);
			this._linkHelp.Name = "_linkHelp";
			this._linkHelp.Padding = new System.Windows.Forms.Padding(0, 0, 0, 16);
			this._linkHelp.Size = new System.Drawing.Size(470, 33);
			this._linkHelp.TabIndex = 4;
			this._linkHelp.TabStop = true;
			this._linkHelp.Text = "If after looking at the report you do not know how to fix things yourself, please" +
    " get help {here}.";
			this._linkHelp.UseCompatibleTextRendering = true;
			this._linkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked);
			// 
			// _linkReport
			// 
			this._linkReport.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._linkReport, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._linkReport, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._linkReport, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._linkReport, "DataMigrationReportNagDlg._linkReport");
			this._linkReport.Location = new System.Drawing.Point(3, 13);
			this._linkReport.Name = "_linkReport";
			this._linkReport.Padding = new System.Windows.Forms.Padding(0, 8, 0, 8);
			this._linkReport.Size = new System.Drawing.Size(68, 29);
			this._linkReport.TabIndex = 6;
			this._linkReport.TabStop = true;
			this._linkReport.Text = "Open Report";
			this._linkReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._linkReport, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.aboutHearThisPack, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._chkDoNotNagAnymore, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this._chkDelete, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this._linkHelp, 0, 2);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(516, 149);
			this.tableLayoutPanel1.TabIndex = 5;
			// 
			// DataMigrationReportNagDlg
			// 
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(541, 214);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this._okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this, "DataMigrationReportNagDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataMigrationReportNagDlg";
			this.Text = "Data Migration Report";
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label aboutHearThisPack;
		private System.Windows.Forms.CheckBox _chkDoNotNagAnymore;
		private System.Windows.Forms.Button _okButton;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.LinkLabel _linkReport;
		private System.Windows.Forms.CheckBox _chkDelete;
		private System.Windows.Forms.LinkLabel _linkHelp;
	}
}
