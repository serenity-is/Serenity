#if !COREFX
using System;
using System.ComponentModel;
using System.Globalization;

namespace Serenity.Data
{
    /// <summary>
    /// Type conversion helper
    /// </summary>
    public static class TypeConversions
    {
        /// <summary>
        /// Changes the type using Convert.ChangeType but also handling NULL values properly.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="conversionType">Type of the conversion.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>Converted type</returns>
        /// <exception cref="ArgumentNullException">conversion type is null</exception>
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