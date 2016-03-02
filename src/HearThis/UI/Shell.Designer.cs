using System.Drawing;

namespace HearThis.UI
{
    partial class Shell
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Shell));
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonAbout = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonChooseProject = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonPublish = new System.Windows.Forms.ToolStripButton();
			this._uiLanguageMenu = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripButtonSyncAndroid = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
			this._btnMode = new System.Windows.Forms.ToolStripDropDownButton();
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._settingsProtectionHelper = new SIL.Windows.Forms.SettingProtection.SettingsProtectionHelper(this.components);
			this._recordingToolControl1 = new HearThis.UI.RecordingToolControl();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this._toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "Shell";
			// 
			// toolStripButtonSave
			// 
			this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonSave.Image = global::HearThis.Properties.Resources.save;
			this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripButtonSave, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripButtonSave, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripButtonSave, "RecordingControl.Save");
			this.toolStripButtonSave.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
			this.toolStripButtonSave.Name = "toolStripButtonSave";
			this.toolStripButtonSave.Size = new System.Drawing.Size(23, 20);
			this.toolStripButtonSave.Text = "Save";
			this.toolStripButtonSave.Click += new System.EventHandler(this.OnSaveClick);
			// 
			// toolStripButtonAbout
			// 
			this.toolStripButtonAbout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripButtonAbout.AutoToolTip = false;
			this.toolStripButtonAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonAbout.ForeColor = System.Drawing.Color.DarkGray;
			this.toolStripButtonAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripButtonAbout, "About...");
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripButtonAbout, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripButtonAbout, "RecordingControl.About");
			this.toolStripButtonAbout.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
			this.toolStripButtonAbout.Name = "toolStripButtonAbout";
			this.toolStripButtonAbout.Size = new System.Drawing.Size(53, 20);
			this.toolStripButtonAbout.Text = "About...";
			this.toolStripButtonAbout.Click += new System.EventHandler(this.OnAboutClick);
			// 
			// toolStripButtonChooseProject
			// 
			this.toolStripButtonChooseProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonChooseProject.Image = global::HearThis.Properties.Resources.folder;
			this.toolStripButtonChooseProject.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripButtonChooseProject, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripButtonChooseProject, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripButtonChooseProject, "RecordingControl.ChooseProject");
			this.toolStripButtonChooseProject.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
			this.toolStripButtonChooseProject.Name = "toolStripButtonChooseProject";
			this.toolStripButtonChooseProject.Size = new System.Drawing.Size(23, 20);
			this.toolStripButtonChooseProject.Text = "Choose Project";
			this.toolStripButtonChooseProject.Click += new System.EventHandler(this.OnChooseProject);
			// 
			// toolStripButtonPublish
			// 
			this.toolStripButtonPublish.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripButtonPublish.AutoToolTip = false;
			this.toolStripButtonPublish.ForeColor = System.Drawing.Color.DarkGray;
			this.toolStripButtonPublish.Image = global::HearThis.Properties.Resources.sabber;
			this.toolStripButtonPublish.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolStripButtonPublish.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripButtonPublish, "Export sound files");
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripButtonPublish, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripButtonPublish, "RecordingControl.PublishSoundFiles");
			this.toolStripButtonPublish.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
			this.toolStripButtonPublish.Name = "toolStripButtonPublish";
			this.toolStripButtonPublish.Size = new System.Drawing.Size(60, 20);
			this.toolStripButtonPublish.Text = "Export";
			this.toolStripButtonPublish.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripButtonPublish.Click += new System.EventHandler(this.OnPublishClick);
			// 
			// _uiLanguageMenu
			// 
			this._uiLanguageMenu.ForeColor = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._uiLanguageMenu, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._uiLanguageMenu, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._uiLanguageMenu, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._uiLanguageMenu, "RecordingControl._uiLanguageMenu");
			this._uiLanguageMenu.Name = "_uiLanguageMenu";
			this._uiLanguageMenu.Size = new System.Drawing.Size(58, 20);
			this._uiLanguageMenu.Text = "English";
			this._uiLanguageMenu.ToolTipText = "User-interface Language";
			// 
			// toolStripButtonSyncAndroid
			// 
			this.toolStripButtonSyncAndroid.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripButtonSyncAndroid.ForeColor = System.Drawing.Color.DarkGray;
			this.toolStripButtonSyncAndroid.Image = global::HearThis.Properties.Resources.Android;
			this.toolStripButtonSyncAndroid.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripButtonSyncAndroid, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripButtonSyncAndroid, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripButtonSyncAndroid, "Shell.toolStripButton1");
			this.toolStripButtonSyncAndroid.Name = "toolStripButtonSyncAndroid";
			this.toolStripButtonSyncAndroid.Size = new System.Drawing.Size(124, 20);
			this.toolStripButtonSyncAndroid.Text = "Sync with Android";
			this.toolStripButtonSyncAndroid.Click += new System.EventHandler(this.toolStripButtonSyncAndroid_Click);
			// 
			// toolStripButtonSettings
			// 
			this.toolStripButtonSettings.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripButtonSettings.ForeColor = System.Drawing.Color.DarkGray;
			this.toolStripButtonSettings.Image = global::HearThis.Properties.Resources.settings24x24;
			this.toolStripButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripButtonSettings, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripButtonSettings, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.toolStripButtonSettings, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripButtonSettings, "RecordingControl.toolStripButtonSettings");
			this.toolStripButtonSettings.Name = "toolStripButtonSettings";
			this.toolStripButtonSettings.Size = new System.Drawing.Size(69, 20);
			this.toolStripButtonSettings.Text = "Settings";
			this.toolStripButtonSettings.ToolTipText = "Administrator Settings";
			this.toolStripButtonSettings.Click += new System.EventHandler(this.OnSettingsButtonClicked);
			// 
			// _btnMode
			// 
			this._btnMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._btnMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this._btnMode.ForeColor = System.Drawing.SystemColors.ControlDark;
			this._btnMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnMode, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnMode, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._btnMode, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._btnMode, "Shell.Shell._btnMode");
			this._btnMode.Name = "_btnMode";
			this._btnMode.Size = new System.Drawing.Size(97, 20);
			this._btnMode.Text = "Administrative";
			this._btnMode.ToolTipText = "Mode";
			this._btnMode.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ModeDropDownItemClicked);
			// 
			// _toolStrip
			// 
			this._toolStrip.AutoSize = false;
			this._toolStrip.BackColor = System.Drawing.Color.Transparent;
			this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSave,
            this.toolStripButtonAbout,
            this.toolStripButtonChooseProject,
            this.toolStripButtonSettings,
            this.toolStripButtonSyncAndroid,
            this.toolStripButtonPublish,
            this._uiLanguageMenu,
            this._btnMode});
			this.l10NSharpExtender1.SetLocalizableToolTip(this._toolStrip, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._toolStrip, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._toolStrip, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._toolStrip, "RecordingControl.ToolStrip");
			this._toolStrip.Location = new System.Drawing.Point(0, 0);
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.Padding = new System.Windows.Forms.Padding(15, 10, 20, 0);
			this._toolStrip.Size = new System.Drawing.Size(719, 33);
			this._toolStrip.TabIndex = 35;
			this._toolStrip.Text = "toolStrip1";
			// 
			// _recordingToolControl1
			// 
			this._recordingToolControl1.BackColor = this._recordingToolControl1.BackColor;
			this._recordingToolControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this._recordingToolControl1.HidingSkippedBlocks = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._recordingToolControl1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._recordingToolControl1, null);
			this.l10NSharpExtender1.SetLocalizingId(this._recordingToolControl1, "Shell.RecordingToolControl");
			this._recordingToolControl1.Location = new System.Drawing.Point(0, 0);
			this._recordingToolControl1.Name = "_recordingToolControl1";
			this._recordingToolControl1.Size = new System.Drawing.Size(719, 529);
			this._recordingToolControl1.TabIndex = 1;
			// 
			// Shell
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this.ClientSize = new System.Drawing.Size(719, 529);
			this.Controls.Add(this._toolStrip);
			this.Controls.Add(this._recordingToolControl1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this, "Shell.HearThis");
			this.MinimumSize = new System.Drawing.Size(719, 534);
			this.Name = "Shell";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "HearThis";
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this._toolStrip.ResumeLayout(false);
			this._toolStrip.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private RecordingToolControl _recordingToolControl1;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private SIL.Windows.Forms.SettingProtection.SettingsProtectionHelper _settingsProtectionHelper;
		private System.Windows.Forms.ToolStripButton toolStripButtonSave;
		private System.Windows.Forms.ToolStripButton toolStripButtonAbout;
		private System.Windows.Forms.ToolStripButton toolStripButtonChooseProject;
		private System.Windows.Forms.ToolStripButton toolStripButtonPublish;
		private System.Windows.Forms.ToolStripDropDownButton _uiLanguageMenu;
		private System.Windows.Forms.ToolStripButton toolStripButtonSyncAndroid;
		private System.Windows.Forms.ToolStripButton toolStripButtonSettings;
		private System.Windows.Forms.ToolStripDropDownButton _btnMode;
		private System.Windows.Forms.ToolStrip _toolStrip;
    }
}

