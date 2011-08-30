namespace HearThis.Publishing
{
    partial class PublishDialog
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
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this._mp3Radio = new System.Windows.Forms.RadioButton();
            this._FlacRadio = new System.Windows.Forms.RadioButton();
            this._publishButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._destinationLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._changeDestinationLink = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this._logBox = new Palaso.Progress.LogBox.LogBox();
            this._openFolderLink = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._mp3Link = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Enabled = false;
            this.radioButton1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.Location = new System.Drawing.Point(27, 75);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(60, 21);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.Text = "Saber";
            this.toolTip1.SetToolTip(this.radioButton1, "http://globalrecordings.net/en/saber");
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Enabled = false;
            this.radioButton2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.Location = new System.Drawing.Point(27, 52);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(92, 21);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "MegaVoice";
            this.toolTip1.SetToolTip(this.radioButton2, "http://www.megavoice.com/");
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // _mp3Radio
            // 
            this._mp3Radio.AutoSize = true;
            this._mp3Radio.Enabled = false;
            this._mp3Radio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._mp3Radio.Location = new System.Drawing.Point(27, 98);
            this._mp3Radio.Name = "_mp3Radio";
            this._mp3Radio.Size = new System.Drawing.Size(118, 21);
            this._mp3Radio.TabIndex = 2;
            this._mp3Radio.Text = "Folder of MP3\'s";
            this._mp3Radio.UseVisualStyleBackColor = true;
            // 
            // _FlacRadio
            // 
            this._FlacRadio.AutoSize = true;
            this._FlacRadio.Checked = true;
            this._FlacRadio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._FlacRadio.Location = new System.Drawing.Point(27, 121);
            this._FlacRadio.Name = "_FlacRadio";
            this._FlacRadio.Size = new System.Drawing.Size(120, 21);
            this._FlacRadio.TabIndex = 3;
            this._FlacRadio.TabStop = true;
            this._FlacRadio.Text = "Folder of FLAC\'s";
            this.toolTip1.SetToolTip(this._FlacRadio, "http://flac.sourceforge.net/");
            this._FlacRadio.UseVisualStyleBackColor = true;
            // 
            // _publishButton
            // 
            this._publishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._publishButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._publishButton.Location = new System.Drawing.Point(369, 230);
            this._publishButton.Name = "_publishButton";
            this._publishButton.Size = new System.Drawing.Size(80, 33);
            this._publishButton.TabIndex = 4;
            this._publishButton.Text = "&Publish";
            this._publishButton.UseVisualStyleBackColor = true;
            this._publishButton.Click += new System.EventHandler(this._publishButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cancelButton.Location = new System.Drawing.Point(277, 230);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(80, 33);
            this._cancelButton.TabIndex = 5;
            this._cancelButton.Text = "&Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _destinationLabel
            // 
            this._destinationLabel.AutoSize = true;
            this._destinationLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._destinationLabel.Location = new System.Drawing.Point(27, 187);
            this._destinationLabel.Name = "_destinationLabel";
            this._destinationLabel.Size = new System.Drawing.Size(64, 17);
            this._destinationLabel.TabIndex = 8;
            this._destinationLabel.Text = "C:\\foobar";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Destination";
            // 
            // _changeDestinationLink
            // 
            this._changeDestinationLink.AutoSize = true;
            this._changeDestinationLink.Enabled = false;
            this._changeDestinationLink.Location = new System.Drawing.Point(150, 169);
            this._changeDestinationLink.Name = "_changeDestinationLink";
            this._changeDestinationLink.Size = new System.Drawing.Size(109, 13);
            this._changeDestinationLink.TabIndex = 10;
            this._changeDestinationLink.TabStop = true;
            this._changeDestinationLink.Text = "Change Destination...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(27, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Format";
            // 
            // _logBox
            // 
            this._logBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._logBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
            this._logBox.CancelRequested = false;
            this._logBox.ErrorEncountered = false;
            this._logBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._logBox.GetDiagnosticsMethod = null;
            this._logBox.Location = new System.Drawing.Point(30, 282);
            this._logBox.Name = "_logBox";
            this._logBox.ShowCopyToClipboardMenuItem = false;
            this._logBox.ShowDetailsMenuItem = false;
            this._logBox.ShowDiagnosticsMenuItem = false;
            this._logBox.ShowFontMenuItem = false;
            this._logBox.ShowMenu = true;
            this._logBox.Size = new System.Drawing.Size(416, 175);
            this._logBox.TabIndex = 12;
            // 
            // _openFolderLink
            // 
            this._openFolderLink.AutoSize = true;
            this._openFolderLink.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._openFolderLink.Location = new System.Drawing.Point(30, 187);
            this._openFolderLink.Name = "_openFolderLink";
            this._openFolderLink.Size = new System.Drawing.Size(193, 17);
            this._openFolderLink.TabIndex = 13;
            this._openFolderLink.TabStop = true;
            this._openFolderLink.Text = "Open folder of published audio";
            this._openFolderLink.Visible = false;
            this._openFolderLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._openFolderLink_LinkClicked);
            // 
            // _mp3Link
            // 
            this._mp3Link.AutoSize = true;
            this._mp3Link.Location = new System.Drawing.Point(151, 103);
            this._mp3Link.Name = "_mp3Link";
            this._mp3Link.Size = new System.Drawing.Size(122, 13);
            this._mp3Link.TabIndex = 14;
            this._mp3Link.TabStop = true;
            this._mp3Link.Text = "Download Mp3 Encoder";
            this._mp3Link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._mp3Link_LinkClicked);
            // 
            // PublishDialog
            // 
            this.AcceptButton = this._publishButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(472, 469);
            this.Controls.Add(this._mp3Link);
            this.Controls.Add(this._openFolderLink);
            this.Controls.Add(this._logBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._changeDestinationLink);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._destinationLabel);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._publishButton);
            this.Controls.Add(this._FlacRadio);
            this.Controls.Add(this._mp3Radio);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PublishDialog";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Publish Sound Files";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton _mp3Radio;
        private System.Windows.Forms.RadioButton _FlacRadio;
        private System.Windows.Forms.Button _publishButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Label _destinationLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel _changeDestinationLink;
        private System.Windows.Forms.Label label3;
        private Palaso.Progress.LogBox.LogBox _logBox;
        private System.Windows.Forms.LinkLabel _openFolderLink;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel _mp3Link;
    }
}