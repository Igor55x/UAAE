using System;
using System.Windows.Forms;
using AssetsAdvancedEditor.Utils;
using UnityTools;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class BundleLoader : Form
    {
        public bool Loaded;
        public BundleFileInstance BundleInst;

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
                    lblCompType.Text = @"Lzma";
                    break;
                case AssetBundleCompressionType.Lz4 or AssetBundleCompressionType.Lz4HC:
                    lblCompType.Text = @"Lz4";
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
                btnLoad.Enabled = true;
                btnCompress.Enabled = true;
            }
            else
            {
                lblNote.Text = @"Bundle is compressed. You must decompress the bundle to load.";
                btnDecompress.Enabled = true;
                BundleInst.file.UnpackInfoOnly();
            }
            lblBundleSize.Text = Extensions.GetFormattedByteSize(GetBundleDataDecompressedSize(BundleInst.file));
        }

        private void BtnDecompress_Click(object sender, EventArgs e)
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

        private void BtnCompress_Click(object sender, EventArgs e)
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

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            Loaded = true;
            Close();
        }

        private long GetBundleDataDecompressedSize(AssetBundleFile bundle)
        {
            var totalSize = 0L;
            foreach (var info in bundle.Metadata.DirectoryInfo)
            {
                totalSize += info.DecompressedSize;
            }
            return totalSize;
        }
    }
}