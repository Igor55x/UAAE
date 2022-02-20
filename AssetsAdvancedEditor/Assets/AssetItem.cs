using System.Windows.Forms;
using UnityTools;

namespace AssetsAdvancedEditor.Assets
{
    public class AssetItem : ListViewItem
    {
        public AssetContainer Cont { get; set; }
        public new string Name { get; set; }
        public string ListName { get; set; }
        public string Container { get; set; }
        public string Type { get; set; }
        public AssetClassID TypeID { get; set; }
        public int FileID { get; set; }
        public long PathID { get; set; }
        public long Size { get; set; }
        public string Modified { get; set; }
        public new long Position { get; set; }
        public ushort MonoID { get; set; }

        public void SetSubItems()
        {
            Text = ListName;
            SubItems.AddRange(new[]
            {
                Container,
                Type,
                ((int)TypeID).ToString(),
                FileID.ToString(),
                PathID.ToString(),
                Size.ToString(),
                Modified
            });
        }

        public void ClearModified()
        {
            SubItems[7].Text = "";
            Modified = "";
        }
    }
}