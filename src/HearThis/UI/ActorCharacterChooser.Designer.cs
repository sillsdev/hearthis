namespace HearThis.UI
{
	partial class ActorCharacterChooser
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
			this._actorList = new System.Windows.Forms.ListBox();
			this._characterList = new System.Windows.Forms.ListBox();
			this._okButton = new System.Windows.Forms.Button();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// _actorList
			// 
			this._actorList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._actorList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._actorList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._actorList.ForeColor = System.Drawing.Color.White;
			this._actorList.FormattingEnabled = true;
			this._actorList.ItemHeight = 16;
			this._actorList.Location = new System.Drawing.Point(16, 79);
			this._actorList.Name = "_actorList";
			this._actorList.Size = new System.Drawing.Size(154, 224);
			this._actorList.TabIndex = 1;
			// 
			// _characterList
			// 
			this._characterList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this._characterList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._characterList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._characterList.ForeColor = System.Drawing.Color.White;
			this._characterList.FormattingEnabled = true;
			this._characterList.ItemHeight = 16;
			this._characterList.Location = new System.Drawing.Point(204, 79);
			this._characterList.Name = "_characterList";
			this._characterList.Size = new System.Drawing.Size(157, 224);
			this._characterList.TabIndex = 2;
			// 
			// _okButton
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this._okButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._okButton, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this._okButton, L10NSharp.LocalizationPriority.High);
			this.l10NSharpExtender1.SetLocalizingId(this._okButton, "Common.OK");
			this._okButton.Location = new System.Drawing.Point(281, 330);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 3;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox2.Image = global::HearThis.Properties.Resources.characters;
			this.pictureBox2.InitialImage = null;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.pictureBox2, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.pictureBox2, null);
			this.l10NSharpExtender1.SetLocalizingId(this.pictureBox2, "ActorCharacterChooser.pictureBox2");
			this.pictureBox2.Location = new System.Drawing.Point(256, 16);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(57, 45);
			this.pictureBox2.TabIndex = 4;
			this.pictureBox2.TabStop = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox1.Image = global::HearThis.Properties.Resources.speakIntoMike75x50;
			this.pictureBox1.InitialImage = null;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.pictureBox1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.pictureBox1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.pictureBox1, "ActorCharacterChooser.pictureBox1");
			this.pictureBox1.Location = new System.Drawing.Point(64, 16);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(85, 45);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "";
			// 
			// ActorCharacterChooser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._characterList);
			this.Controls.Add(this._actorList);
			this.Controls.Add(this.pictureBox1);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "ActorCharacterChooser.ActorCharacterChooser");
			this.Name = "ActorCharacterChooser";
			this.Size = new System.Drawing.Size(374, 364);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ListBox _actorList;
		private System.Windows.Forms.ListBox _characterList;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.PictureBox pictureBox2;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
	}
}
