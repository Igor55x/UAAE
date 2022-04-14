using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using AssetsAdvancedEditor.Utils;
using UnityTools;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class Menu : Form
    {
        public AssetsManager Am;
        public BundleFileInstance BundleInst;
        public BundleLoader Loader;
        public bool Modified;
        public Dictionary<string, BundleReplacer> ModifiedFiles;

        public Menu()
        {
            InitializeComponent();
            ModifiedFiles = new Dictionary<string, BundleReplacer>();
            Modified = false;
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            Am = new AssetsManager();
            const string releaseClassData = "classdata_release.tpk";
            var classDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, releaseClassData);
            if (File.Exists(classDataPath))
            {
                Am.LoadClassPackage(classDataPath);
            }
            else
            {
                MsgBoxUtils.ShowErrorDialog($"Missing {releaseClassData} by exe.\n" +
                            "Please make sure it exists.");
                Close();
                Environment.Exit(1);
            }
        }

        private void MenuOpen_Click(object sender, EventArgs e)
        {
            if (Modified) AskSaveChanges();
            var ofd = new OpenFileDialog
            {
                Title = @"Open assets or bundle file",
                Filter = @"All types (*.*)|*.*|Unity content (*.unity3d;*.assets)|*.unity3d;*.assets|Bundle file (*.unity3d)|*.unity3d|Assets file (*.assets)|*.assets"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            var selectedFile = ofd.FileName;

            var fileType = AssetsBundleDetector.DetectFileType(selectedFile);

            CloseAllFiles();

            switch (fileType)
            {
                case DetectedFileType.AssetsFile:
                {
                    LoadAssetsFile(selectedFile);
                    break;
                }
                case DetectedFileType.BundleFile:
                {
                    LoadBundleFile(selectedFile);
                    break;
                }
                default:
                    MsgBoxUtils.ShowErrorDialog("Unable to read the file!\n" +
                                                "Invalid file or unknown (unsupported) version.");
                    break;
            }
        }

        private void MenuClose_Click(object sender, EventArgs e)
        {
            if (Modified) AskSaveChanges();
            else CloseAllFiles();
        }

        private void MenuSave_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = @"Save bundle file",
                Filter = @"All types (*.*)|*.*|Bundle file (*.unity3d)|*.unity3d"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            SaveBundle(sfd.FileName);
        }

        private void MenuCompress_Click(object sender, EventArgs e)
        {
            new BundleCompression(BundleInst).ShowDialog();
        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            AskSaveChanges();
            if (!Modified)
                Application.Exit();
        }

        private void LoadAssetsFile(string path)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                var fileInst = Am.LoadAssetsFile(path, true);

                if (!LoadOrAskCldb(fileInst))
                    return;

                using var dialog = new AssetsViewer(Am, fileInst);
                dialog.ShowDialog();
            };
            bw.RunWorkerAsync();
        }

        private void LoadBundleFile(string path)
        {
            BundleInst = Am.LoadBundleFile(path, false);
            Loader = new BundleLoader(BundleInst);
            Loader.ShowDialog();
            if (!Loader.Loaded) return;
            SetBundleControlsEnabled(true);

            var infos = BundleInst.file.Metadata.DirectoryInfo;
            for (var i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                cboxBundleContents.Items.Add(new ComboBoxAssetItem
                {
                    DisplayName = info.Name,
                    OriginalName = info.Name,
                    Index = i
                });
            }
            cboxBundleContents.SelectedIndex = 0;
            lblFileName.Text = BundleInst.name;
        }

        private void SaveBundle(string path)
        {
            if (BundleInst == null) return;

            if (Path.GetFullPath(BundleInst.path) == Path.GetFullPath(path))
            {
                var choice = MsgBoxUtils.ShowWarningDialog("This bundle is already open in UAAE, and this action will overwrite it.\n" +
                                                           "Are you sure you want to continue?");
                if (choice != DialogResult.Yes) return;

                var tempPath = Path.Combine(Path.GetTempPath(), BundleInst.name);
                using (var fs = File.OpenWrite(tempPath))
                using (var writer = new EndianWriter(fs, true))
                {
                    BundleInst.file.Write(writer, ModifiedFiles.Values.ToList());
                }
                Am.UnloadBundleFile(path);
                File.Replace(tempPath, path, path + ".backup");
                Am.LoadBundleFile(path, true);
                Modified = false;
            }
            else
            {
                using (var fs = File.OpenWrite(path))
                using (var writer = new EndianWriter(fs, true))
                {
                    BundleInst.file.Write(writer, ModifiedFiles.Values.ToList());
                }
                Modified = false;
            }

            for (var i = 0; i < cboxBundleContents.Items.Count; i++)
            {
                var item = (ComboBoxAssetItem)cboxBundleContents.Items[i];
                item.DisplayName = item.OriginalName;
                if (item.Index == i)
                {
                    cboxBundleContents.SelectedIndex = item.Index;
                }
            }
        }

        private void CloseAllFiles()
        {
            ModifiedFiles.Clear();
            Modified = false;

            Am.UnloadAllAssetsFiles(true);
            Am.UnloadAllBundleFiles();

            SetBundleControlsEnabled(false);
            lblFileName.Text = @"No file opened.";
        }

        private void SetBundleControlsEnabled(bool enabled = true)
        {
            cboxBundleContents.Enabled = enabled;
            if (!enabled)
            {
                cboxBundleContents.Items.Clear();
            }
            btnExport.Enabled = enabled;
            btnImport.Enabled = enabled;
            btnRemove.Enabled = enabled;
            btnInfo.Enabled = enabled;
            btnExportAll.Enabled = enabled;
            btnImportAll.Enabled = enabled;

            MenuOpen.Enabled = !enabled;
            MenuClose.Enabled = enabled;
            MenuSave.Enabled = enabled;
            MenuCompress.Enabled = enabled;
        }

        private bool LoadOrAskCldb(AssetsFileInstance fileInst)
        {
            var unityVersion = fileInst.file.typeTree.unityVersion;
            if (Am.LoadClassDatabaseFromPackage(unityVersion) == null)
            {
                var version = new VersionDialog(unityVersion, Am.classPackage);
                if (version.ShowDialog() != DialogResult.OK)
                    return false;

                if (version.SelectedCldb != null)
                {
                    Am.classFile = version.SelectedCldb;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void AskSaveChanges()
        {
            if (!Modified) return;
            var choice = MsgBoxUtils.ShowInfoDialog("Would you like to save the changes?");
            switch (choice)
            {
                case DialogResult.Yes:
                {
                    var sfd = new SaveFileDialog
                    {
                        Title = @"Save bundle file",
                        Filter = @"All types (*.*)|*.*|Bundle file (*.unity3d)|*.unity3d"
                    };
                    if (sfd.ShowDialog() != DialogResult.OK) return;
                    SaveBundle(sfd.FileName);
                    break;
                }
                case DialogResult.No:
                    CloseAllFiles();
                    break;
            }
        }

        private void MenuAbout_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (BundleInst == null || cboxBundleContents.SelectedItem == null) return;

            var item = (ComboBoxAssetItem)cboxBundleContents.SelectedItem;
            var bunAssetName = item.OriginalName;
            var assetData = BundleHelper.LoadAssetDataFromBundle(BundleInst.file, bunAssetName);

            var sfd = new SaveFileDialog
            {
                Title = @"Save as...",
                Filter = @"All types (*.*)|*.*|Assets file (*.assets)|*.assets",
                FileName = bunAssetName
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            File.WriteAllBytes(sfd.FileName, assetData);
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            if (BundleInst == null) return;
            var ofd = new OpenFileDialog
            {
                Title = @"Open assets file",
                Filter = @"All types (*.*)|*.*|Assets file (*.assets)|*.assets"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            //todo replacer from stream rather than bytes
            //also need to handle closing them somewhere
            //and replacers don't support closing
            var fileName = Path.GetFileName(ofd.FileName);
            var fileBytes = File.ReadAllBytes(ofd.FileName);
            var isSerialized = fileName.EndsWith(".resS") || fileName.EndsWith(".resource");
            var replacer = AssetModifier.CreateBundleReplacer(fileName, isSerialized, fileBytes);
            var item = (ComboBoxAssetItem)cboxBundleContents.SelectedItem;
            if (item.OriginalName == fileName)
            {
                item.DisplayName += " *";
            }
            else
            {
                var newIndex = cboxBundleContents.Items.Count;
                cboxBundleContents.Items.Add(new ComboBoxAssetItem
                {
                    DisplayName = fileName,
                    OriginalName = fileName,
                    Index = newIndex
                });
                cboxBundleContents.SelectedIndex = newIndex;
            }
            ModifiedFiles[fileName] = replacer;
            Modified = true;
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (BundleInst == null || cboxBundleContents.SelectedItem == null) return;

            var item = (ComboBoxAssetItem)cboxBundleContents.SelectedItem;
            var index = item.Index;
            var name = item.OriginalName;
            if (ModifiedFiles.ContainsKey(name))
            {
                ModifiedFiles.Remove(name);
            }
            else
            {
                var isSerialized = BundleInst.file.IsAssetsFile(index);
                ModifiedFiles.Add(name, AssetModifier.CreateBundleRemover(name, isSerialized));
            }

            cboxBundleContents.Items.RemoveAt(index);
            if (cboxBundleContents.Items.Count != 0)
                cboxBundleContents.SelectedIndex = 0;
        }

        private void BtnInfo_Click(object sender, EventArgs e)
        {
            if (BundleInst == null || cboxBundleContents.SelectedItem == null) return;

            var item = (ComboBoxAssetItem)cboxBundleContents.SelectedItem;
            var index = item.Index;
            var bunAssetName = item.OriginalName;

            // When we make a modification to an assets file in the bundle,
            // we replace the assets file in the manager. This way, all we
            // have to do is not reload from the bundle if our assets file
            // has been modified
            var assetMemPath = Path.Combine(BundleInst.path, bunAssetName);
            MemoryStream assetStream = null;
            bool isAssetsFile;
            if (!ModifiedFiles.ContainsKey(bunAssetName))
            {
                var assetData = BundleHelper.LoadAssetDataFromBundle(BundleInst.file, index);
                assetStream = new MemoryStream(assetData);
                isAssetsFile = BundleInst.file.IsAssetsFile(index);
            }
            else
            {
                isAssetsFile = AssetsFile.IsAssetsFile(assetMemPath);
            }

            // [Warning]: does not update if you import an assets file onto
            // a file that wasn't originally an assets file
            if (isAssetsFile)
            {
                var fileInst = Am.LoadAssetsFile(assetStream, assetMemPath, true);

                if (!LoadOrAskCldb(fileInst))
                    return;

                if (BundleInst != null && fileInst.parentBundle == null)
                    fileInst.parentBundle = BundleInst;

                var info = new AssetsViewer(Am, fileInst, true);
                info.Closing += AssetsViewer_Closing;
                info.Show();
            }
            else
            {
                MsgBoxUtils.ShowErrorDialog("This doesn't seem to be a valid assets file!");
            }
        }

        private void BtnExportAll_Click(object sender, EventArgs e)
        {
            if (BundleInst == null) return;

            var ofd = new OpenFolderDialog
            {
                Title = @"Select a folder for assets"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            for (var i = 0; i < cboxBundleContents.Items.Count; i++)
            {
                var bunAssetName = BundleHelper.GetDirInfo(BundleInst.file, i).Name;
                var bunAssetPath = Path.Combine(ofd.Folder, bunAssetName);
                var assetData = BundleHelper.LoadAssetDataFromBundle(BundleInst.file, i);

                // Create dirs if bundle contains / or \\ in path
                if (bunAssetName.Contains('\\') || bunAssetName.Contains('/'))
                {
                    var bunAssetDir = Path.GetDirectoryName(bunAssetPath);
                    if (!Directory.Exists(bunAssetDir))
                    {
                        Directory.CreateDirectory(bunAssetDir);
                    }
                }
                File.WriteAllBytes(bunAssetPath, assetData);
            }
        }

        private void BtnImportAll_Click(object sender, EventArgs e)
        {
            if (BundleInst == null) return;

            var ofd = new OpenFolderDialog
            {
                Title = @"Select an input path"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            foreach (var file in Directory.EnumerateFiles(ofd.Folder))
            {
                //todo replacer from stream rather than bytes
                //also need to handle closing them somewhere
                //and replacers don't support closing
                var fileName = Path.GetFileName(file);
                var fileBytes = File.ReadAllBytes(file);
                var isSerialized = fileName.EndsWith(".resS") || fileName.EndsWith(".resource");
                var replacer = AssetModifier.CreateBundleReplacer(fileName, isSerialized, fileBytes);
                var index = cboxBundleContents.Items.IndexOf(fileName);
                if (index != -1)
                {
                    var item = (ComboBoxAssetItem)cboxBundleContents.Items[index];
                    item.DisplayName += " *";
                }
                else
                {
                    var newIndex = cboxBundleContents.Items.Count;
                    cboxBundleContents.Items.Add(new ComboBoxAssetItem
                    {
                        DisplayName = fileName,
                        OriginalName = fileName,
                        Index = newIndex
                    });
                    cboxBundleContents.SelectedIndex = newIndex;
                }
                ModifiedFiles[fileName] = replacer;
            }
            Modified = true;
        }

        private void AssetsViewer_Closing(object sender, CancelEventArgs e)
        {
            if (sender == null) return;

            var window = (AssetsViewer)sender;

            if (window.ModifiedFiles.Count == 0) return;
            var bunDict = window.ModifiedFiles;

            foreach (var (replacer, assetsStream) in bunDict)
            {
                var fileName = replacer.GetOriginalEntryName();
                ModifiedFiles[fileName] = replacer;

                //replace existing assets file in the manager
                var inst = Am.files.FirstOrDefault(i =>
                    string.Equals(i.name, fileName, StringComparison.CurrentCultureIgnoreCase));
                string assetsManagerName;

                if (inst != null)
                {
                    assetsManagerName = inst.path;
                    Am.files.Remove(inst);
                }
                else //shouldn't happen
                {
                    //we always load bundles from file, so this
                    //should always be somewhere on the disk
                    assetsManagerName = Path.Combine(BundleInst.path, fileName);
                }
                Am.LoadAssetsFile(assetsStream, assetsManagerName, true);
            }

            Modified = true;
        }
    }
}