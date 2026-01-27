namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetSummaryType(IPropertySource source, PropertyItem item)
    {
        var summaryTypeAttr = source.GetAttribute<SummaryTypeAttribute>();

        if (summaryTypeAttr != null)
        {
            item.SummaryType = summaryTypeAttr.Value;
            return;
        }

        if (source.GetAttribute<PrimaryKeyAttribute>() != null ||
            source.GetAttribute<IdentityAttribute>() != null ||
            source.GetAttribute<ForeignKeyAttribute>() != null ||
            source.GetAttribute<LeftJoinAttribute>() != null ||
            source.GetAttribute<UnboundAttribute>() != null)
        {
            item.SummaryType = SummaryType.Disabled;
            return;
        }

        var valueType = source.ValueType;
        if (valueType == typeof(decimal) ||
            valueType == typeof(double) ||
            valueType == typeof(float) ||
            valueType == typeof(long) ||
            valueType == typeof(int) ||
            valueType == typeof(short))
        {
            item.SummaryType = options.DefaultSummaryType ?? SummaryType.Sum;
            return;
        }

        item.SummaryType = SummaryType.Disabled;
    }
}