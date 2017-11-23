using Serenity.ComponentModel;
using Serenity.Data;
using System;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private static void SetFormatting(IPropertySource source, PropertyItem item)
        {
            var formatterTypeAttr = source.GetAttribute<FormatterTypeAttribute>();
            var enumType = source.EnumType;
            var valueType = source.ValueType;
            var basedOnField = source.BasedOnField;

            if (formatterTypeAttr == null)
            {
                if (enumType != null)
                {
                    item.FormatterType = "Enum";
                    item.FormatterParams["enumKey"] = EnumMapper.GetEnumTypeKey(enumType);
                }
                else if (valueType == typeof(DateTime) || valueType == typeof(DateTime?))
                {
                    if (!ReferenceEquals(null, basedOnField) && 
                        basedOnField is DateTimeField &&
                        !((DateTimeField)basedOnField).DateOnly)
                        item.FormatterType = "DateTime";
                    else
                        item.FormatterType = "Date";
                }
                else if (valueType == typeof(Boolean))
                    item.FormatterType = "Checkbox";
                else if (valueType == typeof(Decimal) ||
                    valueType == typeof(Double) ||
                    valueType == typeof(Single) ||
                    valueType == typeof(Int32))
                {
                    item.FormatterType = "Number";
                }
            }
            else
            {
                item.FormatterType = formatterTypeAttr.FormatterType;
                formatterTypeAttr.SetParams(item.FormatterParams);
            }

            var displayFormatAttr = source.GetAttribute<DisplayFormatAttribute>();
            if (displayFormatAttr != null)
            {
                item.DisplayFormat = displayFormatAttr.Value;
                item.FormatterParams["displayFormat"] = displayFormatAttr.Value;
            }

            foreach (FormatterOptionAttribute param in source.GetAttributes<FormatterOptionAttribute>())
            {
                var key = param.Key;
                if (key != null &&
                    key.Length >= 1)
                    key = key.Substring(0, 1).ToLowerInvariant() + key.Substring(1);

                item.FormatterParams[key] = param.Value;
            }
        } 
    }
}