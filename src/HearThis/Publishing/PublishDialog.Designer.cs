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
			this._none = new System.Windows.Forms.RadioButton();
			this._mp3Link = new System.Windows.Forms.LinkLabel();
			this._saberLink = new System.Windows.Forms.LinkLabel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._logBox = new Palaso.UI.WindowsForms.Progress.LogBox();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this._cueSheet = new System.Windows.Forms.RadioButton();
			this._audacityLabelFile = new System.Windows.Forms.RadioButton();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
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
			this._saberRadio.Location = new System.Drawing.Point(3, 77);
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
			this._megavoiceRadio.Location = new System.Drawing.Point(3, 50);
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
			this._mp3Radio.Location = new System.Drawing.Point(3, 104);
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
			this._oggRadio.Location = new System.Drawing.Point(3, 131);
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
			this._publishButton.Location = new System.Drawing.Point(393, 237);
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
			this._destinationLabel.Location = new System.Drawing.Point(27, 237);
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
			this.label2.Location = new System.Drawing.Point(27, 211);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 17);
			this.label2.TabIndex = 9;
			this.label2.Text = "Destination";
			this.label2.Click += new System.EventHandler(this.label2_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.label3, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.label3, null);
			this.l10NSharpExtender1.SetLocalizingId(this.label3, "PublishDialog.Format");
			this.label3.Location = new System.Drawing.Point(3, 0);
			this.label3.Name = "label3";
			this.label3.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.label3.Size = new System.Drawing.Size(93, 20);
			this.label3.TabIndex = 11;
			this.label3.Text = "Audio Format";
			this.label3.Click += new System.EventHandler(this.label3_Click);
			// 
			// _openFolderLink
			// 
			this._openFolderLink.AutoSize = true;
			this._openFolderLink.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._openFolderLink, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._openFolderLink, null);
			this.l10NSharpExtender1.SetLocalizingId(this._openFolderLink, "PublishDialog.OpenFolderLink");
			this._openFolderLink.Location = new System.Drawing.Point(27, 237);
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
			this._flacRadio.Location = new System.Drawing.Point(3, 158);
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
			this._audiBibleRadio.Location = new System.Drawing.Point(3, 23);
			this._audiBibleRadio.Name = "_audiBibleRadio";
			this._audiBibleRadio.Size = new System.Drawing.Size(80, 21);
			this._audiBibleRadio.TabIndex = 0;
			this._audiBibleRadio.Text = "AudiBible";
			this.toolTip1.SetToolTip(this._audiBibleRadio, "http://flac.sourceforge.net/");
			this._audiBibleRadio.UseVisualStyleBackColor = true;
			// 
			// _none
			// 
			this._none.AutoSize = true;
			this._none.Checked = true;
			this._none.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._none, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._none, "");
			this.l10NSharpExtender1.SetLocalizationPriority(this._none, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._none, "PublishDialog.AudiBible");
			this._none.Location = new System.Drawing.Point(18, 23);
			this._none.Name = "_none";
			this._none.Size = new System.Drawing.Size(58, 21);
			this._none.TabIndex = 1;
			this._none.TabStop = true;
			this._none.Text = "None";
			this._none.UseVisualStyleBackColor = true;
			// 
			// _mp3Link
			// 
			this._mp3Link.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._mp3Link, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._mp3Link, null);
			this.l10NSharpExtender1.SetLocalizingId(this._mp3Link, "PublishDialog.Mp3Link");
			this._mp3Link.Location = new System.Drawing.Point(130, 101);
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
			this.l10NSharpExtender1.SetLocalizingId(this._saberLink, "PublishDialog.Mp3Link");
			this._saberLink.Location = new System.Drawing.Point(130, 74);
			this._saberLink.Name = "_saberLink";
			this._saberLink.Size = new System.Drawing.Size(117, 13);
			this._saberLink.TabIndex = 15;
			this._saberLink.TabStop = true;
			this._saberLink.Text = "Requires MP3 Encoder";
			this._saberLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._mp3Link_LinkClicked);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cancelButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cancelButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._cancelButton, "Common.Cancel");
			this._cancelButton.Location = new System.Drawing.Point(479, 237);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 33);
			this._cancelButton.TabIndex = 10;
			this._cancelButton.Text = "&Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
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
			this._logBox.Location = new System.Drawing.Point(30, 286);
			this._logBox.Name = "_logBox";
			this._logBox.ProgressIndicator = null;
			this._logBox.ShowCopyToClipboardMenuItem = false;
			this._logBox.ShowDetailsMenuItem = false;
			this._logBox.ShowDiagnosticsMenuItem = false;
			this._logBox.ShowFontMenuItem = false;
			this._logBox.ShowMenu = true;
			this._logBox.Size = new System.Drawing.Size(524, 227);
			this._logBox.TabIndex = 11;
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.linkLabel1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.linkLabel1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.linkLabel1, "PublishDialog.DestinationLink");
			this.linkLabel1.Location = new System.Drawing.Point(390, 211);
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
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.label1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.label1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.label1, "PublishDialog.Format");
			this.label1.Location = new System.Drawing.Point(18, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.label1.Size = new System.Drawing.Size(234, 20);
			this.label1.TabIndex = 17;
			this.label1.Text = "Verse Index Format";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// _cueSheet
			// 
			this._cueSheet.AutoSize = true;
			this._cueSheet.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cueSheet, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cueSheet, "");
			this.l10NSharpExtender1.SetLocalizationPriority(this._cueSheet, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._cueSheet, "PublishDialog.Megavoice");
			this._cueSheet.Location = new System.Drawing.Point(18, 77);
			this._cueSheet.Name = "_cueSheet";
			this._cueSheet.Size = new System.Drawing.Size(84, 21);
			this._cueSheet.TabIndex = 2;
			this._cueSheet.Text = "Cue Sheet";
			this._cueSheet.UseVisualStyleBackColor = true;
			// 
			// _audacityLabelFile
			// 
			this._audacityLabelFile.AutoSize = true;
			this._audacityLabelFile.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._audacityLabelFile, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._audacityLabelFile, "");
			this.l10NSharpExtender1.SetLocalizationPriority(this._audacityLabelFile, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._audacityLabelFile, "PublishDialog.Megavoice");
			this._audacityLabelFile.Location = new System.Drawing.Point(18, 50);
			this._audacityLabelFile.Name = "_audacityLabelFile";
			this._audacityLabelFile.Size = new System.Drawing.Size(133, 21);
			this._audacityLabelFile.TabIndex = 3;
			this._audacityLabelFile.Text = "Audacity Label File";
			this._audacityLabelFile.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(30, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(521, 186);
			this.tableLayoutPanel1.TabIndex = 16;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this._audiBibleRadio, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this._megavoiceRadio, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this._saberRadio, 0, 3);
			this.tableLayoutPanel2.Controls.Add(this._mp3Radio, 0, 4);
			this.tableLayoutPanel2.Controls.Add(this._oggRadio, 0, 5);
			this.tableLayoutPanel2.Controls.Add(this._flacRadio, 0, 6);
			this.tableLayoutPanel2.Controls.Add(this._mp3Link, 1, 4);
			this.tableLayoutPanel2.Controls.Add(this._saberLink, 1, 3);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 7;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(254, 180);
			this.tableLayoutPanel2.TabIndex = 0;
			this.tableLayoutPanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel2_Paint);
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel3.ColumnCount = 1;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Controls.Add(this._none, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this._audacityLabelFile, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this._cueSheet, 0, 3);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(263, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
			this.tableLayoutPanel3.RowCount = 4;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(255, 180);
			this.tableLayoutPanel3.TabIndex = 1;
			this.tableLayoutPanel3.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel3_Paint);
			// 
			// PublishDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(594, 534);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._openFolderLink);
			this.Controls.Add(this._logBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._destinationLabel);
			this.Controls.Add(this._publishButton);
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
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
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
        private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
        private System.Windows.Forms.RadioButton _audiBibleRadio;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.RadioButton _none;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton _audacityLabelFile;
        private System.Windows.Forms.RadioButton _cueSheet;
    }
}