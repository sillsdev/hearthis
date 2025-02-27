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
			System.Windows.Forms.Label _lblRecordByVerseRangeStart;
			System.Windows.Forms.Label _lblRecordByVerseRangeEnd;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdministrativeSettings));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
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
			this._cboSentenceEndingWhitespace = new SIL.Windows.Forms.CheckedComboBox.CheckedComboBox();
			this._cboPauseWhitespace = new SIL.Windows.Forms.CheckedComboBox.CheckedComboBox();
			this.tabPageInterface = new System.Windows.Forms.TabPage();
			this._groupAdvancedUI = new System.Windows.Forms.GroupBox();
			this._tableLayoutPanelAdvancedUI = new System.Windows.Forms.TableLayoutPanel();
			this._chkEnableClipShifting = new System.Windows.Forms.CheckBox();
			this._lblShiftClipsMenuWarning = new System.Windows.Forms.Label();
			this._lblShiftClipsExplanation = new System.Windows.Forms.Label();
			this._chkShowCheckForProblems = new System.Windows.Forms.CheckBox();
			this._chkShowBookAndChapterLabels = new System.Windows.Forms.CheckBox();
			this.lblColorSchemeChangeRestartWarning = new System.Windows.Forms.Label();
			this._cboColorScheme = new System.Windows.Forms.ComboBox();
			this.lblInterface = new System.Windows.Forms.Label();
			this.tabPageRecordByVerse = new System.Windows.Forms.TabPage();
			this._tableLayoutPanelRecordByVerse = new System.Windows.Forms.TableLayoutPanel();
			this._lblRecordByVerse = new System.Windows.Forms.Label();
			this._gridVerseRanges = new SIL.Windows.Forms.Widgets.BetterGrid.BetterGrid();
			this.colFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._lblWarningRecordByVerse = new System.Windows.Forms.Label();
			this._btnAddRecordByVerseRange = new System.Windows.Forms.Button();
			this._verseCtrlRecordByVerseRangeStart = new SIL.Windows.Forms.Scripture.VerseControl();
			this._verseCtrlRecordByVerseRangeEnd = new SIL.Windows.Forms.Scripture.VerseControl();
			this.tabPageClipEditor = new System.Windows.Forms.TabPage();
			this._tableLayoutPanelClipEditor = new System.Windows.Forms.TableLayoutPanel();
			this._lblInstructions = new System.Windows.Forms.Label();
			this._rdoUseSpecifiedEditor = new System.Windows.Forms.RadioButton();
			this._rdoUseDefaultAssociatedApplication = new System.Windows.Forms.RadioButton();
			this._lblPathToWAVFileEditor = new System.Windows.Forms.Label();
			this._lblWAVFileEditingApplicationName = new System.Windows.Forms.Label();
			this._txtEditingApplicationName = new System.Windows.Forms.TextBox();
			this._chkWAVEditorCommandLineArguments = new System.Windows.Forms.CheckBox();
			this._txtCommandLineArguments = new System.Windows.Forms.TextBox();
			this._btnOpenFileChooser = new System.Windows.Forms.Button();
			this._lblCommandLineArgumentsInstructions = new System.Windows.Forms.Label();
			this._lblWAVEditorCommandLineExample = new System.Windows.Forms.Label();
			this._btnOk = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			_lblBreakClauses = new System.Windows.Forms.Label();
			pnlLine = new System.Windows.Forms.Panel();
			settingsProtectionLauncherButton1 = new SIL.Windows.Forms.SettingProtection.SettingsProtectionLauncherButton();
			_btnCancel = new System.Windows.Forms.Button();
			_lblRecordByVerseRangeStart = new System.Windows.Forms.Label();
			_lblRecordByVerseRangeEnd = new System.Windows.Forms.Label();
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
			this.tabPageRecordByVerse.SuspendLayout();
			this._tableLayoutPanelRecordByVerse.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._gridVerseRanges)).BeginInit();
			this.tabPageClipEditor.SuspendLayout();
			this._tableLayoutPanelClipEditor.SuspendLayout();
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
			_lblBreakClauses.Location = new System.Drawing.Point(14, 220);
			_lblBreakClauses.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			_lblBreakClauses.Name = "_lblBreakClauses";
			_lblBreakClauses.Size = new System.Drawing.Size(35, 13);
			_lblBreakClauses.TabIndex = 3;
			// 
			// pnlLine
			// 
			pnlLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._tableLayoutPanelPunctuation.SetColumnSpan(pnlLine, 3);
			pnlLine.Dock = System.Windows.Forms.DockStyle.Bottom;
			pnlLine.Location = new System.Drawing.Point(14, 204);
			pnlLine.Margin = new System.Windows.Forms.Padding(3, 10, 3, 6);
			pnlLine.Name = "pnlLine";
			pnlLine.Size = new System.Drawing.Size(332, 4);
			pnlLine.TabIndex = 10;
			// 
			// settingsProtectionLauncherButton1
			// 
			settingsProtectionLauncherButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(settingsProtectionLauncherButton1, null);
			this.l10NSharpExtender1.SetLocalizationComment(settingsProtectionLauncherButton1, null);
			this.l10NSharpExtender1.SetLocalizingId(settingsProtectionLauncherButton1, "AdministrativeSettings.SettingsProtectionLauncherButton");
			settingsProtectionLauncherButton1.Location = new System.Drawing.Point(9, 389);
			settingsProtectionLauncherButton1.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
			settingsProtectionLauncherButton1.Name = "settingsProtectionLauncherButton1";
			settingsProtectionLauncherButton1.Size = new System.Drawing.Size(205, 37);
			settingsProtectionLauncherButton1.TabIndex = 8;
			// 
			// _btnCancel
			// 
			_btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(_btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(_btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(_btnCancel, "Common.Cancel");
			_btnCancel.Location = new System.Drawing.Point(298, 403);
			_btnCancel.Name = "_btnCancel";
			_btnCancel.Size = new System.Drawing.Size(75, 23);
			_btnCancel.TabIndex = 9;
			_btnCancel.Text = "&Cancel";
			_btnCancel.UseVisualStyleBackColor = true;
			// 
			// _lblRecordByVerseRangeStart
			// 
			_lblRecordByVerseRangeStart.Anchor = System.Windows.Forms.AnchorStyles.Right;
			_lblRecordByVerseRangeStart.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(_lblRecordByVerseRangeStart, null);
			this.l10NSharpExtender1.SetLocalizationComment(_lblRecordByVerseRangeStart, null);
			this.l10NSharpExtender1.SetLocalizingId(_lblRecordByVerseRangeStart, "AdministrativeSettings._lblRecordByVerseRangeStart");
			_lblRecordByVerseRangeStart.Location = new System.Drawing.Point(112, 78);
			_lblRecordByVerseRangeStart.Name = "_lblRecordByVerseRangeStart";
			_lblRecordByVerseRangeStart.Size = new System.Drawing.Size(33, 13);
			_lblRecordByVerseRangeStart.TabIndex = 10;
			_lblRecordByVerseRangeStart.Text = "From:";
			// 
			// _lblRecordByVerseRangeEnd
			// 
			_lblRecordByVerseRangeEnd.Anchor = System.Windows.Forms.AnchorStyles.Right;
			_lblRecordByVerseRangeEnd.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(_lblRecordByVerseRangeEnd, null);
			this.l10NSharpExtender1.SetLocalizationComment(_lblRecordByVerseRangeEnd, null);
			this.l10NSharpExtender1.SetLocalizingId(_lblRecordByVerseRangeEnd, "AdministrativeSettings._lblRecordByVerseRangeEnd");
			_lblRecordByVerseRangeEnd.Location = new System.Drawing.Point(122, 102);
			_lblRecordByVerseRangeEnd.Name = "_lblRecordByVerseRangeEnd";
			_lblRecordByVerseRangeEnd.Size = new System.Drawing.Size(23, 13);
			_lblRecordByVerseRangeEnd.TabIndex = 13;
			_lblRecordByVerseRangeEnd.Text = "To:";
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
			this.tabControl1.Controls.Add(this.tabPageRecordByVerse);
			this.tabControl1.Controls.Add(this.tabPageClipEditor);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(368, 369);
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
			this.tabPageModes.Location = new System.Drawing.Point(4, 22);
			this.tabPageModes.Name = "tabPageModes";
			this.tabPageModes.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageModes.Size = new System.Drawing.Size(360, 343);
			this.tabPageModes.TabIndex = 0;
			this.tabPageModes.Tag = "Modes";
			this.tabPageModes.Text = "Modes";
			// 
			// lblSelectModes
			// 
			this.lblSelectModes.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblSelectModes, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblSelectModes, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblSelectModes, "AdministrativeSettings.lblSelectModes");
			this.lblSelectModes.Location = new System.Drawing.Point(11, 11);
			this.lblSelectModes.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
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
			this._tableLayoutModes.Location = new System.Drawing.Point(11, 37);
			this._tableLayoutModes.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this._tableLayoutModes.Name = "_tableLayoutModes";
			this._tableLayoutModes.RowCount = 2;
			this._tableLayoutModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutModes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutModes.Size = new System.Drawing.Size(271, 65);
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
			this.lnkNormalRecordingModeSetAsDefault.Location = new System.Drawing.Point(150, 33);
			this.lnkNormalRecordingModeSetAsDefault.Name = "lnkNormalRecordingModeSetAsDefault";
			this.lnkNormalRecordingModeSetAsDefault.Size = new System.Drawing.Size(118, 32);
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
			this.NormalRecording.Padding = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this.NormalRecording.Size = new System.Drawing.Size(114, 26);
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
			this.lnkAdministrativeModeSetAsDefault.Location = new System.Drawing.Point(150, 0);
			this.lnkAdministrativeModeSetAsDefault.Name = "lnkAdministrativeModeSetAsDefault";
			this.lnkAdministrativeModeSetAsDefault.Size = new System.Drawing.Size(118, 33);
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
			this.Administrator.Padding = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this.Administrator.Size = new System.Drawing.Size(141, 27);
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
			this.tabPageSkipping.Location = new System.Drawing.Point(4, 22);
			this.tabPageSkipping.Name = "tabPageSkipping";
			this.tabPageSkipping.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSkipping.Size = new System.Drawing.Size(360, 343);
			this.tabPageSkipping.TabIndex = 1;
			this.tabPageSkipping.Tag = "Skipping";
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
			this._tableLayoutPanelSkipping.Location = new System.Drawing.Point(3, 3);
			this._tableLayoutPanelSkipping.Name = "_tableLayoutPanelSkipping";
			this._tableLayoutPanelSkipping.Padding = new System.Windows.Forms.Padding(11);
			this._tableLayoutPanelSkipping.RowCount = 3;
			this._tableLayoutPanelSkipping.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelSkipping.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelSkipping.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelSkipping.Size = new System.Drawing.Size(354, 337);
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
			this._lblSkippingInstructions.Location = new System.Drawing.Point(11, 44);
			this._lblSkippingInstructions.Margin = new System.Windows.Forms.Padding(0, 0, 3, 6);
			this._lblSkippingInstructions.Name = "_lblSkippingInstructions";
			this._lblSkippingInstructions.Size = new System.Drawing.Size(329, 26);
			this._lblSkippingInstructions.TabIndex = 2;
			this._lblSkippingInstructions.Text = "Select any styles whose text should never be recorded for project {0}.";
			// 
			// _lbSkippedStyles
			// 
			this._lbSkippedStyles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lbSkippedStyles.CheckOnClick = true;
			this._lbSkippedStyles.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lbSkippedStyles, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lbSkippedStyles, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._lbSkippedStyles, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._lbSkippedStyles, "AdministrativeSettings._lbSkippedStyles");
			this._lbSkippedStyles.Location = new System.Drawing.Point(14, 79);
			this._lbSkippedStyles.Name = "_lbSkippedStyles";
			this._lbSkippedStyles.Size = new System.Drawing.Size(326, 244);
			this._lbSkippedStyles.TabIndex = 0;
			// 
			// _chkShowSkipButton
			// 
			this._chkShowSkipButton.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkShowSkipButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkShowSkipButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkShowSkipButton, "AdministrativeSettings._chkShowSkipButton");
			this._chkShowSkipButton.Location = new System.Drawing.Point(14, 14);
			this._chkShowSkipButton.Name = "_chkShowSkipButton";
			this._chkShowSkipButton.Padding = new System.Windows.Forms.Padding(0, 0, 3, 10);
			this._chkShowSkipButton.Size = new System.Drawing.Size(114, 27);
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
			this._btnClearAllSkipInfo.Location = new System.Drawing.Point(117, 207);
			this._btnClearAllSkipInfo.Name = "_btnClearAllSkipInfo";
			this._btnClearAllSkipInfo.Size = new System.Drawing.Size(107, 23);
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
			this.tabPagePunctuation.Location = new System.Drawing.Point(4, 22);
			this.tabPagePunctuation.Name = "tabPagePunctuation";
			this.tabPagePunctuation.Size = new System.Drawing.Size(360, 343);
			this.tabPagePunctuation.TabIndex = 2;
			this.tabPagePunctuation.Tag = "Punctuation";
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
			this._tableLayoutPanelPunctuation.Name = "_tableLayoutPanelPunctuation";
			this._tableLayoutPanelPunctuation.Padding = new System.Windows.Forms.Padding(11);
			this._tableLayoutPanelPunctuation.RowCount = 1;
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelPunctuation.Size = new System.Drawing.Size(360, 343);
			this._tableLayoutPanelPunctuation.TabIndex = 1;
			// 
			// _lblClauseSeparators
			// 
			this._lblClauseSeparators.AutoSize = true;
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._lblClauseSeparators, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblClauseSeparators, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblClauseSeparators, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblClauseSeparators, "AdministrativeSettings._lblClauseSeparators");
			this._lblClauseSeparators.Location = new System.Drawing.Point(55, 217);
			this._lblClauseSeparators.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._lblClauseSeparators.Name = "_lblClauseSeparators";
			this._lblClauseSeparators.Size = new System.Drawing.Size(285, 26);
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
			this._lblAdditionalLineBreakCharacters.Location = new System.Drawing.Point(55, 41);
			this._lblAdditionalLineBreakCharacters.Name = "_lblAdditionalLineBreakCharacters";
			this._lblAdditionalLineBreakCharacters.Size = new System.Drawing.Size(233, 26);
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
			this._txtAdditionalBlockSeparators.Location = new System.Drawing.Point(14, 73);
			this._txtAdditionalBlockSeparators.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this._txtAdditionalBlockSeparators.MinimumSize = new System.Drawing.Size(4, 25);
			this._txtAdditionalBlockSeparators.Name = "_txtAdditionalBlockSeparators";
			this._txtAdditionalBlockSeparators.Size = new System.Drawing.Size(184, 25);
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
			this._txtClauseSeparatorCharacters.Location = new System.Drawing.Point(14, 246);
			this._txtClauseSeparatorCharacters.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this._txtClauseSeparatorCharacters.MinimumSize = new System.Drawing.Size(4, 25);
			this._txtClauseSeparatorCharacters.Name = "_txtClauseSeparatorCharacters";
			this._txtClauseSeparatorCharacters.Size = new System.Drawing.Size(184, 25);
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
			this._lblBreakBlocks.Location = new System.Drawing.Point(14, 41);
			this._lblBreakBlocks.Name = "_lblBreakBlocks";
			this._lblBreakBlocks.Size = new System.Drawing.Size(35, 29);
			this._lblBreakBlocks.TabIndex = 14;
			// 
			// _lblWarningExistingRecordings
			// 
			this._lblWarningExistingRecordings.AutoSize = true;
			this._tableLayoutPanelPunctuation.SetColumnSpan(this._lblWarningExistingRecordings, 3);
			this._lblWarningExistingRecordings.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lblWarningExistingRecordings.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblWarningExistingRecordings, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblWarningExistingRecordings, "Param 0: part 2 of the warning");
			this.l10NSharpExtender1.SetLocalizingId(this._lblWarningExistingRecordings, "AdministrativeSettings._lblWarningExistingRecordings");
			this._lblWarningExistingRecordings.Location = new System.Drawing.Point(14, 155);
			this._lblWarningExistingRecordings.Name = "_lblWarningExistingRecordings";
			this._lblWarningExistingRecordings.Size = new System.Drawing.Size(332, 39);
			this._lblWarningExistingRecordings.TabIndex = 11;
			this._lblWarningExistingRecordings.Text = "This project already has recorded clips for some existing blocks. Changing block " +
    "separation rules means that if you go back and re-record in some chapters, there" +
    " could be a misalignment. {0}";
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
			this._chkBreakAtQuotes.Location = new System.Drawing.Point(14, 14);
			this._chkBreakAtQuotes.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this._chkBreakAtQuotes.Name = "_chkBreakAtQuotes";
			this._chkBreakAtQuotes.Size = new System.Drawing.Size(242, 17);
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
			this._chkBreakAtParagraphBreaks.Location = new System.Drawing.Point(14, 111);
			this._chkBreakAtParagraphBreaks.Name = "_chkBreakAtParagraphBreaks";
			this._chkBreakAtParagraphBreaks.Size = new System.Drawing.Size(332, 41);
			this._chkBreakAtParagraphBreaks.TabIndex = 16;
			this._chkBreakAtParagraphBreaks.Text = "Treat paragraph breaks as separate recording blocks (useful for poetry)";
			this._chkBreakAtParagraphBreaks.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this._chkBreakAtParagraphBreaks.CheckedChanged += new System.EventHandler(this.UpdateWarningTextColor);
			// 
			// _cboSentenceEndingWhitespace
			// 
			this._cboSentenceEndingWhitespace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._cboSentenceEndingWhitespace.CheckOnClick = true;
			this._cboSentenceEndingWhitespace.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cboSentenceEndingWhitespace.DropDownHeight = 1;
			this._cboSentenceEndingWhitespace.DropDownWidth = 260;
			this._cboSentenceEndingWhitespace.IntegralHeight = false;
			this._cboSentenceEndingWhitespace.ItemHeight = 19;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cboSentenceEndingWhitespace, "For scriptio continua languages, select any whitespace characters that are used t" +
        "o indicate sentence breaks.");
			this.l10NSharpExtender1.SetLocalizationComment(this._cboSentenceEndingWhitespace, null);
			this.l10NSharpExtender1.SetLocalizingId(this._cboSentenceEndingWhitespace, "AdministrativeSettings._cboSentenceEndingWhitespace");
			this._cboSentenceEndingWhitespace.Location = new System.Drawing.Point(204, 73);
			this._cboSentenceEndingWhitespace.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this._cboSentenceEndingWhitespace.Name = "_cboSentenceEndingWhitespace";
			this._cboSentenceEndingWhitespace.Size = new System.Drawing.Size(142, 25);
			this._cboSentenceEndingWhitespace.SummaryDisplayMember = "";
			this._cboSentenceEndingWhitespace.TabIndex = 17;
			this._cboSentenceEndingWhitespace.ValueSeparator = ", ";
			this._cboSentenceEndingWhitespace.TextChanged += new System.EventHandler(this.UpdateWarningTextColor);
			// 
			// _cboPauseWhitespace
			// 
			this._cboPauseWhitespace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cboPauseWhitespace.CheckOnClick = true;
			this._cboPauseWhitespace.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cboPauseWhitespace.DropDownHeight = 1;
			this._cboPauseWhitespace.DropDownWidth = 260;
			this._cboPauseWhitespace.IntegralHeight = false;
			this._cboPauseWhitespace.ItemHeight = 19;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cboPauseWhitespace, "For scriptio continua languages, select any whitespace characters that are used t" +
        "o indicate pauses.");
			this.l10NSharpExtender1.SetLocalizationComment(this._cboPauseWhitespace, null);
			this.l10NSharpExtender1.SetLocalizingId(this._cboPauseWhitespace, "AdministrativeSettings._cboPauseWhitespace");
			this._cboPauseWhitespace.Location = new System.Drawing.Point(204, 246);
			this._cboPauseWhitespace.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this._cboPauseWhitespace.Name = "_cboPauseWhitespace";
			this._cboPauseWhitespace.Size = new System.Drawing.Size(142, 25);
			this._cboPauseWhitespace.SummaryDisplayMember = "";
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
			this.tabPageInterface.Location = new System.Drawing.Point(4, 22);
			this.tabPageInterface.Name = "tabPageInterface";
			this.tabPageInterface.Size = new System.Drawing.Size(360, 343);
			this.tabPageInterface.TabIndex = 3;
			this.tabPageInterface.Tag = "Interface";
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
			this._groupAdvancedUI.Location = new System.Drawing.Point(14, 172);
			this._groupAdvancedUI.Name = "_groupAdvancedUI";
			this._groupAdvancedUI.Size = new System.Drawing.Size(331, 144);
			this._groupAdvancedUI.TabIndex = 10;
			this._groupAdvancedUI.TabStop = false;
			this._groupAdvancedUI.Text = "Advanced";
			// 
			// _tableLayoutPanelAdvancedUI
			// 
			this._tableLayoutPanelAdvancedUI.ColumnCount = 1;
			this._tableLayoutPanelAdvancedUI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelAdvancedUI.Controls.Add(this._chkEnableClipShifting, 0, 1);
			this._tableLayoutPanelAdvancedUI.Controls.Add(this._lblShiftClipsMenuWarning, 0, 3);
			this._tableLayoutPanelAdvancedUI.Controls.Add(this._lblShiftClipsExplanation, 0, 2);
			this._tableLayoutPanelAdvancedUI.Controls.Add(this._chkShowCheckForProblems, 0, 0);
			this._tableLayoutPanelAdvancedUI.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanelAdvancedUI.Location = new System.Drawing.Point(3, 16);
			this._tableLayoutPanelAdvancedUI.Name = "_tableLayoutPanelAdvancedUI";
			this._tableLayoutPanelAdvancedUI.RowCount = 4;
			this._tableLayoutPanelAdvancedUI.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelAdvancedUI.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelAdvancedUI.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelAdvancedUI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelAdvancedUI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanelAdvancedUI.Size = new System.Drawing.Size(325, 125);
			this._tableLayoutPanelAdvancedUI.TabIndex = 13;
			// 
			// _chkEnableClipShifting
			// 
			this._chkEnableClipShifting.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkEnableClipShifting, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkEnableClipShifting, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkEnableClipShifting, "AdministrativeSettings._chkEnableClipShifting");
			this._chkEnableClipShifting.Location = new System.Drawing.Point(3, 24);
			this._chkEnableClipShifting.Name = "_chkEnableClipShifting";
			this._chkEnableClipShifting.Size = new System.Drawing.Size(125, 17);
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
			this._lblShiftClipsMenuWarning.Location = new System.Drawing.Point(2, 70);
			this._lblShiftClipsMenuWarning.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this._lblShiftClipsMenuWarning.Name = "_lblShiftClipsMenuWarning";
			this._lblShiftClipsMenuWarning.Size = new System.Drawing.Size(310, 55);
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
			this._lblShiftClipsExplanation.Location = new System.Drawing.Point(3, 44);
			this._lblShiftClipsExplanation.Name = "_lblShiftClipsExplanation";
			this._lblShiftClipsExplanation.Size = new System.Drawing.Size(290, 26);
			this._lblShiftClipsExplanation.TabIndex = 13;
			this._lblShiftClipsExplanation.Text = "To use this command, right-click the block slider in the main window.";
			this._lblShiftClipsExplanation.Visible = false;
			// 
			// _chkShowCheckForProblems
			// 
			this._chkShowCheckForProblems.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkShowCheckForProblems, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkShowCheckForProblems, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkShowCheckForProblems, "AdministrativeSettings._chkShowCheckForProblems");
			this._chkShowCheckForProblems.Location = new System.Drawing.Point(2, 2);
			this._chkShowCheckForProblems.Margin = new System.Windows.Forms.Padding(2);
			this._chkShowCheckForProblems.Name = "_chkShowCheckForProblems";
			this._chkShowCheckForProblems.Size = new System.Drawing.Size(236, 17);
			this._chkShowCheckForProblems.TabIndex = 14;
			this._chkShowCheckForProblems.Text = "Enable the {0} view when in protected mode";
			this._chkShowCheckForProblems.UseVisualStyleBackColor = true;
			// 
			// _chkShowBookAndChapterLabels
			// 
			this._chkShowBookAndChapterLabels.AutoSize = true;
			this._chkShowBookAndChapterLabels.Checked = true;
			this._chkShowBookAndChapterLabels.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkShowBookAndChapterLabels, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkShowBookAndChapterLabels, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkShowBookAndChapterLabels, "AdministrativeSettings._chkShowBookAndChapterLabels");
			this._chkShowBookAndChapterLabels.Location = new System.Drawing.Point(14, 16);
			this._chkShowBookAndChapterLabels.Name = "_chkShowBookAndChapterLabels";
			this._chkShowBookAndChapterLabels.Size = new System.Drawing.Size(275, 17);
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
			this.lblColorSchemeChangeRestartWarning.Location = new System.Drawing.Point(15, 116);
			this.lblColorSchemeChangeRestartWarning.Name = "lblColorSchemeChangeRestartWarning";
			this.lblColorSchemeChangeRestartWarning.Size = new System.Drawing.Size(249, 13);
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
			this._cboColorScheme.Location = new System.Drawing.Point(14, 79);
			this._cboColorScheme.Name = "_cboColorScheme";
			this._cboColorScheme.Size = new System.Drawing.Size(155, 21);
			this._cboColorScheme.TabIndex = 7;
			this._cboColorScheme.SelectedIndexChanged += new System.EventHandler(this.cboColorScheme_SelectedIndexChanged);
			// 
			// lblInterface
			// 
			this.lblInterface.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblInterface, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblInterface, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblInterface, "AdministrativeSettings.lblInterface");
			this.lblInterface.Location = new System.Drawing.Point(11, 53);
			this.lblInterface.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
			this.lblInterface.Name = "lblInterface";
			this.lblInterface.Size = new System.Drawing.Size(76, 13);
			this.lblInterface.TabIndex = 6;
			this.lblInterface.Text = "Color Scheme:";
			// 
			// tabPageRecordByVerse
			// 
			this.tabPageRecordByVerse.Controls.Add(this._tableLayoutPanelRecordByVerse);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabPageRecordByVerse, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabPageRecordByVerse, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabPageRecordByVerse, "AdministrativeSettings.tabPageRecordByVerse");
			this.tabPageRecordByVerse.Location = new System.Drawing.Point(4, 22);
			this.tabPageRecordByVerse.Name = "tabPageRecordByVerse";
			this.tabPageRecordByVerse.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageRecordByVerse.Size = new System.Drawing.Size(360, 343);
			this.tabPageRecordByVerse.TabIndex = 4;
			this.tabPageRecordByVerse.Tag = "RecordByVerse";
			this.tabPageRecordByVerse.Text = "Record by verse";
			this.tabPageRecordByVerse.UseVisualStyleBackColor = true;
			// 
			// _tableLayoutPanelRecordByVerse
			// 
			this._tableLayoutPanelRecordByVerse.BackColor = System.Drawing.SystemColors.ButtonFace;
			this._tableLayoutPanelRecordByVerse.ColumnCount = 2;
			this._tableLayoutPanelRecordByVerse.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelRecordByVerse.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanelRecordByVerse.Controls.Add(_lblRecordByVerseRangeStart, 0, 1);
			this._tableLayoutPanelRecordByVerse.Controls.Add(this._lblRecordByVerse, 0, 0);
			this._tableLayoutPanelRecordByVerse.Controls.Add(this._gridVerseRanges, 0, 4);
			this._tableLayoutPanelRecordByVerse.Controls.Add(this._lblWarningRecordByVerse, 0, 5);
			this._tableLayoutPanelRecordByVerse.Controls.Add(this._btnAddRecordByVerseRange, 1, 3);
			this._tableLayoutPanelRecordByVerse.Controls.Add(_lblRecordByVerseRangeEnd, 0, 2);
			this._tableLayoutPanelRecordByVerse.Controls.Add(this._verseCtrlRecordByVerseRangeStart, 1, 1);
			this._tableLayoutPanelRecordByVerse.Controls.Add(this._verseCtrlRecordByVerseRangeEnd, 1, 2);
			this._tableLayoutPanelRecordByVerse.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanelRecordByVerse.Location = new System.Drawing.Point(3, 3);
			this._tableLayoutPanelRecordByVerse.Name = "_tableLayoutPanelRecordByVerse";
			this._tableLayoutPanelRecordByVerse.Padding = new System.Windows.Forms.Padding(11);
			this._tableLayoutPanelRecordByVerse.RowCount = 6;
			this._tableLayoutPanelRecordByVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelRecordByVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelRecordByVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelRecordByVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelRecordByVerse.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelRecordByVerse.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelRecordByVerse.Size = new System.Drawing.Size(354, 337);
			this._tableLayoutPanelRecordByVerse.TabIndex = 0;
			// 
			// _lblRecordByVerse
			// 
			this._lblRecordByVerse.AutoSize = true;
			this._tableLayoutPanelRecordByVerse.SetColumnSpan(this._lblRecordByVerse, 2);
			this._lblRecordByVerse.Dock = System.Windows.Forms.DockStyle.Top;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblRecordByVerse, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblRecordByVerse, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblRecordByVerse, "AdministrativeSettings.lblRecordByVerse");
			this._lblRecordByVerse.Location = new System.Drawing.Point(14, 11);
			this._lblRecordByVerse.Name = "_lblRecordByVerse";
			this._lblRecordByVerse.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this._lblRecordByVerse.Size = new System.Drawing.Size(326, 62);
			this._lblRecordByVerse.TabIndex = 0;
			this._lblRecordByVerse.Text = resources.GetString("_lblRecordByVerse.Text");
			// 
			// _gridVerseRanges
			// 
			this._gridVerseRanges.AllowUserToAddRows = false;
			this._gridVerseRanges.AllowUserToDeleteRows = false;
			this._gridVerseRanges.AllowUserToOrderColumns = true;
			this._gridVerseRanges.AllowUserToResizeColumns = false;
			this._gridVerseRanges.AllowUserToResizeRows = false;
			this._gridVerseRanges.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this._gridVerseRanges.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
			this._gridVerseRanges.BackgroundColor = System.Drawing.SystemColors.Window;
			this._gridVerseRanges.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._gridVerseRanges.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this._gridVerseRanges.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this._gridVerseRanges.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this._gridVerseRanges.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFrom,
            this.colTo});
			this._tableLayoutPanelRecordByVerse.SetColumnSpan(this._gridVerseRanges, 2);
			this._gridVerseRanges.Dock = System.Windows.Forms.DockStyle.Fill;
			this._gridVerseRanges.DrawTextBoxEditControlBorder = false;
			this._gridVerseRanges.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this._gridVerseRanges.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._gridVerseRanges.FullRowFocusRectangleColor = System.Drawing.SystemColors.ControlDark;
			this._gridVerseRanges.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._gridVerseRanges, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._gridVerseRanges, null);
			this.l10NSharpExtender1.SetLocalizingId(this._gridVerseRanges, "AdministrativeSettings.gridVerseRanges");
			this._gridVerseRanges.Location = new System.Drawing.Point(14, 163);
			this._gridVerseRanges.MultiSelect = false;
			this._gridVerseRanges.Name = "_gridVerseRanges";
			this._gridVerseRanges.PaintFullRowFocusRectangle = true;
			this._gridVerseRanges.PaintHeaderAcrossFullGridWidth = true;
			this._gridVerseRanges.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this._gridVerseRanges.RowHeadersWidth = 22;
			this._gridVerseRanges.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._gridVerseRanges.SelectedCellBackColor = System.Drawing.Color.Empty;
			this._gridVerseRanges.SelectedCellForeColor = System.Drawing.Color.Empty;
			this._gridVerseRanges.SelectedRowBackColor = System.Drawing.Color.Empty;
			this._gridVerseRanges.SelectedRowForeColor = System.Drawing.Color.Empty;
			this._gridVerseRanges.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this._gridVerseRanges.ShowWaterMarkWhenDirty = false;
			this._gridVerseRanges.Size = new System.Drawing.Size(326, 108);
			this._gridVerseRanges.TabIndex = 1;
			this._gridVerseRanges.TextBoxEditControlBorderColor = System.Drawing.Color.Silver;
			this._gridVerseRanges.WaterMark = "";
			// 
			// colFrom
			// 
			this.colFrom.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colFrom.HeaderText = "_L10N_:AdministrativeSettings.gridVerseRanges.From!From";
			this.colFrom.MinimumWidth = 30;
			this.colFrom.Name = "colFrom";
			this.colFrom.ReadOnly = true;
			this.colFrom.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			// 
			// colTo
			// 
			this.colTo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colTo.HeaderText = "_L10N_:AdministrativeSettings.gridVerseRanges.To!To";
			this.colTo.MinimumWidth = 30;
			this.colTo.Name = "colTo";
			this.colTo.ReadOnly = true;
			this.colTo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// _lblWarningRecordByVerse
			// 
			this._lblWarningRecordByVerse.AutoSize = true;
			this._tableLayoutPanelRecordByVerse.SetColumnSpan(this._lblWarningRecordByVerse, 2);
			this._lblWarningRecordByVerse.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblWarningRecordByVerse, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblWarningRecordByVerse, "Param 0: part 2 of the warning");
			this.l10NSharpExtender1.SetLocalizingId(this._lblWarningRecordByVerse, "AdministrativeSettings._lblWarningRecordByVerse");
			this._lblWarningRecordByVerse.Location = new System.Drawing.Point(14, 274);
			this._lblWarningRecordByVerse.Name = "_lblWarningRecordByVerse";
			this._lblWarningRecordByVerse.Size = new System.Drawing.Size(322, 52);
			this._lblWarningRecordByVerse.TabIndex = 2;
			this._lblWarningRecordByVerse.Text = resources.GetString("_lblWarningRecordByVerse.Text");
			this._lblWarningRecordByVerse.Visible = false;
			// 
			// _btnAddRecordByVerseRange
			// 
			this._btnAddRecordByVerseRange.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._btnAddRecordByVerseRange.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnAddRecordByVerseRange, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnAddRecordByVerseRange, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnAddRecordByVerseRange, "AdministrativeSettings._btnAddRecordByVerseRange");
			this._btnAddRecordByVerseRange.Location = new System.Drawing.Point(265, 129);
			this._btnAddRecordByVerseRange.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
			this._btnAddRecordByVerseRange.Name = "_btnAddRecordByVerseRange";
			this._btnAddRecordByVerseRange.Size = new System.Drawing.Size(75, 23);
			this._btnAddRecordByVerseRange.TabIndex = 3;
			this._btnAddRecordByVerseRange.Text = "Add";
			this._btnAddRecordByVerseRange.UseVisualStyleBackColor = true;
			this._btnAddRecordByVerseRange.Click += new System.EventHandler(this._btnAddRecordByVerseRange_Click);
			// 
			// _verseCtrlRecordByVerseRangeStart
			// 
			this._verseCtrlRecordByVerseRangeStart.AdvanceToEnd = false;
			this._verseCtrlRecordByVerseRangeStart.AllowVerseSegments = true;
			this._verseCtrlRecordByVerseRangeStart.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._verseCtrlRecordByVerseRangeStart.AutoSize = true;
			this._verseCtrlRecordByVerseRangeStart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._verseCtrlRecordByVerseRangeStart.BackColor = System.Drawing.SystemColors.Control;
			this._verseCtrlRecordByVerseRangeStart.EmptyBooksColor = System.Drawing.SystemColors.GrayText;
			this._verseCtrlRecordByVerseRangeStart.EmptyBooksFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._verseCtrlRecordByVerseRangeStart, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._verseCtrlRecordByVerseRangeStart, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._verseCtrlRecordByVerseRangeStart, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._verseCtrlRecordByVerseRangeStart, "AdministrativeSettings.VerseControlStart");
			this._verseCtrlRecordByVerseRangeStart.Location = new System.Drawing.Point(148, 73);
			this._verseCtrlRecordByVerseRangeStart.Margin = new System.Windows.Forms.Padding(0);
			this._verseCtrlRecordByVerseRangeStart.Name = "_verseCtrlRecordByVerseRangeStart";
			this._verseCtrlRecordByVerseRangeStart.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._verseCtrlRecordByVerseRangeStart.ShowEmptyBooks = true;
			this._verseCtrlRecordByVerseRangeStart.Size = new System.Drawing.Size(195, 24);
			this._verseCtrlRecordByVerseRangeStart.TabIndex = 14;
			this._verseCtrlRecordByVerseRangeStart.Leave += new System.EventHandler(this.VerseCtrlRecordByVerseRangeChanged);
			// 
			// _verseCtrlRecordByVerseRangeEnd
			// 
			this._verseCtrlRecordByVerseRangeEnd.AdvanceToEnd = false;
			this._verseCtrlRecordByVerseRangeEnd.AllowVerseSegments = true;
			this._verseCtrlRecordByVerseRangeEnd.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._verseCtrlRecordByVerseRangeEnd.AutoSize = true;
			this._verseCtrlRecordByVerseRangeEnd.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._verseCtrlRecordByVerseRangeEnd.BackColor = System.Drawing.SystemColors.Control;
			this._verseCtrlRecordByVerseRangeEnd.EmptyBooksColor = System.Drawing.SystemColors.GrayText;
			this._verseCtrlRecordByVerseRangeEnd.EmptyBooksFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._verseCtrlRecordByVerseRangeEnd, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._verseCtrlRecordByVerseRangeEnd, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._verseCtrlRecordByVerseRangeEnd, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._verseCtrlRecordByVerseRangeEnd, "AdministrativeSettings.VerseControl");
			this._verseCtrlRecordByVerseRangeEnd.Location = new System.Drawing.Point(148, 97);
			this._verseCtrlRecordByVerseRangeEnd.Margin = new System.Windows.Forms.Padding(0);
			this._verseCtrlRecordByVerseRangeEnd.Name = "_verseCtrlRecordByVerseRangeEnd";
			this._verseCtrlRecordByVerseRangeEnd.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._verseCtrlRecordByVerseRangeEnd.ShowEmptyBooks = true;
			this._verseCtrlRecordByVerseRangeEnd.Size = new System.Drawing.Size(195, 24);
			this._verseCtrlRecordByVerseRangeEnd.TabIndex = 15;
			this._verseCtrlRecordByVerseRangeEnd.Leave += new System.EventHandler(this.VerseCtrlRecordByVerseRangeChanged);
			// 
			// tabPageClipEditor
			// 
			this.tabPageClipEditor.Controls.Add(this._tableLayoutPanelClipEditor);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabPageClipEditor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabPageClipEditor, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabPageClipEditor, "AdministrativeSettings.tabPageClipEditor");
			this.tabPageClipEditor.Location = new System.Drawing.Point(4, 22);
			this.tabPageClipEditor.Name = "tabPageClipEditor";
			this.tabPageClipEditor.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageClipEditor.Size = new System.Drawing.Size(360, 343);
			this.tabPageClipEditor.TabIndex = 5;
			this.tabPageClipEditor.Tag = "ClipEditor";
			this.tabPageClipEditor.Text = "Clip Editor";
			this.tabPageClipEditor.UseVisualStyleBackColor = true;
			// 
			// _tableLayoutPanelClipEditor
			// 
			this._tableLayoutPanelClipEditor.BackColor = System.Drawing.SystemColors.ButtonFace;
			this._tableLayoutPanelClipEditor.ColumnCount = 2;
			this._tableLayoutPanelClipEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanelClipEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelClipEditor.Controls.Add(this._lblInstructions, 0, 0);
			this._tableLayoutPanelClipEditor.Controls.Add(this._rdoUseSpecifiedEditor, 0, 1);
			this._tableLayoutPanelClipEditor.Controls.Add(this._rdoUseDefaultAssociatedApplication, 0, 7);
			this._tableLayoutPanelClipEditor.Controls.Add(this._lblPathToWAVFileEditor, 0, 2);
			this._tableLayoutPanelClipEditor.Controls.Add(this._lblWAVFileEditingApplicationName, 0, 3);
			this._tableLayoutPanelClipEditor.Controls.Add(this._txtEditingApplicationName, 0, 4);
			this._tableLayoutPanelClipEditor.Controls.Add(this._chkWAVEditorCommandLineArguments, 1, 3);
			this._tableLayoutPanelClipEditor.Controls.Add(this._txtCommandLineArguments, 1, 4);
			this._tableLayoutPanelClipEditor.Controls.Add(this._btnOpenFileChooser, 1, 1);
			this._tableLayoutPanelClipEditor.Controls.Add(this._lblCommandLineArgumentsInstructions, 0, 5);
			this._tableLayoutPanelClipEditor.Controls.Add(this._lblWAVEditorCommandLineExample, 1, 6);
			this._tableLayoutPanelClipEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanelClipEditor.Location = new System.Drawing.Point(3, 3);
			this._tableLayoutPanelClipEditor.Name = "_tableLayoutPanelClipEditor";
			this._tableLayoutPanelClipEditor.Padding = new System.Windows.Forms.Padding(11);
			this._tableLayoutPanelClipEditor.RowCount = 8;
			this._tableLayoutPanelClipEditor.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelClipEditor.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelClipEditor.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelClipEditor.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelClipEditor.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelClipEditor.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelClipEditor.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelClipEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelClipEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanelClipEditor.Size = new System.Drawing.Size(354, 337);
			this._tableLayoutPanelClipEditor.TabIndex = 7;
			// 
			// _lblInstructions
			// 
			this._lblInstructions.AutoSize = true;
			this._tableLayoutPanelClipEditor.SetColumnSpan(this._lblInstructions, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblInstructions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblInstructions, "Param 0: \"WAV\" (file format); Param 1: \"HearThis\" (program name); Param 2: \" Auda" +
        "city\" (third-party product name); Param 3: Name of \"Check for Problems\" mode (as" +
        " localized)");
			this.l10NSharpExtender1.SetLocalizingId(this._lblInstructions, "AdministrativeSettings._lblInstructions");
			this._lblInstructions.Location = new System.Drawing.Point(14, 11);
			this._lblInstructions.Name = "_lblInstructions";
			this._lblInstructions.Size = new System.Drawing.Size(321, 65);
			this._lblInstructions.TabIndex = 6;
			this._lblInstructions.Text = resources.GetString("_lblInstructions.Text");
			// 
			// _rdoUseSpecifiedEditor
			// 
			this._rdoUseSpecifiedEditor.AutoSize = true;
			this._rdoUseSpecifiedEditor.Checked = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._rdoUseSpecifiedEditor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._rdoUseSpecifiedEditor, "Param 0: \"WAV\" (file format)");
			this.l10NSharpExtender1.SetLocalizingId(this._rdoUseSpecifiedEditor, "AdministrativeSettings._rdoUseSpecifiedEditor");
			this._rdoUseSpecifiedEditor.Location = new System.Drawing.Point(14, 84);
			this._rdoUseSpecifiedEditor.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
			this._rdoUseSpecifiedEditor.Name = "_rdoUseSpecifiedEditor";
			this._rdoUseSpecifiedEditor.Size = new System.Drawing.Size(151, 17);
			this._rdoUseSpecifiedEditor.TabIndex = 8;
			this._rdoUseSpecifiedEditor.TabStop = true;
			this._rdoUseSpecifiedEditor.Text = "Use specified {0} file editor";
			this._rdoUseSpecifiedEditor.UseVisualStyleBackColor = true;
			this._rdoUseSpecifiedEditor.CheckedChanged += new System.EventHandler(this.HandleWavEditorOptionChanged);
			// 
			// _rdoUseDefaultAssociatedApplication
			// 
			this._rdoUseDefaultAssociatedApplication.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._rdoUseDefaultAssociatedApplication, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._rdoUseDefaultAssociatedApplication, null);
			this.l10NSharpExtender1.SetLocalizingId(this._rdoUseDefaultAssociatedApplication, "AdministrativeSettings._rdoUseDefaultAssociatedApplication");
			this._rdoUseDefaultAssociatedApplication.Location = new System.Drawing.Point(14, 290);
			this._rdoUseDefaultAssociatedApplication.Name = "_rdoUseDefaultAssociatedApplication";
			this._rdoUseDefaultAssociatedApplication.Size = new System.Drawing.Size(133, 17);
			this._rdoUseDefaultAssociatedApplication.TabIndex = 9;
			this._rdoUseDefaultAssociatedApplication.Text = "Use default application";
			this._rdoUseDefaultAssociatedApplication.UseVisualStyleBackColor = true;
			this._rdoUseDefaultAssociatedApplication.CheckedChanged += new System.EventHandler(this.HandleWavEditorOptionChanged);
			// 
			// _lblPathToWAVFileEditor
			// 
			this._lblPathToWAVFileEditor.AutoEllipsis = true;
			this._tableLayoutPanelClipEditor.SetColumnSpan(this._lblPathToWAVFileEditor, 2);
			this._lblPathToWAVFileEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblPathToWAVFileEditor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblPathToWAVFileEditor, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._lblPathToWAVFileEditor, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._lblPathToWAVFileEditor, "AdministrativeSettings._lblPathToWAVFileEditor");
			this._lblPathToWAVFileEditor.Location = new System.Drawing.Point(31, 104);
			this._lblPathToWAVFileEditor.Margin = new System.Windows.Forms.Padding(20, 0, 8, 0);
			this._lblPathToWAVFileEditor.Name = "_lblPathToWAVFileEditor";
			this._lblPathToWAVFileEditor.Size = new System.Drawing.Size(304, 13);
			this._lblPathToWAVFileEditor.TabIndex = 13;
			this._lblPathToWAVFileEditor.Text = "#";
			this._lblPathToWAVFileEditor.Visible = false;
			// 
			// _lblWAVFileEditingApplicationName
			// 
			this._lblWAVFileEditingApplicationName.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._lblWAVFileEditingApplicationName.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblWAVFileEditingApplicationName, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblWAVFileEditingApplicationName, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblWAVFileEditingApplicationName, "AdministrativeSettings._lblWAVFileEditingApplicationName");
			this._lblWAVFileEditingApplicationName.Location = new System.Drawing.Point(31, 125);
			this._lblWAVFileEditingApplicationName.Margin = new System.Windows.Forms.Padding(20, 6, 3, 4);
			this._lblWAVFileEditingApplicationName.Name = "_lblWAVFileEditingApplicationName";
			this._lblWAVFileEditingApplicationName.Size = new System.Drawing.Size(85, 13);
			this._lblWAVFileEditingApplicationName.TabIndex = 15;
			this._lblWAVFileEditingApplicationName.Text = "Name to display:";
			this._lblWAVFileEditingApplicationName.Visible = false;
			// 
			// _txtEditingApplicationName
			// 
			this._txtEditingApplicationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtEditingApplicationName.Cursor = System.Windows.Forms.Cursors.Default;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._txtEditingApplicationName, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._txtEditingApplicationName, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._txtEditingApplicationName, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._txtEditingApplicationName, "AdministrativeSettings._txtEditingApplicationName");
			this._txtEditingApplicationName.Location = new System.Drawing.Point(31, 148);
			this._txtEditingApplicationName.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
			this._txtEditingApplicationName.MinimumSize = new System.Drawing.Size(100, 20);
			this._txtEditingApplicationName.Name = "_txtEditingApplicationName";
			this._txtEditingApplicationName.Size = new System.Drawing.Size(134, 20);
			this._txtEditingApplicationName.TabIndex = 16;
			this._txtEditingApplicationName.Visible = false;
			// 
			// _chkWAVEditorCommandLineArguments
			// 
			this._chkWAVEditorCommandLineArguments.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._chkWAVEditorCommandLineArguments.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkWAVEditorCommandLineArguments, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkWAVEditorCommandLineArguments, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkWAVEditorCommandLineArguments, "AdministrativeSettings._chkWAVEditorCommandLineArguments");
			this._chkWAVEditorCommandLineArguments.Location = new System.Drawing.Point(171, 125);
			this._chkWAVEditorCommandLineArguments.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
			this._chkWAVEditorCommandLineArguments.Name = "_chkWAVEditorCommandLineArguments";
			this._chkWAVEditorCommandLineArguments.Size = new System.Drawing.Size(169, 17);
			this._chkWAVEditorCommandLineArguments.TabIndex = 12;
			this._chkWAVEditorCommandLineArguments.Text = "Command-line arguments required";
			this._chkWAVEditorCommandLineArguments.UseVisualStyleBackColor = true;
			this._chkWAVEditorCommandLineArguments.Visible = false;
			this._chkWAVEditorCommandLineArguments.CheckedChanged += new System.EventHandler(this._chkWAVEditorCommandLineArguments_CheckedChanged);
			// 
			// _txtCommandLineArguments
			// 
			this._txtCommandLineArguments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._txtCommandLineArguments, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._txtCommandLineArguments, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._txtCommandLineArguments, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._txtCommandLineArguments, "AdministrativeSettings._txtCommandLineArguments");
			this._txtCommandLineArguments.Location = new System.Drawing.Point(171, 148);
			this._txtCommandLineArguments.Name = "_txtCommandLineArguments";
			this._txtCommandLineArguments.Size = new System.Drawing.Size(169, 20);
			this._txtCommandLineArguments.TabIndex = 11;
			this._txtCommandLineArguments.Visible = false;
			// 
			// _btnOpenFileChooser
			// 
			this._btnOpenFileChooser.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._btnOpenFileChooser.Image = global::HearThis.Properties.Resources.ellipsis;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnOpenFileChooser, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnOpenFileChooser, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._btnOpenFileChooser, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._btnOpenFileChooser, "AdministrativeSettings._btnOpenFileChooser");
			this._btnOpenFileChooser.Location = new System.Drawing.Point(171, 80);
			this._btnOpenFileChooser.Name = "_btnOpenFileChooser";
			this._btnOpenFileChooser.Size = new System.Drawing.Size(24, 20);
			this._btnOpenFileChooser.TabIndex = 14;
			this._btnOpenFileChooser.UseVisualStyleBackColor = true;
			this._btnOpenFileChooser.Click += new System.EventHandler(this._btnOpenFileChooser_Click);
			// 
			// _lblCommandLineArgumentsInstructions
			// 
			this._lblCommandLineArgumentsInstructions.AutoSize = true;
			this._tableLayoutPanelClipEditor.SetColumnSpan(this._lblCommandLineArgumentsInstructions, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblCommandLineArgumentsInstructions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblCommandLineArgumentsInstructions, "Param 0: \"HearThis\" (product name); Param 1: \"{path}\" (literal text)");
			this.l10NSharpExtender1.SetLocalizingId(this._lblCommandLineArgumentsInstructions, "AdministrativeSettings._lblCommandLineArgumentsInstructions");
			this._lblCommandLineArgumentsInstructions.Location = new System.Drawing.Point(31, 177);
			this._lblCommandLineArgumentsInstructions.Margin = new System.Windows.Forms.Padding(20, 6, 3, 0);
			this._lblCommandLineArgumentsInstructions.Name = "_lblCommandLineArgumentsInstructions";
			this._lblCommandLineArgumentsInstructions.Size = new System.Drawing.Size(309, 91);
			this._lblCommandLineArgumentsInstructions.TabIndex = 17;
			this._lblCommandLineArgumentsInstructions.Text = resources.GetString("_lblCommandLineArgumentsInstructions.Text");
			this._lblCommandLineArgumentsInstructions.Visible = false;
			// 
			// _lblWAVEditorCommandLineExample
			// 
			this._lblWAVEditorCommandLineExample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lblWAVEditorCommandLineExample.AutoSize = true;
			this._lblWAVEditorCommandLineExample.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblWAVEditorCommandLineExample, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblWAVEditorCommandLineExample, "Param  is \"{path}\" (literal text)");
			this.l10NSharpExtender1.SetLocalizationPriority(this._lblWAVEditorCommandLineExample, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._lblWAVEditorCommandLineExample, "AdministrativeSettings._lblWAVEditorCommandLineExample");
			this._lblWAVEditorCommandLineExample.Location = new System.Drawing.Point(282, 274);
			this._lblWAVEditorCommandLineExample.Margin = new System.Windows.Forms.Padding(0, 6, 3, 0);
			this._lblWAVEditorCommandLineExample.Name = "_lblWAVEditorCommandLineExample";
			this._lblWAVEditorCommandLineExample.Size = new System.Drawing.Size(58, 13);
			this._lblWAVEditorCommandLineExample.TabIndex = 18;
			this._lblWAVEditorCommandLineExample.Text = "-i {0} -o {0}";
			this._lblWAVEditorCommandLineExample.Visible = false;
			// 
			// _btnOk
			// 
			this._btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnOk, "Common.OK");
			this._btnOk.Location = new System.Drawing.Point(217, 403);
			this._btnOk.Name = "_btnOk";
			this._btnOk.Size = new System.Drawing.Size(75, 23);
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
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = _btnCancel;
			this.ClientSize = new System.Drawing.Size(392, 438);
			this.Controls.Add(_btnCancel);
			this.Controls.Add(settingsProtectionLauncherButton1);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._btnOk);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "RestrictAdministrativeAccess.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(234, 477);
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
			this.tabPageRecordByVerse.ResumeLayout(false);
			this._tableLayoutPanelRecordByVerse.ResumeLayout(false);
			this._tableLayoutPanelRecordByVerse.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._gridVerseRanges)).EndInit();
			this.tabPageClipEditor.ResumeLayout(false);
			this._tableLayoutPanelClipEditor.ResumeLayout(false);
			this._tableLayoutPanelClipEditor.PerformLayout();
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
		private SIL.Windows.Forms.CheckedComboBox.CheckedComboBox _cboSentenceEndingWhitespace;
		private System.Windows.Forms.CheckBox _chkShowCheckForProblems;
		private SIL.Windows.Forms.CheckedComboBox.CheckedComboBox _cboPauseWhitespace;
		private System.Windows.Forms.TabPage tabPageRecordByVerse;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelRecordByVerse;
		private System.Windows.Forms.Label _lblRecordByVerse;
		private SIL.Windows.Forms.Widgets.BetterGrid.BetterGrid _gridVerseRanges;
		private System.Windows.Forms.Label _lblWarningRecordByVerse;
		private System.Windows.Forms.Button _btnAddRecordByVerseRange;
		private System.Windows.Forms.DataGridViewTextBoxColumn colFrom;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTo;
		private SIL.Windows.Forms.Scripture.VerseControl _verseCtrlRecordByVerseRangeStart;
		private SIL.Windows.Forms.Scripture.VerseControl _verseCtrlRecordByVerseRangeEnd;
		private System.Windows.Forms.TabPage tabPageClipEditor;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelClipEditor;
		private System.Windows.Forms.Label _lblInstructions;
		private System.Windows.Forms.RadioButton _rdoUseSpecifiedEditor;
		private System.Windows.Forms.RadioButton _rdoUseDefaultAssociatedApplication;
		private System.Windows.Forms.TextBox _txtCommandLineArguments;
		private System.Windows.Forms.CheckBox _chkWAVEditorCommandLineArguments;
		private System.Windows.Forms.Button _btnOpenFileChooser;
		private System.Windows.Forms.TextBox _txtEditingApplicationName;
		private System.Windows.Forms.Label _lblWAVFileEditingApplicationName;
		private System.Windows.Forms.Label _lblCommandLineArgumentsInstructions;
		private System.Windows.Forms.Label _lblWAVEditorCommandLineExample;
		private System.Windows.Forms.Label _lblPathToWAVFileEditor;
	}
}
