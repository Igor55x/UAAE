using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using AssetsAdvancedEditor.Utils;
using UnityTools;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class AddAssets : Form
    {
        public AssetsWorkspace Workspace;
        public List<AssetItem> Items;
        public long NextPathId;
        public AddAssets(AssetsWorkspace workspace)
        {
            InitializeComponent();
            Workspace = workspace;
            Items = new List<AssetItem>();
        }

        private void AddAssetsDialog_Load(object sender, EventArgs e)
        {
            var i = 0;
            foreach (var fileInst in Workspace.LoadedFiles)
            {
                cboxFileID.Items.Add($"{i++} - {fileInst.name}");
            }
            cboxFileID.SelectedIndex = 0;
            cboxTypePreset.SelectedIndex = 0;
            NextPathId = GetLastPathId();
            boxPathID.Text = GetNextPathId().ToString();
        }

        private long GetNextPathId() => ++NextPathId;

        private long GetLastPathId()
        {
            var fileId = cboxFileID.SelectedIndex;
            var lastId = Workspace.LoadedFiles[fileId].table.Info.Max(i => i.index);

            if (Workspace.NewAssets.Count == 0)
                return lastId;

            var newAssetsLastId = Workspace.NewAssets
                .Where(i => i.Value.GetFileID() == fileId)
                .Max(j => j.Key.pathID);
            return lastId > newAssetsLastId ? lastId : newAssetsLastId;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var cldb = Workspace.Am.classFile;
            var fileInst = Workspace.LoadedFiles[cboxFileID.SelectedIndex];
            AssetTypeTemplateField templateField;

            var fileId = cboxFileID.SelectedIndex;
            if (!long.TryParse(boxPathID.Text, out var pathId))
            {
                MsgBoxUtils.ShowErrorDialog("Path ID is invalid!");
                return;
            }
            var type = boxTypeNameOrID.Text;
            AssetClassID typeId;
            var createBlankAsset = chboxCreateBlankAssets.Checked;

            if (fileInst.file.typeTree.hasTypeTree)
            {
                if (!TryParseTypeTree(fileInst, ref type, createBlankAsset, out templateField, out typeId))
                {
                    if (!TryParseClassDatabase(ref type, createBlankAsset, out templateField, out typeId))
                    {
                        MsgBoxUtils.ShowErrorDialog("Class type is invalid!");
                        return;
                    }

                    //has typetree but had to lookup to cldb
                    //we need to add a new typetree entry because this is
                    //probably not a type that existed in this bundle
                    fileInst.file.typeTree.unity5Types.Add(C2T5.Cldb2TypeTree(cldb, typeId));
                }
            }
            else
            {
                if (!TryParseClassDatabase(ref type, createBlankAsset, out templateField, out typeId))
                {
                    MsgBoxUtils.ShowErrorDialog("Class type is invalid!");
                    return;
                }
            }

            var monoSelected = cboxTypePreset.SelectedIndex == 1;

            ushort monoId;
            if (typeId != AssetClassID.MonoBehaviour || !monoSelected)
            {
                monoId = ushort.MaxValue;
            }
            else
            {
                if (!ushort.TryParse(boxMonoID.Text, out monoId))
                {
                    MsgBoxUtils.ShowErrorDialog("Mono ID is invalid!");
                    return;
                }
            }

            if (!int.TryParse(boxCount.Text, out var count))
            {
                MsgBoxUtils.ShowErrorDialog("The count of assets being created is invalid!");
                return;
            }

            byte[] assetBytes;
            if (createBlankAsset)
            {
                var baseField = ValueBuilder.DefaultValueFieldFromTemplate(templateField);
                assetBytes = baseField.WriteToByteArray();
            }
            else
            {
                assetBytes = Array.Empty<byte>();
            }
            for (var i = 0; i < count; i++)
            {
                var item = new AssetItem
                {
                    Type = type,
                    TypeID = typeId,
                    FileID = fileId,
                    PathID = pathId + i,
                    Size = assetBytes.Length,
                    Modified = "*",
                    MonoID = monoId
                };
                Items.Add(item);
                Workspace.AddReplacer(ref item, AssetModifier.CreateAssetReplacer(item, assetBytes), new MemoryStream(assetBytes));
            }
            DialogResult = DialogResult.OK;
        }

        private bool TryParseClassDatabase(ref string type, bool createBlankAsset, out AssetTypeTemplateField templateField, out AssetClassID typeId)
        {
            templateField = null;

            var cldb = Workspace.Am.classFile;
            ClassDatabaseType cldbType;
            bool needsTypeId;
            if (Enum.TryParse(type, out typeId))
            {
                cldbType = AssetHelper.FindAssetClassByID(cldb, typeId);
                needsTypeId = false;
            }
            else
            {
                cldbType = AssetHelper.FindAssetClassByName(cldb, type);
                needsTypeId = true;
            }

            if (cldbType == null)
            {
                return false;
            }

            if (needsTypeId)
            {
                typeId = cldbType.classId;
            }

            if (createBlankAsset)
            {
                templateField = new AssetTypeTemplateField();
                if (cldbType.fields.Count == 0)
                {
                    typeId = AssetClassID.GameObject;
                    cldbType = AssetHelper.FindAssetClassByID(cldb, typeId);
                }
                type = cldbType.name.GetString(cldb);
                templateField.FromClassDatabase(cldb, cldbType, 0);
            }
            return true;
        }

        private static bool TryParseTypeTree(AssetsFileInstance fileInst, ref string type, bool createBlankAsset, out AssetTypeTemplateField templateField, out AssetClassID typeId)
        {
            templateField = null;

            var tt = fileInst.file.typeTree;
            Type_0D ttType;
            bool needsTypeId;
            if (Enum.TryParse(type, out typeId))
            {
                ttType = AssetHelper.FindTypeTreeTypeByID(tt, typeId);
                needsTypeId = false;
            }
            else
            {
                ttType = AssetHelper.FindTypeTreeTypeByName(tt, type);
                needsTypeId = true;
            }

            if (ttType == null)
            {
                return false;
            }

            if (needsTypeId)
            {
                typeId = ttType.ClassID;
            }

            if (createBlankAsset)
            {
                templateField = new AssetTypeTemplateField();
                if (ttType.ChildrenCount == 0)
                {
                    typeId = AssetClassID.GameObject;
                    ttType = AssetHelper.FindTypeTreeTypeByID(tt, typeId);
                }
                type = ttType.Children[0].GetTypeString(ttType.stringTable);
                templateField.From0D(ttType);
            }
            return true;
        }

        private void cboxFileID_SelectedIndexChanged(object sender, EventArgs e)
        {
            NextPathId = GetLastPathId();
            boxPathID.Text = GetNextPathId().ToString();
        }

        private void cboxTypePreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboxTypePreset.SelectedIndex)
            {
                case 0:
                    lblTypeNameOrID.Text = @"Type name/ID";
                    cboxMonoTypes.SendToBack();
                    break;
                case 1:
                    lblTypeNameOrID.Text = @"Class name";
                    cboxMonoTypes.BringToFront();
                    break;
            }
        }
    }
}
