
using System.Windows.Forms;
using SIL.Media.Naudio.UI;

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
            if (disposing)
			{
				if (components != null)
					components.Dispose();
				Program.UnregisterStringsLocalized(HandleStringsLocalized);
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._bookFlow = new System.Windows.Forms.FlowLayoutPanel();
			this._chapterFlow = new System.Windows.Forms.FlowLayoutPanel();
			this._bookLabel = new System.Windows.Forms.Label();
			this._chapterLabel = new System.Windows.Forms.Label();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._segmentLabel = new System.Windows.Forms.Label();
			this._lineCountLabel = new System.Windows.Forms.Label();
			this._scriptSlider = new HearThis.UI.DiscontiguousProgressTrackBar();
			this._peakMeter = new SIL.Media.Naudio.UI.PeakMeterCtrl();
			this._audioButtonsControl = new HearThis.UI.AudioButtonsControl();
			this._tableLayoutScript = new System.Windows.Forms.TableLayoutPanel();
			this._scriptTextHasChangedControl = new HearThis.UI.ScriptTextHasChangedControl();
			this._scriptControl = new HearThis.UI.ScriptControl();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._instantToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.recordingDeviceButton1 = new SIL.Media.Naudio.UI.RecordingDeviceIndicator();
			this._endOfUnitMessage = new System.Windows.Forms.Label();
			this._nextChapterLink = new System.Windows.Forms.LinkLabel();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._mnuShiftClips = new System.Windows.Forms.ToolStripMenuItem();
			this._smallerButton = new HearThis.UI.HearThisToolbarButton();
			this._largerButton = new HearThis.UI.HearThisToolbarButton();
			this._skipButton = new HearThis.UI.HearThisToolbarButton();
			this._recordInPartsButton = new HearThis.UI.HearThisToolbarButton();
			this._deleteRecordingButton = new HearThis.UI.HearThisToolbarButton();
			this._breakLinesAtCommasButton = new HearThis.UI.HearThisToolbarButton();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this._tableLayoutScript.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this._contextMenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._smallerButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._largerButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._skipButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._recordInPartsButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._deleteRecordingButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._breakLinesAtCommasButton)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this._bookFlow, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._chapterFlow, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this._bookLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._chapterLabel, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this._lineCountLabel, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this._scriptSlider, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this._peakMeter, 1, 7);
			this.tableLayoutPanel1.Controls.Add(this._audioButtonsControl, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this._tableLayoutScript, 0, 7);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 15);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 8;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(667, 457);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// _bookFlow
			// 
			this._bookFlow.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this._bookFlow, 2);
			this._bookFlow.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bookFlow.Location = new System.Drawing.Point(3, 32);
			this._bookFlow.Margin = new System.Windows.Forms.Padding(3, 0, 3, 13);
			this._bookFlow.Name = "_bookFlow";
			this._bookFlow.Size = new System.Drawing.Size(661, 1);
			this._bookFlow.TabIndex = 0;
			this._bookFlow.MouseEnter += new System.EventHandler(this.HandleNavigationArea_MouseEnter);
			this._bookFlow.MouseLeave += new System.EventHandler(this.HandleNavigationArea_MouseLeave);
			// 
			// _chapterFlow
			// 
			this._chapterFlow.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this._chapterFlow, 2);
			this._chapterFlow.Dock = System.Windows.Forms.DockStyle.Fill;
			this._chapterFlow.Location = new System.Drawing.Point(6, 77);
			this._chapterFlow.Margin = new System.Windows.Forms.Padding(6, 0, 3, 3);
			this._chapterFlow.Name = "_chapterFlow";
			this._chapterFlow.Size = new System.Drawing.Size(658, 1);
			this._chapterFlow.TabIndex = 5;
			this._chapterFlow.MouseEnter += new System.EventHandler(this.HandleNavigationArea_MouseEnter);
			this._chapterFlow.MouseLeave += new System.EventHandler(this.HandleNavigationArea_MouseLeave);
			// 
			// _bookLabel
			// 
			this._bookLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._bookLabel.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this._bookLabel, 2);
			this._bookLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._bookLabel.ForeColor = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._bookLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._bookLabel, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._bookLabel, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._bookLabel, "RecordingControl.BookLabel");
			this._bookLabel.Location = new System.Drawing.Point(0, 0);
			this._bookLabel.Margin = new System.Windows.Forms.Padding(0);
			this._bookLabel.Name = "_bookLabel";
			this._bookLabel.Size = new System.Drawing.Size(29, 32);
			this._bookLabel.TabIndex = 3;
			this._bookLabel.Text = "#";
			// 
			// _chapterLabel
			// 
			this._chapterLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._chapterLabel.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this._chapterLabel, 2);
			this._chapterLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._chapterLabel.ForeColor = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._chapterLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._chapterLabel, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._chapterLabel, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._chapterLabel, "RecordingControl.ChapterLabel");
			this._chapterLabel.Location = new System.Drawing.Point(0, 45);
			this._chapterLabel.Margin = new System.Windows.Forms.Padding(0);
			this._chapterLabel.Name = "_chapterLabel";
			this._chapterLabel.Size = new System.Drawing.Size(29, 32);
			this._chapterLabel.TabIndex = 4;
			this._chapterLabel.Text = "#";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this._segmentLabel);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 83);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(361, 35);
			this.flowLayoutPanel1.TabIndex = 41;
			// 
			// _segmentLabel
			// 
			this._segmentLabel.AutoSize = true;
			this._segmentLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._segmentLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._segmentLabel.ForeColor = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._segmentLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._segmentLabel, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._segmentLabel, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._segmentLabel, "RecordingControl.SegmentLabel");
			this._segmentLabel.Location = new System.Drawing.Point(0, 0);
			this._segmentLabel.Margin = new System.Windows.Forms.Padding(0);
			this._segmentLabel.Name = "_segmentLabel";
			this._segmentLabel.Size = new System.Drawing.Size(105, 32);
			this._segmentLabel.TabIndex = 12;
			this._segmentLabel.Text = "Verse 20";
			// 
			// _lineCountLabel
			// 
			this._lineCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lineCountLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._lineCountLabel.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._lineCountLabel.ForeColor = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._lineCountLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._lineCountLabel, null);
			this.l10NSharpExtender1.SetLocalizingId(this._lineCountLabel, "RecordingControl.LineCountLabel");
			this._lineCountLabel.Location = new System.Drawing.Point(414, 80);
			this._lineCountLabel.Name = "_lineCountLabel";
			this._lineCountLabel.Size = new System.Drawing.Size(250, 25);
			this._lineCountLabel.TabIndex = 25;
			this._lineCountLabel.Text = "Block {0}/{1}";
			this._lineCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _scriptSlider
			// 
			this._scriptSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._scriptSlider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this.tableLayoutPanel1.SetColumnSpan(this._scriptSlider, 2);
			this._scriptSlider.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this._scriptSlider, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._scriptSlider, null);
			this.l10NSharpExtender1.SetLocalizingId(this._scriptSlider, "RecordingControl.ScriptLineSlider");
			this._scriptSlider.Location = new System.Drawing.Point(3, 124);
			this._scriptSlider.Name = "_scriptSlider";
			this._scriptSlider.Padding = new System.Windows.Forms.Padding(0, 0, 7, 5);
			this._scriptSlider.SegmentCount = 50;
			this._scriptSlider.Size = new System.Drawing.Size(661, 25);
			this._scriptSlider.TabIndex = 11;
			this._scriptSlider.Value = 4;
			this._scriptSlider.ValueChanged += new System.EventHandler(this.OnLineSlider_ValueChanged);
			this._scriptSlider.MouseClick += new System.Windows.Forms.MouseEventHandler(this._scriptSlider_MouseClick);
			// 
			// _peakMeter
			// 
			this._peakMeter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
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
			this.l10NSharpExtender1.SetLocalizableToolTip(this._peakMeter, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._peakMeter, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._peakMeter, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._peakMeter, "RecordingControl.PeakMeter");
			this._peakMeter.Location = new System.Drawing.Point(644, 345);
			this._peakMeter.Name = "_peakMeter";
			this._peakMeter.ShowGrid = false;
			this._peakMeter.Size = new System.Drawing.Size(20, 109);
			this._peakMeter.TabIndex = 22;
			this._peakMeter.Text = "peakMeterCtrl1";
			// 
			// _audioButtonsControl
			// 
			this._audioButtonsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._audioButtonsControl.AutoSize = true;
			this._audioButtonsControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._audioButtonsControl.BackColor = System.Drawing.Color.Transparent;
			this._audioButtonsControl.ButtonHighlightMode = HearThis.UI.AudioButtonsControl.ButtonHighlightModes.Default;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._audioButtonsControl, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._audioButtonsControl, null);
			this.l10NSharpExtender1.SetLocalizingId(this._audioButtonsControl, "RecordingControl.AudioButtonsControl");
			this._audioButtonsControl.Location = new System.Drawing.Point(516, 155);
			this._audioButtonsControl.Margin = new System.Windows.Forms.Padding(3, 3, 31, 3);
			this._audioButtonsControl.Name = "_audioButtonsControl";
			this._audioButtonsControl.RecordingDevice = null;
			this._audioButtonsControl.ShowNextButton = true;
			this._audioButtonsControl.ShowPlayButton = true;
			this._audioButtonsControl.ShowRecordButton = true;
			this._audioButtonsControl.Size = new System.Drawing.Size(120, 42);
			this._audioButtonsControl.TabIndex = 20;
			this._audioButtonsControl.NextClick += new System.EventHandler(this.OnNextButton);
			this._audioButtonsControl.RecordButtonStateChanged += new HearThis.UI.AudioButtonsControl.ButtonStateChangedHandler(this.OnRecordButtonStateChanged);
			// 
			// _tableLayoutScript
			// 
			this._tableLayoutScript.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableLayoutScript.ColumnCount = 1;
			this._tableLayoutScript.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutScript.Controls.Add(this._scriptTextHasChangedControl, 0, 1);
			this._tableLayoutScript.Controls.Add(this._scriptControl, 0, 0);
			this._tableLayoutScript.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutScript.Location = new System.Drawing.Point(0, 200);
			this._tableLayoutScript.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutScript.Name = "_tableLayoutScript";
			this._tableLayoutScript.RowCount = 2;
			this._tableLayoutScript.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutScript.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutScript.Size = new System.Drawing.Size(411, 257);
			this._tableLayoutScript.TabIndex = 48;
			// 
			// _scriptTextHasChangedControl
			// 
			this._scriptTextHasChangedControl.AllowDrop = true;
			this._scriptTextHasChangedControl.BackColor = System.Drawing.Color.Transparent;
			this._scriptTextHasChangedControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._scriptTextHasChangedControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._scriptTextHasChangedControl.ForeColor = System.Drawing.Color.DarkGray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._scriptTextHasChangedControl, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._scriptTextHasChangedControl, null);
			this.l10NSharpExtender1.SetLocalizingId(this._scriptTextHasChangedControl, "RecordingControl.ScriptTextHasChangedControl");
			this._scriptTextHasChangedControl.Location = new System.Drawing.Point(4, 340);
			this._scriptTextHasChangedControl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this._scriptTextHasChangedControl.Name = "_scriptTextHasChangedControl";
			this._scriptTextHasChangedControl.Size = new System.Drawing.Size(403, 2);
			this._scriptTextHasChangedControl.TabIndex = 47;
			this._scriptTextHasChangedControl.Visible = false;
			this._scriptTextHasChangedControl.ZoomFactor = 1F;
			this._scriptTextHasChangedControl.ProblemIgnoreStateChanged += new System.EventHandler(this._scriptTextHasChangedControl_ProblemIgnoreStateChanged);
			this._scriptTextHasChangedControl.NextClick += new System.EventHandler(this.OnNextButton);
			// 
			// _scriptControl
			// 
			this._scriptControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._scriptControl.BackColor = System.Drawing.Color.Transparent;
			this._scriptControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._scriptControl.ForeColor = System.Drawing.Color.White;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._scriptControl, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._scriptControl, null);
			this.l10NSharpExtender1.SetLocalizingId(this._scriptControl, "RecordingControl.ScriptControl");
			this._scriptControl.Location = new System.Drawing.Point(4, 6);
			this._scriptControl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this._scriptControl.Name = "_scriptControl";
			this._scriptControl.ShowSkippedBlocks = false;
			this._scriptControl.Size = new System.Drawing.Size(403, 322);
			this._scriptControl.TabIndex = 15;
			this._scriptControl.ZoomFactor = 1F;
			this._scriptControl.LocationChanged += new System.EventHandler(this._scriptControl_LocationChanged);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 6500;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 100;
			// 
			// _instantToolTip
			// 
			this._instantToolTip.AutomaticDelay = 0;
			this._instantToolTip.UseAnimation = false;
			this._instantToolTip.UseFading = false;
			// 
			// recordingDeviceButton1
			// 
			this.recordingDeviceButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.recordingDeviceButton1.BackColor = System.Drawing.Color.Transparent;
			this.recordingDeviceButton1.ComputerInternalImage = null;
			this.recordingDeviceButton1.KnownHeadsetImage = null;
			this.recordingDeviceButton1.LineImage = null;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.recordingDeviceButton1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.recordingDeviceButton1, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.recordingDeviceButton1, L10NSharp.LocalizationPriority.Low);
			this.l10NSharpExtender1.SetLocalizingId(this.recordingDeviceButton1, "RecordingControl.RecordingDeviceButton");
			this.recordingDeviceButton1.Location = new System.Drawing.Point(659, 494);
			this.recordingDeviceButton1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this.recordingDeviceButton1.MicrophoneImage = null;
			this.recordingDeviceButton1.Name = "recordingDeviceButton1";
			this.recordingDeviceButton1.NoAudioDeviceImage = null;
			this.recordingDeviceButton1.Recorder = null;
			this.recordingDeviceButton1.RecorderImage = null;
			this.recordingDeviceButton1.Size = new System.Drawing.Size(22, 25);
			this.recordingDeviceButton1.TabIndex = 23;
			this.recordingDeviceButton1.UsbAudioDeviceImage = null;
			this.recordingDeviceButton1.WebcamImage = null;
			// 
			// _endOfUnitMessage
			// 
			this._endOfUnitMessage.BackColor = System.Drawing.Color.Transparent;
			this._endOfUnitMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._endOfUnitMessage.ForeColor = System.Drawing.Color.White;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._endOfUnitMessage, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._endOfUnitMessage, null);
			this.l10NSharpExtender1.SetLocalizingId(this._endOfUnitMessage, "RecordingControl.RecordingToolControl._endOfUnitMessage");
			this._endOfUnitMessage.Location = new System.Drawing.Point(19, 312);
			this._endOfUnitMessage.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this._endOfUnitMessage.Name = "_endOfUnitMessage";
			this._endOfUnitMessage.Size = new System.Drawing.Size(356, 50);
			this._endOfUnitMessage.TabIndex = 35;
			this._endOfUnitMessage.Text = "End of Chapter/Book";
			this._endOfUnitMessage.Visible = false;
			// 
			// _nextChapterLink
			// 
			this._nextChapterLink.BackColor = System.Drawing.Color.Transparent;
			this._nextChapterLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._nextChapterLink.ForeColor = System.Drawing.Color.White;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._nextChapterLink, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._nextChapterLink, null);
			this.l10NSharpExtender1.SetLocalizingId(this._nextChapterLink, "RecordingControl.RecordingToolControl._nextChapterLink");
			this._nextChapterLink.Location = new System.Drawing.Point(19, 365);
			this._nextChapterLink.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this._nextChapterLink.Name = "_nextChapterLink";
			this._nextChapterLink.Size = new System.Drawing.Size(356, 50);
			this._nextChapterLink.TabIndex = 36;
			this._nextChapterLink.TabStop = true;
			this._nextChapterLink.Text = "Go To Chapter x";
			this._nextChapterLink.Visible = false;
			this._nextChapterLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnNextChapterLink_LinkClicked);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "RecordingControl";
			// 
			// _contextMenuStrip
			// 
			this._contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuShiftClips});
			this.l10NSharpExtender1.SetLocalizableToolTip(this._contextMenuStrip, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._contextMenuStrip, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._contextMenuStrip, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this._contextMenuStrip, "RecordingControl._contextMenuStrip._contextMenuStrip");
			this._contextMenuStrip.Name = "_contextMenuStrip";
			this._contextMenuStrip.Size = new System.Drawing.Size(137, 26);
			// 
			// _mnuShiftClips
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this._mnuShiftClips, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._mnuShiftClips, null);
			this.l10NSharpExtender1.SetLocalizingId(this._mnuShiftClips, "RecordingControl.ShiftClipsToolStripMenuItem");
			this._mnuShiftClips.Name = "_mnuShiftClips";
			this._mnuShiftClips.Size = new System.Drawing.Size(136, 22);
			this._mnuShiftClips.Text = "Shift Clips...";
			this._mnuShiftClips.Click += new System.EventHandler(this._mnuShiftClips_Click);
			// 
			// _smallerButton
			// 
			this._smallerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._smallerButton.CheckBox = false;
			this._smallerButton.Checked = false;
			this._smallerButton.Image = global::HearThis.Properties.Resources.BottomToolbar_Smaller;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._smallerButton, "Smaller Text");
			this.l10NSharpExtender1.SetLocalizationComment(this._smallerButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._smallerButton, "RecordingControl.SmallerButton");
			this._smallerButton.Location = new System.Drawing.Point(14, 490);
			this._smallerButton.Name = "_smallerButton";
			this._smallerButton.Size = new System.Drawing.Size(18, 24);
			this._smallerButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._smallerButton.TabIndex = 46;
			this._smallerButton.TabStop = false;
			this._smallerButton.Click += new System.EventHandler(this.OnSmallerClick);
			// 
			// _largerButton
			// 
			this._largerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._largerButton.CheckBox = false;
			this._largerButton.Checked = false;
			this._largerButton.Image = global::HearThis.Properties.Resources.BottomToolbar_Larger;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._largerButton, "Larger Text");
			this.l10NSharpExtender1.SetLocalizationComment(this._largerButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._largerButton, "RecordingControl.LargerButton");
			this._largerButton.Location = new System.Drawing.Point(36, 490);
			this._largerButton.Name = "_largerButton";
			this._largerButton.Size = new System.Drawing.Size(23, 24);
			this._largerButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._largerButton.TabIndex = 45;
			this._largerButton.TabStop = false;
			this._largerButton.Click += new System.EventHandler(this.OnLargerClick);
			// 
			// _skipButton
			// 
			this._skipButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._skipButton.CheckBox = true;
			this._skipButton.Checked = false;
			this._skipButton.Image = global::HearThis.Properties.Resources.BottomToolbar_Skip;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._skipButton, "Skip this block - it does not need to be recorded.");
			this.l10NSharpExtender1.SetLocalizationComment(this._skipButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._skipButton, "RecordingControl.skipButton1");
			this._skipButton.Location = new System.Drawing.Point(374, 490);
			this._skipButton.Margin = new System.Windows.Forms.Padding(0);
			this._skipButton.Name = "_skipButton";
			this._skipButton.Size = new System.Drawing.Size(22, 24);
			this._skipButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._skipButton.TabIndex = 44;
			this._skipButton.TabStop = false;
			this._skipButton.CheckedChanged += new System.EventHandler(this.OnSkipButtonCheckedChanged);
			// 
			// _recordInPartsButton
			// 
			this._recordInPartsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._recordInPartsButton.CheckBox = false;
			this._recordInPartsButton.Checked = false;
			this._recordInPartsButton.Image = global::HearThis.Properties.Resources.BottomToolbar_RecordInParts;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._recordInPartsButton, "Record long lines in parts. (p key)");
			this.l10NSharpExtender1.SetLocalizationComment(this._recordInPartsButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._recordInPartsButton, "RecordingControl.RecordLongLinesInParts");
			this._recordInPartsButton.Location = new System.Drawing.Point(314, 490);
			this._recordInPartsButton.Margin = new System.Windows.Forms.Padding(0);
			this._recordInPartsButton.Name = "_recordInPartsButton";
			this._recordInPartsButton.Size = new System.Drawing.Size(40, 24);
			this._recordInPartsButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._recordInPartsButton.TabIndex = 43;
			this._recordInPartsButton.TabStop = false;
			this._recordInPartsButton.Click += new System.EventHandler(this.longLineButton_Click);
			// 
			// _deleteRecordingButton
			// 
			this._deleteRecordingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._deleteRecordingButton.CheckBox = false;
			this._deleteRecordingButton.Checked = false;
			this._deleteRecordingButton.Image = global::HearThis.Properties.Resources.BottomToolbar_Delete;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._deleteRecordingButton, "Remove this recorded clip (Delete Key)");
			this.l10NSharpExtender1.SetLocalizationComment(this._deleteRecordingButton, "Shows as an \'X\' when on a script line that has been recorded.");
			this.l10NSharpExtender1.SetLocalizingId(this._deleteRecordingButton, "RecordingControl.RemoveThisRecording");
			this._deleteRecordingButton.Location = new System.Drawing.Point(619, 490);
			this._deleteRecordingButton.Name = "_deleteRecordingButton";
			this._deleteRecordingButton.Size = new System.Drawing.Size(21, 24);
			this._deleteRecordingButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._deleteRecordingButton.TabIndex = 39;
			this._deleteRecordingButton.TabStop = false;
			this._deleteRecordingButton.Click += new System.EventHandler(this._deleteRecordingButton_Click);
			this._deleteRecordingButton.MouseEnter += new System.EventHandler(this._deleteRecordingButton_MouseEnter);
			this._deleteRecordingButton.MouseLeave += new System.EventHandler(this._deleteRecordingButton_MouseLeave);
			// 
			// _breakLinesAtCommasButton
			// 
			this._breakLinesAtCommasButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._breakLinesAtCommasButton.CheckBox = true;
			this._breakLinesAtCommasButton.Checked = false;
			this._breakLinesAtCommasButton.Image = global::HearThis.Properties.Resources.BottomToolbar_BreakOnCommas;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._breakLinesAtCommasButton, "Start new line at pause punctuation");
			this.l10NSharpExtender1.SetLocalizationComment(this._breakLinesAtCommasButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._breakLinesAtCommasButton, "RecordingControl.BreakLinesAtClauses");
			this._breakLinesAtCommasButton.Location = new System.Drawing.Point(100, 490);
			this._breakLinesAtCommasButton.Name = "_breakLinesAtCommasButton";
			this._breakLinesAtCommasButton.Size = new System.Drawing.Size(28, 24);
			this._breakLinesAtCommasButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._breakLinesAtCommasButton.TabIndex = 38;
			this._breakLinesAtCommasButton.TabStop = false;
			this._breakLinesAtCommasButton.Click += new System.EventHandler(this._breakLinesAtCommasButton_Click);
			// 
			// RecordingToolControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this.Controls.Add(this._nextChapterLink);
			this.Controls.Add(this._smallerButton);
			this.Controls.Add(this._largerButton);
			this.Controls.Add(this._skipButton);
			this.Controls.Add(this._recordInPartsButton);
			this.Controls.Add(this._deleteRecordingButton);
			this.Controls.Add(this._breakLinesAtCommasButton);
			this.Controls.Add(this.recordingDeviceButton1);
			this.Controls.Add(this._endOfUnitMessage);
			this.Controls.Add(this.tableLayoutPanel1);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this, "RecordingControl.RecordingToolControl.RecordingToolControl");
			this.Margin = new System.Windows.Forms.Padding(10);
			this.Name = "RecordingToolControl";
			this.Size = new System.Drawing.Size(706, 527);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this._tableLayoutScript.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this._contextMenuStrip.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._smallerButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._largerButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._skipButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._recordInPartsButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._deleteRecordingButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._breakLinesAtCommasButton)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label _bookLabel;
        private System.Windows.Forms.Label _chapterLabel;
        private System.Windows.Forms.FlowLayoutPanel _chapterFlow;
        private DiscontiguousProgressTrackBar _scriptSlider;
        private System.Windows.Forms.Label _segmentLabel;
        private ScriptControl _scriptControl;
	    private System.Windows.Forms.Label _endOfUnitMessage;
	    private System.Windows.Forms.LinkLabel _nextChapterLink;
        private AudioButtonsControl _audioButtonsControl;
        private System.Windows.Forms.ToolTip toolTip1;
        private PeakMeterCtrl _peakMeter;
        private System.Windows.Forms.ToolTip _instantToolTip;
        private RecordingDeviceIndicator recordingDeviceButton1;
        private System.Windows.Forms.Label _lineCountLabel;
        private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
        private HearThisToolbarButton _breakLinesAtCommasButton;
		private HearThisToolbarButton _deleteRecordingButton;
		private FlowLayoutPanel flowLayoutPanel1;
		private HearThisToolbarButton _recordInPartsButton;
		private HearThisToolbarButton _skipButton;
		private HearThisToolbarButton _largerButton;
		private HearThisToolbarButton _smallerButton;
		private ContextMenuStrip _contextMenuStrip;
		private ToolStripMenuItem _mnuShiftClips;
		private ScriptTextHasChangedControl _scriptTextHasChangedControl;
		private FlowLayoutPanel _bookFlow;
		private TableLayoutPanel _tableLayoutScript;
	}
}
