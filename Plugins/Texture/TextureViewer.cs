using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityTools;

namespace Plugins.Texture
{
    public partial class TextureViewer : Form
    {
        private Bitmap image;
        private bool loaded;
        private float x, y;
        private int width, height;
        private int lx, ly;
        private int mx, my;
        private float sc;
        private bool mouseDown;

        public TextureViewer(TextureFile tex, byte[] texData)
        {
            InitializeComponent();

            if (texData != null && texData.Length > 0)
            {
                var formatName = ((TextureFormat)tex.m_TextureFormat).ToString().Replace("_", " ");
                Text = $"Texture Viewer [{formatName}]";

                image = new Bitmap(tex.m_Width, tex.m_Height, PixelFormat.Format32bppArgb);

                var rect = new Rectangle(0, 0, image.Width, image.Height);
                var picData = image.LockBits(rect, ImageLockMode.ReadWrite, image.PixelFormat);
                picData.Stride = tex.m_Width * 4;
                Marshal.Copy(texData, 0, picData.Scan0, texData.Length);

                image.UnlockBits(picData);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                width = image.Width;
                height = image.Height;
                sc = 1f;
                mouseDown = false;

                DoubleBuffered = true;

                var workingArea = Screen.PrimaryScreen.WorkingArea;
                var clientDiffWidth = Size.Width - ClientSize.Width;
                var clientDiffHeight = Size.Height - ClientSize.Height;
                ClientSize = new Size(Math.Min(width, workingArea.Width - clientDiffWidth), Math.Min(height, workingArea.Height - clientDiffHeight));

                loaded = true;
            }
        }

        private void TextureViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (loaded && image != null)
            {
                image.Dispose();
                loaded = false;
            }
        }

        private void TextureViewer_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            lblUnsupported.Visible = !loaded;
            if (loaded)
            {
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                var origTransform = graphics.Transform;
                graphics.ScaleTransform(sc, sc);
                graphics.TranslateTransform(x, y);
                graphics.DrawImage(image, 0, 0);
                // For the resizey thing on the bottom right (for some reason is affected by this)
                graphics.Transform = origTransform;
            }
        }

        private void TextureViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = false;
            }
        }

        private void TextureViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                lx = e.X;
                ly = e.Y;
            }
        }

        private void TextureViewer_MouseMove(object sender, MouseEventArgs e)
        {
            mx = e.X;
            my = e.Y;
            if (mouseDown)
            {
                x += (e.X - lx) / sc;
                y += (e.Y - ly) / sc;
                lx = e.X;
                ly = e.Y;
                Refresh();
            }
        }

        private void TextureViewer_MouseWheel(object sender, MouseEventArgs e)
        {
            var oldSc = sc;
            sc *= 1 + (float)e.Delta / 1200;

            var oldImageX = mx / oldSc;
            var oldImageY = my / oldSc;

            var newImageX = mx / sc;
            var newImageY = my / sc;

            x = newImageX - oldImageX + x;
            y = newImageY - oldImageY + y;

            Refresh();
        }
    }
}
