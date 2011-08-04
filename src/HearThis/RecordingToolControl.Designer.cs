namespace HearThis
{
    partial class RecordingToolControl
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
            this._bookFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._bookLabel = new System.Windows.Forms.Label();
            this._chapterLabel = new System.Windows.Forms.Label();
            this._chapterFlow = new System.Windows.Forms.FlowLayoutPanel();
            this._verseSlider = new System.Windows.Forms.TrackBar();
            this._segmentLabel = new System.Windows.Forms.Label();
            this._maxVerseLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this._saveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._generateFiles = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this._recordAndPlayControl = new HearThis.RecordAndPlayControl();
            this._downButton = new HearThis.ImageButton();
            this._upButton = new HearThis.ImageButton();
            this._scriptControl = new HearThis.ScriptControl();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._verseSlider)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bookFlow
            // 
            this._bookFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._bookFlow.Location = new System.Drawing.Point(3, 23);
            this._bookFlow.Margin = new System.Windows.Forms.Padding(3, 3, 3, 13);
            this._bookFlow.Name = "_bookFlow";
            this._bookFlow.Size = new System.Drawing.Size(661, 44);
            this._bookFlow.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this._bookFlow, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._bookLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._chapterLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._chapterFlow, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 30);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(667, 154);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // _bookLabel
            // 
            this._bookLabel.AutoSize = true;
            this._bookLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bookLabel.Location = new System.Drawing.Point(3, 0);
            this._bookLabel.Name = "_bookLabel";
            this._bookLabel.Size = new System.Drawing.Size(45, 17);
            this._bookLabel.TabIndex = 3;
            this._bookLabel.Text = "label1";
            // 
            // _chapterLabel
            // 
            this._chapterLabel.AutoSize = true;
            this._chapterLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._chapterLabel.Location = new System.Drawing.Point(3, 80);
            this._chapterLabel.Name = "_chapterLabel";
            this._chapterLabel.Size = new System.Drawing.Size(45, 17);
            this._chapterLabel.TabIndex = 4;
            this._chapterLabel.Text = "label1";
            // 
            // _chapterFlow
            // 
            this._chapterFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._chapterFlow.Location = new System.Drawing.Point(3, 100);
            this._chapterFlow.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this._chapterFlow.Name = "_chapterFlow";
            this._chapterFlow.Size = new System.Drawing.Size(661, 51);
            this._chapterFlow.TabIndex = 5;
            // 
            // _verseSlider
            // 
            this._verseSlider.LargeChange = 1;
            this._verseSlider.Location = new System.Drawing.Point(13, 207);
            this._verseSlider.Maximum = 40;
            this._verseSlider.Minimum = 1;
            this._verseSlider.Name = "_verseSlider";
            this._verseSlider.Size = new System.Drawing.Size(667, 45);
            this._verseSlider.TabIndex = 11;
            this._verseSlider.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this._verseSlider.Value = 1;
            this._verseSlider.ValueChanged += new System.EventHandler(this.OnVerseSlider_ValueChanged);
            // 
            // _segmentLabel
            // 
            this._segmentLabel.AutoSize = true;
            this._segmentLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._segmentLabel.Location = new System.Drawing.Point(16, 187);
            this._segmentLabel.Name = "_segmentLabel";
            this._segmentLabel.Size = new System.Drawing.Size(60, 17);
            this._segmentLabel.TabIndex = 12;
            this._segmentLabel.Text = "Verse 11";
            // 
            // _maxVerseLabel
            // 
            this._maxVerseLabel.AutoSize = true;
            this._maxVerseLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._maxVerseLabel.Location = new System.Drawing.Point(652, 189);
            this._maxVerseLabel.Name = "_maxVerseLabel";
            this._maxVerseLabel.Size = new System.Drawing.Size(25, 15);
            this._maxVerseLabel.TabIndex = 13;
            this._maxVerseLabel.Text = "153";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._saveButton,
            this.toolStripSeparator1,
            this._generateFiles,
            this.toolStripButton3,
            this.toolStripSeparator2,
            this.toolStripButton4});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(722, 25);
            this.toolStrip1.TabIndex = 21;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // _saveButton
            // 
            this._saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._saveButton.Image = global::HearThis.Properties.Resources.save_all;
            this._saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(23, 22);
            this._saveButton.Text = "toolStripButton1";
            this._saveButton.ToolTipText = "Save";
            this._saveButton.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _generateFiles
            // 
            this._generateFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._generateFiles.Image = global::HearThis.Properties.Resources.generateAudio;
            this._generateFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._generateFiles.Name = "_generateFiles";
            this._generateFiles.Size = new System.Drawing.Size(23, 22);
            this._generateFiles.Text = "Create Audio";
            this._generateFiles.ToolTipText = "Generate Audio Files";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(53, 22);
            this.toolStripButton3.Text = "About...";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripButton3.ToolTipText = "About HearThis...";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = global::HearThis.Properties.Resources.changeProject;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "toolStripButton4";
            this.toolStripButton4.ToolTipText = "Open a different Scripture project.";
            // 
            // _recordAndPlayControl
            // 
            this._recordAndPlayControl.Location = new System.Drawing.Point(16, 288);
            this._recordAndPlayControl.Name = "_recordAndPlayControl";
            this._recordAndPlayControl.Path = "C:\\Users\\John\\AppData\\Local\\Temp\\tmp7357.tmp";
            this._recordAndPlayControl.Size = new System.Drawing.Size(54, 93);
            this._recordAndPlayControl.TabIndex = 20;
            // 
            // _downButton
            // 
            this._downButton.FlatAppearance.BorderSize = 0;
            this._downButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._downButton.Location = new System.Drawing.Point(33, 376);
            this._downButton.Name = "_downButton";
            this._downButton.Size = new System.Drawing.Size(32, 32);
            this._downButton.TabIndex = 19;
            this._downButton.Click += new System.EventHandler(this.OnVerseDownButton);
            // 
            // _upButton
            // 
            this._upButton.FlatAppearance.BorderSize = 0;
            this._upButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._upButton.Location = new System.Drawing.Point(33, 248);
            this._upButton.Name = "_upButton";
            this._upButton.Size = new System.Drawing.Size(32, 32);
            this._upButton.TabIndex = 16;
            this._upButton.Click += new System.EventHandler(this.OnVerseUpButton);
            // 
            // _scriptControl
            // 
            this._scriptControl.BackColor = System.Drawing.Color.Transparent;
            this._scriptControl.Font = new System.Drawing.Font("Andika Basic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._scriptControl.Location = new System.Drawing.Point(80, 248);
            this._scriptControl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._scriptControl.Name = "_scriptControl";
            this._scriptControl.Script = null;
            this._scriptControl.Size = new System.Drawing.Size(597, 167);
            this._scriptControl.TabIndex = 15;
            // 
            // RecordingToolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this._recordAndPlayControl);
            this.Controls.Add(this._downButton);
            this.Controls.Add(this._upButton);
            this.Controls.Add(this._scriptControl);
            this.Controls.Add(this._maxVerseLabel);
            this.Controls.Add(this._segmentLabel);
            this.Controls.Add(this._verseSlider);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RecordingToolControl";
            this.Size = new System.Drawing.Size(722, 400);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._verseSlider)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel _bookFlow;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label _bookLabel;
        private System.Windows.Forms.Label _chapterLabel;
        private System.Windows.Forms.FlowLayoutPanel _chapterFlow;
        private System.Windows.Forms.TrackBar _verseSlider;
        private System.Windows.Forms.Label _segmentLabel;
        private System.Windows.Forms.Label _maxVerseLabel;
        private ScriptControl _scriptControl;
        private ImageButton _upButton;
        private ImageButton _downButton;
        private RecordAndPlayControl _recordAndPlayControl;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton _generateFiles;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton _saveButton;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
    }
}
