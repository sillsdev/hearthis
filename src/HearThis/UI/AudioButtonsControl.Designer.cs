namespace HearThis.UI
{
    partial class AudioButtonsControl
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
            this._startDelayTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._recordButton = new HearThis.UI.RecordButton();
            this._playButton = new HearThis.UI.PlayButton();
            this.SuspendLayout();
            // 
            // _startDelayTimer
            // 
            this._startDelayTimer.Interval = 500;
            this._startDelayTimer.Tick += new System.EventHandler(this.OnStartDelayTimerTick);
            // 
            // _recordButton
            // 
            this._recordButton.BackColor = System.Drawing.Color.Transparent;
            this._recordButton.Enabled = false;
            this._recordButton.Location = new System.Drawing.Point(40, 3);
            this._recordButton.Name = "_recordButton";
            this._recordButton.Size = new System.Drawing.Size(39, 31);
            this._recordButton.TabIndex = 23;
            this._recordButton.Text = "recordButton1";
            this._recordButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnRecordDown);
            this._recordButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnRecordUp);
            // 
            // _playButton
            // 
            this._playButton.BackColor = System.Drawing.Color.Transparent;
            this._playButton.Enabled = false;
            this._playButton.Location = new System.Drawing.Point(4, 4);
            this._playButton.Name = "_playButton";
            this._playButton.Size = new System.Drawing.Size(29, 30);
            this._playButton.TabIndex = 24;
            this._playButton.Text = "playButton1";
            this._playButton.Click += new System.EventHandler(this.OnPlay);
            // 
            // AudioButtonsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this._playButton);
            this.Controls.Add(this._recordButton);
            this.Name = "AudioButtonsControl";
            this.Size = new System.Drawing.Size(81, 40);
            this.Load += new System.EventHandler(this.RecordAndPlayControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer _startDelayTimer;
        private System.Windows.Forms.ToolTip toolTip1;
        private RecordButton _recordButton;
        private PlayButton _playButton;
    }
}
