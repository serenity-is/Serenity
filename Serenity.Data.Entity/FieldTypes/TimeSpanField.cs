using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Serenity.Data
{
    public sealed class TimeSpanField : GenericValueField<TimeSpan>
    {
        public TimeSpanField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, TimeSpan?> getValue = null, Action<Row, TimeSpan?> setValue = null)
            : base(collection, FieldType.Time, name, caption, size, flags, getValue, setValue)
        {
        }

        public static TimeSpanField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, TimeSpan?> getValue, Action<Row, TimeSpan?> setValue)
        {
            return new TimeSpanField(collection, name, caption, size, flags, getValue, setValue);
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
                var value = reader.GetValue(index);
                TimeSpan timeSpan;
                if (value is TimeSpan)
                    timeSpan = (TimeSpan)value;
                else if (value is DateTime)
                    timeSpan = ((DateTime)value).TimeOfDay;
                else
                    timeSpan = TimeSpan.Parse(value.ToString(), CultureInfo.InvariantCulture);

                _setValue(row, timeSpan);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }
#endif

        public new TimeSpan? this[Row row]
        {
            get
            {
                CheckUnassignedRead(row);
                return _getValue(row);
            }
            set
            {
                if (value != null)
                    _setValue(row, value.Value);
                else
                    _setValue(row, null);
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
                _setValue(row, (TimeSpan)value);

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value.HasValue)
                writer.WriteValue(value.Value.ToString("c", CultureInfo.InvariantCulture));
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
                    _setValue(row, Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture).TimeOfDay);
                    break;
                case JsonToken.String:
                    var s = ((string)reader.Value).TrimToNull();
                    if (s == null)
                        _setValue(row, null);
                    else
                        _setValue(row, TimeSpan.ParseExact(s, "c", CultureInfo.InvariantCulture));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }
    }
}