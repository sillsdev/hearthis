using System.Drawing;

namespace HearThis.UI
{
    partial class Shell
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Shell));
			this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._recordingToolControl1 = new HearThis.UI.RecordingToolControl();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this._contextMenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// _contextMenuStrip
			// 
			this._contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
			this.l10NSharpExtender1.SetLocalizableToolTip(this._contextMenuStrip, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._contextMenuStrip, null);
			this.l10NSharpExtender1.SetLocalizingId(this._contextMenuStrip, "_contextMenuStrip");
			this._contextMenuStrip.Name = "_contextMenuStrip";
			this._contextMenuStrip.Size = new System.Drawing.Size(153, 48);
			// 
			// testToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.testToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.testToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.testToolStripMenuItem, ".testToolStripMenuItem");
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			this.testToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.testToolStripMenuItem.Text = "test";
			// 
			// _recordingToolControl1
			// 
			this._recordingToolControl1.BackColor = this._recordingToolControl1.BackColor;
			this._recordingToolControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l10NSharpExtender1.SetLocalizableToolTip(this._recordingToolControl1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this._recordingToolControl1, null);
			this.l10NSharpExtender1.SetLocalizingId(this._recordingToolControl1, "Shell.RecordingToolControl");
			this._recordingToolControl1.Location = new System.Drawing.Point(0, 0);
			this._recordingToolControl1.Name = "_recordingToolControl1";
			this._recordingToolControl1.Size = new System.Drawing.Size(719, 529);
			this._recordingToolControl1.TabIndex = 1;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "HearThis";
			this.l10NSharpExtender1.PrefixForNewItems = "Shell";
			// 
			// Shell
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
			this.ClientSize = new System.Drawing.Size(719, 529);
			this.Controls.Add(this._recordingToolControl1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "Shell.WindowTitle");
			this.MinimumSize = new System.Drawing.Size(719, 534);
			this.Name = "Shell";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "HearThis";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this._contextMenuStrip.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private RecordingToolControl _recordingToolControl1;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
    }
}

