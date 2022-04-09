namespace UnityTools
{
    public class BundleRenamer : BundleReplacer
    {
        private readonly string oldName;
        private readonly string newName;
        private readonly bool hasSerializedData;
        private readonly int bundleListIndex;
        public BundleRenamer(string oldName, string newName, bool hasSerializedData, int bundleListIndex = -1)
        {
            this.oldName = oldName;
            this.newName = newName ?? oldName;
            this.hasSerializedData = hasSerializedData;
            this.bundleListIndex = bundleListIndex;
        }

        public override BundleReplacementType GetReplacementType() => BundleReplacementType.Rename;

        public override int GetBundleListIndex() => bundleListIndex;

        public override string GetOriginalEntryName() => oldName;

        public override string GetEntryName() => newName;

        public override bool HasSerializedData() => hasSerializedData;

        public override long GetSize() => 0;

        public override bool Init(EndianReader entryReader, long entryPos, long entrySize, ClassDatabaseFile typeMeta = null) => true;

        public override void Uninit()
        {
            return;
        }

        public override long Write(EndianWriter writer) => writer.Position;

        public override long WriteReplacer(EndianWriter writer)
        {
            writer.Write((short)1); //replacer type
            writer.Write((byte)0); //file type (0 bundle, 1 assets)
            writer.WriteCountStringInt16(oldName);
            writer.WriteCountStringInt16(newName);
            writer.Write(hasSerializedData);
            return writer.Position;
        }
    }
}
