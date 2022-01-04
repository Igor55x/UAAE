using System.Collections.Generic;

namespace UnityTools
{
    public class ValueBuilder
    {
        public static AssetTypeValueField DefaultValueFieldFromArrayTemplate(AssetTypeValueField arrayField)
        {
            return DefaultValueFieldFromArrayTemplate(arrayField.TemplateField);
        }

        public static AssetTypeValueField DefaultValueFieldFromArrayTemplate(AssetTypeTemplateField arrayField)
        {
            if (!arrayField.isArray)
                return null;

            var templateField = arrayField.children[1];
            return DefaultValueFieldFromTemplate(templateField);
        }

        public static AssetTypeValueField DefaultValueFieldFromTemplate(AssetTypeTemplateField templateField)
        {
            var templateChildren = templateField.children;
            List<AssetTypeValueField> valueChildren;
            if (templateField.isArray ||
                templateField.valueType is EnumValueTypes.String)
            {
                valueChildren = new List<AssetTypeValueField>();
            }
            else
            {
                valueChildren = new List<AssetTypeValueField>();
                for (var i = 0; i < templateChildren.Count; i++)
                {
                    valueChildren.Add(DefaultValueFieldFromTemplate(templateChildren[i]));
                }
            }

            var defaultValue = DefaultValueFromTemplate(templateField);

            var root = new AssetTypeValueField
            {
                Children = valueChildren,
                ChildrenCount = valueChildren.Count,
                TemplateField = templateField,
                Value = defaultValue
            };
            return root;
        }

        public static AssetTypeValue DefaultValueFromTemplate(AssetTypeTemplateField templateField)
        {
            object obj = templateField.valueType switch
            {
                EnumValueTypes.Int8 => 0,
                EnumValueTypes.UInt8 => 0,
                EnumValueTypes.Bool => false,
                EnumValueTypes.Int16 => 0,
                EnumValueTypes.UInt16 => 0,
                EnumValueTypes.Int32 => 0,
                EnumValueTypes.UInt32 => 0u,
                EnumValueTypes.Int64 => 0L,
                EnumValueTypes.UInt64 => 0uL,
                EnumValueTypes.Float => 0f,
                EnumValueTypes.Double => 0d,
                EnumValueTypes.String => string.Empty,
                EnumValueTypes.Array => new AssetTypeArray(),
                EnumValueTypes.ByteArray => new AssetTypeByteArray(),
                _ => null
            };

            if (obj != null || !templateField.isArray)
                return new AssetTypeValue(templateField.valueType, obj);

            //arrays don't usually have their type set,
            //so we have to check .isArray instead
            obj = new AssetTypeArray();
            return new AssetTypeValue(EnumValueTypes.Array, obj);
        }
    }
}
