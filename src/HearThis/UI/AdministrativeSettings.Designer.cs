namespace HearThis.UI
{
	partial class AdministrativeSettings
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
			System.Windows.Forms.TableLayoutPanel _tableLayoutMain;
			this.lnkNormalRecordingModeSetAsDefault = new System.Windows.Forms.LinkLabel();
			this.Administrator = new System.Windows.Forms.CheckBox();
			this.lblSelectModes = new System.Windows.Forms.Label();
			this.NormalRecording = new System.Windows.Forms.CheckBox();
			this.settingsProtectionLauncherButton1 = new Palaso.UI.WindowsForms.SettingProtection.SettingsProtectionLauncherButton();
			this._separatorLine = new System.Windows.Forms.Label();
			this.lnkAdministrativeModeSetAsDefault = new System.Windows.Forms.LinkLabel();
			this._btnOk = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._tableLayoutModes = new System.Windows.Forms.TableLayoutPanel();
			_tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
			_tableLayoutMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this._tableLayoutModes.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayoutMain
			// 
			_tableLayoutMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			_tableLayoutMain.AutoSize = true;
			_tableLayoutMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			_tableLayoutMain.ColumnCount = 1;
			_tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			_tableLayoutMain.Controls.Add(this._tableLayoutModes, 0, 1);
			_tableLayoutMain.Controls.Add(this.lblSelectModes, 0, 0);
			_tableLayoutMain.Controls.Add(this.settingsProtectionLauncherButton1, 0, 3);
			_tableLayoutMain.Controls.Add(this._separatorLine, 0, 2);
			_tableLayoutMain.Location = new System.Drawing.Point(12, 12);
			_tableLayoutMain.Name = "_tableLayoutMain";
			_tableLayoutMain.RowCount = 4;
			_tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			_tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			_tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			_tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			_tableLayoutMain.Size = new System.Drawing.Size(225, 155);
			_tableLayoutMain.TabIndex = 1;
			// 
			// lnkNormalRecordingModeSetAsDefault
			// 
			this.lnkNormalRecordingModeSetAsDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkNormalRecordingModeSetAsDefault.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lnkNormalRecordingModeSetAsDefault, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lnkNormalRecordingModeSetAsDefault, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lnkNormalRecordingModeSetAsDefault, "AdministrativeSettings.lnkSetAsDefault");
			this.lnkNormalRecordingModeSetAsDefault.Location = new System.Drawing.Point(126, 33);
			this.lnkNormalRecordingModeSetAsDefault.Name = "lnkNormalRecordingModeSetAsDefault";
			this.lnkNormalRecordingModeSetAsDefault.Size = new System.Drawing.Size(74, 31);
			this.lnkNormalRecordingModeSetAsDefault.TabIndex = 12;
			this.lnkNormalRecordingModeSetAsDefault.TabStop = true;
			this.lnkNormalRecordingModeSetAsDefault.Text = "Set as Default";
			this.lnkNormalRecordingModeSetAsDefault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Administrator
			// 
			this.Administrator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.Administrator.AutoSize = true;
			this.Administrator.Checked = true;
			this.Administrator.CheckState = System.Windows.Forms.CheckState.Checked;
			this.Administrator.Image = global::HearThis.Properties.Resources._1406663178_tick_circle_frame;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.Administrator, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.Administrator, null);
			this.l10NSharpExtender1.SetLocalizingId(this.Administrator, "AdministrativeSettings._chkAdministrativeMode");
			this.Administrator.Location = new System.Drawing.Point(3, 3);
			this.Administrator.MinimumSize = new System.Drawing.Size(0, 27);
			this.Administrator.Name = "Administrator";
			this.Administrator.Padding = new System.Windows.Forms.Padding(3);
			this.Administrator.Size = new System.Drawing.Size(108, 27);
			this.Administrator.TabIndex = 4;
			this.Administrator.Text = "Administrator";
			this.Administrator.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.Administrator.UseVisualStyleBackColor = true;
			// 
			// lblSelectModes
			// 
			this.lblSelectModes.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblSelectModes, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblSelectModes, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblSelectModes, "label1");
			this.lblSelectModes.Location = new System.Drawing.Point(3, 0);
			this.lblSelectModes.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
			this.lblSelectModes.Name = "lblSelectModes";
			this.lblSelectModes.Size = new System.Drawing.Size(178, 13);
			this.lblSelectModes.TabIndex = 5;
			this.lblSelectModes.Text = "Select the modes to make available:";
			// 
			// NormalRecording
			// 
			this.NormalRecording.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.NormalRecording.AutoSize = true;
			this.NormalRecording.Checked = true;
			this.NormalRecording.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.NormalRecording, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.NormalRecording, null);
			this.l10NSharpExtender1.SetLocalizingId(this.NormalRecording, "AdministrativeSettings._chkNormalRecordingMode");
			this.NormalRecording.Location = new System.Drawing.Point(3, 36);
			this.NormalRecording.MinimumSize = new System.Drawing.Size(0, 25);
			this.NormalRecording.Name = "NormalRecording";
			this.NormalRecording.Padding = new System.Windows.Forms.Padding(3);
			this.NormalRecording.Size = new System.Drawing.Size(117, 25);
			this.NormalRecording.TabIndex = 6;
			this.NormalRecording.Text = "Normal Recording";
			this.NormalRecording.UseVisualStyleBackColor = true;
			// 
			// settingsProtectionLauncherButton1
			// 
			this.settingsProtectionLauncherButton1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.settingsProtectionLauncherButton1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.settingsProtectionLauncherButton1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.settingsProtectionLauncherButton1, "SettingsProtectionLauncherButton");
			this.settingsProtectionLauncherButton1.Location = new System.Drawing.Point(0, 118);
			this.settingsProtectionLauncherButton1.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.settingsProtectionLauncherButton1.Name = "settingsProtectionLauncherButton1";
			this.settingsProtectionLauncherButton1.Size = new System.Drawing.Size(225, 37);
			this.settingsProtectionLauncherButton1.TabIndex = 7;
			// 
			// _separatorLine
			// 
			this._separatorLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._separatorLine.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._separatorLine, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._separatorLine, null);
			this.l10NSharpExtender1.SetLocalizingId(this._separatorLine, "label1");
			this._separatorLine.Location = new System.Drawing.Point(3, 111);
			this._separatorLine.Name = "_separatorLine";
			this._separatorLine.Size = new System.Drawing.Size(219, 2);
			this._separatorLine.TabIndex = 8;
			// 
			// lnkAdministrativeModeSetAsDefault
			// 
			this.lnkAdministrativeModeSetAsDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkAdministrativeModeSetAsDefault.AutoSize = true;
			this.lnkAdministrativeModeSetAsDefault.Enabled = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lnkAdministrativeModeSetAsDefault, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lnkAdministrativeModeSetAsDefault, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lnkAdministrativeModeSetAsDefault, "AdministrativeSettings.lnkSetAsDefault");
			this.lnkAdministrativeModeSetAsDefault.Location = new System.Drawing.Point(126, 0);
			this.lnkAdministrativeModeSetAsDefault.Name = "lnkAdministrativeModeSetAsDefault";
			this.lnkAdministrativeModeSetAsDefault.Size = new System.Drawing.Size(74, 33);
			this.lnkAdministrativeModeSetAsDefault.TabIndex = 11;
			this.lnkAdministrativeModeSetAsDefault.TabStop = true;
			this.lnkAdministrativeModeSetAsDefault.Text = "Set as Default";
			this.lnkAdministrativeModeSetAsDefault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _btnOk
			// 
			this._btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnOk, "RestrictAdministrativeAccess._btnOk");
			this._btnOk.Location = new System.Drawing.Point(87, 183);
			this._btnOk.Name = "_btnOk";
			this._btnOk.Size = new System.Drawing.Size(75, 23);
			this._btnOk.TabIndex = 2;
			this._btnOk.Text = "OK";
			this._btnOk.UseVisualStyleBackColor = true;
			this._btnOk.Click += new System.EventHandler(this.HandleOkButtonClick);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = null;
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// _tableLayoutModes
			// 
			this._tableLayoutModes.AutoSize = true;
			this._tableLayoutModes.ColumnCount = 2;
			this._tableLayoutModes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutModes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutModes.Controls.Add(this.lnkNormalRecordingModeSetAsDefault, 1, 1);
			this._tableLayoutModes.Controls.Add(this.NormalRecording, 0, 1);
			this._tableLayoutModes.Controls.Add(this.lnkAdministrativeModeSetAsDefault, 1, 0);
			this._tableLayoutModes.Controls.Add(this.Administrator, 0, 0);
			this._tableLayoutModes.Location = new System.Drawing.Point(3, 26);
			this._tableLayoutModes.Name = "_tableLayoutModes";
			this._tableLayoutModes.RowCount = 2;
			this._tableLayoutModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutModes.Size = new System.Drawing.Size(203, 64);
			this._tableLayoutModes.TabIndex = 3;
			// 
			// AdministrativeSettings
			// 
			this.AcceptButton = this._btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(249, 218);
			this.Controls.Add(_tableLayoutMain);
			this.Controls.Add(this._btnOk);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "RestrictAdministrativeAccess.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AdministrativeSettings";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Administrator Settings";
			_tableLayoutMain.ResumeLayout(false);
			_tableLayoutMain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this._tableLayoutModes.ResumeLayout(false);
			this._tableLayoutModes.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _btnOk;
		private System.Windows.Forms.CheckBox Administrator;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label lblSelectModes;
		private System.Windows.Forms.CheckBox NormalRecording;
		private Palaso.UI.WindowsForms.SettingProtection.SettingsProtectionLauncherButton settingsProtectionLauncherButton1;
		private System.Windows.Forms.Label _separatorLine;
		private System.Windows.Forms.LinkLabel lnkAdministrativeModeSetAsDefault;
		private System.Windows.Forms.LinkLabel lnkNormalRecordingModeSetAsDefault;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutModes;
	}
}