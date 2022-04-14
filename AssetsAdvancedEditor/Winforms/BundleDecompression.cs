using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using AssetsAdvancedEditor.Utils;
using UnityTools;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class BundleDecompression : Form
    {
        public BundleFileInstance BundleInst;
        public BackgroundWorker Bw;
        public BundleDecompression(BundleFileInstance bundleInst)
        {
            InitializeComponent();
            BundleInst = bundleInst;
        }

        private enum AssetBundleDecompressionType
        {
            File,
            Memory
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (BundleInst == null) return;
            AssetBundleDecompressionType decompType;
            if (rbtnFile.Checked)
                decompType = AssetBundleDecompressionType.File;
            else if (rbtnMemory.Checked)
                decompType = AssetBundleDecompressionType.Memory;
            else
            {
                MsgBoxUtils.ShowErrorDialog("You didn't choose any decompression type!\n" +
                                            "Please go back and select it.\n");
                DialogResult = DialogResult.None;
                return;
            }
            try
            {
                Bw = new BackgroundWorker();
                if (BundleInst.file.IsBundleDataCompressed())
                {
                    switch ((int)decompType)
                    {
                        // Decompress to File
                        case 0:
                        {
                            var sfd = new SaveFileDialog
                            {
                                FileName = BundleInst.name + ".unpacked",
                                Filter = @"All types (*.*)|*.*"
                            };
                            if (sfd.ShowDialog() != DialogResult.OK) return;
                            Bw.DoWork += delegate { DecompressToFile(sfd.FileName); };
                            break;
                        }
                        // Decompress to Memory
                        case 1:
                            Bw.DoWork += delegate { DecompressToMemory(); };
                            break;
                    }
                }
                else
                {
                    Bw.DoWork += delegate { DecompressToMemory(); };
                }
                Bw.RunWorkerCompleted += delegate
                {
                    MsgBoxUtils.ShowInfoDialog("The bundle file has been successfully unpacked!", MessageBoxButtons.OK);
                };
                Bw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MsgBoxUtils.ShowErrorDialog("Something went wrong when unpacking the bundle:\n" + ex);
                DialogResult = DialogResult.Abort;
            }
        }

        private void DecompressToFile(string savePath)
        {
            var bundleStream = File.Open(savePath, FileMode.Create);
            BundleInst.file.Unpack(BundleInst.file.Reader, new EndianWriter(bundleStream, true));

            bundleStream.Position = 0;

            var newBundle = new AssetBundleFile();
            newBundle.Read(new EndianReader(bundleStream, true));

            BundleInst.file.Reader.Close();
            BundleInst.file = newBundle;
        }

        private void DecompressToMemory()
        {
            var bundleStream = new MemoryStream();
            BundleInst.file.Unpack(BundleInst.file.Reader, new EndianWriter(bundleStream, true));

            bundleStream.Position = 0;

            var newBundle = new AssetBundleFile();
            newBundle.Read(new EndianReader(bundleStream, true));

            BundleInst.file.Reader.Close();
            BundleInst.file = newBundle;
        }
    }
}