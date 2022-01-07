using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using AssetsAdvancedEditor.Plugins;
using AssetsAdvancedEditor.Utils;
using AssetsAdvancedEditor.Winforms.AssetSearch;
using UnityTools;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class AssetsViewer : Form
    {
        public AssetsWorkspace Workspace { get; }
        public AssetsManager Am { get; }
        public PluginManager Pm { get; }
        public AssetsFileInstance MainInstance { get; }
        public bool FromBundle { get; }

        public AssetImporter Importer { get; }
        public AssetExporter Exporter { get; }

        public string AssetsFileName { get; }
        public string AssetsRootDir { get; }
        public string UnityVersion { get; }

        public Dictionary<BundleReplacer, MemoryStream> ModifiedFiles { get; private set; }
        //private Stack<List<int>> UndoList { get; }
        //private Stack<List<int>> RedoList { get; }

        #region Searching
        private string searchText;
        private int searchStart;
        private bool searchDown;
        private bool searchCaseSensitive;
        private bool searchStartAtSelection;
        private bool searching;
        #endregion
        
        #region todo
        //MonobehaviourSearch
        //TransformSearch
        //Dependencies
        #endregion

        public AssetsViewer(AssetsManager am, AssetsFileInstance instance, bool fromBundle = false)
        {
            InitializeComponent();

            Workspace = new AssetsWorkspace(am, instance, fromBundle);
            Am = Workspace.Am;
            Pm = Workspace.Pm;
            MainInstance = Workspace.MainInstance;
            FromBundle = Workspace.FromBundle;

            Importer = Workspace.Importer;
            Exporter = Workspace.Exporter;

            AssetsFileName = Workspace.AssetsFileName;
            AssetsRootDir = Workspace.AssetsRootDir;
            UnityVersion = Workspace.UnityVersion;

            ModifiedFiles = new Dictionary<BundleReplacer, MemoryStream>();
            //UndoList = new Stack<List<int>>();
            //RedoList = new Stack<List<int>>();

            SetFormText();
            LoadAssetsToList();
        }

        private void SetFormText() => Text = $@"Assets Info ({UnityVersion})";

        private void LoadAssetsToList()
        {
            MainInstance.file.reader.BigEndian = false;
            Workspace.LoadedFiles.Add(MainInstance);
            foreach (var info in MainInstance.table.Info)
            {
                AddAssetItem(MainInstance, info);
            }

            var id = 1;
            foreach (var dep in MainInstance.dependencies)
            {
                if (dep != null)
                {
                    dep.file.reader.BigEndian = false;
                    Workspace.LoadedFiles.Add(dep);
                    foreach (var inf in dep.table.Info)
                    {
                        AddAssetItem(dep, inf, id);
                    }
                    id++;
                }
            }
        }

        private void AddAssetItem(AssetsFileInstance fileInst, AssetFileInfoEx info, int fileId = 0)
        {
            Extensions.GetAssetItemFast(fileId, fileInst, Workspace, info, out var item);
            item.Cont = new AssetContainer(fileInst);
            Workspace.LoadedAssets.Add(item);
            assetList.Items.Add(new ListViewItem(item.ToArray()));
        }

        private static bool HasName(ClassDatabaseFile cldb, ClassDatabaseType cldbType)
        {
            return cldbType != null && cldbType.fields.Any(f => f.fieldName.GetString(cldb) == "m_Name");
        }

        private static bool HasAnyField(ClassDatabaseType cldbType) => cldbType != null && cldbType.fields.Any();

        private void AddAssetItems(List<AssetItem> items)
        {
            var cldb = Am.classFile;
            foreach (var item in items)
            {
                var name = item.Name;
                var typeId = item.TypeID;

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unnamed asset";
                }
                else if (typeId is AssetClassID.GameObject or AssetClassID.MonoBehaviour)
                {
                    name = $"{item.Type} {name}";
                }

                var cldbType = AssetHelper.FindAssetClassByID(cldb, typeId);
                if (!HasAnyField(cldbType))
                    name = $"{item.Type} {name}";

                if (!HasName(cldb, cldbType))
                    name = "Unnamed asset";

                item.ListName = name;
                assetList.Items.Insert(0, new ListViewItem(item.ToArray()));
                Workspace.LoadedAssets.Insert(0, item);
                assetList.Items[0].Selected = true;
            }
            assetList.Select();
        }

        private void RemoveAssetItems()
        {
            var choice = MsgBoxUtils.ShowWarningDialog("Are you sure you want to remove the selected asset(s)?\n" +
                                                       "This will break any reference to this/these.");
            if (choice != DialogResult.Yes) return;

            foreach (int index in assetList.SelectedIndices)
            {
                var item = Workspace.LoadedAssets[index];
                Workspace.AddReplacer(ref item, AssetModifier.CreateAssetRemover(item));
                assetList.Items.RemoveAt(index);
            }
        }

        //not used now, was originally for the size thing
        //private string ConvertSizes(uint size)
        //{
        //    if (size >= 1048576)
        //        return (size / 1048576f).ToString("N2") + "m";
        //    else if (size >= 1024)
        //        return (size / 1024f).ToString("N2") + "k";
        //    return size + "b";
        //}

        private List<AssetItem> GetSelectedAssetItems()
        {
            var assetItems = new List<AssetItem>();
            foreach (int index in assetList.SelectedIndices)
            {
                var item = Workspace.LoadedAssets[index];
                Workspace.MakeAssetContainer(ref item);
                assetItems.Add(item);
            }
            return assetItems;
        }

        private List<AssetTypeValueField> GetSelectedFields()
        {
            try
            {
                var fields = new List<AssetTypeValueField>();
                foreach (var item in GetSelectedAssetItems())
                {
                    fields.Add(Workspace.GetBaseField(item));
                }
                return fields;
            }
            catch
            {
                MsgBoxUtils.ShowErrorDialog("Unable to process the asset data!\n" +
                                            "This might be due to an incompatible type database.");
                return null;
            }
        }

        private void UpdateAssetInfo()
        {
            foreach (int index in assetList.SelectedIndices)
            {
                var item = Workspace.LoadedAssets[index];

                assetList.Items[index] = new ListViewItem(item.ToArray())
                {
                    Selected = true
                };
            }
            assetList.Select();
        }

        private void ClearModified()
        {
            Workspace.Modified = false;
            for (var i = 0; i < assetList.Items.Count; i++)
            {
                assetList.Items[i].SubItems[7].Text = "";
            }
            Workspace.ClearModified();
        }

        private void assetList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (Workspace.LoadedAssets.Count != assetList.Items.Count) return;  // shouldn't happen
            var details = Workspace.LoadedAssets[e.ItemIndex];
            var name = details.Name;
            var cldb = Am.classFile;
            var cldbType = AssetHelper.FindAssetClassByID(cldb, details.TypeID);
            if (!HasName(cldb, cldbType))
            {
                name = "Unnamed asset";
            }
            else if (details.TypeID is AssetClassID.GameObject or AssetClassID.MonoBehaviour)
            {
                name = $"{details.Type} {name}";
            }
            boxName.Text = name;
            boxPathID.Text = details.PathID.ToString();
            boxFileID.Text = details.FileID.ToString();
            var typeId = (int)details.TypeID;
            if (details.TypeID != AssetClassID.MonoBehaviour)
            {
                if (Enum.TryParse(details.Type, out AssetClassID classId))
                {
                    boxType.Text = details.TypeID == classId
                        ? $@"0x{typeId:X8}"
                        : $@"0x{typeId:X8} ({details.Type})";
                }
                else
                {
                    boxType.Text = $@"0x{typeId:X8} ({details.Type})";
                }
            }
            else
            {
                boxType.Text = details.MonoID != ushort.MaxValue ?
                    $@"0x{details.MonoID:X8} ({details.Type})" :
                    $@"0x{typeId:X8} ({details.Type})";
            }
        }

        private void assetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GetSelectedCount() is not 0) return;
            boxName.Text = "";
            boxPathID.Text = "";
            boxFileID.Text = "";
            boxType.Text = "";
        }

        private void btnViewData_Click(object sender, EventArgs e)
        {
            if (FailIfNothingSelected()) return;
            var baseFields = GetSelectedFields();
            if (baseFields == null) return;
            var cldb = Workspace.Am.classFile;
            for (var i = 0; i < baseFields.Count; i++)
            {
                var baseField = baseFields[i];
                var cldbType = AssetHelper.FindAssetClassByName(cldb, baseField.GetFieldType());
                if (cldbType != null)
                {
                    if (HasAnyField(cldbType)) continue;
                    MsgBoxUtils.ShowErrorDialog("This asset has no data to view.");
                }
                else
                {
                    MsgBoxUtils.ShowErrorDialog("Unknown asset format.");
                }
                baseFields.RemoveAt(i);
                i--;
            }

            if (baseFields.Count == 0) return;
            foreach (var baseField in baseFields)
                new AssetData(Workspace, baseField).Show();
        }

        private void AssetsViewer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    NextSearch();
                    break;
                case Keys.Y when e.Control:
                    //Redo();
                    break;
                case Keys.Z when e.Control:
                    //Undo();
                    break;
            }
        }

        private void AssetsViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Workspace.Modified) AskSaveChanges();
            e.Cancel = Workspace.Modified;
        }

        private void AskSaveChanges()
        {
            if (!Workspace.Modified) return;
            var choice = MsgBoxUtils.ShowInfoDialog("Would you like to save the changes?");
            switch (choice)
            {
                case DialogResult.Yes:
                    SaveFiles();
                    break;
                case DialogResult.No:
                    CloseFiles();
                    break;
            }
        }

        private void SaveFiles(bool overwrite = false)
        {
            if (FromBundle)
            {
                WriteFilesInBundle();
                ClearModified();
            }
            else
            {
                if (overwrite)
                {
                    var choice = MsgBoxUtils.ShowWarningDialog("This action will overwrite the file.\n" +
                                                               "Are you sure you want to continue?");
                    if (choice != DialogResult.Yes) return;
                }
                WriteFiles(overwrite);
                ClearModified();
            }
        }

        public void WriteFiles(bool overwrite = false)
        {
            foreach (var (fileId, replacers) in Workspace.NewReplacers)
            {
                var fileInst = Workspace.LoadedFiles[fileId];
                if (overwrite)
                {
                    var path = fileInst.path;
                    var tempPath = Path.Combine(Path.GetTempPath(), fileInst.name);
                    using var fs = File.OpenWrite(tempPath);
                    using var writer = new AssetsFileWriter(fs);
                    fileInst.file.Write(writer, 0, replacers, 0);
                    Am.UnloadAssetsFile(path);
                    fs.Close();
                    File.Replace(tempPath, path, path + ".backup");
                    Workspace.LoadedFiles[fileId] = Am.LoadAssetsFile(path, false);
                }
                else
                {
                    var sfd = new SaveFileDialog
                    {
                        Title = @"Save as...",
                        Filter = @"All types (*.*)|*.*|Assets file (*.assets)|*.assets",
                        FileName = fileInst.name
                    };
                    if (sfd.ShowDialog() != DialogResult.OK) continue;
                    if (fileInst.path == sfd.FileName)
                    {
                        MsgBoxUtils.ShowErrorDialog("If you want to overwrite files go to \"File->Save\" instead of \"File->Save as...\"!");
                        return;
                    }
                    using var fs = File.OpenWrite(sfd.FileName);
                    using var writer = new AssetsFileWriter(fs);
                    fileInst.file.Write(writer, 0, replacers, 0);
                }
            }
        }

        public void WriteFilesInBundle()
        {
            ModifiedFiles.Clear();
            foreach (var (fileId, replacers) in Workspace.NewReplacers)
            {
                var ms = new MemoryStream();
                var writer = new AssetsFileWriter(ms);
                var fileInst = Workspace.LoadedFiles[fileId];

                fileInst.file.Write(writer, 0, replacers);
                ms.Seek(0, SeekOrigin.Begin);
                ModifiedFiles.Add(AssetModifier.CreateBundleReplacer(fileInst.name, true, ms.ToArray()), ms);
            }
        }

        private void CloseFiles()
        {
            Workspace.LoadedFiles.Clear();
            Am.UnloadAllAssetsFiles(true);
            Workspace.Modified = false;
            Close();
        }

        private void btnExportRaw_Click(object sender, EventArgs e)
        {
            if (FailIfNothingSelected()) return;
            var selectedItems = GetSelectedAssetItems();

            if (GetSelectedCount() > 1)
                BatchExportRaw(selectedItems);
            else
                SingleExportRaw(selectedItems[0]);
        }

        private void BatchExportRaw(List<AssetItem> selectedItems)
        {
            var fd = new OpenFolderDialog
            {
                Title = "Select a folder for the raw assets"
            };
            if (fd.ShowDialog(this) != DialogResult.OK) return;
            foreach (var item in selectedItems)
            {
                var name = Extensions.ReplaceInvalidFileNameChars(item.Name);
                if (string.IsNullOrEmpty(name))
                {
                    name = "Unnamed asset";
                }

                var fileName = $"{name}-{item.Cont.FileInstance.name}-{item.PathID}-{item.Type}.dat";
                var path = Path.Combine(fd.Folder, fileName);
                Exporter.ExportRawAsset(path, item);
            }
        }

        private void SingleExportRaw(AssetItem selectedItem)
        {
            var name = Extensions.ReplaceInvalidFileNameChars(selectedItem.Name);
            if (string.IsNullOrEmpty(name))
            {
                name = "Unnamed asset";
            }
            var sfd = new SaveFileDialog
            {
                Title = @"Save raw asset",
                Filter = @"Raw Unity asset (*.dat)|*.dat",
                FileName = $"{name}-{selectedItem.Cont.FileInstance.name}-{selectedItem.PathID}"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            Exporter.ExportRawAsset(sfd.FileName, selectedItem);
        }

        private void btnExportDump_Click(object sender, EventArgs e)
        {
            if (FailIfNothingSelected()) return;
            var selectedItems = GetSelectedAssetItems();

            if (GetSelectedCount() > 1)
                BatchExportDump(selectedItems);
            else
                SingleExportDump(selectedItems[0]);
        }

        private void BatchExportDump(List<AssetItem> selectedItems)
        {
            var dialog = new DumpTypeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;
            var dumpType = dialog.dumpType;
            var ext = dumpType switch
            {
                DumpType.TXT => ".txt",
                DumpType.XML => ".xml",
                _ => ".txt"
            };
            var fd = new OpenFolderDialog
            {
                Title = "Select a folder for the dumps"
            };
            if (fd.ShowDialog(this) != DialogResult.OK) return;
            foreach (var item in selectedItems)
            {
                var name = Extensions.ReplaceInvalidFileNameChars(item.Name);
                if (string.IsNullOrEmpty(name))
                {
                    name = "Unnamed asset";
                }

                var fileName = $"{name}-{item.Cont.FileInstance.name}-{item.PathID}-{item.Type}{ext}";
                var path = Path.Combine(fd.Folder, fileName);
                Exporter.ExportDump(path, item, dumpType);
            }
        }

        private void SingleExportDump(AssetItem selectedItem)
        {
            var name = Extensions.ReplaceInvalidFileNameChars(selectedItem.Name);
            if (string.IsNullOrEmpty(name))
            {
                name = "Unnamed asset";
            }
            var sfd = new SaveFileDialog
            {
                Title = @"Save dump",
                Filter = @"UAAE text dump (*.txt)|*.txt|UAAE xml dump (*.xml)|*.xml",
                FileName = $"{name}-{selectedItem.Cont.FileInstance.name}-{selectedItem.PathID}-{selectedItem.Type}"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            var dumpType = sfd.FilterIndex switch
            {
                1 => DumpType.TXT,
                2 => DumpType.XML,
                _ => DumpType.TXT
            };
            Exporter.ExportDump(sfd.FileName, selectedItem, dumpType);
        }

        private void btnImportRaw_Click(object sender, EventArgs e)
        {
            if (FailIfNothingSelected()) return;
            var selectedItems = GetSelectedAssetItems();

            if (GetSelectedCount() > 1)
                BatchImportRaw(selectedItems);
            else
                SingleImportRaw(selectedItems[0]);
            UpdateAssetInfo();
        }

        private void BatchImportRaw(List<AssetItem> selectedItems)
        {
            var fd = new OpenFolderDialog
            {
                Title = @"Select an input path"
            };
            if (fd.ShowDialog(this) != DialogResult.OK) return;

            var dialog = new BatchImport(selectedItems, fd.Folder, BatchImportType.Dump);
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var batchItems = dialog.batchItems;
            if (batchItems == null) return;
            foreach (var batchItem in batchItems)
            {
                var selectedFilePath = batchItem.ImportFile;
                var affectedItem = batchItem.Item;

                var replacer = selectedFilePath.EndsWith(".dat") ?
                    Importer.ImportRawAsset(selectedFilePath, affectedItem) :
                    Importer.ImportDump(selectedFilePath, affectedItem, DumpType.TXT);

                Workspace.AddReplacer(ref affectedItem, replacer);
            }
        }

        private void SingleImportRaw(AssetItem selectedItem)
        {
            var ofd = new OpenFileDialog
            {
                Title = @"Import raw asset",
                Filter = @"Raw Unity asset (*.dat)|*.dat"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var replacer = Importer.ImportRawAsset(ofd.FileName, selectedItem);
            Workspace.AddReplacer(ref selectedItem, replacer);
        }

        private void btnImportDump_Click(object sender, EventArgs e)
        {
            if (FailIfNothingSelected()) return;
            var selectedItems = GetSelectedAssetItems();
			
			if (GetSelectedCount() > 1)
				BatchImportDump(selectedItems);
			else
				SingleImportDump(selectedItems[0]);
            UpdateAssetInfo();
        }

        private void BatchImportDump(List<AssetItem> selectedItems)
        {
			var fd = new OpenFolderDialog
			{
				Title = @"Select an input path"
			};
			if (fd.ShowDialog(this) != DialogResult.OK) return;

			var dialog = new BatchImport(selectedItems, fd.Folder, BatchImportType.Dump);
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var batchItems = dialog.batchItems;
            if (batchItems == null) return;
            foreach (var batchItem in batchItems)
            {
                var selectedFilePath = batchItem.ImportFile;
                var affectedItem = batchItem.Item;

                var replacer = selectedFilePath.EndsWith(".dat") ?
                    Importer.ImportRawAsset(selectedFilePath, affectedItem) :
                    Importer.ImportDump(selectedFilePath, affectedItem, DumpType.TXT);

                Workspace.AddReplacer(ref affectedItem, replacer);
            }
        }
        
        private void SingleImportDump(AssetItem selectedItem)
        {
            var ofd = new OpenFileDialog
            {
                Title = @"Import dump",
                Filter = @"UAAE text dump (*.txt)|*.txt" // UAAE xml dump (*.xml)|*.xml
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var replacer = Importer.ImportDump(ofd.FileName, selectedItem, DumpType.TXT);
            Workspace.AddReplacer(ref selectedItem, replacer);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (FailIfNothingSelected()) return;
            var items = GetSelectedAssetItems();
            var editDialog = new EditDialog(this, Workspace, items);
            editDialog.ShowDialog(this);
            UpdateAssetInfo();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var dialog = new AddAssets(Workspace);
            if (dialog.ShowDialog() != DialogResult.OK) return;

            AddAssetItems(dialog.Items);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (FailIfNothingSelected()) return;
            RemoveAssetItems();
        }

        private bool FailIfNothingSelected()
        {
            switch (GetSelectedCount())
            {
                case 0:
                    MsgBoxUtils.ShowErrorDialog("No item selected.");
                    return true;
                default:
                    return false;
            }
        }

        private int GetSelectedCount() => assetList.SelectedItems.Count;

        private void MenuSave_Click(object sender, EventArgs e) => SaveFiles(true);

        private void MenuSaveAs_Click(object sender, EventArgs e) => SaveFiles();

        private void MenuCreateInstallerPackageFile_Click(object sender, EventArgs e)
        {
            new ModMakerDialog(Workspace).ShowDialog();
        }

        private void MenuClose_Click(object sender, EventArgs e)
        {
            if (Workspace.Modified) AskSaveChanges();
            else CloseFiles();
        }

        private void MenuSearchByName_Click(object sender, EventArgs e)
        {
            var dialog = new AssetNameSearch();
            dialog.ShowDialog();
            if (dialog.ok)
            {
                searchStart = 0;
                if (dialog.startAtSelection)
                {
                    var selIndices = new List<int>();
                    foreach (int selIndex in assetList.SelectedIndices)
                    {
                        assetList.Items[selIndex].Selected = false;
                        selIndices.Add(selIndex);
                    }
                    searchStart = selIndices.Count != 0 ? selIndices[^1] : 0;
                }
                searchText = dialog.text;
                searchDown = dialog.isDown;
                searchCaseSensitive = dialog.caseSensitive;
                searchStartAtSelection = dialog.startAtSelection;
                searching = true;
                NextSearch();
            }
        }

        private void MenuContinueSearchF3_Click(object sender, EventArgs e) => NextSearch();

        private void NextSearch()
        {
            var foundResult = false;
            if (searching)
            {
                var items = assetList.Items;
                if (searchDown)
                {
                    for (var i = searchStart; i < items.Count; i++)
                    {
                        var name = items[i].SubItems[0].Text;

                        if (!Extensions.WildcardMatches(name, searchText, searchCaseSensitive))
                            continue;

                        assetList.Items[i].Selected = true;
                        assetList.EnsureVisible(i);
                        searchStart = i + 1;
                        foundResult = true;
                        break;
                    }
                }
                else
                {
                    for (var i = searchStart; i >= 0; i--)
                    {
                        var name = items[i].SubItems[0].Text;

                        if (!Extensions.WildcardMatches(name, searchText, searchCaseSensitive))
                            continue;

                        assetList.Items[i].Selected = true;
                        assetList.EnsureVisible(i);
                        searchStart = i - 1;
                        foundResult = true;
                        break;
                    }
                }
                assetList.Select();
            }

            if (foundResult)
                return;

            MsgBoxUtils.ShowInfoDialog("Can't find any assets that match.", MessageBoxButtons.OK);

            searchText = "";
            searchStart = 0;
            searchDown = false;
            searching = false;
        }

        private void MenuGoToAsset_Click(object sender, EventArgs e)
        {
            var dialog = new GoToAssetDialog(Workspace);
            if (dialog.ShowDialog() != DialogResult.OK) return;
            var foundResult = false;
            for (var i = 0; i < assetList.Items.Count; i++)
            {
                var item = Workspace.LoadedAssets[i];

                if (item.FileID != dialog.FileID || item.PathID != dialog.PathID)
                    continue;

                assetList.Items[i].Selected = true;
                assetList.EnsureVisible(i);
                foundResult = true;
                break;
            }
            if (!foundResult)
            {
                MsgBoxUtils.ShowInfoDialog("Asset not found.", MessageBoxButtons.OK);
                return;
            }
            assetList.Select();
        }

        private void MenuBinaryContentSearch_Click(object sender, EventArgs e)
        {
            // todo
        }

        private void MenuMonobehaviourSearch_Click(object sender, EventArgs e)
        {
            // todo
        }

        private void MenuTransformSearch_Click(object sender, EventArgs e)
        {
            // todo
        }

        private void MenuDependencies_Click(object sender, EventArgs e)
        {
            // todo
        }
    }
}