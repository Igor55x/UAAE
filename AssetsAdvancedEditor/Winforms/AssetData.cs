using System;
using System.IO;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using UnityTools;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class AssetData : Form
    {
        public AssetTypeValueField BaseField;
        public string TempPath;
        public AssetData(AssetTypeValueField baseField)
        {
            InitializeComponent();
            BaseField = baseField;
            PopulateTree();
            LoadDump();
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
                var value = "";
                if (children.GetValue() != null)
                {
                    var evt = children.GetValue().GetValueType();
                    var quote = "";
                    if (evt == EnumValueTypes.String)
                    {
                        quote = "\"";
                    }
                    if ((int)evt >= 1 && (int)evt <= 12)
                    {
                        value = $" = {quote}{children.GetValue().AsString()}{quote}";
                    }
                    if (evt is EnumValueTypes.Array)
                    {
                        var isOneItem = children.ChildrenCount == 1;
                        value = $" ({children.ChildrenCount} {(isOneItem ? "item" : "items")})";
                    }
                    else if (evt is EnumValueTypes.ByteArray)
                    {
                        var size = children.GetValue().AsByteArray().size;
                        var isOneItem = size == 1;
                        value = $" ({size} {(isOneItem ? "item" : "items")})";
                    }
                }

                var childNode = new TreeNode($"{children.GetFieldType()} {children.GetName() + value}");
                node.Nodes.Add(childNode);
                RecursiveTreeLoad(children, childNode);
            }
        }

        private void LoadDump()
        {
            var filePath = Path.GetTempFileName();
            new AssetExporter().ExportDump(filePath, BaseField, DumpType.TXT);
            TempPath = filePath;
            boxDumpView.Lines = File.ReadAllLines(filePath);
        }

        private void openAll_Click(object sender, EventArgs e) => rawViewTree?.ExpandAll();

        private void closeAll_Click(object sender, EventArgs e) => rawViewTree?.CollapseAll();

        private void openDown_Click(object sender, EventArgs e) => rawViewTree.SelectedNode?.ExpandAll();

        private void closeDown_Click(object sender, EventArgs e) => rawViewTree.SelectedNode?.Collapse(false);

        private void AssetData_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (File.Exists(TempPath))
            {
                File.Delete(TempPath);
            }
        }
    }
}