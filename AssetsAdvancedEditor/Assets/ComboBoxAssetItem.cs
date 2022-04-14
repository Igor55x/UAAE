namespace AssetsAdvancedEditor.Assets
{
    public class ComboBoxAssetItem
    {
        public string DisplayName { get; set; }
        public string OriginalName { get; set; }
        public int Index { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
