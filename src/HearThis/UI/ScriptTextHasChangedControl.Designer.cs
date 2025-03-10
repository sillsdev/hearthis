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
            if (disposing)
                components?.Dispose();
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
            this._iconShiftClips = new System.Windows.Forms.PictureBox();
            this.tableMaster = new System.Windows.Forms.TableLayoutPanel();
            this._tableBlockText = new System.Windows.Forms.TableLayoutPanel();
            this._lblNow = new System.Windows.Forms.Label();
            this._lblBefore = new System.Windows.Forms.Label();
            this._txtThen = new System.Windows.Forms.RichTextBox();
            this._txtNow = new System.Windows.Forms.RichTextBox();
            this._tableProblem = new System.Windows.Forms.TableLayoutPanel();
            this._problemIcon = new System.Windows.Forms.PictureBox();
            this._lblProblemSummary = new System.Windows.Forms.Label();
            this._lblResolution = new System.Windows.Forms.Label();
            this._tableButtons = new System.Windows.Forms.TableLayoutPanel();
            this._tableOptions = new System.Windows.Forms.TableLayoutPanel();
            this._lblShiftClips = new System.Windows.Forms.Label();
            this._rdoUseExisting = new System.Windows.Forms.RadioButton();
            this._rdoAskLater = new System.Windows.Forms.RadioButton();
            this._rdoReRecord = new System.Windows.Forms.RadioButton();
            this._btnShiftClips = new System.Windows.Forms.Button();
            this._pnlPlayClip = new System.Windows.Forms.TableLayoutPanel();
            this._lblEditingCompleteInstructions = new System.Windows.Forms.Label();
            this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._btnAskLater = new HearThis.UI.RadioButtonHelperButton();
            this._btnUseExisting = new HearThis.UI.RadioButtonHelperButton();
            this._btnDelete = new HearThis.UI.RadioButtonHelperButton();
            this._audioButtonsControl = new HearThis.UI.AudioButtonsControl();
            this._btnPlayClip = new HearThis.UI.ButtonWithoutFocusRectangle();
            this._btnEditClip = new HearThis.UI.ButtonWithoutFocusRectangle();
            this._editSoundFile = new HearThis.UI.MouseSensitiveIconButton();
            this._copyPathToClipboard = new HearThis.UI.MouseSensitiveIconButton();
            this._nextButton = new HearThis.UI.MouseSensitiveIconButton();
            ((System.ComponentModel.ISupportInitialize)(this._iconShiftClips)).BeginInit();
            this.tableMaster.SuspendLayout();
            this._tableBlockText.SuspendLayout();
            this._tableProblem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._problemIcon)).BeginInit();
            this._tableButtons.SuspendLayout();
            this._tableOptions.SuspendLayout();
            this._pnlPlayClip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._btnAskLater)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._btnUseExisting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._btnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._editSoundFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._copyPathToClipboard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nextButton)).BeginInit();
            this.SuspendLayout();
            // 
            // _iconShiftClips
            // 
            this._iconShiftClips.Image = global::HearThis.Properties.Resources.shift_clips24;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._iconShiftClips, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._iconShiftClips, null);
            this.l10NSharpExtender1.SetLocalizationPriority(this._iconShiftClips, L10NSharp.LocalizationPriority.NotLocalizable);
            this.l10NSharpExtender1.SetLocalizingId(this._iconShiftClips, "ScriptTextHasChangedControl._iconShiftClips");
            this._iconShiftClips.Location = new System.Drawing.Point(27, 100);
            this._iconShiftClips.Margin = new System.Windows.Forms.Padding(7, 1, 0, 1);
            this._iconShiftClips.Name = "_iconShiftClips";
            this._iconShiftClips.Size = new System.Drawing.Size(22, 24);
            this._iconShiftClips.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this._iconShiftClips.TabIndex = 0;
            this._iconShiftClips.TabStop = false;
            // 
            // tableMaster
            // 
            this.tableMaster.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableMaster.ColumnCount = 1;
            this.tableMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMaster.Controls.Add(this._tableBlockText, 0, 1);
            this.tableMaster.Controls.Add(this._tableProblem, 0, 0);
            this.tableMaster.Controls.Add(this._tableButtons, 0, 2);
            this.tableMaster.Location = new System.Drawing.Point(13, 15);
            this.tableMaster.Margin = new System.Windows.Forms.Padding(0);
            this.tableMaster.Name = "tableMaster";
            this.tableMaster.RowCount = 3;
            this.tableMaster.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMaster.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMaster.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMaster.Size = new System.Drawing.Size(724, 526);
            this.tableMaster.TabIndex = 25;
            // 
            // _tableBlockText
            // 
            this._tableBlockText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._tableBlockText.ColumnCount = 2;
            this._tableBlockText.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tableBlockText.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tableBlockText.Controls.Add(this._lblNow, 1, 0);
            this._tableBlockText.Controls.Add(this._lblBefore, 0, 0);
            this._tableBlockText.Controls.Add(this._txtThen, 0, 1);
            this._tableBlockText.Controls.Add(this._txtNow, 1, 1);
            this._tableBlockText.Location = new System.Drawing.Point(3, 73);
            this._tableBlockText.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this._tableBlockText.MinimumSize = new System.Drawing.Size(0, 62);
            this._tableBlockText.Name = "_tableBlockText";
            this._tableBlockText.Padding = new System.Windows.Forms.Padding(0, 14, 0, 10);
            this._tableBlockText.RowCount = 2;
            this._tableBlockText.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableBlockText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableBlockText.Size = new System.Drawing.Size(718, 245);
            this._tableBlockText.TabIndex = 0;
            // 
            // _lblNow
            // 
            this._lblNow.AutoSize = true;
            this._lblNow.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblNow, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblNow, null);
            this.l10NSharpExtender1.SetLocalizingId(this._lblNow, "ScriptTextHasChangedControl._lblNow");
            this._lblNow.Location = new System.Drawing.Point(362, 14);
            this._lblNow.MinimumSize = new System.Drawing.Size(0, 28);
            this._lblNow.Name = "_lblNow";
            this._lblNow.Size = new System.Drawing.Size(46, 28);
            this._lblNow.TabIndex = 4;
            this._lblNow.Text = "Now";
            // 
            // _lblBefore
            // 
            this._lblBefore.AutoSize = true;
            this._lblBefore.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblBefore, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblBefore, "Param 0: the date when the clip was recorded.");
            this.l10NSharpExtender1.SetLocalizingId(this._lblBefore, "ScriptTextHasChangedControl._lblBefore");
            this._lblBefore.Location = new System.Drawing.Point(3, 14);
            this._lblBefore.MinimumSize = new System.Drawing.Size(0, 28);
            this._lblBefore.Name = "_lblBefore";
            this._lblBefore.Size = new System.Drawing.Size(98, 28);
            this._lblBefore.TabIndex = 2;
            this._lblBefore.Text = "Before ({0})";
            // 
            // _txtThen
            // 
            this._txtThen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtThen.BackColor = System.Drawing.SystemColors.Window;
            this._txtThen.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._txtThen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this._txtThen.Location = new System.Drawing.Point(6, 48);
            this._txtThen.Margin = new System.Windows.Forms.Padding(6);
            this._txtThen.MinimumSize = new System.Drawing.Size(0, 24);
            this._txtThen.Name = "_txtThen";
            this._txtThen.ReadOnly = true;
            this._txtThen.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this._txtThen.Size = new System.Drawing.Size(347, 181);
            this._txtThen.TabIndex = 6;
            this._txtThen.Text = "";
            // 
            // _txtNow
            // 
            this._txtNow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtNow.BackColor = System.Drawing.SystemColors.Window;
            this._txtNow.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._txtNow.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this._txtNow.Location = new System.Drawing.Point(365, 48);
            this._txtNow.Margin = new System.Windows.Forms.Padding(6, 6, 3, 6);
            this._txtNow.MinimumSize = new System.Drawing.Size(0, 24);
            this._txtNow.Name = "_txtNow";
            this._txtNow.ReadOnly = true;
            this._txtNow.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this._txtNow.Size = new System.Drawing.Size(350, 181);
            this._txtNow.TabIndex = 7;
            this._txtNow.Text = "";
            // 
            // _tableProblem
            // 
            this._tableProblem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tableProblem.AutoSize = true;
            this._tableProblem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._tableProblem.ColumnCount = 2;
            this._tableProblem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableProblem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableProblem.Controls.Add(this._problemIcon, 0, 0);
            this._tableProblem.Controls.Add(this._lblProblemSummary, 1, 0);
            this._tableProblem.Controls.Add(this._lblResolution, 1, 1);
            this._tableProblem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(212)))), ((int)(((byte)(17)))));
            this._tableProblem.Location = new System.Drawing.Point(3, 3);
            this._tableProblem.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this._tableProblem.MinimumSize = new System.Drawing.Size(0, 26);
            this._tableProblem.Name = "_tableProblem";
            this._tableProblem.Padding = new System.Windows.Forms.Padding(3);
            this._tableProblem.RowCount = 2;
            this._tableProblem.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableProblem.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableProblem.Size = new System.Drawing.Size(718, 60);
            this._tableProblem.TabIndex = 24;
            this._tableProblem.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintRoundedBorder);
            // 
            // _problemIcon
            // 
            this._problemIcon.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._problemIcon.Image = global::HearThis.Properties.Resources.AlertCircle;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._problemIcon, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._problemIcon, null);
            this.l10NSharpExtender1.SetLocalizationPriority(this._problemIcon, L10NSharp.LocalizationPriority.NotLocalizable);
            this.l10NSharpExtender1.SetLocalizingId(this._problemIcon, "ScriptTextHasChangedControl._problemIcon");
            this._problemIcon.Location = new System.Drawing.Point(6, 6);
            this._problemIcon.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this._problemIcon.Name = "_problemIcon";
            this._problemIcon.Size = new System.Drawing.Size(21, 21);
            this._problemIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._problemIcon.TabIndex = 0;
            this._problemIcon.TabStop = false;
            // 
            // _lblProblemSummary
            // 
            this._lblProblemSummary.AutoSize = true;
            this._lblProblemSummary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._lblProblemSummary.Font = new System.Drawing.Font("Segoe UI", 12F);
            this._lblProblemSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(212)))), ((int)(((byte)(17)))));
            this._lblProblemSummary.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblProblemSummary, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblProblemSummary, null);
            this.l10NSharpExtender1.SetLocalizingId(this._lblProblemSummary, "ScriptTextHasChangedControl._lblProblemSummary");
            this._lblProblemSummary.Location = new System.Drawing.Point(40, 6);
            this._lblProblemSummary.Margin = new System.Windows.Forms.Padding(3);
            this._lblProblemSummary.MaximumSize = new System.Drawing.Size(0, 400);
            this._lblProblemSummary.Name = "_lblProblemSummary";
            this._lblProblemSummary.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this._lblProblemSummary.Size = new System.Drawing.Size(439, 21);
            this._lblProblemSummary.TabIndex = 22;
            this._lblProblemSummary.Text = "The text of this block has changed since the clip was recorded.";
            this._lblProblemSummary.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _lblResolution
            // 
            this._lblResolution.AutoSize = true;
            this._lblResolution.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblResolution, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblResolution, null);
            this.l10NSharpExtender1.SetLocalizingId(this._lblResolution, "ScriptTextHasChangedControl._lblResolution");
            this._lblResolution.Location = new System.Drawing.Point(40, 30);
            this._lblResolution.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this._lblResolution.Name = "_lblResolution";
            this._lblResolution.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this._lblResolution.Size = new System.Drawing.Size(121, 24);
            this._lblResolution.TabIndex = 23;
            this._lblResolution.Text = "Decision: It\'s OK";
            this._lblResolution.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _tableButtons
            // 
            this._tableButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tableButtons.AutoSize = true;
            this._tableButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._tableButtons.BackColor = System.Drawing.Color.Transparent;
            this._tableButtons.ColumnCount = 2;
            this._tableButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tableButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tableButtons.Controls.Add(this._tableOptions, 1, 0);
            this._tableButtons.Controls.Add(this._pnlPlayClip, 0, 0);
            this._tableButtons.Controls.Add(this._nextButton, 1, 1);
            this._tableButtons.Location = new System.Drawing.Point(0, 329);
            this._tableButtons.Margin = new System.Windows.Forms.Padding(0);
            this._tableButtons.Name = "_tableButtons";
            this._tableButtons.RowCount = 2;
            this._tableButtons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableButtons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableButtons.Size = new System.Drawing.Size(724, 197);
            this._tableButtons.TabIndex = 23;
            // 
            // _tableOptions
            // 
            this._tableOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tableOptions.AutoSize = true;
            this._tableOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._tableOptions.BackColor = System.Drawing.Color.Transparent;
            this._tableOptions.ColumnCount = 3;
            this._tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableOptions.Controls.Add(this._lblShiftClips, 2, 3);
            this._tableOptions.Controls.Add(this._iconShiftClips, 1, 3);
            this._tableOptions.Controls.Add(this._rdoUseExisting, 0, 1);
            this._tableOptions.Controls.Add(this._rdoAskLater, 0, 0);
            this._tableOptions.Controls.Add(this._rdoReRecord, 0, 2);
            this._tableOptions.Controls.Add(this._btnShiftClips, 2, 4);
            this._tableOptions.Controls.Add(this._btnAskLater, 1, 0);
            this._tableOptions.Controls.Add(this._btnUseExisting, 1, 1);
            this._tableOptions.Controls.Add(this._btnDelete, 1, 2);
            this._tableOptions.Location = new System.Drawing.Point(365, 3);
            this._tableOptions.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this._tableOptions.Name = "_tableOptions";
            this._tableOptions.RowCount = 5;
            this._tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableOptions.Size = new System.Drawing.Size(356, 157);
            this._tableOptions.TabIndex = 24;
            // 
            // _lblShiftClips
            // 
            this._lblShiftClips.AutoSize = true;
            this._lblShiftClips.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblShiftClips, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblShiftClips, null);
            this.l10NSharpExtender1.SetLocalizingId(this._lblShiftClips, "ScriptTextHasChangedControl._lblShiftClips");
            this._lblShiftClips.Location = new System.Drawing.Point(49, 99);
            this._lblShiftClips.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this._lblShiftClips.Name = "_lblShiftClips";
            this._lblShiftClips.Size = new System.Drawing.Size(213, 21);
            this._lblShiftClips.TabIndex = 1;
            this._lblShiftClips.Text = "This clip is for a nearby block.";
            // 
            // _rdoUseExisting
            // 
            this._rdoUseExisting.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._rdoUseExisting.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._rdoUseExisting, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._rdoUseExisting, null);
            this.l10NSharpExtender1.SetLocalizationPriority(this._rdoUseExisting, L10NSharp.LocalizationPriority.NotLocalizable);
            this.l10NSharpExtender1.SetLocalizingId(this._rdoUseExisting, "ScriptTextHasChangedControl._rdoUseExisting");
            this._rdoUseExisting.Location = new System.Drawing.Point(3, 43);
            this._rdoUseExisting.Name = "_rdoUseExisting";
            this._rdoUseExisting.Size = new System.Drawing.Size(14, 13);
            this._rdoUseExisting.TabIndex = 2;
            this._rdoUseExisting.UseVisualStyleBackColor = true;
            this._rdoUseExisting.CheckedChanged += new System.EventHandler(this._rdoUseExisting_CheckedChanged);
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
            this._rdoAskLater.Location = new System.Drawing.Point(3, 10);
            this._rdoAskLater.Name = "_rdoAskLater";
            this._rdoAskLater.Size = new System.Drawing.Size(14, 13);
            this._rdoAskLater.TabIndex = 0;
            this._rdoAskLater.TabStop = true;
            this._rdoAskLater.UseVisualStyleBackColor = true;
            this._rdoAskLater.CheckedChanged += new System.EventHandler(this._rdoAskLater_CheckedChanged);
            // 
            // _rdoReRecord
            // 
            this._rdoReRecord.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._rdoReRecord.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._rdoReRecord, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._rdoReRecord, null);
            this.l10NSharpExtender1.SetLocalizationPriority(this._rdoReRecord, L10NSharp.LocalizationPriority.NotLocalizable);
            this.l10NSharpExtender1.SetLocalizingId(this._rdoReRecord, "ScriptTextHasChangedControl._rdoAskLater");
            this._rdoReRecord.Location = new System.Drawing.Point(3, 76);
            this._rdoReRecord.Name = "_rdoReRecord";
            this._rdoReRecord.Size = new System.Drawing.Size(14, 13);
            this._rdoReRecord.TabIndex = 3;
            this._rdoReRecord.UseVisualStyleBackColor = true;
            this._rdoReRecord.CheckedChanged += new System.EventHandler(this._rdoReRecord_CheckedChanged);
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
            this.l10NSharpExtender1.SetLocalizingId(this._btnShiftClips, "ScriptTextHasChangedControl._btnShiftClips");
            this._btnShiftClips.Location = new System.Drawing.Point(49, 126);
            this._btnShiftClips.Margin = new System.Windows.Forms.Padding(0, 1, 3, 0);
            this._btnShiftClips.Name = "_btnShiftClips";
            this._btnShiftClips.Size = new System.Drawing.Size(131, 31);
            this._btnShiftClips.TabIndex = 12;
            this._btnShiftClips.Text = "Shift Clips Tool...";
            this._btnShiftClips.UseVisualStyleBackColor = true;
            this._btnShiftClips.Click += new System.EventHandler(this._btnShiftClips_Click);
            this._btnShiftClips.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintRoundedBorder);
            this._btnShiftClips.MouseEnter += new System.EventHandler(this._btnShiftClips_MouseEnter);
            this._btnShiftClips.MouseLeave += new System.EventHandler(this._btnShiftClips_MouseLeave);
            // 
            // _pnlPlayClip
            // 
            this._pnlPlayClip.AutoSize = true;
            this._pnlPlayClip.ColumnCount = 3;
            this._pnlPlayClip.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._pnlPlayClip.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._pnlPlayClip.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._pnlPlayClip.Controls.Add(this._audioButtonsControl, 0, 0);
            this._pnlPlayClip.Controls.Add(this._btnPlayClip, 1, 0);
            this._pnlPlayClip.Controls.Add(this._btnEditClip, 1, 1);
            this._pnlPlayClip.Controls.Add(this._lblEditingCompleteInstructions, 1, 2);
            this._pnlPlayClip.Controls.Add(this._editSoundFile, 0, 1);
            this._pnlPlayClip.Controls.Add(this._copyPathToClipboard, 2, 2);
            this._pnlPlayClip.Location = new System.Drawing.Point(3, 8);
            this._pnlPlayClip.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this._pnlPlayClip.Name = "_pnlPlayClip";
            this._pnlPlayClip.RowCount = 3;
            this._pnlPlayClip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._pnlPlayClip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._pnlPlayClip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._pnlPlayClip.Size = new System.Drawing.Size(261, 112);
            this._pnlPlayClip.TabIndex = 23;
            this._pnlPlayClip.Click += new System.EventHandler(this._btnPlayClip_Click);
            this._pnlPlayClip.Paint += new System.Windows.Forms.PaintEventHandler(this._pnlPlayClip_Paint);
            this._pnlPlayClip.MouseEnter += new System.EventHandler(this.PlayClip_MouseEnter);
            this._pnlPlayClip.MouseLeave += new System.EventHandler(this.PlayClip_MouseLeave);
            // 
            // _lblEditingCompleteInstructions
            // 
            this._lblEditingCompleteInstructions.AutoSize = true;
            this._lblEditingCompleteInstructions.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblEditingCompleteInstructions, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblEditingCompleteInstructions, "Param 0: File path");
            this.l10NSharpExtender1.SetLocalizingId(this._lblEditingCompleteInstructions, "ScriptTextHasChangedControl._lblEditingCompleteInstructions");
            this._lblEditingCompleteInstructions.Location = new System.Drawing.Point(46, 79);
            this._lblEditingCompleteInstructions.Name = "_lblEditingCompleteInstructions";
            this._lblEditingCompleteInstructions.Padding = new System.Windows.Forms.Padding(0, 0, 0, 7);
            this._lblEditingCompleteInstructions.Size = new System.Drawing.Size(184, 33);
            this._lblEditingCompleteInstructions.TabIndex = 10;
            this._lblEditingCompleteInstructions.Text = "When editing is complete, save as:\r\n{0}";
            this._lblEditingCompleteInstructions.Visible = false;
            // 
            // l10NSharpExtender1
            // 
            this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
            this.l10NSharpExtender1.PrefixForNewItems = "";
            // 
            // _btnAskLater
            // 
            this._btnAskLater.AutoSize = true;
            this._btnAskLater.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._tableOptions.SetColumnSpan(this._btnAskLater, 2);
            this._btnAskLater.FlatAppearance.BorderSize = 0;
            this._btnAskLater.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._btnAskLater.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._btnAskLater.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnAskLater.Font = new System.Drawing.Font("Segoe UI", 12F);
            this._btnAskLater.HighContrastMouseOverImage = global::HearThis.Properties.Resources.Later_MouseOverHC;
            this._btnAskLater.Image = global::HearThis.Properties.Resources.Later;
            this._btnAskLater.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._btnAskLater, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._btnAskLater, null);
            this.l10NSharpExtender1.SetLocalizingId(this._btnAskLater, "ScriptTextHasChangedControl._btnAskLater");
            this._btnAskLater.Location = new System.Drawing.Point(23, 1);
            this._btnAskLater.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this._btnAskLater.MouseOverImage = global::HearThis.Properties.Resources.Later_MouseOver;
            this._btnAskLater.Name = "_btnAskLater";
            this._btnAskLater.RoundedBorderColor = System.Drawing.Color.Empty;
            this._btnAskLater.RoundedBorderThickness = 0;
            this._btnAskLater.Size = new System.Drawing.Size(127, 31);
            this._btnAskLater.TabIndex = 1;
            this._btnAskLater.Text = "Ask me later";
            this._btnAskLater.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this._btnAskLater.UseVisualStyleBackColor = true;
            // 
            // _btnUseExisting
            // 
            this._btnUseExisting.AutoSize = true;
            this._btnUseExisting.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._tableOptions.SetColumnSpan(this._btnUseExisting, 2);
            this._btnUseExisting.FlatAppearance.BorderSize = 0;
            this._btnUseExisting.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._btnUseExisting.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._btnUseExisting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnUseExisting.Font = new System.Drawing.Font("Segoe UI", 12F);
            this._btnUseExisting.HighContrastMouseOverImage = global::HearThis.Properties.Resources.OK_mouseOverHC;
            this._btnUseExisting.Image = global::HearThis.Properties.Resources.OK;
            this._btnUseExisting.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._btnUseExisting, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._btnUseExisting, null);
            this.l10NSharpExtender1.SetLocalizingId(this._btnUseExisting, "ScriptTextHasChangedControl._btnUseExisting");
            this._btnUseExisting.Location = new System.Drawing.Point(23, 34);
            this._btnUseExisting.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this._btnUseExisting.MouseOverImage = global::HearThis.Properties.Resources.OK_MouseOver;
            this._btnUseExisting.Name = "_btnUseExisting";
            this._btnUseExisting.RoundedBorderColor = System.Drawing.Color.Empty;
            this._btnUseExisting.RoundedBorderThickness = 0;
            this._btnUseExisting.Size = new System.Drawing.Size(202, 31);
            this._btnUseExisting.TabIndex = 10;
            this._btnUseExisting.Text = "It\'s OK, use existing clip";
            this._btnUseExisting.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this._btnUseExisting.UseVisualStyleBackColor = true;
            // 
            // _btnDelete
            // 
            this._btnDelete.AutoSize = true;
            this._btnDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnDelete.BackColor = System.Drawing.Color.Transparent;
            this._tableOptions.SetColumnSpan(this._btnDelete, 2);
            this._btnDelete.FlatAppearance.BorderSize = 0;
            this._btnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnDelete.Font = new System.Drawing.Font("Segoe UI", 12F);
            this._btnDelete.ForeColor = System.Drawing.Color.DarkGray;
            this._btnDelete.HighContrastMouseOverImage = global::HearThis.Properties.Resources.Delete_MouseOverHC;
            this._btnDelete.Image = global::HearThis.Properties.Resources.Delete;
            this._btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._btnDelete, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._btnDelete, null);
            this.l10NSharpExtender1.SetLocalizingId(this._btnDelete, "ScriptTextHasChangedControl._btnDelete");
            this._btnDelete.Location = new System.Drawing.Point(23, 67);
            this._btnDelete.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this._btnDelete.MouseOverImage = global::HearThis.Properties.Resources.Delete_MouseOver;
            this._btnDelete.Name = "_btnDelete";
            this._btnDelete.RoundedBorderColor = System.Drawing.Color.Empty;
            this._btnDelete.RoundedBorderThickness = 0;
            this._btnDelete.Size = new System.Drawing.Size(209, 31);
            this._btnDelete.TabIndex = 9;
            this._btnDelete.Text = "Need to re-record block";
            this._btnDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._btnDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this._btnDelete.UseVisualStyleBackColor = false;
            this._btnDelete.Visible = false;
            // 
            // _audioButtonsControl
            // 
            this._audioButtonsControl.AutoSize = true;
            this._audioButtonsControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._audioButtonsControl.BackColor = System.Drawing.Color.Transparent;
            this._audioButtonsControl.ButtonHighlightMode = HearThis.UI.AudioButtonsControl.ButtonHighlightModes.Play;
            this._audioButtonsControl.HaveSomethingToRecord = false;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._audioButtonsControl, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._audioButtonsControl, null);
            this.l10NSharpExtender1.SetLocalizingId(this._audioButtonsControl, "AudioButtonsControl");
            this._audioButtonsControl.Location = new System.Drawing.Point(0, 0);
            this._audioButtonsControl.Margin = new System.Windows.Forms.Padding(0);
            this._audioButtonsControl.MinimumSize = new System.Drawing.Size(0, 42);
            this._audioButtonsControl.Name = "_audioButtonsControl";
            this._audioButtonsControl.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this._audioButtonsControl.ShowNextButton = false;
            this._audioButtonsControl.ShowRecordButton = false;
            this._audioButtonsControl.Size = new System.Drawing.Size(43, 42);
            this._audioButtonsControl.TabIndex = 8;
            // 
            // _btnPlayClip
            // 
            this._btnPlayClip.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._btnPlayClip.AutoSize = true;
            this._btnPlayClip.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnPlayClip.FlatAppearance.BorderSize = 0;
            this._btnPlayClip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnPlayClip.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.l10NSharpExtender1.SetLocalizableToolTip(this._btnPlayClip, "Play the existing clip (Tab key)");
            this.l10NSharpExtender1.SetLocalizationComment(this._btnPlayClip, null);
            this.l10NSharpExtender1.SetLocalizingId(this._btnPlayClip, "ScriptTextHasChangedControl._btnPlayClip");
            this._btnPlayClip.Location = new System.Drawing.Point(53, 5);
            this._btnPlayClip.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this._btnPlayClip.Name = "_btnPlayClip";
            this._btnPlayClip.Size = new System.Drawing.Size(137, 31);
            this._btnPlayClip.TabIndex = 0;
            this._btnPlayClip.TabStop = false;
            this._btnPlayClip.Text = "Play Existing Clip";
            this._btnPlayClip.UseVisualStyleBackColor = true;
            this._btnPlayClip.Click += new System.EventHandler(this._btnPlayClip_Click);
            this._btnPlayClip.MouseEnter += new System.EventHandler(this.PlayClip_MouseEnter);
            this._btnPlayClip.MouseLeave += new System.EventHandler(this.PlayClip_MouseLeave);
            // 
            // _btnEditClip
            // 
            this._btnEditClip.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._btnEditClip.AutoSize = true;
            this._btnEditClip.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnEditClip.FlatAppearance.BorderSize = 0;
            this._btnEditClip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnEditClip.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l10NSharpExtender1.SetLocalizableToolTip(this._btnEditClip, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._btnEditClip, null);
            this.l10NSharpExtender1.SetLocalizationPriority(this._btnEditClip, L10NSharp.LocalizationPriority.Medium);
            this.l10NSharpExtender1.SetLocalizingId(this._btnEditClip, "ScriptTextHasChangedControl._btnEditClip");
            this._btnEditClip.Location = new System.Drawing.Point(53, 45);
            this._btnEditClip.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this._btnEditClip.Name = "_btnEditClip";
            this._btnEditClip.Size = new System.Drawing.Size(151, 31);
            this._btnEditClip.TabIndex = 9;
            this._btnEditClip.Text = "Open Clip in Editor";
            this._btnEditClip.UseVisualStyleBackColor = true;
            this._btnEditClip.Click += new System.EventHandler(this._editSoundFile_Click);
            this._btnEditClip.MouseEnter += new System.EventHandler(this.EditClip_MouseEnter);
            this._btnEditClip.MouseLeave += new System.EventHandler(this.EditClip_MouseLeave);
            // 
            // _editSoundFile
            // 
            this._editSoundFile.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._editSoundFile.AutoSize = true;
            this._editSoundFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._editSoundFile.FlatAppearance.BorderSize = 0;
            this._editSoundFile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._editSoundFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._editSoundFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._editSoundFile.HighContrastMouseOverImage = global::HearThis.Properties.Resources.sound_edit_MouseOverHC;
            this._editSoundFile.Image = global::HearThis.Properties.Resources.sound_edit;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._editSoundFile, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._editSoundFile, null);
            this.l10NSharpExtender1.SetLocalizingId(this._editSoundFile, "ScriptTextHasChangedControl._editSoundFile");
            this._editSoundFile.Location = new System.Drawing.Point(10, 49);
            this._editSoundFile.MouseOverImage = global::HearThis.Properties.Resources.sound_edit_MouseOver;
            this._editSoundFile.Name = "_editSoundFile";
            this._editSoundFile.RoundedBorderColor = System.Drawing.Color.Empty;
            this._editSoundFile.RoundedBorderThickness = 0;
            this._editSoundFile.Size = new System.Drawing.Size(22, 22);
            this._editSoundFile.TabIndex = 11;
            this._editSoundFile.UseVisualStyleBackColor = true;
            this._editSoundFile.Click += new System.EventHandler(this._editSoundFile_Click);
            this._editSoundFile.MouseEnter += new System.EventHandler(this.EditClip_MouseEnter);
            this._editSoundFile.MouseLeave += new System.EventHandler(this.EditClip_MouseLeave);
            // 
            // _copyPathToClipboard
            // 
            this._copyPathToClipboard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._copyPathToClipboard.AutoSize = true;
            this._copyPathToClipboard.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._copyPathToClipboard.FlatAppearance.BorderSize = 0;
            this._copyPathToClipboard.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._copyPathToClipboard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._copyPathToClipboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._copyPathToClipboard.HighContrastMouseOverImage = global::HearThis.Properties.Resources.copy_MouseOverHC;
            this._copyPathToClipboard.Image = global::HearThis.Properties.Resources.copy;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._copyPathToClipboard, "Copy file path to clipboard");
            this.l10NSharpExtender1.SetLocalizationComment(this._copyPathToClipboard, null);
            this.l10NSharpExtender1.SetLocalizingId(this._copyPathToClipboard, "ScriptTextHasChangedControl._copyPathToClipboard");
            this._copyPathToClipboard.Location = new System.Drawing.Point(236, 84);
            this._copyPathToClipboard.MouseOverImage = global::HearThis.Properties.Resources.copy_MouseOver;
            this._copyPathToClipboard.Name = "_copyPathToClipboard";
            this._copyPathToClipboard.RoundedBorderColor = System.Drawing.Color.Empty;
            this._copyPathToClipboard.RoundedBorderThickness = 0;
            this._copyPathToClipboard.Size = new System.Drawing.Size(22, 22);
            this._copyPathToClipboard.TabIndex = 12;
            this._copyPathToClipboard.UseVisualStyleBackColor = true;
            this._copyPathToClipboard.Visible = false;
            this._copyPathToClipboard.Click += new System.EventHandler(this._copyPathToClipboard_Click);
            // 
            // _nextButton
            // 
            this._nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._nextButton.AutoSize = true;
            this._nextButton.FlatAppearance.BorderSize = 0;
            this._nextButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._nextButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._nextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._nextButton.Font = new System.Drawing.Font("Segoe UI", 14F);
            this._nextButton.HighContrastMouseOverImage = global::HearThis.Properties.Resources.NextArrow_MouseOverHC;
            this._nextButton.Image = global::HearThis.Properties.Resources.NextArrow;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._nextButton, "Next problem in this chapter (Page Down key)");
            this.l10NSharpExtender1.SetLocalizationComment(this._nextButton, null);
            this.l10NSharpExtender1.SetLocalizingId(this._nextButton, "ScriptTextHasChangedControl._nextButton");
            this._nextButton.Location = new System.Drawing.Point(610, 160);
            this._nextButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this._nextButton.MinimumSize = new System.Drawing.Size(106, 37);
            this._nextButton.MouseOverImage = global::HearThis.Properties.Resources.NextArrow_MouseOver;
            this._nextButton.Name = "_nextButton";
            this._nextButton.Padding = new System.Windows.Forms.Padding(10, 1, 6, 1);
            this._nextButton.RoundedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(212)))), ((int)(((byte)(17)))));
            this._nextButton.RoundedBorderThickness = 1;
            this._nextButton.Size = new System.Drawing.Size(106, 37);
            this._nextButton.TabIndex = 21;
            this._nextButton.Text = "Next";
            this._nextButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this._nextButton.UseVisualStyleBackColor = false;
            this._nextButton.Click += new System.EventHandler(this.OnNextButton);
            // 
            // ScriptTextHasChangedControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableMaster);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.DarkGray;
            this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
            this.l10NSharpExtender1.SetLocalizationComment(this, null);
            this.l10NSharpExtender1.SetLocalizationPriority(this, L10NSharp.LocalizationPriority.NotLocalizable);
            this.l10NSharpExtender1.SetLocalizingId(this, "ScriptTextHasChangedControl.ScriptChangedControl");
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.MinimumSize = new System.Drawing.Size(0, 120);
            this.Name = "ScriptTextHasChangedControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(750, 553);
            ((System.ComponentModel.ISupportInitialize)(this._iconShiftClips)).EndInit();
            this.tableMaster.ResumeLayout(false);
            this.tableMaster.PerformLayout();
            this._tableBlockText.ResumeLayout(false);
            this._tableBlockText.PerformLayout();
            this._tableProblem.ResumeLayout(false);
            this._tableProblem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._problemIcon)).EndInit();
            this._tableButtons.ResumeLayout(false);
            this._tableButtons.PerformLayout();
            this._tableOptions.ResumeLayout(false);
            this._tableOptions.PerformLayout();
            this._pnlPlayClip.ResumeLayout(false);
            this._pnlPlayClip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._btnAskLater)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._btnUseExisting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._btnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._editSoundFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._copyPathToClipboard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nextButton)).EndInit();
            this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableBlockText;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label _lblBefore;
		private System.Windows.Forms.Label _lblNow;
		private System.Windows.Forms.RichTextBox _txtThen;
		private System.Windows.Forms.RichTextBox _txtNow;
		private HearThis.UI.RadioButtonHelperButton _btnDelete;
		private HearThis.UI.MouseSensitiveIconButton _nextButton;
		private System.Windows.Forms.Label _lblProblemSummary;
		private TableLayoutPanel _tableOptions;
		private RadioButton _rdoAskLater;
		private HearThis.UI.RadioButtonHelperButton _btnAskLater;
		private HearThis.UI.RadioButtonHelperButton _btnUseExisting;
		private RadioButton _rdoUseExisting;
		private RadioButton _rdoReRecord;
		private Label _lblShiftClips;
		private Button _btnShiftClips;
		private TableLayoutPanel _pnlPlayClip;
		private AudioButtonsControl _audioButtonsControl;
		private ButtonWithoutFocusRectangle _btnPlayClip;
		private PictureBox _problemIcon;
		private Label _lblResolution;
		private TableLayoutPanel _tableProblem;
		private ToolTip toolTip1;
		private TableLayoutPanel tableMaster;
		private TableLayoutPanel _tableButtons;
		private PictureBox _iconShiftClips;
		private ButtonWithoutFocusRectangle _btnEditClip;
		private Label _lblEditingCompleteInstructions;
		private MouseSensitiveIconButton _editSoundFile;
		private MouseSensitiveIconButton _copyPathToClipboard;
	}
}
