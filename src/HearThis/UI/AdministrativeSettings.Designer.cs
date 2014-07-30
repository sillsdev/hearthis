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
			this._tableLayoutPanelModes = new System.Windows.Forms.TableLayoutPanel();
			this.lnkNormalRecordingModeSetAsDefault = new System.Windows.Forms.LinkLabel();
			this.Administrative = new System.Windows.Forms.CheckBox();
			this.lblSelectModes = new System.Windows.Forms.Label();
			this.NormalRecording = new System.Windows.Forms.CheckBox();
			this.settingsProtectionLauncherButton1 = new Palaso.UI.WindowsForms.SettingProtection.SettingsProtectionLauncherButton();
			this._separatorLine = new System.Windows.Forms.Label();
			this.lnkAdministrativeModeSetAsDefault = new System.Windows.Forms.LinkLabel();
			this._btnOk = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._tableLayoutPanelModes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// _tableLayoutPanelModes
			// 
			this._tableLayoutPanelModes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutPanelModes.AutoSize = true;
			this._tableLayoutPanelModes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableLayoutPanelModes.ColumnCount = 2;
			this._tableLayoutPanelModes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelModes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanelModes.Controls.Add(this.lnkNormalRecordingModeSetAsDefault, 1, 2);
			this._tableLayoutPanelModes.Controls.Add(this.Administrative, 0, 1);
			this._tableLayoutPanelModes.Controls.Add(this.lblSelectModes, 0, 0);
			this._tableLayoutPanelModes.Controls.Add(this.NormalRecording, 0, 2);
			this._tableLayoutPanelModes.Controls.Add(this.settingsProtectionLauncherButton1, 0, 4);
			this._tableLayoutPanelModes.Controls.Add(this._separatorLine, 0, 3);
			this._tableLayoutPanelModes.Controls.Add(this.lnkAdministrativeModeSetAsDefault, 1, 1);
			this._tableLayoutPanelModes.Location = new System.Drawing.Point(12, 12);
			this._tableLayoutPanelModes.Name = "_tableLayoutPanelModes";
			this._tableLayoutPanelModes.RowCount = 5;
			this._tableLayoutPanelModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelModes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanelModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelModes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanelModes.Size = new System.Drawing.Size(297, 149);
			this._tableLayoutPanelModes.TabIndex = 1;
			// 
			// lnkNormalRecordingModeSetAsDefault
			// 
			this.lnkNormalRecordingModeSetAsDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkNormalRecordingModeSetAsDefault.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lnkNormalRecordingModeSetAsDefault, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lnkNormalRecordingModeSetAsDefault, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.lnkNormalRecordingModeSetAsDefault, L10NSharp.LocalizationPriority.Medium);
			this.l10NSharpExtender1.SetLocalizingId(this.lnkNormalRecordingModeSetAsDefault, "AdministrativeSettings.lnkSetAsDefault");
			this.lnkNormalRecordingModeSetAsDefault.Location = new System.Drawing.Point(220, 56);
			this.lnkNormalRecordingModeSetAsDefault.Name = "lnkNormalRecordingModeSetAsDefault";
			this.lnkNormalRecordingModeSetAsDefault.Size = new System.Drawing.Size(74, 31);
			this.lnkNormalRecordingModeSetAsDefault.TabIndex = 12;
			this.lnkNormalRecordingModeSetAsDefault.TabStop = true;
			this.lnkNormalRecordingModeSetAsDefault.Text = "Set as Default";
			this.lnkNormalRecordingModeSetAsDefault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Administrative
			// 
			this.Administrative.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.Administrative.AutoSize = true;
			this.Administrative.Checked = true;
			this.Administrative.CheckState = System.Windows.Forms.CheckState.Checked;
			this.Administrative.Image = global::HearThis.Properties.Resources._1406663178_tick_circle_frame;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.Administrative, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.Administrative, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.Administrative, L10NSharp.LocalizationPriority.Medium);
			this.l10NSharpExtender1.SetLocalizingId(this.Administrative, "AdministrativeSettings._chkAdministrativeMode");
			this.Administrative.Location = new System.Drawing.Point(3, 26);
			this.Administrative.MinimumSize = new System.Drawing.Size(0, 27);
			this.Administrative.Name = "Administrative";
			this.Administrative.Padding = new System.Windows.Forms.Padding(3);
			this.Administrative.Size = new System.Drawing.Size(143, 27);
			this.Administrative.TabIndex = 4;
			this.Administrative.Text = "Administrative Mode";
			this.Administrative.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.Administrative.UseVisualStyleBackColor = true;
			// 
			// lblSelectModes
			// 
			this.lblSelectModes.AutoSize = true;
			this._tableLayoutPanelModes.SetColumnSpan(this.lblSelectModes, 2);
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
			this.l10NSharpExtender1.SetLocalizationPriority(this.NormalRecording, L10NSharp.LocalizationPriority.Medium);
			this.l10NSharpExtender1.SetLocalizingId(this.NormalRecording, "AdministrativeSettings._chkNormalRecordingMode");
			this.NormalRecording.Location = new System.Drawing.Point(3, 59);
			this.NormalRecording.MinimumSize = new System.Drawing.Size(0, 25);
			this.NormalRecording.Name = "NormalRecording";
			this.NormalRecording.Padding = new System.Windows.Forms.Padding(3);
			this.NormalRecording.Size = new System.Drawing.Size(147, 25);
			this.NormalRecording.TabIndex = 6;
			this.NormalRecording.Text = "Normal Recording Mode";
			this.NormalRecording.UseVisualStyleBackColor = true;
			// 
			// settingsProtectionLauncherButton1
			// 
			this._tableLayoutPanelModes.SetColumnSpan(this.settingsProtectionLauncherButton1, 2);
			this.settingsProtectionLauncherButton1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.settingsProtectionLauncherButton1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.settingsProtectionLauncherButton1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.settingsProtectionLauncherButton1, "SettingsProtectionLauncherButton");
			this.settingsProtectionLauncherButton1.Location = new System.Drawing.Point(0, 112);
			this.settingsProtectionLauncherButton1.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.settingsProtectionLauncherButton1.Name = "settingsProtectionLauncherButton1";
			this.settingsProtectionLauncherButton1.Size = new System.Drawing.Size(297, 37);
			this.settingsProtectionLauncherButton1.TabIndex = 7;
			// 
			// _separatorLine
			// 
			this._separatorLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._tableLayoutPanelModes.SetColumnSpan(this._separatorLine, 2);
			this._separatorLine.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._separatorLine, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._separatorLine, null);
			this.l10NSharpExtender1.SetLocalizingId(this._separatorLine, "label1");
			this._separatorLine.Location = new System.Drawing.Point(3, 105);
			this._separatorLine.Name = "_separatorLine";
			this._separatorLine.Size = new System.Drawing.Size(291, 2);
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
			this.l10NSharpExtender1.SetLocalizationPriority(this.lnkAdministrativeModeSetAsDefault, L10NSharp.LocalizationPriority.Medium);
			this.l10NSharpExtender1.SetLocalizingId(this.lnkAdministrativeModeSetAsDefault, "AdministrativeSettings.lnkSetAsDefault");
			this.lnkAdministrativeModeSetAsDefault.Location = new System.Drawing.Point(220, 23);
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
			this._btnOk.Location = new System.Drawing.Point(123, 173);
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
			// AdministrativeSettings
			// 
			this.AcceptButton = this._btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(321, 208);
			this.Controls.Add(this._tableLayoutPanelModes);
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
			this.Text = "Administrative Settings";
			this._tableLayoutPanelModes.ResumeLayout(false);
			this._tableLayoutPanelModes.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelModes;
		private System.Windows.Forms.Button _btnOk;
		private System.Windows.Forms.CheckBox Administrative;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label lblSelectModes;
		private System.Windows.Forms.CheckBox NormalRecording;
		private Palaso.UI.WindowsForms.SettingProtection.SettingsProtectionLauncherButton settingsProtectionLauncherButton1;
		private System.Windows.Forms.Label _separatorLine;
		private System.Windows.Forms.LinkLabel lnkAdministrativeModeSetAsDefault;
		private System.Windows.Forms.LinkLabel lnkNormalRecordingModeSetAsDefault;
	}
}