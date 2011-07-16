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
            this._scriptControl = new HearThis.ScriptControl();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
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
            this._segmentLabel.Location = new System.Drawing.Point(3, 160);
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
            // _scriptControl
            // 
            this._scriptControl.BackColor = System.Drawing.Color.Transparent;
            this._scriptControl.Font = new System.Drawing.Font("Andika Basic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._scriptControl.Location = new System.Drawing.Point(87, 202);
            this._scriptControl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._scriptControl.Name = "_scriptControl";
            this._scriptControl.Script = null;
            this._scriptControl.Size = new System.Drawing.Size(559, 142);
            this._scriptControl.TabIndex = 15;
            // 
            // button4
            // 
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CadetBlue;
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Image = global::HearThis.Properties.Resources.down;
            this.button4.Location = new System.Drawing.Point(5, 313);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 46);
            this.button4.TabIndex = 19;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CadetBlue;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Image = global::HearThis.Properties.Resources.play;
            this.button3.Location = new System.Drawing.Point(5, 272);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 46);
            this.button3.TabIndex = 18;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CadetBlue;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Image = global::HearThis.Properties.Resources.record;
            this.button2.Location = new System.Drawing.Point(5, 233);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 46);
            this.button2.TabIndex = 17;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CadetBlue;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = global::HearThis.Properties.Resources.up;
            this.button1.Location = new System.Drawing.Point(5, 193);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 46);
            this.button1.TabIndex = 16;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ScriptureMapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._scriptControl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._maxVerseLabel);
            this.Controls.Add(this._segmentLabel);
            this.Controls.Add(this._verseSlider);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RecordingToolControl";
            this.Size = new System.Drawing.Size(722, 400);
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
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}
