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
            this._lblAboutHearThisPack = new System.Windows.Forms.Label();
            this._lblAboutRestrictToCharacter = new System.Windows.Forms.Label();
            this._limitToCurrentActor = new System.Windows.Forms.CheckBox();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lblAboutHearThisPack
            // 
            this._lblAboutHearThisPack.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this._lblAboutHearThisPack, 3);
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblAboutHearThisPack, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblAboutHearThisPack, "Param is \"HearThis\" (product name)");
            this.l10NSharpExtender1.SetLocalizingId(this._lblAboutHearThisPack, "SaveHearThisPackDlg.AboutHearThisPack");
            this._lblAboutHearThisPack.Location = new System.Drawing.Point(3, 0);
            this._lblAboutHearThisPack.Name = "_lblAboutHearThisPack";
            this._lblAboutHearThisPack.Size = new System.Drawing.Size(436, 39);
            this._lblAboutHearThisPack.TabIndex = 0;
            this._lblAboutHearThisPack.Text = resources.GetString("_lblAboutHearThisPack.Text");
            // 
            // _lblAboutRestrictToCharacter
            // 
            this._lblAboutRestrictToCharacter.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this._lblAboutRestrictToCharacter, 3);
            this.l10NSharpExtender1.SetLocalizableToolTip(this._lblAboutRestrictToCharacter, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._lblAboutRestrictToCharacter, null);
            this.l10NSharpExtender1.SetLocalizingId(this._lblAboutRestrictToCharacter, "SaveHearThisPackDlg.AboutRestrictToCharacter");
            this._lblAboutRestrictToCharacter.Location = new System.Drawing.Point(3, 49);
            this._lblAboutRestrictToCharacter.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this._lblAboutRestrictToCharacter.Name = "_lblAboutRestrictToCharacter";
            this._lblAboutRestrictToCharacter.Size = new System.Drawing.Size(440, 39);
            this._lblAboutRestrictToCharacter.TabIndex = 1;
            this._lblAboutRestrictToCharacter.Text = resources.GetString("_lblAboutRestrictToCharacter.Text");
            // 
            // _limitToCurrentActor
            // 
            this._limitToCurrentActor.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this._limitToCurrentActor, 3);
            this.l10NSharpExtender1.SetLocalizableToolTip(this._limitToCurrentActor, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._limitToCurrentActor, "Parameter is an actor name or \"unassigned\"");
            this.l10NSharpExtender1.SetLocalizingId(this._limitToCurrentActor, "SaveHearThisPackDlg.LimitToCurrentActor");
            this._limitToCurrentActor.Location = new System.Drawing.Point(3, 98);
            this._limitToCurrentActor.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this._limitToCurrentActor.Name = "_limitToCurrentActor";
            this._limitToCurrentActor.Size = new System.Drawing.Size(142, 17);
            this._limitToCurrentActor.TabIndex = 2;
            this._limitToCurrentActor.Text = "Limit to current actor: {0}";
            this._limitToCurrentActor.UseVisualStyleBackColor = true;
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._okButton, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._okButton, null);
            this.l10NSharpExtender1.SetLocalizingId(this._okButton, "Common.OK");
            this._okButton.Location = new System.Drawing.Point(374, 171);
            this._okButton.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 3;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._cancelButton, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._cancelButton, null);
            this.l10NSharpExtender1.SetLocalizingId(this._cancelButton, "Common.Cancel");
            this._cancelButton.Location = new System.Drawing.Point(293, 171);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 4;
            this._cancelButton.Text = "&Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // l10NSharpExtender1
            // 
            this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
            this.l10NSharpExtender1.PrefixForNewItems = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this._okButton, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this._lblAboutHearThisPack, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._lblAboutRestrictToCharacter, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._cancelButton, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this._limitToCurrentActor, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(8, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(452, 197);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // SaveHearThisPackDlg
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(468, 213);
            this.Controls.Add(this.tableLayoutPanel1);
            this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
            this.l10NSharpExtender1.SetLocalizationComment(this, null);
            this.l10NSharpExtender1.SetLocalizingId(this, "SaveHearThisPackDlg.WindowTitle");
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(454, 214);
            this.Name = "SaveHearThisPackDlg";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Save HearThis Pack";
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label _lblAboutHearThisPack;
		private System.Windows.Forms.Label _lblAboutRestrictToCharacter;
		private System.Windows.Forms.CheckBox _limitToCurrentActor;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
