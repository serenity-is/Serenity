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
            Func<IRow, TimeSpan?> getValue = null, Action<IRow, TimeSpan?> setValue = null)
            : base(collection, FieldType.Time, name, caption, size, flags, getValue, setValue)
        {
        }

        public static TimeSpanField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<IRow, TimeSpan?> getValue, Action<IRow, TimeSpan?> setValue)
        {
            return new TimeSpanField(collection, name, caption, size, flags, getValue, setValue);
        }

#if !SILVERLIGHT
        public override void GetFromReader(IDataReader reader, int index, IRow row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
            {
                var value = reader.GetValue(index);
                TimeSpan timeSpan;
                if (value is TimeSpan ts)
                    timeSpan = ts;
                else if (value is DateTime dt)
                    timeSpan = dt.TimeOfDay;
                else
                    timeSpan = TimeSpan.Parse(value.ToString(), CultureInfo.InvariantCulture);

                _setValue(row, timeSpan);
            }

            row.FieldAssignedValue(this);
        }
#endif

        public new TimeSpan? this[IRow row]
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
                row.FieldAssignedValue(this);
            }
        }

        public override object AsObject(IRow row)
        {
            CheckUnassignedRead(row);
            return _getValue(row);
        }

        public override void AsObject(IRow row,object value)
        {
            if (value == null)
                _setValue(row, null);
            else
                _setValue(row, (TimeSpan)value);

            row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, IRow row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value.HasValue)
                writer.WriteValue(value.Value.ToString("c", CultureInfo.InvariantCulture));
            else
                writer.WriteNull();
        }

        public override void ValueFromJson(JsonReader reader, IRow row, JsonSerializer serializer)
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
                    if (obj is DateTime dt)
                        value = dt;
                    else if (obj is DateTimeOffset dto)
                    {
                        _setValue(row, dto.TimeOfDay);
                        break;
                    }
                    else
                        value = Convert.ToDateTime(obj, CultureInfo.InvariantCulture);
                    _setValue(row, value.TimeOfDay);
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

            row.FieldAssignedValue(this);
        }
    }
}