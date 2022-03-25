
namespace AssetsAdvancedEditor.Winforms
{
    partial class BatchExport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchExport));
            this.rbtnXML = new System.Windows.Forms.RadioButton();
            this.rbtnTXT = new System.Windows.Forms.RadioButton();
            this.lblXML = new System.Windows.Forms.Label();
            this.lblTXT = new System.Windows.Forms.Label();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.rbtnJSON = new System.Windows.Forms.RadioButton();
            this.lblJSON = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rbtnXML
            // 
            this.rbtnXML.AutoSize = true;
            this.rbtnXML.Location = new System.Drawing.Point(15, 51);
            this.rbtnXML.Name = "rbtnXML";
            this.rbtnXML.Size = new System.Drawing.Size(14, 13);
            this.rbtnXML.TabIndex = 17;
            this.rbtnXML.TabStop = true;
            this.rbtnXML.UseVisualStyleBackColor = true;
            // 
            // rbtnTXT
            // 
            this.rbtnTXT.AutoSize = true;
            this.rbtnTXT.Location = new System.Drawing.Point(15, 34);
            this.rbtnTXT.Name = "rbtnTXT";
            this.rbtnTXT.Size = new System.Drawing.Size(14, 13);
            this.rbtnTXT.TabIndex = 16;
            this.rbtnTXT.TabStop = true;
            this.rbtnTXT.UseVisualStyleBackColor = true;
            // 
            // lblXML
            // 
            this.lblXML.AutoSize = true;
            this.lblXML.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblXML.Location = new System.Drawing.Point(30, 48);
            this.lblXML.Name = "lblXML";
            this.lblXML.Size = new System.Drawing.Size(144, 17);
            this.lblXML.TabIndex = 15;
            this.lblXML.Text = "XML: UAAE xml dump.";
            // 
            // lblTXT
            // 
            this.lblTXT.AutoSize = true;
            this.lblTXT.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTXT.Location = new System.Drawing.Point(30, 31);
            this.lblTXT.Name = "lblTXT";
            this.lblTXT.Size = new System.Drawing.Size(142, 17);
            this.lblTXT.TabIndex = 14;
            this.lblTXT.Text = "TXT: UAAE text dump.";
            // 
            // lblQuestion
            // 
            this.lblQuestion.AutoSize = true;
            this.lblQuestion.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblQuestion.Location = new System.Drawing.Point(11, 9);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(206, 20);
            this.lblQuestion.TabIndex = 13;
            this.lblQuestion.Text = "Select a dump type to export";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(259, 97);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 25);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(12, 97);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(84, 25);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbtnJSON
            // 
            this.rbtnJSON.AutoSize = true;
            this.rbtnJSON.Location = new System.Drawing.Point(15, 68);
            this.rbtnJSON.Name = "rbtnJSON";
            this.rbtnJSON.Size = new System.Drawing.Size(14, 13);
            this.rbtnJSON.TabIndex = 19;
            this.rbtnJSON.TabStop = true;
            this.rbtnJSON.UseVisualStyleBackColor = true;
            // 
            // lblJSON
            // 
            this.lblJSON.AutoSize = true;
            this.lblJSON.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblJSON.Location = new System.Drawing.Point(30, 65);
            this.lblJSON.Name = "lblJSON";
            this.lblJSON.Size = new System.Drawing.Size(150, 17);
            this.lblJSON.TabIndex = 18;
            this.lblJSON.Text = "JSON: UAAE json dump";
            // 
            // DumpTypeDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(355, 129);
            this.Controls.Add(this.rbtnJSON);
            this.Controls.Add(this.lblJSON);
            this.Controls.Add(this.rbtnXML);
            this.Controls.Add(this.rbtnTXT);
            this.Controls.Add(this.lblXML);
            this.Controls.Add(this.lblTXT);
            this.Controls.Add(this.lblQuestion);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DumpTypeDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select a dump type";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtnXML;
        private System.Windows.Forms.RadioButton rbtnTXT;
        private System.Windows.Forms.Label lblXML;
        private System.Windows.Forms.Label lblTXT;
        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbtnJSON;
        private System.Windows.Forms.Label lblJSON;
    }
}