using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AssetsAdvancedEditor.Assets;
using AssetsAdvancedEditor.Plugins;
using AssetsAdvancedEditor.Utils;
using AssetsAdvancedEditor.Winforms;
using UnityTools;

namespace Plugins.Texture.Options
{
    public class ImportTextureOption : PluginOption
    {
        public ImportTextureOption() => Action = PluginAction.Import;

        public override bool IsValidForPlugin(AssetsManager am, List<AssetItem> selectedItems)
        {
            Description = "Batch import textures";

            if (selectedItems.Count <= 1)
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
            foreach (var item in selectedItems)
            {
                if (!item.Cont.HasInstance)
                {
                    item.Cont = new AssetContainer(item.Cont, TextureHelper.GetByteArrayTexture(workspace, item));
                }
            }

            var ofd = new OpenFolderDialog
            {
                Title = "Select import directory"
            };
            if (ofd.ShowDialog(owner) != DialogResult.OK)
                return false;

            var dir = ofd.Folder;
            var dialog = new BatchImport(selectedItems, dir, BatchImportType.Image);
            if (dialog.ShowDialog(owner) != DialogResult.OK)
                return false;

            var batchItems = dialog.batchItems;
            var success = ImportTextures(owner, batchItems);
            if (success)
            {
                foreach (var batchItem in batchItems)
                {
                    if (batchItem.HasMatchingFile)
                    {
                        var item = batchItem.Item;
                        var savedAsset = item.Cont.TypeInstance.WriteToByteArray();
                        var replacer = AssetModifier.CreateAssetReplacer(batchItem.Item, savedAsset);
                        workspace.AddReplacer(ref item, replacer, new MemoryStream(savedAsset));
                    }
                }
                return true;
            }
            return false;
        }

        private bool ImportTextures(IWin32Window owner, List<BatchImportItem> batchItems)
        {
            var errorBuilder = new StringBuilder();

            foreach (var batchItem in batchItems)
            {
                var item = batchItem.Item;
                var cont = item.Cont;

                var errorAssetName = $"{Path.GetFileName(cont.FileInstance.path)}/{item.PathID}";
                var selectedFilePath = batchItem.ImportFile;

                if (!cont.HasInstance)
                    continue;

                var baseField = cont.TypeInstance.GetBaseField();
                var format = (TextureFormat)baseField.Get("m_TextureFormat").GetValue().AsInt();

                var encodedBytes = TextureManager.ImportTexture(selectedFilePath, format, out var size);

                if (encodedBytes == null)
                {
                    errorBuilder.AppendLine($"[{errorAssetName}]: Failed to encode texture with '{format}' format");
                    continue;
                }

                var m_StreamData = baseField.Get("m_StreamData");
                m_StreamData.Get("offset").GetValue().Set(0);
                m_StreamData.Get("size").GetValue().Set(0);
                m_StreamData.Get("path").GetValue().Set("");

                if (!baseField.Get("m_MipCount").IsDummy())
                    baseField.Get("m_MipCount").GetValue().Set(1);

                baseField.Get("m_TextureFormat").GetValue().Set((int)format);
                baseField.Get("m_CompleteImageSize").GetValue().Set(encodedBytes.Length);

                baseField.Get("m_Width").GetValue().Set(size.Width);
                baseField.Get("m_Height").GetValue().Set(size.Height);

                var image_data = baseField.Get("image data");
                image_data.GetValue().type = EnumValueTypes.ByteArray;
                image_data.TemplateField.valueType = EnumValueTypes.ByteArray;
                var byteArray = new AssetTypeByteArray
                {
                    size = (uint)encodedBytes.Length,
                    data = encodedBytes
                };
                image_data.GetValue().Set(byteArray);
            }

            if (errorBuilder.Length > 0)
            {
                var firstLines = errorBuilder.ToString().Split('\n').Take(10).ToArray();
                var firstLinesStr = string.Join('\n', firstLines);
                MsgBoxUtils.ShowErrorDialog(owner, $"Some errors occurred while exporting:\n{firstLinesStr}");
            }

            return true;
        }
    }
}
