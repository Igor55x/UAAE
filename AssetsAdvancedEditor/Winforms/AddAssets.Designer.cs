
namespace AssetsAdvancedEditor.Winforms
{
    partial class AddAssets
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddAssets));
            this.cboxTypePreset = new System.Windows.Forms.ComboBox();
            this.lblFileID = new System.Windows.Forms.Label();
            this.lblPathID = new System.Windows.Forms.Label();
            this.lblTypeNameOrID = new System.Windows.Forms.Label();
            this.lblMonoID = new System.Windows.Forms.Label();
            this.cboxFileID = new System.Windows.Forms.ComboBox();
            this.boxPathID = new System.Windows.Forms.TextBox();
            this.boxTypeNameOrID = new System.Windows.Forms.TextBox();
            this.boxMonoID = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.boxCount = new System.Windows.Forms.TextBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.cboxMonoTypes = new System.Windows.Forms.ComboBox();
            this.chboxCreateBlankAssets = new System.Windows.Forms.CheckBox();
            this.lblCreateBlankAssets = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboxTypePreset
            // 
            this.cboxTypePreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxTypePreset.FormattingEnabled = true;
            this.cboxTypePreset.Items.AddRange(new object[] {
            "Custom",
            "MonoBehaviour"});
            this.cboxTypePreset.Location = new System.Drawing.Point(12, 13);
            this.cboxTypePreset.Name = "cboxTypePreset";
            this.cboxTypePreset.Size = new System.Drawing.Size(290, 23);
            this.cboxTypePreset.TabIndex = 0;
            this.cboxTypePreset.SelectedIndexChanged += new System.EventHandler(this.cboxTypePreset_SelectedIndexChanged);
            // 
            // lblFileID
            // 
            this.lblFileID.AutoSize = true;
            this.lblFileID.Location = new System.Drawing.Point(12, 50);
            this.lblFileID.Name = "lblFileID";
            this.lblFileID.Size = new System.Drawing.Size(39, 15);
            this.lblFileID.TabIndex = 0;
            this.lblFileID.Text = "File ID";
            // 
            // lblPathID
            // 
            this.lblPathID.AutoSize = true;
            this.lblPathID.Location = new System.Drawing.Point(12, 77);
            this.lblPathID.Name = "lblPathID";
            this.lblPathID.Size = new System.Drawing.Size(45, 15);
            this.lblPathID.TabIndex = 1;
            this.lblPathID.Text = "Path ID";
            // 
            // lblTypeNameOrID
            // 
            this.lblTypeNameOrID.AutoSize = true;
            this.lblTypeNameOrID.Location = new System.Drawing.Point(12, 104);
            this.lblTypeNameOrID.Name = "lblTypeNameOrID";
            this.lblTypeNameOrID.Size = new System.Drawing.Size(80, 15);
            this.lblTypeNameOrID.TabIndex = 2;
            this.lblTypeNameOrID.Text = "Type name/ID";
            // 
            // lblMonoID
            // 
            this.lblMonoID.AutoSize = true;
            this.lblMonoID.Location = new System.Drawing.Point(12, 131);
            this.lblMonoID.Name = "lblMonoID";
            this.lblMonoID.Size = new System.Drawing.Size(83, 15);
            this.lblMonoID.TabIndex = 3;
            this.lblMonoID.Text = "Mono Class ID";
            // 
            // cboxFileID
            // 
            this.cboxFileID.FormattingEnabled = true;
            this.cboxFileID.Location = new System.Drawing.Point(101, 45);
            this.cboxFileID.Name = "cboxFileID";
            this.cboxFileID.Size = new System.Drawing.Size(201, 23);
            this.cboxFileID.TabIndex = 4;
            this.cboxFileID.SelectedIndexChanged += new System.EventHandler(this.cboxFileID_SelectedIndexChanged);
            // 
            // boxPathID
            // 
            this.boxPathID.Location = new System.Drawing.Point(101, 72);
            this.boxPathID.Name = "boxPathID";
            this.boxPathID.Size = new System.Drawing.Size(201, 23);
            this.boxPathID.TabIndex = 5;
            // 
            // boxTypeNameOrID
            // 
            this.boxTypeNameOrID.Location = new System.Drawing.Point(101, 99);
            this.boxTypeNameOrID.Name = "boxTypeNameOrID";
            this.boxTypeNameOrID.Size = new System.Drawing.Size(201, 23);
            this.boxTypeNameOrID.TabIndex = 6;
            this.boxTypeNameOrID.Text = "0";
            // 
            // boxMonoID
            // 
            this.boxMonoID.Location = new System.Drawing.Point(101, 126);
            this.boxMonoID.Name = "boxMonoID";
            this.boxMonoID.Size = new System.Drawing.Size(201, 23);
            this.boxMonoID.TabIndex = 7;
            this.boxMonoID.Text = "-1";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(227, 212);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(120, 212);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // boxCount
            // 
            this.boxCount.Location = new System.Drawing.Point(101, 153);
            this.boxCount.Name = "boxCount";
            this.boxCount.Size = new System.Drawing.Size(201, 23);
            this.boxCount.TabIndex = 11;
            this.boxCount.Text = "1";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(12, 158);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(40, 15);
            this.lblCount.TabIndex = 10;
            this.lblCount.Text = "Count";
            // 
            // cboxMonoTypes
            // 
            this.cboxMonoTypes.FormattingEnabled = true;
            this.cboxMonoTypes.Location = new System.Drawing.Point(101, 99);
            this.cboxMonoTypes.Name = "cboxMonoTypes";
            this.cboxMonoTypes.Size = new System.Drawing.Size(201, 23);
            this.cboxMonoTypes.TabIndex = 12;
            // 
            // chboxCreateBlankAssets
            // 
            this.chboxCreateBlankAssets.AutoSize = true;
            this.chboxCreateBlankAssets.Checked = true;
            this.chboxCreateBlankAssets.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chboxCreateBlankAssets.Location = new System.Drawing.Point(118, 186);
            this.chboxCreateBlankAssets.Name = "chboxCreateBlankAssets";
            this.chboxCreateBlankAssets.Size = new System.Drawing.Size(15, 14);
            this.chboxCreateBlankAssets.TabIndex = 13;
            this.chboxCreateBlankAssets.UseVisualStyleBackColor = true;
            // 
            // lblCreateBlankAssets
            // 
            this.lblCreateBlankAssets.AutoSize = true;
            this.lblCreateBlankAssets.Location = new System.Drawing.Point(12, 185);
            this.lblCreateBlankAssets.Name = "lblCreateBlankAssets";
            this.lblCreateBlankAssets.Size = new System.Drawing.Size(102, 15);
            this.lblCreateBlankAssets.TabIndex = 14;
            this.lblCreateBlankAssets.Text = "Create blank asset";
            // 
            // AddAssetsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(314, 247);
            this.Controls.Add(this.lblCreateBlankAssets);
            this.Controls.Add(this.chboxCreateBlankAssets);
            this.Controls.Add(this.boxCount);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.boxMonoID);
            this.Controls.Add(this.boxTypeNameOrID);
            this.Controls.Add(this.boxPathID);
            this.Controls.Add(this.cboxFileID);
            this.Controls.Add(this.lblMonoID);
            this.Controls.Add(this.lblTypeNameOrID);
            this.Controls.Add(this.lblPathID);
            this.Controls.Add(this.lblFileID);
            this.Controls.Add(this.cboxTypePreset);
            this.Controls.Add(this.cboxMonoTypes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddAssetsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add assets";
            this.Load += new System.EventHandler(this.AddAssetsDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboxTypePreset;
        private System.Windows.Forms.Label lblFileID;
        private System.Windows.Forms.Label lblPathID;
        private System.Windows.Forms.Label lblTypeNameOrID;
        private System.Windows.Forms.Label lblMonoID;
        private System.Windows.Forms.ComboBox cboxFileID;
        private System.Windows.Forms.TextBox boxPathID;
        private System.Windows.Forms.TextBox boxTypeNameOrID;
        private System.Windows.Forms.TextBox boxMonoID;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox boxCount;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.ComboBox cboxMonoTypes;
        private System.Windows.Forms.CheckBox chboxCreateBlankAssets;
        private System.Windows.Forms.Label lblCreateBlankAssets;
    }
}