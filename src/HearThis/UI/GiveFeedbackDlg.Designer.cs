namespace HearThis.UI
{
	partial class GiveFeedbackDlg
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
				components.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GiveFeedbackDlg));
			this._btnOk = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._chkIncludeScreenshot = new System.Windows.Forms.CheckBox();
			this._linkCommunityHelp = new System.Windows.Forms.LinkLabel();
			this._txtTitle = new System.Windows.Forms.TextBox();
			this._lblTitle = new System.Windows.Forms.Label();
			this._lblTypeOfFeedback = new System.Windows.Forms.Label();
			this._lblPriorityOrSeverity = new System.Windows.Forms.Label();
			this._cboTypeOfFeedback = new System.Windows.Forms.ComboBox();
			this._cboPriority = new System.Windows.Forms.ComboBox();
			this._lblDescription = new System.Windows.Forms.Label();
			this._lblProject = new System.Windows.Forms.Label();
			this._chkIncludeRecordingInfo = new System.Windows.Forms.CheckBox();
			this._chkIncludeLog = new System.Windows.Forms.CheckBox();
			this._txtProjectOrWebsite = new System.Windows.Forms.TextBox();
			this._cboProjectOrWebsite = new System.Windows.Forms.ComboBox();
			this._lblInstructions = new System.Windows.Forms.Label();
			this._linkDonate = new System.Windows.Forms.LinkLabel();
			this._btnCancel = new System.Windows.Forms.Button();
			this._lblAffects = new System.Windows.Forms.Label();
			this._cboAffects = new System.Windows.Forms.ComboBox();
			this._lblWebsiteURL = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._richTextBoxDescription = new System.Windows.Forms.RichTextBox();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _btnOk
			// 
			this._btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnOk, "Common.OK");
			this._btnOk.Location = new System.Drawing.Point(499, 480);
			this._btnOk.Name = "_btnOk";
			this._btnOk.Size = new System.Drawing.Size(75, 23);
			this._btnOk.TabIndex = 3;
			this._btnOk.Text = "OK";
			this._btnOk.UseVisualStyleBackColor = true;
			this._btnOk.Click += new System.EventHandler(this._btnOk_Click);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "";
			// 
			// _chkIncludeScreenshot
			// 
			this._chkIncludeScreenshot.AutoSize = true;
			this._chkIncludeScreenshot.Enabled = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkIncludeScreenshot, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkIncludeScreenshot, "Param 0: HearThis main window caption");
			this.l10NSharpExtender1.SetLocalizingId(this._chkIncludeScreenshot, "GiveFeedbackDlg._chkIncludeScreenshot");
			this._chkIncludeScreenshot.Location = new System.Drawing.Point(104, 298);
			this._chkIncludeScreenshot.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
			this._chkIncludeScreenshot.Name = "_chkIncludeScreenshot";
			this._chkIncludeScreenshot.Size = new System.Drawing.Size(184, 17);
			this._chkIncludeScreenshot.TabIndex = 3;
			this._chkIncludeScreenshot.Text = "Include screenshot of {0} window";
			this._chkIncludeScreenshot.UseVisualStyleBackColor = true;
			// 
			// _linkCommunityHelp
			// 
			this._linkCommunityHelp.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this._linkCommunityHelp, 2);
			this._linkCommunityHelp.LinkArea = new System.Windows.Forms.LinkArea(67, 29);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._linkCommunityHelp, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._linkCommunityHelp, "Put square brackets around the text that should appear as the hyperlink. The text" +
        " in the brackets is localizable. However, note that the URL it will take the use" +
        "r to is in English.");
			this.l10NSharpExtender1.SetLocalizingId(this._linkCommunityHelp, "GiveFeedbackDlg._linkCommunityHelp");
			this._linkCommunityHelp.Location = new System.Drawing.Point(3, 0);
			this._linkCommunityHelp.Name = "_linkCommunityHelp";
			this._linkCommunityHelp.Padding = new System.Windows.Forms.Padding(0, 0, 0, 16);
			this._linkCommunityHelp.Size = new System.Drawing.Size(631, 46);
			this._linkCommunityHelp.TabIndex = 4;
			this._linkCommunityHelp.TabStop = true;
			this._linkCommunityHelp.Text = "Before you report a problem or make a suggestion, please visit the [Scripture Sof" +
    "tware Community] page for {0} to see if your issue is already addressed there.";
			this._linkCommunityHelp.UseCompatibleTextRendering = true;
			this._linkCommunityHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked);
			// 
			// _txtTitle
			// 
			this._txtTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._txtTitle, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._txtTitle, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._txtTitle, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._txtTitle, "GiveFeedbackDlg._txtTitle");
			this._txtTitle.Location = new System.Drawing.Point(104, 49);
			this._txtTitle.Name = "_txtTitle";
			this._txtTitle.Size = new System.Drawing.Size(538, 20);
			this._txtTitle.TabIndex = 5;
			this._txtTitle.TextChanged += new System.EventHandler(this.UpdateOkButtonState);
			// 
			// _lblTitle
			// 
			this._lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lblTitle.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblTitle, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblTitle, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblTitle, "GiveFeedbackDlg._lblTitle");
			this._lblTitle.Location = new System.Drawing.Point(25, 46);
			this._lblTitle.Name = "_lblTitle";
			this._lblTitle.Size = new System.Drawing.Size(73, 13);
			this._lblTitle.TabIndex = 6;
			this._lblTitle.Text = "Title of report*";
			this._lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _lblTypeOfFeedback
			// 
			this._lblTypeOfFeedback.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lblTypeOfFeedback.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblTypeOfFeedback, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblTypeOfFeedback, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblTypeOfFeedback, "GiveFeedbackDlg._lblTypeOfFeedback");
			this._lblTypeOfFeedback.Location = new System.Drawing.Point(3, 72);
			this._lblTypeOfFeedback.Name = "_lblTypeOfFeedback";
			this._lblTypeOfFeedback.Size = new System.Drawing.Size(95, 13);
			this._lblTypeOfFeedback.TabIndex = 7;
			this._lblTypeOfFeedback.Text = "Type of feedback*";
			this._lblTypeOfFeedback.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _lblPriorityOrSeverity
			// 
			this._lblPriorityOrSeverity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lblPriorityOrSeverity.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblPriorityOrSeverity, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblPriorityOrSeverity, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblPriorityOrSeverity, "GiveFeedbackDlg._lblPriorityOrSeverity");
			this._lblPriorityOrSeverity.Location = new System.Drawing.Point(49, 99);
			this._lblPriorityOrSeverity.Name = "_lblPriorityOrSeverity";
			this._lblPriorityOrSeverity.Size = new System.Drawing.Size(49, 13);
			this._lblPriorityOrSeverity.TabIndex = 8;
			this._lblPriorityOrSeverity.Text = "Severity*";
			this._lblPriorityOrSeverity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _cboTypeOfFeedback
			// 
			this._cboTypeOfFeedback.FormattingEnabled = true;
			this._cboTypeOfFeedback.Items.AddRange(new object[] {
            "Report problem",
            "Make a suggestion",
            "Express appreciation",
            "Donate to support software development"});
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cboTypeOfFeedback, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cboTypeOfFeedback, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._cboTypeOfFeedback, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._cboTypeOfFeedback, "GiveFeedbackDlg._cboTypeOfFeedback");
			this._cboTypeOfFeedback.Location = new System.Drawing.Point(104, 75);
			this._cboTypeOfFeedback.Name = "_cboTypeOfFeedback";
			this._cboTypeOfFeedback.Size = new System.Drawing.Size(297, 21);
			this._cboTypeOfFeedback.TabIndex = 9;
			this._cboTypeOfFeedback.SelectedIndexChanged += new System.EventHandler(this._cboTypeOfFeedback_SelectedIndexChanged);
			// 
			// _cboPriority
			// 
			this._cboPriority.FormattingEnabled = true;
			this._cboPriority.Items.AddRange(new object[] {
            "I lost data",
            "I was not able to complete a task",
            "I completed a task, but it was difficult",
            "I think the problem is minor"});
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cboPriority, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cboPriority, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._cboPriority, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._cboPriority, "GiveFeedbackDlg._cboPriority");
			this._cboPriority.Location = new System.Drawing.Point(104, 102);
			this._cboPriority.Name = "_cboPriority";
			this._cboPriority.Size = new System.Drawing.Size(297, 21);
			this._cboPriority.TabIndex = 10;
			// 
			// _lblDescription
			// 
			this._lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lblDescription.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblDescription, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblDescription, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblDescription, "GiveFeedbackDlg._lblDescription");
			this._lblDescription.Location = new System.Drawing.Point(34, 126);
			this._lblDescription.Name = "_lblDescription";
			this._lblDescription.Size = new System.Drawing.Size(64, 13);
			this._lblDescription.TabIndex = 11;
			this._lblDescription.Text = "Description*";
			this._lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _lblProject
			// 
			this._lblProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lblProject.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblProject, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblProject, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblProject, "GiveFeedbackDlg._lblProject");
			this._lblProject.Location = new System.Drawing.Point(55, 225);
			this._lblProject.Name = "_lblProject";
			this._lblProject.Size = new System.Drawing.Size(43, 13);
			this._lblProject.TabIndex = 13;
			this._lblProject.Text = "Project:";
			this._lblProject.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _chkIncludeRecordingInfo
			// 
			this._chkIncludeRecordingInfo.AutoSize = true;
			this._chkIncludeRecordingInfo.Checked = true;
			this._chkIncludeRecordingInfo.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkIncludeRecordingInfo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkIncludeRecordingInfo, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkIncludeRecordingInfo, "GiveFeedbackDlg._chkIncludeRecordingInfo");
			this._chkIncludeRecordingInfo.Location = new System.Drawing.Point(104, 321);
			this._chkIncludeRecordingInfo.Name = "_chkIncludeRecordingInfo";
			this._chkIncludeRecordingInfo.Size = new System.Drawing.Size(168, 17);
			this._chkIncludeRecordingInfo.TabIndex = 14;
			this._chkIncludeRecordingInfo.Text = "Include the last clip I recorded";
			this._chkIncludeRecordingInfo.UseVisualStyleBackColor = true;
			// 
			// _chkIncludeLog
			// 
			this._chkIncludeLog.AutoSize = true;
			this._chkIncludeLog.Checked = true;
			this._chkIncludeLog.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkIncludeLog, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkIncludeLog, null);
			this.l10NSharpExtender1.SetLocalizingId(this._chkIncludeLog, "GiveFeedbackDlg._chkIncludeLog");
			this._chkIncludeLog.Location = new System.Drawing.Point(104, 344);
			this._chkIncludeLog.Name = "_chkIncludeLog";
			this._chkIncludeLog.Size = new System.Drawing.Size(112, 17);
			this._chkIncludeLog.TabIndex = 15;
			this._chkIncludeLog.Text = "Include the log file";
			this._chkIncludeLog.UseVisualStyleBackColor = true;
			// 
			// _txtProjectOrWebsite
			// 
			this._txtProjectOrWebsite.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._txtProjectOrWebsite, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._txtProjectOrWebsite, null);
			this.l10NSharpExtender1.SetLocalizingId(this._txtProjectOrWebsite, "GiveFeedbackDlg._txtProjectOrWebsite");
			this._txtProjectOrWebsite.Location = new System.Drawing.Point(104, 255);
			this._txtProjectOrWebsite.Name = "_txtProjectOrWebsite";
			this._txtProjectOrWebsite.Size = new System.Drawing.Size(538, 20);
			this._txtProjectOrWebsite.TabIndex = 16;
			this._txtProjectOrWebsite.Visible = false;
			// 
			// _cboProjectOrWebsite
			// 
			this._cboProjectOrWebsite.FormattingEnabled = true;
			this._cboProjectOrWebsite.Items.AddRange(new object[] {
            "Current project {0}",
            "Not project-specific",
            "Other (specify below)"});
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cboProjectOrWebsite, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cboProjectOrWebsite, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._cboProjectOrWebsite, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._cboProjectOrWebsite, "GiveFeedbackDlg._cboProjectOrWebsite");
			this._cboProjectOrWebsite.Location = new System.Drawing.Point(104, 228);
			this._cboProjectOrWebsite.Name = "_cboProjectOrWebsite";
			this._cboProjectOrWebsite.Size = new System.Drawing.Size(297, 21);
			this._cboProjectOrWebsite.TabIndex = 17;
			// 
			// _lblInstructions
			// 
			this._lblInstructions.AutoSize = true;
			this._lblInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblInstructions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblInstructions, "Param 0: \"HearThis\" (product name)");
			this.l10NSharpExtender1.SetLocalizingId(this._lblInstructions, "GiveFeedbackDlg._lblInstructions");
			this._lblInstructions.Location = new System.Drawing.Point(102, 384);
			this._lblInstructions.Margin = new System.Windows.Forms.Padding(1, 20, 3, 0);
			this._lblInstructions.Name = "_lblInstructions";
			this._lblInstructions.Size = new System.Drawing.Size(526, 39);
			this._lblInstructions.TabIndex = 18;
			this._lblInstructions.Text = resources.GetString("_lblInstructions.Text");
			this._lblInstructions.Visible = false;
			// 
			// _linkDonate
			// 
			this._linkDonate.AutoSize = true;
			this._linkDonate.LinkArea = new System.Windows.Forms.LinkArea(15, 15);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._linkDonate, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._linkDonate, "Put square brackets around the text that should appear as the hyperlink. The text" +
        " in the brackets is localizable. However, note that the URL it will take the use" +
        "r to is in English.");
			this.l10NSharpExtender1.SetLocalizingId(this._linkDonate, "GiveFeedbackDlg._linkDonate");
			this._linkDonate.Location = new System.Drawing.Point(104, 433);
			this._linkDonate.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
			this._linkDonate.Name = "_linkDonate";
			this._linkDonate.Size = new System.Drawing.Size(164, 17);
			this._linkDonate.TabIndex = 19;
			this._linkDonate.TabStop = true;
			this._linkDonate.Text = "Take me to the [donation page].";
			this._linkDonate.UseCompatibleTextRendering = true;
			this._linkDonate.Visible = false;
			// 
			// _btnCancel
			// 
			this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnCancel, "Common.Cancel");
			this._btnCancel.Location = new System.Drawing.Point(580, 480);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 6;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			// 
			// _lblAffects
			// 
			this._lblAffects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lblAffects.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblAffects, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblAffects, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblAffects, "GiveFeedbackDlg._lblAffects");
			this._lblAffects.Location = new System.Drawing.Point(58, 198);
			this._lblAffects.Name = "_lblAffects";
			this._lblAffects.Size = new System.Drawing.Size(40, 13);
			this._lblAffects.TabIndex = 20;
			this._lblAffects.Text = "Affects";
			this._lblAffects.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _cboAffects
			// 
			this._cboAffects.FormattingEnabled = true;
			this._cboAffects.Items.AddRange(new object[] {
            "Does not apply",
            "Data sharing",
            "Exporting",
            "HearThis Android",
            "Installation",
            "Localization",
            "Multiple areas",
            "Navigation to desired book, chapter and block",
            "Other",
            "Playback",
            "Project administration",
            "Project selection",
            "Recording",
            "Settings",
            "Website",
            "Unknown"});
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cboAffects, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cboAffects, null);
			this.l10NSharpExtender1.SetLocalizingId(this._cboAffects, "comboBox2");
			this._cboAffects.Location = new System.Drawing.Point(104, 201);
			this._cboAffects.Name = "_cboAffects";
			this._cboAffects.Size = new System.Drawing.Size(297, 21);
			this._cboAffects.TabIndex = 21;
			// 
			// _lblWebsiteURL
			// 
			this._lblWebsiteURL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lblWebsiteURL.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblWebsiteURL, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblWebsiteURL, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblWebsiteURL, "GiveFeedbackDlg._lblWebsiteURL");
			this._lblWebsiteURL.Location = new System.Drawing.Point(15, 252);
			this._lblWebsiteURL.Name = "_lblWebsiteURL";
			this._lblWebsiteURL.Size = new System.Drawing.Size(83, 13);
			this._lblWebsiteURL.TabIndex = 22;
			this._lblWebsiteURL.Text = "URL of website:";
			this._lblWebsiteURL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._lblWebsiteURL.Visible = false;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._linkCommunityHelp, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._txtTitle, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._lblTitle, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._lblTypeOfFeedback, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._lblPriorityOrSeverity, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this._cboTypeOfFeedback, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this._cboPriority, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this._lblDescription, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this._richTextBoxDescription, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this._lblProject, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this._chkIncludeScreenshot, 1, 8);
			this.tableLayoutPanel1.Controls.Add(this._chkIncludeRecordingInfo, 1, 9);
			this.tableLayoutPanel1.Controls.Add(this._chkIncludeLog, 1, 10);
			this.tableLayoutPanel1.Controls.Add(this._txtProjectOrWebsite, 1, 7);
			this.tableLayoutPanel1.Controls.Add(this._cboProjectOrWebsite, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this._lblInstructions, 1, 11);
			this.tableLayoutPanel1.Controls.Add(this._linkDonate, 1, 12);
			this.tableLayoutPanel1.Controls.Add(this._lblAffects, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this._cboAffects, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this._lblWebsiteURL, 0, 7);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 13;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(645, 450);
			this.tableLayoutPanel1.TabIndex = 5;
			// 
			// _richTextBoxDescription
			// 
			this._richTextBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._richTextBoxDescription.Location = new System.Drawing.Point(104, 129);
			this._richTextBoxDescription.Name = "_richTextBoxDescription";
			this._richTextBoxDescription.Size = new System.Drawing.Size(538, 66);
			this._richTextBoxDescription.TabIndex = 12;
			this._richTextBoxDescription.Text = "";
			// 
			// GiveFeedbackDlg
			// 
			this.AcceptButton = this._btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._btnCancel;
			this.ClientSize = new System.Drawing.Size(670, 515);
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this._btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this, "ReportProblemDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GiveFeedbackDlg";
			this.Text = "Give Feedback";
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button _btnOk;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.CheckBox _chkIncludeScreenshot;
		private System.Windows.Forms.LinkLabel _linkCommunityHelp;
		private System.Windows.Forms.TextBox _txtTitle;
		private System.Windows.Forms.Label _lblTitle;
		private System.Windows.Forms.Label _lblTypeOfFeedback;
		private System.Windows.Forms.Label _lblPriorityOrSeverity;
		private System.Windows.Forms.ComboBox _cboTypeOfFeedback;
		private System.Windows.Forms.ComboBox _cboPriority;
		private System.Windows.Forms.Label _lblDescription;
		private System.Windows.Forms.RichTextBox _richTextBoxDescription;
		private System.Windows.Forms.Label _lblProject;
		private System.Windows.Forms.CheckBox _chkIncludeRecordingInfo;
		private System.Windows.Forms.CheckBox _chkIncludeLog;
		private System.Windows.Forms.TextBox _txtProjectOrWebsite;
		private System.Windows.Forms.ComboBox _cboProjectOrWebsite;
		private System.Windows.Forms.Label _lblInstructions;
		private System.Windows.Forms.LinkLabel _linkDonate;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Label _lblAffects;
		private System.Windows.Forms.ComboBox _cboAffects;
		private System.Windows.Forms.Label _lblWebsiteURL;
	}
}
