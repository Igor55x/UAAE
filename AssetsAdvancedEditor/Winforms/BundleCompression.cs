using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using AssetsAdvancedEditor.Utils;
using UnityTools;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class BundleCompression : Form
    {
        public BundleFileInstance BundleInst;
        public bool Compressed;
        public BundleCompression(BundleFileInstance bundleInst)
        {
            InitializeComponent();
            BundleInst = bundleInst;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (BundleInst == null) return;
            AssetBundleCompressionType compType;
            if (rbtnLZ4.Checked)
                compType = AssetBundleCompressionType.Lz4;
            else if (rbtnLZMA.Checked)
                compType = AssetBundleCompressionType.Lzma;
            else
            {
                MsgBoxUtils.ShowErrorDialog("You didn't choose any compression method!\n" +
                                            "Please go back and select it.\n");
                DialogResult = DialogResult.None;
                return;
            }
            try
            {
                var bw = new BackgroundWorker();
                var sfd = new SaveFileDialog
                {
                    FileName = BundleInst.name + ".packed",
                    Filter = @"All types (*.*)|*.*"
                };
                if (sfd.ShowDialog() != DialogResult.OK) return;
                bw.DoWork += delegate { CompressBundle(BundleInst, sfd.FileName, compType); };
                bw.RunWorkerCompleted += delegate
                {
                    MsgBoxUtils.ShowInfoDialog("The bundle file has been successfully packed!", MessageBoxButtons.OK);
                    Compressed = true;
                };
                bw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MsgBoxUtils.ShowErrorDialog("Something went wrong when packing the bundle:\n" + ex);
                DialogResult = DialogResult.Abort;
            }
        }

        private static void CompressBundle(BundleFileInstance bundleInst, string path, AssetBundleCompressionType compType)
        {
            using var fs = File.OpenWrite(path);
            using var writer = new AssetsFileWriter(fs);
            bundleInst.file.Pack(bundleInst.file.Reader, writer, compType);
        }
    }
}
