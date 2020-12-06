using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Serenity.Data
{
    public sealed class DateTimeOffsetField : GenericValueField<DateTimeOffset>
    {
        public DateTimeOffsetField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
            Func<IRow, DateTimeOffset?> getValue = null, Action<IRow, DateTimeOffset?> setValue = null)
            : base(collection, FieldType.DateTime, name, caption, size, flags, getValue, setValue)
        {
        }

        public static DateTimeOffsetField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<IRow, DateTimeOffset?> getValue, Action<IRow, DateTimeOffset?> setValue)
        {
            return new DateTimeOffsetField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, IRow row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
            {
                DateTimeOffset dto;
                var value = reader.GetValue(index);
                if (value is DateTime dt)
                    dto = dt;
                else if (value is DateTimeOffset dtofs)
                    dto = dtofs;
                else
                    dto = DateTimeOffset.Parse(value.ToString());

                _setValue(row, dto);
            }

            row.FieldAssignedValue(this);
        }

        public new DateTimeOffset? this[IRow row]
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
                    _setValue(row, value);
                row.FieldAssignedValue(this);
            }
        }

        public override object AsObject(IRow row)
        {
            CheckUnassignedRead(row);
            return _getValue(row);
        }

        public override void AsObject(IRow row, object value)
        {
            if (value == null)
                _setValue(row, null);
            else
                _setValue(row, (DateTimeOffset)value);

            row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, IRow row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value.HasValue)
                writer.WriteValue(value.Value.ToString("o"));
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
                    DateTimeOffset value;
                    if (obj is DateTime dt)
                        value = dt;
                    else if (obj is DateTimeOffset dto)
                        value = dto;
                    else
                        value = DateTimeOffset.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture);
                    _setValue(row, value);
                    break;
                case JsonToken.String:
                    var s = ((string)reader.Value).TrimToNull();
                    if (s == null)
                        _setValue(row, null);
                    else
                        _setValue(row, DateTimeOffset.Parse(s, CultureInfo.InvariantCulture));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            row.FieldAssignedValue(this);
        }
    }
}