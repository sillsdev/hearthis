namespace HearThis.UI
{
	partial class UpgradeNeededDialog
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
			this._btnCheckForUpdates = new System.Windows.Forms.Button();
			this._l10NSharpExtender = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._cancelButton = new System.Windows.Forms.Button();
			this._description = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this._l10NSharpExtender)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _btnCheckForUpdates
			// 
			this._btnCheckForUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnCheckForUpdates.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._l10NSharpExtender.SetLocalizableToolTip(this._btnCheckForUpdates, null);
			this._l10NSharpExtender.SetLocalizationComment(this._btnCheckForUpdates, null);
			this._l10NSharpExtender.SetLocalizingId(this._btnCheckForUpdates, "UpgradeNeededDialog.CheckForUpdates");
			this._btnCheckForUpdates.Location = new System.Drawing.Point(340, 145);
			this._btnCheckForUpdates.Name = "_btnCheckForUpdates";
			this._btnCheckForUpdates.Size = new System.Drawing.Size(109, 23);
			this._btnCheckForUpdates.TabIndex = 0;
			this._btnCheckForUpdates.Text = "Check For Updates";
			this._btnCheckForUpdates.UseVisualStyleBackColor = true;
			this._btnCheckForUpdates.Click += new System.EventHandler(this.btnCheckForUpdates_Click);
			// 
			// _l10NSharpExtender
			// 
			this._l10NSharpExtender.LocalizationManagerId = null;
			this._l10NSharpExtender.PrefixForNewItems = null;
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._l10NSharpExtender.SetLocalizableToolTip(this._cancelButton, null);
			this._l10NSharpExtender.SetLocalizationComment(this._cancelButton, null);
			this._l10NSharpExtender.SetLocalizingId(this._cancelButton, "Common.Cancel");
			this._cancelButton.Location = new System.Drawing.Point(259, 145);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 5;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// _description
			// 
			this._description.AutoSize = true;
			this._l10NSharpExtender.SetLocalizableToolTip(this._description, null);
			this._l10NSharpExtender.SetLocalizationComment(this._description, null);
			this._l10NSharpExtender.SetLocalizationPriority(this._description, L10NSharp.LocalizationPriority.NotLocalizable);
			this._l10NSharpExtender.SetLocalizingId(this._description, "UpgradeNeededDialog.label1");
			this._description.Location = new System.Drawing.Point(3, 0);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(0, 13);
			this._description.TabIndex = 6;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Controls.Add(this._description, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(437, 127);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// UpgradeNeededDialog
			// 
			this.AcceptButton = this._btnCheckForUpdates;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(461, 180);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._btnCheckForUpdates);
			this._l10NSharpExtender.SetLocalizableToolTip(this, null);
			this._l10NSharpExtender.SetLocalizationComment(this, null);
			this._l10NSharpExtender.SetLocalizingId(this, "UpgradeNeededDialog.WindowTitle");
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(477, 219);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(477, 219);
			this.Name = "UpgradeNeededDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Need to Upgrade HearThis";
			((System.ComponentModel.ISupportInitialize)(this._l10NSharpExtender)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _btnCheckForUpdates;
		private L10NSharp.UI.L10NSharpExtender _l10NSharpExtender;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Label _description;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}