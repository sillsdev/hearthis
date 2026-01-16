namespace HearThis.UI
{
	partial class AndroidSyncDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AndroidSyncDialog));
            this.qrBox = new System.Windows.Forms.PictureBox();
            this._lblSyncInstructions = new SIL.Windows.Forms.Widgets.BetterLabel();
            this._lblAboutHearThisAndroid = new SIL.Windows.Forms.Widgets.BetterLabel();
            this._playStoreLinkLabel = new SIL.Windows.Forms.Widgets.BetterLinkLabel();
            this._lblAltIP = new System.Windows.Forms.Label();
            this._ipAddressBox = new System.Windows.Forms.TextBox();
            this._syncButton = new System.Windows.Forms.Button();
            this._btnClose = new System.Windows.Forms.Button();
            this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._logBox = new SIL.Windows.Forms.Progress.LogBox();
            ((System.ComponentModel.ISupportInitialize)(this.qrBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // qrBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.qrBox, 3);
            this.l10NSharpExtender1.SetLocalizableToolTip(this.qrBox, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.qrBox, null);
            this.l10NSharpExtender1.SetLocalizingId(this.qrBox, "AndroidSyncDialog.qrBox");
            this.qrBox.Location = new System.Drawing.Point(13, 137);
            this.qrBox.Margin = new System.Windows.Forms.Padding(3, 0, 0, 8);
            this.qrBox.Name = "qrBox";
            this.qrBox.Size = new System.Drawing.Size(151, 126);
            this.qrBox.TabIndex = 1;
            this.qrBox.TabStop = false;
            // 
            // _lblSyncInstructions
            // 
            this._lblSyncInstructions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this._lblSyncInstructions, 3);
            this._lblSyncInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lblSyncInstructions.Enabled = false;
            this._lblSyncInstructions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblSyncInstructions.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblSyncInstructions.IsTextSelectable = false;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblSyncInstructions, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblSyncInstructions, "Param is \"HearThis\" (Android app name); Probably want to keep \"Sync With Desktop " +
        "App\" in English until HTA is localized into the target language.");
            this.l10NSharpExtender1.SetLocalizingId(this._lblSyncInstructions, "AndroidSyncDialog._lblSyncInstructions");
            this._lblSyncInstructions.Location = new System.Drawing.Point(10, 69);
            this._lblSyncInstructions.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this._lblSyncInstructions.Multiline = true;
            this._lblSyncInstructions.Name = "_lblSyncInstructions";
            this._lblSyncInstructions.ReadOnly = true;
            this._lblSyncInstructions.Size = new System.Drawing.Size(386, 60);
            this._lblSyncInstructions.TabIndex = 3;
            this._lblSyncInstructions.TabStop = false;
            this._lblSyncInstructions.Text = resources.GetString("_lblSyncInstructions.Text");
            // 
            // _lblAboutHearThisAndroid
            // 
            this._lblAboutHearThisAndroid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this._lblAboutHearThisAndroid, 3);
            this._lblAboutHearThisAndroid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lblAboutHearThisAndroid.Enabled = false;
            this._lblAboutHearThisAndroid.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblAboutHearThisAndroid.IsTextSelectable = false;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblAboutHearThisAndroid, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblAboutHearThisAndroid, "Param 0: \"HearThis\" (Android app name); Param 1: \"HearThis\" (desktop product name" +
        ")");
            this.l10NSharpExtender1.SetLocalizingId(this._lblAboutHearThisAndroid, "AndroidSyncDialog._lblAboutHearThisAndroid");
            this._lblAboutHearThisAndroid.Location = new System.Drawing.Point(10, 12);
            this._lblAboutHearThisAndroid.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this._lblAboutHearThisAndroid.Multiline = true;
            this._lblAboutHearThisAndroid.Name = "_lblAboutHearThisAndroid";
            this._lblAboutHearThisAndroid.ReadOnly = true;
            this._lblAboutHearThisAndroid.Size = new System.Drawing.Size(386, 26);
            this._lblAboutHearThisAndroid.TabIndex = 1;
            this._lblAboutHearThisAndroid.TabStop = false;
            this._lblAboutHearThisAndroid.Text = "{0} for Android can synchronize with {1} desktop, allowing Android devices to be " +
    "used for recording. Follow this link to install it:";
            // 
            // _playStoreLinkLabel
            // 
            this._playStoreLinkLabel.BackColor = System.Drawing.SystemColors.Control;
            this._playStoreLinkLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this._playStoreLinkLabel, 3);
            this._playStoreLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._playStoreLinkLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this._playStoreLinkLabel.ForeColor = System.Drawing.Color.Blue;
            this._playStoreLinkLabel.IsTextSelectable = false;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._playStoreLinkLabel, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._playStoreLinkLabel, "Param is \"HearThis\" (Android app name)");
            this.l10NSharpExtender1.SetLocalizingId(this._playStoreLinkLabel, "AndroidSyncDialog._playStoreLinkLabel");
            this._playStoreLinkLabel.Location = new System.Drawing.Point(13, 46);
            this._playStoreLinkLabel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 8);
            this._playStoreLinkLabel.Multiline = true;
            this._playStoreLinkLabel.Name = "_playStoreLinkLabel";
            this._playStoreLinkLabel.Size = new System.Drawing.Size(383, 15);
            this._playStoreLinkLabel.TabIndex = 2;
            this._playStoreLinkLabel.TabStop = false;
            this._playStoreLinkLabel.Text = "Get {0} for Android from Google Play";
            this._playStoreLinkLabel.URL = "https://play.google.com/store/apps/details?id=org.sil.hearthis";
            // 
            // _lblAltIP
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._lblAltIP, 3);
            this._lblAltIP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblAltIP, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblAltIP, "Param 0: \"HearThis\" (Android app name); Param 1: name of button (AndroidSyncDialo" +
        "g._syncButton)");
            this.l10NSharpExtender1.SetLocalizingId(this._lblAltIP, "AndroidSyncDialog._lblAltIP");
            this._lblAltIP.Location = new System.Drawing.Point(10, 277);
            this._lblAltIP.Margin = new System.Windows.Forms.Padding(0, 6, 0, 8);
            this._lblAltIP.Name = "_lblAltIP";
            this._lblAltIP.Size = new System.Drawing.Size(386, 32);
            this._lblAltIP.TabIndex = 4;
            this._lblAltIP.Text = "Alternatively, you can enter the code from the Synchronize screen in {0} for Andr" +
    "oid here and then click the \"{1}\" button below.";
            // 
            // _ipAddressBox
            // 
            this._ipAddressBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._ipAddressBox, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._ipAddressBox, null);
            this.l10NSharpExtender1.SetLocalizingId(this._ipAddressBox, "AndroidSyncDialog._ipAddressBox");
            this._ipAddressBox.Location = new System.Drawing.Point(13, 318);
            this._ipAddressBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this._ipAddressBox.Name = "_ipAddressBox";
            this._ipAddressBox.Size = new System.Drawing.Size(100, 20);
            this._ipAddressBox.TabIndex = 5;
            this._ipAddressBox.Enter += new System.EventHandler(this._ipAddressBox_Enter);
            this._ipAddressBox.Leave += new System.EventHandler(this._ipAddressBox_Leave);
            // 
            // _syncButton
            // 
            this._syncButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._syncButton, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._syncButton, null);
            this.l10NSharpExtender1.SetLocalizingId(this._syncButton, "AndroidSyncDialog._syncButton");
            this._syncButton.Location = new System.Drawing.Point(119, 317);
            this._syncButton.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            this._syncButton.Name = "_syncButton";
            this._syncButton.Size = new System.Drawing.Size(75, 23);
            this._syncButton.TabIndex = 6;
            this._syncButton.Text = "Synchronize";
            this._syncButton.Click += new System.EventHandler(this._syncButton_Click);
            // 
            // _btnClose
            // 
            this._btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._btnClose, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._btnClose, null);
            this.l10NSharpExtender1.SetLocalizingId(this._btnClose, "AndroidSyncDialog._btnClose");
            this._btnClose.Location = new System.Drawing.Point(318, 414);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(75, 23);
            this._btnClose.TabIndex = 0;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = true;
            // 
            // l10NSharpExtender1
            // 
            this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
            this.l10NSharpExtender1.PrefixForNewItems = null;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._lblAboutHearThisAndroid, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._btnClose, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this._playStoreLinkLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._syncButton, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this._lblSyncInstructions, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._ipAddressBox, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.qrBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._lblAltIP, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._logBox, 0, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(10, 12, 10, 8);
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(406, 448);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _logBox
            // 
            this._logBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._logBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
            this._logBox.CancelRequested = false;
            this.tableLayoutPanel1.SetColumnSpan(this._logBox, 3);
            this._logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logBox.ErrorEncountered = false;
            this._logBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._logBox.GetDiagnosticsMethod = null;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._logBox, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._logBox, null);
            this.l10NSharpExtender1.SetLocalizingId(this._logBox, "LogBox");
            this._logBox.Location = new System.Drawing.Point(13, 346);
            this._logBox.MaxLength = 715827882;
            this._logBox.MaxLengthErrorMessage = "Maximum length exceeded!";
            this._logBox.Name = "_logBox";
            this._logBox.ProgressIndicator = null;
            this._logBox.ShowCopyToClipboardMenuItem = false;
            this._logBox.ShowDetailsMenuItem = false;
            this._logBox.ShowDiagnosticsMenuItem = false;
            this._logBox.ShowFontMenuItem = false;
            this._logBox.ShowMenu = true;
            this._logBox.Size = new System.Drawing.Size(380, 62);
            this._logBox.TabIndex = 7;
            this._logBox.Visible = false;
            // 
            // AndroidSyncDialog
            // 
            this.AcceptButton = this._btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnClose;
            this.ClientSize = new System.Drawing.Size(406, 448);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
            this.l10NSharpExtender1.SetLocalizationComment(this, "Param is \"HearThis\" (Android app name)");
            this.l10NSharpExtender1.SetLocalizingId(this, "AndroidSyncDialog.WindowTitle");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(422, 450);
            this.Name = "AndroidSyncDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Synchronize with {0} for Android";
            ((System.ComponentModel.ISupportInitialize)(this.qrBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.PictureBox qrBox;
		private SIL.Windows.Forms.Widgets.BetterLinkLabel _playStoreLinkLabel;
		private SIL.Windows.Forms.Widgets.BetterLabel _lblSyncInstructions;
		private SIL.Windows.Forms.Widgets.BetterLabel _lblAboutHearThisAndroid;
		private System.Windows.Forms.Label _lblAltIP;
		private System.Windows.Forms.TextBox _ipAddressBox;
		private System.Windows.Forms.Button _syncButton;
		private System.Windows.Forms.Button _btnClose;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private SIL.Windows.Forms.Progress.LogBox _logBox;
	}
}
