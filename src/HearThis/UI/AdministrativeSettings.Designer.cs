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
			System.Windows.Forms.TabControl tabControl1;
			System.Windows.Forms.Label _lblBreakClauses;
			Palaso.UI.WindowsForms.SettingProtection.SettingsProtectionLauncherButton settingsProtectionLauncherButton1;
			System.Windows.Forms.Button _btnCancel;
			System.Windows.Forms.Panel pnlLine;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdministrativeSettings));
			this.tabPageModes = new System.Windows.Forms.TabPage();
			this.lblSelectModes = new System.Windows.Forms.Label();
			this._tableLayoutModes = new System.Windows.Forms.TableLayoutPanel();
			this.lnkNormalRecordingModeSetAsDefault = new System.Windows.Forms.LinkLabel();
			this.NormalRecording = new System.Windows.Forms.CheckBox();
			this.lnkAdministrativeModeSetAsDefault = new System.Windows.Forms.LinkLabel();
			this.Administrator = new System.Windows.Forms.CheckBox();
			this.tabPageSkipping = new System.Windows.Forms.TabPage();
			this._lblSkippingInstructions = new System.Windows.Forms.Label();
			this._btnClearAllSkipInfo = new System.Windows.Forms.Button();
			this._lbSkippedStyles = new System.Windows.Forms.CheckedListBox();
			this.tabPagePunctuation = new System.Windows.Forms.TabPage();
			this._tableLayoutPanelPunctuation = new System.Windows.Forms.TableLayoutPanel();
			this._lblClauseSeparators = new System.Windows.Forms.Label();
			this._chkBreakAtQuotes = new System.Windows.Forms.CheckBox();
			this._txtClauseSeparatorCharacters = new System.Windows.Forms.TextBox();
			this._btnOk = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._lblWarningExistingRecordings = new System.Windows.Forms.Label();
			tabControl1 = new System.Windows.Forms.TabControl();
			_lblBreakClauses = new System.Windows.Forms.Label();
			settingsProtectionLauncherButton1 = new Palaso.UI.WindowsForms.SettingProtection.SettingsProtectionLauncherButton();
			_btnCancel = new System.Windows.Forms.Button();
			pnlLine = new System.Windows.Forms.Panel();
			tabControl1.SuspendLayout();
			this.tabPageModes.SuspendLayout();
			this._tableLayoutModes.SuspendLayout();
			this.tabPageSkipping.SuspendLayout();
			this.tabPagePunctuation.SuspendLayout();
			this._tableLayoutPanelPunctuation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			tabControl1.Controls.Add(this.tabPageModes);
			tabControl1.Controls.Add(this.tabPageSkipping);
			tabControl1.Controls.Add(this.tabPagePunctuation);
			tabControl1.Location = new System.Drawing.Point(12, 12);
			tabControl1.Name = "tabControl1";
			tabControl1.SelectedIndex = 0;
			tabControl1.Size = new System.Drawing.Size(298, 235);
			tabControl1.TabIndex = 3;
			// 
			// tabPageModes
			// 
			this.tabPageModes.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.tabPageModes.Controls.Add(this.lblSelectModes);
			this.tabPageModes.Controls.Add(this._tableLayoutModes);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabPageModes, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabPageModes, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabPageModes, "tabPage1");
			this.tabPageModes.Location = new System.Drawing.Point(4, 22);
			this.tabPageModes.Name = "tabPageModes";
			this.tabPageModes.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageModes.Size = new System.Drawing.Size(290, 209);
			this.tabPageModes.TabIndex = 0;
			this.tabPageModes.Text = "Modes";
			// 
			// lblSelectModes
			// 
			this.lblSelectModes.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblSelectModes, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblSelectModes, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblSelectModes, "label1");
			this.lblSelectModes.Location = new System.Drawing.Point(6, 11);
			this.lblSelectModes.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
			this.lblSelectModes.Name = "lblSelectModes";
			this.lblSelectModes.Size = new System.Drawing.Size(178, 13);
			this.lblSelectModes.TabIndex = 5;
			this.lblSelectModes.Text = "Select the modes to make available:";
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
			this._tableLayoutModes.Location = new System.Drawing.Point(9, 37);
			this._tableLayoutModes.Name = "_tableLayoutModes";
			this._tableLayoutModes.RowCount = 2;
			this._tableLayoutModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutModes.Size = new System.Drawing.Size(230, 64);
			this._tableLayoutModes.TabIndex = 3;
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
			this.lnkNormalRecordingModeSetAsDefault.Location = new System.Drawing.Point(153, 33);
			this.lnkNormalRecordingModeSetAsDefault.Name = "lnkNormalRecordingModeSetAsDefault";
			this.lnkNormalRecordingModeSetAsDefault.Size = new System.Drawing.Size(74, 31);
			this.lnkNormalRecordingModeSetAsDefault.TabIndex = 12;
			this.lnkNormalRecordingModeSetAsDefault.TabStop = true;
			this.lnkNormalRecordingModeSetAsDefault.Text = "Set as Default";
			this.lnkNormalRecordingModeSetAsDefault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
			this.lnkAdministrativeModeSetAsDefault.Location = new System.Drawing.Point(153, 0);
			this.lnkAdministrativeModeSetAsDefault.Name = "lnkAdministrativeModeSetAsDefault";
			this.lnkAdministrativeModeSetAsDefault.Size = new System.Drawing.Size(74, 33);
			this.lnkAdministrativeModeSetAsDefault.TabIndex = 11;
			this.lnkAdministrativeModeSetAsDefault.TabStop = true;
			this.lnkAdministrativeModeSetAsDefault.Text = "Set as Default";
			this.lnkAdministrativeModeSetAsDefault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
			this.Administrator.Size = new System.Drawing.Size(144, 27);
			this.Administrator.TabIndex = 4;
			this.Administrator.Text = "Administrative Setup";
			this.Administrator.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.Administrator.UseVisualStyleBackColor = true;
			// 
			// tabPageSkipping
			// 
			this.tabPageSkipping.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.tabPageSkipping.Controls.Add(this._lblSkippingInstructions);
			this.tabPageSkipping.Controls.Add(this._btnClearAllSkipInfo);
			this.tabPageSkipping.Controls.Add(this._lbSkippedStyles);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabPageSkipping, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabPageSkipping, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabPageSkipping, "tabPage2");
			this.tabPageSkipping.Location = new System.Drawing.Point(4, 22);
			this.tabPageSkipping.Name = "tabPageSkipping";
			this.tabPageSkipping.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSkipping.Size = new System.Drawing.Size(290, 209);
			this.tabPageSkipping.TabIndex = 1;
			this.tabPageSkipping.Text = "Skipping";
			// 
			// _lblSkippingInstructions
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblSkippingInstructions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblSkippingInstructions, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblSkippingInstructions, "label1");
			this._lblSkippingInstructions.Location = new System.Drawing.Point(6, 9);
			this._lblSkippingInstructions.Name = "_lblSkippingInstructions";
			this._lblSkippingInstructions.Size = new System.Drawing.Size(278, 39);
			this._lblSkippingInstructions.TabIndex = 2;
			this._lblSkippingInstructions.Text = "Select any styles whose text should never be recorded for project {0}.";
			// 
			// _btnClearAllSkipInfo
			// 
			this._btnClearAllSkipInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnClearAllSkipInfo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnClearAllSkipInfo, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnClearAllSkipInfo, "button1");
			this._btnClearAllSkipInfo.Location = new System.Drawing.Point(92, 180);
			this._btnClearAllSkipInfo.Name = "_btnClearAllSkipInfo";
			this._btnClearAllSkipInfo.Size = new System.Drawing.Size(107, 23);
			this._btnClearAllSkipInfo.TabIndex = 1;
			this._btnClearAllSkipInfo.Text = "Clear All Skips";
			this._btnClearAllSkipInfo.UseVisualStyleBackColor = true;
			this._btnClearAllSkipInfo.Click += new System.EventHandler(this.HandleClearAllSkipInfo_Click);
			// 
			// _lbSkippedStyles
			// 
			this._lbSkippedStyles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lbSkippedStyles.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lbSkippedStyles, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lbSkippedStyles, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._lbSkippedStyles, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._lbSkippedStyles, "AdministrativeSettings._lbSkippedStyles");
			this._lbSkippedStyles.Location = new System.Drawing.Point(6, 51);
			this._lbSkippedStyles.Name = "_lbSkippedStyles";
			this._lbSkippedStyles.Size = new System.Drawing.Size(278, 109);
			this._lbSkippedStyles.TabIndex = 0;
			// 
			// tabPagePunctuation
			// 
			this.tabPagePunctuation.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.tabPagePunctuation.Controls.Add(this._tableLayoutPanelPunctuation);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabPagePunctuation, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabPagePunctuation, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabPagePunctuation, "tabPage1");
			this.tabPagePunctuation.Location = new System.Drawing.Point(4, 22);
			this.tabPagePunctuation.Name = "tabPagePunctuation";
			this.tabPagePunctuation.Size = new System.Drawing.Size(290, 209);
			this.tabPagePunctuation.TabIndex = 2;
			this.tabPagePunctuation.Text = "Punctuation";
			// 
			// _tableLayoutPanelPunctuation
			// 
			this._tableLayoutPanelPunctuation.ColumnCount = 2;
			this._tableLayoutPanelPunctuation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanelPunctuation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelPunctuation.Controls.Add(this._lblClauseSeparators, 1, 3);
			this._tableLayoutPanelPunctuation.Controls.Add(this._chkBreakAtQuotes, 0, 0);
			this._tableLayoutPanelPunctuation.Controls.Add(_lblBreakClauses, 0, 3);
			this._tableLayoutPanelPunctuation.Controls.Add(this._txtClauseSeparatorCharacters, 0, 4);
			this._tableLayoutPanelPunctuation.Controls.Add(pnlLine, 0, 2);
			this._tableLayoutPanelPunctuation.Controls.Add(this._lblWarningExistingRecordings, 0, 1);
			this._tableLayoutPanelPunctuation.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanelPunctuation.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanelPunctuation.Name = "_tableLayoutPanelPunctuation";
			this._tableLayoutPanelPunctuation.Padding = new System.Windows.Forms.Padding(3);
			this._tableLayoutPanelPunctuation.RowCount = 5;
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.Size = new System.Drawing.Size(290, 209);
			this._tableLayoutPanelPunctuation.TabIndex = 1;
			// 
			// _lblClauseSeparators
			// 
			this._lblClauseSeparators.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblClauseSeparators, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblClauseSeparators, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._lblClauseSeparators, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._lblClauseSeparators, "AdministrativeSettings._lblClauseSeparators");
			this._lblClauseSeparators.Location = new System.Drawing.Point(47, 127);
			this._lblClauseSeparators.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._lblClauseSeparators.Name = "_lblClauseSeparators";
			this._lblClauseSeparators.Size = new System.Drawing.Size(229, 39);
			this._lblClauseSeparators.TabIndex = 2;
			this._lblClauseSeparators.Text = "List the characters that should be used to separate clauses into lines (when that" +
    " option is selected):";
			// 
			// _chkBreakAtQuotes
			// 
			this._chkBreakAtQuotes.AutoSize = true;
			this._chkBreakAtQuotes.Checked = true;
			this._chkBreakAtQuotes.CheckState = System.Windows.Forms.CheckState.Checked;
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._chkBreakAtQuotes, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkBreakAtQuotes, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkBreakAtQuotes, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._chkBreakAtQuotes, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._chkBreakAtQuotes, "AdministrativeSettings._chkBreakAtQuotes");
			this._chkBreakAtQuotes.Location = new System.Drawing.Point(6, 6);
			this._chkBreakAtQuotes.Name = "_chkBreakAtQuotes";
			this._chkBreakAtQuotes.Size = new System.Drawing.Size(195, 17);
			this._chkBreakAtQuotes.TabIndex = 1;
			this._chkBreakAtQuotes.Text = "Treat quotations as separate blocks";
			this._chkBreakAtQuotes.UseVisualStyleBackColor = true;
			this._chkBreakAtQuotes.CheckedChanged += new System.EventHandler(this._chkBreakAtQuotes_CheckedChanged);
			// 
			// _lblBreakClauses
			// 
			_lblBreakClauses.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			_lblBreakClauses.Image = global::HearThis.Properties.Resources.Icon_LineBreak_Comma_Active;
			this.l10NSharpExtender1.SetLocalizableToolTip(_lblBreakClauses, null);
			this.l10NSharpExtender1.SetLocalizationComment(_lblBreakClauses, null);
			this.l10NSharpExtender1.SetLocalizationPriority(_lblBreakClauses, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(_lblBreakClauses, "AdministrativeSettings._lblBreakClauses");
			_lblBreakClauses.Location = new System.Drawing.Point(6, 124);
			_lblBreakClauses.Name = "_lblBreakClauses";
			_lblBreakClauses.Size = new System.Drawing.Size(35, 13);
			_lblBreakClauses.TabIndex = 3;
			// 
			// _txtClauseSeparatorCharacters
			// 
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._txtClauseSeparatorCharacters, 2);
			this._txtClauseSeparatorCharacters.Dock = System.Windows.Forms.DockStyle.Fill;
			this._txtClauseSeparatorCharacters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._txtClauseSeparatorCharacters, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._txtClauseSeparatorCharacters, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._txtClauseSeparatorCharacters, L10NSharp.LocalizationPriority.Low);
			this.l10NSharpExtender1.SetLocalizingId(this._txtClauseSeparatorCharacters, "AdministrativeSettings._txtClauseSeparatorCharacters");
			this._txtClauseSeparatorCharacters.Location = new System.Drawing.Point(6, 169);
			this._txtClauseSeparatorCharacters.Name = "_txtClauseSeparatorCharacters";
			this._txtClauseSeparatorCharacters.Size = new System.Drawing.Size(278, 20);
			this._txtClauseSeparatorCharacters.TabIndex = 4;
			this._txtClauseSeparatorCharacters.Text = ", ; :";
			// 
			// settingsProtectionLauncherButton1
			// 
			settingsProtectionLauncherButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(settingsProtectionLauncherButton1, null);
			this.l10NSharpExtender1.SetLocalizationComment(settingsProtectionLauncherButton1, null);
			this.l10NSharpExtender1.SetLocalizingId(settingsProtectionLauncherButton1, "SettingsProtectionLauncherButton");
			settingsProtectionLauncherButton1.Location = new System.Drawing.Point(12, 255);
			settingsProtectionLauncherButton1.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
			settingsProtectionLauncherButton1.Name = "settingsProtectionLauncherButton1";
			settingsProtectionLauncherButton1.Size = new System.Drawing.Size(298, 37);
			settingsProtectionLauncherButton1.TabIndex = 8;
			// 
			// _btnCancel
			// 
			_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(_btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(_btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(_btnCancel, "RestrictAdministrativeAccess._btnOk");
			_btnCancel.Location = new System.Drawing.Point(164, 308);
			_btnCancel.Name = "_btnCancel";
			_btnCancel.Size = new System.Drawing.Size(75, 23);
			_btnCancel.TabIndex = 9;
			_btnCancel.Text = "Cancel";
			_btnCancel.UseVisualStyleBackColor = true;
			// 
			// _btnOk
			// 
			this._btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnOk, "RestrictAdministrativeAccess._btnOk");
			this._btnOk.Location = new System.Drawing.Point(83, 308);
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
			// pnlLine
			// 
			this._tableLayoutPanelPunctuation.SetColumnSpan(pnlLine, 2);
			pnlLine.Dock = System.Windows.Forms.DockStyle.Bottom;
			pnlLine.Location = new System.Drawing.Point(6, 119);
			pnlLine.Name = "pnlLine";
			pnlLine.Size = new System.Drawing.Size(278, 2);
			pnlLine.TabIndex = 10;
			// 
			// _lblWarningExistingRecordings
			// 
			this._lblWarningExistingRecordings.AutoSize = true;
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._lblWarningExistingRecordings, 2);
			this._lblWarningExistingRecordings.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblWarningExistingRecordings, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblWarningExistingRecordings, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblWarningExistingRecordings, "label1");
			this._lblWarningExistingRecordings.Location = new System.Drawing.Point(6, 26);
			this._lblWarningExistingRecordings.Name = "_lblWarningExistingRecordings";
			this._lblWarningExistingRecordings.Size = new System.Drawing.Size(276, 78);
			this._lblWarningExistingRecordings.TabIndex = 11;
			this._lblWarningExistingRecordings.Text = resources.GetString("_lblWarningExistingRecordings.Text");
			// 
			// AdministrativeSettings
			// 
			this.AcceptButton = this._btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = _btnCancel;
			this.ClientSize = new System.Drawing.Size(322, 343);
			this.Controls.Add(_btnCancel);
			this.Controls.Add(settingsProtectionLauncherButton1);
			this.Controls.Add(tabControl1);
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
			tabControl1.ResumeLayout(false);
			this.tabPageModes.ResumeLayout(false);
			this.tabPageModes.PerformLayout();
			this._tableLayoutModes.ResumeLayout(false);
			this._tableLayoutModes.PerformLayout();
			this.tabPageSkipping.ResumeLayout(false);
			this.tabPagePunctuation.ResumeLayout(false);
			this._tableLayoutPanelPunctuation.ResumeLayout(false);
			this._tableLayoutPanelPunctuation.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _btnOk;
		private System.Windows.Forms.CheckBox Administrator;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label lblSelectModes;
		private System.Windows.Forms.CheckBox NormalRecording;
		private System.Windows.Forms.LinkLabel lnkAdministrativeModeSetAsDefault;
		private System.Windows.Forms.LinkLabel lnkNormalRecordingModeSetAsDefault;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutModes;
		private System.Windows.Forms.TabPage tabPageModes;
		private System.Windows.Forms.TabPage tabPageSkipping;
		private System.Windows.Forms.Label _lblSkippingInstructions;
		private System.Windows.Forms.Button _btnClearAllSkipInfo;
		private System.Windows.Forms.CheckedListBox _lbSkippedStyles;
		private System.Windows.Forms.TabPage tabPagePunctuation;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelPunctuation;
		private System.Windows.Forms.CheckBox _chkBreakAtQuotes;
		private System.Windows.Forms.Label _lblClauseSeparators;
		private System.Windows.Forms.TextBox _txtClauseSeparatorCharacters;
		private System.Windows.Forms.Label _lblWarningExistingRecordings;
	}
}