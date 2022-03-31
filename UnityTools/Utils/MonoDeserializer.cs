using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityTools
{
    public class MonoDeserializer
    {
        public UnityVersion unityVersion;
        public int childrenCount;
        public List<AssetTypeTemplateField> children;
        private static readonly Dictionary<string, AssemblyDefinition> loadedAssemblies = new ();
        public void Read(string typeName, AssemblyDefinition assembly, UnityVersion unityVersion)
        {
            this.unityVersion = unityVersion;
            children = new List<AssetTypeTemplateField>();
            RecursiveTypeLoad(assembly.MainModule, typeName, children);
            childrenCount = children.Count;
        }

        public void Read(string typeName, string assemblyPath, UnityVersion unityVersion)
        {
            var asmDef = GetAssemblyWithDependencies(assemblyPath);
            Read(typeName, asmDef, unityVersion);
        }

        public static AssemblyDefinition GetAssemblyWithDependencies(string path)
        {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(path));
            var readerParameters = new ReaderParameters()
            {
                AssemblyResolver = resolver
            };
            return AssemblyDefinition.ReadAssembly(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read), readerParameters);
        }

        public static AssetTypeValueField GetMonoBaseField(AssetsManager am, AssetsFileInstance inst, AssetFileInfoEx info, string managedPath, bool cached = true)
        {
            var file = inst.file;
            var baseField = new AssetTypeTemplateField();
            baseField.FromClassDatabase(am.classFile, AssetHelper.FindAssetClassByID(am.classFile, info.curFileType));
            var mainAti = new AssetTypeInstance(baseField, file.reader, info.absoluteFilePos);
            var scriptIndex = AssetHelper.GetScriptIndex(file, info);
            if (scriptIndex != 0xFFFF)
            {
                var scriptAti = am.GetExtAsset(inst, mainAti.GetBaseField().Get("m_Script")).instance;
                var scriptName = scriptAti.GetBaseField().Get("m_ClassName").GetValue().AsString();
                var scriptNamespace = scriptAti.GetBaseField().Get("m_Namespace").GetValue().AsString();
                var assemblyName = scriptAti.GetBaseField().Get("m_AssemblyName").GetValue().AsString();
                var assemblyPath = Path.Combine(managedPath, assemblyName);

                if (scriptNamespace != string.Empty)
                    scriptName = scriptNamespace + "." + scriptName;

                if (File.Exists(assemblyPath))
                {
                    AssemblyDefinition asmDef;
                    if (cached)
                    {
                        if (!loadedAssemblies.ContainsKey(assemblyName))
                        {
                            loadedAssemblies.Add(assemblyName, GetAssemblyWithDependencies(assemblyPath));
                        }
                        asmDef = loadedAssemblies[assemblyName];
                    }
                    else
                    {
                        asmDef = GetAssemblyWithDependencies(assemblyPath);
                    }

                    var mc = new MonoDeserializer();
                    mc.Read(scriptName, asmDef, new UnityVersion(inst.file.typeTree.unityVersion));
                    var monoTemplateFields = mc.children;

                    baseField.AddChildren(monoTemplateFields);

                    mainAti = new AssetTypeInstance(baseField, file.reader, info.absoluteFilePos);
                }
            }
            return mainAti.GetBaseField();
        }

        private void RecursiveTypeLoad(ModuleDefinition module, string typeName, List<AssetTypeTemplateField> templates)
        {
            foreach (var type in module.GetTypes())
            {
                if (type.FullName.Equals(typeName))
                {
                    RecursiveTypeLoad(type, templates);
                    break;
                }
            }
        }

        private void RecursiveTypeLoad(TypeDefinition type, List<AssetTypeTemplateField> templates)
        {
            var baseName = type.BaseType.FullName;
            if (baseName != "System.Object" &&
                baseName != "UnityEngine.Object" &&
                baseName != "UnityEngine.MonoBehaviour" &&
                baseName != "UnityEngine.ScriptableObject")
            {
                var typeDef = type.BaseType.Resolve();
                RecursiveTypeLoad(typeDef, templates);
            }

            foreach (var template in ReadTypes(type))
            {
                templates.Add(template);
            }
        }

        private List<AssetTypeTemplateField> ReadTypes(TypeDefinition type)
        {
            var acceptableFields = GetAcceptableFields(type);
            var localChildren = new List<AssetTypeTemplateField>();
            for (var i = 0; i < acceptableFields.Count; i++)
            {
                var field = new AssetTypeTemplateField();
                var fieldDef = acceptableFields[i];
                var fieldTypeRef = fieldDef.FieldType;
                var fieldType = fieldTypeRef.Resolve();
                var fieldTypeName = fieldType.Name;
                var isArrayOrList = false;

                if (fieldTypeRef.MetadataType == MetadataType.Array)
                {
                    var arrType = (ArrayType)fieldTypeRef;
                    isArrayOrList = arrType.IsVector;
                }
                else if (fieldType.FullName == "System.Collections.Generic.List`1")
                {
                    fieldType = ((GenericInstanceType)fieldDef.FieldType).GenericArguments[0].Resolve();
                    fieldTypeName = fieldType.Name;
                    isArrayOrList = true;
                }

                field.name = fieldDef.Name;
                field.type = ConvertBaseToPrimitive(fieldTypeName);
                if (IsPrimitiveType(fieldType))
                {
                    field.childrenCount = 0;
                    field.children = new List<AssetTypeTemplateField>();
                }
                else if (fieldType.Name.Equals("String"))
                {
                    SetString(field);
                }
                else if (IsSpecialUnityType(fieldType))
                {
                    SetSpecialUnity(field, fieldType);
                }
                else if (DerivesFromUEObject(fieldType))
                {
                    SetPPtr(field, true);
                }
                else if (fieldType.IsSerializable)
                {
                    SetSerialized(field, fieldType);
                }

                if (fieldType.IsEnum)
                {
                    field.valueType = EnumValueTypes.Int32;
                }
                else
                {
                    field.valueType = AssetTypeValueField.GetValueTypeByTypeName(field.type);
                }
                field.align = TypeAligns(field.valueType);
                field.hasValue = field.valueType != EnumValueTypes.None;

                if (isArrayOrList)
                {
                    field = SetArray(field);
                }
                localChildren.Add(field);
            }
            return localChildren;
        }

        private List<FieldDefinition> GetAcceptableFields(TypeDefinition typeDef)
        {
            var validFields = new List<FieldDefinition>();
            foreach (var f in typeDef.Fields)
            {
                if (HasFlag(f.Attributes, FieldAttributes.Public) ||
                    f.CustomAttributes.Any(a => a.AttributeType.Name.Equals("SerializeField"))) //field is public or has exception attribute
                {
                    if (!HasFlag(f.Attributes, FieldAttributes.Static) &&
                        !HasFlag(f.Attributes, FieldAttributes.NotSerialized) &&
                        !f.IsInitOnly &&
                        !f.HasConstant) //field is not public, has exception attribute, readonly, or const
                    {
                        var ft = f.FieldType;
                        if (f.FieldType.IsArray)
                        {
                            ft = ft.GetElementType();
                        }
                        var ftd = ft.Resolve();
                        if (ftd != null)
                        {
                            if (ftd.IsPrimitive ||
                                ftd.IsEnum ||
                                ftd.IsSerializable ||
                                DerivesFromUEObject(ftd) ||
                                IsSpecialUnityType(ftd)) //field has a serializable type
                            {
                                validFields.Add(f);
                            }
                        }
                    }
                }
            }
            return validFields;
        }

        private readonly Dictionary<string, string> baseToPrimitive = new ()
        {
            {"Boolean","bool"},
            {"Int64","long"},
            {"Int16","short"},
            {"UInt64","ulong"},
            {"UInt32","uint"},
            {"UInt16","ushort"},
            {"Char","char"},
            {"Byte","byte"},
            {"SByte","sbyte"},
            {"Double","double"},
            {"Single","float"},
            {"Int32","int"},
            {"String","string"}
        };

        private string ConvertBaseToPrimitive(string name)
        {
            if (baseToPrimitive.ContainsKey(name))
            {
                return baseToPrimitive[name];
            }
            return name;
        }

        private bool IsPrimitiveType(TypeDefinition typeDef)
        {
            var name = typeDef.FullName;
            if (typeDef.IsEnum ||
                name == "System.Boolean" ||
                name == "System.Int64" ||
                name == "System.Int16" ||
                name == "System.UInt64" ||
                name == "System.UInt32" ||
                name == "System.UInt16" ||
                name == "System.Char" ||
                name == "System.Byte" ||
                name == "System.SByte" ||
                name == "System.Double" ||
                name == "System.Single" ||
                name == "System.Int32") return true;
            return false;
        }

        private bool IsSpecialUnityType(TypeDefinition typeDef)
        {
            var name = typeDef.FullName;
            if (name == "UnityEngine.Color" ||
                name == "UnityEngine.Color32" ||
                name == "UnityEngine.Gradient" ||
                name == "UnityEngine.Vector2" ||
                name == "UnityEngine.Vector3" ||
                name == "UnityEngine.Vector4" ||
                name == "UnityEngine.LayerMask" ||
                name == "UnityEngine.Quaternion" ||
                name == "UnityEngine.Bounds" ||
                name == "UnityEngine.Rect" ||
                name == "UnityEngine.Matrix4x4" ||
                name == "UnityEngine.AnimationCurve" ||
                name == "UnityEngine.GUIStyle" ||
                name == "UnityEngine.Vector2Int" ||
                name == "UnityEngine.Vector3Int" ||
                name == "UnityEngine.BoundsInt") return true;
            return false;
        }

        private bool DerivesFromUEObject(TypeDefinition typeDef)
        {
            if (typeDef.IsInterface)
                return false;
            if (typeDef.BaseType.FullName == "UnityEngine.Object" ||
                typeDef.FullName == "UnityEngine.Object")
                return true;
            if (typeDef.BaseType.FullName != "System.Object")
                return DerivesFromUEObject(typeDef.BaseType.Resolve());
            return false;
        }

        private bool TypeAligns(EnumValueTypes valueType)
        {
            if (valueType.Equals(EnumValueTypes.Bool) ||
                valueType.Equals(EnumValueTypes.Int8) ||
                valueType.Equals(EnumValueTypes.UInt8) ||
                valueType.Equals(EnumValueTypes.Int16) ||
                valueType.Equals(EnumValueTypes.UInt16))
                return true;
            return false;
        }

        private AssetTypeTemplateField SetArray(AssetTypeTemplateField field)
        {
            var size = new AssetTypeTemplateField
            {
                name = "size",
                type = "int",
                valueType = EnumValueTypes.Int32,
                isArray = false,
                align = false,
                hasValue = true,
                childrenCount = 0,
                children = new List<AssetTypeTemplateField>()
            };

            var data = new AssetTypeTemplateField
            {
                name = new string(field.name),
                type = new string(field.type),
                valueType = field.valueType,
                isArray = false,
                align = false,//IsAlignable(field.valueType);
                hasValue = field.hasValue,
                childrenCount = field.childrenCount,
                children = field.children
            };

            var array = new AssetTypeTemplateField
            {
                name = new string(field.name),
                type = "Array",
                valueType = EnumValueTypes.Array,
                isArray = true,
                align = true,
                hasValue = false,
                childrenCount = 2,
                children = new List<AssetTypeTemplateField>()
                {
                    size, data
                }
            };

            return array;
        }

        private void SetString(AssetTypeTemplateField field)
        {
            field.childrenCount = 1;

            var size = new AssetTypeTemplateField
            {
                name = "size",
                type = "int",
                valueType = EnumValueTypes.Int32,
                isArray = false,
                align = false,
                hasValue = true,
                childrenCount = 0,
                children = new List<AssetTypeTemplateField>()
            };

            var data = new AssetTypeTemplateField
            {
                name = "data",
                type = "char",
                valueType = EnumValueTypes.UInt8,
                isArray = false,
                align = false,
                hasValue = true,
                childrenCount = 0,
                children = new List<AssetTypeTemplateField>()
            };

            var array = new AssetTypeTemplateField
            {
                name = "Array",
                type = "Array",
                valueType = EnumValueTypes.Array,
                isArray = true,
                align = true,
                hasValue = false,
                childrenCount = 2,
                children = new List<AssetTypeTemplateField>()
                {
                    size, data
                }
            };

            field.children = new List<AssetTypeTemplateField>()
            {
                array
            };
        }

        private void SetPPtr(AssetTypeTemplateField field, bool dollar)
        {
            if (dollar)
                field.type = $"PPtr<${field.type}>";
            else
                field.type = $"PPtr<{field.type}>";

            field.childrenCount = 2;

            var fileID = new AssetTypeTemplateField
            {
                name = "m_FileID",
                type = "int",
                valueType = EnumValueTypes.Int32,
                isArray = false,
                align = false,
                hasValue = true,
                childrenCount = 0,
                children = new List<AssetTypeTemplateField>()
            };

            var pathID = new AssetTypeTemplateField
            {
                name = "m_PathID"
            };
            if (unityVersion.Major >= 5)
            {
                pathID.type = "SInt64";
                pathID.valueType = EnumValueTypes.Int64;
            }
            else
            {
                pathID.type = "int";
                pathID.valueType = EnumValueTypes.Int32;
            }
            pathID.isArray = false;
            pathID.align = false;
            pathID.hasValue = true;
            pathID.childrenCount = 0;
            pathID.children = new List<AssetTypeTemplateField>();
            field.children = new List<AssetTypeTemplateField>()
            {
                fileID, pathID
            };
        }

        private void SetSerialized(AssetTypeTemplateField field, TypeDefinition type)
        {
            var types = new List<AssetTypeTemplateField>();
            RecursiveTypeLoad(type, types);
            field.childrenCount = types.Count;
            field.children = types.ToList();
        }

        #region Special unity serialization
        private void SetSpecialUnity(AssetTypeTemplateField field, TypeDefinition type)
        {
            switch (type.Name)
            {
                case "Gradient":
                    SetGradient(field);
                    break;
                case "AnimationCurve":
                    SetAnimationCurve(field);
                    break;
                case "LayerMask":
                    SetBitField(field);
                    break;
                case "Bounds":
                    SetAABB(field);
                    break;
                case "Rect":
                    SetRectf(field);
                    break;
                case "Color32":
                    SetGradientRGBAb(field);
                    break;
                case "GUIStyle":
                    SetGUIStyle(field);
                    break;
                case "BoundsInt":
                    SetAABBInt(field);
                    break;
                case "Vector2Int":
                    SetVec2Int(field);
                    break;
                case "Vector3Int":
                    SetVec3Int(field);
                    break;
                default:
                    SetSerialized(field, type);
                    break;
            }
        }

        private void SetGradient(AssetTypeTemplateField field)
        {
            AssetTypeTemplateField key0, key1, key2, key3, key4, key5, key6, key7;
            if (unityVersion.Major > 5 || (unityVersion.Major == 5 && unityVersion.Minor >= 6))
            {
                key0 = CreateTemplateField("key0", "ColorRGBA", EnumValueTypes.None, 4, RGBAf());
                key1 = CreateTemplateField("key1", "ColorRGBA", EnumValueTypes.None, 4, RGBAf());
                key2 = CreateTemplateField("key2", "ColorRGBA", EnumValueTypes.None, 4, RGBAf());
                key3 = CreateTemplateField("key3", "ColorRGBA", EnumValueTypes.None, 4, RGBAf());
                key4 = CreateTemplateField("key4", "ColorRGBA", EnumValueTypes.None, 4, RGBAf());
                key5 = CreateTemplateField("key5", "ColorRGBA", EnumValueTypes.None, 4, RGBAf());
                key6 = CreateTemplateField("key6", "ColorRGBA", EnumValueTypes.None, 4, RGBAf());
                key7 = CreateTemplateField("key7", "ColorRGBA", EnumValueTypes.None, 4, RGBAf());
            }
            else
            {
                key0 = CreateTemplateField("key0", "ColorRGBA", EnumValueTypes.None, 1, RGBA32());
                key1 = CreateTemplateField("key1", "ColorRGBA", EnumValueTypes.None, 1, RGBA32());
                key2 = CreateTemplateField("key2", "ColorRGBA", EnumValueTypes.None, 1, RGBA32());
                key3 = CreateTemplateField("key3", "ColorRGBA", EnumValueTypes.None, 1, RGBA32());
                key4 = CreateTemplateField("key4", "ColorRGBA", EnumValueTypes.None, 1, RGBA32());
                key5 = CreateTemplateField("key5", "ColorRGBA", EnumValueTypes.None, 1, RGBA32());
                key6 = CreateTemplateField("key6", "ColorRGBA", EnumValueTypes.None, 1, RGBA32());
                key7 = CreateTemplateField("key7", "ColorRGBA", EnumValueTypes.None, 1, RGBA32());
            }
            var ctime0 = CreateTemplateField("ctime0", "UInt16", EnumValueTypes.UInt16);
            var ctime1 = CreateTemplateField("ctime1", "UInt16", EnumValueTypes.UInt16);
            var ctime2 = CreateTemplateField("ctime2", "UInt16", EnumValueTypes.UInt16);
            var ctime3 = CreateTemplateField("ctime3", "UInt16", EnumValueTypes.UInt16);
            var ctime4 = CreateTemplateField("ctime4", "UInt16", EnumValueTypes.UInt16);
            var ctime5 = CreateTemplateField("ctime5", "UInt16", EnumValueTypes.UInt16);
            var ctime6 = CreateTemplateField("ctime6", "UInt16", EnumValueTypes.UInt16);
            var ctime7 = CreateTemplateField("ctime7", "UInt16", EnumValueTypes.UInt16);
            var atime0 = CreateTemplateField("atime0", "UInt16", EnumValueTypes.UInt16);
            var atime1 = CreateTemplateField("atime1", "UInt16", EnumValueTypes.UInt16);
            var atime2 = CreateTemplateField("atime2", "UInt16", EnumValueTypes.UInt16);
            var atime3 = CreateTemplateField("atime3", "UInt16", EnumValueTypes.UInt16);
            var atime4 = CreateTemplateField("atime4", "UInt16", EnumValueTypes.UInt16);
            var atime5 = CreateTemplateField("atime5", "UInt16", EnumValueTypes.UInt16);
            var atime6 = CreateTemplateField("atime6", "UInt16", EnumValueTypes.UInt16);
            var atime7 = CreateTemplateField("atime7", "UInt16", EnumValueTypes.UInt16);
            var m_Mode = CreateTemplateField("m_Mode", "int", EnumValueTypes.Int32);
            var m_NumColorKeys = CreateTemplateField("m_NumColorKeys", "UInt8", EnumValueTypes.UInt8);
            var m_NumAlphaKeys = CreateTemplateField("m_NumAlphaKeys", "UInt8", EnumValueTypes.UInt8, false, true);
            if (unityVersion.Major > 5 || (unityVersion.Major == 5 && unityVersion.Minor >= 5))
            {
                field.childrenCount = 27;
                field.children = new List<AssetTypeTemplateField>()
                {
                    key0, key1, key2, key3, key4, key5, key6, key7, ctime0, ctime1, ctime2, ctime3, ctime4, ctime5, ctime6, ctime7, atime0, atime1, atime2, atime3, atime4, atime5, atime6, atime7, m_Mode, m_NumColorKeys, m_NumAlphaKeys
                };
            }
            else
            {
                field.childrenCount = 26;
                field.children = new List<AssetTypeTemplateField>()
                {
                    key0, key1, key2, key3, key4, key5, key6, key7, ctime0, ctime1, ctime2, ctime3, ctime4, ctime5, ctime6, ctime7, atime0, atime1, atime2, atime3, atime4, atime5, atime6, atime7, m_NumColorKeys, m_NumAlphaKeys
                };
            }
        }

        private List<AssetTypeTemplateField> RGBAf()
        {
            var r = CreateTemplateField("r", "float", EnumValueTypes.Float);
            var g = CreateTemplateField("g", "float", EnumValueTypes.Float);
            var b = CreateTemplateField("b", "float", EnumValueTypes.Float);
            var a = CreateTemplateField("a", "float", EnumValueTypes.Float);
            return new List<AssetTypeTemplateField>() { r, g, b, a };
        }

        private List<AssetTypeTemplateField> RGBA32()
        {
            var rgba = CreateTemplateField("rgba", "unsigned int", EnumValueTypes.UInt32);
            return new List<AssetTypeTemplateField>() { rgba };
        }

        private void SetAnimationCurve(AssetTypeTemplateField field)
        {
            field.childrenCount = 4;
            var time = CreateTemplateField("time", "float", EnumValueTypes.Float);
            var value = CreateTemplateField("value", "float", EnumValueTypes.Float);
            var inSlope = CreateTemplateField("inSlope", "float", EnumValueTypes.Float);
            var outSlope = CreateTemplateField("outSlope", "float", EnumValueTypes.Float);
            //new in 2019
            var weightedMode = CreateTemplateField("weightedMode", "int", EnumValueTypes.Int32);
            var inWeight = CreateTemplateField("inWeight", "float", EnumValueTypes.Float);
            var outWeight = CreateTemplateField("outWeight", "float", EnumValueTypes.Float);
            /////////////
            var size = CreateTemplateField("size", "int", EnumValueTypes.Int32);
            AssetTypeTemplateField data;
            if (unityVersion.Major >= 2018)
            {
                data = CreateTemplateField("data", "Keyframe", EnumValueTypes.None, 7, new List<AssetTypeTemplateField>()
                {
                    time, value, inSlope, outSlope, weightedMode, inWeight, outWeight
                });
            }
            else
            {
                data = CreateTemplateField("data", "Keyframe", EnumValueTypes.None, 4, new List<AssetTypeTemplateField>()
                {
                    time, value, inSlope, outSlope
                });
            }
            var Array = CreateTemplateField("Array", "Array", EnumValueTypes.Array, true, false, 2, new List<AssetTypeTemplateField>()
            {
                size, data
            });
            var m_Curve = CreateTemplateField("m_Curve", "vector", EnumValueTypes.None, 1, new List<AssetTypeTemplateField>()
            {
                Array
            });
            var m_PreInfinity = CreateTemplateField("m_PreInfinity", "int", EnumValueTypes.Int32);
            var m_PostInfinity = CreateTemplateField("m_PostInfinity", "int", EnumValueTypes.Int32);
            var m_RotationOrder = CreateTemplateField("m_RotationOrder", "int", EnumValueTypes.Int32);
            field.children = new List<AssetTypeTemplateField>()
            {
                m_Curve, m_PreInfinity, m_PostInfinity, m_RotationOrder
            };
        }

        private void SetBitField(AssetTypeTemplateField field)
        {
            field.childrenCount = 1;
            var m_Bits = CreateTemplateField("m_Bits", "unsigned int", EnumValueTypes.UInt32);
            field.children = new List<AssetTypeTemplateField>()
            {
                m_Bits
            };
        }

        private void SetAABB(AssetTypeTemplateField field)
        {
            field.childrenCount = 2;
            var m_Center = CreateTemplateField("m_Center", "Vector3f", EnumValueTypes.None, 3, Vec3f());
            var m_Extent = CreateTemplateField("m_Extent", "Vector3f", EnumValueTypes.None, 3, Vec3f());
            field.children = new List<AssetTypeTemplateField>()
            {
                m_Center, m_Extent
            };
        }

        private List<AssetTypeTemplateField> Vec3f()
        {
            var x = CreateTemplateField("x", "float", EnumValueTypes.Float);
            var y = CreateTemplateField("y", "float", EnumValueTypes.Float);
            var z = CreateTemplateField("z", "float", EnumValueTypes.Float);
            return new List<AssetTypeTemplateField>() { x, y, z };
        }

        private void SetRectf(AssetTypeTemplateField field)
        {
            field.childrenCount = 4;
            var x = CreateTemplateField("x", "float", EnumValueTypes.Float);
            var y = CreateTemplateField("y", "float", EnumValueTypes.Float);
            var width = CreateTemplateField("width", "float", EnumValueTypes.Float);
            var height = CreateTemplateField("height", "float", EnumValueTypes.Float);
            field.children = new List<AssetTypeTemplateField>()
            {
                x, y, width, height
            };
        }

        private void SetGradientRGBAb(AssetTypeTemplateField field)
        {
            field.childrenCount = 1;
            var rgba = CreateTemplateField("rgba", "unsigned int", EnumValueTypes.UInt32);
            field.children = new List<AssetTypeTemplateField>()
            {
                rgba
            };
        }
        //only supports 2019 right now
        private void SetGUIStyle(AssetTypeTemplateField field)
        {
            field.childrenCount = 26;
            var m_Name = CreateTemplateField("m_Name", "string", EnumValueTypes.String, 1, String());
            var m_Normal = CreateTemplateField("m_Normal", "GUIStyleState", EnumValueTypes.None, 2, GUIStyleState());
            var m_Hover = CreateTemplateField("m_Hover", "GUIStyleState", EnumValueTypes.None, 2, GUIStyleState());
            var m_Active = CreateTemplateField("m_Active", "GUIStyleState", EnumValueTypes.None, 2, GUIStyleState());
            var m_Focused = CreateTemplateField("m_Focused", "GUIStyleState", EnumValueTypes.None, 2, GUIStyleState());
            var m_OnNormal = CreateTemplateField("m_OnNormal", "GUIStyleState", EnumValueTypes.None, 2, GUIStyleState());
            var m_OnHover = CreateTemplateField("m_OnHover", "GUIStyleState", EnumValueTypes.None, 2, GUIStyleState());
            var m_OnActive = CreateTemplateField("m_OnActive", "GUIStyleState", EnumValueTypes.None, 2, GUIStyleState());
            var m_OnFocused = CreateTemplateField("m_OnFocused", "GUIStyleState", EnumValueTypes.None, 2, GUIStyleState());
            var m_Border = CreateTemplateField("m_Border", "RectOffset", EnumValueTypes.None, 4, RectOffset());
            var m_Margin = CreateTemplateField("m_Margin", "RectOffset", EnumValueTypes.None, 4, RectOffset());
            var m_Padding = CreateTemplateField("m_Padding", "RectOffset", EnumValueTypes.None, 4, RectOffset());
            var m_Overflow = CreateTemplateField("m_Overflow", "RectOffset", EnumValueTypes.None, 4, RectOffset());
            var m_Font = CreateTemplateField("m_Font", "PPtr<Font>", EnumValueTypes.None, 2, PPtr());
            var m_FontSize = CreateTemplateField("m_FontSize", "int", EnumValueTypes.Int32);
            var m_FontStyle = CreateTemplateField("m_FontStyle", "int", EnumValueTypes.Int32);
            var m_Alignment = CreateTemplateField("m_Alignment", "int", EnumValueTypes.Int32);
            var m_WordWrap = CreateTemplateField("m_WordWrap", "bool", EnumValueTypes.Bool);
            var m_RichText = CreateTemplateField("m_RichText", "bool", EnumValueTypes.Bool, false, true);
            var m_TextClipping = CreateTemplateField("m_TextClipping", "int", EnumValueTypes.Int32);
            var m_ImagePosition = CreateTemplateField("m_ImagePosition", "int", EnumValueTypes.Int32);
            var m_ContentOffset = CreateTemplateField("m_ContentOffset", "Vector2f", EnumValueTypes.None, 2, Vec2f());
            var m_FixedWidth = CreateTemplateField("m_FixedWidth", "float", EnumValueTypes.Float);
            var m_FixedHeight = CreateTemplateField("m_FixedHeight", "float", EnumValueTypes.Float);
            var m_StretchWidth = CreateTemplateField("m_StretchWidth", "bool", EnumValueTypes.Bool);
            var m_StretchHeight = CreateTemplateField("m_StretchHeight", "bool", EnumValueTypes.Bool, false, true);
            field.children = new List<AssetTypeTemplateField>()
            {
                m_Name, m_Normal, m_Hover, m_Active, m_Focused, m_OnNormal, m_OnHover, m_OnActive, m_OnFocused, m_Border, m_Margin, m_Padding, m_Overflow, m_Font, m_FontSize, m_FontStyle, m_Alignment, m_WordWrap, m_RichText, m_TextClipping, m_ImagePosition, m_ContentOffset, m_FixedWidth, m_FixedHeight, m_StretchWidth, m_StretchHeight
            };
        }

        private void SetAABBInt(AssetTypeTemplateField field)
        {
            field.childrenCount = 2;
            var m_Center = CreateTemplateField("m_Center", "Vector3Int", EnumValueTypes.None, 3, Vec3Int());
            var m_Extent = CreateTemplateField("m_Extent", "Vector3Int", EnumValueTypes.None, 3, Vec3Int());
            field.children = new List<AssetTypeTemplateField>()
            {
                m_Center, m_Extent
            };
        }

        private List<AssetTypeTemplateField> Vec3Int()
        {
            var m_X = CreateTemplateField("m_X", "int", EnumValueTypes.Int32);
            var m_Y = CreateTemplateField("m_Y", "int", EnumValueTypes.Int32);
            var m_Z = CreateTemplateField("m_Z", "int", EnumValueTypes.Int32);
            return new List<AssetTypeTemplateField>() { m_X, m_Y, m_Z };
        }

        private void SetVec2Int(AssetTypeTemplateField field)
        {
            field.childrenCount = 2;
            var m_X = CreateTemplateField("m_X", "int", EnumValueTypes.Int32);
            var m_Y = CreateTemplateField("m_Y", "int", EnumValueTypes.Int32);
            field.children = new List<AssetTypeTemplateField>()
            {
                m_X, m_Y
            };
        }

        private void SetVec3Int(AssetTypeTemplateField field)
        {
            field.childrenCount = 3;
            var m_X = CreateTemplateField("m_X", "int", EnumValueTypes.Int32);
            var m_Y = CreateTemplateField("m_Y", "int", EnumValueTypes.Int32);
            var m_Z = CreateTemplateField("m_Z", "int", EnumValueTypes.Int32);
            field.children = new List<AssetTypeTemplateField>()
            {
                m_X, m_Y, m_Z
            };
        }

        private List<AssetTypeTemplateField> String()
        {
            var size = CreateTemplateField("size", "int", EnumValueTypes.Int32);
            var data = CreateTemplateField("char", "data", EnumValueTypes.UInt8);
            var Array = CreateTemplateField("Array", "Array", EnumValueTypes.Array, true, true, 2, new List<AssetTypeTemplateField>()
            {
                size, data
            });
            return new List<AssetTypeTemplateField>() { Array };
        }

        private List<AssetTypeTemplateField> GUIStyleState()
        {
            var m_Background = CreateTemplateField("m_Background", "PPtr<Texture2D>", EnumValueTypes.None, 2, PPtr());
            var m_TextColor = CreateTemplateField("m_TextColor", "ColorRGBA", EnumValueTypes.None, 4, RGBAf());
            return new List<AssetTypeTemplateField>() { m_Background, m_TextColor };
        }

        private List<AssetTypeTemplateField> RectOffset()
        {
            var m_Left = CreateTemplateField("m_Left", "int", EnumValueTypes.Int32);
            var m_Right = CreateTemplateField("m_Right", "int", EnumValueTypes.Int32);
            var m_Top = CreateTemplateField("m_Top", "int", EnumValueTypes.Int32);
            var m_Bottom = CreateTemplateField("m_Bottom", "int", EnumValueTypes.Int32);
            return new List<AssetTypeTemplateField>() { m_Left, m_Right, m_Top, m_Bottom };
        }

        private List<AssetTypeTemplateField> PPtr()
        {
            var m_FileID = CreateTemplateField("m_FileID", "int", EnumValueTypes.Int32);
            var m_PathID = CreateTemplateField("m_PathID", "SInt64", EnumValueTypes.Int64);
            return new List<AssetTypeTemplateField>() { m_FileID, m_PathID };
        }

        private List<AssetTypeTemplateField> Vec2f()
        {
            var x = CreateTemplateField("x", "float", EnumValueTypes.Float);
            var y = CreateTemplateField("y", "float", EnumValueTypes.Float);
            return new List<AssetTypeTemplateField>() { x, y };
        }

        private AssetTypeTemplateField CreateTemplateField(string name, string type, EnumValueTypes valueType)
        {
            return CreateTemplateField(name, type, valueType, false, false, 0, null);
        }

        private AssetTypeTemplateField CreateTemplateField(string name, string type, EnumValueTypes valueType, bool isArray, bool align)
        {
            return CreateTemplateField(name, type, valueType, isArray, align, 0, null);
        }

        private AssetTypeTemplateField CreateTemplateField(string name, string type, EnumValueTypes valueType, int childrenCount, List<AssetTypeTemplateField> children)
        {
            return CreateTemplateField(name, type, valueType, false, false, childrenCount, children);
        }

        private AssetTypeTemplateField CreateTemplateField(string name, string type, EnumValueTypes valueType, bool isArray, bool align, int childrenCount, List<AssetTypeTemplateField> children)
        {
            var field = new AssetTypeTemplateField
            {
                name = name,
                type = type,
                valueType = valueType,
                isArray = isArray,
                align = align,
                hasValue = valueType != EnumValueTypes.None,
                childrenCount = childrenCount,
                children = children
            };
            return field;
        }
        #endregion

        #region .net polyfill
        //https://stackoverflow.com/a/4108907
        private static bool HasFlag(Enum variable, Enum value)
        {
            if (variable == null)
                return false;

            if (value == null)
                throw new ArgumentNullException("value");

            if (!Enum.IsDefined(variable.GetType(), value))
            {
                throw new ArgumentException(string.Format(
                    "Enumeration type mismatch. The flag is of type '{0}', was expecting '{1}'.",
                    value.GetType(), variable.GetType()));
            }

            var num = Convert.ToUInt64(value);
            return (Convert.ToUInt64(variable) & num) == num;
        }
        #endregion
    }
}
