using Serenity.ComponentModel;
using Serenity.Data;
using System;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetWidth(IPropertySource source, PropertyItem item)
        {
            var widthAttr = source.GetAttribute<WidthAttribute>();
            var basedOnField = source.BasedOnField;
            item.Width = widthAttr == null ? (!ReferenceEquals(null, basedOnField) ? AutoWidth(basedOnField) : 80) : widthAttr.Value;
            if (widthAttr != null && (widthAttr.Min != 0))
                item.MinWidth = widthAttr.Min;

            if (widthAttr != null && (widthAttr.Max != 0))
                item.MaxWidth = widthAttr.Max;

            var labelWidthAttr = source.GetAttribute<LabelWidthAttribute>();
            if (labelWidthAttr != null)
                item.LabelWidth = labelWidthAttr.Value;
        }

        private static int AutoWidth(Field field)
        {
            var name = field.Name;

            switch (field.Type)
            {
                case FieldType.String:
                    if (field.Size != 0 && field.Size <= 25)
                        return Math.Max(field.Size * 6, 150);
                    else if (field.Size == 0)
                        return 250;
                    else
                        return 150;
                case FieldType.Boolean:
                    return 40;
                case FieldType.DateTime:
                    return 85;
                case FieldType.Time:
                    return 70;
                case FieldType.Int16:
                    return 55;
                case FieldType.Int32:
                    return 65;
                case FieldType.Single:
                case FieldType.Double:
                case FieldType.Decimal:
                    return 85;
                default:
                    return 80;
            }
        }
    }
}