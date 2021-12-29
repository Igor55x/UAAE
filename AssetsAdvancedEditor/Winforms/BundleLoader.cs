using System;
using System.Windows.Forms;
using UnityTools;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class BundleLoader : Form
    {
        public BundleFileInstance BundleInst;
        public bool Loaded;

        public BundleLoader(BundleFileInstance bundleInst)
        {
            InitializeComponent();
            BundleInst = bundleInst;
        }

        private void BundleLoader_Load(object sender, EventArgs e)
        {
            if (BundleInst == null) return;
            var compType = BundleInst.file.Header.GetCompressionType();
            switch (compType)
            {
                case AssetBundleCompressionType.None:
                    lblCompType.Text = @"None";
                    break;
                case AssetBundleCompressionType.Lzma:
                    lblCompType.Text = @"LZMA";
                    break;
                case AssetBundleCompressionType.Lz4 or AssetBundleCompressionType.Lz4HC:
                    lblCompType.Text = @"LZ4";
                    break;
                default:
                    lblCompType.Text = @"Unknown";
                    lblNote.Text = @"Looks like the bundle is packed with a custom compression type and cannot be unpacked!";
                    btnLoad.Enabled = false;
                    btnDecompress.Enabled = false;
                    btnCompress.Enabled = false;
                    return;
            }

            if (compType == 0)
            {
                lblNote.Text = @"Bundle is not compressed. You can load it or compress it.";
                btnCompress.Enabled = true;
                btnLoad.Enabled = true;
            }
            else
            {
                lblNote.Text = @"Bundle is compressed. You must decompress the bundle to load.";
                btnDecompress.Enabled = true;
            }
        }

        private void btnDecompress_Click(object sender, EventArgs e)
        {
            if (BundleInst == null) return;
            var dialog = new BundleDecompression(BundleInst);
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                if (result != DialogResult.Abort)
                    Close();
                return;
            }

            lblNote.Text = @"Decompressing...";
            btnDecompress.Enabled = false;
            btnCompress.Enabled = false;
            dialog.Bw.RunWorkerCompleted += delegate
            {
                lblNote.Text = @"Done. Click Load to open the bundle.";
                btnDecompress.Enabled = false;
                btnCompress.Enabled = true;
                btnLoad.Enabled = true;
            };
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            if (BundleInst == null) return;
            var dialog = new BundleCompression(BundleInst);
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK && dialog.Compressed)
            {
                if (result != DialogResult.Abort)
                    Close();
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Loaded = true;
            Close();
        }
    }
}