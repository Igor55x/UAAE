namespace AssetsAdvancedEditor.Winforms
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.programName = new System.Windows.Forms.Label();
            this.shortDescription = new System.Windows.Forms.Label();
            this.secondLink = new System.Windows.Forms.LinkLabel();
            this.firstLink = new System.Windows.Forms.LinkLabel();
            this.hint = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // programName
            // 
            this.programName.AutoSize = true;
            this.programName.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.programName.Location = new System.Drawing.Point(9, 6);
            this.programName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.programName.Name = "programName";
            this.programName.Size = new System.Drawing.Size(355, 25);
            this.programName.TabIndex = 0;
            this.programName.Text = "Unity Assets Advanced Editor v1.3";
            // 
            // shortDescription
            // 
            this.shortDescription.AutoSize = true;
            this.shortDescription.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.shortDescription.Location = new System.Drawing.Point(18, 40);
            this.shortDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.shortDescription.Name = "shortDescription";
            this.shortDescription.Size = new System.Drawing.Size(80, 21);
            this.shortDescription.TabIndex = 1;
            this.shortDescription.Text = "By Igor55";
            // 
            // secondLink
            // 
            this.secondLink.AutoSize = true;
            this.secondLink.Location = new System.Drawing.Point(20, 85);
            this.secondLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.secondLink.Name = "secondLink";
            this.secondLink.Size = new System.Drawing.Size(308, 15);
            this.secondLink.TabIndex = 3;
            this.secondLink.TabStop = true;
            this.secondLink.Text = "https://community.7daystodie.com/profile/418-derpopo";
            this.secondLink.VisitedLinkColor = System.Drawing.Color.Blue;
            this.secondLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.secondLink_LinkClicked);
            // 
            // firstLink
            // 
            this.firstLink.AutoSize = true;
            this.firstLink.Location = new System.Drawing.Point(20, 70);
            this.firstLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.firstLink.Name = "firstLink";
            this.firstLink.Size = new System.Drawing.Size(153, 15);
            this.firstLink.TabIndex = 2;
            this.firstLink.TabStop = true;
            this.firstLink.Text = "https://github.com/Igor55x";
            this.firstLink.VisitedLinkColor = System.Drawing.Color.Blue;
            this.firstLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.firstLink_LinkClicked);
            // 
            // hint
            // 
            this.hint.AutoSize = true;
            this.hint.Location = new System.Drawing.Point(12, 119);
            this.hint.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.hint.Name = "hint";
            this.hint.Size = new System.Drawing.Size(217, 15);
            this.hint.TabIndex = 4;
            this.hint.Text = "See README.txt for license information.";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(326, 113);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 27);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // About
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(418, 144);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.hint);
            this.Controls.Add(this.firstLink);
            this.Controls.Add(this.secondLink);
            this.Controls.Add(this.shortDescription);
            this.Controls.Add(this.programName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Unity Assets Advanced Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label programName;
        private System.Windows.Forms.Label shortDescription;
        private System.Windows.Forms.LinkLabel secondLink;
        private System.Windows.Forms.LinkLabel firstLink;
        private System.Windows.Forms.Label hint;
        private System.Windows.Forms.Button btnOK;
    }
}