using System;

namespace Serenity.Data
{
    public delegate string ValueFormatter(object context, string column, object value, string format, IFormatProvider formatProvider);
}
