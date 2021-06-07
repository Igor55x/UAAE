﻿using System;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using AssetsTools.NET;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class AssetData : Form
    {
        public AssetsWorkspace Workspace;
        public AssetTypeValueField BaseField;
        public AssetData(AssetsWorkspace workspace, AssetTypeValueField baseField)
        {
            InitializeComponent();
            Workspace = workspace;
            BaseField = baseField;
            PopulateTree();
        }

        private void PopulateTree()
        {
            rawViewTree.Nodes.Add(BaseField.GetFieldType() + " " + BaseField.GetName());
            RecursiveTreeLoad(BaseField, rawViewTree.Nodes[0]);
        }

        private void RecursiveTreeLoad(AssetTypeValueField assetField, TreeNode node)
        {
            if (assetField.childrenCount == 0) return;
            foreach (var children in assetField.children)
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
                    if (1 <= (int)evt && (int)evt <= 12)
                    {
                        value = $" = {quote}{children.GetValue().AsString()}{quote}";
                    }
                    var isOneItem = children.childrenCount == 1;
                    if (evt is EnumValueTypes.Array or EnumValueTypes.ByteArray)
                    {
                        value = $" ({children.childrenCount} {(isOneItem ? "item" : "items")})";
                    }
                }

                node.Nodes.Add($"{children.GetFieldType()} {children.GetName() + value}");
                RecursiveTreeLoad(children, node.LastNode);
            }
        }

        private void openAll_Click(object sender, EventArgs e) => rawViewTree?.ExpandAll();

        private void closeAll_Click(object sender, EventArgs e) => rawViewTree?.CollapseAll();

        private void openDown_Click(object sender, EventArgs e) => rawViewTree.SelectedNode?.ExpandAll();

        private void closeDown_Click(object sender, EventArgs e) => rawViewTree.SelectedNode?.Collapse(false);
    }
}