using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using AssetsAdvancedEditor.Utils;
using ModInstaller;
using UnityTools;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class ModMakerDialog : Form
    {
        public bool FromBundle;
        public AssetsWorkspace Workspace;
        public TreeNode affectedBundleFiles;
        public TreeNode affectedAssetsFiles;
        public List<ModMakerItem> filesItems;
        public Dictionary<string, ModMakerItem> filesDict;
        public ModMakerDialog(AssetsWorkspace workspace)
        {
            InitializeComponent();
            FromBundle = workspace.FromBundle;
            Workspace = workspace;
            filesItems = new List<ModMakerItem>();
            filesDict = new Dictionary<string, ModMakerItem>();
            PopulateTree();
        }

        private void PopulateTree()
        {
            affectedBundleFiles = new TreeNode("Affected bundles");
            affectedAssetsFiles = new TreeNode("Affected assets files");

            affectedFilesList.Nodes.Add(affectedBundleFiles);
            affectedFilesList.Nodes.Add(affectedAssetsFiles);

            var rootPath = boxModBaseFolderPath.Text;
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = Workspace.AssetsRootDir;
            }

            TreeNode curNode = null;
            foreach (var (assetId, replacer) in Workspace.NewAssets)
            {
                var file = assetId.fileName;
                if (!filesDict.ContainsKey(file))
                {
                    var newFileItem = new ModMakerItem(file, rootPath);
                    filesItems.Add(newFileItem);
                    filesDict.Add(file, newFileItem);
                    curNode = affectedAssetsFiles.Nodes.Add(file);
                }

                var modMakerReplacer = new ModMakerReplacerItem(assetId, replacer);
                filesDict[file].Replacers.Add(modMakerReplacer);
                curNode?.Nodes.Add(modMakerReplacer.DisplayText);
            }
        }

        private void BuildEmip(string path)
        {
            var emip = new InstallerPackageFile
            {
                magic = "EMIP",
                includesCldb = false,
                modName = boxModName.Text,
                modCreators = boxModCreators.Text,
                modDescription = boxModDescription.Text,
                affectedFiles = new List<InstallerPackageAssetsDescription>()
            };

            foreach (var file in filesItems)
            {
                //hack pls fix thx
                var filePath = file.RelPath;
                var desc = new InstallerPackageAssetsDescription
                {
                    isBundle = FromBundle,
                    path = filePath,
                    replacers = new List<object>()
                };
                foreach (var change in file.Replacers)
                {
                    desc.replacers.Add(change.Replacer);
                }
                emip.affectedFiles.Add(desc);
            }

            using var fs = File.OpenWrite(path);
            using var writer = new EndianWriter(fs, true);
            emip.Write(writer);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFolderDialog
            {
                Title = "Select a base folder"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;
            boxModBaseFolderPath.Text = ofd.Folder;
        }

        private void btnImportPackage_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = @"Open mod installer package",
                Filter = @"UABE Mod Installer Package (*.emip)|*.emip",
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var rootPath = Path.GetDirectoryName(ofd.FileName) ?? Workspace.AssetsRootDir;
            var emip = new InstallerPackageFile();
            using var fs = File.OpenRead(ofd.FileName);
            var reader = new EndianReader(fs, true);
            emip.Read(reader);

            boxModName.Text = emip.modName;
            boxModCreators.Text = emip.modCreators;
            boxModDescription.Text = emip.modDescription;

            foreach (var affectedFile in emip.affectedFiles)
            {
                if (affectedFile.isBundle) continue;

                var file = Path.GetFullPath(affectedFile.path, rootPath);
                if (!filesDict.ContainsKey(file))
                {
                    var newFileItem = new ModMakerItem(file, rootPath);
                    filesItems.Add(newFileItem);
                    filesDict.Add(file, newFileItem);
                }

                var fileItem = filesDict[file];

                foreach (AssetsReplacer replacer in affectedFile.replacers)
                {
                    var assetId = new AssetID(file, replacer.GetPathID());
                    fileItem.Replacers.Add(new ModMakerReplacerItem(assetId, replacer));
                }
            }
        }

        private void btnRemoveChange_Click(object sender, EventArgs e)
        {
            var choice = MsgBoxUtils.ShowWarningDialog("Are you sure you want to remove the selected change?");
            if (choice != DialogResult.Yes) return;

            var remNode = affectedFilesList.SelectedNode;
            if (remNode.Parent != null)
            {
                filesDict.Remove(remNode.Parent.Text);
                remNode.Remove();
                foreach (var mmItem in filesItems)
                {
                    mmItem.Replacers.RemoveAll(r => r.DisplayText == remNode.Text);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = @"Save mod installer package",
                Filter = @"UABE Mod Installer Package (*.emip)|*.emip",
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            BuildEmip(sfd.FileName);
        }
    }
}
