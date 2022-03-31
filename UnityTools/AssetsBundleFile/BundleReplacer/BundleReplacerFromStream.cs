using System.IO;

namespace UnityTools
{
    public class BundleReplacerFromStream : BundleReplacer
    {
        private readonly string oldName;
        private readonly string newName;
        private readonly bool hasSerializedData;
        private readonly Stream stream;
        private readonly long offset;
        private readonly long size;
        private readonly int bundleListIndex;

        public BundleReplacerFromStream(string oldName, string newName, bool hasSerializedData, Stream stream, long offset, long size, int bundleListIndex = -1)
        {
            this.oldName = oldName;
            this.newName = newName ?? oldName;
            this.hasSerializedData = hasSerializedData;
            this.stream = stream;
            this.offset = offset;
            this.size = size == -1 ? stream.Length : size;
            this.bundleListIndex = bundleListIndex;
        }

        public override BundleReplacementType GetReplacementType() => BundleReplacementType.AddOrModify;

        public override int GetBundleListIndex() => bundleListIndex;

        public override string GetOriginalEntryName() => oldName;

        public override string GetEntryName() => newName;

        public override bool HasSerializedData() => hasSerializedData;

        public override long GetSize() => size;

        public override bool Init(AssetsFileReader entryReader, long entryPos, long entrySize, ClassDatabaseFile typeMeta = null)
        {
            return true;
        }
        
        public override void Uninit()
        {
            return;
        }

        public override long Write(AssetsFileWriter writer)
        {
            stream.Position = offset;
            stream.CopyTo(writer.BaseStream, (int)size);
            return writer.Position;
        }

        public override long WriteReplacer(AssetsFileWriter writer)
        {
            writer.Write((short)2); //replacer type
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
