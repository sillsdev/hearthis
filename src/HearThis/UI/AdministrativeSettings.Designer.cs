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
			if (disposing)
			{
				if (components != null)
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
			System.Windows.Forms.Label _lblBreakClauses;
			System.Windows.Forms.Panel pnlLine;
			SIL.Windows.Forms.SettingProtection.SettingsProtectionLauncherButton settingsProtectionLauncherButton1;
			System.Windows.Forms.Button _btnCancel;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdministrativeSettings));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageModes = new System.Windows.Forms.TabPage();
			this.lblSelectModes = new System.Windows.Forms.Label();
			this._tableLayoutModes = new System.Windows.Forms.TableLayoutPanel();
			this.lnkNormalRecordingModeSetAsDefault = new System.Windows.Forms.LinkLabel();
			this.NormalRecording = new System.Windows.Forms.CheckBox();
			this.lnkAdministrativeModeSetAsDefault = new System.Windows.Forms.LinkLabel();
			this.Administrator = new System.Windows.Forms.CheckBox();
			this.tabPageSkipping = new System.Windows.Forms.TabPage();
			this._tableLayoutPanelSkipping = new System.Windows.Forms.TableLayoutPanel();
			this._lblSkippingInstructions = new System.Windows.Forms.Label();
			this._lbSkippedStyles = new System.Windows.Forms.CheckedListBox();
			this._chkShowSkipButton = new System.Windows.Forms.CheckBox();
			this._btnClearAllSkipInfo = new System.Windows.Forms.Button();
			this.tabPagePunctuation = new System.Windows.Forms.TabPage();
			this._tableLayoutPanelPunctuation = new System.Windows.Forms.TableLayoutPanel();
			this._lblClauseSeparators = new System.Windows.Forms.Label();
			this._lblAdditionalLineBreakCharacters = new System.Windows.Forms.Label();
			this._txtAdditionalBlockSeparators = new System.Windows.Forms.TextBox();
			this._txtClauseSeparatorCharacters = new System.Windows.Forms.TextBox();
			this._lblBreakBlocks = new System.Windows.Forms.Label();
			this._lblWarningExistingRecordings = new System.Windows.Forms.Label();
			this._chkBreakAtQuotes = new System.Windows.Forms.CheckBox();
			this._chkBreakAtParagraphBreaks = new System.Windows.Forms.CheckBox();
			this._cboSentenceEndingWhitespace = new CheckComboBoxTest.CheckedComboBox();
			this._cboPauseWhitespace = new CheckComboBoxTest.CheckedComboBox();
			this.tabPageInterface = new System.Windows.Forms.TabPage();
			this._groupAdvancedUI = new System.Windows.Forms.GroupBox();
			this._tableLayoutPanelAdvancedUI = new System.Windows.Forms.TableLayoutPanel();
			this._chkEnableClipShifting = new System.Windows.Forms.CheckBox();
			this._lblShiftClipsMenuWarning = new System.Windows.Forms.Label();
			this._lblShiftClipsExplanation = new System.Windows.Forms.Label();
			this._chkShowBookAndChapterLabels = new System.Windows.Forms.CheckBox();
			this.lblColorSchemeChangeRestartWarning = new System.Windows.Forms.Label();
			this._cboColorScheme = new System.Windows.Forms.ComboBox();
			this.lblInterface = new System.Windows.Forms.Label();
			this._btnOk = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			_lblBreakClauses = new System.Windows.Forms.Label();
			pnlLine = new System.Windows.Forms.Panel();
			settingsProtectionLauncherButton1 = new SIL.Windows.Forms.SettingProtection.SettingsProtectionLauncherButton();
			_btnCancel = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPageModes.SuspendLayout();
			this._tableLayoutModes.SuspendLayout();
			this.tabPageSkipping.SuspendLayout();
			this._tableLayoutPanelSkipping.SuspendLayout();
			this.tabPagePunctuation.SuspendLayout();
			this._tableLayoutPanelPunctuation.SuspendLayout();
			this.tabPageInterface.SuspendLayout();
			this._groupAdvancedUI.SuspendLayout();
			this._tableLayoutPanelAdvancedUI.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// _lblBreakClauses
			// 
			_lblBreakClauses.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			_lblBreakClauses.Image = global::HearThis.Properties.Resources.BottomToolbar_BreakOnCommas;
			this.l10NSharpExtender1.SetLocalizableToolTip(_lblBreakClauses, null);
			this.l10NSharpExtender1.SetLocalizationComment(_lblBreakClauses, null);
			this.l10NSharpExtender1.SetLocalizationPriority(_lblBreakClauses, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(_lblBreakClauses, "AdministrativeSettings._lblBreakClauses");
			_lblBreakClauses.Location = new System.Drawing.Point(19, 312);
			_lblBreakClauses.Margin = new System.Windows.Forms.Padding(4, 7, 4, 0);
			_lblBreakClauses.Name = "_lblBreakClauses";
			_lblBreakClauses.Size = new System.Drawing.Size(47, 16);
			_lblBreakClauses.TabIndex = 3;
			// 
			// pnlLine
			// 
			pnlLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._tableLayoutPanelPunctuation.SetColumnSpan(pnlLine, 3);
			pnlLine.Dock = System.Windows.Forms.DockStyle.Bottom;
			pnlLine.Location = new System.Drawing.Point(19, 294);
			pnlLine.Margin = new System.Windows.Forms.Padding(4, 12, 4, 7);
			pnlLine.Name = "pnlLine";
			pnlLine.Size = new System.Drawing.Size(445, 4);
			pnlLine.TabIndex = 10;
			// 
			// settingsProtectionLauncherButton1
			// 
			settingsProtectionLauncherButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(settingsProtectionLauncherButton1, null);
			this.l10NSharpExtender1.SetLocalizationComment(settingsProtectionLauncherButton1, null);
			this.l10NSharpExtender1.SetLocalizingId(settingsProtectionLauncherButton1, "AdministrativeSettings.SettingsProtectionLauncherButton");
			settingsProtectionLauncherButton1.Location = new System.Drawing.Point(12, 479);
			settingsProtectionLauncherButton1.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			settingsProtectionLauncherButton1.Name = "settingsProtectionLauncherButton1";
			settingsProtectionLauncherButton1.Size = new System.Drawing.Size(273, 46);
			settingsProtectionLauncherButton1.TabIndex = 8;
			// 
			// _btnCancel
			// 
			_btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(_btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(_btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(_btnCancel, "Common.Cancel");
			_btnCancel.Location = new System.Drawing.Point(397, 496);
			_btnCancel.Margin = new System.Windows.Forms.Padding(4);
			_btnCancel.Name = "_btnCancel";
			_btnCancel.Size = new System.Drawing.Size(100, 28);
			_btnCancel.TabIndex = 9;
			_btnCancel.Text = "&Cancel";
			_btnCancel.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPageModes);
			this.tabControl1.Controls.Add(this.tabPageSkipping);
			this.tabControl1.Controls.Add(this.tabPagePunctuation);
			this.tabControl1.Controls.Add(this.tabPageInterface);
			this.tabControl1.Location = new System.Drawing.Point(16, 15);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(491, 454);
			this.tabControl1.TabIndex = 3;
			// 
			// tabPageModes
			// 
			this.tabPageModes.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.tabPageModes.Controls.Add(this.lblSelectModes);
			this.tabPageModes.Controls.Add(this._tableLayoutModes);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabPageModes, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabPageModes, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabPageModes, "AdministrativeSettings.tabPageModes");
			this.tabPageModes.Location = new System.Drawing.Point(4, 25);
			this.tabPageModes.Margin = new System.Windows.Forms.Padding(4);
			this.tabPageModes.Name = "tabPageModes";
			this.tabPageModes.Padding = new System.Windows.Forms.Padding(4);
			this.tabPageModes.Size = new System.Drawing.Size(483, 425);
			this.tabPageModes.TabIndex = 0;
			this.tabPageModes.Text = "Modes";
			// 
			// lblSelectModes
			// 
			this.lblSelectModes.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblSelectModes, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblSelectModes, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblSelectModes, "AdministrativeSettings.lblSelectModes");
			this.lblSelectModes.Location = new System.Drawing.Point(15, 14);
			this.lblSelectModes.Margin = new System.Windows.Forms.Padding(0, 0, 4, 12);
			this.lblSelectModes.Name = "lblSelectModes";
			this.lblSelectModes.Size = new System.Drawing.Size(225, 16);
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
			this._tableLayoutModes.Location = new System.Drawing.Point(15, 46);
			this._tableLayoutModes.Margin = new System.Windows.Forms.Padding(0, 4, 4, 4);
			this._tableLayoutModes.Name = "_tableLayoutModes";
			this._tableLayoutModes.RowCount = 2;
			this._tableLayoutModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutModes.Size = new System.Drawing.Size(361, 80);
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
			this.lnkNormalRecordingModeSetAsDefault.Location = new System.Drawing.Point(181, 41);
			this.lnkNormalRecordingModeSetAsDefault.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lnkNormalRecordingModeSetAsDefault.Name = "lnkNormalRecordingModeSetAsDefault";
			this.lnkNormalRecordingModeSetAsDefault.Size = new System.Drawing.Size(176, 39);
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
			this.NormalRecording.Location = new System.Drawing.Point(4, 45);
			this.NormalRecording.Margin = new System.Windows.Forms.Padding(4);
			this.NormalRecording.MinimumSize = new System.Drawing.Size(0, 31);
			this.NormalRecording.Name = "NormalRecording";
			this.NormalRecording.Padding = new System.Windows.Forms.Padding(0, 4, 4, 4);
			this.NormalRecording.Size = new System.Drawing.Size(141, 31);
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
			this.lnkAdministrativeModeSetAsDefault.Location = new System.Drawing.Point(181, 0);
			this.lnkAdministrativeModeSetAsDefault.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lnkAdministrativeModeSetAsDefault.Name = "lnkAdministrativeModeSetAsDefault";
			this.lnkAdministrativeModeSetAsDefault.Size = new System.Drawing.Size(176, 41);
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
			this.Administrator.Location = new System.Drawing.Point(4, 4);
			this.Administrator.Margin = new System.Windows.Forms.Padding(4);
			this.Administrator.MinimumSize = new System.Drawing.Size(0, 33);
			this.Administrator.Name = "Administrator";
			this.Administrator.Padding = new System.Windows.Forms.Padding(0, 4, 4, 4);
			this.Administrator.Size = new System.Drawing.Size(169, 33);
			this.Administrator.TabIndex = 4;
			this.Administrator.Text = "Administrative Setup";
			this.Administrator.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.Administrator.UseVisualStyleBackColor = true;
			// 
			// tabPageSkipping
			// 
			this.tabPageSkipping.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.tabPageSkipping.Controls.Add(this._tableLayoutPanelSkipping);
			this.tabPageSkipping.Controls.Add(this._btnClearAllSkipInfo);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabPageSkipping, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabPageSkipping, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabPageSkipping, "AdministrativeSettings.tabPageSkipping");
			this.tabPageSkipping.Location = new System.Drawing.Point(4, 25);
			this.tabPageSkipping.Margin = new System.Windows.Forms.Padding(4);
			this.tabPageSkipping.Name = "tabPageSkipping";
			this.tabPageSkipping.Padding = new System.Windows.Forms.Padding(4);
			this.tabPageSkipping.Size = new System.Drawing.Size(483, 425);
			this.tabPageSkipping.TabIndex = 1;
			this.tabPageSkipping.Text = "Skipping";
			// 
			// _tableLayoutPanelSkipping
			// 
			this._tableLayoutPanelSkipping.ColumnCount = 1;
			this._tableLayoutPanelSkipping.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelSkipping.Controls.Add(this._lblSkippingInstructions, 0, 1);
			this._tableLayoutPanelSkipping.Controls.Add(this._lbSkippedStyles, 0, 2);
			this._tableLayoutPanelSkipping.Controls.Add(this._chkShowSkipButton, 0, 0);
			this._tableLayoutPanelSkipping.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanelSkipping.Location = new System.Drawing.Point(4, 4);
			this._tableLayoutPanelSkipping.Margin = new System.Windows.Forms.Padding(4);
			this._tableLayoutPanelSkipping.Name = "_tableLayoutPanelSkipping";
			this._tableLayoutPanelSkipping.Padding = new System.Windows.Forms.Padding(15, 14, 15, 14);
			this._tableLayoutPanelSkipping.RowCount = 3;
			this._tableLayoutPanelSkipping.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelSkipping.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelSkipping.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelSkipping.Size = new System.Drawing.Size(475, 417);
			this._tableLayoutPanelSkipping.TabIndex = 3;
			// 
			// _lblSkippingInstructions
			// 
			this._lblSkippingInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lblSkippingInstructions.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblSkippingInstructions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblSkippingInstructions, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblSkippingInstructions, "AdministrativeSettings._lblSkippingInstructions");
			this._lblSkippingInstructions.Location = new System.Drawing.Point(15, 54);
			this._lblSkippingInstructions.Margin = new System.Windows.Forms.Padding(0, 0, 4, 7);
			this._lblSkippingInstructions.Name = "_lblSkippingInstructions";
			this._lblSkippingInstructions.Size = new System.Drawing.Size(441, 16);
			this._lblSkippingInstructions.TabIndex = 2;
			this._lblSkippingInstructions.Text = "Select any styles whose text should never be recorded for project {0}.";
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
			this._lbSkippedStyles.Location = new System.Drawing.Point(19, 81);
			this._lbSkippedStyles.Margin = new System.Windows.Forms.Padding(4);
			this._lbSkippedStyles.Name = "_lbSkippedStyles";
			this._lbSkippedStyles.Size = new System.Drawing.Size(437, 310);
			this._lbSkippedStyles.TabIndex = 0;
			// 
			// _chkShowSkipButton
			// 
			this._chkShowSkipButton.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkShowSkipButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkShowSkipButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkShowSkipButton, "AdministrativeSettings._chkShowSkipButton");
			this._chkShowSkipButton.Location = new System.Drawing.Point(19, 18);
			this._chkShowSkipButton.Margin = new System.Windows.Forms.Padding(4);
			this._chkShowSkipButton.Name = "_chkShowSkipButton";
			this._chkShowSkipButton.Padding = new System.Windows.Forms.Padding(0, 0, 4, 12);
			this._chkShowSkipButton.Size = new System.Drawing.Size(134, 32);
			this._chkShowSkipButton.TabIndex = 3;
			this._chkShowSkipButton.Text = "Show Skip Button";
			this._chkShowSkipButton.UseVisualStyleBackColor = true;
			// 
			// _btnClearAllSkipInfo
			// 
			this._btnClearAllSkipInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnClearAllSkipInfo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnClearAllSkipInfo, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnClearAllSkipInfo, "AdministrativeSettings._btnClearAllSkipInfo");
			this._btnClearAllSkipInfo.Location = new System.Drawing.Point(156, 255);
			this._btnClearAllSkipInfo.Margin = new System.Windows.Forms.Padding(4);
			this._btnClearAllSkipInfo.Name = "_btnClearAllSkipInfo";
			this._btnClearAllSkipInfo.Size = new System.Drawing.Size(143, 28);
			this._btnClearAllSkipInfo.TabIndex = 1;
			this._btnClearAllSkipInfo.Text = "Clear All Skips";
			this._btnClearAllSkipInfo.UseVisualStyleBackColor = true;
			this._btnClearAllSkipInfo.Visible = false;
			this._btnClearAllSkipInfo.Click += new System.EventHandler(this.HandleClearAllSkipInfo_Click);
			// 
			// tabPagePunctuation
			// 
			this.tabPagePunctuation.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.tabPagePunctuation.Controls.Add(this._tableLayoutPanelPunctuation);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabPagePunctuation, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabPagePunctuation, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabPagePunctuation, "AdministrativeSettings.tabPagePunctuation");
			this.tabPagePunctuation.Location = new System.Drawing.Point(4, 25);
			this.tabPagePunctuation.Margin = new System.Windows.Forms.Padding(4);
			this.tabPagePunctuation.Name = "tabPagePunctuation";
			this.tabPagePunctuation.Size = new System.Drawing.Size(483, 425);
			this.tabPagePunctuation.TabIndex = 2;
			this.tabPagePunctuation.Text = "Punctuation";
			// 
			// _tableLayoutPanelPunctuation
			// 
			this._tableLayoutPanelPunctuation.ColumnCount = 3;
			this._tableLayoutPanelPunctuation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanelPunctuation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelPunctuation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanelPunctuation.Controls.Add(this._lblClauseSeparators, 1, 6);
			this._tableLayoutPanelPunctuation.Controls.Add(_lblBreakClauses, 0, 6);
			this._tableLayoutPanelPunctuation.Controls.Add(pnlLine, 0, 5);
			this._tableLayoutPanelPunctuation.Controls.Add(this._lblAdditionalLineBreakCharacters, 1, 1);
			this._tableLayoutPanelPunctuation.Controls.Add(this._txtAdditionalBlockSeparators, 0, 2);
			this._tableLayoutPanelPunctuation.Controls.Add(this._txtClauseSeparatorCharacters, 0, 7);
			this._tableLayoutPanelPunctuation.Controls.Add(this._lblBreakBlocks, 0, 1);
			this._tableLayoutPanelPunctuation.Controls.Add(this._lblWarningExistingRecordings, 0, 4);
			this._tableLayoutPanelPunctuation.Controls.Add(this._chkBreakAtQuotes, 0, 0);
			this._tableLayoutPanelPunctuation.Controls.Add(this._chkBreakAtParagraphBreaks, 0, 3);
			this._tableLayoutPanelPunctuation.Controls.Add(this._cboSentenceEndingWhitespace, 2, 2);
			this._tableLayoutPanelPunctuation.Controls.Add(this._cboPauseWhitespace, 2, 7);
			this._tableLayoutPanelPunctuation.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanelPunctuation.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanelPunctuation.Margin = new System.Windows.Forms.Padding(4);
			this._tableLayoutPanelPunctuation.Name = "_tableLayoutPanelPunctuation";
			this._tableLayoutPanelPunctuation.Padding = new System.Windows.Forms.Padding(15, 14, 15, 14);
			this._tableLayoutPanelPunctuation.RowCount = 1;
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.Size = new System.Drawing.Size(483, 425);
			this._tableLayoutPanelPunctuation.TabIndex = 1;
			// 
			// _lblClauseSeparators
			// 
			this._lblClauseSeparators.AutoSize = true;
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._lblClauseSeparators, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblClauseSeparators, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblClauseSeparators, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblClauseSeparators, "AdministrativeSettings._lblClauseSeparators");
			this._lblClauseSeparators.Location = new System.Drawing.Point(74, 309);
			this._lblClauseSeparators.Margin = new System.Windows.Forms.Padding(4, 4, 4, 0);
			this._lblClauseSeparators.Name = "_lblClauseSeparators";
			this._lblClauseSeparators.Size = new System.Drawing.Size(382, 32);
			this._lblClauseSeparators.TabIndex = 2;
			this._lblClauseSeparators.Text = "Pause punctuation (used when option to break blocks into lines is selected):";
			// 
			// _lblAdditionalLineBreakCharacters
			// 
			this._lblAdditionalLineBreakCharacters.AutoSize = true;
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._lblAdditionalLineBreakCharacters, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblAdditionalLineBreakCharacters, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblAdditionalLineBreakCharacters, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblAdditionalLineBreakCharacters, "AdministrativeSettings._lblAdditionalLineBreakCharacters");
			this._lblAdditionalLineBreakCharacters.Location = new System.Drawing.Point(74, 50);
			this._lblAdditionalLineBreakCharacters.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._lblAdditionalLineBreakCharacters.Name = "_lblAdditionalLineBreakCharacters";
			this._lblAdditionalLineBreakCharacters.Size = new System.Drawing.Size(385, 32);
			this._lblAdditionalLineBreakCharacters.TabIndex = 12;
			this._lblAdditionalLineBreakCharacters.Text = "Additional characters (besides sentence-ending punctuation) to break text into bl" +
    "ocks:";
			// 
			// _txtAdditionalBlockSeparators
			// 
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._txtAdditionalBlockSeparators, 2);
			this._txtAdditionalBlockSeparators.Dock = System.Windows.Forms.DockStyle.Fill;
			this._txtAdditionalBlockSeparators.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._txtAdditionalBlockSeparators, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._txtAdditionalBlockSeparators, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._txtAdditionalBlockSeparators, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._txtAdditionalBlockSeparators, "AdministrativeSettings._txtAdditionalBlockSeparators");
			this._txtAdditionalBlockSeparators.Location = new System.Drawing.Point(19, 90);
			this._txtAdditionalBlockSeparators.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
			this._txtAdditionalBlockSeparators.Name = "_txtAdditionalBlockSeparators";
			this._txtAdditionalBlockSeparators.Size = new System.Drawing.Size(247, 25);
			this._txtAdditionalBlockSeparators.TabIndex = 13;
			this._txtAdditionalBlockSeparators.TextChanged += new System.EventHandler(this.UpdateWarningTextColor);
			this._txtAdditionalBlockSeparators.Leave += new System.EventHandler(this._txtAdditionalBlockSeparators_Leave);
			// 
			// _txtClauseSeparatorCharacters
			// 
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._txtClauseSeparatorCharacters, 2);
			this._txtClauseSeparatorCharacters.Dock = System.Windows.Forms.DockStyle.Fill;
			this._txtClauseSeparatorCharacters.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._txtClauseSeparatorCharacters, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._txtClauseSeparatorCharacters, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._txtClauseSeparatorCharacters, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._txtClauseSeparatorCharacters, "AdministrativeSettings._txtClauseSeparatorCharacters");
			this._txtClauseSeparatorCharacters.Location = new System.Drawing.Point(19, 345);
			this._txtClauseSeparatorCharacters.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
			this._txtClauseSeparatorCharacters.Name = "_txtClauseSeparatorCharacters";
			this._txtClauseSeparatorCharacters.Size = new System.Drawing.Size(247, 25);
			this._txtClauseSeparatorCharacters.TabIndex = 15;
			this._txtClauseSeparatorCharacters.Leave += new System.EventHandler(this._txtClauseSeparatorCharacters_Leave);
			// 
			// _lblBreakBlocks
			// 
			this._lblBreakBlocks.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._lblBreakBlocks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._lblBreakBlocks.Image = global::HearThis.Properties.Resources.Icon_BlockBreak;
			this._lblBreakBlocks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblBreakBlocks, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblBreakBlocks, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblBreakBlocks, "AdministrativeSettings._lblBreakBlocks");
			this._lblBreakBlocks.Location = new System.Drawing.Point(19, 50);
			this._lblBreakBlocks.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._lblBreakBlocks.Name = "_lblBreakBlocks";
			this._lblBreakBlocks.Size = new System.Drawing.Size(47, 36);
			this._lblBreakBlocks.TabIndex = 14;
			// 
			// _lblWarningExistingRecordings
			// 
			this._lblWarningExistingRecordings.AutoSize = true;
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._lblWarningExistingRecordings, 3);
			this._lblWarningExistingRecordings.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lblWarningExistingRecordings.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblWarningExistingRecordings, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblWarningExistingRecordings, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblWarningExistingRecordings, "AdministrativeSettings._lblWarningExistingRecordings");
			this._lblWarningExistingRecordings.Location = new System.Drawing.Point(19, 186);
			this._lblWarningExistingRecordings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._lblWarningExistingRecordings.Name = "_lblWarningExistingRecordings";
			this._lblWarningExistingRecordings.Size = new System.Drawing.Size(445, 96);
			this._lblWarningExistingRecordings.TabIndex = 11;
			this._lblWarningExistingRecordings.Text = resources.GetString("_lblWarningExistingRecordings.Text");
			// 
			// _chkBreakAtQuotes
			// 
			this._chkBreakAtQuotes.AutoSize = true;
			this._chkBreakAtQuotes.Checked = true;
			this._chkBreakAtQuotes.CheckState = System.Windows.Forms.CheckState.Checked;
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._chkBreakAtQuotes, 3);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkBreakAtQuotes, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkBreakAtQuotes, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkBreakAtQuotes, "AdministrativeSettings._chkBreakAtQuotes");
			this._chkBreakAtQuotes.Location = new System.Drawing.Point(19, 18);
			this._chkBreakAtQuotes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
			this._chkBreakAtQuotes.Name = "_chkBreakAtQuotes";
			this._chkBreakAtQuotes.Size = new System.Drawing.Size(302, 20);
			this._chkBreakAtQuotes.TabIndex = 1;
			this._chkBreakAtQuotes.Text = "Treat quotations as separate recording blocks";
			this._chkBreakAtQuotes.UseVisualStyleBackColor = true;
			this._chkBreakAtQuotes.CheckedChanged += new System.EventHandler(this.UpdateWarningTextColor);
			// 
			// _chkBreakAtParagraphBreaks
			// 
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._chkBreakAtParagraphBreaks, 3);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkBreakAtParagraphBreaks, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkBreakAtParagraphBreaks, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkBreakAtParagraphBreaks, "AdministrativeSettings._chkBreakAtParagraphBreaks");
			this._chkBreakAtParagraphBreaks.Location = new System.Drawing.Point(19, 132);
			this._chkBreakAtParagraphBreaks.Margin = new System.Windows.Forms.Padding(4);
			this._chkBreakAtParagraphBreaks.Name = "_chkBreakAtParagraphBreaks";
			this._chkBreakAtParagraphBreaks.Size = new System.Drawing.Size(443, 50);
			this._chkBreakAtParagraphBreaks.TabIndex = 16;
			this._chkBreakAtParagraphBreaks.Text = "Treat paragraph breaks as separate recording blocks (useful for poetry)";
			this._chkBreakAtParagraphBreaks.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this._chkBreakAtParagraphBreaks.CheckedChanged += new System.EventHandler(this.UpdateWarningTextColor);
			// 
			// _cboSentenceEndingWhitespace
			// 
			this._cboSentenceEndingWhitespace.CheckOnClick = true;
			this._cboSentenceEndingWhitespace.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this._cboSentenceEndingWhitespace.DropDownHeight = 1;
			this._cboSentenceEndingWhitespace.DropDownWidth = 350;
			this._cboSentenceEndingWhitespace.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
			this._cboSentenceEndingWhitespace.FormattingEnabled = true;
			this._cboSentenceEndingWhitespace.IntegralHeight = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cboSentenceEndingWhitespace, "For scriptio continua languages, select any whitespace characters that are uses t" +
        "o indicate sentence breaks.");
			this.l10NSharpExtender1.SetLocalizationComment(this._cboSentenceEndingWhitespace, null);
			this.l10NSharpExtender1.SetLocalizingId(this._cboSentenceEndingWhitespace, "AdministrativeSettings._cboSentenceEndingWhitespace");
			this._cboSentenceEndingWhitespace.Location = new System.Drawing.Point(274, 90);
			this._cboSentenceEndingWhitespace.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
			this._cboSentenceEndingWhitespace.Name = "_cboSentenceEndingWhitespace";
			this._cboSentenceEndingWhitespace.Size = new System.Drawing.Size(190, 26);
			this._cboSentenceEndingWhitespace.TabIndex = 17;
			this._cboSentenceEndingWhitespace.ValueSeparator = ", ";
			this._cboSentenceEndingWhitespace.TextChanged += new System.EventHandler(this.UpdateWarningTextColor);
			// 
			// _cboPauseWhitespace
			// 
			this._cboPauseWhitespace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cboPauseWhitespace.CheckOnClick = true;
			this._cboPauseWhitespace.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this._cboPauseWhitespace.DropDownHeight = 1;
			this._cboPauseWhitespace.DropDownWidth = 350;
			this._cboPauseWhitespace.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
			this._cboPauseWhitespace.FormattingEnabled = true;
			this._cboPauseWhitespace.IntegralHeight = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cboPauseWhitespace, "For scriptio continua languages, select any whitespace characters that are uses t" +
        "o indicate pauses.");
			this.l10NSharpExtender1.SetLocalizationComment(this._cboPauseWhitespace, null);
			this.l10NSharpExtender1.SetLocalizingId(this._cboPauseWhitespace, "AdministrativeSettings._cboPauseWhitespace");
			this._cboPauseWhitespace.Location = new System.Drawing.Point(274, 345);
			this._cboPauseWhitespace.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
			this._cboPauseWhitespace.Name = "_cboPauseWhitespace";
			this._cboPauseWhitespace.Size = new System.Drawing.Size(190, 26);
			this._cboPauseWhitespace.TabIndex = 18;
			this._cboPauseWhitespace.ValueSeparator = ", ";
			// 
			// tabPageInterface
			// 
			this.tabPageInterface.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.tabPageInterface.Controls.Add(this._groupAdvancedUI);
			this.tabPageInterface.Controls.Add(this._chkShowBookAndChapterLabels);
			this.tabPageInterface.Controls.Add(this.lblColorSchemeChangeRestartWarning);
			this.tabPageInterface.Controls.Add(this._cboColorScheme);
			this.tabPageInterface.Controls.Add(this.lblInterface);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabPageInterface, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabPageInterface, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabPageInterface, "AdministrativeSettings.tabPageInterface");
			this.tabPageInterface.Location = new System.Drawing.Point(4, 25);
			this.tabPageInterface.Margin = new System.Windows.Forms.Padding(4);
			this.tabPageInterface.Name = "tabPageInterface";
			this.tabPageInterface.Size = new System.Drawing.Size(483, 425);
			this.tabPageInterface.TabIndex = 3;
			this.tabPageInterface.Text = "Interface";
			// 
			// _groupAdvancedUI
			// 
			this._groupAdvancedUI.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._groupAdvancedUI.Controls.Add(this._tableLayoutPanelAdvancedUI);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._groupAdvancedUI, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._groupAdvancedUI, null);
			this.l10NSharpExtender1.SetLocalizingId(this._groupAdvancedUI, "AdministrativeSettings._groupAdvancedUI");
			this._groupAdvancedUI.Location = new System.Drawing.Point(19, 212);
			this._groupAdvancedUI.Margin = new System.Windows.Forms.Padding(4);
			this._groupAdvancedUI.Name = "_groupAdvancedUI";
			this._groupAdvancedUI.Padding = new System.Windows.Forms.Padding(4);
			this._groupAdvancedUI.Size = new System.Drawing.Size(441, 177);
			this._groupAdvancedUI.TabIndex = 10;
			this._groupAdvancedUI.TabStop = false;
			this._groupAdvancedUI.Text = "Advanced";
			// 
			// _tableLayoutPanelAdvancedUI
			// 
			this._tableLayoutPanelAdvancedUI.ColumnCount = 1;
			this._tableLayoutPanelAdvancedUI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelAdvancedUI.Controls.Add(this._chkEnableClipShifting, 0, 0);
			this._tableLayoutPanelAdvancedUI.Controls.Add(this._lblShiftClipsMenuWarning, 0, 2);
			this._tableLayoutPanelAdvancedUI.Controls.Add(this._lblShiftClipsExplanation, 0, 1);
			this._tableLayoutPanelAdvancedUI.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanelAdvancedUI.Location = new System.Drawing.Point(4, 19);
			this._tableLayoutPanelAdvancedUI.Margin = new System.Windows.Forms.Padding(4);
			this._tableLayoutPanelAdvancedUI.Name = "_tableLayoutPanelAdvancedUI";
			this._tableLayoutPanelAdvancedUI.RowCount = 3;
			this._tableLayoutPanelAdvancedUI.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelAdvancedUI.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelAdvancedUI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelAdvancedUI.Size = new System.Drawing.Size(433, 154);
			this._tableLayoutPanelAdvancedUI.TabIndex = 13;
			// 
			// _chkEnableClipShifting
			// 
			this._chkEnableClipShifting.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkEnableClipShifting, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkEnableClipShifting, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkEnableClipShifting, "AdministrativeSettings._chkEnableClipShifting");
			this._chkEnableClipShifting.Location = new System.Drawing.Point(4, 4);
			this._chkEnableClipShifting.Margin = new System.Windows.Forms.Padding(4);
			this._chkEnableClipShifting.Name = "_chkEnableClipShifting";
			this._chkEnableClipShifting.Size = new System.Drawing.Size(153, 20);
			this._chkEnableClipShifting.TabIndex = 11;
			this._chkEnableClipShifting.Text = "Enable {0} command";
			this._chkEnableClipShifting.UseVisualStyleBackColor = true;
			this._chkEnableClipShifting.CheckedChanged += new System.EventHandler(this.chkEnableClipShifting_CheckedChanged);
			// 
			// _lblShiftClipsMenuWarning
			// 
			this._lblShiftClipsMenuWarning.AutoSize = true;
			this._lblShiftClipsMenuWarning.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblShiftClipsMenuWarning, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblShiftClipsMenuWarning, "Param 0: name of \"Shift Clips\" menu command; Param 1: HearThis (program name)");
			this.l10NSharpExtender1.SetLocalizingId(this._lblShiftClipsMenuWarning, "AdministrativeSettings._lblShiftClipsMenuWarning");
			this._lblShiftClipsMenuWarning.Location = new System.Drawing.Point(4, 44);
			this._lblShiftClipsMenuWarning.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._lblShiftClipsMenuWarning.Name = "_lblShiftClipsMenuWarning";
			this._lblShiftClipsMenuWarning.Size = new System.Drawing.Size(411, 80);
			this._lblShiftClipsMenuWarning.TabIndex = 12;
			this._lblShiftClipsMenuWarning.Text = resources.GetString("_lblShiftClipsMenuWarning.Text");
			this._lblShiftClipsMenuWarning.Visible = false;
			// 
			// _lblShiftClipsExplanation
			// 
			this._lblShiftClipsExplanation.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblShiftClipsExplanation, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblShiftClipsExplanation, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblShiftClipsExplanation, "AdministrativeSettings._labelShiftClipsExplanation");
			this._lblShiftClipsExplanation.Location = new System.Drawing.Point(4, 28);
			this._lblShiftClipsExplanation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._lblShiftClipsExplanation.Name = "_lblShiftClipsExplanation";
			this._lblShiftClipsExplanation.Size = new System.Drawing.Size(407, 16);
			this._lblShiftClipsExplanation.TabIndex = 13;
			this._lblShiftClipsExplanation.Text = "To use this command, right-click the block slider in the main window.";
			this._lblShiftClipsExplanation.Visible = false;
			// 
			// _chkShowBookAndChapterLabels
			// 
			this._chkShowBookAndChapterLabels.AutoSize = true;
			this._chkShowBookAndChapterLabels.Checked = true;
			this._chkShowBookAndChapterLabels.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkShowBookAndChapterLabels, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkShowBookAndChapterLabels, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkShowBookAndChapterLabels, "AdministrativeSettings._chkShowBookAndChapterLabels");
			this._chkShowBookAndChapterLabels.Location = new System.Drawing.Point(19, 20);
			this._chkShowBookAndChapterLabels.Margin = new System.Windows.Forms.Padding(4);
			this._chkShowBookAndChapterLabels.Name = "_chkShowBookAndChapterLabels";
			this._chkShowBookAndChapterLabels.Size = new System.Drawing.Size(337, 20);
			this._chkShowBookAndChapterLabels.TabIndex = 9;
			this._chkShowBookAndChapterLabels.Text = "Show book and chapter labels on navigation buttons";
			this._chkShowBookAndChapterLabels.UseVisualStyleBackColor = true;
			// 
			// lblColorSchemeChangeRestartWarning
			// 
			this.lblColorSchemeChangeRestartWarning.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblColorSchemeChangeRestartWarning, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblColorSchemeChangeRestartWarning, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblColorSchemeChangeRestartWarning, "AdministrativeSettings.lblColorSchemeChangeRestartWarning");
			this.lblColorSchemeChangeRestartWarning.Location = new System.Drawing.Point(20, 143);
			this.lblColorSchemeChangeRestartWarning.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblColorSchemeChangeRestartWarning.Name = "lblColorSchemeChangeRestartWarning";
			this.lblColorSchemeChangeRestartWarning.Size = new System.Drawing.Size(311, 16);
			this.lblColorSchemeChangeRestartWarning.TabIndex = 8;
			this.lblColorSchemeChangeRestartWarning.Text = "HearThis will restart to apply the new color scheme.";
			this.lblColorSchemeChangeRestartWarning.Visible = false;
			// 
			// _cboColorScheme
			// 
			this._cboColorScheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboColorScheme.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cboColorScheme, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cboColorScheme, null);
			this.l10NSharpExtender1.SetLocalizingId(this._cboColorScheme, "AdministrativeSettings.comboBox1");
			this._cboColorScheme.Location = new System.Drawing.Point(19, 97);
			this._cboColorScheme.Margin = new System.Windows.Forms.Padding(4);
			this._cboColorScheme.Name = "_cboColorScheme";
			this._cboColorScheme.Size = new System.Drawing.Size(205, 24);
			this._cboColorScheme.TabIndex = 7;
			this._cboColorScheme.SelectedIndexChanged += new System.EventHandler(this.cboColorScheme_SelectedIndexChanged);
			// 
			// lblInterface
			// 
			this.lblInterface.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblInterface, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblInterface, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblInterface, "AdministrativeSettings.lblInterface");
			this.lblInterface.Location = new System.Drawing.Point(15, 65);
			this.lblInterface.Margin = new System.Windows.Forms.Padding(0, 0, 4, 12);
			this.lblInterface.Name = "lblInterface";
			this.lblInterface.Size = new System.Drawing.Size(96, 16);
			this.lblInterface.TabIndex = 6;
			this.lblInterface.Text = "Color Scheme:";
			// 
			// _btnOk
			// 
			this._btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnOk, "Common.OK");
			this._btnOk.Location = new System.Drawing.Point(289, 496);
			this._btnOk.Margin = new System.Windows.Forms.Padding(4);
			this._btnOk.Name = "_btnOk";
			this._btnOk.Size = new System.Drawing.Size(100, 28);
			this._btnOk.TabIndex = 2;
			this._btnOk.Text = "OK";
			this._btnOk.UseVisualStyleBackColor = true;
			this._btnOk.Click += new System.EventHandler(this.HandleOkButtonClick);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "";
			// 
			// AdministrativeSettings
			// 
			this.AcceptButton = this._btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = _btnCancel;
			this.ClientSize = new System.Drawing.Size(523, 539);
			this.Controls.Add(_btnCancel);
			this.Controls.Add(settingsProtectionLauncherButton1);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._btnOk);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "RestrictAdministrativeAccess.WindowTitle");
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(307, 578);
			this.Name = "AdministrativeSettings";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Settings";
			this.tabControl1.ResumeLayout(false);
			this.tabPageModes.ResumeLayout(false);
			this.tabPageModes.PerformLayout();
			this._tableLayoutModes.ResumeLayout(false);
			this._tableLayoutModes.PerformLayout();
			this.tabPageSkipping.ResumeLayout(false);
			this._tableLayoutPanelSkipping.ResumeLayout(false);
			this._tableLayoutPanelSkipping.PerformLayout();
			this.tabPagePunctuation.ResumeLayout(false);
			this._tableLayoutPanelPunctuation.ResumeLayout(false);
			this._tableLayoutPanelPunctuation.PerformLayout();
			this.tabPageInterface.ResumeLayout(false);
			this.tabPageInterface.PerformLayout();
			this._groupAdvancedUI.ResumeLayout(false);
			this._tableLayoutPanelAdvancedUI.ResumeLayout(false);
			this._tableLayoutPanelAdvancedUI.PerformLayout();
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
		private System.Windows.Forms.Label _lblClauseSeparators;
		private System.Windows.Forms.Label _lblWarningExistingRecordings;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelSkipping;
		private System.Windows.Forms.CheckBox _chkShowSkipButton;
		private System.Windows.Forms.Label _lblAdditionalLineBreakCharacters;
		private System.Windows.Forms.Label _lblBreakBlocks;
		private System.Windows.Forms.TextBox _txtAdditionalBlockSeparators;
		private System.Windows.Forms.TextBox _txtClauseSeparatorCharacters;
		private System.Windows.Forms.CheckBox _chkBreakAtQuotes;
		private System.Windows.Forms.CheckBox _chkBreakAtParagraphBreaks;
		private System.Windows.Forms.TabPage tabPageInterface;
		private System.Windows.Forms.ComboBox _cboColorScheme;
		private System.Windows.Forms.Label lblInterface;
		private System.Windows.Forms.Label lblColorSchemeChangeRestartWarning;
		private System.Windows.Forms.CheckBox _chkShowBookAndChapterLabels;
		private System.Windows.Forms.GroupBox _groupAdvancedUI;
		private System.Windows.Forms.Label _lblShiftClipsMenuWarning;
		private System.Windows.Forms.CheckBox _chkEnableClipShifting;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelAdvancedUI;
		private System.Windows.Forms.Label _lblShiftClipsExplanation;
		private CheckComboBoxTest.CheckedComboBox _cboSentenceEndingWhitespace;
		private CheckComboBoxTest.CheckedComboBox _cboPauseWhitespace;
	}
}
