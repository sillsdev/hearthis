namespace HearThis.UI
{
	partial class SaveHearThisPackDlg
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveHearThisPackDlg));
			this.aboutHearThisPack = new System.Windows.Forms.Label();
			this.aboutRestrictToCharacter = new System.Windows.Forms.Label();
			this._limitToCurrentActor = new System.Windows.Forms.CheckBox();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// aboutHearThisPack
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.aboutHearThisPack, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.aboutHearThisPack, null);
			this.l10NSharpExtender1.SetLocalizingId(this.aboutHearThisPack, "SaveHearThisPackDlg.AboutHearThisPack");
			this.aboutHearThisPack.Location = new System.Drawing.Point(12, 9);
			this.aboutHearThisPack.Name = "aboutHearThisPack";
			this.aboutHearThisPack.Size = new System.Drawing.Size(430, 49);
			this.aboutHearThisPack.TabIndex = 0;
			this.aboutHearThisPack.Text = "A HearThis Pack compresses all your recordings into a single file that can be sha" +
    "red with others working on the same project. Make sure you have the same exact v" +
    "ersion of the project!";
			// 
			// aboutRestrictToCharacter
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.aboutRestrictToCharacter, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.aboutRestrictToCharacter, null);
			this.l10NSharpExtender1.SetLocalizingId(this.aboutRestrictToCharacter, "SaveHearThisPackDlg.AboutRestrictToCharacter");
			this.aboutRestrictToCharacter.Location = new System.Drawing.Point(12, 58);
			this.aboutRestrictToCharacter.Name = "aboutRestrictToCharacter";
			this.aboutRestrictToCharacter.Size = new System.Drawing.Size(430, 47);
			this.aboutRestrictToCharacter.TabIndex = 1;
			this.aboutRestrictToCharacter.Text = resources.GetString("aboutRestrictToCharacter.Text");
			// 
			// _limitToCurrentActor
			// 
			this._limitToCurrentActor.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._limitToCurrentActor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._limitToCurrentActor, null);
			this.l10NSharpExtender1.SetLocalizingId(this._limitToCurrentActor, "SaveHearThisPackDlg.LimitToCurrentActor");
			this._limitToCurrentActor.Location = new System.Drawing.Point(17, 113);
			this._limitToCurrentActor.Name = "_limitToCurrentActor";
			this._limitToCurrentActor.Size = new System.Drawing.Size(142, 17);
			this._limitToCurrentActor.TabIndex = 2;
			this._limitToCurrentActor.Text = "Limit to current actor, {0}";
			this._limitToCurrentActor.UseVisualStyleBackColor = true;
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._okButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._okButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._okButton, "Common.OK");
			this._okButton.Location = new System.Drawing.Point(354, 133);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 3;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._cancelButton, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._cancelButton, null);
			this.l10NSharpExtender1.SetLocalizingId(this._cancelButton, "Common.Cancel");
			this._cancelButton.Location = new System.Drawing.Point(250, 133);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 4;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "";
			// 
			// SaveHearThisPackDlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(454, 179);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._limitToCurrentActor);
			this.Controls.Add(this.aboutRestrictToCharacter);
			this.Controls.Add(this.aboutHearThisPack);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "SaveHearThisPackDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SaveHearThisPackDlg";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Save HearThis Pack";
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label aboutHearThisPack;
		private System.Windows.Forms.Label aboutRestrictToCharacter;
		private System.Windows.Forms.CheckBox _limitToCurrentActor;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
	}
}