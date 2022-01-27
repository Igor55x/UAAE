using System.Text;

namespace UnityTools
{
    public class Type_0D
    {
        public AssetClassID ClassID;

        public bool IsStrippedType;
        public ushort ScriptIndex;
        public Hash128 ScriptHash;

        public Hash128 TypeHash;
        public uint ChildrenCount;
        public TypeField_0D[] Children;

        public uint stringTableLen;
        public string stringTable;

        public int DependencyCount;
        public int[] Dependencies;

        public void Read(bool hasTypeTree, AssetsFileReader reader, uint version)
        {
            ClassID = (AssetClassID)reader.ReadInt32();
            if (version >= 0x10)
            {
                IsStrippedType = reader.ReadBoolean();
            }

            ScriptIndex = version >= 0x11 ? reader.ReadUInt16() : (ushort)0xffff;

            if (version < 0x11 && ClassID < 0 || version >= 0x11 && ClassID is AssetClassID.MonoBehaviour)
            {
                ScriptHash = new Hash128(reader);
            }
            TypeHash = new Hash128(reader);

            if (!hasTypeTree)
                return;

            ChildrenCount = reader.ReadUInt32();
            stringTableLen = reader.ReadUInt32();
            Children = new TypeField_0D[ChildrenCount];
            for (var i = 0; i < ChildrenCount; i++)
            {
                var typeField_0D = new TypeField_0D();
                typeField_0D.Read(reader, version);
                Children[i] = typeField_0D;
            }
            stringTable = Encoding.UTF8.GetString(reader.ReadBytes((int)stringTableLen));
            if (version >= 0x15)
            {
                DependencyCount = reader.ReadInt32();
                Dependencies = new int[DependencyCount];
                for (var i = 0; i < DependencyCount; i++)
                {
                    Dependencies[i] = reader.ReadInt32();
                }
            }
        }

        public void Write(bool hasTypeTree, AssetsFileWriter writer, uint version)
        {
            writer.Write((int)ClassID);
            if (version >= 0x10)
            {
                writer.Write(IsStrippedType);
            }

            if (version >= 0x11)
            {
                writer.Write(ScriptIndex);
            }

            if (version < 0x11 && ClassID < 0 || version >= 0x11 && ClassID is AssetClassID.MonoBehaviour)
            {
                ScriptHash.Write(writer);
            }
            TypeHash.Write(writer);

            if (!hasTypeTree)
                return;

            writer.Write(ChildrenCount);
            writer.Write(stringTable.Length);
            for (var i = 0; i < ChildrenCount; i++)
            {
                Children[i].Write(writer, version);
            }
            writer.Write(stringTable);
            if (version >= 0x15)
            {
                writer.Write(DependencyCount);
                for (var i = 0; i < DependencyCount; i++)
                {
                    writer.Write(Dependencies[i]);
                }
            }
        }

        public static readonly string strTable = "AABB\0AnimationClip\0AnimationCurve\0AnimationState\0Array\0Base\0BitField\0bitset\0bool\0char\0ColorRGBA\0Component\0data\0deque\0double\0dynamic_array\0FastPropertyName\0first\0float\0Font\0GameObject\0Generic Mono\0GradientNEW\0GUID\0GUIStyle\0int\0list\0long long\0map\0Matrix4x4f\0MdFour\0MonoBehaviour\0MonoScript\0m_ByteSize\0m_Curve\0m_EditorClassIdentifier\0m_EditorHideFlags\0m_Enabled\0m_ExtensionPtr\0m_GameObject\0m_Index\0m_IsArray\0m_IsStatic\0m_MetaFlag\0m_Name\0m_ObjectHideFlags\0m_PrefabInternal\0m_PrefabParentObject\0m_Script\0m_StaticEditorFlags\0m_Type\0m_Version\0Object\0pair\0PPtr<Component>\0PPtr<GameObject>\0PPtr<Material>\0PPtr<MonoBehaviour>\0PPtr<MonoScript>\0PPtr<Object>\0PPtr<Prefab>\0PPtr<Sprite>\0PPtr<TextAsset>\0PPtr<Texture>\0PPtr<Texture2D>\0PPtr<Transform>\0Prefab\0Quaternionf\0Rectf\0RectInt\0RectOffset\0second\0set\0short\0size\0SInt16\0SInt32\0SInt64\0SInt8\0staticvector\0string\0TextAsset\0TextMesh\0Texture\0Texture2D\0Transform\0TypelessData\0UInt16\0UInt32\0UInt64\0UInt8\0unsigned int\0unsigned long long\0unsigned short\0vector\0Vector2f\0Vector3f\0Vector4f\0m_ScriptingClassIdentifier\0Gradient\0Type*\0int2_storage\0int3_storage\0BoundsInt\0m_CorrespondingSourceObject\0m_PrefabInstance\0m_PrefabAsset\0FileSize\0Hash128\0";
    }
}
