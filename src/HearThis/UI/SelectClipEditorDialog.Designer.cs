namespace HearThis.UI
{
	partial class SelectClipEditorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectClipEditorDialog));
            this._l10NSharpExtender = new L10NSharp.UI.L10NSharpExtender(this.components);
            this._btnCancel = new System.Windows.Forms.Button();
            this._lblInstructions = new System.Windows.Forms.Label();
            this._btnOk = new System.Windows.Forms.Button();
            this._rdoUseSpecifiedEditor = new System.Windows.Forms.RadioButton();
            this._rdoUseDefaultAssociatedApplication = new System.Windows.Forms.RadioButton();
            this._txtCommandLineArguments = new System.Windows.Forms.TextBox();
            this._chkWAVEditorCommandLineArguments = new System.Windows.Forms.CheckBox();
            this._lblPathToWAVFileEditor = new System.Windows.Forms.Label();
            this._btnOpenFileChooser = new System.Windows.Forms.Button();
            this._lblWAVFileEditingApplicationName = new System.Windows.Forms.Label();
            this._txtEditingApplicationName = new System.Windows.Forms.TextBox();
            this._lblCommandLineArgumentsInstructions = new System.Windows.Forms.Label();
            this._lblWAVEditorCommandLineExample = new System.Windows.Forms.Label();
            this._tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this._l10NSharpExtender)).BeginInit();
            this._tableLayoutPanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // _l10NSharpExtender
            // 
            this._l10NSharpExtender.LocalizationManagerId = null;
            this._l10NSharpExtender.PrefixForNewItems = null;
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._l10NSharpExtender.SetLocalizableToolTip(this._btnCancel, null);
            this._l10NSharpExtender.SetLocalizationComment(this._btnCancel, null);
            this._l10NSharpExtender.SetLocalizingId(this._btnCancel, "Common.Cancel");
            this._btnCancel.Location = new System.Drawing.Point(437, 239);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 5;
            this._btnCancel.Text = "&Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // _lblInstructions
            // 
            this._lblInstructions.AutoSize = true;
            this._tableLayoutPanelMain.SetColumnSpan(this._lblInstructions, 5);
            this._l10NSharpExtender.SetLocalizableToolTip(this._lblInstructions, null);
            this._l10NSharpExtender.SetLocalizationComment(this._lblInstructions, "Param 0: \"WAV\" (file format); Param 1: \"HearThis\" (program name); Param 2: \"Adobe" +
        " Audacity\" (third-party product name); Param 3: Name of \"Check for Problems\" mod" +
        "e (as localized)");
            this._l10NSharpExtender.SetLocalizingId(this._lblInstructions, "SelectClipEditorDialog._lblInstructions");
            this._lblInstructions.Location = new System.Drawing.Point(3, 0);
            this._lblInstructions.Name = "_lblInstructions";
            this._lblInstructions.Size = new System.Drawing.Size(507, 39);
            this._lblInstructions.TabIndex = 6;
            this._lblInstructions.Text = resources.GetString("_lblInstructions.Text");
            // 
            // _btnOk
            // 
            this._btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOk.Enabled = false;
            this._l10NSharpExtender.SetLocalizableToolTip(this._btnOk, null);
            this._l10NSharpExtender.SetLocalizationComment(this._btnOk, null);
            this._l10NSharpExtender.SetLocalizingId(this._btnOk, "Common.OK");
            this._btnOk.Location = new System.Drawing.Point(356, 239);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(75, 23);
            this._btnOk.TabIndex = 7;
            this._btnOk.Text = "OK";
            this._btnOk.UseVisualStyleBackColor = true;
            this._btnOk.Click += new System.EventHandler(this._btnOk_Click);
            // 
            // _rdoUseSpecifiedEditor
            // 
            this._rdoUseSpecifiedEditor.AutoSize = true;
            this._rdoUseSpecifiedEditor.Checked = true;
            this._l10NSharpExtender.SetLocalizableToolTip(this._rdoUseSpecifiedEditor, null);
            this._l10NSharpExtender.SetLocalizationComment(this._rdoUseSpecifiedEditor, "Param 0: \"WAV\" (file format)");
            this._l10NSharpExtender.SetLocalizingId(this._rdoUseSpecifiedEditor, "SelectClipEditorDialog._rdoUseSpecifiedEditor");
            this._rdoUseSpecifiedEditor.Location = new System.Drawing.Point(3, 118);
            this._rdoUseSpecifiedEditor.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this._rdoUseSpecifiedEditor.Name = "_rdoUseSpecifiedEditor";
            this._rdoUseSpecifiedEditor.Size = new System.Drawing.Size(151, 17);
            this._rdoUseSpecifiedEditor.TabIndex = 8;
            this._rdoUseSpecifiedEditor.TabStop = true;
            this._rdoUseSpecifiedEditor.Text = "Use specified {0} file editor";
            this._rdoUseSpecifiedEditor.UseVisualStyleBackColor = true;
            this._rdoUseSpecifiedEditor.CheckedChanged += new System.EventHandler(this.HandleWAVEditoOptionChanged);
            // 
            // _rdoUseDefaultAssociatedApplication
            // 
            this._rdoUseDefaultAssociatedApplication.AutoSize = true;
            this._tableLayoutPanelMain.SetColumnSpan(this._rdoUseDefaultAssociatedApplication, 2);
            this._l10NSharpExtender.SetLocalizableToolTip(this._rdoUseDefaultAssociatedApplication, null);
            this._l10NSharpExtender.SetLocalizationComment(this._rdoUseDefaultAssociatedApplication, null);
            this._l10NSharpExtender.SetLocalizingId(this._rdoUseDefaultAssociatedApplication, "SelectClipEditorDialog._rdoUseDefaultAssociatedApplication");
            this._rdoUseDefaultAssociatedApplication.Location = new System.Drawing.Point(3, 216);
            this._rdoUseDefaultAssociatedApplication.Name = "_rdoUseDefaultAssociatedApplication";
            this._rdoUseDefaultAssociatedApplication.Size = new System.Drawing.Size(133, 17);
            this._rdoUseDefaultAssociatedApplication.TabIndex = 9;
            this._rdoUseDefaultAssociatedApplication.Text = "Use default application";
            this._rdoUseDefaultAssociatedApplication.UseVisualStyleBackColor = true;
            this._rdoUseDefaultAssociatedApplication.CheckedChanged += new System.EventHandler(this.HandleWAVEditoOptionChanged);
            // 
            // _txtCommandLineArguments
            // 
            this._txtCommandLineArguments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tableLayoutPanelMain.SetColumnSpan(this._txtCommandLineArguments, 3);
            this._l10NSharpExtender.SetLocalizableToolTip(this._txtCommandLineArguments, null);
            this._l10NSharpExtender.SetLocalizationComment(this._txtCommandLineArguments, null);
            this._l10NSharpExtender.SetLocalizationPriority(this._txtCommandLineArguments, L10NSharp.LocalizationPriority.NotLocalizable);
            this._l10NSharpExtender.SetLocalizingId(this._txtCommandLineArguments, "SelectClipEditorDialog._txtCommandLineArguments");
            this._txtCommandLineArguments.Location = new System.Drawing.Point(327, 141);
            this._txtCommandLineArguments.Name = "_txtCommandLineArguments";
            this._txtCommandLineArguments.Size = new System.Drawing.Size(185, 20);
            this._txtCommandLineArguments.TabIndex = 11;
            this._txtCommandLineArguments.Visible = false;
            // 
            // _chkWAVEditorCommandLineArguments
            // 
            this._chkWAVEditorCommandLineArguments.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._chkWAVEditorCommandLineArguments.AutoSize = true;
            this._tableLayoutPanelMain.SetColumnSpan(this._chkWAVEditorCommandLineArguments, 3);
            this._chkWAVEditorCommandLineArguments.Enabled = false;
            this._l10NSharpExtender.SetLocalizableToolTip(this._chkWAVEditorCommandLineArguments, null);
            this._l10NSharpExtender.SetLocalizationComment(this._chkWAVEditorCommandLineArguments, null);
            this._l10NSharpExtender.SetLocalizationPriority(this._chkWAVEditorCommandLineArguments, L10NSharp.LocalizationPriority.NotLocalizable);
            this._l10NSharpExtender.SetLocalizingId(this._chkWAVEditorCommandLineArguments, "SelectClipEditorDialog._chkWAVEditorCommandLineArguments");
            this._chkWAVEditorCommandLineArguments.Location = new System.Drawing.Point(327, 118);
            this._chkWAVEditorCommandLineArguments.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this._chkWAVEditorCommandLineArguments.Name = "_chkWAVEditorCommandLineArguments";
            this._chkWAVEditorCommandLineArguments.Size = new System.Drawing.Size(185, 17);
            this._chkWAVEditorCommandLineArguments.TabIndex = 12;
            this._chkWAVEditorCommandLineArguments.Text = "Command-line arguments required";
            this._chkWAVEditorCommandLineArguments.UseVisualStyleBackColor = true;
            this._chkWAVEditorCommandLineArguments.CheckedChanged += new System.EventHandler(this._chkWAVEditorCommandLineArguments_CheckedChanged);
            // 
            // _lblPathToWAVFileEditor
            // 
            this._lblPathToWAVFileEditor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblPathToWAVFileEditor.AutoEllipsis = true;
            this._lblPathToWAVFileEditor.AutoSize = true;
            this._tableLayoutPanelMain.SetColumnSpan(this._lblPathToWAVFileEditor, 2);
            this._l10NSharpExtender.SetLocalizableToolTip(this._lblPathToWAVFileEditor, null);
            this._l10NSharpExtender.SetLocalizationComment(this._lblPathToWAVFileEditor, null);
            this._l10NSharpExtender.SetLocalizationPriority(this._lblPathToWAVFileEditor, L10NSharp.LocalizationPriority.NotLocalizable);
            this._l10NSharpExtender.SetLocalizingId(this._lblPathToWAVFileEditor, "SelectClipEditorDialog._lblPathToWAVFileEditor");
            this._lblPathToWAVFileEditor.Location = new System.Drawing.Point(20, 144);
            this._lblPathToWAVFileEditor.Margin = new System.Windows.Forms.Padding(20, 0, 8, 0);
            this._lblPathToWAVFileEditor.Name = "_lblPathToWAVFileEditor";
            this._lblPathToWAVFileEditor.Size = new System.Drawing.Size(14, 13);
            this._lblPathToWAVFileEditor.TabIndex = 13;
            this._lblPathToWAVFileEditor.Text = "#";
            this._lblPathToWAVFileEditor.Visible = false;
            // 
            // _btnOpenFileChooser
            // 
            this._btnOpenFileChooser.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._btnOpenFileChooser.Image = global::HearThis.Properties.Resources.ellipsis;
            this._l10NSharpExtender.SetLocalizableToolTip(this._btnOpenFileChooser, null);
            this._l10NSharpExtender.SetLocalizationComment(this._btnOpenFileChooser, null);
            this._l10NSharpExtender.SetLocalizationPriority(this._btnOpenFileChooser, L10NSharp.LocalizationPriority.NotLocalizable);
            this._l10NSharpExtender.SetLocalizingId(this._btnOpenFileChooser, "SelectClipEditorDialog._btnOpenFileChooser");
            this._btnOpenFileChooser.Location = new System.Drawing.Point(297, 114);
            this._btnOpenFileChooser.Name = "_btnOpenFileChooser";
            this._btnOpenFileChooser.Size = new System.Drawing.Size(24, 20);
            this._btnOpenFileChooser.TabIndex = 14;
            this._btnOpenFileChooser.UseVisualStyleBackColor = true;
            this._btnOpenFileChooser.Click += new System.EventHandler(this._btnOpenFileChooser_Click);
            // 
            // _lblWAVFileEditingApplicationName
            // 
            this._lblWAVFileEditingApplicationName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblWAVFileEditingApplicationName.AutoSize = true;
            this._tableLayoutPanelMain.SetColumnSpan(this._lblWAVFileEditingApplicationName, 3);
            this._l10NSharpExtender.SetLocalizableToolTip(this._lblWAVFileEditingApplicationName, null);
            this._l10NSharpExtender.SetLocalizationComment(this._lblWAVFileEditingApplicationName, null);
            this._l10NSharpExtender.SetLocalizingId(this._lblWAVFileEditingApplicationName, "SelectClipEditorDialog._lblWAVFileEditingApplicationName");
            this._lblWAVFileEditingApplicationName.Location = new System.Drawing.Point(327, 170);
            this._lblWAVFileEditingApplicationName.Margin = new System.Windows.Forms.Padding(3, 6, 3, 4);
            this._lblWAVFileEditingApplicationName.Name = "_lblWAVFileEditingApplicationName";
            this._lblWAVFileEditingApplicationName.Size = new System.Drawing.Size(125, 13);
            this._lblWAVFileEditingApplicationName.TabIndex = 15;
            this._lblWAVFileEditingApplicationName.Text = "Editing application name:";
            this._lblWAVFileEditingApplicationName.Visible = false;
            // 
            // _txtEditingApplicationName
            // 
            this._txtEditingApplicationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tableLayoutPanelMain.SetColumnSpan(this._txtEditingApplicationName, 3);
            this._l10NSharpExtender.SetLocalizableToolTip(this._txtEditingApplicationName, null);
            this._l10NSharpExtender.SetLocalizationComment(this._txtEditingApplicationName, null);
            this._l10NSharpExtender.SetLocalizationPriority(this._txtEditingApplicationName, L10NSharp.LocalizationPriority.NotLocalizable);
            this._l10NSharpExtender.SetLocalizingId(this._txtEditingApplicationName, "SelectClipEditorDialog._txtEditingApplicationName");
            this._txtEditingApplicationName.Location = new System.Drawing.Point(327, 190);
            this._txtEditingApplicationName.Name = "_txtEditingApplicationName";
            this._txtEditingApplicationName.Size = new System.Drawing.Size(185, 20);
            this._txtEditingApplicationName.TabIndex = 16;
            this._txtEditingApplicationName.Visible = false;
            // 
            // _lblCommandLineArgumentsInstructions
            // 
            this._lblCommandLineArgumentsInstructions.AutoSize = true;
            this._tableLayoutPanelMain.SetColumnSpan(this._lblCommandLineArgumentsInstructions, 5);
            this._l10NSharpExtender.SetLocalizableToolTip(this._lblCommandLineArgumentsInstructions, null);
            this._l10NSharpExtender.SetLocalizationComment(this._lblCommandLineArgumentsInstructions, "Param 0: \"HearThis\" (product name); Param 1: \"{path}\" (literal text)");
            this._l10NSharpExtender.SetLocalizingId(this._lblCommandLineArgumentsInstructions, "SelectClipEditorDialog._lblCommandLineArgumentsInstructions");
            this._lblCommandLineArgumentsInstructions.Location = new System.Drawing.Point(3, 45);
            this._lblCommandLineArgumentsInstructions.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this._lblCommandLineArgumentsInstructions.Name = "_lblCommandLineArgumentsInstructions";
            this._lblCommandLineArgumentsInstructions.Size = new System.Drawing.Size(503, 52);
            this._lblCommandLineArgumentsInstructions.TabIndex = 17;
            this._lblCommandLineArgumentsInstructions.Text = resources.GetString("_lblCommandLineArgumentsInstructions.Text");
            this._lblCommandLineArgumentsInstructions.Visible = false;
            // 
            // _lblWAVEditorCommandLineExample
            // 
            this._lblWAVEditorCommandLineExample.AutoSize = true;
            this._lblWAVEditorCommandLineExample.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._l10NSharpExtender.SetLocalizableToolTip(this._lblWAVEditorCommandLineExample, null);
            this._l10NSharpExtender.SetLocalizationComment(this._lblWAVEditorCommandLineExample, null);
            this._l10NSharpExtender.SetLocalizationPriority(this._lblWAVEditorCommandLineExample, L10NSharp.LocalizationPriority.NotLocalizable);
            this._l10NSharpExtender.SetLocalizingId(this._lblWAVEditorCommandLineExample, "SelectClipEditorDialog._lblWAVEditorCommandLineExample");
            this._lblWAVEditorCommandLineExample.Location = new System.Drawing.Point(8, 103);
            this._lblWAVEditorCommandLineExample.Margin = new System.Windows.Forms.Padding(8, 6, 3, 0);
            this._lblWAVEditorCommandLineExample.Name = "_lblWAVEditorCommandLineExample";
            this._lblWAVEditorCommandLineExample.Size = new System.Drawing.Size(58, 13);
            this._lblWAVEditorCommandLineExample.TabIndex = 18;
            this._lblWAVEditorCommandLineExample.Text = "-i {0} -o {0}";
            this._lblWAVEditorCommandLineExample.Visible = false;
            // 
            // _tableLayoutPanelMain
            // 
            this._tableLayoutPanelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tableLayoutPanelMain.ColumnCount = 5;
            this._tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanelMain.Controls.Add(this._lblInstructions, 0, 0);
            this._tableLayoutPanelMain.Controls.Add(this._rdoUseSpecifiedEditor, 0, 4);
            this._tableLayoutPanelMain.Controls.Add(this._rdoUseDefaultAssociatedApplication, 0, 8);
            this._tableLayoutPanelMain.Controls.Add(this._txtCommandLineArguments, 2, 5);
            this._tableLayoutPanelMain.Controls.Add(this._chkWAVEditorCommandLineArguments, 2, 4);
            this._tableLayoutPanelMain.Controls.Add(this._btnOpenFileChooser, 1, 4);
            this._tableLayoutPanelMain.Controls.Add(this._btnCancel, 4, 9);
            this._tableLayoutPanelMain.Controls.Add(this._btnOk, 3, 9);
            this._tableLayoutPanelMain.Controls.Add(this._txtEditingApplicationName, 2, 7);
            this._tableLayoutPanelMain.Controls.Add(this._lblWAVFileEditingApplicationName, 2, 6);
            this._tableLayoutPanelMain.Controls.Add(this._lblCommandLineArgumentsInstructions, 0, 1);
            this._tableLayoutPanelMain.Controls.Add(this._lblWAVEditorCommandLineExample, 0, 2);
            this._tableLayoutPanelMain.Controls.Add(this._lblPathToWAVFileEditor, 0, 5);
            this._tableLayoutPanelMain.Location = new System.Drawing.Point(12, 12);
            this._tableLayoutPanelMain.Name = "_tableLayoutPanelMain";
            this._tableLayoutPanelMain.RowCount = 10;
            this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanelMain.Size = new System.Drawing.Size(515, 265);
            this._tableLayoutPanelMain.TabIndex = 6;
            // 
            // SelectClipEditorDialog
            // 
            this.AcceptButton = this._btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(539, 289);
            this.Controls.Add(this._tableLayoutPanelMain);
            this._l10NSharpExtender.SetLocalizableToolTip(this, null);
            this._l10NSharpExtender.SetLocalizationComment(this, null);
            this._l10NSharpExtender.SetLocalizationPriority(this, L10NSharp.LocalizationPriority.NotLocalizable);
            this._l10NSharpExtender.SetLocalizingId(this, "SelectClipEditorDialog.WindowTitle");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(477, 219);
            this.Name = "SelectClipEditorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Clip Editor";
            ((System.ComponentModel.ISupportInitialize)(this._l10NSharpExtender)).EndInit();
            this._tableLayoutPanelMain.ResumeLayout(false);
            this._tableLayoutPanelMain.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
		private L10NSharp.UI.L10NSharpExtender _l10NSharpExtender;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelMain;
		private System.Windows.Forms.Label _lblInstructions;
		private System.Windows.Forms.Button _btnOk;
		private System.Windows.Forms.RadioButton _rdoUseSpecifiedEditor;
		private System.Windows.Forms.RadioButton _rdoUseDefaultAssociatedApplication;
		private System.Windows.Forms.TextBox _txtCommandLineArguments;
		private System.Windows.Forms.CheckBox _chkWAVEditorCommandLineArguments;
		private System.Windows.Forms.Label _lblPathToWAVFileEditor;
		private System.Windows.Forms.Button _btnOpenFileChooser;
		private System.Windows.Forms.Label _lblWAVFileEditingApplicationName;
		private System.Windows.Forms.TextBox _txtEditingApplicationName;
		private System.Windows.Forms.Label _lblCommandLineArgumentsInstructions;
		private System.Windows.Forms.Label _lblWAVEditorCommandLineExample;
	}
}
