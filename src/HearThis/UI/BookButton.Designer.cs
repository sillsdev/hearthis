namespace HearThis.UI
{
    partial class BookButton
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
            this._dangerousMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._makeDummyRecordings = new System.Windows.Forms.ToolStripMenuItem();
            this._dangerousMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this._dangerousMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._makeDummyRecordings});
            this._dangerousMenu.Name = "_dangerousMenu";
            this._dangerousMenu.Size = new System.Drawing.Size(253, 26);
            this._dangerousMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // _makeDummyRecordings
            // 
            this._makeDummyRecordings.Name = "_makeDummyRecordings";
            this._makeDummyRecordings.Size = new System.Drawing.Size(252, 22);
            this._makeDummyRecordings.Text = "Fill with dummy audio recordings";
            this._makeDummyRecordings.Click += new System.EventHandler(this._makeDummyRecordings_Click);
            // 
            // BookButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "BookButton";
            this.Size = new System.Drawing.Size(12, 22);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this._dangerousMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip _dangerousMenu;
        private System.Windows.Forms.ToolStripMenuItem _makeDummyRecordings;
    }
}
