using System;
using System.IO;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using UnityTools;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class AssetData : Form
    {
        public string TempPath;
        public AssetTypeValueField BaseField;

        public AssetData(AssetTypeValueField baseField)
        {
            InitializeComponent();
            BaseField = baseField;
            PopulateTree();
        }

        private void PopulateTree()
        {
            var baseItemNode = new TreeNode($"{BaseField.GetFieldType()} {BaseField.GetName()}");
            rawViewTree.Nodes.Add(baseItemNode);
            RecursiveTreeLoad(BaseField, baseItemNode);
        }

        private static void RecursiveTreeLoad(AssetTypeValueField assetField, TreeNode node)
        {
            if (assetField.ChildrenCount <= 0) return;
            foreach (var children in assetField.Children)
            {
                if (children == null) return;
                var value = children.GetValue();
                var valueStr = "";
                if (value != null)
                {
                    var evt = value.GetValueType();
                    var quote = "";
                    if (evt == EnumValueTypes.String)
                    {
                        quote = "\"";
                    }
                    if ((int)evt >= 1 && (int)evt <= 12)
                    {
                        valueStr = $" = {quote}{value.AsString()}{quote}";
                    }
                    if (evt is EnumValueTypes.Array)
                    {
                        var isOneItem = children.ChildrenCount == 1;
                        valueStr = $" ({children.ChildrenCount} {(isOneItem ? "item" : "items")})";
                    }
                    else if (evt is EnumValueTypes.ByteArray)
                    {
                        var size = value.AsByteArray().size;
                        var isOneItem = size == 1;
                        valueStr = $" ({size} {(isOneItem ? "item" : "items")})";
                    }
                }

                var childNode = new TreeNode($"{children.GetFieldType()} {children.GetName() + valueStr}");
                node.Nodes.Add(childNode);
                RecursiveTreeLoad(children, childNode);
            }
        }

        private void CboxDumpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TempPath = Path.GetTempFileName();
            var dumpType = (DumpType)cboxDumpType.SelectedIndex;
            AssetExporter.ExportDump(TempPath, BaseField, dumpType);
            boxDumpView.Lines = File.ReadAllLines(TempPath);
        }

        private void OpenAll_Click(object sender, EventArgs e) => rawViewTree?.ExpandAll();

        private void CloseAll_Click(object sender, EventArgs e) => rawViewTree?.CollapseAll();

        private void OpenDown_Click(object sender, EventArgs e) => rawViewTree.SelectedNode?.ExpandAll();

        private void CloseDown_Click(object sender, EventArgs e) => rawViewTree.SelectedNode?.Collapse(false);

        private void AssetData_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (File.Exists(TempPath))
            {
                File.Delete(TempPath);
            }
        }
    }
}