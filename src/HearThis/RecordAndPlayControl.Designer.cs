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
            this._playButton = new HearThis.ImageButton();
            this._recordButton = new HearThis.ImageButton();
            this.SuspendLayout();
            // 
            // _playButton
            // 
            this._playButton.Location = new System.Drawing.Point(2, 41);
            this._playButton.Name = "_playButton";
            this._playButton.Size = new System.Drawing.Size(81, 50);
            this._playButton.TabIndex = 20;
            this._playButton.Click += new System.EventHandler(this._playButton_Click);
            // 
            // _recordButton
            // 
            this._recordButton.Location = new System.Drawing.Point(2, 2);
            this._recordButton.Name = "_recordButton";
            this._recordButton.Size = new System.Drawing.Size(81, 50);
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
            this.Size = new System.Drawing.Size(79, 90);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageButton _playButton;
        private ImageButton _recordButton;
    }
}
