using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using AssetsAdvancedEditor.Plugins;
using AssetsAdvancedEditor.Utils;
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
            var fileInst = item.Cont.FileInstance;
            var errorAssetName = $"{Path.GetFileName(fileInst.path)}/{item.PathID}";
            var texField = TextureHelper.GetByteArrayTexture(workspace, item).GetBaseField();
            var texFile = TextureFile.ReadTextureFile(texField);

            //bundle resS
            if (!TextureHelper.GetResSTexture(texFile, fileInst))
            {
                var resSName = Path.GetFileName(texFile.m_StreamData.path);
                MsgBoxUtils.ShowErrorDialog($"[{errorAssetName}]: resS was detected but {resSName} was not found in bundle");
                return false;
            }

            var data = TextureHelper.GetRawTextureBytes(texFile, fileInst);

            if (data == null)
            {
                var resSName = Path.GetFileName(texFile.m_StreamData.path);
                MsgBoxUtils.ShowErrorDialog(owner, $"[{errorAssetName}]: resS was detected but {resSName} was not found on disk");
                return false;
            }

            var editTexDialog = new TextureEditor(texFile, texField, data);
            if (editTexDialog.ShowDialog(owner) != DialogResult.OK)
                return false;

            var savedAsset = texField.WriteToByteArray();
            var replacer = AssetModifier.CreateAssetReplacer(item, savedAsset);

            workspace.AddReplacer(ref item, replacer, new MemoryStream(savedAsset));
            return true;
        }
    }
}
