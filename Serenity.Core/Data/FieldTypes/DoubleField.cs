using System;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace Serenity.Data
{
    public sealed class DoubleField : GenericValueField<Double>
    {
        public DoubleField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, Double?> getValue = null, Action<Row, Double?> setValue = null)
            : base(collection, FieldType.Double, name, caption, size, flags, getValue, setValue)
        {
        }

        public static DoubleField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, Double?> getValue, Action<Row, Double?> setValue)
        {
            return new DoubleField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
                _setValue(row, Convert.ToDouble(index, CultureInfo.InvariantCulture));

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
                    _setValue(row, Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture));
                    break;
                case JsonToken.String:
                    var s = ((string)reader.Value).TrimToNull();
                    if (s == null)
                        _setValue(row, null);
                    else
                        _setValue(row, Convert.ToDouble(s, CultureInfo.InvariantCulture));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }
    }
}
