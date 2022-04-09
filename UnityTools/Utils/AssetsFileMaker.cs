using System.Collections.Generic;
using System.IO;

namespace UnityTools.Utils
{
    public class AssetsFileMaker
    {
		public static byte[] CreateBlankAssets(string engineVersion, List<Type_0D> types)
		{
            using var ms = new MemoryStream();
            using var writer = new EndianWriter(ms, true);
            var header = new AssetsFileHeader
            {
                MetadataSize = 0,
                FileSize = 0x1000,
                Version = 0x11,
                DataOffset = 0x1000,
                Endianness = false,
                Reserved = new byte[3]
            };

            var typeTree = new TypeTree()
            {
                unityVersion = engineVersion,
                version = 0x5,
                hasTypeTree = true,
                fieldCount = types.Count,
                unity5Types = types
            };

            header.Write(writer);
            writer.BigEndian = false;
            typeTree.Write(writer, 0x11);
            typeTree.Write(writer, 0x11);
            writer.Write((uint)0);
            writer.Align();
            //preload table and dependencies
            writer.Write((uint)0);
            writer.Write((uint)0);

            //due to a write bug in at.net we have to pad to 0x1000
            while (ms.Position < 0x1000)
            {
                writer.Write((byte)0);
            }

            return ms.ToArray();
        }
	}
}
