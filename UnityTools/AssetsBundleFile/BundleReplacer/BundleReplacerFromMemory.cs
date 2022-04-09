namespace UnityTools
{
    public class BundleReplacerFromMemory : BundleReplacer
    {
        private readonly string oldName;
        private readonly string newName;
        private readonly bool hasSerializedData;
        private readonly byte[] buffer;
        private readonly long size;
        private readonly int bundleListIndex;

        public BundleReplacerFromMemory(string oldName, string newName, bool hasSerializedData, byte[] buffer, long size, int bundleListIndex = -1)
        {
            this.oldName = oldName;
            this.newName = newName ?? oldName;
            this.hasSerializedData = hasSerializedData;
            this.buffer = buffer;
            this.size = size == -1 ? buffer.Length : size;
            this.bundleListIndex = bundleListIndex;
        }

        public override BundleReplacementType GetReplacementType() => BundleReplacementType.AddOrModify;

        public override int GetBundleListIndex() => bundleListIndex;

        public override string GetOriginalEntryName() => oldName;

        public override string GetEntryName() => newName;

        public override bool HasSerializedData() => hasSerializedData;

        public override long GetSize() => size;

        public override bool Init(EndianReader entryReader, long entryPos, long entrySize, ClassDatabaseFile typeMeta = null)
        {
            return true;
        }

        public override void Uninit()
        {
            return;
        }

        public override long Write(EndianWriter writer)
        {
            writer.Write(buffer);
            return writer.Position;
        }

        public override long WriteReplacer(EndianWriter writer)
        {
            writer.Write((short)3); //replacer type
            writer.Write((byte)0); //file type (0 bundle, 1 assets)
            writer.WriteCountStringInt16(oldName);
            writer.WriteCountStringInt16(newName);
            writer.Write(hasSerializedData);
            writer.Write(GetSize());
            Write(writer);

            return writer.Position;
        }
    }
}
