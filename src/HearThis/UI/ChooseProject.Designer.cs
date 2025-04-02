namespace HearThis.UI
{
    partial class ChooseProject
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseProject));
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._linkCreateFromBundle = new System.Windows.Forms.LinkLabel();
			this._linkCreateFromGlyssenScript = new System.Windows.Forms.LinkLabel();
			this._linkFindParatextProjectsFolder = new System.Windows.Forms.LinkLabel();
			this._lblParatextNotInstalled = new System.Windows.Forms.Label();
			this._projectsList = new HearThis.UI.ExistingProjectsList();
			this._lblNoParatextProjectsInFolder = new System.Windows.Forms.Label();
			this._lblParatextProjectsFolderLabel = new System.Windows.Forms.Label();
			this.m_btnFindParatextProjectsFolder = new System.Windows.Forms.Button();
			this._lblParatextProjectsFolder = new System.Windows.Forms.Label();
			this._lblNoParatextProjects = new System.Windows.Forms.Label();
			this._lblParatext7Installed = new System.Windows.Forms.Label();
			this._tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
			this._tableLayoutPanelParatextProjectsFolder = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this._tableLayoutPanelMain.SuspendLayout();
			this._tableLayoutPanelParatextProjectsFolder.SuspendLayout();
			this.SuspendLayout();
			//
			// _okButton
			//
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._okButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._okButton, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._okButton, L10NSharp.LocalizationPriority.High);
			this.l10NSharpExtender1.SetLocalizingId(this._okButton, "Common.OK");
			this._okButton.Location = new System.Drawing.Point(213, 296);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(69, 24);
			this._okButton.TabIndex = 1;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			//
			// _cancelButton
			//
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cancelButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cancelButton, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._cancelButton, L10NSharp.LocalizationPriority.High);
			this.l10NSharpExtender1.SetLocalizingId(this._cancelButton, "Common.Cancel");
			this._cancelButton.Location = new System.Drawing.Point(288, 296);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(69, 24);
			this._cancelButton.TabIndex = 2;
			this._cancelButton.Text = "&Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			//
			// l10NSharpExtender1
			//
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "";
			//
			// _linkCreateFromBundle
			//
			this._linkCreateFromBundle.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._linkCreateFromBundle, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._linkCreateFromBundle, null);
			this.l10NSharpExtender1.SetLocalizingId(this._linkCreateFromBundle, "ChooseProject._linkCreateFromBundle");
			this._linkCreateFromBundle.Location = new System.Drawing.Point(0, 245);
			this._linkCreateFromBundle.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this._linkCreateFromBundle.Name = "_linkCreateFromBundle";
			this._linkCreateFromBundle.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
			this._linkCreateFromBundle.Size = new System.Drawing.Size(211, 23);
			this._linkCreateFromBundle.TabIndex = 3;
			this._linkCreateFromBundle.TabStop = true;
			this._linkCreateFromBundle.Text = "Create new project from text release bundle";
			this._linkCreateFromBundle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._linkCreateFromBundle_LinkClicked);
	        //
	        // _linkCreateFromGlyssenScript
	        //
	        this._linkCreateFromGlyssenScript.AutoSize = true;
	        this.l10NSharpExtender1.SetLocalizableToolTip(this._linkCreateFromGlyssenScript, null);
	        this.l10NSharpExtender1.SetLocalizationComment(this._linkCreateFromGlyssenScript, null);
	        this.l10NSharpExtender1.SetLocalizingId(this._linkCreateFromGlyssenScript, "ChooseProject._linkCreateFromGlyssenScript");
	        this._linkCreateFromGlyssenScript.Location = new System.Drawing.Point(0, 275);
	        this._linkCreateFromGlyssenScript.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
	        this._linkCreateFromGlyssenScript.Name = "_linkCreateFromGlyssenScript";
	        this._linkCreateFromGlyssenScript.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
	        this._linkCreateFromGlyssenScript.Size = new System.Drawing.Size(211, 23);
	        this._linkCreateFromGlyssenScript.TabIndex = 3;
	        this._linkCreateFromGlyssenScript.TabStop = true;
	        this._linkCreateFromGlyssenScript.Text = "Open GlyssenScript for dramatized recordings";
	        this._linkCreateFromGlyssenScript.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._linkCreateFromGlyssenScript_LinkClicked);
			//
			// _linkFindParatextProjectsFolder
			//
			this._linkFindParatextProjectsFolder.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._linkFindParatextProjectsFolder, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._linkFindParatextProjectsFolder, null);
			this.l10NSharpExtender1.SetLocalizingId(this._linkFindParatextProjectsFolder, "ChooseProject._linkFindParatextProjectsFolder");
			this._linkFindParatextProjectsFolder.Location = new System.Drawing.Point(0, 222);
			this._linkFindParatextProjectsFolder.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this._linkFindParatextProjectsFolder.Name = "_linkFindParatextProjectsFolder";
			this._linkFindParatextProjectsFolder.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
			this._linkFindParatextProjectsFolder.Size = new System.Drawing.Size(138, 23);
			this._linkFindParatextProjectsFolder.TabIndex = 4;
			this._linkFindParatextProjectsFolder.TabStop = true;
			this._linkFindParatextProjectsFolder.Text = "Find Paratext projects folder";
			this._linkFindParatextProjectsFolder.Visible = false;
			this._linkFindParatextProjectsFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._linkFindParatextProjectsFolder_LinkClicked);
			//
			// _lblParatextNotInstalled
			//
			this._lblParatextNotInstalled.AutoSize = true;
			this._lblParatextNotInstalled.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblParatextNotInstalled, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblParatextNotInstalled, "Displayed when no version of Paratext is installed (or registration info has not been f" +
				"illed in) and user has not specified a location where Paratext data can be found. Param 0 is \"Paratext\" (program name); Params 1 and 2 are \"8\" and \"9\" (major ve" +
				"rsion numbers).");
			this.l10NSharpExtender1.SetLocalizingId(this._lblParatextNotInstalled, "ChooseProject._lblParatextNotInstalled");
			this._lblParatextNotInstalled.Location = new System.Drawing.Point(0, 139);
			this._lblParatextNotInstalled.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this._lblParatextNotInstalled.Name = "_lblParatextNotInstalled";
			this._lblParatextNotInstalled.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
			this._lblParatextNotInstalled.Size = new System.Drawing.Size(278, 23);
			this._lblParatextNotInstalled.TabIndex = 6;
			this._lblParatextNotInstalled.Text = "{0} {1} or {2} does not appear to be installed on this computer, or it has not yet been registered.";
			this._lblParatextNotInstalled.Visible = false;
			//
			// _projectsList
			//
			this._projectsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._projectsList.IncludeHiddenProjects = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._projectsList, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._projectsList, null);
			this.l10NSharpExtender1.SetLocalizingId(this._projectsList, "ChooseProject.ExistingProjectsList");
			this._projectsList.Location = new System.Drawing.Point(3, 3);
			this._projectsList.Name = "_projectsList";
			this._projectsList.Size = new System.Drawing.Size(339, 74);
			this._projectsList.TabIndex = 0;
			this._projectsList.SelectedProjectChanged += new System.EventHandler(this._projectsList_SelectedProjectChanged);
			this._projectsList.DoubleClick += new System.EventHandler(this._projectsList_DoubleClick);
			//
			// _lblNoParatextProjectsInFolder
			//
			this._lblNoParatextProjectsInFolder.AutoSize = true;
			this._lblNoParatextProjectsInFolder.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblNoParatextProjectsInFolder, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblNoParatextProjectsInFolder, "Displayed when Paratext is not installed and user has chosen a folder that does n" +
        "ot contain any Paratext projects.");
			this.l10NSharpExtender1.SetLocalizingId(this._lblNoParatextProjectsInFolder, "ChooseProject._lblNoParatextProjectsInFolder");
			this._lblNoParatextProjectsInFolder.Location = new System.Drawing.Point(0, 93);
			this._lblNoParatextProjectsInFolder.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this._lblNoParatextProjectsInFolder.Name = "_lblNoParatextProjectsInFolder";
			this._lblNoParatextProjectsInFolder.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
			this._lblNoParatextProjectsInFolder.Size = new System.Drawing.Size(221, 23);
			this._lblNoParatextProjectsInFolder.TabIndex = 7;
			this._lblNoParatextProjectsInFolder.Text = "No Paratext projects were found in this folder:";
			this._lblNoParatextProjectsInFolder.Visible = false;
			//
			// _lblParatextProjectsFolderLabel
			//
			this._lblParatextProjectsFolderLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._lblParatextProjectsFolderLabel.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblParatextProjectsFolderLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblParatextProjectsFolderLabel, "Displayed when Paratext is not installed and user has chosen a folder that contai" +
        "ns one or more Paratext projects.");
			this.l10NSharpExtender1.SetLocalizingId(this._lblParatextProjectsFolderLabel, "ChooseProject._lblParatextProjectsFolderLabel");
			this._lblParatextProjectsFolderLabel.Location = new System.Drawing.Point(0, 80);
			this._lblParatextProjectsFolderLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this._lblParatextProjectsFolderLabel.Name = "_lblParatextProjectsFolderLabel";
			this._lblParatextProjectsFolderLabel.Size = new System.Drawing.Size(118, 13);
			this._lblParatextProjectsFolderLabel.TabIndex = 0;
			this._lblParatextProjectsFolderLabel.Text = "Paratext projects folder:";
			this._lblParatextProjectsFolderLabel.Visible = false;
			//
			// m_btnFindParatextProjectsFolder
			//
			this.m_btnFindParatextProjectsFolder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_btnFindParatextProjectsFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.m_btnFindParatextProjectsFolder.Image = global::HearThis.Properties.Resources.ellipsis;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnFindParatextProjectsFolder, "Select folder where Paratext projects are located");
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnFindParatextProjectsFolder, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnFindParatextProjectsFolder, "ChooseProject.m_btnFindParatextProjectsFolder");
			this.m_btnFindParatextProjectsFolder.Location = new System.Drawing.Point(312, 3);
			this.m_btnFindParatextProjectsFolder.Name = "m_btnFindParatextProjectsFolder";
			this.m_btnFindParatextProjectsFolder.Size = new System.Drawing.Size(30, 18);
			this.m_btnFindParatextProjectsFolder.TabIndex = 1;
			this.m_btnFindParatextProjectsFolder.UseVisualStyleBackColor = true;
			this.m_btnFindParatextProjectsFolder.Click += new System.EventHandler(this.HandleFindParatextProjectsFolderButtonClicked);
			//
			// _lblParatextProjectsFolder
			//
			this._lblParatextProjectsFolder.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._lblParatextProjectsFolder.AutoEllipsis = true;
			this._lblParatextProjectsFolder.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblParatextProjectsFolder, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblParatextProjectsFolder, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._lblParatextProjectsFolder, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._lblParatextProjectsFolder, "ChooseProject._lblParatextProjectsFolder");
			this._lblParatextProjectsFolder.Location = new System.Drawing.Point(3, 5);
			this._lblParatextProjectsFolder.Name = "_lblParatextProjectsFolder";
			this._lblParatextProjectsFolder.Size = new System.Drawing.Size(14, 13);
			this._lblParatextProjectsFolder.TabIndex = 2;
			this._lblParatextProjectsFolder.Text = "#";
			//
			// _lblNoParatextProjects
			//
			this._lblNoParatextProjects.AutoSize = true;
			this._lblNoParatextProjects.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblNoParatextProjects, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblNoParatextProjects, "Displayed when Paratext is installed but there are no Paratext projects.");
			this.l10NSharpExtender1.SetLocalizingId(this._lblNoParatextProjects, "ChooseProject._lblNoParatextProjects");
			this._lblNoParatextProjects.Location = new System.Drawing.Point(0, 116);
			this._lblNoParatextProjects.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this._lblNoParatextProjects.Name = "_lblNoParatextProjects";
			this._lblNoParatextProjects.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
			this._lblNoParatextProjects.Size = new System.Drawing.Size(243, 23);
			this._lblNoParatextProjects.TabIndex = 9;
			this._lblNoParatextProjects.Text = "No Paratext projects were found on this computer.";
			this._lblNoParatextProjects.Visible = false;
			//
			// _lblParatext7Installed
			//
			this._lblParatext7Installed.AutoSize = true;
			this._lblParatext7Installed.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblParatext7Installed, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblParatext7Installed, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblParatext7Installed, "ChooseProject._lblParatext7Installed");
			this._lblParatext7Installed.Location = new System.Drawing.Point(3, 162);
			this._lblParatext7Installed.Name = "_lblParatext7Installed";
			this._lblParatext7Installed.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
			this._lblParatext7Installed.Size = new System.Drawing.Size(320, 36);
			this._lblParatext7Installed.TabIndex = 10;
			this._lblParatext7Installed.Text = "This computer has Paratext 7 installed. To open a Paratext 7 project use a " +
    "version of HearThis earlier than 1.5.";
			this._lblParatext7Installed.Visible = false;
			//
			// _tableLayoutPanelMain
			//
			this._tableLayoutPanelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutPanelMain.ColumnCount = 1;
			this._tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelMain.Controls.Add(this._lblParatextProjectsFolderLabel, 0, 1);
			this._tableLayoutPanelMain.Controls.Add(this._linkFindParatextProjectsFolder, 0, 7);
			this._tableLayoutPanelMain.Controls.Add(this._lblParatextNotInstalled, 0, 4);
			this._tableLayoutPanelMain.Controls.Add(this._projectsList, 0, 0);
			this._tableLayoutPanelMain.Controls.Add(this._linkCreateFromBundle, 0, 8);
	        this._tableLayoutPanelMain.Controls.Add(this._linkCreateFromGlyssenScript, 0, 9);
			this._tableLayoutPanelMain.Controls.Add(this._lblNoParatextProjectsInFolder, 0, 2);
			this._tableLayoutPanelMain.Controls.Add(this._tableLayoutPanelParatextProjectsFolder, 0, 6);
			this._tableLayoutPanelMain.Controls.Add(this._lblNoParatextProjects, 0, 3);
			this._tableLayoutPanelMain.Controls.Add(this._lblParatext7Installed, 0, 5);
			this._tableLayoutPanelMain.Location = new System.Drawing.Point(12, 12);
			this._tableLayoutPanelMain.Name = "_tableLayoutPanelMain";
			this._tableLayoutPanelMain.RowCount = 10;
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
	        this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelMain.Size = new System.Drawing.Size(345, 268);
			this._tableLayoutPanelMain.TabIndex = 4;
			//
			// _tableLayoutPanelParatextProjectsFolder
			//
			this._tableLayoutPanelParatextProjectsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutPanelParatextProjectsFolder.AutoSize = true;
			this._tableLayoutPanelParatextProjectsFolder.ColumnCount = 2;
			this._tableLayoutPanelParatextProjectsFolder.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelParatextProjectsFolder.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanelParatextProjectsFolder.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanelParatextProjectsFolder.Controls.Add(this.m_btnFindParatextProjectsFolder, 1, 0);
			this._tableLayoutPanelParatextProjectsFolder.Controls.Add(this._lblParatextProjectsFolder, 0, 0);
			this._tableLayoutPanelParatextProjectsFolder.Location = new System.Drawing.Point(0, 198);
			this._tableLayoutPanelParatextProjectsFolder.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutPanelParatextProjectsFolder.Name = "_tableLayoutPanelParatextProjectsFolder";
			this._tableLayoutPanelParatextProjectsFolder.RowCount = 1;
			this._tableLayoutPanelParatextProjectsFolder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelParatextProjectsFolder.Size = new System.Drawing.Size(345, 24);
			this._tableLayoutPanelParatextProjectsFolder.TabIndex = 8;
			this._tableLayoutPanelParatextProjectsFolder.Visible = false;
			//
			// ChooseProject
			//
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(369, 332);
			this.Controls.Add(this._tableLayoutPanelMain);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "ChooseProject.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(385, 370);
			this.Name = "ChooseProject";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Choose Project";
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this._tableLayoutPanelMain.ResumeLayout(false);
			this._tableLayoutPanelMain.PerformLayout();
			this._tableLayoutPanelParatextProjectsFolder.ResumeLayout(false);
			this._tableLayoutPanelParatextProjectsFolder.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private HearThis.UI.ExistingProjectsList _projectsList;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.LinkLabel _linkCreateFromBundle;
	    private System.Windows.Forms.LinkLabel _linkCreateFromGlyssenScript;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelMain;
		private System.Windows.Forms.LinkLabel _linkFindParatextProjectsFolder;
		private System.Windows.Forms.Label _lblParatextNotInstalled;
		private System.Windows.Forms.Label _lblParatextProjectsFolderLabel;
		private System.Windows.Forms.Label _lblNoParatextProjectsInFolder;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelParatextProjectsFolder;
		private System.Windows.Forms.Button m_btnFindParatextProjectsFolder;
		private System.Windows.Forms.Label _lblParatextProjectsFolder;
		private System.Windows.Forms.Label _lblNoParatextProjects;
		private System.Windows.Forms.Label _lblParatext7Installed;
	}
}
