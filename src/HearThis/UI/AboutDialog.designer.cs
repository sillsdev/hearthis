

namespace HearThis.UI
{
	partial class AboutDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
			this._labelVersionInfo = new System.Windows.Forms.Label();
			this.lblSubTitle = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this._releaseNotesLabel = new System.Windows.Forms.LinkLabel();
			this._checkForUpdates = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			//
			// _labelVersionInfo
			//
			resources.ApplyResources(this._labelVersionInfo, "_labelVersionInfo");
			this._labelVersionInfo.BackColor = System.Drawing.Color.Transparent;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._labelVersionInfo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._labelVersionInfo, null);
			this.l10NSharpExtender1.SetLocalizingId(this._labelVersionInfo, "AboutDialog.LabelVersionInfo");
			this._labelVersionInfo.Name = "_labelVersionInfo";
			//
			// lblSubTitle
			//
			resources.ApplyResources(this.lblSubTitle, "lblSubTitle");
			this.lblSubTitle.AutoEllipsis = true;
			this.lblSubTitle.BackColor = System.Drawing.Color.Transparent;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblSubTitle, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblSubTitle, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblSubTitle, "AboutDialog.SubTitle");
			this.lblSubTitle.Name = "lblSubTitle";
			//
			// pictureBox1
			//
			this.pictureBox1.Image = global::HearThis.Properties.Resources.i328x128;
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.pictureBox1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.pictureBox1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.pictureBox1, "AboutDialog.pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			//
			// label1
			//
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.label1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.label1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.label1, "AboutDialog.AuthorNames");
			this.label1.Name = "label1";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			//
			// linkLabel1
			//
			resources.ApplyResources(this.linkLabel1, "linkLabel1");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.linkLabel1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.linkLabel1, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.linkLabel1, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.linkLabel1, "AboutDialog.linkLabel1");
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			//
			// _releaseNotesLabel
			//
			resources.ApplyResources(this._releaseNotesLabel, "_releaseNotesLabel");
			this.l10NSharpExtender1.SetLocalizableToolTip(this._releaseNotesLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._releaseNotesLabel, null);
			this.l10NSharpExtender1.SetLocalizingId(this._releaseNotesLabel, "AboutDialog.ReleaseNotesLabel");
			this._releaseNotesLabel.Name = "_releaseNotesLabel";
			this._releaseNotesLabel.TabStop = true;
			this._releaseNotesLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._releaseNotesLabel_LinkClicked);
			//
			// _checkForUpdates
			//
			this.l10NSharpExtender1.SetLocalizableToolTip(this._checkForUpdates, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._checkForUpdates, null);
			this.l10NSharpExtender1.SetLocalizingId(this._checkForUpdates, "AboutDialog.CheckForUpdates");
			resources.ApplyResources(this._checkForUpdates, "_checkForUpdates");
			this._checkForUpdates.Name = "_checkForUpdates";
			this._checkForUpdates.UseVisualStyleBackColor = true;
			this._checkForUpdates.Click += new System.EventHandler(this._checkForUpdates_Click);
			//
			// l10NSharpExtender1
			//
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "AboutDialog";
			//
			// AboutDialog
			//
			this.AcceptButton = this._checkForUpdates;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this._checkForUpdates);
			this.Controls.Add(this._releaseNotesLabel);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.lblSubTitle);
			this.Controls.Add(this._labelVersionInfo);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "AboutDialog.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutDialog";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _labelVersionInfo;
		private System.Windows.Forms.Label lblSubTitle;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.LinkLabel _releaseNotesLabel;
		private System.Windows.Forms.Button _checkForUpdates;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
	}
}