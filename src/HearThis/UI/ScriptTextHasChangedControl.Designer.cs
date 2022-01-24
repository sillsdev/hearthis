using System;
using System.Windows.Forms;

namespace HearThis.UI
{
    partial class ScriptTextHasChangedControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.PictureBox iconShiftClips;
			this._txtThen = new System.Windows.Forms.RichTextBox();
			this._masterTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._lblProblemSummary = new System.Windows.Forms.Button();
			this._lblNow = new System.Windows.Forms.Label();
			this._panelThen = new System.Windows.Forms.FlowLayoutPanel();
			this._lblThen = new System.Windows.Forms.Label();
			this._lblRecordedDate = new System.Windows.Forms.Label();
			this._txtNow = new System.Windows.Forms.RichTextBox();
			this._nextButton = new HearThis.UI.ArrowButton();
			this._pnlPlayClip = new System.Windows.Forms.FlowLayoutPanel();
			this._audioButtonsControl = new HearThis.UI.AudioButtonsControl();
			this._btnPlayClip = new System.Windows.Forms.Button();
			this._tableOptions = new System.Windows.Forms.TableLayoutPanel();
			this._btnUseExisting = new System.Windows.Forms.Button();
			this._rdoUseExisting = new System.Windows.Forms.RadioButton();
			this._rdoAskLater = new System.Windows.Forms.RadioButton();
			this._btnAskLater = new System.Windows.Forms.Button();
			this._rdoReRecord = new System.Windows.Forms.RadioButton();
			this._btnDelete = new System.Windows.Forms.Button();
			this._flowNearbyText = new System.Windows.Forms.FlowLayoutPanel();
			this._lblShiftClips = new System.Windows.Forms.Label();
			this._btnShiftClips = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			iconShiftClips = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(iconShiftClips)).BeginInit();
			this._masterTableLayoutPanel.SuspendLayout();
			this._panelThen.SuspendLayout();
			this._pnlPlayClip.SuspendLayout();
			this._tableOptions.SuspendLayout();
			this._flowNearbyText.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// iconShiftClips
			// 
			iconShiftClips.Image = global::HearThis.Properties.Resources.shift_clips24;
			this.l10NSharpExtender1.SetLocalizableToolTip(iconShiftClips, null);
			this.l10NSharpExtender1.SetLocalizationComment(iconShiftClips, null);
			this.l10NSharpExtender1.SetLocalizingId(iconShiftClips, "pictureBox1");
			iconShiftClips.Location = new System.Drawing.Point(3, 3);
			iconShiftClips.Name = "iconShiftClips";
			iconShiftClips.Size = new System.Drawing.Size(24, 20);
			iconShiftClips.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			iconShiftClips.TabIndex = 0;
			iconShiftClips.TabStop = false;
			// 
			// _txtThen
			// 
			this._txtThen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtThen.BackColor = System.Drawing.SystemColors.Window;
			this._txtThen.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._txtThen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this._txtThen.Location = new System.Drawing.Point(16, 95);
			this._txtThen.Margin = new System.Windows.Forms.Padding(6);
			this._txtThen.Name = "_txtThen";
			this._txtThen.ReadOnly = true;
			this._txtThen.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this._txtThen.Size = new System.Drawing.Size(316, 64);
			this._txtThen.TabIndex = 6;
			this._txtThen.Text = "";
			// 
			// _masterTableLayoutPanel
			// 
			this._masterTableLayoutPanel.ColumnCount = 2;
			this._masterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._masterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._masterTableLayoutPanel.Controls.Add(this._lblProblemSummary, 0, 0);
			this._masterTableLayoutPanel.Controls.Add(this._lblNow, 1, 2);
			this._masterTableLayoutPanel.Controls.Add(this._panelThen, 0, 2);
			this._masterTableLayoutPanel.Controls.Add(this._txtThen, 0, 3);
			this._masterTableLayoutPanel.Controls.Add(this._txtNow, 1, 3);
			this._masterTableLayoutPanel.Controls.Add(this._nextButton, 1, 6);
			this._masterTableLayoutPanel.Controls.Add(this._pnlPlayClip, 0, 5);
			this._masterTableLayoutPanel.Controls.Add(this._tableOptions, 1, 5);
			this._masterTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._masterTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._masterTableLayoutPanel.Name = "_masterTableLayoutPanel";
			this._masterTableLayoutPanel.Padding = new System.Windows.Forms.Padding(10);
			this._masterTableLayoutPanel.RowCount = 7;
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._masterTableLayoutPanel.Size = new System.Drawing.Size(677, 429);
			this._masterTableLayoutPanel.TabIndex = 0;
			this._masterTableLayoutPanel.Paint += new System.Windows.Forms.PaintEventHandler(this._masterTableLayoutPanel_Paint);
			this._masterTableLayoutPanel.Resize += new System.EventHandler(this._masterTableLayoutPanel_Resize);
			// 
			// _lblProblemSummary
			// 
			this._lblProblemSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._masterTableLayoutPanel.SetColumnSpan(this._lblProblemSummary, 2);
			this._lblProblemSummary.FlatAppearance.BorderSize = 0;
			this._lblProblemSummary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._lblProblemSummary.Font = new System.Drawing.Font("Segoe UI", 12F);
			this._lblProblemSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(212)))), ((int)(((byte)(17)))));
			this._lblProblemSummary.Image = global::HearThis.Properties.Resources.AlertCircle;
			this._lblProblemSummary.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblProblemSummary, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblProblemSummary, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._lblProblemSummary, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._lblProblemSummary, "ScriptTextHasChangedControl._lblProblemSummary");
			this._lblProblemSummary.Location = new System.Drawing.Point(13, 13);
			this._lblProblemSummary.Name = "_lblProblemSummary";
			this._lblProblemSummary.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this._lblProblemSummary.Size = new System.Drawing.Size(651, 40);
			this._lblProblemSummary.TabIndex = 22;
			this._lblProblemSummary.Text = "The text has changed since it was recorded.";
			this._lblProblemSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._lblProblemSummary.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._lblProblemSummary.UseVisualStyleBackColor = true;
			this._lblProblemSummary.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintRoundedBorder);
			// 
			// _lblNow
			// 
			this._lblNow.AutoSize = true;
			this._lblNow.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblNow, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblNow, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblNow, "ScriptTextHasChangedControl._lblNow");
			this._lblNow.Location = new System.Drawing.Point(341, 66);
			this._lblNow.Name = "_lblNow";
			this._lblNow.Size = new System.Drawing.Size(46, 23);
			this._lblNow.TabIndex = 4;
			this._lblNow.Text = "Now";
			// 
			// _panelThen
			// 
			this._panelThen.AutoSize = true;
			this._panelThen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelThen.Controls.Add(this._lblThen);
			this._panelThen.Controls.Add(this._lblRecordedDate);
			this._panelThen.Location = new System.Drawing.Point(10, 66);
			this._panelThen.Margin = new System.Windows.Forms.Padding(0);
			this._panelThen.Name = "_panelThen";
			this._panelThen.Size = new System.Drawing.Size(115, 23);
			this._panelThen.TabIndex = 19;
			// 
			// _lblThen
			// 
			this._lblThen.AutoSize = true;
			this._lblThen.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblThen, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblThen, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblThen, "ScriptTextHasChangedControl._lblThen");
			this._lblThen.Location = new System.Drawing.Point(3, 0);
			this._lblThen.Name = "_lblThen";
			this._lblThen.Size = new System.Drawing.Size(60, 23);
			this._lblThen.TabIndex = 2;
			this._lblThen.Text = "Before";
			// 
			// _lblRecordedDate
			// 
			this._lblRecordedDate.AutoSize = true;
			this._lblRecordedDate.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblRecordedDate, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblRecordedDate, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblRecordedDate, "ScriptTextHasChangedControl._lblRecordedDate");
			this._lblRecordedDate.Location = new System.Drawing.Point(69, 0);
			this._lblRecordedDate.Name = "_lblRecordedDate";
			this._lblRecordedDate.Size = new System.Drawing.Size(43, 23);
			this._lblRecordedDate.TabIndex = 3;
			this._lblRecordedDate.Text = "({0})";
			// 
			// _txtNow
			// 
			this._txtNow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtNow.BackColor = System.Drawing.SystemColors.Window;
			this._txtNow.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._txtNow.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this._txtNow.Location = new System.Drawing.Point(344, 95);
			this._txtNow.Margin = new System.Windows.Forms.Padding(6, 6, 3, 6);
			this._txtNow.Name = "_txtNow";
			this._txtNow.ReadOnly = true;
			this._txtNow.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this._txtNow.Size = new System.Drawing.Size(320, 64);
			this._txtNow.TabIndex = 7;
			this._txtNow.Text = "";
			// 
			// _nextButton
			// 
			this._nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._nextButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._nextButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(212)))), ((int)(((byte)(17)))));
			this._nextButton.CancellableMouseDownCall = null;
			this._nextButton.Font = new System.Drawing.Font("Segoe UI", 14F);
			this._nextButton.IsDefault = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._nextButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._nextButton, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._nextButton, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._nextButton, "ScriptTextHasChangedControl._nextButton");
			this._nextButton.Location = new System.Drawing.Point(562, 376);
			this._nextButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this._nextButton.Name = "_nextButton";
			this._nextButton.Padding = new System.Windows.Forms.Padding(6);
			this._nextButton.PaddingBetweenTextAndImage = 8;
			this._nextButton.RoundedBorder = true;
			this._nextButton.Size = new System.Drawing.Size(102, 40);
			this._nextButton.State = HearThis.UI.BtnState.Normal;
			this._nextButton.TabIndex = 21;
			this._nextButton.Text = "Next";
			this._nextButton.TextForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(212)))), ((int)(((byte)(255)))));
			this._nextButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this._nextButton.Click += new System.EventHandler(this.OnNextButton);
			this._nextButton.MouseEnter += new System.EventHandler(this._nextButton_MouseEnter);
			this._nextButton.MouseLeave += new System.EventHandler(this._nextButton_MouseLeave);
			// 
			// _pnlPlayClip
			// 
			this._pnlPlayClip.AutoSize = true;
			this._pnlPlayClip.Controls.Add(this._audioButtonsControl);
			this._pnlPlayClip.Controls.Add(this._btnPlayClip);
			this._pnlPlayClip.Location = new System.Drawing.Point(13, 178);
			this._pnlPlayClip.Name = "_pnlPlayClip";
			this._pnlPlayClip.Size = new System.Drawing.Size(195, 42);
			this._pnlPlayClip.TabIndex = 23;
			this._pnlPlayClip.Paint += new System.Windows.Forms.PaintEventHandler(this._pnlPlayClip_Paint);
			// 
			// _audioButtonsControl
			// 
			this._audioButtonsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._audioButtonsControl.AutoSize = true;
			this._audioButtonsControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._audioButtonsControl.BackColor = System.Drawing.Color.Transparent;
			this._audioButtonsControl.ButtonHighlightMode = HearThis.UI.AudioButtonsControl.ButtonHighlightModes.Play;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._audioButtonsControl, "Play existing clip");
			this.l10NSharpExtender1.SetLocalizationComment(this._audioButtonsControl, null);
			this.l10NSharpExtender1.SetLocalizingId(this._audioButtonsControl, "AudioButtonsControl");
			this._audioButtonsControl.Location = new System.Drawing.Point(0, 0);
			this._audioButtonsControl.Margin = new System.Windows.Forms.Padding(0);
			this._audioButtonsControl.MinimumSize = new System.Drawing.Size(0, 42);
			this._audioButtonsControl.Name = "_audioButtonsControl";
			this._audioButtonsControl.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
			this._audioButtonsControl.RecordingDevice = null;
			this._audioButtonsControl.ShowNextButton = false;
			this._audioButtonsControl.ShowPlayButton = true;
			this._audioButtonsControl.ShowRecordButton = false;
			this._audioButtonsControl.Size = new System.Drawing.Size(43, 42);
			this._audioButtonsControl.TabIndex = 8;
			// 
			// _btnPlayClip
			// 
			this._btnPlayClip.AutoSize = true;
			this._btnPlayClip.FlatAppearance.BorderSize = 0;
			this._btnPlayClip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnPlayClip.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnPlayClip, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnPlayClip, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnPlayClip, "button1");
			this._btnPlayClip.Location = new System.Drawing.Point(53, 3);
			this._btnPlayClip.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
			this._btnPlayClip.Name = "_btnPlayClip";
			this._btnPlayClip.Size = new System.Drawing.Size(139, 33);
			this._btnPlayClip.TabIndex = 0;
			this._btnPlayClip.Text = "Play Existing Clip";
			this._btnPlayClip.UseVisualStyleBackColor = true;
			this._btnPlayClip.Click += new System.EventHandler(this._btnPlayClip_Click);
			this._btnPlayClip.MouseEnter += new System.EventHandler(this._btnPlayClip_MouseEnter);
			this._btnPlayClip.MouseLeave += new System.EventHandler(this._btnPlayClip_MouseLeave);
			// 
			// _tableOptions
			// 
			this._tableOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._tableOptions.AutoSize = true;
			this._tableOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableOptions.ColumnCount = 2;
			this._tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableOptions.Controls.Add(this._btnUseExisting, 1, 1);
			this._tableOptions.Controls.Add(this._rdoUseExisting, 0, 1);
			this._tableOptions.Controls.Add(this._rdoAskLater, 0, 0);
			this._tableOptions.Controls.Add(this._btnAskLater, 1, 0);
			this._tableOptions.Controls.Add(this._rdoReRecord, 0, 2);
			this._tableOptions.Controls.Add(this._btnDelete, 1, 2);
			this._tableOptions.Controls.Add(this._flowNearbyText, 1, 3);
			this._tableOptions.Controls.Add(this._btnShiftClips, 1, 4);
			this._tableOptions.Location = new System.Drawing.Point(341, 178);
			this._tableOptions.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._tableOptions.Name = "_tableOptions";
			this._tableOptions.RowCount = 5;
			this._tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableOptions.Size = new System.Drawing.Size(323, 198);
			this._tableOptions.TabIndex = 24;
			// 
			// _btnUseExisting
			// 
			this._btnUseExisting.AutoSize = true;
			this._btnUseExisting.FlatAppearance.BorderSize = 0;
			this._btnUseExisting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnUseExisting.Font = new System.Drawing.Font("Segoe UI", 12F);
			this._btnUseExisting.Image = global::HearThis.Properties.Resources.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnUseExisting, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnUseExisting, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnUseExisting, "ScriptTextHasChangedControl._btnUseExisting");
			this._btnUseExisting.Location = new System.Drawing.Point(23, 40);
			this._btnUseExisting.Name = "_btnUseExisting";
			this._btnUseExisting.Size = new System.Drawing.Size(200, 31);
			this._btnUseExisting.TabIndex = 10;
			this._btnUseExisting.Text = "It\'s OK, use existing clip";
			this._btnUseExisting.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._btnUseExisting.UseVisualStyleBackColor = true;
			this._btnUseExisting.Click += new System.EventHandler(this.SelectRadioButton);
			this._btnUseExisting.MouseEnter += new System.EventHandler(this.HandleMouseEnterButton);
			this._btnUseExisting.MouseLeave += new System.EventHandler(this.HandleMouseLeaveButton);
			// 
			// _rdoUseExisting
			// 
			this._rdoUseExisting.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._rdoUseExisting.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._rdoUseExisting, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._rdoUseExisting, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._rdoUseExisting, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._rdoUseExisting, "ScriptTextHasChangedControl._rdoUseExisting");
			this._rdoUseExisting.Location = new System.Drawing.Point(3, 49);
			this._rdoUseExisting.Name = "_rdoUseExisting";
			this._rdoUseExisting.Size = new System.Drawing.Size(14, 13);
			this._rdoUseExisting.TabIndex = 2;
			this._rdoUseExisting.UseVisualStyleBackColor = true;
			this._rdoUseExisting.CheckedChanged += new System.EventHandler(this._rdoUseExisting_CheckedChanged);
			this._rdoUseExisting.MouseEnter += new System.EventHandler(this.HandleMouseEnterRadioButton);
			this._rdoUseExisting.MouseLeave += new System.EventHandler(this.HandleMouseLeaveRadioButton);
			// 
			// _rdoAskLater
			// 
			this._rdoAskLater.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._rdoAskLater.AutoSize = true;
			this._rdoAskLater.Checked = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._rdoAskLater, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._rdoAskLater, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._rdoAskLater, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._rdoAskLater, "ScriptTextHasChangedControl._rdoAskLater");
			this._rdoAskLater.Location = new System.Drawing.Point(3, 12);
			this._rdoAskLater.Name = "_rdoAskLater";
			this._rdoAskLater.Size = new System.Drawing.Size(14, 13);
			this._rdoAskLater.TabIndex = 0;
			this._rdoAskLater.TabStop = true;
			this._rdoAskLater.UseVisualStyleBackColor = true;
			this._rdoAskLater.CheckedChanged += new System.EventHandler(this._rdoAskLater_CheckedChanged);
			this._rdoAskLater.MouseEnter += new System.EventHandler(this.HandleMouseEnterRadioButton);
			this._rdoAskLater.MouseLeave += new System.EventHandler(this.HandleMouseLeaveRadioButton);
			// 
			// _btnAskLater
			// 
			this._btnAskLater.AutoSize = true;
			this._btnAskLater.FlatAppearance.BorderSize = 0;
			this._btnAskLater.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnAskLater.Font = new System.Drawing.Font("Segoe UI", 12F);
			this._btnAskLater.Image = global::HearThis.Properties.Resources.Later;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnAskLater, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnAskLater, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnAskLater, "ScriptTextHasChangedControl._btnAskLater");
			this._btnAskLater.Location = new System.Drawing.Point(23, 3);
			this._btnAskLater.Name = "_btnAskLater";
			this._btnAskLater.Size = new System.Drawing.Size(127, 31);
			this._btnAskLater.TabIndex = 1;
			this._btnAskLater.Text = "Ask me later";
			this._btnAskLater.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._btnAskLater.UseVisualStyleBackColor = true;
			this._btnAskLater.Click += new System.EventHandler(this.SelectRadioButton);
			this._btnAskLater.MouseEnter += new System.EventHandler(this.HandleMouseEnterButton);
			this._btnAskLater.MouseLeave += new System.EventHandler(this.HandleMouseLeaveButton);
			// 
			// _rdoReRecord
			// 
			this._rdoReRecord.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._rdoReRecord.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._rdoReRecord, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._rdoReRecord, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._rdoReRecord, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._rdoReRecord, "ScriptTextHasChangedControl._rdoAskLater");
			this._rdoReRecord.Location = new System.Drawing.Point(3, 86);
			this._rdoReRecord.Name = "_rdoReRecord";
			this._rdoReRecord.Size = new System.Drawing.Size(14, 13);
			this._rdoReRecord.TabIndex = 3;
			this._rdoReRecord.UseVisualStyleBackColor = true;
			this._rdoReRecord.CheckedChanged += new System.EventHandler(this._rdoReRecord_CheckedChanged);
			this._rdoReRecord.MouseEnter += new System.EventHandler(this.HandleMouseEnterRadioButton);
			this._rdoReRecord.MouseLeave += new System.EventHandler(this.HandleMouseLeaveRadioButton);
			// 
			// _btnDelete
			// 
			this._btnDelete.AutoSize = true;
			this._btnDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._btnDelete.BackColor = System.Drawing.Color.Transparent;
			this._btnDelete.FlatAppearance.BorderSize = 0;
			this._btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnDelete.Font = new System.Drawing.Font("Segoe UI", 12F);
			this._btnDelete.ForeColor = System.Drawing.Color.DarkGray;
			this._btnDelete.Image = global::HearThis.Properties.Resources.BottomToolbar_Delete;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnDelete, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnDelete, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnDelete, "ScriptTextHasChangedControl._btnDelete");
			this._btnDelete.Location = new System.Drawing.Point(23, 77);
			this._btnDelete.Name = "_btnDelete";
			this._btnDelete.Size = new System.Drawing.Size(159, 31);
			this._btnDelete.TabIndex = 9;
			this._btnDelete.Text = "Need to re-record";
			this._btnDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._btnDelete.UseVisualStyleBackColor = false;
			this._btnDelete.Visible = false;
			this._btnDelete.Click += new System.EventHandler(this.SelectRadioButton);
			this._btnDelete.MouseEnter += new System.EventHandler(this.HandleMouseEnterButton);
			this._btnDelete.MouseLeave += new System.EventHandler(this.HandleMouseLeaveButton);
			// 
			// _flowNearbyText
			// 
			this._flowNearbyText.AutoSize = true;
			this._flowNearbyText.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._flowNearbyText.Controls.Add(iconShiftClips);
			this._flowNearbyText.Controls.Add(this._lblShiftClips);
			this._flowNearbyText.Location = new System.Drawing.Point(23, 114);
			this._flowNearbyText.Name = "_flowNearbyText";
			this._flowNearbyText.Size = new System.Drawing.Size(292, 26);
			this._flowNearbyText.TabIndex = 11;
			// 
			// _lblShiftClips
			// 
			this._lblShiftClips.AutoSize = true;
			this._lblShiftClips.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblShiftClips, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblShiftClips, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblShiftClips, "ScriptTextHasChangedControl._lblShiftClips");
			this._lblShiftClips.Location = new System.Drawing.Point(33, 0);
			this._lblShiftClips.Name = "_lblShiftClips";
			this._lblShiftClips.Size = new System.Drawing.Size(256, 21);
			this._lblShiftClips.TabIndex = 1;
			this._lblShiftClips.Text = "This recording is for a nearby block.";
			// 
			// _btnShiftClips
			// 
			this._btnShiftClips.AutoSize = true;
			this._btnShiftClips.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._btnShiftClips.FlatAppearance.BorderSize = 0;
			this._btnShiftClips.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnShiftClips.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnShiftClips, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnShiftClips, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnShiftClips, "button1");
			this._btnShiftClips.Location = new System.Drawing.Point(50, 146);
			this._btnShiftClips.Margin = new System.Windows.Forms.Padding(30, 3, 3, 0);
			this._btnShiftClips.Name = "_btnShiftClips";
			this._btnShiftClips.Size = new System.Drawing.Size(131, 31);
			this._btnShiftClips.TabIndex = 12;
			this._btnShiftClips.Text = "Shift Clips Tool...";
			this._btnShiftClips.UseVisualStyleBackColor = true;
			this._btnShiftClips.Click += new System.EventHandler(this._btnShiftClips_Click);
			this._btnShiftClips.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintRoundedBorder);
			this._btnShiftClips.MouseEnter += new System.EventHandler(this.HandleMouseEnterButton);
			this._btnShiftClips.MouseLeave += new System.EventHandler(this.HandleMouseLeaveButton);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "";
			// 
			// ScriptTextHasChangedControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Transparent;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._masterTableLayoutPanel);
			this.ForeColor = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this, "ScriptTextHasChangedControl.ScriptChangedControl");
			this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this.MinimumSize = new System.Drawing.Size(0, 120);
			this.Name = "ScriptTextHasChangedControl";
			this.Size = new System.Drawing.Size(677, 429);
			((System.ComponentModel.ISupportInitialize)(iconShiftClips)).EndInit();
			this._masterTableLayoutPanel.ResumeLayout(false);
			this._masterTableLayoutPanel.PerformLayout();
			this._panelThen.ResumeLayout(false);
			this._panelThen.PerformLayout();
			this._pnlPlayClip.ResumeLayout(false);
			this._pnlPlayClip.PerformLayout();
			this._tableOptions.ResumeLayout(false);
			this._tableOptions.PerformLayout();
			this._flowNearbyText.ResumeLayout(false);
			this._flowNearbyText.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.TableLayoutPanel _masterTableLayoutPanel;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label _lblThen;
		private System.Windows.Forms.Label _lblRecordedDate;
		private System.Windows.Forms.Label _lblNow;
		private System.Windows.Forms.RichTextBox _txtThen;
		private System.Windows.Forms.RichTextBox _txtNow;
		private AudioButtonsControl _audioButtonsControl;
		private Button _btnDelete;
		private System.Windows.Forms.FlowLayoutPanel _panelThen;
		private ArrowButton _nextButton;
		private Button _lblProblemSummary;
		private FlowLayoutPanel _pnlPlayClip;
		private Button _btnPlayClip;
		private TableLayoutPanel _tableOptions;
		private RadioButton _rdoAskLater;
		private Button _btnAskLater;
		private Button _btnUseExisting;
		private RadioButton _rdoUseExisting;
		private RadioButton _rdoReRecord;
		private FlowLayoutPanel _flowNearbyText;
		private Label _lblShiftClips;
		private Button _btnShiftClips;
	}
}
