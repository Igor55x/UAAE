namespace AssetsAdvancedEditor.Winforms
{
    partial class BundleLoader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BundleLoader));
            this.btnDecompress = new System.Windows.Forms.Button();
            this.btnCompress = new System.Windows.Forms.Button();
            this.lblCompressionMethod = new System.Windows.Forms.Label();
            this.lblNote = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblCompType = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.lblBundleSize = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnDecompress
            // 
            this.btnDecompress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDecompress.Enabled = false;
            this.btnDecompress.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnDecompress.Location = new System.Drawing.Point(18, 96);
            this.btnDecompress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDecompress.Name = "btnDecompress";
            this.btnDecompress.Size = new System.Drawing.Size(185, 36);
            this.btnDecompress.TabIndex = 1;
            this.btnDecompress.Text = "Decompress Bundle";
            this.btnDecompress.UseVisualStyleBackColor = true;
            this.btnDecompress.Click += new System.EventHandler(this.BtnDecompress_Click);
            // 
            // btnCompress
            // 
            this.btnCompress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompress.Enabled = false;
            this.btnCompress.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCompress.Location = new System.Drawing.Point(212, 96);
            this.btnCompress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(185, 36);
            this.btnCompress.TabIndex = 2;
            this.btnCompress.Text = "Compress Bundle";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.BtnCompress_Click);
            // 
            // lblCompressionMethod
            // 
            this.lblCompressionMethod.AutoSize = true;
            this.lblCompressionMethod.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCompressionMethod.Location = new System.Drawing.Point(113, 10);
            this.lblCompressionMethod.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCompressionMethod.Name = "lblCompressionMethod";
            this.lblCompressionMethod.Size = new System.Drawing.Size(142, 17);
            this.lblCompressionMethod.TabIndex = 4;
            this.lblCompressionMethod.Text = "Compression Method: ";
            // 
            // lblNote
            // 
            this.lblNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNote.AutoSize = true;
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblNote.Location = new System.Drawing.Point(16, 33);
            this.lblNote.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(37, 17);
            this.lblNote.TabIndex = 6;
            this.lblNote.Text = "Note";
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Enabled = false;
            this.btnLoad.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnLoad.Location = new System.Drawing.Point(18, 52);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(379, 35);
            this.btnLoad.TabIndex = 7;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // lblCompType
            // 
            this.lblCompType.AutoSize = true;
            this.lblCompType.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCompType.Location = new System.Drawing.Point(246, 10);
            this.lblCompType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCompType.Name = "lblCompType";
            this.lblCompType.Size = new System.Drawing.Size(41, 17);
            this.lblCompType.TabIndex = 8;
            this.lblCompType.Text = "None";
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSize.Location = new System.Drawing.Point(16, 10);
            this.lblSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(38, 17);
            this.lblSize.TabIndex = 9;
            this.lblSize.Text = "Size: ";
            // 
            // lblBundleSize
            // 
            this.lblBundleSize.AutoSize = true;
            this.lblBundleSize.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBundleSize.Location = new System.Drawing.Point(46, 10);
            this.lblBundleSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBundleSize.Name = "lblBundleSize";
            this.lblBundleSize.Size = new System.Drawing.Size(15, 17);
            this.lblBundleSize.TabIndex = 10;
            this.lblBundleSize.Text = "0";
            // 
            // BundleLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(414, 144);
            this.Controls.Add(this.lblBundleSize);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.btnCompress);
            this.Controls.Add(this.lblCompType);
            this.Controls.Add(this.btnDecompress);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.lblCompressionMethod);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "BundleLoader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bundle Loader";
            this.Load += new System.EventHandler(this.BundleLoader_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnDecompress;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.Label lblCompressionMethod;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblCompType;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label lblBundleSize;
    }
}