using System;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace Serenity.Data
{
    public sealed class SingleField : GenericValueField<Single>
    {
        public SingleField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, Single?> getValue = null, Action<Row, Single?> setValue = null)
            : base(collection, FieldType.Single, name, caption, size, flags, getValue, setValue)
        {
        }

        public static SingleField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, Single?> getValue, Action<Row, Single?> setValue)
        {
            return new SingleField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            var value = reader.GetValue(index);
            if (value is DBNull)
                _setValue(row, null);
            else if (value is Single)
                _setValue(row, (Single)value);
            else
                _setValue(row, Convert.ToSingle(value, CultureInfo.InvariantCulture));

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer)
        {
            writer.WriteValue(_getValue(row));
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
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Boolean:
                    _setValue(row, Convert.ToSingle(reader.Value, CultureInfo.InvariantCulture));
                    break;
                case JsonToken.String:
                    var s = ((string)reader.Value).TrimToNull();
                    if (s == null)
                        _setValue(row, null);
                    else
                        _setValue(row, Convert.ToSingle(s, CultureInfo.InvariantCulture));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }
    }
}
