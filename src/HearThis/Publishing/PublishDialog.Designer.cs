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
			System.Windows.Forms.Label label4;
			this._saberRadio = new System.Windows.Forms.RadioButton();
			this._megaVoiceRadio = new System.Windows.Forms.RadioButton();
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
			this._logBox = new SIL.Windows.Forms.Progress.LogBox();
			this._changeDestinationLink = new System.Windows.Forms.LinkLabel();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this._cueSheet = new System.Windows.Forms.RadioButton();
			this._audacityLabelFile = new System.Windows.Forms.RadioButton();
			this._lblBooksToPublish = new System.Windows.Forms.Label();
			this._rdoAllBooks = new System.Windows.Forms.RadioButton();
			this._rdoCurrentBook = new System.Windows.Forms.RadioButton();
			this._scrAppBuilderRadio = new System.Windows.Forms.RadioButton();
			this._includePhraseLevelLabels = new System.Windows.Forms.CheckBox();
			this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanelAudioFormat = new System.Windows.Forms.TableLayoutPanel();
			this._tableLayoutRight = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanelVerseIndexFormat = new System.Windows.Forms.TableLayoutPanel();
			this._tableLayoutPanelBooksToPublish = new System.Windows.Forms.TableLayoutPanel();
			label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.tableLayoutPanelMain.SuspendLayout();
			this.tableLayoutPanelAudioFormat.SuspendLayout();
			this._tableLayoutRight.SuspendLayout();
			this.tableLayoutPanelVerseIndexFormat.SuspendLayout();
			this._tableLayoutPanelBooksToPublish.SuspendLayout();
			this.SuspendLayout();
			// 
			// label4
			// 
			label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.l10NSharpExtender1.SetLocalizableToolTip(label4, null);
			this.l10NSharpExtender1.SetLocalizationComment(label4, null);
			this.l10NSharpExtender1.SetLocalizationPriority(label4, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(label4, "PublishDialog.label4");
			label4.Location = new System.Drawing.Point(30, 237);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(610, 2);
			label4.TabIndex = 17;
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
			// 
			// _megaVoiceRadio
			// 
			this._megaVoiceRadio.AutoSize = true;
			this._megaVoiceRadio.Enabled = false;
			this._megaVoiceRadio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._megaVoiceRadio, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._megaVoiceRadio, "");
			this.l10NSharpExtender1.SetLocalizationPriority(this._megaVoiceRadio, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._megaVoiceRadio, "PublishDialog.Megavoice");
			this._megaVoiceRadio.Location = new System.Drawing.Point(3, 50);
			this._megaVoiceRadio.Name = "_megaVoiceRadio";
			this._megaVoiceRadio.Size = new System.Drawing.Size(92, 21);
			this._megaVoiceRadio.TabIndex = 1;
			this._megaVoiceRadio.Text = "MegaVoice";
			this.toolTip1.SetToolTip(this._megaVoiceRadio, "http://www.megavoice.com/");
			this._megaVoiceRadio.UseVisualStyleBackColor = true;
			// 
			// _mp3Radio
			// 
			this._mp3Radio.AutoSize = true;
			this._mp3Radio.Enabled = false;
			this._mp3Radio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._mp3Radio, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._mp3Radio, null);
			this.l10NSharpExtender1.SetLocalizingId(this._mp3Radio, "PublishDialog.Mp3");
			this._mp3Radio.Location = new System.Drawing.Point(3, 131);
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
			this._oggRadio.Location = new System.Drawing.Point(3, 158);
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
			this.l10NSharpExtender1.SetLocalizingId(this._publishButton, "PublishDialog.ExportButton");
			this._publishButton.Location = new System.Drawing.Point(479, 278);
			this._publishButton.Name = "_publishButton";
			this._publishButton.Size = new System.Drawing.Size(80, 33);
			this._publishButton.TabIndex = 9;
			this._publishButton.Text = "&Export";
			this._publishButton.UseVisualStyleBackColor = true;
			this._publishButton.Click += new System.EventHandler(this._publishButton_Click);
			// 
			// _destinationLabel
			// 
			this._destinationLabel.AutoEllipsis = true;
			this._destinationLabel.AutoSize = true;
			this._destinationLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._destinationLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._destinationLabel, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._destinationLabel, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._destinationLabel, "PublishDialog.DestinationPath");
			this._destinationLabel.Location = new System.Drawing.Point(27, 278);
			this._destinationLabel.MaximumSize = new System.Drawing.Size(386, 17);
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
			this.label2.Location = new System.Drawing.Point(27, 252);
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
			this.label3.Location = new System.Drawing.Point(3, 0);
			this.label3.Name = "label3";
			this.label3.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.label3.Size = new System.Drawing.Size(93, 20);
			this.label3.TabIndex = 11;
			this.label3.Text = "Audio Format";
			// 
			// _openFolderLink
			// 
			this._openFolderLink.AutoEllipsis = true;
			this._openFolderLink.AutoSize = true;
			this._openFolderLink.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._openFolderLink, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._openFolderLink, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._openFolderLink, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._openFolderLink, "PublishDialog.OpenFolderLink");
			this._openFolderLink.Location = new System.Drawing.Point(27, 278);
			this._openFolderLink.MaximumSize = new System.Drawing.Size(386, 17);
			this._openFolderLink.Name = "_openFolderLink";
			this._openFolderLink.Size = new System.Drawing.Size(189, 17);
			this._openFolderLink.TabIndex = 8;
			this._openFolderLink.TabStop = true;
			this._openFolderLink.Text = "Open folder of exported audio";
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
			this._flacRadio.Location = new System.Drawing.Point(3, 185);
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
			this.l10NSharpExtender1.SetLocalizingId(this._none, "PublishDialog.PublishDialog._none");
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
			this._mp3Link.Location = new System.Drawing.Point(159, 128);
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
			this._saberLink.Location = new System.Drawing.Point(159, 74);
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
			this._cancelButton.Location = new System.Drawing.Point(565, 278);
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
			this._logBox.Location = new System.Drawing.Point(30, 328);
			this._logBox.Name = "_logBox";
			this._logBox.ProgressIndicator = null;
			this._logBox.ShowCopyToClipboardMenuItem = false;
			this._logBox.ShowDetailsMenuItem = false;
			this._logBox.ShowDiagnosticsMenuItem = false;
			this._logBox.ShowFontMenuItem = false;
			this._logBox.ShowMenu = true;
			this._logBox.Size = new System.Drawing.Size(610, 161);
			this._logBox.TabIndex = 11;
			// 
			// _changeDestinationLink
			// 
			this._changeDestinationLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._changeDestinationLink.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._changeDestinationLink, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._changeDestinationLink, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._changeDestinationLink, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._changeDestinationLink, "PublishDialog.PublishDialog._changeDestinationLink");
			this._changeDestinationLink.Location = new System.Drawing.Point(476, 252);
			this._changeDestinationLink.Name = "_changeDestinationLink";
			this._changeDestinationLink.Size = new System.Drawing.Size(109, 13);
			this._changeDestinationLink.TabIndex = 7;
			this._changeDestinationLink.TabStop = true;
			this._changeDestinationLink.Text = "Change Destination...";
			this._changeDestinationLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._changeDestinationLink_LinkClicked);
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
			this.label1.Size = new System.Drawing.Size(269, 20);
			this.label1.TabIndex = 17;
			this.label1.Text = "Verse Index Format";
			// 
			// _cueSheet
			// 
			this._cueSheet.AutoSize = true;
			this._cueSheet.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cueSheet, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cueSheet, "");
			this.l10NSharpExtender1.SetLocalizationPriority(this._cueSheet, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._cueSheet, "PublishDialog.PublishDialog._cueSheet");
			this._cueSheet.Location = new System.Drawing.Point(18, 104);
			this._cueSheet.Name = "_cueSheet";
			this._cueSheet.Size = new System.Drawing.Size(84, 21);
			this._cueSheet.TabIndex = 2;
			this._cueSheet.Text = "Cue Sheet";
			this._cueSheet.UseVisualStyleBackColor = true;
			this._cueSheet.Visible = false;
			// 
			// _audacityLabelFile
			// 
			this._audacityLabelFile.AutoSize = true;
			this._audacityLabelFile.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._audacityLabelFile, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._audacityLabelFile, "");
			this.l10NSharpExtender1.SetLocalizationPriority(this._audacityLabelFile, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._audacityLabelFile, "PublishDialog.PublishDialog._audacityLabelFile");
			this._audacityLabelFile.Location = new System.Drawing.Point(18, 50);
			this._audacityLabelFile.Name = "_audacityLabelFile";
			this._audacityLabelFile.Size = new System.Drawing.Size(269, 21);
			this._audacityLabelFile.TabIndex = 3;
			this._audacityLabelFile.Text = "Audacity Label File (Scripture App Builder)";
			this._audacityLabelFile.UseVisualStyleBackColor = true;
			this._audacityLabelFile.CheckedChanged += new System.EventHandler(this._audacityLabelFile_CheckedChanged);
			// 
			// _lblBooksToPublish
			// 
			this._lblBooksToPublish.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lblBooksToPublish.AutoSize = true;
			this._lblBooksToPublish.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblBooksToPublish, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblBooksToPublish, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._lblBooksToPublish, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._lblBooksToPublish, "PublishDialog.PublishDialog._lblBooksToPublish");
			this._lblBooksToPublish.Location = new System.Drawing.Point(18, 0);
			this._lblBooksToPublish.Name = "_lblBooksToPublish";
			this._lblBooksToPublish.Padding = new System.Windows.Forms.Padding(0, 10, 0, 3);
			this._lblBooksToPublish.Size = new System.Drawing.Size(269, 30);
			this._lblBooksToPublish.TabIndex = 19;
			this._lblBooksToPublish.Text = "Books to Export";
			// 
			// _rdoAllBooks
			// 
			this._rdoAllBooks.AutoSize = true;
			this._rdoAllBooks.Checked = true;
			this._rdoAllBooks.Font = new System.Drawing.Font("Segoe UI", 9.75F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._rdoAllBooks, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._rdoAllBooks, null);
			this.l10NSharpExtender1.SetLocalizingId(this._rdoAllBooks, "PublishDialog.PublishDialog._rdoAllBooks");
			this._rdoAllBooks.Location = new System.Drawing.Point(18, 33);
			this._rdoAllBooks.Name = "_rdoAllBooks";
			this._rdoAllBooks.Size = new System.Drawing.Size(139, 21);
			this._rdoAllBooks.TabIndex = 20;
			this._rdoAllBooks.TabStop = true;
			this._rdoAllBooks.Text = "All books in project";
			this._rdoAllBooks.UseVisualStyleBackColor = true;
			// 
			// _rdoCurrentBook
			// 
			this._rdoCurrentBook.AutoSize = true;
			this._rdoCurrentBook.Font = new System.Drawing.Font("Segoe UI", 9.75F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._rdoCurrentBook, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._rdoCurrentBook, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._rdoCurrentBook, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._rdoCurrentBook, "PublishDialog.PublishDialog._rdoCurrentBook");
			this._rdoCurrentBook.Location = new System.Drawing.Point(18, 60);
			this._rdoCurrentBook.Name = "_rdoCurrentBook";
			this._rdoCurrentBook.Size = new System.Drawing.Size(124, 21);
			this._rdoCurrentBook.TabIndex = 21;
			this._rdoCurrentBook.TabStop = true;
			this._rdoCurrentBook.Text = "Current Book: {0}";
			this._rdoCurrentBook.UseVisualStyleBackColor = true;
			// 
			// _scrAppBuilderRadio
			// 
			this._scrAppBuilderRadio.AutoSize = true;
			this._scrAppBuilderRadio.Font = new System.Drawing.Font("Segoe UI", 9.75F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._scrAppBuilderRadio, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._scrAppBuilderRadio, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._scrAppBuilderRadio, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._scrAppBuilderRadio, "PublishDialog.PublishDialog._scrAppBuilderRadio");
			this._scrAppBuilderRadio.Location = new System.Drawing.Point(3, 104);
			this._scrAppBuilderRadio.Name = "_scrAppBuilderRadio";
			this._scrAppBuilderRadio.Size = new System.Drawing.Size(150, 21);
			this._scrAppBuilderRadio.TabIndex = 16;
			this._scrAppBuilderRadio.TabStop = true;
			this._scrAppBuilderRadio.Text = "Scripture App Builder";
			this._scrAppBuilderRadio.UseVisualStyleBackColor = true;
			this._scrAppBuilderRadio.CheckedChanged += new System.EventHandler(this._scrAppBuilderRadio_CheckedChanged);
			// 
			// _includePhraseLevelLabels
			// 
			this._includePhraseLevelLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._includePhraseLevelLabels.AutoSize = true;
			this._includePhraseLevelLabels.Enabled = false;
			this._includePhraseLevelLabels.Font = new System.Drawing.Font("Segoe UI", 9.75F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._includePhraseLevelLabels, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._includePhraseLevelLabels, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._includePhraseLevelLabels, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._includePhraseLevelLabels, "PublishDialog.PublishDialog._chkPhraseLevelLabels");
			this._includePhraseLevelLabels.Location = new System.Drawing.Point(55, 77);
			this._includePhraseLevelLabels.Name = "_includePhraseLevelLabels";
			this._includePhraseLevelLabels.Size = new System.Drawing.Size(232, 21);
			this._includePhraseLevelLabels.TabIndex = 18;
			this._includePhraseLevelLabels.Text = "Include labels for phrase-level clips";
			this._includePhraseLevelLabels.UseVisualStyleBackColor = true;
			this._includePhraseLevelLabels.CheckedChanged += new System.EventHandler(this._includePhraseLevelLabels_CheckedChanged);
			// 
			// tableLayoutPanelMain
			// 
			this.tableLayoutPanelMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanelMain.ColumnCount = 3;
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelAudioFormat, 0, 0);
			this.tableLayoutPanelMain.Controls.Add(this._tableLayoutRight, 2, 0);
			this.tableLayoutPanelMain.Location = new System.Drawing.Point(30, 12);
			this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
			this.tableLayoutPanelMain.RowCount = 1;
			this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanelMain.Size = new System.Drawing.Size(610, 215);
			this.tableLayoutPanelMain.TabIndex = 16;
			// 
			// tableLayoutPanelAudioFormat
			// 
			this.tableLayoutPanelAudioFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanelAudioFormat.AutoSize = true;
			this.tableLayoutPanelAudioFormat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelAudioFormat.ColumnCount = 2;
			this.tableLayoutPanelAudioFormat.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelAudioFormat.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelAudioFormat.Controls.Add(this.label3, 0, 0);
			this.tableLayoutPanelAudioFormat.Controls.Add(this._audiBibleRadio, 0, 1);
			this.tableLayoutPanelAudioFormat.Controls.Add(this._megaVoiceRadio, 0, 2);
			this.tableLayoutPanelAudioFormat.Controls.Add(this._saberRadio, 0, 3);
			this.tableLayoutPanelAudioFormat.Controls.Add(this._mp3Radio, 0, 5);
			this.tableLayoutPanelAudioFormat.Controls.Add(this._oggRadio, 0, 6);
			this.tableLayoutPanelAudioFormat.Controls.Add(this._flacRadio, 0, 7);
			this.tableLayoutPanelAudioFormat.Controls.Add(this._mp3Link, 1, 5);
			this.tableLayoutPanelAudioFormat.Controls.Add(this._saberLink, 1, 3);
			this.tableLayoutPanelAudioFormat.Controls.Add(this._scrAppBuilderRadio, 0, 4);
			this.tableLayoutPanelAudioFormat.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanelAudioFormat.Name = "tableLayoutPanelAudioFormat";
			this.tableLayoutPanelAudioFormat.RowCount = 8;
			this.tableLayoutPanelAudioFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelAudioFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelAudioFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelAudioFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelAudioFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelAudioFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelAudioFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelAudioFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelAudioFormat.Size = new System.Drawing.Size(279, 209);
			this.tableLayoutPanelAudioFormat.TabIndex = 0;
			// 
			// _tableLayoutRight
			// 
			this._tableLayoutRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutRight.AutoSize = true;
			this._tableLayoutRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableLayoutRight.ColumnCount = 1;
			this._tableLayoutRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutRight.Controls.Add(this.tableLayoutPanelVerseIndexFormat, 0, 0);
			this._tableLayoutRight.Controls.Add(this._tableLayoutPanelBooksToPublish, 0, 2);
			this._tableLayoutRight.Location = new System.Drawing.Point(311, 3);
			this._tableLayoutRight.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._tableLayoutRight.Name = "_tableLayoutRight";
			this._tableLayoutRight.RowCount = 3;
			this._tableLayoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutRight.Size = new System.Drawing.Size(296, 212);
			this._tableLayoutRight.TabIndex = 2;
			// 
			// tableLayoutPanelVerseIndexFormat
			// 
			this.tableLayoutPanelVerseIndexFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanelVerseIndexFormat.AutoSize = true;
			this.tableLayoutPanelVerseIndexFormat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelVerseIndexFormat.ColumnCount = 1;
			this.tableLayoutPanelVerseIndexFormat.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanelVerseIndexFormat.Controls.Add(this._none, 0, 1);
			this.tableLayoutPanelVerseIndexFormat.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanelVerseIndexFormat.Controls.Add(this._audacityLabelFile, 0, 2);
			this.tableLayoutPanelVerseIndexFormat.Controls.Add(this._cueSheet, 0, 4);
			this.tableLayoutPanelVerseIndexFormat.Controls.Add(this._includePhraseLevelLabels, 0, 3);
			this.tableLayoutPanelVerseIndexFormat.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanelVerseIndexFormat.Name = "tableLayoutPanelVerseIndexFormat";
			this.tableLayoutPanelVerseIndexFormat.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
			this.tableLayoutPanelVerseIndexFormat.RowCount = 5;
			this.tableLayoutPanelVerseIndexFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelVerseIndexFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelVerseIndexFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelVerseIndexFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelVerseIndexFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelVerseIndexFormat.Size = new System.Drawing.Size(290, 128);
			this.tableLayoutPanelVerseIndexFormat.TabIndex = 1;
			// 
			// _tableLayoutPanelBooksToPublish
			// 
			this._tableLayoutPanelBooksToPublish.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutPanelBooksToPublish.AutoSize = true;
			this._tableLayoutPanelBooksToPublish.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableLayoutPanelBooksToPublish.ColumnCount = 1;
			this._tableLayoutPanelBooksToPublish.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanelBooksToPublish.Controls.Add(this._rdoCurrentBook, 0, 2);
			this._tableLayoutPanelBooksToPublish.Controls.Add(this._lblBooksToPublish, 0, 0);
			this._tableLayoutPanelBooksToPublish.Controls.Add(this._rdoAllBooks, 0, 1);
			this._tableLayoutPanelBooksToPublish.Location = new System.Drawing.Point(3, 125);
			this._tableLayoutPanelBooksToPublish.Name = "_tableLayoutPanelBooksToPublish";
			this._tableLayoutPanelBooksToPublish.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
			this._tableLayoutPanelBooksToPublish.RowCount = 3;
			this._tableLayoutPanelBooksToPublish.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelBooksToPublish.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelBooksToPublish.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanelBooksToPublish.Size = new System.Drawing.Size(290, 84);
			this._tableLayoutPanelBooksToPublish.TabIndex = 2;
			// 
			// PublishDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(670, 510);
			this.Controls.Add(label4);
			this.Controls.Add(this.tableLayoutPanelMain);
			this.Controls.Add(this._changeDestinationLink);
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
			this.MinimumSize = new System.Drawing.Size(550, 500);
			this.Name = "PublishDialog";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Export Sound Files";
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.tableLayoutPanelMain.ResumeLayout(false);
			this.tableLayoutPanelMain.PerformLayout();
			this.tableLayoutPanelAudioFormat.ResumeLayout(false);
			this.tableLayoutPanelAudioFormat.PerformLayout();
			this._tableLayoutRight.ResumeLayout(false);
			this._tableLayoutRight.PerformLayout();
			this.tableLayoutPanelVerseIndexFormat.ResumeLayout(false);
			this.tableLayoutPanelVerseIndexFormat.PerformLayout();
			this._tableLayoutPanelBooksToPublish.ResumeLayout(false);
			this._tableLayoutPanelBooksToPublish.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton _saberRadio;
        private System.Windows.Forms.RadioButton _megaVoiceRadio;
        private System.Windows.Forms.RadioButton _mp3Radio;
        private System.Windows.Forms.RadioButton _oggRadio;
        private System.Windows.Forms.Button _publishButton;
        private System.Windows.Forms.Label _destinationLabel;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private SIL.Windows.Forms.Progress.LogBox _logBox;
        private System.Windows.Forms.LinkLabel _openFolderLink;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel _mp3Link;
        private System.Windows.Forms.LinkLabel _saberLink;
        private System.Windows.Forms.RadioButton _flacRadio;
        private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.LinkLabel _changeDestinationLink;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
        private System.Windows.Forms.RadioButton _audiBibleRadio;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelAudioFormat;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelVerseIndexFormat;
        private System.Windows.Forms.RadioButton _none;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton _audacityLabelFile;
        private System.Windows.Forms.RadioButton _cueSheet;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutRight;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelBooksToPublish;
		private System.Windows.Forms.RadioButton _rdoCurrentBook;
		private System.Windows.Forms.Label _lblBooksToPublish;
		private System.Windows.Forms.RadioButton _rdoAllBooks;
		private System.Windows.Forms.RadioButton _scrAppBuilderRadio;
		private System.Windows.Forms.CheckBox _includePhraseLevelLabels;
    }
}