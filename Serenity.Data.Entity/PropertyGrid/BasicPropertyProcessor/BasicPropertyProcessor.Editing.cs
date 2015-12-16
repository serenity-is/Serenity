using Serenity.ComponentModel;
using Serenity.Data;
using System;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetEditing(IPropertySource source, PropertyItem item)
        {
            var editorTypeAttr = source.GetAttribute<EditorTypeAttribute>();

            if (editorTypeAttr == null)
            {
                item.EditorType = AutoDetermineEditorType(source.ValueType, source.EnumType);
            }
            else
            {
                item.EditorType = editorTypeAttr.EditorType;
                editorTypeAttr.SetParams(item.EditorParams);
            }

            if (source.EnumType != null)
                item.EditorParams["enumKey"] = EnumMapper.GetEnumTypeKey(source.EnumType);

            if (!ReferenceEquals(null, source.BasedOnField))
            {
                if (item.EditorType == "Decimal" &&
                    (source.BasedOnField is DoubleField ||
                        source.BasedOnField is DecimalField) &&
                        source.BasedOnField.Size > 0 &&
                        source.BasedOnField.Scale < source.BasedOnField.Size)
                {
                    string minVal = new String('0', source.BasedOnField.Size - source.BasedOnField.Scale);
                    if (source.BasedOnField.Scale > 0)
                        minVal += "." + new String('0', source.BasedOnField.Scale);
                    string maxVal = minVal.Replace('0', '9');
                    item.EditorParams["minValue"] = minVal;
                    item.EditorParams["maxValue"] = maxVal;
                }
                else if (source.BasedOnField.Size > 0)
                {
                    item.EditorParams["maxLength"] = source.BasedOnField.Size;
                    item.MaxLength = source.BasedOnField.Size;
                }
            }

            var maxLengthAttr = source.GetAttribute<MaxLengthAttribute>();
            if (maxLengthAttr != null)
            {
                item.MaxLength = maxLengthAttr.MaxLength;
                item.EditorParams["maxLength"] = maxLengthAttr.MaxLength;
            }

            foreach (EditorOptionAttribute param in source.GetAttributes<EditorOptionAttribute>())
            {
                var key = param.Key;
                if (key != null &&
                    key.Length >= 1)
                    key = key.Substring(0, 1).ToLowerInvariant() + key.Substring(1);

                item.EditorParams[key] = param.Value;
            }
        }

        private static string AutoDetermineEditorType(Type valueType, Type enumType)
        {
            if (enumType != null)
                return "Enum";
            else if (valueType == typeof(string))
                return "String";
            else if (valueType == typeof(Int32) || valueType == typeof(Int16))
                return "Integer";
            else if (valueType == typeof(DateTime))
                return "Date";
            else if (valueType == typeof(Boolean))
                return "Boolean";
            else if (valueType == typeof(Decimal) || valueType == typeof(Double) || valueType == typeof(Single))
                return "Decimal";
            else
                return "String";
        }
    }
}