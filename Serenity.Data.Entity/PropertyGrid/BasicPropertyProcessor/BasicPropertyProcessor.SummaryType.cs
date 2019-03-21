using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.IO;

namespace Serenity.PropertyGrid
{
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
                if (valueType == typeof(Decimal) ||
                    valueType == typeof(Double) ||
                    valueType == typeof(Single) ||
                    valueType == typeof(Int64) ||
                    valueType == typeof(Int32) ||
                    valueType == typeof(Int16))
                {
                    item.SummaryType = SummaryType.Sum;
                    return;
                }

                if (valueType != typeof(String) &&
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
}