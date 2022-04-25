using System;
using System.Windows.Forms;
using AssetsAdvancedEditor.Utils;
using UnityTools;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace Plugins.Texture
{
    public partial class TextureEditor : Form
    {
        private TextureFile Tex { get; }
        private AssetTypeValueField TexField { get; }
        private byte[] ImageBytes;
        public TextureEditor(TextureFile tex, AssetTypeValueField texField, byte[] data)
        {
            InitializeComponent();

            Tex = tex;
            TexField = texField;
            ImageBytes = data;

            foreach (var format in Enum.GetNames(typeof(TextureFormat)))
            {
                cboxTextureFormat.Items.Add(format);
            }

            foreach (var filterMode in Enum.GetNames(typeof(FilterMode)))
            {
                cboxFilterMode.Items.Add(filterMode);
            }

            foreach (var wrapMode in Enum.GetNames(typeof(WrapMode)))
            {
                cboxWrapU.Items.Add(wrapMode);
                cboxWrapV.Items.Add(wrapMode);
            }

            foreach (var colorSpace in Enum.GetNames(typeof(ColorSpace)))
            {
                cboxColorSpace.Items.Add(colorSpace);
            }

            tboxTextureName.Text = tex.m_Name;
            cboxTextureFormat.SelectedIndex = tex.m_TextureFormat - 1;
            chboxHasMipMaps.Checked = tex.m_MipMap;
            chboxIsReadable.Checked = tex.m_IsReadable;
            cboxFilterMode.SelectedIndex = tex.m_TextureSettings.m_FilterMode;
            tboxAniso.Text = tex.m_TextureSettings.m_Aniso.ToString();
            tboxMipMapBias.Text = tex.m_TextureSettings.m_MipBias.ToString();
            cboxWrapU.SelectedIndex = tex.m_TextureSettings.m_WrapU;
            cboxWrapV.SelectedIndex = tex.m_TextureSettings.m_WrapV;
            tboxLightmapFormat.Text = tex.m_LightmapFormat.ToString();
            cboxColorSpace.SelectedIndex = tex.m_ColorSpace;
        }

        private void BtnLoad_Click(object sender, EventArgs e)
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
                Tex.m_Width = image.Width;
                Tex.m_Height = image.Height;

                image.Mutate(i => i.Flip(FlipMode.Vertical));
                var imgBytes = new byte[Tex.m_Width * Tex.m_Height * 4];
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

        private void BtnView_Click(object sender, EventArgs e)
        {
            new TextureViewer(Tex, ImageBytes).ShowDialog();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (ImageBytes == null)
            {
                DialogResult = DialogResult.Cancel;
            }

            var format = (TextureFormat)(cboxTextureFormat.SelectedIndex + 1);
            var encodedBytes = TextureManager.EncodeTexture(ImageBytes, Tex.m_Width, Tex.m_Height, format);

            var m_StreamData = TexField.Get("m_StreamData");
            m_StreamData.Get("offset").GetValue().Set(0);
            m_StreamData.Get("size").GetValue().Set(0);
            m_StreamData.Get("path").GetValue().Set("");

            var image_data = TexField.Get("image data");
            image_data.GetValue().type = EnumValueTypes.ByteArray;
            image_data.TemplateField.valueType = EnumValueTypes.ByteArray;
            var byteArray = new AssetTypeByteArray
            {
                size = (uint)encodedBytes.Length,
                data = encodedBytes
            };
            image_data.GetValue().Set(byteArray);

            TexField.Get("m_ColorSpace").GetValue().Set(cboxColorSpace.SelectedIndex);
            TexField.Get("m_LightmapFormat").GetValue().Set(tboxLightmapFormat.Text);

            var m_TextureSettings = TexField.Get("m_TextureSettings");
            m_TextureSettings.Get("m_FilterMode").GetValue().Set(cboxFilterMode.SelectedIndex);
            m_TextureSettings.Get("m_Aniso").GetValue().Set(tboxAniso.Text);
            m_TextureSettings.Get("m_MipBias").GetValue().Set(tboxMipMapBias.Text);
            m_TextureSettings.Get("m_WrapU").GetValue().Set(cboxWrapU.SelectedIndex);
            m_TextureSettings.Get("m_WrapV").GetValue().Set(cboxWrapV.SelectedIndex);
            m_TextureSettings.Get("m_WrapW").GetValue().Set(1);

            TexField.Get("m_TextureDimension").GetValue().Set(2);

            TexField.Get("m_ImageCount").GetValue().Set(1);

            TexField.Get("m_IsReadable").GetValue().Set(false);

            TexField.Get("m_MipCount").GetValue().Set(1);

            TexField.Get("m_TextureFormat").GetValue().Set((int)format);

            TexField.Get("m_CompleteImageSize").GetValue().Set(encodedBytes.Length);

            TexField.Get("m_Width").GetValue().Set(Tex.m_Width);
            TexField.Get("m_Height").GetValue().Set(Tex.m_Height);

            TexField.Get("m_Name").GetValue().Set(tboxTextureName.Text);
        }
    }
}
