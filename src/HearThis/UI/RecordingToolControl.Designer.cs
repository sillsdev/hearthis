using Palaso.Media.Naudio;
using Palaso.Media.Naudio.UI;

namespace HearThis.UI
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
            this.components = new System.ComponentModel.Container();
            Palaso.Media.Naudio.AudioRecorder audioRecorder2 = new Palaso.Media.Naudio.AudioRecorder();
            NAudio.Wave.WaveFormat waveFormat2 = new NAudio.Wave.WaveFormat();
            this._bookFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._chapterFlow = new System.Windows.Forms.FlowLayoutPanel();
            this._bookLabel = new System.Windows.Forms.Label();
            this._chapterLabel = new System.Windows.Forms.Label();
            this._segmentLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this._instantToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.recordingDeviceButton1 = new Palaso.Media.Naudio.UI.RecordingDeviceButton();
            this._peakMeter = new Palaso.Media.Naudio.UI.PeakMeterCtrl();
            this._lineCountLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this._nextButton = new HearThis.UI.ArrowButton();
            this._audioButtonsControl = new HearThis.UI.AudioButtonsControl();
            this._scriptControl = new HearThis.UI.ScriptControl();
            this._scriptLineSlider = new HearThis.UI.DiscontiguousProgressTrackBar();
            this.button4 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._scriptLineSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // _bookFlow
            // 
            this._bookFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._bookFlow.Location = new System.Drawing.Point(3, 34);
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
            this.tableLayoutPanel1.Controls.Add(this._chapterFlow, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._bookLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._chapterLabel, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 45);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(667, 198);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // _chapterFlow
            // 
            this._chapterFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._chapterFlow.Location = new System.Drawing.Point(3, 122);
            this._chapterFlow.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this._chapterFlow.Name = "_chapterFlow";
            this._chapterFlow.Size = new System.Drawing.Size(661, 89);
            this._chapterFlow.TabIndex = 5;
            // 
            // _bookLabel
            // 
            this._bookLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._bookLabel.AutoSize = true;
            this._bookLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bookLabel.ForeColor = System.Drawing.Color.DarkGray;
            this._bookLabel.Location = new System.Drawing.Point(0, 0);
            this._bookLabel.Margin = new System.Windows.Forms.Padding(0);
            this._bookLabel.Name = "_bookLabel";
            this._bookLabel.Size = new System.Drawing.Size(79, 31);
            this._bookLabel.TabIndex = 3;
            this._bookLabel.Text = "label1";
            // 
            // _chapterLabel
            // 
            this._chapterLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._chapterLabel.AutoSize = true;
            this._chapterLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._chapterLabel.ForeColor = System.Drawing.Color.DarkGray;
            this._chapterLabel.Location = new System.Drawing.Point(0, 91);
            this._chapterLabel.Margin = new System.Windows.Forms.Padding(0);
            this._chapterLabel.Name = "_chapterLabel";
            this._chapterLabel.Size = new System.Drawing.Size(79, 31);
            this._chapterLabel.TabIndex = 4;
            this._chapterLabel.Text = "label1";
            // 
            // _segmentLabel
            // 
            this._segmentLabel.AutoSize = true;
            this._segmentLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._segmentLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._segmentLabel.ForeColor = System.Drawing.Color.DarkGray;
            this._segmentLabel.Location = new System.Drawing.Point(16, 242);
            this._segmentLabel.Margin = new System.Windows.Forms.Padding(0);
            this._segmentLabel.Name = "_segmentLabel";
            this._segmentLabel.Size = new System.Drawing.Size(107, 32);
            this._segmentLabel.TabIndex = 12;
            this._segmentLabel.Text = "Verse 11";
            // 
            // button3
            // 
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.DimGray;
            this.button3.Image = global::HearThis.Properties.Resources.sabber;
            this.button3.Location = new System.Drawing.Point(533, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(51, 26);
            this.button3.TabIndex = 30;
            this.toolTip1.SetToolTip(this.button3, "Publish files for various devices");
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.OnPublishClick);
            // 
            // button2
            // 
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.DimGray;
            this.button2.Image = global::HearThis.Properties.Resources.folder;
            this.button2.Location = new System.Drawing.Point(581, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(43, 26);
            this.button2.TabIndex = 29;
            this.toolTip1.SetToolTip(this.button2, "Choose Paratext project...");
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OnChangeProjectButton_Click);
            // 
            // _instantToolTip
            // 
            this._instantToolTip.AutomaticDelay = 0;
            this._instantToolTip.UseAnimation = false;
            this._instantToolTip.UseFading = false;
            // 
            // recordingDeviceButton1
            // 
            this.recordingDeviceButton1.BackColor = System.Drawing.Color.Transparent;
            this.recordingDeviceButton1.Location = new System.Drawing.Point(640, 476);
            this.recordingDeviceButton1.Name = "recordingDeviceButton1";
            this.recordingDeviceButton1.Recorder = null;
            this.recordingDeviceButton1.Size = new System.Drawing.Size(33, 37);
            this.recordingDeviceButton1.TabIndex = 23;
            // 
            // _peakMeter
            // 
            this._peakMeter.BandsCount = 1;
            this._peakMeter.ColorHigh = System.Drawing.Color.Red;
            this._peakMeter.ColorHighBack = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this._peakMeter.ColorMedium = System.Drawing.Color.Yellow;
            this._peakMeter.ColorMediumBack = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(150)))));
            this._peakMeter.ColorNormal = System.Drawing.Color.Green;
            this._peakMeter.ColorNormalBack = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(255)))), ((int)(((byte)(150)))));
            this._peakMeter.FalloffColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this._peakMeter.FalloffEffect = false;
            this._peakMeter.GridColor = System.Drawing.Color.Gainsboro;
            this._peakMeter.LEDCount = 15;
            this._peakMeter.Location = new System.Drawing.Point(652, 361);
            this._peakMeter.Name = "_peakMeter";
            this._peakMeter.ShowGrid = false;
            this._peakMeter.Size = new System.Drawing.Size(20, 109);
            this._peakMeter.TabIndex = 22;
            this._peakMeter.Text = "peakMeterCtrl1";
            // 
            // _lineCountLabel
            // 
            this._lineCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lineCountLabel.AutoSize = true;
            this._lineCountLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._lineCountLabel.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lineCountLabel.ForeColor = System.Drawing.Color.DarkGray;
            this._lineCountLabel.Location = new System.Drawing.Point(634, 242);
            this._lineCountLabel.Name = "_lineCountLabel";
            this._lineCountLabel.Size = new System.Drawing.Size(42, 25);
            this._lineCountLabel.TabIndex = 25;
            this._lineCountLabel.Text = "997";
            this._lineCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.DimGray;
            this.button1.Location = new System.Drawing.Point(623, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 26);
            this.button1.TabIndex = 28;
            this.button1.Text = "About...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnAboutClick);
            // 
            // _nextButton
            // 
            this._nextButton.BackColor = System.Drawing.Color.Transparent;
            this._nextButton.Enabled = false;
            this._nextButton.Location = new System.Drawing.Point(640, 306);
            this._nextButton.Name = "_nextButton";
            this._nextButton.Size = new System.Drawing.Size(32, 33);
            this._nextButton.State = HearThis.UI.BtnState.Normal;
            this._nextButton.TabIndex = 27;
            this._nextButton.Text = "_nextButton";
            this._nextButton.Click += new System.EventHandler(this.OnLineDownButton);
            // 
            // _audioButtonsControl
            // 
            this._audioButtonsControl.BackColor = System.Drawing.Color.Transparent;
            this._audioButtonsControl.Location = new System.Drawing.Point(557, 303);
            this._audioButtonsControl.Name = "_audioButtonsControl";
            this._audioButtonsControl.Path = "";
            audioRecorder2.MicrophoneLevel = 100D;
            audioRecorder2.RecordedTime = System.TimeSpan.Parse("00:00:00");
            audioRecorder2.RecordingFormat = waveFormat2;
            audioRecorder2.SelectedDevice = null;
            this._audioButtonsControl.Recorder = audioRecorder2;
            this._audioButtonsControl.RecordingDevice = null;
            this._audioButtonsControl.Size = new System.Drawing.Size(78, 43);
            this._audioButtonsControl.TabIndex = 20;
            // 
            // _scriptControl
            // 
            this._scriptControl.BackColor = System.Drawing.Color.Transparent;
            this._scriptControl.Font = new System.Drawing.Font("Andika Basic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._scriptControl.ForeColor = System.Drawing.Color.White;
            this._scriptControl.Location = new System.Drawing.Point(19, 307);
            this._scriptControl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._scriptControl.Name = "_scriptControl";
            this._scriptControl.Script = null;
            this._scriptControl.Size = new System.Drawing.Size(602, 167);
            this._scriptControl.TabIndex = 15;
            // 
            // _scriptLineSlider
            // 
            this._scriptLineSlider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._scriptLineSlider.LargeChange = 1;
            this._scriptLineSlider.Location = new System.Drawing.Point(13, 261);
            this._scriptLineSlider.Maximum = 50;
            this._scriptLineSlider.Minimum = 1;
            this._scriptLineSlider.Name = "_scriptLineSlider";
            this._scriptLineSlider.Size = new System.Drawing.Size(667, 45);
            this._scriptLineSlider.TabIndex = 11;
            this._scriptLineSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this._scriptLineSlider.Value = 4;
            this._scriptLineSlider.ValueChanged += new System.EventHandler(this.OnLineSlider_ValueChanged);
            // 
            // button4
            // 
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.DimGray;
            this.button4.Image = global::HearThis.Properties.Resources.disk16x16;
            this.button4.Location = new System.Drawing.Point(12, 6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(29, 26);
            this.button4.TabIndex = 31;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnSaveClick);
            // 
            // RecordingToolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._nextButton);
            this.Controls.Add(this._lineCountLabel);
            this.Controls.Add(this.recordingDeviceButton1);
            this.Controls.Add(this._peakMeter);
            this.Controls.Add(this._audioButtonsControl);
            this.Controls.Add(this._scriptControl);
            this.Controls.Add(this._segmentLabel);
            this.Controls.Add(this._scriptLineSlider);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RecordingToolControl";
            this.Size = new System.Drawing.Size(698, 518);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RecordingToolControl_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RecordingToolControl_KeyPress);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._scriptLineSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel _bookFlow;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label _bookLabel;
        private System.Windows.Forms.Label _chapterLabel;
        private System.Windows.Forms.FlowLayoutPanel _chapterFlow;
        private DiscontiguousProgressTrackBar _scriptLineSlider;
        private System.Windows.Forms.Label _segmentLabel;
        private ScriptControl _scriptControl;
        private AudioButtonsControl _audioButtonsControl;
        private System.Windows.Forms.ToolTip toolTip1;
        private PeakMeterCtrl _peakMeter;
        private System.Windows.Forms.ToolTip _instantToolTip;
        private RecordingDeviceButton recordingDeviceButton1;
        private System.Windows.Forms.Label _lineCountLabel;
        private ArrowButton _nextButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}
