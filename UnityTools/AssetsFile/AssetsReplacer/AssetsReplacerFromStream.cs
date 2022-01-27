using System.Collections.Generic;
using System.IO;

namespace UnityTools
{
    public class AssetsReplacerFromStream : AssetsReplacer
    {
        private readonly int fileId;
        private readonly long pathId;
        private readonly AssetClassID classId;
        private readonly Stream stream;
        private readonly long offset;
        private readonly long size;
        private ushort monoScriptIndex;
        private Hash128 propertiesHash;
        private Hash128 scriptIdHash;
        private ClassDatabaseFile file;
        private ClassDatabaseType type;
        private List<AssetPPtr> preloadList;

        public AssetsReplacerFromStream(int fileId, long pathId, AssetClassID classId, ushort monoScriptIndex, Stream stream, long offset, long size)
        {
            this.fileId = fileId;
            this.pathId = pathId;
            this.classId = classId;
            this.monoScriptIndex = monoScriptIndex;
            this.stream = stream;
            this.offset = offset;
            this.size = size;
            this.preloadList = new List<AssetPPtr>();
        }
        public override AssetsReplacementType GetReplacementType() => AssetsReplacementType.AddOrModify;

        public override int GetFileID() => fileId;

        public override long GetPathID() => pathId;

        public override AssetClassID GetClassID() => classId;

        public override ushort GetMonoScriptID() => monoScriptIndex;

        public override void SetMonoScriptID(ushort scriptId) => monoScriptIndex = scriptId;

        public override long GetSize() => size;

        public override bool GetPropertiesHash(out Hash128 propertiesHash)
        {
            propertiesHash = this.propertiesHash;
            return true;
        }

        public override bool SetPropertiesHash(Hash128 propertiesHash)
        {
            this.propertiesHash = propertiesHash;
            return true;
        }

        public override bool GetScriptIDHash(out Hash128 scriptIdHash)
        {
            scriptIdHash = this.scriptIdHash;
            return true;
        }

        public override bool SetScriptIDHash(Hash128 scriptIdHash)
        {
            this.scriptIdHash = scriptIdHash;
            return true;
        }

        public override bool GetTypeInfo(out ClassDatabaseFile file, out ClassDatabaseType type)
        {
            file = this.file;
            type = this.type;
            return true;
        }

        public override bool SetTypeInfo(ClassDatabaseFile file, ClassDatabaseType type, bool localCopy)
        {
            this.file = file;
            this.type = type;
            return true;
        }

        public override bool GetPreloadDependencies(out List<AssetPPtr> preloadList)
        {
            preloadList = new List<AssetPPtr>(this.preloadList);
            return true;
        }

        public override bool SetPreloadDependencies(List<AssetPPtr> preloadList)
        {
            this.preloadList = preloadList;
            return true;
        }

        public override bool AddPreloadDependency(AssetPPtr dependency)
        {
            preloadList.Add(dependency);
            return true;
        }

        public override long Write(AssetsFileWriter writer)
        {
            stream.Position = offset;
            stream.CopyToCompat(writer.BaseStream, size);
            return writer.Position;
        }

        public override long WriteReplacer(AssetsFileWriter writer)
        {
            writer.Write((short)2); //replacer type
            writer.Write((byte)1); //file type (0 bundle, 1 assets)
            writer.Write((byte)1); //idk, always 1
            writer.Write(0); //always 0 even when fileid is something else
            writer.Write(GetPathID());
            writer.Write((int)GetClassID());
            writer.Write(GetMonoScriptID());

            writer.Write(preloadList.Count);
            foreach (var PPtr in preloadList)
            {
                writer.Write(PPtr.fileID);
                writer.Write(PPtr.pathID);
            }

            //flag1, unknown
            writer.Write((byte)0);
            //flag2
            if (propertiesHash.Data != null)
            {
                writer.Write((byte)1);
                writer.Write(propertiesHash.Data);
            }
            else
            {
                writer.Write((byte)0);
            }
            //flag3
            if (scriptIdHash.Data != null)
            {
                writer.Write((byte)1);
                writer.Write(scriptIdHash.Data);
            }
            else
            {
                writer.Write((byte)0);
            }
            //flag4
            if (file != null)
            {
                writer.Write((byte)1);
                file.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }

            writer.Write(GetSize());
            Write(writer);

            return writer.Position;
        }

        public override void Dispose()
        {
            stream.Dispose();
        }
    }
}
