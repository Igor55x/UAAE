using System.Collections.Generic;
using UnityTools.Utils;

namespace UnityTools
{
    public class BundleReplacerFromAssets : BundleReplacer
    {
        private readonly string oldName;
        private readonly string newName;
        private readonly List<AssetsReplacer> replacers;
        private readonly uint fileId;
        private readonly int bundleListIndex;
        private AssetsFile assetsFile;
        private ClassDatabaseFile typeMeta;

        public BundleReplacerFromAssets(string oldName, string newName, AssetsFile assetsFile, List<AssetsReplacer> replacers, uint fileId, int bundleListIndex = -1)
        {
            this.oldName = oldName;
            this.newName = newName ?? oldName;
            this.assetsFile = assetsFile;
            this.replacers = replacers;
            this.fileId = fileId;
            this.bundleListIndex = bundleListIndex;
        }

        public override BundleReplacementType GetReplacementType() => BundleReplacementType.AddOrModify;

        public override int GetBundleListIndex() => bundleListIndex;

        public override string GetOriginalEntryName() => oldName;

        public override string GetEntryName() => newName;

        public override bool HasSerializedData() => true;

        public override long GetSize() => -1; //todo

        public override bool Init(AssetsFileReader entryReader, long entryPos, long entrySize, ClassDatabaseFile typeMeta = null)
        {
            if (assetsFile != null)
                return false;

            this.typeMeta = typeMeta;
            var stream = new SegmentStream(entryReader.BaseStream, entryPos, entrySize);
            var reader = new AssetsFileReader(stream);
            assetsFile = new AssetsFile(reader);
            return true;
        }

        public override void Uninit() => assetsFile.Close();

        public override long Write(AssetsFileWriter writer)
        {
            // Some parts of an assets file need to be aligned to a multiple of 4/8/16 bytes,
            // but for this to work correctly, the start of the file of course needs to be aligned too.
            // In a loose .assets file this is true by default, but inside a bundle file,
            // this might not be the case. Therefore wrap the bundle output stream in a SegmentStream
            // which will make it look like the start of the new assets file is at position 0
            var alignedStream = new SegmentStream(writer.BaseStream, writer.Position);
            var alignedWriter = new AssetsFileWriter(alignedStream);
            assetsFile.Write(alignedWriter, replacers, -1, typeMeta);
            writer.Position = writer.BaseStream.Length;
            return writer.Position;
        }

        public override long WriteReplacer(AssetsFileWriter writer)
        {
            writer.Write((short)4); //replacer type
            writer.Write((byte)0); //file type (0 bundle, 1 assets)
            writer.WriteCountStringInt16(oldName);
            writer.WriteCountStringInt16(newName);
            writer.Write(HasSerializedData());
            writer.Write((long)replacers.Count);
            foreach (var replacer in replacers)
            {
                replacer.WriteReplacer(writer);
            }

            return writer.Position;
        }
    }
}
