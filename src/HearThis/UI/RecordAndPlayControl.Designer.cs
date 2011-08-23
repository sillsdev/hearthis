namespace HearThis.UI
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._playButton = new HearThis.UI.BitmapButton();
            this._recordButton = new HearThis.UI.BitmapButton();
            this.SuspendLayout();
            // 
            // _levelMeterTimer
            // 
            this._levelMeterTimer.Interval = 10;
            this._levelMeterTimer.Tick += new System.EventHandler(this._levelMeterTimer_Tick);
            // 
            // _playButton
            // 
            this._playButton.BorderColor = System.Drawing.Color.DarkBlue;
            this._playButton.FlatAppearance.BorderSize = 0;
            this._playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._playButton.FocusRectangleEnabled = true;
            this._playButton.Image = null;
            this._playButton.ImageBorderColor = System.Drawing.Color.Chocolate;
            this._playButton.ImageBorderEnabled = false;
            this._playButton.ImageDropShadow = false;
            this._playButton.ImageFocused = null;
            this._playButton.ImageInactive = global::HearThis.Properties.Resources.playDisabled;
            this._playButton.ImageMouseOver = null;
            this._playButton.ImageNormal = global::HearThis.Properties.Resources.play;
            this._playButton.ImagePressed = null;
            this._playButton.InnerBorderColor = System.Drawing.Color.LightGray;
            this._playButton.InnerBorderColor_Focus = System.Drawing.Color.LightBlue;
            this._playButton.InnerBorderColor_MouseOver = System.Drawing.Color.Gold;
            this._playButton.Location = new System.Drawing.Point(11, 42);
            this._playButton.Name = "_playButton";
            this._playButton.OffsetPressedContent = true;
            this._playButton.Size = new System.Drawing.Size(44, 36);
            this._playButton.StretchImage = false;
            this._playButton.TabIndex = 22;
            this._playButton.TextDropShadow = false;
            this._playButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolTip1.SetToolTip(this._playButton, "To hear what you recorded, click this or press Enter");
            this._playButton.UseVisualStyleBackColor = true;
            this._playButton.Click += new System.EventHandler(this.OnPlay);
            // 
            // _recordButton
            // 
            this._recordButton.BorderColor = System.Drawing.Color.DarkBlue;
            this._recordButton.FlatAppearance.BorderSize = 0;
            this._recordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._recordButton.FocusRectangleEnabled = true;
            this._recordButton.Image = null;
            this._recordButton.ImageBorderColor = System.Drawing.Color.Chocolate;
            this._recordButton.ImageBorderEnabled = false;
            this._recordButton.ImageDropShadow = false;
            this._recordButton.ImageFocused = null;
            this._recordButton.ImageInactive = global::HearThis.Properties.Resources.recordDisabled;
            this._recordButton.ImageMouseOver = null;
            this._recordButton.ImageNormal = global::HearThis.Properties.Resources.record;
            this._recordButton.ImagePressed = global::HearThis.Properties.Resources.recordActive;
            this._recordButton.InnerBorderColor = System.Drawing.Color.LightGray;
            this._recordButton.InnerBorderColor_Focus = System.Drawing.Color.LightBlue;
            this._recordButton.InnerBorderColor_MouseOver = System.Drawing.Color.Gold;
            this._recordButton.Location = new System.Drawing.Point(11, 0);
            this._recordButton.Name = "_recordButton";
            this._recordButton.OffsetPressedContent = true;
            this._recordButton.Size = new System.Drawing.Size(44, 36);
            this._recordButton.StretchImage = false;
            this._recordButton.TabIndex = 21;
            this._recordButton.TextDropShadow = false;
            this._recordButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolTip1.SetToolTip(this._recordButton, "To record, click this or hold down Space bar while speaking");
            this._recordButton.UseVisualStyleBackColor = true;
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

        private System.Windows.Forms.Timer _levelMeterTimer;
        private BitmapButton _recordButton;
        private BitmapButton _playButton;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
