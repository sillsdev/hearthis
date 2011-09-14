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
            Palaso.Media.Naudio.AudioRecorder audioRecorder1 = new Palaso.Media.Naudio.AudioRecorder();
            NAudio.Wave.WaveFormat waveFormat1 = new NAudio.Wave.WaveFormat();
            this._bookFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._chapterFlow = new System.Windows.Forms.FlowLayoutPanel();
            this._bookLabel = new System.Windows.Forms.Label();
            this._chapterLabel = new System.Windows.Forms.Label();
            this._segmentLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this._saveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._generateFiles = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._changeProjectButton = new System.Windows.Forms.ToolStripButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._downButton = new HearThis.UI.ImageButton();
            this._upButton = new HearThis.UI.ImageButton();
            this._instantToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.recordingDeviceButton1 = new Palaso.Media.Naudio.UI.RecordingDeviceButton();
            this._peakMeter = new Palaso.Media.Naudio.UI.PeakMeterCtrl();
            this._audioButtonsControl = new HearThis.UI.AudioButtonsControl();
            this._scriptControl = new HearThis.UI.ScriptControl();
            this._scriptLineSlider = new HearThis.UI.DiscontiguousProgressTrackBar();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._scriptLineSlider)).BeginInit();
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
            this.tableLayoutPanel1.Controls.Add(this._chapterFlow, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._bookLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._chapterLabel, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 30);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(667, 175);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // _chapterFlow
            // 
            this._chapterFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._chapterFlow.Location = new System.Drawing.Point(3, 100);
            this._chapterFlow.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this._chapterFlow.Name = "_chapterFlow";
            this._chapterFlow.Size = new System.Drawing.Size(661, 89);
            this._chapterFlow.TabIndex = 5;
            // 
            // _bookLabel
            // 
            this._bookLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._bookLabel.AutoSize = true;
            this._bookLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bookLabel.Location = new System.Drawing.Point(3, 1);
            this._bookLabel.Name = "_bookLabel";
            this._bookLabel.Size = new System.Drawing.Size(45, 17);
            this._bookLabel.TabIndex = 3;
            this._bookLabel.Text = "label1";
            // 
            // _chapterLabel
            // 
            this._chapterLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._chapterLabel.AutoSize = true;
            this._chapterLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._chapterLabel.Location = new System.Drawing.Point(3, 81);
            this._chapterLabel.Name = "_chapterLabel";
            this._chapterLabel.Size = new System.Drawing.Size(45, 17);
            this._chapterLabel.TabIndex = 4;
            this._chapterLabel.Text = "label1";
            // 
            // _segmentLabel
            // 
            this._segmentLabel.AutoSize = true;
            this._segmentLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._segmentLabel.Location = new System.Drawing.Point(16, 215);
            this._segmentLabel.Name = "_segmentLabel";
            this._segmentLabel.Size = new System.Drawing.Size(60, 17);
            this._segmentLabel.TabIndex = 12;
            this._segmentLabel.Text = "Verse 11";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._saveButton,
            this.toolStripSeparator1,
            this._generateFiles,
            this.toolStripButton3,
            this.toolStripSeparator2,
            this._changeProjectButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(698, 25);
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
            this._generateFiles.Image = global::HearThis.Properties.Resources.generateAudio;
            this._generateFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._generateFiles.Name = "_generateFiles";
            this._generateFiles.Size = new System.Drawing.Size(66, 22);
            this._generateFiles.Text = "Publish";
            this._generateFiles.ToolTipText = "Publish Audio Files";
            this._generateFiles.Click += new System.EventHandler(this._generateFiles_Click);
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
            // _changeProjectButton
            // 
            this._changeProjectButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._changeProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._changeProjectButton.Image = global::HearThis.Properties.Resources.changeProject;
            this._changeProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._changeProjectButton.Name = "_changeProjectButton";
            this._changeProjectButton.Size = new System.Drawing.Size(23, 22);
            this._changeProjectButton.Text = "toolStripButton4";
            this._changeProjectButton.ToolTipText = "Open a different Scripture project.";
            this._changeProjectButton.Click += new System.EventHandler(this.OnChangeProjectButton_Click);
            // 
            // _downButton
            // 
            this._downButton.FlatAppearance.BorderSize = 0;
            this._downButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._downButton.Location = new System.Drawing.Point(21, 399);
            this._downButton.Name = "_downButton";
            this._downButton.Size = new System.Drawing.Size(32, 32);
            this._downButton.TabIndex = 19;
            this.toolTip1.SetToolTip(this._downButton, "Press PageDown key to go back to go to next line.");
            this._downButton.Click += new System.EventHandler(this.OnLineDownButton);
            // 
            // _upButton
            // 
            this._upButton.FlatAppearance.BorderSize = 0;
            this._upButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._upButton.Location = new System.Drawing.Point(21, 276);
            this._upButton.Name = "_upButton";
            this._upButton.Size = new System.Drawing.Size(32, 32);
            this._upButton.TabIndex = 16;
            this.toolTip1.SetToolTip(this._upButton, "Press PageUp key to go back to previous line.");
            this._upButton.Click += new System.EventHandler(this.OnLineUpButton);
            // 
            // _instantToolTip
            // 
            this._instantToolTip.AutomaticDelay = 0;
            this._instantToolTip.UseAnimation = false;
            this._instantToolTip.UseFading = false;
            // 
            // recordingDeviceButton1
            // 
            this.recordingDeviceButton1.Location = new System.Drawing.Point(647, 423);
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
            this._peakMeter.Location = new System.Drawing.Point(654, 273);
            this._peakMeter.Name = "_peakMeter";
            this._peakMeter.ShowGrid = false;
            this._peakMeter.Size = new System.Drawing.Size(17, 144);
            this._peakMeter.TabIndex = 22;
            this._peakMeter.Text = "peakMeterCtrl1";
            // 
            // _audioButtonsControl
            // 
            this._audioButtonsControl.Location = new System.Drawing.Point(4, 316);
            this._audioButtonsControl.Name = "_audioButtonsControl";
            this._audioButtonsControl.Path = "";
            audioRecorder1.MicrophoneLevel = 100D;
            audioRecorder1.RecordedTime = System.TimeSpan.Parse("00:00:00");
            audioRecorder1.RecordingFormat = waveFormat1;
            audioRecorder1.SelectedDevice = null;
            this._audioButtonsControl.Recorder = audioRecorder1;
            this._audioButtonsControl.RecordingDevice = null;
            this._audioButtonsControl.Size = new System.Drawing.Size(54, 93);
            this._audioButtonsControl.TabIndex = 20;
            // 
            // _scriptControl
            // 
            this._scriptControl.BackColor = System.Drawing.Color.Transparent;
            this._scriptControl.Font = new System.Drawing.Font("Andika Basic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._scriptControl.Location = new System.Drawing.Point(80, 276);
            this._scriptControl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._scriptControl.Name = "_scriptControl";
            this._scriptControl.Script = null;
            this._scriptControl.Size = new System.Drawing.Size(597, 167);
            this._scriptControl.TabIndex = 15;
            // 
            // _scriptLineSlider
            // 
            this._scriptLineSlider.LargeChange = 1;
            this._scriptLineSlider.Location = new System.Drawing.Point(13, 234);
            this._scriptLineSlider.Maximum = 50;
            this._scriptLineSlider.Minimum = 1;
            this._scriptLineSlider.Name = "_scriptLineSlider";
            this._scriptLineSlider.Size = new System.Drawing.Size(667, 45);
            this._scriptLineSlider.TabIndex = 11;
            this._scriptLineSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this._scriptLineSlider.Value = 4;
            this._scriptLineSlider.ValueChanged += new System.EventHandler(this.OnLineSlider_ValueChanged);
            // 
            // RecordingToolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.recordingDeviceButton1);
            this.Controls.Add(this._peakMeter);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this._audioButtonsControl);
            this.Controls.Add(this._downButton);
            this.Controls.Add(this._upButton);
            this.Controls.Add(this._scriptControl);
            this.Controls.Add(this._segmentLabel);
            this.Controls.Add(this._scriptLineSlider);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RecordingToolControl";
            this.Size = new System.Drawing.Size(698, 461);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RecordingToolControl_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RecordingToolControl_KeyPress);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private ImageButton _upButton;
        private ImageButton _downButton;
        private AudioButtonsControl _audioButtonsControl;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton _generateFiles;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton _saveButton;
        private System.Windows.Forms.ToolStripButton _changeProjectButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private PeakMeterCtrl _peakMeter;
        private System.Windows.Forms.ToolTip _instantToolTip;
        private RecordingDeviceButton recordingDeviceButton1;
    }
}
