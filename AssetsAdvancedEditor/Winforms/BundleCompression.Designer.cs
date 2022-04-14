
namespace AssetsAdvancedEditor.Winforms
{
    partial class BundleCompression
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BundleCompression));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.lblLZ4 = new System.Windows.Forms.Label();
            this.lblLZMA = new System.Windows.Forms.Label();
            this.rbtnLZ4 = new System.Windows.Forms.RadioButton();
            this.rbtnLZMA = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(259, 77);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 25);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(12, 77);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(84, 25);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // lblQuestion
            // 
            this.lblQuestion.AutoSize = true;
            this.lblQuestion.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblQuestion.Location = new System.Drawing.Point(11, 6);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(333, 20);
            this.lblQuestion.TabIndex = 6;
            this.lblQuestion.Text = "What compression method do you want to use?";
            // 
            // lblLZ4
            // 
            this.lblLZ4.AutoSize = true;
            this.lblLZ4.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblLZ4.Location = new System.Drawing.Point(30, 28);
            this.lblLZ4.Name = "lblLZ4";
            this.lblLZ4.Size = new System.Drawing.Size(166, 17);
            this.lblLZ4.TabIndex = 7;
            this.lblLZ4.Text = "LZ4: Faster but larger size.";
            // 
            // lblLZMA
            // 
            this.lblLZMA.AutoSize = true;
            this.lblLZMA.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblLZMA.Location = new System.Drawing.Point(30, 45);
            this.lblLZMA.Name = "lblLZMA";
            this.lblLZMA.Size = new System.Drawing.Size(191, 17);
            this.lblLZMA.TabIndex = 8;
            this.lblLZMA.Text = "LZMA: Slower but smaller size.";
            // 
            // rbtnLZ4
            // 
            this.rbtnLZ4.AutoSize = true;
            this.rbtnLZ4.Location = new System.Drawing.Point(15, 31);
            this.rbtnLZ4.Name = "rbtnLZ4";
            this.rbtnLZ4.Size = new System.Drawing.Size(14, 13);
            this.rbtnLZ4.TabIndex = 9;
            this.rbtnLZ4.TabStop = true;
            this.rbtnLZ4.UseVisualStyleBackColor = true;
            // 
            // rbtnLZMA
            // 
            this.rbtnLZMA.AutoSize = true;
            this.rbtnLZMA.Location = new System.Drawing.Point(15, 48);
            this.rbtnLZMA.Name = "rbtnLZMA";
            this.rbtnLZMA.Size = new System.Drawing.Size(14, 13);
            this.rbtnLZMA.TabIndex = 10;
            this.rbtnLZMA.TabStop = true;
            this.rbtnLZMA.UseVisualStyleBackColor = true;
            // 
            // BundleCompression
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(355, 114);
            this.Controls.Add(this.rbtnLZMA);
            this.Controls.Add(this.rbtnLZ4);
            this.Controls.Add(this.lblLZMA);
            this.Controls.Add(this.lblLZ4);
            this.Controls.Add(this.lblQuestion);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BundleCompression";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bundle compression";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.Label lblLZ4;
        private System.Windows.Forms.Label lblLZMA;
        private System.Windows.Forms.RadioButton rbtnLZ4;
        private System.Windows.Forms.RadioButton rbtnLZMA;
    }
}