using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using AssetsAdvancedEditor.Plugins;
using AssetsAdvancedEditor.Utils;
using UnityTools;

namespace Plugins.Texture.Options
{
    public class ExportTextureOption : PluginOption
    {
        public ExportTextureOption() => Action = PluginAction.Export;

        public override bool IsValidForPlugin(AssetsManager am, List<AssetItem> selectedItems)
        {
            Description = selectedItems.Count > 1 ? "Batch export Png/Tga" : "Export Png/Tga";

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
            return selectedItems.Count > 1
                ? BatchExport(owner, workspace, selectedItems)
                : SingleExport(owner, workspace, selectedItems[0]);
        }

        public bool BatchExport(IWin32Window owner, AssetsWorkspace workspace, List<AssetItem> selectedItems)
        {
            foreach (var item in selectedItems)
            {
                if (!item.Cont.HasInstance)
                {
                    item.Cont = new AssetContainer(item.Cont, TextureHelper.GetByteArrayTexture(workspace, item));
                }
            }

            var ofd = new OpenFolderDialog
            {
                Title = "Select export directory"
            };
            if (ofd.ShowDialog(owner) != DialogResult.OK)
                return false;

            var dir = ofd.Folder;
            var errorBuilder = new StringBuilder();
            foreach (var item in selectedItems)
            {
                var fileInst = item.Cont.FileInstance;
                var errorAssetName = $"{Path.GetFileName(fileInst.path)}/{item.PathID}";
                var texBaseField = workspace.GetBaseField(item);
                var texFile = TextureFile.ReadTextureFile(texBaseField);

                //0x0 texture, usually called like Font Texture or smth
                if (texFile.m_Width == 0 && texFile.m_Height == 0)
                    continue;

                var fixedName = Extensions.ReplaceInvalidFileNameChars(texFile.m_Name);
                var file = Path.Combine(dir, $"{fixedName}-{Path.GetFileName(fileInst.path)}-{item.PathID}.png");

                //bundle resS
                if (!TextureHelper.GetResSTexture(texFile, item))
                {
                    var resSName = Path.GetFileName(texFile.m_StreamData.path);
                    errorBuilder.AppendLine($"[{errorAssetName}]: resS was detected but {resSName} was not found in bundle");
                    continue;
                }

                var data = TextureHelper.GetRawTextureBytes(texFile, fileInst);

                if (data == null)
                {
                    var resSName = Path.GetFileName(texFile.m_StreamData.path);
                    errorBuilder.AppendLine($"[{errorAssetName}]: resS was detected but {resSName} was not found on disk");
                    continue;
                }

                var success = TextureManager.ExportTexture(data, file, texFile.m_Width, texFile.m_Height, (TextureFormat)texFile.m_TextureFormat);
                if (success) continue;
                errorBuilder.AppendLine($"[{errorAssetName}]: Failed to decode texture with '{(TextureFormat)texFile.m_TextureFormat}' format");
            }

            if (errorBuilder.Length > 0)
            {
                var firstLines = errorBuilder.ToString().Split('\n').Take(10).ToArray();
                var firstLinesStr = string.Join('\n', firstLines);
                MsgBoxUtils.ShowErrorDialog(owner, $"Some errors occurred while exporting:\n{firstLinesStr}");
            }

            return true;
        }

        public bool SingleExport(IWin32Window owner, AssetsWorkspace workspace, AssetItem selectedItem)
        {
            var fileInst = selectedItem.Cont.FileInstance;
            if (!selectedItem.Cont.HasInstance)
            {
                selectedItem.Cont = new AssetContainer(selectedItem.Cont, TextureHelper.GetByteArrayTexture(workspace, selectedItem));
            }
            var texField = selectedItem.Cont.TypeInstance.GetBaseField();
            var texFile = TextureFile.ReadTextureFile(texField);
            var fixedName = Extensions.ReplaceInvalidFileNameChars(texFile.m_Name);
            var sfd = new SaveFileDialog
            {
                Title = @"Save texture",
                Filter = @"PNG file (*.png)|*.png|TGA file (*.tga)|*.tga|All types (*.*)|*.*",
                FileName = $"{fixedName}-{Path.GetFileName(fileInst.path)}-{selectedItem.PathID}"
            };
            if (sfd.ShowDialog(owner) != DialogResult.OK)
                return false;

            var file = sfd.FileName;
            var errorAssetName = $"{Path.GetFileName(fileInst.path)}/{selectedItem.PathID}";

            //bundle resS
            if (!TextureHelper.GetResSTexture(texFile, selectedItem))
            {
                var resSName = Path.GetFileName(texFile.m_StreamData.path);
                MsgBoxUtils.ShowErrorDialog($"[{errorAssetName}]: resS was detected but {resSName} was not found in bundle");
                return false;
            }

            var data = TextureHelper.GetRawTextureBytes(texFile, fileInst);

            if (data == null)
            {
                var resSName = Path.GetFileName(texFile.m_StreamData.path);
                MsgBoxUtils.ShowErrorDialog($"[{errorAssetName}]: resS was detected but {resSName} was not found on disk");
                return false;
            }

            var success = TextureManager.ExportTexture(data, file, texFile.m_Width, texFile.m_Height, (TextureFormat)texFile.m_TextureFormat);
            if (!success)
            {
                MsgBoxUtils.ShowErrorDialog(owner, $"[{errorAssetName}]: Failed to decode texture with '{(TextureFormat)texFile.m_TextureFormat}' format");
            }
            return success;
        }
    }
}
