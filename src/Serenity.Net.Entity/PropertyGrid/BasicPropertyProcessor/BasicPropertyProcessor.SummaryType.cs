using System.IO;

namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetSummaryType(IPropertySource source, PropertyItem item)
    {
        var summaryTypeAttr = source.GetAttribute<SummaryTypeAttribute>();

        if (summaryTypeAttr == null)
        {
            if (source.GetAttribute<PrimaryKeyAttribute>() != null ||
                source.GetAttribute<IdentityAttribute>() != null ||
                source.GetAttribute<ForeignKeyAttribute>() != null ||
                source.GetAttribute<LeftJoinAttribute>() != null)
            {
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
                item.SummaryType = SummaryType.Sum;
                return;
            }

            if (valueType != typeof(string) &&
                valueType != typeof(bool) &&
                valueType != typeof(Stream) &&
                valueType != typeof(Guid) &&
                valueType != typeof(DateTime) &&
                valueType != typeof(TimeSpan))
            {
                item.SummaryType = SummaryType.None;
                return;
            }
        }
        else if (summaryTypeAttr.Value != SummaryType.Disabled)
            item.SummaryType = summaryTypeAttr.Value;
    }
}