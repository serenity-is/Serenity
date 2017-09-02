#if !COREFX
using System;
using System.ComponentModel;
using System.Globalization;

namespace Serenity.Data
{
    public static class TypeConversions
    {
        public static object ChangeType(object value, Type conversionType,
            CultureInfo culture = null)
        {
            if (conversionType == null)
                throw new ArgumentNullException("conversionType");

            if (conversionType is Type &&
                (value == null || value is Type))
                return value;

            if (conversionType.IsGenericType &&
                conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return Convert.ChangeType(value, conversionType, culture ?? CultureInfo.CurrentCulture);
        }
    }
}
#endif