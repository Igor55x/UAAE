using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using AssetsAdvancedEditor.Utils;
using UnityTools;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace Plugins.Texture
{
    public partial class EditTextureDialog : Form
    {
        private TextureFile tex { get; }
        private AssetTypeValueField texField { get; }
        private byte[] imgBytes;
        public EditTextureDialog(TextureFile tex, AssetTypeValueField texField)
        {
            InitializeComponent();
            this.tex = tex;
            this.texField = texField;
            imgBytes = null;

            var formats = Enum.GetValues(typeof(TextureFormat));
            foreach (var format in formats)
            {
                cboxTextureFormat.Items.Add(format);
            }

            tboxTextureName.Text = tex.m_Name;
            cboxTextureFormat.SelectedIndex = tex.m_TextureFormat - 1;
            chboxHasMipMaps.Checked = tex.m_MipMap;
            chboxIsReadable.Checked = tex.m_IsReadable;
            cboxFilterMode.SelectedIndex = tex.m_TextureSettings.m_FilterMode;
            tboxAniso.Text = tex.m_TextureSettings.m_Aniso.ToString();
            tboxMipMapBias.Text = tex.m_TextureSettings.m_MipBias.ToString(CultureInfo.CurrentCulture);
            cboxWrapU.SelectedIndex = tex.m_TextureSettings.m_WrapU;
            cboxWrapV.SelectedIndex = tex.m_TextureSettings.m_WrapV;
            tboxLightmapFormat.Text = tex.m_LightmapFormat.ToString();
            cboxColorSpace.SelectedIndex = tex.m_ColorSpace;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = @"Open texture",
                Filter = @"PNG file (*.png)|*.png|TGA file (*.tga)|*.tga|All types (*.*)|*.*"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            try
            {
                using var image = Image.Load<Rgba32>(ofd.FileName);
                tex.m_Width = image.Width;
                tex.m_Height = image.Height;

                image.Mutate(i => i.Flip(FlipMode.Vertical));
                var imgBytes = new byte[tex.m_Width * tex.m_Height * Unsafe.SizeOf<Rgba32>()];
                image.CopyPixelDataTo(imgBytes);
                if (imgBytes == null)
                {
                    MsgBoxUtils.ShowErrorDialog(this, "Failed to parse current texture.");
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtils.ShowErrorDialog(this, "Something went wrong when importing the texture:\n" + ex);
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            // todo
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (imgBytes == null)
            {
                DialogResult = DialogResult.Cancel;
            }

            var format = (TextureFormat)(cboxTextureFormat.SelectedIndex + 1);
            var encodedBytes = TextureManager.EncodeTexture(imgBytes, tex.m_Width, tex.m_Height, format);

            var m_StreamData = texField.Get("m_StreamData");
            m_StreamData.Get("offset").GetValue().Set(0);
            m_StreamData.Get("size").GetValue().Set(0);
            m_StreamData.Get("path").GetValue().Set("");

            var image_data = texField.Get("image data");
            image_data.GetValue().type = EnumValueTypes.ByteArray;
            image_data.TemplateField.valueType = EnumValueTypes.ByteArray;
            var byteArray = new AssetTypeByteArray
            {
                size = (uint)encodedBytes.Length,
                data = encodedBytes
            };
            image_data.GetValue().Set(byteArray);

            texField.Get("m_ColorSpace").GetValue().Set(cboxColorSpace.SelectedIndex);
            texField.Get("m_LightmapFormat").GetValue().Set(tboxLightmapFormat.Text);

            var m_TextureSettings = texField.Get("m_TextureSettings");
            m_TextureSettings.Get("m_FilterMode").GetValue().Set(cboxFilterMode.SelectedIndex);
            m_TextureSettings.Get("m_Aniso").GetValue().Set(tboxAniso.Text);
            m_TextureSettings.Get("m_MipBias").GetValue().Set(tboxMipMapBias.Text);
            m_TextureSettings.Get("m_WrapU").GetValue().Set(cboxWrapU.SelectedIndex);
            m_TextureSettings.Get("m_WrapV").GetValue().Set(cboxWrapV.SelectedIndex);
            m_TextureSettings.Get("m_WrapW").GetValue().Set(1);

            texField.Get("m_TextureDimension").GetValue().Set(2);

            texField.Get("m_ImageCount").GetValue().Set(1);

            texField.Get("m_IsReadable").GetValue().Set(false);

            texField.Get("m_MipCount").GetValue().Set(1);

            texField.Get("m_TextureFormat").GetValue().Set((int)format);

            texField.Get("m_CompleteImageSize").GetValue().Set(encodedBytes.Length);

            texField.Get("m_Width").GetValue().Set(tex.m_Width);
            texField.Get("m_Height").GetValue().Set(tex.m_Height);

            texField.Get("m_Name").GetValue().Set(tboxTextureName.Text);
        }
    }
}
