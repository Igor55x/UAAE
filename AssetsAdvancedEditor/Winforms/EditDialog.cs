using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using AssetsAdvancedEditor.Plugins;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class EditDialog : Form
    {
        private new IWin32Window Owner { get; }
        private AssetsWorkspace Workspace { get; }
        private List<AssetItem> SelectedItems { get; }
        public EditDialog(IWin32Window owner, AssetsWorkspace workspace, List<AssetItem> selectedItems)
        {
            InitializeComponent();

            Owner = owner;
            Workspace = workspace;
            SelectedItems = selectedItems;
            var plugInfos = workspace.Pm.GetSupportedPlugins(selectedItems);
            foreach (var plugInfo in plugInfos)
            {
                lboxPluginsList.Items.Add(plugInfo);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (lboxPluginsList.SelectedItem is PluginMenuInfo plugInfo)
            {
                var plugOption = plugInfo.PluginOpt;
                plugOption.ExecutePlugin(Owner, Workspace, SelectedItems);
            }
        }
    }
}
