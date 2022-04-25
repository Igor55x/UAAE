namespace Plugins.Texture
{
    partial class TextureViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureViewer));
            this.lblUnsupported = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblUnsupported
            // 
            this.lblUnsupported.AutoSize = true;
            this.lblUnsupported.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblUnsupported.Location = new System.Drawing.Point(176, 194);
            this.lblUnsupported.Name = "lblUnsupported";
            this.lblUnsupported.Size = new System.Drawing.Size(353, 64);
            this.lblUnsupported.TabIndex = 0;
            this.lblUnsupported.Text = "Unsupported texture format\nor texture could not be parsed.";
            // 
            // TextureViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(728, 509);
            this.Controls.Add(this.lblUnsupported);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "TextureViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Texture Viewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TextureViewer_FormClosed);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TextureViewer_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextureViewer_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TextureViewer_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextureViewer_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.TextureViewer_MouseWheel);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUnsupported;
    }
}