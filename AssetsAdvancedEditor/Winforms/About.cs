using System.Diagnostics;
using System.Windows.Forms;

namespace AssetsAdvancedEditor.Winforms
{
    public partial class About : Form
    {
        public About() => InitializeComponent();

        private void FirstLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink("https://github.com/Igor55x");
        }

        private void SecondLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink("https://community.7daystodie.com/profile/418-derpopo");
        }

        private static void OpenLink(string url)
        {
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = true,
                    FileName = url
                }
            };
            process.Start();
        }
    }
}