using System.Collections.Generic;
using System.IO;

namespace UnityTools
{
    public class AssetTypeInstance
    {
        public int BaseFieldCount;
        public List<AssetTypeValueField> BaseFields { get; }

        public AssetTypeInstance(AssetTypeTemplateField baseField, AssetsFileReader reader, long filePos)
            : this(new[] { baseField }, reader, filePos) { }

        public AssetTypeInstance(AssetTypeTemplateField[] baseFields, AssetsFileReader reader, long filePos)
        {
            reader.BigEndian = false;
            reader.Position = filePos;
            BaseFieldCount = baseFields.Length;
            BaseFields = new List<AssetTypeValueField>(BaseFieldCount);
            for (var i = 0; i < BaseFieldCount; i++)
            {
                var valueField = baseFields[i].MakeValue(reader);
                BaseFields.Add(valueField);
            }
        }

        public static AssetTypeValueField GetDummyAssetTypeField()
        {
            var valueField = new AssetTypeValueField
            {
                ChildrenCount = -1
            };
            return valueField;
        }

        public AssetTypeValueField GetBaseField(int index = 0)
        {
            if (index < 0 || index >= BaseFieldCount)
                return GetDummyAssetTypeField();
            return BaseFields[index];
        }

        public bool SetChildList(int index, List<AssetTypeValueField> children)
        {
            if (index < 0 || index >= BaseFieldCount)
                return false;
            var valueField = BaseFields[index];
            valueField.SetChildrenList(children);
            return true;
        }

        public void Write(AssetsFileWriter writer)
        {
            foreach (var baseField in BaseFields)
            {
                baseField.Write(writer);
            }
        }

        public byte[] WriteToByteArray(bool bigEndian = false)
        {
            using var ms = new MemoryStream();
            using var writer = new AssetsFileWriter(ms)
            {
                BigEndian = bigEndian
            };
            Write(writer);
            return ms.ToArray();
        }
    }
}
