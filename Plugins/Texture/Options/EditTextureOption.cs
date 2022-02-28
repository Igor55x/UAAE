using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using AssetsAdvancedEditor.Plugins;
using UnityTools;

namespace Plugins.Texture.Options
{
    public class EditTextureOption : PluginOption
    {
        public EditTextureOption() => Action = PluginAction.Import;

        public override bool IsValidForPlugin(AssetsManager am, List<AssetItem> selectedItems)
        {
            Description = "Edit texture";

            if (selectedItems.Count != 1)
                return false;

            var classId = AssetHelper.FindAssetClassByName(am.classFile, "Texture2D").classId;

            foreach (var item in selectedItems)
            {
                if (item.TypeID != classId)
                    return false;
            }
            return true;
        }

        public override bool ExecutePlugin(IWin32Window owner, AssetsWorkspace workspace, List<AssetItem> selectedItems)
        {
            var item = selectedItems[0];

            var texField = TextureHelper.GetByteArrayTexture(workspace, item).GetBaseField();

            var texFile = TextureFile.ReadTextureFile(texField);
            var editTexDialog = new EditTextureDialog(texFile, texField);
            if (editTexDialog.ShowDialog(owner) != DialogResult.OK)
                return false;

            var savedAsset = texField.WriteToByteArray();
            var replacer = AssetModifier.CreateAssetReplacer(item, savedAsset);

            workspace.AddReplacer(ref item, replacer, new MemoryStream(savedAsset));
            return true;
        }
    }
}
