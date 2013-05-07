

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
			this._labelVersionInfo.AutoSize = true;
			this._labelVersionInfo.BackColor = System.Drawing.Color.Transparent;
			this._labelVersionInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._labelVersionInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._labelVersionInfo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._labelVersionInfo, null);
			this.l10NSharpExtender1.SetLocalizingId(this._labelVersionInfo, "AboutDialog.LabelVersionInfo");
			this._labelVersionInfo.Location = new System.Drawing.Point(13, 265);
			this._labelVersionInfo.Name = "_labelVersionInfo";
			this._labelVersionInfo.Size = new System.Drawing.Size(167, 15);
			this._labelVersionInfo.TabIndex = 1;
			this._labelVersionInfo.Text = "Version {0}.{1}.{2}    Built on {3}";
			//
			// lblSubTitle
			//
			this.lblSubTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lblSubTitle.AutoEllipsis = true;
			this.lblSubTitle.BackColor = System.Drawing.Color.Transparent;
			this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
			this.lblSubTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblSubTitle, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblSubTitle, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblSubTitle, "AboutDialog.SubTitle");
			this.lblSubTitle.Location = new System.Drawing.Point(12, 156);
			this.lblSubTitle.Name = "lblSubTitle";
			this.lblSubTitle.Size = new System.Drawing.Size(331, 21);
			this.lblSubTitle.TabIndex = 0;
			this.lblSubTitle.Text = "Bible Audio Recording Made Fun && Easy";
			//
			// pictureBox1
			//
			this.pictureBox1.Image = global::HearThis.Properties.Resources.i328x128;
			this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.pictureBox1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.pictureBox1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.pictureBox1, "AboutDialog.pictureBox1");
			this.pictureBox1.Location = new System.Drawing.Point(15, 22);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(328, 128);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 5;
			this.pictureBox1.TabStop = false;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.label1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.label1, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.label1, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.label1, "AboutDialog.AuthorNames");
			this.label1.Location = new System.Drawing.Point(13, 187);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(241, 30);
			this.label1.TabIndex = 6;
			this.label1.Text = "John Hatton, Gordon Martin, John Thomson\r\n     SIL International";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			//
			// linkLabel1
			//
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.linkLabel1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.linkLabel1, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.linkLabel1, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.linkLabel1, "AboutDialog.linkLabel1");
			this.linkLabel1.Location = new System.Drawing.Point(13, 290);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(147, 15);
			this.linkLabel1.TabIndex = 7;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "http://HearThis.Palaso.org";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			//
			// _releaseNotesLabel
			//
			this._releaseNotesLabel.AutoSize = true;
			this._releaseNotesLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._releaseNotesLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._releaseNotesLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._releaseNotesLabel, null);
			this.l10NSharpExtender1.SetLocalizingId(this._releaseNotesLabel, "AboutDialog.ReleaseNotesLabel");
			this._releaseNotesLabel.Location = new System.Drawing.Point(13, 319);
			this._releaseNotesLabel.Name = "_releaseNotesLabel";
			this._releaseNotesLabel.Size = new System.Drawing.Size(80, 15);
			this._releaseNotesLabel.TabIndex = 8;
			this._releaseNotesLabel.TabStop = true;
			this._releaseNotesLabel.Text = "Release Notes";
			this._releaseNotesLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._releaseNotesLabel_LinkClicked);
			//
			// _checkForUpdates
			//
			this.l10NSharpExtender1.SetLocalizableToolTip(this._checkForUpdates, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._checkForUpdates, null);
			this.l10NSharpExtender1.SetLocalizingId(this._checkForUpdates, "AboutDialog.CheckForUpdates");
			this._checkForUpdates.Location = new System.Drawing.Point(16, 232);
			this._checkForUpdates.Name = "_checkForUpdates";
			this._checkForUpdates.Size = new System.Drawing.Size(128, 23);
			this._checkForUpdates.TabIndex = 9;
			this._checkForUpdates.Text = "Check For Udpates";
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
			this.ClientSize = new System.Drawing.Size(355, 355);
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
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About HearThis";
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