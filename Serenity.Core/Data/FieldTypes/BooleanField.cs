using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using System.Globalization;

namespace Serenity.Data
{
    public sealed class BooleanField : GenericValueField<Boolean>
    {
        public BooleanField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, Boolean?> getValue = null, Action<Row, Boolean?> setValue = null)
            : base(collection, FieldType.Boolean, name, caption, size, flags, getValue, setValue)
        {
        }

        public static BooleanField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags, 
            Func<Row, Boolean?> getValue, Action<Row, Boolean?> setValue)
        {
            return new BooleanField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
                _setValue(row, reader.GetBoolean(index));

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public override void ValueToJson(Newtonsoft.Json.JsonWriter writer, Row row, JsonSerializer serializer)
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
                case JsonToken.Boolean:
                    _setValue(row, (Boolean)reader.Value);
                    break;
                case JsonToken.Integer:
                case JsonToken.Float:
                    _setValue(row, Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
                    break;
                case JsonToken.String:
                    var s = ((string)reader.Value).TrimToNull();
                    if (s == null)
                        _setValue(row, null);
                    else
                        _setValue(row, Convert.ToBoolean(s, CultureInfo.InvariantCulture));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }
    }
}
