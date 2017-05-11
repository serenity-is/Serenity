using System;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;

namespace Serenity.Data
{
    public sealed class DateTimeField : GenericValueField<DateTime>
    {
        public DateTimeField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, DateTime?> getValue = null, Action<Row, DateTime?> setValue = null)
            : base(collection, FieldType.DateTime, name, caption, size, flags, getValue, setValue)
        {
        }

        public static DateTimeField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, DateTime?> getValue, Action<Row, DateTime?> setValue)
        {
            return new DateTimeField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override object ConvertValue(object source, IFormatProvider provider)
        {
#if COREFX
            if (source is JValue)
                source = ((JValue)source).Value;
#endif
            if (source == null)
                return null;
            else
            {
                if (source is DateTime)
                    return (DateTime)source;

                if (source is DateTimeOffset)
                    return ((DateTimeOffset)source).DateTime;

                return Convert.ChangeType(source, typeof(DateTime), provider);
            }
        }


#if !SILVERLIGHT
        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
            {
                var datetime = reader.GetDateTime(index);
                if (DateTimeKind != System.DateTimeKind.Unspecified)
                    datetime = DateTime.SpecifyKind(datetime, DateTimeKind);
                _setValue(row, datetime);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }
#endif

        public DateTimeKind DateTimeKind { get; set; }

        public static DateTime ToDateTimeKind(DateTime value, DateTimeKind dateTimeKind)
        {
            if (dateTimeKind == System.DateTimeKind.Unspecified)
                return value;

            if (dateTimeKind == System.DateTimeKind.Utc)
                return value.ToUniversalTime();
            else
                return value.ToLocalTime();
        }

        private DateTime ToDateTimeKind(DateTime value)
        {
            if (this.DateTimeKind == System.DateTimeKind.Unspecified)
                return value;

            if (this.DateTimeKind == System.DateTimeKind.Utc)
                return value.ToUniversalTime();
            else
                return value.ToLocalTime();
        }

        public new DateTime? this[Row row]
        {
            get
            {
                CheckUnassignedRead(row);
                return _getValue(row);
            }
            set
            {
                if (value != null)
                    _setValue(row, ToDateTimeKind(value.Value));
                else
                    _setValue(row, value);
                if (row.tracking)
                    row.FieldAssignedValue(this);
            }
        }

        public override object AsObject(Row row)
        {
            CheckUnassignedRead(row);
            return _getValue(row);
        }

        public override void AsObject(Row row, object value)
        {
            if (value == null)
                _setValue(row, null);
            else
                _setValue(row, ToDateTimeKind((DateTime)value));

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value.HasValue)
                writer.WriteValue(value.Value.ToString(
                    (DateTimeKind == System.DateTimeKind.Unspecified ||
                    DateTimeKind == System.DateTimeKind.Local) ? DateHelper.ISODateTimeFormatLocal : DateHelper.ISODateTimeFormatUTC, CultureInfo.InvariantCulture));
            else
                writer.WriteNull();
        }

        public override void ValueFromJson(JsonReader reader, Row row, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            switch (reader.TokenType)
            {
                case JsonToken.Null:
                case JsonToken.Undefined:
                    _setValue(row, null);
                    break;
                case JsonToken.Date:
                    var obj = reader.Value;
                    DateTime value;
                    if (obj is DateTime)
                        value = (DateTime)obj;
                    else if (obj is DateTimeOffset)
                        value = ((DateTimeOffset)obj).DateTime;
                    else
                        value = Convert.ToDateTime(obj, CultureInfo.InvariantCulture);
                    _setValue(row, ToDateTimeKind(value));
                    break;
                case JsonToken.String:
                    var s = ((string)reader.Value).TrimToNull();
                    if (s == null)
                        _setValue(row, null);
                    else
                        _setValue(row, ToDateTimeKind(Convert.ToDateTime(s, CultureInfo.InvariantCulture)));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }
    }
}