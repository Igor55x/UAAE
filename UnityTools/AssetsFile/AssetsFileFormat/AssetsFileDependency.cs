namespace UnityTools
{
    public class AssetsFileDependency
    {
        public string bufferedPath;
        public GUID128 guid;
        public int type;
        public string assetPath;
        public string originalAssetPath;
        public void Read(EndianReader reader)
        {
            bufferedPath = reader.ReadNullTerminated();
            guid = new GUID128();
            guid.Read(reader);
            type = reader.ReadInt32();
            assetPath = reader.ReadNullTerminated();
            originalAssetPath = assetPath;

            if (assetPath.EndsWith("unity_builtin_extra") ||
                assetPath.EndsWith("unity default resources") ||
                assetPath.EndsWith("unity editor resources"))
            {
                assetPath = $"Resources/{assetPath}";
            }
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteNullTerminated(bufferedPath);
            guid.Write(writer);
            writer.Write(type);
            var assetPathTemp = assetPath;

            if (assetPath.EndsWith("unity_builtin_extra") ||
                assetPath.EndsWith("unity default resources") ||
                assetPath.EndsWith("unity editor resources")
                && originalAssetPath != string.Empty)
            {
                assetPathTemp = originalAssetPath;
            }
            writer.WriteNullTerminated(assetPathTemp);
        }
    }
}
