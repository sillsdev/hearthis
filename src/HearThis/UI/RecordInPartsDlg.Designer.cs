namespace HearThis.UI
{
	partial class RecordInPartsDlg
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._labelOne = new System.Windows.Forms.Label();
			this._labelTwo = new System.Windows.Forms.Label();
			this.labelPlus = new System.Windows.Forms.Label();
			this._cancelButton = new System.Windows.Forms.Button();
			this._useRecordingsButton = new System.Windows.Forms.Button();
			this._instructionsLabel = new System.Windows.Forms.Label();
			this._recordTextBox = new System.Windows.Forms.RichTextBox();
			this._labelBothOne = new System.Windows.Forms.Label();
			this._labelBothTwo = new System.Windows.Forms.Label();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._audioButtonsBoth = new HearThis.UI.AudioButtonsControl();
			this._audioButtonsSecond = new HearThis.UI.AudioButtonsControl();
			this._audioButtonsFirst = new HearThis.UI.AudioButtonsControl();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// _labelOne
			// 
			this._labelOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._labelOne.AutoSize = true;
			this._labelOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
			this._labelOne.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._labelOne, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._labelOne, null);
			this.l10NSharpExtender1.SetLocalizingId(this._labelOne, "RecordingControl.RecordInPartsDlg._labelOne");
			this._labelOne.Location = new System.Drawing.Point(546, 213);
			this._labelOne.Name = "_labelOne";
			this._labelOne.Size = new System.Drawing.Size(26, 29);
			this._labelOne.TabIndex = 24;
			this._labelOne.Text = "1";
			// 
			// _labelTwo
			// 
			this._labelTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._labelTwo.AutoSize = true;
			this._labelTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
			this._labelTwo.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._labelTwo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._labelTwo, null);
			this.l10NSharpExtender1.SetLocalizingId(this._labelTwo, "RecordingControl.RecordInPartsDlg._labelTwo");
			this._labelTwo.Location = new System.Drawing.Point(546, 279);
			this._labelTwo.Name = "_labelTwo";
			this._labelTwo.Size = new System.Drawing.Size(26, 29);
			this._labelTwo.TabIndex = 25;
			this._labelTwo.Text = "2";
			// 
			// labelPlus
			// 
			this.labelPlus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPlus.AutoSize = true;
			this.labelPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
			this.labelPlus.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.labelPlus, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.labelPlus, null);
			this.l10NSharpExtender1.SetLocalizingId(this.labelPlus, "RecordingControl.RecordInPartsDlg.labelPlus");
			this.labelPlus.Location = new System.Drawing.Point(528, 342);
			this.labelPlus.Name = "labelPlus";
			this.labelPlus.Size = new System.Drawing.Size(27, 29);
			this.labelPlus.TabIndex = 26;
			this.labelPlus.Text = "+";
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cancelButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cancelButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._cancelButton, "Common.Cancel");
			this._cancelButton.Location = new System.Drawing.Point(384, 396);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 27;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = false;
			// 
			// _useRecordingsButton
			// 
			this._useRecordingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._useRecordingsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this._useRecordingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._useRecordingsButton.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._useRecordingsButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._useRecordingsButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._useRecordingsButton, "RecordingControl.RecordInPartsDlg.UseRecordingsButton");
			this._useRecordingsButton.Location = new System.Drawing.Point(490, 396);
			this._useRecordingsButton.Name = "_useRecordingsButton";
			this._useRecordingsButton.Size = new System.Drawing.Size(151, 23);
			this._useRecordingsButton.TabIndex = 28;
			this._useRecordingsButton.Text = "&Use These Recordings";
			this._useRecordingsButton.UseVisualStyleBackColor = false;
			this._useRecordingsButton.Click += new System.EventHandler(this._useRecordingsButton_Click);
			// 
			// _instructionsLabel
			// 
			this._instructionsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._instructionsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this._instructionsLabel.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._instructionsLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._instructionsLabel, null);
			this.l10NSharpExtender1.SetLocalizingId(this._instructionsLabel, "RecordingControl.RecordInPartsDlg.Instructions");
			this._instructionsLabel.Location = new System.Drawing.Point(25, 25);
			this._instructionsLabel.Name = "_instructionsLabel";
			this._instructionsLabel.Size = new System.Drawing.Size(591, 45);
			this._instructionsLabel.TabIndex = 29;
			this._instructionsLabel.Text = "You can divide the line wherever you want. Just record the first part, then the s" +
    "econd part. If you want to, you can click in the text to help remember where the" +
    " second part starts.";
			// 
			// _recordTextBox
			// 
			this._recordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._recordTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._recordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._recordTextBox.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this._recordTextBox.Location = new System.Drawing.Point(28, 73);
			this._recordTextBox.Name = "_recordTextBox";
			this._recordTextBox.Size = new System.Drawing.Size(479, 306);
			this._recordTextBox.TabIndex = 30;
			this._recordTextBox.Text = "";
			// 
			// _labelBothOne
			// 
			this._labelBothOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._labelBothOne.AutoSize = true;
			this._labelBothOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
			this._labelBothOne.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._labelBothOne, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._labelBothOne, null);
			this.l10NSharpExtender1.SetLocalizingId(this._labelBothOne, "RecordingControl.RecordInPartsDlg._labelBothOne");
			this._labelBothOne.Location = new System.Drawing.Point(513, 342);
			this._labelBothOne.Name = "_labelBothOne";
			this._labelBothOne.Size = new System.Drawing.Size(26, 29);
			this._labelBothOne.TabIndex = 31;
			this._labelBothOne.Text = "1";
			// 
			// _labelBothTwo
			// 
			this._labelBothTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._labelBothTwo.AutoSize = true;
			this._labelBothTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
			this._labelBothTwo.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._labelBothTwo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._labelBothTwo, null);
			this.l10NSharpExtender1.SetLocalizingId(this._labelBothTwo, "RecordingControl.RecordInPartsDlg._labelBothTwo");
			this._labelBothTwo.Location = new System.Drawing.Point(546, 342);
			this._labelBothTwo.Name = "_labelBothTwo";
			this._labelBothTwo.Size = new System.Drawing.Size(26, 29);
			this._labelBothTwo.TabIndex = 32;
			this._labelBothTwo.Text = "2";
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "RecordingControl";
			// 
			// _audioButtonsBoth
			// 
			this._audioButtonsBoth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._audioButtonsBoth.BackColor = System.Drawing.Color.Transparent;
			this._audioButtonsBoth.ButtonHighlightMode = HearThis.UI.AudioButtonsControl.ButtonHighlightModes.Default;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._audioButtonsBoth, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._audioButtonsBoth, null);
			this.l10NSharpExtender1.SetLocalizingId(this._audioButtonsBoth, "RecordingControl.RecordInPartsDlg.AudioButtonsControl");
			this._audioButtonsBoth.Location = new System.Drawing.Point(571, 336);
			this._audioButtonsBoth.Name = "_audioButtonsBoth";
			this._audioButtonsBoth.RecordingDevice = null;
			this._audioButtonsBoth.Size = new System.Drawing.Size(37, 43);
			this._audioButtonsBoth.TabIndex = 23;
			// 
			// _audioButtonsSecond
			// 
			this._audioButtonsSecond.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._audioButtonsSecond.BackColor = System.Drawing.Color.Transparent;
			this._audioButtonsSecond.ButtonHighlightMode = HearThis.UI.AudioButtonsControl.ButtonHighlightModes.Default;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._audioButtonsSecond, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._audioButtonsSecond, null);
			this.l10NSharpExtender1.SetLocalizingId(this._audioButtonsSecond, "RecordingControl.RecordInPartsDlg.AudioButtonsControl");
			this._audioButtonsSecond.Location = new System.Drawing.Point(571, 273);
			this._audioButtonsSecond.Name = "_audioButtonsSecond";
			this._audioButtonsSecond.RecordingDevice = null;
			this._audioButtonsSecond.Size = new System.Drawing.Size(77, 43);
			this._audioButtonsSecond.TabIndex = 22;
			// 
			// _audioButtonsFirst
			// 
			this._audioButtonsFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._audioButtonsFirst.BackColor = System.Drawing.Color.Transparent;
			this._audioButtonsFirst.ButtonHighlightMode = HearThis.UI.AudioButtonsControl.ButtonHighlightModes.Default;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._audioButtonsFirst, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._audioButtonsFirst, null);
			this.l10NSharpExtender1.SetLocalizingId(this._audioButtonsFirst, "RecordingControl.RecordInPartsDlg.AudioButtonsControl");
			this._audioButtonsFirst.Location = new System.Drawing.Point(571, 207);
			this._audioButtonsFirst.Name = "_audioButtonsFirst";
			this._audioButtonsFirst.RecordingDevice = null;
			this._audioButtonsFirst.Size = new System.Drawing.Size(77, 43);
			this._audioButtonsFirst.TabIndex = 21;
			// 
			// RecordInPartsDlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(659, 441);
			this.Controls.Add(this._labelBothTwo);
			this.Controls.Add(this._recordTextBox);
			this.Controls.Add(this._instructionsLabel);
			this.Controls.Add(this._useRecordingsButton);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this.labelPlus);
			this.Controls.Add(this._labelTwo);
			this.Controls.Add(this._labelOne);
			this.Controls.Add(this._audioButtonsBoth);
			this.Controls.Add(this._audioButtonsSecond);
			this.Controls.Add(this._audioButtonsFirst);
			this.Controls.Add(this._labelBothOne);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "RecordInPartsDlg.WindowTitle");
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(600, 350);
			this.Name = "RecordInPartsDlg";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Record Long Line in Parts";
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private AudioButtonsControl _audioButtonsFirst;
		private AudioButtonsControl _audioButtonsSecond;
		private AudioButtonsControl _audioButtonsBoth;
		private System.Windows.Forms.Label _labelOne;
		private System.Windows.Forms.Label _labelTwo;
		private System.Windows.Forms.Label labelPlus;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _useRecordingsButton;
		private System.Windows.Forms.Label _instructionsLabel;
		private System.Windows.Forms.RichTextBox _recordTextBox;
		private System.Windows.Forms.Label _labelBothOne;
		private System.Windows.Forms.Label _labelBothTwo;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
	}
}