using System;
using System.ComponentModel;
using System.Globalization;

namespace Serenity.Data
{
    public class DataEnumConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) ||
                sourceType == typeof(Int32) ||
                sourceType == typeof(Int64) ||
                sourceType == typeof(Int16))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {           
            if (value is string || value is Int32 || value is Int64 || value is Int16)
                return DataEnum.ConvertFromString(typeof(T), value.ToString());
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
