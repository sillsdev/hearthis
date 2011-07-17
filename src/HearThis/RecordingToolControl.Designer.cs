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
            this.label1 = new System.Windows.Forms.Label();
            this._downButton = new HearThis.ImageButton();
            this._upButton = new HearThis.ImageButton();
            this._scriptControl = new HearThis.ScriptControl();
            this._recordAndPlayControl = new HearThis.RecordAndPlayControl();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._verseSlider)).BeginInit();
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 9);
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
            this._verseSlider.Location = new System.Drawing.Point(670, 3);
            this._verseSlider.Maximum = 40;
            this._verseSlider.Minimum = 1;
            this._verseSlider.Name = "_verseSlider";
            this._verseSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this._verseSlider.Size = new System.Drawing.Size(45, 394);
            this._verseSlider.TabIndex = 11;
            this._verseSlider.Value = 1;
            this._verseSlider.ValueChanged += new System.EventHandler(this.OnVerseSlider_ValueChanged);
            // 
            // _segmentLabel
            // 
            this._segmentLabel.AutoSize = true;
            this._segmentLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._segmentLabel.Location = new System.Drawing.Point(16, 166);
            this._segmentLabel.Name = "_segmentLabel";
            this._segmentLabel.Size = new System.Drawing.Size(60, 17);
            this._segmentLabel.TabIndex = 12;
            this._segmentLabel.Text = "Verse 11";
            // 
            // _maxVerseLabel
            // 
            this._maxVerseLabel.AutoSize = true;
            this._maxVerseLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._maxVerseLabel.Location = new System.Drawing.Point(697, 373);
            this._maxVerseLabel.Name = "_maxVerseLabel";
            this._maxVerseLabel.Size = new System.Drawing.Size(25, 15);
            this._maxVerseLabel.TabIndex = 13;
            this._maxVerseLabel.Text = "153";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(702, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 15);
            this.label1.TabIndex = 14;
            this.label1.Text = "1";
            // 
            // _downButton
            // 
            this._downButton.Location = new System.Drawing.Point(-8, 322);
            this._downButton.Name = "_downButton";
            this._downButton.Size = new System.Drawing.Size(81, 50);
            this._downButton.TabIndex = 19;
            this._downButton.Click += new System.EventHandler(this.OnVerseDownButton);
            // 
            // _upButton
            // 
            this._upButton.Location = new System.Drawing.Point(-8, 195);
            this._upButton.Name = "_upButton";
            this._upButton.Size = new System.Drawing.Size(81, 50);
            this._upButton.TabIndex = 16;
            this._upButton.Click += new System.EventHandler(this.OnVerseUpButton);
            // 
            // _scriptControl
            // 
            this._scriptControl.BackColor = System.Drawing.Color.Transparent;
            this._scriptControl.Font = new System.Drawing.Font("Andika Basic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._scriptControl.Location = new System.Drawing.Point(62, 202);
            this._scriptControl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._scriptControl.Name = "_scriptControl";
            this._scriptControl.Script = null;
            this._scriptControl.Size = new System.Drawing.Size(601, 142);
            this._scriptControl.TabIndex = 15;
            // 
            // _recordAndPlayControl
            // 
            this._recordAndPlayControl.Location = new System.Drawing.Point(-11, 231);
            this._recordAndPlayControl.Name = "_recordAndPlayControl";
            this._recordAndPlayControl.Size = new System.Drawing.Size(79, 90);
            this._recordAndPlayControl.TabIndex = 20;
            // 
            // RecordingToolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this._recordAndPlayControl);
            this.Controls.Add(this._downButton);
            this.Controls.Add(this._upButton);
            this.Controls.Add(this._scriptControl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._maxVerseLabel);
            this.Controls.Add(this._segmentLabel);
            this.Controls.Add(this._verseSlider);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RecordingToolControl";
            this.Size = new System.Drawing.Size(722, 400);
            this.Load += new System.EventHandler(this.RecordingToolControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._verseSlider)).EndInit();
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
        private System.Windows.Forms.Label label1;
        private ScriptControl _scriptControl;
        private ImageButton _upButton;
        private ImageButton _downButton;
        private RecordAndPlayControl _recordAndPlayControl;
    }
}
