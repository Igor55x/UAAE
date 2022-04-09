namespace UnityTools
{
    public class BundleRemover : BundleReplacer
    {
        private readonly string name;
        private readonly bool hasSerializedData;
        private readonly int bundleListIndex;
        public BundleRemover(string name, bool hasSerializedData, int bundleListIndex = -1)
        {
            this.name = name;
            this.hasSerializedData = hasSerializedData;
            this.bundleListIndex = bundleListIndex;
        }

        public override BundleReplacementType GetReplacementType() => BundleReplacementType.Remove;

        public override int GetBundleListIndex() => bundleListIndex;

        public override string GetOriginalEntryName() => name;

        public override string GetEntryName() => name;

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
            writer.Write((short)0); //replacer type
            writer.Write((byte)0); //file type (0 bundle, 1 assets)
            writer.WriteCountStringInt16(name);
            return writer.Position;
        }
    }
}
