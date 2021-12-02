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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._lblProblemSummary = new System.Windows.Forms.Label();
			this._flowLayoutPanelThen = new System.Windows.Forms.FlowLayoutPanel();
			this._lblThen = new System.Windows.Forms.Label();
			this._lblRecordedDate = new System.Windows.Forms.Label();
			this._lblNow = new System.Windows.Forms.Label();
			this._txtThen = new System.Windows.Forms.TextBox();
			this._txtNow = new System.Windows.Forms.TextBox();
			this._btnDelete = new SIL.Windows.Forms.Widgets.BitmapButton();
			this._lblDelete = new System.Windows.Forms.Label();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._audioButtonsControl = new HearThis.UI.AudioButtonsControl();
			this._problemIcon = new HearThis.UI.ExclamationIcon();
			this._chkIgnoreProblem = new HearThis.UI.ImageCheckBox();
			this._nextButton = new HearThis.UI.ArrowButton();
			this.tableLayoutPanel1.SuspendLayout();
			this._flowLayoutPanelThen.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this._lblProblemSummary, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this._flowLayoutPanelThen, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._lblNow, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this._txtThen, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this._txtNow, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this._audioButtonsControl, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._problemIcon, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._btnDelete, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this._chkIgnoreProblem, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this._lblDelete, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this._nextButton, 2, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
			this.tableLayoutPanel1.RowCount = 7;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(571, 216);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _lblProblemSummary
			// 
			this._lblProblemSummary.AutoSize = true;
			this._lblProblemSummary.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblProblemSummary, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblProblemSummary, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblProblemSummary, "ScriptTextHasChangedControl._lblProblemSummary");
			this._lblProblemSummary.Location = new System.Drawing.Point(50, 10);
			this._lblProblemSummary.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
			this._lblProblemSummary.Name = "_lblProblemSummary";
			this._lblProblemSummary.Size = new System.Drawing.Size(382, 25);
			this._lblProblemSummary.TabIndex = 0;
			this._lblProblemSummary.Text = "The text has changed since it was recorded.";
			// 
			// _flowLayoutPanelThen
			// 
			this._flowLayoutPanelThen.Controls.Add(this._lblThen);
			this._flowLayoutPanelThen.Controls.Add(this._lblRecordedDate);
			this._flowLayoutPanelThen.Dock = System.Windows.Forms.DockStyle.Fill;
			this._flowLayoutPanelThen.Location = new System.Drawing.Point(47, 47);
			this._flowLayoutPanelThen.Margin = new System.Windows.Forms.Padding(0);
			this._flowLayoutPanelThen.Name = "_flowLayoutPanelThen";
			this._flowLayoutPanelThen.Size = new System.Drawing.Size(479, 25);
			this._flowLayoutPanelThen.TabIndex = 3;
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
			this._lblThen.Size = new System.Drawing.Size(48, 23);
			this._lblThen.TabIndex = 2;
			this._lblThen.Text = "Then";
			// 
			// _lblRecordedDate
			// 
			this._lblRecordedDate.AutoSize = true;
			this._lblRecordedDate.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblRecordedDate, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblRecordedDate, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblRecordedDate, "ScriptTextHasChangedControl._lblRecordedDate");
			this._lblRecordedDate.Location = new System.Drawing.Point(57, 0);
			this._lblRecordedDate.Name = "_lblRecordedDate";
			this._lblRecordedDate.Size = new System.Drawing.Size(43, 23);
			this._lblRecordedDate.TabIndex = 3;
			this._lblRecordedDate.Text = "({0})";
			// 
			// _lblNow
			// 
			this._lblNow.AutoSize = true;
			this._lblNow.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblNow, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblNow, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblNow, "ScriptTextHasChangedControl._lblNow");
			this._lblNow.Location = new System.Drawing.Point(50, 91);
			this._lblNow.Name = "_lblNow";
			this._lblNow.Size = new System.Drawing.Size(46, 23);
			this._lblNow.TabIndex = 4;
			this._lblNow.Text = "Now";
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
			this._txtThen.Location = new System.Drawing.Point(65, 78);
			this._txtThen.Margin = new System.Windows.Forms.Padding(18, 6, 3, 3);
			this._txtThen.Multiline = true;
			this._txtThen.Name = "_txtThen";
			this._txtThen.ReadOnly = true;
			this._txtThen.Size = new System.Drawing.Size(458, 10);
			this._txtThen.TabIndex = 6;
			// 
			// _txtNow
			// 
			this._txtNow.BackColor = System.Drawing.SystemColors.Window;
			this._txtNow.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._txtNow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._txtNow, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._txtNow, null);
			this.l10NSharpExtender1.SetLocalizingId(this._txtNow, "textBox1");
			this._txtNow.Location = new System.Drawing.Point(65, 120);
			this._txtNow.Margin = new System.Windows.Forms.Padding(18, 6, 3, 3);
			this._txtNow.Multiline = true;
			this._txtNow.Name = "_txtNow";
			this._txtNow.ReadOnly = true;
			this._txtNow.Size = new System.Drawing.Size(458, 10);
			this._txtNow.TabIndex = 7;
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
			this._btnDelete.Font = new System.Drawing.Font("Segoe UI", 12F);
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
			this._btnDelete.Location = new System.Drawing.Point(13, 171);
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
			// 
			// _lblDelete
			// 
			this._lblDelete.AutoSize = true;
			this._lblDelete.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lblDelete, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lblDelete, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lblDelete, "ScriptTextHasChangedControl._lblDelete");
			this._lblDelete.Location = new System.Drawing.Point(50, 174);
			this._lblDelete.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
			this._lblDelete.Name = "_lblDelete";
			this._lblDelete.Padding = new System.Windows.Forms.Padding(0, 1, 0, 3);
			this._lblDelete.Size = new System.Drawing.Size(179, 25);
			this._lblDelete.TabIndex = 11;
			this._lblDelete.Text = "Needs to be re-recorded";
			this._lblDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "";
			// 
			// _audioButtonsControl
			// 
			this._audioButtonsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._audioButtonsControl.AutoSize = true;
			this._audioButtonsControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._audioButtonsControl.BackColor = System.Drawing.Color.Transparent;
			this._audioButtonsControl.ButtonHighlightMode = HearThis.UI.AudioButtonsControl.ButtonHighlightModes.Play;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._audioButtonsControl, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._audioButtonsControl, null);
			this.l10NSharpExtender1.SetLocalizingId(this._audioButtonsControl, "AudioButtonsControl");
			this._audioButtonsControl.Location = new System.Drawing.Point(10, 72);
			this._audioButtonsControl.Margin = new System.Windows.Forms.Padding(0);
			this._audioButtonsControl.MinimumSize = new System.Drawing.Size(0, 42);
			this._audioButtonsControl.Name = "_audioButtonsControl";
			this._audioButtonsControl.RecordingDevice = null;
			this.tableLayoutPanel1.SetRowSpan(this._audioButtonsControl, 3);
			this._audioButtonsControl.ShowNextButton = false;
			this._audioButtonsControl.ShowPlayButton = true;
			this._audioButtonsControl.ShowRecordButton = false;
			this._audioButtonsControl.Size = new System.Drawing.Size(37, 42);
			this._audioButtonsControl.TabIndex = 8;
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
			this.tableLayoutPanel1.SetColumnSpan(this._chkIgnoreProblem, 2);
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
			this._chkIgnoreProblem.Location = new System.Drawing.Point(13, 136);
			this._chkIgnoreProblem.MinimumSize = new System.Drawing.Size(29, 29);
			this._chkIgnoreProblem.Name = "_chkIgnoreProblem";
			this._chkIgnoreProblem.Size = new System.Drawing.Size(510, 29);
			this._chkIgnoreProblem.TabIndex = 5;
			this._chkIgnoreProblem.Text = "Existing recording matches the current block text";
			this._chkIgnoreProblem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this._chkIgnoreProblem.UseVisualStyleBackColor = true;
			this._chkIgnoreProblem.CheckedChanged += new System.EventHandler(this._chkIgnoreProblem_CheckedChanged);
			// 
			// _nextButton
			// 
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
			// ScriptTextHasChangedControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Transparent;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.tableLayoutPanel1);
			this.ForeColor = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this, "ScriptTextHasChangedControl.ScriptChangedControl");
			this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this.Name = "ScriptTextHasChangedControl";
			this.Size = new System.Drawing.Size(571, 216);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this._flowLayoutPanelThen.ResumeLayout(false);
			this._flowLayoutPanelThen.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label _lblProblemSummary;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.FlowLayoutPanel _flowLayoutPanelThen;
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
	}
}
