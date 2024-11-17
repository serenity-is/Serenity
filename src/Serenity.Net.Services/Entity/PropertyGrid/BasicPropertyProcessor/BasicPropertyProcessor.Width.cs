namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private LabelWidthAttribute labelWidthPrior;

    private void SetWidth(IPropertySource source, PropertyItem item)
    {
        var widthAttr = source.GetAttribute<WidthAttribute>();
        var basedOnField = source.BasedOnField;

        item.Width = widthAttr == null || widthAttr.Value == 0 ?
            (basedOnField is not null ? AutoWidth(basedOnField) : 80) : widthAttr.Value;

        if (widthAttr != null && widthAttr.Value != 0)
            item.WidthSet = true;

        if (widthAttr != null && (widthAttr.Min != 0))
            item.MinWidth = widthAttr.Min;

        if (widthAttr != null && (widthAttr.Max != 0))
            item.MaxWidth = widthAttr.Max;

        var labelWidthAttr = source.GetAttribute<LabelWidthAttribute>() ?? labelWidthPrior;
        if (labelWidthAttr != null)
        {
            item.LabelWidth = labelWidthAttr.Value;

            if (!labelWidthAttr.JustThis)
                labelWidthPrior = labelWidthAttr.UntilNext ? labelWidthAttr : null;
        }
    }

    private static int AutoWidth(Field field)
    {
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