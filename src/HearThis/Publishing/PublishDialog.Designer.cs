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
			System.Windows.Threading.DispatcherSynchronizationContext dispatcherSynchronizationContext1 = new System.Windows.Threading.DispatcherSynchronizationContext();
			this._saberRadio = new System.Windows.Forms.RadioButton();
			this._megavoiceRadio = new System.Windows.Forms.RadioButton();
			this._mp3Radio = new System.Windows.Forms.RadioButton();
			this._oggRadio = new System.Windows.Forms.RadioButton();
			this._publishButton = new System.Windows.Forms.Button();
			this._destinationLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._openFolderLink = new System.Windows.Forms.LinkLabel();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._flacRadio = new System.Windows.Forms.RadioButton();
			this._audiBibleRadio = new System.Windows.Forms.RadioButton();
			this._mp3Link = new System.Windows.Forms.LinkLabel();
			this._saberLink = new System.Windows.Forms.LinkLabel();
			this.button1 = new System.Windows.Forms.Button();
			this._logBox = new Palaso.UI.WindowsForms.Progress.LogBox();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// _saberRadio
			// 
			this._saberRadio.AutoSize = true;
			this._saberRadio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._saberRadio, "");
			this.l10NSharpExtender1.SetLocalizationComment(this._saberRadio, "");
			this.l10NSharpExtender1.SetLocalizationPriority(this._saberRadio, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._saberRadio, "PublishDialog.Saber");
			this._saberRadio.Location = new System.Drawing.Point(28, 95);
			this._saberRadio.Name = "_saberRadio";
			this._saberRadio.Size = new System.Drawing.Size(60, 21);
			this._saberRadio.TabIndex = 2;
			this._saberRadio.Text = "Saber";
			this.toolTip1.SetToolTip(this._saberRadio, "http://globalrecordings.net/en/saber");
			this._saberRadio.UseVisualStyleBackColor = true;
			this._saberRadio.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
			// 
			// _megavoiceRadio
			// 
			this._megavoiceRadio.AutoSize = true;
			this._megavoiceRadio.Enabled = false;
			this._megavoiceRadio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._megavoiceRadio, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._megavoiceRadio, "");
			this.l10NSharpExtender1.SetLocalizationPriority(this._megavoiceRadio, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._megavoiceRadio, "PublishDialog.Megavoice");
			this._megavoiceRadio.Location = new System.Drawing.Point(28, 72);
			this._megavoiceRadio.Name = "_megavoiceRadio";
			this._megavoiceRadio.Size = new System.Drawing.Size(92, 21);
			this._megavoiceRadio.TabIndex = 1;
			this._megavoiceRadio.Text = "MegaVoice";
			this.toolTip1.SetToolTip(this._megavoiceRadio, "http://www.megavoice.com/");
			this._megavoiceRadio.UseVisualStyleBackColor = true;
			// 
			// _mp3Radio
			// 
			this._mp3Radio.AutoSize = true;
			this._mp3Radio.Enabled = false;
			this._mp3Radio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._mp3Radio, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._mp3Radio, null);
			this.l10NSharpExtender1.SetLocalizingId(this._mp3Radio, "PublishDialog.Mp3");
			this._mp3Radio.Location = new System.Drawing.Point(28, 118);
			this._mp3Radio.Name = "_mp3Radio";
			this._mp3Radio.Size = new System.Drawing.Size(118, 21);
			this._mp3Radio.TabIndex = 3;
			this._mp3Radio.Text = "Folder of MP3\'s";
			this._mp3Radio.UseVisualStyleBackColor = true;
			// 
			// _oggRadio
			// 
			this._oggRadio.AutoSize = true;
			this._oggRadio.Checked = true;
			this._oggRadio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._oggRadio, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._oggRadio, null);
			this.l10NSharpExtender1.SetLocalizingId(this._oggRadio, "PublishDialog.Ogg");
			this._oggRadio.Location = new System.Drawing.Point(28, 141);
			this._oggRadio.Name = "_oggRadio";
			this._oggRadio.Size = new System.Drawing.Size(118, 21);
			this._oggRadio.TabIndex = 4;
			this._oggRadio.TabStop = true;
			this._oggRadio.Text = "Folder of Ogg\'s";
			this.toolTip1.SetToolTip(this._oggRadio, "http://flac.sourceforge.net/");
			this._oggRadio.UseVisualStyleBackColor = true;
			// 
			// _publishButton
			// 
			this._publishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._publishButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._publishButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._publishButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._publishButton, "PublishDialog.PublishButton");
			this._publishButton.Location = new System.Drawing.Point(283, 243);
			this._publishButton.Name = "_publishButton";
			this._publishButton.Size = new System.Drawing.Size(80, 33);
			this._publishButton.TabIndex = 9;
			this._publishButton.Text = "&Publish";
			this._publishButton.UseVisualStyleBackColor = true;
			this._publishButton.Click += new System.EventHandler(this._publishButton_Click);
			// 
			// _destinationLabel
			// 
			this._destinationLabel.AutoSize = true;
			this._destinationLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._destinationLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._destinationLabel, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._destinationLabel, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._destinationLabel, "PublishDialog.DestinationPath");
			this._destinationLabel.Location = new System.Drawing.Point(30, 224);
			this._destinationLabel.Name = "_destinationLabel";
			this._destinationLabel.Size = new System.Drawing.Size(64, 17);
			this._destinationLabel.TabIndex = 8;
			this._destinationLabel.Text = "C:\\foobar";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.label2, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.label2, null);
			this.l10NSharpExtender1.SetLocalizingId(this.label2, "PublishDialog.DestinationLabel");
			this.label2.Location = new System.Drawing.Point(30, 203);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 17);
			this.label2.TabIndex = 9;
			this.label2.Text = "Destination";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.label3, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.label3, null);
			this.l10NSharpExtender1.SetLocalizingId(this.label3, "PublishDialog.Format");
			this.label3.Location = new System.Drawing.Point(27, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 17);
			this.label3.TabIndex = 11;
			this.label3.Text = "Format";
			// 
			// _openFolderLink
			// 
			this._openFolderLink.AutoSize = true;
			this._openFolderLink.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._openFolderLink, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._openFolderLink, null);
			this.l10NSharpExtender1.SetLocalizingId(this._openFolderLink, "PublishDialog.OpenFolderLink");
			this._openFolderLink.Location = new System.Drawing.Point(33, 224);
			this._openFolderLink.Name = "_openFolderLink";
			this._openFolderLink.Size = new System.Drawing.Size(193, 17);
			this._openFolderLink.TabIndex = 8;
			this._openFolderLink.TabStop = true;
			this._openFolderLink.Text = "Open folder of published audio";
			this._openFolderLink.Visible = false;
			this._openFolderLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._openFolderLink_LinkClicked);
			// 
			// _flacRadio
			// 
			this._flacRadio.AutoSize = true;
			this._flacRadio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._flacRadio, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._flacRadio, null);
			this.l10NSharpExtender1.SetLocalizingId(this._flacRadio, "PublishDialog.Flac");
			this._flacRadio.Location = new System.Drawing.Point(28, 164);
			this._flacRadio.Name = "_flacRadio";
			this._flacRadio.Size = new System.Drawing.Size(120, 21);
			this._flacRadio.TabIndex = 5;
			this._flacRadio.Text = "Folder of FLAC\'s";
			this.toolTip1.SetToolTip(this._flacRadio, "http://flac.sourceforge.net/");
			this._flacRadio.UseVisualStyleBackColor = true;
			// 
			// _audiBibleRadio
			// 
			this._audiBibleRadio.AutoSize = true;
			this._audiBibleRadio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._audiBibleRadio, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._audiBibleRadio, "");
			this.l10NSharpExtender1.SetLocalizationPriority(this._audiBibleRadio, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._audiBibleRadio, "PublishDialog.AudiBible");
			this._audiBibleRadio.Location = new System.Drawing.Point(28, 49);
			this._audiBibleRadio.Name = "_audiBibleRadio";
			this._audiBibleRadio.Size = new System.Drawing.Size(80, 21);
			this._audiBibleRadio.TabIndex = 0;
			this._audiBibleRadio.Text = "AudiBible";
			this.toolTip1.SetToolTip(this._audiBibleRadio, "http://flac.sourceforge.net/");
			this._audiBibleRadio.UseVisualStyleBackColor = true;
			// 
			// _mp3Link
			// 
			this._mp3Link.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._mp3Link, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._mp3Link, null);
			this.l10NSharpExtender1.SetLocalizingId(this._mp3Link, "PublishDialog.Mp3Link");
			this._mp3Link.Location = new System.Drawing.Point(151, 103);
			this._mp3Link.Name = "_mp3Link";
			this._mp3Link.Size = new System.Drawing.Size(117, 13);
			this._mp3Link.TabIndex = 14;
			this._mp3Link.TabStop = true;
			this._mp3Link.Text = "Requires MP3 Encoder";
			this._mp3Link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._mp3Link_LinkClicked);
			// 
			// _saberLink
			// 
			this._saberLink.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._saberLink, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._saberLink, null);
			this.l10NSharpExtender1.SetLocalizingId(this._saberLink, "PublishDialog.SaberLink");
			this._saberLink.Location = new System.Drawing.Point(151, 83);
			this._saberLink.Name = "_saberLink";
			this._saberLink.Size = new System.Drawing.Size(117, 13);
			this._saberLink.TabIndex = 15;
			this._saberLink.TabStop = true;
			this._saberLink.Text = "Requires MP3 Encoder";
			// 
			// button1
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.button1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.button1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.button1, "Common.Cancel");
			this.button1.Location = new System.Drawing.Point(369, 243);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 33);
			this.button1.TabIndex = 10;
			this.button1.Text = "&Cancel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _logBox
			// 
			this._logBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._logBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._logBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
			this._logBox.CancelRequested = false;
			this._logBox.ErrorEncountered = false;
			this._logBox.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._logBox.GetDiagnosticsMethod = null;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._logBox, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._logBox, null);
			this.l10NSharpExtender1.SetLocalizingId(this._logBox, "PublishDialog.PublishDialog.LogBox");
			this._logBox.Location = new System.Drawing.Point(30, 282);
			this._logBox.Name = "_logBox";
			this._logBox.ProgressIndicator = null;
			this._logBox.ShowCopyToClipboardMenuItem = false;
			this._logBox.ShowDetailsMenuItem = false;
			this._logBox.ShowDiagnosticsMenuItem = false;
			this._logBox.ShowFontMenuItem = false;
			this._logBox.ShowMenu = true;
			this._logBox.Size = new System.Drawing.Size(416, 196);
			this._logBox.SyncContext = dispatcherSynchronizationContext1;
			this._logBox.TabIndex = 11;
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.linkLabel1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.linkLabel1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.linkLabel1, "PublishDialog.DestinationLink");
			this.linkLabel1.Location = new System.Drawing.Point(280, 206);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(109, 13);
			this.linkLabel1.TabIndex = 7;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Change Destination...";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._changeDestinationLink_LinkClicked);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "PublishDialog";
			// 
			// PublishDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(472, 490);
			this.Controls.Add(this._audiBibleRadio);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this._flacRadio);
			this.Controls.Add(this._saberLink);
			this.Controls.Add(this._mp3Link);
			this.Controls.Add(this._openFolderLink);
			this.Controls.Add(this._logBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._destinationLabel);
			this.Controls.Add(this._publishButton);
			this.Controls.Add(this._oggRadio);
			this.Controls.Add(this._mp3Radio);
			this.Controls.Add(this._megavoiceRadio);
			this.Controls.Add(this._saberRadio);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "PublishDialog.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PublishDialog";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Publish Sound Files";
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton _saberRadio;
        private System.Windows.Forms.RadioButton _megavoiceRadio;
        private System.Windows.Forms.RadioButton _mp3Radio;
        private System.Windows.Forms.RadioButton _oggRadio;
        private System.Windows.Forms.Button _publishButton;
        private System.Windows.Forms.Label _destinationLabel;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private Palaso.UI.WindowsForms.Progress.LogBox _logBox;
        private System.Windows.Forms.LinkLabel _openFolderLink;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel _mp3Link;
        private System.Windows.Forms.LinkLabel _saberLink;
        private System.Windows.Forms.RadioButton _flacRadio;
        private System.Windows.Forms.Button button1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
        private System.Windows.Forms.RadioButton _audiBibleRadio;
    }
}