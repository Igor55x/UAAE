namespace AssetsAdvancedEditor.Winforms
{
    partial class Menu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuLoadPackageFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuCompress = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuModMaker = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuCreateStandaloneexeInstaller = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuCreatePackageFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEditTypeDatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEditTypePackage = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.lblFileName = new System.Windows.Forms.Label();
            this.cboxBundleContents = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnExportAll = new System.Windows.Forms.Button();
            this.btnImportAll = new System.Windows.Forms.Button();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.MenuOptions,
            this.MenuHelp});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.MenuStrip.Size = new System.Drawing.Size(385, 24);
            this.MenuStrip.TabIndex = 0;
            this.MenuStrip.Text = "MenuStrip";
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuOpen,
            this.MenuLoadPackageFile,
            this.MenuClose,
            this.MenuSave,
            this.MenuSeparator1,
            this.MenuCompress,
            this.MenuModMaker,
            this.MenuExit});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 20);
            this.MenuFile.Text = "&File";
            // 
            // MenuOpen
            // 
            this.MenuOpen.Name = "MenuOpen";
            this.MenuOpen.Size = new System.Drawing.Size(168, 22);
            this.MenuOpen.Text = "&Open";
            this.MenuOpen.Click += new System.EventHandler(this.MenuOpen_Click);
            // 
            // MenuLoadPackageFile
            // 
            this.MenuLoadPackageFile.Enabled = false;
            this.MenuLoadPackageFile.Name = "MenuLoadPackageFile";
            this.MenuLoadPackageFile.Size = new System.Drawing.Size(168, 22);
            this.MenuLoadPackageFile.Text = "&Load Package File";
            // 
            // MenuClose
            // 
            this.MenuClose.Enabled = false;
            this.MenuClose.Name = "MenuClose";
            this.MenuClose.Size = new System.Drawing.Size(168, 22);
            this.MenuClose.Text = "&Close";
            this.MenuClose.Click += new System.EventHandler(this.MenuClose_Click);
            // 
            // MenuSave
            // 
            this.MenuSave.Enabled = false;
            this.MenuSave.Name = "MenuSave";
            this.MenuSave.Size = new System.Drawing.Size(168, 22);
            this.MenuSave.Text = "&Save";
            this.MenuSave.Click += new System.EventHandler(this.MenuSave_Click);
            // 
            // MenuSeparator1
            // 
            this.MenuSeparator1.Name = "MenuSeparator1";
            this.MenuSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // MenuCompress
            // 
            this.MenuCompress.Enabled = false;
            this.MenuCompress.Name = "MenuCompress";
            this.MenuCompress.Size = new System.Drawing.Size(168, 22);
            this.MenuCompress.Text = "&Compress";
            this.MenuCompress.Click += new System.EventHandler(this.MenuCompress_Click);
            // 
            // MenuModMaker
            // 
            this.MenuModMaker.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuCreateStandaloneexeInstaller,
            this.MenuCreatePackageFile});
            this.MenuModMaker.Enabled = false;
            this.MenuModMaker.Name = "MenuModMaker";
            this.MenuModMaker.Size = new System.Drawing.Size(168, 22);
            this.MenuModMaker.Text = "Mod Maker";
            // 
            // MenuCreateStandaloneexeInstaller
            // 
            this.MenuCreateStandaloneexeInstaller.Name = "MenuCreateStandaloneexeInstaller";
            this.MenuCreateStandaloneexeInstaller.Size = new System.Drawing.Size(237, 22);
            this.MenuCreateStandaloneexeInstaller.Text = "Create standalone .exe installer";
            // 
            // MenuCreatePackageFile
            // 
            this.MenuCreatePackageFile.Name = "MenuCreatePackageFile";
            this.MenuCreatePackageFile.Size = new System.Drawing.Size(237, 22);
            this.MenuCreatePackageFile.Text = "Create package file";
            // 
            // MenuExit
            // 
            this.MenuExit.Name = "MenuExit";
            this.MenuExit.Size = new System.Drawing.Size(168, 22);
            this.MenuExit.Text = "&Exit";
            this.MenuExit.Click += new System.EventHandler(this.MenuExit_Click);
            // 
            // MenuOptions
            // 
            this.MenuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuEditTypeDatabase,
            this.MenuEditTypePackage});
            this.MenuOptions.Name = "MenuOptions";
            this.MenuOptions.Size = new System.Drawing.Size(61, 20);
            this.MenuOptions.Text = "&Options";
            // 
            // MenuEditTypeDatabase
            // 
            this.MenuEditTypeDatabase.Enabled = false;
            this.MenuEditTypeDatabase.Name = "MenuEditTypeDatabase";
            this.MenuEditTypeDatabase.Size = new System.Drawing.Size(172, 22);
            this.MenuEditTypeDatabase.Text = "Edit Type &Database";
            // 
            // MenuEditTypePackage
            // 
            this.MenuEditTypePackage.Enabled = false;
            this.MenuEditTypePackage.Name = "MenuEditTypePackage";
            this.MenuEditTypePackage.Size = new System.Drawing.Size(172, 22);
            this.MenuEditTypePackage.Text = "Edit Type &Package";
            // 
            // MenuHelp
            // 
            this.MenuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuAbout});
            this.MenuHelp.Name = "MenuHelp";
            this.MenuHelp.Size = new System.Drawing.Size(44, 20);
            this.MenuHelp.Text = "&Help";
            // 
            // MenuAbout
            // 
            this.MenuAbout.Name = "MenuAbout";
            this.MenuAbout.Size = new System.Drawing.Size(116, 22);
            this.MenuAbout.Text = "&About...";
            this.MenuAbout.Click += new System.EventHandler(this.MenuAbout_Click);
            // 
            // lblFileName
            // 
            this.lblFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFileName.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFileName.Location = new System.Drawing.Point(14, 22);
            this.lblFileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(357, 19);
            this.lblFileName.TabIndex = 1;
            this.lblFileName.Text = "No file opened.";
            this.lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cboxBundleContents
            // 
            this.cboxBundleContents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboxBundleContents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxBundleContents.Enabled = false;
            this.cboxBundleContents.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cboxBundleContents.FormattingEnabled = true;
            this.cboxBundleContents.Location = new System.Drawing.Point(14, 47);
            this.cboxBundleContents.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cboxBundleContents.Name = "cboxBundleContents";
            this.cboxBundleContents.Size = new System.Drawing.Size(358, 25);
            this.cboxBundleContents.TabIndex = 2;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Enabled = false;
            this.btnExport.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnExport.Location = new System.Drawing.Point(13, 78);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(87, 33);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Enabled = false;
            this.btnImport.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnImport.Location = new System.Drawing.Point(104, 78);
            this.btnImport.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(87, 33);
            this.btnImport.TabIndex = 4;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.BtnImport_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Enabled = false;
            this.btnRemove.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnRemove.Location = new System.Drawing.Point(195, 78);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(87, 33);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.BtnRemove_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfo.Enabled = false;
            this.btnInfo.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnInfo.Location = new System.Drawing.Point(286, 78);
            this.btnInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(87, 33);
            this.btnInfo.TabIndex = 5;
            this.btnInfo.Text = "Info";
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.BtnInfo_Click);
            // 
            // btnExportAll
            // 
            this.btnExportAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportAll.Enabled = false;
            this.btnExportAll.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnExportAll.Location = new System.Drawing.Point(13, 113);
            this.btnExportAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExportAll.Name = "btnExportAll";
            this.btnExportAll.Size = new System.Drawing.Size(178, 33);
            this.btnExportAll.TabIndex = 7;
            this.btnExportAll.Text = "Export all";
            this.btnExportAll.UseVisualStyleBackColor = true;
            this.btnExportAll.Click += new System.EventHandler(this.BtnExportAll_Click);
            // 
            // btnImportAll
            // 
            this.btnImportAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportAll.Enabled = false;
            this.btnImportAll.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnImportAll.Location = new System.Drawing.Point(195, 113);
            this.btnImportAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnImportAll.Name = "btnImportAll";
            this.btnImportAll.Size = new System.Drawing.Size(178, 33);
            this.btnImportAll.TabIndex = 8;
            this.btnImportAll.Text = "Import all";
            this.btnImportAll.UseVisualStyleBackColor = true;
            this.btnImportAll.Click += new System.EventHandler(this.BtnImportAll_Click);
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(385, 152);
            this.Controls.Add(this.btnImportAll);
            this.Controls.Add(this.btnInfo);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.cboxBundleContents);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.MenuStrip);
            this.Controls.Add(this.btnExportAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Unity Assets Advanced Editor";
            this.Load += new System.EventHandler(this.Menu_Load);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem MenuOpen;
        private System.Windows.Forms.ToolStripMenuItem MenuLoadPackageFile;
        private System.Windows.Forms.ToolStripMenuItem MenuClose;
        private System.Windows.Forms.ToolStripMenuItem MenuSave;
        private System.Windows.Forms.ToolStripSeparator MenuSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MenuCompress;
        private System.Windows.Forms.ToolStripMenuItem MenuModMaker;
        private System.Windows.Forms.ToolStripMenuItem MenuCreateStandaloneexeInstaller;
        private System.Windows.Forms.ToolStripMenuItem MenuCreatePackageFile;
        private System.Windows.Forms.ToolStripMenuItem MenuExit;
        private System.Windows.Forms.ToolStripMenuItem MenuOptions;
        private System.Windows.Forms.ToolStripMenuItem MenuEditTypeDatabase;
        private System.Windows.Forms.ToolStripMenuItem MenuEditTypePackage;
        private System.Windows.Forms.ToolStripMenuItem MenuHelp;
        private System.Windows.Forms.ToolStripMenuItem MenuAbout;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.ComboBox cboxBundleContents;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Button btnExportAll;
        private System.Windows.Forms.Button btnImportAll;
    }
}

