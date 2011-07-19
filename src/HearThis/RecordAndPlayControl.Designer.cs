namespace HearThis
{
    partial class RecordAndPlayControl
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
            this._levelMeterTimer = new System.Windows.Forms.Timer(this.components);
            this._playButton = new HearThis.ImageButton();
            this._recordButton = new HearThis.SoundLevelButton();
            this.SuspendLayout();
            // 
            // _levelMeterTimer
            // 
            this._levelMeterTimer.Interval = 10;
            this._levelMeterTimer.Tick += new System.EventHandler(this._levelMeterTimer_Tick);
            // 
            // _playButton
            // 
            this._playButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._playButton.FlatAppearance.BorderSize = 0;
            this._playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._playButton.Location = new System.Drawing.Point(15, 41);
            this._playButton.Name = "_playButton";
            this._playButton.Size = new System.Drawing.Size(32, 32);
            this._playButton.TabIndex = 20;
            this._playButton.Click += new System.EventHandler(this._playButton_Click_1);
            // 
            // _recordButton
            // 
            this._recordButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._recordButton.DetectedLevel = 0F;
            this._recordButton.FlatAppearance.BorderSize = 0;
            this._recordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._recordButton.Location = new System.Drawing.Point(15, 2);
            this._recordButton.Name = "_recordButton";
            this._recordButton.Size = new System.Drawing.Size(32, 32);
            this._recordButton.TabIndex = 19;
            this._recordButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnRecordDown);
            this._recordButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnRecordUp);
            // 
            // RecordAndPlayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._playButton);
            this.Controls.Add(this._recordButton);
            this.Name = "RecordAndPlayControl";
            this.Size = new System.Drawing.Size(51, 81);
            this.Load += new System.EventHandler(this.RecordAndPlayControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageButton _playButton;
        private SoundLevelButton _recordButton;
        private System.Windows.Forms.Timer _levelMeterTimer;
    }
}
