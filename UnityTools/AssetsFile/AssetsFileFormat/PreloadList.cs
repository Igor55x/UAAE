using System.Collections.Generic;

namespace UnityTools
{
    public class PreloadList
    {
        public int len;
        public List<AssetPPtr> items;

        public void Read(EndianReader reader)
        {
            len = reader.ReadInt32();
            items = new List<AssetPPtr>();
            for (var i = 0; i < len; i++)
            {
                var pptr = new AssetPPtr();
                pptr.Read(reader);
                items.Add(pptr);
            }
        }

        public void Write(EndianWriter writer)
        {
            writer.Write(len);
            for (var i = 0; i < len; i++)
            {
                items[i].Write(writer);
            }
        }
    }
}
