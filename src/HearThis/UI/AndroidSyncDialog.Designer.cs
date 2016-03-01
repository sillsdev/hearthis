namespace HearThis.UI
{
	partial class AndroidSyncDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.qrBox = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this._ipAddressBox = new System.Windows.Forms.TextBox();
			this._syncButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.qrBox)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(295, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "To sync with Android: tap Menu button, Sync, scan this code";
			// 
			// qrBox
			// 
			this.qrBox.Location = new System.Drawing.Point(45, 75);
			this.qrBox.Name = "qrBox";
			this.qrBox.Size = new System.Drawing.Size(300, 300);
			this.qrBox.TabIndex = 1;
			this.qrBox.TabStop = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 412);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(351, 32);
			this.label3.TabIndex = 3;
			this.label3.Text = "Or you can enter the code from the Android Synchronization screen here and then c" +
    "lick the button below";
			// 
			// _ipAddressBox
			// 
			this._ipAddressBox.Location = new System.Drawing.Point(15, 457);
			this._ipAddressBox.Name = "_ipAddressBox";
			this._ipAddressBox.Size = new System.Drawing.Size(348, 20);
			this._ipAddressBox.TabIndex = 4;
			// 
			// _syncButton
			// 
			this._syncButton.Location = new System.Drawing.Point(139, 483);
			this._syncButton.Name = "_syncButton";
			this._syncButton.Size = new System.Drawing.Size(75, 23);
			this._syncButton.TabIndex = 5;
			this._syncButton.Text = "Synchronize";
			this._syncButton.UseVisualStyleBackColor = true;
			this._syncButton.Click += new System.EventHandler(this._syncButton_Click);
			// 
			// AndroidSyncDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(405, 516);
			this.Controls.Add(this._syncButton);
			this.Controls.Add(this._ipAddressBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.qrBox);
			this.Controls.Add(this.label1);
			this.MinimizeBox = false;
			this.Name = "AndroidSyncDialog";
			this.Text = "Synchronize with HearThis for Android";
			this.Load += new System.EventHandler(this.AndroidSyncDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.qrBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox qrBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _ipAddressBox;
		private System.Windows.Forms.Button _syncButton;
	}
}