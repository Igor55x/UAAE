
namespace AssetsAdvancedEditor.Winforms
{
    partial class BundleDecompression
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BundleDecompression));
            this.rbtnMemory = new System.Windows.Forms.RadioButton();
            this.rbtnFile = new System.Windows.Forms.RadioButton();
            this.lblMemory = new System.Windows.Forms.Label();
            this.lblFile = new System.Windows.Forms.Label();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbtnMemory
            // 
            this.rbtnMemory.AutoSize = true;
            this.rbtnMemory.Location = new System.Drawing.Point(15, 51);
            this.rbtnMemory.Name = "rbtnMemory";
            this.rbtnMemory.Size = new System.Drawing.Size(14, 13);
            this.rbtnMemory.TabIndex = 17;
            this.rbtnMemory.TabStop = true;
            this.rbtnMemory.UseVisualStyleBackColor = true;
            // 
            // rbtnFile
            // 
            this.rbtnFile.AutoSize = true;
            this.rbtnFile.Location = new System.Drawing.Point(15, 34);
            this.rbtnFile.Name = "rbtnFile";
            this.rbtnFile.Size = new System.Drawing.Size(14, 13);
            this.rbtnFile.TabIndex = 16;
            this.rbtnFile.TabStop = true;
            this.rbtnFile.UseVisualStyleBackColor = true;
            // 
            // lblMemory
            // 
            this.lblMemory.AutoSize = true;
            this.lblMemory.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMemory.Location = new System.Drawing.Point(30, 48);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(286, 17);
            this.lblMemory.TabIndex = 15;
            this.lblMemory.Text = "Memory: Decompress the bundle to memory.";
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFile.Location = new System.Drawing.Point(30, 31);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(220, 17);
            this.lblFile.TabIndex = 14;
            this.lblFile.Text = "File: Decompress the bundle to file.";
            // 
            // lblQuestion
            // 
            this.lblQuestion.AutoSize = true;
            this.lblQuestion.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblQuestion.Location = new System.Drawing.Point(11, 9);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(327, 20);
            this.lblQuestion.TabIndex = 13;
            this.lblQuestion.Text = "What decompression type do you want to use?";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(259, 80);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 25);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(12, 80);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(84, 25);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BundleDecompression
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(355, 114);
            this.Controls.Add(this.rbtnMemory);
            this.Controls.Add(this.rbtnFile);
            this.Controls.Add(this.lblMemory);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.lblQuestion);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BundleDecompression";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bundle decompression";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtnMemory;
        private System.Windows.Forms.RadioButton rbtnFile;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}