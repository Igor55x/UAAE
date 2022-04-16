namespace AssetsAdvancedEditor.Winforms
{
    partial class AssetsViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssetsViewer));
            this.lblAssets = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.boxName = new System.Windows.Forms.TextBox();
            this.boxPathID = new System.Windows.Forms.TextBox();
            this.lblPathID = new System.Windows.Forms.Label();
            this.lblFileID = new System.Windows.Forms.Label();
            this.boxFileID = new System.Windows.Forms.TextBox();
            this.lblType = new System.Windows.Forms.Label();
            this.boxType = new System.Windows.Forms.TextBox();
            this.btnViewData = new System.Windows.Forms.Button();
            this.btnExportRaw = new System.Windows.Forms.Button();
            this.btnExportDump = new System.Windows.Forms.Button();
            this.btnImportRaw = new System.Windows.Forms.Button();
            this.btnImportDump = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuModMaker = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuCreateStandaloneexeInstaller = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuCreateInstallerPackageFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuView = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSearchByName = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuBinaryContentSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuMonobehaviourSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTransformSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuContinueSearchF3 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuGoToAsset = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuDependencies = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuAssetPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuGetScriptInformation = new System.Windows.Forms.ToolStripMenuItem();
            this.assetList = new System.Windows.Forms.ListView();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.containerColumn = new System.Windows.Forms.ColumnHeader();
            this.typeColumn = new System.Windows.Forms.ColumnHeader();
            this.typeIDColumn = new System.Windows.Forms.ColumnHeader();
            this.fileIDColumn = new System.Windows.Forms.ColumnHeader();
            this.pathIDColumn = new System.Windows.Forms.ColumnHeader();
            this.sizeColumn = new System.Windows.Forms.ColumnHeader();
            this.modifiedColumn = new System.Windows.Forms.ColumnHeader();
            this.boxMonoID = new System.Windows.Forms.TextBox();
            this.lblMonoID = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAssets
            // 
            this.lblAssets.AutoSize = true;
            this.lblAssets.Location = new System.Drawing.Point(14, 28);
            this.lblAssets.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAssets.Name = "lblAssets";
            this.lblAssets.Size = new System.Drawing.Size(40, 15);
            this.lblAssets.TabIndex = 1;
            this.lblAssets.Text = "Assets";
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(757, 46);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(39, 15);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Name";
            // 
            // boxName
            // 
            this.boxName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.boxName.BackColor = System.Drawing.SystemColors.Window;
            this.boxName.Location = new System.Drawing.Point(760, 65);
            this.boxName.Margin = new System.Windows.Forms.Padding(16, 3, 16, 3);
            this.boxName.Name = "boxName";
            this.boxName.ReadOnly = true;
            this.boxName.Size = new System.Drawing.Size(210, 23);
            this.boxName.TabIndex = 3;
            // 
            // boxPathID
            // 
            this.boxPathID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.boxPathID.BackColor = System.Drawing.SystemColors.Window;
            this.boxPathID.Location = new System.Drawing.Point(760, 110);
            this.boxPathID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.boxPathID.Name = "boxPathID";
            this.boxPathID.ReadOnly = true;
            this.boxPathID.Size = new System.Drawing.Size(210, 23);
            this.boxPathID.TabIndex = 5;
            // 
            // lblPathID
            // 
            this.lblPathID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPathID.AutoSize = true;
            this.lblPathID.Location = new System.Drawing.Point(757, 91);
            this.lblPathID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPathID.Name = "lblPathID";
            this.lblPathID.Size = new System.Drawing.Size(45, 15);
            this.lblPathID.TabIndex = 4;
            this.lblPathID.Text = "Path ID";
            // 
            // lblFileID
            // 
            this.lblFileID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFileID.AutoSize = true;
            this.lblFileID.Location = new System.Drawing.Point(757, 136);
            this.lblFileID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFileID.Name = "lblFileID";
            this.lblFileID.Size = new System.Drawing.Size(39, 15);
            this.lblFileID.TabIndex = 6;
            this.lblFileID.Text = "File ID";
            // 
            // boxFileID
            // 
            this.boxFileID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.boxFileID.BackColor = System.Drawing.SystemColors.Window;
            this.boxFileID.Location = new System.Drawing.Point(760, 155);
            this.boxFileID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.boxFileID.Name = "boxFileID";
            this.boxFileID.ReadOnly = true;
            this.boxFileID.Size = new System.Drawing.Size(210, 23);
            this.boxFileID.TabIndex = 7;
            // 
            // lblType
            // 
            this.lblType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(757, 181);
            this.lblType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(31, 15);
            this.lblType.TabIndex = 8;
            this.lblType.Text = "Type";
            // 
            // boxType
            // 
            this.boxType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.boxType.BackColor = System.Drawing.SystemColors.Window;
            this.boxType.Location = new System.Drawing.Point(760, 200);
            this.boxType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.boxType.Name = "boxType";
            this.boxType.ReadOnly = true;
            this.boxType.Size = new System.Drawing.Size(210, 23);
            this.boxType.TabIndex = 9;
            // 
            // btnViewData
            // 
            this.btnViewData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewData.Location = new System.Drawing.Point(759, 272);
            this.btnViewData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnViewData.Name = "btnViewData";
            this.btnViewData.Size = new System.Drawing.Size(212, 27);
            this.btnViewData.TabIndex = 10;
            this.btnViewData.Text = "View Data";
            this.btnViewData.UseVisualStyleBackColor = true;
            this.btnViewData.Click += new System.EventHandler(this.BtnViewData_Click);
            // 
            // btnExportRaw
            // 
            this.btnExportRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportRaw.Location = new System.Drawing.Point(759, 336);
            this.btnExportRaw.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExportRaw.Name = "btnExportRaw";
            this.btnExportRaw.Size = new System.Drawing.Size(212, 27);
            this.btnExportRaw.TabIndex = 11;
            this.btnExportRaw.Text = "Export Raw";
            this.btnExportRaw.UseVisualStyleBackColor = true;
            this.btnExportRaw.Click += new System.EventHandler(this.BtnExportRaw_Click);
            // 
            // btnExportDump
            // 
            this.btnExportDump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportDump.Location = new System.Drawing.Point(759, 368);
            this.btnExportDump.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExportDump.Name = "btnExportDump";
            this.btnExportDump.Size = new System.Drawing.Size(212, 27);
            this.btnExportDump.TabIndex = 12;
            this.btnExportDump.Text = "Export Dump";
            this.btnExportDump.UseVisualStyleBackColor = true;
            this.btnExportDump.Click += new System.EventHandler(this.BtnExportDump_Click);
            // 
            // btnImportRaw
            // 
            this.btnImportRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportRaw.Location = new System.Drawing.Point(759, 400);
            this.btnImportRaw.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnImportRaw.Name = "btnImportRaw";
            this.btnImportRaw.Size = new System.Drawing.Size(212, 27);
            this.btnImportRaw.TabIndex = 13;
            this.btnImportRaw.Text = "Import Raw";
            this.btnImportRaw.UseVisualStyleBackColor = true;
            this.btnImportRaw.Click += new System.EventHandler(this.BtnImportRaw_Click);
            // 
            // btnImportDump
            // 
            this.btnImportDump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportDump.Location = new System.Drawing.Point(759, 432);
            this.btnImportDump.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnImportDump.Name = "btnImportDump";
            this.btnImportDump.Size = new System.Drawing.Size(212, 27);
            this.btnImportDump.TabIndex = 14;
            this.btnImportDump.Text = "Import Dump";
            this.btnImportDump.UseVisualStyleBackColor = true;
            this.btnImportDump.Click += new System.EventHandler(this.BtnImportDump_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Location = new System.Drawing.Point(759, 304);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(212, 27);
            this.btnEdit.TabIndex = 15;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(759, 496);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(212, 27);
            this.btnRemove.TabIndex = 16;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.BtnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(759, 464);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(212, 27);
            this.btnAdd.TabIndex = 17;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.MenuView,
            this.MenuTools});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(985, 24);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuSave,
            this.MenuSaveAs,
            this.MenuModMaker,
            this.MenuClose});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 20);
            this.MenuFile.Text = "File";
            // 
            // MenuSave
            // 
            this.MenuSave.Name = "MenuSave";
            this.MenuSave.Size = new System.Drawing.Size(135, 22);
            this.MenuSave.Text = "Save";
            this.MenuSave.Click += new System.EventHandler(this.MenuSave_Click);
            // 
            // MenuSaveAs
            // 
            this.MenuSaveAs.Name = "MenuSaveAs";
            this.MenuSaveAs.Size = new System.Drawing.Size(135, 22);
            this.MenuSaveAs.Text = "Save as...";
            this.MenuSaveAs.Click += new System.EventHandler(this.MenuSaveAs_Click);
            // 
            // MenuModMaker
            // 
            this.MenuModMaker.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuCreateStandaloneexeInstaller,
            this.MenuCreateInstallerPackageFile});
            this.MenuModMaker.Name = "MenuModMaker";
            this.MenuModMaker.Size = new System.Drawing.Size(135, 22);
            this.MenuModMaker.Text = "Mod Maker";
            // 
            // MenuCreateStandaloneexeInstaller
            // 
            this.MenuCreateStandaloneexeInstaller.Enabled = false;
            this.MenuCreateStandaloneexeInstaller.Name = "MenuCreateStandaloneexeInstaller";
            this.MenuCreateStandaloneexeInstaller.Size = new System.Drawing.Size(237, 22);
            this.MenuCreateStandaloneexeInstaller.Text = "Create standalone .exe installer";
            // 
            // MenuCreateInstallerPackageFile
            // 
            this.MenuCreateInstallerPackageFile.Name = "MenuCreateInstallerPackageFile";
            this.MenuCreateInstallerPackageFile.Size = new System.Drawing.Size(237, 22);
            this.MenuCreateInstallerPackageFile.Text = "Create installer package file";
            this.MenuCreateInstallerPackageFile.Click += new System.EventHandler(this.MenuCreateInstallerPackageFile_Click);
            // 
            // MenuClose
            // 
            this.MenuClose.Name = "MenuClose";
            this.MenuClose.Size = new System.Drawing.Size(135, 22);
            this.MenuClose.Text = "Close";
            this.MenuClose.Click += new System.EventHandler(this.MenuClose_Click);
            // 
            // MenuView
            // 
            this.MenuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuSearch,
            this.MenuContinueSearchF3,
            this.MenuGoToAsset,
            this.MenuDependencies,
            this.MenuWindow});
            this.MenuView.Name = "MenuView";
            this.MenuView.Size = new System.Drawing.Size(44, 20);
            this.MenuView.Text = "View";
            // 
            // MenuSearch
            // 
            this.MenuSearch.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuSearchByName,
            this.MenuBinaryContentSearch,
            this.MenuMonobehaviourSearch,
            this.MenuTransformSearch});
            this.MenuSearch.Name = "MenuSearch";
            this.MenuSearch.Size = new System.Drawing.Size(183, 22);
            this.MenuSearch.Text = "Search...";
            // 
            // MenuSearchByName
            // 
            this.MenuSearchByName.Name = "MenuSearchByName";
            this.MenuSearchByName.Size = new System.Drawing.Size(197, 22);
            this.MenuSearchByName.Text = "Asset by name";
            this.MenuSearchByName.Click += new System.EventHandler(this.MenuSearchByName_Click);
            // 
            // MenuBinaryContentSearch
            // 
            this.MenuBinaryContentSearch.Name = "MenuBinaryContentSearch";
            this.MenuBinaryContentSearch.Size = new System.Drawing.Size(197, 22);
            this.MenuBinaryContentSearch.Text = "Content Search";
            this.MenuBinaryContentSearch.Click += new System.EventHandler(this.MenuBinaryContentSearch_Click);
            // 
            // MenuMonobehaviourSearch
            // 
            this.MenuMonobehaviourSearch.Name = "MenuMonobehaviourSearch";
            this.MenuMonobehaviourSearch.Size = new System.Drawing.Size(197, 22);
            this.MenuMonobehaviourSearch.Text = "Monobehaviour Search";
            this.MenuMonobehaviourSearch.Click += new System.EventHandler(this.MenuMonobehaviourSearch_Click);
            // 
            // MenuTransformSearch
            // 
            this.MenuTransformSearch.Name = "MenuTransformSearch";
            this.MenuTransformSearch.Size = new System.Drawing.Size(197, 22);
            this.MenuTransformSearch.Text = "Transform Search";
            this.MenuTransformSearch.Click += new System.EventHandler(this.MenuTransformSearch_Click);
            // 
            // MenuContinueSearchF3
            // 
            this.MenuContinueSearchF3.Name = "MenuContinueSearchF3";
            this.MenuContinueSearchF3.Size = new System.Drawing.Size(183, 22);
            this.MenuContinueSearchF3.Text = "Continue search (F3)";
            this.MenuContinueSearchF3.Click += new System.EventHandler(this.MenuContinueSearchF3_Click);
            // 
            // MenuGoToAsset
            // 
            this.MenuGoToAsset.Name = "MenuGoToAsset";
            this.MenuGoToAsset.Size = new System.Drawing.Size(183, 22);
            this.MenuGoToAsset.Text = "Go to asset";
            this.MenuGoToAsset.Click += new System.EventHandler(this.MenuGoToAsset_Click);
            // 
            // MenuDependencies
            // 
            this.MenuDependencies.Name = "MenuDependencies";
            this.MenuDependencies.Size = new System.Drawing.Size(183, 22);
            this.MenuDependencies.Text = "Dependencies";
            this.MenuDependencies.Click += new System.EventHandler(this.MenuDependencies_Click);
            // 
            // MenuWindow
            // 
            this.MenuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuAssetPreview});
            this.MenuWindow.Name = "MenuWindow";
            this.MenuWindow.Size = new System.Drawing.Size(183, 22);
            this.MenuWindow.Text = "Window";
            // 
            // MenuAssetPreview
            // 
            this.MenuAssetPreview.Name = "MenuAssetPreview";
            this.MenuAssetPreview.Size = new System.Drawing.Size(146, 22);
            this.MenuAssetPreview.Text = "Asset Preview";
            // 
            // MenuTools
            // 
            this.MenuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuGetScriptInformation});
            this.MenuTools.Name = "MenuTools";
            this.MenuTools.Size = new System.Drawing.Size(46, 20);
            this.MenuTools.Text = "Tools";
            // 
            // MenuGetScriptInformation
            // 
            this.MenuGetScriptInformation.Name = "MenuGetScriptInformation";
            this.MenuGetScriptInformation.Size = new System.Drawing.Size(190, 22);
            this.MenuGetScriptInformation.Text = "Get script information";
            // 
            // assetList
            // 
            this.assetList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.assetList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.containerColumn,
            this.typeColumn,
            this.typeIDColumn,
            this.fileIDColumn,
            this.pathIDColumn,
            this.sizeColumn,
            this.modifiedColumn});
            this.assetList.HideSelection = false;
            this.assetList.Location = new System.Drawing.Point(14, 46);
            this.assetList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.assetList.Name = "assetList";
            this.assetList.Size = new System.Drawing.Size(732, 476);
            this.assetList.TabIndex = 19;
            this.assetList.UseCompatibleStateImageBehavior = false;
            this.assetList.View = System.Windows.Forms.View.Details;
            this.assetList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.AssetList_ItemSelectionChanged);
            this.assetList.SelectedIndexChanged += new System.EventHandler(this.AssetList_SelectedIndexChanged);
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 195;
            // 
            // containerColumn
            // 
            this.containerColumn.Text = "Container";
            this.containerColumn.Width = 100;
            // 
            // typeColumn
            // 
            this.typeColumn.Text = "Type";
            this.typeColumn.Width = 140;
            // 
            // typeIDColumn
            // 
            this.typeIDColumn.Text = "Type ID";
            this.typeIDColumn.Width = 53;
            // 
            // fileIDColumn
            // 
            this.fileIDColumn.Text = "File ID";
            this.fileIDColumn.Width = 47;
            // 
            // pathIDColumn
            // 
            this.pathIDColumn.Text = "Path ID";
            this.pathIDColumn.Width = 55;
            // 
            // sizeColumn
            // 
            this.sizeColumn.Text = "Size (bytes)";
            this.sizeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.sizeColumn.Width = 71;
            // 
            // modifiedColumn
            // 
            this.modifiedColumn.Text = "Modified";
            // 
            // boxMonoID
            // 
            this.boxMonoID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.boxMonoID.BackColor = System.Drawing.SystemColors.Window;
            this.boxMonoID.Location = new System.Drawing.Point(760, 245);
            this.boxMonoID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.boxMonoID.Name = "boxMonoID";
            this.boxMonoID.ReadOnly = true;
            this.boxMonoID.Size = new System.Drawing.Size(210, 23);
            this.boxMonoID.TabIndex = 21;
            // 
            // lblMonoID
            // 
            this.lblMonoID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMonoID.AutoSize = true;
            this.lblMonoID.Location = new System.Drawing.Point(757, 226);
            this.lblMonoID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMonoID.Name = "lblMonoID";
            this.lblMonoID.Size = new System.Drawing.Size(53, 15);
            this.lblMonoID.TabIndex = 20;
            this.lblMonoID.Text = "Mono ID";
            // 
            // AssetsViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(985, 537);
            this.Controls.Add(this.boxMonoID);
            this.Controls.Add(this.lblMonoID);
            this.Controls.Add(this.assetList);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnImportDump);
            this.Controls.Add(this.btnImportRaw);
            this.Controls.Add(this.btnExportDump);
            this.Controls.Add(this.btnExportRaw);
            this.Controls.Add(this.btnViewData);
            this.Controls.Add(this.boxType);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.boxFileID);
            this.Controls.Add(this.lblFileID);
            this.Controls.Add(this.boxPathID);
            this.Controls.Add(this.lblPathID);
            this.Controls.Add(this.boxName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblAssets);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "AssetsViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Assets Info";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AssetsViewer_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AssetsViewer_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblAssets;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox boxName;
        private System.Windows.Forms.TextBox boxPathID;
        private System.Windows.Forms.Label lblPathID;
        private System.Windows.Forms.Label lblFileID;
        private System.Windows.Forms.TextBox boxFileID;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.TextBox boxType;
        private System.Windows.Forms.Button btnViewData;
        private System.Windows.Forms.Button btnExportRaw;
        private System.Windows.Forms.Button btnExportDump;
        private System.Windows.Forms.Button btnImportRaw;
        private System.Windows.Forms.Button btnImportDump;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem MenuSave;
        private System.Windows.Forms.ToolStripMenuItem MenuSaveAs;
        private System.Windows.Forms.ToolStripMenuItem MenuModMaker;
        private System.Windows.Forms.ToolStripMenuItem MenuCreateStandaloneexeInstaller;
        private System.Windows.Forms.ToolStripMenuItem MenuCreateInstallerPackageFile;
        private System.Windows.Forms.ToolStripMenuItem MenuClose;
        private System.Windows.Forms.ToolStripMenuItem MenuView;
        private System.Windows.Forms.ToolStripMenuItem MenuSearch;
        private System.Windows.Forms.ToolStripMenuItem MenuContinueSearchF3;
        private System.Windows.Forms.ToolStripMenuItem MenuGoToAsset;
        private System.Windows.Forms.ToolStripMenuItem MenuDependencies;
        private System.Windows.Forms.ToolStripMenuItem MenuWindow;
        private System.Windows.Forms.ToolStripMenuItem MenuAssetPreview;
        private System.Windows.Forms.ListView assetList;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader typeColumn;
        private System.Windows.Forms.ColumnHeader fileIDColumn;
        private System.Windows.Forms.ColumnHeader pathIDColumn;
        private System.Windows.Forms.ColumnHeader sizeColumn;
        private System.Windows.Forms.ColumnHeader modifiedColumn;
        private System.Windows.Forms.ToolStripMenuItem MenuSearchByName;
        private System.Windows.Forms.ToolStripMenuItem MenuBinaryContentSearch;
        private System.Windows.Forms.ToolStripMenuItem MenuMonobehaviourSearch;
        private System.Windows.Forms.ToolStripMenuItem MenuTransformSearch;
        private System.Windows.Forms.ColumnHeader typeIDColumn;
        private System.Windows.Forms.ToolStripMenuItem MenuTools;
        private System.Windows.Forms.ToolStripMenuItem MenuGetScriptInformation;
        private System.Windows.Forms.ColumnHeader containerColumn;
        private System.Windows.Forms.TextBox boxMonoID;
        private System.Windows.Forms.Label lblMonoID;
    }
}