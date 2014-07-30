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
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonPublish = new System.Windows.Forms.ToolStripButton();
			this._uiLanguageMenu = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
			this._settingsProtectionHelper = new Palaso.UI.WindowsForms.SettingProtection.SettingsProtectionHelper(this.components);
			this._cboMode = new System.Windows.Forms.ToolStripComboBox();
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
			// _toolStrip
			// 
			this._toolStrip.AutoSize = false;
			this._toolStrip.BackColor = System.Drawing.Color.Transparent;
			this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton4,
            this.toolStripButton3,
            this.toolStripButtonPublish,
            this._uiLanguageMenu,
            this.toolStripButtonSettings,
            this._cboMode});
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
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = global::HearThis.Properties.Resources.save;
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripButton1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripButton1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripButton1, "RecordingControl.Save");
			this.toolStripButton1.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(23, 20);
			this.toolStripButton1.Text = "Save";
			this.toolStripButton1.Click += new System.EventHandler(this.OnSaveClick);
			// 
			// toolStripButton4
			// 
			this.toolStripButton4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripButton4.AutoToolTip = false;
			this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton4.ForeColor = System.Drawing.Color.DarkGray;
			this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripButton4, "About...");
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripButton4, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripButton4, "RecordingControl.About");
			this.toolStripButton4.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
			this.toolStripButton4.Name = "toolStripButton4";
			this.toolStripButton4.Size = new System.Drawing.Size(53, 20);
			this.toolStripButton4.Text = "About...";
			this.toolStripButton4.Click += new System.EventHandler(this.OnAboutClick);
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton3.Image = global::HearThis.Properties.Resources.folder;
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripButton3, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripButton3, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripButton3, "RecordingControl.ChooseProject");
			this.toolStripButton3.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(23, 20);
			this.toolStripButton3.Text = "Choose Paratext project";
			this.toolStripButton3.Click += new System.EventHandler(this.OnChooseProject);
			// 
			// toolStripButtonPublish
			// 
			this.toolStripButtonPublish.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripButtonPublish.AutoToolTip = false;
			this.toolStripButtonPublish.ForeColor = System.Drawing.Color.DarkGray;
			this.toolStripButtonPublish.Image = global::HearThis.Properties.Resources.sabber;
			this.toolStripButtonPublish.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolStripButtonPublish.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripButtonPublish, "Publish sound files");
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripButtonPublish, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripButtonPublish, "RecordingControl.PublishSoundFiles");
			this.toolStripButtonPublish.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
			this.toolStripButtonPublish.Name = "toolStripButtonPublish";
			this.toolStripButtonPublish.Size = new System.Drawing.Size(66, 20);
			this.toolStripButtonPublish.Text = "Publish";
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
			// 
			// toolStripButtonSettings
			// 
			this.toolStripButtonSettings.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripButtonSettings.AutoToolTip = false;
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
			this.toolStripButtonSettings.Click += new System.EventHandler(this.OnSettingsButtonClicked);
			// 
			// _cboMode
			// 
			this._cboMode.AutoToolTip = true;
			this._cboMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._cboMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboMode.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cboMode, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cboMode, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._cboMode, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._cboMode, "Shell.Shell._cboMode");
			this._cboMode.Name = "_cboMode";
			this._cboMode.Size = new System.Drawing.Size(180, 23);
			this._cboMode.ToolTipText = "Select the view mode for HearThis";
			this._cboMode.SelectedIndexChanged += new System.EventHandler(this.SelectedModeChanged);
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
			this.l10NSharpExtender1.SetLocalizingId(this, "MainWindow.UnusedTitle");
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
		private System.Windows.Forms.ToolStrip _toolStrip;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton4;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripButton toolStripButtonPublish;
		private System.Windows.Forms.ToolStripDropDownButton _uiLanguageMenu;
		private System.Windows.Forms.ToolStripButton toolStripButtonSettings;
		private Palaso.UI.WindowsForms.SettingProtection.SettingsProtectionHelper _settingsProtectionHelper;
		private System.Windows.Forms.ToolStripComboBox _cboMode;
    }
}

