

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
			this._labelVersionInfo = new System.Windows.Forms.Label();
			this.lblSubTitle = new System.Windows.Forms.Label();
			this._buttonOK = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this._releaseNotesLabel = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			//
			// _labelVersionInfo
			//
			resources.ApplyResources(this._labelVersionInfo, "_labelVersionInfo");
			this._labelVersionInfo.BackColor = System.Drawing.Color.Transparent;
			this._labelVersionInfo.Name = "_labelVersionInfo";
			//
			// lblSubTitle
			//
			resources.ApplyResources(this.lblSubTitle, "lblSubTitle");
			this.lblSubTitle.AutoEllipsis = true;
			this.lblSubTitle.BackColor = System.Drawing.Color.Transparent;
			this.lblSubTitle.Name = "lblSubTitle";
			//
			// _buttonOK
			//
			resources.ApplyResources(this._buttonOK, "_buttonOK");
			this._buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._buttonOK.Name = "_buttonOK";
			this._buttonOK.UseVisualStyleBackColor = true;
			//
			// pictureBox1
			//
			this.pictureBox1.Image = global::HearThis.Properties.Resources.i328x128;
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			//
			// label1
			//
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Name = "label1";
			//
			// linkLabel1
			//
			resources.ApplyResources(this.linkLabel1, "linkLabel1");
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			//
			// _releaseNotesLabel
			//
			resources.ApplyResources(this._releaseNotesLabel, "_releaseNotesLabel");
			this._releaseNotesLabel.Name = "_releaseNotesLabel";
			this._releaseNotesLabel.TabStop = true;
			this._releaseNotesLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._releaseNotesLabel_LinkClicked);
			//
			// AboutDialog
			//
			this.AcceptButton = this._buttonOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this._releaseNotesLabel);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this._buttonOK);
			this.Controls.Add(this.lblSubTitle);
			this.Controls.Add(this._labelVersionInfo);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutDialog";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _labelVersionInfo;
		private System.Windows.Forms.Label lblSubTitle;
		private System.Windows.Forms.Button _buttonOK;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.LinkLabel _releaseNotesLabel;
	}
}