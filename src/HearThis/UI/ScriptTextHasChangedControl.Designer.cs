using System;

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
			System.Drawing.Imaging.ImageAttributes imageAttributes1 = new System.Drawing.Imaging.ImageAttributes();
			System.Drawing.Imaging.ImageAttributes imageAttributes2 = new System.Drawing.Imaging.ImageAttributes();
			System.Drawing.Imaging.ImageAttributes imageAttributes3 = new System.Drawing.Imaging.ImageAttributes();
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
			this._masterTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._lblProblemSummary = new System.Windows.Forms.Label();
			this._btnDelete = new SIL.Windows.Forms.Widgets.BitmapButton();
			this._lblDelete = new System.Windows.Forms.Label();
			this._lblUndoDelete = new System.Windows.Forms.Label();
			this._btnUndoDelete = new SIL.Windows.Forms.Widgets.BitmapButton();
			this._btnShiftClips = new SIL.Windows.Forms.Widgets.BitmapButton();
			this._lblShiftClips = new System.Windows.Forms.Label();
			this._tableThenVsNow = new System.Windows.Forms.TableLayoutPanel();
			this._lblNow = new System.Windows.Forms.Label();
			this._txtNow = new System.Windows.Forms.TextBox();
			this._txtThen = new System.Windows.Forms.TextBox();
			this._panelThen = new System.Windows.Forms.FlowLayoutPanel();
			this._lblThen = new System.Windows.Forms.Label();
			this._lblRecordedDate = new System.Windows.Forms.Label();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._problemIcon = new HearThis.UI.ExclamationIcon();
			this._chkIgnoreProblem = new HearThis.UI.ImageCheckBox();
			this._nextButton = new HearThis.UI.ArrowButton();
			this._audioButtonsControl = new HearThis.UI.AudioButtonsControl();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._masterTableLayoutPanel.SuspendLayout();
			this._tableThenVsNow.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			this._panelThen.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// _masterTableLayoutPanel
			// 
			this._masterTableLayoutPanel.ColumnCount = 3;
			this._masterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._masterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._masterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._masterTableLayoutPanel.Controls.Add(this._lblProblemSummary, 1, 0);
			this._masterTableLayoutPanel.Controls.Add(this._problemIcon, 0, 0);
			this._masterTableLayoutPanel.Controls.Add(this._btnDelete, 0, 4);
			this._masterTableLayoutPanel.Controls.Add(this._chkIgnoreProblem, 0, 3);
			this._masterTableLayoutPanel.Controls.Add(this._lblDelete, 1, 4);
			this._masterTableLayoutPanel.Controls.Add(this._nextButton, 2, 0);
			this._masterTableLayoutPanel.Controls.Add(this._lblUndoDelete, 2, 6);
			this._masterTableLayoutPanel.Controls.Add(this._btnUndoDelete, 1, 6);
			this._masterTableLayoutPanel.Controls.Add(this._btnShiftClips, 0, 5);
			this._masterTableLayoutPanel.Controls.Add(this._lblShiftClips, 1, 5);
			this._masterTableLayoutPanel.Controls.Add(this._tableThenVsNow, 0, 1);
			this._masterTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._masterTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._masterTableLayoutPanel.Name = "_masterTableLayoutPanel";
			this._masterTableLayoutPanel.Padding = new System.Windows.Forms.Padding(10);
			this._masterTableLayoutPanel.RowCount = 7;
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._masterTableLayoutPanel.Size = new System.Drawing.Size(571, 352);
			this._masterTableLayoutPanel.TabIndex = 0;
			this._masterTableLayoutPanel.Resize += new System.EventHandler(this._masterTableLayoutPanel_Resize);
			// 
			// _lblProblemSummary
			// 
			this._lblProblemSummary.AutoSize = true;
			this._lblProblemSummary.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblProblemSummary, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblProblemSummary, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblProblemSummary, "ScriptTextHasChangedControl._lblProblemSummary");
			this._lblProblemSummary.Location = new System.Drawing.Point(48, 10);
			this._lblProblemSummary.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
			this._lblProblemSummary.Name = "_lblProblemSummary";
			this._lblProblemSummary.Size = new System.Drawing.Size(307, 21);
			this._lblProblemSummary.TabIndex = 0;
			this._lblProblemSummary.Text = "The text has changed since it was recorded.";
			// 
			// _btnDelete
			// 
			this._btnDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._btnDelete.BorderColor = System.Drawing.Color.DarkGray;
			this._btnDelete.DisabledTextColor = System.Drawing.Color.DimGray;
			this._btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._btnDelete.FlatAppearance.BorderSize = 0;
			this._btnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveCaption;
			this._btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this._btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnDelete.FocusRectangleEnabled = true;
			this._btnDelete.ForeColor = System.Drawing.Color.DarkGray;
			this._btnDelete.Image = global::HearThis.Properties.Resources.BottomToolbar_Delete;
			this._btnDelete.ImageAttributes = imageAttributes1;
			this._btnDelete.ImageBorderColor = System.Drawing.Color.Transparent;
			this._btnDelete.ImageBorderEnabled = false;
			this._btnDelete.ImageDropShadow = true;
			this._btnDelete.ImageFocused = null;
			this._btnDelete.ImageInactive = null;
			this._btnDelete.ImageMouseOver = global::HearThis.Properties.Resources.Delete_MouseOver;
			this._btnDelete.ImageNormal = global::HearThis.Properties.Resources.BottomToolbar_Delete;
			this._btnDelete.ImagePressed = global::HearThis.Properties.Resources.Delete_MouseOver;
			this._btnDelete.InnerBorderColor = System.Drawing.Color.Transparent;
			this._btnDelete.InnerBorderColor_Focus = System.Drawing.Color.DarkGray;
			this._btnDelete.InnerBorderColor_MouseOver = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnDelete, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnDelete, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._btnDelete, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._btnDelete, "ScriptTextHasChangedControl._btnDelete");
			this._btnDelete.Location = new System.Drawing.Point(13, 245);
			this._btnDelete.Name = "_btnDelete";
			this._btnDelete.OffsetPressedContent = true;
			this._btnDelete.Size = new System.Drawing.Size(29, 31);
			this._btnDelete.StretchImage = true;
			this._btnDelete.TabIndex = 9;
			this._btnDelete.TextDropShadow = false;
			this._btnDelete.TextWordWrap = false;
			this._btnDelete.UseVisualStyleBackColor = false;
			this._btnDelete.Visible = false;
			this._btnDelete.Click += new System.EventHandler(this._btnDelete_Click);
			this._btnDelete.MouseEnter += new System.EventHandler(this.BitmapButtonMouseEnter);
			this._btnDelete.MouseLeave += new System.EventHandler(this.BitmapButtonMouseLeave);
			// 
			// _lblDelete
			// 
			this._lblDelete.AutoSize = true;
			this._lblDelete.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblDelete, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblDelete, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblDelete, "ScriptTextHasChangedControl._lblDelete");
			this._lblDelete.Location = new System.Drawing.Point(48, 248);
			this._lblDelete.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
			this._lblDelete.Name = "_lblDelete";
			this._lblDelete.Padding = new System.Windows.Forms.Padding(0, 1, 0, 3);
			this._lblDelete.Size = new System.Drawing.Size(179, 25);
			this._lblDelete.TabIndex = 11;
			this._lblDelete.Text = "Needs to be re-recorded";
			this._lblDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _lblUndoDelete
			// 
			this._lblUndoDelete.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._lblUndoDelete.AutoSize = true;
			this._lblUndoDelete.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblUndoDelete, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblUndoDelete, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblUndoDelete, "ScriptTextHasChangedControl._lblUndoDelete");
			this._lblUndoDelete.Location = new System.Drawing.Point(472, 319);
			this._lblUndoDelete.Name = "_lblUndoDelete";
			this._lblUndoDelete.Size = new System.Drawing.Size(86, 19);
			this._lblUndoDelete.TabIndex = 14;
			this._lblUndoDelete.Text = "Undo Delete";
			this._lblUndoDelete.Visible = false;
			// 
			// _btnUndoDelete
			// 
			this._btnUndoDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._btnUndoDelete.AutoSize = true;
			this._btnUndoDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._btnUndoDelete.BorderColor = System.Drawing.Color.DarkGray;
			this._btnUndoDelete.DisabledTextColor = System.Drawing.Color.DimGray;
			this._btnUndoDelete.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._btnUndoDelete.FlatAppearance.BorderSize = 0;
			this._btnUndoDelete.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveCaption;
			this._btnUndoDelete.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this._btnUndoDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnUndoDelete.FocusRectangleEnabled = true;
			this._btnUndoDelete.ForeColor = System.Drawing.Color.DarkGray;
			this._btnUndoDelete.Image = null;
			this._btnUndoDelete.ImageAttributes = imageAttributes2;
			this._btnUndoDelete.ImageBorderColor = System.Drawing.Color.Transparent;
			this._btnUndoDelete.ImageBorderEnabled = false;
			this._btnUndoDelete.ImageDropShadow = true;
			this._btnUndoDelete.ImageFocused = null;
			this._btnUndoDelete.ImageInactive = null;
			this._btnUndoDelete.ImageMouseOver = global::HearThis.Properties.Resources.undo_MouseOver;
			this._btnUndoDelete.ImageNormal = global::HearThis.Properties.Resources.undo;
			this._btnUndoDelete.ImagePressed = global::HearThis.Properties.Resources.undo_MouseOver;
			this._btnUndoDelete.InnerBorderColor = System.Drawing.Color.Transparent;
			this._btnUndoDelete.InnerBorderColor_Focus = System.Drawing.Color.DarkGray;
			this._btnUndoDelete.InnerBorderColor_MouseOver = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnUndoDelete, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnUndoDelete, null);
			this.l10NSharpExtender1.SetLocalizingId(this._btnUndoDelete, "ScriptTextHasChangedControl._btnUndoDelete");
			this._btnUndoDelete.Location = new System.Drawing.Point(446, 319);
			this._btnUndoDelete.Name = "_btnUndoDelete";
			this._btnUndoDelete.OffsetPressedContent = true;
			this._btnUndoDelete.Size = new System.Drawing.Size(20, 20);
			this._btnUndoDelete.StretchImage = true;
			this._btnUndoDelete.TabIndex = 13;
			this._btnUndoDelete.TextDropShadow = false;
			this._btnUndoDelete.TextWordWrap = false;
			this._btnUndoDelete.UseVisualStyleBackColor = false;
			this._btnUndoDelete.Visible = false;
			this._btnUndoDelete.Click += new System.EventHandler(this._btnUndoDelete_Click);
			// 
			// _btnShiftClips
			// 
			this._btnShiftClips.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._btnShiftClips.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._btnShiftClips.BorderColor = System.Drawing.Color.DarkGray;
			this._btnShiftClips.DisabledTextColor = System.Drawing.Color.DimGray;
			this._btnShiftClips.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._btnShiftClips.FlatAppearance.BorderSize = 0;
			this._btnShiftClips.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveCaption;
			this._btnShiftClips.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this._btnShiftClips.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnShiftClips.FocusRectangleEnabled = true;
			this._btnShiftClips.ForeColor = System.Drawing.Color.DarkGray;
			this._btnShiftClips.Image = global::HearThis.Properties.Resources.BottomToolbar_Delete;
			this._btnShiftClips.ImageAttributes = imageAttributes3;
			this._btnShiftClips.ImageBorderColor = System.Drawing.Color.Transparent;
			this._btnShiftClips.ImageBorderEnabled = false;
			this._btnShiftClips.ImageDropShadow = true;
			this._btnShiftClips.ImageFocused = null;
			this._btnShiftClips.ImageInactive = null;
			this._btnShiftClips.ImageMouseOver = global::HearThis.Properties.Resources.shift_clips24_MouseOver;
			this._btnShiftClips.ImageNormal = global::HearThis.Properties.Resources.shift_clips24;
			this._btnShiftClips.ImagePressed = global::HearThis.Properties.Resources.shift_clips24_MouseOver;
			this._btnShiftClips.InnerBorderColor = System.Drawing.Color.Transparent;
			this._btnShiftClips.InnerBorderColor_Focus = System.Drawing.Color.DarkGray;
			this._btnShiftClips.InnerBorderColor_MouseOver = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._btnShiftClips, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._btnShiftClips, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._btnShiftClips, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._btnShiftClips, "ScriptTextHasChangedControl._btnShiftClips");
			this._btnShiftClips.Location = new System.Drawing.Point(13, 282);
			this._btnShiftClips.Name = "_btnShiftClips";
			this._btnShiftClips.OffsetPressedContent = true;
			this._btnShiftClips.Size = new System.Drawing.Size(29, 31);
			this._btnShiftClips.StretchImage = true;
			this._btnShiftClips.TabIndex = 15;
			this._btnShiftClips.TextDropShadow = false;
			this._btnShiftClips.TextWordWrap = false;
			this._btnShiftClips.UseVisualStyleBackColor = false;
			this._btnShiftClips.Visible = false;
			this._btnShiftClips.Click += new System.EventHandler(this._btnShiftClips_Click);
			this._btnShiftClips.MouseEnter += new System.EventHandler(this.BitmapButtonMouseEnter);
			this._btnShiftClips.MouseLeave += new System.EventHandler(this.BitmapButtonMouseLeave);
			// 
			// _lblShiftClips
			// 
			this._lblShiftClips.AutoSize = true;
			this._lblShiftClips.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblShiftClips, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblShiftClips, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblShiftClips, "ScriptTextHasChangedControl._lblShiftClips");
			this._lblShiftClips.Location = new System.Drawing.Point(48, 285);
			this._lblShiftClips.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
			this._lblShiftClips.Name = "_lblShiftClips";
			this._lblShiftClips.Padding = new System.Windows.Forms.Padding(1, 0, 3, 0);
			this._lblShiftClips.Size = new System.Drawing.Size(266, 21);
			this._lblShiftClips.TabIndex = 16;
			this._lblShiftClips.Text = "Shift clips to align to current blocks...";
			this._lblShiftClips.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _tableThenVsNow
			// 
			this._tableThenVsNow.AutoSize = true;
			this._tableThenVsNow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableThenVsNow.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this._tableThenVsNow.ColumnCount = 2;
			this._masterTableLayoutPanel.SetColumnSpan(this._tableThenVsNow, 3);
			this._tableThenVsNow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableThenVsNow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableThenVsNow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableThenVsNow.Controls.Add(this._lblNow, 1, 0);
			this._tableThenVsNow.Controls.Add(this._txtNow, 1, 1);
			this._tableThenVsNow.Controls.Add(tableLayoutPanel1, 0, 1);
			this._tableThenVsNow.Controls.Add(this._panelThen, 0, 0);
			this._tableThenVsNow.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableThenVsNow.Location = new System.Drawing.Point(13, 50);
			this._tableThenVsNow.Name = "_tableThenVsNow";
			this._tableThenVsNow.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
			this._tableThenVsNow.RowCount = 2;
			this._tableThenVsNow.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableThenVsNow.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableThenVsNow.Size = new System.Drawing.Size(545, 153);
			this._tableThenVsNow.TabIndex = 19;
			// 
			// _lblNow
			// 
			this._lblNow.AutoSize = true;
			this._lblNow.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblNow, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblNow, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblNow, "ScriptTextHasChangedControl._lblNow");
			this._lblNow.Location = new System.Drawing.Point(276, 1);
			this._lblNow.Name = "_lblNow";
			this._lblNow.Size = new System.Drawing.Size(46, 23);
			this._lblNow.TabIndex = 4;
			this._lblNow.Text = "Now";
			// 
			// _txtNow
			// 
			this._txtNow.BackColor = System.Drawing.SystemColors.Window;
			this._txtNow.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._txtNow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._txtNow, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._txtNow, null);
			this.l10NSharpExtender1.SetLocalizingId(this._txtNow, "textBox1");
			this._txtNow.Location = new System.Drawing.Point(279, 31);
			this._txtNow.Margin = new System.Windows.Forms.Padding(6, 6, 3, 3);
			this._txtNow.Multiline = true;
			this._txtNow.Name = "_txtNow";
			this._txtNow.ReadOnly = true;
			this._txtNow.Size = new System.Drawing.Size(262, 119);
			this._txtNow.TabIndex = 7;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(this._txtThen, 1, 0);
			tableLayoutPanel1.Controls.Add(this._audioButtonsControl, 0, 0);
			tableLayoutPanel1.Location = new System.Drawing.Point(1, 25);
			tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 1;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Size = new System.Drawing.Size(271, 128);
			tableLayoutPanel1.TabIndex = 20;
			// 
			// _txtThen
			// 
			this._txtThen.BackColor = System.Drawing.SystemColors.Window;
			this._txtThen.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._txtThen.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._txtThen, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._txtThen, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._txtThen, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._txtThen, "ScriptTextHasChangedControl._txtThen");
			this._txtThen.Location = new System.Drawing.Point(43, 6);
			this._txtThen.Margin = new System.Windows.Forms.Padding(6, 6, 6, 3);
			this._txtThen.Multiline = true;
			this._txtThen.Name = "_txtThen";
			this._txtThen.ReadOnly = true;
			this._txtThen.Size = new System.Drawing.Size(222, 119);
			this._txtThen.TabIndex = 6;
			// 
			// _panelThen
			// 
			this._panelThen.AutoSize = true;
			this._panelThen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelThen.Controls.Add(this._lblThen);
			this._panelThen.Controls.Add(this._lblRecordedDate);
			this._panelThen.Location = new System.Drawing.Point(1, 1);
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
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "";
			// 
			// _problemIcon
			// 
			this._problemIcon.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._problemIcon, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._problemIcon, null);
			this.l10NSharpExtender1.SetLocalizingId(this._problemIcon, "ScriptTextHasChangedControl._problemIcon");
			this._problemIcon.Location = new System.Drawing.Point(13, 10);
			this._problemIcon.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this._problemIcon.Name = "_problemIcon";
			this._problemIcon.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
			this._problemIcon.Size = new System.Drawing.Size(29, 31);
			this._problemIcon.TabIndex = 10;
			// 
			// _chkIgnoreProblem
			// 
			this._chkIgnoreProblem.AutoSize = true;
			this._chkIgnoreProblem.BoxBackColor = System.Drawing.Color.Transparent;
			this._masterTableLayoutPanel.SetColumnSpan(this._chkIgnoreProblem, 2);
			this._chkIgnoreProblem.Dock = System.Windows.Forms.DockStyle.Fill;
			this._chkIgnoreProblem.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._chkIgnoreProblem.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveCaption;
			this._chkIgnoreProblem.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this._chkIgnoreProblem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._chkIgnoreProblem.Font = new System.Drawing.Font("Segoe UI", 12F);
			this._chkIgnoreProblem.GapBetweenBoxAndText = 8;
			this._chkIgnoreProblem.ImageCheckedFocused = global::HearThis.Properties.Resources.green_check;
			this._chkIgnoreProblem.ImageCheckedInactive = global::HearThis.Properties.Resources.green_check;
			this._chkIgnoreProblem.ImageCheckedMouseOver = global::HearThis.Properties.Resources.green_check;
			this._chkIgnoreProblem.ImageCheckedNormal = global::HearThis.Properties.Resources.green_check;
			this._chkIgnoreProblem.ImagePadding = new System.Windows.Forms.Padding(2);
			this._chkIgnoreProblem.InnerBorderColor = System.Drawing.Color.Transparent;
			this._chkIgnoreProblem.InnerBorderColorFocused = System.Drawing.Color.DarkGray;
			this._chkIgnoreProblem.InnerBorderColorMouseOver = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chkIgnoreProblem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chkIgnoreProblem, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._chkIgnoreProblem, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._chkIgnoreProblem, "ScriptTextHasChangedControl._chkIgnoreProblem");
			this._chkIgnoreProblem.Location = new System.Drawing.Point(13, 210);
			this._chkIgnoreProblem.MinimumSize = new System.Drawing.Size(29, 29);
			this._chkIgnoreProblem.Name = "_chkIgnoreProblem";
			this._chkIgnoreProblem.Size = new System.Drawing.Size(453, 29);
			this._chkIgnoreProblem.TabIndex = 5;
			this._chkIgnoreProblem.Text = "Existing recording matches the current block text";
			this._chkIgnoreProblem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this._chkIgnoreProblem.UseVisualStyleBackColor = true;
			this._chkIgnoreProblem.CheckedChanged += new System.EventHandler(this._chkIgnoreProblem_CheckedChanged);
			// 
			// _nextButton
			// 
			this._nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._nextButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._nextButton.CancellableMouseDownCall = null;
			this._nextButton.IsDefault = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._nextButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._nextButton, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._nextButton, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._nextButton, "ScriptTextHasChangedControl._nextButton");
			this._nextButton.Location = new System.Drawing.Point(529, 13);
			this._nextButton.Name = "_nextButton";
			this._nextButton.Size = new System.Drawing.Size(29, 31);
			this._nextButton.State = HearThis.UI.BtnState.Normal;
			this._nextButton.TabIndex = 12;
			this._nextButton.Click += new System.EventHandler(this.OnNextButton);
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
			this._audioButtonsControl.RecordingDevice = null;
			this._audioButtonsControl.ShowNextButton = false;
			this._audioButtonsControl.ShowPlayButton = true;
			this._audioButtonsControl.ShowRecordButton = false;
			this._audioButtonsControl.Size = new System.Drawing.Size(37, 42);
			this._audioButtonsControl.TabIndex = 8;
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
			this.Size = new System.Drawing.Size(571, 352);
			this._masterTableLayoutPanel.ResumeLayout(false);
			this._masterTableLayoutPanel.PerformLayout();
			this._tableThenVsNow.ResumeLayout(false);
			this._tableThenVsNow.PerformLayout();
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			this._panelThen.ResumeLayout(false);
			this._panelThen.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.TableLayoutPanel _masterTableLayoutPanel;
		private System.Windows.Forms.Label _lblProblemSummary;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label _lblThen;
		private System.Windows.Forms.Label _lblRecordedDate;
		private System.Windows.Forms.Label _lblNow;
		private HearThis.UI.ImageCheckBox _chkIgnoreProblem;
		private System.Windows.Forms.TextBox _txtThen;
		private System.Windows.Forms.TextBox _txtNow;
		private AudioButtonsControl _audioButtonsControl;
		private SIL.Windows.Forms.Widgets.BitmapButton _btnDelete;
		private HearThis.UI.ExclamationIcon _problemIcon;
		private System.Windows.Forms.Label _lblDelete;
		private ArrowButton _nextButton;
		private SIL.Windows.Forms.Widgets.BitmapButton _btnUndoDelete;
		private System.Windows.Forms.Label _lblUndoDelete;
		private SIL.Windows.Forms.Widgets.BitmapButton _btnShiftClips;
		private System.Windows.Forms.Label _lblShiftClips;
		private System.Windows.Forms.FlowLayoutPanel _panelThen;
		private System.Windows.Forms.TableLayoutPanel _tableThenVsNow;
	}
}
