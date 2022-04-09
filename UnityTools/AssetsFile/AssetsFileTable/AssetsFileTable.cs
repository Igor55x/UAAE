using System.Collections.Generic;
using System.IO;

namespace UnityTools
{
    public class AssetsFileTable
    {
        public AssetsFile File;
        public EndianReader Reader;
        public Stream ReaderPar;

        public uint InfoCount;
        public AssetFileInfoEx[] Info;

        private Dictionary<long, int> LookupBase;
        
        public AssetsFileTable(AssetsFile file)
        {
            File = file;
            Reader = file.reader;
            ReaderPar = file.readerPar;
            Reader.BigEndian = file.header.Endianness;
            Reader.Position = file.AssetTablePos;
            InfoCount = file.AssetCount;
            Info = new AssetFileInfoEx[InfoCount];
            for (var i = 0; i < InfoCount; i++)
            {
                var assetFileInfoSet = new AssetFileInfoEx();
                assetFileInfoSet.Read(file.header.Version, Reader);
                assetFileInfoSet.absoluteFilePos = file.header.DataOffset + assetFileInfoSet.curFileOffset;
                if (file.header.Version < 0x10)
                {
                    if (assetFileInfoSet.curFileTypeOrIndex < 0)
                    {
                        assetFileInfoSet.curFileType = AssetClassID.MonoBehaviour;
                    }
                    else
                    {
                        assetFileInfoSet.curFileType = (AssetClassID)assetFileInfoSet.curFileTypeOrIndex;
                    }
                }
                else
                {
                    assetFileInfoSet.curFileType = file.typeTree.unity5Types[assetFileInfoSet.curFileTypeOrIndex].ClassID;
                }
                Info[i] = assetFileInfoSet;
            }
        }
        
        public AssetFileInfoEx GetAssetInfo(long pathId)
        {
            if (LookupBase != null)
            {
                if (LookupBase.ContainsKey(pathId))
                {
                    return Info[LookupBase[pathId]];
                }
            }
            else
            {
                foreach (var info in Info)
                {
                    if (info.index == pathId)
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        public void GenerateQuickLookupTree()
        {
            LookupBase = new Dictionary<long, int>();
            for (var i = 0; i < Info.Length; i++)
            {
                var info = Info[i];
                LookupBase[info.index] = i;
            }
        }
    }
}
